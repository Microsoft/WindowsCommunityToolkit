// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.HighPerformance.Shared.Buffers.Internals;

#nullable enable

namespace UnitTests.HighPerformance.Extensions
{
    public partial class Test_ReadOnlySpanExtensions
    {
        /// <summary>
        /// Gets the list of counts to test the extension for
        /// </summary>
        private static ReadOnlySpan<int> TestCounts => new[] { 0, 1, 7, 128, 255, 256, short.MaxValue, short.MaxValue + 1, 123_938, 1_678_922, 71_890_819 };

        [TestCategory("ReadOnlySpanExtensions")]
        [TestMethod]
        public void Test_ReadOnlySpanExtensions_RandomCount8()
        {
            TestForType((byte)123, CreateRandomData);
            TestForType((sbyte)123, CreateRandomData);
            TestForType(true, CreateRandomData);
        }

        [TestCategory("ReadOnlySpanExtensions")]
        [TestMethod]
        public void Test_ReadOnlySpanExtensions_RandomCount16()
        {
            TestForType((ushort)4712, CreateRandomData);
            TestForType((short)4712, CreateRandomData);
            TestForType((char)4712, CreateRandomData);
        }

        [TestCategory("ReadOnlySpanExtensions")]
        [TestMethod]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1139", Justification = "Easier to tell types apart at a glance")]
        public void Test_ReadOnlySpanExtensions_RandomCount32()
        {
            TestForType((int)37438941, CreateRandomData);
            TestForType((uint)37438941, CreateRandomData);
            TestForType(MathF.PI, CreateRandomData);
        }

        [TestCategory("ReadOnlySpanExtensions")]
        [TestMethod]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1139", Justification = "Easier to tell types apart at a glance")]
        public void Test_ReadOnlySpanExtensions_RandomCount64()
        {
            TestForType((long)47128480128401, CreateRandomData);
            TestForType((ulong)47128480128401, CreateRandomData);
            TestForType(Math.PI, CreateRandomData);
        }

        [TestCategory("ReadOnlySpanExtensions")]
        [TestMethod]
        public void Test_ReadOnlySpanExtensions_RandomCountManaged()
        {
            var value = new Int(37438941);

            // We can skip the most expensive test in this case, as we're not testing
            // a SIMD enabled path. The last test requires a very high memory usage which
            // sometimes causes the CI test runner to fail with an out of memory exception.
            // Since we don't need to double check overflows in the managed case, which is
            // just a classic linear loop with some optimizations, omitting this case is fine.
            foreach (var count in TestCounts.Slice(0, TestCounts.Length - 1))
            {
                var random = new Random(count);

                Int[] data = new Int[count];

                foreach (ref Int item in data.AsSpan())
                {
                    item = new Int(random.Next());
                }

                // Fill at least 20% of the items with a matching value
                int minimum = count / 20;

                for (int i = 0; i < minimum; i++)
                {
                    data[random.Next(0, count)] = value;
                }

                int result = data.Count(value);
                int expected = CountWithForeach(data, value);

                Assert.AreEqual(result, expected, $"Failed {typeof(Int)} test with count {count}: got {result} instead of {expected}");
            }
        }

        // Dummy type to test the managed code path of the API
        private sealed class Int : IEquatable<Int>
        {
            private int Value { get; }

            public Int(int value) => Value = value;

            public bool Equals(Int? other)
            {
                if (other is null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return this.Value == other.Value;
            }

            public override bool Equals(object? obj)
            {
                return ReferenceEquals(this, obj) || (obj is Int other && Equals(other));
            }

            public override int GetHashCode()
            {
                return this.Value;
            }
        }

        [TestCategory("ReadOnlySpanExtensions")]
        [TestMethod]
        public void Test_ReadOnlySpanExtensions_FilledCount8()
        {
            TestForType((byte)123, (count, _) => CreateFilledData(count, (byte)123));
            TestForType((sbyte)123, (count, _) => CreateFilledData(count, (sbyte)123));
            TestForType(true, (count, _) => CreateFilledData(count, true));
        }

