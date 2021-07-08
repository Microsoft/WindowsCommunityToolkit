// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using CommunityToolkit.HighPerformance.Helpers;
#if SPAN_RUNTIME_SUPPORT
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance.Buffers.Internals;
#endif
using CommunityToolkit.HighPerformance.Helpers.Internals;
using RuntimeHelpers = CommunityToolkit.HighPerformance.Helpers.Internals.RuntimeHelpers;

namespace CommunityToolkit.HighPerformance
{
    /// <summary>
    /// Helpers for working with the <see cref="Array"/> type.
    /// </summary>
    public static partial class ArrayExtensions
    {
        /// <summary>
        /// Returns a reference to the first element within a given 3D <typeparamref name="T"/> array, with no bounds checks.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input 3D <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input <typeparamref name="T"/> array instance.</param>
        /// <returns>A reference to the first element within <paramref name="array"/>, or the location it would have used, if <paramref name="array"/> is empty.</returns>
        /// <remarks>This method doesn't do any bounds checks, therefore it is responsibility of the caller to perform checks in case the returned value is dereferenced.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T DangerousGetReference<T>(this T[,,] array)
        {
#if NETCORE_RUNTIME
            var arrayData = Unsafe.As<RawArray3DData>(array)!;
            ref T r0 = ref Unsafe.As<byte, T>(ref arrayData.Data);

            return ref r0;
#else
            IntPtr offset = RuntimeHelpers.GetArray3DDataByteOffset<T>();

            return ref ObjectMarshal.DangerousGetObjectDataReferenceAt<T>(array, offset);
#endif
        }

        /// <summary>
        /// Returns a reference to an element at a specified coordinate within a given 3D <typeparamref name="T"/> array, with no bounds checks.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input 3D <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input 2D <typeparamref name="T"/> array instance.</param>
        /// <param name="i">The depth index of the element to retrieve within <paramref name="array"/>.</param>
        /// <param name="j">The vertical index of the element to retrieve within <paramref name="array"/>.</param>
        /// <param name="k">The horizontal index of the element to retrieve within <paramref name="array"/>.</param>
        /// <returns>A reference to the element within <paramref name="array"/> at the coordinate specified by <paramref name="i"/> and <paramref name="j"/>.</returns>
        /// <remarks>
        /// This method doesn't do any bounds checks, therefore it is responsibility of the caller to ensure the <paramref name="i"/>
        /// and <paramref name="j"/> parameters are valid. Furthermore, this extension will ignore the lower bounds for the input
        /// array, and will just assume that the input index is 0-based. It is responsibility of the caller to adjust the input
        /// indices to account for the actual lower bounds, if the input array has either axis not starting at 0.
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T DangerousGetReferenceAt<T>(this T[,,] array, int i, int j, int k)
        {
#if NETCORE_RUNTIME
            var arrayData = Unsafe.As<RawArray3DData>(array)!;
            nint offset =
                ((nint)(uint)i * (nint)(uint)arrayData.Height * (nint)(uint)arrayData.Width) +
                ((nint)(uint)j * (nint)(uint)arrayData.Width) + (nint)(uint)k;
            ref T r0 = ref Unsafe.As<byte, T>(ref arrayData.Data);
            ref T ri = ref Unsafe.Add(ref r0, offset);

            return ref ri;
#else
            int
                height = array.GetLength(1),
                width = array.GetLength(2);
            nint index =
                ((nint)(uint)i * (nint)(uint)height * (nint)(uint)width) +
                ((nint)(uint)j * (nint)(uint)width) + (nint)(uint)k;
            IntPtr offset = RuntimeHelpers.GetArray3DDataByteOffset<T>();
            ref T r0 = ref ObjectMarshal.DangerousGetObjectDataReferenceAt<T>(array, offset);
            ref T ri = ref Unsafe.Add(ref r0, index);

            return ref ri;
#endif
        }

#if NETCORE_RUNTIME
        // See description for this in the 2D partial file.
        // Using the CHW naming scheme here (like with RGB images).
        [StructLayout(LayoutKind.Sequential)]
        private sealed class RawArray3DData
        {
#pragma warning disable CS0649 // Unassigned fields
#pragma warning disable SA1401 // Fields should be private
            public IntPtr Length;
            public int Channel;
            public int Height;
            public int Width;
            public int ChannelLowerBound;
            public int HeightLowerBound;
            public int WidthLowerBound;
            public byte Data;
#pragma warning restore CS0649
#pragma warning restore SA1401
        }
#endif

#if SPAN_RUNTIME_SUPPORT
        /// <summary>
        /// Creates a new <see cref="Memory{T}"/> over an input 3D <typeparamref name="T"/> array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input 3D <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input 3D <typeparamref name="T"/> array instance.</param>
        /// <returns>A <see cref="Memory{T}"/> instance with the values of <paramref name="array"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Memory<T> AsMemory<T>(this T[,,]? array)
        {
            if (array is null)
            {
                return default;
            }

            if (array.IsCovariant())
            {
                ThrowArrayTypeMismatchException();
            }

            IntPtr offset = RuntimeHelpers.GetArray3DDataByteOffset<T>();
            int length = array.Length;

            return new RawObjectMemoryManager<T>(array, offset, length).Memory;
        }

