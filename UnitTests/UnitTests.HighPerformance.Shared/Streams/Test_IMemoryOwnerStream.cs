// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.Toolkit.HighPerformance.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.HighPerformance.Streams
{
    [TestClass]
    public class Test_IMemoryOwnerStream
    {
        [TestCategory("IMemoryOwnerStream")]
        [TestMethod]
        public void Test_IMemoryOwnerStream_Lifecycle()
        {
            MemoryOwner<byte> buffer = MemoryOwner<byte>.Allocate(100);

            Stream stream = buffer.AsStream();

            Assert.IsTrue(stream.CanRead);
            Assert.IsTrue(stream.CanSeek);
            Assert.IsTrue(stream.CanWrite);
            Assert.AreEqual(stream.Length, buffer.Length);
            Assert.AreEqual(stream.Position, 0);

            stream.Dispose();

            Assert.ThrowsException<ObjectDisposedException>(() => buffer.Memory);
        }
    }
}