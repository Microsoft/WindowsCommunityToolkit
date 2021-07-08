// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Notifications.Adaptive.Elements;

namespace CommunityToolkit.WinUI.Notifications
{
    internal class BaseTextHelper
    {
        internal static Element_AdaptiveText CreateBaseElement(IBaseText current)
        {
            return new Element_AdaptiveText()
            {
                Text = current.Text,
                Lang = current.Language
            };
        }
    }
}