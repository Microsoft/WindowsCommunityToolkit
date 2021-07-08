// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text.RegularExpressions;
using CommunityToolkit.WinUI.UI.Media.Geometry.Elements.Path;

namespace CommunityToolkit.WinUI.UI.Media.Geometry.Core
{
    /// <summary>
    /// Factory class to instantiate various PathElements.
    /// </summary>
    internal static class PathElementFactory
    {
        /// <summary>
        /// Creates a default Path Element for the given PathFigureType.
        /// </summary>
        /// <param name="figureType">PathFigureType</param>
        /// <returns>ICanvasPathElement</returns>
        internal static ICanvasPathElement CreateDefaultPathElement(PathFigureType figureType)
        {
            if (figureType == PathFigureType.FillRule)
            {
                return new FillRuleElement();
            }

            static ICanvasPathElement Throw() => throw new ArgumentException("Creation of Only Default FillRuleElement is supported.");

            return Throw();
        }

        /// <summary>
        /// Creates a default Path Element for the given PathElementType.
        /// </summary>
        /// <param name="elementType">PathElementType</param>
        /// <returns>ICanvasPathElement</returns>
        internal static ICanvasPathElement CreateDefaultPathElement(PathElementType elementType)
        {
            if (elementType == PathElementType.ClosePath)
            {
                return new ClosePathElement();
            }

            static ICanvasPathElement Throw() => throw new ArgumentException("Creation of Only Default ClosePathElement is supported.");

            return Throw();
        }

        /// <summary>
        /// Instantiates a PathElement based on the PathFigureType.
        /// </summary>
        /// <param name="figureType">PathFigureType</param>
        /// <param name="match">Match object</param>
        /// <param name="index">Index of the path element in the Path data</param>
        /// <returns>ICanvasPathElement</returns>
        internal static ICanvasPathElement CreatePathFigure(PathFigureType figureType, Match match, int index)
        {
            var element = CreatePathElement(figureType);
            element?.Initialize(match, index);
            return element;
        }

        /// <summary>
        /// Instantiates a PathElement based on the PathFigureType.
        /// </summary>
        /// <param name="figureType">PathFigureType</param>
        /// <param name="capture">Capture object</param>
        /// <param name="index">Index of the capture</param>
        /// <param name="isRelative">Indicates whether the coordinates are absolute or relative</param>
        /// <returns>ICanvasPathElement</returns>
        internal static ICanvasPathElement CreateAdditionalPathFigure(PathFigureType figureType, Capture capture, int index, bool isRelative)
        {
            var element = CreatePathElement(figureType);
            element?.InitializeAdditional(capture, index, isRelative);
            return element;
        }

        /// <summary>
        /// Instantiates a PathElement based on the PathElementType.
        /// </summary>
        /// <param name="elementType">PathElementType</param>
        /// <param name="match">Match object</param>
        /// <param name="index">Index of the path element in the Path data</param>
        /// <returns>ICanvasPathElement</returns>
        internal static ICanvasPathElement CreatePathElement(PathElementType elementType, Match match, int index)
        {
            var element = CreatePathElement(elementType);
            element?.Initialize(match, index);
            return element;
        }

        /// <summary>
        /// Instantiates a PathElement based on the PathElementType.
        /// </summary>
        /// <param name="elementType">PathElementType</param>
        /// <param name="capture">Capture object</param>
        /// <param name="index">Index of the capture</param>
        /// <param name="isRelative">Indicates whether the coordinates are absolute or relative</param>
        /// <returns>ICanvasPathElement</returns>
        internal static ICanvasPathElement CreateAdditionalPathElement(PathElementType elementType, Capture capture, int index, bool isRelative)
        {
            // Additional attributes in MoveTo Command must be converted
            // to Line commands
            if (elementType == PathElementType.MoveTo)
            {
                elementType = PathElementType.Line;
            }

            var element = CreatePathElement(elementType);
            element?.InitializeAdditional(capture, index, isRelative);
            return element;
        }

        /// <summary>
        /// Instantiates a PathElement based on the PathFigureType.
        /// </summary>
        /// <param name="figureType">PathFigureType</param>
        /// <returns>ICanvasPathElement</returns>
        private static ICanvasPathElement CreatePathElement(PathFigureType figureType)
        {
            return figureType switch
            {
                PathFigureType.FillRule => new FillRuleElement(),
                PathFigureType.PathFigure => new CanvasPathFigure(),
                PathFigureType.EllipseFigure => new CanvasEllipseFigure(),
                PathFigureType.PolygonFigure => new CanvasPolygonFigure(),
                PathFigureType.RectangleFigure => new CanvasRectangleFigure(),
                PathFigureType.RoundedRectangleFigure => new CanvasRoundRectangleFigure(),
                _ => throw new ArgumentOutOfRangeException(nameof(figureType), figureType, "Invalid PathFigureType!")
            };
        }

        /// <summary>
        /// Instantiates a PathElement based on the PathElementType.
        /// </summary>
        /// <param name="elementType">PathElementType</param>
        /// <returns>ICanvasPathElement</returns>
        private static ICanvasPathElement CreatePathElement(PathElementType elementType)
        {
            return elementType switch
            {
                PathElementType.MoveTo => new MoveToElement(),
                PathElementType.Line => new LineElement(),
                PathElementType.HorizontalLine => new HorizontalLineElement(),
                PathElementType.VerticalLine => new VerticalLineElement(),
                PathElementType.QuadraticBezier => new QuadraticBezierElement(),
                PathElementType.SmoothQuadraticBezier => new SmoothQuadraticBezierElement(),
                PathElementType.CubicBezier => new CubicBezierElement(),
                PathElementType.SmoothCubicBezier => new SmoothCubicBezierElement(),
                PathElementType.Arc => new ArcElement(),
                PathElementType.ClosePath => new ClosePathElement(),
                _ => throw new ArgumentOutOfRangeException(nameof(elementType), elementType, "Invalid PathElementType!")
            };
        }
    }
}