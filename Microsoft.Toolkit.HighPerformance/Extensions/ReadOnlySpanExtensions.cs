// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Toolkit.HighPerformance.Enumerables;
using Microsoft.Toolkit.HighPerformance.Helpers.Internals;

namespace Microsoft.Toolkit.HighPerformance
{
    /// <summary>
    /// Helpers for working with the <see cref="ReadOnlySpan{T}"/> type.
    /// </summary>
    public static class ReadOnlySpanExtensions
    {
        /// <summary>
        /// Returns a reference to the first element within a given <see cref="ReadOnlySpan{T}"/>, with no bounds checks.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
        /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance.</param>
        /// <returns>A reference to the first element within <paramref name="span"/>.</returns>
        /// <remarks>This method doesn't do any bounds checks, therefore it is responsibility of the caller to perform checks in case the returned value is dereferenced.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T DangerousGetReference<T>(this ReadOnlySpan<T> span)
        {
            return ref MemoryMarshal.GetReference(span);
        }

        /// <summary>
        /// Returns a reference to an element at a specified index within a given <see cref="ReadOnlySpan{T}"/>, with no bounds checks.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
        /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance.</param>
        /// <param name="i">The index of the element to retrieve within <paramref name="span"/>.</param>
        /// <returns>A reference to the element within <paramref name="span"/> at the index specified by <paramref name="i"/>.</returns>
        /// <remarks>This method doesn't do any bounds checks, therefore it is responsibility of the caller to ensure the <paramref name="i"/> parameter is valid.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T DangerousGetReferenceAt<T>(this ReadOnlySpan<T> span, int i)
        {
            // Here we assume the input index will never be negative, so we do a (nint)(uint) cast
            // to force the JIT to skip the sign extension when going from int to native int.
            // On .NET Core 3.1, if we only use Unsafe.Add(ref r0, i), we get the following:
            // =============================
            // L0000: mov rax, [rcx]
            // L0003: movsxd rdx, edx
            // L0006: lea rax, [rax+rdx*4]
            // L000a: ret
            // =============================
            // Note the movsxd (move with sign extension) to expand the index passed in edx to
            // the whole rdx register. This is unnecessary and more expensive than just a mov,
            // which when done to a large register size automatically zeroes the upper bits.
            // With the (nint)(uint) cast, we get the following codegen instead:
            // =============================
            // L0000: mov rax, [rcx]
            // L0003: mov edx, edx
            // L0005: lea rax, [rax+rdx*4]
            // L0009: ret
            // =============================
            // Here we can see how the index is extended to a native integer with just a mov,
            // which effectively only zeroes the upper bits of the same register used as source.
            // These three casts are a bit verbose, but they do the trick on both 32 bit and 64
            // bit architectures, producing optimal code in both cases (they are either completely
            // elided on 32 bit systems, or result in the correct register expansion when on 64 bit).
            // We first do an unchecked conversion to uint (which is just a reinterpret-cast). We
            // then cast to nint, so that we can obtain an IntPtr value without the range check (since
            // uint could be out of range there if the original index was negative). The final result
            // is a clean mov as shown above. This will eventually be natively supported by the JIT
            // compiler (see https://github.com/dotnet/runtime/issues/38794), but doing this here
            // still ensures the optimal codegen even on existing runtimes (eg. .NET Core 2.1 and 3.1).
            ref T r0 = ref MemoryMarshal.GetReference(span);
            ref T ri = ref Unsafe.Add(ref r0, (nint)(uint)i);

            return ref ri;
        }

        /// <summary>
        /// Returns a reference to an element at a specified index within a given <see cref="ReadOnlySpan{T}"/>, with no bounds checks.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
        /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance.</param>
        /// <param name="i">The index of the element to retrieve within <paramref name="span"/>.</param>
        /// <returns>A reference to the element within <paramref name="span"/> at the index specified by <paramref name="i"/>.</returns>
        /// <remarks>This method doesn't do any bounds checks, therefore it is responsibility of the caller to ensure the <paramref name="i"/> parameter is valid.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T DangerousGetReferenceAt<T>(this ReadOnlySpan<T> span, nint i)
        {
            ref T r0 = ref MemoryMarshal.GetReference(span);
            ref T ri = ref Unsafe.Add(ref r0, i);

            return ref ri;
        }

