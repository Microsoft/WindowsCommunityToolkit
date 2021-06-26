// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.Toolkit.HighPerformance.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.HighPerformance.Extensions
{
    [TestClass]
    public class Test_HashCodeExtensions
    {
        /// <summary>
        /// Gets the list of counts to test the extension for
        /// </summary>
        private static ReadOnlySpan<int> TestCounts => new[] { 0, 1, 7, 128, 255, 256, short.MaxValue, 245_000 };

        [TestCategory("HashCodeOfT")]
        [TestMethod]
        public void Test_HashCodeOfT_VectorSupportedTypes_TestRepeatCount8()
        {
            TestForType<byte>();
            TestForType<sbyte>();
            TestForType<bool>();
        }

        [TestCategory("HashCodeOfT")]
        [TestMethod]
        public void Test_HashCodeOfT_VectorSupportedTypes_TestRepeatCount16()
        {
            TestForType<ushort>();
            TestForType<short>();
        }

        [TestCategory("HashCodeOfT")]
        [TestMethod]
        public void Test_HashCodeOfT_VectorSupportedTypes_TestRepeatCount32()
        {
            TestForType<uint>();
            TestForType<int>();
            TestForType<float>();
        }

        [TestCategory("HashCodeOfT")]
        [TestMethod]
        public void Test_HashCodeOfT_VectorSupportedTypes_TestRepeatCount64()
        {
            TestForType<ulong>();
            TestForType<long>();
            TestForType<double>();
        }

        [TestCategory("HashCodeOfT")]
        [TestMethod]
        public void Test_HashCodeOfT_VectorUnsupportedTypes_TestRepeat()
        {
            TestForType<char>();
        }

        /// <summary>
        /// Performs a test for a specified type.
        /// </summary>
        /// <typeparam name="T">The type to test.</typeparam>
        private static void TestForType<T>()
            where T : unmanaged, IEquatable<T>
        {
            foreach (var count in TestCounts)
            {
                T[] data = CreateRandomData<T>(count);

                HashCode hashCode1 = default;

                hashCode1.Add<T>(data);

                int hash1 = hashCode1.ToHashCode();

                HashCode hashCode2 = default;

                hashCode2.Add<T>(data);

                int hash2 = hashCode2.ToHashCode();

                int hash3 = HashCode<T>.Combine(data);

                Assert.AreEqual(hash1, hash2, $"Failed {typeof(T)} test with count {count}: got {hash1} and then {hash2}");
                Assert.AreEqual(hash1, hash3, $"Failed {typeof(T)} test with count {count}: got {hash1} and then {hash3}");
            }
        }

        /// <summary>
        /// Creates a random <typeparamref name="T"/> array filled with random data.
        /// </summary>
        /// <typeparam name="T">The type of items to put in the array.</typeparam>
        /// <param name="count">The number of array items to create.</param>
        /// <returns>An array of random <typeparamref name="T"/> elements.</returns>
        [Pure]
        private static T[] CreateRandomData<T>(int count)
            where T : unmanaged
        {
            var random = new Random(count);

            T[] data = new T[count];

            foreach (ref byte n in MemoryMarshal.AsBytes(data.AsSpan()))
            {
                n = (byte)random.Next(0, byte.MaxValue);
            }

            return data;
        }
    }
}