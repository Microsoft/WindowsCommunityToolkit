// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Rect = Windows.Foundation.Rect;

namespace Microsoft.Toolkit.Uwp
{
    /// <summary>
    /// Extensions for the <see cref="Rect"/> type.
    /// </summary>
    public static class RectExtensions
    {
        /// <summary>
        /// Determines if a rectangle intersects with another rectangle.
        /// </summary>
        /// <param name="rect1">The first rectangle to test.</param>
        /// <param name="rect2">The second rectangle to test.</param>
        /// <returns>This method returns <see langword="true"/> if there is any intersection, otherwise <see langword="false"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectsWith(this Rect rect1, Rect rect2)
        {
            if (rect1.IsEmpty || rect2.IsEmpty)
            {
                return false;
            }

            return (rect1.Left <= rect2.Right) &&
                   (rect1.Right >= rect2.Left) &&
                   (rect1.Top <= rect2.Bottom) &&
                   (rect1.Bottom >= rect2.Top);
        }
    }
}