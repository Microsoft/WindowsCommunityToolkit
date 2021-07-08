// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.HighPerformance.Enumerables
{
    /// <summary>
    /// A <see langword="ref"/> <see langword="struct"/> that tokenizes a given <see cref="Span{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of items to enumerate.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ref struct SpanTokenizer<T>
        where T : IEquatable<T>
    {
        /// <summary>
        /// The source <see cref="Span{T}"/> instance.
        /// </summary>
        private readonly Span<T> span;

        /// <summary>
        /// The separator item to use.
        /// </summary>
        private readonly T separator;

        /// <summary>
        /// The current initial offset.
        /// </summary>
        private int start;

        /// <summary>
        /// The current final offset.
        /// </summary>
        private int end;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpanTokenizer{T}"/> struct.
        /// </summary>
        /// <param name="span">The source <see cref="Span{T}"/> instance.</param>
        /// <param name="separator">The separator item to use.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SpanTokenizer(Span<T> span, T separator)
        {
            this.span = span;
            this.separator = separator;
            this.start = 0;
            this.end = -1;
        }

        /// <summary>
        /// Implements the duck-typed <see cref="IEnumerable{T}.GetEnumerator"/> method.
        /// </summary>
        /// <returns>An <see cref="SpanTokenizer{T}"/> instance targeting the current <see cref="Span{T}"/> value.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly SpanTokenizer<T> GetEnumerator() => this;

        /// <summary>
        /// Implements the duck-typed <see cref="System.Collections.IEnumerator.MoveNext"/> method.
        /// </summary>
        /// <returns><see langword="true"/> whether a new element is available, <see langword="false"/> otherwise</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            int
                newEnd = this.end + 1,
                length = this.span.Length;

            // Additional check if the separator is not the last character
            if (newEnd <= length)
            {
                this.start = newEnd;

                int index = this.span.Slice(newEnd).IndexOf(this.separator);

                // Extract the current subsequence
                if (index >= 0)
                {
                    this.end = newEnd + index;

                    return true;
                }

                this.end = length;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the duck-typed <see cref="IEnumerator{T}.Current"/> property.
        /// </summary>
        public readonly Span<T> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.span.Slice(this.start, this.end - this.start);
        }
    }
}