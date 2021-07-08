// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.UI.Media
{
    /// <summary>
    /// Specifies the way in which an alpha channel affects color channels.
    /// </summary>
    public enum AlphaMode
    {
        /// <summary>
        /// Provides better transparent effects without a white bloom.
        /// </summary>
        Premultiplied = 0,

        /// <summary>
        /// WPF default handling of alpha channel during transparent blending.
        /// </summary>
        Straight = 1,
    }
}