// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Microsoft.Toolkit.HighPerformance.Buffers;

namespace Microsoft.Toolkit.HighPerformance.Streams
{
    /// <summary>
    /// An <see cref="IBufferWriter{T}"/> implementation wrapping an <see cref="IBufferWriter{T}"/> instance.
    /// </summary>
    internal readonly struct IBufferWriterOwner : IBufferWriter<byte>
    {
        /// <summary>
        /// The wrapped <see cref="ArrayPoolBufferWriter{T}"/> array.
        /// </summary>
        private readonly IBufferWriter<byte> writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="IBufferWriterOwner"/> struct.
        /// </summary>
        /// <param name="writer">The wrapped <see cref="IBufferWriter{T}"/> instance.</param>
        public IBufferWriterOwner(IBufferWriter<byte> writer)
        {
            this.writer = writer;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int count)
        {
            this.writer.Advance(count);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> GetMemory(int sizeHint = 0)
        {
            return this.writer.GetMemory(sizeHint);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> GetSpan(int sizeHint = 0)
        {
            return this.writer.GetSpan(sizeHint);
        }
    }
}
