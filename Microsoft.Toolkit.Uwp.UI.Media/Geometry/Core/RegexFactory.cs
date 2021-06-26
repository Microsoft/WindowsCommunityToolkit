// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("UnitTests.UWP")]

namespace Microsoft.Toolkit.Uwp.UI.Media.Geometry.Core
{
    /// <summary>
    /// Contains all the Regular Expressions which are used for parsing the Win2d Path Mini Language.
    /// </summary>
    internal static class RegexFactory
    {
        // Whitespace
        private const string Spacer = @"\s*";

        // Whitespace or comma
        private const string SpaceOrComma = @"(?:\s+|\s*,\s*)";

        // Whitespace or comma or a minus/plus sign (look ahead)
        private const string Sep = @"(?:\s+|\s*,\s*|(?=[-+.]))";

        // Whitespace or comma or a '#' sign (look ahead)
        private const string ColorSep = @"(?:\s+|\s*,\s*|(?=[#]))";

        // Positive Integer
        private const string Integer = @"[+-]?[0-9]+";

        // Positive Integer
        private const string PositiveInteger = @"[+]?[0-9]+";

        // Floating point number
        private const string Float = @"(?:[-+]?[0-9]*\.?[0-9]+(?:[eE][-+]?[0-9]+)?)";

        // Positive Floating point number
        private const string PositiveFloat = @"(?:[+]?[0-9]*\.?[0-9]+(?:[eE][-+]?[0-9]+)?)";

        // Floating point number between 0 and 1, inclusive
        // private const string Float01 = @"(?:(?<!\d*[1-9]\.?0*)(?:(?:0+(?:\.\d+)?)|(?:\.\d+)|(?:1(?!\.0*[1-9]+)(?:\.0+)?)))";
        private const string Float01 = @"(?:" +
                                       @"(?:(?<!\d*[1-9]\.?0*)(?:(?:0+(?:\.\d+)?)|(?:1(?!\.0*[1-9]+)(?:\.0+)?)))|" +
                                       @"(?:(?<!\d*[1-9])(?:\.\d+))|" +
                                       @"(?:(?<=(?:\.\d+))(?:\.\d+))" +
                                       @")";

        // Hexadecimal characters
        private const string Hex = "(?:[a-f]|[A-F]|[0-9])";

        // Position
        private static readonly string Pos = $"{Float}{Sep}{Float}";

        // MoveTo
        private static readonly string MoveTo = $"(?<MoveTo>[Mm]{Spacer}{Pos}(?:{Sep}{Pos})*{Spacer})";

        // Line
        private static readonly string Line = $"(?<Line>[Ll]{Spacer}{Pos}(?:{Sep}{Pos})*{Spacer})";

        // Horizontal Line
        private static readonly string HorizontalLine = $"(?<HorizontalLine>[Hh]{Spacer}{Float}(?:{Sep}{Float})*{Spacer})";

        // Vertical Line
        private static readonly string VerticalLine = $"(?<VerticalLine>[Vv]{Spacer}{Float}(?:{Sep}{Float})*{Spacer})";

        // Quadratic Bezier
        private static readonly string QuadraticBezier = $"(?<QuadraticBezier>[Qq]{Spacer}{Pos}{Sep}{Pos}(?:{Sep}{Pos}{Sep}{Pos})*{Spacer})";

        // Smooth Quadratic Bezier
        private static readonly string SmoothQuadraticBezier = $"(?<SmoothQuadraticBezier>[Tt]{Spacer}{Pos}(?:{Sep}{Pos})*{Spacer})";

        // Cubic Bezier
        private static readonly string CubicBezier = $"(?<CubicBezier>[Cc]{Spacer}{Pos}{Sep}{Pos}{Sep}{Pos}(?:{Sep}{Pos}{Sep}{Pos}{Sep}{Pos})*{Spacer})";

        // Smooth Cubic Bezier
        private static readonly string SmoothCubicBezier = $"(?<SmoothCubicBezier>[Ss]{Spacer}{Pos}{Sep}{Pos}(?:{Sep}{Pos}{Sep}{Pos})*{Spacer})";

        // Arc
        private static readonly string Arc = $"(?<Arc>[Aa]{Spacer}{Float}{Sep}{Float}{Sep}{Float}{SpaceOrComma}[01]{SpaceOrComma}[01]{Sep}{Pos}" +
                                            $"(?:{Sep}{Float}{Sep}{Float}{Sep}{Float}{SpaceOrComma}[01]{SpaceOrComma}[01]{Sep}{Pos})*{Spacer})";

        // Close Path
        private static readonly string ClosePath = $"(?<ClosePath>[Zz]{Spacer})";

