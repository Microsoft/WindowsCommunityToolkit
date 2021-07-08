// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Graphics.Canvas.Geometry;

namespace CommunityToolkit.WinUI.UI.Media.Geometry.Elements.Path
{
    /// <summary>
    /// Interface for a Path Element which serves
    /// as a building block for CanvasPathGeometry
    /// </summary>
    internal interface ICanvasPathElement
    {
        /// <summary>
        /// Gets index of the Path Element in the Path Data
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Gets path data defining the Path Element
        /// </summary>
        string Data { get; }

        /// <summary>
        /// Gets number of non-whitespace characters in
        /// the Path Element Data
        /// </summary>
        int ValidationCount { get; }

        /// <summary>
        /// Gets a value indicating whether the path element contains
        /// absolute or relative coordinates.
        /// </summary>
        bool IsRelative { get; }

        /// <summary>
        /// Initializes the Path Element with the given Match
        /// </summary>
        /// <param name="match">Match object</param>
        /// <param name="index">Index of the Path Element in the Path data.</param>
        void Initialize(Match match, int index);

        /// <summary>
        /// Initializes the Path Element with the given Capture
        /// </summary>
        /// <param name="capture">Capture object</param>
        /// <param name="index">Index of the Path Element in the Path data.</param>
        /// <param name="isRelative">Indicates whether the Path Element coordinates are
        /// absolute or relative</param>
        void InitializeAdditional(Capture capture, int index, bool isRelative);

        /// <summary>
        /// Adds the Path Element to the PathBuilder.
        /// </summary>
        /// <param name="pathBuilder">CanvasPathBuilder object</param>
        /// <param name="currentPoint">The current point on the path before the path element is added</param>
        /// <param name="lastElement">The previous PathElement in the Path.</param>
        /// <returns>The current point on the path after the path element is added</returns>
        Vector2 CreatePath(CanvasPathBuilder pathBuilder, Vector2 currentPoint, ref ICanvasPathElement lastElement);
    }
}