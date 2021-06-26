// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Toolkit.HighPerformance.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.HighPerformance.Buffers
{
    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649", Justification = "Test class for generic type")]
    public class Test_MemoryBufferWriterOfT
    {
        [TestCategory("MemoryBufferWriterOfT")]
        [TestMethod]
        public void Test_MemoryBufferWriterOfT_AllocateAndGetMemoryAndSpan()
        {
            Memory<byte> memory = new byte[256];

            var writer = new MemoryBufferWriter<byte>(memory);

            Assert.AreEqual(writer.Capacity, 256);
            Assert.AreEqual(writer.FreeCapacity, 256);
            Assert.AreEqual(writer.WrittenCount, 0);
            Assert.IsTrue(writer.WrittenMemory.IsEmpty);
            Assert.IsTrue(writer.WrittenSpan.IsEmpty);

            Span<byte> span = writer.GetSpan(43);

            Assert.AreEqual(span.Length, memory.Length);

            writer.Advance(43);

            Assert.AreEqual(writer.Capacity, 256);
            Assert.AreEqual(writer.FreeCapacity, 256 - 43);
            Assert.AreEqual(writer.WrittenCount, 43);
            Assert.AreEqual(writer.WrittenMemory.Length, 43);
            Assert.AreEqual(writer.WrittenSpan.Length, 43);

            Assert.AreEqual(memory.Length - 43, writer.GetSpan().Length);
            Assert.AreEqual(memory.Length - 43, writer.GetMemory().Length);
            Assert.AreEqual(memory.Length - 43, writer.GetSpan(22).Length);
            Assert.AreEqual(memory.Length - 43, writer.GetMemory(22).Length);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => writer.Advance(-1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => writer.GetMemory(-1));
            Assert.ThrowsException<ArgumentException>(() => writer.GetSpan(1024));
            Assert.ThrowsException<ArgumentException>(() => writer.GetMemory(1024));
            Assert.ThrowsException<ArgumentException>(() => writer.Advance(1024));
        }

        [TestCategory("MemoryBufferWriterOfT")]
        [TestMethod]
        public void Test_MemoryBufferWriterOfT_Clear()
        {
            Memory<byte> memory = new byte[256];

            var writer = new MemoryBufferWriter<byte>(memory);

            Span<byte> span = writer.GetSpan(4).Slice(0, 4);

            byte[] data = { 1, 2, 3, 4 };

            data.CopyTo(span);

            writer.Advance(4);

            Assert.AreEqual(writer.WrittenCount, 4);
            Assert.IsTrue(span.SequenceEqual(data));

            writer.Clear();

            Assert.AreEqual(writer.WrittenCount, 0);
            Assert.IsTrue(span.ToArray().All(b => b == 0));
        }
    }
}