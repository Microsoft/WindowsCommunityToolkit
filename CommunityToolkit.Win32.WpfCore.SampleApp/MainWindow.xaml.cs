// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CommunityToolkit.WinUI.Notifications;
using Windows.Services.Maps;
using Windows.UI.Notifications;

namespace CommunityToolkit.Win32.WpfCore.SampleApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // IMPORTANT: Look at App.xaml.cs for handling toast activation
        }

        private async void ButtonPopToast_Click(object sender, RoutedEventArgs e)
        {
            if (ToastNotificationManagerCompat.CreateToastNotifier().Setting != NotificationSetting.Enabled)
            {
                MessageBox.Show("Notifications are disabled from the system settings.");
                return;
            }

            string title = "Andrew sent you a picture";
            string content = "Check this out, The Enchantments!";
            string image = "https://picsum.photos/364/202?image=883";
            int conversationId = 5;

            // Construct the toast content and show it!
            new ToastContentBuilder()

                // Arguments that are returned when the user clicks the toast or a button
                .AddArgument("action", MyToastActions.ViewConversation)
                .AddArgument("conversationId", conversationId)

                // Visual content
                .AddText(title)
                .AddText(content)
                .AddInlineImage(new Uri(await DownloadImageToDisk(image)))
                .AddAppLogoOverride(new Uri(await DownloadImageToDisk("https://unsplash.it/64?image=1005")), ToastGenericAppLogoCrop.Circle)

                // Text box for typing a reply
                .AddInputTextBox("tbReply", "Type a reply")

                // Buttons
                .AddButton(new ToastButton()
                    .SetContent("Reply")
                    .AddArgument("action", MyToastActions.Reply)
                    .SetBackgroundActivation())

                .AddButton(new ToastButton()
                    .SetContent("Like")
                    .AddArgument("action", MyToastActions.Like)
                    .SetBackgroundActivation())

                .AddButton(new ToastButton()
                    .SetContent("View")
                    .AddArgument("action", MyToastActions.ViewImage)
                    .AddArgument("imageUrl", image))

                // And show the toast!
                .Show();
        }

        private static bool _hasPerformedCleanup;

        private static async Task<string> DownloadImageToDisk(string httpImage)
        {
            // Toasts can live for up to 3 days, so we cache images for up to 3 days.
            // Note that this is a very simple cache that doesn't account for space usage, so
            // this could easily consume a lot of space within the span of 3 days.
            try
            {
                if (ToastNotificationManagerCompat.CanUseHttpImages)
                {
                    return httpImage;
                }

                var directory = Directory.CreateDirectory(System.IO.Path.GetTempPath() + "WindowsNotifications.DesktopToasts.Images");

                if (!_hasPerformedCleanup)
                {
                    // First time we run, we'll perform cleanup of old images
                    _hasPerformedCleanup = true;

                    foreach (var d in directory.EnumerateDirectories())
                    {
                        if (d.CreationTimeUtc.Date < DateTime.UtcNow.Date.AddDays(-3))
                        {
                            d.Delete(true);
                        }
                    }
                }

                var dayDirectory = directory.CreateSubdirectory(DateTime.UtcNow.Day.ToString());
                string imagePath = dayDirectory.FullName + "\\" + (uint)httpImage.GetHashCode();

                if (File.Exists(imagePath))
                {
                    return imagePath;
                }

                HttpClient c = new HttpClient();
                using (var stream = await c.GetStreamAsync(httpImage))
                {
                    using (var fileStream = File.OpenWrite(imagePath))
                    {
                        stream.CopyTo(fileStream);
                    }
                }

                return imagePath;
            }
            catch
            {
                return string.Empty;
            }
        }

        internal void ShowConversation(int conversationId)
        {
            ContentBody.Content = new TextBlock()
            {
                Text = "You've just opened conversation " + conversationId,
                FontWeight = FontWeights.Bold
            };
        }

        internal void ShowImage(string imageUrl)
        {
            ContentBody.Content = new Image()
            {
                Source = new BitmapImage(new Uri(imageUrl))
            };
        }

        private void ButtonClearToasts_Click(object sender, RoutedEventArgs e)
        {
            ToastNotificationManagerCompat.History.Clear();
        }

        private async void ButtonScheduleToast_Click(object sender, RoutedEventArgs e)
        {
            // Schedule a toast to appear in 5 seconds
            new ToastContentBuilder()

                // Arguments that are returned when the user clicks the toast or a button
                .AddArgument("action", MyToastActions.ViewConversation)
                .AddArgument("conversationId", 7764)

                .AddText("Scheduled toast notification")

                .Schedule(DateTime.Now.AddSeconds(5));

            // Inform the user
            var tb = new TextBlock()
            {
                Text = "Toast scheduled to appear in 5 seconds",
                FontWeight = FontWeights.Bold
            };

            ContentBody.Content = tb;

            // And after 5 seconds, clear the informational message
            await Task.Delay(5000);

            if (ContentBody.Content == tb)
            {
                ContentBody.Content = null;
            }
        }

        private async void ButtonProgressToast_Click(object sender, RoutedEventArgs e)
        {
            const string tag = "progressToast";

            new ToastContentBuilder()
                .AddArgument("action", MyToastActions.ViewConversation)
                .AddArgument("conversationId", 423)
                .AddText("Sending image to conversation...")
                .AddVisualChild(new AdaptiveProgressBar()
                {
                    Value = new BindableProgressBarValue("progress"),
                    Status = "Sending..."
                })
                .Show(toast =>
                {
                    toast.Tag = tag;

                    toast.Data = new NotificationData(new Dictionary<string, string>()
                    {
                        { "progress", "0" }
                    });
                });

            double progress = 0;

            while (progress < 1)
            {
                await Task.Delay(new Random().Next(1000, 3000));

                progress += (new Random().NextDouble() * 0.15) + 0.1;

                ToastNotificationManagerCompat.CreateToastNotifier().Update(
                    new NotificationData(new Dictionary<string, string>()
                    {
                        { "progress", progress.ToString() }
                    }), tag);
            }

            new ToastContentBuilder()
                .AddArgument("action", MyToastActions.ViewConversation)
                .AddArgument("conversationId", 423)
                .AddText("Sent image to conversation!")
                .Show(toast =>
                {
                    toast.Tag = tag;
                });
        }
    }
}