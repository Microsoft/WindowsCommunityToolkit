// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Notifications;
using Windows.UI.Notifications;

namespace SmokeTest
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ToastContent content = GenerateToastContent();
            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(content.GetXml()));
        }

        public static ToastContent GenerateToastContent()
        {
            var builder = new ToastContentBuilder().SetToastScenario(ToastScenario.Reminder)
                .AddToastActivationInfo("action=viewEvent&eventId=1983", ToastActivationType.Foreground)
                .AddText("Adaptive Tiles Meeting")
                .AddText("Conf Room 2001 / Building 135")
                .AddText("10:00 AM - 10:30 AM")
                .AddComboBox(
                    "snoozeTime",
                    "15",
                    ("1", "1 minute"),
                    ("15", "15 minutes"),
                    ("60", "1 hour"),
                    ("240", "4 hours"),
                    ("1440", "1 day"))
                .AddButton(new ToastButtonSnooze() { SelectionBoxId = "snoozeTime" })
                .AddButton(new ToastButtonDismiss());

            return builder.Content;
        }
    }
}