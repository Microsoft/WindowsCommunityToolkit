// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.Toolkit.HighPerformance.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.HighPerformance.Extensions
{
    [TestClass]
    public class Test_IMemoryOwnerExtensions
    {
        [TestCategory("IMemoryOwnerExtensions")]
        [TestMethod]
        public void Test_IMemoryOwnerExtensions_EmptyIMemoryOwnerStream()
        {
            MemoryOwner<byte> buffer = MemoryOwner<byte>.Empty;

            Stream stream = buffer.AsStream();

            Assert.IsNotNull(stream);
            Assert.AreEqual(buffer.Length, 0);
            Assert.AreEqual(stream.Length, 0);
            Assert.IsTrue(stream.CanWrite);
        }

        [TestCategory("IMemoryOwnerExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_IMemoryOwnerStream()
        {
            MemoryOwner<byte> buffer = MemoryOwner<byte>.Allocate(1024);

            Stream stream = buffer.AsStream();

            Assert.IsNotNull(stream);
            Assert.AreEqual(stream.Length, buffer.Length);
            Assert.IsTrue(stream.CanWrite);
        }

        [TestCategory("IMemoryOwnerExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_IMemoryOwnerStream_DoesNotAlterExistingData()
        {
            MemoryOwner<byte> buffer = MemoryOwner<byte>.Allocate(1024);

            // Fill the buffer with sample data
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer.Span[i] = unchecked((byte)(i & byte.MaxValue));
            }

            Stream stream = buffer.AsStream();

            Assert.IsNotNull(stream);
            Assert.AreEqual(stream.Length, buffer.Length);
            Assert.IsTrue(stream.CanWrite);

            // Validate that creating the stream doesn't alter the underlying buffer
            for (int i = 0; i < buffer.Length; i++)
            {
                Assert.AreEqual(buffer.Span[i], unchecked((byte)(i & byte.MaxValue)));
            }
        }
    }
}