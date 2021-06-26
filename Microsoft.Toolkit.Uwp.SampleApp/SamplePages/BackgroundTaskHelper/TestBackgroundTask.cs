// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.Notifications;
using Windows.ApplicationModel.Background;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    internal class TestBackgroundTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            new ToastContentBuilder()
                .AddText("New toast notification (BackgroundTaskHelper).")
                .Show();
        }
    }
}