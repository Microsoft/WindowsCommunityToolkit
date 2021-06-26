// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Uwp.UI.Media.Geometry.Core
{
    /// <summary>
    /// Enum for the various PathFigures.
    /// </summary>
    internal enum PathFigureType
    {
        FillRule,
        PathFigure,
        EllipseFigure,
        PolygonFigure,
        RectangleFigure,
        RoundedRectangleFigure
    }

    /// <summary>
    /// Enum for the various PathElements.
    /// </summary>
    internal enum PathElementType
    {
        MoveTo,
        Line,
        HorizontalLine,
        VerticalLine,
        QuadraticBezier,
        SmoothQuadraticBezier,
        CubicBezier,
        SmoothCubicBezier,
        Arc,
        ClosePath
    }

    /// <summary>
    /// Enum for the various types of Brushes.
    /// </summary>
    internal enum BrushType
    {
        SolidColor,
        LinearGradient,
        RadialGradient,
        LinearGradientHdr,
        RadialGradientHdr
    }

    /// <summary>
    /// Enum for the various types of GradientStop attributes.
    /// </summary>
    internal enum GradientStopAttributeType
    {
        Main,
        Additional,
        MainHdr,
        AdditionalHdr
    }
}