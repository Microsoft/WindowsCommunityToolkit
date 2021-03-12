// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
#if !WINDOWS_UWP
using System.Runtime.CompilerServices;
#endif
using Microsoft.Toolkit.HighPerformance;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.HighPerformance
{
    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649", Justification = "Test class for generic type")]
    public class Test_Memory2DT
    {
        [TestCategory("Memory2DT")]
        [TestMethod]
        public void Test_Memory2DT_Empty()
        {
            // Create a few empty Memory2D<T> instances in different ways and
            // check to ensure the right parameters were used to initialize them.
            Memory2D<int> empty1 = default;

            Assert.IsTrue(empty1.IsEmpty);
            Assert.AreEqual(empty1.Length, 0);
            Assert.AreEqual(empty1.Width, 0);
            Assert.AreEqual(empty1.Height, 0);

            Memory2D<string> empty2 = Memory2D<string>.Empty;

            Assert.IsTrue(empty2.IsEmpty);
            Assert.AreEqual(empty2.Length, 0);
            Assert.AreEqual(empty2.Width, 0);
            Assert.AreEqual(empty2.Height, 0);

            Memory2D<int> empty3 = new int[4, 0];

            Assert.IsTrue(empty3.IsEmpty);
            Assert.AreEqual(empty3.Length, 0);
            Assert.AreEqual(empty3.Width, 0);
            Assert.AreEqual(empty3.Height, 4);

            Memory2D<int> empty4 = new int[0, 7];

            Assert.IsTrue(empty4.IsEmpty);
            Assert.AreEqual(empty4.Length, 0);
            Assert.AreEqual(empty4.Width, 7);
            Assert.AreEqual(empty4.Height, 0);
        }

        [TestCategory("Memory2DT")]
        [TestMethod]
        public void Test_Memory2DT_Array1DConstructor()
        {
            int[] array =
            {
                1, 2, 3, 4, 5, 6
            };

            // Create a memory over a 1D array with 2D data in row-major order. This tests
            // the T[] array constructor for Memory2D<T> with custom size and pitch.
            Memory2D<int> memory2d = new Memory2D<int>(array, 1, 2, 2, 1);

            Assert.IsFalse(memory2d.IsEmpty);
            Assert.AreEqual(memory2d.Length, 4);
            Assert.AreEqual(memory2d.Width, 2);
            Assert.AreEqual(memory2d.Height, 2);
            Assert.AreEqual(memory2d.Span[0, 0], 2);
            Assert.AreEqual(memory2d.Span[1, 1], 6);

            // Also ensure the right exceptions are thrown with invalid parameters, such as
            // negative indices, indices out of range, values that are too big, etc.
            Assert.ThrowsException<ArrayTypeMismatchException>(() => new Memory2D<object>(new string[1], 1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array, -99, 1, 1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array, 0, -10, 1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array, 0, 1, 1, -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array, 0, 1, -100, 1));
            Assert.ThrowsException<ArgumentException>(() => new Memory2D<int>(array, 0, 2, 4, 0));
            Assert.ThrowsException<ArgumentException>(() => new Memory2D<int>(array, 0, 3, 3, 0));
            Assert.ThrowsException<ArgumentException>(() => new Memory2D<int>(array, 1, 2, 3, 0));
            Assert.ThrowsException<ArgumentException>(() => new Memory2D<int>(array, 0, 10, 1, 120));
        }

        [TestCategory("Memory2DT")]
        [TestMethod]
        public void Test_Memory2DT_Array2DConstructor_1()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            // Test the constructor taking a T[,] array that is mapped directly (no slicing)
            Memory2D<int> memory2d = new Memory2D<int>(array);

            Assert.IsFalse(memory2d.IsEmpty);
            Assert.AreEqual(memory2d.Length, 6);
            Assert.AreEqual(memory2d.Width, 3);
            Assert.AreEqual(memory2d.Height, 2);
            Assert.AreEqual(memory2d.Span[0, 1], 2);
            Assert.AreEqual(memory2d.Span[1, 2], 6);

            // Here we test the check for covariance: we can't create a Memory2D<T> from a U[,] array
            // where U is assignable to T (as in, U : T). This would cause a type safety violation on write.
            Assert.ThrowsException<ArrayTypeMismatchException>(() => new Memory2D<object>(new string[1, 2]));
        }

        [TestCategory("Memory2DT")]
        [TestMethod]
        public void Test_Memory2DT_Array2DConstructor_2()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            // Same as above, but this time we also slice the memory to test the other constructor
            Memory2D<int> memory2d = new Memory2D<int>(array, 0, 1, 2, 2);

            Assert.IsFalse(memory2d.IsEmpty);
            Assert.AreEqual(memory2d.Length, 4);
            Assert.AreEqual(memory2d.Width, 2);
            Assert.AreEqual(memory2d.Height, 2);
            Assert.AreEqual(memory2d.Span[0, 0], 2);
            Assert.AreEqual(memory2d.Span[1, 1], 6);

            Assert.ThrowsException<ArrayTypeMismatchException>(() => new Memory2D<object>(new string[1, 2], 0, 0, 2, 2));
        }

        [TestCategory("Memory2DT")]
        [TestMethod]
        public void Test_Memory2DT_Array3DConstructor_1()
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

            // Same as above, but we test the constructor taking a layer within a 3D array
            Memory2D<int> memory2d = new Memory2D<int>(array, 1);

            Assert.IsFalse(memory2d.IsEmpty);
            Assert.AreEqual(memory2d.Length, 6);
            Assert.AreEqual(memory2d.Width, 3);
            Assert.AreEqual(memory2d.Height, 2);
            Assert.AreEqual(memory2d.Span[0, 1], 20);
            Assert.AreEqual(memory2d.Span[1, 2], 60);

            // A couple of tests for invalid parameters, ie. layers out of range
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array, -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array, 2));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array, 20));
        }

        [TestCategory("Memory2DT")]
        [TestMethod]
        public void Test_Memory2DT_Array3DConstructor_2()
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

            // Same as above, but we also slice the target layer in the 3D array. In this case we're creating
            // a Memory<int> instance from a slice in the layer at depth 1 in our 3D array, and with an area
            // starting at coorsinates (0, 1), with a height of 2 and width of 2. So we want to wrap the
            // square with items [20, 30, 50, 60] in the second layer of the 3D array above.
            Memory2D<int> memory2d = new Memory2D<int>(array, 1, 0, 1, 2, 2);

            Assert.IsFalse(memory2d.IsEmpty);
            Assert.AreEqual(memory2d.Length, 4);
            Assert.AreEqual(memory2d.Width, 2);
            Assert.AreEqual(memory2d.Height, 2);
            Assert.AreEqual(memory2d.Span[0, 0], 20);
            Assert.AreEqual(memory2d.Span[1, 1], 60);

            // Same as above, testing a few cases with invalid parameters
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array, -1, 1, 1, 1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array, 1, -1, 1, 1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array, 1, 1, -1, 1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array, 1, 1, 1, -1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array, 1, 1, 1, 1, -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array, 2, 0, 0, 2, 3));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array, 0, 0, 1, 2, 3));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array, 0, 0, 0, 2, 4));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array, 0, 0, 0, 3, 3));
        }

