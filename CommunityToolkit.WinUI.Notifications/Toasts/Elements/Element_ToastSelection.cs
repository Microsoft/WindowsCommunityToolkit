// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Notifications
{
    [NotificationXmlElement("selection")]
    internal sealed class Element_ToastSelection : IElement_ToastInputChild
    {
        /// <summary>
        /// Gets or sets the id attribute for apps to retrieve back the user selected input after the app is activated. Required
        /// </summary>
        [NotificationXmlAttribute("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the text to display for this selection element.
        /// </summary>
        [NotificationXmlAttribute("content")]
        public string Content { get; set; }
    }
}