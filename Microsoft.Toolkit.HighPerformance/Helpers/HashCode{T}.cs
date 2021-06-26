// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NETSTANDARD1_4

using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Toolkit.HighPerformance.Helpers.Internals;
#if SPAN_RUNTIME_SUPPORT
using RuntimeHelpers = System.Runtime.CompilerServices.RuntimeHelpers;
#else
using RuntimeHelpers = Microsoft.Toolkit.HighPerformance.Helpers.Internals.RuntimeHelpers;
#endif

namespace Microsoft.Toolkit.HighPerformance.Helpers
{
    /// <summary>
    /// Combines the hash code of sequences of <typeparamref name="T"/> values into a single hash code.
    /// </summary>
    /// <typeparam name="T">The type of values to hash.</typeparam>
    /// <remarks>
    /// The hash codes returned by the <see cref="Combine"/> method are only guaranteed to be repeatable for
    /// the current execution session, just like with the available <see cref="HashCode"/> APIs.In other words,
    /// hashing the same <see cref="ReadOnlySpan{T}"/> collection multiple times in the same process will always
    /// result in the same hash code, while the same collection being hashed again from another process
    /// (or another instance of the same process) is not guaranteed to result in the same final value.
    /// For more info, see <see href="https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode#remarks"/>.
    /// </remarks>
    public struct HashCode<T>
        where T : notnull
    {
        /// <summary>
        /// Gets a content hash from the input <see cref="ReadOnlySpan{T}"/> instance using the xxHash32 algorithm.
        /// </summary>
        /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance</param>
        /// <returns>The xxHash32 value for the input <see cref="ReadOnlySpan{T}"/> instance</returns>
        /// <remarks>The xxHash32 is only guaranteed to be deterministic within the scope of a single app execution</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine(ReadOnlySpan<T> span)
        {
            int hash = CombineValues(span);

            return HashCode.Combine(hash);
        }

        /// <summary>
        /// Gets a content hash from the input <see cref="ReadOnlySpan{T}"/> instance.
        /// </summary>
        /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance</param>
        /// <returns>The hash code for the input <see cref="ReadOnlySpan{T}"/> instance</returns>
        /// <remarks>The returned hash code is not processed through <see cref="HashCode"/> APIs.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int CombineValues(ReadOnlySpan<T> span)
        {
            ref T r0 = ref MemoryMarshal.GetReference(span);

            // If typeof(T) is not unmanaged, iterate over all the items one by one.
            // This check is always known in advance either by the JITter or by the AOT
            // compiler, so this branch will never actually be executed by the code.
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                return SpanHelper.GetDjb2HashCode(ref r0, (nint)(uint)span.Length);
            }

            // Get the info for the target memory area to process.
            // The line below is computing the total byte size for the span,
            // and we cast both input factors to uint first to avoid sign extensions
            // (they're both guaranteed to always be positive values), and to let the
            // JIT avoid the 64 bit computation entirely when running in a 32 bit
            // process. In that case it will just compute the byte size as a 32 bit
            // multiplication with overflow, which is guaranteed never to happen anyway.
            ref byte rb = ref Unsafe.As<T, byte>(ref r0);
            nint length = (nint)((uint)span.Length * (uint)Unsafe.SizeOf<T>());

            return SpanHelper.GetDjb2LikeByteHash(ref rb, length);
        }
    }
}

#endif