// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
#if NETCOREAPP3_1 || NET5_0
using static System.Numerics.BitOperations;
#endif

namespace CommunityToolkit.HighPerformance.Helpers.Internals
{
    /// <summary>
    /// Utility methods for intrinsic bit-twiddling operations. The methods use hardware intrinsics
    /// when available on the underlying platform, otherwise they use optimized software fallbacks.
    /// </summary>
    internal static class BitOperations
    {
        /// <summary>
        /// Rounds up an <see cref="int"/> value to a power of 2.
        /// </summary>
        /// <param name="x">The input value to round up.</param>
        /// <returns>The smallest power of two greater than or equal to <paramref name="x"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RoundUpPowerOfTwo(int x)
        {
#if NETCOREAPP3_1 || NET5_0
            return 1 << (32 - LeadingZeroCount((uint)(x - 1)));
#else
            x--;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            x++;

            return x;
#endif
        }
    }
}