#if !WINDOWS_UWP
        [TestCategory("Memory2DT")]
        [TestMethod]
        public void Test_Memory2DT_MemoryConstructor()
        {
            Memory<int> memory = new[]
            {
                1, 2, 3, 4, 5, 6
            };

            // We also test the constructor that takes an input Memory<T> instance.
            // This is only available on runtimes with fast Span<T> support, as otherwise
            // the implementation would be too complex and slow to work in this case.
            // Conceptually, this works the same as when wrapping a 1D array with row-major items.
            Memory2D<int> memory2d = memory.AsMemory2D(1, 2, 2, 1);

            Assert.IsFalse(memory2d.IsEmpty);
            Assert.AreEqual(memory2d.Length, 4);
            Assert.AreEqual(memory2d.Width, 2);
            Assert.AreEqual(memory2d.Height, 2);
            Assert.AreEqual(memory2d.Span[0, 0], 2);
            Assert.AreEqual(memory2d.Span[1, 1], 6);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => memory.AsMemory2D(-99, 1, 1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => memory.AsMemory2D(0, -10, 1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => memory.AsMemory2D(0, 1, 1, -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => memory.AsMemory2D(0, 1, -100, 1));
            Assert.ThrowsException<ArgumentException>(() => memory.AsMemory2D(0, 2, 4, 0));
            Assert.ThrowsException<ArgumentException>(() => memory.AsMemory2D(0, 3, 3, 0));
            Assert.ThrowsException<ArgumentException>(() => memory.AsMemory2D(1, 2, 3, 0));
            Assert.ThrowsException<ArgumentException>(() => memory.AsMemory2D(0, 10, 1, 120));
        }
#endif

        [TestCategory("Memory2DT")]
        [TestMethod]
        public void Test_Memory2DT_Slice_1()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            Memory2D<int> memory2d = new Memory2D<int>(array);

            // Test a slice from a Memory2D<T> with valid parameters
            Memory2D<int> slice1 = memory2d.Slice(1, 1, 1, 2);

            Assert.AreEqual(slice1.Length, 2);
            Assert.AreEqual(slice1.Height, 1);
            Assert.AreEqual(slice1.Width, 2);
            Assert.AreEqual(slice1.Span[0, 0], 5);
            Assert.AreEqual(slice1.Span[0, 1], 6);

            // Same above, but we test slicing a pre-sliced instance as well. This
            // is done to verify that the internal offsets are properly tracked
            // across multiple slicing operations, instead of just in the first.
            Memory2D<int> slice2 = memory2d.Slice(0, 1, 2, 2);

            Assert.AreEqual(slice2.Length, 4);
            Assert.AreEqual(slice2.Height, 2);
            Assert.AreEqual(slice2.Width, 2);
            Assert.AreEqual(slice2.Span[0, 0], 2);
            Assert.AreEqual(slice2.Span[1, 0], 5);
            Assert.AreEqual(slice2.Span[1, 1], 6);

            // A few invalid slicing operations, with out of range parameters
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array).Slice(-1, 1, 1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array).Slice(1, -1, 1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array).Slice(1, 1, 1, -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array).Slice(1, 1, -1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array).Slice(10, 1, 1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array).Slice(1, 12, 1, 12));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array).Slice(1, 1, 55, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array).Slice(0, 0, 2, 4));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array).Slice(0, 0, 3, 3));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array).Slice(0, 1, 2, 3));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Memory2D<int>(array).Slice(1, 0, 2, 3));
        }

        [TestCategory("Memory2DT")]
        [TestMethod]
        public void Test_Memory2DT_Slice_2()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            Memory2D<int> memory2d = new Memory2D<int>(array);

            // Mostly the same test as above, just with different parameters
            Memory2D<int> slice1 = memory2d.Slice(0, 0, 2, 2);

            Assert.AreEqual(slice1.Length, 4);
            Assert.AreEqual(slice1.Height, 2);
            Assert.AreEqual(slice1.Width, 2);
            Assert.AreEqual(slice1.Span[0, 0], 1);
            Assert.AreEqual(slice1.Span[1, 1], 5);

            Memory2D<int> slice2 = slice1.Slice(1, 0, 1, 2);

            Assert.AreEqual(slice2.Length, 2);
            Assert.AreEqual(slice2.Height, 1);
            Assert.AreEqual(slice2.Width, 2);
            Assert.AreEqual(slice2.Span[0, 0], 4);
            Assert.AreEqual(slice2.Span[0, 1], 5);

            Memory2D<int> slice3 = slice2.Slice(0, 1, 1, 1);

            Assert.AreEqual(slice3.Length, 1);
            Assert.AreEqual(slice3.Height, 1);
            Assert.AreEqual(slice3.Width, 1);
            Assert.AreEqual(slice3.Span[0, 0], 5);
        }

        [TestCategory("Memory2DT")]
        [TestMethod]
        public void Test_Memory2DT_TryGetMemory_1()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            Memory2D<int> memory2d = new Memory2D<int>(array);

            // Here we test that we can get a Memory<T> from a 2D one when the underlying
            // data is contiguous. Note that in this case this can only work on runtimes
            // with fast Span<T> support, because otherwise it's not possible to get a
            // Memory<T> (or a Span<T> too, for that matter) from a 2D array.
            bool success = memory2d.TryGetMemory(out Memory<int> memory);

