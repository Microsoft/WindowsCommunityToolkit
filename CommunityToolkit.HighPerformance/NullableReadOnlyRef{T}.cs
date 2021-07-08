// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if SPAN_RUNTIME_SUPPORT

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CommunityToolkit.HighPerformance
{
    /// <summary>
    /// A <see langword="struct"/> that can store an optional readonly reference to a value of a specified type.
    /// </summary>
    /// <typeparam name="T">The type of value to reference.</typeparam>
    public readonly ref struct NullableReadOnlyRef<T>
    {
        /// <summary>
        /// The 1-length <see cref="ReadOnlySpan{T}"/> instance used to track the target <typeparamref name="T"/> value.
        /// </summary>
        private readonly ReadOnlySpan<T> span;

        /// <summary>
        /// Initializes a new instance of the <see cref="NullableReadOnlyRef{T}"/> struct.
        /// </summary>
        /// <param name="value">The readonly reference to the target <typeparamref name="T"/> value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NullableReadOnlyRef(in T value)
        {
            ref T r0 = ref Unsafe.AsRef(value);

            this.span = MemoryMarshal.CreateReadOnlySpan(ref r0, 1);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullableReadOnlyRef{T}"/> struct.
        /// </summary>
        /// <param name="span">The <see cref="ReadOnlySpan{T}"/> instance to track the target <typeparamref name="T"/> reference.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private NullableReadOnlyRef(ReadOnlySpan<T> span)
        {
            this.span = span;
        }

        /// <summary>
        /// Gets a <see cref="NullableReadOnlyRef{T}"/> instance representing a <see langword="null"/> reference.
        /// </summary>
        public static NullableReadOnlyRef<T> Null
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => default;
        }

        /// <summary>
        /// Gets a value indicating whether or not the current <see cref="NullableReadOnlyRef{T}"/> instance wraps a valid reference that can be accessed.
        /// </summary>
        public unsafe bool HasValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                // See comment in NullableRef<T> about this
                byte length = unchecked((byte)this.span.Length);

                return *(bool*)&length;
            }
        }

        /// <summary>
        /// Gets the <typeparamref name="T"/> reference represented by the current <see cref="NullableReadOnlyRef{T}"/> instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="HasValue"/> is <see langword="false"/>.</exception>
        public ref readonly T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (!HasValue)
                {
                    ThrowInvalidOperationException();
                }

                return ref MemoryMarshal.GetReference(this.span);
            }
        }

        /// <summary>
        /// Implicitly converts a <see cref="Ref{T}"/> instance into a <see cref="NullableReadOnlyRef{T}"/> one.
        /// </summary>
        /// <param name="reference">The input <see cref="Ref{T}"/> instance.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator NullableReadOnlyRef<T>(Ref<T> reference)
        {
            return new(reference.Span);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ReadOnlyRef{T}"/> instance into a <see cref="NullableReadOnlyRef{T}"/> one.
        /// </summary>
        /// <param name="reference">The input <see cref="ReadOnlyRef{T}"/> instance.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator NullableReadOnlyRef<T>(ReadOnlyRef<T> reference)
        {
            return new(reference.Span);
        }

        /// <summary>
        /// Implicitly converts a <see cref="NullableRef{T}"/> instance into a <see cref="NullableReadOnlyRef{T}"/> one.
        /// </summary>
        /// <param name="reference">The input <see cref="Ref{T}"/> instance.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator NullableReadOnlyRef<T>(NullableRef<T> reference)
        {
            return new(reference.Span);
        }

        /// <summary>
        /// Explicitly gets the <typeparamref name="T"/> value from a given <see cref="NullableReadOnlyRef{T}"/> instance.
        /// </summary>
        /// <param name="reference">The input <see cref="NullableReadOnlyRef{T}"/> instance.</param>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="HasValue"/> is <see langword="false"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator T(NullableReadOnlyRef<T> reference)
        {
            return reference.Value;
        }

        /// <summary>
        /// Throws a <see cref="InvalidOperationException"/> when trying to access <see cref="Value"/> for a default instance.
        /// </summary>
        private static void ThrowInvalidOperationException()
        {
            throw new InvalidOperationException("The current instance doesn't have a value that can be accessed");
        }
    }
}

#endif