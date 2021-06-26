// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace Microsoft.Toolkit.HighPerformance.Helpers.Internals
{
    /// <summary>
    /// Helpers to process sequences of values by reference with a given step.
    /// </summary>
    internal static class RefEnumerableHelper
    {
        /// <summary>
        /// Clears a target memory area.
        /// </summary>
        /// <typeparam name="T">The type of values to clear.</typeparam>
        /// <param name="r0">A <typeparamref name="T"/> reference to the start of the memory area.</param>
        /// <param name="length">The number of items in the memory area.</param>
        /// <param name="step">The number of items between each consecutive target value.</param>
        public static void Clear<T>(ref T r0, nint length, nint step)
        {
            nint offset = 0;

            // Main loop with 8 unrolled iterations
            while (length >= 8)
            {
                Unsafe.Add(ref r0, offset) = default!;
                Unsafe.Add(ref r0, offset += step) = default!;
                Unsafe.Add(ref r0, offset += step) = default!;
                Unsafe.Add(ref r0, offset += step) = default!;
                Unsafe.Add(ref r0, offset += step) = default!;
                Unsafe.Add(ref r0, offset += step) = default!;
                Unsafe.Add(ref r0, offset += step) = default!;
                Unsafe.Add(ref r0, offset += step) = default!;

                length -= 8;
                offset += step;
            }

            if (length >= 4)
            {
                Unsafe.Add(ref r0, offset) = default!;
                Unsafe.Add(ref r0, offset += step) = default!;
                Unsafe.Add(ref r0, offset += step) = default!;
                Unsafe.Add(ref r0, offset += step) = default!;

                length -= 4;
                offset += step;
            }

            // Clear the remaining values
            while (length > 0)
            {
                Unsafe.Add(ref r0, offset) = default!;

                length -= 1;
                offset += step;
            }
        }

        /// <summary>
        /// Copies a sequence of discontiguous items from one memory area to another.
        /// </summary>
        /// <typeparam name="T">The type of items to copy.</typeparam>
        /// <param name="sourceRef">The source reference to copy from.</param>
        /// <param name="destinationRef">The target reference to copy to.</param>
        /// <param name="length">The total number of items to copy.</param>
        /// <param name="sourceStep">The step between consecutive items in the memory area pointed to by <paramref name="sourceRef"/>.</param>
        public static void CopyTo<T>(ref T sourceRef, ref T destinationRef, nint length, nint sourceStep)
        {
            nint
                sourceOffset = 0,
                destinationOffset = 0;

            while (length >= 8)
            {
                Unsafe.Add(ref destinationRef, destinationOffset + 0) = Unsafe.Add(ref sourceRef, sourceOffset);
                Unsafe.Add(ref destinationRef, destinationOffset + 1) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);
                Unsafe.Add(ref destinationRef, destinationOffset + 2) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);
                Unsafe.Add(ref destinationRef, destinationOffset + 3) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);
                Unsafe.Add(ref destinationRef, destinationOffset + 4) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);
                Unsafe.Add(ref destinationRef, destinationOffset + 5) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);
                Unsafe.Add(ref destinationRef, destinationOffset + 6) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);
                Unsafe.Add(ref destinationRef, destinationOffset + 7) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);

                length -= 8;
                sourceOffset += sourceStep;
                destinationOffset += 8;
            }

            if (length >= 4)
            {
                Unsafe.Add(ref destinationRef, destinationOffset + 0) = Unsafe.Add(ref sourceRef, sourceOffset);
                Unsafe.Add(ref destinationRef, destinationOffset + 1) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);
                Unsafe.Add(ref destinationRef, destinationOffset + 2) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);
                Unsafe.Add(ref destinationRef, destinationOffset + 3) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);

                length -= 4;
                sourceOffset += sourceStep;
                destinationOffset += 4;
            }

            while (length > 0)
            {
                Unsafe.Add(ref destinationRef, destinationOffset) = Unsafe.Add(ref sourceRef, sourceOffset);

                length -= 1;
                sourceOffset += sourceStep;
                destinationOffset += 1;
            }
        }

        /// <summary>
        /// Copies a sequence of discontiguous items from one memory area to another.
        /// </summary>
        /// <typeparam name="T">The type of items to copy.</typeparam>
        /// <param name="sourceRef">The source reference to copy from.</param>
        /// <param name="destinationRef">The target reference to copy to.</param>
        /// <param name="length">The total number of items to copy.</param>
        /// <param name="sourceStep">The step between consecutive items in the memory area pointed to by <paramref name="sourceRef"/>.</param>
        /// <param name="destinationStep">The step between consecutive items in the memory area pointed to by <paramref name="destinationRef"/>.</param>
        public static void CopyTo<T>(ref T sourceRef, ref T destinationRef, nint length, nint sourceStep, nint destinationStep)
        {
            nint
                sourceOffset = 0,
                destinationOffset = 0;

            while (length >= 8)
            {
                Unsafe.Add(ref destinationRef, destinationOffset) = Unsafe.Add(ref sourceRef, sourceOffset);
                Unsafe.Add(ref destinationRef, destinationOffset += destinationStep) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);
                Unsafe.Add(ref destinationRef, destinationOffset += destinationStep) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);
                Unsafe.Add(ref destinationRef, destinationOffset += destinationStep) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);
                Unsafe.Add(ref destinationRef, destinationOffset += destinationStep) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);
                Unsafe.Add(ref destinationRef, destinationOffset += destinationStep) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);
                Unsafe.Add(ref destinationRef, destinationOffset += destinationStep) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);
                Unsafe.Add(ref destinationRef, destinationOffset += destinationStep) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);

                length -= 8;
                sourceOffset += sourceStep;
                destinationOffset += destinationStep;
            }

            if (length >= 4)
            {
                Unsafe.Add(ref destinationRef, destinationOffset) = Unsafe.Add(ref sourceRef, sourceOffset);
                Unsafe.Add(ref destinationRef, destinationOffset += destinationStep) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);
                Unsafe.Add(ref destinationRef, destinationOffset += destinationStep) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);
                Unsafe.Add(ref destinationRef, destinationOffset += destinationStep) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);

                length -= 4;
                sourceOffset += sourceStep;
                destinationOffset += destinationStep;
            }

            while (length > 0)
            {
                Unsafe.Add(ref destinationRef, destinationOffset) = Unsafe.Add(ref sourceRef, sourceOffset);

                length -= 1;
                sourceOffset += sourceStep;
                destinationOffset += destinationStep;
            }
        }

        /// <summary>
        /// Copies a sequence of discontiguous items from one memory area to another. This mirrors
        /// <see cref="CopyTo{T}(ref T,ref T,nint,nint)"/>, but <paramref name="sourceStep"/> refers to <paramref name="destinationRef"/> instead.
        /// </summary>
        /// <typeparam name="T">The type of items to copy.</typeparam>
        /// <param name="sourceRef">The source reference to copy from.</param>
        /// <param name="destinationRef">The target reference to copy to.</param>
        /// <param name="length">The total number of items to copy.</param>
        /// <param name="sourceStep">The step between consecutive items in the memory area pointed to by <paramref name="sourceRef"/>.</param>
        public static void CopyFrom<T>(ref T sourceRef, ref T destinationRef, nint length, nint sourceStep)
        {
            nint
                sourceOffset = 0,
                destinationOffset = 0;

            while (length >= 8)
            {
                Unsafe.Add(ref destinationRef, destinationOffset) = Unsafe.Add(ref sourceRef, sourceOffset);
                Unsafe.Add(ref destinationRef, destinationOffset += sourceStep) = Unsafe.Add(ref sourceRef, sourceOffset + 1);
                Unsafe.Add(ref destinationRef, destinationOffset += sourceStep) = Unsafe.Add(ref sourceRef, sourceOffset + 2);
                Unsafe.Add(ref destinationRef, destinationOffset += sourceStep) = Unsafe.Add(ref sourceRef, sourceOffset + 3);
                Unsafe.Add(ref destinationRef, destinationOffset += sourceStep) = Unsafe.Add(ref sourceRef, sourceOffset + 4);
                Unsafe.Add(ref destinationRef, destinationOffset += sourceStep) = Unsafe.Add(ref sourceRef, sourceOffset + 5);
                Unsafe.Add(ref destinationRef, destinationOffset += sourceStep) = Unsafe.Add(ref sourceRef, sourceOffset + 6);
                Unsafe.Add(ref destinationRef, destinationOffset += sourceStep) = Unsafe.Add(ref sourceRef, sourceOffset + 7);

                length -= 8;
                sourceOffset += 8;
                destinationOffset += sourceStep;
            }

            if (length >= 4)
            {
                Unsafe.Add(ref destinationRef, destinationOffset) = Unsafe.Add(ref sourceRef, sourceOffset);
                Unsafe.Add(ref destinationRef, destinationOffset += sourceStep) = Unsafe.Add(ref sourceRef, sourceOffset + 1);
                Unsafe.Add(ref destinationRef, destinationOffset += sourceStep) = Unsafe.Add(ref sourceRef, sourceOffset + 2);
                Unsafe.Add(ref destinationRef, destinationOffset += sourceStep) = Unsafe.Add(ref sourceRef, sourceOffset + 3);

                length -= 4;
                sourceOffset += 4;
                destinationOffset += sourceStep;
            }

            while (length > 0)
            {
                Unsafe.Add(ref destinationRef, destinationOffset) = Unsafe.Add(ref sourceRef, sourceOffset);

                length -= 1;
                sourceOffset += 1;
                destinationOffset += sourceStep;
            }
        }

        /// <summary>
        /// Fills a target memory area.
        /// </summary>
        /// <typeparam name="T">The type of values to fill.</typeparam>
        /// <param name="r0">A <typeparamref name="T"/> reference to the start of the memory area.</param>
        /// <param name="length">The number of items in the memory area.</param>
        /// <param name="step">The number of items between each consecutive target value.</param>
        /// <param name="value">The value to assign to every item in the target memory area.</param>
        public static void Fill<T>(ref T r0, nint length, nint step, T value)
        {
            nint offset = 0;

            while (length >= 8)
            {
                Unsafe.Add(ref r0, offset) = value;
                Unsafe.Add(ref r0, offset += step) = value;
                Unsafe.Add(ref r0, offset += step) = value;
                Unsafe.Add(ref r0, offset += step) = value;
                Unsafe.Add(ref r0, offset += step) = value;
                Unsafe.Add(ref r0, offset += step) = value;
                Unsafe.Add(ref r0, offset += step) = value;
                Unsafe.Add(ref r0, offset += step) = value;

                length -= 8;
                offset += step;
            }

            if (length >= 4)
            {
                Unsafe.Add(ref r0, offset) = value;
                Unsafe.Add(ref r0, offset += step) = value;
                Unsafe.Add(ref r0, offset += step) = value;
                Unsafe.Add(ref r0, offset += step) = value;

                length -= 4;
                offset += step;
            }

            while (length > 0)
            {
                Unsafe.Add(ref r0, offset) = value;

                length -= 1;
                offset += step;
            }
        }
    }
}