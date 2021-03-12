// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.Toolkit.HighPerformance.Enumerables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.HighPerformance.Extensions
{
    public partial class Test_ArrayExtensions
    {
        [TestCategory("ArrayExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_2D_DangerousGetReference_Int()
        {
            int[,] array =
            {
                { 1, 2, 3, 4 },
                { 5, 6, 7, 8 },
                { 9, 10, 11, 12 }
            };

            // See comments in Test_ArrayExtensions.1D for how these tests work
            ref int r0 = ref array.DangerousGetReference();
            ref int r1 = ref array[0, 0];

            Assert.IsTrue(Unsafe.AreSame(ref r0, ref r1));
        }

        [TestCategory("ArrayExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_2D_DangerousGetReference_String()
        {
            string[,] array =
            {
                { "a", "bb", "ccc" },
                { "dddd", "eeeee", "ffffff" }
            };

            ref string r0 = ref array.DangerousGetReference();
            ref string r1 = ref array[0, 0];

            Assert.IsTrue(Unsafe.AreSame(ref r0, ref r1));
        }

        [TestCategory("ArrayExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_2D_DangerousGetReferenceAt_Zero()
        {
            int[,] array =
            {
                { 1, 2, 3, 4 },
                { 5, 6, 7, 8 },
                { 9, 10, 11, 12 }
            };

            ref int r0 = ref array.DangerousGetReferenceAt(0, 0);
            ref int r1 = ref array[0, 0];

            Assert.IsTrue(Unsafe.AreSame(ref r0, ref r1));
        }

        [TestCategory("ArrayExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_2D_DangerousGetReferenceAt_Index()
        {
            int[,] array =
            {
                { 1, 2, 3, 4 },
                { 5, 6, 7, 8 },
                { 9, 10, 11, 12 }
            };

            ref int r0 = ref array.DangerousGetReferenceAt(1, 3);
            ref int r1 = ref array[1, 3];

            Assert.IsTrue(Unsafe.AreSame(ref r0, ref r1));
        }

        [TestCategory("ArrayExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_2D_AsSpan2DAndFillArrayMid()
        {
            bool[,] test = new bool[4, 5];

            // To fill an array we now go through the Span2D<T> type, which includes all
            // the necessary logic to perform the operation. In these tests we just create
            // one through the extension, slice it and then fill it. For instance in this
            // one, we're creating a Span2D<bool> from coordinates (1, 1), with a height of
            // 2 and a width of 2, and then filling it. Then we just compare the results.
            test.AsSpan2D(1, 1, 2, 3).Fill(true);

            var expected = new[,]
            {
                { false, false, false, false, false },
                { false,  true,  true,  true, false },
                { false,  true,  true,  true, false },
                { false, false, false, false, false },
            };

            CollectionAssert.AreEqual(expected, test);
        }

        [TestCategory("ArrayExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_2D_AsSpan2DAndFillArrayTwice()
        {
            bool[,] test = new bool[4, 5];

            test.AsSpan2D(0, 0, 2, 1).Fill(true);
            test.AsSpan2D(1, 3, 2, 2).Fill(true);

            var expected = new[,]
            {
                { true,  false, false, false, false },
                { true,  false, false,  true,  true },
                { false, false, false,  true,  true },
                { false, false, false, false, false },
            };

            CollectionAssert.AreEqual(expected, test);
        }

        [TestCategory("ArrayExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_2D_AsSpan2DAndFillArrayBottomEdgeBoundary()
        {
            bool[,] test = new bool[4, 5];

            test.AsSpan2D(1, 2, 3, 2).Fill(true);

            var expected = new[,]
            {
                { false, false, false, false, false },
                { false, false,  true,  true, false },
                { false, false,  true,  true, false },
                { false, false,  true,  true, false },
            };

            CollectionAssert.AreEqual(expected, test);
        }

        [TestCategory("ArrayExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_2D_AsSpan2DAndFillArrayBottomRightCornerBoundary()
        {
            bool[,] test = new bool[5, 4];

            test.AsSpan2D(3, 2, 2, 2).Fill(true);

            var expected = new[,]
            {
                { false, false, false, false },
                { false, false, false, false },
                { false, false, false, false },
                { false, false,  true,  true },
                { false, false,  true,  true },
            };

            CollectionAssert.AreEqual(expected, test);
        }

        [TestCategory("ArrayExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_2D_GetRow_Rectangle()
        {
            int[,] array =
            {
                { 1, 2, 3, 4 },
                { 5, 6, 7, 8 },
                { 9, 10, 11, 12 }
            };

            // Here we use the enumerator on the RefEnumerator<T> type to traverse items in a row
            // by reference. For each one, we check that the reference does in fact point to the
            // item we expect in the underlying array (in this case, items on row 1).
            int j = 0;
            foreach (ref int value in array.GetRow(1))
            {
                Assert.IsTrue(Unsafe.AreSame(ref value, ref array[1, j++]));
            }

            // Check that RefEnumerable<T>.ToArray() works correctly
            CollectionAssert.AreEqual(array.GetRow(1).ToArray(), new[] { 5, 6, 7, 8 });

            // Test an empty array
            Assert.AreSame(new int[1, 0].GetRow(0).ToArray(), Array.Empty<int>());

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => array.GetRow(-1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => array.GetRow(3));

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => array.GetRow(20));
        }

        [TestCategory("ArrayExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_2D_GetColumn_Rectangle()
        {
            int[,] array =
            {
                { 1, 2, 3, 4 },
                { 5, 6, 7, 8 },
                { 9, 10, 11, 12 }
            };

            // Same as above, but this time we iterate a column instead (so non contiguous items)
            int i = 0;
            foreach (ref int value in array.GetColumn(1))
            {
                Assert.IsTrue(Unsafe.AreSame(ref value, ref array[i++, 1]));
            }

            CollectionAssert.AreEqual(array.GetColumn(1).ToArray(), new[] { 2, 6, 10 });

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => array.GetColumn(-1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => array.GetColumn(4));

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => array.GetColumn(20));
        }

        [TestCategory("ArrayExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_2D_GetRow_Empty()
        {
            int[,] array = new int[0, 0];

            // Try to get a row from an empty array (the row index isn't in range)
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => array.GetRow(0).ToArray());
        }

        [TestCategory("ArrayExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_2D_GetRowOrColumn_Helpers()
        {
            int[,] array =
            {
                { 1, 2, 3, 4 },
                { 5, 6, 7, 8 },
                { 9, 10, 11, 12 },
                { 13, 14, 15, 16 }
            };

            // Get a row and test the Clear method. Note that the Span2D<T> here is sliced
            // starting from the second column, so this method should clear the row from index 1.
            array.AsSpan2D(1, 1, 3, 3).GetRow(0).Clear();

            int[,] expected =
            {
                { 1, 2, 3, 4 },
                { 5, 0, 0, 0 },
                { 9, 10, 11, 12 },
                { 13, 14, 15, 16 }
            };

            CollectionAssert.AreEqual(array, expected);

            // Same as before, but this time we fill a column with a value
            array.GetColumn(2).Fill(42);

            expected = new[,]
            {
                { 1, 2, 42, 4 },
                { 5, 0, 42, 0 },
                { 9, 10, 42, 12 },
                { 13, 14, 42, 16 }
            };

            CollectionAssert.AreEqual(array, expected);

            int[] copy = new int[4];

            // Get a row and copy items to a target span (in this case, wrapping an array)
            array.GetRow(2).CopyTo(copy);

            int[] result = { 9, 10, 42, 12 };

            CollectionAssert.AreEqual(copy, result);

            // Same as above, but copying from a column (so we test non contiguous sequences too)
            array.GetColumn(1).CopyTo(copy);

            result = new[] { 2, 0, 10, 14 };

            CollectionAssert.AreEqual(copy, result);

            // Some invalid attempts to copy to an empty span or sequence
            Assert.ThrowsException<ArgumentException>(() => array.GetRow(0).CopyTo(default(RefEnumerable<int>)));
            Assert.ThrowsException<ArgumentException>(() => array.GetRow(0).CopyTo(default(Span<int>)));

            Assert.ThrowsException<ArgumentException>(() => array.GetColumn(0).CopyTo(default(RefEnumerable<int>)));
            Assert.ThrowsException<ArgumentException>(() => array.GetColumn(0).CopyTo(default(Span<int>)));

            // Same as CopyTo, but this will fail gracefully with an invalid target
            Assert.IsTrue(array.GetRow(2).TryCopyTo(copy));
            Assert.IsFalse(array.GetRow(0).TryCopyTo(default(Span<int>)));

            result = new[] { 9, 10, 42, 12 };

            CollectionAssert.AreEqual(copy, result);

            // Also fill a row and then further down clear a column (trying out all possible combinations)
            array.GetRow(2).Fill(99);

            expected = new[,]
            {
                { 1, 2, 42, 4 },
                { 5, 0, 42, 0 },
                { 99, 99, 99, 99 },
                { 13, 14, 42, 16 }
            };

            CollectionAssert.AreEqual(array, expected);

            array.GetColumn(2).Clear();

            expected = new[,]
            {
                { 1, 2, 0, 4 },
                { 5, 0, 0, 0 },
                { 99, 99, 0, 99 },
                { 13, 14, 0, 16 }
            };

            CollectionAssert.AreEqual(array, expected);
        }

        [TestCategory("ArrayExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_2D_ReadOnlyGetRowOrColumn_Helpers()
        {
            int[,] array =
            {
                { 1, 2, 3, 4 },
                { 5, 6, 7, 8 },
                { 9, 10, 11, 12 },
                { 13, 14, 15, 16 }
            };

            // This test pretty much does the same things as the method above, but this time
            // using a source ReadOnlySpan2D<T>, so that the sequence type being tested is
            // ReadOnlyRefEnumerable<T> instead (which shares most features but is separate).
            ReadOnlySpan2D<int> span2D = array;

            int[] copy = new int[4];

            span2D.GetRow(2).CopyTo(copy);

            int[] result = { 9, 10, 11, 12 };

            CollectionAssert.AreEqual(copy, result);

            span2D.GetColumn(1).CopyTo(copy);

            result = new[] { 2, 6, 10, 14 };

            CollectionAssert.AreEqual(copy, result);

            Assert.ThrowsException<ArgumentException>(() => ((ReadOnlySpan2D<int>)array).GetRow(0).CopyTo(default(RefEnumerable<int>)));
            Assert.ThrowsException<ArgumentException>(() => ((ReadOnlySpan2D<int>)array).GetRow(0).CopyTo(default(Span<int>)));

            Assert.ThrowsException<ArgumentException>(() => ((ReadOnlySpan2D<int>)array).GetColumn(0).CopyTo(default(RefEnumerable<int>)));
            Assert.ThrowsException<ArgumentException>(() => ((ReadOnlySpan2D<int>)array).GetColumn(0).CopyTo(default(Span<int>)));

            Assert.IsTrue(span2D.GetRow(2).TryCopyTo(copy));
            Assert.IsFalse(span2D.GetRow(2).TryCopyTo(default(Span<int>)));

            result = new[] { 9, 10, 11, 12 };

            CollectionAssert.AreEqual(copy, result);
        }

        [TestCategory("ArrayExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_2D_RefEnumerable_Misc()
        {
            int[,] array1 =
            {
                { 1, 2, 3, 4 },
                { 5, 6, 7, 8 },
                { 9, 10, 11, 12 },
                { 13, 14, 15, 16 }
            };

            int[,] array2 = new int[4, 4];

            // Copy to enumerable with source step == 1, destination step == 1
            array1.GetRow(0).CopyTo(array2.GetRow(0));

            // Copy enumerable with source step == 1, destination step != 1
            array1.GetRow(1).CopyTo(array2.GetColumn(1));

            // Copy enumerable with source step != 1, destination step == 1
            array1.GetColumn(2).CopyTo(array2.GetRow(2));

            // Copy enumerable with source step != 1, destination step != 1
            array1.GetColumn(3).CopyTo(array2.GetColumn(3));

            int[,] result =
            {
                { 1, 5, 3, 4 },
                { 0, 6, 0, 8 },
                { 3, 7, 11, 12 },
                { 0, 8, 0, 16 }
            };

            CollectionAssert.AreEqual(array2, result);

            // Test a valid and an invalid TryCopyTo call with the RefEnumerable<T> overload
            bool shouldBeTrue = array1.GetRow(0).TryCopyTo(array2.GetColumn(0));
            bool shouldBeFalse = array1.GetRow(0).TryCopyTo(default(RefEnumerable<int>));

            result = new[,]
            {
                { 1, 5, 3, 4 },
                { 2, 6, 0, 8 },
                { 3, 7, 11, 12 },
                { 4, 8, 0, 16 }
            };

            CollectionAssert.AreEqual(array2, result);

            Assert.IsTrue(shouldBeTrue);
            Assert.IsFalse(shouldBeFalse);
        }

        [TestCategory("ArrayExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_2D_GetColumn_Empty()
        {
            int[,] array = new int[0, 0];

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => array.GetColumn(0).ToArray());
        }

#if NETCOREAPP3_1 || NET5_0
        [TestCategory("ArrayExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_2D_AsSpan_Empty()
        {
            int[,] array = new int[0, 0];

            Span<int> span = array.AsSpan();

            // Check that the empty array was loaded properly
            Assert.AreEqual(span.Length, array.Length);
            Assert.IsTrue(span.IsEmpty);
        }

        [TestCategory("ArrayExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_2D_AsSpan_Populated()
        {
            int[,] array =
            {
                { 1, 2, 3, 4 },
                { 5, 6, 7, 8 },
                { 9, 10, 11, 12 }
            };

            Span<int> span = array.AsSpan();

            // Test the total length of the span
            Assert.AreEqual(span.Length, array.Length);

            ref int r0 = ref array[0, 0];
            ref int r1 = ref span[0];

            // Similarly to the top methods, here we compare a given reference to
            // ensure they point to the right element back in the original array.
            Assert.IsTrue(Unsafe.AreSame(ref r0, ref r1));
        }
#endif
    }
}
