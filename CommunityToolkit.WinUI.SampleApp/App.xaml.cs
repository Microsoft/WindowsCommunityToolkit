// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Helpers;
using CommunityToolkit.WinUI.SampleApp.Common;
using CommunityToolkit.WinUI.SampleApp.Styles;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.System.Profile;
using Windows.UI.ViewManagement;
using WinRT;

namespace CommunityToolkit.WinUI.SampleApp
{
    public sealed partial class App : Application
    {
        private MainWindow _window;

        public IntPtr WindowHandle { get; private set; }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
        internal interface IWindowNative
        {
            IntPtr WindowHandle { get; }
        }

        [ComImport]
        [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IInitializeWithWindow
        {
            void Initialize(IntPtr hwnd);
        }

        public App()
        {
            InitializeComponent();

            // Suspending += OnSuspending;
        }

        /*
        protected override async void OnActivated(IActivatedEventArgs args)
        {
            RunAppInitialization(null);

            if (args.Kind == ActivationKind.Protocol)
            {
                try
                {
                    // Launching via protocol link
                    var parser = DeepLinkParser.Create(args);
                    var targetSample = await Sample.FindAsync(parser.Root, parser["sample"]);
                    if (targetSample != null)
                    {
                        Shell.Current?.NavigateToSample(targetSample);
                    }
                }
                catch (Exception ex)
                {
                    global::System.Diagnostics.Debug.WriteLine($"Error processing protocol launch: {ex.ToString()}");
                }
            }
        }
        */

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs e)
        {
            base.OnLaunched(e);

            IActivatedEventArgs activatedArgs = AppInstance.GetActivatedEventArgs();

            if (activatedArgs.PreviousExecutionState != ApplicationExecutionState.Running
                && activatedArgs.PreviousExecutionState != ApplicationExecutionState.Suspended)
            {
                RunAppInitialization(e?.Arguments);
            }

            SystemInformation.Instance.TrackAppUse(activatedArgs);
        }

        /*
        /// <summary>
        /// Event fired when a Background Task is activated (in Single Process Model)
        /// </summary>
        /// <param name="args">Arguments that describe the BackgroundTask activated</param>
        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);

            var deferral = args.TaskInstance.GetDeferral();

            switch (args.TaskInstance.Task.Name)
            {
                case Constants.TestBackgroundTaskName:
                    new TestBackgroundTask().Run(args.TaskInstance);
                    break;
            }

            deferral.Complete();
        }
        */

        private void RunAppInitialization(string launchParameters)
        {
            ThemeInjector.InjectThemeResources(Resources);

            // Go full screen on Xbox
            if (AnalyticsInfo.VersionInfo.GetDeviceFormFactor() == DeviceFormFactor.Xbox)
            {
                Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
            }

            // Initialize the constant for the app display name, used for tile and toast previews
            if (Constants.ApplicationDisplayName == null)
            {
                // Constants.ApplicationDisplayName = (await Package.Current.GetAppListEntriesAsync())[0].DisplayInfo.DisplayName;
            }

            // Check if the Cache is Latest, wipe if not.
            Sample.EnsureCacheLatest();

            _window = new MainWindow(launchParameters);

            IWindowNative windowWrapper = _window.As<IWindowNative>();
            WindowHandle = windowWrapper.WindowHandle;

            _window.Activate();
        }

        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            try
            {
                // Here we flush the Clipboard to make sure content in clipboard to remain available
                // after the application shuts down.
                Clipboard.Flush();
            }
            catch (Exception)
            {
                // ignore
            }

            try
            {
                await Task.Delay(2000);
            }
            catch
            {
                // ignore
            }
            finally
            {
                deferral.Complete();
            }
        }
    }
}