// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Toolkit.HighPerformance.Buffers;
using Microsoft.Toolkit.HighPerformance.Streams;

namespace Microsoft.Toolkit.HighPerformance
{
    /// <summary>
    /// Helpers for working with the <see cref="ArrayPoolBufferWriter{T}"/> type.
    /// </summary>
    public static class ArrayPoolBufferWriterExtensions
    {
        /// <summary>
        /// Returns a <see cref="Stream"/> that can be used to write to a target an <see cref="ArrayPoolBufferWriter{T}"/> of <see cref="byte"/> instance.
        /// </summary>
        /// <param name="writer">The target <see cref="ArrayPoolBufferWriter{T}"/> instance.</param>
        /// <returns>A <see cref="Stream"/> wrapping <paramref name="writer"/> and writing data to its underlying buffer.</returns>
        /// <remarks>The returned <see cref="Stream"/> can only be written to and does not support seeking.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stream AsStream(this ArrayPoolBufferWriter<byte> writer)
        {
            return new IBufferWriterStream<ArrayBufferWriterOwner>(new ArrayBufferWriterOwner(writer));
        }
    }
}