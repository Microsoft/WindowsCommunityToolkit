// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture.Frames;

namespace CommunityToolkit.WinUI.SampleApp.SamplePages
{
    /// <summary>
    /// Sample page for Camera Helper
    /// </summary>
    public sealed partial class CameraHelperPage : Page
    {
        private CameraHelper _cameraHelper;
        private VideoFrame _currentVideoFrame;
        private SoftwareBitmapSource _softwareBitmapSource;

        public CameraHelperPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _softwareBitmapSource = new SoftwareBitmapSource();
            CurrentFrameImage.Source = _softwareBitmapSource;

            /*
            Application.Current.Suspending += Application_Suspending;
            Application.Current.Resuming += Application_Resuming;
            */

            await InitializeAsync();
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Application.Current.Suspending -= Application_Suspending;
            // Application.Current.Resuming -= Application_Resuming;
            await CleanUpAsync();
        }

        private async void Application_Suspending(object sender, SuspendingEventArgs e)
        {
            if (Frame?.CurrentSourcePageType == typeof(CameraHelperPage))
            {
                var deferral = e.SuspendingOperation.GetDeferral();
                await CleanUpAsync();
                deferral.Complete();
            }
        }

        private async void Application_Resuming(object sender, object e)
        {
            await InitializeAsync();
        }

        private void CameraHelper_FrameArrived(object sender, FrameEventArgs e)
        {
            _currentVideoFrame = e.VideoFrame;
        }

        private async Task InitializeAsync()
        {
            var frameSourceGroups = await CameraHelper.GetFrameSourceGroupsAsync();
            if (_cameraHelper == null)
            {
                _cameraHelper = new CameraHelper();
            }

            var result = await _cameraHelper.InitializeAndStartCaptureAsync();
            if (result == CameraHelperResult.Success)
            {
                // Subscribe to the video frame as they arrive
                _cameraHelper.FrameArrived += CameraHelper_FrameArrived;
                FrameSourceGroupCombo.ItemsSource = frameSourceGroups;
                FrameSourceGroupCombo.SelectionChanged += FrameSourceGroupCombo_SelectionChanged;
                FrameSourceGroupCombo.SelectedIndex = 0;
            }

            SetUIControls(result);
        }

        private async void FrameSourceGroupCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FrameSourceGroupCombo.SelectedItem is MediaFrameSourceGroup selectedGroup)
            {
                _cameraHelper.FrameSourceGroup = selectedGroup;
                var result = await _cameraHelper.InitializeAndStartCaptureAsync();
                SetUIControls(result);
            }
        }

        private void SetUIControls(CameraHelperResult result)
        {
            var success = result == CameraHelperResult.Success;
            if (!success)
            {
                _currentVideoFrame = null;
            }

            CameraErrorTextBlock.Text = result.ToString();
            CameraErrorTextBlock.Visibility = success ? Visibility.Collapsed : Visibility.Visible;

            CaptureButton.IsEnabled = success;
            CurrentFrameImage.Opacity = success ? 1 : 0.5;
        }

        private async void CaptureButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var softwareBitmap = _currentVideoFrame?.SoftwareBitmap;

            if (softwareBitmap != null)
            {
                if (softwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 || softwareBitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
                {
                    softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                }

                await _softwareBitmapSource.SetBitmapAsync(softwareBitmap);
            }
        }

        private async Task CleanUpAsync()
        {
            if (FrameSourceGroupCombo != null)
            {
                FrameSourceGroupCombo.SelectionChanged -= FrameSourceGroupCombo_SelectionChanged;
            }

            if (_cameraHelper != null)
            {
                _cameraHelper.FrameArrived -= CameraHelper_FrameArrived;
                await _cameraHelper.CleanUpAsync();
                _cameraHelper = null;
            }
        }
    }
}