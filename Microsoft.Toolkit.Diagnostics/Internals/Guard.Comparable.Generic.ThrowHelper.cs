// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Toolkit.Diagnostics
{
    /// <summary>
    /// Helper methods to verify conditions when running code.
    /// </summary>
    public static partial class Guard
    {
        /// <summary>
        /// Helper methods to efficiently throw exceptions.
        /// </summary>
        private static partial class ThrowHelper
        {
            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsDefault{T}"/> fails.
            /// </summary>
            /// <typeparam name="T">The type of <see langword="struct"/> value type being tested.</typeparam>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsDefault<T>(T value, string name)
                where T : struct
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(T).ToTypeString()}) must be the default value {AssertString(default(T))}, was {AssertString(value)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsNotDefault{T}"/> fails.
            /// </summary>
            /// <typeparam name="T">The type of <see langword="struct"/> value type being tested.</typeparam>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsNotDefault<T>(string name)
                where T : struct
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(T).ToTypeString()}) must not be the default value {AssertString(default(T))}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsEqualTo{T}"/> fails.
            /// </summary>
            /// <typeparam name="T">The type of values being tested.</typeparam>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsEqualTo<T>(T value, T target, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(T).ToTypeString()}) must be equal to {AssertString(target)}, was {AssertString(value)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsNotEqualTo{T}"/> fails.
            /// </summary>
            /// <typeparam name="T">The type of values being tested.</typeparam>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsNotEqualTo<T>(T value, T target, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(T).ToTypeString()}) must not be equal to {AssertString(target)}, was {AssertString(value)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsBitwiseEqualTo{T}"/> fails.
            /// </summary>
            /// <typeparam name="T">The type of input values being compared.</typeparam>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForBitwiseEqualTo<T>(T value, T target, string name)
                where T : unmanaged
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(T).ToTypeString()}) is not a bitwise match, was <{value.ToHexString()}> instead of <{target.ToHexString()}>", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsLessThan{T}"/> fails.
            /// </summary>
            /// <typeparam name="T">The type of values being tested.</typeparam>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsLessThan<T>(T value, T maximum, string name)
            {
                throw new ArgumentOutOfRangeException(name, value!, $"Parameter {AssertString(name)} ({typeof(T).ToTypeString()}) must be less than {AssertString(maximum)}, was {AssertString(value)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsLessThanOrEqualTo{T}"/> fails.
            /// </summary>
            /// <typeparam name="T">The type of values being tested.</typeparam>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsLessThanOrEqualTo<T>(T value, T maximum, string name)
            {
                throw new ArgumentOutOfRangeException(name, value!, $"Parameter {AssertString(name)} ({typeof(T).ToTypeString()}) must be less than or equal to {AssertString(maximum)}, was {AssertString(value)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsGreaterThan{T}"/> fails.
            /// </summary>
            /// <typeparam name="T">The type of values being tested.</typeparam>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsGreaterThan<T>(T value, T minimum, string name)
            {
                throw new ArgumentOutOfRangeException(name, value!, $"Parameter {AssertString(name)} ({typeof(T).ToTypeString()}) must be greater than {AssertString(minimum)}, was {AssertString(value)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsGreaterThanOrEqualTo{T}"/> fails.
            /// </summary>
            /// <typeparam name="T">The type of values being tested.</typeparam>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsGreaterThanOrEqualTo<T>(T value, T minimum, string name)
            {
                throw new ArgumentOutOfRangeException(name, value!, $"Parameter {AssertString(name)} ({typeof(T).ToTypeString()}) must be greater than or equal to {AssertString(minimum)}, was {AssertString(value)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsInRange{T}"/> fails.
            /// </summary>
            /// <typeparam name="T">The type of values being tested.</typeparam>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsInRange<T>(T value, T minimum, T maximum, string name)
            {
                throw new ArgumentOutOfRangeException(name, value!, $"Parameter {AssertString(name)} ({typeof(T).ToTypeString()}) must be in the range given by {AssertString(minimum)} and {AssertString(maximum)}, was {AssertString(value)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsInRange{T}"/> fails.
            /// </summary>
            /// <typeparam name="T">The type of values being tested.</typeparam>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsNotInRange<T>(T value, T minimum, T maximum, string name)
            {
                throw new ArgumentOutOfRangeException(name, value!, $"Parameter {AssertString(name)} ({typeof(T).ToTypeString()}) must not be in the range given by {AssertString(minimum)} and {AssertString(maximum)}, was {AssertString(value)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsBetween{T}"/> fails.
            /// </summary>
            /// <typeparam name="T">The type of values being tested.</typeparam>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsBetween<T>(T value, T minimum, T maximum, string name)
            {
                throw new ArgumentOutOfRangeException(name, value!, $"Parameter {AssertString(name)} ({typeof(T).ToTypeString()}) must be between {AssertString(minimum)} and {AssertString(maximum)}, was {AssertString(value)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsNotBetween{T}"/> fails.
            /// </summary>
            /// <typeparam name="T">The type of values being tested.</typeparam>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsNotBetween<T>(T value, T minimum, T maximum, string name)
            {
                throw new ArgumentOutOfRangeException(name, value!, $"Parameter {AssertString(name)} ({typeof(T).ToTypeString()}) must not be between {AssertString(minimum)} and {AssertString(maximum)}, was {AssertString(value)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsBetweenOrEqualTo{T}"/> fails.
            /// </summary>
            /// <typeparam name="T">The type of values being tested.</typeparam>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsBetweenOrEqualTo<T>(T value, T minimum, T maximum, string name)
            {
                throw new ArgumentOutOfRangeException(name, value!, $"Parameter {AssertString(name)} ({typeof(T).ToTypeString()}) must be between or equal to {AssertString(minimum)} and {AssertString(maximum)}, was {AssertString(value)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsNotBetweenOrEqualTo{T}"/> fails.
            /// </summary>
            /// <typeparam name="T">The type of values being tested.</typeparam>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsNotBetweenOrEqualTo<T>(T value, T minimum, T maximum, string name)
            {
                throw new ArgumentOutOfRangeException(name, value!, $"Parameter {AssertString(name)} ({typeof(T).ToTypeString()}) must not be between or equal to {AssertString(minimum)} and {AssertString(maximum)}, was {AssertString(value)}");
            }
        }
    }
}