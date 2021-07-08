// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if SPAN_RUNTIME_SUPPORT
using CommunityToolkit.HighPerformance.Buffers.Internals;
#endif
using CommunityToolkit.HighPerformance.Helpers;
using CommunityToolkit.HighPerformance.Memory.Internals;
using CommunityToolkit.HighPerformance.Memory.Views;
using static CommunityToolkit.HighPerformance.Helpers.Internals.RuntimeHelpers;

#pragma warning disable CA2231

namespace CommunityToolkit.HighPerformance
{
    /// <summary>
    /// <see cref="Memory2D{T}"/> represents a 2D region of arbitrary memory. It is to <see cref="Span2D{T}"/>
    /// what <see cref="Memory{T}"/> is to <see cref="Span{T}"/>. For further details on how the internal layout
    /// is structured, see the docs for <see cref="Span2D{T}"/>. The <see cref="Memory2D{T}"/> type can wrap arrays
    /// of any rank, provided that a valid series of parameters for the target memory area(s) are specified.
    /// </summary>
    /// <typeparam name="T">The type of items in the current <see cref="Memory2D{T}"/> instance.</typeparam>
    [DebuggerTypeProxy(typeof(MemoryDebugView2D<>))]
    [DebuggerDisplay("{ToString(),raw}")]
    public readonly struct Memory2D<T> : IEquatable<Memory2D<T>>
    {
        /// <summary>
        /// The target <see cref="object"/> instance, if present.
        /// </summary>
        private readonly object? instance;

        /// <summary>
        /// The initial offset within <see cref="instance"/>.
        /// </summary>
        private readonly IntPtr offset;

        /// <summary>
        /// The height of the specified 2D region.
        /// </summary>
        private readonly int height;

        /// <summary>
        /// The width of the specified 2D region.
        /// </summary>
        private readonly int width;

        /// <summary>
        /// The pitch of the specified 2D region.
        /// </summary>
        private readonly int pitch;

        /// <summary>
        /// Initializes a new instance of the <see cref="Memory2D{T}"/> struct.
        /// </summary>
        /// <param name="array">The target array to wrap.</param>
        /// <param name="height">The height of the resulting 2D area.</param>
        /// <param name="width">The width of each row in the resulting 2D area.</param>
        /// <exception cref="ArrayTypeMismatchException">
        /// Thrown when <paramref name="array"/> doesn't match <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when either <paramref name="height"/> or <paramref name="width"/> are invalid.
        /// </exception>
        /// <remarks>The total area must match the length of <paramref name="array"/>.</remarks>
        public Memory2D(T[] array, int height, int width)
            : this(array, 0, height, width, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Memory2D{T}"/> struct.
        /// </summary>
        /// <param name="array">The target array to wrap.</param>
        /// <param name="offset">The initial offset within <paramref name="array"/>.</param>
        /// <param name="height">The height of the resulting 2D area.</param>
        /// <param name="width">The width of each row in the resulting 2D area.</param>
        /// <param name="pitch">The pitch in the resulting 2D area.</param>
        /// <exception cref="ArrayTypeMismatchException">
        /// Thrown when <paramref name="array"/> doesn't match <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when one of the input parameters is out of range.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the requested area is outside of bounds for <paramref name="array"/>.
        /// </exception>
        public Memory2D(T[] array, int offset, int height, int width, int pitch)
        {
            if (array.IsCovariant())
            {
                ThrowHelper.ThrowArrayTypeMismatchException();
            }

            if ((uint)offset > (uint)array.Length)
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForOffset();
            }

            if (height < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForHeight();
            }

            if (width < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForWidth();
            }

            if (pitch < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForPitch();
            }

            int
                area = OverflowHelper.ComputeInt32Area(height, width, pitch),
                remaining = array.Length - offset;

            if (area > remaining)
            {
                ThrowHelper.ThrowArgumentException();
            }

            this.instance = array;
            this.offset = ObjectMarshal.DangerousGetObjectDataByteOffset(array, ref array.DangerousGetReferenceAt(offset));
            this.height = height;
            this.width = width;
            this.pitch = pitch;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Memory2D{T}"/> struct wrapping a 2D array.
        /// </summary>
        /// <param name="array">The given 2D array to wrap.</param>
        /// <exception cref="ArrayTypeMismatchException">
        /// Thrown when <paramref name="array"/> doesn't match <typeparamref name="T"/>.
        /// </exception>
        public Memory2D(T[,]? array)
        {
            if (array is null)
            {
                this = default;

                return;
            }

            if (array.IsCovariant())
            {
                ThrowHelper.ThrowArrayTypeMismatchException();
            }

            this.instance = array;
            this.offset = GetArray2DDataByteOffset<T>();
            this.height = array.GetLength(0);
            this.width = array.GetLength(1);
            this.pitch = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Memory2D{T}"/> struct wrapping a 2D array.
        /// </summary>
        /// <param name="array">The given 2D array to wrap.</param>
        /// <param name="row">The target row to map within <paramref name="array"/>.</param>
        /// <param name="column">The target column to map within <paramref name="array"/>.</param>
        /// <param name="height">The height to map within <paramref name="array"/>.</param>
        /// <param name="width">The width to map within <paramref name="array"/>.</param>
        /// <exception cref="ArrayTypeMismatchException">
        /// Thrown when <paramref name="array"/> doesn't match <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when either <paramref name="height"/>, <paramref name="width"/> or <paramref name="height"/>
        /// are negative or not within the bounds that are valid for <paramref name="array"/>.
        /// </exception>
        public Memory2D(T[,]? array, int row, int column, int height, int width)
        {
            if (array is null)
            {
                if (row != 0 || column != 0 || height != 0 || width != 0)
                {
                    ThrowHelper.ThrowArgumentException();
                }

                this = default;

                return;
            }

            if (array.IsCovariant())
            {
                ThrowHelper.ThrowArrayTypeMismatchException();
            }

            int
                rows = array.GetLength(0),
                columns = array.GetLength(1);

            if ((uint)row >= (uint)rows)
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForRow();
            }

            if ((uint)column >= (uint)columns)
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForColumn();
            }

            if ((uint)height > (uint)(rows - row))
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForHeight();
            }

            if ((uint)width > (uint)(columns - column))
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForWidth();
            }

