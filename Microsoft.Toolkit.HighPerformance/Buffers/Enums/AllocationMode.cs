// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.HighPerformance.Buffers
{
    /// <summary>
    /// An <see langword="enum"/> that indicates a mode to use when allocating buffers.
    /// </summary>
    public enum AllocationMode
    {
        /// <summary>
        /// The default allocation mode for pooled memory (rented buffers are not cleared).
        /// </summary>
        Default,

        /// <summary>
        /// Clear pooled buffers when renting them.
        /// </summary>
        Clear
    }
}