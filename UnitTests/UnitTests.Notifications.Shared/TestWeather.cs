// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Notifications;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Notifications
{
    [TestClass]
    public class TestWeather
    {
        private const string ImageMostlyCloudy = "Assets\\Tiles\\Mostly Cloudy.png";
        private const string ImageSunny = "Assets\\Tiles\\Sunny.png";
        private const string ImageCloudy = "Assets\\Tiles\\Cloudy.png";

        private const string BackgroundImageMostlyCloudy = "Assets\\Tiles\\Mostly Cloudy-Background.jpg";

        [TestMethod]
        public void TestWeatherTile()
        {
            var backgroundImage = BackgroundImageMostlyCloudy;
            int overlay = 30;

            TileBindingContentAdaptive smallContent = new TileBindingContentAdaptive()
            {
                TextStacking = TileTextStacking.Center,
                BackgroundImage = new TileBackgroundImage() { Source = backgroundImage, HintOverlay = overlay },
                Children =
                {
                    new AdaptiveText()
                    {
                        Text = "Mon",
                        HintStyle = AdaptiveTextStyle.Body,
                        HintAlign = AdaptiveTextAlign.Center
                    },

                    new AdaptiveText()
                    {
                        Text = "63°",
                        HintStyle = AdaptiveTextStyle.Base,
                        HintAlign = AdaptiveTextAlign.Center
                    }
                }
            };

            TileBindingContentAdaptive mediumContent = new TileBindingContentAdaptive()
            {
                BackgroundImage = new TileBackgroundImage() { Source = backgroundImage, HintOverlay = overlay },
                Children =
                {
                    new AdaptiveGroup()
                    {
                        Children =
                        {
                            GenerateMediumSubgroup("Mon", ImageMostlyCloudy, 63, 42),
                            GenerateMediumSubgroup("Tue", ImageCloudy, 57, 38)
                        }
                    }
                }
            };

            TileBindingContentAdaptive wideContent = new TileBindingContentAdaptive()
            {
                BackgroundImage = new TileBackgroundImage() { Source = backgroundImage, HintOverlay = overlay },
                Children =
                {
                    new AdaptiveGroup()
                    {
                        Children =
                        {
                            GenerateWideSubgroup("Mon", ImageMostlyCloudy, 63, 42),
                            GenerateWideSubgroup("Tue", ImageCloudy, 57, 38),
                            GenerateWideSubgroup("Wed", ImageSunny, 59, 43),
                            GenerateWideSubgroup("Thu", ImageSunny, 62, 42),
                            GenerateWideSubgroup("Fri", ImageSunny, 71, 66)
                        }
                    }
                }
            };

            TileBindingContentAdaptive largeContent = new TileBindingContentAdaptive()
            {
                BackgroundImage = new TileBackgroundImage() { Source = backgroundImage, HintOverlay = overlay },
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
                                    new AdaptiveImage() { Source = ImageMostlyCloudy }
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
                            GenerateLargeSubgroup("Tue", ImageCloudy, 57, 38),
                            GenerateLargeSubgroup("Wed", ImageSunny, 59, 43),
                            GenerateLargeSubgroup("Thu", ImageSunny, 62, 42),
                            GenerateLargeSubgroup("Fri", ImageSunny, 71, 66)
                        }
                    }
                }
            };

            TileContent content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    DisplayName = "Seattle",

                    TileSmall = new TileBinding()
                    {
                        Content = smallContent
                    },

                    TileMedium = new TileBinding()
                    {
                        Content = mediumContent,
                        Branding = TileBranding.Name
                    },

                    TileWide = new TileBinding()
                    {
                        Content = wideContent,
                        Branding = TileBranding.NameAndLogo
                    },

                    TileLarge = new TileBinding()
                    {
                        Content = largeContent,
                        Branding = TileBranding.NameAndLogo
                    }
                }
            };

            string expectedPayload = $@"<?xml version=""1.0"" encoding=""utf-8""?><tile><visual displayName=""Seattle""><binding template=""TileSmall"" hint-textStacking=""center"">{GenerateStringBackgroundImage()}<text hint-align=""center"" hint-style=""body"">Mon</text><text hint-align=""center"" hint-style=""base"">63°</text></binding><binding template=""TileMedium"" branding=""name"">{GenerateStringBackgroundImage()}<group>";

            // Medium tile subgroups
            expectedPayload += GenerateStringMediumSubgroup("Mon", ImageMostlyCloudy, 63, 42);
            expectedPayload += GenerateStringMediumSubgroup("Tue", ImageCloudy, 57, 38);

            expectedPayload += "</group></binding>";

            // Wide tile
            expectedPayload += @"<binding template=""TileWide"" branding=""nameAndLogo"">";
            expectedPayload += GenerateStringBackgroundImage();
            expectedPayload += "<group>";

            // Wide tile subgroups
            expectedPayload += GenerateStringWideSubgroup("Mon", ImageMostlyCloudy, 63, 42);
            expectedPayload += GenerateStringWideSubgroup("Tue", ImageCloudy, 57, 38);
            expectedPayload += GenerateStringWideSubgroup("Wed", ImageSunny, 59, 43);
            expectedPayload += GenerateStringWideSubgroup("Thu", ImageSunny, 62, 42);
            expectedPayload += GenerateStringWideSubgroup("Fri", ImageSunny, 71, 66);

            expectedPayload += "</group></binding>";

            // Large tile
            expectedPayload += @"<binding template=""TileLarge"" branding=""nameAndLogo"">";
            expectedPayload += GenerateStringBackgroundImage();
            expectedPayload += $@"<group><subgroup hint-weight=""30""><image src=""{ImageMostlyCloudy}"" /></subgroup><subgroup><text hint-style=""base"">Monday</text><text>63° / 42°</text><text hint-style=""captionSubtle"">20% chance of rain</text><text hint-style=""captionSubtle"">Winds 5 mph NE</text></subgroup></group>";

            expectedPayload += "<text />";
            expectedPayload += "<group>";

            // Large tile subgroups
            expectedPayload += GenerateStringLargeSubgroup("Tue", ImageCloudy, 57, 38);
            expectedPayload += GenerateStringLargeSubgroup("Wed", ImageSunny, 59, 43);
            expectedPayload += GenerateStringLargeSubgroup("Thu", ImageSunny, 62, 42);
            expectedPayload += GenerateStringLargeSubgroup("Fri", ImageSunny, 71, 66);

            expectedPayload += "</group></binding></visual></tile>";

            AssertHelper.AssertTile(expectedPayload, content);
        }

        private static string GenerateStringBackgroundImage()
        {
            return $@"<image src=""{BackgroundImageMostlyCloudy}"" placement=""background"" hint-overlay=""30""/>";
        }

        private static string GenerateStringMediumSubgroup(string day, string image, int high, int low)
        {
            return $@"<subgroup><text hint-align=""center"">{day}</text><image src=""{image}"" hint-removeMargin=""true"" /><text hint-align=""center"">{high}°</text><text hint-align=""center"" hint-style=""captionSubtle"">{low}°</text></subgroup>";
        }

        private static string GenerateStringWideSubgroup(string day, string image, int high, int low)
        {
            return $@"<subgroup hint-weight=""1""><text hint-align=""center"">{day}</text><image src=""{image}"" hint-removeMargin=""true"" /><text hint-align=""center"">{high}°</text><text hint-align=""center"" hint-style=""captionSubtle"">{low}°</text></subgroup>";
        }

        private static string GenerateStringLargeSubgroup(string day, string image, int high, int low)
        {
            return $@"<subgroup hint-weight=""1""><text hint-align=""center"">{day}</text><image src=""{image}"" /><text hint-align=""center"">{high}°</text><text hint-align=""center"" hint-style=""captionSubtle"">{low}°</text></subgroup>";
        }

        private static AdaptiveSubgroup GenerateMediumSubgroup(string day, string image, int high, int low)
        {
            return new AdaptiveSubgroup()
            {
                Children =
                {
                    new AdaptiveText()
                    {
                        Text = day,
                        HintAlign = AdaptiveTextAlign.Center
                    },

                    new AdaptiveImage()
                    {
                        Source = image,
                        HintRemoveMargin = true
                    },

                    new AdaptiveText()
                    {
                        Text = high + "°",
                        HintAlign = AdaptiveTextAlign.Center
                    },

                    new AdaptiveText()
                    {
                        Text = low + "°",
                        HintAlign = AdaptiveTextAlign.Center,
                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                    }
                }
            };
        }

        private static AdaptiveSubgroup GenerateWideSubgroup(string day, string image, int high, int low)
        {
            var subgroup = GenerateMediumSubgroup(day, image, high, low);

            subgroup.HintWeight = 1;

            return subgroup;
        }

        private static AdaptiveSubgroup GenerateLargeSubgroup(string day, string image, int high, int low)
        {
            var subgroup = GenerateWideSubgroup(day, image, high, low);

            (subgroup.Children[1] as AdaptiveImage).HintRemoveMargin = null;

            return subgroup;
        }
    }
}