// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
#if SPAN_RUNTIME_SUPPORT
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance.Buffers.Internals;
#endif
using CommunityToolkit.HighPerformance.Enumerables;
using CommunityToolkit.HighPerformance.Helpers;
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
        /// Returns a reference to the first element within a given 2D <typeparamref name="T"/> array, with no bounds checks.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input 2D <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input <typeparamref name="T"/> array instance.</param>
        /// <returns>A reference to the first element within <paramref name="array"/>, or the location it would have used, if <paramref name="array"/> is empty.</returns>
        /// <remarks>This method doesn't do any bounds checks, therefore it is responsibility of the caller to perform checks in case the returned value is dereferenced.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T DangerousGetReference<T>(this T[,] array)
        {
#if NETCORE_RUNTIME
            var arrayData = Unsafe.As<RawArray2DData>(array)!;
            ref T r0 = ref Unsafe.As<byte, T>(ref arrayData.Data);

            return ref r0;
#else
            IntPtr offset = RuntimeHelpers.GetArray2DDataByteOffset<T>();

            return ref ObjectMarshal.DangerousGetObjectDataReferenceAt<T>(array, offset);
#endif
        }

        /// <summary>
        /// Returns a reference to an element at a specified coordinate within a given 2D <typeparamref name="T"/> array, with no bounds checks.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input 2D <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input 2D <typeparamref name="T"/> array instance.</param>
        /// <param name="i">The vertical index of the element to retrieve within <paramref name="array"/>.</param>
        /// <param name="j">The horizontal index of the element to retrieve within <paramref name="array"/>.</param>
        /// <returns>A reference to the element within <paramref name="array"/> at the coordinate specified by <paramref name="i"/> and <paramref name="j"/>.</returns>
        /// <remarks>
        /// This method doesn't do any bounds checks, therefore it is responsibility of the caller to ensure the <paramref name="i"/>
        /// and <paramref name="j"/> parameters are valid. Furthermore, this extension will ignore the lower bounds for the input
        /// array, and will just assume that the input index is 0-based. It is responsibility of the caller to adjust the input
        /// indices to account for the actual lower bounds, if the input array has either axis not starting at 0.
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T DangerousGetReferenceAt<T>(this T[,] array, int i, int j)
        {
#if NETCORE_RUNTIME
            var arrayData = Unsafe.As<RawArray2DData>(array)!;
            nint offset = ((nint)(uint)i * (nint)(uint)arrayData.Width) + (nint)(uint)j;
            ref T r0 = ref Unsafe.As<byte, T>(ref arrayData.Data);
            ref T ri = ref Unsafe.Add(ref r0, offset);

            return ref ri;
#else
            int width = array.GetLength(1);
            nint index = ((nint)(uint)i * (nint)(uint)width) + (nint)(uint)j;
            IntPtr offset = RuntimeHelpers.GetArray2DDataByteOffset<T>();
            ref T r0 = ref ObjectMarshal.DangerousGetObjectDataReferenceAt<T>(array, offset);
            ref T ri = ref Unsafe.Add(ref r0, index);

            return ref ri;
#endif
        }

#if NETCORE_RUNTIME
        // Description adapted from CoreCLR: see https://source.dot.net/#System.Private.CoreLib/src/System/Runtime/CompilerServices/RuntimeHelpers.CoreCLR.cs,285.
        // CLR 2D arrays are laid out in memory as follows:
        // [ sync block || pMethodTable || Length (padded to IntPtr) || HxW || HxW bounds || array data .. ]
        //                 ^                                                                 ^
        //                 |                                                                 \-- ref Unsafe.As<RawArray2DData>(array).Data
        //                 \-- array
        // The length is always padded to IntPtr just like with SZ arrays.
        // The total data padding is therefore 20 bytes on x86 (4 + 4 + 4 + 4 + 4), or 24 bytes on x64.
        [StructLayout(LayoutKind.Sequential)]
        private sealed class RawArray2DData
        {
#pragma warning disable CS0649 // Unassigned fields
#pragma warning disable SA1401 // Fields should be private
            public IntPtr Length;
            public int Height;
            public int Width;
            public int HeightLowerBound;
            public int WidthLowerBound;
            public byte Data;
#pragma warning restore CS0649
#pragma warning restore SA1401
        }
