// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Numerics;
using System.Text.RegularExpressions;
using CommunityToolkit.WinUI.UI.Media.Geometry.Core;
using Microsoft.Graphics.Canvas.Geometry;

namespace CommunityToolkit.WinUI.UI.Media.Geometry.Elements.Path
{
    /// <summary>
    /// Class representing the Ellipse Figure in a Path Geometry
    /// </summary>
    internal class CanvasEllipseFigure : AbstractPathElement
    {
        private float _radiusX;
        private float _radiusY;
        private float _x;
        private float _y;

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasEllipseFigure"/> class.
        /// </summary>
        public CanvasEllipseFigure()
        {
            _radiusX = _radiusY = _x = _y = 0;
        }

        /// <summary>
        /// Adds the Path Element to the Path.
        /// </summary>
        /// <param name="pathBuilder">CanvasPathBuilder object</param>
        /// <param name="currentPoint">The last active location in the Path before adding
        /// the EllipseFigure</param>
        /// <param name="lastElement">The previous PathElement in the Path.</param>
        /// <returns>The latest location in the Path after adding the EllipseFigure</returns>
        public override Vector2 CreatePath(CanvasPathBuilder pathBuilder, Vector2 currentPoint, ref ICanvasPathElement lastElement)
        {
            // Calculate coordinates
            var center = new Vector2(_x, _y);
            if (IsRelative)
            {
                center += currentPoint;
            }

            // Execute command
            pathBuilder.AddEllipseFigure(center.X, center.Y, _radiusX, _radiusY);

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
            return RegexFactory.GetAttributesRegex(PathFigureType.EllipseFigure);
        }

        /// <summary>
        /// Gets the Path Element Attributes from the Match
        /// </summary>
        /// <param name="match">Match object</param>
        protected override void GetAttributes(Match match)
        {
            float.TryParse(match.Groups["RadiusX"].Value, out _radiusX);

            // Sanitize by taking the absolute value
            _radiusX = Math.Abs(_radiusX);
            float.TryParse(match.Groups["RadiusY"].Value, out _radiusY);

            // Sanitize by taking the absolute value
            _radiusY = Math.Abs(_radiusY);
            float.TryParse(match.Groups["X"].Value, out _x);
            float.TryParse(match.Groups["Y"].Value, out _y);
        }
    }
}