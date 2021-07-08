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
    /// Class representing the MoveTo Command in a Path Geometry
    /// </summary>
    internal class MoveToElement : AbstractPathElement
    {
        private float _x;
        private float _y;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveToElement"/> class.
        /// </summary>
        public MoveToElement()
        {
            _x = _y = 0;
        }

        /// <summary>
        /// Initializes the Path Element with the given Capture
        /// </summary>
        /// <param name="capture">Capture object</param>
        /// <param name="index">Index of the Path Element in the Path data.</param>
        /// <param name="isRelative">Indicates whether the Path Element coordinates are
        /// absolute or relative</param>
        public override void InitializeAdditional(Capture capture, int index, bool isRelative)
        {
            // Do nothing as this scenario is not valid for MoveTo Command
            // Additional coordinates specified with MoveTo will be converted to Line Commands
        }

        /// <summary>
        /// Adds the Path Element to the PathBuilder.
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
            pathBuilder.BeginFigure(point);

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
            return RegexFactory.GetAttributesRegex(PathElementType.MoveTo);
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