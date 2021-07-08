// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.HighPerformance.Shared.Buffers.Internals;

namespace UnitTests.HighPerformance.Helpers
{
    [TestClass]
    public partial class Test_ParallelHelper
    {
        /// <summary>
        /// Gets the list of counts to test the For (1D) extensions for
        /// </summary>
        private static ReadOnlySpan<int> TestForCounts => new[] { 0, 1, 7, 128, 255, 256, short.MaxValue, short.MaxValue + 1, 123_938, 1_678_922, 71_890_819 };

        [TestCategory("ParallelHelper")]
        [TestMethod]
        public unsafe void Test_ParallelHelper_ForWithIndices()
        {
            foreach (int count in TestForCounts)
            {
                using UnmanagedSpanOwner<int> data = new UnmanagedSpanOwner<int>(count);

                data.GetSpan().Clear();

                ParallelHelper.For(0, data.Length, new Assigner(data.Length, data.Ptr));

                foreach (var item in data.GetSpan().Enumerate())
                {
                    if (item.Index != item.Value)
                    {
                        Assert.Fail($"Invalid item at position {item.Index}, value was {item.Value}");
                    }
                }
            }
        }

#if NETCOREAPP3_1 || NET5_0
        [TestCategory("ParallelHelper")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_ParallelHelper_ForInvalidRange_FromEnd()
        {
            ParallelHelper.For<Assigner>(..^1);
        }

        [TestCategory("ParallelHelper")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_ParallelHelper_ForInvalidRange_RangeAll()
        {
            ParallelHelper.For<Assigner>(..);
        }

        [TestCategory("ParallelHelper")]
        [TestMethod]
        public unsafe void Test_ParallelHelper_ForWithRanges()
        {
            foreach (int count in TestForCounts)
            {
                using UnmanagedSpanOwner<int> data = new UnmanagedSpanOwner<int>(count);

                data.GetSpan().Clear();

                ParallelHelper.For(..data.Length, new Assigner(data.Length, data.Ptr));

                foreach (var item in data.GetSpan().Enumerate())
                {
                    if (item.Index != item.Value)
                    {
                        Assert.Fail($"Invalid item at position {item.Index}, value was {item.Value}");
                    }
                }
            }
        }
#endif

        /// <summary>
        /// A type implementing <see cref="IAction"/> to initialize an array
        /// </summary>
        private readonly unsafe struct Assigner : IAction
        {
            private readonly int length;
            private readonly int* ptr;

            public Assigner(int length, int* ptr)
            {
                this.length = length;
                this.ptr = ptr;
            }

            /// <inheritdoc/>
            public void Invoke(int i)
            {
                if ((uint)i >= (uint)this.length)
                {
                    throw new IndexOutOfRangeException($"The target position was out of range, was {i} and should've been in [0, {this.length})");
                }

                if (this.ptr[i] != 0)
                {
                    throw new InvalidOperationException($"Invalid target position {i}, was {this.ptr[i]} instead of 0");
                }

                this.ptr[i] = i;
            }
        }
    }
}