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
    /// Class representing the Quadratic Bezier Element in a Path Geometry
    /// </summary>
    internal class QuadraticBezierElement : AbstractPathElement
    {
        private float _x1;
        private float _y1;
        private float _x;
        private float _y;
        private Vector2 _absoluteControlPoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuadraticBezierElement"/> class.
        /// </summary>
        public QuadraticBezierElement()
        {
            _x1 = _y1 = 0;
            _x = _y = 0;
            _absoluteControlPoint = Vector2.Zero;
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
            var controlPoint = new Vector2(_x1, _y1);
            var point = new Vector2(_x, _y);

            if (IsRelative)
            {
                controlPoint += currentPoint;
                point += currentPoint;
            }

            // Save the absolute control point so that it can be used by the following
            // SmoothQuadraticBezierElement (if any)
            _absoluteControlPoint = controlPoint;

            // Execute command
            pathBuilder.AddQuadraticBezier(controlPoint, point);

            // Set Last Element
            lastElement = this;

            // Return current point
            return point;
        }

        /// <summary>
        /// Gets the Control Point of this Quadratic Bezier
        /// </summary>
        /// <returns>Vector2</returns>
        public Vector2 GetControlPoint()
        {
            return _absoluteControlPoint;
        }

        /// <summary>
        /// Get the Regex for extracting Path Element Attributes
        /// </summary>
        /// <returns>Instance of <see cref="Regex"/></returns>
        protected override Regex GetAttributesRegex()
        {
            return RegexFactory.GetAttributesRegex(PathElementType.QuadraticBezier);
        }

        /// <summary>
        /// Gets the Path Element Attributes from the Match
        /// </summary>
        /// <param name="match">Match object</param>
        protected override void GetAttributes(Match match)
        {
            float.TryParse(match.Groups["X1"].Value, out _x1);
            float.TryParse(match.Groups["Y1"].Value, out _y1);
            float.TryParse(match.Groups["X"].Value, out _x);
            float.TryParse(match.Groups["Y"].Value, out _y);
        }
    }
}