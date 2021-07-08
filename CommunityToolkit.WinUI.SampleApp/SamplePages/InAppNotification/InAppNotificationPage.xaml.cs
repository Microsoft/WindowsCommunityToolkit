// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows.Input;
using CommunityToolkit.WinUI.UI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.WinUI.SampleApp.SamplePages
{
    public sealed partial class InAppNotificationPage : Page, IXamlRenderListener
    {
        private InAppNotification _exampleInAppNotification;
        private InAppNotification _exampleCustomInAppNotification;
        private InAppNotification _exampleVSCodeInAppNotification;
        private ResourceDictionary _resources;

        public bool IsRootGridActualWidthLargerThan700 { get; set; }

        public int NotificationDuration { get; set; } = 0;

        public InAppNotificationPage()
        {
            InitializeComponent();
            Load();
        }

        public void OnXamlRendered(FrameworkElement control)
        {
            NotificationDuration = 0;

            _exampleInAppNotification = control.FindChild("ExampleInAppNotification") as InAppNotification;
            _exampleCustomInAppNotification = control.FindChild("ExampleCustomInAppNotification") as InAppNotification;
            _exampleVSCodeInAppNotification = control.FindChild("ExampleVSCodeInAppNotification") as InAppNotification;
            _resources = control.Resources;

            var notificationDurationTextBox = control.FindChild("NotificationDurationTextBox") as TextBox;
            if (notificationDurationTextBox != null)
            {
                notificationDurationTextBox.TextChanged += NotificationDurationTextBox_TextChanged;
            }
        }

        private void Load()
        {
            SampleController.Current.RegisterNewCommand("Show notification with random text", (sender, args) =>
            {
                _exampleVSCodeInAppNotification.Dismiss(true);
                _exampleCustomInAppNotification.Dismiss(true);
                _exampleInAppNotification?.Show(GetRandomText(), NotificationDuration);
            });

            SampleController.Current.RegisterNewCommand("Show notification with object", (sender, args) =>
            {
                _exampleVSCodeInAppNotification.Dismiss(true);
                _exampleCustomInAppNotification.Dismiss(true);

                var random = new Random();
                _exampleInAppNotification?.Show(new KeyValuePair<int, string>(random.Next(1, 10), GetRandomText()), NotificationDuration);
            });

            SampleController.Current.RegisterNewCommand("Show notification with buttons (without DataTemplate)", (sender, args) =>
            {
                _exampleVSCodeInAppNotification.Dismiss(true);
                _exampleCustomInAppNotification.Dismiss(true);

                var grid = new Grid()
                {
                    Margin = new Thickness(0, 0, -18, 0)
                };

                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // Text part
                var textBlock = new TextBlock
                {
                    Text = "Do you like it?",
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 24, 0),
                    FontSize = 14
                };
                grid.Children.Add(textBlock);

                // Buttons part
                var stackPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center
                };

                var yesButton = new Button
                {
                    Content = "Yes",
                    Width = 100,
                    Height = 32,
                    FontSize = 14
                };
                yesButton.Click += YesButton_Click;
                stackPanel.Children.Add(yesButton);

                var noButton = new Button
                {
                    Content = "No",
                    Width = 100,
                    Height = 32,
                    FontSize = 14,
                    Margin = new Thickness(4, 0, 0, 0)
                };
                noButton.Click += NoButton_Click;
                stackPanel.Children.Add(noButton);

                Grid.SetColumn(stackPanel, 1);
                grid.Children.Add(stackPanel);

                _exampleInAppNotification?.Show(grid, NotificationDuration);
            });

            SampleController.Current.RegisterNewCommand("Show notification with buttons (with DataTemplate)", (sender, args) =>
            {
                _exampleVSCodeInAppNotification.Dismiss(true);
                _exampleInAppNotification.Dismiss(true);

                object inAppNotificationWithButtonsTemplateResource = null;
                bool? isTemplatePresent = _resources?.TryGetValue("InAppNotificationWithButtonsTemplate", out inAppNotificationWithButtonsTemplateResource);
                if (isTemplatePresent == true && inAppNotificationWithButtonsTemplateResource is DataTemplate inAppNotificationWithButtonsTemplate)
                {
                    _exampleCustomInAppNotification.Show(inAppNotificationWithButtonsTemplate, NotificationDuration);
                }
            });

            SampleController.Current.RegisterNewCommand("Show notification with Visual Studio Code template (info notification)", (sender, args) =>
            {
                _exampleInAppNotification.Dismiss(true);
                _exampleCustomInAppNotification.Dismiss(true);
                _exampleVSCodeInAppNotification.Show(NotificationDuration);
            });

            SampleController.Current.RegisterNewCommand("Dismiss", (sender, args) =>
            {
                // Dismiss all notifications (should not be replicated in production)
                _exampleInAppNotification.Dismiss(true);
                _exampleCustomInAppNotification.Dismiss(true);
                _exampleVSCodeInAppNotification.Dismiss(true);
            });
        }

        private string GetRandomText()
        {
            var random = new Random();
            int result = random.Next(1, 4);

            switch (result)
            {
                case 1: return "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec sollicitudin bibendum enim at tincidunt. Praesent egestas ipsum ligula, nec tincidunt lacus semper non.";
                case 2: return "Pellentesque in risus eget leo rhoncus ultricies nec id ante.";
                case 3: default: return "Sed quis nisi quis nunc condimentum varius id consectetur metus. Duis mauris sapien, commodo eget erat ac, efficitur iaculis magna. Morbi eu velit nec massa pharetra cursus. Fusce non quam egestas leo finibus interdum eu ac massa. Quisque nec justo leo. Aenean scelerisque placerat ultrices. Sed accumsan lorem at arcu commodo tristique.";
            }
        }

        private void NotificationDurationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int newDuration;
            if (int.TryParse((sender as TextBox)?.Text, out newDuration))
            {
                NotificationDuration = newDuration;
            }
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            _exampleInAppNotification?.Dismiss();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            _exampleInAppNotification?.Dismiss();
        }
    }

#pragma warning disable SA1402 // File may only contain a single class
    internal class DismissCommand : ICommand
#pragma warning restore SA1402 // File may only contain a single class
    {
#pragma warning disable CS0067 // An event was declared but never used in the class in which it was declared.
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067 // An event was declared but never used in the class in which it was declared.

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            (parameter as InAppNotification)?.Dismiss();
        }
    }
}