// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// =====================
// Auto generated file
// =====================

using System;
using System.Collections.Generic;
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
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsEmpty{T}(T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsEmpty<T>(Span<T> span, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(Span<T>).ToTypeString()}) must be empty, had a size of {AssertString(span.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeEqualTo<T>(Span<T> span, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(Span<T>).ToTypeString()}) must have a size equal to {size}, had a size of {AssertString(span.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeNotEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeNotEqualTo<T>(Span<T> span, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(Span<T>).ToTypeString()}) must have a size not equal to {size}, had a size of {AssertString(span.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeGreaterThan{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeGreaterThan<T>(Span<T> span, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(Span<T>).ToTypeString()}) must have a size over {size}, had a size of {AssertString(span.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeGreaterThanOrEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeGreaterThanOrEqualTo<T>(Span<T> span, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(Span<T>).ToTypeString()}) must have a size of at least {size}, had a size of {AssertString(span.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThan{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThan<T>(Span<T> span, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(Span<T>).ToTypeString()}) must have a size less than {size}, had a size of {AssertString(span.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThanOrEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThanOrEqualTo<T>(Span<T> span, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(Span<T>).ToTypeString()}) must have a size less than or equal to {size}, had a size of {AssertString(span.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeEqualTo{T}(T[],T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeEqualTo<T>(Span<T> source, Span<T> destination, string name)
            {
                throw new ArgumentException($"The source {AssertString(name)} ({typeof(Span<T>).ToTypeString()}) must have a size equal to {AssertString(destination.Length)} (the destination), had a size of {AssertString(source.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThanOrEqualTo{T}(T[],T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThanOrEqualTo<T>(Span<T> source, Span<T> destination, string name)
            {
                throw new ArgumentException($"The source {AssertString(name)} ({typeof(Span<T>).ToTypeString()}) must have a size less than or equal to {AssertString(destination.Length)} (the destination), had a size of {AssertString(source.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsInRangeFor{T}(int,T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsInRangeFor<T>(int index, Span<T> span, string name)
            {
                throw new ArgumentOutOfRangeException(name, index, $"Parameter {AssertString(name)} (int) must be in the range given by <0> and {AssertString(span.Length)} to be a valid index for the target collection ({typeof(Span<T>).ToTypeString()}), was {AssertString(index)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsNotInRangeFor{T}(int,T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsNotInRangeFor<T>(int index, Span<T> span, string name)
            {
                throw new ArgumentOutOfRangeException(name, index, $"Parameter {AssertString(name)} (int) must not be in the range given by <0> and {AssertString(span.Length)} to be an invalid index for the target collection ({typeof(Span<T>).ToTypeString()}), was {AssertString(index)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsEmpty{T}(T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsEmpty<T>(ReadOnlySpan<T> span, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(ReadOnlySpan<T>).ToTypeString()}) must be empty, had a size of {AssertString(span.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeEqualTo<T>(ReadOnlySpan<T> span, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(ReadOnlySpan<T>).ToTypeString()}) must have a size equal to {size}, had a size of {AssertString(span.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeNotEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeNotEqualTo<T>(ReadOnlySpan<T> span, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(ReadOnlySpan<T>).ToTypeString()}) must have a size not equal to {size}, had a size of {AssertString(span.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeGreaterThan{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeGreaterThan<T>(ReadOnlySpan<T> span, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(ReadOnlySpan<T>).ToTypeString()}) must have a size over {size}, had a size of {AssertString(span.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeGreaterThanOrEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeGreaterThanOrEqualTo<T>(ReadOnlySpan<T> span, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(ReadOnlySpan<T>).ToTypeString()}) must have a size of at least {size}, had a size of {AssertString(span.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThan{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThan<T>(ReadOnlySpan<T> span, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(ReadOnlySpan<T>).ToTypeString()}) must have a size less than {size}, had a size of {AssertString(span.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThanOrEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThanOrEqualTo<T>(ReadOnlySpan<T> span, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(ReadOnlySpan<T>).ToTypeString()}) must have a size less than or equal to {size}, had a size of {AssertString(span.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeEqualTo{T}(T[],T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeEqualTo<T>(ReadOnlySpan<T> source, Span<T> destination, string name)
            {
                throw new ArgumentException($"The source {AssertString(name)} ({typeof(ReadOnlySpan<T>).ToTypeString()}) must have a size equal to {AssertString(destination.Length)} (the destination), had a size of {AssertString(source.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThanOrEqualTo{T}(T[],T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThanOrEqualTo<T>(ReadOnlySpan<T> source, Span<T> destination, string name)
            {
                throw new ArgumentException($"The source {AssertString(name)} ({typeof(ReadOnlySpan<T>).ToTypeString()}) must have a size less than or equal to {AssertString(destination.Length)} (the destination), had a size of {AssertString(source.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsInRangeFor{T}(int,T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsInRangeFor<T>(int index, ReadOnlySpan<T> span, string name)
            {
                throw new ArgumentOutOfRangeException(name, index, $"Parameter {AssertString(name)} (int) must be in the range given by <0> and {AssertString(span.Length)} to be a valid index for the target collection ({typeof(ReadOnlySpan<T>).ToTypeString()}), was {AssertString(index)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsNotInRangeFor{T}(int,T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsNotInRangeFor<T>(int index, ReadOnlySpan<T> span, string name)
            {
                throw new ArgumentOutOfRangeException(name, index, $"Parameter {AssertString(name)} (int) must not be in the range given by <0> and {AssertString(span.Length)} to be an invalid index for the target collection ({typeof(ReadOnlySpan<T>).ToTypeString()}), was {AssertString(index)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsEmpty{T}(T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsEmpty<T>(Memory<T> memory, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(Memory<T>).ToTypeString()}) must be empty, had a size of {AssertString(memory.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeEqualTo<T>(Memory<T> memory, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(Memory<T>).ToTypeString()}) must have a size equal to {size}, had a size of {AssertString(memory.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeNotEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeNotEqualTo<T>(Memory<T> memory, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(Memory<T>).ToTypeString()}) must have a size not equal to {size}, had a size of {AssertString(memory.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeGreaterThan{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeGreaterThan<T>(Memory<T> memory, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(Memory<T>).ToTypeString()}) must have a size over {size}, had a size of {AssertString(memory.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeGreaterThanOrEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeGreaterThanOrEqualTo<T>(Memory<T> memory, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(Memory<T>).ToTypeString()}) must have a size of at least {size}, had a size of {AssertString(memory.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThan{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThan<T>(Memory<T> memory, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(Memory<T>).ToTypeString()}) must have a size less than {size}, had a size of {AssertString(memory.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThanOrEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThanOrEqualTo<T>(Memory<T> memory, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(Memory<T>).ToTypeString()}) must have a size less than or equal to {size}, had a size of {AssertString(memory.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeEqualTo{T}(T[],T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeEqualTo<T>(Memory<T> source, Memory<T> destination, string name)
            {
                throw new ArgumentException($"The source {AssertString(name)} ({typeof(Memory<T>).ToTypeString()}) must have a size equal to {AssertString(destination.Length)} (the destination), had a size of {AssertString(source.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThanOrEqualTo{T}(T[],T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThanOrEqualTo<T>(Memory<T> source, Memory<T> destination, string name)
            {
                throw new ArgumentException($"The source {AssertString(name)} ({typeof(Memory<T>).ToTypeString()}) must have a size less than or equal to {AssertString(destination.Length)} (the destination), had a size of {AssertString(source.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsInRangeFor{T}(int,T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsInRangeFor<T>(int index, Memory<T> memory, string name)
            {
                throw new ArgumentOutOfRangeException(name, index, $"Parameter {AssertString(name)} (int) must be in the range given by <0> and {AssertString(memory.Length)} to be a valid index for the target collection ({typeof(Memory<T>).ToTypeString()}), was {AssertString(index)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsNotInRangeFor{T}(int,T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsNotInRangeFor<T>(int index, Memory<T> memory, string name)
            {
                throw new ArgumentOutOfRangeException(name, index, $"Parameter {AssertString(name)} (int) must not be in the range given by <0> and {AssertString(memory.Length)} to be an invalid index for the target collection ({typeof(Memory<T>).ToTypeString()}), was {AssertString(index)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsEmpty{T}(T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsEmpty<T>(ReadOnlyMemory<T> memory, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(ReadOnlyMemory<T>).ToTypeString()}) must be empty, had a size of {AssertString(memory.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeEqualTo<T>(ReadOnlyMemory<T> memory, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(ReadOnlyMemory<T>).ToTypeString()}) must have a size equal to {size}, had a size of {AssertString(memory.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeNotEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeNotEqualTo<T>(ReadOnlyMemory<T> memory, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(ReadOnlyMemory<T>).ToTypeString()}) must have a size not equal to {size}, had a size of {AssertString(memory.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeGreaterThan{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeGreaterThan<T>(ReadOnlyMemory<T> memory, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(ReadOnlyMemory<T>).ToTypeString()}) must have a size over {size}, had a size of {AssertString(memory.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeGreaterThanOrEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeGreaterThanOrEqualTo<T>(ReadOnlyMemory<T> memory, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(ReadOnlyMemory<T>).ToTypeString()}) must have a size of at least {size}, had a size of {AssertString(memory.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThan{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThan<T>(ReadOnlyMemory<T> memory, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(ReadOnlyMemory<T>).ToTypeString()}) must have a size less than {size}, had a size of {AssertString(memory.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThanOrEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThanOrEqualTo<T>(ReadOnlyMemory<T> memory, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(ReadOnlyMemory<T>).ToTypeString()}) must have a size less than or equal to {size}, had a size of {AssertString(memory.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeEqualTo{T}(T[],T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeEqualTo<T>(ReadOnlyMemory<T> source, Memory<T> destination, string name)
            {
                throw new ArgumentException($"The source {AssertString(name)} ({typeof(ReadOnlyMemory<T>).ToTypeString()}) must have a size equal to {AssertString(destination.Length)} (the destination), had a size of {AssertString(source.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThanOrEqualTo{T}(T[],T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThanOrEqualTo<T>(ReadOnlyMemory<T> source, Memory<T> destination, string name)
            {
                throw new ArgumentException($"The source {AssertString(name)} ({typeof(ReadOnlyMemory<T>).ToTypeString()}) must have a size less than or equal to {AssertString(destination.Length)} (the destination), had a size of {AssertString(source.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsInRangeFor{T}(int,T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsInRangeFor<T>(int index, ReadOnlyMemory<T> memory, string name)
            {
                throw new ArgumentOutOfRangeException(name, index, $"Parameter {AssertString(name)} (int) must be in the range given by <0> and {AssertString(memory.Length)} to be a valid index for the target collection ({typeof(ReadOnlyMemory<T>).ToTypeString()}), was {AssertString(index)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsNotInRangeFor{T}(int,T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsNotInRangeFor<T>(int index, ReadOnlyMemory<T> memory, string name)
            {
                throw new ArgumentOutOfRangeException(name, index, $"Parameter {AssertString(name)} (int) must not be in the range given by <0> and {AssertString(memory.Length)} to be an invalid index for the target collection ({typeof(ReadOnlyMemory<T>).ToTypeString()}), was {AssertString(index)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsEmpty{T}(T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsEmpty<T>(T[] array, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(T[]).ToTypeString()}) must be empty, had a size of {AssertString(array.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeEqualTo<T>(T[] array, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(T[]).ToTypeString()}) must have a size equal to {size}, had a size of {AssertString(array.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeNotEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeNotEqualTo<T>(T[] array, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(T[]).ToTypeString()}) must have a size not equal to {size}, had a size of {AssertString(array.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeGreaterThan{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeGreaterThan<T>(T[] array, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(T[]).ToTypeString()}) must have a size over {size}, had a size of {AssertString(array.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeGreaterThanOrEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeGreaterThanOrEqualTo<T>(T[] array, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(T[]).ToTypeString()}) must have a size of at least {size}, had a size of {AssertString(array.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThan{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThan<T>(T[] array, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(T[]).ToTypeString()}) must have a size less than {size}, had a size of {AssertString(array.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThanOrEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThanOrEqualTo<T>(T[] array, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(T[]).ToTypeString()}) must have a size less than or equal to {size}, had a size of {AssertString(array.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeEqualTo{T}(T[],T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeEqualTo<T>(T[] source, T[] destination, string name)
            {
                throw new ArgumentException($"The source {AssertString(name)} ({typeof(T[]).ToTypeString()}) must have a size equal to {AssertString(destination.Length)} (the destination), had a size of {AssertString(source.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThanOrEqualTo{T}(T[],T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThanOrEqualTo<T>(T[] source, T[] destination, string name)
            {
                throw new ArgumentException($"The source {AssertString(name)} ({typeof(T[]).ToTypeString()}) must have a size less than or equal to {AssertString(destination.Length)} (the destination), had a size of {AssertString(source.Length)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsInRangeFor{T}(int,T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsInRangeFor<T>(int index, T[] array, string name)
            {
                throw new ArgumentOutOfRangeException(name, index, $"Parameter {AssertString(name)} (int) must be in the range given by <0> and {AssertString(array.Length)} to be a valid index for the target collection ({typeof(T[]).ToTypeString()}), was {AssertString(index)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsNotInRangeFor{T}(int,T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsNotInRangeFor<T>(int index, T[] array, string name)
            {
                throw new ArgumentOutOfRangeException(name, index, $"Parameter {AssertString(name)} (int) must not be in the range given by <0> and {AssertString(array.Length)} to be an invalid index for the target collection ({typeof(T[]).ToTypeString()}), was {AssertString(index)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsEmpty{T}(T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsEmpty<T>(List<T> list, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(List<T>).ToTypeString()}) must be empty, had a size of {AssertString(list.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeEqualTo<T>(List<T> list, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(List<T>).ToTypeString()}) must have a size equal to {size}, had a size of {AssertString(list.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeNotEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeNotEqualTo<T>(List<T> list, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(List<T>).ToTypeString()}) must have a size not equal to {size}, had a size of {AssertString(list.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeGreaterThan{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeGreaterThan<T>(List<T> list, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(List<T>).ToTypeString()}) must have a size over {size}, had a size of {AssertString(list.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeGreaterThanOrEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeGreaterThanOrEqualTo<T>(List<T> list, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(List<T>).ToTypeString()}) must have a size of at least {size}, had a size of {AssertString(list.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThan{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThan<T>(List<T> list, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(List<T>).ToTypeString()}) must have a size less than {size}, had a size of {AssertString(list.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThanOrEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThanOrEqualTo<T>(List<T> list, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(List<T>).ToTypeString()}) must have a size less than or equal to {size}, had a size of {AssertString(list.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeEqualTo{T}(T[],T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeEqualTo<T>(List<T> source, List<T> destination, string name)
            {
                throw new ArgumentException($"The source {AssertString(name)} ({typeof(List<T>).ToTypeString()}) must have a size equal to {AssertString(destination.Count)} (the destination), had a size of {AssertString(source.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThanOrEqualTo{T}(T[],T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThanOrEqualTo<T>(List<T> source, List<T> destination, string name)
            {
                throw new ArgumentException($"The source {AssertString(name)} ({typeof(List<T>).ToTypeString()}) must have a size less than or equal to {AssertString(destination.Count)} (the destination), had a size of {AssertString(source.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsInRangeFor{T}(int,T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsInRangeFor<T>(int index, List<T> list, string name)
            {
                throw new ArgumentOutOfRangeException(name, index, $"Parameter {AssertString(name)} (int) must be in the range given by <0> and {AssertString(list.Count)} to be a valid index for the target collection ({typeof(List<T>).ToTypeString()}), was {AssertString(index)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsNotInRangeFor{T}(int,T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsNotInRangeFor<T>(int index, List<T> list, string name)
            {
                throw new ArgumentOutOfRangeException(name, index, $"Parameter {AssertString(name)} (int) must not be in the range given by <0> and {AssertString(list.Count)} to be an invalid index for the target collection ({typeof(List<T>).ToTypeString()}), was {AssertString(index)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsEmpty{T}(T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsEmpty<T>(ICollection<T> collection, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(ICollection<T>).ToTypeString()}) must be empty, had a size of {AssertString(collection.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeEqualTo<T>(ICollection<T> collection, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(ICollection<T>).ToTypeString()}) must have a size equal to {size}, had a size of {AssertString(collection.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeNotEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeNotEqualTo<T>(ICollection<T> collection, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(ICollection<T>).ToTypeString()}) must have a size not equal to {size}, had a size of {AssertString(collection.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeGreaterThan{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeGreaterThan<T>(ICollection<T> collection, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(ICollection<T>).ToTypeString()}) must have a size over {size}, had a size of {AssertString(collection.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeGreaterThanOrEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeGreaterThanOrEqualTo<T>(ICollection<T> collection, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(ICollection<T>).ToTypeString()}) must have a size of at least {size}, had a size of {AssertString(collection.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThan{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThan<T>(ICollection<T> collection, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(ICollection<T>).ToTypeString()}) must have a size less than {size}, had a size of {AssertString(collection.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThanOrEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThanOrEqualTo<T>(ICollection<T> collection, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(ICollection<T>).ToTypeString()}) must have a size less than or equal to {size}, had a size of {AssertString(collection.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeEqualTo{T}(T[],T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeEqualTo<T>(ICollection<T> source, ICollection<T> destination, string name)
            {
                throw new ArgumentException($"The source {AssertString(name)} ({typeof(ICollection<T>).ToTypeString()}) must have a size equal to {AssertString(destination.Count)} (the destination), had a size of {AssertString(source.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThanOrEqualTo{T}(T[],T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThanOrEqualTo<T>(ICollection<T> source, ICollection<T> destination, string name)
            {
                throw new ArgumentException($"The source {AssertString(name)} ({typeof(ICollection<T>).ToTypeString()}) must have a size less than or equal to {AssertString(destination.Count)} (the destination), had a size of {AssertString(source.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsInRangeFor{T}(int,T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsInRangeFor<T>(int index, ICollection<T> collection, string name)
            {
                throw new ArgumentOutOfRangeException(name, index, $"Parameter {AssertString(name)} (int) must be in the range given by <0> and {AssertString(collection.Count)} to be a valid index for the target collection ({typeof(ICollection<T>).ToTypeString()}), was {AssertString(index)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsNotInRangeFor{T}(int,T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsNotInRangeFor<T>(int index, ICollection<T> collection, string name)
            {
                throw new ArgumentOutOfRangeException(name, index, $"Parameter {AssertString(name)} (int) must not be in the range given by <0> and {AssertString(collection.Count)} to be an invalid index for the target collection ({typeof(ICollection<T>).ToTypeString()}), was {AssertString(index)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsEmpty{T}(T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsEmpty<T>(IReadOnlyCollection<T> collection, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(IReadOnlyCollection<T>).ToTypeString()}) must be empty, had a size of {AssertString(collection.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeEqualTo<T>(IReadOnlyCollection<T> collection, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(IReadOnlyCollection<T>).ToTypeString()}) must have a size equal to {size}, had a size of {AssertString(collection.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeNotEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeNotEqualTo<T>(IReadOnlyCollection<T> collection, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(IReadOnlyCollection<T>).ToTypeString()}) must have a size not equal to {size}, had a size of {AssertString(collection.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeGreaterThan{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeGreaterThan<T>(IReadOnlyCollection<T> collection, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(IReadOnlyCollection<T>).ToTypeString()}) must have a size over {size}, had a size of {AssertString(collection.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeGreaterThanOrEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeGreaterThanOrEqualTo<T>(IReadOnlyCollection<T> collection, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(IReadOnlyCollection<T>).ToTypeString()}) must have a size of at least {size}, had a size of {AssertString(collection.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThan{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThan<T>(IReadOnlyCollection<T> collection, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(IReadOnlyCollection<T>).ToTypeString()}) must have a size less than {size}, had a size of {AssertString(collection.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThanOrEqualTo{T}(T[],int,string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThanOrEqualTo<T>(IReadOnlyCollection<T> collection, int size, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(IReadOnlyCollection<T>).ToTypeString()}) must have a size less than or equal to {size}, had a size of {AssertString(collection.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeEqualTo{T}(T[],T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeEqualTo<T>(IReadOnlyCollection<T> source, ICollection<T> destination, string name)
            {
                throw new ArgumentException($"The source {AssertString(name)} ({typeof(IReadOnlyCollection<T>).ToTypeString()}) must have a size equal to {AssertString(destination.Count)} (the destination), had a size of {AssertString(source.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.HasSizeLessThanOrEqualTo{T}(T[],T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForHasSizeLessThanOrEqualTo<T>(IReadOnlyCollection<T> source, ICollection<T> destination, string name)
            {
                throw new ArgumentException($"The source {AssertString(name)} ({typeof(IReadOnlyCollection<T>).ToTypeString()}) must have a size less than or equal to {AssertString(destination.Count)} (the destination), had a size of {AssertString(source.Count)}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsInRangeFor{T}(int,T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsInRangeFor<T>(int index, IReadOnlyCollection<T> collection, string name)
            {
                throw new ArgumentOutOfRangeException(name, index, $"Parameter {AssertString(name)} (int) must be in the range given by <0> and {AssertString(collection.Count)} to be a valid index for the target collection ({typeof(IReadOnlyCollection<T>).ToTypeString()}), was {AssertString(index)}");
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="Guard.IsNotInRangeFor{T}(int,T[],string)"/> (or an overload) fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeExceptionForIsNotInRangeFor<T>(int index, IReadOnlyCollection<T> collection, string name)
            {
                throw new ArgumentOutOfRangeException(name, index, $"Parameter {AssertString(name)} (int) must not be in the range given by <0> and {AssertString(collection.Count)} to be an invalid index for the target collection ({typeof(IReadOnlyCollection<T>).ToTypeString()}), was {AssertString(index)}");
            }
        }
    }
}
