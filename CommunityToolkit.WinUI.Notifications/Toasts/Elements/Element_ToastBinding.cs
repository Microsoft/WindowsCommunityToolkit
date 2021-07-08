// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace CommunityToolkit.WinUI.Notifications
{
    [NotificationXmlElement("binding")]
    internal sealed class Element_ToastBinding
    {
        public Element_ToastBinding(ToastTemplateType template)
        {
            Template = template;
        }

        [NotificationXmlAttribute("template")]
        public ToastTemplateType Template { get; private set; }

        /// <summary>
        /// Gets or sets a value whether Windows should append a query string to the image URI supplied in the Tile notification. Use this attribute if your server hosts images and can handle query strings, either by retrieving an image variant based on the query strings or by ignoring the query string and returning the image as specified without the query string. This query string specifies scale, contrast setting, and language; for instance, a value of
        ///
        /// "www.website.com/images/hello.png"
        ///
        /// included in the notification becomes
        ///
        /// "www.website.com/images/hello.png?ms-scale=100&amp;ms-contrast=standard&amp;ms-lang=en-us"
        /// </summary>
        [NotificationXmlAttribute("addImageQuery")]
        public bool? AddImageQuery { get; set; }

        /// <summary>
        /// Gets or sets a default base URI that is combined with relative URIs in image source attributes.
        /// </summary>
        [NotificationXmlAttribute("baseUri")]
        public Uri BaseUri { get; set; }

        /// <summary>
        /// Gets or sets the target locale of the XML payload, specified as a BCP-47 language tags such as "en-US" or "fr-FR". The locale specified here overrides that in visual, but can be overridden by that in text. If this value is a literal string, this attribute defaults to the user's UI language. If this value is a string reference, this attribute defaults to the locale chosen by Windows Runtime in resolving the string. See Remarks for when this value isn't specified.
        /// </summary>
        [NotificationXmlAttribute("lang")]
        public string Language { get; set; }

        [NotificationXmlAttribute("experienceType")]
        public string ExperienceType { get; set; }

        public IList<IElement_ToastBindingChild> Children { get; private set; } = new List<IElement_ToastBindingChild>();
    }

    internal interface IElement_ToastBindingChild
    {
    }

    internal enum ToastTemplateType
    {
        ToastGeneric,
        ToastImageAndText01,
        ToastImageAndText02,
        ToastImageAndText03,
        ToastImageAndText04,
        ToastText01,
        ToastText02,
        ToastText03,
        ToastText04
    }
}