        // CanvasPathFigure
        private static readonly string CanvasPathFigureRegexString =
            $"{MoveTo}" + // M x,y
             "(" +
            $"{Line}+|" + // L x,y
            $"{HorizontalLine}+|" + // H x
            $"{VerticalLine}+|" + // V y
            $"{QuadraticBezier}+|" + // Q x1,y1 x,y
            $"{SmoothQuadraticBezier}+|" + // T x,y
            $"{CubicBezier}+|" + // C x1,y1 x2,y2 x,y
            $"{SmoothCubicBezier}+|" + // S x2,y2 x,y
            $"{Arc}+|" + // A radX, radY, angle, isLargeArc, sweepDirection, x, y
             ")+" +
            $"{ClosePath}?"; // Close Path (Optional)

        // Fill Rule
        private static readonly string FillRule = $"{Spacer}(?<FillRule>[Ff]{Spacer}[01])";

        // PathFigure
        private static readonly string PathFigure = $"{Spacer}(?<PathFigure>{CanvasPathFigureRegexString})";

        // Ellipse Figure
        private static readonly string EllipseFigure = $"{Spacer}(?<EllipseFigure>[Oo]{Spacer}{Float}{Sep}{Float}{Sep}{Pos}" +
                                                       $"(?:{Sep}{Float}{Sep}{Float}{Sep}{Pos})*)";

        // Polygon Figure
        private static readonly string PolygonFigure = $"{Spacer}(?<PolygonFigure>[Pp]{Spacer}{Integer}{Sep}{Float}{Sep}{Pos}" +
                                                       $"(?:{Sep}{Integer}{Sep}{Float}{Sep}{Pos})*)";

        // Rectangle Figure
        private static readonly string RectangleFigure = $"{Spacer}(?<RectangleFigure>[Rr]{Spacer}{Pos}{Sep}{Float}{Sep}{Float}" +
                                                         $"(?:{Sep}{Pos}{Sep}{Float}{Sep}{Float})*)";

        // Rounded Rectangle Figure
        private static readonly string RoundedRectangleFigure = $"{Spacer}(?<RoundedRectangleFigure>[Uu]{Spacer}{Pos}{Sep}{Float}{Sep}{Float}{Sep}{Float}{Sep}{Float}" +
                                                         $"(?:{Sep}{Pos}{Sep}{Float}{Sep}{Float}{Sep}{Float}{Sep}{Float})*)";

        // CanvasGeometry
        private static readonly string CanvasGeometryRegexString =
            $"{FillRule}?" + // F0 or F1
             "(" +
            $"{PathFigure}+|" + // Path Figure
            $"{EllipseFigure}+|" + // O radX, radY, centerX, centerY
            $"{PolygonFigure}+|" + // P numSides, radius, centerX, centerY
            $"{RectangleFigure}+|" + // R x, y, width, height
            $"{RoundedRectangleFigure}+" + // U x, y, width, height, radiusX, radiusY
             ")+";

        // MoveTo
        private static readonly string MoveToAttributes = $"(?<X>{Float}){Sep}(?<Y>{Float})";
        private static readonly string MoveToRegexString = $"{Spacer}(?<Main>(?<Command>[Mm]){Spacer}{MoveToAttributes})" +
                                                           $"(?<Additional>{Sep}{Pos})*";

        // Line
        private static readonly string LineAttributes = $"(?<X>{Float}){Sep}(?<Y>{Float})";
        private static readonly string LineRegexString = $"{Spacer}(?<Main>(?<Command>[Ll]){Spacer}{LineAttributes})" +
                                                         $"(?<Additional>{Sep}{Pos})*";

        // Horizontal Line
        private static readonly string HorizontalLineAttributes = $"(?<X>{Float})";
        private static readonly string HorizontalLineRegexString = $"{Spacer}(?<Main>(?<Command>[Hh]){Spacer}{HorizontalLineAttributes})" +
                                                                   $"(?<Additional>{Sep}{Float})*";

        // Vertical Line
        private static readonly string VerticalLineAttributes = $"(?<Y>{Float})";
        private static readonly string VerticalLineRegexString = $"{Spacer}(?<Main>(?<Command>[Vv]){Spacer}{VerticalLineAttributes})" +
                                                                 $"(?<Additional>{Sep}{Float})*";

        // Quadratic Bezier
        private static readonly string QuadraticBezierAttributes = $"(?<X1>{Float}){Sep}(?<Y1>{Float}){Sep}(?<X>{Float}){Sep}(?<Y>{Float})";
        private static readonly string QuadraticBezierRegexString = $"{Spacer}(?<Main>(?<Command>[Qq]){Spacer}{QuadraticBezierAttributes})" +
                                                                    $"(?<Additional>{Sep}{Pos}{Sep}{Pos})*";

