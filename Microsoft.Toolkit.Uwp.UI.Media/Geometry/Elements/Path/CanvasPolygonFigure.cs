// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Numerics;
using System.Text.RegularExpressions;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Toolkit.Uwp.UI.Media.Geometry.Core;

namespace Microsoft.Toolkit.Uwp.UI.Media.Geometry.Elements.Path
{
    /// <summary>
    /// Class representing the Polygon Figure in a Path Geometry
    /// </summary>
    internal class CanvasPolygonFigure : AbstractPathElement
    {
        private int _numSides;
        private float _radius;
        private float _x;
        private float _y;

        public CanvasPolygonFigure()
        {
            _numSides = 0;
            _radius = _x = _y = 0;
        }

        /// <summary>
        /// Adds the Path Element to the Path.
        /// </summary>
        /// <param name="pathBuilder">CanvasPathBuilder object</param>
        /// <param name="currentPoint">The last active location in the Path before adding
        /// the PolygonFigure</param>
        /// <param name="lastElement">The previous PathElement in the Path.</param>
        /// <returns>The latest location in the Path after adding the PolygonFigure</returns>
        public override Vector2 CreatePath(CanvasPathBuilder pathBuilder, Vector2 currentPoint, ref ICanvasPathElement lastElement)
        {
            // Calculate coordinates
            var center = new Vector2(_x, _y);
            if (IsRelative)
            {
                center += currentPoint;
            }

            // Execute command
            pathBuilder.AddPolygonFigure(_numSides, center.X, center.Y, _radius);

            // No need to update the lastElement or currentPoint here as we are creating
            // a separate closed figure here. So current point will not change.
            return currentPoint;
        }

        /// <summary>
        /// Get the Regex for extracting Path Element Attributes
        /// </summary>
        /// <returns>Instance of <see cref="Regex"/></returns>
        protected override Regex GetAttributesRegex()
        {
            return RegexFactory.GetAttributesRegex(PathFigureType.PolygonFigure);
        }

        /// <summary>
        /// Gets the Path Element Attributes from the Match
        /// </summary>
        /// <param name="match">Match object</param>
        protected override void GetAttributes(Match match)
        {
            // Sides
            int.TryParse(match.Groups["Sides"].Value, out _numSides);

            // Sanitize by taking the absolute value
            _numSides = Math.Abs(_numSides);

            // Radius
            float.TryParse(match.Groups["Radius"].Value, out _radius);

            // Sanitize by taking the absolute value
            _radius = Math.Abs(_radius);

            float.TryParse(match.Groups["X"].Value, out _x);
            float.TryParse(match.Groups["Y"].Value, out _y);
        }
    }
}