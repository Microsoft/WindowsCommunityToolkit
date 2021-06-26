// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using Microsoft.Graphics.Canvas;

namespace Microsoft.Toolkit.Uwp.UI.Media.Geometry.Elements.Stroke
{
    /// <summary>
    /// Interface for Stroke Element
    /// </summary>
    internal interface ICanvasStrokeElement
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
        /// Initializes the Stroke Element with the given Capture.
        /// </summary>
        /// <param name="match">Match object</param>
        void Initialize(Match match);

        /// <summary>
        /// Creates the ICanvasStroke from the parsed data.
        /// </summary>
        /// <returns>ICanvasStroke</returns>
        ICanvasStroke CreateStroke(ICanvasResourceCreator resourceCreator);
    }
}