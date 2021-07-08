// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Versioning;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.UI.Composition;

namespace CommunityToolkit.WinUI.UI.Media.Geometry
{
    /// <summary>
    /// Extension methods for compositor to support Win2d Path Mini Language.
    /// </summary>
    public static class CompositorGeometryExtensions
    {
        /// <summary>
        /// Creates a <see cref="CompositionPath"/> based on the specified path data.
        /// </summary>
        /// <param name="compositor"><see cref="Compositor"/></param>
        /// <param name="pathData">Path data (Win2d Path Mini Language) in string format.</param>
        /// <returns><see cref="CompositionPath"/></returns>
        public static CompositionPath CreatePath(this Compositor compositor, string pathData)
        {
            // Create CanvasGeometry
            var geometry = CanvasPathGeometry.CreateGeometry(pathData);

            // Create CompositionPath
            return new CompositionPath(geometry);
        }

        /// <summary>
        /// Creates a <see cref="CompositionPathGeometry"/> based on the given path data.
        /// </summary>
        /// <param name="compositor"><see cref="Compositor"/></param>
        /// <param name="pathData">Path data (Win2d Path Mini Language) in string format.</param>
        /// <returns><see cref="CompositionPathGeometry"/></returns>
        public static CompositionPathGeometry CreatePathGeometry(this Compositor compositor, string pathData)
        {
            // Create CanvasGeometry
            var geometry = CanvasPathGeometry.CreateGeometry(pathData);

            // Create CompositionPathGeometry
            return compositor.CreatePathGeometry(new CompositionPath(geometry));
        }

        /// <summary>
        /// Creates a <see cref="CompositionSpriteShape"/> based on the specified path data.
        /// </summary>
        /// <param name="compositor"><see cref="Compositor"/></param>
        /// <param name="pathData">Path data (Win2d Path Mini Language) in string format.</param>
        /// <returns><see cref="CompositionSpriteShape"/></returns>
        public static CompositionSpriteShape CreateSpriteShape(this Compositor compositor, string pathData)
        {
            // Create CanvasGeometry
            var geometry = CanvasPathGeometry.CreateGeometry(pathData);

            // Create CompositionPathGeometry
            var pathGeometry = compositor.CreatePathGeometry(new CompositionPath(geometry));

            // Create CompositionSpriteShape
            return compositor.CreateSpriteShape(pathGeometry);
        }

        /// <summary>
        /// Creates a <see cref="CompositionGeometricClip"/> from the specified <see cref="CanvasGeometry"/>.
        /// </summary>
        /// <param name="compositor"><see cref="Compositor"/></param>
        /// <param name="geometry"><see cref="CanvasGeometry"/></param>
        /// <returns>CompositionGeometricClip</returns>
        public static CompositionGeometricClip CreateGeometricClip(this Compositor compositor, CanvasGeometry geometry)
        {
            // Create the CompositionPath
            var path = new CompositionPath(geometry);

            // Create the CompositionPathGeometry
            var pathGeometry = compositor.CreatePathGeometry(path);

            // Create the CompositionGeometricClip
            return compositor.CreateGeometricClip(pathGeometry);
        }

        /// <summary>
        /// Parses the specified path data and converts it to <see cref="CompositionGeometricClip"/>.
        /// </summary>
        /// <param name="compositor"><see cref="Compositor"/></param>
        /// <param name="pathData">Path data (Win2d Path Mini Language) in string format.</param>
        /// <returns><see cref="CompositionGeometricClip"/></returns>
        public static CompositionGeometricClip CreateGeometricClip(this Compositor compositor, string pathData)
        {
            // Create the CanvasGeometry from the path data
            var geometry = CanvasPathGeometry.CreateGeometry(pathData);

            // Create the CompositionGeometricClip
            return compositor.CreateGeometricClip(geometry);
        }
    }
}