        /// <summary>
        /// Returns a reference to the first element within a given <see cref="ReadOnlySpan{T}"/>, clamping the input index in the valid range.
        /// If the <paramref name="i"/> parameter exceeds the length of <paramref name="span"/>, it will be clamped to 0.
        /// Therefore, the returned reference will always point to a valid element within <paramref name="span"/>, assuming it is not empty.
        /// This method is specifically meant to efficiently index lookup tables, especially if they point to constant data.
        /// Consider this example where a lookup table is used to validate whether a given character is within a specific set:
        /// <code>
        /// public static ReadOnlySpan&lt;bool> ValidSetLookupTable => new bool[]
        /// {
        ///     false, true, true, true, true, true, false, true,
        ///     false, false, true, false, true, false, true, false,
        ///     true, false, false, true, false, false, false, false,
        ///     false, false, false, false, true, true, false, true
        /// };
        ///
        /// int ch = Console.Read();
        /// bool isValid = ValidSetLookupTable.DangerousGetLookupReference(ch);
        /// </code>
        /// Even if the input index is outside the range of the lookup table, being clamped to 0, it will
        /// just cause the value 0 to be returned in this case, which is functionally the same for the check
        /// being performed. This extension can easily be used whenever the first position in a lookup
        /// table being referenced corresponds to a falsey value, like in this case.
        /// Additionally, the example above leverages a compiler optimization introduced with C# 7.3,
        /// which allows <see cref="ReadOnlySpan{T}"/> instances pointing to compile-time constant data
        /// to be directly mapped to the static .text section in the final assembly: the array being
        /// created in code will never actually be allocated, and the <see cref="ReadOnlySpan{T}"/> will
        /// just point to constant data. Note that this only works for blittable values that are not
        /// dependent on the byte endianness of the system, like <see cref="byte"/> or <see cref="bool"/>.
        /// For more info, see <see href="https://vcsjones.dev/2019/02/01/csharp-readonly-span-bytes-static/"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
        /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance.</param>
        /// <param name="i">The index of the element to retrieve within <paramref name="span"/>.</param>
        /// <returns>
        /// A reference to the element within <paramref name="span"/> at the index specified by <paramref name="i"/>,
        /// or a reference to the first element within <paramref name="span"/> if <paramref name="i"/> was not a valid index.
        /// </returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ref readonly T DangerousGetLookupReferenceAt<T>(this ReadOnlySpan<T> span, int i)
        {
            // Check whether the input is in range by first casting both
            // operands to uint and then comparing them, as this allows
            // the test to also identify cases where the input index is
            // less than zero. The resulting bool is then reinterpreted
            // as a byte (either 1 or 0), and then decremented.
            // This will result in either 0 if the input index was
            // valid for the target span, or -1 (0xFFFFFFFF) otherwise.
            // The result is then negated, producing the value 0xFFFFFFFF
            // for valid indices, or 0 otherwise. The generated mask
            // is then combined with the original index. This leaves
            // the index intact if it was valid, otherwise zeros it.
            // The computed offset is finally used to access the
            // lookup table, and it is guaranteed to never go out of
            // bounds unless the input span was just empty, which for a
            // lookup table can just be assumed to always be false.
            bool isInRange = (uint)i < (uint)span.Length;
            byte rangeFlag = *(byte*)&isInRange;
            uint
                negativeFlag = unchecked(rangeFlag - 1u),
                mask = ~negativeFlag,
                offset = (uint)i & mask;
            ref T r0 = ref MemoryMarshal.GetReference(span);
            ref T r1 = ref Unsafe.Add(ref r0, (nint)offset);

            return ref r1;
        }

#if SPAN_RUNTIME_SUPPORT
        /// <summary>
        /// Returns a <see cref="ReadOnlySpan2D{T}"/> instance wrapping the underlying data for the given <see cref="ReadOnlySpan{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of items in the input <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
        /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance.</param>
        /// <param name="height">The height of the resulting 2D area.</param>
        /// <param name="width">The width of each row in the resulting 2D area.</param>
        /// <returns>The resulting <see cref="ReadOnlySpan2D{T}"/> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when one of the input parameters is out of range.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the requested area is outside of bounds for <paramref name="span"/>.
        /// </exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan2D<T> AsSpan2D<T>(this ReadOnlySpan<T> span, int height, int width)
        {
            return new(span, height, width);
        }

        /// <summary>
        /// Returns a <see cref="ReadOnlySpan2D{T}"/> instance wrapping the underlying data for the given <see cref="ReadOnlySpan{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of items in the input <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
        /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance.</param>
        /// <param name="offset">The initial offset within <paramref name="span"/>.</param>
        /// <param name="height">The height of the resulting 2D area.</param>
        /// <param name="width">The width of each row in the resulting 2D area.</param>
        /// <param name="pitch">The pitch in the resulting 2D area.</param>
        /// <returns>The resulting <see cref="ReadOnlySpan2D{T}"/> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when one of the input parameters is out of range.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the requested area is outside of bounds for <paramref name="span"/>.
        /// </exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan2D<T> AsSpan2D<T>(this ReadOnlySpan<T> span, int offset, int height, int width, int pitch)
        {
            return new(span, offset, height, width, pitch);
        }
#endif