        // Smooth Quadratic Bezier
        private static readonly string SmoothQuadraticBezierAttributes = $"(?<X>{Float}){Sep}(?<Y>{Float})";
        private static readonly string SmoothQuadraticBezierRegexString = $"{Spacer}(?<Main>(?<Command>[Tt]){Spacer}{SmoothQuadraticBezierAttributes})" +
                                                                         $"(?<Additional>{Sep}{Pos})*";

        // Cubic Bezier
        private static readonly string CubicBezierAttributes = $"(?<X1>{Float}){Sep}(?<Y1>{Float}){Sep}(?<X2>{Float}){Sep}(?<Y2>{Float}){Sep}" +
                                                               $"(?<X>{Float}){Sep}(?<Y>{Float})";

        private static readonly string CubicBezierRegexString = $"{Spacer}(?<Main>(?<Command>[Cc]){Spacer}{CubicBezierAttributes})" +
                                                               $"(?<Additional>{Sep}{Pos}{Sep}{Pos}{Sep}{Pos})*";

        // Smooth Cubic Bezier
        private static readonly string SmoothCubicBezierAttributes = $"(?<X2>{Float}){Sep}(?<Y2>{Float}){Sep}(?<X>{Float}){Sep}(?<Y>{Float})";
        private static readonly string SmoothCubicBezierRegexString = $"{Spacer}(?<Main>(?<Command>[Ss]){Spacer}{SmoothCubicBezierAttributes})" +
                                                                     $"(?<Additional>{Sep}{Pos}{Sep}{Pos})*";

        // Arc
        private static readonly string ArcAttributes = $"(?<RadiusX>{Float}){Sep}(?<RadiusY>{Float}){Sep}(?<Angle>{Float}){SpaceOrComma}" +
                                                      $"(?<IsLargeArc>[01]){SpaceOrComma}(?<SweepDirection>[01]){Sep}(?<X>{Float}){Sep}(?<Y>{Float})";

        private static readonly string ArcRegexString = $"{Spacer}(?<Main>(?<Command>[Aa]){Spacer}{ArcAttributes})" +
                                                       $"(?<Additional>{Sep}{Float}{Sep}{Float}{Sep}{Float}{SpaceOrComma}[01]{SpaceOrComma}[01]{Sep}{Pos})*";

        // Close Path
        private static readonly string ClosePathRegexString = $"{Spacer}(?<Main>(?<Command>[Zz])){Spacer}";

        // Fill Rule
        private static readonly string FillRuleRegexString = $"{Spacer}(?<Main>(?<Command>[Ff]){Spacer}(?<FillValue>[01]))";

        // Path Figure
        private static readonly string PathFigureRegexString = $"{Spacer}(?<Main>{PathFigure})";

        // Ellipse Figure
        private static readonly string EllipseFigureAttributes = $"(?<RadiusX>{Float}){Sep}(?<RadiusY>{Float}){Sep}" +
                                                                $"(?<X>{Float}){Sep}(?<Y>{Float})";

        private static readonly string EllipseFigureRegexString = $"{Spacer}(?<Main>(?<Command>[Oo]){Spacer}{EllipseFigureAttributes})" +
                                                                 $"(?<Additional>{Sep}{Float}{Sep}{Float}{Sep}{Pos})*";

        // Polygon Figure
        private static readonly string PolygonFigureAttributes = $"(?<Sides>{Integer}){Sep}(?<Radius>{Float}){Sep}(?<X>{Float}){Sep}(?<Y>{Float})";
        private static readonly string PolygonFigureRegexString = $"{Spacer}(?<Main>(?<Command>[Pp]){Spacer}{PolygonFigureAttributes})" +
                                                                 $"(?<Additional>{Sep}{Integer}{Sep}{Float}{Sep}{Pos})*";

        // Rectangle Figure
        private static readonly string RectangleFigureAttributes = $"(?<X>{Float}){Sep}(?<Y>{Float}){Sep}(?<Width>{Float}){Sep}(?<Height>{Float})";
        private static readonly string RectangleFigureRegexString = $"{Spacer}(?<Main>(?<Command>[Rr]){Spacer}{RectangleFigureAttributes})" +
                                                                 $"(?<Additional>{Sep}{Pos}{Sep}{Float}{Sep}{Float})*";

        // Rectangle Figure
        private static readonly string RoundedRectangleFigureAttributes = $"(?<X>{Float}){Sep}(?<Y>{Float}){Sep}(?<Width>{Float}){Sep}(?<Height>{Float})" +
                                                                          $"{Sep}(?<RadiusX>{Float}){Sep}(?<RadiusY>{Float})";

        private static readonly string RoundedRectangleFigureRegexString = $"{Spacer}(?<Main>(?<Command>[Uu]){Spacer}{RoundedRectangleFigureAttributes})" +
                                                                 $"(?<Additional>{Sep}{Pos}{Sep}{Float}{Sep}{Float}{Sep}{Float}{Sep}{Float})*";

