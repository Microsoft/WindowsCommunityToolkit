// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Microsoft.Toolkit.HighPerformance.Buffers.Views;

namespace Microsoft.Toolkit.HighPerformance.Buffers
{
    /// <summary>
    /// Represents an output sink into which <typeparamref name="T"/> data can be written, backed by a <see cref="Memory{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of items to write to the current instance.</typeparam>
    /// <remarks>
    /// This is a custom <see cref="IBufferWriter{T}"/> implementation that wraps a <see cref="Memory{T}"/> instance.
    /// It can be used to bridge APIs consuming an <see cref="IBufferWriter{T}"/> with existing <see cref="Memory{T}"/>
    /// instances (or objects that can be converted to a <see cref="Memory{T}"/>), to ensure the data is written directly
    /// to the intended buffer, with no possibility of doing additional allocations or expanding the available capacity.
    /// </remarks>
    [DebuggerTypeProxy(typeof(MemoryDebugView<>))]
    [DebuggerDisplay("{ToString(),raw}")]
    public sealed class MemoryBufferWriter<T> : IBuffer<T>
    {
        /// <summary>
        /// The underlying <see cref="Memory{T}"/> instance.
        /// </summary>
        private readonly Memory<T> memory;

#pragma warning disable IDE0032 // Use field over auto-property (like in ArrayPoolBufferWriter<T>)
        /// <summary>
        /// The starting offset within <see cref="memory"/>.
        /// </summary>
        private int index;
#pragma warning restore IDE0032

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryBufferWriter{T}"/> class.
        /// </summary>
        /// <param name="memory">The target <see cref="Memory{T}"/> instance to write to.</param>
        public MemoryBufferWriter(Memory<T> memory)
        {
            this.memory = memory;
        }

        /// <inheritdoc/>
        public ReadOnlyMemory<T> WrittenMemory
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.memory.Slice(0, this.index);
        }

        /// <inheritdoc/>
        public ReadOnlySpan<T> WrittenSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.memory.Slice(0, this.index).Span;
        }

        /// <inheritdoc/>
        public int WrittenCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.index;
        }

        /// <inheritdoc/>
        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.memory.Length;
        }

        /// <inheritdoc/>
        public int FreeCapacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.memory.Length - this.index;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.memory.Slice(0, this.index).Span.Clear();
            this.index = 0;
        }

        /// <inheritdoc/>
        public void Advance(int count)
        {
            if (count < 0)
            {
                ThrowArgumentOutOfRangeExceptionForNegativeCount();
            }

            if (this.index > this.memory.Length - count)
            {
                ThrowArgumentExceptionForAdvancedTooFar();
            }

            this.index += count;
        }

        /// <inheritdoc/>
        public Memory<T> GetMemory(int sizeHint = 0)
        {
            ValidateSizeHint(sizeHint);

            return this.memory.Slice(this.index);
        }

        /// <inheritdoc/>
        public Span<T> GetSpan(int sizeHint = 0)
        {
            ValidateSizeHint(sizeHint);

            return this.memory.Slice(this.index).Span;
        }

        /// <summary>
        /// Validates the requested size for either <see cref="GetMemory"/> or <see cref="GetSpan"/>.
        /// </summary>
        /// <param name="sizeHint">The minimum number of items to ensure space for in <see cref="memory"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ValidateSizeHint(int sizeHint)
        {
            if (sizeHint < 0)
            {
                ThrowArgumentOutOfRangeExceptionForNegativeSizeHint();
            }

            if (sizeHint == 0)
            {
                sizeHint = 1;
            }

            if (sizeHint > FreeCapacity)
            {
                ThrowArgumentExceptionForCapacityExceeded();
            }
        }

        /// <inheritdoc/>
        [Pure]
        public override string ToString()
        {
            // See comments in MemoryOwner<T> about this
            if (typeof(T) == typeof(char))
            {
                return this.memory.Slice(0, this.index).ToString();
            }

            // Same representation used in Span<T>
            return $"Microsoft.Toolkit.HighPerformance.Buffers.MemoryBufferWriter<{typeof(T)}>[{this.index}]";
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> when the requested count is negative.
        /// </summary>
        private static void ThrowArgumentOutOfRangeExceptionForNegativeCount()
        {
            throw new ArgumentOutOfRangeException("count", "The count can't be a negative value");
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> when the size hint is negative.
        /// </summary>
        private static void ThrowArgumentOutOfRangeExceptionForNegativeSizeHint()
        {
            throw new ArgumentOutOfRangeException("sizeHint", "The size hint can't be a negative value");
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> when the requested count is negative.
        /// </summary>
        private static void ThrowArgumentExceptionForAdvancedTooFar()
        {
            throw new ArgumentException("The buffer writer has advanced too far");
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when the requested size exceeds the capacity.
        /// </summary>
        private static void ThrowArgumentExceptionForCapacityExceeded()
        {
            throw new ArgumentException("The buffer writer doesn't have enough capacity left");
        }
    }
}