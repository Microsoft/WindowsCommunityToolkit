// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
#if SPAN_RUNTIME_SUPPORT
using System.Runtime.InteropServices;
#else
using CommunityToolkit.HighPerformance.Helpers;
#endif

namespace CommunityToolkit.HighPerformance
{
    /// <summary>
    /// A <see langword="struct"/> that can store a readonly reference to a value of a specified type.
    /// </summary>
    /// <typeparam name="T">The type of value to reference.</typeparam>
    public readonly ref struct ReadOnlyRef<T>
    {
#if SPAN_RUNTIME_SUPPORT
        /// <summary>
        /// The 1-length <see cref="ReadOnlySpan{T}"/> instance used to track the target <typeparamref name="T"/> value.
        /// </summary>
        internal readonly ReadOnlySpan<T> Span;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRef{T}"/> struct.
        /// </summary>
        /// <param name="value">The readonly reference to the target <typeparamref name="T"/> value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlyRef(in T value)
        {
            ref T r0 = ref Unsafe.AsRef(value);

            Span = MemoryMarshal.CreateReadOnlySpan(ref r0, 1);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRef{T}"/> struct.
        /// </summary>
        /// <param name="pointer">The pointer to the target value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ReadOnlyRef(void* pointer)
            : this(in Unsafe.AsRef<T>(pointer))
        {
        }

        /// <summary>
        /// Gets the readonly <typeparamref name="T"/> reference represented by the current <see cref="Ref{T}"/> instance.
        /// </summary>
        public ref readonly T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref MemoryMarshal.GetReference(Span);
        }

        /// <summary>
        /// Implicitly converts a <see cref="Ref{T}"/> instance into a <see cref="ReadOnlyRef{T}"/> one.
        /// </summary>
        /// <param name="reference">The input <see cref="Ref{T}"/> instance.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ReadOnlyRef<T>(Ref<T> reference)
        {
            return new(in reference.Value);
        }
#else
        /// <summary>
        /// The owner <see cref="object"/> the current instance belongs to
        /// </summary>
        private readonly object owner;

        /// <summary>
        /// The target offset within <see cref="owner"/> the current instance is pointing to
        /// </summary>
        private readonly IntPtr offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRef{T}"/> struct.
        /// </summary>
        /// <param name="owner">The owner <see cref="object"/> to create a portable reference for.</param>
        /// <param name="offset">The target offset within <paramref name="owner"/> for the target reference.</param>
        /// <remarks>The <paramref name="offset"/> parameter is not validated, and it's responsibility of the caller to ensure it's valid.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ReadOnlyRef(object owner, IntPtr offset)
        {
            this.owner = owner;
            this.offset = offset;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRef{T}"/> struct.
        /// </summary>
        /// <param name="owner">The owner <see cref="object"/> to create a portable reference for.</param>
        /// <param name="value">The target reference to point to (it must be within <paramref name="owner"/>).</param>
        /// <remarks>The <paramref name="value"/> parameter is not validated, and it's responsibility of the caller to ensure it's valid.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlyRef(object owner, in T value)
        {
            this.owner = owner;
            this.offset = ObjectMarshal.DangerousGetObjectDataByteOffset(owner, ref Unsafe.AsRef(value));
        }

        /// <summary>
        /// Gets the readonly <typeparamref name="T"/> reference represented by the current <see cref="Ref{T}"/> instance.
        /// </summary>
        public ref readonly T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref ObjectMarshal.DangerousGetObjectDataReferenceAt<T>(this.owner, this.offset);
        }

        /// <summary>
        /// Implicitly converts a <see cref="Ref{T}"/> instance into a <see cref="ReadOnlyRef{T}"/> one.
        /// </summary>
        /// <param name="reference">The input <see cref="Ref{T}"/> instance.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ReadOnlyRef<T>(Ref<T> reference)
        {
            return new(reference.Owner, reference.Offset);
        }
#endif

        /// <summary>
        /// Implicitly gets the <typeparamref name="T"/> value from a given <see cref="ReadOnlyRef{T}"/> instance.
        /// </summary>
        /// <param name="reference">The input <see cref="ReadOnlyRef{T}"/> instance.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T(ReadOnlyRef<T> reference)
        {
            return reference.Value;
        }
    }
}