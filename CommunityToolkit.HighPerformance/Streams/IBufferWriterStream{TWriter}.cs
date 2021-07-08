// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace CommunityToolkit.HighPerformance.Streams
{
    /// <summary>
    /// A <see cref="Stream"/> implementation wrapping an <see cref="IBufferWriter{T}"/> instance.
    /// </summary>
    /// <typeparam name="TWriter">The type of buffer writer to use.</typeparam>
    internal sealed partial class IBufferWriterStream<TWriter> : Stream
        where TWriter : struct, IBufferWriter<byte>
    {
        /// <summary>
        /// The target <typeparamref name="TWriter"/> instance to use.
        /// </summary>
        private readonly TWriter bufferWriter;

        /// <summary>
        /// Indicates whether or not the current instance has been disposed
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="IBufferWriterStream{TWriter}"/> class.
        /// </summary>
        /// <param name="bufferWriter">The target <see cref="IBufferWriter{T}"/> instance to use.</param>
        public IBufferWriterStream(TWriter bufferWriter)
        {
            this.bufferWriter = bufferWriter;
        }

        /// <inheritdoc/>
        public override bool CanRead => false;

        /// <inheritdoc/>
        public override bool CanSeek => false;

        /// <inheritdoc/>
        public override bool CanWrite
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => !this.disposed;
        }

        /// <inheritdoc/>
        public override long Length => throw MemoryStream.GetNotSupportedException();

        /// <inheritdoc/>
        public override long Position
        {
            get => throw MemoryStream.GetNotSupportedException();
            set => throw MemoryStream.GetNotSupportedException();
        }

        /// <inheritdoc/>
        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            throw MemoryStream.GetNotSupportedException();
        }

        /// <inheritdoc/>
        public override void Flush()
        {
        }

        /// <inheritdoc/>
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(cancellationToken);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override Task<int> ReadAsync(byte[]? buffer, int offset, int count, CancellationToken cancellationToken)
        {
            throw MemoryStream.GetNotSupportedException();
        }

        /// <inheritdoc/>
        public override Task WriteAsync(byte[]? buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(cancellationToken);
            }

            try
            {
                Write(buffer, offset, count);

                return Task.CompletedTask;
            }
            catch (OperationCanceledException e)
            {
                return Task.FromCanceled(e.CancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw MemoryStream.GetNotSupportedException();
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            throw MemoryStream.GetNotSupportedException();
        }

        /// <inheritdoc/>
        public override int Read(byte[]? buffer, int offset, int count)
        {
            throw MemoryStream.GetNotSupportedException();
        }

        /// <inheritdoc/>
        public override int ReadByte()
        {
            throw MemoryStream.GetNotSupportedException();
        }

        /// <inheritdoc/>
        public override void Write(byte[]? buffer, int offset, int count)
        {
            MemoryStream.ValidateDisposed(this.disposed);
            MemoryStream.ValidateBuffer(buffer, offset, count);

            Span<byte>
                source = buffer.AsSpan(offset, count),
                destination = this.bufferWriter.GetSpan(count);

            if (!source.TryCopyTo(destination))
            {
                MemoryStream.ThrowArgumentExceptionForEndOfStreamOnWrite();
            }

            this.bufferWriter.Advance(count);
        }

        /// <inheritdoc/>
        public override void WriteByte(byte value)
        {
            MemoryStream.ValidateDisposed(this.disposed);

            this.bufferWriter.GetSpan(1)[0] = value;

            this.bufferWriter.Advance(1);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            this.disposed = true;
        }
    }
}