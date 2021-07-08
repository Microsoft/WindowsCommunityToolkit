// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.Notifications
{
#if !WINRT

    /// <summary>
    /// Builder class used to create <see cref="TileContent"/>
    /// </summary>
    public partial class TileContentBuilder
    {
        /// <summary>
        /// Flag used to create all tile size (Small , Medium, Large and Wide)
        /// </summary>
        public const TileSize AllSize = TileSize.Small | TileSize.Medium | TileSize.Large | TileSize.Wide;

        /// <summary>
        /// Gets internal instance of <see cref="TileContent"/>. This is equivalent to the call to <see cref="TileContentBuilder.GetTileContent"/>.
        /// </summary>
        public TileContent Content
        {
            get; private set;
        }

        private TileVisual Visual
        {
            get
            {
                if (Content.Visual == null)
                {
                    Content.Visual = new TileVisual();
                }

                return Content.Visual;
            }
        }

        private TileBinding SmallTile
        {
            get
            {
                return Visual.TileSmall;
            }

            set
            {
                Visual.TileSmall = value;
            }
        }

        private TileBinding MediumTile
        {
            get
            {
                return Visual.TileMedium;
            }

            set
            {
                Visual.TileMedium = value;
            }
        }

        private TileBinding WideTile
        {
            get
            {
                return Visual.TileWide;
            }

            set
            {
                Visual.TileWide = value;
            }
        }

        private TileBinding LargeTile
        {
            get
            {
                return Visual.TileLarge;
            }

            set
            {
                Visual.TileLarge = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TileContentBuilder"/> class.
        /// </summary>
        public TileContentBuilder()
        {
            Content = new TileContent();
        }

        /// <summary>
        /// Add a tile layout size that the notification will be displayed on.
        /// </summary>
        /// <param name="size">The size of tile that the notification will be displayed on.</param>
        /// <param name="tileContent">Specialized tile content. Use for special tile template. Default to NULL.</param>
        /// <returns>The current instance of <see cref="TileContentBuilder"/></returns>
        public TileContentBuilder AddTile(TileSize size, ITileBindingContent tileContent = null)
        {
            if (size.HasFlag(TileSize.Small))
            {
                SmallTile = new TileBinding();
                SmallTile.Content = tileContent ?? new TileBindingContentAdaptive();
            }

            if (size.HasFlag(TileSize.Medium))
            {
                MediumTile = new TileBinding();
                MediumTile.Content = tileContent ?? new TileBindingContentAdaptive();
            }

            if (size.HasFlag(TileSize.Wide))
            {
                WideTile = new TileBinding();
                WideTile.Content = tileContent ?? new TileBindingContentAdaptive();
            }

            if (size.HasFlag(TileSize.Large))
            {
                LargeTile = new TileBinding();
                LargeTile.Content = tileContent ?? new TileBindingContentAdaptive();
            }

            return this;
        }

        /// <summary>
        /// Set how the tile notification should display the application branding.
        /// </summary>
        /// <param name="branding">How branding should appear on the tile</param>
        /// <param name="size">The tile size that the <paramref name="branding"/> parameter should be applied to. Default to all currently supported tile size.</param>
        /// <returns>The current instance of <see cref="TileContentBuilder"/></returns>
        public TileContentBuilder SetBranding(TileBranding branding, TileSize size = AllSize)
        {
            if (size == AllSize)
            {
                // Set on visual.
                Visual.Branding = branding;
            }
            else
            {
                if (size.HasFlag(TileSize.Small) && SmallTile != null)
                {
                    SmallTile.Branding = branding;
                }

                if (size.HasFlag(TileSize.Medium) && MediumTile != null)
                {
                    MediumTile.Branding = branding;
                }

                if (size.HasFlag(TileSize.Wide) && WideTile != null)
                {
                    WideTile.Branding = branding;
                }

                if (size.HasFlag(TileSize.Large) && LargeTile != null)
                {
                    LargeTile.Branding = branding;
                }
            }

            return this;
        }

        /// <summary>
        /// Set the name that will be used to override the application's name on the tile notification.
        /// </summary>
        /// <param name="displayName">Custom name to display on the tile in place of the application's name</param>
        /// <param name="size">The tile size that <paramref name="displayName"/> parameter should be applied to. Default to all currently supported tile size.</param>
        /// <returns>The current instance of <see cref="TileContentBuilder"/></returns>
        public TileContentBuilder SetDisplayName(string displayName, TileSize size = AllSize)
        {
            if (size == AllSize)
            {
                // Set on visual.
                Visual.DisplayName = displayName;
            }
            else
            {
                if (size.HasFlag(TileSize.Small) && SmallTile != null)
                {
                    SmallTile.DisplayName = displayName;
                }

                if (size.HasFlag(TileSize.Medium) && MediumTile != null)
                {
                    MediumTile.DisplayName = displayName;
                }

                if (size.HasFlag(TileSize.Wide) && WideTile != null)
                {
                    WideTile.DisplayName = displayName;
                }

                if (size.HasFlag(TileSize.Large) && LargeTile != null)
                {
                    LargeTile.DisplayName = displayName;
                }
            }

            return this;
        }

        /// <summary>
        /// Set the optional background image that stays behind the tile notification.
        /// </summary>
        /// <param name="imageUri">Source of the background image</param>
        /// <param name="size">The tile size that the background image should be applied to. Default to all currently supported tile size.</param>
        /// <param name="alternateText">Description of the background image, for user of assistance technology</param>
        /// <param name="addImageQuery">
        /// Indicating whether Windows should append a query string to the image URI supplied in the Tile notification.
        /// Use this attribute if your server hosts images and can handle query strings, either by retrieving an image variant based on the query strings or by ignoring the query string and returning the image as specified without the query string.
        /// This query string specifies scale, contrast setting, and language.
        /// </param>
        /// <param name="hintOverlay">The opacity of the black overlay on the background image.</param>
        /// <param name="hintCrop">Desired cropping of the image.</param>
        /// <returns>The current instance of <see cref="TileContentBuilder"/></returns>
        public TileContentBuilder SetBackgroundImage(Uri imageUri, TileSize size = AllSize, string alternateText = default(string), bool? addImageQuery = default(bool?), int? hintOverlay = default(int?), TileBackgroundImageCrop hintCrop = TileBackgroundImageCrop.Default)
        {
            TileBackgroundImage backgroundImage = new TileBackgroundImage();
            backgroundImage.Source = imageUri.OriginalString;
            backgroundImage.HintCrop = hintCrop;

            if (alternateText != default(string))
            {
                backgroundImage.AlternateText = alternateText;
            }

            if (addImageQuery != default(bool?))
            {
                backgroundImage.AddImageQuery = addImageQuery;
            }

            if (hintOverlay != default(int?))
            {
                backgroundImage.HintOverlay = hintOverlay;
            }

            return SetBackgroundImage(backgroundImage, size);
        }

        /// <summary>
        /// Set the optional background image that stays behind the tile notification.
        /// </summary>
        /// <param name="backgroundImage">An instance of <see cref="TileBackgroundImage"/> as the background image for the tile.</param>
        /// <param name="size">The tile size that the background image should be applied to. Default to all currently supported tile size.</param>
        /// <returns>The current instance of <see cref="TileContentBuilder"/></returns>
        public TileContentBuilder SetBackgroundImage(TileBackgroundImage backgroundImage, TileSize size = AllSize)
        {
            // Set to any available tile at the moment of calling.
            if (size.HasFlag(TileSize.Small) && SmallTile != null)
            {
                GetAdaptiveTileContent(SmallTile).BackgroundImage = backgroundImage;
            }

            if (size.HasFlag(TileSize.Medium) && MediumTile != null)
            {
                GetAdaptiveTileContent(MediumTile).BackgroundImage = backgroundImage;
            }

            if (size.HasFlag(TileSize.Wide) && WideTile != null)
            {
                GetAdaptiveTileContent(WideTile).BackgroundImage = backgroundImage;
            }

            if (size.HasFlag(TileSize.Large) && LargeTile != null)
            {
                GetAdaptiveTileContent(LargeTile).BackgroundImage = backgroundImage;
            }

            return this;
        }

        /// <summary>
        /// Set the Tile's Peek Image that animate from the top of the tile notification.
        /// </summary>
        /// <param name="imageUri">Source of the peek image</param>
        /// <param name="size">The tile size that the peek image should be applied to. Default to all currently supported tile size.</param>
        /// <param name="alternateText">Description of the peek image, for user of assistance technology</param>
        /// <param name="addImageQuery">
        /// Indicating whether Windows should append a query string to the image URI supplied in the Tile notification.
        /// Use this attribute if your server hosts images and can handle query strings, either by retrieving an image variant based on the query strings or by ignoring the query string and returning the image as specified without the query string.
        /// This query string specifies scale, contrast setting, and language.
        /// </param>
        /// <param name="hintOverlay">The opacity of the black overlay on the peek image.</param>
        /// <param name="hintCrop">Desired cropping of the image.</param>
        /// <returns>The current instance of <see cref="TileContentBuilder"/></returns>
        public TileContentBuilder SetPeekImage(Uri imageUri, TileSize size = AllSize, string alternateText = default(string), bool? addImageQuery = default(bool?), int? hintOverlay = default(int?), TilePeekImageCrop hintCrop = TilePeekImageCrop.Default)
        {
            TilePeekImage peekImage = new TilePeekImage();
            peekImage.Source = imageUri.OriginalString;
            peekImage.HintCrop = hintCrop;

            if (alternateText != default(string))
            {
                peekImage.AlternateText = alternateText;
            }

            if (addImageQuery != default(bool?))
            {
                peekImage.AddImageQuery = addImageQuery;
            }

            if (hintOverlay != default(int?))
            {
                peekImage.HintOverlay = hintOverlay;
            }

            return SetPeekImage(peekImage, size);
        }

        /// <summary>
        /// Set the Tile's Peek Image that animate from the top of the tile notification.
        /// </summary>
        /// <param name="peekImage">An instance of <see cref="TilePeekImage"/> for the Tile's peek image </param>
        /// <param name="size">The tile size that the peek image should be applied to. Default to all currently supported tile size.</param>
        /// <returns>The current instance of <see cref="TileContentBuilder"/></returns>
        public TileContentBuilder SetPeekImage(TilePeekImage peekImage, TileSize size = AllSize)
        {
            // Set to any available tile at the moment of calling.
            if (size.HasFlag(TileSize.Small) && SmallTile != null)
            {
                GetAdaptiveTileContent(SmallTile).PeekImage = peekImage;
            }

            if (size.HasFlag(TileSize.Medium) && MediumTile != null)
            {
                GetAdaptiveTileContent(MediumTile).PeekImage = peekImage;
            }

            if (size.HasFlag(TileSize.Wide) && WideTile != null)
            {
                GetAdaptiveTileContent(WideTile).PeekImage = peekImage;
            }

            if (size.HasFlag(TileSize.Large) && LargeTile != null)
            {
                GetAdaptiveTileContent(LargeTile).PeekImage = peekImage;
            }

            return this;
        }

        /// <summary>
        /// Set the text stacking (vertical alignment) of the entire binding element.
        /// </summary>
        /// <param name="textStacking">Text Stacking Option</param>
        /// <param name="size">The tile size that the peek image should be applied to. Default to all currently supported tile size.</param>
        /// <returns>The current instance of <see cref="TileContentBuilder"/></returns>
        public TileContentBuilder SetTextStacking(TileTextStacking textStacking, TileSize size = AllSize)
        {
            // Set to any available tile at the moment of calling.
            if (size.HasFlag(TileSize.Small) && SmallTile != null)
            {
                GetAdaptiveTileContent(SmallTile).TextStacking = textStacking;
            }

            if (size.HasFlag(TileSize.Medium) && MediumTile != null)
            {
                GetAdaptiveTileContent(MediumTile).TextStacking = textStacking;
            }

            if (size.HasFlag(TileSize.Wide) && WideTile != null)
            {
                GetAdaptiveTileContent(WideTile).TextStacking = textStacking;
            }

            if (size.HasFlag(TileSize.Large) && LargeTile != null)
            {
                GetAdaptiveTileContent(LargeTile).TextStacking = textStacking;
            }

            return this;
        }

        /// <summary>
        /// Set the tile's activation arguments for tile notification.
        /// </summary>
        /// <param name="args">App-Defined custom arguments that will be passed in when the user click on the tile when this tile notification is being displayed.</param>
        /// <param name="size">The tile size that the custom argument should be applied to. Default to all currently supported tile size.</param>
        /// <returns>The current instance of <see cref="TileContentBuilder"/></returns>
        public TileContentBuilder SetActivationArgument(string args, TileSize size = AllSize)
        {
            if (size == AllSize)
            {
                Visual.Arguments = args;
            }
            else
            {
                if (size.HasFlag(TileSize.Small) && SmallTile != null)
                {
                    SmallTile.Arguments = args;
                }

                if (size.HasFlag(TileSize.Medium) && MediumTile != null)
                {
                    MediumTile.Arguments = args;
                }

                if (size.HasFlag(TileSize.Wide) && WideTile != null)
                {
                    WideTile.Arguments = args;
                }

                if (size.HasFlag(TileSize.Large) && LargeTile != null)
                {
                    LargeTile.Arguments = args;
                }
            }

            return this;
        }

        /// <summary>
        /// Add a custom text that will appear on the tile notification.
        /// </summary>
        /// <param name="text">Custom text to display on the tile.</param>
        /// <param name="size">The tile size that the custom text would be added to. Default to all currently supported tile size.</param>
        /// <param name="hintStyle">Style that controls the text's font size, weight, and opacity.</param>
        /// <param name="hintWrap">Indicating whether text wrapping is enabled. For Tiles, this is false by default.</param>
        /// <param name="hintMaxLines">The maximum number of lines the text element is allowed to display. For Tiles, this is infinity by default</param>
        /// <param name="hintMinLines">The minimum number of lines the text element must display.</param>
        /// <param name="hintAlign">The horizontal alignment of the text</param>
        /// <param name="language">
        /// The target locale of the XML payload, specified as a BCP-47 language tags such as "en-US" or "fr-FR". The locale specified here overrides any other specified locale, such as that in binding or visual.
        /// </param>
        /// <returns>The current instance of <see cref="TileContentBuilder"/></returns>
        public TileContentBuilder AddText(string text, TileSize size = AllSize, AdaptiveTextStyle? hintStyle = null, bool? hintWrap = default(bool?), int? hintMaxLines = default(int?), int? hintMinLines = default(int?), AdaptiveTextAlign? hintAlign = null, string language = default(string))
        {
            // Create the adaptive text.
            AdaptiveText adaptive = new AdaptiveText()
            {
                Text = text
            };

            if (hintStyle != null)
            {
                adaptive.HintStyle = hintStyle.Value;
            }

            if (hintAlign != null)
            {
                adaptive.HintAlign = hintAlign.Value;
            }

            if (hintWrap != default(bool?))
            {
                adaptive.HintWrap = hintWrap;
            }

            if (hintMaxLines != default(int?))
            {
                adaptive.HintMaxLines = hintMaxLines;
            }

            if (hintMinLines != default(int?) && hintMinLines > 0)
            {
                adaptive.HintMinLines = hintMinLines;
            }

            if (language != default(string))
            {
                adaptive.Language = language;
            }

            // Add to the tile content
            return AddAdaptiveTileVisualChild(adaptive, size);
        }

        /// <summary>
        /// Add an adaptive child to the tile notification.
        /// </summary>
        /// <param name="child">An adaptive child to add</param>
        /// <param name="size">Tile size that the adaptive child should be added to. Default to all currently supported tile size.</param>
        /// <returns>The current instance of <see cref="TileContentBuilder"/></returns>
        /// <remarks>
        /// This can be used to add Group and Subgroup to the tile.
        /// </remarks>
        public TileContentBuilder AddAdaptiveTileVisualChild(ITileBindingContentAdaptiveChild child, TileSize size = AllSize)
        {
            if (size.HasFlag(TileSize.Small) && SmallTile != null && GetAdaptiveTileContent(SmallTile) != null)
            {
                GetAdaptiveTileContent(SmallTile).Children.Add(child);
            }

            if (size.HasFlag(TileSize.Medium) && MediumTile != null && GetAdaptiveTileContent(MediumTile) != null)
            {
                GetAdaptiveTileContent(MediumTile).Children.Add(child);
            }

            if (size.HasFlag(TileSize.Wide) && WideTile != null && GetAdaptiveTileContent(WideTile) != null)
            {
                GetAdaptiveTileContent(WideTile).Children.Add(child);
            }

            if (size.HasFlag(TileSize.Large) && LargeTile != null && GetAdaptiveTileContent(LargeTile) != null)
            {
                GetAdaptiveTileContent(LargeTile).Children.Add(child);
            }

            return this;
        }

        /// <summary>
        /// Get the instance of <see cref="TileContent"/> that has been built by the builder with specified configuration so far.
        /// </summary>
        /// <returns>An instance of <see cref="TileContent"/> that can be used to create tile notification.</returns>
        public TileContent GetTileContent()
        {
            return Content;
        }

        private TileBindingContentAdaptive GetAdaptiveTileContent(TileBinding binding)
        {
            return binding.Content as TileBindingContentAdaptive;
        }
    }

#endif
}