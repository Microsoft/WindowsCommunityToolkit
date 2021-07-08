// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.WinUI.UI.Animations.Builders.Helpers
{
    /// <summary>
    /// A small generic builder type that allows to create <see cref="ArraySegment{T}"/> instances.
    /// </summary>
    /// <typeparam name="T">The type of items to create a sequence of.</typeparam>
    internal struct ListBuilder<T>
    {
        /// <summary>
        /// The <typeparamref name="T"/> array in use.
        /// </summary>
        private T[] array;

        /// <summary>
        /// The current index.
        /// </summary>
        private int index;

        /// <summary>
        /// Gets an emoty <see cref="ListBuilder{T}"/> instance.
        /// </summary>
        public static ListBuilder<T> Empty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ListBuilder<T> builder;
                builder.array = new T[1];
                builder.index = 0;

                return builder;
            }
        }

        /// <summary>
        /// Appens an item to the current builder.
        /// </summary>
        /// <param name="item">The item to append.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(T item)
        {
            if (this.index >= this.array.Length)
            {
                Array.Resize(ref this.array, this.array.Length * 2);
            }

            this.array[this.index++] = item;
        }

        /// <summary>
        /// Gets a <see cref="ArraySegment{T}"/> instance with the current items.
        /// </summary>
        /// <returns>A <see cref="ArraySegment{T}"/> instance with the current items.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArraySegment<T> GetArraySegment()
        {
            return new(this.array, 0, this.index);
        }
    }
}