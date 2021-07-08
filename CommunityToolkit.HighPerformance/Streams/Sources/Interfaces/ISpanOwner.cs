// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.HighPerformance.Streams
{
    /// <summary>
    /// An interface for types acting as sources for <see cref="Span{T}"/> instances.
    /// </summary>
    internal interface ISpanOwner
    {
        /// <summary>
        /// Gets the length of the underlying memory area.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets a <see cref="Span{T}"/> instance wrapping the underlying memory area.
        /// </summary>
        Span<byte> Span { get; }
    }
}