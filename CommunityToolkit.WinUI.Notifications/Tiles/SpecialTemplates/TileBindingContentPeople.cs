// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// New in 1511: Supported on Medium, Wide, and Large (Desktop and Mobile).
    /// Previously for RTM: Phone-only. Supported on Medium and Wide.
    /// </summary>
    public sealed class TileBindingContentPeople : ITileBindingContent
    {
        /// <summary>
        /// Gets images that will roll around as circles.
        /// </summary>
        public IList<TileBasicImage> Images { get; private set; } = new List<TileBasicImage>();

        internal TileTemplateNameV3 GetTemplateName(TileSize size)
        {
            return TileSizeToAdaptiveTemplateConverter.Convert(size);
        }

        internal void PopulateElement(Element_TileBinding binding, TileSize size)
        {
            binding.Presentation = TilePresentation.People;

            foreach (var img in Images)
            {
                binding.Children.Add(img.ConvertToElement());
            }
        }
    }
}