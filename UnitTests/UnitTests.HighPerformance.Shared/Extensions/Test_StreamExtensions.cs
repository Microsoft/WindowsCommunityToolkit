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
    public class Test_StreamExtensions
    {
        [TestCategory("StreamExtensions")]
        [TestMethod]
        public void Test_StreamExtensions_ReadWrite()
        {
            // bool (1), int (4), float (4), long (8) = 17 bytes.
            // Leave two extra bytes for the partial read (fail).
            Stream stream = new byte[19].AsMemory().AsStream();

            stream.Write(true);
            stream.Write(42);
            stream.Write(3.14f);
            stream.Write(unchecked(uint.MaxValue * 324823489204ul));

            Assert.AreEqual(stream.Position, 17);

            Assert.ThrowsException<ArgumentException>(() => stream.Write(long.MaxValue));

            stream.Position = 0;

            Assert.AreEqual(true, stream.Read<bool>());
            Assert.AreEqual(42, stream.Read<int>());
            Assert.AreEqual(3.14f, stream.Read<float>());
            Assert.AreEqual(unchecked(uint.MaxValue * 324823489204ul), stream.Read<ulong>());

            Assert.ThrowsException<InvalidOperationException>(() => stream.Read<long>());
        }
    }
}