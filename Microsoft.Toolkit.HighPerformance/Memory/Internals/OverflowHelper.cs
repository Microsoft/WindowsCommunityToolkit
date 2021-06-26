// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using static System.Math;

namespace Microsoft.Toolkit.HighPerformance.Memory.Internals
{
    /// <summary>
    /// A helper to validate arithmetic operations for <see cref="Memory2D{T}"/> and <see cref="Span2D{T}"/>.
    /// </summary>
    internal static class OverflowHelper
    {
        /// <summary>
        /// Ensures that the input parameters will not exceed the maximum native int value when indexing.
        /// </summary>
        /// <param name="height">The height of the 2D memory area to map.</param>
        /// <param name="width">The width of the 2D memory area to map.</param>
        /// <param name="pitch">The pitch of the 2D memory area to map (the distance between each row).</param>
        /// <exception cref="OverflowException">Throw when the inputs don't fit in the expected range.</exception>
        /// <remarks>The input parameters are assumed to always be positive.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureIsInNativeIntRange(int height, int width, int pitch)
        {
            // As per the layout used in the Memory2D<T> and Span2D<T> types, we have the
            // following memory representation with respect to height, width and pitch:
            //
            //               _________width_________  ________...
            //              /                       \/
            // | -- | -- | -- | -- | -- | -- | -- | -- | -- | -- |_
            // | -- | -- | XX | XX | XX | XX | XX | XX | -- | -- | |
            // | -- | -- | XX | XX | XX | XX | XX | XX | -- | -- | |
            // | -- | -- | XX | XX | XX | XX | XX | XX | -- | -- | |_height
            // | -- | -- | XX | XX | XX | XX | XX | XX | -- | -- |_|
            // | -- | -- | -- | -- | -- | -- | -- | -- | -- | -- |
            // | -- | -- | -- | -- | -- | -- | -- | -- | -- | -- |
            // ...__pitch__/
            //
            // The indexing logic works on nint values in unchecked mode, with no overflow checks,
            // which means it relies on the maximum element index to always be within <= nint.MaxValue.
            // To ensure no overflows will ever occur there, we need to ensure that no instance can be
            // created with parameters that could cause an overflow in case any item was accessed, so we
            // need to ensure no overflows occurs when calculating the index of the last item in each view.
            // The logic below calculates that index with overflow checks, throwing if one is detected.
            // Note that we're subtracting 1 to the height as we don't want to include the trailing pitch
            // for the 2D memory area, and also 1 to the width as the index is 0-based, as usual.
            // Additionally, we're also ensuring that the stride is never greater than int.MaxValue, for
            // consistency with how ND arrays work (int.MaxValue as upper bound for each axis), and to
            // allow for faster iteration in the RefEnumerable<T> type, when traversing columns.
            _ = checked(((nint)(width + pitch) * Max(unchecked(height - 1), 0)) + Max(unchecked(width - 1), 0));
        }

        /// <summary>
        /// Ensures that the input parameters will not exceed <see cref="int.MaxValue"/> when indexing.
        /// </summary>
        /// <param name="height">The height of the 2D memory area to map.</param>
        /// <param name="width">The width of the 2D memory area to map.</param>
        /// <param name="pitch">The pitch of the 2D memory area to map (the distance between each row).</param>
        /// <returns>The area resulting from the given parameters.</returns>
        /// <exception cref="OverflowException">Throw when the inputs don't fit in the expected range.</exception>
        /// <remarks>The input parameters are assumed to always be positive.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ComputeInt32Area(int height, int width, int pitch)
        {
            return checked(((width + pitch) * Max(unchecked(height - 1), 0)) + width);
        }
    }
}