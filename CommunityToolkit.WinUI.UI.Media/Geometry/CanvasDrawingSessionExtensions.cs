// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Windows.Foundation;
using Windows.UI;

namespace CommunityToolkit.WinUI.UI.Media.Geometry
{
    /// <summary>
    /// Extension methods for CanvasDrawingSession.
    /// </summary>
    public static class CanvasDrawingSessionExtensions
    {
        /// <summary>
        /// Draws a circle of at the given center, having the specified radius, using a CanvasStroke to define the stroke width, the stroke color and stroke style.
        /// </summary>
        /// <param name="session">CanvasDrawingSession</param>
        /// <param name="centerPoint">Center of the Circle</param>
        /// <param name="radius">Radius of the Circle</param>
        /// <param name="stroke">CanvasStroke defining the stroke width, the stroke
        /// color and stroke style.</param>
        public static void DrawCircle(this CanvasDrawingSession session, Vector2 centerPoint, float radius, ICanvasStroke stroke)
        {
            session.DrawCircle(centerPoint, radius, stroke.Brush, stroke.Width, stroke.Style);
        }

        /// <summary>
        /// Draws a circle of at the given center, having the specified radius, using a CanvasStroke to define the stroke width, the stroke color and stroke style.
        /// </summary>
        /// <param name="session">CanvasDrawingSession</param>
        /// <param name="x">Offset of the Center in x axis</param>
        /// <param name="y">Ordinate of the Center in the y axis</param>
        /// <param name="radius">Radius of the Circle</param>
        /// <param name="stroke">CanvasStroke defining the stroke width, the stroke
        /// color and stroke style.</param>
        public static void DrawCircle(this CanvasDrawingSession session, float x, float y, float radius, ICanvasStroke stroke)
        {
            session.DrawCircle(x, y, radius, stroke.Brush, stroke.Width, stroke.Style);
        }

        /// <summary>
        /// Draws an Ellipse of at the given center, having the specified radius, using a CanvasStroke to define the stroke width, the stroke color and stroke style.
        /// </summary>
        /// <param name="session">CanvasDrawingSession</param>
        /// <param name="centerPoint">Center of the Circle</param>
        /// <param name="radiusX">Radius in the X axis</param>
        /// <param name="radiusY">Radius in the Y axis</param>
        /// <param name="stroke">CanvasStroke defining the stroke width, the stroke
        /// color and stroke style.</param>
        public static void DrawEllipse(this CanvasDrawingSession session, Vector2 centerPoint, float radiusX, float radiusY, ICanvasStroke stroke)
        {
            session.DrawEllipse(centerPoint, radiusX, radiusY, stroke.Brush, stroke.Width, stroke.Style);
        }

        /// <summary>
        /// Draws an Ellipse of at the given center, having the specified radius, using a CanvasStroke to define the stroke width, the stroke color and stroke style.
        /// </summary>
        /// <param name="session">CanvasDrawingSession</param>
        /// <param name="x">Offset of the Center on the x axis</param>
        /// <param name="y">Offset of the Center on the y axis</param>
        /// <param name="radiusX">Radius in the X axis</param>
        /// <param name="radiusY">Radius in the Y axis</param>
        /// <param name="stroke">CanvasStroke defining the stroke width, the stroke
        /// color and stroke style.</param>
        public static void DrawEllipse(this CanvasDrawingSession session, float x, float y, float radiusX, float radiusY, ICanvasStroke stroke)
        {
            session.DrawEllipse(x, y, radiusX, radiusY, stroke.Brush, stroke.Width, stroke.Style);
        }

        /// <summary>
        /// Draws a geometry relative to the origin, using a CanvasStroke to define the stroke width, the stroke color and stroke style.
        /// </summary>
        /// <param name="session">CanvasDrawingSession</param>
        /// <param name="geometry">CanvasGeometry to render</param>
        /// <param name="stroke">CanvasStroke defining the stroke width, the stroke
        /// color and stroke style.</param>
        public static void DrawGeometry(this CanvasDrawingSession session, CanvasGeometry geometry, ICanvasStroke stroke)
        {
            session.DrawGeometry(geometry, stroke.Brush, stroke.Width, stroke.Style);
        }

