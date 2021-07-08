// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Notifications
{
    [NotificationXmlElement("tile")]
    internal sealed class Element_Tile : BaseElement
    {
        public Element_TileVisual Visual { get; set; }
    }
}