#if WINDOWS_UWP
            Assert.IsFalse(success);
            Assert.IsTrue(memory.IsEmpty);
#else
            Assert.IsTrue(success);
            Assert.AreEqual(memory.Length, array.Length);
            Assert.IsTrue(Unsafe.AreSame(ref array[0, 0], ref memory.Span[0]));
#endif
        }

        [TestCategory("Memory2DT")]
        [TestMethod]
        public void Test_Memory2DT_TryGetMemory_2()
        {
            int[] array = { 1, 2, 3, 4 };

            Memory2D<int> memory2d = new Memory2D<int>(array, 2, 2);

            // Same test as above, but this will always succeed on all runtimes,
            // as creating a Memory<T> from a 1D array is always supported.
            bool success = memory2d.TryGetMemory(out Memory<int> memory);

            Assert.IsTrue(success);
            Assert.AreEqual(memory.Length, array.Length);
            Assert.AreEqual(memory.Span[2], 3);
        }

#if !WINDOWS_UWP
        [TestCategory("Memory2DT")]
        [TestMethod]
        public void Test_Memory2DT_TryGetMemory_3()
        {
            Memory<int> data = new[] { 1, 2, 3, 4 };

            Memory2D<int> memory2d = data.AsMemory2D(2, 2);

            // Same as above, just with the extra Memory<T> indirection. Same as above,
            // this test is only supported on runtimes with fast Span<T> support.
            // On others, we just don't expose the Memory<T>.AsMemory2D extension.
            bool success = memory2d.TryGetMemory(out Memory<int> memory);

            Assert.IsTrue(success);
            Assert.AreEqual(memory.Length, data.Length);
            Assert.AreEqual(memory.Span[2], 3);
        }
