// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using CommunityToolkit.HighPerformance.Enumerables;
using CommunityToolkit.HighPerformance.Memory.Internals;
#if SPAN_RUNTIME_SUPPORT
using System.Runtime.InteropServices;
#else
using RuntimeHelpers = CommunityToolkit.HighPerformance.Helpers.Internals.RuntimeHelpers;
#endif

namespace CommunityToolkit.HighPerformance
{
    /// <inheritdoc cref="Span2D{T}"/>
    public readonly ref partial struct Span2D<T>
    {
        /// <summary>
        /// Gets an enumerable that traverses items in a specified row.
        /// </summary>
        /// <param name="row">The target row to enumerate within the current <see cref="Span2D{T}"/> instance.</param>
        /// <returns>A <see cref="RefEnumerable{T}"/> with target items to enumerate.</returns>
        /// <remarks>The returned <see cref="RefEnumerable{T}"/> value shouldn't be used directly: use this extension in a <see langword="foreach"/> loop.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RefEnumerable<T> GetRow(int row)
        {
            if ((uint)row >= Height)
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForRow();
            }

            nint startIndex = (nint)(uint)this.Stride * (nint)(uint)row;
            ref T r0 = ref DangerousGetReference();
            ref T r1 = ref Unsafe.Add(ref r0, startIndex);

#if SPAN_RUNTIME_SUPPORT
            return new RefEnumerable<T>(ref r1, Width, 1);
#else
            IntPtr offset = RuntimeHelpers.GetObjectDataOrReferenceByteOffset(this.Instance, ref r1);

            return new RefEnumerable<T>(this.Instance, offset, this.width, 1);
#endif
        }

        /// <summary>
        /// Gets an enumerable that traverses items in a specified column.
        /// </summary>
        /// <param name="column">The target column to enumerate within the current <see cref="Span2D{T}"/> instance.</param>
        /// <returns>A <see cref="RefEnumerable{T}"/> with target items to enumerate.</returns>
        /// <remarks>The returned <see cref="RefEnumerable{T}"/> value shouldn't be used directly: use this extension in a <see langword="foreach"/> loop.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RefEnumerable<T> GetColumn(int column)
        {
            if ((uint)column >= Width)
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForColumn();
            }

            ref T r0 = ref DangerousGetReference();
            ref T r1 = ref Unsafe.Add(ref r0, (nint)(uint)column);

#if SPAN_RUNTIME_SUPPORT
            return new RefEnumerable<T>(ref r1, Height, this.Stride);
#else
            IntPtr offset = RuntimeHelpers.GetObjectDataOrReferenceByteOffset(this.Instance, ref r1);

            return new RefEnumerable<T>(this.Instance, offset, Height, this.Stride);
#endif
        }

        /// <summary>
        /// Returns an enumerator for the current <see cref="Span2D{T}"/> instance.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to traverse the items in the current <see cref="Span2D{T}"/> instance
        /// </returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => new(this);

        /// <summary>
        /// Provides an enumerator for the elements of a <see cref="Span2D{T}"/> instance.
        /// </summary>
        public ref struct Enumerator
        {
#if SPAN_RUNTIME_SUPPORT
            /// <summary>
            /// The <see cref="Span{T}"/> instance pointing to the first item in the target memory area.
            /// </summary>
            /// <remarks>Just like in <see cref="Span2D{T}"/>, the length is the height of the 2D region.</remarks>
            private readonly Span<T> span;
#else
            /// <summary>
            /// The target <see cref="object"/> instance, if present.
            /// </summary>
            private readonly object? instance;

            /// <summary>
            /// The initial offset within <see cref="instance"/>.
            /// </summary>
            private readonly IntPtr offset;

            /// <summary>
            /// The height of the specified 2D region.
            /// </summary>
            private readonly int height;
#endif

            /// <summary>
            /// The width of the specified 2D region.
            /// </summary>
            private readonly int width;

            /// <summary>
            /// The stride of the specified 2D region.
            /// </summary>
            private readonly int stride;

            /// <summary>
            /// The current horizontal offset.
            /// </summary>
            private int x;

            /// <summary>
            /// The current vertical offset.
            /// </summary>
            private int y;

            /// <summary>
            /// Initializes a new instance of the <see cref="Enumerator"/> struct.
            /// </summary>
            /// <param name="span">The target <see cref="Span2D{T}"/> instance to enumerate.</param>
            internal Enumerator(Span2D<T> span)
            {
#if SPAN_RUNTIME_SUPPORT
                this.span = span.span;
#else
                this.instance = span.Instance;
                this.offset = span.Offset;
                this.height = span.height;
#endif
                this.width = span.width;
                this.stride = span.Stride;
                this.x = -1;
                this.y = 0;
            }

            /// <summary>
            /// Implements the duck-typed <see cref="System.Collections.IEnumerator.MoveNext"/> method.
            /// </summary>
            /// <returns><see langword="true"/> whether a new element is available, <see langword="false"/> otherwise</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                int x = this.x + 1;

                // Horizontal move, within range
                if (x < this.width)
                {
                    this.x = x;

                    return true;
                }

                // We reached the end of a row and there is at least
                // another row available: wrap to a new line and continue.
                this.x = 0;

#if SPAN_RUNTIME_SUPPORT
                return ++this.y < this.span.Length;
#else
                return ++this.y < this.height;
#endif
            }

            /// <summary>
            /// Gets the duck-typed <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> property.
            /// </summary>
            public readonly ref T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
#if SPAN_RUNTIME_SUPPORT
                    ref T r0 = ref MemoryMarshal.GetReference(this.span);
#else
                    ref T r0 = ref RuntimeHelpers.GetObjectDataAtOffsetOrPointerReference<T>(this.instance, this.offset);
#endif
                    nint index = ((nint)(uint)this.y * (nint)(uint)this.stride) + (nint)(uint)this.x;

                    return ref Unsafe.Add(ref r0, index);
                }
            }
        }
    }
}