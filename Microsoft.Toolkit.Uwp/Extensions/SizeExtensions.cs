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
    /// Extensions for the <see cref="Size"/> type.
    /// </summary>
    public static class SizeExtensions
    {
        /// <summary>
        /// Creates a new <see cref="Rect"/> of the specified size, starting at the origin.
        /// </summary>
        /// <param name="size">The input <see cref="Size"/> value to convert.</param>
        /// <returns>A <see cref="Rect"/> value of the specified size, starting at the origin.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect ToRect(this Size size)
        {
            return new Rect(0, 0, size.Width, size.Height);
        }

        /// <summary>
        /// Creates a new <see cref="Rect"/> of the specified size, starting at the given coordinates.
        /// </summary>
        /// <param name="size">The input <see cref="Size"/> value to convert.</param>
        /// <param name="x">The horizontal offset.</param>
        /// <param name="y">The vertical offset.</param>
        /// <returns>A <see cref="Rect"/> value of the specified size, starting at the given coordinates.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect ToRect(this Size size, double x, double y)
        {
            return new Rect(x, y, size.Width, size.Height);
        }

        /// <summary>
        /// Creates a new <see cref="Rect"/> of the specified size, starting at the given position.
        /// </summary>
        /// <param name="size">The input <see cref="Size"/> value to convert.</param>
        /// <param name="point">The starting position to use.</param>
        /// <returns>A <see cref="Rect"/> value of the specified size, starting at the given position.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect ToRect(this Size size, Point point)
        {
            return new Rect(point, size);
        }
    }
}