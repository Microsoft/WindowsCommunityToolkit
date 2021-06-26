// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Toolkit.Uwp.SampleApp.Common;
using Microsoft.Toolkit.Uwp.SampleApp.Models;
using NotificationsVisualizerLibrary;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    public sealed partial class ToastPage : Page
    {
        private ToastContent _toastContent;

        public ToastPage()
        {
            InitializeComponent();
            Initialize();
        }

#pragma warning disable SA1008 // Parenthesis spacing
#pragma warning disable SA1117 // Parameters must be on same line or separate lines

        public static ToastContent GenerateToastContent()
        {
            var builder = new ToastContentBuilder()
                .SetToastScenario(ToastScenario.Reminder)
                .AddArgument("action", "viewEvent")
                .AddArgument("eventId", 1983)
                .AddText("Adaptive Tiles Meeting")
                .AddText("Conf Room 2001 / Building 135")
                .AddText("10:00 AM - 10:30 AM")
                .AddComboBox("snoozeTime", "15", ("1", "1 minute"),
                                                 ("15", "15 minutes"),
                                                 ("60", "1 hour"),
                                                 ("240", "4 hours"),
                                                 ("1440", "1 day"))
                .AddButton(new ToastButton()
                    .SetSnoozeActivation("snoozeTime"))
                .AddButton(new ToastButton()
                    .SetDismissActivation());

            return builder.Content;
        }

#pragma warning restore SA1008
#pragma warning restore SA1117

        private void ButtonPopToast_Click(object sender, RoutedEventArgs e)
        {
            PopToast();
        }

        private void PopToast()
        {
            ToastNotificationManagerCompat.CreateToastNotifier().Show(new ToastNotification(_toastContent.GetXml()));
        }

        private void Initialize()
        {
            // Generate the toast notification content
            _toastContent = GenerateToastContent();

            // Prepare and update preview toast
            PreviewToastReminder.Properties = new PreviewToastProperties()
            {
                BackgroundColor = Constants.ApplicationBackgroundColor,
                DisplayName = Constants.ApplicationDisplayName,
                Square44x44Logo = Constants.Square44x44Logo
            };
            PreviewToastReminder.Initialize(_toastContent.GetXml());
        }
    }
}