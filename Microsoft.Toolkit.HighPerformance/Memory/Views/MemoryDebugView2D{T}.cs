// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace Microsoft.Toolkit.HighPerformance.Memory.Views
{
    /// <summary>
    /// A debug proxy used to display items in a 2D layout.
    /// </summary>
    /// <typeparam name="T">The type of items to display.</typeparam>
    internal sealed class MemoryDebugView2D<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryDebugView2D{T}"/> class with the specified parameters.
        /// </summary>
        /// <param name="memory">The input <see cref="Memory2D{T}"/> instance with the items to display.</param>
        public MemoryDebugView2D(Memory2D<T> memory)
        {
            this.Items = memory.ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryDebugView2D{T}"/> class with the specified parameters.
        /// </summary>
        /// <param name="memory">The input <see cref="ReadOnlyMemory2D{T}"/> instance with the items to display.</param>
        public MemoryDebugView2D(ReadOnlyMemory2D<T> memory)
        {
            this.Items = memory.ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryDebugView2D{T}"/> class with the specified parameters.
        /// </summary>
        /// <param name="span">The input <see cref="Span2D{T}"/> instance with the items to display.</param>
        public MemoryDebugView2D(Span2D<T> span)
        {
            this.Items = span.ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryDebugView2D{T}"/> class with the specified parameters.
        /// </summary>
        /// <param name="span">The input <see cref="ReadOnlySpan2D{T}"/> instance with the items to display.</param>
        public MemoryDebugView2D(ReadOnlySpan2D<T> span)
        {
            this.Items = span.ToArray();
        }

        /// <summary>
        /// Gets the items to display for the current instance
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public T[,]? Items { get; }
    }
}