// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;

namespace Microsoft.Toolkit.HighPerformance.Buffers
{
    /// <summary>
    /// An interface that expands <see cref="IBufferWriter{T}"/> with the ability to also inspect
    /// the written data, and to reset the underlying buffer to write again from the start.
    /// </summary>
    /// <typeparam name="T">The type of items in the current buffer.</typeparam>
    public interface IBuffer<T> : IBufferWriter<T>
    {
        /// <summary>
        /// Gets the data written to the underlying buffer so far, as a <see cref="ReadOnlyMemory{T}"/>.
        /// </summary>
        ReadOnlyMemory<T> WrittenMemory { get; }

        /// <summary>
        /// Gets the data written to the underlying buffer so far, as a <see cref="ReadOnlySpan{T}"/>.
        /// </summary>
        ReadOnlySpan<T> WrittenSpan { get; }

        /// <summary>
        /// Gets the amount of data written to the underlying buffer so far.
        /// </summary>
        int WrittenCount { get; }

        /// <summary>
        /// Gets the total amount of space within the underlying buffer.
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// Gets the amount of space available that can still be written into without forcing the underlying buffer to grow.
        /// </summary>
        int FreeCapacity { get; }

        /// <summary>
        /// Clears the data written to the underlying buffer.
        /// </summary>
        /// <remarks>
        /// You must clear the <see cref="IBuffer{T}"/> instance before trying to re-use it.
        /// </remarks>
        void Clear();
    }
}