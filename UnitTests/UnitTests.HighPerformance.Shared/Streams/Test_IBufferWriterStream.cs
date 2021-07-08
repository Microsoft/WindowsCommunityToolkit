// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.HighPerformance.Streams
{
    [TestClass]
    public class Test_IBufferWriterStream
    {
        [TestCategory("IBufferWriterStream")]
        [TestMethod]
        public void Test_IBufferWriterStream_Lifecycle()
        {
            ArrayPoolBufferWriter<byte> writer = new ArrayPoolBufferWriter<byte>();

            // Get a stream from a buffer writer aand validate that it can only be written to.
            // This is to mirror the same functionality as the IBufferWriter<T> interface.
            Stream stream = ((IBufferWriter<byte>)writer).AsStream();

            Assert.IsFalse(stream.CanRead);
            Assert.IsFalse(stream.CanSeek);
            Assert.IsTrue(stream.CanWrite);

            Assert.ThrowsException<NotSupportedException>(() => stream.Length);
            Assert.ThrowsException<NotSupportedException>(() => stream.Position);

            // Dispose the stream and check that no operation is now allowed
            stream.Dispose();

            Assert.IsFalse(stream.CanRead);
            Assert.IsFalse(stream.CanSeek);
            Assert.IsFalse(stream.CanWrite);
            Assert.ThrowsException<NotSupportedException>(() => stream.Length);
            Assert.ThrowsException<NotSupportedException>(() => stream.Position);
        }

        [TestCategory("IBufferWriterStream")]
        [TestMethod]
        public void Test_IBufferWriterStream_Write_Array()
        {
            ArrayPoolBufferWriter<byte> writer = new ArrayPoolBufferWriter<byte>();
            Stream stream = ((IBufferWriter<byte>)writer).AsStream();

            byte[] data = Test_MemoryStream.CreateRandomData(64);

            // Write random data to the stream wrapping the buffer writer, and validate
            // that the state of the writer is consistent, and the written content matches.
            stream.Write(data, 0, data.Length);

            Assert.AreEqual(writer.WrittenCount, data.Length);
            Assert.IsTrue(writer.WrittenSpan.SequenceEqual(data));

            // A few tests with invalid inputs (null buffers, invalid indices, etc.)
            Assert.ThrowsException<ArgumentNullException>(() => stream.Write(null, 0, 10));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => stream.Write(data, -1, 10));
            Assert.ThrowsException<ArgumentException>(() => stream.Write(data, 200, 10));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => stream.Write(data, 0, -24));
            Assert.ThrowsException<ArgumentException>(() => stream.Write(data, 0, 200));

            stream.Dispose();

            Assert.ThrowsException<ObjectDisposedException>(() => stream.Write(data, 0, data.Length));
        }

        [TestCategory("IBufferWriterStream")]
        [TestMethod]
        public async Task Test_IBufferWriterStream_WriteAsync_Array()
        {
            ArrayPoolBufferWriter<byte> writer = new ArrayPoolBufferWriter<byte>();
            Stream stream = ((IBufferWriter<byte>)writer).AsStream();

            byte[] data = Test_MemoryStream.CreateRandomData(64);

            // Same test as above, but using an asynchronous write instead
            await stream.WriteAsync(data, 0, data.Length);

            Assert.AreEqual(writer.WrittenCount, data.Length);
            Assert.IsTrue(writer.WrittenSpan.SequenceEqual(data));

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => stream.WriteAsync(null, 0, 10));
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => stream.WriteAsync(data, -1, 10));
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => stream.WriteAsync(data, 200, 10));
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => stream.WriteAsync(data, 0, -24));
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => stream.WriteAsync(data, 0, 200));

            stream.Dispose();

            await Assert.ThrowsExceptionAsync<ObjectDisposedException>(() => stream.WriteAsync(data, 0, data.Length));
        }

        [TestCategory("IBufferWriterStream")]
        [TestMethod]
        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1500", Justification = "Array initialization")]
        public void Test_IBufferWriterStream_WriteByte()
        {
            ArrayPoolBufferWriter<byte> writer = new ArrayPoolBufferWriter<byte>();
            Stream stream = ((IBufferWriter<byte>)writer).AsStream();

            ReadOnlySpan<byte> data = stackalloc byte[] { 1, 128, 255, 32 };

            foreach (var item in data.Enumerate())
            {
                // Since we're enumerating, we can also double check the current written count
                // at each iteration, to ensure the writes are done correctly every time.
                Assert.AreEqual(writer.WrittenCount, item.Index);

                // Write a number of bytes one by one to test this API as well
                stream.WriteByte(item.Value);
            }

            // Validate the final written length and actual data
            Assert.AreEqual(writer.WrittenCount, data.Length);
            Assert.IsTrue(data.SequenceEqual(writer.WrittenSpan));

            Assert.ThrowsException<NotSupportedException>(() => stream.ReadByte());
        }

        [TestCategory("IBufferWriterStream")]
        [TestMethod]
        public void Test_IBufferWriterStream_Write_Span()
        {
            ArrayPoolBufferWriter<byte> writer = new ArrayPoolBufferWriter<byte>();
            Stream stream = ((IBufferWriter<byte>)writer).AsStream();

            Memory<byte> data = Test_MemoryStream.CreateRandomData(64);

            // This will use the extension when on .NET Standard 2.0,
            // as the Stream class doesn't have Spam<T> or Memory<T>
            // public APIs there. This is the case eg. on UWP as well.
            stream.Write(data.Span);

            Assert.AreEqual(writer.WrittenCount, data.Length);
            Assert.IsTrue(data.Span.SequenceEqual(writer.WrittenSpan));
        }

        [TestCategory("IBufferWriterStream")]
        [TestMethod]
        public async Task Test_IBufferWriterStream_WriteAsync_Memory()
        {
            ArrayPoolBufferWriter<byte> writer = new ArrayPoolBufferWriter<byte>();
            Stream stream = ((IBufferWriter<byte>)writer).AsStream();

            Memory<byte> data = Test_MemoryStream.CreateRandomData(64);

            // Same as the other asynchronous test above, but writing from a Memory<T>
            await stream.WriteAsync(data);

            Assert.AreEqual(writer.WrittenCount, data.Length);
            Assert.IsTrue(data.Span.SequenceEqual(writer.WrittenSpan));
        }
    }
}