// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.MediaProperties;

namespace Microsoft.Toolkit.Uwp.Helpers
{
    /// <summary>
    /// Helper class for capturing frames from available camera sources.
    /// Make sure you have the capability webcam enabled for your app to access the device's camera.
    /// </summary>
    public class CameraHelper : IDisposable
    {
        private static IReadOnlyList<MediaFrameSourceGroup> _frameSourceGroups;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);
        private MediaCapture _mediaCapture;
        private MediaFrameReader _frameReader;
        private MediaFrameSourceGroup _group;
        private bool groupChanged = false;
        private bool _initialized;

        /// <summary>
        /// Gets a list of <see cref="MediaFrameSourceGroup"/> available for video preview or video record.
        /// </summary>
        /// <returns>A <see cref="MediaFrameSourceGroup"/> list.</returns>
        public static async Task<IReadOnlyList<MediaFrameSourceGroup>> GetFrameSourceGroupsAsync()
        {
            if (_frameSourceGroups == null)
            {
                var videoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
                var groups = await MediaFrameSourceGroup.FindAllAsync();

                // Filter out color video preview and video record type sources and remove duplicates video devices.
                _frameSourceGroups = groups.Where(g => g.SourceInfos.Any(s => s.SourceKind == MediaFrameSourceKind.Color &&
                                                                            (s.MediaStreamType == MediaStreamType.VideoPreview || s.MediaStreamType == MediaStreamType.VideoRecord))
                                                                            && g.SourceInfos.All(sourceInfo => videoDevices.Any(vd => vd.Id == sourceInfo.DeviceInformation.Id))).ToList();
            }

            return _frameSourceGroups;
        }

        /// <summary>
        /// Gets a list of <see cref="MediaFrameFormat"/> available on the source.
        /// </summary>
        public IReadOnlyList<MediaFrameFormat> FrameFormatsAvailable { get; private set; }

        /// <summary>
        /// Gets or sets the source group for camera video preview.
        /// </summary>
        public MediaFrameSourceGroup FrameSourceGroup
        {
            get => _group;
            set
            {
                groupChanged = _group != value;
                _group = value;
            }
        }

        /// <summary>
        /// Gets the currently selected <see cref="MediaFrameSource"/> for video preview.
        /// </summary>
        public MediaFrameSource PreviewFrameSource { get; private set; }

        /// <summary>
        /// Occurs when a new frame arrives.
        /// </summary>
        public event EventHandler<FrameEventArgs> FrameArrived;

