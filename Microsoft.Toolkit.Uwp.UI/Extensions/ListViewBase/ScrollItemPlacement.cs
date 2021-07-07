// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Uwp.UI
{
    /// <summary>
    /// Item Position
    /// </summary>
    public enum ScrollItemPlacement
    {
        /// <summary>
        /// If visible then it will not scroll, if not then item will be aligned to the nearest edge
        /// </summary>
        Default,

        /// <summary>
        /// Aligned left
        /// </summary>
        Left,

        /// <summary>
        /// Aligned top
        /// </summary>
        Top,

        /// <summary>
        /// Aligned center
        /// </summary>
        Center,

        /// <summary>
        /// Aligned right
        /// </summary>
        Right,

        /// <summary>
        /// Aligned bottom
        /// </summary>
        Bottom
    }
}
