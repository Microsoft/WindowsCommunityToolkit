// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.HighPerformance.Helpers
{
    public partial class Test_ParallelHelper
    {
        [TestCategory("ParallelHelper")]
        [TestMethod]
        [DataRow(1, 1, 0, 0, 1, 1)]
        [DataRow(1, 2, 0, 0, 1, 2)]
        [DataRow(2, 3, 0, 0, 2, 3)]
        [DataRow(2, 3, 0, 1, 2, 2)]
        [DataRow(3, 3, 1, 1, 2, 2)]
        [DataRow(12, 12, 2, 4, 3, 3)]
        [DataRow(64, 64, 0, 0, 32, 32)]
        [DataRow(64, 64, 13, 14, 23, 22)]
        public unsafe void Test_ParallelHelper_ForEach_In2D(
            int sizeY,
            int sizeX,
            int row,
            int column,
            int height,
            int width)
        {
            int[,] data = CreateRandomData2D(sizeY, sizeX);

            // Create a memory wrapping the random array with the given parameters
            ReadOnlyMemory2D<int> memory = data.AsMemory2D(row, column, height, width);

            Assert.AreEqual(memory.Length, height * width);
            Assert.AreEqual(memory.Height, height);
            Assert.AreEqual(memory.Width, width);

            int sum = 0;

            // Sum all the items in parallel. The Summer type takes a pointer to a target value
            // and adds values to it in a thread-safe manner (with an interlocked add).
            ParallelHelper.ForEach(memory, new Summer(&sum));

            int expected = 0;

            // Calculate the sum iteratively as a baseline for comparison
            foreach (int n in memory.Span)
            {
                expected += n;
            }

            Assert.AreEqual(sum, expected, $"The sum doesn't match, was {sum} instead of {expected}");
        }

        /// <summary>
        /// Creates a random 2D <see cref="int"/> array filled with random numbers.
        /// </summary>
        /// <param name="height">The height of the array to create.</param>
        /// <param name="width">The width of the array to create.</param>
        /// <returns>An array of random <see cref="int"/> elements.</returns>
        [Pure]
        private static int[,] CreateRandomData2D(int height, int width)
        {
            var random = new Random((height * 33) + width);

            int[,] data = new int[height, width];

            foreach (ref int n in data.AsSpan2D())
            {
                n = random.Next(0, byte.MaxValue);
            }

            return data;
        }
    }
}