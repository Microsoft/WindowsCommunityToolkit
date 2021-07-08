// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.Notifications
{
    internal sealed class EnumStringAttribute : Attribute
    {
        public string String { get; }

        public EnumStringAttribute(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            String = s;
        }

        public override string ToString()
        {
            return String;
        }
    }
}