// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using CommunityToolkit.HighPerformance;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.HighPerformance.Extensions
{
    [TestClass]
    public class Test_ReadOnlyMemoryExtensions
    {
        [TestCategory("ReadOnlyMemoryExtensions")]
        [TestMethod]
        public void Test_ReadOnlyMemoryExtensions_EmptyMemoryStream()
        {
            ReadOnlyMemory<byte> memory = default;

            Stream stream = memory.AsStream();

            Assert.IsNotNull(stream);
            Assert.AreEqual(stream.Length, memory.Length);
            Assert.IsFalse(stream.CanWrite);
        }

        [TestCategory("ReadOnlyMemoryExtensions")]
        [TestMethod]
        public void Test_ReadOnlyMemoryExtensions_MemoryStream()
        {
            ReadOnlyMemory<byte> memory = new byte[1024];

            Stream stream = memory.AsStream();

            Assert.IsNotNull(stream);
            Assert.AreEqual(stream.Length, memory.Length);
            Assert.IsFalse(stream.CanWrite);
        }
    }
}