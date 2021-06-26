// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// =====================
// Auto generated file
// =====================

using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Toolkit.Diagnostics
{
    /// <summary>
    /// Helper methods to verify conditions when running code.
    /// </summary>
    public static partial class Guard
    {
        /// <summary>
        /// Asserts that the input value must be equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="byte"/> value to test.</param>
        /// <param name="target">The target <see cref="byte"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is != <paramref name="target"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsEqualTo(byte value, byte target, string name)
        {
            if (value == target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be not equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="byte"/> value to test.</param>
        /// <param name="target">The target <see cref="byte"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is == <paramref name="target"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotEqualTo(byte value, byte target, string name)
        {
            if (value != target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="byte"/> value to test.</param>
        /// <param name="maximum">The exclusive maximum <see cref="byte"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThan(byte value, byte maximum, string name)
        {
            if (value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThan(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="byte"/> value to test.</param>
        /// <param name="maximum">The inclusive maximum <see cref="byte"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThanOrEqualTo(byte value, byte maximum, string name)
        {
            if (value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThanOrEqualTo(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="byte"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="byte"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThan(byte value, byte minimum, string name)
        {
            if (value > minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThan(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="byte"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="byte"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThanOrEqualTo(byte value, byte minimum, string name)
        {
            if (value >= minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThanOrEqualTo(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given range.
        /// </summary>
        /// <param name="value">The input <see cref="byte"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="byte"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="byte"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsInRange(byte value, byte minimum, byte maximum, string name)
        {
            if (value >= minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given range.
        /// </summary>
        /// <param name="value">The input <see cref="byte"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="byte"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="byte"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotInRange(byte value, byte minimum, byte maximum, string name)
        {
            if (value < minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="byte"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="byte"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="byte"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetween(byte value, byte minimum, byte maximum, string name)
        {
            if (value > minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="byte"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="byte"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="byte"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetween(byte value, byte minimum, byte maximum, string name)
        {
            if (value <= minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="byte"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="byte"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see cref="byte"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or > <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetweenOrEqualTo(byte value, byte minimum, byte maximum, string name)
        {
            if (value >= minimum && value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="byte"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="byte"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see cref="byte"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt;= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetweenOrEqualTo(byte value, byte minimum, byte maximum, string name)
        {
            if (value < minimum || value > maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="sbyte"/> value to test.</param>
        /// <param name="target">The target <see cref="sbyte"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is != <paramref name="target"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsEqualTo(sbyte value, sbyte target, string name)
        {
            if (value == target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be not equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="sbyte"/> value to test.</param>
        /// <param name="target">The target <see cref="sbyte"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is == <paramref name="target"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotEqualTo(sbyte value, sbyte target, string name)
        {
            if (value != target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="sbyte"/> value to test.</param>
        /// <param name="maximum">The exclusive maximum <see cref="sbyte"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThan(sbyte value, sbyte maximum, string name)
        {
            if (value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThan(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="sbyte"/> value to test.</param>
        /// <param name="maximum">The inclusive maximum <see cref="sbyte"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThanOrEqualTo(sbyte value, sbyte maximum, string name)
        {
            if (value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThanOrEqualTo(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="sbyte"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="sbyte"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThan(sbyte value, sbyte minimum, string name)
        {
            if (value > minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThan(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="sbyte"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="sbyte"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThanOrEqualTo(sbyte value, sbyte minimum, string name)
        {
            if (value >= minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThanOrEqualTo(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given range.
        /// </summary>
        /// <param name="value">The input <see cref="sbyte"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="sbyte"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="sbyte"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsInRange(sbyte value, sbyte minimum, sbyte maximum, string name)
        {
            if (value >= minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given range.
        /// </summary>
        /// <param name="value">The input <see cref="sbyte"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="sbyte"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="sbyte"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotInRange(sbyte value, sbyte minimum, sbyte maximum, string name)
        {
            if (value < minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="sbyte"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="sbyte"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="sbyte"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetween(sbyte value, sbyte minimum, sbyte maximum, string name)
        {
            if (value > minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="sbyte"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="sbyte"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="sbyte"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetween(sbyte value, sbyte minimum, sbyte maximum, string name)
        {
            if (value <= minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="sbyte"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="sbyte"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see cref="sbyte"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or > <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetweenOrEqualTo(sbyte value, sbyte minimum, sbyte maximum, string name)
        {
            if (value >= minimum && value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="sbyte"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="sbyte"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see cref="sbyte"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt;= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetweenOrEqualTo(sbyte value, sbyte minimum, sbyte maximum, string name)
        {
            if (value < minimum || value > maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="short"/> value to test.</param>
        /// <param name="target">The target <see cref="short"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is != <paramref name="target"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsEqualTo(short value, short target, string name)
        {
            if (value == target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be not equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="short"/> value to test.</param>
        /// <param name="target">The target <see cref="short"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is == <paramref name="target"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotEqualTo(short value, short target, string name)
        {
            if (value != target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="short"/> value to test.</param>
        /// <param name="maximum">The exclusive maximum <see cref="short"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThan(short value, short maximum, string name)
        {
            if (value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThan(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="short"/> value to test.</param>
        /// <param name="maximum">The inclusive maximum <see cref="short"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThanOrEqualTo(short value, short maximum, string name)
        {
            if (value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThanOrEqualTo(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="short"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="short"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThan(short value, short minimum, string name)
        {
            if (value > minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThan(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="short"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="short"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThanOrEqualTo(short value, short minimum, string name)
        {
            if (value >= minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThanOrEqualTo(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given range.
        /// </summary>
        /// <param name="value">The input <see cref="short"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="short"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="short"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsInRange(short value, short minimum, short maximum, string name)
        {
            if (value >= minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given range.
        /// </summary>
        /// <param name="value">The input <see cref="short"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="short"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="short"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotInRange(short value, short minimum, short maximum, string name)
        {
            if (value < minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="short"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="short"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="short"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetween(short value, short minimum, short maximum, string name)
        {
            if (value > minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="short"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="short"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="short"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetween(short value, short minimum, short maximum, string name)
        {
            if (value <= minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="short"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="short"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see cref="short"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or > <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetweenOrEqualTo(short value, short minimum, short maximum, string name)
        {
            if (value >= minimum && value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="short"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="short"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see cref="short"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt;= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetweenOrEqualTo(short value, short minimum, short maximum, string name)
        {
            if (value < minimum || value > maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="ushort"/> value to test.</param>
        /// <param name="target">The target <see cref="ushort"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is != <paramref name="target"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsEqualTo(ushort value, ushort target, string name)
        {
            if (value == target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be not equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="ushort"/> value to test.</param>
        /// <param name="target">The target <see cref="ushort"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is == <paramref name="target"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotEqualTo(ushort value, ushort target, string name)
        {
            if (value != target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="ushort"/> value to test.</param>
        /// <param name="maximum">The exclusive maximum <see cref="ushort"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThan(ushort value, ushort maximum, string name)
        {
            if (value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThan(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="ushort"/> value to test.</param>
        /// <param name="maximum">The inclusive maximum <see cref="ushort"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThanOrEqualTo(ushort value, ushort maximum, string name)
        {
            if (value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThanOrEqualTo(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="ushort"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="ushort"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThan(ushort value, ushort minimum, string name)
        {
            if (value > minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThan(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="ushort"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="ushort"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThanOrEqualTo(ushort value, ushort minimum, string name)
        {
            if (value >= minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThanOrEqualTo(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given range.
        /// </summary>
        /// <param name="value">The input <see cref="ushort"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="ushort"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="ushort"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsInRange(ushort value, ushort minimum, ushort maximum, string name)
        {
            if (value >= minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given range.
        /// </summary>
        /// <param name="value">The input <see cref="ushort"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="ushort"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="ushort"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotInRange(ushort value, ushort minimum, ushort maximum, string name)
        {
            if (value < minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="ushort"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="ushort"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="ushort"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetween(ushort value, ushort minimum, ushort maximum, string name)
        {
            if (value > minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="ushort"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="ushort"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="ushort"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetween(ushort value, ushort minimum, ushort maximum, string name)
        {
            if (value <= minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="ushort"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="ushort"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see cref="ushort"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or > <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetweenOrEqualTo(ushort value, ushort minimum, ushort maximum, string name)
        {
            if (value >= minimum && value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="ushort"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="ushort"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see cref="ushort"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt;= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetweenOrEqualTo(ushort value, ushort minimum, ushort maximum, string name)
        {
            if (value < minimum || value > maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="char"/> value to test.</param>
        /// <param name="target">The target <see cref="char"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is != <paramref name="target"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsEqualTo(char value, char target, string name)
        {
            if (value == target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be not equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="char"/> value to test.</param>
        /// <param name="target">The target <see cref="char"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is == <paramref name="target"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotEqualTo(char value, char target, string name)
        {
            if (value != target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="char"/> value to test.</param>
        /// <param name="maximum">The exclusive maximum <see cref="char"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThan(char value, char maximum, string name)
        {
            if (value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThan(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="char"/> value to test.</param>
        /// <param name="maximum">The inclusive maximum <see cref="char"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThanOrEqualTo(char value, char maximum, string name)
        {
            if (value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThanOrEqualTo(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="char"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="char"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThan(char value, char minimum, string name)
        {
            if (value > minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThan(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="char"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="char"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThanOrEqualTo(char value, char minimum, string name)
        {
            if (value >= minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThanOrEqualTo(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given range.
        /// </summary>
        /// <param name="value">The input <see cref="char"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="char"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="char"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsInRange(char value, char minimum, char maximum, string name)
        {
            if (value >= minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given range.
        /// </summary>
        /// <param name="value">The input <see cref="char"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="char"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="char"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotInRange(char value, char minimum, char maximum, string name)
        {
            if (value < minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="char"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="char"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="char"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetween(char value, char minimum, char maximum, string name)
        {
            if (value > minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="char"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="char"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="char"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetween(char value, char minimum, char maximum, string name)
        {
            if (value <= minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="char"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="char"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see cref="char"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or > <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetweenOrEqualTo(char value, char minimum, char maximum, string name)
        {
            if (value >= minimum && value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="char"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="char"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see cref="char"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt;= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetweenOrEqualTo(char value, char minimum, char maximum, string name)
        {
            if (value < minimum || value > maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="int"/> value to test.</param>
        /// <param name="target">The target <see cref="int"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is != <paramref name="target"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsEqualTo(int value, int target, string name)
        {
            if (value == target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be not equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="int"/> value to test.</param>
        /// <param name="target">The target <see cref="int"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is == <paramref name="target"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotEqualTo(int value, int target, string name)
        {
            if (value != target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="int"/> value to test.</param>
        /// <param name="maximum">The exclusive maximum <see cref="int"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThan(int value, int maximum, string name)
        {
            if (value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThan(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="int"/> value to test.</param>
        /// <param name="maximum">The inclusive maximum <see cref="int"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThanOrEqualTo(int value, int maximum, string name)
        {
            if (value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThanOrEqualTo(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="int"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="int"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThan(int value, int minimum, string name)
        {
            if (value > minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThan(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="int"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="int"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThanOrEqualTo(int value, int minimum, string name)
        {
            if (value >= minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThanOrEqualTo(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given range.
        /// </summary>
        /// <param name="value">The input <see cref="int"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="int"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="int"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsInRange(int value, int minimum, int maximum, string name)
        {
            if (value >= minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given range.
        /// </summary>
        /// <param name="value">The input <see cref="int"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="int"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="int"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotInRange(int value, int minimum, int maximum, string name)
        {
            if (value < minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="int"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="int"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="int"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetween(int value, int minimum, int maximum, string name)
        {
            if (value > minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="int"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="int"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="int"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetween(int value, int minimum, int maximum, string name)
        {
            if (value <= minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="int"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="int"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see cref="int"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or > <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetweenOrEqualTo(int value, int minimum, int maximum, string name)
        {
            if (value >= minimum && value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="int"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="int"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see cref="int"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt;= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetweenOrEqualTo(int value, int minimum, int maximum, string name)
        {
            if (value < minimum || value > maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="uint"/> value to test.</param>
        /// <param name="target">The target <see cref="uint"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is != <paramref name="target"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsEqualTo(uint value, uint target, string name)
        {
            if (value == target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be not equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="uint"/> value to test.</param>
        /// <param name="target">The target <see cref="uint"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is == <paramref name="target"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotEqualTo(uint value, uint target, string name)
        {
            if (value != target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="uint"/> value to test.</param>
        /// <param name="maximum">The exclusive maximum <see cref="uint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThan(uint value, uint maximum, string name)
        {
            if (value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThan(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="uint"/> value to test.</param>
        /// <param name="maximum">The inclusive maximum <see cref="uint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThanOrEqualTo(uint value, uint maximum, string name)
        {
            if (value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThanOrEqualTo(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="uint"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="uint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThan(uint value, uint minimum, string name)
        {
            if (value > minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThan(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="uint"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="uint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThanOrEqualTo(uint value, uint minimum, string name)
        {
            if (value >= minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThanOrEqualTo(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given range.
        /// </summary>
        /// <param name="value">The input <see cref="uint"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="uint"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="uint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsInRange(uint value, uint minimum, uint maximum, string name)
        {
            if (value >= minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given range.
        /// </summary>
        /// <param name="value">The input <see cref="uint"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="uint"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="uint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotInRange(uint value, uint minimum, uint maximum, string name)
        {
            if (value < minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="uint"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="uint"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="uint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetween(uint value, uint minimum, uint maximum, string name)
        {
            if (value > minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="uint"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="uint"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="uint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetween(uint value, uint minimum, uint maximum, string name)
        {
            if (value <= minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="uint"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="uint"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see cref="uint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or > <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetweenOrEqualTo(uint value, uint minimum, uint maximum, string name)
        {
            if (value >= minimum && value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="uint"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="uint"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see cref="uint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt;= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetweenOrEqualTo(uint value, uint minimum, uint maximum, string name)
        {
            if (value < minimum || value > maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="float"/> value to test.</param>
        /// <param name="target">The target <see cref="float"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is != <paramref name="target"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsEqualTo(float value, float target, string name)
        {
            if (value == target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be not equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="float"/> value to test.</param>
        /// <param name="target">The target <see cref="float"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is == <paramref name="target"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotEqualTo(float value, float target, string name)
        {
            if (value != target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="float"/> value to test.</param>
        /// <param name="maximum">The exclusive maximum <see cref="float"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThan(float value, float maximum, string name)
        {
            if (value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThan(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="float"/> value to test.</param>
        /// <param name="maximum">The inclusive maximum <see cref="float"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThanOrEqualTo(float value, float maximum, string name)
        {
            if (value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThanOrEqualTo(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="float"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="float"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThan(float value, float minimum, string name)
        {
            if (value > minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThan(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="float"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="float"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThanOrEqualTo(float value, float minimum, string name)
        {
            if (value >= minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThanOrEqualTo(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given range.
        /// </summary>
        /// <param name="value">The input <see cref="float"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="float"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="float"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsInRange(float value, float minimum, float maximum, string name)
        {
            if (value >= minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given range.
        /// </summary>
        /// <param name="value">The input <see cref="float"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="float"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="float"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotInRange(float value, float minimum, float maximum, string name)
        {
            if (value < minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="float"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="float"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="float"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetween(float value, float minimum, float maximum, string name)
        {
            if (value > minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="float"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="float"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="float"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetween(float value, float minimum, float maximum, string name)
        {
            if (value <= minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="float"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="float"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see cref="float"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or > <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetweenOrEqualTo(float value, float minimum, float maximum, string name)
        {
            if (value >= minimum && value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="float"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="float"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see cref="float"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt;= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetweenOrEqualTo(float value, float minimum, float maximum, string name)
        {
            if (value < minimum || value > maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="long"/> value to test.</param>
        /// <param name="target">The target <see cref="long"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is != <paramref name="target"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsEqualTo(long value, long target, string name)
        {
            if (value == target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be not equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="long"/> value to test.</param>
        /// <param name="target">The target <see cref="long"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is == <paramref name="target"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotEqualTo(long value, long target, string name)
        {
            if (value != target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="long"/> value to test.</param>
        /// <param name="maximum">The exclusive maximum <see cref="long"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThan(long value, long maximum, string name)
        {
            if (value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThan(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="long"/> value to test.</param>
        /// <param name="maximum">The inclusive maximum <see cref="long"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThanOrEqualTo(long value, long maximum, string name)
        {
            if (value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThanOrEqualTo(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="long"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="long"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThan(long value, long minimum, string name)
        {
            if (value > minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThan(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="long"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="long"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThanOrEqualTo(long value, long minimum, string name)
        {
            if (value >= minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThanOrEqualTo(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given range.
        /// </summary>
        /// <param name="value">The input <see cref="long"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="long"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="long"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsInRange(long value, long minimum, long maximum, string name)
        {
            if (value >= minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given range.
        /// </summary>
        /// <param name="value">The input <see cref="long"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="long"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="long"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotInRange(long value, long minimum, long maximum, string name)
        {
            if (value < minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="long"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="long"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="long"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetween(long value, long minimum, long maximum, string name)
        {
            if (value > minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="long"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="long"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="long"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetween(long value, long minimum, long maximum, string name)
        {
            if (value <= minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="long"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="long"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see cref="long"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or > <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetweenOrEqualTo(long value, long minimum, long maximum, string name)
        {
            if (value >= minimum && value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="long"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="long"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see cref="long"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt;= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetweenOrEqualTo(long value, long minimum, long maximum, string name)
        {
            if (value < minimum || value > maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="ulong"/> value to test.</param>
        /// <param name="target">The target <see cref="ulong"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is != <paramref name="target"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsEqualTo(ulong value, ulong target, string name)
        {
            if (value == target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be not equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="ulong"/> value to test.</param>
        /// <param name="target">The target <see cref="ulong"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is == <paramref name="target"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotEqualTo(ulong value, ulong target, string name)
        {
            if (value != target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="ulong"/> value to test.</param>
        /// <param name="maximum">The exclusive maximum <see cref="ulong"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThan(ulong value, ulong maximum, string name)
        {
            if (value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThan(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="ulong"/> value to test.</param>
        /// <param name="maximum">The inclusive maximum <see cref="ulong"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThanOrEqualTo(ulong value, ulong maximum, string name)
        {
            if (value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThanOrEqualTo(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="ulong"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="ulong"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThan(ulong value, ulong minimum, string name)
        {
            if (value > minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThan(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="ulong"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="ulong"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThanOrEqualTo(ulong value, ulong minimum, string name)
        {
            if (value >= minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThanOrEqualTo(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given range.
        /// </summary>
        /// <param name="value">The input <see cref="ulong"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="ulong"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="ulong"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsInRange(ulong value, ulong minimum, ulong maximum, string name)
        {
            if (value >= minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given range.
        /// </summary>
        /// <param name="value">The input <see cref="ulong"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="ulong"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="ulong"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotInRange(ulong value, ulong minimum, ulong maximum, string name)
        {
            if (value < minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="ulong"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="ulong"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="ulong"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetween(ulong value, ulong minimum, ulong maximum, string name)
        {
            if (value > minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="ulong"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="ulong"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="ulong"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetween(ulong value, ulong minimum, ulong maximum, string name)
        {
            if (value <= minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="ulong"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="ulong"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see cref="ulong"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or > <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetweenOrEqualTo(ulong value, ulong minimum, ulong maximum, string name)
        {
            if (value >= minimum && value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="ulong"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="ulong"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see cref="ulong"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt;= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetweenOrEqualTo(ulong value, ulong minimum, ulong maximum, string name)
        {
            if (value < minimum || value > maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="double"/> value to test.</param>
        /// <param name="target">The target <see cref="double"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is != <paramref name="target"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsEqualTo(double value, double target, string name)
        {
            if (value == target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be not equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="double"/> value to test.</param>
        /// <param name="target">The target <see cref="double"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is == <paramref name="target"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotEqualTo(double value, double target, string name)
        {
            if (value != target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="double"/> value to test.</param>
        /// <param name="maximum">The exclusive maximum <see cref="double"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThan(double value, double maximum, string name)
        {
            if (value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThan(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="double"/> value to test.</param>
        /// <param name="maximum">The inclusive maximum <see cref="double"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThanOrEqualTo(double value, double maximum, string name)
        {
            if (value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThanOrEqualTo(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="double"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="double"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThan(double value, double minimum, string name)
        {
            if (value > minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThan(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="double"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="double"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThanOrEqualTo(double value, double minimum, string name)
        {
            if (value >= minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThanOrEqualTo(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given range.
        /// </summary>
        /// <param name="value">The input <see cref="double"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="double"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="double"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsInRange(double value, double minimum, double maximum, string name)
        {
            if (value >= minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given range.
        /// </summary>
        /// <param name="value">The input <see cref="double"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="double"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="double"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotInRange(double value, double minimum, double maximum, string name)
        {
            if (value < minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="double"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="double"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="double"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetween(double value, double minimum, double maximum, string name)
        {
            if (value > minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="double"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="double"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="double"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetween(double value, double minimum, double maximum, string name)
        {
            if (value <= minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="double"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="double"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see cref="double"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or > <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetweenOrEqualTo(double value, double minimum, double maximum, string name)
        {
            if (value >= minimum && value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="double"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="double"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see cref="double"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt;= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetweenOrEqualTo(double value, double minimum, double maximum, string name)
        {
            if (value < minimum || value > maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="decimal"/> value to test.</param>
        /// <param name="target">The target <see cref="decimal"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is != <paramref name="target"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsEqualTo(decimal value, decimal target, string name)
        {
            if (value == target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be not equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="decimal"/> value to test.</param>
        /// <param name="target">The target <see cref="decimal"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is == <paramref name="target"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotEqualTo(decimal value, decimal target, string name)
        {
            if (value != target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="decimal"/> value to test.</param>
        /// <param name="maximum">The exclusive maximum <see cref="decimal"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThan(decimal value, decimal maximum, string name)
        {
            if (value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThan(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="decimal"/> value to test.</param>
        /// <param name="maximum">The inclusive maximum <see cref="decimal"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThanOrEqualTo(decimal value, decimal maximum, string name)
        {
            if (value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThanOrEqualTo(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="decimal"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="decimal"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThan(decimal value, decimal minimum, string name)
        {
            if (value > minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThan(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="decimal"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="decimal"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThanOrEqualTo(decimal value, decimal minimum, string name)
        {
            if (value >= minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThanOrEqualTo(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given range.
        /// </summary>
        /// <param name="value">The input <see cref="decimal"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="decimal"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="decimal"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsInRange(decimal value, decimal minimum, decimal maximum, string name)
        {
            if (value >= minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given range.
        /// </summary>
        /// <param name="value">The input <see cref="decimal"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="decimal"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="decimal"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotInRange(decimal value, decimal minimum, decimal maximum, string name)
        {
            if (value < minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="decimal"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="decimal"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="decimal"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetween(decimal value, decimal minimum, decimal maximum, string name)
        {
            if (value > minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="decimal"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see cref="decimal"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see cref="decimal"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetween(decimal value, decimal minimum, decimal maximum, string name)
        {
            if (value <= minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="decimal"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="decimal"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see cref="decimal"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or > <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetweenOrEqualTo(decimal value, decimal minimum, decimal maximum, string name)
        {
            if (value >= minimum && value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see cref="decimal"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see cref="decimal"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see cref="decimal"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt;= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetweenOrEqualTo(decimal value, decimal minimum, decimal maximum, string name)
        {
            if (value < minimum || value > maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see langword="nint"/> value to test.</param>
        /// <param name="target">The target <see langword="nint"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is != <paramref name="target"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsEqualTo(nint value, nint target, string name)
        {
            if (value == target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be not equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see langword="nint"/> value to test.</param>
        /// <param name="target">The target <see langword="nint"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is == <paramref name="target"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotEqualTo(nint value, nint target, string name)
        {
            if (value != target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than a specified value.
        /// </summary>
        /// <param name="value">The input <see langword="nint"/> value to test.</param>
        /// <param name="maximum">The exclusive maximum <see langword="nint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThan(nint value, nint maximum, string name)
        {
            if (value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThan(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see langword="nint"/> value to test.</param>
        /// <param name="maximum">The inclusive maximum <see langword="nint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThanOrEqualTo(nint value, nint maximum, string name)
        {
            if (value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThanOrEqualTo(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than a specified value.
        /// </summary>
        /// <param name="value">The input <see langword="nint"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see langword="nint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThan(nint value, nint minimum, string name)
        {
            if (value > minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThan(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see langword="nint"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see langword="nint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThanOrEqualTo(nint value, nint minimum, string name)
        {
            if (value >= minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThanOrEqualTo(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given range.
        /// </summary>
        /// <param name="value">The input <see langword="nint"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see langword="nint"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see langword="nint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsInRange(nint value, nint minimum, nint maximum, string name)
        {
            if (value >= minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given range.
        /// </summary>
        /// <param name="value">The input <see langword="nint"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see langword="nint"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see langword="nint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotInRange(nint value, nint minimum, nint maximum, string name)
        {
            if (value < minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see langword="nint"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see langword="nint"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see langword="nint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetween(nint value, nint minimum, nint maximum, string name)
        {
            if (value > minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see langword="nint"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see langword="nint"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see langword="nint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetween(nint value, nint minimum, nint maximum, string name)
        {
            if (value <= minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see langword="nint"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see langword="nint"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see langword="nint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or > <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetweenOrEqualTo(nint value, nint minimum, nint maximum, string name)
        {
            if (value >= minimum && value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see langword="nint"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see langword="nint"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see langword="nint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt;= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetweenOrEqualTo(nint value, nint minimum, nint maximum, string name)
        {
            if (value < minimum || value > maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see langword="nuint"/> value to test.</param>
        /// <param name="target">The target <see langword="nuint"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is != <paramref name="target"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsEqualTo(nuint value, nuint target, string name)
        {
            if (value == target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be not equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see langword="nuint"/> value to test.</param>
        /// <param name="target">The target <see langword="nuint"/> value to test for.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is == <paramref name="target"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotEqualTo(nuint value, nuint target, string name)
        {
            if (value != target)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotEqualTo(value, target, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than a specified value.
        /// </summary>
        /// <param name="value">The input <see langword="nuint"/> value to test.</param>
        /// <param name="maximum">The exclusive maximum <see langword="nuint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThan(nuint value, nuint maximum, string name)
        {
            if (value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThan(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be less than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see langword="nuint"/> value to test.</param>
        /// <param name="maximum">The inclusive maximum <see langword="nuint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="maximum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsLessThanOrEqualTo(nuint value, nuint maximum, string name)
        {
            if (value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThanOrEqualTo(value, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than a specified value.
        /// </summary>
        /// <param name="value">The input <see langword="nuint"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see langword="nuint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThan(nuint value, nuint minimum, string name)
        {
            if (value > minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThan(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be greater than or equal to a specified value.
        /// </summary>
        /// <param name="value">The input <see langword="nuint"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see langword="nuint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/>.</exception>
        /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsGreaterThanOrEqualTo(nuint value, nuint minimum, string name)
        {
            if (value >= minimum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThanOrEqualTo(value, minimum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given range.
        /// </summary>
        /// <param name="value">The input <see langword="nuint"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see langword="nuint"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see langword="nuint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsInRange(nuint value, nuint minimum, nuint maximum, string name)
        {
            if (value >= minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given range.
        /// </summary>
        /// <param name="value">The input <see langword="nuint"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see langword="nuint"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see langword="nuint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotInRange(nuint value, nuint minimum, nuint maximum, string name)
        {
            if (value < minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotInRange(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see langword="nuint"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see langword="nuint"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see langword="nuint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetween(nuint value, nuint minimum, nuint maximum, string name)
        {
            if (value > minimum && value < maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see langword="nuint"/> value to test.</param>
        /// <param name="minimum">The exclusive minimum <see langword="nuint"/> value that is accepted.</param>
        /// <param name="maximum">The exclusive maximum <see langword="nuint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetween(nuint value, nuint minimum, nuint maximum, string name)
        {
            if (value <= minimum || value >= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetween(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must be in a given interval.
        /// </summary>
        /// <param name="value">The input <see langword="nuint"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see langword="nuint"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see langword="nuint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or > <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsBetweenOrEqualTo(nuint value, nuint minimum, nuint maximum, string name)
        {
            if (value >= minimum && value <= maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetweenOrEqualTo(value, minimum, maximum, name);
        }

        /// <summary>
        /// Asserts that the input value must not be in a given interval.
        /// </summary>
        /// <param name="value">The input <see langword="nuint"/> value to test.</param>
        /// <param name="minimum">The inclusive minimum <see langword="nuint"/> value that is accepted.</param>
        /// <param name="maximum">The inclusive maximum <see langword="nuint"/> value that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt;= <paramref name="maximum"/>.</exception>
        /// <remarks>
        /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
        /// The method is generic to avoid boxing the parameters, if they are value types.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotBetweenOrEqualTo(nuint value, nuint minimum, nuint maximum, string name)
        {
            if (value < minimum || value > maximum)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetweenOrEqualTo(value, minimum, maximum, name);
        }
    }
}