#endif

        [TestCategory("Memory2DT")]
        [TestMethod]
        public unsafe void Test_Memory2DT_Pin_1()
        {
            int[] array = { 1, 2, 3, 4 };

            // We create a Memory2D<T> from an array and verify that pinning this
            // instance correctly returns a pointer to the right array element.
            Memory2D<int> memory2d = new Memory2D<int>(array, 2, 2);

            using var pin = memory2d.Pin();

            Assert.AreEqual(((int*)pin.Pointer)[0], 1);
            Assert.AreEqual(((int*)pin.Pointer)[3], 4);
        }

        [TestCategory("Memory2DT")]
        [TestMethod]
        public unsafe void Test_Memory2DT_Pin_2()
        {
            int[] array = { 1, 2, 3, 4 };

            // Same as above, but we test with a sliced Memory2D<T> instance
            Memory2D<int> memory2d = new Memory2D<int>(array, 2, 2);

            using var pin = memory2d.Pin();

            Assert.AreEqual(((int*)pin.Pointer)[0], 1);
            Assert.AreEqual(((int*)pin.Pointer)[3], 4);
        }

        [TestCategory("Memory2DT")]
        [TestMethod]
        public void Test_Memory2DT_ToArray_1()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            // Here we create a Memory2D<T> instance from a 2D array and then verify that
            // calling ToArray() creates an array that matches the contents of the first.
            Memory2D<int> memory2d = new Memory2D<int>(array);

            int[,] copy = memory2d.ToArray();

            Assert.AreEqual(copy.GetLength(0), array.GetLength(0));
            Assert.AreEqual(copy.GetLength(1), array.GetLength(1));

            CollectionAssert.AreEqual(array, copy);
        }

        [TestCategory("Memory2DT")]
        [TestMethod]
        public void Test_Memory2DT_ToArray_2()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            // Same as above, but with a sliced Memory2D<T> instance
            Memory2D<int> memory2d = new Memory2D<int>(array, 0, 0, 2, 2);

            int[,] copy = memory2d.ToArray();

            Assert.AreEqual(copy.GetLength(0), 2);
            Assert.AreEqual(copy.GetLength(1), 2);

            int[,] expected =
            {
                { 1, 2 },
                { 4, 5 }
            };

            CollectionAssert.AreEqual(expected, copy);
        }

        [TestCategory("Memory2DT")]
        [TestMethod]
        public void Test_Memory2DT_Equals()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            // Here we want to verify that the Memory2D<T>.Equals method works correctly. This is true
            // when the wrapped instance is the same, and the various internal offsets and sizes match.
            Memory2D<int> memory2d = new Memory2D<int>(array);

            Assert.IsFalse(memory2d.Equals(null));
            Assert.IsFalse(memory2d.Equals(new Memory2D<int>(array, 0, 1, 2, 2)));
            Assert.IsTrue(memory2d.Equals(new Memory2D<int>(array)));
            Assert.IsTrue(memory2d.Equals(memory2d));

            // This should work also when casting to a ReadOnlyMemory2D<T> instance
            ReadOnlyMemory2D<int> readOnlyMemory2d = memory2d;

            Assert.IsTrue(memory2d.Equals(readOnlyMemory2d));
            Assert.IsFalse(memory2d.Equals(readOnlyMemory2d.Slice(0, 1, 2, 2)));
        }

        [TestCategory("Memory2DT")]
        [TestMethod]
        public void Test_Memory2DT_GetHashCode()
        {
            // An emoty Memory2D<T> has just 0 as the hashcode
            Assert.AreEqual(Memory2D<int>.Empty.GetHashCode(), 0);

            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            Memory2D<int> memory2d = new Memory2D<int>(array);

            // Ensure that the GetHashCode method is repeatable
            int a = memory2d.GetHashCode(), b = memory2d.GetHashCode();

            Assert.AreEqual(a, b);

            // The hashcode shouldn't match when the size is different
            int c = new Memory2D<int>(array, 0, 1, 2, 2).GetHashCode();

            Assert.AreNotEqual(a, c);
        }

        [TestCategory("Memory2DT")]
        [TestMethod]
        public void Test_Memory2DT_ToString()
        {
            int[,] array =
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };

            Memory2D<int> memory2d = new Memory2D<int>(array);

            // Here we just want to verify that the type is nicely printed as expected, along with the size
            string text = memory2d.ToString();

            const string expected = "Microsoft.Toolkit.HighPerformance.Memory2D<System.Int32>[2, 3]";

            Assert.AreEqual(text, expected);
        }
    }
}