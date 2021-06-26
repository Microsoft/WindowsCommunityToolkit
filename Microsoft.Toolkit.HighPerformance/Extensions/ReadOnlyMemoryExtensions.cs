// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Toolkit.HighPerformance.Buffers.Internals;
using Microsoft.Toolkit.HighPerformance.Buffers.Internals.Interfaces;
using MemoryStream = Microsoft.Toolkit.HighPerformance.Streams.MemoryStream;

namespace Microsoft.Toolkit.HighPerformance
{
    /// <summary>
    /// Helpers for working with the <see cref="ReadOnlyMemory{T}"/> type.
    /// </summary>
    public static class ReadOnlyMemoryExtensions
    {
#if SPAN_RUNTIME_SUPPORT
        /// <summary>
        /// Returns a <see cref="ReadOnlyMemory2D{T}"/> instance wrapping the underlying data for the given <see cref="ReadOnlyMemory{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of items in the input <see cref="ReadOnlyMemory{T}"/> instance.</typeparam>
        /// <param name="memory">The input <see cref="ReadOnlyMemory{T}"/> instance.</param>
        /// <param name="height">The height of the resulting 2D area.</param>
        /// <param name="width">The width of each row in the resulting 2D area.</param>
        /// <returns>The resulting <see cref="ReadOnlyMemory2D{T}"/> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when one of the input parameters is out of range.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the requested area is outside of bounds for <paramref name="memory"/>.
        /// </exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlyMemory2D<T> AsMemory2D<T>(this ReadOnlyMemory<T> memory, int height, int width)
        {
            return new(memory, height, width);
        }

        /// <summary>
        /// Returns a <see cref="ReadOnlyMemory2D{T}"/> instance wrapping the underlying data for the given <see cref="ReadOnlyMemory{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of items in the input <see cref="ReadOnlyMemory{T}"/> instance.</typeparam>
        /// <param name="memory">The input <see cref="ReadOnlyMemory{T}"/> instance.</param>
        /// <param name="offset">The initial offset within <paramref name="memory"/>.</param>
        /// <param name="height">The height of the resulting 2D area.</param>
        /// <param name="width">The width of each row in the resulting 2D area.</param>
        /// <param name="pitch">The pitch in the resulting 2D area.</param>
        /// <returns>The resulting <see cref="ReadOnlyMemory2D{T}"/> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when one of the input parameters is out of range.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the requested area is outside of bounds for <paramref name="memory"/>.
        /// </exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlyMemory2D<T> AsMemory2D<T>(this ReadOnlyMemory<T> memory, int offset, int height, int width, int pitch)
        {
            return new(memory, offset, height, width, pitch);
        }
#endif

        /// <summary>
        /// Casts a <see cref="ReadOnlyMemory{T}"/> of one primitive type <typeparamref name="T"/> to <see cref="ReadOnlyMemory{T}"/> of bytes.
        /// </summary>
        /// <typeparam name="T">The type if items in the source <see cref="ReadOnlyMemory{T}"/>.</typeparam>
        /// <param name="memory">The source <see cref="ReadOnlyMemory{T}"/>, of type <typeparamref name="T"/>.</param>
        /// <returns>A <see cref="ReadOnlyMemory{T}"/> of bytes.</returns>
        /// <exception cref="OverflowException">
        /// Thrown if the <see cref="ReadOnlyMemory{T}.Length"/> property of the new <see cref="ReadOnlyMemory{T}"/> would exceed <see cref="int.MaxValue"/>.
        /// </exception>
        /// <exception cref="ArgumentException">Thrown when the data store of <paramref name="memory"/> is not supported.</exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlyMemory<byte> AsBytes<T>(this ReadOnlyMemory<T> memory)
            where T : unmanaged
        {
            return Cast<T, byte>(memory);
        }

        /// <summary>
        /// Casts a <see cref="ReadOnlyMemory{T}"/> of one primitive type <typeparamref name="TFrom"/> to another primitive type <typeparamref name="TTo"/>.
        /// </summary>
        /// <typeparam name="TFrom">The type of items in the source <see cref="ReadOnlyMemory{T}"/>.</typeparam>
        /// <typeparam name="TTo">The type of items in the destination <see cref="ReadOnlyMemory{T}"/>.</typeparam>
        /// <param name="memory">The source <see cref="ReadOnlyMemory{T}"/>, of type <typeparamref name="TFrom"/>.</param>
        /// <returns>A <see cref="ReadOnlyMemory{T}"/> of type <typeparamref name="TTo"/></returns>
        /// <exception cref="ArgumentException">Thrown when the data store of <paramref name="memory"/> is not supported.</exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlyMemory<TTo> Cast<TFrom, TTo>(this ReadOnlyMemory<TFrom> memory)
            where TFrom : unmanaged
            where TTo : unmanaged
        {
            if (memory.IsEmpty)
            {
                return default;
            }

            if (typeof(TFrom) == typeof(char) &&
                MemoryMarshal.TryGetString((ReadOnlyMemory<char>)(object)memory, out string? text, out int start, out int length))
            {
                return new StringMemoryManager<TTo>(text!, start, length).Memory;
            }

            if (MemoryMarshal.TryGetArray(memory, out ArraySegment<TFrom> segment))
            {
                return new ArrayMemoryManager<TFrom, TTo>(segment.Array!, segment.Offset, segment.Count).Memory;
            }

            if (MemoryMarshal.TryGetMemoryManager<TFrom, MemoryManager<TFrom>>(memory, out var memoryManager, out start, out length))
            {
                // If the memory manager is the one resulting from a previous cast, we can use it directly to retrieve
                // a new manager for the target type that wraps the original data store, instead of creating one that
                // wraps the current manager. This ensures that doing repeated casts always results in only up to one
                // indirection level in the chain of memory managers needed to access the target data buffer to use.
                if (memoryManager is IMemoryManager wrappingManager)
                {
                    return wrappingManager.GetMemory<TTo>(start, length);
                }

                return new ProxyMemoryManager<TFrom, TTo>(memoryManager, start, length).Memory;
            }

            // Throws when the memory instance has an unsupported backing store
            static ReadOnlyMemory<TTo> ThrowArgumentExceptionForUnsupportedMemory()
            {
                throw new ArgumentException("The input instance doesn't have a supported underlying data store.");
            }

            return ThrowArgumentExceptionForUnsupportedMemory();
        }

        /// <summary>
        /// Returns a <see cref="Stream"/> wrapping the contents of the given <see cref="ReadOnlyMemory{T}"/> of <see cref="byte"/> instance.
        /// </summary>
        /// <param name="memory">The input <see cref="ReadOnlyMemory{T}"/> of <see cref="byte"/> instance.</param>
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
        public static Stream AsStream(this ReadOnlyMemory<byte> memory)
        {
            return MemoryStream.Create(memory, true);
        }
    }
}