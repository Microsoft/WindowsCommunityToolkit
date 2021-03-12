// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.ApplicationModel;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    /// <summary>
    /// CameraPreviewPage
    /// </summary>
    public sealed partial class CameraPreviewPage : Page, IXamlRenderListener
    {
        private static SemaphoreSlim semaphoreSlim;
        private VideoFrame _currentVideoFrame;
        private SoftwareBitmapSource _softwareBitmapSource;
        private CameraPreview _cameraPreviewControl;
        private Image _imageControl;
        private TextBlock _errorMessageText;

        public CameraPreviewPage()
        {
            this.InitializeComponent();
            semaphoreSlim = new SemaphoreSlim(1);
        }

        public async void OnXamlRendered(FrameworkElement control)
        {
            // Using a semaphore lock for synchronization.
            // This method gets called multiple times when accessing the page from Latest Pages
            // and creates unused duplicate references to Camera and memory leaks.
            await semaphoreSlim.WaitAsync();

            var cameraHelper = _cameraPreviewControl?.CameraHelper;
            UnsubscribeFromEvents();

            _cameraPreviewControl = control.FindChild<CameraPreview>();
            if (_cameraPreviewControl != null)
            {
                _cameraPreviewControl.PreviewFailed += CameraPreviewControl_PreviewFailed;
                await _cameraPreviewControl.StartAsync(cameraHelper);
                _cameraPreviewControl.CameraHelper.FrameArrived += CameraPreviewControl_FrameArrived;
            }

            _imageControl = control.FindDescendant("CurrentFrameImage") as Image;
            if (_imageControl != null)
            {
                _softwareBitmapSource = new SoftwareBitmapSource();
                _imageControl.Source = _softwareBitmapSource;
            }

            _errorMessageText = control.FindDescendant("ErrorMessage") as TextBlock;

            semaphoreSlim.Release();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SampleController.Current.RegisterNewCommand("Capture Current Frame", CaptureButton_Click);
            Application.Current.Suspending += Application_Suspending;
            Application.Current.Resuming += Application_Resuming;
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            Application.Current.Suspending -= Application_Suspending;
            Application.Current.Resuming -= Application_Resuming;
            await CleanUpAsync();
        }

        private async void Application_Suspending(object sender, SuspendingEventArgs e)
        {
            if (Frame?.CurrentSourcePageType == typeof(CameraPreviewPage))
            {
                var deferral = e.SuspendingOperation.GetDeferral();
                await CleanUpAsync();
                deferral.Complete();
            }
        }

        private async void Application_Resuming(object sender, object e)
        {
            if (_cameraPreviewControl != null)
            {
                var cameraHelper = _cameraPreviewControl.CameraHelper;
                _cameraPreviewControl.PreviewFailed += CameraPreviewControl_PreviewFailed;
                await _cameraPreviewControl.StartAsync(cameraHelper);
                _cameraPreviewControl.CameraHelper.FrameArrived += CameraPreviewControl_FrameArrived;
            }
        }

        private void CameraPreviewControl_FrameArrived(object sender, FrameEventArgs e)
        {
            _currentVideoFrame = e.VideoFrame;
        }

        private void CameraPreviewControl_PreviewFailed(object sender, PreviewFailedEventArgs e)
        {
            if (_errorMessageText != null)
            {
                _errorMessageText.Text = e.Error;
            }
        }

        private async void CaptureButton_Click(object sender, RoutedEventArgs e)
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

        private void UnsubscribeFromEvents()
        {
            if (_cameraPreviewControl != null)
            {
                if (_cameraPreviewControl.CameraHelper != null)
                {
                    _cameraPreviewControl.CameraHelper.FrameArrived -= CameraPreviewControl_FrameArrived;
                }

                _cameraPreviewControl.PreviewFailed -= CameraPreviewControl_PreviewFailed;
            }
        }

        private async Task CleanUpAsync()
        {
            UnsubscribeFromEvents();

            if (_cameraPreviewControl != null)
            {
                _cameraPreviewControl.Stop();
                await _cameraPreviewControl.CameraHelper?.CleanUpAsync();
            }
        }
    }
}