        [TestCategory("ReadOnlySpanExtensions")]
        [TestMethod]
        public void Test_ReadOnlySpanExtensions_FilledCount16()
        {
            TestForType((ushort)4712, (count, _) => CreateFilledData(count, (ushort)4712));
            TestForType((short)4712, (count, _) => CreateFilledData(count, (short)4712));
            TestForType((char)4712, (count, _) => CreateFilledData(count, (char)4712));
        }

        [TestCategory("ReadOnlySpanExtensions")]
        [TestMethod]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1139", Justification = "Easier to tell types apart at a glance")]
        public void Test_ReadOnlySpanExtensions_FilledCount32()
        {
            TestForType((int)37438941, (count, _) => CreateFilledData(count, (int)37438941));
            TestForType((uint)37438941, (count, _) => CreateFilledData(count, (uint)37438941));
        }

        [TestCategory("ReadOnlySpanExtensions")]
        [TestMethod]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1139", Justification = "Easier to tell types apart at a glance")]
        public void Test_ReadOnlySpanExtensions_FilledCount64()
        {
            TestForType((long)47128480128401, (count, _) => CreateFilledData(count, (long)47128480128401));
            TestForType((ulong)47128480128401, (count, _) => CreateFilledData(count, (ulong)47128480128401));
        }

        /// <summary>
        /// Performs a test for a specified type.
        /// </summary>
        /// <typeparam name="T">The type to test.</typeparam>
        /// <param name="value">The target value to look for.</param>
        /// <param name="provider">The function to use to create random data.</param>
        private static void TestForType<T>(T value, Func<int, T, UnmanagedSpanOwner<T>> provider)
            where T : unmanaged, IEquatable<T>
        {
            foreach (var count in TestCounts)
            {
                using UnmanagedSpanOwner<T> data = provider(count, value);

                int result = data.GetSpan().Count(value);
                int expected = CountWithForeach(data.GetSpan(), value);

                Assert.AreEqual(result, expected, $"Failed {typeof(T)} test with count {count}: got {result} instead of {expected}");
            }
        }

        /// <summary>
        /// Counts the number of occurrences of a given value into a target <see cref="ReadOnlySpan{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of items to count.</typeparam>
        /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance to read.</param>
        /// <param name="value">The value to look for.</param>
        /// <returns>The number of occurrences of <paramref name="value"/> in <paramref name="span"/>.</returns>
        [Pure]
        private static int CountWithForeach<T>(ReadOnlySpan<T> span, T value)
            where T : IEquatable<T>
        {
            int count = 0;

            foreach (var item in span)
            {
                if (item.Equals(value))
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Creates a random <typeparamref name="T"/> array filled with random data.
        /// </summary>
        /// <typeparam name="T">The type of items to put in the array.</typeparam>
        /// <param name="count">The number of array items to create.</param>
        /// <param name="value">The value to look for.</param>
        /// <returns>An array of random <typeparamref name="T"/> elements.</returns>
        [Pure]
        private static UnmanagedSpanOwner<T> CreateRandomData<T>(int count, T value)
            where T : unmanaged
        {
            var random = new Random(count);

            UnmanagedSpanOwner<T> data = new UnmanagedSpanOwner<T>(count);

            foreach (ref byte n in MemoryMarshal.AsBytes(data.GetSpan()))
            {
                n = (byte)random.Next(0, byte.MaxValue);
            }

            // Fill at least 20% of the items with a matching value
            int minimum = count / 20;

            Span<T> span = data.GetSpan();

            for (int i = 0; i < minimum; i++)
            {
                span[random.Next(0, count)] = value;
            }

            return data;
        }

        /// <summary>
        /// Creates a <typeparamref name="T"/> array filled with a given value.
        /// </summary>
        /// <typeparam name="T">The type of items to put in the array.</typeparam>
        /// <param name="count">The number of array items to create.</param>
        /// <param name="value">The value to use to populate the array.</param>
        /// <returns>An array of <typeparamref name="T"/> elements.</returns>
        [Pure]
        private static UnmanagedSpanOwner<T> CreateFilledData<T>(int count, T value)
            where T : unmanaged
        {
            UnmanagedSpanOwner<T> data = new UnmanagedSpanOwner<T>(count);

            data.GetSpan().Fill(value);

            return data;
        }
    }
}
