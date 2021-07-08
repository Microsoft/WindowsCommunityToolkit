// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using UITests.App.Pages;
using Windows.ApplicationModel;

namespace UITests.App
{
    /// <summary>
    /// Application class for hosting UI pages to test.
    /// </summary>
    public sealed partial class App
    {
        public App()
        {
            this.InitializeComponent();

            // this.Suspending += OnSuspending;
            this.UnhandledException += this.App_UnhandledException;
        }

        [ThreadStatic]
        private static Window currentWindow = null;

        public static Window CurrentWindow
        {
            get
            {
                if (currentWindow == null)
                {
                    currentWindow = new Window
                    {
                        Title = "UITests.App"
                    };
                }

                return currentWindow;
            }
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // TODO: Also Log to a file?
            Log.Error($"Unhandled Exception: {e.Message}");
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            InitAppService();

            Action createRoot = () =>
            {
                var rootFrame = App.CurrentWindow.Content as Frame;

                // Do not repeat app initialization when the Window already has content,
                // just ensure that the window is active
                if (rootFrame == null)
                {
                    // Create a Frame to act as the navigation context and navigate to the first page
                    rootFrame = new Frame();

                    rootFrame.NavigationFailed += OnNavigationFailed;

                    App.CurrentWindow.Content = rootFrame;
                    rootFrame.Navigate(typeof(MainTestHost), e.Arguments);
                }
            };

            if (Environment.GetCommandLineArgs().Length <= 1)
            {
                createRoot();
            }
            else
            {
                var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
                if (dispatcherQueue == null)
                {
                    throw new Exception("DispatcherQueue not available");
                }

                System.Threading.Tasks.Task.Delay(2000).ContinueWith(
                    (t) =>
                    {
                        var ignored = dispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () => { createRoot(); });
                    });
            }

            // Ensure the current window is active
            App.CurrentWindow.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            Log.Error("Failed to load root page: " + e.SourcePageType.FullName);
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}