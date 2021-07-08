// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !WINDOWS_UWP

using System;
using System.Runtime.CompilerServices;
using CommunityToolkit.HighPerformance.Enumerables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.HighPerformance.Enumerables
{
    [TestClass]
    public class Test_ReadOnlyRefEnumerable
    {
        [TestCategory("ReadOnlyRefEnumerable")]
        [TestMethod]
        [DataRow(1, 1, new[] { 1 })]
        [DataRow(4, 1, new[] { 1, 2, 3, 4 })]
        [DataRow(4, 4, new[] { 1, 5, 9, 13 })]
        [DataRow(4, 5, new[] { 1, 6, 11, 16 })]
        public void Test_ReadOnlyRefEnumerable_DangerousCreate_Ok(int length, int step, int[] values)
        {
            Span<int> data = new[]
            {
                1, 2, 3, 4,
                5, 6, 7, 8,
                9, 10, 11, 12,
                13, 14, 15, 16
            };

            ReadOnlyRefEnumerable<int> enumerable = ReadOnlyRefEnumerable<int>.DangerousCreate(in data[0], length, step);

            int[] result = enumerable.ToArray();

            CollectionAssert.AreEqual(result, values);
        }

        [TestCategory("ReadOnlyRefEnumerable")]
        [TestMethod]
        [DataRow(-44, 10)]
        [DataRow(10, -14)]
        [DataRow(-32, -1)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_ReadOnlyRefEnumerable_DangerousCreate_BelowZero(int length, int step)
        {
            _ = ReadOnlyRefEnumerable<int>.DangerousCreate(in Unsafe.NullRef<int>(), length, step);
        }

        [TestCategory("ReadOnlyRefEnumerable")]
        [TestMethod]
        [DataRow(1, new[] { 1 })]
        [DataRow(1, new[] { 1, 2, 3, 4 })]
        [DataRow(4, new[] { 1, 5, 9, 13 })]
        [DataRow(5, new[] { 1, 6, 11, 16 })]
        public void Test_ReadOnlyRefEnumerable_Indexer(int step, int[] values)
        {
            Span<int> data = new[]
            {
                1, 2, 3, 4,
                5, 6, 7, 8,
                9, 10, 11, 12,
                13, 14, 15, 16
            };

            ReadOnlyRefEnumerable<int> enumerable = ReadOnlyRefEnumerable<int>.DangerousCreate(in data[0], values.Length, step);

            for (int i = 0; i < enumerable.Length; i++)
            {
                Assert.AreEqual(enumerable[i], values[i]);
            }
        }

        [TestCategory("ReadOnlyRefEnumerable")]
        [TestMethod]
        public void Test_ReadOnlyRefEnumerable_Indexer_ThrowsIndexOutOfRange()
        {
            int[] array = new[]
            {
                0, 0, 0, 0
            };

            Assert.ThrowsException<IndexOutOfRangeException>(() => ReadOnlyRefEnumerable<int>.DangerousCreate(in array[0], array.Length, 1)[-1]);
            Assert.ThrowsException<IndexOutOfRangeException>(() => ReadOnlyRefEnumerable<int>.DangerousCreate(in array[0], array.Length, 1)[array.Length]);
        }

#if NETCOREAPP3_1_OR_GREATER
        [TestCategory("ReadOnlyRefEnumerable")]
        [TestMethod]
        [DataRow(1, new[] { 1 })]
        [DataRow(1, new[] { 1, 2, 3, 4 })]
        [DataRow(4, new[] { 1, 5, 9, 13 })]
        [DataRow(5, new[] { 1, 6, 11, 16 })]
        public void Test_ReadOnlyRefEnumerable_Index_Indexer(int step, int[] values)
        {
            Span<int> data = new[]
            {
                1, 2, 3, 4,
                5, 6, 7, 8,
                9, 10, 11, 12,
                13, 14, 15, 16
            };

            ReadOnlyRefEnumerable<int> enumerable = ReadOnlyRefEnumerable<int>.DangerousCreate(in data[0], values.Length, step);

            for (int i = 1; i <= enumerable.Length; i++)
            {
                Assert.AreEqual(values[^i], enumerable[^i]);
            }
        }

        [TestCategory("ReadOnlyRefEnumerable")]
        [TestMethod]
        public void Test_ReadOnlyRefEnumerable_Index_Indexer_ThrowsIndexOutOfRange()
        {
            int[] array = new[]
            {
                0, 0, 0, 0
            };

            Assert.ThrowsException<IndexOutOfRangeException>(() => ReadOnlyRefEnumerable<int>.DangerousCreate(in array[0], array.Length, 1)[new Index(array.Length)]);
            Assert.ThrowsException<IndexOutOfRangeException>(() => ReadOnlyRefEnumerable<int>.DangerousCreate(in array[0], array.Length, 1)[^0]);
        }
#endif
    }
}

#endif