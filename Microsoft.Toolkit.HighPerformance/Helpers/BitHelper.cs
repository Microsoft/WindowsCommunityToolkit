// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
#if NETCOREAPP3_1 || NET5_0
using System.Runtime.Intrinsics.X86;
#endif

namespace Microsoft.Toolkit.HighPerformance.Helpers
{
    /// <summary>
    /// Helpers to perform bit operations on numeric types.
    /// </summary>
    public static class BitHelper
    {
        /// <summary>
        /// Checks whether or not a given bit is set.
        /// </summary>
        /// <param name="value">The input <see cref="uint"/> value.</param>
        /// <param name="n">The position of the bit to check (in [0, 31] range).</param>
        /// <returns>Whether or not the n-th bit is set.</returns>
        /// <remarks>
        /// This method doesn't validate <paramref name="n"/> against the valid range.
        /// If the parameter is not valid, the result will just be inconsistent.
        /// Additionally, no conditional branches are used to retrieve the flag.
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool HasFlag(uint value, int n)
        {
            // Read the n-th bit, downcast to byte
            byte flag = (byte)((value >> n) & 1);

            // Reinterpret the byte to avoid the test, setnz and
            // movzx instructions (asm x64). This is because the JIT
            // compiler is able to optimize this reinterpret-cast as
            // a single "and eax, 0x1" instruction, whereas if we had
            // compared the previous computed flag against 0, the assembly
            // would have had to perform the test, set the non-zero
            // flag and then extend the (byte) result to eax.
            return *(bool*)&flag;
        }

        /// <summary>
        /// Checks whether or not a given bit is set in a given bitwise lookup table.
        /// This method provides a branchless, register-based (with no memory accesses) way to
        /// check whether a given value is valid, according to a precomputed lookup table.
        /// It is similar in behavior to <see cref="HasFlag(uint,int)"/>, with the main difference
        /// being that this method will also validate the input <paramref name="x"/> parameter, and
        /// will always return <see langword="false"/> if it falls outside of the expected interval.
        /// Additionally, this method accepts a <paramref name="min"/> parameter, which is used to
        /// decrement the input parameter <paramref name="x"/> to ensure that the range of accepted
        /// values fits within the available 32 bits of the lookup table in use.
        /// For more info on this optimization technique, see <see href="https://egorbo.com/llvm-range-checks.html"/>.
        /// Here is how the code from the link above would be implemented using this method:
        /// <code>
        /// bool IsReservedCharacter(char c)
        /// {
        ///     return BitHelper.HasLookupFlag(314575237u, c, 36);
        /// }
        /// </code>
        /// The resulted assembly is virtually identical, with the added optimization that the one
        /// produced by <see cref="HasLookupFlag(uint,int,int)"/> has no conditional branches at all.
        /// </summary>
        /// <param name="table">The input lookup table to use.</param>
        /// <param name="x">The input value to check.</param>
        /// <param name="min">The minimum accepted value for <paramref name="x"/> (defaults to 0).</param>
        /// <returns>Whether or not the corresponding flag for <paramref name="x"/> is set in <paramref name="table"/>.</returns>
        /// <remarks>
        /// For best results, as shown in the sample code, both <paramref name="table"/> and <paramref name="min"/>
        /// should be compile-time constants, so that the JIT compiler will be able to produce more efficient code.
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool HasLookupFlag(uint table, int x, int min = 0)
        {
            // First, the input value is scaled down by the given minimum.
            // This step will be skipped entirely if min is just the default of 0.
            // The valid range is given by 32, which is the number of bits in the
            // lookup table. The input value is first cast to uint so that if it was
            // negative, the check will fail as well. Then, the result of this
            // operation is used to compute a bitwise flag of either 0xFFFFFFFF if the
            // input is accepted, or all 0 otherwise. The target bit is then extracted,
            // and this value is combined with the previous mask. This is done so that
            // if the shift was performed with a value that was too high, which has an
            // undefined behavior and could produce a non-0 value, the mask will reset
            // the final value anyway. This result is then unchecked-cast to a byte (as
            // it is guaranteed to always be either 1 or 0), and then reinterpreted
            // as a bool just like in the HasFlag method above, and then returned.
            int i = x - min;
            bool isInRange = (uint)i < 32u;
            byte byteFlag = *(byte*)&isInRange;
            int
                negativeFlag = byteFlag - 1,
                mask = ~negativeFlag,
                shift = unchecked((int)((table >> i) & 1)),
                and = shift & mask;
            byte result = unchecked((byte)and);
            bool valid = *(bool*)&result;

            return valid;
        }