        // ARGB Color
        private static readonly string HexColor = $"(?:#?(?:{Hex}{{2}})?{Hex}{{6}})";

        // Alpha
        private static readonly string Alpha = $"(?<Alpha>{Hex}{{2}})";

        // Red
        private static readonly string Red = $"(?<Red>{Hex}{{2}})";

        // Green
        private static readonly string Green = $"(?<Green>{Hex}{{2}})";

        // Blue
        private static readonly string Blue = $"(?<Blue>{Hex}{{2}})";

        // Hexadecimal Color
        private static readonly string RgbColor = $"(?<RgbColor>{HexColor})";

        // HDR Color (Vector4 in which each component has a value between 0 and 1, inclusive)
        private static readonly string HdrColor = $"(?<HdrColor>{Float01}{Sep}{Float01}{Sep}{Float01}{Sep}{Float01})";

        // Hexadecimal Color Attributes
        private static readonly string RgbColorAttributes = $"(?<RgbColor>#{{0,1}}{Alpha}{{0,1}}{Red}{Green}{Blue})";

        // HDR Color Attributes (Vector4 in which each component has a value between 0 and 1, inclusive)
        private static readonly string HdrColorAttributes = $"(?<HdrColor>(?<X>{Float01}){Sep}(?<Y>{Float01}){Sep}(?<Z>{Float01}){Sep}(?<W>{Float01}))";

        private static readonly string ColorRegexString = $"(?:{RgbColorAttributes}|{HdrColorAttributes})";

        // Start Point
        private static readonly string StartPoint = $"(?<StartPoint>[Mm]{Spacer}{Pos}{Spacer})";

        // End Point
        private static readonly string EndPoint = $"(?<EndPoint>[Zz]{Spacer}{Pos}{Spacer})";

        // Opacity
        private static readonly string Opacity = $"(?<Opacity>[Oo]{Spacer}{Float01}{Spacer})";

        // Alpha Mode
        private static readonly string AlphaMode = $"(?<AlphaMode>[Aa]{Spacer}[012]{Spacer})";

        // Buffer Precision
        private static readonly string BufferPrecision = $"(?<BufferPrecision>[Bb]{Spacer}[01234]{Spacer})";

        // Edge Behavior
        private static readonly string EdgeBehavior = $"(?<EdgeBehavior>[Ee]{Spacer}[012]{Spacer})";

        // PreInterpolation Color Space
        private static readonly string PreColorSpace = $"(?<PreColorSpace>[Pp]{Spacer}[012]{Spacer})";

        // PostInterpolation Color Space
        private static readonly string PostColorSpace = $"(?<PostColorSpace>[Rr]{Spacer}[012]{Spacer})";

        // Radius in X-axis
        private static readonly string RadiusX = $"(?<RadiusX>{Float})";

        // Radius in Y-axis
        private static readonly string RadiusY = $"(?<RadiusY>{Float})";

        // Center location on X-axis
        private static readonly string CenterX = $"(?<CenterX>{Float})";

        // Center location on Y-axis
        private static readonly string CenterY = $"(?<CenterY>{Float})";

        // Origin Offset
        private static readonly string OriginOffset = $"(?<OriginOffset>[Ff]{Spacer}{Pos}{Spacer})";

        // GradientStops
        private static readonly string GradientStops = $"(?<GradientStops>[Ss]{Spacer}{Float01}{ColorSep}{HexColor}(?:{Sep}{Float01}{ColorSep}{HexColor})*{Spacer})";

        // GradientStopHdrs
        private static readonly string GradientStopHdrs = $"(?<GradientStops>[Ss]{Spacer}{Float01}{Sep}{HdrColor}(?:{Sep}{Float01}{Sep}{HdrColor})*{Spacer})";

        // Solid Color Brush
        private static readonly string SolidColorBrush = $"(?<SolidColorBrush>[Ss][Cc]{Spacer}(?:{RgbColor}|{HdrColor}){Spacer}{Opacity}?)";

        // LinearGradient
        private static readonly string LinearGradient = $"(?<LinearGradient>[Ll][Gg]{Spacer}{StartPoint}{EndPoint}" +
                                                        $"{Opacity}?{AlphaMode}?{BufferPrecision}?{EdgeBehavior}?{PreColorSpace}?{PostColorSpace}?" +
                                                        $"{GradientStops}+{Spacer})";

        // RadialGradient
        private static readonly string RadialGradient = $"(?<RadialGradient>[Rr][Gg]{Spacer}{RadiusX}{Sep}{RadiusY}{Sep}{CenterX}{Sep}{CenterY}{Spacer}" +
                                                        $"{Opacity}?{AlphaMode}?{BufferPrecision}?{EdgeBehavior}?{OriginOffset}?{PreColorSpace}?{PostColorSpace}?" +
                                                        $"{GradientStops}+{Spacer})";

