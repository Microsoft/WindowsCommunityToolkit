// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// Specifies content you want to appear in a My People shoulder tap notification. For more info, see the My People notifications documentation. New in Fall Creators Update.
    /// </summary>
    public sealed class ToastBindingShoulderTap
    {
        /// <summary>
        /// Gets or sets the image to be displayed in the shoulder tap notification. Required.
        /// </summary>
        public ToastShoulderTapImage Image { get; set; }

        /// <summary>
        /// Gets or sets the target locale of the XML payload, specified as BCP-47 language tags such as "en-US"
        /// or "fr-FR". This locale is overridden by any locale specified in binding or text. If this value is
        /// a literal string, this attribute defaults to the user's UI language. If this value is a string reference,
        /// this attribute defaults to the locale chosen by Windows Runtime in resolving the string.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets a default base URI that is combined with relative URIs in image source attributes.
        /// </summary>
        public Uri BaseUri { get; set; }

        /// <summary>
        /// Gets or sets a value whether Windows is allowed to append a query string to the image URI supplied in the Toast notification. Use this attribute if your server hosts images and can handle query strings, either by retrieving an image variant based on the query strings or by ignoring the query string and returning the image as specified without the query string. This query string specifies scale, contrast setting, and language.
        /// </summary>
        public bool? AddImageQuery { get; set; }

        internal Element_ToastBinding ConvertToElement()
        {
            Element_ToastBinding binding = new Element_ToastBinding(ToastTemplateType.ToastGeneric)
            {
                ExperienceType = "shoulderTap",
                BaseUri = BaseUri,
                AddImageQuery = AddImageQuery,
                Language = Language
            };

            // If there's an image, add it
            if (Image != null)
            {
                binding.Children.Add(Image.ConvertToElement());
            }

            return binding;
        }
    }
}