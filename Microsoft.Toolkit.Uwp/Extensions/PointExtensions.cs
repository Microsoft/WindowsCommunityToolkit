// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Point = Windows.Foundation.Point;
using Rect = Windows.Foundation.Rect;
using Size = Windows.Foundation.Size;

namespace Microsoft.Toolkit.Uwp
{
    /// <summary>
    /// Extensions for the <see cref="Point"/> type.
    /// </summary>
    public static class PointExtensions
    {
        /// <summary>
        /// Creates a new <see cref="Rect"/> of the specified size, starting at a given point.
        /// </summary>
        /// <param name="point">The input <see cref="Point"/> value to convert.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <returns>A <see cref="Rect"/> value of the specified size, starting at the given point.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect ToRect(this Point point, double width, double height)
        {
            return new Rect(point.X, point.Y, width, height);
        }

        /// <summary>
        /// Creates a new <see cref="Rect"/> ending at the specified point, starting at the given coordinates.
        /// </summary>
        /// <param name="point">The input <see cref="Point"/> value to convert.</param>
        /// <param name="end">The ending position for the rectangle.</param>
        /// <returns>A <see cref="Rect"/> value between the two specified points.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect ToRect(this Point point, Point end)
        {
            return new Rect(point, end);
        }

        /// <summary>
        /// Creates a new <see cref="Rect"/> of the specified size, starting at the given coordinates.
        /// </summary>
        /// <param name="point">The input <see cref="Point"/> value to convert.</param>
        /// <param name="size">The size of the rectangle to create.</param>
        /// <returns>A <see cref="Rect"/> value of the specified size, starting at the given coordinates.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect ToRect(this Point point, Size size)
        {
            return new Rect(point, size);
        }
    }
}
