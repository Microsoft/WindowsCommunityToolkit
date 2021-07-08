// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using CommunityToolkit.WinUI.UI.Media.Geometry.Core;
using CommunityToolkit.WinUI.UI.Media.Geometry.Elements.Path;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;

namespace CommunityToolkit.WinUI.UI.Media.Geometry.Parsers
{
    /// <summary>
    /// Parser for CanvasGeometry.
    /// </summary>
    internal static class CanvasGeometryParser
    {
        /// <summary>
        /// Parses the Path data in string format and converts it to <see cref="CanvasGeometry"/>.
        /// </summary>
        /// <param name="resourceCreator">ICanvasResourceCreator</param>
        /// <param name="pathData">Path data</param>
        /// <returns><see cref="CanvasGeometry"/></returns>
        internal static CanvasGeometry Parse(ICanvasResourceCreator resourceCreator, string pathData)
        {
            var pathFigures = new List<ICanvasPathElement>();

            var matches = RegexFactory.CanvasGeometryRegex.Matches(pathData);

            // If no match is found or no captures in the match, then it means // that the path data is invalid.
            if (matches.Count == 0)
            {
                ThrowForZeroCount();
            }

            // If the match contains more than one captures, it means that there are multiple FillRuleElements present in the path data.
            // There can be only one FillRuleElement in the path data (at the beginning).
            if (matches.Count > 1)
            {
                ThrowForNotOneCount();
            }

            var figures = new List<ICanvasPathElement>();

            foreach (PathFigureType type in Enum.GetValues(typeof(PathFigureType)))
            {
                foreach (Capture figureCapture in matches[0].Groups[type.ToString()].Captures)
                {
                    var figureRootIndex = figureCapture.Index;
                    var regex = RegexFactory.GetRegex(type);
                    var figureMatch = regex.Match(figureCapture.Value);
                    if (!figureMatch.Success)
                    {
                        continue;
                    }

                    // Process the 'Main' Group which contains the Path Command and
                    // corresponding attributes
                    var figure = PathElementFactory.CreatePathFigure(type, figureMatch, figureRootIndex);
                    figures.Add(figure);

                    // Process the 'Additional' Group which contains just the attributes
                    figures.AddRange(from Capture capture in figureMatch.Groups["Additional"].Captures
                                     select PathElementFactory.CreateAdditionalPathFigure(type, capture, figureRootIndex + capture.Index, figure.IsRelative));
                }
            }

            // Sort the figures by their indices
            pathFigures.AddRange(figures.OrderBy(f => f.Index));
            if (pathFigures.Count > 0)
            {
                // Check if the first element in the _figures list is a FillRuleElement
                // which would indicate the fill rule to be followed while creating the
                // path. If it is not present, then insert a default FillRuleElement at
                // the beginning.
                if ((pathFigures.ElementAt(0) as FillRuleElement) == null)
                {
                    pathFigures.Insert(0, PathElementFactory.CreateDefaultPathElement(PathFigureType.FillRule));
                }
            }
            else
            {
                return null;
            }

            // Perform validation to check if there are any invalid characters in the path data that were not captured
            var preValidationCount = RegexFactory.ValidationRegex.Replace(pathData, string.Empty).Length;
            var postValidationCount = pathFigures.Sum(x => x.ValidationCount);

            // If there are invalid characters, extract them and add them to the ArgumentException message
            if (preValidationCount != postValidationCount)
            {
                [MethodImpl(MethodImplOptions.NoInlining)]
                static void ThrowForInvalidCharacters(List<ICanvasPathElement> pathFigures, string pathData)
                {
                    var parseIndex = 0;
                    foreach (var figure in pathFigures)
                    {
                        parseIndex = pathData.IndexOf(figure.Data, parseIndex, StringComparison.Ordinal) + figure.Data.Length;
                    }

                    var errorString = pathData.Substring(parseIndex);
                    if (errorString.Length > 30)
                    {
                        errorString = $"{errorString.Substring(0, 30)}...";
                    }

                    throw new ArgumentException($"PATH_ERR003:Path data contains invalid characters!\nIndex: {parseIndex}\n{errorString}");
                }

                ThrowForInvalidCharacters(pathFigures, pathData);
            }

            ICanvasPathElement lastElement = null;
            var currentPoint = Vector2.Zero;

            using var pathBuilder = new CanvasPathBuilder(resourceCreator);
            foreach (var pathFigure in pathFigures)
            {
                currentPoint = pathFigure.CreatePath(pathBuilder, currentPoint, ref lastElement);
            }

            return CanvasGeometry.CreatePath(pathBuilder);

            static void ThrowForZeroCount() => throw new ArgumentException("PATH_ERR000:Invalid Path data! No matching path data found!");
            static void ThrowForNotOneCount() => throw new ArgumentException("PATH_ERR001:Multiple FillRule elements present in Path Data!\n" +
                                                                             "There should be only one FillRule within the Path Data. " +
                                                                             "You can either remove additional FillRule elements or split the Path Data " +
                                                                             "into multiple Path Data and call the CanvasPathGeometry.CreateGeometry() method on each of them.");
        }
    }
}