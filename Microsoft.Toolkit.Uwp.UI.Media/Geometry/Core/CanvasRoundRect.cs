// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Numerics;

namespace Microsoft.Toolkit.Uwp.UI.Media.Geometry.Core
{
    /// <summary>
    /// Structure which encapsulates the details of each of the core points  of the path of the rounded rectangle which is calculated based on
    /// either the given (Size, CornerRadius, BorderThickness and Padding) or (Size, RadiusX and RadiusY).
    /// </summary>
    internal struct CanvasRoundRect
    {
        private const float Factor = 0.5f;

        private readonly float _leftTopWidth;
        private readonly float _topLeftHeight;
        private readonly float _topRightHeight;
        private readonly float _rightTopWidth;
        private readonly float _rightBottomWidth;
        private readonly float _bottomRightHeight;
        private readonly float _bottomLeftHeight;
        private readonly float _leftBottomWidth;

        // This is the location of the properties within the Rect
        //   |--LeftTop----------------------RightTop--|
        //   |                                         |
        // TopLeft                                TopRight
        //   |                                         |
        //   |                                         |
        //   |                                         |
        //   |                                         |
        //   |                                         |
        //   |                                         |
        // BottomLeft                          BottomRight
        //   |                                         |
        //   |--LeftBottom----------------RightBottom--|
        internal float LeftTopX { get; private set; }

        internal float LeftTopY { get; private set; }

        internal float TopLeftX { get; private set; }

        internal float TopLeftY { get; private set; }

        internal float TopRightX { get; private set; }

        internal float TopRightY { get; private set; }

        internal float RightTopX { get; private set; }

        internal float RightTopY { get; private set; }

        internal float RightBottomX { get; private set; }

        internal float RightBottomY { get; private set; }

        internal float BottomRightX { get; private set; }

        internal float BottomRightY { get; private set; }

        internal float BottomLeftX { get; private set; }

        internal float BottomLeftY { get; private set; }

        internal float LeftBottomX { get; private set; }

        internal float LeftBottomY { get; private set; }

        internal float Width { get; }

