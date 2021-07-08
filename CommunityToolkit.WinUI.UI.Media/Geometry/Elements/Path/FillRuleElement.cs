// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Numerics;
using System.Text.RegularExpressions;
using Microsoft.Graphics.Canvas.Geometry;

namespace CommunityToolkit.WinUI.UI.Media.Geometry.Elements.Path
{
    /// <summary>
    /// Class representing the Fill Rule Element in a Path Geometry
    /// </summary>
    internal class FillRuleElement : AbstractPathElement
    {
        private CanvasFilledRegionDetermination _fillValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="FillRuleElement"/> class.
        /// </summary>
        public FillRuleElement()
        {
            _fillValue = CanvasFilledRegionDetermination.Alternate;
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
            // Do nothing as this scenario is not valid for this Path Element
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
            // Execute command
            pathBuilder.SetFilledRegionDetermination(_fillValue);

            // Set Last Element
            lastElement = this;

            // Return current point
            return currentPoint;
        }

        /// <summary>
        /// Get the Regex for extracting Path Element Attributes
        /// </summary>
        /// <returns>Instance of <see cref="Regex"/></returns>
        protected override Regex GetAttributesRegex()
        {
            // Not applicable for this Path Element
            return null;
        }

        /// <summary>
        /// Gets the Path Element Attributes from the Match
        /// </summary>
        /// <param name="match">Match object</param>
        protected override void GetAttributes(Match match)
        {
            Enum.TryParse(match.Groups["FillValue"].Value, out _fillValue);
        }
    }
}