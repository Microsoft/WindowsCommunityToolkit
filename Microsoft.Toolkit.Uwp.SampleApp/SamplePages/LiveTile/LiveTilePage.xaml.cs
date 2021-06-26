// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Toolkit.Uwp.SampleApp.Common;
using Microsoft.Toolkit.Uwp.SampleApp.Models;
using NotificationsVisualizerLibrary;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    public sealed partial class LiveTilePage : Page
    {
        private TileContent _tileContent;

        public LiveTilePage()
        {
            InitializeComponent();
            Initialize();
        }

        public static TileContent GenerateTileContent(string username, string avatarLogoSource)
        {
            var builder = new TileContentBuilder();

            // Medium Tile built using only builder method.
            builder.AddTile(Notifications.TileSize.Medium)
                .SetPeekImage(new Uri(avatarLogoSource, UriKind.Relative), Notifications.TileSize.Medium, hintCrop: TilePeekImageCrop.Circle)
                .SetTextStacking(TileTextStacking.Center, Notifications.TileSize.Medium)
                .AddText("Hi,", Notifications.TileSize.Medium, hintStyle: AdaptiveTextStyle.Base, hintAlign: AdaptiveTextAlign.Center)
                .AddText(username, Notifications.TileSize.Medium, hintStyle: AdaptiveTextStyle.CaptionSubtle, hintAlign: AdaptiveTextAlign.Center);

            // Wide Tile using custom-made layout.
            builder.AddTile(Notifications.TileSize.Wide)
                .AddAdaptiveTileVisualChild(GenerateWideTileContent(username, avatarLogoSource), Notifications.TileSize.Wide);

            // Large Tile using custom-made layout conjunction with builder helper method
            builder.AddTile(Notifications.TileSize.Large)
                .AddAdaptiveTileVisualChild(CreateLargeTileLogoPayload(avatarLogoSource), Notifications.TileSize.Large)
                .AddText("Hi,", Notifications.TileSize.Large, hintAlign: AdaptiveTextAlign.Center, hintStyle: AdaptiveTextStyle.Title)
                .AddText(username, Notifications.TileSize.Large, hintAlign: AdaptiveTextAlign.Center, hintStyle: AdaptiveTextStyle.SubtitleSubtle);

            return builder.Content;
        }

        private static ITileBindingContentAdaptiveChild GenerateWideTileContent(string username, string avatarLogoSource)
        {
            return new AdaptiveGroup()
            {
                Children =
                {
                    new AdaptiveSubgroup()
                    {
                        HintWeight = 33,

                        Children =
                        {
                            new AdaptiveImage()
                            {
                                Source = avatarLogoSource,
                                HintCrop = AdaptiveImageCrop.Circle
                            }
                        }
                    },

                    new AdaptiveSubgroup()
                    {
                        HintTextStacking = AdaptiveSubgroupTextStacking.Center,

                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = "Hi,",
                                HintStyle = AdaptiveTextStyle.Title
                            },

                            new AdaptiveText()
                            {
                                Text = username,
                                HintStyle = AdaptiveTextStyle.SubtitleSubtle
                            }
                        }
                    }
                }
            };
        }

        private static ITileBindingContentAdaptiveChild CreateLargeTileLogoPayload(string avatarLogoSource)
        {
            return new AdaptiveGroup()
            {
                Children =
                {
                    new AdaptiveSubgroup()
                    {
                        HintWeight = 1
                    },

                    // We surround the image by two subgroups so that it doesn't take the full width
                    new AdaptiveSubgroup()
                    {
                        HintWeight = 2,
                        Children =
                        {
                            new AdaptiveImage()
                            {
                                Source = avatarLogoSource,
                                HintCrop = AdaptiveImageCrop.Circle
                            }
                        }
                    },

                    new AdaptiveSubgroup()
                    {
                        HintWeight = 1
                    }
                }
            };
        }

        private void ButtonPinTile_Click(object sender, RoutedEventArgs e)
        {
            PinTile();
        }

        private async void PinTile()
        {
            SecondaryTile tile = new SecondaryTile(DateTime.Now.Ticks.ToString())
            {
                DisplayName = "Xbox",
                Arguments = "args"
            };
            tile.VisualElements.Square150x150Logo = Constants.Square150x150Logo;
            tile.VisualElements.Wide310x150Logo = Constants.Wide310x150Logo;
            tile.VisualElements.Square310x310Logo = Constants.Square310x310Logo;
            tile.VisualElements.ShowNameOnSquare150x150Logo = true;
            tile.VisualElements.ShowNameOnSquare310x310Logo = true;
            tile.VisualElements.ShowNameOnWide310x150Logo = true;

            if (!await tile.RequestCreateAsync())
            {
                return;
            }

            TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId).Update(new TileNotification(_tileContent.GetXml()));
        }

        private void Initialize()
        {
            // Generate the tile notification content
            _tileContent = GenerateTileContent("MasterHip", "Assets/Photos/Owl.jpg");

            // Prepare and update the preview tiles
            var previewTiles = new PreviewTile[] { MediumPreviewTile, WidePreviewTile, LargePreviewTile };
            foreach (var tile in previewTiles)
            {
                tile.DisplayName = "Xbox";
                tile.VisualElements.BackgroundColor = Constants.ApplicationBackgroundColor;
                tile.VisualElements.ShowNameOnSquare150x150Logo = true;
                tile.VisualElements.ShowNameOnSquare310x310Logo = true;
                tile.VisualElements.ShowNameOnWide310x150Logo = true;
                tile.VisualElements.Square44x44Logo = Constants.Square44x44Logo;
                tile.VisualElements.Square150x150Logo = Constants.Square150x150Logo;
                tile.VisualElements.Wide310x150Logo = Constants.Wide310x150Logo;
                tile.VisualElements.Square310x310Logo = Constants.Square310x310Logo;
                _ = tile.UpdateAsync(); // Commit changes (no need to await)

                tile.CreateTileUpdater().Update(new TileNotification(_tileContent.GetXml()));
            }
        }
    }
}