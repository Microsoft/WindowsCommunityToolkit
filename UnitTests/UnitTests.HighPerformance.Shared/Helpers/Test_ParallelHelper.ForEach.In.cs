// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using CommunityToolkit.HighPerformance.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.HighPerformance.Shared.Buffers.Internals;

namespace UnitTests.HighPerformance.Helpers
{
    public partial class Test_ParallelHelper
    {
        [TestCategory("ParallelHelper")]
        [TestMethod]
        public unsafe void Test_ParallelHelper_ForEach_In()
        {
            foreach (int count in TestForCounts)
            {
                using UnmanagedSpanOwner<int> data = CreateRandomData(count);

                int sum = 0;

                ParallelHelper.ForEach<int, Summer>(data.Memory, new Summer(&sum));

                int expected = 0;

                foreach (int n in data.GetSpan())
                {
                    expected += n;
                }

                Assert.AreEqual(sum, expected, $"The sum doesn't match, was {sum} instead of {expected}");
            }
        }

        /// <summary>
        /// A type implementing <see cref="IInAction{T}"/> to sum array elements.
        /// </summary>
        private readonly unsafe struct Summer : IInAction<int>
        {
            private readonly int* ptr;

            public Summer(int* ptr) => this.ptr = ptr;

            /// <inheritdoc/>
            public void Invoke(in int i) => Interlocked.Add(ref Unsafe.AsRef<int>(this.ptr), i);
        }

        /// <summary>
        /// Creates a random <see cref="int"/> array filled with random numbers.
        /// </summary>
        /// <param name="count">The number of array items to create.</param>
        /// <returns>An array of random <see cref="int"/> elements.</returns>
        [Pure]
        private static UnmanagedSpanOwner<int> CreateRandomData(int count)
        {
            var random = new Random(count);

            UnmanagedSpanOwner<int> data = new UnmanagedSpanOwner<int>(count);

            foreach (ref int n in data.GetSpan())
            {
                n = random.Next(0, byte.MaxValue);
            }

            return data;
        }
    }
}