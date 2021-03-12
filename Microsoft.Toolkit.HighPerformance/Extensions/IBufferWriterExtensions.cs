// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Toolkit.HighPerformance.Buffers;
using Microsoft.Toolkit.HighPerformance.Streams;

namespace Microsoft.Toolkit.HighPerformance
{
    /// <summary>
    /// Helpers for working with the <see cref="IBufferWriter{T}"/> type.
    /// </summary>
    public static class IBufferWriterExtensions
    {
        /// <summary>
        /// Returns a <see cref="Stream"/> that can be used to write to a target an <see cref="IBufferWriter{T}"/> of <see cref="byte"/> instance.
        /// </summary>
        /// <param name="writer">The target <see cref="IBufferWriter{T}"/> instance.</param>
        /// <returns>A <see cref="Stream"/> wrapping <paramref name="writer"/> and writing data to its underlying buffer.</returns>
        /// <remarks>The returned <see cref="Stream"/> can only be written to and does not support seeking.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stream AsStream(this IBufferWriter<byte> writer)
        {
            if (writer.GetType() == typeof(ArrayPoolBufferWriter<byte>))
            {
                // If the input writer is of type ArrayPoolBufferWriter<byte>, we can use the type
                // specific buffer writer owner to let the JIT elide callvirts when accessing it.
                var internalWriter = Unsafe.As<ArrayPoolBufferWriter<byte>>(writer)!;

                return new IBufferWriterStream<ArrayBufferWriterOwner>(new ArrayBufferWriterOwner(internalWriter));
            }

            return new IBufferWriterStream<IBufferWriterOwner>(new IBufferWriterOwner(writer));
        }

        /// <summary>
        /// Writes a value of a specified type into a target <see cref="IBufferWriter{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of value to write.</typeparam>
        /// <param name="writer">The target <see cref="IBufferWriter{T}"/> instance to write to.</param>
        /// <param name="value">The input value to write to <paramref name="writer"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="writer"/> reaches the end.</exception>
        public static void Write<T>(this IBufferWriter<byte> writer, T value)
            where T : unmanaged
        {
            int length = Unsafe.SizeOf<T>();
            Span<byte> span = writer.GetSpan(1);

            if (span.Length < length)
            {
                ThrowArgumentExceptionForEndOfBuffer();
            }

            ref byte r0 = ref MemoryMarshal.GetReference(span);

            Unsafe.WriteUnaligned(ref r0, value);

            writer.Advance(length);
        }

        /// <summary>
        /// Writes a value of a specified type into a target <see cref="IBufferWriter{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of value to write.</typeparam>
        /// <param name="writer">The target <see cref="IBufferWriter{T}"/> instance to write to.</param>
        /// <param name="value">The input value to write to <paramref name="writer"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="writer"/> reaches the end.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<T>(this IBufferWriter<T> writer, T value)
        {
            Span<T> span = writer.GetSpan(1);

            if (span.Length < 1)
            {
                ThrowArgumentExceptionForEndOfBuffer();
            }

            MemoryMarshal.GetReference(span) = value;

            writer.Advance(1);
        }

        /// <summary>
        /// Writes a series of items of a specified type into a target <see cref="IBufferWriter{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of value to write.</typeparam>
        /// <param name="writer">The target <see cref="IBufferWriter{T}"/> instance to write to.</param>
        /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> to write to <paramref name="writer"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="writer"/> reaches the end.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<T>(this IBufferWriter<byte> writer, ReadOnlySpan<T> span)
            where T : unmanaged
        {
            ReadOnlySpan<byte> source = MemoryMarshal.AsBytes(span);
            Span<byte> destination = writer.GetSpan(source.Length);

            source.CopyTo(destination);

            writer.Advance(source.Length);
        }

#if !SPAN_RUNTIME_SUPPORT
        /// <summary>
        /// Writes a series of items of a specified type into a target <see cref="IBufferWriter{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of value to write.</typeparam>
        /// <param name="writer">The target <see cref="IBufferWriter{T}"/> instance to write to.</param>
        /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> to write to <paramref name="writer"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="writer"/> reaches the end.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<T>(this IBufferWriter<T> writer, ReadOnlySpan<T> span)
        {
            Span<T> destination = writer.GetSpan(span.Length);

            span.CopyTo(destination);

            writer.Advance(span.Length);
        }
#endif

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when trying to write too many bytes to the target writer.
        /// </summary>
        private static void ThrowArgumentExceptionForEndOfBuffer()
        {
            throw new ArgumentException("The current buffer writer can't contain the requested input data.");
        }
    }
}
