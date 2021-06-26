// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using Microsoft.Toolkit.HighPerformance.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.HighPerformance.Shared.Buffers.Internals;

namespace UnitTests.HighPerformance.Helpers
{
    public partial class Test_ParallelHelper
    {
        /// <summary>
        /// Gets the list of counts to test the For2D extensions for
        /// </summary>
        private static ReadOnlySpan<Size> TestFor2DSizes => new[]
        {
            new Size(0, 0),
            new Size(0, 1),
            new Size(1, 1),
            new Size(3, 3),
            new Size(1024, 1024),
            new Size(512, 2175),
            new Size(4039, 11231)
        };

        [TestCategory("ParallelHelper")]
        [TestMethod]
        public unsafe void Test_ParallelHelper_For2DWithIndices()
        {
            foreach (var size in TestFor2DSizes)
            {
                using UnmanagedSpanOwner<int> data = new UnmanagedSpanOwner<int>(size.Height * size.Width);

                data.GetSpan().Clear();

                ParallelHelper.For2D(0, size.Height, 0, size.Width, new Assigner2D(size.Height, size.Width, data.Ptr));

                for (int i = 0; i < size.Height; i++)
                {
                    for (int j = 0; j < size.Width; j++)
                    {
                        if (data.Ptr[(i * size.Width) + j] != unchecked(i * 397 ^ j))
                        {
                            Assert.Fail($"Invalid item at position [{i},{j}], value was {data.Ptr[(i * size.Width) + j]} instead of {unchecked(i * 397 ^ j)}");
                        }
                    }
                }
            }
        }

#if NETCOREAPP3_1 || NET5_0
        [TestCategory("ParallelHelper")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_ParallelHelper_For2DInvalidRange_FromEnd()
        {
            ParallelHelper.For2D<Assigner2D>(..^1, ..4);
        }

        [TestCategory("ParallelHelper")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_ParallelHelper_For2DInvalidRange_RangeAll()
        {
            ParallelHelper.For2D<Assigner2D>(..5, ..);
        }

        [TestCategory("ParallelHelper")]
        [TestMethod]
        public unsafe void Test_ParallelHelper_For2DWithRanges()
        {
            foreach (var size in TestFor2DSizes)
            {
                using UnmanagedSpanOwner<int> data = new UnmanagedSpanOwner<int>(size.Height * size.Width);

                data.GetSpan().Clear();

                ParallelHelper.For2D(..size.Height, ..size.Width, new Assigner2D(size.Height, size.Width, data.Ptr));

                for (int i = 0; i < size.Height; i++)
                {
                    for (int j = 0; j < size.Width; j++)
                    {
                        if (data.Ptr[(i * size.Width) + j] != unchecked(i * 397 ^ j))
                        {
                            Assert.Fail($"Invalid item at position [{i},{j}], value was {data.Ptr[(i * size.Width) + j]} instead of {unchecked(i * 397 ^ j)}");
                        }
                    }
                }
            }
        }
#endif

        /// <summary>
        /// A type implementing <see cref="IAction"/> to initialize a 2D array
        /// </summary>
        private readonly unsafe struct Assigner2D : IAction2D
        {
            private readonly int height;
            private readonly int width;
            private readonly int* ptr;

            public Assigner2D(int height, int width, int* ptr)
            {
                this.height = height;
                this.width = width;
                this.ptr = ptr;
            }

            /// <inheritdoc/>
            public void Invoke(int i, int j)
            {
                if ((uint)i >= (uint)this.height ||
                    (uint)j >= (uint)this.width)
                {
                    throw new IndexOutOfRangeException($"The target position was invalid, was [{i}, {j}], should've been in [0, {this.height}] and [0, {this.width}]");
                }

                if (this.ptr[(i * this.width) + j] != 0)
                {
                    throw new InvalidOperationException($"Invalid target position [{i},{j}], was {this.ptr[(i * this.width) + j]} instead of 0");
                }

                this.ptr[(i * this.width) + j] = unchecked(i * 397 ^ j);
            }
        }
    }
}