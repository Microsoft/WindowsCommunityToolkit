// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//// UWP Replacement for WPF RadialGradientBrush: https://msdn.microsoft.com/en-us/library/system.windows.media.radialgradientbrush(v=vs.110).aspx.

using System;
using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace CommunityToolkit.WinUI.UI.Media
{
    /// <summary>
    /// RadialGradientBrush - This GradientBrush defines its Gradient as an interpolation
    /// within an Ellipse.
    /// </summary>
    [Obsolete("Please migrate to the RadialGradientBrush control from WinUI, this control will be removed in a future release. https://aka.ms/winui")]
    [ContentProperty(Name = nameof(GradientStops))]
    public partial class RadialGradientBrush : CanvasBrushBase
    {
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var brush = (RadialGradientBrush)d;

            // We need to recreate the brush on any property change.
            brush.OnDisconnected();
            brush.OnConnected();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RadialGradientBrush"/> class.
        /// </summary>
        public RadialGradientBrush()
        {
            // Rendering surface size, if this is too small the gradient will be pixelated.
            // Larger targets aren't effected as one would expect unless the gradient is very complex.
            // This seems like a good compromise.
            SurfaceWidth = 512;
            SurfaceHeight = 512;

            GradientStops = new GradientStopCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RadialGradientBrush"/> class
        /// with two colors specified for GradientStops at
        /// offsets 0.0 and 1.0.
        /// </summary>
        /// <param name="startColor"> The Color at offset 0.0. </param>
        /// <param name="endColor"> The Color at offset 1.0. </param>
        public RadialGradientBrush(Color startColor, Color endColor)
            : this()
        {
            GradientStops.Add(new GradientStop() { Color = startColor, Offset = 0.0 });
            GradientStops.Add(new GradientStop() { Color = endColor, Offset = 1.0 });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RadialGradientBrush"/> class with GradientStops set to the passed-in collection.
        /// </summary>
        /// <param name="gradientStopCollection"> GradientStopCollection to set on this brush. </param>
        public RadialGradientBrush(GradientStopCollection gradientStopCollection)
            : this()
        {
            GradientStops = gradientStopCollection;
        }

        /// <inheritdoc/>
        protected override bool OnDraw(CanvasDevice device, CanvasDrawingSession session, Vector2 size)
        {
            // Create our Brush
            if (GradientStops != null && GradientStops.Count > 0)
            {
                var gradientBrush = new CanvasRadialGradientBrush(
                                        device,
                                        GradientStops.ToWin2DGradientStops(),
                                        SpreadMethod.ToEdgeBehavior(),
                                        (CanvasAlphaMode)(int)AlphaMode,
                                        ColorInterpolationMode.ToCanvasColorSpace(),
                                        CanvasColorSpace.Srgb,
                                        CanvasBufferPrecision.Precision8UIntNormalized)
                {
                    // Calculate Surface coordinates from 0.0-1.0 range given in WPF brush
                    RadiusX = size.X * (float)RadiusX,
                    RadiusY = size.Y * (float)RadiusY,
                    Center = size * Center.ToVector2(),

                    // Calculate Win2D Offset from origin/center used in WPF brush
                    OriginOffset = size * (GradientOrigin.ToVector2() - Center.ToVector2()),
                };

                // Use brush to draw on our canvas
                session.FillRectangle(size.ToRect(), gradientBrush);

                gradientBrush.Dispose();

                return true;
            }

            return false;
        }
    }
}