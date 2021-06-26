// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.Toolkit.HighPerformance.Enumerables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.HighPerformance
{
    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649", Justification = "Test class for generic type")]
    public class Test_Span2DT
    {
        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_Empty()
        {
            // Like in the tests for Memory2D<T>, here we validate a number of empty spans
            Span2D<int> empty1 = default;

            Assert.IsTrue(empty1.IsEmpty);
            Assert.AreEqual(empty1.Length, 0);
            Assert.AreEqual(empty1.Width, 0);
            Assert.AreEqual(empty1.Height, 0);

            Span2D<string> empty2 = Span2D<string>.Empty;

            Assert.IsTrue(empty2.IsEmpty);
            Assert.AreEqual(empty2.Length, 0);
            Assert.AreEqual(empty2.Width, 0);
            Assert.AreEqual(empty2.Height, 0);

            Span2D<int> empty3 = new int[4, 0];

            Assert.IsTrue(empty3.IsEmpty);
            Assert.AreEqual(empty3.Length, 0);
            Assert.AreEqual(empty3.Width, 0);
            Assert.AreEqual(empty3.Height, 4);

            Span2D<int> empty4 = new int[0, 7];

            Assert.IsTrue(empty4.IsEmpty);
            Assert.AreEqual(empty4.Length, 0);
            Assert.AreEqual(empty4.Width, 7);
            Assert.AreEqual(empty4.Height, 0);
        }

#if !WINDOWS_UWP
        [TestCategory("Span2DT")]
        [TestMethod]
        public unsafe void Test_Span2DT_RefConstructor()
        {
            Span<int> span = stackalloc[]
            {
                1, 2, 3, 4, 5, 6
            };

            // Test for a Span2D<T> instance created from a target reference. This is only supported
            // on runtimes with fast Span<T> support (as we need the API to power this with just a ref).
            Span2D<int> span2d = Span2D<int>.DangerousCreate(ref span[0], 2, 3, 0);

            Assert.IsFalse(span2d.IsEmpty);
            Assert.AreEqual(span2d.Length, 6);
            Assert.AreEqual(span2d.Width, 3);
            Assert.AreEqual(span2d.Height, 2);

            span2d[0, 0] = 99;
            span2d[1, 2] = 101;

            // Validate that those values were mapped to the right spot in the target span
            Assert.AreEqual(span[0], 99);
            Assert.AreEqual(span[5], 101);

            // A few cases with invalid indices
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Span2D<int>.DangerousCreate(ref Unsafe.AsRef<int>(null), -1, 0, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Span2D<int>.DangerousCreate(ref Unsafe.AsRef<int>(null), 1, -2, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Span2D<int>.DangerousCreate(ref Unsafe.AsRef<int>(null), 1, 0, -5));
        }
#endif

        [TestCategory("Span2DT")]
        [TestMethod]
        public unsafe void Test_Span2DT_PtrConstructor()
        {
            int* ptr = stackalloc[]
            {
                1,
                2,
                3,
                4,
                5,
                6
            };

            // Same as above, but creating a Span2D<T> from a raw pointer
            Span2D<int> span2d = new Span2D<int>(ptr, 2, 3, 0);

            Assert.IsFalse(span2d.IsEmpty);
            Assert.AreEqual(span2d.Length, 6);
            Assert.AreEqual(span2d.Width, 3);
            Assert.AreEqual(span2d.Height, 2);

            span2d[0, 0] = 99;
            span2d[1, 2] = 101;

            Assert.AreEqual(ptr[0], 99);
            Assert.AreEqual(ptr[5], 101);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>((void*)0, -1, 0, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>((void*)0, 1, -2, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>((void*)0, 1, 0, -5));
            Assert.ThrowsException<ArgumentException>(() => new Span2D<string>((void*)0, 2, 2, 0));
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_Array1DConstructor()
        {
            int[] array =
            {
                1, 2, 3, 4, 5, 6
            };

            // Same as above, but wrapping a 1D array with data in row-major order
            Span2D<int> span2d = new Span2D<int>(array, 1, 2, 2, 1);

            Assert.IsFalse(span2d.IsEmpty);
            Assert.AreEqual(span2d.Length, 4);
            Assert.AreEqual(span2d.Width, 2);
            Assert.AreEqual(span2d.Height, 2);

            span2d[0, 0] = 99;
            span2d[1, 1] = 101;

            Assert.AreEqual(array[1], 99);
            Assert.AreEqual(array[5], 101);

            // The first check fails due to the array covariance test mentioned in the Memory2D<T> tests.
            // The others just validate a number of cases with invalid arguments (eg. out of range).
            Assert.ThrowsException<ArrayTypeMismatchException>(() => new Span2D<object>(new string[1], 1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array, -99, 1, 1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array, 0, -10, 1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array, 0, 1, 1, -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array, 0, 1, -100, 1));
            Assert.ThrowsException<ArgumentException>(() => new Span2D<int>(array, 0, 10, 1, 120));
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_Array2DConstructor_1()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            // Same as above, but directly wrapping a 2D array
            Span2D<int> span2d = new Span2D<int>(array);

            Assert.IsFalse(span2d.IsEmpty);
            Assert.AreEqual(span2d.Length, 6);
            Assert.AreEqual(span2d.Width, 3);
            Assert.AreEqual(span2d.Height, 2);

            span2d[0, 1] = 99;
            span2d[1, 2] = 101;

            Assert.AreEqual(array[0, 1], 99);
            Assert.AreEqual(array[1, 2], 101);

            Assert.ThrowsException<ArrayTypeMismatchException>(() => new Span2D<object>(new string[1, 2]));
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_Array2DConstructor_2()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            // Same as above, but with a custom slicing over the target 2D array
            Span2D<int> span2d = new Span2D<int>(array, 0, 1, 2, 2);

            Assert.IsFalse(span2d.IsEmpty);
            Assert.AreEqual(span2d.Length, 4);
            Assert.AreEqual(span2d.Width, 2);
            Assert.AreEqual(span2d.Height, 2);

            span2d[0, 0] = 99;
            span2d[1, 1] = 101;

            Assert.AreEqual(array[0, 1], 99);
            Assert.AreEqual(array[1, 2], 101);

            Assert.ThrowsException<ArrayTypeMismatchException>(() => new Span2D<object>(new string[1, 2], 0, 0, 2, 2));
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_Array3DConstructor_1()
        {
            int[,,] array =
            {
                {
                    { 1, 2, 3 },
                    { 4, 5, 6 }
                },
                {
                    { 10, 20, 30 },
                    { 40, 50, 60 }
                }
            };

            // Here we wrap a layer in a 3D array instead, the rest is the same
            Span2D<int> span2d = new Span2D<int>(array, 1);

            Assert.IsFalse(span2d.IsEmpty);
            Assert.AreEqual(span2d.Length, 6);
            Assert.AreEqual(span2d.Width, 3);
            Assert.AreEqual(span2d.Height, 2);

            span2d[0, 1] = 99;
            span2d[1, 2] = 101;

            Assert.AreEqual(span2d[0, 0], 10);
            Assert.AreEqual(array[1, 0, 1], 99);
            Assert.AreEqual(array[1, 1, 2], 101);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array, -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array, 20));
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_Array3DConstructor_2()
        {
            int[,,] array =
            {
                {
                    { 1, 2, 3 },
                    { 4, 5, 6 }
                },
                {
                    { 10, 20, 30 },
                    { 40, 50, 60 }
                }
            };

            // Same as above, but also slicing a target 2D area in the 3D array layer
            Span2D<int> span2d = new Span2D<int>(array, 1, 0, 1, 2, 2);

            Assert.IsFalse(span2d.IsEmpty);
            Assert.AreEqual(span2d.Length, 4);
            Assert.AreEqual(span2d.Width, 2);
            Assert.AreEqual(span2d.Height, 2);

            span2d[0, 1] = 99;
            span2d[1, 1] = 101;

            Assert.AreEqual(span2d[0, 0], 20);
            Assert.AreEqual(array[1, 0, 2], 99);
            Assert.AreEqual(array[1, 1, 2], 101);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array, -1, 1, 1, 1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array, 1, -1, 1, 1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array, 1, 1, -1, 1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array, 1, 1, 1, -1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array, 1, 1, 1, 1, -1));
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_FillAndClear_1()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            // Tests for the Fill and Clear APIs for Span2D<T>. These should fill
            // or clear the entire wrapped 2D array (just like eg. Span<T>.Fill).
            Span2D<int> span2d = new Span2D<int>(array);

            span2d.Fill(42);

            Assert.IsTrue(array.Cast<int>().All(n => n == 42));

            span2d.Clear();

            Assert.IsTrue(array.Cast<int>().All(n => n == 0));
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_Fill_Empty()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            // Same as above, but with an initial slicing as well to ensure
            // these method work correctly with different internal offsets
            Span2D<int> span2d = new Span2D<int>(array, 0, 0, 0, 0);

            span2d.Fill(42);

            CollectionAssert.AreEqual(array, array);

            span2d.Clear();

            CollectionAssert.AreEqual(array, array);
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_FillAndClear_2()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            // Same as above, just with different slicing to a target smaller 2D area
            Span2D<int> span2d = new Span2D<int>(array, 0, 1, 2, 2);

            span2d.Fill(42);

            int[,] filled =
            {
                { 1, 42, 42 },
                { 4, 42, 42 }
            };

            CollectionAssert.AreEqual(array, filled);

            span2d.Clear();

            int[,] cleared =
            {
                { 1, 0, 0 },
                { 4, 0, 0 }
            };

            CollectionAssert.AreEqual(array, cleared);
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_CopyTo_Empty()
        {
            Span2D<int> span2d = Span2D<int>.Empty;

            int[] target = new int[0];

            // Copying an emoty Span2D<T> to an empty array is just a no-op
            span2d.CopyTo(target);
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_CopyTo_1()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            Span2D<int> span2d = new Span2D<int>(array);

            int[] target = new int[array.Length];

            // Here we copy a Span2D<T> to a target Span<T> mapping an array.
            // This is valid, and the data will just be copied in row-major order.
            span2d.CopyTo(target);

            CollectionAssert.AreEqual(array, target);

            // Exception due to the target span being too small for the source Span2D<T> instance
            Assert.ThrowsException<ArgumentException>(() => new Span2D<int>(array).CopyTo(Span<int>.Empty));
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_CopyTo_2()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            // Same as above, but with different initial slicing
            Span2D<int> span2d = new Span2D<int>(array, 0, 1, 2, 2);

            int[] target = new int[4];

            span2d.CopyTo(target);

            int[] expected = { 2, 3, 5, 6 };

            CollectionAssert.AreEqual(target, expected);

            Assert.ThrowsException<ArgumentException>(() => new Span2D<int>(array).CopyTo(Span<int>.Empty));
            Assert.ThrowsException<ArgumentException>(() => new Span2D<int>(array, 0, 1, 2, 2).CopyTo(Span<int>.Empty));
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_CopyTo2D_1()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            Span2D<int> span2d = new Span2D<int>(array);

            int[,] target = new int[2, 3];

            // Same as above, but copying to a target Span2D<T> instead. Note
            // that this method uses the implicit T[,] to Span2D<T> conversion.
            span2d.CopyTo(target);

            CollectionAssert.AreEqual(array, target);

            Assert.ThrowsException<ArgumentException>(() => new Span2D<int>(array).CopyTo(Span2D<int>.Empty));
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_CopyTo2D_2()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            // Same as above, but with extra initial slicing
            Span2D<int> span2d = new Span2D<int>(array, 0, 1, 2, 2);

            int[,] target = new int[2, 2];

            span2d.CopyTo(target);

            int[,] expected =
            {
                { 2, 3 },
                { 5, 6 }
            };

            CollectionAssert.AreEqual(target, expected);

            Assert.ThrowsException<ArgumentException>(() => new Span2D<int>(array).CopyTo(new Span2D<int>(target)));
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_TryCopyTo()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            Span2D<int> span2d = new Span2D<int>(array);

            int[] target = new int[array.Length];

            // Here we test the safe TryCopyTo method, which will fail gracefully
            Assert.IsTrue(span2d.TryCopyTo(target));
            Assert.IsFalse(span2d.TryCopyTo(Span<int>.Empty));

            int[] expected = { 1, 2, 3, 4, 5, 6 };

            CollectionAssert.AreEqual(target, expected);
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_TryCopyTo2D()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            // Same as above, but copying to a 2D array with the safe TryCopyTo method
            Span2D<int> span2d = new Span2D<int>(array);

            int[,] target = new int[2, 3];

            Assert.IsTrue(span2d.TryCopyTo(target));
            Assert.IsFalse(span2d.TryCopyTo(Span2D<int>.Empty));

            int[,] expected =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            CollectionAssert.AreEqual(target, expected);
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public unsafe void Test_Span2DT_GetPinnableReference()
        {
            // Here we test that a ref from an empty Span2D<T> returns a null ref
            Assert.IsTrue(Unsafe.AreSame(
                ref Unsafe.AsRef<int>(null),
                ref Span2D<int>.Empty.GetPinnableReference()));

            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            Span2D<int> span2d = new Span2D<int>(array);

            ref int r0 = ref span2d.GetPinnableReference();

            // Here we test that GetPinnableReference returns a ref to the first array element
            Assert.IsTrue(Unsafe.AreSame(ref r0, ref array[0, 0]));
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public unsafe void Test_Span2DT_DangerousGetReference()
        {
            // Same as above, but using DangerousGetReference instead (faster, no conditional check)
            Assert.IsTrue(Unsafe.AreSame(
                ref Unsafe.AsRef<int>(null),
                ref Span2D<int>.Empty.DangerousGetReference()));

            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            Span2D<int> span2d = new Span2D<int>(array);

            ref int r0 = ref span2d.DangerousGetReference();

            Assert.IsTrue(Unsafe.AreSame(ref r0, ref array[0, 0]));
        }

#if NETCOREAPP3_1_OR_GREATER
        [TestCategory("Span2DT")]
        [TestMethod]
        public unsafe void Test_Span2DT_Index_Indexer_1()
        {
            int[,] array = new int[4, 4];

            Span2D<int> span2d = new Span2D<int>(array);

            ref int arrayRef = ref array[1, 3];
            ref int span2dRef = ref span2d[1, ^1];

            Assert.IsTrue(Unsafe.AreSame(ref arrayRef, ref span2dRef));
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public unsafe void Test_Span2DT_Index_Indexer_2()
        {
            int[,] array = new int[4, 4];

            Span2D<int> span2d = new Span2D<int>(array);

            ref int arrayRef = ref array[2, 1];
            ref int span2dRef = ref span2d[^2, ^3];

            Assert.IsTrue(Unsafe.AreSame(ref arrayRef, ref span2dRef));
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public unsafe void Test_Span2DT_Index_Indexer_Fail()
        {
            int[,] array = new int[4, 4];

            Span2D<int> span2d = new Span2D<int>(array);

            ref int span2dRef = ref span2d[^6, 2];
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public unsafe void Test_Span2DT_Range_Indexer_1()
        {
            int[,] array = new int[4, 4];

            Span2D<int> span2d = new Span2D<int>(array);
            Span2D<int> slice = span2d[1.., 1..];

            Assert.AreEqual(slice.Length, 9);
            Assert.IsTrue(Unsafe.AreSame(ref array[1, 1], ref slice[0, 0]));
            Assert.IsTrue(Unsafe.AreSame(ref array[3, 3], ref slice[2, 2]));
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public unsafe void Test_Span2DT_Range_Indexer_2()
        {
            int[,] array = new int[4, 4];

            Span2D<int> span2d = new Span2D<int>(array);
            Span2D<int> slice = span2d[0..^2, 1..^1];

            Assert.AreEqual(slice.Length, 4);
            Assert.IsTrue(Unsafe.AreSame(ref array[0, 1], ref slice[0, 0]));
            Assert.IsTrue(Unsafe.AreSame(ref array[1, 2], ref slice[1, 1]));
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public unsafe void Test_Span2DT_Range_Indexer_Fail()
        {
            int[,] array = new int[4, 4];

            Span2D<int> span2d = new Span2D<int>(array);
            _ = span2d[0..6, 2..^1];

            Assert.Fail();
        }
#endif

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_Slice_1()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            // Here we have a number of tests that just take an initial 2D array, create a Span2D<T>,
            // perform a number of slicing operations and then validate the parameters for the resulting
            // instances, and that the indexer works correctly and maps to the right original elements.
            Span2D<int> span2d = new Span2D<int>(array);

            Span2D<int> slice1 = span2d.Slice(1, 1, 1, 2);

            Assert.AreEqual(slice1.Length, 2);
            Assert.AreEqual(slice1.Height, 1);
            Assert.AreEqual(slice1.Width, 2);
            Assert.AreEqual(slice1[0, 0], 5);
            Assert.AreEqual(slice1[0, 1], 6);

            Span2D<int> slice2 = span2d.Slice(0, 1, 2, 2);

            Assert.AreEqual(slice2.Length, 4);
            Assert.AreEqual(slice2.Height, 2);
            Assert.AreEqual(slice2.Width, 2);
            Assert.AreEqual(slice2[0, 0], 2);
            Assert.AreEqual(slice2[1, 0], 5);
            Assert.AreEqual(slice2[1, 1], 6);

            // Some checks for invalid arguments
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array).Slice(-1, 1, 1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array).Slice(1, -1, 1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array).Slice(1, 1, 1, -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array).Slice(1, 1, -1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array).Slice(10, 1, 1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array).Slice(1, 12, 1, 12));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array).Slice(1, 1, 55, 1));
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_Slice_2()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            Span2D<int> span2d = new Span2D<int>(array);

            // Same as above, but with some different slicing
            Span2D<int> slice1 = span2d.Slice(0, 0, 2, 2);

            Assert.AreEqual(slice1.Length, 4);
            Assert.AreEqual(slice1.Height, 2);
            Assert.AreEqual(slice1.Width, 2);
            Assert.AreEqual(slice1[0, 0], 1);
            Assert.AreEqual(slice1[1, 1], 5);

            Span2D<int> slice2 = slice1.Slice(1, 0, 1, 2);

            Assert.AreEqual(slice2.Length, 2);
            Assert.AreEqual(slice2.Height, 1);
            Assert.AreEqual(slice2.Width, 2);
            Assert.AreEqual(slice2[0, 0], 4);
            Assert.AreEqual(slice2[0, 1], 5);

            Span2D<int> slice3 = slice2.Slice(0, 1, 1, 1);

            Assert.AreEqual(slice3.Length, 1);
            Assert.AreEqual(slice3.Height, 1);
            Assert.AreEqual(slice3.Width, 1);
            Assert.AreEqual(slice3[0, 0], 5);
        }

#if !WINDOWS_UWP
        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_GetRowSpan()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            Span2D<int> span2d = new Span2D<int>(array);

            // Here we create a Span2D<T> from a 2D array and want to get a Span<T> from
            // a specific row. This is only supported on runtimes with fast Span<T> support
            // for the same reason mentioned in the Memory2D<T> tests (we need the Span<T>
            // constructor that only takes a target ref). Then we just get some references
            // to items in this span and compare them against references into the original
            // 2D array to ensure they match and point to the correct elements from there.
            Span<int> span = span2d.GetRowSpan(1);

            Assert.IsTrue(Unsafe.AreSame(
                ref span[0],
                ref array[1, 0]));
            Assert.IsTrue(Unsafe.AreSame(
                ref span[2],
                ref array[1, 2]));

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array).GetRowSpan(-1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array).GetRowSpan(5));
        }
#endif

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_TryGetSpan_From1DArray_1()
        {
            int[] array = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            Span2D<int> span2d = new Span2D<int>(array, 3, 3);

            bool success = span2d.TryGetSpan(out Span<int> span);

            Assert.IsTrue(success);
            Assert.AreEqual(span.Length, span2d.Length);
            Assert.IsTrue(Unsafe.AreSame(ref array[0], ref span[0]));
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_TryGetSpan_From1DArray_2()
        {
            int[] array = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            Span2D<int> span2d = new Span2D<int>(array, 3, 3).Slice(1, 0, 2, 3);

            bool success = span2d.TryGetSpan(out Span<int> span);

            Assert.IsTrue(success);
            Assert.AreEqual(span.Length, span2d.Length);
            Assert.IsTrue(Unsafe.AreSame(ref array[3], ref span[0]));
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_TryGetSpan_From1DArray_3()
        {
            int[] array = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            Span2D<int> span2d = new Span2D<int>(array, 3, 3).Slice(0, 1, 3, 2);

            bool success = span2d.TryGetSpan(out Span<int> span);

            Assert.IsFalse(success);
            Assert.AreEqual(span.Length, 0);
        }

        // See https://github.com/windows-toolkit/WindowsCommunityToolkit/issues/3947
        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_TryGetSpan_From1DArray_4()
        {
            int[] array = new int[128];
            Span2D<int> span2d = new Span2D<int>(array, 8, 16);

            bool success = span2d.TryGetSpan(out Span<int> span);

            Assert.IsTrue(success);
            Assert.AreEqual(span.Length, span2d.Length);
            Assert.IsTrue(Unsafe.AreSame(ref array[0], ref span[0]));
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_TryGetSpan_From2DArray_1()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            Span2D<int> span2d = new Span2D<int>(array);

            // This API tries to get a Span<T> for the entire contents of Span2D<T>.
            // This only works on runtimes if the underlying data is contiguous
            // and of a size that can fit into a single Span<T>. In this specific test,
            // this is not expected to work on UWP because it can't create a Span<T>
            // from a 2D array (reasons explained in the comments for the test above).
            bool success = span2d.TryGetSpan(out Span<int> span);

#if WINDOWS_UWP
            // Can't get a Span<T> over a T[,] array on UWP
            Assert.IsFalse(success);
            Assert.AreEqual(span.Length, 0);
#else
            Assert.IsTrue(success);
            Assert.AreEqual(span.Length, span2d.Length);
#endif
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_TryGetSpan_From2DArray_2()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            // Same as above, but this will always fail because we're creating
            // a Span2D<T> wrapping non contiguous data (the pitch is not 0).
            Span2D<int> span2d = new Span2D<int>(array, 0, 0, 2, 2);

            bool success = span2d.TryGetSpan(out Span<int> span);

            Assert.IsFalse(success);
            Assert.IsTrue(span.IsEmpty);
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_ToArray_1()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            // Here we create a Span2D<T> and verify that ToArray() produces
            // a 2D array that is identical to the original one being wrapped.
            Span2D<int> span2d = new Span2D<int>(array);

            int[,] copy = span2d.ToArray();

            Assert.AreEqual(copy.GetLength(0), array.GetLength(0));
            Assert.AreEqual(copy.GetLength(1), array.GetLength(1));

            CollectionAssert.AreEqual(array, copy);
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_ToArray_2()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            // Same as above, but with extra initial slicing
            Span2D<int> span2d = new Span2D<int>(array, 0, 0, 2, 2);

            int[,] copy = span2d.ToArray();

            Assert.AreEqual(copy.GetLength(0), 2);
            Assert.AreEqual(copy.GetLength(1), 2);

            int[,] expected =
            {
                { 1, 2 },
                { 4, 5 }
            };

            CollectionAssert.AreEqual(expected, copy);
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Test_Span2DT_Equals()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            Span2D<int> span2d = new Span2D<int>(array);

            // Span2D<T>.Equals always throw (this mirrors the behavior of Span<T>.Equals)
            _ = span2d.Equals(null);
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Test_Span2DT_GetHashCode()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            Span2D<int> span2d = new Span2D<int>(array);

            // Same as above, this always throws
            _ = span2d.GetHashCode();
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_ToString()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            Span2D<int> span2d = new Span2D<int>(array);

            // Verify that we get the nicely formatted string
            string text = span2d.ToString();

            const string expected = "Microsoft.Toolkit.HighPerformance.Span2D<System.Int32>[2, 3]";

            Assert.AreEqual(text, expected);
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_opEquals()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            // Create two Span2D<T> instances wrapping the same array with the same
            // parameters, and verify that the equality operators work correctly.
            Span2D<int> span2d_1 = new Span2D<int>(array);
            Span2D<int> span2d_2 = new Span2D<int>(array);

            Assert.IsTrue(span2d_1 == span2d_2);
            Assert.IsFalse(span2d_1 == Span2D<int>.Empty);
            Assert.IsTrue(Span2D<int>.Empty == Span2D<int>.Empty);

            // Same as above, but verify that a sliced span is not reported as equal
            Span2D<int> span2d_3 = new Span2D<int>(array, 0, 0, 2, 2);

            Assert.IsFalse(span2d_1 == span2d_3);
            Assert.IsFalse(span2d_3 == Span2D<int>.Empty);
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_ImplicitCast()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            // Verify that an explicit constructor and the implicit conversion
            // operator generate an identical Span2D<T> instance from the array.
            Span2D<int> span2d_1 = array;
            Span2D<int> span2d_2 = new Span2D<int>(array);

            Assert.IsTrue(span2d_1 == span2d_2);
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_GetRow()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            // Get a target row and verify the contents match with our data
            RefEnumerable<int> enumerable = new Span2D<int>(array).GetRow(1);

            int[] expected = { 4, 5, 6 };

            CollectionAssert.AreEqual(enumerable.ToArray(), expected);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array).GetRow(-1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array).GetRow(2));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array).GetRow(1000));
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public unsafe void Test_Span2DT_Pointer_GetRow()
        {
            int* array = stackalloc[]
            {
                1, 2, 3,
                4, 5, 6
            };

            // Same as above, but with a Span2D<T> wrapping a raw pointer
            RefEnumerable<int> enumerable = new Span2D<int>(array, 2, 3, 0).GetRow(1);

            int[] expected = { 4, 5, 6 };

            CollectionAssert.AreEqual(enumerable.ToArray(), expected);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array, 2, 3, 0).GetRow(-1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array, 2, 3, 0).GetRow(2));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array, 2, 3, 0).GetRow(1000));
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_GetColumn()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            // Same as above, but getting a column instead
            RefEnumerable<int> enumerable = new Span2D<int>(array).GetColumn(2);

            int[] expected = { 3, 6 };

            CollectionAssert.AreEqual(enumerable.ToArray(), expected);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array).GetColumn(-1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array).GetColumn(3));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array).GetColumn(1000));
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public unsafe void Test_Span2DT_Pointer_GetColumn()
        {
            int* array = stackalloc[]
            {
                1, 2, 3,
                4, 5, 6
            };

            // Same as above, but wrapping a raw pointer
            RefEnumerable<int> enumerable = new Span2D<int>(array, 2, 3, 0).GetColumn(2);

            int[] expected = { 3, 6 };

            CollectionAssert.AreEqual(enumerable.ToArray(), expected);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array, 2, 3, 0).GetColumn(-1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array, 2, 3, 0).GetColumn(3));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Span2D<int>(array, 2, 3, 0).GetColumn(1000));
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_GetEnumerator()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            int[] result = new int[4];
            int i = 0;

            // Here we want to test the Span2D<T> enumerator. We create a Span2D<T> instance over
            // a given section of the initial 2D array, then iterate over it and store the items
            // into a temporary array. We then just compare the contents to ensure they match.
            foreach (ref var item in new Span2D<int>(array, 0, 1, 2, 2))
            {
                // Check the reference to ensure it points to the right original item
                Assert.IsTrue(Unsafe.AreSame(
                    ref array[i / 2, (i % 2) + 1],
                    ref item));

                // Also store the value to compare it later (redundant, but just in case)
                result[i++] = item;
            }

            int[] expected = { 2, 3, 5, 6 };

            CollectionAssert.AreEqual(result, expected);
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public unsafe void Test_Span2DT_Pointer_GetEnumerator()
        {
            int* array = stackalloc[]
            {
                1, 2, 3,
                4, 5, 6
            };

            int[] result = new int[4];
            int i = 0;

            // Same test as above, but wrapping a raw pointer
            foreach (ref var item in new Span2D<int>(array + 1, 2, 2, 1))
            {
                // Check the reference again
                Assert.IsTrue(Unsafe.AreSame(
                    ref Unsafe.AsRef<int>(&array[((i / 2) * 3) + (i % 2) + 1]),
                    ref item));

                result[i++] = item;
            }

            int[] expected = { 2, 3, 5, 6 };

            CollectionAssert.AreEqual(result, expected);
        }

        [TestCategory("Span2DT")]
        [TestMethod]
        public void Test_Span2DT_GetEnumerator_Empty()
        {
            var enumerator = Span2D<int>.Empty.GetEnumerator();

            // Ensure that an enumerator from an empty Span2D<T> can't move next
            Assert.IsFalse(enumerator.MoveNext());
        }
    }
}