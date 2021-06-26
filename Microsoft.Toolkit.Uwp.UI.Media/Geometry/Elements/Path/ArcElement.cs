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
    /// Class representing the Arc Element in a Path Geometry
    /// </summary>
    internal class ArcElement : AbstractPathElement
    {
        private float _radiusX;
        private float _radiusY;
        private float _angle;
        private CanvasArcSize _arcSize;
        private CanvasSweepDirection _sweepDirection;
        private float _x;
        private float _y;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArcElement"/> class.
        /// </summary>
        public ArcElement()
        {
            _radiusX = _radiusY = _angle = 0;
            _arcSize = CanvasArcSize.Small;
            _sweepDirection = CanvasSweepDirection.Clockwise;
            _sweepDirection = 0;
            _x = _y = 0;
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
            var point = new Vector2(_x, _y);
            if (IsRelative)
            {
                point += currentPoint;
            }

            // Execute command
            pathBuilder.AddArc(point, _radiusX, _radiusY, _angle, _sweepDirection, _arcSize);

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
            return RegexFactory.GetAttributesRegex(PathElementType.Arc);
        }

        /// <summary>
        /// Gets the Path Element Attributes from the Match
        /// </summary>
        /// <param name="match">Match object</param>
        protected override void GetAttributes(Match match)
        {
            float.TryParse(match.Groups["RadiusX"].Value, out _radiusX);

            // Sanitize by taking only the absolute value
            _radiusX = Math.Abs(_radiusX);
            float.TryParse(match.Groups["RadiusY"].Value, out _radiusY);

            // Sanitize by taking only the absolute value
            _radiusY = Math.Abs(_radiusY);

            // Angle is provided in degrees
            float.TryParse(match.Groups["Angle"].Value, out _angle);

            // Convert angle to radians as CanvasPathBuilder.AddArc() method
            // requires the angle to be in radians
            _angle *= Scalar.DegreesToRadians;
            Enum.TryParse(match.Groups["IsLargeArc"].Value, out _arcSize);
            Enum.TryParse(match.Groups["SweepDirection"].Value, out _sweepDirection);
            float.TryParse(match.Groups["X"].Value, out _x);
            float.TryParse(match.Groups["Y"].Value, out _y);
        }
    }
}