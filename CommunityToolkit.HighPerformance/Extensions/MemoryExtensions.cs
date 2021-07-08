// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MemoryStream = CommunityToolkit.HighPerformance.Streams.MemoryStream;

namespace CommunityToolkit.HighPerformance
{
    /// <summary>
    /// Helpers for working with the <see cref="Memory{T}"/> type.
    /// </summary>
    public static class MemoryExtensions
    {
#if SPAN_RUNTIME_SUPPORT
        /// <summary>
        /// Returns a <see cref="Memory2D{T}"/> instance wrapping the underlying data for the given <see cref="Memory{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of items in the input <see cref="Memory{T}"/> instance.</typeparam>
        /// <param name="memory">The input <see cref="Memory{T}"/> instance.</param>
        /// <param name="height">The height of the resulting 2D area.</param>
        /// <param name="width">The width of each row in the resulting 2D area.</param>
        /// <returns>The resulting <see cref="Memory2D{T}"/> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when one of the input parameters is out of range.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the requested area is outside of bounds for <paramref name="memory"/>.
        /// </exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Memory2D<T> AsMemory2D<T>(this Memory<T> memory, int height, int width)
        {
            return new(memory, height, width);
        }

        /// <summary>
        /// Returns a <see cref="Memory2D{T}"/> instance wrapping the underlying data for the given <see cref="Memory{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of items in the input <see cref="Memory{T}"/> instance.</typeparam>
        /// <param name="memory">The input <see cref="Memory{T}"/> instance.</param>
        /// <param name="offset">The initial offset within <paramref name="memory"/>.</param>
        /// <param name="height">The height of the resulting 2D area.</param>
        /// <param name="width">The width of each row in the resulting 2D area.</param>
        /// <param name="pitch">The pitch in the resulting 2D area.</param>
        /// <returns>The resulting <see cref="Memory2D{T}"/> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when one of the input parameters is out of range.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the requested area is outside of bounds for <paramref name="memory"/>.
        /// </exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Memory2D<T> AsMemory2D<T>(this Memory<T> memory, int offset, int height, int width, int pitch)
        {
            return new(memory, offset, height, width, pitch);
        }
#endif

        /// <summary>
        /// Casts a <see cref="Memory{T}"/> of one primitive type <typeparamref name="T"/> to <see cref="Memory{T}"/> of bytes.
        /// </summary>
        /// <typeparam name="T">The type if items in the source <see cref="Memory{T}"/>.</typeparam>
        /// <param name="memory">The source <see cref="Memory{T}"/>, of type <typeparamref name="T"/>.</param>
        /// <returns>A <see cref="Memory{T}"/> of bytes.</returns>
        /// <exception cref="OverflowException">
        /// Thrown if the <see cref="Memory{T}.Length"/> property of the new <see cref="Memory{T}"/> would exceed <see cref="int.MaxValue"/>.
        /// </exception>
        /// <exception cref="ArgumentException">Thrown when the data store of <paramref name="memory"/> is not supported.</exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Memory<byte> AsBytes<T>(this Memory<T> memory)
            where T : unmanaged
        {
            return MemoryMarshal.AsMemory(((ReadOnlyMemory<T>)memory).Cast<T, byte>());
        }

        /// <summary>
        /// Casts a <see cref="Memory{T}"/> of one primitive type <typeparamref name="TFrom"/> to another primitive type <typeparamref name="TTo"/>.
        /// </summary>
        /// <typeparam name="TFrom">The type of items in the source <see cref="Memory{T}"/>.</typeparam>
        /// <typeparam name="TTo">The type of items in the destination <see cref="Memory{T}"/>.</typeparam>
        /// <param name="memory">The source <see cref="Memory{T}"/>, of type <typeparamref name="TFrom"/>.</param>
        /// <returns>A <see cref="Memory{T}"/> of type <typeparamref name="TTo"/></returns>
        /// <exception cref="ArgumentException">Thrown when the data store of <paramref name="memory"/> is not supported.</exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Memory<TTo> Cast<TFrom, TTo>(this Memory<TFrom> memory)
            where TFrom : unmanaged
            where TTo : unmanaged
        {
            return MemoryMarshal.AsMemory(((ReadOnlyMemory<TFrom>)memory).Cast<TFrom, TTo>());
        }

        /// <summary>
        /// Returns a <see cref="Stream"/> wrapping the contents of the given <see cref="Memory{T}"/> of <see cref="byte"/> instance.
        /// </summary>
        /// <param name="memory">The input <see cref="Memory{T}"/> of <see cref="byte"/> instance.</param>
        /// <returns>A <see cref="Stream"/> wrapping the data within <paramref name="memory"/>.</returns>
        /// <remarks>
        /// Since this method only receives a <see cref="Memory{T}"/> instance, which does not track
        /// the lifetime of its underlying buffer, it is responsibility of the caller to manage that.
        /// In particular, the caller must ensure that the target buffer is not disposed as long
        /// as the returned <see cref="Stream"/> is in use, to avoid unexpected issues.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown when <paramref name="memory"/> has an invalid data store.</exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stream AsStream(this Memory<byte> memory)
        {
            return MemoryStream.Create(memory, false);
        }
    }
}