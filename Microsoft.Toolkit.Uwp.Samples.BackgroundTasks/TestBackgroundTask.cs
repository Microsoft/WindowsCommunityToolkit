// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.Notifications;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace Microsoft.Toolkit.Uwp.Samples.BackgroundTasks
{
    public sealed class TestBackgroundTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Create content of the toast notification
            var toastContent = new ToastContent()
            {
                Scenario = ToastScenario.Default,
                Visual = new ToastVisual
                {
                    BindingGeneric = new ToastBindingGeneric
                    {
                        Children =
                        {
                            new AdaptiveText
                            {
                                Text = "New toast notification (BackgroundTaskHelper)."
                            }
                        }
                    }
                }
            };

            // Create & show toast notification
            var toastNotification = new ToastNotification(toastContent.GetXml());
            ToastNotificationManagerCompat.CreateToastNotifier().Show(toastNotification);
        }
    }
}