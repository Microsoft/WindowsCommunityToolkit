// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.Notifications
{
    internal static class TileSizeToAdaptiveTemplateConverter
    {
        public static TileTemplateNameV3 Convert(TileSize size)
        {
            switch (size)
            {
                case TileSize.Small:
                    return TileTemplateNameV3.TileSmall;

                case TileSize.Medium:
                    return TileTemplateNameV3.TileMedium;

                case TileSize.Wide:
                    return TileTemplateNameV3.TileWide;

                case TileSize.Large:
                    return TileTemplateNameV3.TileLarge;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}