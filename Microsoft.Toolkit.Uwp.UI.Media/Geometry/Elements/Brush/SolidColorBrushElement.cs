// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Toolkit.Uwp.UI.Media.Geometry.Core;
using Microsoft.Toolkit.Uwp.UI.Media.Geometry.Parsers;
using Windows.UI;

namespace Microsoft.Toolkit.Uwp.UI.Media.Geometry.Elements.Brush
{
    /// <summary>
    /// Represents a CanvasSolidColorBrush
    /// </summary>
    internal sealed class SolidColorBrushElement : AbstractCanvasBrushElement
    {
        private Color _color;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolidColorBrushElement"/> class.
        /// </summary>
        /// <param name="capture">Capture object</param>
        public SolidColorBrushElement(Capture capture)
        {
            // Set the default values
            _color = Colors.Transparent;
            _opacity = 1f;

            Initialize(capture);
        }

        /// <summary>
        /// Creates the ICanvasBrush from the parsed data
        /// </summary>
        /// <param name="resourceCreator">ICanvasResourceCreator object</param>
        /// <returns>Instance of <see cref="ICanvasBrush"/></returns>
        public override ICanvasBrush CreateBrush(ICanvasResourceCreator resourceCreator)
        {
            return new CanvasSolidColorBrush(resourceCreator, _color)
            {
                Opacity = _opacity
            };
        }

        /// <summary>
        /// Gets the Regex for extracting Brush Element Attributes
        /// </summary>
        /// <returns>Regex</returns>
        protected override Regex GetAttributesRegex()
        {
            return RegexFactory.GetAttributesRegex(BrushType.SolidColor);
        }

        /// <summary>
        /// Gets the Brush Element Attributes from the Match
        /// </summary>
        /// <param name="match">Match object</param>
        protected override void GetAttributes(Match match)
        {
            // Parse the Color
            _color = ColorParser.Parse(match);

            // Opacity (optional)
            var group = match.Groups["Opacity"];
            if (group.Success)
            {
                float.TryParse(group.Value, out _opacity);
            }
        }
    }
}