// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINDOWS_UWP
using Windows.Data.Xml.Dom;
#endif

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// Notification content object to display a number on a Tile's badge.
    /// </summary>
    public sealed class BadgeNumericContent : INotificationContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BadgeNumericContent"/> class.
        /// Default constructor to create a numeric badge content object.
        /// </summary>
        public BadgeNumericContent()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BadgeNumericContent"/> class.
        /// Constructor to create a numeric badge content object with a number.
        /// </summary>
        /// <param name="number">
        /// The number that will appear on the badge.  If the number is 0, the badge
        /// will be removed.
        /// </param>
        public BadgeNumericContent(uint number)
        {
            _number = number;
        }

        /// <summary>
        /// Gets or sets the number that will appear on the badge. If the number is 0, the badge
        /// will be removed.
        /// </summary>
        public uint Number
        {
            get { return _number; }
            set { _number = value; }
        }

        /// <summary>
        /// Retrieves the notification Xml content as a string.
        /// </summary>
        /// <returns>The notification Xml content as a string.</returns>
        public string GetContent()
        {
            return string.Format("<badge value='{0}'/>", _number);
        }

        /// <summary>
        /// Retrieves the notification Xml content as a string.
        /// </summary>
        /// <returns>The notification Xml content as a string.</returns>
        public override string ToString()
        {
            return GetContent();
        }

#if WINDOWS_UWP
        /// <summary>
        /// Retrieves the notification Xml content as a WinRT Xml document.
        /// </summary>
        /// <returns>The notification Xml content as a WinRT Xml document.</returns>
        public XmlDocument GetXml()
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(GetContent());
            return xml;
        }
#endif

        private uint _number = 0;
    }
}