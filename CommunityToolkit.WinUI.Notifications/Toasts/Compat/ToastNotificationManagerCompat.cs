// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINDOWS_UWP

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Windows.ApplicationModel;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// Provides access to sending and managing toast notifications. Works for all types of apps, even Win32 non-MSIX/sparse apps.
    /// </summary>
    public static class ToastNotificationManagerCompat
    {
#if WIN32
        private const string TOAST_ACTIVATED_LAUNCH_ARG = "-ToastActivated";
        private const string REG_HAS_SENT_NOTIFICATION = "HasSentNotification";
        private const string REG_HAS_7_0_1_FIX = "Has7.0.1Fix";
        internal const string DEFAULT_GROUP = "toolkitGroupNull";

        private const int CLASS_E_NOAGGREGATION = -2147221232;
        private const int E_NOINTERFACE = -2147467262;
        private const int CLSCTX_LOCAL_SERVER = 4;
        private const int REGCLS_MULTIPLEUSE = 1;
        private const int S_OK = 0;
        private static readonly Guid IUnknownGuid = new Guid("00000000-0000-0000-C000-000000000046");

        private static bool _registeredOnActivated;
        private static List<OnActivated> _onActivated = new List<OnActivated>();

        private static bool _hasSentNotification;

        /// <summary>
        /// Event that is triggered when a notification or notification button is clicked. Subscribe to this event in your app's initial startup code.
        /// </summary>
        public static event OnActivated OnActivated
        {
            add
            {
                lock (_onActivated)
                {
                    if (!_registeredOnActivated)
                    {
                        // Desktop Bridge apps will dynamically register upon first subscription to event
                        try
                        {
                            CreateAndRegisterActivator();
                        }
                        catch (Exception ex)
                        {
                            _initializeEx = new InvalidOperationException("Failed to register notification activator", ex);
                        }
                    }

                    _onActivated.Add(value);
                }
            }

            remove
            {
                lock (_onActivated)
                {
                    _onActivated.Remove(value);
                }
            }
        }

        internal static void OnActivatedInternal(string args, Internal.InternalNotificationActivator.NOTIFICATION_USER_INPUT_DATA[] input, string aumid)
        {
            ValueSet userInput = new ValueSet();

            if (input != null)
            {
                foreach (var val in input)
                {
                    userInput.Add(val.Key, val.Value);
                }
            }

            var e = new ToastNotificationActivatedEventArgsCompat()
            {
                Argument = args,
                UserInput = userInput
            };

            OnActivated[] listeners;
            lock (_onActivated)
            {
                listeners = _onActivated.ToArray();
            }

            foreach (var listener in listeners)
            {
                listener(e);
            }
        }

        private static string _win32Aumid;
        private static string _clsid;
        private static InvalidOperationException _initializeEx;

        static ToastNotificationManagerCompat()
        {
            try
            {
                Initialize();
            }
            catch (Exception ex)
            {
                // We catch the exception so that things like subscribing to the event handler doesn't crash app
                _initializeEx = new InvalidOperationException("Failed initializing notifications", ex);
            }
        }

        private static void Initialize()
        {
            // If containerized
            if (DesktopBridgeHelpers.IsContainerized())
            {
                // No need to do anything additional, already registered through manifest
                return;
            }

            Win32AppInfo win32AppInfo = null;

            // If sparse
            if (DesktopBridgeHelpers.HasIdentity())
            {
                _win32Aumid = new ManifestHelper().GetAumidFromPackageManifest();
            }
            else
            {
                win32AppInfo = Win32AppInfo.Get();
                _win32Aumid = win32AppInfo.Aumid;
            }

            // Create and register activator
            var activatorType = CreateAndRegisterActivator();

            // Register via registry
            using (var rootKey = Registry.CurrentUser.CreateSubKey(GetRegistrySubKey()))
            {
                // If they don't have identity, we need to specify the display assets
                if (!DesktopBridgeHelpers.HasIdentity())
                {
                    // Set the display name and icon uri
                    rootKey.SetValue("DisplayName", win32AppInfo.DisplayName);

                    if (win32AppInfo.IconPath != null)
                    {
                        rootKey.SetValue("IconUri", win32AppInfo.IconPath);
                    }
                    else
                    {
                        if (rootKey.GetValue("IconUri") != null)
                        {
                            rootKey.DeleteValue("IconUri");
                        }
                    }

                    // Background color only appears in the settings page, format is
                    // hex without leading #, like "FFDDDDDD"
                    rootKey.SetValue("IconBackgroundColor", "FFDDDDDD");

                    // Additionally, we need to read whether they've sent a notification before
                    _hasSentNotification = rootKey.GetValue(REG_HAS_SENT_NOTIFICATION) != null;

                    // And read if we've already applied the 7_0_1 fix
                    bool has7_0_1fix = rootKey.GetValue(REG_HAS_7_0_1_FIX) != null;

                    // If it doesn't have the fix yet
                    if (!has7_0_1fix)
                    {
                        // If the AUMID changed
                        if (win32AppInfo.Pre7_0_1Aumid != null)
                        {
                            // Uninstall the old AUMID
                            CleanUpOldAumid(win32AppInfo.Pre7_0_1Aumid);
                        }

                        // Set that it has the fix so we don't try uninstalling again in the future
                        rootKey.SetValue(REG_HAS_7_0_1_FIX, 1);
                    }
                }

                rootKey.SetValue("CustomActivator", string.Format("{{{0}}}", activatorType.GUID));
            }
        }

        private static string GetRegistrySubKey()
        {
            return GetRegistrySubKey(_win32Aumid);
        }

        private static string GetRegistrySubKey(string win32Aumid)
        {
            return @"Software\Classes\AppUserModelId\" + win32Aumid;
        }

        private static Type CreateActivatorType()
        {
            // https://stackoverflow.com/questions/24069352/c-sharp-typebuilder-generate-class-with-function-dynamically
            // For .NET Core we use https://stackoverflow.com/questions/36937276/is-there-any-replace-of-assemblybuilder-definedynamicassembly-in-net-core
            AssemblyName aName = new AssemblyName("DynamicComActivator");
            AssemblyBuilder aBuilder = AssemblyBuilder.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);

            // For a single-module assembly, the module name is usually the assembly name plus an extension.
            ModuleBuilder mb = aBuilder.DefineDynamicModule(aName.Name);

            // Create class which extends NotificationActivator
            TypeBuilder tb = mb.DefineType(
                name: "MyNotificationActivator",
                attr: TypeAttributes.Public,
                parent: typeof(Internal.InternalNotificationActivator),
                interfaces: new Type[0]);

            if (DesktopBridgeHelpers.IsContainerized())
            {
                _clsid = new ManifestHelper().GetClsidFromPackageManifest();
            }
            else
            {
                _clsid = Win32AppInfo.GenerateGuid(_win32Aumid);
            }

            tb.SetCustomAttribute(new CustomAttributeBuilder(
                con: typeof(GuidAttribute).GetConstructor(new Type[] { typeof(string) }),
                constructorArgs: new object[] { _clsid }));

            tb.SetCustomAttribute(new CustomAttributeBuilder(
                con: typeof(ComVisibleAttribute).GetConstructor(new Type[] { typeof(bool) }),
                constructorArgs: new object[] { true }));

            tb.SetCustomAttribute(new CustomAttributeBuilder(
#pragma warning disable CS0618 // Type or member is obsolete
                con: typeof(ComSourceInterfacesAttribute).GetConstructor(new Type[] { typeof(Type) }),
#pragma warning restore CS0618 // Type or member is obsolete
                constructorArgs: new object[] { typeof(Internal.InternalNotificationActivator.INotificationActivationCallback) }));

            tb.SetCustomAttribute(new CustomAttributeBuilder(
                con: typeof(ClassInterfaceAttribute).GetConstructor(new Type[] { typeof(ClassInterfaceType) }),
                constructorArgs: new object[] { ClassInterfaceType.None }));

            return tb.CreateType();
        }

        private static Type CreateAndRegisterActivator()
        {
            var activatorType = CreateActivatorType();
            RegisterActivator(activatorType);
            _registeredOnActivated = true;
            return activatorType;
        }

        private static void RegisterActivator(Type activatorType)
        {
            if (!DesktopBridgeHelpers.IsContainerized())
            {
                string exePath = Process.GetCurrentProcess().MainModule.FileName;
                RegisterComServer(activatorType, exePath);
            }

            // Big thanks to FrecherxDachs for figuring out the following code which works in .NET Core 3: https://github.com/FrecherxDachs/UwpNotificationNetCoreTest
            var uuid = activatorType.GUID;
            NativeMethods.CoRegisterClassObject(
                uuid,
                new NotificationActivatorClassFactory(activatorType),
                CLSCTX_LOCAL_SERVER,
                REGCLS_MULTIPLEUSE,
                out _);
        }

        private static void RegisterComServer(Type activatorType, string exePath)
        {
            // We register the EXE to start up when the notification is activated
            string regString = string.Format("SOFTWARE\\Classes\\CLSID\\{{{0}}}", activatorType.GUID);
            using (var key = Registry.CurrentUser.CreateSubKey(regString + "\\LocalServer32"))
            {
                // Include a flag so we know this was a toast activation and should wait for COM to process
                // We also wrap EXE path in quotes for extra security
                key.SetValue(null, '"' + exePath + '"' + " " + TOAST_ACTIVATED_LAUNCH_ARG);
            }

            if (IsElevated)
            {
                //// For elevated apps, we need to ensure they'll activate in existing running process by adding
                //// some values in local machine
                using (var key = Registry.LocalMachine.CreateSubKey(regString))
                {
                    // Same as above, except also including AppId to link to our AppId entry below
                    using (var localServer32 = key.CreateSubKey("LocalServer32"))
                    {
                        localServer32.SetValue(null, '"' + exePath + '"' + " " + TOAST_ACTIVATED_LAUNCH_ARG);
                    }

                    key.SetValue("AppId", "{" + activatorType.GUID + "}");
                }

                // This tells COM to match any client, so Action Center will activate our elevated process.
                // More info: https://docs.microsoft.com/windows/win32/com/runas
                using (var key = Registry.LocalMachine.CreateSubKey(string.Format("SOFTWARE\\Classes\\AppID\\{{{0}}}", activatorType.GUID)))
                {
                    key.SetValue("RunAs", "Interactive User");
                }
            }
        }

        /// <summary>
        /// Gets whether the current process was activated due to a toast activation. If so, the OnActivated event will be triggered soon after process launch.
        /// </summary>
        /// <returns>True if the current process was activated due to a toast activation, otherwise false.</returns>
        public static bool WasCurrentProcessToastActivated()
        {
            return Environment.GetCommandLineArgs().Contains(TOAST_ACTIVATED_LAUNCH_ARG);
        }

        [ComImport]
        [Guid("00000001-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IClassFactory
        {
            [PreserveSig]
            int CreateInstance(IntPtr pUnkOuter, ref Guid riid, out IntPtr ppvObject);

            [PreserveSig]
            int LockServer(bool fLock);
        }

        private class NotificationActivatorClassFactory : IClassFactory
        {
            private Type _activatorType;

            public NotificationActivatorClassFactory(Type activatorType)
            {
                _activatorType = activatorType;
            }

            public int CreateInstance(IntPtr pUnkOuter, ref Guid riid, out IntPtr ppvObject)
            {
                ppvObject = IntPtr.Zero;

                if (pUnkOuter != IntPtr.Zero)
                {
                    Marshal.ThrowExceptionForHR(CLASS_E_NOAGGREGATION);
                }

                if (riid == _activatorType.GUID || riid == IUnknownGuid)
                {
                    // Create the instance of the .NET object
                    ppvObject = Marshal.GetComInterfaceForObject(
                        Activator.CreateInstance(_activatorType),
                        typeof(Internal.InternalNotificationActivator.INotificationActivationCallback));
                }
                else
                {
                    // The object that ppvObject points to does not support the
                    // interface identified by riid.
                    Marshal.ThrowExceptionForHR(E_NOINTERFACE);
                }

                return S_OK;
            }

            public int LockServer(bool fLock)
            {
                return S_OK;
            }
        }

        private static bool IsElevated
        {
            get
            {
                return new System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent()).IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
        }
#endif

        /// <summary>
        /// Creates a toast notifier.
        /// </summary>
        /// <returns><see cref="ToastNotifierCompat"/>An instance of the toast notifier.</returns>
        public static ToastNotifierCompat CreateToastNotifier()
        {
#if WIN32
            if (_initializeEx != null)
            {
                throw _initializeEx;
            }

            if (DesktopBridgeHelpers.HasIdentity())
            {
                return new ToastNotifierCompat(ToastNotificationManager.CreateToastNotifier());
            }
            else
            {
                return new ToastNotifierCompat(ToastNotificationManager.CreateToastNotifier(_win32Aumid));
            }
#else
            return new ToastNotifierCompat(ToastNotificationManager.CreateToastNotifier());
#endif
        }

        /// <summary>
        /// Gets the <see cref="ToastNotificationHistoryCompat"/> object.
        /// </summary>
        public static ToastNotificationHistoryCompat History
        {
            get
            {
#if WIN32
                if (_initializeEx != null)
                {
                    throw _initializeEx;
                }

                return new ToastNotificationHistoryCompat(DesktopBridgeHelpers.HasIdentity() ? null : _win32Aumid);
#else
                return new ToastNotificationHistoryCompat(null);
#endif
            }
        }

        /// <summary>
        /// Gets a value indicating whether http images can be used within toasts. This is true if running with package identity (UWP, MSIX, or sparse package).
        /// </summary>
        public static bool CanUseHttpImages
        {
            get
            {
#if WIN32
                return DesktopBridgeHelpers.HasIdentity();
#else
                return true;
#endif
            }
        }

#if WIN32
        /// <summary>
        /// If you're not using MSIX, call this when your app is being uninstalled to properly clean up all notifications and notification-related resources. Note that this must be called from your app's main EXE (the one that you used notifications for) and not a separate uninstall EXE. If called from a MSIX app, this method no-ops.
        /// </summary>
        public static void Uninstall()
        {
            if (DesktopBridgeHelpers.IsContainerized())
            {
                // Packaged containerized apps automatically clean everything up already
                return;
            }

            if (!DesktopBridgeHelpers.HasIdentity())
            {
                try
                {
                    // Remove all scheduled notifications (do this first before clearing current notifications)
                    var notifier = CreateToastNotifier();
                    foreach (var scheduled in notifier.GetScheduledToastNotifications())
                    {
                        try
                        {
                            notifier.RemoveFromSchedule(scheduled);
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }

                try
                {
                    // Clear all current notifications
                    History.Clear();
                }
                catch
                {
                }
            }

            try
            {
                // Remove registry key
                if (_win32Aumid != null)
                {
                    Registry.CurrentUser.DeleteSubKey(GetRegistrySubKey());
                }
            }
            catch
            {
            }

            try
            {
                if (_clsid != null)
                {
                    try
                    {
                        Registry.CurrentUser.DeleteSubKeyTree(string.Format("SOFTWARE\\Classes\\CLSID\\{{{0}}}", _clsid));
                    }
                    catch
                    {
                    }

                    if (IsElevated)
                    {
                        try
                        {
                            Registry.LocalMachine.DeleteSubKeyTree(string.Format("SOFTWARE\\Classes\\CLSID\\{{{0}}}", _clsid));
                        }
                        catch
                        {
                        }

                        try
                        {
                            Registry.LocalMachine.DeleteSubKeyTree(string.Format("SOFTWARE\\Classes\\AppID\\{{{0}}}", _clsid));
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch
            {
            }

            try
            {
                // Delete any of the app files
                var appDataFolderPath = Win32AppInfo.GetAppDataFolderPath(_win32Aumid);
                if (Directory.Exists(appDataFolderPath))
                {
                    Directory.Delete(appDataFolderPath, recursive: true);
                }
            }
            catch
            {
            }
        }

        private static void CleanUpOldAumid(string oldAumid)
        {
            try
            {
                // Remove all scheduled notifications (do this first before clearing current notifications)
                var notifier = ToastNotificationManager.CreateToastNotifier(oldAumid);
                foreach (var scheduled in notifier.GetScheduledToastNotifications())
                {
                    try
                    {
                        notifier.RemoveFromSchedule(scheduled);
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }

            try
            {
                // Clear all current notifications
                ToastNotificationManager.History.Clear(oldAumid);
            }
            catch
            {
            }

            try
            {
                // Remove registry key
                Registry.CurrentUser.DeleteSubKey(GetRegistrySubKey(oldAumid));
            }
            catch
            {
            }

            try
            {
                // Delete any of the app files
                var appDataFolderPath = Win32AppInfo.GetAppDataFolderPath(oldAumid);
                if (Directory.Exists(appDataFolderPath))
                {
                    Directory.Delete(appDataFolderPath, recursive: true);
                }
            }
            catch
            {
            }
        }
#endif

#if WIN32
        internal static void SetHasSentToastNotification()
        {
            // For plain Win32 apps, record that we've sent a notification
            if (!_hasSentNotification && !DesktopBridgeHelpers.HasIdentity())
            {
                _hasSentNotification = true;

                try
                {
                    using (var rootKey = Registry.CurrentUser.CreateSubKey(GetRegistrySubKey()))
                    {
                        rootKey.SetValue(REG_HAS_SENT_NOTIFICATION, 1);
                    }
                }
                catch
                {
                }
            }
        }

        internal static void PreRegisterIdentityLessApp()
        {
            // For plain Win32 apps, we first have to have send a toast notification once before using scheduled toasts.
            if (!_hasSentNotification && !DesktopBridgeHelpers.HasIdentity())
            {
                const string tag = "toolkit1stNotif";

                // Show the toast
                new ToastContentBuilder()
                    .AddText("New notification")
                    .Show(toast =>
                    {
                        // We'll hide the popup and set the toast to expire in case removing doesn't work
                        toast.SuppressPopup = true;
                        toast.Tag = tag;
                        toast.ExpirationTime = DateTime.Now.AddSeconds(15);
                    });

                // And then remove it
                ToastNotificationManagerCompat.History.Remove(tag);
            }
        }
#endif
    }
}

#endif