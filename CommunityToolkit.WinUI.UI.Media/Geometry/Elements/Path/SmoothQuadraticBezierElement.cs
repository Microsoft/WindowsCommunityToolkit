// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using System.Text.RegularExpressions;
using CommunityToolkit.WinUI.UI.Media.Geometry.Core;
using Microsoft.Graphics.Canvas.Geometry;

namespace CommunityToolkit.WinUI.UI.Media.Geometry.Elements.Path
{
    /// <summary>
    /// Class representing the Smooth Quadratic Bezier Element in a Path Geometry
    /// </summary>
    internal class SmoothQuadraticBezierElement : AbstractPathElement
    {
        private float _x;
        private float _y;
        private Vector2 _absoluteControlPoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmoothQuadraticBezierElement"/> class.
        /// </summary>
        public SmoothQuadraticBezierElement()
        {
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
            // Check if the last element was a Quadratic Bezier
            if (lastElement is QuadraticBezierElement quadBezier)
            {
                // Reflect the control point of the Quadratic Bezier over the current point. The
                // resulting point will be the control point of this Bezier.
                _absoluteControlPoint = Utils.Reflect(quadBezier.GetControlPoint(), currentPoint);
            }

            // Or if the last element was s Smooth Quadratic Bezier
            else
            {
                // If the last element was a Smooth Quadratic Bezier then reflect its control point
                // over the current point. The resulting point will be the control point of this
                // Bezier. Otherwise, if the last element was not a Smooth Quadratic Bezier,
                // then the currentPoint will be the control point of this Bezier.
                _absoluteControlPoint = lastElement is SmoothQuadraticBezierElement smoothQuadBezier
                    ? Utils.Reflect(smoothQuadBezier.GetControlPoint(), currentPoint)
                    : currentPoint;
            }

            var point = new Vector2(_x, _y);

            if (IsRelative)
            {
                point += currentPoint;
            }

            // Execute command
            pathBuilder.AddQuadraticBezier(_absoluteControlPoint, point);

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
            return RegexFactory.GetAttributesRegex(PathElementType.SmoothQuadraticBezier);
        }

        /// <summary>
        /// Gets the Path Element Attributes from the Match
        /// </summary>
        /// <param name="match">Match object</param>
        protected override void GetAttributes(Match match)
        {
            float.TryParse(match.Groups["X"].Value, out _x);
            float.TryParse(match.Groups["Y"].Value, out _y);
        }
    }
}