        // LinearGradientHdr
        private static readonly string LinearGradientHdr = $"(?<LinearGradientHdr>[Ll][Hh]{Spacer}{StartPoint}{EndPoint}" +
                                                        $"{Opacity}?{AlphaMode}?{BufferPrecision}?{EdgeBehavior}?{PreColorSpace}?{PostColorSpace}?" +
                                                        $"{GradientStopHdrs}+{Spacer})";

        // RadialGradientHdr
        private static readonly string RadialGradientHdr = $"(?<RadialGradientHdr>[Rr][Hh]{Spacer}{RadiusX}{Sep}{RadiusY}{Sep}{CenterX}{Sep}{CenterY}{Spacer}" +
                                                        $"{Opacity}?{AlphaMode}?{BufferPrecision}?{EdgeBehavior}?{OriginOffset}?{PreColorSpace}?{PostColorSpace}?" +
                                                        $"{GradientStopHdrs}+{Spacer})";

        // Regex for the CanvasBrush
        private static readonly string CanvasBrushRegexString = $"(?<CanvasBrush>{SolidColorBrush}|{LinearGradient}|{RadialGradient}|{LinearGradientHdr}|{RadialGradientHdr})";

        // Start Point
        private static readonly string StartPointAttr = $"(?:[Mm]{Spacer}(?<StartX>{Float}){Sep}(?<StartY>{Float}){Spacer})";

        // End Point
        private static readonly string EndPointAttr = $"(?:[Zz]{Spacer}(?<EndX>{Float}){Sep}(?<EndY>{Float}){Spacer})";

        // Opacity
        private static readonly string OpacityAttr = $"(?:[Oo]{Spacer}(?<Opacity>{Float01}){Spacer})";

        // Alpha Mode
        private static readonly string AlphaModeAttr = $"(?:[Aa]{Spacer}(?<AlphaMode>[012]){Spacer})";

        // Buffer Precision
        private static readonly string BufferPrecisionAttr = $"(?:[Bb]{Spacer}(?<BufferPrecision>[01234]){Spacer})";

        // Edge Behavior
        private static readonly string EdgeBehaviorAttr = $"(?:[Ee]{Spacer}(?<EdgeBehavior>[012]){Spacer})";

        // PreInterpolation Color Space
        private static readonly string PreColorSpaceAttr = $"(?:[Pp]{Spacer}(?<PreColorSpace>[012]){Spacer})";

        // PostInterpolation Color Space
        private static readonly string PostColorSpaceAttr = $"(?:[Rr]{Spacer}(?<PostColorSpace>[012]){Spacer})";

        // Origin Offset
        private static readonly string OriginOffsetAttr = $"(?<OriginOffset>[Ff]{Spacer}(?<OffsetX>{Float}){Sep}(?<OffsetY>{Float}){Spacer})";

        // GradientStop Attributes
        private static readonly string GradientStopAttributes = $"(?<Position>{Float01}){ColorSep}{RgbColorAttributes}";
        private static readonly string GradientStopMainAttributes = $"(?<Main>[Ss]{Spacer}{GradientStopAttributes})";
        private static readonly string GradientStopRegexString = $"(?<GradientStops>{GradientStopMainAttributes}" + $"(?<Additional>{Sep}{Float01}{ColorSep}{HexColor})*{Spacer})";

        // GradientStopHdr Attributes
        private static readonly string GradientStopHdrAttributes = $"(?<Position>{Float01}){Sep}{HdrColorAttributes}";
        private static readonly string GradientStopHdrMainAttributes = $"(?<Main>(?<Command>[Ss]){Spacer}{GradientStopHdrAttributes})";
        private static readonly string GradientStopHdrRegexString = $"(?<GradientStops>{GradientStopHdrMainAttributes}" + $"(?<Additional>{Sep}{Float01}{Sep}{HdrColor})*{Spacer})";

        // Regex for SolidColorBrush Attributes
        private static readonly string SolidColorBrushRegexString = $"(?:[Ss][Cc]{Spacer}(?:{RgbColorAttributes}|{HdrColorAttributes}){Spacer}{OpacityAttr}?)";

        // Regex for LinearGradient Attributes
        private static readonly string LinearGradientRegexString = $"[Ll][Gg]{Spacer}{StartPointAttr}{EndPointAttr}" +
                                                                   $"{OpacityAttr}?{AlphaModeAttr}?{BufferPrecisionAttr}?" +
                                                                   $"{EdgeBehaviorAttr}?{PreColorSpaceAttr}?{PostColorSpaceAttr}?" +
                                                                   $"{GradientStops}+{Spacer}";

