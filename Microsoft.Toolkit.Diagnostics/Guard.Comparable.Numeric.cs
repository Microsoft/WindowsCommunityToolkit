// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        /// Asserts that the input value must be within a given distance from a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="int"/> value to test.</param>
        /// <param name="target">The target <see cref="int"/> value to test for.</param>
        /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if (<paramref name="value"/> - <paramref name="target"/>) > <paramref name="delta"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsCloseTo(int value, int target, uint delta, string name)
        {
            uint difference;

            if (value >= target)
            {
                difference = (uint)(value - target);
            }
            else
            {
                difference = (uint)(target - value);
            }

            if (difference <= delta)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsCloseTo(value, target, delta, name);
        }

        /// <summary>
        /// Asserts that the input value must not be within a given distance from a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="int"/> value to test.</param>
        /// <param name="target">The target <see cref="int"/> value to test for.</param>
        /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if (<paramref name="value"/> - <paramref name="target"/>) &lt;= <paramref name="delta"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotCloseTo(int value, int target, uint delta, string name)
        {
            uint difference;

            if (value >= target)
            {
                difference = (uint)(value - target);
            }
            else
            {
                difference = (uint)(target - value);
            }

            if (difference > delta)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotCloseTo(value, target, delta, name);
        }

        /// <summary>
        /// Asserts that the input value must be within a given distance from a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="long"/> value to test.</param>
        /// <param name="target">The target <see cref="long"/> value to test for.</param>
        /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if (<paramref name="value"/> - <paramref name="target"/>) > <paramref name="delta"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsCloseTo(long value, long target, ulong delta, string name)
        {
            ulong difference;

            if (value >= target)
            {
                difference = (ulong)(value - target);
            }
            else
            {
                difference = (ulong)(target - value);
            }

            if (difference <= delta)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsCloseTo(value, target, delta, name);
        }

        /// <summary>
        /// Asserts that the input value must not be within a given distance from a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="long"/> value to test.</param>
        /// <param name="target">The target <see cref="long"/> value to test for.</param>
        /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if (<paramref name="value"/> - <paramref name="target"/>) &lt;= <paramref name="delta"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotCloseTo(long value, long target, ulong delta, string name)
        {
            ulong difference;

            if (value >= target)
            {
                difference = (ulong)(value - target);
            }
            else
            {
                difference = (ulong)(target - value);
            }

            if (difference > delta)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotCloseTo(value, target, delta, name);
        }

        /// <summary>
        /// Asserts that the input value must be within a given distance from a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="float"/> value to test.</param>
        /// <param name="target">The target <see cref="float"/> value to test for.</param>
        /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if (<paramref name="value"/> - <paramref name="target"/>) > <paramref name="delta"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsCloseTo(float value, float target, float delta, string name)
        {
            if (Math.Abs(value - target) <= delta)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsCloseTo(value, target, delta, name);
        }

        /// <summary>
        /// Asserts that the input value must not be within a given distance from a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="float"/> value to test.</param>
        /// <param name="target">The target <see cref="float"/> value to test for.</param>
        /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if (<paramref name="value"/> - <paramref name="target"/>) &lt;= <paramref name="delta"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotCloseTo(float value, float target, float delta, string name)
        {
            if (Math.Abs(value - target) > delta)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotCloseTo(value, target, delta, name);
        }

        /// <summary>
        /// Asserts that the input value must be within a given distance from a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="double"/> value to test.</param>
        /// <param name="target">The target <see cref="double"/> value to test for.</param>
        /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if (<paramref name="value"/> - <paramref name="target"/>) > <paramref name="delta"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsCloseTo(double value, double target, double delta, string name)
        {
            if (Math.Abs(value - target) <= delta)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsCloseTo(value, target, delta, name);
        }

        /// <summary>
        /// Asserts that the input value must not be within a given distance from a specified value.
        /// </summary>
        /// <param name="value">The input <see cref="double"/> value to test.</param>
        /// <param name="target">The target <see cref="double"/> value to test for.</param>
        /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if (<paramref name="value"/> - <paramref name="target"/>) &lt;= <paramref name="delta"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotCloseTo(double value, double target, double delta, string name)
        {
            if (Math.Abs(value - target) > delta)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotCloseTo(value, target, delta, name);
        }

        /// <summary>
        /// Asserts that the input value must be within a given distance from a specified value.
        /// </summary>
        /// <param name="value">The input <see langword="nint"/> value to test.</param>
        /// <param name="target">The target <see langword="nint"/> value to test for.</param>
        /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if (<paramref name="value"/> - <paramref name="target"/>) > <paramref name="delta"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsCloseTo(nint value, nint target, nuint delta, string name)
        {
            nuint difference;

            if (value >= target)
            {
                difference = (nuint)(value - target);
            }
            else
            {
                difference = (nuint)(target - value);
            }

            if (difference <= delta)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsCloseTo(value, target, delta, name);
        }

        /// <summary>
        /// Asserts that the input value must not be within a given distance from a specified value.
        /// </summary>
        /// <param name="value">The input <see langword="nint"/> value to test.</param>
        /// <param name="target">The target <see langword="nint"/> value to test for.</param>
        /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if (<paramref name="value"/> - <paramref name="target"/>) &lt;= <paramref name="delta"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotCloseTo(nint value, nint target, nuint delta, string name)
        {
            nuint difference;

            if (value >= target)
            {
                difference = (nuint)(value - target);
            }
            else
            {
                difference = (nuint)(target - value);
            }

            if (difference > delta)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotCloseTo(value, target, delta, name);
        }
    }
}