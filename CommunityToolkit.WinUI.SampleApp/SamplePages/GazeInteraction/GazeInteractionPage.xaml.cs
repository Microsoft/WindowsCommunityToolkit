// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Input.GazeInteraction;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Core;

namespace CommunityToolkit.WinUI.SampleApp.SamplePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GazeInteractionPage : IXamlRenderListener
    {
        private GazeElement gazeButtonControl;
        private GazePointer gazePointer;

        private int dwellCount = 0;

        public GazeInteractionPage()
        {
            this.InitializeComponent();
        }

        public void OnXamlRendered(FrameworkElement control)
        {
            GazeInput.IsDeviceAvailableChanged += GazeInput_IsDeviceAvailableChanged;

            WarnUserToPlugInDevice();

            var buttonControl = control.FindChild("TargetButton") as Button;

            if (buttonControl != null)
            {
                buttonControl.Click += TargetButton_Click;

                gazeButtonControl = GazeInput.GetGazeElement(buttonControl);

                if (gazeButtonControl == null)
                {
                    gazeButtonControl = new GazeElement();
                    GazeInput.SetGazeElement(buttonControl, gazeButtonControl);
                }

                if (gazeButtonControl != null)
                {
                    gazeButtonControl.DwellProgressFeedback += OnProgressFeedback;
                    gazeButtonControl.Invoked += OnGazeInvoked;
                    gazeButtonControl.StateChanged += GazeButtonControl_StateChanged;
                }
            }

            gazePointer = GazeInput.GetGazePointer(null);

            var coreWindow = CoreWindow.GetForCurrentThread();
            if (coreWindow != null)
            {
                coreWindow.KeyDown += (CoreWindow sender, KeyEventArgs args) => gazePointer.Click();
            }
        }

        private void GazeInput_IsDeviceAvailableChanged(object sender, object e)
        {
            DispatcherQueue.TryEnqueue(WarnUserToPlugInDevice);
        }

        private void WarnUserToPlugInDevice()
        {
            if (GazeInput.IsDeviceAvailable)
            {
                WarningText.Visibility = Visibility.Collapsed;
            }
            else
            {
                WarningText.Visibility = Visibility.Visible;
            }
        }

        private void OnGazeInvoked(object sender, DwellInvokedRoutedEventArgs e)
        {
        }

        private void OnProgressFeedback(object sender, DwellProgressEventArgs e)
        {
            DwellProgressBar.Maximum = 1.0;
            DwellProgressBar.Value = e.Progress;
            if (e.State == DwellProgressState.Complete)
            {
                DwellProgressBar.Value = 0;
            }
        }

        private void GazeButtonControl_StateChanged(object sender, StateChangedEventArgs ea)
        {
            if (ea.PointerState == PointerState.Enter)
            {
                EnterRec.Visibility = Visibility.Visible;
                DwellCountText.Visibility = Visibility.Collapsed;
                DwellCountText.Text = string.Empty;
                dwellCount = 0;
                ExitRec.Visibility = Visibility.Collapsed;
            }

            if (ea.PointerState == PointerState.Fixation)
            {
                FixationRec.Visibility = Visibility.Visible;
            }

            if (ea.PointerState == PointerState.Dwell)
            {
                if (dwellCount == 0)
                {
                    DwellRec.Visibility = Visibility.Visible;
                    dwellCount = 1;
                }
                else
                {
                    RepeatRec.Visibility = Visibility.Visible;
                    DwellCountText.Text = dwellCount.ToString();
                    DwellCountText.Visibility = Visibility.Visible;
                    dwellCount += 1;
                }
            }

            if (ea.PointerState == PointerState.Exit)
            {
                ExitRec.Visibility = Visibility.Visible;

                EnterRec.Visibility = Visibility.Collapsed;
                FixationRec.Visibility = Visibility.Collapsed;
                DwellRec.Visibility = Visibility.Collapsed;
                RepeatRec.Visibility = Visibility.Collapsed;
                DwellProgressBar.Value = 0;
            }
        }

        private int _targetButtonClickCount = 0;

        private void TargetButton_Click(object sender, RoutedEventArgs e)
        {
            ClickCount.Text = $"Number of clicks = {++_targetButtonClickCount}";
        }
    }
}