        // Regex for RadialGradient Attributes
        private static readonly string RadialGradientRegexString = $"[Rr][Gg]{Spacer}{RadiusX}{Sep}{RadiusY}{Sep}{CenterX}{Sep}{CenterY}{Spacer}" +
                                                                   $"{OpacityAttr}?{AlphaModeAttr}?{BufferPrecisionAttr}?{EdgeBehaviorAttr}?" +
                                                                   $"{OriginOffsetAttr}?{PreColorSpaceAttr}?{PostColorSpaceAttr}?" +
                                                                   $"{GradientStops}+{Spacer}";

        // Regex for LinearGradientHdr Attributes
        private static readonly string LinearGradientHdrRegexString = $"[Ll][Hh]{Spacer}{StartPointAttr}{EndPointAttr}" +
                                                                      $"{OpacityAttr}?{AlphaModeAttr}?{BufferPrecisionAttr}?" +
                                                                      $"{EdgeBehaviorAttr}?{PreColorSpaceAttr}?{PostColorSpaceAttr}?" +
                                                                      $"{GradientStopHdrs}+{Spacer}";

        // Regex for RadialGradientHdr Attributes
        private static readonly string RadialGradientHdrRegexString = $"[Rr][Hh]{Spacer}{RadiusX}{Sep}{RadiusY}{Sep}{CenterX}{Sep}{CenterY}{Spacer}" +
                                                                      $"{OpacityAttr}?{AlphaModeAttr}?{BufferPrecisionAttr}?{EdgeBehaviorAttr}?" +
                                                                      $"{OriginOffsetAttr}?{PreColorSpaceAttr}?{PostColorSpaceAttr}?" +
                                                                      $"{GradientStopHdrs}+{Spacer}";

        // CanvasStrokeStyle attributes
        private static readonly string DashStyle = $"(?:[Dd][Ss]{Spacer}(?<DashStyle>[01234]){Spacer})";
        private static readonly string LineJoin = $"(?:[Ll][Jj]{Spacer}(?<LineJoin>[0123]){Spacer})";
        private static readonly string MiterLimit = $"(?:[Mm][Ll]{Spacer}(?<MiterLimit>{Float}){Spacer})";
        private static readonly string DashOffset = $"(?:[Dd][Oo]{Spacer}(?<DashOffset>{Float}){Spacer})";
        private static readonly string StartCap = $"(?:[Ss][Cc]{Spacer}(?<StartCap>[0123]){Spacer})";
        private static readonly string EndCap = $"(?:[Ee][Cc]{Spacer}(?<EndCap>[0123]){Spacer})";
        private static readonly string DashCap = $"(?:[Dd][Cc]{Spacer}(?<DashCap>[0123]){Spacer})";
        private static readonly string TransformBehavior = $"(?:[Tt][Bb]{Spacer}(?<TransformBehavior>[012]){Spacer})";
        private static readonly string CustomDashAttribute = $"(?<DashSize>{Float}){Sep}(?<SpaceSize>{Float})";
        private static readonly string CustomDashStyle = $"(?<CustomDashStyle>[Cc][Dd][Ss]{Spacer}(?<Main>{CustomDashAttribute})" + $"(?<Additional>{Sep}{Float}{Sep}{Float})*{Spacer})";

        // CanvasStrokeStyle Regex
        private static readonly string CanvasStrokeStyleRegexString = $"(?<CanvasStrokeStyle>[Cc][Ss][Ss]{Spacer}{DashStyle}?{LineJoin}?{MiterLimit}?{DashOffset}?" +
                                                                      $"{StartCap}?{EndCap}?{DashCap}?{TransformBehavior}?{CustomDashStyle}?)";

        // CanvasStroke Regex
        private static readonly string CanvasStrokeRegexString = $"(?<CanvasStroke>[Ss][Tt]{Spacer}" +
                                                                 $"(?<StrokeWidth>{Float}){Spacer}" +
                                                                 $"{CanvasBrushRegexString}{Spacer}" +
                                                                 $"{CanvasStrokeStyleRegexString}?)";

        private static readonly Dictionary<PathFigureType, Regex> PathFigureRegexes;
        private static readonly Dictionary<PathFigureType, Regex> PathFigureAttributeRegexes;
        private static readonly Dictionary<PathElementType, Regex> PathElementRegexes;
        private static readonly Dictionary<PathElementType, Regex> PathElementAttributeRegexes;
        private static readonly Dictionary<BrushType, Regex> BrushRegexes;
        private static readonly Dictionary<GradientStopAttributeType, Regex> GradientStopAttributeRegexes;

        /// <summary>
        /// Gets the Regex to perform validation of Path data.
        /// </summary>
        public static Regex ValidationRegex { get; }

        /// <summary>
        /// Gets the Regex for parsing the CanvasGeometry string.
        /// </summary>
        public static Regex CanvasGeometryRegex { get; }

        /// <summary>
        /// Gets the Regex for parsing Hexadecimal Color string.
        /// </summary>
        public static Regex ColorRegex { get; }

