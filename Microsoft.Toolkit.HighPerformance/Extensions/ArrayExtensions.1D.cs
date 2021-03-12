// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
#if NETCORE_RUNTIME || NET5_0
using System.Runtime.InteropServices;
#endif
using Microsoft.Toolkit.HighPerformance.Enumerables;
#if !NETCORE_RUNTIME && !NET5_0
using Microsoft.Toolkit.HighPerformance.Helpers;
#endif
using Microsoft.Toolkit.HighPerformance.Helpers.Internals;
using RuntimeHelpers = Microsoft.Toolkit.HighPerformance.Helpers.Internals.RuntimeHelpers;

namespace Microsoft.Toolkit.HighPerformance
{
    /// <summary>
    /// Helpers for working with the <see cref="Array"/> type.
    /// </summary>
    public static partial class ArrayExtensions
    {
        /// <summary>
        /// Returns a reference to the first element within a given <typeparamref name="T"/> array, with no bounds checks.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input <typeparamref name="T"/> array instance.</param>
        /// <returns>A reference to the first element within <paramref name="array"/>, or the location it would have used, if <paramref name="array"/> is empty.</returns>
        /// <remarks>This method doesn't do any bounds checks, therefore it is responsibility of the caller to perform checks in case the returned value is dereferenced.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T DangerousGetReference<T>(this T[] array)
        {
#if NET5_0
            return ref MemoryMarshal.GetArrayDataReference(array);
#elif NETCORE_RUNTIME
            var arrayData = Unsafe.As<RawArrayData>(array)!;
            ref T r0 = ref Unsafe.As<byte, T>(ref arrayData.Data);

            return ref r0;
#else
            IntPtr offset = RuntimeHelpers.GetArrayDataByteOffset<T>();

            return ref ObjectMarshal.DangerousGetObjectDataReferenceAt<T>(array, offset);
#endif
        }

        /// <summary>
        /// Returns a reference to an element at a specified index within a given <typeparamref name="T"/> array, with no bounds checks.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input <typeparamref name="T"/> array instance.</param>
        /// <param name="i">The index of the element to retrieve within <paramref name="array"/>.</param>
        /// <returns>A reference to the element within <paramref name="array"/> at the index specified by <paramref name="i"/>.</returns>
        /// <remarks>This method doesn't do any bounds checks, therefore it is responsibility of the caller to ensure the <paramref name="i"/> parameter is valid.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T DangerousGetReferenceAt<T>(this T[] array, int i)
        {
#if NET5_0
            ref T r0 = ref MemoryMarshal.GetArrayDataReference(array);
            ref T ri = ref Unsafe.Add(ref r0, (nint)(uint)i);

            return ref ri;
#elif NETCORE_RUNTIME
            var arrayData = Unsafe.As<RawArrayData>(array)!;
            ref T r0 = ref Unsafe.As<byte, T>(ref arrayData.Data);
            ref T ri = ref Unsafe.Add(ref r0, (nint)(uint)i);

            return ref ri;
#else
            IntPtr offset = RuntimeHelpers.GetArrayDataByteOffset<T>();
            ref T r0 = ref ObjectMarshal.DangerousGetObjectDataReferenceAt<T>(array, offset);
            ref T ri = ref Unsafe.Add(ref r0, (nint)(uint)i);

            return ref ri;
#endif
        }

#if NETCORE_RUNTIME
        // Description taken from CoreCLR: see https://source.dot.net/#System.Private.CoreLib/src/System/Runtime/CompilerServices/RuntimeHelpers.CoreCLR.cs,285.
        // CLR arrays are laid out in memory as follows (multidimensional array bounds are optional):
        // [ sync block || pMethodTable || num components || MD array bounds || array data .. ]
        //                 ^                                 ^                  ^ returned reference
        //                 |                                 \-- ref Unsafe.As<RawArrayData>(array).Data
        //                 \-- array
        // The base size of an array includes all the fields before the array data,
        // including the sync block and method table. The reference to RawData.Data
        // points at the number of components, skipping over these two pointer-sized fields.
        [StructLayout(LayoutKind.Sequential)]
        private sealed class RawArrayData
        {
#pragma warning disable CS0649 // Unassigned fields
#pragma warning disable SA1401 // Fields should be private
            public IntPtr Length;
            public byte Data;
#pragma warning restore CS0649
#pragma warning restore SA1401
        }
#endif