        /// <summary>
        /// Draws a geometry relative to the specified position, using a CanvasStroke to define the stroke width, the stroke color and stroke style.
        /// </summary>
        /// <param name="session">CanvasDrawingSession</param>
        /// <param name="geometry">CanvasGeometry to render</param>
        /// <param name="offset">Offset</param>
        /// <param name="stroke">CanvasStroke defining the stroke width, the stroke
        /// color and stroke style.</param>
        public static void DrawGeometry(this CanvasDrawingSession session, CanvasGeometry geometry, Vector2 offset, ICanvasStroke stroke)
        {
            session.DrawGeometry(geometry, offset, stroke.Brush, stroke.Width, stroke.Style);
        }

        /// <summary>
        /// Draws a geometry relative to the specified position, using a CanvasStroke to define the stroke width, the stroke color and stroke style.
        /// </summary>
        /// <param name="session">CanvasDrawingSession</param>
        /// <param name="geometry">CanvasGeometry to render</param>
        /// <param name="x">Offset on the x axis</param>
        /// <param name="y">Offset on the y axis</param>
        /// <param name="stroke">CanvasStroke defining the stroke width, the stroke
        /// color and stroke style.</param>
        public static void DrawGeometry(this CanvasDrawingSession session, CanvasGeometry geometry, float x, float y, ICanvasStroke stroke)
        {
            session.DrawGeometry(geometry, x, y, stroke.Brush, stroke.Width, stroke.Style);
        }

        /// <summary>
        /// Draws a line between the specified positions, using a CanvasStroke to define the stroke width, the stroke color and stroke style.
        /// </summary>
        /// <param name="session">CanvasDrawingSession</param>
        /// <param name="point0">Starting position of the line</param>
        /// <param name="point1">Ending position of the line</param>
        /// <param name="stroke">CanvasStroke defining the stroke width, the stroke
        /// color and stroke style.</param>
        public static void DrawLine(this CanvasDrawingSession session, Vector2 point0, Vector2 point1, ICanvasStroke stroke)
        {
            session.DrawLine(point0, point1, stroke.Brush, stroke.Width, stroke.Style);
        }

        /// <summary>
        /// Draws a line between the specified positions, using a CanvasStroke to define the stroke width, the stroke color and stroke style.
        /// </summary>
        /// <param name="session">CanvasDrawingSession</param>
        /// <param name="x0">Offset of Starting position of the line on x-axis</param>
        /// <param name="y0">Offset of Starting position of the line on y-axis</param>
        /// <param name="x1">Offset of Ending position of the line on x-axis</param>
        /// <param name="y1">Offset of Ending position of the line on y-axis</param>
        /// <param name="stroke">CanvasStroke defining the stroke width, the stroke
        /// color and stroke style.</param>
        public static void DrawLine(this CanvasDrawingSession session, float x0, float y0, float x1, float y1, ICanvasStroke stroke)
        {
            session.DrawLine(x0, y0, x1, y1, stroke.Brush, stroke.Width, stroke.Style);
        }

        /// <summary>
        /// Draws a Rectangle of the specified dimensions, using a CanvasStroke to define the stroke width, the stroke color and stroke style.
        /// </summary>
        /// <param name="session">CanvasDrawingSession</param>
        /// <param name="rect">Rectangle dimensions</param>
        /// <param name="stroke">CanvasStroke defining the stroke width, the stroke
        /// color and stroke style.</param>
        public static void DrawRectangle(this CanvasDrawingSession session, Rect rect, ICanvasStroke stroke)
        {
            session.DrawRectangle(rect, stroke.Brush, stroke.Width, stroke.Style);
        }

        /// <summary>
        /// Draws a Rectangle of the specified dimensions, using a CanvasStroke to define the stroke width, the stroke color and stroke style.
        /// </summary>
        /// <param name="session">CanvasDrawingSession</param>
        /// <param name="x">Offset of the top left corner of the Rectangle on the x-axis</param>
        /// <param name="y">Offset of the top left corner of the Rectangle on the y-axis</param>
        /// <param name="w">Width of the Rectangle</param>
        /// <param name="h">Height of the Rectangle</param>
        /// <param name="stroke">CanvasStroke defining the stroke width, the stroke
        /// color and stroke style.</param>
        public static void DrawRectangle(this CanvasDrawingSession session, float x, float y, float w, float h, ICanvasStroke stroke)
        {
            session.DrawRectangle(x, y, w, h, stroke.Brush, stroke.Width, stroke.Style);
        }

