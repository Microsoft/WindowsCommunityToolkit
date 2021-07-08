// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable SA1512

// The portable implementation in this type is originally from CoreFX.
// See https://github.com/dotnet/corefx/blob/release/2.1/src/System.Memory/src/System/SpanHelpers.cs.

using System;
using System.Diagnostics.Contracts;
#if !SPAN_RUNTIME_SUPPORT
using System.Reflection;
#endif
using System.Runtime.CompilerServices;

namespace CommunityToolkit.HighPerformance.Helpers.Internals
{
    /// <summary>
    /// A helper class that with utility methods for dealing with references, and other low-level details.
    /// It also contains some APIs that act as polyfills for .NET Standard 2.0 and below.
    /// </summary>
    internal static class RuntimeHelpers
    {
        /// <summary>
        /// Converts a length of items from one size to another (rounding towards zero).
        /// </summary>
        /// <typeparam name="TFrom">The source type of items.</typeparam>
        /// <typeparam name="TTo">The target type of items.</typeparam>
        /// <param name="length">The input length to convert.</param>
        /// <returns>The converted length for the specified argument and types.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ConvertLength<TFrom, TTo>(int length)
            where TFrom : unmanaged
            where TTo : unmanaged
        {
            if (sizeof(TFrom) == sizeof(TTo))
            {
                return length;
            }
            else if (sizeof(TFrom) == 1)
            {
                return (int)((uint)length / (uint)sizeof(TTo));
            }
            else
            {
                ulong targetLength = (ulong)(uint)length * (uint)sizeof(TFrom) / (uint)sizeof(TTo);

                return checked((int)targetLength);
            }
        }

        /// <summary>
        /// Gets the length of a given array as a native integer.
        /// </summary>
        /// <typeparam name="T">The type of values in the array.</typeparam>
        /// <param name="array">The input <see cref="Array"/> instance.</param>
        /// <returns>The total length of <paramref name="array"/> as a native integer.</returns>
        /// <remarks>
        /// This method is needed because this expression is not inlined correctly if the target array
        /// is only visible as a non-generic <see cref="Array"/> instance, because the C# compiler will
        /// not be able to emit the <see langword="ldlen"/> opcode instead of calling the right method.
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint GetArrayNativeLength<T>(T[] array)
        {
#if NETSTANDARD1_4
            // .NET Standard 1.4 doesn't include the API to get the long length, so
            // we just cast the length and throw in case the array is larger than
            // int.MaxValue. There's not much we can do in this specific case.
            return (nint)(uint)array.Length;
#else
            return (nint)array.LongLength;
#endif
        }

        /// <summary>
        /// Gets the length of a given array as a native integer.
        /// </summary>
        /// <param name="array">The input <see cref="Array"/> instance.</param>
        /// <returns>The total length of <paramref name="array"/> as a native integer.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint GetArrayNativeLength(Array array)
        {
#if NETSTANDARD1_4
            return (nint)(uint)array.Length;
#else
            return (nint)array.LongLength;
#endif
        }

        /// <summary>
        /// Gets the byte offset to the first <typeparamref name="T"/> element in a SZ array.
        /// </summary>
        /// <typeparam name="T">The type of values in the array.</typeparam>
        /// <returns>The byte offset to the first <typeparamref name="T"/> element in a SZ array.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr GetArrayDataByteOffset<T>()
        {
            return TypeInfo<T>.ArrayDataByteOffset;
        }

        /// <summary>
        /// Gets the byte offset to the first <typeparamref name="T"/> element in a 2D array.
        /// </summary>
        /// <typeparam name="T">The type of values in the array.</typeparam>
        /// <returns>The byte offset to the first <typeparamref name="T"/> element in a 2D array.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr GetArray2DDataByteOffset<T>()
        {
            return TypeInfo<T>.Array2DDataByteOffset;
        }

        /// <summary>
        /// Gets the byte offset to the first <typeparamref name="T"/> element in a 3D array.
        /// </summary>
        /// <typeparam name="T">The type of values in the array.</typeparam>
        /// <returns>The byte offset to the first <typeparamref name="T"/> element in a 3D array.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr GetArray3DDataByteOffset<T>()
        {
            return TypeInfo<T>.Array3DDataByteOffset;
        }

#if !SPAN_RUNTIME_SUPPORT
        /// <summary>
        /// Gets a byte offset describing a portable pinnable reference. This can either be an
        /// interior pointer into some object data (described with a valid <see cref="object"/> reference
        /// and a reference to some of its data), or a raw pointer (described with a <see langword="null"/>
        /// reference to an <see cref="object"/>, and a reference that is assumed to refer to pinned data).
        /// </summary>
        /// <typeparam name="T">The type of field being referenced.</typeparam>
        /// <param name="obj">The input <see cref="object"/> hosting the target field.</param>
        /// <param name="data">A reference to a target field of type <typeparamref name="T"/> within <paramref name="obj"/>.</param>
        /// <returns>
        /// The <see cref="IntPtr"/> value representing the offset to the target field from the start of the object data
        /// for the parameter <paramref name="obj"/>, or the value of the raw pointer passed as a tracked reference.
        /// </returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IntPtr GetObjectDataOrReferenceByteOffset<T>(object? obj, ref T data)
        {
            if (obj is null)
            {
                return (IntPtr)Unsafe.AsPointer(ref data);
            }

            return ObjectMarshal.DangerousGetObjectDataByteOffset(obj, ref data);
        }

