// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using CommunityToolkit.WinUI.Notifications.Adaptive;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// Supported on all sizes. This is the recommended way of specifying your Tile content. Adaptive Tile templates are the de-facto choice for Windows 10, and you can create a wide variety of custom Tiles through adaptive.
    /// </summary>
    public sealed class TileBindingContentAdaptive : ITileBindingContent
    {
        /// <summary>
        /// Gets <see cref="AdaptiveText"/>, <see cref="AdaptiveImage"/>, and <see cref="AdaptiveGroup"/> objects that can be added as children. The children are displayed in a vertical StackPanel fashion.
        /// </summary>
        public IList<ITileBindingContentAdaptiveChild> Children { get; private set; } = new List<ITileBindingContentAdaptiveChild>();

        /// <summary>
        /// Gets or sets an optional background image that gets displayed behind all the Tile content, full bleed.
        /// </summary>
        public TileBackgroundImage BackgroundImage { get; set; }

        /// <summary>
        /// Gets or sets an optional peek image that animates in from the top of the Tile.
        /// </summary>
        public TilePeekImage PeekImage { get; set; }

        /// <summary>
        /// Gets or sets the text stacking (vertical alignment) of the entire binding element.
        /// </summary>
        public TileTextStacking TextStacking { get; set; } = Element_TileBinding.DEFAULT_TEXT_STACKING;

        internal TileTemplateNameV3 GetTemplateName(TileSize size)
        {
            return TileSizeToAdaptiveTemplateConverter.Convert(size);
        }

        internal void PopulateElement(Element_TileBinding binding, TileSize size)
        {
            // Assign properties
            binding.TextStacking = TextStacking;

            // Add the background image if there's one
            if (BackgroundImage != null)
            {
                // And add it as a child
                binding.Children.Add(BackgroundImage.ConvertToElement());
            }

            // Add the peek image if there's one
            if (PeekImage != null)
            {
                var el = PeekImage.ConvertToElement();

                binding.Children.Add(el);
            }

            // And then add all the children
            foreach (var child in Children)
            {
                binding.Children.Add(ConvertToBindingChildElement(child));
            }
        }

        private static IElement_TileBindingChild ConvertToBindingChildElement(ITileBindingContentAdaptiveChild child)
        {
            return (IElement_TileBindingChild)AdaptiveHelper.ConvertToElement(child);
        }
    }
}