        /// <summary>
        /// Checks whether the given value has any bytes that are set to 0.
        /// That is, given a <see cref="uint"/> value, which has a total of 4 bytes,
        /// it checks whether any of those have all the bits set to 0.
        /// </summary>
        /// <param name="value">The input value to check.</param>
        /// <returns>Whether <paramref name="value"/> has any bytes set to 0.</returns>
        /// <remarks>
        /// This method contains no branches.
        /// For more background on this subject, see <see href="https://graphics.stanford.edu/~seander/bithacks.html#ZeroInWord"/>.
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasZeroByte(uint value)
        {
            return ((value - 0x0101_0101u) & ~value & 0x8080_8080u) != 0;
        }

        /// <summary>
        /// Checks whether the given value has any bytes that are set to 0.
        /// This method mirrors <see cref="HasZeroByte(uint)"/>, but with <see cref="ulong"/> values.
        /// </summary>
        /// <param name="value">The input value to check.</param>
        /// <returns>Whether <paramref name="value"/> has any bytes set to 0.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasZeroByte(ulong value)
        {
            return ((value - 0x0101_0101_0101_0101ul) & ~value & 0x8080_8080_8080_8080ul) != 0;
        }

        /// <summary>
        /// Checks whether a byte in the input <see cref="uint"/> value matches a target value.
        /// </summary>
        /// <param name="value">The input value to check.</param>
        /// <param name="target">The target byte to look for.</param>
        /// <returns>Whether <paramref name="value"/> has any bytes set to <paramref name="target"/>.</returns>
        /// <remarks>
        /// This method contains no branches.
        /// For more info, see <see href="https://graphics.stanford.edu/~seander/bithacks.html#ZeroInWord"/>.
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasByteEqualTo(uint value, byte target)
        {
            return HasZeroByte(value ^ (0x0101_0101u * target));
        }

        /// <summary>
        /// Checks whether a byte in the input <see cref="uint"/> value matches a target value.
        /// This method mirrors <see cref="HasByteEqualTo(uint,byte)"/>, but with <see cref="ulong"/> values.
        /// </summary>
        /// <param name="value">The input value to check.</param>
        /// <param name="target">The target byte to look for.</param>
        /// <returns>Whether <paramref name="value"/> has any bytes set to <paramref name="target"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasByteEqualTo(ulong value, byte target)
        {
            return HasZeroByte(value ^ (0x0101_0101_0101_0101u * target));
        }

        /// <summary>
        /// Sets a bit to a specified value.
        /// </summary>
        /// <param name="value">The target <see cref="uint"/> value.</param>
        /// <param name="n">The position of the bit to set or clear (in [0, 31] range).</param>
        /// <param name="flag">The value to assign to the target bit.</param>
        /// <remarks>
        /// Just like <see cref="HasFlag(uint,int)"/>, this method doesn't validate <paramref name="n"/>
        /// and does not contain branching instructions, so it's well suited for use in tight loops as well.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetFlag(ref uint value, int n, bool flag)
        {
            value = SetFlag(value, n, flag);
        }

        /// <summary>
        /// Sets a bit to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="uint"/> value.</param>
        /// <param name="n">The position of the bit to set or clear (in [0, 31] range).</param>
        /// <param name="flag">The value to assign to the target bit.</param>
        /// <returns>An <see cref="uint"/> value equal to <paramref name="value"/> except for the <paramref name="n"/>-th bit.</returns>
        /// <remarks>
        /// Just like <see cref="HasFlag(uint,int)"/>, this method doesn't validate <paramref name="n"/>
        /// and does not contain branching instructions, so it's well suited for use in tight loops as well.
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe uint SetFlag(uint value, int n, bool flag)
        {
            // Shift a bit left to the n-th position, negate the
            // resulting value and perform an AND with the input value.
            // This effectively clears the n-th bit of our input.
            uint
                bit = 1u << n,
                not = ~bit,
                and = value & not;

            // Reinterpret the flag as 1 or 0, and cast to uint,
            // then we left shift the uint flag to the right position
            // and perform an OR with the resulting value of the previous
            // operation. This will always guaranteed to work, thanks to the
            // initial code clearing that bit before setting it again.
            bool copy = flag;
            uint
                flag32 = *(byte*)&copy,
                shift = flag32 << n,
                or = and | shift;

            return or;
        }

