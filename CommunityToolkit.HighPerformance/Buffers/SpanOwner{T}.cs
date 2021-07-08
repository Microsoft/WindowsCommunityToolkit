// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
#if NETCORE_RUNTIME || NET5_0
using System.Runtime.InteropServices;
#endif
using CommunityToolkit.HighPerformance.Buffers.Views;

namespace CommunityToolkit.HighPerformance.Buffers
{
    /// <summary>
    /// A stack-only type with the ability to rent a buffer of a specified length and getting a <see cref="Span{T}"/> from it.
    /// This type mirrors <see cref="MemoryOwner{T}"/> but without allocations and with further optimizations.
    /// As this is a stack-only type, it relies on the duck-typed <see cref="IDisposable"/> pattern introduced with C# 8.
    /// It should be used like so:
    /// <code>
    /// using (SpanOwner&lt;byte> buffer = SpanOwner&lt;byte>.Allocate(1024))
    /// {
    ///     // Use the buffer here...
    /// }
    /// </code>
    /// As soon as the code leaves the scope of that <see langword="using"/> block, the underlying buffer will automatically
    /// be disposed. The APIs in <see cref="SpanOwner{T}"/> rely on this pattern for extra performance, eg. they don't perform
    /// the additional checks that are done in <see cref="MemoryOwner{T}"/> to ensure that the buffer hasn't been disposed
    /// before returning a <see cref="Memory{T}"/> or <see cref="Span{T}"/> instance from it.
    /// As such, this type should always be used with a <see langword="using"/> block or expression.
    /// Not doing so will cause the underlying buffer not to be returned to the shared pool.
    /// </summary>
    /// <typeparam name="T">The type of items to store in the current instance.</typeparam>
    [DebuggerTypeProxy(typeof(MemoryDebugView<>))]
    [DebuggerDisplay("{ToString(),raw}")]
    public readonly ref struct SpanOwner<T>
    {
#pragma warning disable IDE0032
        /// <summary>
        /// The usable length within <see cref="array"/>.
        /// </summary>
        private readonly int length;
#pragma warning restore IDE0032

        /// <summary>
        /// The <see cref="ArrayPool{T}"/> instance used to rent <see cref="array"/>.
        /// </summary>
        private readonly ArrayPool<T> pool;

        /// <summary>
        /// The underlying <typeparamref name="T"/> array.
        /// </summary>
        private readonly T[] array;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpanOwner{T}"/> struct with the specified parameters.
        /// </summary>
        /// <param name="length">The length of the new memory buffer to use.</param>
        /// <param name="pool">The <see cref="ArrayPool{T}"/> instance to use.</param>
        /// <param name="mode">Indicates the allocation mode to use for the new buffer to rent.</param>
        private SpanOwner(int length, ArrayPool<T> pool, AllocationMode mode)
        {
            this.length = length;
            this.pool = pool;
            this.array = pool.Rent(length);

            if (mode == AllocationMode.Clear)
            {
                this.array.AsSpan(0, length).Clear();
            }
        }

        /// <summary>
        /// Gets an empty <see cref="SpanOwner{T}"/> instance.
        /// </summary>
        [Pure]
        public static SpanOwner<T> Empty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(0, ArrayPool<T>.Shared, AllocationMode.Default);
        }

        /// <summary>
        /// Creates a new <see cref="SpanOwner{T}"/> instance with the specified parameters.
        /// </summary>
        /// <param name="size">The length of the new memory buffer to use.</param>
        /// <returns>A <see cref="SpanOwner{T}"/> instance of the requested length.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="size"/> is not valid.</exception>
        /// <remarks>This method is just a proxy for the <see langword="private"/> constructor, for clarity.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanOwner<T> Allocate(int size) => new(size, ArrayPool<T>.Shared, AllocationMode.Default);

