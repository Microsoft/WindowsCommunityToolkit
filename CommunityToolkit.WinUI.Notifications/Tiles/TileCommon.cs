// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// Represent the all tile sizes that are available.
    /// </summary>
    [Flags]
    public enum TileSize
    {
        /// <summary>
        /// Small Square Tile
        /// </summary>
        Small = 1,

        /// <summary>
        /// Medium Square Tile
        /// </summary>
        Medium = 2,

        /// <summary>
        /// Wide Rectangle Tile
        /// </summary>
        Wide = 4,

        /// <summary>
        /// Large Square Tile
        /// </summary>
        Large = 8
    }
}