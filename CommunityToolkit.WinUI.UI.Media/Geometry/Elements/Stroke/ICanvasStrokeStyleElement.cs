// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using Microsoft.Graphics.Canvas.Geometry;

namespace CommunityToolkit.WinUI.UI.Media.Geometry.Elements.Stroke
{
    /// <summary>
    /// Interface for the CanvasStrokeStyle Element
    /// </summary>
    internal interface ICanvasStrokeStyleElement
    {
        /// <summary>
        /// Gets the Stroke data defining the Brush Element.
        /// </summary>
        string Data { get; }

        /// <summary>
        /// Gets the number of non-whitespace characters in the Stroke Data.
        /// </summary>
        int ValidationCount { get; }

        /// <summary>
        /// Gets the CanvasStrokeStyle obtained by parsing the style data.
        /// </summary>
        CanvasStrokeStyle Style { get; }

        /// <summary>
        /// Initializes the Stroke Element with the given Capture.
        /// </summary>
        /// <param name="match">Match object</param>
        void Initialize(Match match);
    }
}