            this.instance = array;
            this.offset = ObjectMarshal.DangerousGetObjectDataByteOffset(array, ref array.DangerousGetReferenceAt(row, column));
            this.height = height;
            this.width = width;
            this.pitch = columns - width;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Memory2D{T}"/> struct wrapping a layer in a 3D array.
        /// </summary>
        /// <param name="array">The given 3D array to wrap.</param>
        /// <param name="depth">The target layer to map within <paramref name="array"/>.</param>
        /// <exception cref="ArrayTypeMismatchException">
        /// Thrown when <paramref name="array"/> doesn't match <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when a parameter is invalid.</exception>
        public Memory2D(T[,,] array, int depth)
        {
            if (array.IsCovariant())
            {
                ThrowHelper.ThrowArrayTypeMismatchException();
            }

            if ((uint)depth >= (uint)array.GetLength(0))
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForDepth();
            }

            this.instance = array;
            this.offset = ObjectMarshal.DangerousGetObjectDataByteOffset(array, ref array.DangerousGetReferenceAt(depth, 0, 0));
            this.height = array.GetLength(1);
            this.width = array.GetLength(2);
            this.pitch = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Memory2D{T}"/> struct wrapping a layer in a 3D array.
        /// </summary>
        /// <param name="array">The given 3D array to wrap.</param>
        /// <param name="depth">The target layer to map within <paramref name="array"/>.</param>
        /// <param name="row">The target row to map within <paramref name="array"/>.</param>
        /// <param name="column">The target column to map within <paramref name="array"/>.</param>
        /// <param name="height">The height to map within <paramref name="array"/>.</param>
        /// <param name="width">The width to map within <paramref name="array"/>.</param>
        /// <exception cref="ArrayTypeMismatchException">
        /// Thrown when <paramref name="array"/> doesn't match <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when a parameter is invalid.</exception>
        public Memory2D(T[,,] array, int depth, int row, int column, int height, int width)
        {
            if (array.IsCovariant())
            {
                ThrowHelper.ThrowArrayTypeMismatchException();
            }

            if ((uint)depth >= (uint)array.GetLength(0))
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForDepth();
            }