        /// <summary>
        /// Extracts a bit field range from a given value.
        /// </summary>
        /// <param name="value">The input <see cref="uint"/> value.</param>
        /// <param name="start">The initial index of the range to extract (in [0, 31] range).</param>
        /// <param name="length">The length of the range to extract (depends on <paramref name="start"/>).</param>
        /// <returns>The value of the extracted range within <paramref name="value"/>.</returns>
        /// <remarks>
        /// This method doesn't validate <paramref name="start"/> and <paramref name="length"/>.
        /// If either parameter is not valid, the result will just be inconsistent. The method
        /// should not be used to set all the bits at once, and it is not guaranteed to work in
        /// that case, which would just be equivalent to assigning the <see cref="uint"/> value.
        /// Additionally, no conditional branches are used to retrieve the range.
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ExtractRange(uint value, byte start, byte length)
        {
#if NETCOREAPP3_1 || NET5_0
            if (Bmi1.IsSupported)
            {
                return Bmi1.BitFieldExtract(value, start, length);
            }
#endif

            return (value >> start) & ((1u << length) - 1u);
        }

        /// <summary>
        /// Sets a bit field range within a target value.
        /// </summary>
        /// <param name="value">The target <see cref="uint"/> value.</param>
        /// <param name="start">The initial index of the range to extract (in [0, 31] range).</param>
        /// <param name="length">The length of the range to extract (depends on <paramref name="start"/>).</param>
        /// <param name="flags">The input flags to insert in the target range.</param>
        /// <remarks>
        /// Just like <see cref="ExtractRange(uint,byte,byte)"/>, this method doesn't validate the parameters
        /// and does not contain branching instructions, so it's well suited for use in tight loops as well.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetRange(ref uint value, byte start, byte length, uint flags)
        {
            value = SetRange(value, start, length, flags);
        }

        /// <summary>
        /// Sets a bit field range within a target value.
        /// </summary>
        /// <param name="value">The initial <see cref="uint"/> value.</param>
        /// <param name="start">The initial index of the range to extract (in [0, 31] range).</param>
        /// <param name="length">The length of the range to extract (depends on <paramref name="start"/>).</param>
        /// <param name="flags">The input flags to insert in the target range.</param>
        /// <returns>The updated bit field value after setting the specified range.</returns>
        /// <remarks>
        /// Just like <see cref="ExtractRange(uint,byte,byte)"/>, this method doesn't validate the parameters
        /// and does not contain branching instructions, so it's well suited for use in tight loops as well.
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SetRange(uint value, byte start, byte length, uint flags)
        {
            uint
                highBits = (1u << length) - 1u,
                loadMask = highBits << start,
                storeMask = (flags & highBits) << start;

#if NETCOREAPP3_1 || NET5_0
            if (Bmi1.IsSupported)
            {
                return Bmi1.AndNot(loadMask, value) | storeMask;
            }
#endif

            return (~loadMask & value) | storeMask;
        }

        /// <summary>
        /// Checks whether or not a given bit is set.
        /// </summary>
        /// <param name="value">The input <see cref="ulong"/> value.</param>
        /// <param name="n">The position of the bit to check (in [0, 63] range).</param>
        /// <returns>Whether or not the n-th bit is set.</returns>
        /// <remarks>
        /// This method doesn't validate <paramref name="n"/> against the valid range.
        /// If the parameter is not valid, the result will just be inconsistent.
        /// Additionally, no conditional branches are used to retrieve the flag.
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool HasFlag(ulong value, int n)
        {
            // Same logic as the uint version, see that for more info
            byte flag = (byte)((value >> n) & 1);

            return *(bool*)&flag;
        }

        /// <summary>
        /// Checks whether or not a given bit is set in a given bitwise lookup table.
        /// For more info, check the XML docs of the <see cref="HasLookupFlag(uint,int,int)"/> overload.
        /// </summary>
        /// <param name="table">The input lookup table to use.</param>
        /// <param name="x">The input value to check.</param>
        /// <param name="min">The minimum accepted value for <paramref name="x"/> (defaults to 0).</param>
        /// <returns>Whether or not the corresponding flag for <paramref name="x"/> is set in <paramref name="table"/>.</returns>
        /// <remarks>
        /// For best results, as shown in the sample code, both <paramref name="table"/> and <paramref name="min"/>
        /// should be compile-time constants, so that the JIT compiler will be able to produce more efficient code.
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool HasLookupFlag(ulong table, int x, int min = 0)
        {
            int i = x - min;
            bool isInRange = (uint)i < 64u;
            byte byteFlag = *(byte*)&isInRange;
            int
                negativeFlag = byteFlag - 1,
                mask = ~negativeFlag,
                shift = unchecked((int)((table >> i) & 1)),
                and = shift & mask;
            byte result = unchecked((byte)and);
            bool valid = *(bool*)&result;

            return valid;
        }

