// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using CommunityToolkit.WinUI.UI.Media.Geometry.Core;
using CommunityToolkit.WinUI.UI.Media.Geometry.Elements.Stroke;
using Microsoft.Graphics.Canvas.Geometry;

namespace CommunityToolkit.WinUI.UI.Media.Geometry.Parsers
{
    /// <summary>
    /// Parser for the CanvasStrokeStyle
    /// </summary>
    internal static class CanvasStrokeStyleParser
    {
        /// <summary>
        /// Parses the given style data and converts it to CanvasStrokeStyle.
        /// </summary>
        /// <param name="styleData">Style data</param>
        /// <returns>CanvasStrokeStyle</returns>
        internal static CanvasStrokeStyle Parse(string styleData)
        {
            var matches = RegexFactory.CanvasStrokeStyleRegex.Matches(styleData);

            // If no match is found or no captures in the match, then it means that the style data is invalid.
            if (matches.Count == 0)
            {
                ThrowForZeroCount();
            }

            // If the match contains more than one captures, it means that there
            // are multiple CanvasStrokeStyles present in the CanvasStrokeStyle data. There should
            // be only one CanvasStrokeStyle defined in the CanvasStrokeStyle data.
            if (matches.Count > 1)
            {
                ThrowForNotOneCount();
            }

            // There should be only one match
            var match = matches[0];
            var styleElement = new CanvasStrokeStyleElement(match);

            // Perform validation to check if there are any invalid characters in the brush data that were not captured
            var preValidationCount = RegexFactory.ValidationRegex.Replace(styleData, string.Empty).Length;
            var postValidationCount = styleElement.ValidationCount;

            // If there are invalid characters, extract them and add them to the ArgumentException message
            if (preValidationCount != postValidationCount)
            {
                [MethodImpl(MethodImplOptions.NoInlining)]
                static void ThrowForInvalidCharacters(CanvasStrokeStyleElement styleElement, string styleData)
                {
                    var parseIndex = 0;
                    if (!string.IsNullOrWhiteSpace(styleElement.Data))
                    {
                        parseIndex = styleData.IndexOf(styleElement.Data, parseIndex, StringComparison.Ordinal);
                    }

                    var errorString = styleData.Substring(parseIndex);
                    if (errorString.Length > 30)
                    {
                        errorString = $"{errorString.Substring(0, 30)}...";
                    }

                    throw new ArgumentException($"STYLE_ERR003:Style data contains invalid characters!\nIndex: {parseIndex}\n{errorString}");
                }

                ThrowForInvalidCharacters(styleElement, styleData);
            }

            return styleElement.Style;

            static void ThrowForZeroCount() => throw new ArgumentException("STYLE_ERR001:Invalid CanvasStrokeStyle data! No matching CanvasStrokeStyle found!");
            static void ThrowForNotOneCount() => throw new ArgumentException("STYLE_ERR002:Multiple CanvasStrokeStyles defined in CanvasStrokeStyle Data! " +
                                                                             "There should be only one CanvasStrokeStyle definition within the CanvasStrokeStyle Data. " +
                                                                             "You can either remove CanvasStrokeStyle definitions or split the CanvasStrokeStyle Data " +
                                                                             "into multiple CanvasStrokeStyle Data and call the CanvasPathGeometry.CreateStrokeStyle() method on each of them.");
        }

        /// <summary>
        /// Parses and constructs a ICanvasStrokeStyleElement from the specified Match object.
        /// </summary>
        /// <param name="match">Match object</param>
        /// <returns>ICanvasStrokeStyleElement</returns>
        internal static ICanvasStrokeStyleElement Parse(Match match)
        {
            return new CanvasStrokeStyleElement(match);
        }
    }
}