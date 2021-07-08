// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Notifications.Adaptive.Elements
{
    internal enum AdaptiveImagePlacement
    {
        [EnumString("inline")]
        Inline,

        [EnumString("background")]
        Background,

        [EnumString("peek")]
        Peek,

        [EnumString("hero")]
        Hero,

        [EnumString("appLogoOverride")]
        AppLogoOverride
    }
}