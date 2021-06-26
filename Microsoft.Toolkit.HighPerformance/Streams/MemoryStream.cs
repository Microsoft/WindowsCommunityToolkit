// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.Toolkit.HighPerformance.Streams
{
    /// <summary>
    /// A factory class to produce <see cref="MemoryStream{TSource}"/> instances.
    /// </summary>
    internal static partial class MemoryStream
    {
        /// <summary>
        /// Creates a new <see cref="Stream"/> from the input <see cref="ReadOnlyMemory{T}"/> of <see cref="byte"/> instance.
        /// </summary>
        /// <param name="memory">The input <see cref="ReadOnlyMemory{T}"/> instance.</param>
        /// <param name="isReadOnly">Indicates whether or not <paramref name="memory"/> can be written to.</param>
        /// <returns>A <see cref="Stream"/> wrapping the underlying data for <paramref name="memory"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="memory"/> has an invalid data store.</exception>
        [Pure]
        public static Stream Create(ReadOnlyMemory<byte> memory, bool isReadOnly)
        {
            if (memory.IsEmpty)
            {
                // Return an empty stream if the memory was empty
                return new MemoryStream<ArrayOwner>(ArrayOwner.Empty, isReadOnly);
            }

            if (MemoryMarshal.TryGetArray(memory, out ArraySegment<byte> segment))
            {
                var arraySpanSource = new ArrayOwner(segment.Array!, segment.Offset, segment.Count);

                return new MemoryStream<ArrayOwner>(arraySpanSource, isReadOnly);
            }

            if (MemoryMarshal.TryGetMemoryManager<byte, MemoryManager<byte>>(memory, out var memoryManager, out int start, out int length))
            {
                MemoryManagerOwner memoryManagerSpanSource = new MemoryManagerOwner(memoryManager, start, length);

                return new MemoryStream<MemoryManagerOwner>(memoryManagerSpanSource, isReadOnly);
            }

            return ThrowNotSupportedExceptionForInvalidMemory();
        }

        /// <summary>
        /// Creates a new <see cref="Stream"/> from the input <see cref="IMemoryOwner{T}"/> of <see cref="byte"/> instance.
        /// </summary>
        /// <param name="memoryOwner">The input <see cref="IMemoryOwner{T}"/> instance.</param>
        /// <returns>A <see cref="Stream"/> wrapping the underlying data for <paramref name="memoryOwner"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="memoryOwner"/> has an invalid data store.</exception>
        [Pure]
        public static Stream Create(IMemoryOwner<byte> memoryOwner)
        {
            Memory<byte> memory = memoryOwner.Memory;

            if (memory.IsEmpty)
            {
                // Return an empty stream if the memory was empty
                return new IMemoryOwnerStream<ArrayOwner>(ArrayOwner.Empty, memoryOwner);
            }

            if (MemoryMarshal.TryGetArray(memory, out ArraySegment<byte> segment))
            {
                var arraySpanSource = new ArrayOwner(segment.Array!, segment.Offset, segment.Count);

                return new IMemoryOwnerStream<ArrayOwner>(arraySpanSource, memoryOwner);
            }

            if (MemoryMarshal.TryGetMemoryManager<byte, MemoryManager<byte>>(memory, out var memoryManager, out int start, out int length))
            {
                MemoryManagerOwner memoryManagerSpanSource = new MemoryManagerOwner(memoryManager, start, length);

                return new IMemoryOwnerStream<MemoryManagerOwner>(memoryManagerSpanSource, memoryOwner);
            }

            return ThrowNotSupportedExceptionForInvalidMemory();
        }

        /// <summary>
        /// Throws a <see cref="ArgumentException"/> when a given <see cref="Memory{T}"/>
        /// or <see cref="IMemoryOwner{T}"/> instance has an unsupported backing store.
        /// </summary>
        /// <returns>Nothing, this method always throws.</returns>
        private static Stream ThrowNotSupportedExceptionForInvalidMemory()
        {
            throw new ArgumentException("The input instance doesn't have a valid underlying data store.");
        }
    }
}