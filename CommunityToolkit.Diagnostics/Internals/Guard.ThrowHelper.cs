// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace CommunityToolkit.Diagnostics
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
            /// Returns a formatted representation of the input value.
            /// </summary>
            /// <param name="obj">The input <see cref="object"/> to format.</param>
            /// <returns>A formatted representation of <paramref name="obj"/> to display in error messages.</returns>
            [Pure]
            private static string AssertString(object? obj)
            {
                return obj switch
                {
                    string _ => $"\"{obj}\"",
                    null => "null",
                    _ => $"<{obj}>"
                };
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsNull{T}(T,string)"/> (where <typeparamref name="T"/> is <see langword="class"/>) fails.
            /// </summary>
            /// <typeparam name="T">The type of the input value.</typeparam>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsNull<T>(T value, string name)
                where T : class
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(T).ToTypeString()}) must be null, was {AssertString(value)} ({value.GetType().ToTypeString()})", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsNull{T}(T,string)"/> (where <typeparamref name="T"/> is <see langword="struct"/>) fails.
            /// </summary>
            /// <typeparam name="T">The type of the input value.</typeparam>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsNull<T>(T? value, string name)
                where T : struct
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(T?).ToTypeString()}) must be null, was {AssertString(value)} ({typeof(T).ToTypeString()})", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentNullException"/> when <see cref="Guard.IsNotNull{T}(T,string)"/> fails.
            /// </summary>
            /// <typeparam name="T">The type of the input value.</typeparam>
            [DoesNotReturn]
            public static void ThrowArgumentNullExceptionForIsNotNull<T>(string name)
            {
                throw new ArgumentNullException(name, $"Parameter {AssertString(name)} ({typeof(T).ToTypeString()}) must be not null)");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsOfType{T}"/> fails.
            /// </summary>
            /// <typeparam name="T">The type of the input value.</typeparam>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsOfType<T>(object value, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} must be of type {typeof(T).ToTypeString()}, was {value.GetType().ToTypeString()}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsNotOfType{T}"/> fails.
            /// </summary>
            /// <typeparam name="T">The type of the input value.</typeparam>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsNotOfType<T>(object value, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} must not be of type {typeof(T).ToTypeString()}, was {value.GetType().ToTypeString()}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsOfType"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsOfType(object value, Type type, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} must be of type {type.ToTypeString()}, was {value.GetType().ToTypeString()}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsNotOfType"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsNotOfType(object value, Type type, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} must not be of type {type.ToTypeString()}, was {value.GetType().ToTypeString()}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsAssignableToType{T}"/> fails.
            /// </summary>
            /// <typeparam name="T">The type being checked against.</typeparam>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsAssignableToType<T>(object value, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} must be assignable to type {typeof(T).ToTypeString()}, was {value.GetType().ToTypeString()}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsNotAssignableToType{T}"/> fails.
            /// </summary>
            /// <typeparam name="T">The type being checked against.</typeparam>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsNotAssignableToType<T>(object value, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} must not be assignable to type {typeof(T).ToTypeString()}, was {value.GetType().ToTypeString()}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsAssignableToType"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsAssignableToType(object value, Type type, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} must be assignable to type {type.ToTypeString()}, was {value.GetType().ToTypeString()}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsAssignableToType"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsNotAssignableToType(object value, Type type, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} must not be assignable to type {type.ToTypeString()}, was {value.GetType().ToTypeString()}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsReferenceEqualTo{T}"/> fails.
            /// </summary>
            /// <typeparam name="T">The type of input value being compared.</typeparam>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsReferenceEqualTo<T>(string name)
                where T : class
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(T).ToTypeString()}) must be the same instance as the target object", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsReferenceNotEqualTo{T}"/> fails.
            /// </summary>
            /// <typeparam name="T">The type of input value being compared.</typeparam>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsReferenceNotEqualTo<T>(string name)
                where T : class
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(T).ToTypeString()}) must not be the same instance as the target object", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsTrue(bool,string)"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsTrue(string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} must be true, was false", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsTrue(bool,string,string)"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsTrue(string name, string message)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} must be true, was false: {AssertString(message)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsFalse(bool,string)"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsFalse(string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} must be false, was true", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsFalse(bool,string,string)"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsFalse(string name, string message)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} must be false, was true: {AssertString(message)}", name);
            }
        }
    }
}