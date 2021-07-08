// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
#if WINDOWS_UWP
using Windows.Data.Xml.Dom;
#endif

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// Notification content object to display a glyph on a Tile's badge.
    /// </summary>
    public sealed class BadgeGlyphContent : INotificationContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BadgeGlyphContent"/> class.
        /// Default constructor to create a glyph badge content object.
        /// </summary>
        public BadgeGlyphContent()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BadgeGlyphContent"/> class.
        /// Constructor to create a glyph badge content object with a glyph.
        /// </summary>
        /// <param name="glyph">The glyph to be displayed on the badge.</param>
        public BadgeGlyphContent(BadgeGlyphValue glyph)
        {
            _glyph = glyph;
        }

        /// <summary>
        /// Gets or sets the glyph to be displayed on the badge.
        /// </summary>
        public BadgeGlyphValue Glyph
        {
            get
            {
                return _glyph;
            }

            set
            {
                if (!Enum.IsDefined(typeof(BadgeGlyphValue), value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _glyph = value;
            }
        }

        /// <summary>
        /// Retrieves the notification Xml content as a string.
        /// </summary>
        /// <returns>The notification Xml content as a string.</returns>
        public string GetContent()
        {
            if (!Enum.IsDefined(typeof(BadgeGlyphValue), _glyph))
            {
                throw new NotificationContentValidationException("The badge glyph property was left unset.");
            }

            string glyphString = _glyph.ToString();

            // lower case the first character of the enum value to match the Xml schema
            glyphString = string.Format("{0}{1}", char.ToLowerInvariant(glyphString[0]), glyphString.Substring(1));
            return string.Format("<badge value='{0}'/>", glyphString);
        }

        /// <summary>
        /// Retrieves the notification XML content as a string.
        /// </summary>
        /// <returns>The notification XML content as a string.</returns>
        public override string ToString()
        {
            return GetContent();
        }

#if WINDOWS_UWP
        /// <summary>
        /// Retrieves the notification XML content as a WinRT Xml document.
        /// </summary>
        /// <returns>The notification XML content as a WinRT Xml document.</returns>
        public XmlDocument GetXml()
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(GetContent());
            return xml;
        }
#endif

        private BadgeGlyphValue _glyph = (BadgeGlyphValue)(-1);
    }
}