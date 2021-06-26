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
    /// Abstract base class for all Path Elements
    /// </summary>
    internal abstract class AbstractPathElement : ICanvasPathElement
    {
        /// <summary>
        /// Gets or sets index of the Path Element in the Path Data
        /// </summary>
        public int Index { get; protected set; } = -1;

        /// <summary>
        /// Gets or sets path data defining the Path Element
        /// </summary>
        public string Data { get; protected set; } = string.Empty;

        /// <summary>
        /// Gets or sets number of non-whitespace characters in
        /// the Path Element Data
        /// </summary>
        public int ValidationCount { get; protected set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether the path element contains
        /// absolute or relative coordinates.
        /// </summary>
        public bool IsRelative { get; protected set; } = false;

        /// <summary>
        /// Initializes the Path Element with the given Match
        /// </summary>
        /// <param name="match">Match object</param>
        /// <param name="index">Index within the match</param>
        public virtual void Initialize(Match match, int index)
        {
            var main = match.Groups["Main"];
            Index = index;
            Data = main.Value;
            var command = match.Groups["Command"].Value[0];
            IsRelative = char.IsLower(command);

            // Get the Path Element attributes
            GetAttributes(match);

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
        public virtual void InitializeAdditional(Capture capture, int index, bool isRelative)
        {
            Index = index;
            Data = capture.Value;
            IsRelative = isRelative;

            var match = GetAttributesRegex().Match(Data);
            if (match.Captures.Count != 1)
            {
                return;
            }

            // Get the Path Element attributes
            GetAttributes(match);

            // Get the number of non-whitespace characters in the data
            ValidationCount = RegexFactory.ValidationRegex.Replace(Data, string.Empty).Length;
        }

        /// <summary>
        /// Adds the Path Element to the Path.
        /// </summary>
        /// <param name="pathBuilder">CanvasPathBuilder object</param>
        /// <param name="currentPoint">The last active location in the Path before adding
        /// the Path Element</param>
        /// <param name="lastElement">The previous PathElement in the Path.</param>
        /// <returns>The latest location in the Path after adding the Path Element</returns>
        public abstract Vector2 CreatePath(CanvasPathBuilder pathBuilder, Vector2 currentPoint, ref ICanvasPathElement lastElement);

        /// <summary>
        /// Get the Regex for extracting Path Element Attributes
        /// </summary>
        /// <returns>Instance of <see cref="Regex"/></returns>
        protected abstract Regex GetAttributesRegex();

        /// <summary>
        /// Gets the Path Element Attributes from the Match
        /// </summary>
        /// <param name="match">Match object</param>
        protected abstract void GetAttributes(Match match);
    }
}