        /// <summary>
        /// Gets the Regex for parsing the ICanvasBrush string.
        /// </summary>
        public static Regex CanvasBrushRegex { get; }

        /// <summary>
        /// Gets the Regex for parsing the GradientStop string.
        /// </summary>
        public static Regex GradientStopRegex { get; }

        /// <summary>
        /// Gets the Regex for parsing the GradientStopHdr string.
        /// </summary>
        public static Regex GradientStopHdrRegex { get; }

        /// <summary>
        /// Gets the Regex for parsing the CanvasStrokeStyle string.
        /// </summary>
        public static Regex CanvasStrokeStyleRegex { get; }

        /// <summary>
        /// Gets the Regex for parsing the CanvasStroke string.
        /// </summary>
        public static Regex CanvasStrokeRegex { get; }

        /// <summary>
        /// Gets the Regex for parsing the CustomDashStyle attributes.
        /// </summary>
        public static Regex CustomDashAttributeRegex { get; }

        /// <summary>
        /// Initializes static members of the <see cref="RegexFactory"/> class.
        /// </summary>
        static RegexFactory()
        {
            PathFigureRegexes = new Dictionary<PathFigureType, Regex>
            {
                [PathFigureType.FillRule] = new Regex(FillRuleRegexString, RegexOptions.Compiled),
                [PathFigureType.PathFigure] = new Regex(PathFigureRegexString, RegexOptions.Compiled),
                [PathFigureType.EllipseFigure] = new Regex(EllipseFigureRegexString, RegexOptions.Compiled),
                [PathFigureType.PolygonFigure] = new Regex(PolygonFigureRegexString, RegexOptions.Compiled),
                [PathFigureType.RectangleFigure] = new Regex(RectangleFigureRegexString, RegexOptions.Compiled),
                [PathFigureType.RoundedRectangleFigure] = new Regex(RoundedRectangleFigureRegexString, RegexOptions.Compiled)
            };

            PathFigureAttributeRegexes = new Dictionary<PathFigureType, Regex>
            {
                // Not Applicable for FillRuleElement
                [PathFigureType.FillRule] = null,

                // Not Applicable for CanvasPathFigure
                [PathFigureType.PathFigure] = null,
                [PathFigureType.EllipseFigure] = new Regex($"{Sep}{EllipseFigureAttributes}", RegexOptions.Compiled),
                [PathFigureType.PolygonFigure] = new Regex($"{Sep}{PolygonFigureAttributes}", RegexOptions.Compiled),
                [PathFigureType.RectangleFigure] = new Regex($"{Sep}{RectangleFigureAttributes}", RegexOptions.Compiled),
                [PathFigureType.RoundedRectangleFigure] = new Regex($"{Sep}{RoundedRectangleFigureAttributes}", RegexOptions.Compiled)
            };

            PathElementRegexes = new Dictionary<PathElementType, Regex>
            {
                [PathElementType.MoveTo] = new Regex(MoveToRegexString, RegexOptions.Compiled),
                [PathElementType.Line] = new Regex(LineRegexString, RegexOptions.Compiled),
                [PathElementType.HorizontalLine] = new Regex(HorizontalLineRegexString, RegexOptions.Compiled),
                [PathElementType.VerticalLine] = new Regex(VerticalLineRegexString, RegexOptions.Compiled),
                [PathElementType.QuadraticBezier] = new Regex(QuadraticBezierRegexString, RegexOptions.Compiled),
                [PathElementType.SmoothQuadraticBezier] = new Regex(SmoothQuadraticBezierRegexString, RegexOptions.Compiled),
                [PathElementType.CubicBezier] = new Regex(CubicBezierRegexString, RegexOptions.Compiled),
                [PathElementType.SmoothCubicBezier] = new Regex(SmoothCubicBezierRegexString, RegexOptions.Compiled),
                [PathElementType.Arc] = new Regex(ArcRegexString, RegexOptions.Compiled),
                [PathElementType.ClosePath] = new Regex(ClosePathRegexString, RegexOptions.Compiled)
            };

            PathElementAttributeRegexes = new Dictionary<PathElementType, Regex>
            {
                [PathElementType.MoveTo] = new Regex($"{Sep}{MoveToAttributes}", RegexOptions.Compiled),
                [PathElementType.Line] = new Regex($"{Sep}{LineAttributes}", RegexOptions.Compiled),
                [PathElementType.HorizontalLine] = new Regex($"{Sep}{HorizontalLineAttributes}", RegexOptions.Compiled),
                [PathElementType.VerticalLine] = new Regex($"{Sep}{VerticalLineAttributes}", RegexOptions.Compiled),
                [PathElementType.QuadraticBezier] = new Regex($"{Sep}{QuadraticBezierAttributes}", RegexOptions.Compiled),
                [PathElementType.SmoothQuadraticBezier] = new Regex($"{Sep}{SmoothQuadraticBezierAttributes}", RegexOptions.Compiled),
                [PathElementType.CubicBezier] = new Regex($"{Sep}{CubicBezierAttributes}", RegexOptions.Compiled),
                [PathElementType.SmoothCubicBezier] = new Regex($"{Sep}{SmoothCubicBezierAttributes}", RegexOptions.Compiled),
                [PathElementType.Arc] = new Regex($"{Sep}{ArcAttributes}", RegexOptions.Compiled),

                // Not Applicable for ClosePathElement as it has no attributes
                [PathElementType.ClosePath] = null
            };

            BrushRegexes = new Dictionary<BrushType, Regex>
            {
                [BrushType.SolidColor] = new Regex(SolidColorBrushRegexString, RegexOptions.Compiled),
                [BrushType.LinearGradient] = new Regex(LinearGradientRegexString, RegexOptions.Compiled),
                [BrushType.LinearGradientHdr] = new Regex(LinearGradientHdrRegexString, RegexOptions.Compiled),
                [BrushType.RadialGradient] = new Regex(RadialGradientRegexString, RegexOptions.Compiled),
                [BrushType.RadialGradientHdr] = new Regex(RadialGradientHdrRegexString, RegexOptions.Compiled)
            };

            GradientStopAttributeRegexes = new Dictionary<GradientStopAttributeType, Regex>
            {
                [GradientStopAttributeType.Main] = new Regex(GradientStopMainAttributes, RegexOptions.Compiled),
                [GradientStopAttributeType.Additional] = new Regex($"{Sep}{GradientStopAttributes}", RegexOptions.Compiled),
                [GradientStopAttributeType.MainHdr] = new Regex(GradientStopHdrMainAttributes, RegexOptions.Compiled),
                [GradientStopAttributeType.AdditionalHdr] = new Regex($"{Sep}{GradientStopHdrAttributes}", RegexOptions.Compiled)
            };

            ValidationRegex = new Regex(@"\s+");
            CanvasGeometryRegex = new Regex(CanvasGeometryRegexString, RegexOptions.Compiled);
            ColorRegex = new Regex(ColorRegexString, RegexOptions.Compiled);
            CanvasBrushRegex = new Regex(CanvasBrushRegexString, RegexOptions.Compiled);
            GradientStopRegex = new Regex(GradientStopRegexString, RegexOptions.Compiled);
            GradientStopHdrRegex = new Regex(GradientStopHdrRegexString, RegexOptions.Compiled);
            CanvasStrokeStyleRegex = new Regex(CanvasStrokeStyleRegexString, RegexOptions.Compiled);
            CanvasStrokeRegex = new Regex(CanvasStrokeRegexString, RegexOptions.Compiled);
            CustomDashAttributeRegex = new Regex($"{Sep}{CustomDashAttribute}", RegexOptions.Compiled);
        }

