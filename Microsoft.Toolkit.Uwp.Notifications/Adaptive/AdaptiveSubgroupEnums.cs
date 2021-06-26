// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Uwp.Notifications
{
    /// <summary>
    /// TextStacking specifies the vertical alignment of content.
    /// </summary>
    public enum AdaptiveSubgroupTextStacking
    {
        /// <summary>
        /// Renderer automatically selects the default vertical alignment.
        /// </summary>
        Default,

        /// <summary>
        /// Vertical align to the top.
        /// </summary>
        [EnumString("top")]
        Top,

        /// <summary>
        /// Vertical align to the center.
        /// </summary>
        [EnumString("center")]
        Center,

        /// <summary>
        /// Vertical align to the bottom.
        /// </summary>
        [EnumString("bottom")]
        Bottom
    }
}