        /// <summary>
        /// Draws a Rounded Rectangle of the specified dimensions, using a CanvasStroke to define the stroke width, the stroke color and stroke style.
        /// </summary>
        /// <param name="session">CanvasDrawingSession</param>
        /// <param name="rect">Rectangle dimensions</param>
        /// <param name="radiusX">Corner Radius on the x axis</param>
        /// <param name="radiusY">Corner Radius on the y axis</param>
        /// <param name="stroke">CanvasStroke defining the stroke width, the stroke
        /// color and stroke style.</param>
        public static void DrawRoundedRectangle(this CanvasDrawingSession session, Rect rect, float radiusX, float radiusY, ICanvasStroke stroke)
        {
            session.DrawRoundedRectangle(rect, radiusX, radiusY, stroke.Brush, stroke.Width, stroke.Style);
        }

        /// <summary>
        /// Draws a Rounded Rectangle of the specified dimensions, using a CanvasStroke to define the stroke width, the stroke color and stroke style.
        /// </summary>
        /// <param name="session">CanvasDrawingSession</param>
        /// <param name="x">Offset of the top left corner of the  Rounded Rectangle on the x-axis</param>
        /// <param name="y">Offset of the top left corner of the  Rounded Rectangle on the y-axis</param>
        /// <param name="w">Width of the  Rounded Rectangle</param>
        /// <param name="h">Height of the  Rounded Rectangle</param>
        /// <param name="radiusX">Corner Radius on the x axis</param>
        /// <param name="radiusY">Corner Radius on the y axis</param>
        /// <param name="stroke">CanvasStroke defining the stroke width, the stroke
        /// color and stroke style.</param>
        public static void DrawRoundedRectangle(this CanvasDrawingSession session, float x, float y, float w, float h, float radiusX, float radiusY, ICanvasStroke stroke)
        {
            session.DrawRoundedRectangle(x, y, w, h, radiusX, radiusY, stroke.Brush, stroke.Width, stroke.Style);
        }

        /// <summary>
        /// Draws a Squircle of the specified dimensions, using a CanvasStroke to define the stroke width, the stroke color and stroke style.
        /// </summary>
        /// <param name="session">CanvasDrawingSession</param>
        /// <param name="x">Offset of the top left corner of the  Squircle on the x-axis</param>
        /// <param name="y">Offset of the top left corner of the  Squircle on the y-axis</param>
        /// <param name="w">Width of the  Squircle</param>
        /// <param name="h">Height of the  Squircle</param>
        /// <param name="radiusX">Corner Radius on the x axis</param>
        /// <param name="radiusY">Corner Radius on the y axis</param>
        /// <param name="stroke">CanvasStroke defining the stroke width, the stroke
        /// color and stroke style.</param>
        public static void DrawSquircle(this CanvasDrawingSession session, float x, float y, float w, float h, float radiusX, float radiusY, ICanvasStroke stroke)
        {
            using var geometry = CanvasPathGeometry.CreateSquircle(session.Device, x, y, w, h, radiusX, radiusY);
            session.DrawGeometry(geometry, stroke);
        }

        /// <summary>
        /// Draws a Squircle of the specified dimensions, using a CanvasStroke to define the stroke width, the stroke color and stroke style.
        /// </summary>
        /// <param name="session">CanvasDrawingSession</param>
        /// <param name="x">Offset of the top left corner of the  Squircle on the x-axis</param>
        /// <param name="y">Offset of the top left corner of the  Squircle on the y-axis</param>
        /// <param name="w">Width of the  Squircle</param>
        /// <param name="h">Height of the  Squircle</param>
        /// <param name="radiusX">Corner Radius on the x axis</param>
        /// <param name="radiusY">Corner Radius on the y axis</param>
        /// <param name="offset">Offset of the Squircle from the origin.</param>
        /// <param name="stroke">CanvasStroke defining the stroke width, the stroke
        /// color and stroke style.</param>
        public static void DrawSquircle(this CanvasDrawingSession session, float x, float y, float w, float h, float radiusX, float radiusY, Vector2 offset, ICanvasStroke stroke)
        {
            using var geometry = CanvasPathGeometry.CreateSquircle(session.Device, x, y, w, h, radiusX, radiusY);
            session.DrawGeometry(geometry, offset, stroke);
        }

