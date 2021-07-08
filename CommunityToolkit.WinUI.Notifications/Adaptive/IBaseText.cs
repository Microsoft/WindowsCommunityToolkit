// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// Defines the basic properties of a text element.
    /// </summary>
    public interface IBaseText
    {
        /// <summary>
        /// Gets or sets the text to display.
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Gets or sets the target locale of the XML payload, specified as a BCP-47 language tags such as "en-US" or "fr-FR". The locale specified here overrides any other specified locale, such as that in binding or visual. If this value is a literal string, this attribute defaults to the user's UI language. If this value is a string reference, this attribute defaults to the locale chosen by Windows Runtime in resolving the string.
        /// </summary>
        string Language { get; set; }
    }
}