#endif

        /// <summary>
        /// Returns a <see cref="RefEnumerable{T}"/> over a row in a given 2D <typeparamref name="T"/> array instance.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input 2D <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input <typeparamref name="T"/> array instance.</param>
        /// <param name="row">The target row to retrieve (0-based index).</param>
        /// <returns>A <see cref="RefEnumerable{T}"/> with the items from the target row within <paramref name="array"/>.</returns>
        /// <remarks>The returned <see cref="RefEnumerable{T}"/> value shouldn't be used directly: use this extension in a <see langword="foreach"/> loop.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when one of the input parameters is out of range.</exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RefEnumerable<T> GetRow<T>(this T[,] array, int row)
        {
            if (array.IsCovariant())
            {
                ThrowArrayTypeMismatchException();
            }

            int height = array.GetLength(0);

            if ((uint)row >= (uint)height)
            {
                ThrowArgumentOutOfRangeExceptionForRow();
            }

            int width = array.GetLength(1);

#if SPAN_RUNTIME_SUPPORT
            ref T r0 = ref array.DangerousGetReferenceAt(row, 0);

            return new RefEnumerable<T>(ref r0, width, 1);
#else
            ref T r0 = ref array.DangerousGetReferenceAt(row, 0);
            IntPtr offset = ObjectMarshal.DangerousGetObjectDataByteOffset(array, ref r0);

            return new RefEnumerable<T>(array, offset, width, 1);
#endif
        }

        /// <summary>
        /// Returns a <see cref="RefEnumerable{T}"/> that returns the items from a given column in a given 2D <typeparamref name="T"/> array instance.
        /// This extension should be used directly within a <see langword="foreach"/> loop:
        /// <code>
        /// int[,] matrix =
        /// {
        ///     { 1, 2, 3 },
        ///     { 4, 5, 6 },
        ///     { 7, 8, 9 }
        /// };
        ///
        /// foreach (ref int number in matrix.GetColumn(1))
        /// {
        ///     // Access the current number by reference here...
        /// }
        /// </code>
        /// The compiler will take care of properly setting up the <see langword="foreach"/> loop with the type returned from this method.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input 2D <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input <typeparamref name="T"/> array instance.</param>
        /// <param name="column">The target column to retrieve (0-based index).</param>
        /// <returns>A wrapper type that will handle the column enumeration for <paramref name="array"/>.</returns>
        /// <remarks>The returned <see cref="RefEnumerable{T}"/> value shouldn't be used directly: use this extension in a <see langword="foreach"/> loop.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when one of the input parameters is out of range.</exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RefEnumerable<T> GetColumn<T>(this T[,] array, int column)
        {
            if (array.IsCovariant())
            {
                ThrowArrayTypeMismatchException();
            }

            int width = array.GetLength(1);

            if ((uint)column >= (uint)width)
            {
                ThrowArgumentOutOfRangeExceptionForColumn();
            }

            int height = array.GetLength(0);

#if SPAN_RUNTIME_SUPPORT
            ref T r0 = ref array.DangerousGetReferenceAt(0, column);

            return new RefEnumerable<T>(ref r0, height, width);
#else
            ref T r0 = ref array.DangerousGetReferenceAt(0, column);
            IntPtr offset = ObjectMarshal.DangerousGetObjectDataByteOffset(array, ref r0);

            return new RefEnumerable<T>(array, offset, height, width);
#endif
        }

        /// <summary>
        /// Creates a new <see cref="Span2D{T}"/> over an input 2D <typeparamref name="T"/> array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input 2D <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input 2D <typeparamref name="T"/> array instance.</param>
        /// <returns>A <see cref="Span2D{T}"/> instance with the values of <paramref name="array"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span2D<T> AsSpan2D<T>(this T[,]? array)
        {
            return new(array);
        }

        /// <summary>
        /// Creates a new <see cref="Span2D{T}"/> over an input 2D <typeparamref name="T"/> array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input 2D <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input 2D <typeparamref name="T"/> array instance.</param>
        /// <param name="row">The target row to map within <paramref name="array"/>.</param>
        /// <param name="column">The target column to map within <paramref name="array"/>.</param>
        /// <param name="height">The height to map within <paramref name="array"/>.</param>
        /// <param name="width">The width to map within <paramref name="array"/>.</param>
        /// <exception cref="ArrayTypeMismatchException">
        /// Thrown when <paramref name="array"/> doesn't match <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when either <paramref name="height"/>, <paramref name="width"/> or <paramref name="height"/>
        /// are negative or not within the bounds that are valid for <paramref name="array"/>.
        /// </exception>
        /// <returns>A <see cref="Span2D{T}"/> instance with the values of <paramref name="array"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span2D<T> AsSpan2D<T>(this T[,]? array, int row, int column, int height, int width)
        {
            return new(array, row, column, height, width);
        }

        /// <summary>
        /// Creates a new <see cref="Memory2D{T}"/> over an input 2D <typeparamref name="T"/> array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input 2D <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input 2D <typeparamref name="T"/> array instance.</param>
        /// <returns>A <see cref="Memory2D{T}"/> instance with the values of <paramref name="array"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Memory2D<T> AsMemory2D<T>(this T[,]? array)
        {
            return new(array);
        }

        /// <summary>
        /// Creates a new <see cref="Memory2D{T}"/> over an input 2D <typeparamref name="T"/> array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input 2D <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input 2D <typeparamref name="T"/> array instance.</param>
        /// <param name="row">The target row to map within <paramref name="array"/>.</param>
        /// <param name="column">The target column to map within <paramref name="array"/>.</param>
        /// <param name="height">The height to map within <paramref name="array"/>.</param>
        /// <param name="width">The width to map within <paramref name="array"/>.</param>
        /// <exception cref="ArrayTypeMismatchException">
        /// Thrown when <paramref name="array"/> doesn't match <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when either <paramref name="height"/>, <paramref name="width"/> or <paramref name="height"/>
        /// are negative or not within the bounds that are valid for <paramref name="array"/>.
        /// </exception>
        /// <returns>A <see cref="Memory2D{T}"/> instance with the values of <paramref name="array"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Memory2D<T> AsMemory2D<T>(this T[,]? array, int row, int column, int height, int width)
        {
            return new(array, row, column, height, width);
        }

