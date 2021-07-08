// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using System.Text;
using Microsoft.Graphics.Canvas.Geometry;

namespace CommunityToolkit.WinUI.SampleApp.SamplePages
{
    /// <summary>
    /// Class to read the <see cref="CanvasGeometry"/> path data.
    /// </summary>
    internal class GeometryStreamReader : ICanvasPathReceiver
    {
        private readonly StringBuilder _cmdBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryStreamReader"/> class.
        /// </summary>
        public GeometryStreamReader()
        {
            _cmdBuilder = new StringBuilder();
        }

        /// <summary>
        /// Starts logging the data for the sample app
        /// </summary>
        public void StartLogging()
        {
            _cmdBuilder.Clear();
            _cmdBuilder.AppendLine($"using (var pathBuilder = new CanvasPathBuilder(null))");
            _cmdBuilder.AppendLine("{\n");
        }

        /// <summary>
        /// Finishes reading the geometry path data and returns the data as formatted string.
        /// </summary>
        /// <returns><see cref="CanvasPathBuilder"/> commands to create the CanvasGeometry</returns>
        public string EndLogging()
        {
            _cmdBuilder.AppendLine("}");
            return _cmdBuilder.ToString();
        }

        /// <summary>
        /// Starts a new figure at the specified point, with the specified figure fill option.
        /// </summary>
        /// <param name="point">Start point</param>
        /// <param name="fill"><see cref="CanvasFigureFill"/></param>
        public void BeginFigure(Vector2 point, CanvasFigureFill fill)
        {
            _cmdBuilder.AppendLine($"\n    pathBuilder.BeginFigure(new Vector2({point.X}, {point.Y}));");
        }

        /// <summary>
        /// Adds a single arc to the path, specified by start and end points through which an ellipse will be fitted.
        /// </summary>
        /// <param name="point">Start Point</param>
        /// <param name="x">radiusX</param>
        /// <param name="y">radiusY</param>
        /// <param name="z">rotationAngle</param>
        /// <param name="sweepDirection"><see cref="CanvasSweepDirection"/></param>
        /// <param name="arcSize"><see cref="CanvasArcSize"/></param>
        public void AddArc(Vector2 point, float x, float y, float z, CanvasSweepDirection sweepDirection, CanvasArcSize arcSize)
        {
            _cmdBuilder.AppendLine($"    pathBuilder.AddArc(new Vector2({point.X}, {point.Y}), {x}, {y}, {z}, {sweepDirection}, {arcSize});");
        }

        /// <summary>
        /// Adds a cubic bezier to the path. The bezier starts where the path left off, and has the specified control points and end point.
        /// </summary>
        /// <param name="controlPoint1">First ControlPoint</param>
        /// <param name="controlPoint2">Second Control Point</param>
        /// <param name="endPoint">EndPoint</param>
        public void AddCubicBezier(Vector2 controlPoint1, Vector2 controlPoint2, Vector2 endPoint)
        {
            _cmdBuilder.AppendLine($"    pathBuilder.AddCubicBezier(new Vector2({controlPoint1.X}, {controlPoint1.Y}), new Vector2({controlPoint2.X}, {controlPoint2.Y}), new Vector2({endPoint.X}, {endPoint.Y}));");
        }

        /// <summary>
        /// Adds a line segment to the path, with the specified end point.
        /// </summary>
        /// <param name="endPoint">EndPoint</param>
        public void AddLine(Vector2 endPoint)
        {
            _cmdBuilder.AppendLine($"    pathBuilder.AddLine(new Vector2({endPoint.X}, {endPoint.Y}));");
        }

        /// <summary>
        /// Adds a quadratic bezier to the path. The bezier starts where the path left off, and has the specified control point and end point.
        /// </summary>
        /// <param name="controlPoint">Control Point</param>
        /// <param name="endPoint">EndPoint</param>
        public void AddQuadraticBezier(Vector2 controlPoint, Vector2 endPoint)
        {
            _cmdBuilder.AppendLine($"    pathBuilder.AddQuadraticBezier(new Vector2({controlPoint.X}, {controlPoint.Y}), new Vector2({endPoint.X}, {endPoint.Y}));");
        }

        /// <summary>
        /// Specifies the method used to determine which points are inside the geometry described by this path builder, and which points are outside.
        /// </summary>
        /// <param name="filledRegionDetermination"><see cref="CanvasFilledRegionDetermination"/></param>
        public void SetFilledRegionDetermination(CanvasFilledRegionDetermination filledRegionDetermination)
        {
            _cmdBuilder.AppendLine($"    pathBuilder.SetFilledRegionDetermination(CanvasFilledRegionDetermination.{filledRegionDetermination});");
        }

        /// <summary>
        /// Specifies stroke and join options to be applied to new segments added to the path builder.
        /// </summary>
        /// <param name="figureSegmentOptions"><see cref="CanvasFigureSegmentOptions"/></param>
        public void SetSegmentOptions(CanvasFigureSegmentOptions figureSegmentOptions)
        {
            // Do nothing
        }

        /// <summary>
        /// >Ends the current figure; optionally, closes it.
        /// </summary>
        /// <param name="figureLoop"><see cref="CanvasFigureLoop"/></param>
        public void EndFigure(CanvasFigureLoop figureLoop)
        {
            _cmdBuilder.AppendLine($"    pathBuilder.EndFigure({(figureLoop == CanvasFigureLoop.Closed ? "CanvasFigureLoop.Closed" : "CanvasFigureLoop.Open")});");
        }
    }
}