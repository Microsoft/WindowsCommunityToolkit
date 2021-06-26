// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Toolkit.Uwp.SampleApp.Common;
using NotificationsVisualizerLibrary;
using Windows.System.Profile;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    public sealed partial class WeatherLiveTileAndToastPage
    {
        private TileContent _tileContent;
        private ToastContent _toastContent;

        public WeatherLiveTileAndToastPage()
        {
            InitializeComponent();
            Initialize();
        }

        public static ToastContent GenerateToastContent()
        {
            ToastContentBuilder builder = new ToastContentBuilder();

            // Include launch string so we know what to open when user clicks toast
            builder.AddArgument("action", "viewForecast");
            builder.AddArgument("zip", 98008);

            // We'll always have this summary text on our toast notification
            // (it is required that your toast starts with a text element)
            builder.AddText("Today will be mostly sunny with a high of 63 and a low of 42.");

            // If Adaptive Toast Notifications are supported
            if (IsAdaptiveToastSupported())
            {
                // Use the rich Tile-like visual layout
                builder.AddVisualChild(new AdaptiveGroup()
                {
                    Children =
                    {
                        GenerateSubgroup("Mon", "Mostly Cloudy.png", 63, 42),
                        GenerateSubgroup("Tue", "Cloudy.png", 57, 38),
                        GenerateSubgroup("Wed", "Sunny.png", 59, 43),
                        GenerateSubgroup("Thu", "Sunny.png", 62, 42),
                        GenerateSubgroup("Fri", "Sunny.png", 71, 66)
                    }
                });
            }

            // Otherwise...
            else
            {
                // We'll just add two simple lines of text
                builder.AddText("Monday ⛅ 63° / 42°")
                    .AddText("Tuesday ☁ 57° / 38°");
            }

            // Set the base URI for the images, so we don't redundantly specify the entire path
            builder.Content.Visual.BaseUri = new Uri("Assets/NotificationAssets/", UriKind.Relative);

            return builder.Content;
        }

        public static TileContent GenerateTileContent()
        {
            TileContentBuilder builder = new TileContentBuilder();

            // Small Tile
            builder.AddTile(Notifications.TileSize.Small)
                .SetTextStacking(TileTextStacking.Center, Notifications.TileSize.Small)
                .AddText("Mon", Notifications.TileSize.Small, hintStyle: AdaptiveTextStyle.Body, hintAlign: AdaptiveTextAlign.Center)
                .AddText("63°", Notifications.TileSize.Small, hintStyle: AdaptiveTextStyle.Base, hintAlign: AdaptiveTextAlign.Center);

            // Medium Tile
            builder.AddTile(Notifications.TileSize.Medium)
                .AddAdaptiveTileVisualChild(
                    new AdaptiveGroup()
                    {
                        Children =
                        {
                            GenerateSubgroup("Mon", "Mostly Cloudy.png", 63, 42),
                            GenerateSubgroup("Tue", "Cloudy.png", 57, 38)
                        }
                    }, Notifications.TileSize.Medium);

            // Wide Tile
            builder.AddTile(Notifications.TileSize.Wide)
                .AddAdaptiveTileVisualChild(
                new AdaptiveGroup()
                {
                    Children =
                    {
                        GenerateSubgroup("Mon", "Mostly Cloudy.png", 63, 42),
                        GenerateSubgroup("Tue", "Cloudy.png", 57, 38),
                        GenerateSubgroup("Wed", "Sunny.png", 59, 43),
                        GenerateSubgroup("Thu", "Sunny.png", 62, 42),
                        GenerateSubgroup("Fri", "Sunny.png", 71, 66)
                    }
                }, Notifications.TileSize.Wide);

            // Large tile
            builder.AddTile(Notifications.TileSize.Large, GenerateLargeTileContent());

            // Set the base URI for the images, so we don't redundantly specify the entire path
            builder.Content.Visual.BaseUri = new Uri("Assets/NotificationAssets/", UriKind.Relative);

            return builder.Content;
        }

        private static bool IsAdaptiveToastSupported()
        {
            switch (AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                case "Windows.Desktop":
                    return true;

                // Other device families do not support adaptive toasts
                default:
                    return false;
            }
        }

        private static TileBindingContentAdaptive GenerateLargeTileContent()
        {
            return new TileBindingContentAdaptive()
            {
                Children =
                {
                    new AdaptiveGroup()
                    {
                        Children =
                        {
                            new AdaptiveSubgroup()
                            {
                                HintWeight = 30,
                                Children =
                                {
                                    new AdaptiveImage() { Source = "Mostly Cloudy.png" }
                                }
                            },

                            new AdaptiveSubgroup()
                            {
                                Children =
                                {
                                    new AdaptiveText()
                                    {
                                        Text = "Monday",
                                        HintStyle = AdaptiveTextStyle.Base
                                    },

                                    new AdaptiveText()
                                    {
                                        Text = "63° / 42°"
                                    },

                                    new AdaptiveText()
                                    {
                                        Text = "20% chance of rain",
                                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    },

                                    new AdaptiveText()
                                    {
                                        Text = "Winds 5 mph NE",
                                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    }
                                }
                            }
                        }
                    },

                    // For spacing
                    new AdaptiveText(),

                    new AdaptiveGroup()
                    {
                        Children =
                        {
                            GenerateLargeSubgroup("Tue", "Cloudy.png", 57, 38),
                            GenerateLargeSubgroup("Wed", "Sunny.png", 59, 43),
                            GenerateLargeSubgroup("Thu", "Sunny.png", 62, 42),
                            GenerateLargeSubgroup("Fri", "Sunny.png", 71, 66)
                        }
                    }
                }
            };
        }

        private static AdaptiveSubgroup GenerateSubgroup(string day, string img, int tempHi, int tempLo)
        {
            return new AdaptiveSubgroup()
            {
                HintWeight = 1,

                Children =
                {
                    // Day
                    new AdaptiveText()
                    {
                        Text = day,
                        HintAlign = AdaptiveTextAlign.Center
                    },

                    // Image
                    new AdaptiveImage()
                    {
                        Source = img,
                        HintRemoveMargin = true
                    },

                    // High temp
                    new AdaptiveText()
                    {
                        Text = tempHi + "°",
                        HintAlign = AdaptiveTextAlign.Center
                    },

                    // Low temp
                    new AdaptiveText()
                    {
                        Text = tempLo + "°",
                        HintAlign = AdaptiveTextAlign.Center,
                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                    }
                }
            };
        }

        private static AdaptiveSubgroup GenerateLargeSubgroup(string day, string image, int high, int low)
        {
            // Generate the normal subgroup
            var subgroup = GenerateSubgroup(day, image, high, low);

            // Allow there to be padding around the image
            (subgroup.Children[1] as AdaptiveImage).HintRemoveMargin = null;

            return subgroup;
        }

        private static AdaptiveText GenerateLegacyToastText(string day, string weatherEmoji, int tempHi, int tempLo)
        {
            return new AdaptiveText()
            {
                Text = $"{day} {weatherEmoji} {tempHi}° / {tempLo}°"
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
                DisplayName = "WeatherSample",
                Arguments = "args"
            };
            tile.VisualElements.ShowNameOnSquare150x150Logo = true;
            tile.VisualElements.ShowNameOnSquare310x310Logo = true;
            tile.VisualElements.ShowNameOnWide310x150Logo = true;
            tile.VisualElements.Square150x150Logo = Constants.Square150x150Logo;
            tile.VisualElements.Wide310x150Logo = Constants.Wide310x150Logo;
            tile.VisualElements.Square310x310Logo = Constants.Square310x310Logo;

            if (!await tile.RequestCreateAsync())
            {
                return;
            }

            TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId).Update(new TileNotification(_tileContent.GetXml()));
        }

        private void ButtonPopToast_Click(object sender, RoutedEventArgs e)
        {
            PopToast();
        }

        private void PopToast()
        {
            ToastNotificationManagerCompat.CreateToastNotifier().Show(new ToastNotification(_toastContent.GetXml()));
        }

        private void Initialize()
        {
            // Generate the tile notification content
            _tileContent = GenerateTileContent();

            // Generate the toast notification content
            _toastContent = GenerateToastContent();

            // Prepare and update the preview tiles
            var previewTiles = new PreviewTile[] { PreviewTileSmall, PreviewTileMedium, PreviewTileWide, PreviewTileLarge };
            foreach (var tile in previewTiles)
            {
                tile.DisplayName = "WeatherSample";
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

            // Prepare and update preview toast
            PreviewToastWeather.Properties = new PreviewToastProperties()
            {
                BackgroundColor = Constants.ApplicationBackgroundColor,
                DisplayName = Constants.ApplicationDisplayName,
                Square44x44Logo = Constants.Square44x44Logo
            };
            PreviewToastWeather.Initialize(_toastContent.GetXml());
        }
    }
}