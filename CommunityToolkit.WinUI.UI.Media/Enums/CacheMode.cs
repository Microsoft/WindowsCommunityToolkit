// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.UI.Media
{
    /// <summary>
    /// Indicates the cache mode to use when loading a Win2D image
    /// </summary>
    public enum CacheMode
    {
        /// <summary>
        /// The default behavior, the cache is enabled
        /// </summary>
        Default,

        /// <summary>
        /// Reload the target image and overwrite the cached entry, if it exists
        /// </summary>
        Overwrite,

        /// <summary>
        /// The cache is disabled and new images are always reloaded
        /// </summary>
        Disabled
    }
}