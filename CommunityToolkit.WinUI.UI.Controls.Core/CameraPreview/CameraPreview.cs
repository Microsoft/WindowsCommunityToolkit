// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Media.Capture.Frames;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Camera Control to preview video. Can subscribe to video frames, software bitmap when they arrive.
    /// </summary>
    // TODO: WinUI3 Preview4 removed MediaPlayerElement
    // [TemplatePart(Name = Preview_MediaPlayerElementControl, Type = typeof(MediaPlayerElement))]
    [TemplatePart(Name = Preview_FrameSourceGroupButton, Type = typeof(Button))]
    public partial class CameraPreview : Control
    {
        private CameraHelper _cameraHelper;
        private MediaPlayer _mediaPlayer;

        // TODO: WinUI3 Preview4 removed MediaPlayerElement
        // private MediaPlayerElement _mediaPlayerElementControl;
        private Button _frameSourceGroupButton;

        private IReadOnlyList<MediaFrameSourceGroup> _frameSourceGroups;

        private bool IsFrameSourceGroupButtonAvailable => _frameSourceGroups != null && _frameSourceGroups.Count > 1;

        /// <summary>
        /// Gets Camera Helper
        /// </summary>
        public CameraHelper CameraHelper { get => _cameraHelper; private set => _cameraHelper = value; }

        /// <summary>
        /// Initialize control with a default CameraHelper instance for video preview and frame capture.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task StartAsync()
        {
            await StartAsync(new CameraHelper());
        }

        /// <summary>
        /// Initialize control with a CameraHelper instance for video preview and frame capture.
        /// </summary>
        /// <param name="cameraHelper"><see cref="CameraHelper"/></param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task StartAsync(CameraHelper cameraHelper)
        {
            if (cameraHelper == null)
            {
                cameraHelper = new CameraHelper();
            }

            _cameraHelper = cameraHelper;
            _frameSourceGroups = await CameraHelper.GetFrameSourceGroupsAsync();

            // UI controls exist and are initialized
            // TODO: WinUI3 Preview4 removed MediaPlayerElement
            // if (_mediaPlayerElementControl != null)
            // {
            //    await InitializeAsync();
            // }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraPreview"/> class.
        /// </summary>
        public CameraPreview()
        {
            this.DefaultStyleKey = typeof(CameraPreview);
        }

        /// <inheritdoc/>
        protected async override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_frameSourceGroupButton != null)
            {
                _frameSourceGroupButton.Click -= FrameSourceGroupButton_ClickAsync;
            }

            // TODO: WinUI3 Preview4 removed MediaPlayerElement
            // _mediaPlayerElementControl = (MediaPlayerElement)GetTemplateChild(Preview_MediaPlayerElementControl);
            _frameSourceGroupButton = (Button)GetTemplateChild(Preview_FrameSourceGroupButton);

            if (_frameSourceGroupButton != null)
            {
                _frameSourceGroupButton.Click += FrameSourceGroupButton_ClickAsync;
                _frameSourceGroupButton.IsEnabled = false;
                _frameSourceGroupButton.Visibility = Visibility.Collapsed;
            }

            if (_cameraHelper != null)
            {
                await InitializeAsync();
            }
        }

        private async Task InitializeAsync()
        {
            var result = await _cameraHelper.InitializeAndStartCaptureAsync();
            if (result != CameraHelperResult.Success)
            {
                InvokePreviewFailed(result.ToString());
            }

            SetUIControls(result);
        }

        private async void FrameSourceGroupButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            var oldGroup = _cameraHelper.FrameSourceGroup;
            var currentIndex = _frameSourceGroups.Select((grp, index) => new { grp, index }).First(v => v.grp.Id == oldGroup.Id).index;
            var newIndex = currentIndex < (_frameSourceGroups.Count - 1) ? currentIndex + 1 : 0;
            var group = _frameSourceGroups[newIndex];
            _frameSourceGroupButton.IsEnabled = false;
            _cameraHelper.FrameSourceGroup = group;
            await InitializeAsync();
        }

        private void InvokePreviewFailed(string error)
        {
            EventHandler<PreviewFailedEventArgs> handler = PreviewFailed;
            handler?.Invoke(this, new PreviewFailedEventArgs { Error = error });
        }

        private void SetMediaPlayerSource()
        {
            try
            {
                var frameSource = _cameraHelper?.PreviewFrameSource;
                if (frameSource != null)
                {
                    if (_mediaPlayer == null)
                    {
                        _mediaPlayer = new MediaPlayer
                        {
                            AutoPlay = true,
                            RealTimePlayback = true
                        };
                    }

                    _mediaPlayer.Source = MediaSource.CreateFromMediaFrameSource(frameSource);

                    // TODO: WinUI3 Preview4 removed MediaPlayerElement
                    // _mediaPlayerElementControl.SetMediaPlayer(_mediaPlayer);
                }
            }
            catch (Exception ex)
            {
                InvokePreviewFailed(ex.Message);
            }
        }

        private void SetUIControls(CameraHelperResult result)
        {
            var success = result == CameraHelperResult.Success;
            if (success)
            {
                SetMediaPlayerSource();
            }
            else
            {
                // TODO: WinUI3 Preview4 removed MediaPlayerElement
                // _mediaPlayerElementControl.SetMediaPlayer(null);
            }

            _frameSourceGroupButton.IsEnabled = IsFrameSourceGroupButtonAvailable;
            SetFrameSourceGroupButtonVisibility();
        }

        private void SetFrameSourceGroupButtonVisibility()
        {
            _frameSourceGroupButton.Visibility = IsFrameSourceGroupButtonAvailable && IsFrameSourceGroupButtonVisible
                                                                ? Visibility.Visible
                                                                : Visibility.Collapsed;
        }

        /// <summary>
        /// Stops preview.
        /// </summary>
        public void Stop()
        {
            // TODO: WinUI3 Preview4 removed MediaPlayerElement
            // if (_mediaPlayerElementControl != null)
            // {
            //    _mediaPlayerElementControl.SetMediaPlayer(null);
            // }
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Dispose();
                _mediaPlayer = null;
            }
        }
    }
}