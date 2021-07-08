// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.Notifications
{
    internal sealed class NotificationXmlAttributeAttribute : Attribute
    {
        public string Name { get; private set; }

        public object DefaultValue { get; private set; }

        public NotificationXmlAttributeAttribute(string name, object defaultValue = null)
        {
            Name = name;
            DefaultValue = defaultValue;
        }
    }
}