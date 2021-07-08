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
    /// Class representing the Smooth Cubic Bezier Element in a Path Geometry
    /// </summary>
    internal class SmoothCubicBezierElement : AbstractPathElement
    {
        private float _x2;
        private float _y2;
        private float _x;
        private float _y;
        private Vector2 _absoluteControlPoint2;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmoothCubicBezierElement"/> class.
        /// </summary>
        public SmoothCubicBezierElement()
        {
            _x2 = _y2 = 0;
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
            // Check if the last element was a Cubic Bezier
            Vector2 controlPoint1;
            if (lastElement is CubicBezierElement cubicBezier)
            {
                // Reflect the second control point of the cubic bezier over the current point. The
                // resulting point will be the first control point of this Bezier.
                controlPoint1 = Utils.Reflect(cubicBezier.GetControlPoint(), currentPoint);
            }

            // Or if the last element was s Smooth Cubic Bezier
            else
            {
                // If the last element was a Smooth Cubic Bezier then reflect its second control point
                // over the current point. The resulting point will be the first control point of this
                // Bezier. Otherwise, if the last element was not a Smooth Cubic Bezier then the
                // currentPoint will be the first control point of this Bezier
                controlPoint1 = lastElement is SmoothCubicBezierElement smoothCubicBezier
                    ? Utils.Reflect(smoothCubicBezier.GetControlPoint(), currentPoint)
                    : currentPoint;
            }

            var controlPoint2 = new Vector2(_x2, _y2);
            var point = new Vector2(_x, _y);

            if (IsRelative)
            {
                controlPoint2 += currentPoint;
                point += currentPoint;
            }

            // Save the second absolute control point so that it can be used by the following
            // SmoothCubicBezierElement (if any)
            _absoluteControlPoint2 = controlPoint2;

            // Execute command
            pathBuilder.AddCubicBezier(controlPoint1, controlPoint2, point);

            // Set Last Element
            lastElement = this;

            // Return current point
            return point;
        }

        /// <summary>
        /// Gets the Second Control Point of this Cubic Bezier
        /// </summary>
        /// <returns>Vector2</returns>
        public Vector2 GetControlPoint()
        {
            return _absoluteControlPoint2;
        }

        /// <summary>
        /// Get the Regex for extracting Path Element Attributes
        /// </summary>
        /// <returns>Instance of <see cref="Regex"/></returns>
        protected override Regex GetAttributesRegex()
        {
            return RegexFactory.GetAttributesRegex(PathElementType.SmoothCubicBezier);
        }

        /// <summary>
        /// Gets the Path Element Attributes from the Match
        /// </summary>
        /// <param name="match">Match object</param>
        protected override void GetAttributes(Match match)
        {
            float.TryParse(match.Groups["X2"].Value, out _x2);
            float.TryParse(match.Groups["Y2"].Value, out _y2);
            float.TryParse(match.Groups["X"].Value, out _x);
            float.TryParse(match.Groups["Y"].Value, out _y);
        }
    }
}