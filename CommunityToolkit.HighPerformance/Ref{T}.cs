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
    /// A <see langword="struct"/> that can store a reference to a value of a specified type.
    /// </summary>
    /// <typeparam name="T">The type of value to reference.</typeparam>
    public readonly ref struct Ref<T>
    {
#if SPAN_RUNTIME_SUPPORT
        /// <summary>
        /// The 1-length <see cref="Span{T}"/> instance used to track the target <typeparamref name="T"/> value.
        /// </summary>
        internal readonly Span<T> Span;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ref{T}"/> struct.
        /// </summary>
        /// <param name="value">The reference to the target <typeparamref name="T"/> value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Ref(ref T value)
        {
            Span = MemoryMarshal.CreateSpan(ref value, 1);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ref{T}"/> struct.
        /// </summary>
        /// <param name="pointer">The pointer to the target value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Ref(void* pointer)
            : this(ref Unsafe.AsRef<T>(pointer))
        {
        }

        /// <summary>
        /// Gets the <typeparamref name="T"/> reference represented by the current <see cref="Ref{T}"/> instance.
        /// </summary>
        public ref T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref MemoryMarshal.GetReference(Span);
        }
#else
        /// <summary>
        /// The owner <see cref="object"/> the current instance belongs to
        /// </summary>
        internal readonly object Owner;

        /// <summary>
        /// The target offset within <see cref="Owner"/> the current instance is pointing to
        /// </summary>
        /// <remarks>
        /// Using an <see cref="IntPtr"/> instead of <see cref="int"/> to avoid the int to
        /// native int conversion in the generated asm (an extra movsxd on x64).
        /// </remarks>
        internal readonly IntPtr Offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ref{T}"/> struct.
        /// </summary>
        /// <param name="owner">The owner <see cref="object"/> to create a portable reference for.</param>
        /// <param name="value">The target reference to point to (it must be within <paramref name="owner"/>).</param>
        /// <remarks>The <paramref name="value"/> parameter is not validated, and it's responsibility of the caller to ensure it's valid.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Ref(object owner, ref T value)
        {
            Owner = owner;
            Offset = ObjectMarshal.DangerousGetObjectDataByteOffset(owner, ref value);
        }

        /// <summary>
        /// Gets the <typeparamref name="T"/> reference represented by the current <see cref="Ref{T}"/> instance.
        /// </summary>
        public ref T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref ObjectMarshal.DangerousGetObjectDataReferenceAt<T>(Owner, Offset);
        }
#endif

        /// <summary>
        /// Implicitly gets the <typeparamref name="T"/> value from a given <see cref="Ref{T}"/> instance.
        /// </summary>
        /// <param name="reference">The input <see cref="Ref{T}"/> instance.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T(Ref<T> reference)
        {
            return reference.Value;
        }
    }
}