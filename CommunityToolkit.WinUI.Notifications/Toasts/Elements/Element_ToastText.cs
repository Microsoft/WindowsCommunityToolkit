// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Notifications
{
    [NotificationXmlElement("text")]
    internal sealed class Element_ToastText : IElement_ToastBindingChild
    {
        internal const ToastTextPlacement DEFAULT_PLACEMENT = ToastTextPlacement.Inline;

        [NotificationXmlContent]
        public string Text { get; set; }

        [NotificationXmlAttribute("lang")]
        public string Lang { get; set; }

        [NotificationXmlAttribute("placement", DEFAULT_PLACEMENT)]
        public ToastTextPlacement Placement { get; set; } = DEFAULT_PLACEMENT;
    }

    internal enum ToastTextPlacement
    {
        Inline,

        [EnumString("attribution")]
        Attribution
    }
}