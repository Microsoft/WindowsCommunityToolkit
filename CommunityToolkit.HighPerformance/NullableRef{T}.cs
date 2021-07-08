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
    /// A <see langword="struct"/> that can store an optional reference to a value of a specified type.
    /// </summary>
    /// <typeparam name="T">The type of value to reference.</typeparam>
    public readonly ref struct NullableRef<T>
    {
        /// <summary>
        /// The 1-length <see cref="Span{T}"/> instance used to track the target <typeparamref name="T"/> value.
        /// </summary>
        internal readonly Span<T> Span;

        /// <summary>
        /// Initializes a new instance of the <see cref="NullableRef{T}"/> struct.
        /// </summary>
        /// <param name="value">The reference to the target <typeparamref name="T"/> value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NullableRef(ref T value)
        {
            Span = MemoryMarshal.CreateSpan(ref value, 1);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullableRef{T}"/> struct.
        /// </summary>
        /// <param name="span">The <see cref="Span{T}"/> instance to track the target <typeparamref name="T"/> reference.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private NullableRef(Span<T> span)
        {
            Span = span;
        }

        /// <summary>
        /// Gets a <see cref="NullableRef{T}"/> instance representing a <see langword="null"/> reference.
        /// </summary>
        public static NullableRef<T> Null
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => default;
        }

        /// <summary>
        /// Gets a value indicating whether or not the current <see cref="NullableRef{T}"/> instance wraps a valid reference that can be accessed.
        /// </summary>
        public unsafe bool HasValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                // We know that the span will always have a length of either
                // 1 or 0, so instead of using a cmp instruction and setting the
                // zero flag to produce our boolean value, we can just cast
                // the length to byte without overflow checks (doing a cast will
                // also account for the byte endianness of the current system),
                // and then reinterpret that value to a bool flag.
                // This results in a single movzx instruction on x86-64.
                byte length = unchecked((byte)Span.Length);

                return *(bool*)&length;
            }
        }

        /// <summary>
        /// Gets the <typeparamref name="T"/> reference represented by the current <see cref="NullableRef{T}"/> instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="HasValue"/> is <see langword="false"/>.</exception>
        public ref T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (!HasValue)
                {
                    ThrowInvalidOperationException();
                }

                return ref MemoryMarshal.GetReference(Span);
            }
        }

        /// <summary>
        /// Implicitly converts a <see cref="Ref{T}"/> instance into a <see cref="NullableRef{T}"/> one.
        /// </summary>
        /// <param name="reference">The input <see cref="Ref{T}"/> instance.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator NullableRef<T>(Ref<T> reference)
        {
            return new(reference.Span);
        }

        /// <summary>
        /// Explicitly gets the <typeparamref name="T"/> value from a given <see cref="NullableRef{T}"/> instance.
        /// </summary>
        /// <param name="reference">The input <see cref="NullableRef{T}"/> instance.</param>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="HasValue"/> is <see langword="false"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator T(NullableRef<T> reference)
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