        /// <summary>
        /// Gets a reference from data describing a portable pinnable reference. This can either be an
        /// interior pointer into some object data (described with a valid <see cref="object"/> reference
        /// and a byte offset into its data), or a raw pointer (described with a <see langword="null"/>
        /// reference to an <see cref="object"/>, and a byte offset representing the value of the raw pointer).
        /// </summary>
        /// <typeparam name="T">The type of reference to retrieve.</typeparam>
        /// <param name="obj">The input <see cref="object"/> hosting the target field.</param>
        /// <param name="offset">The input byte offset for the <typeparamref name="T"/> reference to retrieve.</param>
        /// <returns>A <typeparamref name="T"/> reference matching the given parameters.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ref T GetObjectDataAtOffsetOrPointerReference<T>(object? obj, IntPtr offset)
        {
            if (obj is null)
            {
                return ref Unsafe.AsRef<T>((void*)offset);
            }

            return ref ObjectMarshal.DangerousGetObjectDataReferenceAt<T>(obj, offset);
        }

        /// <summary>
        /// Checks whether or not a given type is a reference type or contains references.
        /// </summary>
        /// <typeparam name="T">The type to check.</typeparam>
        /// <returns>Whether or not <typeparamref name="T"/> respects the <see langword="unmanaged"/> constraint.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsReferenceOrContainsReferences<T>()
        {
            return TypeInfo<T>.IsReferenceOrContainsReferences;
        }

        /// <summary>
        /// Implements the logic for <see cref="IsReferenceOrContainsReferences{T}"/>.
        /// </summary>
        /// <param name="type">The current type to check.</param>
        /// <returns>Whether or not <paramref name="type"/> is a reference type or contains references.</returns>
        [Pure]
        private static bool IsReferenceOrContainsReferences(Type type)
        {
            // Common case, for primitive types
            if (type.GetTypeInfo().IsPrimitive)
            {
                return false;
            }

            if (!type.GetTypeInfo().IsValueType)
            {
                return true;
            }

            // Check if the type is Nullable<T>
            if (Nullable.GetUnderlyingType(type) is Type nullableType)
            {
                type = nullableType;
            }

            if (type.GetTypeInfo().IsEnum)
            {
                return false;
            }

            // Complex struct, recursively inspect all fields
            foreach (FieldInfo field in type.GetTypeInfo().DeclaredFields)
            {
                if (field.IsStatic)
                {
                    continue;
                }

                if (IsReferenceOrContainsReferences(field.FieldType))
                {
                    return true;
                }
            }

            return false;
        }
#endif

        /// <summary>
        /// A private generic class to preload type info for arbitrary runtime types.
        /// </summary>
        /// <typeparam name="T">The type to load info for.</typeparam>
        private static class TypeInfo<T>
        {
            /// <summary>
            /// The byte offset to the first <typeparamref name="T"/> element in a SZ array.
            /// </summary>
            public static readonly IntPtr ArrayDataByteOffset = MeasureArrayDataByteOffset();

            /// <summary>
            /// The byte offset to the first <typeparamref name="T"/> element in a 2D array.
            /// </summary>
            public static readonly IntPtr Array2DDataByteOffset = MeasureArray2DDataByteOffset();

            /// <summary>
            /// The byte offset to the first <typeparamref name="T"/> element in a 3D array.
            /// </summary>
            public static readonly IntPtr Array3DDataByteOffset = MeasureArray3DDataByteOffset();

#if !SPAN_RUNTIME_SUPPORT
            /// <summary>
            /// Indicates whether <typeparamref name="T"/> does not respect the <see langword="unmanaged"/> constraint.
            /// </summary>
            public static readonly bool IsReferenceOrContainsReferences = IsReferenceOrContainsReferences(typeof(T));
#endif

            /// <summary>
            /// Computes the value for <see cref="ArrayDataByteOffset"/>.
            /// </summary>
            /// <returns>The value of <see cref="ArrayDataByteOffset"/> for the current runtime.</returns>
            [Pure]
            private static IntPtr MeasureArrayDataByteOffset()
            {
                var array = new T[1];

                return ObjectMarshal.DangerousGetObjectDataByteOffset(array, ref array[0]);
            }

            /// <summary>
            /// Computes the value for <see cref="Array2DDataByteOffset"/>.
            /// </summary>
            /// <returns>The value of <see cref="Array2DDataByteOffset"/> for the current runtime.</returns>
            [Pure]
            private static IntPtr MeasureArray2DDataByteOffset()
            {
                var array = new T[1, 1];

                return ObjectMarshal.DangerousGetObjectDataByteOffset(array, ref array[0, 0]);
            }

            /// <summary>
            /// Computes the value for <see cref="Array3DDataByteOffset"/>.
            /// </summary>
            /// <returns>The value of <see cref="Array3DDataByteOffset"/> for the current runtime.</returns>
            [Pure]
            private static IntPtr MeasureArray3DDataByteOffset()
            {
                var array = new T[1, 1, 1];

                return ObjectMarshal.DangerousGetObjectDataByteOffset(array, ref array[0, 0, 0]);
            }
        }
    }
}