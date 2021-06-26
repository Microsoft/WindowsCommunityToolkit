// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if SPAN_RUNTIME_SUPPORT

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Toolkit.HighPerformance.Helpers;

namespace Microsoft.Toolkit.HighPerformance.Buffers.Internals
{
    /// <summary>
    /// A custom <see cref="MemoryManager{T}"/> that can wrap arbitrary <see cref="object"/> instances.
    /// </summary>
    /// <typeparam name="T">The type of elements in the target memory area.</typeparam>
    internal sealed class RawObjectMemoryManager<T> : MemoryManager<T>
    {
        /// <summary>
        /// The target <see cref="object"/> instance.
        /// </summary>
        private readonly object instance;

        /// <summary>
        /// The initial offset within <see cref="instance"/>.
        /// </summary>
        private readonly IntPtr offset;

        /// <summary>
        /// The length of the target memory area.
        /// </summary>
        private readonly int length;

        /// <summary>
        /// Initializes a new instance of the <see cref="RawObjectMemoryManager{T}"/> class.
        /// </summary>
        /// <param name="instance">The target <see cref="object"/> instance.</param>
        /// <param name="offset">The starting offset within <paramref name="instance"/>.</param>
        /// <param name="length">The usable length within <paramref name="instance"/>.</param>
        public RawObjectMemoryManager(object instance, IntPtr offset, int length)
        {
            this.instance = instance;
            this.offset = offset;
            this.length = length;
        }

        /// <inheritdoc/>
        public override Span<T> GetSpan()
        {
            ref T r0 = ref ObjectMarshal.DangerousGetObjectDataReferenceAt<T>(this.instance, this.offset);

            return MemoryMarshal.CreateSpan(ref r0, this.length);
        }

        /// <inheritdoc/>
        public override unsafe MemoryHandle Pin(int elementIndex = 0)
        {
            if ((uint)elementIndex >= (uint)this.length)
            {
                ThrowArgumentOutOfRangeExceptionForInvalidElementIndex();
            }

            // Allocating a pinned handle for the array with fail and throw an exception
            // if the array contains non blittable data. This is the expected behavior and
            // the same happens when trying to pin a Memory<T> instance obtained through
            // traditional means (eg. via the implicit T[] array conversion), if T is a
            // reference type or a type containing some references.
            GCHandle handle = GCHandle.Alloc(this.instance, GCHandleType.Pinned);
            ref T r0 = ref ObjectMarshal.DangerousGetObjectDataReferenceAt<T>(this.instance, this.offset);
            ref T r1 = ref Unsafe.Add(ref r0, (nint)(uint)elementIndex);
            void* p = Unsafe.AsPointer(ref r1);

            return new MemoryHandle(p, handle);
        }

        /// <inheritdoc/>
        public override void Unpin()
        {
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> when the input index for <see cref="Pin"/> is not valid.
        /// </summary>
        private static void ThrowArgumentOutOfRangeExceptionForInvalidElementIndex()
        {
            throw new ArgumentOutOfRangeException("elementIndex", "The input element index was not in the valid range");
        }
    }
}

#endif