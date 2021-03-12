// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Uwp;
using UITests.App.Pages;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace UITests.App
{
    /// <summary>
    /// MainPage hosting all other test pages.
    /// </summary>
    public sealed partial class MainTestHost : IRecipient<RequestPageMessage>
    {
        private DispatcherQueue _queue;

        private Assembly _executingAssembly = Assembly.GetExecutingAssembly();

        private TaskCompletionSource<bool> _loadingStateTask;

        public MainTestHost()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Register<RequestPageMessage>(this);

            _queue = DispatcherQueue.GetForCurrentThread();
        }

        public void Receive(RequestPageMessage message)
        {
            // Reply with task back to so it can be properly awaited link:App.AppService.xaml.cs#L56
            message.Reply(OpenPage(message.PageName));
        }

        private async Task<bool> OpenPage(string pageName)
        {
            try
            {
                Log.Comment("Trying to Load Page: " + pageName);

                _loadingStateTask = new TaskCompletionSource<bool>();

                // Ensure we're on the UI thread as we'll be called from the AppService now.
                _ = _queue.EnqueueAsync(() =>
                {
                    // Navigate without extra animations
                    navigationFrame.Navigate(FindPageType(pageName), new SuppressNavigationTransitionInfo());
                });

                // Wait for load to complete
                await _loadingStateTask.Task;
            }
            catch (Exception e)
            {
                Log.Error("Exception Loading Page {0}: {1} ", pageName, e.Message);
                return false;
            }

            return true;
        }

        private Type FindPageType(string pageName)
        {
            try
            {
                return _executingAssembly.GetType("UITests.App.Pages." + pageName);
            }
            catch (Exception e)
            {
                Log.Error("Exception Finding Page {0}: {1} ", pageName, e.Message);
            }

            return null;
        }

        private void NavigationFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            Log.Comment("Navigated to Page {0}", e.SourcePageType.FullName);
            if (e.Content is Page page)
            {
                if (page.IsLoaded)
                {
                    Log.Comment("Loaded Page {0}", e.SourcePageType.FullName);
                    _loadingStateTask.SetResult(true);
                }
                else
                {
                    page.Loaded += this.Page_Loaded;
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var page = sender as Page;

            page.Loaded -= Page_Loaded;

            Log.Comment("Loaded Page (E) {0}", page.GetType().FullName);
            _loadingStateTask.SetResult(true);
        }

        private void NavigationFrame_NavigationFailed(object sender, Windows.UI.Xaml.Navigation.NavigationFailedEventArgs e)
        {
            Log.Error("Failed to navigate to page {0}", e.SourcePageType.FullName);
            _loadingStateTask.SetResult(false);
        }
    }
}
