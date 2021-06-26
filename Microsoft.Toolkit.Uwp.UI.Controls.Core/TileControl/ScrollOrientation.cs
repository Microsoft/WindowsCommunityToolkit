// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Orientation of the scroll
    /// </summary>
    public enum ScrollOrientation
    {
        /// <summary>
        /// Scroll only Horizontally (and optimize the number of image used)
        /// </summary>
        Horizontal,

        /// <summary>
        /// Scroll only Vertically (and optimize the number of image used)
        /// </summary>
        Vertical,

        /// <summary>
        /// Scroll both Horizontally and vertically
        /// </summary>
        Both
    }
}