        /// <summary>
        /// Gets the index of an element of a given <see cref="ReadOnlySpan{T}"/> from its reference.
        /// </summary>
        /// <typeparam name="T">The type if items in the input <see cref="ReadOnlySpan{T}"/>.</typeparam>
        /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> to calculate the index for.</param>
        /// <param name="value">The reference to the target item to get the index for.</param>
        /// <returns>The index of <paramref name="value"/> within <paramref name="span"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> does not belong to <paramref name="span"/>.</exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf<T>(this ReadOnlySpan<T> span, in T value)
        {
            ref T r0 = ref MemoryMarshal.GetReference(span);
            ref T r1 = ref Unsafe.AsRef(value);
            IntPtr byteOffset = Unsafe.ByteOffset(ref r0, ref r1);

            nint elementOffset = byteOffset / (nint)(uint)Unsafe.SizeOf<T>();

            if ((nuint)elementOffset >= (uint)span.Length)
            {
                SpanExtensions.ThrowArgumentOutOfRangeExceptionForInvalidReference();
            }

            return (int)elementOffset;
        }

        /// <summary>
        /// Counts the number of occurrences of a given value into a target <see cref="ReadOnlySpan{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of items in the input <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
        /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance to read.</param>
        /// <param name="value">The <typeparamref name="T"/> value to look for.</param>
        /// <returns>The number of occurrences of <paramref name="value"/> in <paramref name="span"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count<T>(this ReadOnlySpan<T> span, T value)
            where T : IEquatable<T>
        {
            ref T r0 = ref MemoryMarshal.GetReference(span);
            nint length = (nint)(uint)span.Length;

            return (int)SpanHelper.Count(ref r0, length, value);
        }

        /// <summary>
        /// Casts a <see cref="ReadOnlySpan{T}"/> of one primitive type <typeparamref name="T"/> to <see cref="ReadOnlySpan{T}"/> of bytes.
        /// </summary>
        /// <typeparam name="T">The type if items in the source <see cref="ReadOnlySpan{T}"/>.</typeparam>
        /// <param name="span">The source slice, of type <typeparamref name="T"/>.</param>
        /// <returns>A <see cref="ReadOnlySpan{T}"/> of bytes.</returns>
        /// <exception cref="OverflowException">
        /// Thrown if the <see cref="ReadOnlySpan{T}.Length"/> property of the new <see cref="ReadOnlySpan{T}"/> would exceed <see cref="int.MaxValue"/>.
        /// </exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<byte> AsBytes<T>(this ReadOnlySpan<T> span)
            where T : unmanaged
        {
            return MemoryMarshal.AsBytes(span);
        }

        /// <summary>
        /// Casts a <see cref="ReadOnlySpan{T}"/> of one primitive type <typeparamref name="TFrom"/> to another primitive type <typeparamref name="TTo"/>.
        /// </summary>
        /// <typeparam name="TFrom">The type of items in the source <see cref="ReadOnlySpan{T}"/>.</typeparam>
        /// <typeparam name="TTo">The type of items in the destination <see cref="ReadOnlySpan{T}"/>.</typeparam>
        /// <param name="span">The source slice, of type <typeparamref name="TFrom"/>.</param>
        /// <returns>A <see cref="ReadOnlySpan{T}"/> of type <typeparamref name="TTo"/></returns>
        /// <remarks>
        /// Supported only for platforms that support misaligned memory access or when the memory block is aligned by other means.
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<TTo> Cast<TFrom, TTo>(this ReadOnlySpan<TFrom> span)
            where TFrom : unmanaged
            where TTo : unmanaged
        {
            return MemoryMarshal.Cast<TFrom, TTo>(span);
        }

        /// <summary>
        /// Enumerates the items in the input <see cref="ReadOnlySpan{T}"/> instance, as pairs of value/index values.
        /// This extension should be used directly within a <see langword="foreach"/> loop:
        /// <code>
        /// ReadOnlySpan&lt;string&gt; words = new[] { "Hello", ", ", "world", "!" };
        ///
        /// foreach (var item in words.Enumerate())
        /// {
        ///     // Access the index and value of each item here...
        ///     int index = item.Index;
        ///     string value = item.Value;
        /// }
        /// </code>
        /// The compiler will take care of properly setting up the <see langword="foreach"/> loop with the type returned from this method.
        /// </summary>
        /// <typeparam name="T">The type of items to enumerate.</typeparam>
        /// <param name="span">The source <see cref="ReadOnlySpan{T}"/> to enumerate.</param>
        /// <returns>A wrapper type that will handle the value/index enumeration for <paramref name="span"/>.</returns>
        /// <remarks>The returned <see cref="ReadOnlySpanEnumerable{T}"/> value shouldn't be used directly: use this extension in a <see langword="foreach"/> loop.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpanEnumerable<T> Enumerate<T>(this ReadOnlySpan<T> span)
        {
            return new(span);
        }