            int
                rows = array.GetLength(1),
                columns = array.GetLength(2);

            if ((uint)row >= (uint)rows)
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForRow();
            }

            if ((uint)column >= (uint)columns)
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForColumn();
            }

            if ((uint)height > (uint)(rows - row))
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForHeight();
            }

            if ((uint)width > (uint)(columns - column))
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForWidth();
            }

            this.instance = array;
            this.offset = ObjectMarshal.DangerousGetObjectDataByteOffset(array, ref array.DangerousGetReferenceAt(depth, row, column));
            this.height = height;
            this.width = width;
            this.pitch = columns - width;
        }

#if SPAN_RUNTIME_SUPPORT
        /// <summary>
        /// Initializes a new instance of the <see cref="Memory2D{T}"/> struct.
        /// </summary>
        /// <param name="memoryManager">The target <see cref="MemoryManager{T}"/> to wrap.</param>
        /// <param name="height">The height of the resulting 2D area.</param>
        /// <param name="width">The width of each row in the resulting 2D area.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when either <paramref name="height"/> or <paramref name="width"/> are invalid.
        /// </exception>
        /// <remarks>The total area must match the length of <paramref name="memoryManager"/>.</remarks>
        public Memory2D(MemoryManager<T> memoryManager, int height, int width)
            : this(memoryManager, 0, height, width, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Memory2D{T}"/> struct.
        /// </summary>
        /// <param name="memoryManager">The target <see cref="MemoryManager{T}"/> to wrap.</param>
        /// <param name="offset">The initial offset within <paramref name="memoryManager"/>.</param>
        /// <param name="height">The height of the resulting 2D area.</param>
        /// <param name="width">The width of each row in the resulting 2D area.</param>
        /// <param name="pitch">The pitch in the resulting 2D area.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when one of the input parameters is out of range.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the requested area is outside of bounds for <paramref name="memoryManager"/>.
        /// </exception>
        public Memory2D(MemoryManager<T> memoryManager, int offset, int height, int width, int pitch)
        {
            int length = memoryManager.GetSpan().Length;

            if ((uint)offset > (uint)length)
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForOffset();
            }

            if (height < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForHeight();
            }

            if (width < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForWidth();
            }

            if (pitch < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForPitch();
            }

            if (width == 0 || height == 0)
            {
                this = default;

                return;
            }

            int
                area = OverflowHelper.ComputeInt32Area(height, width, pitch),
                remaining = length - offset;

            if (area > remaining)
            {
                ThrowHelper.ThrowArgumentException();
            }

            this.instance = memoryManager;
            this.offset = (nint)(uint)offset;
            this.height = height;
            this.width = width;
            this.pitch = pitch;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Memory2D{T}"/> struct.
        /// </summary>
        /// <param name="memory">The target <see cref="Memory{T}"/> to wrap.</param>
        /// <param name="height">The height of the resulting 2D area.</param>
        /// <param name="width">The width of each row in the resulting 2D area.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when either <paramref name="height"/> or <paramref name="width"/> are invalid.
        /// </exception>
        /// <remarks>The total area must match the length of <paramref name="memory"/>.</remarks>
        internal Memory2D(Memory<T> memory, int height, int width)
            : this(memory, 0, height, width, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Memory2D{T}"/> struct.
        /// </summary>
        /// <param name="memory">The target <see cref="Memory{T}"/> to wrap.</param>
        /// <param name="offset">The initial offset within <paramref name="memory"/>.</param>
        /// <param name="height">The height of the resulting 2D area.</param>
        /// <param name="width">The width of each row in the resulting 2D area.</param>
        /// <param name="pitch">The pitch in the resulting 2D area.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when one of the input parameters is out of range.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the requested area is outside of bounds for <paramref name="memory"/>.
        /// </exception>
        internal Memory2D(Memory<T> memory, int offset, int height, int width, int pitch)
        {
            if ((uint)offset > (uint)memory.Length)
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForOffset();
            }

            if (height < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForHeight();
            }

            if (width < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForWidth();
            }

            if (pitch < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForPitch();
            }

            if (width == 0 || height == 0)
            {
                this = default;

                return;
            }

            int
                area = OverflowHelper.ComputeInt32Area(height, width, pitch),
                remaining = memory.Length - offset;

            if (area > remaining)
            {
                ThrowHelper.ThrowArgumentException();
            }

            // Check if the Memory<T> instance wraps a string. This is possible in case
            // consumers do an unsafe cast for the entire Memory<T> object, and while not
            // really safe it is still supported in CoreCLR too, so we're following suit here.
            if (typeof(T) == typeof(char) &&
                MemoryMarshal.TryGetString(Unsafe.As<Memory<T>, Memory<char>>(ref memory), out string? text, out int textStart, out _))
            {
                ref char r0 = ref text.DangerousGetReferenceAt(textStart + offset);

                this.instance = text;
                this.offset = ObjectMarshal.DangerousGetObjectDataByteOffset(text, ref r0);
            }
            else if (MemoryMarshal.TryGetArray(memory, out ArraySegment<T> segment))
            {
                // Check if the input Memory<T> instance wraps an array we can access.
                // This is fine, since Memory<T> on its own doesn't control the lifetime
                // of the underlying array anyway, and this Memory2D<T> type would do the same.
                // Using the array directly makes retrieving a Span2D<T> faster down the line,
                // as we no longer have to jump through the boxed Memory<T> first anymore.
                T[] array = segment.Array!;

                this.instance = array;
                this.offset = ObjectMarshal.DangerousGetObjectDataByteOffset(array, ref array.DangerousGetReferenceAt(segment.Offset + offset));
            }
            else if (MemoryMarshal.TryGetMemoryManager<T, MemoryManager<T>>(memory, out var memoryManager, out int memoryManagerStart, out _))
            {
                this.instance = memoryManager;
                this.offset = (nint)(uint)(memoryManagerStart + offset);
            }
            else
            {
                ThrowHelper.ThrowArgumentExceptionForUnsupportedType();

                this.instance = null;
                this.offset = default;
            }

            this.height = height;
            this.width = width;
            this.pitch = pitch;
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="Memory2D{T}"/> struct with the specified parameters.
        /// </summary>
        /// <param name="instance">The target <see cref="object"/> instance.</param>
        /// <param name="offset">The initial offset within <see cref="instance"/>.</param>
        /// <param name="height">The height of the 2D memory area to map.</param>
        /// <param name="width">The width of the 2D memory area to map.</param>
        /// <param name="pitch">The pitch of the 2D memory area to map.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Memory2D(object instance, IntPtr offset, int height, int width, int pitch)
        {
            this.instance = instance;
            this.offset = offset;
            this.height = height;
            this.width = width;
            this.pitch = pitch;
        }

        /// <summary>
        /// Creates a new <see cref="Memory2D{T}"/> instance from an arbitrary object.
        /// </summary>
        /// <param name="instance">The <see cref="object"/> instance holding the data to map.</param>
        /// <param name="value">The target reference to point to (it must be within <paramref name="instance"/>).</param>
        /// <param name="height">The height of the 2D memory area to map.</param>
        /// <param name="width">The width of the 2D memory area to map.</param>
        /// <param name="pitch">The pitch of the 2D memory area to map.</param>
        /// <returns>A <see cref="Memory2D{T}"/> instance with the specified parameters.</returns>
        /// <remarks>The <paramref name="value"/> parameter is not validated, and it's responsibility of the caller to ensure it's valid.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when one of the input parameters is out of range.
        /// </exception>
        [Pure]
        public static Memory2D<T> DangerousCreate(object instance, ref T value, int height, int width, int pitch)
        {
            if (height < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForHeight();
            }

            if (width < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForWidth();
            }

            if (pitch < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForPitch();
            }

            OverflowHelper.EnsureIsInNativeIntRange(height, width, pitch);

            IntPtr offset = ObjectMarshal.DangerousGetObjectDataByteOffset(instance, ref value);

            return new Memory2D<T>(instance, offset, height, width, pitch);
        }

        /// <summary>
        /// Gets an empty <see cref="Memory2D{T}"/> instance.
        /// </summary>
        public static Memory2D<T> Empty => default;

        /// <summary>
        /// Gets a value indicating whether the current <see cref="Memory2D{T}"/> instance is empty.
        /// </summary>
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.height == 0 || this.width == 0;
        }

        /// <summary>
        /// Gets the length of the current <see cref="Memory2D{T}"/> instance.
        /// </summary>
        public nint Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (nint)(uint)this.height * (nint)(uint)this.width;
        }

        /// <summary>
        /// Gets the height of the underlying 2D memory area.
        /// </summary>
        public int Height
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.height;
        }

        /// <summary>
        /// Gets the width of the underlying 2D memory area.
        /// </summary>
        public int Width
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.width;
        }

        /// <summary>
        /// Gets a <see cref="Span2D{T}"/> instance from the current memory.
        /// </summary>
        public Span2D<T> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (this.instance is not null)
                {
#if SPAN_RUNTIME_SUPPORT
                    if (this.instance is MemoryManager<T> memoryManager)
                    {
                        ref T r0 = ref memoryManager.GetSpan().DangerousGetReference();
                        ref T r1 = ref Unsafe.Add(ref r0, this.offset);

                        return new Span2D<T>(ref r1, this.height, this.width, this.pitch);
                    }
                    else
                    {
                        ref T r0 = ref ObjectMarshal.DangerousGetObjectDataReferenceAt<T>(this.instance, this.offset);

                        return new Span2D<T>(ref r0, this.height, this.width, this.pitch);
                    }
#else
                    return new Span2D<T>(this.instance, this.offset, this.height, this.width, this.pitch);
#endif
                }

                return default;
            }
        }

