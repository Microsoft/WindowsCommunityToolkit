// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.Notifications.Adaptive.Elements;

namespace Microsoft.Toolkit.Uwp.Notifications
{
    /// <summary>
    /// Defines an attribution text element to be displayed on the Toast notification.
    /// </summary>
    public sealed class ToastGenericAttributionText : IBaseText
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToastGenericAttributionText"/> class.
        /// An attribution text element to be displayed on the Toast notification.
        /// </summary>
        public ToastGenericAttributionText()
        {
        }

        /// <summary>
        /// Gets or sets the text to display.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the target locale of the XML payload, specified as a BCP-47 language tags such as "en-US" or "fr-FR". The locale specified here overrides any other specified locale, such as that in binding or visual. If this value is a literal string, this attribute defaults to the user's UI language. If this value is a string reference, this attribute defaults to the locale chosen by Windows Runtime in resolving the string.
        /// </summary>
        public string Language { get; set; }

        internal Element_AdaptiveText ConvertToElement()
        {
            var el = BaseTextHelper.CreateBaseElement(this);

            el.Placement = AdaptiveTextPlacement.Attribution;

            return el;
        }
    }
}