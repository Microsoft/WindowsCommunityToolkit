// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using CommunityToolkit.WinUI.UI.Media.Geometry.Core;
using Microsoft.Graphics.Canvas.Geometry;

namespace CommunityToolkit.WinUI.UI.Media.Geometry.Elements.Path
{
    /// <summary>
    /// Class which contains a collection of ICanvasPathElements
    /// which can be used to create CanvasGeometry.
    /// </summary>
    internal class CanvasPathFigure : AbstractPathElement
    {
        // Collection of Path Elements
        private List<ICanvasPathElement> _elements;

        public CanvasPathFigure()
        {
            _elements = new List<ICanvasPathElement>();
            ValidationCount = 0;
        }

        /// <summary>
        /// Initializes the Path Element with the given Match
        /// </summary>
        /// <param name="match">Match object</param>
        /// <param name="index">Index of the path element in the Path data.</param>
        public override void Initialize(Match match, int index)
        {
            Index = index;
            var main = match.Groups["Main"];
            Data = main.Value;

            var elements = new List<ICanvasPathElement>();
            foreach (PathElementType type in Enum.GetValues(typeof(PathElementType)))
            {
                foreach (Capture elementCapture in match.Groups[type.ToString()].Captures)
                {
                    var elementRootIndex = elementCapture.Index;
                    var regex = RegexFactory.GetRegex(type);
                    var elementMatch = regex.Match(elementCapture.Value);
                    var isRelative = false;

                    // Process the 'Main' Group which contains the Path Command and
                    // corresponding attributes
                    if (elementMatch.Groups["Main"].Captures.Count == 1)
                    {
                        var figure = PathElementFactory.CreatePathElement(type, elementMatch, elementRootIndex);
                        elements.Add(figure);
                        isRelative = figure.IsRelative;
                    }

                    // Process the 'Additional' Group which contains just the attributes
                    elements.AddRange(from Capture capture in elementMatch.Groups["Additional"].Captures
                                      select PathElementFactory.CreateAdditionalPathElement(type, capture, elementRootIndex + capture.Index, isRelative));
                }
            }

            // Sort the path elements based on their index value
            _elements.AddRange(elements.OrderBy(e => e.Index));
            if (_elements.Count <= 0)
            {
                return;
            }

            // Check if the last path element in the figure is an ClosePathElement
            // which would indicate that the path needs to be closed. Otherwise,
            // add a default ClosePathElement at the end to indicate that the path
            // is not closed.
            var lastElement = _elements.ElementAt(_elements.Count - 1);
            if ((lastElement as ClosePathElement) == null)
            {
                _elements.Add(PathElementFactory.CreateDefaultPathElement(PathElementType.ClosePath));
            }

            // Validation Count will be the cumulative sum of the validation count
            // of child elements of the PathFigure
            ValidationCount = _elements.Sum(x => x.ValidationCount);
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
            // Do nothing as this scenario is not valid for CanvasPathFigure
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
            foreach (var pathElement in _elements)
            {
                currentPoint = pathElement.CreatePath(pathBuilder, currentPoint, ref lastElement);
            }

            return currentPoint;
        }

        /// <summary>
        /// Get the Regex for extracting Path Element Attributes
        /// </summary>
        /// <returns>Instance of <see cref="Regex"/></returns>
        protected override Regex GetAttributesRegex()
        {
            return null;
        }

        /// <summary>
        /// Gets the Path Element Attributes from the Match
        /// </summary>
        /// <param name="match">Match object</param>
        protected override void GetAttributes(Match match)
        {
            // Do nothing
        }
    }
}