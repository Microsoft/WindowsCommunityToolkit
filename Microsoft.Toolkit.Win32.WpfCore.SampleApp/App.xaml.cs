// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using Microsoft.Toolkit.Uwp.Notifications;

namespace Microsoft.Toolkit.Win32.WpfCore.SampleApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Listen to toast notification activations
            ToastNotificationManagerCompat.OnActivated += this.ToastNotificationManagerCompat_OnActivated;

            // If launched from a toast
            if (ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
            {
                // Our OnActivated callback will run after this completes,
                // and will show a window if necessary.
            }
            else
            {
                // Show the window
                // In App.xaml, be sure to remove the StartupUri so that a window doesn't
                // get created by default, since we're creating windows ourselves (and sometimes we
                // don't want to create a window if handling a background activation).
                new MainWindow().Show();
            }
        }

        private void ToastNotificationManagerCompat_OnActivated(ToastNotificationActivatedEventArgsCompat e)
        {
            Dispatcher.Invoke(() =>
            {
                // If arguments are empty, that means the app title within Action Center was clicked.
                if (e.Argument.Length == 0)
                {
                    OpenWindowIfNeeded();
                    return;
                }

                // Parse the toast arguments
                ToastArguments args = ToastArguments.Parse(e.Argument);

                int conversationId = args.GetInt("conversationId");

                // If no specific action, view the conversation
                if (args.TryGetValue("action", out MyToastActions action))
                {
                    switch (action)
                    {
                        // View conversation
                        case MyToastActions.ViewConversation:

                            // Make sure we have a window open and in foreground
                            OpenWindowIfNeeded();

                            // And then show the conversation
                            (Current.Windows[0] as MainWindow).ShowConversation(conversationId);

                            break;

                        // Open the image
                        case MyToastActions.ViewImage:

                            // The URL retrieved from the toast args
                            string imageUrl = args["imageUrl"];

                            // Make sure we have a window open and in foreground
                            OpenWindowIfNeeded();

                            // And then show the image
                            (Current.Windows[0] as MainWindow).ShowImage(imageUrl);

                            break;

                        // Background: Quick reply to the conversation
                        case MyToastActions.Reply:

                            // Get the response the user typed
                            string msg = e.UserInput["tbReply"] as string;

                            // And send this message
                            ShowToast("Message sent: " + msg + "\nconversationId: " + conversationId);

                            // If there's no windows open, exit the app
                            if (Current.Windows.Count == 0)
                            {
                                Current.Shutdown();
                            }

                            break;

                        // Background: Send a like
                        case MyToastActions.Like:

                            ShowToast($"Like sent to conversation {conversationId}!");

                            // If there's no windows open, exit the app
                            if (Current.Windows.Count == 0)
                            {
                                Current.Shutdown();
                            }

                            break;

                        default:

                            OpenWindowIfNeeded();

                            break;
                    }
                }
            });
        }

        private void OpenWindowIfNeeded()
        {
            // Make sure we have a window open (in case user clicked toast while app closed)
            if (App.Current.Windows.Count == 0)
            {
                new MainWindow().Show();
            }

            // Activate the window, bringing it to focus
            App.Current.Windows[0].Activate();

            // And make sure to maximize the window too, in case it was currently minimized
            App.Current.Windows[0].WindowState = WindowState.Normal;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // If your app has an installer, you should call this when your app is uninstalled. Otherwise, if your app is a "portable app" and you no longer need notifications while the app is closed, you can call this upon exit.
            // ToastNotificationManagerCompat.Uninstall();
        }

        private void ShowToast(string msg)
        {
            // Construct the visuals of the toast and show it!
            new ToastContentBuilder()
                .AddText(msg)
                .Show();
        }
    }
}