        /// <summary>
        /// Creates a new <see cref="Span{T}"/> over an input 3D <typeparamref name="T"/> array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input 3D <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input 3D <typeparamref name="T"/> array instance.</param>
        /// <returns>A <see cref="Span{T}"/> instance with the values of <paramref name="array"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> AsSpan<T>(this T[,,]? array)
        {
            if (array is null)
            {
                return default;
            }

            if (array.IsCovariant())
            {
                ThrowArrayTypeMismatchException();
            }

            ref T r0 = ref array.DangerousGetReference();
            int length = array.Length;

            return MemoryMarshal.CreateSpan(ref r0, length);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Span{T}"/> struct wrapping a layer in a 3D array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input 3D <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The given 3D array to wrap.</param>
        /// <param name="depth">The target layer to map within <paramref name="array"/>.</param>
        /// <exception cref="ArrayTypeMismatchException">Thrown when <paramref name="array"/> doesn't match <typeparamref name="T"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="depth"/> is invalid.</exception>
        /// <returns>A <see cref="Span{T}"/> instance wrapping the target layer within <paramref name="array"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> AsSpan<T>(this T[,,] array, int depth)
        {
            if (array.IsCovariant())
            {
                ThrowArrayTypeMismatchException();
            }

            if ((uint)depth >= (uint)array.GetLength(0))
            {
                ThrowArgumentOutOfRangeExceptionForDepth();
            }

            ref T r0 = ref array.DangerousGetReferenceAt(depth, 0, 0);
            int length = checked(array.GetLength(1) * array.GetLength(2));

            return MemoryMarshal.CreateSpan(ref r0, length);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Memory{T}"/> struct wrapping a layer in a 3D array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input 3D <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The given 3D array to wrap.</param>
        /// <param name="depth">The target layer to map within <paramref name="array"/>.</param>
        /// <exception cref="ArrayTypeMismatchException">Thrown when <paramref name="array"/> doesn't match <typeparamref name="T"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="depth"/> is invalid.</exception>
        /// <returns>A <see cref="Memory{T}"/> instance wrapping the target layer within <paramref name="array"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Memory<T> AsMemory<T>(this T[,,] array, int depth)
        {
            if (array.IsCovariant())
            {
                ThrowArrayTypeMismatchException();
            }

            if ((uint)depth >= (uint)array.GetLength(0))
            {
                ThrowArgumentOutOfRangeExceptionForDepth();
            }

            ref T r0 = ref array.DangerousGetReferenceAt(depth, 0, 0);
            IntPtr offset = ObjectMarshal.DangerousGetObjectDataByteOffset(array, ref r0);
            int length = checked(array.GetLength(1) * array.GetLength(2));

            return new RawObjectMemoryManager<T>(array, offset, length).Memory;
        }
#endif

        /// <summary>
        /// Creates a new instance of the <see cref="Span2D{T}"/> struct wrapping a layer in a 3D array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input 3D <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The given 3D array to wrap.</param>
        /// <param name="depth">The target layer to map within <paramref name="array"/>.</param>
        /// <exception cref="ArrayTypeMismatchException">
        /// Thrown when <paramref name="array"/> doesn't match <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="ArgumentException">Thrown when either <paramref name="depth"/> is invalid.</exception>
        /// <returns>A <see cref="Span2D{T}"/> instance wrapping the target layer within <paramref name="array"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span2D<T> AsSpan2D<T>(this T[,,] array, int depth)
        {
            return new(array, depth);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Memory2D{T}"/> struct wrapping a layer in a 3D array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input 3D <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The given 3D array to wrap.</param>
        /// <param name="depth">The target layer to map within <paramref name="array"/>.</param>
        /// <exception cref="ArrayTypeMismatchException">
        /// Thrown when <paramref name="array"/> doesn't match <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="ArgumentException">Thrown when either <paramref name="depth"/> is invalid.</exception>
        /// <returns>A <see cref="Memory2D{T}"/> instance wrapping the target layer within <paramref name="array"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Memory2D<T> AsMemory2D<T>(this T[,,] array, int depth)
        {
            return new(array, depth);
        }

        /// <summary>
        /// Counts the number of occurrences of a given value into a target 3D <typeparamref name="T"/> array instance.
        /// </summary>
        /// <typeparam name="T">The type of items in the input 3D <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input 3D <typeparamref name="T"/> array instance.</param>
        /// <param name="value">The <typeparamref name="T"/> value to look for.</param>
        /// <returns>The number of occurrences of <paramref name="value"/> in <paramref name="array"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count<T>(this T[,,] array, T value)
            where T : IEquatable<T>
        {
            ref T r0 = ref array.DangerousGetReference();
            nint
                length = RuntimeHelpers.GetArrayNativeLength(array),
                count = SpanHelper.Count(ref r0, length, value);

            if ((nuint)count > int.MaxValue)
            {
                ThrowOverflowException();
            }

            return (int)count;
        }

        /// <summary>
        /// Gets a content hash from the input 3D <typeparamref name="T"/> array instance using the Djb2 algorithm.
        /// For more info, see the documentation for <see cref="ReadOnlySpanExtensions.GetDjb2HashCode{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of items in the input 3D <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input 3D <typeparamref name="T"/> array instance.</param>
        /// <returns>The Djb2 value for the input 3D <typeparamref name="T"/> array instance.</returns>
        /// <remarks>The Djb2 hash is fully deterministic and with no random components.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetDjb2HashCode<T>(this T[,,] array)
            where T : notnull
        {
            ref T r0 = ref array.DangerousGetReference();
            nint length = RuntimeHelpers.GetArrayNativeLength(array);

            return SpanHelper.GetDjb2HashCode(ref r0, length);
        }

        /// <summary>
        /// Checks whether or not a given <typeparamref name="T"/> array is covariant.
        /// </summary>
        /// <typeparam name="T">The type of items in the input <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input <typeparamref name="T"/> array instance.</param>
        /// <returns>Whether or not <paramref name="array"/> is covariant.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCovariant<T>(this T[,,] array)
        {
            return default(T) is null && array.GetType() != typeof(T[,,]);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> when the "depth" parameter is invalid.
        /// </summary>
        private static void ThrowArgumentOutOfRangeExceptionForDepth()
        {
            throw new ArgumentOutOfRangeException("depth");
        }
    }
}