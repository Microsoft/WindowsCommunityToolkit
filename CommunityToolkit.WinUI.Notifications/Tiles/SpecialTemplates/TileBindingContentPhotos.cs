// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// Animates through a slide show of photos. Supported on all sizes.
    /// </summary>
    public sealed class TileBindingContentPhotos : ITileBindingContent
    {
        /// <summary>
        /// Gets the collection of slide show images. Up to 12 images can be provided (Mobile will only display up to 9), which will be used for the slide show. Adding more than 12 will throw an exception.
        /// </summary>
        public IList<TileBasicImage> Images { get; private set; } = new LimitedList<TileBasicImage>(12);

        internal TileTemplateNameV3 GetTemplateName(TileSize size)
        {
            return TileSizeToAdaptiveTemplateConverter.Convert(size);
        }

        internal void PopulateElement(Element_TileBinding binding, TileSize size)
        {
            binding.Presentation = TilePresentation.Photos;

            foreach (var img in Images)
            {
                binding.Children.Add(img.ConvertToElement());
            }
        }
    }
}