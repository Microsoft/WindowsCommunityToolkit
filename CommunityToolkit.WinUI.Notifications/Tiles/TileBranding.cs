// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// The form that the Tile should use to display the app's brand.
    /// </summary>
    public enum TileBranding
    {
        /// <summary>
        /// The default choice. If ShowNameOn___ is true for the Tile size being displayed, then branding will be "Name". Otherwise it will be "None".
        /// </summary>
        Auto,

        /// <summary>
        /// No branding will be displayed.
        /// </summary>
        [EnumString("none")]
        None,

        /// <summary>
        /// The DisplayName will be shown.
        /// </summary>
        [EnumString("name")]
        Name,

        /// <summary>
        /// Desktop-only. The Square44x44Logo will be shown. On Mobile, this will fallback to Name.
        /// </summary>
        [EnumString("logo")]
        Logo,

        /// <summary>
        /// Desktop-only. Both the DisplayName and Square44x44Logo will be shown. On Mobile, this will fallback to Name.
        /// </summary>
        [EnumString("nameAndLogo")]
        NameAndLogo
    }
}