        /// <summary>
        /// Initializes Camera Media Capture settings and initializes Frame Reader to capture frames in real time.
        /// If no MediaFrameSourceGroup is provided, it selects the first available camera source to  use for media capture.
        /// You could select a specific MediaFrameSourceGroup from the available sources using the CameraHelper FrameSourceGroups property.
        /// </summary>
        /// <returns>Result of the async operation.<see cref="CameraHelperResult"/></returns>
        public async Task<CameraHelperResult> InitializeAndStartCaptureAsync()
        {
            CameraHelperResult result;
            try
            {
                await _semaphoreSlim.WaitAsync();

                // if FrameSourceGroup hasn't changed from last initialization, just return back.
                if (_initialized && _group != null && !groupChanged)
                {
                    return CameraHelperResult.Success;
                }

                groupChanged = false;

                await StopReaderAsync();

                if (_mediaCapture != null)
                {
                    _mediaCapture.Dispose();
                    _mediaCapture = null;
                }

                if (_frameSourceGroups == null)
                {
                    _frameSourceGroups = await GetFrameSourceGroupsAsync();
                }

                if (_group == null)
                {
                    _group = _frameSourceGroups.FirstOrDefault();
                }
                else
                {
                    // Verify selected group is part of existing FrameSourceGroups
                    _group = _frameSourceGroups.FirstOrDefault(g => g.Id == _group.Id);
                }

                // If there is no camera source available, we can't proceed
                if (_group == null)
                {
                    return CameraHelperResult.NoFrameSourceGroupAvailable;
                }

                result = await InitializeMediaCaptureAsync();

                if (PreviewFrameSource != null)
                {
                    _frameReader = await _mediaCapture.CreateFrameReaderAsync(PreviewFrameSource);
                    _frameReader.AcquisitionMode = MediaFrameReaderAcquisitionMode.Realtime;

                    _frameReader.FrameArrived += Reader_FrameArrived;

                    if (_frameReader == null)
                    {
                        result = CameraHelperResult.CreateFrameReaderFailed;
                    }
                    else
                    {
                        MediaFrameReaderStartStatus statusResult = await _frameReader.StartAsync();
                        if (statusResult != MediaFrameReaderStartStatus.Success)
                        {
                            result = CameraHelperResult.StartFrameReaderFailed;
                        }
                    }
                }

                _initialized = result == CameraHelperResult.Success;
                return result;
            }
            catch (Exception)
            {
                await CleanUpAsync();
                return CameraHelperResult.InitializationFailed_UnknownError;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        /// <summary>
        /// Clean up the Camera Helper resources
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task CleanUpAsync()
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                _initialized = false;
                await StopReaderAsync();

                if (_mediaCapture != null)
                {
                    _mediaCapture.Dispose();
                    _mediaCapture = null;
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async Task<CameraHelperResult> InitializeMediaCaptureAsync()
        {
            if (_mediaCapture == null)
            {
                _mediaCapture = new MediaCapture();
            }

            var settings = new MediaCaptureInitializationSettings()
            {
                SourceGroup = _group,
                MemoryPreference = MediaCaptureMemoryPreference.Cpu,
                StreamingCaptureMode = StreamingCaptureMode.Video
            };

            try
            {
                await _mediaCapture.InitializeAsync(settings);

                // Find the first video preview or record stream available
                PreviewFrameSource = _mediaCapture.FrameSources.FirstOrDefault(source => source.Value.Info.MediaStreamType == MediaStreamType.VideoPreview
                                                                                      && source.Value.Info.SourceKind == MediaFrameSourceKind.Color).Value;
                if (PreviewFrameSource == null)
                {
                    PreviewFrameSource = _mediaCapture.FrameSources.FirstOrDefault(source => source.Value.Info.MediaStreamType == MediaStreamType.VideoRecord
                                                                                          && source.Value.Info.SourceKind == MediaFrameSourceKind.Color).Value;
                }

                if (PreviewFrameSource == null)
                {
                    return CameraHelperResult.NoFrameSourceAvailable;
                }

                // Get only formats of a certain frame-rate and compatible subtype for previewing, order them by resolution
                FrameFormatsAvailable = PreviewFrameSource.SupportedFormats.Where(format =>
                    format.FrameRate.Numerator / format.FrameRate.Denominator >= 15 // fps
                    && (string.Compare(format.Subtype, MediaEncodingSubtypes.Nv12, true) == 0
                        || string.Compare(format.Subtype, MediaEncodingSubtypes.Bgra8, true) == 0
                        || string.Compare(format.Subtype, MediaEncodingSubtypes.Yuy2, true) == 0
                        || string.Compare(format.Subtype, MediaEncodingSubtypes.Rgb32, true) == 0))?.OrderBy(format => format.VideoFormat.Width * format.VideoFormat.Height).ToList();

                if (FrameFormatsAvailable == null || !FrameFormatsAvailable.Any())
                {
                    return CameraHelperResult.NoCompatibleFrameFormatAvailable;
                }

                // Set the format with the highest resolution available by default
                var defaultFormat = FrameFormatsAvailable.Last();
                await PreviewFrameSource.SetFormatAsync(defaultFormat);
            }
            catch (UnauthorizedAccessException)
            {
                await StopReaderAsync();

                if (_mediaCapture != null)
                {
                    _mediaCapture.Dispose();
                    _mediaCapture = null;
                }

                return CameraHelperResult.CameraAccessDenied;
            }
            catch (Exception)
            {
                await StopReaderAsync();

                if (_mediaCapture != null)
                {
                    _mediaCapture.Dispose();
                    _mediaCapture = null;
                }

                return CameraHelperResult.InitializationFailed_UnknownError;
            }

            return CameraHelperResult.Success;
        }

        /// <summary>
        /// Stops reading from the frame reader and disposes of the reader.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task StopReaderAsync()
        {
            if (_frameReader != null)
            {
                _frameReader.FrameArrived -= Reader_FrameArrived;
                await _frameReader.StopAsync();
                _frameReader.Dispose();
                _frameReader = null;
            }
        }

        /// <summary>
        /// Handles the frame arrived event by converting the frame to a displayable
        /// format and rendering it to the screen.
        /// </summary>
        private void Reader_FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            // TryAcquireLatestFrame will return the latest frame that has not yet been acquired.
            // This can return null if there is no such frame, or if the reader is not in the
            // "Started" state. The latter can occur if a FrameArrived event was in flight
            // when the reader was stopped.
            var frame = sender.TryAcquireLatestFrame();
            if (frame != null)
            {
                var vmf = frame.VideoMediaFrame;
                EventHandler<FrameEventArgs> handler = FrameArrived;
                var frameArgs = new FrameEventArgs() { VideoFrame = vmf.GetVideoFrame() };
                handler?.Invoke(sender, frameArgs);
            }
        }

        private bool disposedValue = false;

        /// <inheritdoc/>
        public async void Dispose()
        {
            if (!disposedValue)
            {
                disposedValue = true;
                await CleanUpAsync();
            }
        }
    }
}