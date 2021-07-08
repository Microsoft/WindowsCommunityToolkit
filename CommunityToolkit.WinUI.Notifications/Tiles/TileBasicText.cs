// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Notifications.Adaptive.Elements;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// A text element on the Tile.
    /// </summary>
    public sealed class TileBasicText
    {
        /// <summary>
        /// Gets or sets the text value that will be shown in the text field.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the target locale of the XML payload, specified as a BCP-47 language tags such as "en-US" or "fr-FR". The locale specified here overrides any other specified locale, such as that in binding or visual. If this value is a literal string, this attribute defaults to the user's UI language. If this value is a string reference, this attribute defaults to the locale chosen by Windows Runtime in resolving the string.
        /// </summary>
        public string Lang { get; set; }

        internal Element_AdaptiveText ConvertToElement()
        {
            return new Element_AdaptiveText()
            {
                Text = Text,
                Lang = Lang
            };
        }

        /// <summary>
        /// Returns the Text property's value.
        /// </summary>
        /// <returns>The Text property's value.</returns>
        public override string ToString()
        {
            return Text;
        }
    }
}