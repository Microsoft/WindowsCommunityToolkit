// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// Defines the visual aspects of a Toast notification.
    /// </summary>
    public sealed class ToastVisual
    {
        /// <summary>
        /// Gets or sets the target locale of the XML payload, specified as BCP-47 language tags such as "en-US" or "fr-FR". This locale is overridden by any locale specified in binding or text. If this value is a literal string, this attribute defaults to the user's UI language. If this value is a string reference, this attribute defaults to the locale chosen by Windows Runtime in resolving the string.
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

        /// <summary>
        /// Gets or sets the generic Toast binding, which can be rendered on all devices. This binding is required and cannot be null.
        /// </summary>
        public ToastBindingGeneric BindingGeneric { get; set; }

        /// <summary>
        /// Gets or sets a binding for shoulder tap notifications, which integrate with My People. See the My People documentation for more info. New in Fall Creators Update.
        /// </summary>
        public ToastBindingShoulderTap BindingShoulderTap { get; set; }

        internal Element_ToastVisual ConvertToElement()
        {
            var visual = new Element_ToastVisual()
            {
                Language = Language,
                BaseUri = BaseUri,
                AddImageQuery = AddImageQuery
            };

            if (BindingGeneric == null)
            {
                throw new NullReferenceException("BindingGeneric must be initialized");
            }

            Element_ToastBinding binding = BindingGeneric.ConvertToElement();

            // TODO: If a BaseUri wasn't provided, we can potentially optimize the payload size by calculating the best BaseUri
            visual.Bindings.Add(binding);

            if (BindingShoulderTap != null)
            {
                visual.Bindings.Add(BindingShoulderTap.ConvertToElement());
            }

            return visual;
        }
    }
}