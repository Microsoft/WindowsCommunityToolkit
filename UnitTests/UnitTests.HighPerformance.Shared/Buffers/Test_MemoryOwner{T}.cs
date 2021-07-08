// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommunityToolkit.HighPerformance.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.HighPerformance.Shared.Buffers;

namespace UnitTests.HighPerformance.Buffers
{
    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649", Justification = "Test class for generic type")]
    public class Test_MemoryOwnerOfT
    {
        [TestCategory("MemoryOwnerOfT")]
        [TestMethod]
        public void Test_MemoryOwnerOfT_AllocateAndGetMemoryAndSpan()
        {
            using var buffer = MemoryOwner<int>.Allocate(127);

            Assert.IsTrue(buffer.Length == 127);
            Assert.IsTrue(buffer.Memory.Length == 127);
            Assert.IsTrue(buffer.Span.Length == 127);

            buffer.Span.Fill(42);

            Assert.IsTrue(buffer.Memory.Span.ToArray().All(i => i == 42));
            Assert.IsTrue(buffer.Span.ToArray().All(i => i == 42));
        }

        [TestCategory("MemoryOwnerOfT")]
        [TestMethod]
        public void Test_MemoryOwnerOfT_AllocateFromCustomPoolAndGetMemoryAndSpan()
        {
            var pool = new TrackingArrayPool<int>();

            using (var buffer = MemoryOwner<int>.Allocate(127, pool))
            {
                Assert.AreEqual(pool.RentedArrays.Count, 1);

                Assert.IsTrue(buffer.Length == 127);
                Assert.IsTrue(buffer.Memory.Length == 127);
                Assert.IsTrue(buffer.Span.Length == 127);

                buffer.Span.Fill(42);

                Assert.IsTrue(buffer.Memory.Span.ToArray().All(i => i == 42));
                Assert.IsTrue(buffer.Span.ToArray().All(i => i == 42));
            }

            Assert.AreEqual(pool.RentedArrays.Count, 0);
        }

        [TestCategory("MemoryOwnerOfT")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_MemoryOwnerOfT_InvalidRequestedSize()
        {
            using var buffer = MemoryOwner<int>.Allocate(-1);

            Assert.Fail("You shouldn't be here");
        }

        [TestCategory("MemoryOwnerOfT")]
        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Test_MemoryOwnerOfT_DisposedMemory()
        {
            var buffer = MemoryOwner<int>.Allocate(127);

            buffer.Dispose();

            _ = buffer.Memory;
        }

        [TestCategory("MemoryOwnerOfT")]
        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Test_MemoryOwnerOfT_DisposedSpan()
        {
            var buffer = MemoryOwner<int>.Allocate(127);

            buffer.Dispose();

            _ = buffer.Span;
        }

        [TestCategory("MemoryOwnerOfT")]
        [TestMethod]
        public void Test_MemoryOwnerOfT_MultipleDispose()
        {
            var buffer = MemoryOwner<int>.Allocate(127);

            buffer.Dispose();
            buffer.Dispose();
            buffer.Dispose();
            buffer.Dispose();

            // This test consists in just getting here without crashes.
            // We're validating that calling Dispose multiple times
            // by accident doesn't cause issues, and just does nothing.
        }

        [TestCategory("MemoryOwnerOfT")]
        [TestMethod]
        public void Test_MemoryOwnerOfT_PooledBuffersAndClear()
        {
            using (var buffer = MemoryOwner<int>.Allocate(127))
            {
                buffer.Span.Fill(42);
            }

            using (var buffer = MemoryOwner<int>.Allocate(127))
            {
                Assert.IsTrue(buffer.Span.ToArray().All(i => i == 42));
            }

            using (var buffer = MemoryOwner<int>.Allocate(127, AllocationMode.Clear))
            {
                Assert.IsTrue(buffer.Span.ToArray().All(i => i == 0));
            }
        }

        [TestCategory("MemoryOwnerOfT")]
        [TestMethod]
        public void Test_MemoryOwnerOfT_AllocateAndGetArray()
        {
            var buffer = MemoryOwner<int>.Allocate(127);

            // Here we allocate a MemoryOwner<T> instance with a requested size of 127, which means it
            // internally requests an array of size 127 from ArrayPool<T>.Shared. We then get the array
            // segment, so we need to verify that (since buffer is not disposed) the returned array is
            // not null, is of size >= the requested one (since ArrayPool<T> by definition returns an
            // array that is at least of the requested size), and that the offset and count properties
            // match our input values (same length, and offset at 0 since the buffer was not sliced).
            var segment = buffer.DangerousGetArray();

            Assert.IsNotNull(segment.Array);
            Assert.IsTrue(segment.Array.Length >= buffer.Length);
            Assert.AreEqual(segment.Offset, 0);
            Assert.AreEqual(segment.Count, buffer.Length);

            var second = buffer.Slice(10, 80);

            // The original buffer instance is disposed here, because calling Slice transfers
            // the ownership of the internal buffer to the new instance (this is documented in
            // XML docs for the MemoryOwner<T>.Slice method).
            Assert.ThrowsException<ObjectDisposedException>(() => buffer.DangerousGetArray());

            segment = second.DangerousGetArray();

            // Same as before, but we now also verify the initial offset != 0, as we used Slice
            Assert.IsNotNull(segment.Array);
            Assert.IsTrue(segment.Array.Length >= second.Length);
            Assert.AreEqual(segment.Offset, 10);
            Assert.AreEqual(segment.Count, second.Length);

            second.Dispose();

            Assert.ThrowsException<ObjectDisposedException>(() => second.DangerousGetArray());
        }
    }
}