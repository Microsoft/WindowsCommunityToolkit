// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Notifications
{
    internal enum TilePresentation
    {
        [EnumString("people")]
        People,

        [EnumString("photos")]
        Photos,

        [EnumString("contact")]
        Contact
    }

    internal enum TileImagePlacement
    {
        [EnumString("inline")]
        Inline,

        [EnumString("background")]
        Background,

        [EnumString("peek")]
        Peek
    }
}