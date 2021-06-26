// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.System;
using Windows.UI.Xaml.Media.Imaging;

namespace SmokeTest
{
    public sealed partial class MainPage
    {
        private CameraHelper _cameraHelper;
        private DispatcherQueue _dispatcherQueue;

        public MainPage()
        {
            InitializeComponent();
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            image.Source = new SoftwareBitmapSource();
        }

        private async void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (_cameraHelper != null)
            {
                _cameraHelper.FrameArrived -= CameraHelper_FrameArrived;
                await _cameraHelper.CleanUpAsync();
            }

            _cameraHelper = new CameraHelper();
            var result = await _cameraHelper.InitializeAndStartCaptureAsync();

            if (result == CameraHelperResult.Success)
            {
                _cameraHelper.FrameArrived += CameraHelper_FrameArrived;
            }
            else
            {
                var errorMessage = result.ToString();
            }
        }

        private void CameraHelper_FrameArrived(object sender, FrameEventArgs e)
        {
            VideoFrame currentVideoFrame = e.VideoFrame;

            SoftwareBitmap softwareBitmap = currentVideoFrame.SoftwareBitmap;

            if (softwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 || softwareBitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
            {
                softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            }

            _dispatcherQueue.TryEnqueue(async () =>
            {
                if (image.Source is SoftwareBitmapSource softwareBitmapSource)
                {
                    await softwareBitmapSource.SetBitmapAsync(softwareBitmap);
                }
            });
        }
    }
}