        /// <summary>
        /// Get the Regex for the given PathFigureType
        /// </summary>
        /// <param name="figureType">PathFigureType</param>
        /// <returns>Regex</returns>
        internal static Regex GetRegex(PathFigureType figureType)
        {
            return PathFigureRegexes[figureType];
        }

        /// <summary>
        /// Get the Regex for the given PathElementType
        /// </summary>
        /// <param name="elementType">PathElementType</param>
        /// <returns>Regex</returns>
        internal static Regex GetRegex(PathElementType elementType)
        {
            return PathElementRegexes[elementType];
        }

        /// <summary>
        /// Get the Regex for extracting attributes of the given PathFigureType
        /// </summary>
        /// <param name="figureType">PathFigureType</param>
        /// <returns>Regex</returns>
        internal static Regex GetAttributesRegex(PathFigureType figureType)
        {
            return PathFigureAttributeRegexes[figureType];
        }

        /// <summary>
        /// Get the Regex for extracting attributes of the given PathElementType
        /// </summary>
        /// <param name="elementType">PathElementType</param>
        /// <returns>Regex</returns>
        internal static Regex GetAttributesRegex(PathElementType elementType)
        {
            return PathElementAttributeRegexes[elementType];
        }

        /// <summary>
        /// Gets the Regex for extracting the attributes of the given BrushType
        /// </summary>
        /// <param name="brushType">BrushType</param>
        /// <returns>Regex</returns>
        internal static Regex GetAttributesRegex(BrushType brushType)
        {
            return BrushRegexes[brushType];
        }

        /// <summary>
        /// Gets the Regex for extracting the attributes of the given
        /// GradientStopAttributeType
        /// </summary>
        /// <param name="gsAttrType">GradientStopAttributeType</param>
        /// <returns>Regex</returns>
        internal static Regex GetAttributesRegex(GradientStopAttributeType gsAttrType)
        {
            return GradientStopAttributeRegexes[gsAttrType];
        }
    }
}