// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// Phone-only. Supported on Small, Medium, and Wide.
    /// </summary>
    public sealed class TileBindingContentContact : ITileBindingContent
    {
        /// <summary>
        /// Gets or sets the image to display.
        /// </summary>
        public TileBasicImage Image { get; set; }

        /// <summary>
        /// Gets or sets a line of text that is displayed. Not displayed on Small Tile.
        /// </summary>
        public TileBasicText Text { get; set; }

        internal TileTemplateNameV3 GetTemplateName(TileSize size)
        {
            return TileSizeToAdaptiveTemplateConverter.Convert(size);
        }

        internal void PopulateElement(Element_TileBinding binding, TileSize size)
        {
            binding.Presentation = TilePresentation.Contact;

            // Small size doesn't display the text, so no reason to include it in the payload
            if (Text != null && size != TileSize.Small)
            {
                binding.Children.Add(Text.ConvertToElement());
            }

            if (Image != null)
            {
                binding.Children.Add(Image.ConvertToElement());
            }
        }
    }
}