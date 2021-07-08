// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace CommunityToolkit.WinUI.Notifications
{
    [NotificationXmlElement("input")]
    internal sealed class Element_ToastInput : IElement_ToastActionsChild
    {
        /// <summary>
        /// Gets or sets the required attributes for developers to retrieve user inputs once the app is activated (in the foreground or background).
        /// </summary>
        [NotificationXmlAttribute("id")]
        public string Id { get; set; }

        [NotificationXmlAttribute("type")]
        public ToastInputType Type { get; set; }

        /// <summary>
        /// Gets or sets the optional title attribute and is for developers to specify a title for the input for shells to render when there is affordance.
        /// </summary>
        [NotificationXmlAttribute("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the optional placeholderContent attribute and is the grey-out hint text for text input type. This attribute is ignored when the input type is not �text�.
        /// </summary>
        [NotificationXmlAttribute("placeHolderContent")]
        public string PlaceholderContent { get; set; }

        /// <summary>
        /// Gets or sets the optional defaultInput attribute and it allows developer to provide a default input value.
        /// </summary>
        [NotificationXmlAttribute("defaultInput")]
        public string DefaultInput { get; set; }

        public IList<IElement_ToastInputChild> Children { get; private set; } = new List<IElement_ToastInputChild>();
    }

    internal interface IElement_ToastInputChild
    {
    }

    internal enum ToastInputType
    {
        [EnumString("text")]
        Text,

        [EnumString("selection")]
        Selection
    }
}