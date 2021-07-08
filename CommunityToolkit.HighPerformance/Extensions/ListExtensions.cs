// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET5_0

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CommunityToolkit.HighPerformance
{
    /// <summary>
    /// Helpers for working with the <see cref="List{T}"/> type.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Creates a new <see cref="Span{T}"/> over an input <see cref="List{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input <see cref="List{T}"/> instance.</typeparam>
        /// <param name="list">The input <see cref="List{T}"/> instance.</param>
        /// <returns>A <see cref="Span{T}"/> instance with the values of <paramref name="list"/>.</returns>
        /// <remarks>
        /// Note that the returned <see cref="Span{T}"/> is only guaranteed to be valid as long as the items within
        /// <paramref name="list"/> are not modified. Doing so might cause the <see cref="List{T}"/> to swap its
        /// internal buffer, causing the returned <see cref="Span{T}"/> to become out of date. That means that in this
        /// scenario, the <see cref="Span{T}"/> would end up wrapping an array no longer in use. Always make sure to use
        /// the returned <see cref="Span{T}"/> while the target <see cref="List{T}"/> is not modified.
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> AsSpan<T>(this List<T>? list)
        {
            return CollectionsMarshal.AsSpan(list);
        }
    }
}

#endif