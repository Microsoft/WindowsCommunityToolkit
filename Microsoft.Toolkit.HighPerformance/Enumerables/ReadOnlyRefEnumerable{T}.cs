// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
#if SPAN_RUNTIME_SUPPORT
using System.Runtime.InteropServices;
#endif
using Microsoft.Toolkit.HighPerformance.Helpers.Internals;
using Microsoft.Toolkit.HighPerformance.Memory.Internals;

#if !SPAN_RUNTIME_SUPPORT
using RuntimeHelpers = Microsoft.Toolkit.HighPerformance.Helpers.Internals.RuntimeHelpers;
#endif

namespace Microsoft.Toolkit.HighPerformance.Enumerables
{
    /// <summary>
    /// A <see langword="ref"/> <see langword="struct"/> that iterates readonly items from arbitrary memory locations.
    /// </summary>
    /// <typeparam name="T">The type of items to enumerate.</typeparam>
    public readonly ref struct ReadOnlyRefEnumerable<T>
    {
#if SPAN_RUNTIME_SUPPORT
        /// <summary>
        /// The <see cref="ReadOnlySpan{T}"/> instance pointing to the first item in the target memory area.
        /// </summary>
        /// <remarks>The <see cref="ReadOnlySpan{T}.Length"/> field maps to the total available length.</remarks>
        private readonly ReadOnlySpan<T> span;
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
        /// The total available length for the sequence.
        /// </summary>
        private readonly int length;
#endif

        /// <summary>
        /// The distance between items in the sequence to enumerate.
        /// </summary>
        /// <remarks>The distance refers to <typeparamref name="T"/> items, not byte offset.</remarks>
        private readonly int step;

#if SPAN_RUNTIME_SUPPORT
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRefEnumerable{T}"/> struct.
        /// </summary>
        /// <param name="span">The <see cref="ReadOnlySpan{T}"/> instance pointing to the first item in the target memory area.</param>
        /// <param name="step">The distance between items in the sequence to enumerate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ReadOnlyRefEnumerable(ReadOnlySpan<T> span, int step)
        {
            this.span = span;
            this.step = step;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRefEnumerable{T}"/> struct.
        /// </summary>
        /// <param name="reference">A reference to the first item of the sequence.</param>
        /// <param name="length">The number of items in the sequence.</param>
        /// <param name="step">The distance between items in the sequence to enumerate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ReadOnlyRefEnumerable(in T reference, int length, int step)
        {
            this.span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(reference), length);
            this.step = step;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ReadOnlyRefEnumerable{T}"/> struct with the specified parameters.
        /// </summary>
        /// <param name="value">The reference to the first <typeparamref name="T"/> item to map.</param>
        /// <param name="length">The number of items in the sequence.</param>
        /// <param name="step">The distance between items in the sequence to enumerate.</param>
        /// <returns>A <see cref="ReadOnlyRefEnumerable{T}"/> instance with the specified parameters.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when one of the parameters are negative.</exception>
        [Pure]
        public static ReadOnlyRefEnumerable<T> DangerousCreate(in T value, int length, int step)
        {
            if (length < 0)
            {
                ThrowArgumentOutOfRangeExceptionForLength();
            }

            if (step < 0)
            {
                ThrowArgumentOutOfRangeExceptionForStep();
            }

            OverflowHelper.EnsureIsInNativeIntRange(length, 1, step);

            return new ReadOnlyRefEnumerable<T>(in value, length, step);
        }
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRefEnumerable{T}"/> struct.
        /// </summary>
        /// <param name="instance">The target <see cref="object"/> instance.</param>
        /// <param name="offset">The initial offset within <see paramref="instance"/>.</param>
        /// <param name="length">The number of items in the sequence.</param>
        /// <param name="step">The distance between items in the sequence to enumerate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ReadOnlyRefEnumerable(object? instance, IntPtr offset, int length, int step)
        {
            this.instance = instance;
            this.offset = offset;
            this.length = length;
            this.step = step;
        }
#endif

        /// <inheritdoc cref="System.Collections.IEnumerable.GetEnumerator"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator()
        {
#if SPAN_RUNTIME_SUPPORT
            return new Enumerator(this.span, this.step);
#else
            return new Enumerator(this.instance, this.offset, this.length, this.step);
#endif
        }

        /// <summary>
        /// Copies the contents of this <see cref="ReadOnlyRefEnumerable{T}"/> into a destination <see cref="RefEnumerable{T}"/> instance.
        /// </summary>
        /// <param name="destination">The destination <see cref="RefEnumerable{T}"/> instance.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="destination"/> is shorter than the source <see cref="ReadOnlyRefEnumerable{T}"/> instance.
        /// </exception>
        public void CopyTo(RefEnumerable<T> destination)
        {
#if SPAN_RUNTIME_SUPPORT
            if (this.step == 1)
            {
                destination.CopyFrom(this.span);

                return;
            }

            if (destination.Step == 1)
            {
                CopyTo(destination.Span);

                return;
            }

            ref T sourceRef = ref this.span.DangerousGetReference();
            ref T destinationRef = ref destination.Span.DangerousGetReference();
            int
                sourceLength = this.span.Length,
                destinationLength = destination.Span.Length;
#else
            ref T sourceRef = ref RuntimeHelpers.GetObjectDataAtOffsetOrPointerReference<T>(this.instance, this.offset);
            ref T destinationRef = ref RuntimeHelpers.GetObjectDataAtOffsetOrPointerReference<T>(destination.Instance, destination.Offset);
            int
                sourceLength = this.length,
                destinationLength = destination.Length;
#endif

            if ((uint)destinationLength < (uint)sourceLength)
            {
                ThrowArgumentExceptionForDestinationTooShort();
            }

            RefEnumerableHelper.CopyTo(ref sourceRef, ref destinationRef, (nint)(uint)sourceLength, (nint)(uint)this.step, (nint)(uint)destination.Step);
        }

        /// <summary>
        /// Attempts to copy the current <see cref="ReadOnlyRefEnumerable{T}"/> instance to a destination <see cref="RefEnumerable{T}"/>.
        /// </summary>
        /// <param name="destination">The target <see cref="RefEnumerable{T}"/> of the copy operation.</param>
        /// <returns>Whether or not the operation was successful.</returns>
        public bool TryCopyTo(RefEnumerable<T> destination)
        {
#if SPAN_RUNTIME_SUPPORT
            int
                sourceLength = this.span.Length,
                destinationLength = destination.Span.Length;
#else
            int
                sourceLength = this.length,
                destinationLength = destination.Length;
#endif

            if (destinationLength >= sourceLength)
            {
                CopyTo(destination);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Copies the contents of this <see cref="RefEnumerable{T}"/> into a destination <see cref="Span{T}"/> instance.
        /// </summary>
        /// <param name="destination">The destination <see cref="Span{T}"/> instance.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="destination"/> is shorter than the source <see cref="RefEnumerable{T}"/> instance.
        /// </exception>
        public void CopyTo(Span<T> destination)
        {
#if SPAN_RUNTIME_SUPPORT
            if (this.step == 1)
            {
                this.span.CopyTo(destination);

                return;
            }

            ref T sourceRef = ref this.span.DangerousGetReference();
            int length = this.span.Length;
#else
            ref T sourceRef = ref RuntimeHelpers.GetObjectDataAtOffsetOrPointerReference<T>(this.instance, this.offset);
            int length = this.length;
#endif
            if ((uint)destination.Length < (uint)length)
            {
                ThrowArgumentExceptionForDestinationTooShort();
            }

            ref T destinationRef = ref destination.DangerousGetReference();

            RefEnumerableHelper.CopyTo(ref sourceRef, ref destinationRef, (nint)(uint)length, (nint)(uint)this.step);
        }

        /// <summary>
        /// Attempts to copy the current <see cref="RefEnumerable{T}"/> instance to a destination <see cref="Span{T}"/>.
        /// </summary>
        /// <param name="destination">The target <see cref="Span{T}"/> of the copy operation.</param>
        /// <returns>Whether or not the operation was successful.</returns>
        public bool TryCopyTo(Span<T> destination)
        {
#if SPAN_RUNTIME_SUPPORT
            int length = this.span.Length;
#else
            int length = this.length;
#endif

            if (destination.Length >= length)
            {
                CopyTo(destination);

                return true;
            }

            return false;
        }

        /// <inheritdoc cref="RefEnumerable{T}.ToArray"/>
        [Pure]
        public T[] ToArray()
        {
#if SPAN_RUNTIME_SUPPORT
            int length = this.span.Length;
#else
            int length = this.length;
#endif

            // Empty array if no data is mapped
            if (length == 0)
            {
                return Array.Empty<T>();
            }

            T[] array = new T[length];

            CopyTo(array);

            return array;
        }

        /// <summary>
        /// Implicitly converts a <see cref="RefEnumerable{T}"/> instance into a <see cref="ReadOnlyRefEnumerable{T}"/> one.
        /// </summary>
        /// <param name="enumerable">The input <see cref="RefEnumerable{T}"/> instance.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ReadOnlyRefEnumerable<T>(RefEnumerable<T> enumerable)
        {
#if SPAN_RUNTIME_SUPPORT
            return new ReadOnlyRefEnumerable<T>(enumerable.Span, enumerable.Step);
#else
            return new ReadOnlyRefEnumerable<T>(enumerable.Instance, enumerable.Offset, enumerable.Length, enumerable.Step);
#endif
        }

        /// <summary>
        /// A custom enumerator type to traverse items within a <see cref="ReadOnlyRefEnumerable{T}"/> instance.
        /// </summary>
        public ref struct Enumerator
        {
#if SPAN_RUNTIME_SUPPORT
            /// <inheritdoc cref="ReadOnlyRefEnumerable{T}.span"/>
            private readonly ReadOnlySpan<T> span;
#else
            /// <inheritdoc cref="ReadOnlyRefEnumerable{T}.instance"/>
            private readonly object? instance;

            /// <inheritdoc cref="ReadOnlyRefEnumerable{T}.offset"/>
            private readonly IntPtr offset;

            /// <inheritdoc cref="ReadOnlyRefEnumerable{T}.length"/>
            private readonly int length;
#endif

            /// <inheritdoc cref="ReadOnlyRefEnumerable{T}.step"/>
            private readonly int step;

            /// <summary>
            /// The current position in the sequence.
            /// </summary>
            private int position;

#if SPAN_RUNTIME_SUPPORT
            /// <summary>
            /// Initializes a new instance of the <see cref="Enumerator"/> struct.
            /// </summary>
            /// <param name="span">The <see cref="ReadOnlySpan{T}"/> instance with the info on the items to traverse.</param>
            /// <param name="step">The distance between items in the sequence to enumerate.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(ReadOnlySpan<T> span, int step)
            {
                this.span = span;
                this.step = step;
                this.position = -1;
            }
#else
            /// <summary>
            /// Initializes a new instance of the <see cref="Enumerator"/> struct.
            /// </summary>
            /// <param name="instance">The target <see cref="object"/> instance.</param>
            /// <param name="offset">The initial offset within <see paramref="instance"/>.</param>
            /// <param name="length">The number of items in the sequence.</param>
            /// <param name="step">The distance between items in the sequence to enumerate.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(object? instance, IntPtr offset, int length, int step)
            {
                this.instance = instance;
                this.offset = offset;
                this.length = length;
                this.step = step;
                this.position = -1;
            }
#endif

            /// <inheritdoc cref="System.Collections.IEnumerator.MoveNext"/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
#if SPAN_RUNTIME_SUPPORT
                return ++this.position < this.span.Length;
#else
                return ++this.position < this.length;
#endif
            }

            /// <inheritdoc cref="System.Collections.Generic.IEnumerator{T}.Current"/>
            public readonly ref readonly T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
#if SPAN_RUNTIME_SUPPORT
                    ref T r0 = ref this.span.DangerousGetReference();
#else
                    ref T r0 = ref RuntimeHelpers.GetObjectDataAtOffsetOrPointerReference<T>(this.instance, this.offset);
#endif
                    nint offset = (nint)(uint)this.position * (nint)(uint)this.step;
                    ref T ri = ref Unsafe.Add(ref r0, offset);

                    return ref ri;
                }
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> when the "length" parameter is invalid.
        /// </summary>
        private static void ThrowArgumentOutOfRangeExceptionForLength()
        {
            throw new ArgumentOutOfRangeException("length");
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> when the "step" parameter is invalid.
        /// </summary>
        private static void ThrowArgumentOutOfRangeExceptionForStep()
        {
            throw new ArgumentOutOfRangeException("step");
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when the target span is too short.
        /// </summary>
        private static void ThrowArgumentExceptionForDestinationTooShort()
        {
            throw new ArgumentException("The target span is too short to copy all the current items to");
        }
    }
}
