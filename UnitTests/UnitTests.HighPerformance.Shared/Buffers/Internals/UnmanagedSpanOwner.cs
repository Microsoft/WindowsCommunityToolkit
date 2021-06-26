// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnitTests.HighPerformance.Shared.Buffers.Internals
{
    /// <summary>
    /// An owner for a buffer of an unmanaged type, recycling <see cref="byte"/> arrays to save memory.
    /// </summary>
    /// <typeparam name="T">The type of items to store in the rented buffers.</typeparam>
    internal sealed unsafe class UnmanagedSpanOwner<T> : MemoryManager<T>
        where T : unmanaged
    {
        /// <summary>
        /// The size of the current instance
        /// </summary>
        private readonly int length;

        /// <summary>
        /// The pointer to the underlying <see cref="byte"/> array.
        /// </summary>
        private IntPtr ptr;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnmanagedSpanOwner{T}"/> class.
        /// </summary>
        /// <param name="size">The size of the buffer to rent.</param>
        public UnmanagedSpanOwner(int size)
        {
            this.ptr = Marshal.AllocHGlobal(size * Unsafe.SizeOf<T>());
            this.length = size;
        }

        /// <summary>
        /// Gets the length of the buffer in use.
        /// </summary>
        public int Length => this.length;

        /// <summary>
        /// Gets a pointer to the start of the buffer in use.
        /// </summary>
        public T* Ptr => (T*)this.ptr;

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            IntPtr ptr = this.ptr;

            if (ptr == IntPtr.Zero)
            {
                return;
            }

            this.ptr = IntPtr.Zero;

            Marshal.FreeHGlobal(ptr);
        }

        /// <inheritdoc/>
        public override Span<T> GetSpan()
        {
            return new Span<T>((void*)this.ptr, this.length);
        }

        /// <inheritdoc/>
        public override MemoryHandle Pin(int elementIndex = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Unpin()
        {
            throw new NotImplementedException();
        }
    }
}