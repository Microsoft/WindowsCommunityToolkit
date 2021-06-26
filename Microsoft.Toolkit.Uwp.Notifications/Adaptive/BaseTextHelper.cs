// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.Notifications.Adaptive.Elements;

namespace Microsoft.Toolkit.Uwp.Notifications
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