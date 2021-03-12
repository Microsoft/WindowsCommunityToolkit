// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// The <see cref="ImageCropper"/> control allows user to crop image freely.
    /// </summary>
    public partial class ImageCropper
    {
        private const double ThresholdValue = 0.001;

        private static async Task CropImageAsync(WriteableBitmap writeableBitmap, IRandomAccessStream stream, Rect croppedRect, BitmapFileFormat bitmapFileFormat)
        {
            croppedRect.X = Math.Max(croppedRect.X, 0);
            croppedRect.Y = Math.Max(croppedRect.Y, 0);
            var x = (uint)Math.Floor(croppedRect.X);
            var y = (uint)Math.Floor(croppedRect.Y);
            var width = (uint)Math.Floor(croppedRect.Width);
            var height = (uint)Math.Floor(croppedRect.Height);
            using (var sourceStream = writeableBitmap.PixelBuffer.AsStream())
            {
                var buffer = new byte[sourceStream.Length];
                await sourceStream.ReadAsync(buffer, 0, buffer.Length);
                var bitmapEncoder = await BitmapEncoder.CreateAsync(GetEncoderId(bitmapFileFormat), stream);
                bitmapEncoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)writeableBitmap.PixelWidth, (uint)writeableBitmap.PixelHeight, 96.0, 96.0, buffer);
                bitmapEncoder.BitmapTransform.Bounds = new BitmapBounds
                {
                    X = x,
                    Y = y,
                    Width = width,
                    Height = height
                };
                await bitmapEncoder.FlushAsync();
            }
        }

        private static async Task CropImageWithShapeAsync(WriteableBitmap writeableBitmap, IRandomAccessStream stream, Rect croppedRect, BitmapFileFormat bitmapFileFormat, CropShape cropShape)
        {
            var device = CanvasDevice.GetSharedDevice();
            var clipGeometry = CreateClipGeometry(device, cropShape, new Size(croppedRect.Width, croppedRect.Height));
            if (clipGeometry == null)
            {
                return;
            }

            CanvasBitmap sourceBitmap = null;
            using (var randomAccessStream = new InMemoryRandomAccessStream())
            {
                await CropImageAsync(writeableBitmap, randomAccessStream, croppedRect, bitmapFileFormat);
                sourceBitmap = await CanvasBitmap.LoadAsync(device, randomAccessStream);
            }

            using (var offScreen = new CanvasRenderTarget(device, (float)croppedRect.Width, (float)croppedRect.Height, 96f))
            {
                using (var drawingSession = offScreen.CreateDrawingSession())
                using (var markCommandList = new CanvasCommandList(device))
                {
                    using (var markDrawingSession = markCommandList.CreateDrawingSession())
                    {
                        markDrawingSession.FillGeometry(clipGeometry, Colors.Black);
                    }

                    var alphaMaskEffect = new AlphaMaskEffect
                    {
                        Source = sourceBitmap,
                        AlphaMask = markCommandList
                    };
                    drawingSession.DrawImage(alphaMaskEffect);
                    alphaMaskEffect.Dispose();
                }

                clipGeometry.Dispose();
                sourceBitmap.Dispose();
                var pixelBytes = offScreen.GetPixelBytes();
                var bitmapEncoder = await BitmapEncoder.CreateAsync(GetEncoderId(bitmapFileFormat), stream);
                bitmapEncoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, offScreen.SizeInPixels.Width, offScreen.SizeInPixels.Height, 96.0, 96.0, pixelBytes);
                await bitmapEncoder.FlushAsync();
            }
        }

        private static CanvasGeometry CreateClipGeometry(ICanvasResourceCreator resourceCreator, CropShape cropShape, Size croppedSize)
        {
            switch (cropShape)
            {
                case CropShape.Rectangular:
                    break;
                case CropShape.Circular:
                    var radiusX = croppedSize.Width / 2;
                    var radiusY = croppedSize.Height / 2;
                    var center = new Point(radiusX, radiusY);
                    return CanvasGeometry.CreateEllipse(resourceCreator, center.ToVector2(), (float)radiusX, (float)radiusY);
            }

            return null;
        }

        private static Guid GetEncoderId(BitmapFileFormat bitmapFileFormat)
        {
            switch (bitmapFileFormat)
            {
                case BitmapFileFormat.Bmp:
                    return BitmapEncoder.BmpEncoderId;
                case BitmapFileFormat.Png:
                    return BitmapEncoder.PngEncoderId;
                case BitmapFileFormat.Jpeg:
                    return BitmapEncoder.JpegEncoderId;
                case BitmapFileFormat.Tiff:
                    return BitmapEncoder.TiffEncoderId;
                case BitmapFileFormat.Gif:
                    return BitmapEncoder.GifEncoderId;
                case BitmapFileFormat.JpegXR:
                    return BitmapEncoder.JpegXREncoderId;
            }

            return BitmapEncoder.PngEncoderId;
        }

        /// <summary>
        /// Gets the closest point in the rectangle to a given point.
        /// </summary>
        /// <param name="targetRect">The rectangle.</param>
        /// <param name="point">The test point.</param>
        /// <returns>A point within a rectangle.</returns>
        private static Point GetSafePoint(Rect targetRect, Point point)
        {
            var safePoint = new Point(point.X, point.Y);
            if (safePoint.X < targetRect.X)
            {
                safePoint.X = targetRect.X;
            }

            if (safePoint.X > targetRect.X + targetRect.Width)
            {
                safePoint.X = targetRect.X + targetRect.Width;
            }

            if (safePoint.Y < targetRect.Y)
            {
                safePoint.Y = targetRect.Y;
            }

            if (safePoint.Y > targetRect.Y + targetRect.Height)
            {
                safePoint.Y = targetRect.Y + targetRect.Height;
            }

            return safePoint;
        }

        /// <summary>
        /// Test whether the point is in the rectangle.
        /// Similar to the Rect.Contains method, this method adds redundancy.
        /// </summary>
        /// <param name="targetRect">the rectangle.</param>
        /// <param name="point">The test point.</param>
        /// <returns>bool</returns>
        private static bool IsSafePoint(Rect targetRect, Point point)
        {
            if (point.X - targetRect.X < -ThresholdValue)
            {
                return false;
            }

            if (point.X - (targetRect.X + targetRect.Width) > ThresholdValue)
            {
                return false;
            }

            if (point.Y - targetRect.Y < -ThresholdValue)
            {
                return false;
            }

            if (point.Y - (targetRect.Y + targetRect.Height) > ThresholdValue)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether a rectangle satisfies the minimum size limit.
        /// </summary>
        /// <param name="startPoint">The point on the upper left corner.</param>
        /// <param name="endPoint">The point on the lower right corner.</param>
        /// <param name="minSize">The minimum size.</param>
        /// <returns>bool</returns>
        private static bool IsSafeRect(Point startPoint, Point endPoint, Size minSize)
        {
            var checkPoint = new Point(startPoint.X + minSize.Width, startPoint.Y + minSize.Height);
            return checkPoint.X - endPoint.X < ThresholdValue
                   && checkPoint.Y - endPoint.Y < ThresholdValue;
        }

        /// <summary>
        /// Gets a rectangle with a minimum size limit.
        /// </summary>
        /// <param name="startPoint">The point on the upper left corner.</param>
        /// <param name="endPoint">The point on the lower right corner.</param>
        /// <param name="minSize">The minimum size.</param>
        /// <param name="position">The control point.</param>
        /// <returns>The right rectangle.</returns>
        private static Rect GetSafeRect(Point startPoint, Point endPoint, Size minSize, ThumbPosition position)
        {
            var checkPoint = new Point(startPoint.X + minSize.Width, startPoint.Y + minSize.Height);
            switch (position)
            {
                case ThumbPosition.Top:
                    if (checkPoint.Y > endPoint.Y)
                    {
                        startPoint.Y = endPoint.Y - minSize.Height;
                    }

                    break;
                case ThumbPosition.Bottom:
                    if (checkPoint.Y > endPoint.Y)
                    {
                        endPoint.Y = startPoint.Y + minSize.Height;
                    }

                    break;
                case ThumbPosition.Left:
                    if (checkPoint.X > endPoint.X)
                    {
                        startPoint.X = endPoint.X - minSize.Width;
                    }

                    break;
                case ThumbPosition.Right:
                    if (checkPoint.X > endPoint.X)
                    {
                        endPoint.X = startPoint.X + minSize.Width;
                    }

                    break;
                case ThumbPosition.UpperLeft:
                    if (checkPoint.X > endPoint.X)
                    {
                        startPoint.X = endPoint.X - minSize.Width;
                    }

                    if (checkPoint.Y > endPoint.Y)
                    {
                        startPoint.Y = endPoint.Y - minSize.Height;
                    }

                    break;
                case ThumbPosition.UpperRight:
                    if (checkPoint.X > endPoint.X)
                    {
                        endPoint.X = startPoint.X + minSize.Width;
                    }

                    if (checkPoint.Y > endPoint.Y)
                    {
                        startPoint.Y = endPoint.Y - minSize.Height;
                    }

                    break;
                case ThumbPosition.LowerLeft:
                    if (checkPoint.X > endPoint.X)
                    {
                        startPoint.X = endPoint.X - minSize.Width;
                    }

                    if (checkPoint.Y > endPoint.Y)
                    {
                        endPoint.Y = startPoint.Y + minSize.Height;
                    }

                    break;
                case ThumbPosition.LowerRight:
                    if (checkPoint.X > endPoint.X)
                    {
                        endPoint.X = startPoint.X + minSize.Width;
                    }

                    if (checkPoint.Y > endPoint.Y)
                    {
                        endPoint.Y = startPoint.Y + minSize.Height;
                    }

                    break;
            }

            return startPoint.ToRect(endPoint);
        }

        /// <summary>
        /// Gets the maximum rectangle embedded in the rectangle by a given aspect ratio.
        /// </summary>
        /// <param name="targetRect">The rectangle.</param>
        /// <param name="aspectRatio">The aspect ratio.</param>
        /// <returns>The right rectangle.</returns>
        private static Rect GetUniformRect(Rect targetRect, double aspectRatio)
        {
            var ratio = targetRect.Width / targetRect.Height;
            var cx = targetRect.X + (targetRect.Width / 2);
            var cy = targetRect.Y + (targetRect.Height / 2);
            double width, height;
            if (aspectRatio > ratio)
            {
                width = targetRect.Width;
                height = width / aspectRatio;
            }
            else
            {
                height = targetRect.Height;
                width = height * aspectRatio;
            }

            return new Rect(cx - (width / 2f), cy - (height / 2f), width, height);
        }

        private static bool IsValidRect(Rect targetRect)
        {
            return !targetRect.IsEmpty && targetRect.Width > 0 && targetRect.Height > 0;
        }

        private static Point GetSafeSizeChangeWhenKeepAspectRatio(Rect targetRect, ThumbPosition thumbPosition, Rect selectedRect, Point originSizeChange, double aspectRatio)
        {
            var safeWidthChange = originSizeChange.X;
            var safeHeightChange = originSizeChange.Y;
            var maxWidthChange = 0d;
            var maxHeightChange = 0d;
            switch (thumbPosition)
            {
                case ThumbPosition.Top:
                    maxWidthChange = targetRect.Width - selectedRect.Width;
                    maxHeightChange = selectedRect.Top - targetRect.Top;
                    break;
                case ThumbPosition.Bottom:
                    maxWidthChange = targetRect.Width - selectedRect.Width;
                    maxHeightChange = targetRect.Bottom - selectedRect.Bottom;
                    break;
                case ThumbPosition.Left:
                    maxWidthChange = selectedRect.Left - targetRect.Left;
                    maxHeightChange = targetRect.Height - selectedRect.Height;
                    break;
                case ThumbPosition.Right:
                    maxWidthChange = targetRect.Right - selectedRect.Right;
                    maxHeightChange = targetRect.Height - selectedRect.Height;
                    break;
                case ThumbPosition.UpperLeft:
                    maxWidthChange = selectedRect.Left - targetRect.Left;
                    maxHeightChange = selectedRect.Top - targetRect.Top;
                    break;
                case ThumbPosition.UpperRight:
                    maxWidthChange = targetRect.Right - selectedRect.Right;
                    maxHeightChange = selectedRect.Top - targetRect.Top;
                    break;
                case ThumbPosition.LowerLeft:
                    maxWidthChange = selectedRect.Left - targetRect.Left;
                    maxHeightChange = targetRect.Bottom - selectedRect.Bottom;
                    break;
                case ThumbPosition.LowerRight:
                    maxWidthChange = targetRect.Right - selectedRect.Right;
                    maxHeightChange = targetRect.Bottom - selectedRect.Bottom;
                    break;
            }

            if (originSizeChange.X > maxWidthChange)
            {
                safeWidthChange = maxWidthChange;
                safeHeightChange = safeWidthChange / aspectRatio;
            }

            if (originSizeChange.Y > maxHeightChange)
            {
                safeHeightChange = maxHeightChange;
                safeWidthChange = safeHeightChange * aspectRatio;
            }

            return new Point(safeWidthChange, safeHeightChange);
        }

        private static bool CanContains(Rect targetRect, Rect testRect)
        {
            return (targetRect.Width - testRect.Width > -ThresholdValue) && (targetRect.Height - testRect.Height > -ThresholdValue);
        }

        private static bool TryGetContainedRect(Rect targetRect, ref Rect testRect)
        {
            if (!CanContains(targetRect, testRect))
            {
                return false;
            }

            if (targetRect.Left > testRect.Left)
            {
                testRect.X += targetRect.Left - testRect.Left;
            }

            if (targetRect.Top > testRect.Top)
            {
                testRect.Y += targetRect.Top - testRect.Top;
            }

            if (targetRect.Right < testRect.Right)
            {
                testRect.X += targetRect.Right - testRect.Right;
            }

            if (targetRect.Bottom < testRect.Bottom)
            {
                testRect.Y += targetRect.Bottom - testRect.Bottom;
            }

            return true;
        }

        private static bool IsCornerThumb(ThumbPosition thumbPosition)
        {
            switch (thumbPosition)
            {
                case ThumbPosition.Top:
                case ThumbPosition.Bottom:
                case ThumbPosition.Left:
                case ThumbPosition.Right:
                    return false;
                case ThumbPosition.UpperLeft:
                case ThumbPosition.UpperRight:
                case ThumbPosition.LowerLeft:
                case ThumbPosition.LowerRight:
                    return true;
            }

            return false;
        }
    }
}
