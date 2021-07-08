// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WIN32

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Windows.UI.Notifications;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// Helper for .NET Framework applications to display toast notifications and respond to toast events
    /// </summary>
    [Obsolete("We recommend switching to the new ToastNotificationManagerCompat. For Win32 apps, it no longer requires a Start menu shortcut, and activation now uses a straightforward event handler (no NotificationActivator class and COM GUIDs needed)!")]
    public class DesktopNotificationManagerCompat
    {
        /// <summary>
        /// A constant that is used as the launch arg when your EXE is launched from a toast notification.
        /// </summary>
        public const string ToastActivatedLaunchArg = "-ToastActivated";

        private const int CLASS_E_NOAGGREGATION = -2147221232;
        private const int E_NOINTERFACE = -2147467262;
        private const int CLSCTX_LOCAL_SERVER = 4;
        private const int REGCLS_MULTIPLEUSE = 1;
        private const int S_OK = 0;
        private static readonly Guid IUnknownGuid = new Guid("00000000-0000-0000-C000-000000000046");

        private static bool _registeredAumidAndComServer;
        private static string _aumid;
        private static bool _registeredActivator;

        /// <summary>
        /// If you're not using MSIX or sparse packages, you must call this method to register your AUMID with the Compat library and to
        /// register your COM CLSID and EXE in LocalServer32 registry. Feel free to call this regardless, and we will no-op if running
        /// under Desktop Bridge. Call this upon application startup, before calling any other APIs.
        /// </summary>
        /// <typeparam name="T">Your implementation of NotificationActivator. Must have GUID and ComVisible attributes on class.</typeparam>
        /// <param name="aumid">An AUMID that uniquely identifies your application.</param>
        public static void RegisterAumidAndComServer<T>(string aumid)
            where T : NotificationActivator
        {
            if (string.IsNullOrWhiteSpace(aumid))
            {
                throw new ArgumentException("You must provide an AUMID.", nameof(aumid));
            }

            // If running as Desktop Bridge
            if (DesktopBridgeHelpers.HasIdentity())
            {
                // Clear the AUMID since Desktop Bridge doesn't use it, and then we're done.
                // Desktop Bridge apps are registered with platform through their manifest.
                // Their LocalServer32 key is also registered through their manifest.
                _aumid = null;
                _registeredAumidAndComServer = true;
                return;
            }

            _aumid = aumid;

            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            RegisterComServer<T>(exePath);

            _registeredAumidAndComServer = true;
        }

        private static void RegisterComServer<T>(string exePath)
            where T : NotificationActivator
        {
            // We register the EXE to start up when the notification is activated
            string regString = string.Format("SOFTWARE\\Classes\\CLSID\\{{{0}}}\\LocalServer32", typeof(T).GUID);
            var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regString);

            // Include a flag so we know this was a toast activation and should wait for COM to process
            // We also wrap EXE path in quotes for extra security
            key.SetValue(null, '"' + exePath + '"' + " " + ToastActivatedLaunchArg);
        }

        /*
        * RegisterActivator code and all internal dependencies is from FrecherxDachs.
        * See entry #2 in ThirdPartyNotices.txt in root repository directory for full license. */

        /// <summary>
        /// Registers the activator type as a COM server client so that Windows can launch your activator.
        /// </summary>
        /// <typeparam name="T">Your implementation of NotificationActivator. Must have GUID and ComVisible attributes on class.</typeparam>
        public static void RegisterActivator<T>()
            where T : NotificationActivator, new()
        {
            // Big thanks to FrecherxDachs for figuring out the following code which works in .NET Core 3: https://github.com/FrecherxDachs/UwpNotificationNetCoreTest
            var uuid = typeof(T).GUID;
            uint cookie;
            NativeMethods.CoRegisterClassObject(
                uuid,
                new NotificationActivatorClassFactory<T>(),
                CLSCTX_LOCAL_SERVER,
                REGCLS_MULTIPLEUSE,
                out cookie);

            _registeredActivator = true;
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

        private class NotificationActivatorClassFactory<T> : IClassFactory
            where T : NotificationActivator, new()
        {
            public int CreateInstance(IntPtr pUnkOuter, ref Guid riid, out IntPtr ppvObject)
            {
                ppvObject = IntPtr.Zero;

                if (pUnkOuter != IntPtr.Zero)
                {
                    Marshal.ThrowExceptionForHR(CLASS_E_NOAGGREGATION);
                }

                if (riid == typeof(T).GUID || riid == IUnknownGuid)
                {
                    // Create the instance of the .NET object
                    ppvObject = Marshal.GetComInterfaceForObject(
                        new T(),
                        typeof(NotificationActivator.INotificationActivationCallback));
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

        /// <summary>
        /// Creates a toast notifier. You must have called <see cref="RegisterActivator{T}"/> first (and also <see cref="RegisterAumidAndComServer(string)"/> if you're a classic Win32 app), or this will throw an exception.
        /// </summary>
        /// <returns><see cref="ToastNotifier"/></returns>
        public static ToastNotifier CreateToastNotifier()
        {
            EnsureRegistered();

            if (_aumid != null)
            {
                // Non-Desktop Bridge
                return ToastNotificationManager.CreateToastNotifier(_aumid);
            }
            else
            {
                // Desktop Bridge
                return ToastNotificationManager.CreateToastNotifier();
            }
        }

        /// <summary>
        /// Gets the <see cref="DesktopNotificationHistoryCompat"/> object. You must have called <see cref="RegisterActivator{T}"/> first (and also <see cref="RegisterAumidAndComServer(string)"/> if you're a classic Win32 app), or this will throw an exception.
        /// </summary>
        public static DesktopNotificationHistoryCompat History
        {
            get
            {
                EnsureRegistered();

                return new DesktopNotificationHistoryCompat(_aumid);
            }
        }

        private static void EnsureRegistered()
        {
            // If not registered AUMID yet
            if (!_registeredAumidAndComServer)
            {
                // Check if Desktop Bridge
                if (DesktopBridgeHelpers.HasIdentity())
                {
                    // Implicitly registered, all good!
                    _registeredAumidAndComServer = true;
                }
                else
                {
                    // Otherwise, incorrect usage
                    throw new Exception("You must call RegisterAumidAndComServer first.");
                }
            }

            // If not registered activator yet
            if (!_registeredActivator)
            {
                // Incorrect usage
                throw new Exception("You must call RegisterActivator first.");
            }
        }

        /// <summary>
        /// Gets a value indicating whether http images can be used within toasts. This is true if running with package identity (MSIX or sparse package).
        /// </summary>
        public static bool CanUseHttpImages
        {
            get { return DesktopBridgeHelpers.HasIdentity(); }
        }
    }
}

#endif