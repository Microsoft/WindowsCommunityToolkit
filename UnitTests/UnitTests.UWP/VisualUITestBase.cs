// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace UnitTests
{
    /// <summary>
    /// Base class to be used in API tests which require UI layout or rendering to occur first.
    /// For more E2E scenarios or testing components for user interation, see integration test suite instead.
    /// Use this class when an API needs direct access to test functions of the UI itself in more simplistic scenarios (i.e. visual tree helpers).
    /// </summary>
    public class VisualUITestBase
    {
        /// <summary>
        /// Sets the content of the test app to a simple <see cref="FrameworkElement"/> to load into the visual tree.
        /// Waits for that element to be loaded and rendered before returning.
        /// </summary>
        /// <param name="content">Content to set in test app.</param>
        /// <returns>When UI is loaded.</returns>
        protected Task SetTestContentAsync(FrameworkElement content)
        {
            return App.DispatcherQueue.EnqueueAsync(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<bool>();

                async void Callback(object sender, RoutedEventArgs args)
                {
                    content.Loaded -= Callback;

                    // Wait for first Render pass
                    await CompositionTargetHelper.ExecuteAfterCompositionRenderingAsync(() => { });

                    taskCompletionSource.SetResult(true);
                }

                // Going to wait for our original content to unload
                content.Loaded += Callback;

                // Trigger that now
                try
                {
                    App.ContentRoot = content;
                }
                catch (Exception e)
                {
                    taskCompletionSource.SetException(e);
                }

                return taskCompletionSource.Task;
            });
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            await App.DispatcherQueue.EnqueueAsync(() =>
            {
                // If we didn't set our content we don't have to do anything but complete here.
                if (App.ContentRoot is null)
                {
                    taskCompletionSource.SetResult(true);
                    return;
                }

                // Going to wait for our original content to unload
                App.ContentRoot.Unloaded += (_, _) => taskCompletionSource.SetResult(true);

                // Trigger that now
                App.ContentRoot = null;
            });

            await taskCompletionSource.Task;
        }
    }
}
