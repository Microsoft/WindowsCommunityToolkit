// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace CommunityToolkit.HighPerformance.Streams
{
    /// <summary>
    /// A <see cref="Stream"/> implementation wrapping a <see cref="Memory{T}"/> or <see cref="ReadOnlyMemory{T}"/> instance.
    /// </summary>
    /// <typeparam name="TSource">The type of source to use for the underlying data.</typeparam>
    /// <remarks>
    /// This type is not marked as <see langword="sealed"/> so that it can be inherited by
    /// <see cref="IMemoryOwnerStream{TSource}"/>, which adds the <see cref="IDisposable"/> support for
    /// the wrapped buffer. We're not worried about the performance penalty here caused by the JIT
    /// not being able to resolve the <see langword="callvirt"/> instruction, as this type is
    /// only exposed as a <see cref="Stream"/> anyway, so the generated code would be the same.
    /// </remarks>
    internal partial class MemoryStream<TSource> : Stream
        where TSource : struct, ISpanOwner
    {
        /// <summary>
        /// Indicates whether <see cref="source"/> can be written to.
        /// </summary>
        private readonly bool isReadOnly;

        /// <summary>
        /// The <typeparamref name="TSource"/> instance currently in use.
        /// </summary>
        private TSource source;

        /// <summary>
        /// The current position within <see cref="source"/>.
        /// </summary>
        private int position;

        /// <summary>
        /// Indicates whether or not the current instance has been disposed
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryStream{TSource}"/> class.
        /// </summary>
        /// <param name="source">The input <typeparamref name="TSource"/> instance to use.</param>
        /// <param name="isReadOnly">Indicates whether <paramref name="source"/> can be written to.</param>
        public MemoryStream(TSource source, bool isReadOnly)
        {
            this.source = source;
            this.isReadOnly = isReadOnly;
        }

        /// <inheritdoc/>
        public sealed override bool CanRead
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => !this.disposed;
        }

        /// <inheritdoc/>
        public sealed override bool CanSeek
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => !this.disposed;
        }

        /// <inheritdoc/>
        public sealed override bool CanWrite
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => !this.isReadOnly && !this.disposed;
        }

        /// <inheritdoc/>
        public sealed override long Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                MemoryStream.ValidateDisposed(this.disposed);

                return this.source.Length;
            }
        }

        /// <inheritdoc/>
        public sealed override long Position
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                MemoryStream.ValidateDisposed(this.disposed);

                return this.position;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                MemoryStream.ValidateDisposed(this.disposed);
                MemoryStream.ValidatePosition(value, this.source.Length);

                this.position = unchecked((int)value);
            }
        }

        /// <inheritdoc/>
        public sealed override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(cancellationToken);
            }

            try
            {
                CopyTo(destination, bufferSize);

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
        public sealed override void Flush()
        {
        }

        /// <inheritdoc/>
        public sealed override Task FlushAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(cancellationToken);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public sealed override Task<int> ReadAsync(byte[]? buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<int>(cancellationToken);
            }

            try
            {
                int result = Read(buffer, offset, count);

                return Task.FromResult(result);
            }
            catch (OperationCanceledException e)
            {
                return Task.FromCanceled<int>(e.CancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException<int>(e);
            }
        }

        /// <inheritdoc/>
        public sealed override Task WriteAsync(byte[]? buffer, int offset, int count, CancellationToken cancellationToken)
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
        public sealed override long Seek(long offset, SeekOrigin origin)
        {
            MemoryStream.ValidateDisposed(this.disposed);

            long index = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => this.position + offset,
                SeekOrigin.End => this.source.Length + offset,
                _ => MemoryStream.ThrowArgumentExceptionForSeekOrigin()
            };

            MemoryStream.ValidatePosition(index, this.source.Length);

            this.position = unchecked((int)index);

            return index;
        }

        /// <inheritdoc/>
        public sealed override void SetLength(long value)
        {
            throw MemoryStream.GetNotSupportedException();
        }

        /// <inheritdoc/>
        public sealed override int Read(byte[]? buffer, int offset, int count)
        {
            MemoryStream.ValidateDisposed(this.disposed);
            MemoryStream.ValidateBuffer(buffer, offset, count);

            int
                bytesAvailable = this.source.Length - this.position,
                bytesCopied = Math.Min(bytesAvailable, count);

            Span<byte>
                source = this.source.Span.Slice(this.position, bytesCopied),
                destination = buffer.AsSpan(offset, bytesCopied);

            source.CopyTo(destination);

            this.position += bytesCopied;

            return bytesCopied;
        }

        /// <inheritdoc/>
        public sealed override int ReadByte()
        {
            MemoryStream.ValidateDisposed(this.disposed);

            if (this.position == this.source.Length)
            {
                return -1;
            }

            return this.source.Span[this.position++];
        }

        /// <inheritdoc/>
        public sealed override void Write(byte[]? buffer, int offset, int count)
        {
            MemoryStream.ValidateDisposed(this.disposed);
            MemoryStream.ValidateCanWrite(CanWrite);
            MemoryStream.ValidateBuffer(buffer, offset, count);

            Span<byte>
                source = buffer.AsSpan(offset, count),
                destination = this.source.Span.Slice(this.position);

            if (!source.TryCopyTo(destination))
            {
                MemoryStream.ThrowArgumentExceptionForEndOfStreamOnWrite();
            }

            this.position += source.Length;
        }

        /// <inheritdoc/>
        public sealed override void WriteByte(byte value)
        {
            MemoryStream.ValidateDisposed(this.disposed);
            MemoryStream.ValidateCanWrite(CanWrite);

            if (this.position == this.source.Length)
            {
                MemoryStream.ThrowArgumentExceptionForEndOfStreamOnWrite();
            }

            this.source.Span[this.position++] = value;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.source = default;
        }
    }
}