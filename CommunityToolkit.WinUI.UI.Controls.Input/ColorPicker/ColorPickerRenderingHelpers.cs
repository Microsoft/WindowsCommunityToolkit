// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.UI;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Contains the rendering methods used within <see cref="ColorPicker"/>.
    /// </summary>
    internal class ColorPickerRenderingHelpers
    {
        /// <summary>
        /// Generates a new bitmap of the specified size by changing a specific color channel.
        /// This will produce a gradient representing all possible differences of that color channel.
        /// </summary>
        /// <param name="width">The pixel width (X, horizontal) of the resulting bitmap.</param>
        /// <param name="height">The pixel height (Y, vertical) of the resulting bitmap.</param>
        /// <param name="orientation">The orientation of the resulting bitmap (gradient direction).</param>
        /// <param name="colorRepresentation">The color representation being used: RGBA or HSVA.</param>
        /// <param name="channel">The specific color channel to vary.</param>
        /// <param name="baseHsvColor">The base HSV color used for channels not being changed.</param>
        /// <param name="checkerColor">The color of the checker background square.</param>
        /// <param name="isAlphaMaxForced">Fix the alpha channel value to maximum during calculation.
        /// This will remove any alpha/transparency from the other channel backgrounds.</param>
        /// <param name="isSaturationValueMaxForced">Fix the saturation and value channels to maximum
        /// during calculation in HSVA color representation.
        /// This will ensure colors are always discernible regardless of saturation/value.</param>
        /// <returns>A new bitmap representing a gradient of color channel values.</returns>
        public static async Task<byte[]> CreateChannelBitmapAsync(
            int width,
            int height,
            Orientation orientation,
            ColorRepresentation colorRepresentation,
            ColorChannel channel,
            HsvColor baseHsvColor,
            Color? checkerColor,
            bool isAlphaMaxForced,
            bool isSaturationValueMaxForced)
        {
            if (width == 0 || height == 0)
            {
                return null;
            }

            var bitmap = await Task.Run<byte[]>(async () =>
            {
                int pixelDataIndex = 0;
                double channelStep;
                byte[] bgraPixelData;
                byte[] bgraCheckeredPixelData = null;
                Color baseRgbColor = Colors.White;
                Color rgbColor;
                int bgraPixelDataHeight;
                int bgraPixelDataWidth;

                // Allocate the buffer
                // BGRA formatted color channels 1 byte each (4 bytes in a pixel)
                bgraPixelData = new byte[width * height * 4];
                bgraPixelDataHeight = height * 4;
                bgraPixelDataWidth = width * 4;

                // Maximize alpha channel value
                if (isAlphaMaxForced &&
                    channel != ColorChannel.Alpha)
                {
                    baseHsvColor = new HsvColor()
                    {
                        H = baseHsvColor.H,
                        S = baseHsvColor.S,
                        V = baseHsvColor.V,
                        A = 1.0
                    };
                }

                // Convert HSV to RGB once
                if (colorRepresentation == ColorRepresentation.Rgba)
                {
                    baseRgbColor = WinUI.Helpers.ColorHelper.FromHsv(
                        baseHsvColor.H,
                        baseHsvColor.S,
                        baseHsvColor.V,
                        baseHsvColor.A);
                }

                // Maximize Saturation and Value channels when in HSVA mode
                if (isSaturationValueMaxForced &&
                    colorRepresentation == ColorRepresentation.Hsva &&
                    channel != ColorChannel.Alpha)
                {
                    switch (channel)
                    {
                        case ColorChannel.Channel1:
                            baseHsvColor = new HsvColor()
                            {
                                H = baseHsvColor.H,
                                S = 1.0,
                                V = 1.0,
                                A = baseHsvColor.A
                            };
                            break;
                        case ColorChannel.Channel2:
                            baseHsvColor = new HsvColor()
                            {
                                H = baseHsvColor.H,
                                S = baseHsvColor.S,
                                V = 1.0,
                                A = baseHsvColor.A
                            };
                            break;
                        case ColorChannel.Channel3:
                            baseHsvColor = new HsvColor()
                            {
                                H = baseHsvColor.H,
                                S = 1.0,
                                V = baseHsvColor.V,
                                A = baseHsvColor.A
                            };
                            break;
                    }
                }

                // Create a checkered background
                if (checkerColor != null)
                {
                    bgraCheckeredPixelData = await CreateCheckeredBitmapAsync(
                        width,
                        height,
                        checkerColor.Value);
                }

                // Create the color channel gradient
                if (orientation == Orientation.Horizontal)
                {
                    // Determine the numerical increment of the color steps within the channel
                    if (colorRepresentation == ColorRepresentation.Hsva)
                    {
                        if (channel == ColorChannel.Channel1)
                        {
                            channelStep = 360.0 / width;
                        }
                        else
                        {
                            channelStep = 1.0 / width;
                        }
                    }
                    else
                    {
                        channelStep = 255.0 / width;
                    }

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            if (y == 0)
                            {
                                rgbColor = GetColor(x * channelStep);

                                // Get a new color
                                bgraPixelData[pixelDataIndex + 0] = Convert.ToByte(rgbColor.B * rgbColor.A / 255);
                                bgraPixelData[pixelDataIndex + 1] = Convert.ToByte(rgbColor.G * rgbColor.A / 255);
                                bgraPixelData[pixelDataIndex + 2] = Convert.ToByte(rgbColor.R * rgbColor.A / 255);
                                bgraPixelData[pixelDataIndex + 3] = rgbColor.A;
                            }
                            else
                            {
                                // Use the color in the row above
                                // Remember the pixel data is 1 dimensional instead of 2
                                bgraPixelData[pixelDataIndex + 0] = bgraPixelData[pixelDataIndex + 0 - bgraPixelDataWidth];
                                bgraPixelData[pixelDataIndex + 1] = bgraPixelData[pixelDataIndex + 1 - bgraPixelDataWidth];
                                bgraPixelData[pixelDataIndex + 2] = bgraPixelData[pixelDataIndex + 2 - bgraPixelDataWidth];
                                bgraPixelData[pixelDataIndex + 3] = bgraPixelData[pixelDataIndex + 3 - bgraPixelDataWidth];
                            }

                            pixelDataIndex += 4;
                        }
                    }
                }
                else
                {
                    // Determine the numerical increment of the color steps within the channel
                    if (colorRepresentation == ColorRepresentation.Hsva)
                    {
                        if (channel == ColorChannel.Channel1)
                        {
                            channelStep = 360.0 / height;
                        }
                        else
                        {
                            channelStep = 1.0 / height;
                        }
                    }
                    else
                    {
                        channelStep = 255.0 / height;
                    }

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            if (x == 0)
                            {
                                // The lowest channel value should be at the 'bottom' of the bitmap
                                rgbColor = GetColor((height - 1 - y) * channelStep);

                                // Get a new color
                                bgraPixelData[pixelDataIndex + 0] = Convert.ToByte(rgbColor.B * rgbColor.A / 255);
                                bgraPixelData[pixelDataIndex + 1] = Convert.ToByte(rgbColor.G * rgbColor.A / 255);
                                bgraPixelData[pixelDataIndex + 2] = Convert.ToByte(rgbColor.R * rgbColor.A / 255);
                                bgraPixelData[pixelDataIndex + 3] = rgbColor.A;
                            }
                            else
                            {
                                // Use the color in the column to the left
                                // Remember the pixel data is 1 dimensional instead of 2
                                bgraPixelData[pixelDataIndex + 0] = bgraPixelData[pixelDataIndex - 4];
                                bgraPixelData[pixelDataIndex + 1] = bgraPixelData[pixelDataIndex - 3];
                                bgraPixelData[pixelDataIndex + 2] = bgraPixelData[pixelDataIndex - 2];
                                bgraPixelData[pixelDataIndex + 3] = bgraPixelData[pixelDataIndex - 1];
                            }

                            pixelDataIndex += 4;
                        }
                    }
                }

                // Composite the checkered background with color channel gradient for final result
                // The height/width are not checked as both bitmaps were built with the same values
                if ((checkerColor != null) &&
                    (bgraCheckeredPixelData != null))
                {
                    pixelDataIndex = 0;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            /* The following algorithm is used to blend the two bitmaps creating the final composite.
                             * In this formula, pixel data is normalized 0..1, actual pixel data is in the range 0..255.
                             * The color channel gradient should apply OVER the checkered background.
                             *
                             * R =  R0 * A0 * (1 - A1) + R1 * A1  =  RA0 * (1 - A1) + RA1
                             * G =  G0 * A0 * (1 - A1) + G1 * A1  =  GA0 * (1 - A1) + GA1
                             * B =  B0 * A0 * (1 - A1) + B1 * A1  =  BA0 * (1 - A1) + BA1
                             * A =  A0 * (1 - A1) + A1            =  A0 * (1 - A1) + A1
                             *
                             * Considering only the red channel, some algebraic transformation is applied to
                             * make the math quicker to solve.
                             *
                             * => ((RA0 / 255.0) * (1.0 - A1 / 255.0) + (RA1 / 255.0)) * 255.0
                             * => ((RA0 * 255) - (RA0 * A1) + (RA1 * 255)) / 255
                             */

                            // Bottom layer
                            byte rXa0 = bgraCheckeredPixelData[pixelDataIndex + 2];
                            byte gXa0 = bgraCheckeredPixelData[pixelDataIndex + 1];
                            byte bXa0 = bgraCheckeredPixelData[pixelDataIndex + 0];
                            byte a0 = bgraCheckeredPixelData[pixelDataIndex + 3];

                            // Top layer
                            byte rXa1 = bgraPixelData[pixelDataIndex + 2];
                            byte gXa1 = bgraPixelData[pixelDataIndex + 1];
                            byte bXa1 = bgraPixelData[pixelDataIndex + 0];
                            byte a1 = bgraPixelData[pixelDataIndex + 3];

                            bgraPixelData[pixelDataIndex + 0] = Convert.ToByte(((bXa0 * 255) - (bXa0 * a1) + (bXa1 * 255)) / 255);
                            bgraPixelData[pixelDataIndex + 1] = Convert.ToByte(((gXa0 * 255) - (gXa0 * a1) + (gXa1 * 255)) / 255);
                            bgraPixelData[pixelDataIndex + 2] = Convert.ToByte(((rXa0 * 255) - (rXa0 * a1) + (rXa1 * 255)) / 255);
                            bgraPixelData[pixelDataIndex + 3] = Convert.ToByte(((a0 * 255) - (a0 * a1) + (a1 * 255)) / 255);

                            pixelDataIndex += 4;
                        }
                    }
                }

                Color GetColor(double channelValue)
                {
                    Color newRgbColor = Colors.White;

                    switch (channel)
                    {
                        case ColorChannel.Channel1:
                        {
                            if (colorRepresentation == ColorRepresentation.Hsva)
                            {
                                // Sweep hue
                                newRgbColor = WinUI.Helpers.ColorHelper.FromHsv(
                                    Math.Clamp(channelValue, 0.0, 360.0),
                                    baseHsvColor.S,
                                    baseHsvColor.V,
                                    baseHsvColor.A);
                            }
                            else
                            {
                                // Sweep red
                                newRgbColor = new Color
                                {
                                    R = Convert.ToByte(Math.Clamp(channelValue, 0.0, 255.0)),
                                    G = baseRgbColor.G,
                                    B = baseRgbColor.B,
                                    A = baseRgbColor.A
                                };
                            }

                            break;
                        }

                        case ColorChannel.Channel2:
                        {
                            if (colorRepresentation == ColorRepresentation.Hsva)
                            {
                                // Sweep saturation
                                newRgbColor = WinUI.Helpers.ColorHelper.FromHsv(
                                    baseHsvColor.H,
                                    Math.Clamp(channelValue, 0.0, 1.0),
                                    baseHsvColor.V,
                                    baseHsvColor.A);
                            }
                            else
                            {
                                // Sweep green
                                newRgbColor = new Color
                                {
                                    R = baseRgbColor.R,
                                    G = Convert.ToByte(Math.Clamp(channelValue, 0.0, 255.0)),
                                    B = baseRgbColor.B,
                                    A = baseRgbColor.A
                                };
                            }

                            break;
                        }

                        case ColorChannel.Channel3:
                        {
                            if (colorRepresentation == ColorRepresentation.Hsva)
                            {
                                // Sweep value
                                newRgbColor = WinUI.Helpers.ColorHelper.FromHsv(
                                    baseHsvColor.H,
                                    baseHsvColor.S,
                                    Math.Clamp(channelValue, 0.0, 1.0),
                                    baseHsvColor.A);
                            }
                            else
                            {
                                // Sweep blue
                                newRgbColor = new Color
                                {
                                    R = baseRgbColor.R,
                                    G = baseRgbColor.G,
                                    B = Convert.ToByte(Math.Clamp(channelValue, 0.0, 255.0)),
                                    A = baseRgbColor.A
                                };
                            }

                            break;
                        }

                        case ColorChannel.Alpha:
                        {
                            if (colorRepresentation == ColorRepresentation.Hsva)
                            {
                                // Sweep alpha
                                newRgbColor = WinUI.Helpers.ColorHelper.FromHsv(
                                    baseHsvColor.H,
                                    baseHsvColor.S,
                                    baseHsvColor.V,
                                    Math.Clamp(channelValue, 0.0, 1.0));
                            }
                            else
                            {
                                // Sweep alpha
                                newRgbColor = new Color
                                {
                                    R = baseRgbColor.R,
                                    G = baseRgbColor.G,
                                    B = baseRgbColor.B,
                                    A = Convert.ToByte(Math.Clamp(channelValue, 0.0, 255.0))
                                };
                            }

                            break;
                        }
                    }

                    return newRgbColor;
                }

                return bgraPixelData;
            });

            return bitmap;
        }

        /// <summary>
        /// Generates a new checkered bitmap of the specified size.
        /// </summary>
        /// <remarks>
        /// This is a port and heavy modification of the code here:
        /// https://github.com/microsoft/microsoft-ui-xaml/blob/865e4fcc00e8649baeaec1ba7daeca398671aa72/dev/ColorPicker/ColorHelpers.cpp#L363
        /// UWP needs TiledBrush support.
        /// </remarks>
        /// <param name="width">The pixel width (X, horizontal) of the checkered bitmap.</param>
        /// <param name="height">The pixel height (Y, vertical) of the checkered bitmap.</param>
        /// <param name="checkerColor">The color of the checker square.</param>
        /// <returns>A new checkered bitmap of the specified size.</returns>
        public static async Task<byte[]> CreateCheckeredBitmapAsync(
            int width,
            int height,
            Color checkerColor)
        {
            // The size of the checker is important. You want it big enough that the grid is clearly discernible.
            // However, the squares should be small enough they don't appear unnaturally cut at the edge of backgrounds.
            int checkerSize = 4;

            if (width == 0 || height == 0)
            {
                return null;
            }

            var bitmap = await Task.Run<byte[]>(() =>
            {
                int pixelDataIndex = 0;
                byte[] bgraPixelData;

                // Allocate the buffer
                // BGRA formatted color channels 1 byte each (4 bytes in a pixel)
                bgraPixelData = new byte[width * height * 4];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // We want the checkered pattern to alternate both vertically and horizontally.
                        // In order to achieve that, we'll toggle visibility of the current pixel on or off
                        // depending on both its x- and its y-position.  If x == CheckerSize, we'll turn visibility off,
                        // but then if y == CheckerSize, we'll turn it back on.
                        // The below is a shorthand for the above intent.
                        bool pixelShouldBeBlank = ((x / checkerSize) + (y / checkerSize)) % 2 == 0 ? true : false;

                        // Remember, use BGRA pixel format with pre-multiplied alpha values
                        if (pixelShouldBeBlank)
                        {
                            bgraPixelData[pixelDataIndex + 0] = 0;
                            bgraPixelData[pixelDataIndex + 1] = 0;
                            bgraPixelData[pixelDataIndex + 2] = 0;
                            bgraPixelData[pixelDataIndex + 3] = 0;
                        }
                        else
                        {
                            bgraPixelData[pixelDataIndex + 0] = Convert.ToByte(checkerColor.B * checkerColor.A / 255);
                            bgraPixelData[pixelDataIndex + 1] = Convert.ToByte(checkerColor.G * checkerColor.A / 255);
                            bgraPixelData[pixelDataIndex + 2] = Convert.ToByte(checkerColor.R * checkerColor.A / 255);
                            bgraPixelData[pixelDataIndex + 3] = checkerColor.A;
                        }

                        pixelDataIndex += 4;
                    }
                }

                return bgraPixelData;
            });

            return bitmap;
        }

        /// <summary>
        /// Converts the given bitmap (in raw BGRA pre-multiplied alpha pixels) into an image brush
        /// that can be used in the UI.
        /// </summary>
        /// <param name="bitmap">The bitmap (in raw BGRA pre-multiplied alpha pixels) to convert to a brush.</param>
        /// <param name="width">The pixel width of the bitmap.</param>
        /// <param name="height">The pixel height of the bitmap.</param>
        /// <returns>A new ImageBrush.</returns>
        public static async Task<ImageBrush> BitmapToBrushAsync(
            byte[] bitmap,
            int width,
            int height)
        {
            var writableBitmap = new WriteableBitmap(width, height);
            using (Stream stream = writableBitmap.PixelBuffer.AsStream())
            {
                await stream.WriteAsync(bitmap, 0, bitmap.Length);
            }

            var brush = new ImageBrush()
            {
                ImageSource = writableBitmap,
                Stretch = Stretch.None
            };

            return brush;
        }

        /// <summary>
        /// Centralizes code to create a checker brush for a <see cref="Border"/>.
        /// </summary>
        /// <param name="border">Border which will have its Background modified.</param>
        /// <param name="color">Color to use for transparent checkerboard.</param>
        /// <returns>Task</returns>
        public static async Task UpdateBorderBackgroundWithCheckerAsync(Border border, Color color)
        {
            if (border != null)
            {
                int width = Convert.ToInt32(border.ActualWidth);
                int height = Convert.ToInt32(border.ActualHeight);

                var bitmap = await ColorPickerRenderingHelpers.CreateCheckeredBitmapAsync(
                    width,
                    height,
                    color);

                if (bitmap != null)
                {
                    border.Background = await ColorPickerRenderingHelpers.BitmapToBrushAsync(bitmap, width, height);
                }
            }
        }
    }
}