        /// <summary>
        /// Tokenizes the values in the input <see cref="ReadOnlySpan{T}"/> instance using a specified separator.
        /// This extension should be used directly within a <see langword="foreach"/> loop:
        /// <code>
        /// ReadOnlySpan&lt;char&gt; text = "Hello, world!";
        ///
        /// foreach (var token in text.Tokenize(','))
        /// {
        ///     // Access the tokens here...
        /// }
        /// </code>
        /// The compiler will take care of properly setting up the <see langword="foreach"/> loop with the type returned from this method.
        /// </summary>
        /// <typeparam name="T">The type of items in the <see cref="ReadOnlySpan{T}"/> to tokenize.</typeparam>
        /// <param name="span">The source <see cref="ReadOnlySpan{T}"/> to tokenize.</param>
        /// <param name="separator">The separator <typeparamref name="T"/> item to use.</param>
        /// <returns>A wrapper type that will handle the tokenization for <paramref name="span"/>.</returns>
        /// <remarks>The returned <see cref="ReadOnlySpanTokenizer{T}"/> value shouldn't be used directly: use this extension in a <see langword="foreach"/> loop.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpanTokenizer<T> Tokenize<T>(this ReadOnlySpan<T> span, T separator)
            where T : IEquatable<T>
        {
            return new(span, separator);
        }

        /// <summary>
        /// Gets a content hash from the input <see cref="ReadOnlySpan{T}"/> instance using the Djb2 algorithm.
        /// It was designed by <see href="https://en.wikipedia.org/wiki/Daniel_J._Bernstein">Daniel J. Bernstein</see> and is a
        /// <see href="https://en.wikipedia.org/wiki/List_of_hash_functions#Non-cryptographic_hash_functions">non-cryptographic has function</see>.
        /// The main advantages of this algorithm are a good distribution of the resulting hash codes, which results in a relatively low
        /// number of collisions, while at the same time being particularly fast to process, making it suitable for quickly hashing
        /// even long sequences of values. For the reference implementation, see: <see href="http://www.cse.yorku.ca/~oz/hash.html"/>.
        /// For details on the used constants, see the details provided in this StackOverflow answer (as well as the accepted one):
        /// <see href="https://stackoverflow.com/questions/10696223/reason-for-5381-number-in-djb-hash-function/13809282#13809282"/>.
        /// Additionally, a comparison between some common hashing algorithms can be found in the reply to this StackExchange question:
        /// <see href="https://softwareengineering.stackexchange.com/questions/49550/which-hashing-algorithm-is-best-for-uniqueness-and-speed"/>.
        /// Note that the exact implementation is slightly different in this method when it is not called on a sequence of <see cref="byte"/>
        /// values: in this case the <see cref="object.GetHashCode"/> method will be invoked for each <typeparamref name="T"/> value in
        /// the provided <see cref="ReadOnlySpan{T}"/> instance, and then those values will be combined using the Djb2 algorithm.
        /// </summary>
        /// <typeparam name="T">The type of items in the input <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
        /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance.</param>
        /// <returns>The Djb2 value for the input <see cref="ReadOnlySpan{T}"/> instance.</returns>
        /// <remarks>The Djb2 hash is fully deterministic and with no random components.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetDjb2HashCode<T>(this ReadOnlySpan<T> span)
            where T : notnull
        {
            ref T r0 = ref MemoryMarshal.GetReference(span);
            nint length = (nint)(uint)span.Length;

            return SpanHelper.GetDjb2HashCode(ref r0, length);
        }

        /// <summary>
        /// Copies the contents of a given <see cref="ReadOnlySpan{T}"/> into destination <see cref="RefEnumerable{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of items in the input <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
        /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance.</param>
        /// <param name="destination">The <see cref="RefEnumerable{T}"/> instance to copy items into.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the destination <see cref="RefEnumerable{T}"/> is shorter than the source <see cref="ReadOnlySpan{T}"/>.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<T>(this ReadOnlySpan<T> span, RefEnumerable<T> destination)
        {
            destination.CopyFrom(span);
        }

        /// <summary>
        /// Attempts to copy the contents of a given <see cref="ReadOnlySpan{T}"/> into destination <see cref="RefEnumerable{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of items in the input <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
        /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance.</param>
        /// <param name="destination">The <see cref="RefEnumerable{T}"/> instance to copy items into.</param>
        /// <returns>Whether or not the operation was successful.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCopyTo<T>(this ReadOnlySpan<T> span, RefEnumerable<T> destination)
        {
            return destination.TryCopyFrom(span);
        }
    }
}
