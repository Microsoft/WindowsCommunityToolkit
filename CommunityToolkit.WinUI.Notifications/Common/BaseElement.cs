// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Text;
#if WINDOWS_UWP
using Windows.Data.Xml.Dom;
#endif

namespace CommunityToolkit.WinUI.Notifications
{
    internal abstract class BaseElement
    {
        /// <summary>
        /// Retrieves the notification XML content as a string.
        /// </summary>
        /// <returns>The notification XML content as a string.</returns>
        public string GetContent()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (global::System.Xml.XmlWriter writer = global::System.Xml.XmlWriter.Create(stream, new global::System.Xml.XmlWriterSettings()
                {
                    Encoding = Encoding.UTF8, // Use UTF-8 encoding to save space (it defaults to UTF-16 which is 2x the size)
                    Indent = false,
                    NewLineOnAttributes = false
                }))
                {
                    XmlWriterHelper.Write(writer, this);
                }

                stream.Position = 0;

                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

#if WINDOWS_UWP
        /// <summary>
        /// Retrieves the notification XML content as a WinRT XML document.
        /// </summary>
        /// <returns>The notification XML content as a WinRT XML document.</returns>
        public XmlDocument GetXml()
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(GetContent());
            return xml;
        }
#endif
    }
}