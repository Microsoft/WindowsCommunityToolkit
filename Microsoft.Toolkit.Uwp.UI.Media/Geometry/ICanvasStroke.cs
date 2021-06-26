// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;

namespace Microsoft.Toolkit.Uwp.UI.Media.Geometry
{
    /// <summary>
    /// Interface to represent the Stroke which can be used to render an outline on a <see cref="CanvasGeometry"/>.
    /// </summary>
    public interface ICanvasStroke
    {
        /// <summary>
        /// Gets or sets the brush with which the <see cref="CanvasStroke"/> will be rendered.
        /// </summary>
        ICanvasBrush Brush { get; set; }

        /// <summary>
        /// Gets or sets the width of the <see cref="CanvasStroke"/>.
        /// </summary>
        float Width { get; set; }

        /// <summary>
        /// Gets or sets the Style of the <see cref="CanvasStroke"/>.
        /// </summary>
        CanvasStrokeStyle Style { get; set; }

        /// <summary>
        /// Gets or sets transform matrix of the <see cref="CanvasStroke"/> brush.
        /// </summary>
        Matrix3x2 Transform { get; set; }
    }
}