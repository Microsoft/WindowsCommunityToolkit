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
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsNullOrEmpty"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsNullOrEmpty(string? text, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} (string) must be null or empty, was {AssertString(text)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsNotNullOrEmpty"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsNotNullOrEmpty(string? text, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} (string) must not be null or empty, was {(text is null ? "null" : "empty")}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsNullOrWhitespace"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsNullOrWhiteSpace(string? text, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} (string) must be null or whitespace, was {AssertString(text)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsNotNullOrWhitespace"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsNotNullOrWhiteSpace(string? text, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} (string) must not be null or whitespace, was {(text is null ? "null" : "whitespace")}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsEmpty"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsEmpty(string text, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} (string) must be empty, was {AssertString(text)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsNotEmpty"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsNotEmpty(string text, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} (string) must not be empty", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsWhitespace"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsWhiteSpace(string text, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} (string) must be whitespace, was {AssertString(text)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsNotWhitespace"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsNotWhiteSpace(string text, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} (string) must not be whitespace, was {AssertString(text)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeEqualTo(string,int,string)"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeEqualTo(string text, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} (string) must have a size equal to {size}, had a size of {text.Length} and was {AssertString(text)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeNotEqualTo"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeNotEqualTo(string text, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} (string) must not have a size equal to {size}, was {AssertString(text)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeGreaterThan"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeGreaterThan(string text, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} (string) must have a size over {size}, had a size of {text.Length} and was {AssertString(text)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeGreaterThanOrEqualTo"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeGreaterThanOrEqualTo(string text, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} (string) must have a size of at least {size}, had a size of {text.Length} and was {AssertString(text)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThan"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThan(string text, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} (string) must have a size less than {size}, had a size of {text.Length} and was {AssertString(text)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThanOrEqualTo(string,int,string)"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThanOrEqualTo(string text, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} (string) must have a size less than or equal to {size}, had a size of {text.Length} and was {AssertString(text)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeEqualTo(string,string,string)"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeEqualTo(string source, string destination, string name)
            {
                throw new ArgumentException($"The source {AssertString(name)} (string) must have a size equal to {AssertString(destination.Length)} (the destination), had a size of {AssertString(source.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThanOrEqualTo(string,string,string)"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThanOrEqualTo(string source, string destination, string name)
            {
                throw new ArgumentException($"The source {AssertString(name)} (string) must have a size less than or equal to {AssertString(destination.Length)} (the destination), had a size of {AssertString(source.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsInRangeFor(int,string,string)"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsInRangeFor(int index, string text, string name)
            {
                throw new ArgumentOutOfRangeException(name, index, $"Parameter {AssertString(name)} (int) must be in the range given by <0> and {AssertString(text.Length)} to be a valid index for the target string, was {AssertString(index)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsNotInRangeFor(int,string,string)"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsNotInRangeFor(int index, string text, string name)
            {
                throw new ArgumentOutOfRangeException(name, index, $"Parameter {AssertString(name)} (int) must not be in the range given by <0> and {AssertString(text.Length)} to be an invalid index for the target string, was {AssertString(index)}");
            }
        }
    }
}