        /// <summary>
        /// Counts the number of occurrences of a given value into a target <typeparamref name="T"/> array instance.
        /// </summary>
        /// <typeparam name="T">The type of items in the input <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input <typeparamref name="T"/> array instance.</param>
        /// <param name="value">The <typeparamref name="T"/> value to look for.</param>
        /// <returns>The number of occurrences of <paramref name="value"/> in <paramref name="array"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count<T>(this T[] array, T value)
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
        /// Enumerates the items in the input <typeparamref name="T"/> array instance, as pairs of reference/index values.
        /// This extension should be used directly within a <see langword="foreach"/> loop:
        /// <code>
        /// int[] numbers = new[] { 1, 2, 3, 4, 5, 6, 7 };
        ///
        /// foreach (var item in numbers.Enumerate())
        /// {
        ///     // Access the index and value of each item here...
        ///     int index = item.Index;
        ///     ref int value = ref item.Value;
        /// }
        /// </code>
        /// The compiler will take care of properly setting up the <see langword="foreach"/> loop with the type returned from this method.
        /// </summary>
        /// <typeparam name="T">The type of items to enumerate.</typeparam>
        /// <param name="array">The source <typeparamref name="T"/> array to enumerate.</param>
        /// <returns>A wrapper type that will handle the reference/index enumeration for <paramref name="array"/>.</returns>
        /// <remarks>The returned <see cref="SpanEnumerable{T}"/> value shouldn't be used directly: use this extension in a <see langword="foreach"/> loop.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanEnumerable<T> Enumerate<T>(this T[] array)
        {
            return new(array);
        }

        /// <summary>
        /// Tokenizes the values in the input <typeparamref name="T"/> array instance using a specified separator.
        /// This extension should be used directly within a <see langword="foreach"/> loop:
        /// <code>
        /// char[] text = "Hello, world!".ToCharArray();
        ///
        /// foreach (var token in text.Tokenize(','))
        /// {
        ///     // Access the tokens here...
        /// }
        /// </code>
        /// The compiler will take care of properly setting up the <see langword="foreach"/> loop with the type returned from this method.
        /// </summary>
        /// <typeparam name="T">The type of items in the <typeparamref name="T"/> array to tokenize.</typeparam>
        /// <param name="array">The source <typeparamref name="T"/> array to tokenize.</param>
        /// <param name="separator">The separator <typeparamref name="T"/> item to use.</param>
        /// <returns>A wrapper type that will handle the tokenization for <paramref name="array"/>.</returns>
        /// <remarks>The returned <see cref="SpanTokenizer{T}"/> value shouldn't be used directly: use this extension in a <see langword="foreach"/> loop.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanTokenizer<T> Tokenize<T>(this T[] array, T separator)
            where T : IEquatable<T>
        {
            return new(array, separator);
        }

        /// <summary>
        /// Gets a content hash from the input <typeparamref name="T"/> array instance using the Djb2 algorithm.
        /// For more info, see the documentation for <see cref="ReadOnlySpanExtensions.GetDjb2HashCode{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of items in the input <typeparamref name="T"/> array instance.</typeparam>
        /// <param name="array">The input <typeparamref name="T"/> array instance.</param>
        /// <returns>The Djb2 value for the input <typeparamref name="T"/> array instance.</returns>
        /// <remarks>The Djb2 hash is fully deterministic and with no random components.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetDjb2HashCode<T>(this T[] array)
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
        public static bool IsCovariant<T>(this T[] array)
        {
            return default(T) is null && array.GetType() != typeof(T[]);
        }

        /// <summary>
        /// Throws an <see cref="OverflowException"/> when the "column" parameter is invalid.
        /// </summary>
        private static void ThrowOverflowException()
        {
            throw new OverflowException();
        }
    }
}
