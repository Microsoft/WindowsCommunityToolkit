// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        public void Test_ParallelHelper_ForEach_Ref2D(
            int sizeY,
            int sizeX,
            int row,
            int column,
            int height,
            int width)
        {
            int[,]
                data = CreateRandomData2D(sizeY, sizeX),
                copy = (int[,])data.Clone();

            // Prepare the target data iteratively
            foreach (ref int n in copy.AsSpan2D(row, column, height, width))
            {
                n = unchecked(n * 397);
            }

            Memory2D<int> memory = data.AsMemory2D(row, column, height, width);

            Assert.AreEqual(memory.Length, height * width);
            Assert.AreEqual(memory.Height, height);
            Assert.AreEqual(memory.Width, width);

            // Do the same computation in paralellel, then compare the two arrays
            ParallelHelper.ForEach(memory, new Multiplier(397));

            CollectionAssert.AreEqual(data, copy);
        }
    }
}