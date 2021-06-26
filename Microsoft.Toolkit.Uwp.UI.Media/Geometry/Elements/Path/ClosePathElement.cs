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
    /// Class representing the ClosePath command in a Path Geometry
    /// </summary>
    internal class ClosePathElement : AbstractPathElement
    {
        private bool _isFigureClosed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClosePathElement"/> class.
        /// </summary>
        public ClosePathElement()
        {
            _isFigureClosed = false;
        }

        /// <summary>
        /// Initializes the Path Element with the given Match
        /// </summary>
        /// <param name="match">Match object</param>
        /// <param name="index">Index of the Path Element in the Path data.</param>
        public override void Initialize(Match match, int index)
        {
            var main = match.Groups["Main"];
            Index = index;
            Data = main.Value;
            var command = match.Groups["Command"];

            // If the Command is captured, it means that 'Z' is provided and hence
            // the figure must be closed
            if (command.Captures.Count == 1)
            {
                _isFigureClosed = true;
            }

            // Get the number of non-whitespace characters in the data
            ValidationCount = RegexFactory.ValidationRegex.Replace(main.Value, string.Empty).Length;
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
            pathBuilder.EndFigure(_isFigureClosed ? CanvasFigureLoop.Closed : CanvasFigureLoop.Open);

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
            // Attributes are not present
            return null;
        }

        /// <summary>
        /// Gets the Path Element Attributes from the Match
        /// </summary>
        /// <param name="match">Match object</param>
        protected override void GetAttributes(Match match)
        {
            // Do Nothing
        }
    }
}