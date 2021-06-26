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

namespace Microsoft.Toolkit.Uwp.UI.Media.Geometry.Elements.Brush
{
    /// <summary>
    /// Represents a CanvasRadialGradientBrush with GradientStopHdrs
    /// </summary>
    internal sealed class RadialGradientHdrBrushElement : AbstractCanvasBrushElement
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
        private List<CanvasGradientStopHdr> _gradientStopHdrs;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadialGradientHdrBrushElement"/> class.
        /// </summary>
        /// <param name="capture">Capture object</param>
        public RadialGradientHdrBrushElement(Capture capture)
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
            _gradientStopHdrs = new List<CanvasGradientStopHdr>();

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
            var brush = CanvasRadialGradientBrush.CreateHdr(
                resourceCreator,
                _gradientStopHdrs.ToArray(),
                _edgeBehavior,
                _alphaMode,
                _preInterpolationColorSpace,
                _postInterpolationColorSpace,
                _bufferPrecision);

            brush.RadiusX = _radiusX;
            brush.RadiusY = _radiusY;
            brush.Center = _center;
            brush.OriginOffset = _originOffset;
            brush.Opacity = _opacity;

            return brush;
        }

        /// <summary>
        /// Gets the Regex for extracting Brush Element Attributes
        /// </summary>
        /// <returns>Regex</returns>
        protected override Regex GetAttributesRegex()
        {
            return RegexFactory.GetAttributesRegex(BrushType.RadialGradientHdr);
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

            // GradientStopHdrs
            group = match.Groups["GradientStops"];
            if (group.Success)
            {
                _gradientStopHdrs.Clear();
                foreach (Capture capture in group.Captures)
                {
                    var gradientMatch = RegexFactory.GradientStopHdrRegex.Match(capture.Value);
                    if (!gradientMatch.Success)
                    {
                        continue;
                    }

                    float position;
                    float x = 0, y = 0, z = 0, w = 0;
                    var main = gradientMatch.Groups["Main"];
                    if (main.Success)
                    {
                        var mainMatch = RegexFactory.GetAttributesRegex(GradientStopAttributeType.MainHdr)
                                                    .Match(main.Value);

                        float.TryParse(mainMatch.Groups["Position"].Value, out position);
                        float.TryParse(mainMatch.Groups["X"].Value, out x);
                        float.TryParse(mainMatch.Groups["Y"].Value, out y);
                        float.TryParse(mainMatch.Groups["Z"].Value, out z);
                        float.TryParse(mainMatch.Groups["W"].Value, out w);

                        var color = new Vector4(x, y, z, w);

                        _gradientStopHdrs.Add(new CanvasGradientStopHdr()
                        {
                            Color = color,
                            Position = position
                        });
                    }

                    var additional = gradientMatch.Groups["Additional"];
                    if (!additional.Success)
                    {
                        continue;
                    }

                    foreach (Capture addCapture in additional.Captures)
                    {
                        var addMatch = RegexFactory.GetAttributesRegex(GradientStopAttributeType.AdditionalHdr)
                                                   .Match(addCapture.Value);
                        float.TryParse(addMatch.Groups["Position"].Value, out position);
                        float.TryParse(addMatch.Groups["X"].Value, out x);
                        float.TryParse(addMatch.Groups["Y"].Value, out y);
                        float.TryParse(addMatch.Groups["Z"].Value, out z);
                        float.TryParse(addMatch.Groups["W"].Value, out w);

                        var color = new Vector4(x, y, z, w);

                        _gradientStopHdrs.Add(new CanvasGradientStopHdr()
                        {
                            Color = color,
                            Position = position
                        });
                    }
                }

                // Sort the stops based on their position
                if (_gradientStopHdrs.Any())
                {
                    _gradientStopHdrs = _gradientStopHdrs.OrderBy(g => g.Position).ToList();
                }
            }
        }
    }
}