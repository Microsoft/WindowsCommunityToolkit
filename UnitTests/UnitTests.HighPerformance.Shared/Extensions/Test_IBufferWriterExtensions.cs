// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
#if !WINDOWS_UWP
using System.Buffers;
#endif
using System.IO;
using System.Runtime.CompilerServices;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.HighPerformance.Extensions
{
    [TestClass]
    public class Test_IBufferWriterExtensions
    {
        [TestCategory("IBufferWriterExtensions")]
        [TestMethod]
        public void Test_IBufferWriterExtensions_WriteReadOverBytes()
        {
            ArrayPoolBufferWriter<byte> writer = new ArrayPoolBufferWriter<byte>();

            byte b = 255;
            char c = '$';
            float f = 3.14f;
            double d = 6.28;
            Guid guid = Guid.NewGuid();

            writer.Write(b);
            writer.Write(c);
            writer.Write(f);
            writer.Write(d);
            writer.Write(guid);

            int count = sizeof(byte) + sizeof(char) + sizeof(float) + sizeof(double) + Unsafe.SizeOf<Guid>();

            Assert.AreEqual(count, writer.WrittenCount);

            using Stream reader = writer.WrittenMemory.AsStream();

            Assert.AreEqual(b, reader.Read<byte>());
            Assert.AreEqual(c, reader.Read<char>());
            Assert.AreEqual(f, reader.Read<float>());
            Assert.AreEqual(d, reader.Read<double>());
            Assert.AreEqual(guid, reader.Read<Guid>());
        }

        [TestCategory("IBufferWriterExtensions")]
        [TestMethod]
        public void Test_IBufferWriterExtensions_WriteReadItem_Guid()
        {
            Test_IBufferWriterExtensions_WriteReadItem(Guid.NewGuid(), Guid.NewGuid());
        }

        [TestCategory("IBufferWriterExtensions")]
        [TestMethod]
        public void Test_IBufferWriterExtensions_WriteReadItem_String()
        {
            Test_IBufferWriterExtensions_WriteReadItem("Hello", "World");
        }

        private static void Test_IBufferWriterExtensions_WriteReadItem<T>(T a, T b)
            where T : IEquatable<T>
        {
            ArrayPoolBufferWriter<T> writer = new ArrayPoolBufferWriter<T>();

            writer.Write(a);
            writer.Write(b);

            Assert.AreEqual(2, writer.WrittenCount);

            ReadOnlySpan<T> span = writer.WrittenSpan;

            Assert.AreEqual(a, span[0]);
            Assert.AreEqual(b, span[1]);
        }

        [TestCategory("IBufferWriterExtensions")]
        [TestMethod]
        public void Test_IBufferWriterExtensions_WriteReadOverBytes_ReadOnlySpan()
        {
            int[] buffer = new int[128];

            var random = new Random(42);

            foreach (ref var n in buffer.AsSpan())
            {
                n = random.Next(int.MinValue, int.MaxValue);
            }

            ArrayPoolBufferWriter<byte> writer = new ArrayPoolBufferWriter<byte>();

            writer.Write<int>(buffer);

            Assert.AreEqual(sizeof(int) * buffer.Length, writer.WrittenCount);

            ReadOnlySpan<byte> span = writer.WrittenSpan;

            Assert.IsTrue(span.SequenceEqual(buffer.AsSpan().AsBytes()));
        }

        [TestCategory("IBufferWriterExtensions")]
        [TestMethod]
        public void Test_IBufferWriterExtensions_WriteReadOverItems_ReadOnlySpan()
        {
            int[] buffer = new int[128];

            var random = new Random(42);

            foreach (ref var n in buffer.AsSpan())
            {
                n = random.Next(int.MinValue, int.MaxValue);
            }

            ArrayPoolBufferWriter<int> writer = new ArrayPoolBufferWriter<int>();

            writer.Write(buffer.AsSpan());

            Assert.AreEqual(buffer.Length, writer.WrittenCount);

            ReadOnlySpan<int> span = writer.WrittenSpan;

            Assert.IsTrue(span.SequenceEqual(buffer.AsSpan()));
        }
    }
}