// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Toolkit.HighPerformance.Buffers.Internals.Interfaces;
using RuntimeHelpers = Microsoft.Toolkit.HighPerformance.Helpers.Internals.RuntimeHelpers;

namespace Microsoft.Toolkit.HighPerformance.Buffers.Internals
{
    /// <summary>
    /// A custom <see cref="MemoryManager{T}"/> that casts data from a <see cref="string"/> to <typeparamref name="TTo"/> values.
    /// </summary>
    /// <typeparam name="TTo">The target type to cast the source characters to.</typeparam>
    internal sealed class StringMemoryManager<TTo> : MemoryManager<TTo>, IMemoryManager
        where TTo : unmanaged
    {
        /// <summary>
        /// The source <see cref="string"/> to read data from.
        /// </summary>
        private readonly string text;

        /// <summary>
        /// The starting offset within <see name="array"/>.
        /// </summary>
        private readonly int offset;

        /// <summary>
        /// The original used length for <see name="array"/>.
        /// </summary>
        private readonly int length;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringMemoryManager{T}"/> class.
        /// </summary>
        /// <param name="text">The source <see cref="string"/> to read data from.</param>
        /// <param name="offset">The starting offset within <paramref name="text"/>.</param>
        /// <param name="length">The original used length for <paramref name="text"/>.</param>
        public StringMemoryManager(string text, int offset, int length)
        {
            this.text = text;
            this.offset = offset;
            this.length = length;
        }

        /// <inheritdoc/>
        public override Span<TTo> GetSpan()
        {
#if SPAN_RUNTIME_SUPPORT
            ref char r0 = ref this.text.DangerousGetReferenceAt(this.offset);
            ref TTo r1 = ref Unsafe.As<char, TTo>(ref r0);
            int length = RuntimeHelpers.ConvertLength<char, TTo>(this.length);

            return MemoryMarshal.CreateSpan(ref r1, length);
#else
            ReadOnlyMemory<char> memory = this.text.AsMemory(this.offset, this.length);
            Span<char> span = MemoryMarshal.AsMemory(memory).Span;

            return MemoryMarshal.Cast<char, TTo>(span);
#endif
        }

        /// <inheritdoc/>
        public override unsafe MemoryHandle Pin(int elementIndex = 0)
        {
            if ((uint)elementIndex >= (uint)(this.length * Unsafe.SizeOf<char>() / Unsafe.SizeOf<TTo>()))
            {
                ThrowArgumentOutOfRangeExceptionForInvalidIndex();
            }

            int
                bytePrefix = this.offset * Unsafe.SizeOf<char>(),
                byteSuffix = elementIndex * Unsafe.SizeOf<TTo>(),
                byteOffset = bytePrefix + byteSuffix;

            GCHandle handle = GCHandle.Alloc(this.text, GCHandleType.Pinned);

            ref char r0 = ref this.text.DangerousGetReference();
            ref byte r1 = ref Unsafe.As<char, byte>(ref r0);
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
            int
                absoluteOffset = this.offset + RuntimeHelpers.ConvertLength<TTo, char>(offset),
                absoluteLength = RuntimeHelpers.ConvertLength<TTo, char>(length);

            if (typeof(T) == typeof(char))
            {
                ReadOnlyMemory<char> memory = this.text.AsMemory(absoluteOffset, absoluteLength);

                return (Memory<T>)(object)MemoryMarshal.AsMemory(memory);
            }

            return new StringMemoryManager<T>(this.text, absoluteOffset, absoluteLength).Memory;
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
