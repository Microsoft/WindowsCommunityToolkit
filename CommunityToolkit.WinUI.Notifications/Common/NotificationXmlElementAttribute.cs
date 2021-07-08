// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.Notifications
{
    internal sealed class NotificationXmlElementAttribute : Attribute
    {
        public string Name { get; private set; }

        public NotificationXmlElementAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name cannot be null or whitespace");
            }

            Name = name;
        }
    }
}