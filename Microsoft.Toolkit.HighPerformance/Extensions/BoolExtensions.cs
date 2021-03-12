// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Microsoft.Toolkit.HighPerformance
{
    /// <summary>
    /// Helpers for working with the <see cref="bool"/> type.
    /// </summary>
    public static class BoolExtensions
    {
        /// <summary>
        /// Converts the given <see cref="bool"/> value into a <see cref="byte"/>.
        /// </summary>
        /// <param name="flag">The input value to convert.</param>
        /// <returns>1 if <paramref name="flag"/> is <see langword="true"/>, 0 otherwise.</returns>
        /// <remarks>This method does not contain branching instructions.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte ToByte(this bool flag)
        {
            // Whenever we need to take the address of an argument, we make a local copy first.
            // This will be removed by the JIT anyway, but it can help produce better codegen and
            // remove unwanted stack spills if the caller is using constant arguments. This is
            // because taking the address of an argument can interfere with some of the flow
            // analysis executed by the JIT, which can in some cases block constant propagation.
            bool copy = flag;

            return *(byte*)&copy;
        }

        /// <summary>
        /// Converts the given <see cref="bool"/> value to an <see cref="int"/> mask with
        /// all bits representing the value of the input flag (either 0xFFFFFFFF or 0x00000000).
        /// </summary>
        /// <param name="flag">The input value to convert.</param>
        /// <returns>0xFFFFFFFF if <paramref name="flag"/> is <see langword="true"/>, 0x00000000 otherwise.</returns>
        /// <remarks>
        /// This method does not contain branching instructions, and it is only guaranteed to work with
        /// <see cref="bool"/> values being either 0 or 1. Operations producing a <see cref="bool"/> result,
        /// such as numerical comparisons, always result in a valid value. If the <see cref="bool"/> value is
        /// produced by fields with a custom <see cref="System.Runtime.InteropServices.FieldOffsetAttribute"/>,
        /// or by using <see cref="Unsafe.As{T}"/> or other unsafe APIs to directly manipulate the underlying
        /// data though, it is responsibility of the caller to ensure the validity of the provided value.
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ToBitwiseMask32(this bool flag)
        {
            bool copy = flag;
            byte rangeFlag = *(byte*)&copy;
            int
                negativeFlag = rangeFlag - 1,
                mask = ~negativeFlag;

            return mask;
        }

        /// <summary>
        /// Converts the given <see cref="bool"/> value to a <see cref="long"/> mask with
        /// all bits representing the value of the input flag (either all 1s or 0s).
        /// </summary>
        /// <param name="flag">The input value to convert.</param>
        /// <returns>All 1s if <paramref name="flag"/> is <see langword="true"/>, all 0s otherwise.</returns>
        /// <remarks>This method does not contain branching instructions. See additional note in <see cref="ToBitwiseMask32"/>.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe long ToBitwiseMask64(this bool flag)
        {
            bool copy = flag;
            byte rangeFlag = *(byte*)&copy;
            long
                negativeFlag = (long)rangeFlag - 1,
                mask = ~negativeFlag;

            return mask;
        }
    }
}
