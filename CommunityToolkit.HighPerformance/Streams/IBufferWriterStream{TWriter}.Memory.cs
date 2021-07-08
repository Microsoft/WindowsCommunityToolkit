// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if SPAN_RUNTIME_SUPPORT

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CommunityToolkit.HighPerformance.Streams
{
    /// <inheritdoc cref="IBufferWriterStream{TWriter}"/>
    internal sealed partial class IBufferWriterStream<TWriter>
    {
        /// <inheritdoc/>
        public override void CopyTo(Stream destination, int bufferSize)
        {
            throw MemoryStream.GetNotSupportedException();
        }

        /// <inheritdoc/>
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            throw MemoryStream.GetNotSupportedException();
        }

        /// <inheritdoc/>
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return new ValueTask(Task.FromCanceled(cancellationToken));
            }

            try
            {
                Write(buffer.Span);

                return default;
            }
            catch (OperationCanceledException e)
            {
                return new ValueTask(Task.FromCanceled(e.CancellationToken));
            }
            catch (Exception e)
            {
                return new ValueTask(Task.FromException(e));
            }
        }

        /// <inheritdoc/>
        public override int Read(Span<byte> buffer)
        {
            throw MemoryStream.GetNotSupportedException();
        }

        /// <inheritdoc/>
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            MemoryStream.ValidateDisposed(this.disposed);

            Span<byte> destination = this.bufferWriter.GetSpan(buffer.Length);

            if (!buffer.TryCopyTo(destination))
            {
                MemoryStream.ThrowArgumentExceptionForEndOfStreamOnWrite();
            }

            this.bufferWriter.Advance(buffer.Length);
        }
    }
}

#endif