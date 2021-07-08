// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Windows.UI;

namespace CommunityToolkit.WinUI.UI.Media.Geometry
{
    /// <summary>
    /// Class to represent the Stroke which can be used to render an outline on a <see cref="CanvasGeometry"/>
    /// </summary>
    public sealed class CanvasStroke : ICanvasStroke
    {
        /// <summary>
        /// Gets or sets the brush with which the stroke will be rendered
        /// </summary>
        public ICanvasBrush Brush { get; set; }

        /// <summary>
        /// Gets or sets the width of the <see cref="CanvasStroke"/>
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Gets or sets the Style of the <see cref="CanvasStroke"/>
        /// </summary>
        public CanvasStrokeStyle Style { get; set; }

        /// <summary>
        /// Gets or sets the Transform matrix of the <see cref="CanvasStroke"/> brush.
        /// </summary>
        public Matrix3x2 Transform
        {
            get => GetTransform();

            set => SetTransform(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasStroke"/> class.
        /// </summary>
        /// <param name="brush">The brush with which the <see cref="CanvasStroke"/> will be rendered</param>
        /// <param name="strokeWidth">Width of the <see cref="CanvasStroke"/></param>
        public CanvasStroke(ICanvasBrush brush, float strokeWidth = 1f)
            : this(brush, strokeWidth, new CanvasStrokeStyle())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasStroke"/> class.
        /// </summary>
        /// <param name="brush">The brush with which the <see cref="CanvasStroke"/> will be rendered</param>
        /// <param name="strokeWidth">Width of the <see cref="CanvasStroke"/></param>
        /// <param name="strokeStyle">Style of the <see cref="CanvasStroke"/></param>
        public CanvasStroke(ICanvasBrush brush, float strokeWidth, CanvasStrokeStyle strokeStyle)
        {
            Brush = brush;
            Width = strokeWidth;
            Style = strokeStyle;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasStroke"/> class.
        /// </summary>
        /// <param name="device">ICanvasResourceCreator</param>
        /// <param name="strokeColor">Color of the <see cref="CanvasStroke"/></param>
        /// <param name="strokeWidth">Width of the <see cref="CanvasStroke"/></param>
        public CanvasStroke(ICanvasResourceCreator device, Color strokeColor, float strokeWidth = 1f)
            : this(device, strokeColor, strokeWidth, new CanvasStrokeStyle())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasStroke"/> class.
        /// </summary>
        /// <param name="device">ICanvasResourceCreator</param>
        /// <param name="strokeColor">Color of the <see cref="CanvasStroke"/></param>
        /// <param name="strokeWidth">Width of the <see cref="CanvasStroke"/></param>
        /// <param name="strokeStyle">Style of the <see cref="CanvasStroke"/></param>
        public CanvasStroke(ICanvasResourceCreator device, Color strokeColor, float strokeWidth, CanvasStrokeStyle strokeStyle)
        {
            Brush = new CanvasSolidColorBrush(device, strokeColor);
            Width = strokeWidth;
            Style = strokeStyle;
        }

        /// <summary>
        /// Sets the <see cref="CanvasStroke"/>'s Transform.
        /// </summary>
        /// <param name="value">Transform matrix to set</param>
        private void SetTransform(Matrix3x2 value)
        {
            if (Brush != null)
            {
                Brush.Transform = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="CanvasStroke"/>'s Transform. If stroke is null, then returns Matrix3x2.Identity.
        /// </summary>
        /// <returns>Transform matrix of the <see cref="CanvasStroke"/></returns>
        private Matrix3x2 GetTransform()
        {
            return Brush?.Transform ?? Matrix3x2.Identity;
        }
    }
}