        /// <summary>
        /// Fills a Squircle of the specified dimensions, using the given color.
        /// </summary>
        /// <param name="session">CanvasDrawingSession</param>
        /// <param name="x">Offset of the top left corner of the  Squircle on the x-axis</param>
        /// <param name="y">Offset of the top left corner of the  Squircle on the y-axis</param>
        /// <param name="w">Width of the  Squircle</param>
        /// <param name="h">Height of the  Squircle</param>
        /// <param name="radiusX">Corner Radius on the x axis</param>
        /// <param name="radiusY">Corner Radius on the y axis</param>
        /// <param name="color">Color to fill the Squircle.</param>
        public static void FillSquircle(this CanvasDrawingSession session, float x, float y, float w, float h, float radiusX, float radiusY, Color color)
        {
            using var geometry = CanvasPathGeometry.CreateSquircle(session.Device, x, y, w, h, radiusX, radiusY);
            session.FillGeometry(geometry, color);
        }

        /// <summary>
        /// Fills a Squircle of the specified dimensions, using the given brush.
        /// </summary>
        /// <param name="session">CanvasDrawingSession</param>
        /// <param name="x">Offset of the top left corner of the  Squircle on the x-axis</param>
        /// <param name="y">Offset of the top left corner of the  Squircle on the y-axis</param>
        /// <param name="w">Width of the  Squircle</param>
        /// <param name="h">Height of the  Squircle</param>
        /// <param name="radiusX">Corner Radius on the x axis</param>
        /// <param name="radiusY">Corner Radius on the y axis</param>
        /// <param name="brush">Brush to fill the Squircle.</param>
        public static void FillSquircle(this CanvasDrawingSession session, float x, float y, float w, float h, float radiusX, float radiusY, ICanvasBrush brush)
        {
            using var geometry = CanvasPathGeometry.CreateSquircle(session.Device, x, y, w, h, radiusX, radiusY);
            session.FillGeometry(geometry, brush);
        }

        /// <summary>
        /// Fills a Squircle of the specified dimensions, using the given color at specified offset.
        /// </summary>
        /// <param name="session">CanvasDrawingSession</param>
        /// <param name="x">Offset of the top left corner of the  Squircle on the x-axis</param>
        /// <param name="y">Offset of the top left corner of the  Squircle on the y-axis</param>
        /// <param name="w">Width of the  Squircle</param>
        /// <param name="h">Height of the  Squircle</param>
        /// <param name="radiusX">Corner Radius on the x axis</param>
        /// <param name="radiusY">Corner Radius on the y axis</param>
        /// <param name="offset">Offset of the Squircle from the origin.</param>
        /// <param name="color">Color to fill the Squircle.</param>
        public static void FillSquircle(this CanvasDrawingSession session, float x, float y, float w, float h, float radiusX, float radiusY, Vector2 offset, Color color)
        {
            using var geometry = CanvasPathGeometry.CreateSquircle(session.Device, x, y, w, h, radiusX, radiusY);
            session.FillGeometry(geometry, offset, color);
        }

        /// <summary>
        /// Fills a Squircle of the specified dimensions, using the given brush at specified offset.
        /// </summary>
        /// <param name="session">CanvasDrawingSession</param>
        /// <param name="x">Offset of the top left corner of the  Squircle on the x-axis</param>
        /// <param name="y">Offset of the top left corner of the  Squircle on the y-axis</param>
        /// <param name="w">Width of the  Squircle</param>
        /// <param name="h">Height of the  Squircle</param>
        /// <param name="radiusX">Corner Radius on the x axis</param>
        /// <param name="radiusY">Corner Radius on the y axis</param>
        /// <param name="offset">Offset of the Squircle from the origin.</param>
        /// <param name="brush">Brush to fill the Squircle.</param>
        public static void FillSquircle(this CanvasDrawingSession session, float x, float y, float w, float h, float radiusX, float radiusY, Vector2 offset, ICanvasBrush brush)
        {
            using var geometry = CanvasPathGeometry.CreateSquircle(session.Device, x, y, w, h, radiusX, radiusY);
            session.FillGeometry(geometry, offset, brush);
        }
    }
}