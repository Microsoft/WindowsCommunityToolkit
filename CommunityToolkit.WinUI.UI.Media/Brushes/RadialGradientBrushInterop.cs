// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;

namespace CommunityToolkit.WinUI.UI.Media
{
    /// <summary>
    /// Interop Helpers between WPF and Win2D used by the <see cref="RadialGradientBrush"/>
    /// </summary>
    internal static class RadialGradientBrushInterop
    {
        /// <summary>
        /// Converts a WPF <see cref="ColorInterpolationMode"/> to a Win2D <see cref="CanvasColorSpace"/>.
        /// https://msdn.microsoft.com/en-us/library/system.windows.media.colorinterpolationmode(v=vs.110).aspx
        /// http://microsoft.github.io/Win2D/html/T_Microsoft_Graphics_Canvas_CanvasColorSpace.htm
        /// </summary>
        /// <param name="colorspace"><see cref="ColorInterpolationMode"/> mode.</param>
        /// <returns><see cref="CanvasColorSpace"/> space.</returns>
        public static CanvasColorSpace ToCanvasColorSpace(this ColorInterpolationMode colorspace)
        {
            switch (colorspace)
            {
                case ColorInterpolationMode.ScRgbLinearInterpolation:
                    return CanvasColorSpace.ScRgb;
                case ColorInterpolationMode.SRgbLinearInterpolation:
                    return CanvasColorSpace.Srgb;
            }

            return CanvasColorSpace.Custom;
        }

        /// <summary>
        /// Converts a WPF <see cref="GradientSpreadMethod"/> to a Win2D <see cref="CanvasEdgeBehavior"/>.
        /// https://msdn.microsoft.com/en-us/library/system.windows.media.gradientspreadmethod(v=vs.110).aspx
        /// http://microsoft.github.io/Win2D/html/T_Microsoft_Graphics_Canvas_CanvasEdgeBehavior.htm
        /// </summary>
        /// <param name="method"><see cref="GradientSpreadMethod"/> method.</param>
        /// <returns><see cref="CanvasEdgeBehavior"/> behavior.</returns>
        public static CanvasEdgeBehavior ToEdgeBehavior(this GradientSpreadMethod method)
        {
            switch (method)
            {
                case GradientSpreadMethod.Pad:
                    return CanvasEdgeBehavior.Clamp;
                case GradientSpreadMethod.Reflect:
                    return CanvasEdgeBehavior.Mirror;
                case GradientSpreadMethod.Repeat:
                    return CanvasEdgeBehavior.Wrap;
            }

            return CanvasEdgeBehavior.Clamp;
        }

        /// <summary>
        /// Returns a new <see cref="Windows.Foundation.Rect(double, double, double, double)"/> representing the size of the <see cref="Vector2"/>.
        /// </summary>
        /// <param name="vector"><see cref="Vector2"/> vector representing object size for Rectangle.</param>
        /// <returns><see cref="Windows.Foundation.Rect(double, double, double, double)"/> value.</returns>
        public static Rect ToRect(this Vector2 vector)
        {
            return new Rect(0, 0, vector.X, vector.Y);
        }

        /// <summary>
        /// Returns a new <see cref="Vector2"/> representing the <see cref="Size(double,double)"/>.
        /// </summary>
        /// <param name="size"><see cref="Size(double,double)"/> value.</param>
        /// <returns><see cref="Vector2"/> value.</returns>
        public static Vector2 ToVector2(this Size size)
        {
            return new Vector2((float)size.Width, (float)size.Height);
        }

        /// <summary>
        /// Converts a <see cref="GradientStopCollection"/> to an array of <see cref="CanvasGradientStop"/>.
        /// </summary>
        /// <param name="stops"><see cref="GradientStopCollection"/> collection of gradient stops.</param>
        /// <returns>New array of <see cref="CanvasGradientStop"/> stops.</returns>
        public static CanvasGradientStop[] ToWin2DGradientStops(this GradientStopCollection stops)
        {
            var canvasStops = new CanvasGradientStop[stops.Count];

            int x = 0;
            foreach (var stop in stops)
            {
                canvasStops[x++] = new CanvasGradientStop { Color = stop.Color, Position = (float)stop.Offset };
            }

            return canvasStops;
        }
    }
}