        internal float Height { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasRoundRect"/> struct.
        /// </summary>
        /// <param name="origin">Origin of the Rect (absolute location of Top Left corner)</param>
        /// <param name="size">Size of the Rect</param>
        /// <param name="cornerRadius">CornerRadius</param>
        /// <param name="borderThickness">BorderThickness</param>
        /// <param name="padding">Padding</param>
        /// <param name="isOuterBorder">Flag to indicate whether outer or inner border needs
        /// to be calculated</param>
        internal CanvasRoundRect(Vector2 origin, Vector2 size, Vector4 cornerRadius, Vector4 borderThickness, Vector4 padding, bool isOuterBorder)
            : this()
        {
            Width = Math.Max(0f, size.X);
            Height = Math.Max(0f, size.Y);

            var left = Factor * (borderThickness.X + padding.X);
            var top = Factor * (borderThickness.Y + padding.Y);
            var right = Factor * (borderThickness.Z + padding.Z);
            var bottom = Factor * (borderThickness.W + padding.W);

            if (isOuterBorder)
            {
                // Top Left corner radius
                if (cornerRadius.X.IsZero())
                {
                    _leftTopWidth = _topLeftHeight = 0f;
                }
                else
                {
                    _leftTopWidth = cornerRadius.X + left;
                    _topLeftHeight = cornerRadius.X + top;
                }

                // Top Right corner radius
                if (cornerRadius.Y.IsZero())
                {
                    _topRightHeight = _rightTopWidth = 0f;
                }
                else
                {
                    _topRightHeight = cornerRadius.Y + top;
                    _rightTopWidth = cornerRadius.Y + right;
                }

                // Bottom Right corner radius
                if (cornerRadius.Z.IsZero())
                {
                    _rightBottomWidth = _bottomRightHeight = 0f;
                }
                else
                {
                    _rightBottomWidth = cornerRadius.Z + right;
                    _bottomRightHeight = cornerRadius.Z + bottom;
                }

                // Bottom Left corner radius
                if (cornerRadius.W.IsZero())
                {
                    _bottomLeftHeight = _leftBottomWidth = 0f;
                }
                else
                {
                    _bottomLeftHeight = cornerRadius.W + bottom;
                    _leftBottomWidth = cornerRadius.W + left;
                }
            }
            else
            {
                _leftTopWidth = Math.Max(0f, cornerRadius.X - left);
                _topLeftHeight = Math.Max(0f, cornerRadius.X - top);
                _topRightHeight = Math.Max(0f, cornerRadius.Y - top);
                _rightTopWidth = Math.Max(0f, cornerRadius.Y - right);
                _rightBottomWidth = Math.Max(0f, cornerRadius.Z - right);
                _bottomRightHeight = Math.Max(0f, cornerRadius.Z - bottom);
                _bottomLeftHeight = Math.Max(0f, cornerRadius.W - bottom);
                _leftBottomWidth = Math.Max(0f, cornerRadius.W - left);
            }

            // Calculate the anchor points
            ComputeCoordinates(origin.X, origin.Y);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasRoundRect"/> struct.
        /// </summary>
        /// <param name="origin">Top Left corner of the Rounded Rectangle</param>
        /// <param name="size">Dimensions of the Rounded Rectangle</param>
        /// <param name="radiusX">Radius of the corners on the x-axis</param>
        /// <param name="radiusY">Radius of the corners on the y-axis</param>
        internal CanvasRoundRect(Vector2 origin, Vector2 size, float radiusX, float radiusY)
            : this(origin.X, origin.Y, size.X, size.Y, radiusX, radiusY)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasRoundRect"/> struct.
        /// </summary>
        /// <param name="x">X offset of the Top Left corner of the Rounded Rectangle</param>
        /// <param name="y">Y offset of the Top Left corner of the Rounded Rectangle</param>
        /// <param name="width">Width of the Rounded Rectangle.</param>
        /// <param name="height">Height of the Rounded Rectangle.</param>
        /// <param name="radiusX">Radius of the corners on the x-axis</param>
        /// <param name="radiusY">Radius of the corners on the y-axis</param>
        internal CanvasRoundRect(float x, float y, float width, float height, float radiusX, float radiusY)
            : this()
        {
            Width = Math.Max(0f, width);
            Height = Math.Max(0f, height);

            // Sanitize the radii by taking the absolute value
            radiusX = Math.Min(Math.Abs(radiusX), width / 2f);
            radiusY = Math.Min(Math.Abs(radiusY), height / 2);

            _leftTopWidth = radiusX;
            _rightTopWidth = radiusX;
            _rightBottomWidth = radiusX;
            _leftBottomWidth = radiusX;
            _topLeftHeight = radiusY;
            _topRightHeight = radiusY;
            _bottomRightHeight = radiusY;
            _bottomLeftHeight = radiusY;

            ComputeCoordinates(x, y);
        }

        /// <summary>
        /// Computes the coordinates of the crucial points on the CanvasRoundRect
        /// </summary>
        /// <param name="originX">X coordinate of the origin.</param>
        /// <param name="originY">Y coordinate of the origin.</param>
        private void ComputeCoordinates(float originX, float originY)
        {
            // compute the coordinates of the key points
            var leftTopX = _leftTopWidth;
            var leftTopY = 0f;
            var rightTopX = Width - _rightTopWidth;
            var rightTopY = 0f;
            var topRightX = Width;
            var topRightY = _topRightHeight;
            var bottomRightX = Width;
            var bottomRightY = Height - _bottomRightHeight;
            var rightBottomX = Width - _rightBottomWidth;
            var rightBottomY = Height;
            var leftBottomX = _leftBottomWidth;
            var leftBottomY = Height;
            var bottomLeftX = 0f;
            var bottomLeftY = Height - _bottomLeftHeight;
            var topLeftX = 0f;
            var topLeftY = _topLeftHeight;

            // check anchors for overlap and resolve by partitioning corners according to
            // the percentage of each one.
            // top edge
            if (leftTopX > rightTopX)
            {
                var v = _leftTopWidth / (_leftTopWidth + _rightTopWidth) * Width;
                leftTopX = v;
                rightTopX = v;
            }

            // right edge
            if (topRightY > bottomRightY)
            {
                var v = _topRightHeight / (_topRightHeight + _bottomRightHeight) * Height;
                topRightY = v;
                bottomRightY = v;
            }

            // bottom edge
            if (leftBottomX > rightBottomX)
            {
                var v = _leftBottomWidth / (_leftBottomWidth + _rightBottomWidth) * Width;
                rightBottomX = v;
                leftBottomX = v;
            }

            // left edge
            if (topLeftY > bottomLeftY)
            {
                var v = _topLeftHeight / (_topLeftHeight + _bottomLeftHeight) * Height;
                bottomLeftY = v;
                topLeftY = v;
            }

            // Apply origin translation
            LeftTopX = leftTopX + originX;
            LeftTopY = leftTopY + originY;
            RightTopX = rightTopX + originX;
            RightTopY = rightTopY + originY;
            TopRightX = topRightX + originX;
            TopRightY = topRightY + originY;
            BottomRightX = bottomRightX + originX;
            BottomRightY = bottomRightY + originY;
            RightBottomX = rightBottomX + originX;
            RightBottomY = rightBottomY + originY;
            LeftBottomX = leftBottomX + originX;
            LeftBottomY = leftBottomY + originY;
            BottomLeftX = bottomLeftX + originX;
            BottomLeftY = bottomLeftY + originY;
            TopLeftX = topLeftX + originX;
            TopLeftY = topLeftY + originY;
        }
    }
}