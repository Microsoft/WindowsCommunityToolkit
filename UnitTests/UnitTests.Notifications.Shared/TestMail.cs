// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Notifications;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Notifications
{
    [TestClass]
    public class TestMail
    {
        private const string FirstFrom = "Jennifer Parker";
        private const string FirstSubject = "Photos from our trip";
        private const string FirstBody = "Check out these awesome photos I took while in New Zealand!";

        private const string SecondFrom = "Steve Bosniak";
        private const string SecondSubject = "Build 2015 Dinner";
        private const string SecondBody = "Want to go out for dinner after Build tonight?";

        [TestCategory("EndToEnd/Mail")]
        [TestMethod]
        public void TestMailTile()
        {
            TileBinding small = new TileBinding()
            {
                Content = new TileBindingContentIconic()
                {
                    Icon = new TileBasicImage() { Source = "Assets\\Mail.png" }
                }
            };

            TileBinding medium = new TileBinding()
            {
                Branding = TileBranding.Logo,

                Content = new TileBindingContentAdaptive()
                {
                    Children =
                    {
                        GenerateFirstMessage(false),
                        GenerateSpacer(),
                        GenerateSecondMessage(false)
                    }
                }
            };

            TileBinding wideAndLarge = new TileBinding()
            {
                Branding = TileBranding.NameAndLogo,

                Content = new TileBindingContentAdaptive()
                {
                    Children =
                    {
                        GenerateFirstMessage(true),
                        GenerateSpacer(),
                        GenerateSecondMessage(true)
                    }
                }
            };

            TileContent content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileSmall = small,
                    TileMedium = medium,
                    TileWide = wideAndLarge,
                    TileLarge = wideAndLarge
                }
            };

            string expectedXml = $@"<?xml version=""1.0"" encoding=""utf-8""?><tile><visual><binding template=""TileSquare71x71IconWithBadge""><image id=""1"" src=""Assets\Mail.png"" /></binding>";

            // Medium
            expectedXml += @"<binding template=""TileMedium"" branding=""logo"">";
            expectedXml += GenerateXmlGroups(false);
            expectedXml += "</binding>";

            // Wide
            expectedXml += @"<binding template=""TileWide"" branding=""nameAndLogo"">";
            expectedXml += GenerateXmlGroups(true);
            expectedXml += "</binding>";

            // Large
            expectedXml += @"<binding template=""TileLarge"" branding=""nameAndLogo"">";
            expectedXml += GenerateXmlGroups(true);
            expectedXml += "</binding>";

            expectedXml += "</visual></tile>";

            AssertHelper.AssertTile(expectedXml, content);
        }

        private static string GenerateXmlGroups(bool makeLarge)
        {
            return GenerateXmlGroup(FirstFrom, FirstSubject, FirstBody, makeLarge) + "<text />" + GenerateXmlGroup(SecondFrom, SecondSubject, SecondBody, makeLarge);
        }

        private static string GenerateXmlGroup(string from, string subject, string body, bool makeLarge)
        {
            string xml = "<group><subgroup><text";

            if (makeLarge)
            {
                xml += @" hint-style=""subtitle""";
            }
            else
            {
                xml += " hint-style='caption'";
            }

            xml += $@">{from}</text><text hint-style=""captionSubtle"">{subject}</text><text hint-style=""captionSubtle"">{body}</text></subgroup></group>";

            return xml;
        }

        private static AdaptiveText GenerateSpacer()
        {
            return new AdaptiveText();
        }

        private static AdaptiveGroup GenerateFirstMessage(bool makeLarge)
        {
            return GenerateMessage(FirstFrom, FirstSubject, FirstBody, makeLarge);
        }

        private static AdaptiveGroup GenerateSecondMessage(bool makeLarge)
        {
            return GenerateMessage(SecondFrom, SecondSubject, SecondBody, makeLarge);
        }

        private static AdaptiveGroup GenerateMessage(string from, string subject, string body, bool makeLarge)
        {
            return new AdaptiveGroup()
            {
                Children =
                {
                    new AdaptiveSubgroup()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = from,
                                HintStyle = makeLarge ? AdaptiveTextStyle.Subtitle : AdaptiveTextStyle.Caption
                            },

                            new AdaptiveText()
                            {
                                Text = subject,
                                HintStyle = AdaptiveTextStyle.CaptionSubtle
                            },

                            new AdaptiveText()
                            {
                                Text = body,
                                HintStyle = AdaptiveTextStyle.CaptionSubtle
                            }
                        }
                    }
                }
            };
        }
    }
}