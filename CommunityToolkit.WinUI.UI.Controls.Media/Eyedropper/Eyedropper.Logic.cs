// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Foundation;
using Windows.Graphics.DirectX;
using Windows.Graphics.Display;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// The <see cref="Eyedropper"/> control can pick up a color from anywhere in your application.
    /// </summary>
    public partial class Eyedropper
    {
        private void UpdateEyedropper(Point position)
        {
            if (_appScreenshot == null)
            {
                return;
            }

            _layoutTransform.X = position.X - (ActualWidth / 2);
            _layoutTransform.Y = position.Y - ActualHeight;

            var x = (int)Math.Ceiling(Math.Min(_appScreenshot.SizeInPixels.Width - 1, Math.Max(position.X, 0)));
            var y = (int)Math.Ceiling(Math.Min(_appScreenshot.SizeInPixels.Height - 1, Math.Max(position.Y, 0)));
            Color = _appScreenshot.GetPixelColors(x, y, 1, 1).Single();
            UpdatePreview(x, y);
        }

        private void UpdateWorkArea()
        {
            if (_targetGrid == null)
            {
                return;
            }

            if (WorkArea == default(Rect))
            {
                _targetGrid.Margin = default(Thickness);
            }
            else
            {
                var left = WorkArea.Left;
                var top = WorkArea.Top;
                double right;
                double bottom;
                if (XamlRoot != null)
                {
                    right = XamlRoot.Size.Width - WorkArea.Right;
                    bottom = XamlRoot.Size.Height - WorkArea.Bottom;
                }
                else
                {
                    right = Window.Current.Bounds.Width - WorkArea.Right;
                    bottom = Window.Current.Bounds.Height - WorkArea.Bottom;
                }

                _targetGrid.Margin = new Thickness(left, top, right, bottom);
            }
        }

        private void UpdatePreview(int centerX, int centerY)
        {
            var halfPixelCountPerRow = (PixelCountPerRow - 1) / 2;
            var left = (int)Math.Min(
                _appScreenshot.SizeInPixels.Width - 1,
                Math.Max(centerX - halfPixelCountPerRow, 0));
            var top = (int)Math.Min(
                _appScreenshot.SizeInPixels.Height - 1,
                Math.Max(centerY - halfPixelCountPerRow, 0));
            var right = (int)Math.Min(centerX + halfPixelCountPerRow, _appScreenshot.SizeInPixels.Width - 1);
            var bottom = (int)Math.Min(centerY + halfPixelCountPerRow, _appScreenshot.SizeInPixels.Height - 1);
            var width = right - left + 1;
            var height = bottom - top + 1;
            var colors = _appScreenshot.GetPixelColors(left, top, width, height);
            var colorStartX = left - (centerX - halfPixelCountPerRow);
            var colorStartY = top - (centerY - halfPixelCountPerRow);
            var colorEndX = colorStartX + width;
            var colorEndY = colorStartY + height;

            var size = new Size(PreviewPixelsPerRawPixel, PreviewPixelsPerRawPixel);
            var startPoint = new Point(0, PreviewPixelsPerRawPixel * colorStartY);

            using (var drawingSession = _previewImageSource.CreateDrawingSession(Colors.White))
            {
                for (var i = colorStartY; i < colorEndY; i++)
                {
                    startPoint.X = colorStartX * PreviewPixelsPerRawPixel;
                    for (var j = colorStartX; j < colorEndX; j++)
                    {
                        var color = colors[((i - colorStartY) * width) + (j - colorStartX)];
                        drawingSession.FillRectangle(startPoint.ToRect(size), color);
                        startPoint.X += PreviewPixelsPerRawPixel;
                    }

                    startPoint.Y += PreviewPixelsPerRawPixel;
                }
            }
        }

        internal async Task UpdateAppScreenshotAsync()
        {
            double scale;
            double width;
            double height;
            UIElement content;
            if (XamlRoot != null)
            {
                scale = XamlRoot.RasterizationScale;
                width = XamlRoot.Size.Width;
                height = XamlRoot.Size.Height;
                content = XamlRoot.Content;
            }
            else
            {
                var displayInfo = DisplayInformation.GetForCurrentView();
                scale = displayInfo.RawPixelsPerViewPixel;
                width = Window.Current.Bounds.Width;
                height = Window.Current.Bounds.Height;
                content = Window.Current.Content;
            }

            try
            {
                var renderTarget = new RenderTargetBitmap();
                var scaleWidth = (int)Math.Ceiling(width / scale);
                var scaleHeight = (int)Math.Ceiling(height / scale);
                await renderTarget.RenderAsync(content, scaleWidth, scaleHeight);
                var pixels = await renderTarget.GetPixelsAsync();

                _appScreenshot?.Dispose();
                _appScreenshot = null;
                _appScreenshot = CanvasBitmap.CreateFromBytes(_device, pixels, renderTarget.PixelWidth, renderTarget.PixelHeight, DirectXPixelFormat.B8G8R8A8UIntNormalized);
            }
            catch (OutOfMemoryException ex)
            {
                global::System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}