#if SPAN_RUNTIME_SUPPORT
        /// <summary>
        /// Returns a <see cref="Span{T}"/> over a row in a given 2D <typeparamref name="T"/> array instance.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input 2D <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input <typeparamref name="T"/> array instance.</param>
        /// <param name="row">The target row to retrieve (0-based index).</param>
        /// <returns>A <see cref="Span{T}"/> with the items from the target row within <paramref name="array"/>.</returns>
        /// <exception cref="ArrayTypeMismatchException">Thrown when <paramref name="array"/> doesn't match <typeparamref name="T"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="row"/> is invalid.</exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> GetRowSpan<T>(this T[,] array, int row)
        {
            if (array.IsCovariant())
            {
                ThrowArrayTypeMismatchException();
            }

            if ((uint)row >= (uint)array.GetLength(0))
            {
                ThrowArgumentOutOfRangeExceptionForRow();
            }

            ref T r0 = ref array.DangerousGetReferenceAt(row, 0);

            return MemoryMarshal.CreateSpan(ref r0, array.GetLength(1));
        }

        /// <summary>
        /// Returns a <see cref="Memory{T}"/> over a row in a given 2D <typeparamref name="T"/> array instance.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input 2D <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input <typeparamref name="T"/> array instance.</param>
        /// <param name="row">The target row to retrieve (0-based index).</param>
        /// <returns>A <see cref="Memory{T}"/> with the items from the target row within <paramref name="array"/>.</returns>
        /// <exception cref="ArrayTypeMismatchException">Thrown when <paramref name="array"/> doesn't match <typeparamref name="T"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="row"/> is invalid.</exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Memory<T> GetRowMemory<T>(this T[,] array, int row)
        {
            if (array.IsCovariant())
            {
                ThrowArrayTypeMismatchException();
            }

            if ((uint)row >= (uint)array.GetLength(0))
            {
                ThrowArgumentOutOfRangeExceptionForRow();
            }

            ref T r0 = ref array.DangerousGetReferenceAt(row, 0);
            IntPtr offset = ObjectMarshal.DangerousGetObjectDataByteOffset(array, ref r0);

            return new RawObjectMemoryManager<T>(array, offset, array.GetLength(1)).Memory;
        }

        /// <summary>
        /// Creates a new <see cref="Memory{T}"/> over an input 2D <typeparamref name="T"/> array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input 2D <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input 2D <typeparamref name="T"/> array instance.</param>
        /// <returns>A <see cref="Memory{T}"/> instance with the values of <paramref name="array"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Memory<T> AsMemory<T>(this T[,]? array)
        {
            if (array is null)
            {
                return default;
            }

            if (array.IsCovariant())
            {
                ThrowArrayTypeMismatchException();
            }

            IntPtr offset = RuntimeHelpers.GetArray2DDataByteOffset<T>();
            int length = array.Length;

            return new RawObjectMemoryManager<T>(array, offset, length).Memory;
        }

        /// <summary>
        /// Creates a new <see cref="Span{T}"/> over an input 2D <typeparamref name="T"/> array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input 2D <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input 2D <typeparamref name="T"/> array instance.</param>
        /// <returns>A <see cref="Span{T}"/> instance with the values of <paramref name="array"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> AsSpan<T>(this T[,]? array)
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
#endif

        /// <summary>
        /// Counts the number of occurrences of a given value into a target 2D <typeparamref name="T"/> array instance.
        /// </summary>
        /// <typeparam name="T">The type of items in the input 2D <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input 2D <typeparamref name="T"/> array instance.</param>
        /// <param name="value">The <typeparamref name="T"/> value to look for.</param>
        /// <returns>The number of occurrences of <paramref name="value"/> in <paramref name="array"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int Count<T>(this T[,] array, T value)
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
        /// Gets a content hash from the input 2D <typeparamref name="T"/> array instance using the Djb2 algorithm.
        /// For more info, see the documentation for <see cref="ReadOnlySpanExtensions.GetDjb2HashCode{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of items in the input 2D <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input 2D <typeparamref name="T"/> array instance.</param>
        /// <returns>The Djb2 value for the input 2D <typeparamref name="T"/> array instance.</returns>
        /// <remarks>The Djb2 hash is fully deterministic and with no random components.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int GetDjb2HashCode<T>(this T[,] array)
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
        public static bool IsCovariant<T>(this T[,] array)
        {
            return default(T) is null && array.GetType() != typeof(T[,]);
        }

        /// <summary>
        /// Throws an <see cref="ArrayTypeMismatchException"/> when using an array of an invalid type.
        /// </summary>
        private static void ThrowArrayTypeMismatchException()
        {
            throw new ArrayTypeMismatchException("The given array doesn't match the specified type T");
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> when the "row" parameter is invalid.
        /// </summary>
        private static void ThrowArgumentOutOfRangeExceptionForRow()
        {
            throw new ArgumentOutOfRangeException("row");
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> when the "column" parameter is invalid.
        /// </summary>
        private static void ThrowArgumentOutOfRangeExceptionForColumn()
        {
            throw new ArgumentOutOfRangeException("column");
        }
    }
}