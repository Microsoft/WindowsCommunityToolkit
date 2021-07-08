// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// Supported on Small and Medium. Enables an iconic Tile template, where you can have an icon and badge display next to each other on the Tile, in true classic Windows Phone style. The number next to the icon is achieved through a separate badge notification.
    /// </summary>
    public sealed class TileBindingContentIconic : ITileBindingContent
    {
        /// <summary>
        /// Gets or sets, at minimum, to support both Desktop and Phone, Small and Medium tiles, a square aspect ratio image with a resolution of 200x200, PNG format, with transparency and no color other than white. For more info see: http://blogs.msdn.com/b/tiles_and_toasts/archive/2015/07/31/iconic-tile-template-for-windows-10.aspx
        /// </summary>
        public TileBasicImage Icon { get; set; }

        internal TileTemplateNameV3 GetTemplateName(TileSize size)
        {
            switch (size)
            {
                case TileSize.Small:
                    return TileTemplateNameV3.TileSquare71x71IconWithBadge;

                case TileSize.Medium:
                    return TileTemplateNameV3.TileSquare150x150IconWithBadge;

                default:
                    throw new ArgumentException("The Iconic template is only supported on Small and Medium tiles.");
            }
        }

        internal void PopulateElement(Element_TileBinding binding, TileSize size)
        {
            if (Icon != null)
            {
                var element = Icon.ConvertToElement();
                element.Id = 1;
                binding.Children.Add(element);
            }
        }
    }
}