#if NETSTANDARD2_1_OR_GREATER
        /// <summary>
        /// Slices the current instance with the specified parameters.
        /// </summary>
        /// <param name="rows">The target range of rows to select.</param>
        /// <param name="columns">The target range of columns to select.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when either <paramref name="rows"/> or <paramref name="columns"/> are invalid.
        /// </exception>
        /// <returns>A new <see cref="Memory2D{T}"/> instance representing a slice of the current one.</returns>
        public Memory2D<T> this[Range rows, Range columns]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var (row, height) = rows.GetOffsetAndLength(this.height);
                var (column, width) = columns.GetOffsetAndLength(this.width);

                return Slice(row, column, height, width);
            }
        }
#endif

        /// <summary>
        /// Slices the current instance with the specified parameters.
        /// </summary>
        /// <param name="row">The target row to map within the current instance.</param>
        /// <param name="column">The target column to map within the current instance.</param>
        /// <param name="height">The height to map within the current instance.</param>
        /// <param name="width">The width to map within the current instance.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when either <paramref name="height"/>, <paramref name="width"/> or <paramref name="height"/>
        /// are negative or not within the bounds that are valid for the current instance.
        /// </exception>
        /// <returns>A new <see cref="Memory2D{T}"/> instance representing a slice of the current one.</returns>
        [Pure]
        public Memory2D<T> Slice(int row, int column, int height, int width)
        {
            if ((uint)row >= Height)
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForRow();
            }

            if ((uint)column >= this.width)
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForColumn();
            }

            if ((uint)height > (Height - row))
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForHeight();
            }

            if ((uint)width > (this.width - column))
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionForWidth();
            }

            int
                shift = ((this.width + this.pitch) * row) + column,
                pitch = this.pitch + (this.width - width);

            IntPtr offset = this.offset + (shift * Unsafe.SizeOf<T>());

            return new Memory2D<T>(this.instance!, offset, height, width, pitch);
        }

        /// <summary>
        /// Copies the contents of this <see cref="Memory2D{T}"/> into a destination <see cref="Memory{T}"/> instance.
        /// </summary>
        /// <param name="destination">The destination <see cref="Memory{T}"/> instance.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="destination" /> is shorter than the source <see cref="Memory2D{T}"/> instance.
        /// </exception>
        public void CopyTo(Memory<T> destination) => Span.CopyTo(destination.Span);

        /// <summary>
        /// Attempts to copy the current <see cref="Memory2D{T}"/> instance to a destination <see cref="Memory{T}"/>.
        /// </summary>
        /// <param name="destination">The target <see cref="Memory{T}"/> of the copy operation.</param>
        /// <returns>Whether or not the operation was successful.</returns>
        public bool TryCopyTo(Memory<T> destination) => Span.TryCopyTo(destination.Span);

        /// <summary>
        /// Copies the contents of this <see cref="Memory2D{T}"/> into a destination <see cref="Memory2D{T}"/> instance.
        /// For this API to succeed, the target <see cref="Memory2D{T}"/> has to have the same shape as the current one.
        /// </summary>
        /// <param name="destination">The destination <see cref="Memory2D{T}"/> instance.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="destination" /> is shorter than the source <see cref="Memory2D{T}"/> instance.
        /// </exception>
        public void CopyTo(Memory2D<T> destination) => Span.CopyTo(destination.Span);

        /// <summary>
        /// Attempts to copy the current <see cref="Memory2D{T}"/> instance to a destination <see cref="Memory2D{T}"/>.
        /// For this API to succeed, the target <see cref="Memory2D{T}"/> has to have the same shape as the current one.
        /// </summary>
        /// <param name="destination">The target <see cref="Memory2D{T}"/> of the copy operation.</param>
        /// <returns>Whether or not the operation was successful.</returns>
        public bool TryCopyTo(Memory2D<T> destination) => Span.TryCopyTo(destination.Span);

        /// <summary>
        /// Creates a handle for the memory.
        /// The GC will not move the memory until the returned <see cref="MemoryHandle"/>
        /// is disposed, enabling taking and using the memory's address.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// An instance with nonprimitive (non-blittable) members cannot be pinned.
        /// </exception>
        /// <returns>A <see cref="MemoryHandle"/> instance wrapping the pinned handle.</returns>
        public unsafe MemoryHandle Pin()
        {
            if (this.instance is not null)
            {
                if (this.instance is MemoryManager<T> memoryManager)
                {
                    return memoryManager.Pin();
                }

                GCHandle handle = GCHandle.Alloc(this.instance, GCHandleType.Pinned);

                void* pointer = Unsafe.AsPointer(ref ObjectMarshal.DangerousGetObjectDataReferenceAt<T>(this.instance, this.offset));

                return new MemoryHandle(pointer, handle);
            }

            return default;
        }

        /// <summary>
        /// Tries to get a <see cref="Memory{T}"/> instance, if the underlying buffer is contiguous and small enough.
        /// </summary>
        /// <param name="memory">The resulting <see cref="Memory{T}"/>, in case of success.</param>
        /// <returns>Whether or not <paramref name="memory"/> was correctly assigned.</returns>
        public bool TryGetMemory(out Memory<T> memory)
        {
            if (this.pitch == 0 &&
                Length <= int.MaxValue)
            {
                // Empty Memory2D<T> instance
                if (this.instance is null)
                {
                    memory = default;
                }
                else if (typeof(T) == typeof(char) && this.instance.GetType() == typeof(string))
                {
                    string text = Unsafe.As<string>(this.instance)!;
                    int index = text.AsSpan().IndexOf(in ObjectMarshal.DangerousGetObjectDataReferenceAt<char>(text, this.offset));
                    ReadOnlyMemory<char> temp = text.AsMemory(index, (int)Length);

                    // The string type could still be present if a user ends up creating a
                    // Memory2D<T> instance from a string using DangerousCreate. Similarly to
                    // how CoreCLR handles the equivalent case in Memory<T>, here we just do
                    // the necessary steps to still retrieve a Memory<T> instance correctly
                    // wrapping the target string. In this case, it is up to the caller
                    // to make sure not to ever actually write to the resulting Memory<T>.
                    memory = MemoryMarshal.AsMemory<T>(Unsafe.As<ReadOnlyMemory<char>, Memory<T>>(ref temp));
                }
                else if (this.instance is MemoryManager<T> memoryManager)
                {
                    // If the object is a MemoryManager<T>, just slice it as needed
                    memory = memoryManager.Memory.Slice((int)(nint)this.offset, this.height * this.width);
                }
                else if (this.instance.GetType() == typeof(T[]))
                {
                    // If it's a T[] array, also handle the initial offset
                    T[] array = Unsafe.As<T[]>(this.instance)!;
                    int index = array.AsSpan().IndexOf(ref ObjectMarshal.DangerousGetObjectDataReferenceAt<T>(array, this.offset));

                    memory = array.AsMemory(index, this.height * this.width);
                }
#if SPAN_RUNTIME_SUPPORT
                else if (this.instance.GetType() == typeof(T[,]) ||
                         this.instance.GetType() == typeof(T[,,]))
                {
                    // If the object is a 2D or 3D array, we can create a Memory<T> from the RawObjectMemoryManager<T> type.
                    // We just need to use the precomputed offset pointing to the first item in the current instance,
                    // and the current usable length. We don't need to retrieve the current index, as the manager just offsets.
                    memory = new RawObjectMemoryManager<T>(this.instance, this.offset, this.height * this.width).Memory;
                }
#endif
                else
                {
                    // Reuse a single failure path to reduce
                    // the number of returns in the method
                    goto Failure;
                }

                return true;
            }

            Failure:

            memory = default;

            return false;
        }

        /// <summary>
        /// Copies the contents of the current <see cref="Memory2D{T}"/> instance into a new 2D array.
        /// </summary>
        /// <returns>A 2D array containing the data in the current <see cref="Memory2D{T}"/> instance.</returns>
        [Pure]
        public T[,] ToArray() => Span.ToArray();

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object? obj)
        {
            if (obj is Memory2D<T> memory)
            {
                return Equals(memory);
            }

            if (obj is ReadOnlyMemory2D<T> readOnlyMemory)
            {
                return readOnlyMemory.Equals(this);
            }

            return false;
        }

        /// <inheritdoc/>
        public bool Equals(Memory2D<T> other)
        {
            return
                this.instance == other.instance &&
                this.offset == other.offset &&
                this.height == other.height &&
                this.width == other.width &&
                this.pitch == other.pitch;
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            if (this.instance is not null)
            {
#if !NETSTANDARD1_4
                return HashCode.Combine(
                    RuntimeHelpers.GetHashCode(this.instance),
                    this.offset,
                    this.height,
                    this.width,
                    this.pitch);
#else
                Span<int> values = stackalloc int[]
                {
                    RuntimeHelpers.GetHashCode(this.instance),
                    this.offset.GetHashCode(),
                    this.height,
                    this.width,
                    this.pitch
                };

                return values.GetDjb2HashCode();
#endif
            }

            return 0;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"CommunityToolkit.HighPerformance.Memory2D<{typeof(T)}>[{this.height}, {this.width}]";
        }

        /// <summary>
        /// Defines an implicit conversion of an array to a <see cref="Memory2D{T}"/>
        /// </summary>
        public static implicit operator Memory2D<T>(T[,]? array) => new(array);
    }
}