// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Toolkit.HighPerformance.Helpers
{
    /// <summary>
    /// Helpers for working with <see cref="object"/> instances.
    /// </summary>
    public static class ObjectMarshal
    {
        /// <summary>
        /// Calculates the byte offset to a specific field within a given <see cref="object"/>.
        /// </summary>
        /// <typeparam name="T">The type of field being referenced.</typeparam>
        /// <param name="obj">The input <see cref="object"/> hosting the target field.</param>
        /// <param name="data">A reference to a target field of type <typeparamref name="T"/> within <paramref name="obj"/>.</param>
        /// <returns>
        /// The <see cref="IntPtr"/> value representing the offset to the target field from the start of the object data
        /// for the parameter <paramref name="obj"/>. The offset is in relation to the first usable byte after the method table.
        /// </returns>
        /// <remarks>The input parameters are not validated, and it's responsibility of the caller to ensure that
        /// the <paramref name="data"/> reference is actually pointing to a memory location within <paramref name="obj"/>.
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr DangerousGetObjectDataByteOffset<T>(object obj, ref T data)
        {
            var rawObj = Unsafe.As<RawObjectData>(obj)!;
            ref byte r0 = ref rawObj.Data;
            ref byte r1 = ref Unsafe.As<T, byte>(ref data);

            return Unsafe.ByteOffset(ref r0, ref r1);
        }

        /// <summary>
        /// Gets a <typeparamref name="T"/> reference to data within a given <see cref="object"/> at a specified offset.
        /// </summary>
        /// <typeparam name="T">The type of reference to retrieve.</typeparam>
        /// <param name="obj">The input <see cref="object"/> hosting the target field.</param>
        /// <param name="offset">The input byte offset for the <typeparamref name="T"/> reference to retrieve.</param>
        /// <returns>A <typeparamref name="T"/> reference at a specified offset within <paramref name="obj"/>.</returns>
        /// <remarks>
        /// None of the input arguments is validated, and it is responsibility of the caller to ensure they are valid.
        /// In particular, using an invalid offset might cause the retrieved reference to be misaligned with the
        /// desired data, which would break the type system. Or, if the offset causes the retrieved reference to point
        /// to a memory location outside of the input <see cref="object"/> instance, that might lead to runtime crashes.
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T DangerousGetObjectDataReferenceAt<T>(object obj, IntPtr offset)
        {
            var rawObj = Unsafe.As<RawObjectData>(obj)!;
            ref byte r0 = ref rawObj.Data;
            ref byte r1 = ref Unsafe.AddByteOffset(ref r0, offset);
            ref T r2 = ref Unsafe.As<byte, T>(ref r1);

            return ref r2;
        }

        // Description adapted from CoreCLR, see:
        // https://source.dot.net/#System.Private.CoreLib/src/System/Runtime/CompilerServices/RuntimeHelpers.CoreCLR.cs,301.
        // CLR objects are laid out in memory as follows:
        // [ sync block || pMethodTable || raw data .. ]
        //                 ^               ^
        //                 |               \-- ref Unsafe.As<RawObjectData>(owner).Data
        //                 \-- object
        // The reference to RawObjectData.Data points to the first data byte in the
        // target object, skipping over the sync block, method table and string length.
        // Even though the description above links to the CoreCLR source, this approach
        // can actually work on any .NET runtime, as it doesn't rely on a specific memory
        // layout. Even if some 3rd party .NET runtime had some additional fields in the
        // object header, before the field being referenced, the returned offset would still
        // be valid when used on instances of that particular type, as it's only being
        // used as a relative offset from the location pointed by the object reference.
        [StructLayout(LayoutKind.Explicit)]
        private sealed class RawObjectData
        {
            [FieldOffset(0)]
#pragma warning disable SA1401 // Fields should be private
            public byte Data;
#pragma warning restore SA1401
        }

        /// <summary>
        /// Tries to get a boxed <typeparamref name="T"/> value from an input <see cref="object"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of value to try to unbox.</typeparam>
        /// <param name="obj">The input <see cref="object"/> instance to check.</param>
        /// <param name="value">The resulting <typeparamref name="T"/> value, if <paramref name="obj"/> was in fact a boxed <typeparamref name="T"/> value.</param>
        /// <returns><see langword="true"/> if a <typeparamref name="T"/> value was retrieved correctly, <see langword="false"/> otherwise.</returns>
        /// <remarks>
        /// This extension behaves just like the following method:
        /// <code>
        /// public static bool TryUnbox&lt;T>(object obj, out T value)
        /// {
        ///     if (obj is T)
        ///     {
        ///         value = (T)obj;
        ///
        ///         return true;
        ///     }
        ///
        ///     value = default;
        ///
        ///     return false;
        /// }
        /// </code>
        /// But in a more efficient way, and with the ability to also assign the unboxed value
        /// directly on an existing T variable, which is not possible with the code above.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryUnbox<T>(this object obj, out T value)
            where T : struct
        {
            if (obj.GetType() == typeof(T))
            {
                value = Unsafe.Unbox<T>(obj);

                return true;
            }

            value = default;

            return false;
        }

        /// <summary>
        /// Unboxes a <typeparamref name="T"/> value from an input <see cref="object"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of value to unbox.</typeparam>
        /// <param name="obj">The input <see cref="object"/> instance, representing a boxed <typeparamref name="T"/> value.</param>
        /// <returns>The <typeparamref name="T"/> value boxed in <paramref name="obj"/>.</returns>
        /// <exception cref="InvalidCastException">Thrown when <paramref name="obj"/> is not of type <typeparamref name="T"/>.</exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T DangerousUnbox<T>(object obj)
            where T : struct
        {
            return ref Unsafe.Unbox<T>(obj);
        }
    }
}