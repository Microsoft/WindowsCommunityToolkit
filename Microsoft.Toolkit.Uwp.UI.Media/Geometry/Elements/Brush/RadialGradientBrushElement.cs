// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Toolkit.Uwp.UI.Media.Geometry.Core;
using Microsoft.Toolkit.Uwp.UI.Media.Geometry.Parsers;
using Windows.UI;

namespace Microsoft.Toolkit.Uwp.UI.Media.Geometry.Elements.Brush
{
    /// <summary>
    /// Represents a CanvasRadialGradientBrush with GradientStops
    /// </summary>
    internal sealed class RadialGradientBrushElement : AbstractCanvasBrushElement
    {
        private float _radiusX;
        private float _radiusY;
        private Vector2 _center;
        private Vector2 _originOffset;
        private CanvasAlphaMode _alphaMode;
        private CanvasBufferPrecision _bufferPrecision;
        private CanvasEdgeBehavior _edgeBehavior;
        private CanvasColorSpace _preInterpolationColorSpace;
        private CanvasColorSpace _postInterpolationColorSpace;
        private List<CanvasGradientStop> _gradientStops;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadialGradientBrushElement"/> class.
        /// </summary>
        /// <param name="capture">Capture object</param>
        public RadialGradientBrushElement(Capture capture)
        {
            // Set the default values
            _radiusX = 0f;
            _radiusY = 0f;
            _center = Vector2.Zero;
            _originOffset = Vector2.Zero;
            _opacity = 1f;
            _alphaMode = (CanvasAlphaMode)0;
            _bufferPrecision = (CanvasBufferPrecision)0;
            _edgeBehavior = (CanvasEdgeBehavior)0;

            // Default ColorSpace is sRGB
            _preInterpolationColorSpace = CanvasColorSpace.Srgb;
            _postInterpolationColorSpace = CanvasColorSpace.Srgb;
            _gradientStops = new List<CanvasGradientStop>();

            // Initialize
            Initialize(capture);
        }

        /// <summary>
        /// Creates the CanvasLinearGradientBrush from the parsed data
        /// </summary>
        /// <param name="resourceCreator">ICanvasResourceCreator object</param>
        /// <returns>Instance of <see cref="ICanvasBrush"/></returns>
        public override ICanvasBrush CreateBrush(ICanvasResourceCreator resourceCreator)
        {
            var brush = new CanvasRadialGradientBrush(
                resourceCreator,
                _gradientStops.ToArray(),
                _edgeBehavior,
                _alphaMode,
                _preInterpolationColorSpace,
                _postInterpolationColorSpace,
                _bufferPrecision)
            {
                RadiusX = _radiusX,
                RadiusY = _radiusY,
                Center = _center,
                OriginOffset = _originOffset,
                Opacity = _opacity
            };

            return brush;
        }

        /// <summary>
        /// Gets the Regex for extracting Brush Element Attributes
        /// </summary>
        /// <returns>Regex</returns>
        protected override Regex GetAttributesRegex()
        {
            return RegexFactory.GetAttributesRegex(BrushType.RadialGradient);
        }

        /// <summary>
        /// Gets the Brush Element Attributes from the Match
        /// </summary>
        /// <param name="match">Match object</param>
        protected override void GetAttributes(Match match)
        {
            // RadiusX
            float.TryParse(match.Groups["RadiusX"].Value, out _radiusX);

            // Sanitize by taking the absolute value
            _radiusX = Math.Abs(_radiusX);

            // RadiusY
            float.TryParse(match.Groups["RadiusY"].Value, out _radiusY);

            // Sanitize by taking the absolute value
            _radiusY = Math.Abs(_radiusY);

            // CenterX
            float.TryParse(match.Groups["CenterX"].Value, out var centerX);

            // CenterY
            float.TryParse(match.Groups["CenterY"].Value, out var centerY);
            _center = new Vector2(centerX, centerY);

            // Opacity (optional)
            var group = match.Groups["Opacity"];
            if (group.Success)
            {
                float.TryParse(group.Value, out _opacity);
            }

            // Origin Offset (optional)
            group = match.Groups["OriginOffset"];
            if (group.Success)
            {
                float.TryParse(match.Groups["OffsetX"].Value, out var offsetX);
                float.TryParse(match.Groups["OffsetY"].Value, out var offsetY);
                _originOffset = new Vector2(offsetX, offsetY);
            }

            // Alpha Mode (optional)
            group = match.Groups["AlphaMode"];
            if (group.Success)
            {
                Enum.TryParse(group.Value, out _alphaMode);
            }

            // Buffer Precision (optional)
            group = match.Groups["BufferPrecision"];
            if (group.Success)
            {
                Enum.TryParse(group.Value, out _bufferPrecision);
            }

            // Edge Behavior (optional)
            group = match.Groups["EdgeBehavior"];
            if (group.Success)
            {
                Enum.TryParse(group.Value, out _edgeBehavior);
            }

            // Pre Interpolation ColorSpace (optional)
            group = match.Groups["PreColorSpace"];
            if (group.Success)
            {
                Enum.TryParse(group.Value, out _preInterpolationColorSpace);
            }

            // Post Interpolation ColorSpace (optional)
            group = match.Groups["PostColorSpace"];
            if (group.Success)
            {
                Enum.TryParse(group.Value, out _postInterpolationColorSpace);
            }

            // Gradient Stops
            group = match.Groups["GradientStops"];
            if (group.Success)
            {
                _gradientStops.Clear();
                foreach (Capture capture in group.Captures)
                {
                    var gradientMatch = RegexFactory.GradientStopRegex.Match(capture.Value);
                    if (!gradientMatch.Success)
                    {
                        continue;
                    }

                    float position;
                    Color color;

                    // Main Attributes
                    var main = gradientMatch.Groups["Main"];
                    if (main.Success)
                    {
                        var mainMatch = RegexFactory.GetAttributesRegex(GradientStopAttributeType.Main).Match(main.Value);
                        float.TryParse(mainMatch.Groups["Position"].Value, out position);
                        color = ColorParser.Parse(mainMatch);

                        _gradientStops.Add(new CanvasGradientStop()
                        {
                            Color = color,
                            Position = position
                        });
                    }

                    // Additional Attributes
                    var additional = gradientMatch.Groups["Additional"];
                    if (!additional.Success)
                    {
                        continue;
                    }

                    foreach (Capture addCapture in additional.Captures)
                    {
                        var addMatch = RegexFactory.GetAttributesRegex(GradientStopAttributeType.Additional).Match(addCapture.Value);
                        float.TryParse(addMatch.Groups["Position"].Value, out position);
                        color = ColorParser.Parse(addMatch);

                        _gradientStops.Add(new CanvasGradientStop()
                        {
                            Color = color,
                            Position = position
                        });
                    }
                }

                // Sort the stops based on their position
                if (_gradientStops.Any())
                {
                    _gradientStops = _gradientStops.OrderBy(g => g.Position).ToList();
                }
            }
        }
    }
}