        /// <summary>
        /// Creates a new <see cref="SpanOwner{T}"/> instance with the specified parameters.
        /// </summary>
        /// <param name="size">The length of the new memory buffer to use.</param>
        /// <param name="pool">The <see cref="ArrayPool{T}"/> instance to use.</param>
        /// <returns>A <see cref="SpanOwner{T}"/> instance of the requested length.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="size"/> is not valid.</exception>
        /// <remarks>This method is just a proxy for the <see langword="private"/> constructor, for clarity.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanOwner<T> Allocate(int size, ArrayPool<T> pool) => new(size, pool, AllocationMode.Default);

        /// <summary>
        /// Creates a new <see cref="SpanOwner{T}"/> instance with the specified parameters.
        /// </summary>
        /// <param name="size">The length of the new memory buffer to use.</param>
        /// <param name="mode">Indicates the allocation mode to use for the new buffer to rent.</param>
        /// <returns>A <see cref="SpanOwner{T}"/> instance of the requested length.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="size"/> is not valid.</exception>
        /// <remarks>This method is just a proxy for the <see langword="private"/> constructor, for clarity.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanOwner<T> Allocate(int size, AllocationMode mode) => new(size, ArrayPool<T>.Shared, mode);

        /// <summary>
        /// Creates a new <see cref="SpanOwner{T}"/> instance with the specified parameters.
        /// </summary>
        /// <param name="size">The length of the new memory buffer to use.</param>
        /// <param name="pool">The <see cref="ArrayPool{T}"/> instance to use.</param>
        /// <param name="mode">Indicates the allocation mode to use for the new buffer to rent.</param>
        /// <returns>A <see cref="SpanOwner{T}"/> instance of the requested length.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="size"/> is not valid.</exception>
        /// <remarks>This method is just a proxy for the <see langword="private"/> constructor, for clarity.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanOwner<T> Allocate(int size, ArrayPool<T> pool, AllocationMode mode) => new(size, pool, mode);

        /// <summary>
        /// Gets the number of items in the current instance
        /// </summary>
        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.length;
        }

        /// <summary>
        /// Gets a <see cref="Span{T}"/> wrapping the memory belonging to the current instance.
        /// </summary>
        public Span<T> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
#if NETCORE_RUNTIME || NET5_0
                ref T r0 = ref array!.DangerousGetReference();

                return MemoryMarshal.CreateSpan(ref r0, this.length);
#else
                return new Span<T>(this.array, 0, this.length);
#endif
            }
        }

        /// <summary>
        /// Returns a reference to the first element within the current instance, with no bounds check.
        /// </summary>
        /// <returns>A reference to the first element within the current instance.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T DangerousGetReference()
        {
            return ref this.array.DangerousGetReference();
        }

        /// <summary>
        /// Gets an <see cref="ArraySegment{T}"/> instance wrapping the underlying <typeparamref name="T"/> array in use.
        /// </summary>
        /// <returns>An <see cref="ArraySegment{T}"/> instance wrapping the underlying <typeparamref name="T"/> array in use.</returns>
        /// <remarks>
        /// This method is meant to be used when working with APIs that only accept an array as input, and should be used with caution.
        /// In particular, the returned array is rented from an array pool, and it is responsibility of the caller to ensure that it's
        /// not used after the current <see cref="SpanOwner{T}"/> instance is disposed. Doing so is considered undefined behavior,
        /// as the same array might be in use within another <see cref="SpanOwner{T}"/> instance.
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArraySegment<T> DangerousGetArray()
        {
            return new(array!, 0, this.length);
        }

        /// <summary>
        /// Implements the duck-typed <see cref="IDisposable.Dispose"/> method.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            this.pool.Return(this.array);
        }

        /// <inheritdoc/>
        [Pure]
        public override string ToString()
        {
            if (typeof(T) == typeof(char) &&
                this.array is char[] chars)
            {
                return new string(chars, 0, this.length);
            }

            // Same representation used in Span<T>
            return $"CommunityToolkit.HighPerformance.Buffers.SpanOwner<{typeof(T)}>[{this.length}]";
        }
    }
}