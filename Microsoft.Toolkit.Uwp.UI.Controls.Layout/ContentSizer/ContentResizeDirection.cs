// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Enum to indicate whether <see cref="ContentSizer"/> resizes Vertically or Horizontally.
    /// </summary>
    public enum ContentResizeDirection
    {
        /// <summary>
        /// Determines whether to resize rows or columns based on its Alignment and
        /// width compared to height
        /// </summary>
        Auto, // TODO: Detect?

        /// <summary>
        /// Resize columns when dragging Splitter.
        /// </summary>
        Vertical,

        /// <summary>
        /// Resize rows when dragging Splitter.
        /// </summary>
        Horizontal
    }
}
