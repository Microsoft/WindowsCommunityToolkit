// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using System.Text.RegularExpressions;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Toolkit.Uwp.UI.Media.Geometry.Core;

namespace Microsoft.Toolkit.Uwp.UI.Media.Geometry.Elements.Path
{
    /// <summary>
    /// Class representing the Horizontal Line Element in a Path Geometry
    /// </summary>
    internal class HorizontalLineElement : AbstractPathElement
    {
        private float _x;

        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalLineElement"/> class.
        /// </summary>
        public HorizontalLineElement()
        {
            _x = 0;
        }

        /// <summary>
        /// Adds the Path Element to the Path.
        /// </summary>
        /// <param name="pathBuilder">CanvasPathBuilder object</param>
        /// <param name="currentPoint">The last active location in the Path before adding
        /// the Path Element</param>
        /// <param name="lastElement">The previous PathElement in the Path.</param>
        /// <returns>The latest location in the Path after adding the Path Element</returns>
        public override Vector2 CreatePath(CanvasPathBuilder pathBuilder, Vector2 currentPoint, ref ICanvasPathElement lastElement)
        {
            // Calculate coordinates
            var point = IsRelative ?
                new Vector2(currentPoint.X + _x, currentPoint.Y) : new Vector2(_x, currentPoint.Y);

            // Execute command
            pathBuilder.AddLine(point);

            // Set Last Element
            lastElement = this;

            // Return current point
            return point;
        }

        /// <summary>
        /// Get the Regex for extracting Path Element Attributes
        /// </summary>
        /// <returns>Instance of <see cref="Regex"/></returns>
        protected override Regex GetAttributesRegex()
        {
            return RegexFactory.GetAttributesRegex(PathElementType.HorizontalLine);
        }

        /// <summary>
        /// Gets the Path Element Attributes from the Match
        /// </summary>
        /// <param name="match">Match object</param>
        protected override void GetAttributes(Match match)
        {
            float.TryParse(match.Groups["X"].Value, out _x);
        }
    }
}