        /// <summary>
        /// Sets a bit to a specified value.
        /// </summary>
        /// <param name="value">The target <see cref="ulong"/> value.</param>
        /// <param name="n">The position of the bit to set or clear (in [0, 63] range).</param>
        /// <param name="flag">The value to assign to the target bit.</param>
        /// <remarks>
        /// Just like <see cref="HasFlag(ulong,int)"/>, this method doesn't validate <paramref name="n"/>
        /// and does not contain branching instructions, so it's well suited for use in tight loops as well.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetFlag(ref ulong value, int n, bool flag)
        {
            value = SetFlag(value, n, flag);
        }

        /// <summary>
        /// Sets a bit to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="ulong"/> value.</param>
        /// <param name="n">The position of the bit to set or clear (in [0, 63] range).</param>
        /// <param name="flag">The value to assign to the target bit.</param>
        /// <returns>An <see cref="ulong"/> value equal to <paramref name="value"/> except for the <paramref name="n"/>-th bit.</returns>
        /// <remarks>
        /// Just like <see cref="HasFlag(ulong,int)"/>, this method doesn't validate <paramref name="n"/>
        /// and does not contain branching instructions, so it's well suited for use in tight loops as well.
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ulong SetFlag(ulong value, int n, bool flag)
        {
            ulong
                bit = 1ul << n,
                not = ~bit,
                and = value & not;
            bool copy = flag;
            ulong flag64 = *(byte*)&copy,
                shift = flag64 << n,
                or = and | shift;

            return or;
        }

        /// <summary>
        /// Extracts a bit field range from a given value.
        /// </summary>
        /// <param name="value">The input <see cref="ulong"/> value.</param>
        /// <param name="start">The initial index of the range to extract (in [0, 63] range).</param>
        /// <param name="length">The length of the range to extract (depends on <paramref name="start"/>).</param>
        /// <returns>The value of the extracted range within <paramref name="value"/>.</returns>
        /// <remarks>
        /// This method doesn't validate <paramref name="start"/> and <paramref name="length"/>.
        /// If either parameter is not valid, the result will just be inconsistent. The method
        /// should not be used to set all the bits at once, and it is not guaranteed to work in
        /// that case, which would just be equivalent to assigning the <see cref="ulong"/> value.
        /// Additionally, no conditional branches are used to retrieve the range.
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ExtractRange(ulong value, byte start, byte length)
        {
#if NETCOREAPP3_1 || NET5_0
            if (Bmi1.X64.IsSupported)
            {
                return Bmi1.X64.BitFieldExtract(value, start, length);
            }
#endif

            return (value >> start) & ((1ul << length) - 1ul);
        }

        /// <summary>
        /// Sets a bit field range within a target value.
        /// </summary>
        /// <param name="value">The target <see cref="ulong"/> value.</param>
        /// <param name="start">The initial index of the range to extract (in [0, 63] range).</param>
        /// <param name="length">The length of the range to extract (depends on <paramref name="start"/>).</param>
        /// <param name="flags">The input flags to insert in the target range.</param>
        /// <remarks>
        /// Just like <see cref="ExtractRange(ulong,byte,byte)"/>, this method doesn't validate the parameters
        /// and does not contain branching instructions, so it's well suited for use in tight loops as well.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetRange(ref ulong value, byte start, byte length, ulong flags)
        {
            value = SetRange(value, start, length, flags);
        }

        /// <summary>
        /// Sets a bit field range within a target value.
        /// </summary>
        /// <param name="value">The initial <see cref="ulong"/> value.</param>
        /// <param name="start">The initial index of the range to extract (in [0, 63] range).</param>
        /// <param name="length">The length of the range to extract (depends on <paramref name="start"/>).</param>
        /// <param name="flags">The input flags to insert in the target range.</param>
        /// <returns>The updated bit field value after setting the specified range.</returns>
        /// <remarks>
        /// Just like <see cref="ExtractRange(ulong,byte,byte)"/>, this method doesn't validate the parameters
        /// and does not contain branching instructions, so it's well suited for use in tight loops as well.
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SetRange(ulong value, byte start, byte length, ulong flags)
        {
            ulong
                highBits = (1ul << length) - 1ul,
                loadMask = highBits << start,
                storeMask = (flags & highBits) << start;

#if NETCOREAPP3_1 || NET5_0
            if (Bmi1.X64.IsSupported)
            {
                return Bmi1.X64.AndNot(loadMask, value) | storeMask;
            }
#endif

            return (~loadMask & value) | storeMask;
        }
    }
}