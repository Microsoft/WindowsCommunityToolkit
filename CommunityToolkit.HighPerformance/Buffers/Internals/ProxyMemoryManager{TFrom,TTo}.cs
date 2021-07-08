// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance.Buffers.Internals.Interfaces;
using RuntimeHelpers = CommunityToolkit.HighPerformance.Helpers.Internals.RuntimeHelpers;

namespace CommunityToolkit.HighPerformance.Buffers.Internals
{
    /// <summary>
    /// A custom <see cref="MemoryManager{T}"/> that casts data from a <see cref="MemoryManager{T}"/> of <typeparamref name="TFrom"/>, to <typeparamref name="TTo"/> values.
    /// </summary>
    /// <typeparam name="TFrom">The source type of items to read.</typeparam>
    /// <typeparam name="TTo">The target type to cast the source items to.</typeparam>
    internal sealed class ProxyMemoryManager<TFrom, TTo> : MemoryManager<TTo>, IMemoryManager
        where TFrom : unmanaged
        where TTo : unmanaged
    {
        /// <summary>
        /// The source <see cref="MemoryManager{T}"/> to read data from.
        /// </summary>
        private readonly MemoryManager<TFrom> memoryManager;

        /// <summary>
        /// The starting offset within <see name="memoryManager"/>.
        /// </summary>
        private readonly int offset;

        /// <summary>
        /// The original used length for <see name="memoryManager"/>.
        /// </summary>
        private readonly int length;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyMemoryManager{TFrom, TTo}"/> class.
        /// </summary>
        /// <param name="memoryManager">The source <see cref="MemoryManager{T}"/> to read data from.</param>
        /// <param name="offset">The starting offset within <paramref name="memoryManager"/>.</param>
        /// <param name="length">The original used length for <paramref name="memoryManager"/>.</param>
        public ProxyMemoryManager(MemoryManager<TFrom> memoryManager, int offset, int length)
        {
            this.memoryManager = memoryManager;
            this.offset = offset;
            this.length = length;
        }

        /// <inheritdoc/>
        public override Span<TTo> GetSpan()
        {
            Span<TFrom> span = this.memoryManager.GetSpan().Slice(this.offset, this.length);

            return MemoryMarshal.Cast<TFrom, TTo>(span);
        }

        /// <inheritdoc/>
        public override MemoryHandle Pin(int elementIndex = 0)
        {
            if ((uint)elementIndex >= (uint)(this.length * Unsafe.SizeOf<TFrom>() / Unsafe.SizeOf<TTo>()))
            {
                ThrowArgumentExceptionForInvalidIndex();
            }

            int
                bytePrefix = this.offset * Unsafe.SizeOf<TFrom>(),
                byteSuffix = elementIndex * Unsafe.SizeOf<TTo>(),
                byteOffset = bytePrefix + byteSuffix;

#if NETSTANDARD1_4
            int
                shiftedOffset = byteOffset / Unsafe.SizeOf<TFrom>(),
                remainder = byteOffset - (shiftedOffset * Unsafe.SizeOf<TFrom>());
#else
            int shiftedOffset = Math.DivRem(byteOffset, Unsafe.SizeOf<TFrom>(), out int remainder);
#endif

            if (remainder != 0)
            {
                ThrowArgumentExceptionForInvalidAlignment();
            }

            return this.memoryManager.Pin(shiftedOffset);
        }

        /// <inheritdoc/>
        public override void Unpin()
        {
            this.memoryManager.Unpin();
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            ((IDisposable)this.memoryManager).Dispose();
        }

        /// <inheritdoc/>
        public Memory<T> GetMemory<T>(int offset, int length)
            where T : unmanaged
        {
            // Like in the other memory manager, calculate the absolute offset and length
            int
                absoluteOffset = this.offset + RuntimeHelpers.ConvertLength<TTo, TFrom>(offset),
                absoluteLength = RuntimeHelpers.ConvertLength<TTo, TFrom>(length);

            // Skip one indirection level and slice the original memory manager, if possible
            if (typeof(T) == typeof(TFrom))
            {
                return (Memory<T>)(object)this.memoryManager.Memory.Slice(absoluteOffset, absoluteLength);
            }

            return new ProxyMemoryManager<TFrom, T>(this.memoryManager, absoluteOffset, absoluteLength).Memory;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> when the target index for <see cref="Pin"/> is invalid.
        /// </summary>
        private static void ThrowArgumentExceptionForInvalidIndex()
        {
            throw new ArgumentOutOfRangeException("elementIndex", "The input index is not in the valid range");
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Pin"/> receives an invalid target index.
        /// </summary>
        private static void ThrowArgumentExceptionForInvalidAlignment()
        {
            throw new ArgumentOutOfRangeException("elementIndex", "The input index doesn't result in an aligned item access");
        }
    }
}