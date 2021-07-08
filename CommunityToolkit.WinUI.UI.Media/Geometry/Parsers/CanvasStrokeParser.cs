// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using CommunityToolkit.WinUI.UI.Media.Geometry.Core;
using CommunityToolkit.WinUI.UI.Media.Geometry.Elements.Stroke;
using Microsoft.Graphics.Canvas;

namespace CommunityToolkit.WinUI.UI.Media.Geometry.Parsers
{
    /// <summary>
    /// Parser for CanvasStroke
    /// </summary>
    internal static class CanvasStrokeParser
    {
        /// <summary>
        /// Parses the Stroke Data string and converts it into ICanvasStrokeElement.
        /// </summary>
        /// <param name="strokeData">Stroke Data string</param>
        /// <returns>ICanvasStrokeElement</returns>
        internal static ICanvasStrokeElement Parse(string strokeData)
        {
            var matches = RegexFactory.CanvasStrokeRegex.Matches(strokeData);

            // If no match is found or no captures in the match, then it means
            // that the stroke data is invalid.
            if (matches.Count == 0)
            {
                ThrowForZeroCount();
            }

            // If the match contains more than one captures, it means that there
            // are multiple CanvasStrokes present in the stroke data. There should
            // be only one CanvasStroke defined in the stroke data.
            if (matches.Count > 1)
            {
                ThrowForNotOneCount();
            }

            // There should be only one match
            var match = matches[0];
            var strokeElement = new CanvasStrokeElement(match);

            // Perform validation to check if there are any invalid characters in the stroke data that were not captured
            var preValidationCount = RegexFactory.ValidationRegex.Replace(strokeData, string.Empty).Length;
            var postValidationCount = strokeElement.ValidationCount;

            // If there are invalid characters, extract them and add them to the ArgumentException message
            if (preValidationCount != postValidationCount)
            {
                [MethodImpl(MethodImplOptions.NoInlining)]
                static void ThrowForInvalidCharacters(CanvasStrokeElement strokeElement, string strokeData)
                {
                    var parseIndex = 0;
                    if (!string.IsNullOrWhiteSpace(strokeElement.Data))
                    {
                        parseIndex = strokeData.IndexOf(strokeElement.Data, parseIndex, StringComparison.Ordinal);
                    }

                    var errorString = strokeData.Substring(parseIndex);
                    if (errorString.Length > 30)
                    {
                        errorString = $"{errorString.Substring(0, 30)}...";
                    }

                    throw new ArgumentException($"STROKE_ERR003:Stroke data contains invalid characters!\nIndex: {parseIndex}\n{errorString}");
                }

                ThrowForInvalidCharacters(strokeElement, strokeData);
            }

            return strokeElement;

            static void ThrowForZeroCount() => throw new ArgumentException("STROKE_ERR001:Invalid Stroke data! No matching CanvasStroke found!");
            static void ThrowForNotOneCount() => throw new ArgumentException("STROKE_ERR002:Multiple CanvasStrokes defined in Stroke Data! " +
                                                                             "There should be only one CanvasStroke definition within the Stroke Data. " +
                                                                             "You can either remove CanvasStroke definitions or split the Stroke Data " +
                                                                             "into multiple Stroke Data and call the CanvasPathGeometry.CreateStroke() method on each of them.");
        }

        /// <summary>
        /// Parses the Stroke Data string and converts it into CanvasStroke.
        /// </summary>
        /// <param name="resourceCreator">ICanvasResourceCreator</param>
        /// <param name="strokeData">Stroke Data string</param>
        /// <returns>ICanvasStroke</returns>
        internal static ICanvasStroke Parse(ICanvasResourceCreator resourceCreator, string strokeData)
        {
            // Parse the stroke data to create the ICanvasStrokeElement
            var strokeElement = Parse(strokeData);

            // Create the CanvasStroke from the strokeElement
            return strokeElement.CreateStroke(resourceCreator);
        }
    }
}