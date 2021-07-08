// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.Diagnostics
{
    /// <summary>
    /// Helper methods to verify conditions when running code.
    /// </summary>
    public static partial class Guard
    {
        /// <summary>
        /// Asserts that the input value is <see langword="default"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see langword="struct"/> value type being tested.</typeparam>
        /// <param name="value">The input value to test.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not <see langword="default"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsDefault<T>(T value, string name)
            where T : struct, IEquatable<T>
        {
            if (value.Equals(default))
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsDefault(value, name);
        }

        /// <summary>
        /// Asserts that the input value is not <see langword="default"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see langword="struct"/> value type being tested.</typeparam>
        /// <param name="value">The input value to test.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is <see langword="default"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotDefault<T>(T value, string name)
            where T : struct, IEquatable<T>
        {
            if (!value.Equals(default))
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotDefault<T>(name);
        }

        /// <summary>
        /// Asserts that the input value must be equal to a specified value.
        /// </summary>
        /// <typeparam name="T">The type of input values to compare.</typeparam>
        /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
        /// <param name="target">The target <typeparamref name="T"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is != <paramref name="target"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsEqualTo<T>(T value, T target, string name)
            where T : notnull, IEquatable<T>
        {
            if (value.Equals(target))
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be not equal to a specified value.
        /// </summary>
        /// <typeparam name="T">The type of input values to compare.</typeparam>
        /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
        /// <param name="target">The target <typeparamref name="T"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is == <paramref name="target"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotEqualTo<T>(T value, T target, string name)
            where T : notnull, IEquatable<T>
        {
            if (!value.Equals(target))
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be a bitwise match with a specified value.
        /// </summary>
        /// <typeparam name="T">The type of input values to compare.</typeparam>
        /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
        /// <param name="target">The target <typeparamref name="T"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not a bitwise match for <paramref name="target"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void IsBitwiseEqualTo<T>(T value, T target, string name)
            where T : unmanaged
        {
            // Include some fast paths if the input type is of size 1, 2, 4, 8, or 16.
            // In those cases, just reinterpret the bytes as values of an integer type,
            // and compare them directly, which is much faster than having a loop over each byte.
            // The conditional branches below are known at compile time by the JIT compiler,
            // so that only the right one will actually be translated into native code.
            if (sizeof(T) == 1)
            {
                byte valueByte = Unsafe.As<T, byte>(ref value);
                byte targetByte = Unsafe.As<T, byte>(ref target);

                if (valueByte == targetByte)
                {
                    return;
                }

                ThrowHelper.ThrowArgumentExceptionForBitwiseEqualTo(value, target, name);
            }
            else if (sizeof(T) == 2)
            {
                ushort valueUShort = Unsafe.As<T, ushort>(ref value);
                ushort targetUShort = Unsafe.As<T, ushort>(ref target);

                if (valueUShort == targetUShort)
                {
                    return;
                }

                ThrowHelper.ThrowArgumentExceptionForBitwiseEqualTo(value, target, name);
            }
            else if (sizeof(T) == 4)
            {
                uint valueUInt = Unsafe.As<T, uint>(ref value);
                uint targetUInt = Unsafe.As<T, uint>(ref target);

                if (valueUInt == targetUInt)
                {
                    return;
                }

                ThrowHelper.ThrowArgumentExceptionForBitwiseEqualTo(value, target, name);
            }
            else if (sizeof(T) == 8)
            {
                ulong valueULong = Unsafe.As<T, ulong>(ref value);
                ulong targetULong = Unsafe.As<T, ulong>(ref target);

                if (Bit64Compare(ref valueULong, ref targetULong))
                {
                    return;
                }

                ThrowHelper.ThrowArgumentExceptionForBitwiseEqualTo(value, target, name);
            }
            else if (sizeof(T) == 16)
            {
                ulong valueULong0 = Unsafe.As<T, ulong>(ref value);
                ulong targetULong0 = Unsafe.As<T, ulong>(ref target);

                if (Bit64Compare(ref valueULong0, ref targetULong0))
                {
                    ulong valueULong1 = Unsafe.Add(ref Unsafe.As<T, ulong>(ref value), 1);
                    ulong targetULong1 = Unsafe.Add(ref Unsafe.As<T, ulong>(ref target), 1);

                    if (Bit64Compare(ref valueULong1, ref targetULong1))
                    {
                        return;
                    }
                }

                ThrowHelper.ThrowArgumentExceptionForBitwiseEqualTo(value, target, name);
            }
            else
            {
                Span<byte> valueBytes = new Span<byte>(Unsafe.AsPointer(ref value), sizeof(T));
                Span<byte> targetBytes = new Span<byte>(Unsafe.AsPointer(ref target), sizeof(T));

                if (valueBytes.SequenceEqual(targetBytes))
                {
                    return;
                }

                ThrowHelper.ThrowArgumentExceptionForBitwiseEqualTo(value, target, name);
            }
        }

        // Compares 64 bits of data from two given memory locations for bitwise equality
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe bool Bit64Compare(ref ulong left, ref ulong right)
        {
            // Handles 32 bit case, because using ulong is inefficient
            if (sizeof(IntPtr) == 4)
            {
                ref int r0 = ref Unsafe.As<ulong, int>(ref left);
                ref int r1 = ref Unsafe.As<ulong, int>(ref right);

                return r0 == r1 &&
                       Unsafe.Add(ref r0, 1) == Unsafe.Add(ref r1, 1);
            }

            return left == right;
        }

        /// <summary>
        /// Asserts that the input value must be less than a specified value.
        /// </summary>
        /// <typeparam name="T">The type of input values to compare.</typeparam>
        /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
        /// <param name="maximum">The exclusive maximum <typeparamref name="T"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThan<T>(T value, T maximum, string name)
            where T : notnull, IComparable<T>
        {
            if (value.CompareTo(maximum) < 0)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThan(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than or equal to a specified value.
        /// </summary>
        /// <typeparam name="T">The type of input values to compare.</typeparam>
        /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
        /// <param name="maximum">The inclusive maximum <typeparamref name="T"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThanOrEqualTo<T>(T value, T maximum, string name)
            where T : notnull, IComparable<T>
        {
            if (value.CompareTo(maximum) <= 0)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThanOrEqualTo(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than a specified value.
        /// </summary>
        /// <typeparam name="T">The type of input values to compare.</typeparam>
        /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <typeparamref name="T"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThan<T>(T value, T minimum, string name)
            where T : notnull, IComparable<T>
        {
            if (value.CompareTo(minimum) > 0)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThan(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than or equal to a specified value.
        /// </summary>
        /// <typeparam name="T">The type of input values to compare.</typeparam>
        /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <typeparamref name="T"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThanOrEqualTo<T>(T value, T minimum, string name)
            where T : notnull, IComparable<T>
        {
            if (value.CompareTo(minimum) >= 0)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThanOrEqualTo(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given range.
        /// </summary>
        /// <typeparam name="T">The type of input values to compare.</typeparam>
        /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <typeparamref name="T"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <typeparamref name="T"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsInRange<T>(T value, T minimum, T maximum, string name)
            where T : notnull, IComparable<T>
        {
            if (value.CompareTo(minimum) >= 0 && value.CompareTo(maximum) < 0)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given range.
        /// </summary>
        /// <typeparam name="T">The type of input values to compare.</typeparam>
        /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <typeparamref name="T"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <typeparamref name="T"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotInRange<T>(T value, T minimum, T maximum, string name)
            where T : notnull, IComparable<T>
        {
            if (value.CompareTo(minimum) < 0 || value.CompareTo(maximum) >= 0)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <typeparam name="T">The type of input values to compare.</typeparam>
        /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <typeparamref name="T"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <typeparamref name="T"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetween<T>(T value, T minimum, T maximum, string name)
            where T : notnull, IComparable<T>
        {
            if (value.CompareTo(minimum) > 0 && value.CompareTo(maximum) < 0)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <typeparam name="T">The type of input values to compare.</typeparam>
        /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <typeparamref name="T"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <typeparamref name="T"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetween<T>(T value, T minimum, T maximum, string name)
            where T : notnull, IComparable<T>
        {
            if (value.CompareTo(minimum) <= 0 || value.CompareTo(maximum) >= 0)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <typeparam name="T">The type of input values to compare.</typeparam>
        /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <typeparamref name="T"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <typeparamref name="T"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or > <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetweenOrEqualTo<T>(T value, T minimum, T maximum, string name)
            where T : notnull, IComparable<T>
        {
            if (value.CompareTo(minimum) >= 0 && value.CompareTo(maximum) <= 0)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <typeparam name="T">The type of input values to compare.</typeparam>
        /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <typeparamref name="T"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <typeparamref name="T"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt;= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetweenOrEqualTo<T>(T value, T minimum, T maximum, string name)
            where T : notnull, IComparable<T>
        {
            if (value.CompareTo(minimum) < 0 || value.CompareTo(maximum) > 0)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetweenOrEqualTo(value, minimum, maximum, name);
        }
    }
}