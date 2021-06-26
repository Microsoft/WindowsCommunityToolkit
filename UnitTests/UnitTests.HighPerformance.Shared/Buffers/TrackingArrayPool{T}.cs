// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Collections.Generic;

namespace UnitTests.HighPerformance.Shared.Buffers
{
    public sealed class TrackingArrayPool<T> : ArrayPool<T>
    {
        private readonly ArrayPool<T> pool = ArrayPool<T>.Create();

        private readonly HashSet<T[]> arrays = new HashSet<T[]>();

        /// <summary>
        /// Gets the collection of currently rented out arrays
        /// </summary>
        public IReadOnlyCollection<T[]> RentedArrays => this.arrays;

        /// <inheritdoc/>
        public override T[] Rent(int minimumLength)
        {
            T[] array = this.pool.Rent(minimumLength);

            this.arrays.Add(array);

            return array;
        }

        /// <inheritdoc/>
        public override void Return(T[] array, bool clearArray = false)
        {
            this.arrays.Remove(array);

            this.pool.Return(array, clearArray);
        }
    }
}