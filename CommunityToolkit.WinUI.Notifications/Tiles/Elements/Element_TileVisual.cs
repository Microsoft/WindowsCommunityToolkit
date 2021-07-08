// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace CommunityToolkit.WinUI.Notifications
{
    [NotificationXmlElement("visual")]
    internal sealed class Element_TileVisual
    {
        internal const TileBranding DEFAULT_BRANDING = TileBranding.Auto;
        internal const bool DEFAULT_ADD_IMAGE_QUERY = false;

        [NotificationXmlAttribute("addImageQuery")]
        public bool? AddImageQuery { get; set; }

        [NotificationXmlAttribute("baseUri")]
        public Uri BaseUri { get; set; }

        [NotificationXmlAttribute("branding", DEFAULT_BRANDING)]
        public TileBranding Branding { get; set; } = DEFAULT_BRANDING;

        [NotificationXmlAttribute("contentId")]
        public string ContentId { get; set; }

        [NotificationXmlAttribute("displayName")]
        public string DisplayName { get; set; }

        [NotificationXmlAttribute("lang")]
        public string Language { get; set; }

        [NotificationXmlAttribute("arguments")]
        public string Arguments { get; set; }

        public IList<Element_TileBinding> Bindings { get; private set; } = new List<Element_TileBinding>();
    }
}