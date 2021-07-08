// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.WinUI.Input.GazeInteraction;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GazeInputTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            ShowCursor.IsChecked = GazeInput.GetIsCursorVisible(this);

            GazeInput.IsDeviceAvailableChanged += GazeInput_IsDeviceAvailableChanged;
            GazeInput_IsDeviceAvailableChanged(null, null);
        }

        private void GazeInput_IsDeviceAvailableChanged(object sender, object e)
        {
            DeviceAvailable.Text = GazeInput.IsDeviceAvailable ? "Eye tracker device available" : "No eye tracker device available";
        }

        private void OnStateChanged(object sender, StateChangedEventArgs ea)
        {
            Dwell.Content = ea.PointerState.ToString();
        }

        private void Dwell_Click(object sender, RoutedEventArgs e)
        {
            Dwell.Content = "Clicked";
        }

        private void ShowCursor_Toggle(object sender, RoutedEventArgs e)
        {
            if (ShowCursor.IsChecked.HasValue)
            {
                GazeInput.SetIsCursorVisible(this, ShowCursor.IsChecked.Value);
            }
        }

        private int _clickCount;

        private void OnLegacyInvoked(object sender, RoutedEventArgs e)
        {
            _clickCount++;
            HowButton.Content = string.Format("{0}: Legacy click", _clickCount);
        }

        private void OnGazeInvoked(object sender, DwellInvokedRoutedEventArgs e)
        {
            _clickCount++;
            HowButton.Content = string.Format("{0}: Accessible click", _clickCount);
            e.Handled = true;
        }

        private void OnInvokeProgress(object sender, DwellProgressEventArgs e)
        {
            if (e.State == DwellProgressState.Progressing)
            {
                ProgressShow.Value = 100.0 * e.Progress;
            }

            ProgressShow.IsIndeterminate = e.State == DwellProgressState.Complete;
            e.Handled = true;
        }

        private void SpawnClicked(object sender, RoutedEventArgs e)
        {
            var m_window = new MainWindow();
            m_window.Activate();
        }

        private async void DialogClicked(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Sample Dialog",
                Content = "This is an example content dialog",
                CloseButtonText = "Close"
            };
            await dialog.ShowAsync();
        }
    }
}