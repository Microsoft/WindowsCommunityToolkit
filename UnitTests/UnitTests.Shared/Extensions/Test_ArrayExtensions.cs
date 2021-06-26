// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Toolkit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Extensions
{
    [TestClass]
    public class Test_ArrayExtensions
    {
        [TestCategory("ArrayExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_Jagged_GetColumn()
        {
            int[][] array =
            {
                new int[] { 5, 2, 4 },
                new int[] { 6, 3 },
                new int[] { 7 }
            };

            var col = array.GetColumn(1).ToArray();

            CollectionAssert.AreEquivalent(new int[] { 2, 3, 0 }, col);
        }

        [TestCategory("ArrayExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_Jagged_GetColumn_Exception()
        {
            int[][] array =
            {
                new int[] { 5, 2, 4 },
                new int[] { 6, 3 },
                new int[] { 7 }
            };

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                array.GetColumn(-1).ToArray();
            });

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                array.GetColumn(3).ToArray();
            });
        }

        [TestCategory("ArrayExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_Rectangular_ToString()
        {
            int[,] array =
            {
                { 5, 2,  4 },
                { 6, 3, -1 },
                { 7, 0,  9 }
            };

            string value = array.ToArrayString();

            Debug.WriteLine(value);

            Assert.AreEqual("[[5,\t2,\t4]," + Environment.NewLine + " [6,\t3,\t-1]," + Environment.NewLine + " [7,\t0,\t9]]", value);
        }

        [TestCategory("ArrayExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_Jagged_ToString()
        {
            int[][] array =
            {
                new int[] { 5, 2 },
                new int[] { 6, 3, -1, 2 },
                new int[] { 7, 0,  9 }
            };

            string value = array.ToArrayString();

            Debug.WriteLine(value);

            Assert.AreEqual("[[5,\t2]," + Environment.NewLine + " [6,\t3,\t-1,\t2]," + Environment.NewLine + " [7,\t0,\t9]]", value);
        }
    }
}