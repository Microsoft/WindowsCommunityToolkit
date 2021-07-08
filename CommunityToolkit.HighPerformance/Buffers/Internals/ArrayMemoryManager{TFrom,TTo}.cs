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
    /// A custom <see cref="MemoryManager{T}"/> that casts data from a <typeparamref name="TFrom"/> array, to <typeparamref name="TTo"/> values.
    /// </summary>
    /// <typeparam name="TFrom">The source type of items to read.</typeparam>
    /// <typeparam name="TTo">The target type to cast the source items to.</typeparam>
    internal sealed class ArrayMemoryManager<TFrom, TTo> : MemoryManager<TTo>, IMemoryManager
        where TFrom : unmanaged
        where TTo : unmanaged
    {
        /// <summary>
        /// The source <typeparamref name="TFrom"/> array to read data from.
        /// </summary>
        private readonly TFrom[] array;

        /// <summary>
        /// The starting offset within <see name="array"/>.
        /// </summary>
        private readonly int offset;

        /// <summary>
        /// The original used length for <see name="array"/>.
        /// </summary>
        private readonly int length;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayMemoryManager{TFrom, TTo}"/> class.
        /// </summary>
        /// <param name="array">The source <typeparamref name="TFrom"/> array to read data from.</param>
        /// <param name="offset">The starting offset within <paramref name="array"/>.</param>
        /// <param name="length">The original used length for <paramref name="array"/>.</param>
        public ArrayMemoryManager(TFrom[] array, int offset, int length)
        {
            this.array = array;
            this.offset = offset;
            this.length = length;
        }

        /// <inheritdoc/>
        public override Span<TTo> GetSpan()
        {
#if SPAN_RUNTIME_SUPPORT
            ref TFrom r0 = ref this.array.DangerousGetReferenceAt(this.offset);
            ref TTo r1 = ref Unsafe.As<TFrom, TTo>(ref r0);
            int length = RuntimeHelpers.ConvertLength<TFrom, TTo>(this.length);

            return MemoryMarshal.CreateSpan(ref r1, length);
#else
            Span<TFrom> span = this.array.AsSpan(this.offset, this.length);

            // We rely on MemoryMarshal.Cast here to deal with calculating the effective
            // size of the new span to return. This will also make the behavior consistent
            // for users that are both using this type as well as casting spans directly.
            return MemoryMarshal.Cast<TFrom, TTo>(span);
#endif
        }

        /// <inheritdoc/>
        public override unsafe MemoryHandle Pin(int elementIndex = 0)
        {
            if ((uint)elementIndex >= (uint)(this.length * Unsafe.SizeOf<TFrom>() / Unsafe.SizeOf<TTo>()))
            {
                ThrowArgumentOutOfRangeExceptionForInvalidIndex();
            }

            int
                bytePrefix = this.offset * Unsafe.SizeOf<TFrom>(),
                byteSuffix = elementIndex * Unsafe.SizeOf<TTo>(),
                byteOffset = bytePrefix + byteSuffix;

            GCHandle handle = GCHandle.Alloc(this.array, GCHandleType.Pinned);

            ref TFrom r0 = ref this.array.DangerousGetReference();
            ref byte r1 = ref Unsafe.As<TFrom, byte>(ref r0);
            ref byte r2 = ref Unsafe.Add(ref r1, byteOffset);
            void* pi = Unsafe.AsPointer(ref r2);

            return new MemoryHandle(pi, handle);
        }

        /// <inheritdoc/>
        public override void Unpin()
        {
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
        }

        /// <inheritdoc/>
        public Memory<T> GetMemory<T>(int offset, int length)
            where T : unmanaged
        {
            // We need to calculate the right offset and length of the new Memory<T>. The local offset
            // is the original offset into the wrapped TFrom[] array, while the input offset is the one
            // with respect to TTo items in the Memory<TTo> instance that is currently being cast.
            int
                absoluteOffset = this.offset + RuntimeHelpers.ConvertLength<TTo, TFrom>(offset),
                absoluteLength = RuntimeHelpers.ConvertLength<TTo, TFrom>(length);

            // We have a special handling in cases where the user is circling back to the original type
            // of the wrapped array. In this case we can just return a memory wrapping that array directly,
            // with offset and length being adjusted, without the memory manager indirection.
            if (typeof(T) == typeof(TFrom))
            {
                return (Memory<T>)(object)this.array.AsMemory(absoluteOffset, absoluteLength);
            }

            return new ArrayMemoryManager<TFrom, T>(this.array, absoluteOffset, absoluteLength).Memory;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> when the target index for <see cref="Pin"/> is invalid.
        /// </summary>
        private static void ThrowArgumentOutOfRangeExceptionForInvalidIndex()
        {
            throw new ArgumentOutOfRangeException("elementIndex", "The input index is not in the valid range");
        }
    }
}