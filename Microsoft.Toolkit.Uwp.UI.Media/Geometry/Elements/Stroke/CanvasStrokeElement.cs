// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text.RegularExpressions;
using Microsoft.Graphics.Canvas;
using Microsoft.Toolkit.Uwp.UI.Media.Geometry.Core;
using Microsoft.Toolkit.Uwp.UI.Media.Geometry.Elements.Brush;
using Microsoft.Toolkit.Uwp.UI.Media.Geometry.Parsers;

namespace Microsoft.Toolkit.Uwp.UI.Media.Geometry.Elements.Stroke
{
    /// <summary>
    /// Represents a Stroke Element.
    /// </summary>
    internal sealed class CanvasStrokeElement : AbstractCanvasStrokeElement
    {
        private float _width;
        private ICanvasBrushElement _brush;
        private ICanvasStrokeStyleElement _style;
        private int _widthValidationCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasStrokeElement"/> class.
        /// </summary>
        /// <param name="match">Match object</param>
        public CanvasStrokeElement(Match match)
        {
            _width = 1f;
            _brush = null;
            _style = null;
            _widthValidationCount = 0;

            Initialize(match);
        }

        /// <summary>
        /// Creates the ICanvasStroke from the parsed data.
        /// </summary>
        /// <returns>ICanvasStroke</returns>
        public override ICanvasStroke CreateStroke(ICanvasResourceCreator resourceCreator)
        {
            return new CanvasStroke(_brush.CreateBrush(resourceCreator), _width, _style.Style);
        }

        /// <summary>
        /// Gets the Stroke Element Attributes from the Match.
        /// </summary>
        /// <param name="match">Match object</param>
        protected override void GetAttributes(Match match)
        {
            // Stroke Width
            var group = match.Groups["StrokeWidth"];
            float.TryParse(group.Value, out _width);

            // Sanitize by taking the absolute value
            _width = Math.Abs(_width);

            _widthValidationCount = RegexFactory.ValidationRegex.Replace(group.Value, string.Empty).Length;

            // Stroke Brush
            group = match.Groups["CanvasBrush"];
            if (group.Success)
            {
                _brush = CanvasBrushParser.Parse(group.Value);
            }

            // If the ICanvasBrushElement was not created, then the ICanvasStroke cannot be created
            if (_brush == null)
            {
                static void Throw(string value) => throw new ArgumentException($"Unable to create a valid ICanvasBrush for the ICanvasStroke with the following Brush data - '{value}'.");

                Throw(group.Value);
            }

            // Stroke Style
            _style = CanvasStrokeStyleParser.Parse(match);
        }

        /// <summary>
        /// Gets the number of non-whitespace characters in the data.
        /// </summary>
        protected override void Validate()
        {
            // Add 2 to the Validation Count to include the stroke command 'ST'
            ValidationCount += 2;

            // StrokeWidth Validation Count
            ValidationCount += _widthValidationCount;

            // Stroke Brush Validation Count
            if (_brush != null)
            {
                ValidationCount += _brush.ValidationCount;
            }

            // Stroke Style Validation Count
            if (_style != null)
            {
                ValidationCount += _style.ValidationCount;
            }
        }
    }
}