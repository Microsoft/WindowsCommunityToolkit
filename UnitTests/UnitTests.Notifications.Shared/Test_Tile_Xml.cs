// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.WinUI.Notifications;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Notifications
{
    [TestClass]
    public class Test_Tile_Xml
    {
        [TestMethod]
        public void Test_Tile_Xml_Tile_Default()
        {
            TileContent tile = new TileContent();

            AssertPayload("<tile/>", tile);
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_Default()
        {
            // Assert the defaults
            AssertVisual("<visual/>", new TileVisual());
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_AddImageQuery_False()
        {
            AssertVisual(
                "<visual addImageQuery='false'/>",
                new TileVisual()
                {
                    AddImageQuery = false
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_AddImageQuery_True()
        {
            AssertVisual(
                "<visual addImageQuery='true'/>",
                new TileVisual()
                {
                    AddImageQuery = true
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_BaseUri_Null()
        {
            AssertVisual(
                "<visual />",
                new TileVisual()
                {
                    BaseUri = null
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_BaseUri_Value()
        {
            AssertVisual(
                "<visual baseUri='http://msn.com/'/>",
                new TileVisual()
                {
                    BaseUri = new Uri("http://msn.com")
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_Branding_Auto()
        {
            AssertVisual(
                "<visual />",
                new TileVisual()
                {
                    Branding = TileBranding.Auto
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_Branding_Name()
        {
            AssertVisual(
                "<visual branding='name'/>",
                new TileVisual()
                {
                    Branding = TileBranding.Name
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_Branding_Logo()
        {
            AssertVisual(
                "<visual branding='logo'/>",
                new TileVisual()
                {
                    Branding = TileBranding.Logo
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_Branding_NameAndLogo()
        {
            AssertVisual(
                "<visual branding='nameAndLogo'/>",
                new TileVisual()
                {
                    Branding = TileBranding.NameAndLogo
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_Branding_None()
        {
            AssertVisual(
                "<visual branding='none'/>",
                new TileVisual()
                {
                    Branding = TileBranding.None
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_ContentId_Null()
        {
            AssertVisual(
                "<visual />",
                new TileVisual()
                {
                    ContentId = null
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_ContentId_Value()
        {
            AssertVisual(
                "<visual contentId='tsla'/>",
                new TileVisual()
                {
                    ContentId = "tsla"
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_DisplayName_Null()
        {
            AssertVisual(
                "<visual />",
                new TileVisual()
                {
                    DisplayName = null
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_DisplayName_Value()
        {
            AssertVisual(
                "<visual displayName='My name'/>",
                new TileVisual()
                {
                    DisplayName = "My name"
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_Language_Null()
        {
            AssertVisual(
                "<visual />",
                new TileVisual()
                {
                    Language = null
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_Language_Value()
        {
            AssertVisual(
                "<visual lang='en-US'/>",
                new TileVisual()
                {
                    Language = "en-US"
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_Arguments_Null()
        {
            AssertVisual(
                "<visual />",
                new TileVisual()
                {
                    Arguments = null
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_Arguments_EmptyString()
        {
            AssertVisual(
                "<visual arguments=''/>",
                new TileVisual()
                {
                    Arguments = string.Empty
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_Arguments_Value()
        {
            AssertVisual(
                "<visual arguments='action=viewStory&amp;story=53'/>",
                new TileVisual()
                {
                    Arguments = "action=viewStory&story=53"
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_LockDetailedStatus1_NoMatchingText()
        {
            AssertVisual(
                "<visual><binding template='TileWide' hint-lockDetailedStatus1='Status 1'><text>Awesome</text><text>Cool</text></binding></visual>",
                new TileVisual()
                {
                    LockDetailedStatus1 = "Status 1",

                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText() { Text = "Awesome" },
                                new AdaptiveText() { Text = "Cool" }
                            }
                        }
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_LockDetailedStatus1_MatchingText_InBinding()
        {
            AssertVisual(
                "<visual><binding template='TileWide'><text>Awesome</text><text>Cool</text><text id='1'>Status 1</text><text>Blah</text></binding></visual>",
                new TileVisual()
                {
                    LockDetailedStatus1 = "Status 1",

                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText() { Text = "Awesome" },
                                new AdaptiveText() { Text = "Cool" },
                                new AdaptiveText() { Text = "Status 1" },
                                new AdaptiveText() { Text = "Blah" }
                            }
                        }
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_LockDetailedStatus1_MatchingText_InSubgroup()
        {
            // The lockscreen only looks at ID's in the immediate binding children. So anything in the groups/subgroups are
            // ignored. Thus, if text matches there, it still has to be placed as a hint.
            AssertVisual(
                "<visual><binding template='TileWide' hint-lockDetailedStatus1='Status 1'><text>Awesome</text><group><subgroup><image src='Fable.jpg' /><text>Status 1</text><text>Cool</text></subgroup></group><text>Blah</text></binding></visual>",
                new TileVisual()
                {
                    LockDetailedStatus1 = "Status 1",

                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText() { Text = "Awesome" },
                                new AdaptiveGroup()
                                {
                                    Children =
                                    {
                                        new AdaptiveSubgroup()
                                        {
                                            Children =
                                            {
                                                new AdaptiveImage()
                                                {
                                                    Source = "Fable.jpg"
                                                },
                                                new AdaptiveText() { Text = "Status 1" },
                                                new AdaptiveText() { Text = "Cool" }
                                            }
                                        }
                                    }
                                },
                                new AdaptiveText() { Text = "Blah" }
                            }
                        }
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_LockDetailedStatus2_NoMatchingText()
        {
            AssertVisual(
                "<visual><binding template='TileWide' hint-lockDetailedStatus2='Status 2'><text>Awesome</text><text>Cool</text></binding></visual>",
                new TileVisual()
                {
                    LockDetailedStatus2 = "Status 2",

                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText() { Text = "Awesome" },
                                new AdaptiveText() { Text = "Cool" }
                            }
                        }
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_LockDetailedStatus2_MatchingText_InBinding()
        {
            AssertVisual(
                "<visual><binding template='TileWide'><text>Awesome</text><text>Cool</text><text id='2'>Status 2</text><text>Blah</text></binding></visual>",
                new TileVisual()
                {
                    LockDetailedStatus2 = "Status 2",

                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText() { Text = "Awesome" },
                                new AdaptiveText() { Text = "Cool" },
                                new AdaptiveText() { Text = "Status 2" },
                                new AdaptiveText() { Text = "Blah" }
                            }
                        }
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_LockDetailedStatus2_MatchingText_InSubgroup()
        {
            AssertVisual(
                "<visual><binding template='TileWide' hint-lockDetailedStatus2='Status 2'><text>Awesome</text><group><subgroup><image src='Fable.jpg' /><text>Status 2</text><text>Cool</text></subgroup></group><text>Blah</text></binding></visual>",
                new TileVisual()
                {
                    LockDetailedStatus2 = "Status 2",

                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText() { Text = "Awesome" },
                                new AdaptiveGroup()
                                {
                                    Children =
                                    {
                                        new AdaptiveSubgroup()
                                        {
                                            Children =
                                            {
                                                new AdaptiveImage()
                                                {
                                                    Source = "Fable.jpg"
                                                },
                                                new AdaptiveText() { Text = "Status 2" },
                                                new AdaptiveText() { Text = "Cool" }
                                            }
                                        }
                                    }
                                },
                                new AdaptiveText() { Text = "Blah" }
                            }
                        }
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_LockDetailedStatus3_NoMatchingText()
        {
            AssertVisual(
                "<visual><binding template='TileWide' hint-lockDetailedStatus3='Status 3'><text>Awesome</text><text>Cool</text></binding></visual>",
                new TileVisual()
                {
                    LockDetailedStatus3 = "Status 3",

                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText() { Text = "Awesome" },
                                new AdaptiveText() { Text = "Cool" }
                            }
                        }
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_LockDetailedStatus3_MatchingText_InBinding()
        {
            AssertVisual(
                "<visual><binding template='TileWide'><text>Awesome</text><text>Cool</text><text id='3'>Status 3</text><text>Blah</text></binding></visual>",
                new TileVisual()
                {
                    LockDetailedStatus3 = "Status 3",

                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText() { Text = "Awesome" },
                                new AdaptiveText() { Text = "Cool" },
                                new AdaptiveText() { Text = "Status 3" },
                                new AdaptiveText() { Text = "Blah" }
                            }
                        }
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Visual_LockDetailedStatus3_MatchingText_InSubgroup()
        {
            AssertVisual(
                "<visual><binding template='TileWide' hint-lockDetailedStatus3='Status 3'><text>Awesome</text><group><subgroup><image src='Fable.jpg' /><text>Status 3</text><text>Cool</text></subgroup></group><text>Blah</text></binding></visual>",
                new TileVisual()
                {
                    LockDetailedStatus3 = "Status 3",

                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText() { Text = "Awesome" },
                                new AdaptiveGroup()
                                {
                                    Children =
                                    {
                                        new AdaptiveSubgroup()
                                        {
                                            Children =
                                            {
                                                new AdaptiveImage()
                                                {
                                                    Source = "Fable.jpg"
                                                },
                                                new AdaptiveText() { Text = "Status 3" },
                                                new AdaptiveText() { Text = "Cool" }
                                            }
                                        }
                                    }
                                },
                                new AdaptiveText() { Text = "Blah" }
                            }
                        }
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Binding_Default()
        {
            AssertBindingMedium("<binding template='TileMedium'/>", new TileBinding());
        }

        [TestMethod]
        public void Test_Tile_Xml_Binding_AddImageQuery_False()
        {
            AssertBindingMedium(
                "<binding template='TileMedium' addImageQuery='false'/>",
                new TileBinding()
                {
                    AddImageQuery = false
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Binding_AddImageQuery_True()
        {
            AssertBindingMedium(
                "<binding template='TileMedium' addImageQuery='true'/>",
                new TileBinding()
                {
                    AddImageQuery = true
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Binding_BaseUri_Null()
        {
            AssertBindingMedium(
                "<binding template='TileMedium'/>",
                new TileBinding()
                {
                    BaseUri = null
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Binding_BaseUri_Value()
        {
            AssertBindingMedium(
                "<binding template='TileMedium' baseUri='http://msn.com/'/>",
                new TileBinding()
                {
                    BaseUri = new Uri("http://msn.com")
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Binding_Branding_Auto()
        {
            AssertBindingMedium(
                "<binding template='TileMedium'/>",
                new TileBinding()
                {
                    Branding = TileBranding.Auto
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Binding_Branding_None()
        {
            AssertBindingMedium(
                "<binding template='TileMedium' branding='none'/>",
                new TileBinding()
                {
                    Branding = TileBranding.None
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Binding_Branding_Name()
        {
            AssertBindingMedium(
                "<binding template='TileMedium' branding='name'/>",
                new TileBinding()
                {
                    Branding = TileBranding.Name
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Binding_Branding_Logo()
        {
            AssertBindingMedium(
                "<binding template='TileMedium' branding='logo'/>",
                new TileBinding()
                {
                    Branding = TileBranding.Logo
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Binding_Branding_NameAndLogo()
        {
            AssertBindingMedium(
                "<binding template='TileMedium' branding='nameAndLogo'/>",
                new TileBinding()
                {
                    Branding = TileBranding.NameAndLogo
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Binding_ContentId_Null()
        {
            AssertBindingMedium(
                "<binding template='TileMedium' />",
                new TileBinding()
                {
                    ContentId = null
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Binding_ContentId_Value()
        {
            AssertBindingMedium(
                "<binding template='TileMedium' contentId='myId'/>",
                new TileBinding()
                {
                    ContentId = "myId"
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Binding_DisplayName_Null()
        {
            AssertBindingMedium(
                "<binding template='TileMedium' />",
                new TileBinding()
                {
                    DisplayName = null
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Binding_DisplayName_Value()
        {
            AssertBindingMedium(
                "<binding template='TileMedium' displayName='My name'/>",
                new TileBinding()
                {
                    DisplayName = "My name"
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Binding_Language_Null()
        {
            AssertBindingMedium(
                "<binding template='TileMedium' />",
                new TileBinding()
                {
                    Language = null
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Binding_Language_Value()
        {
            AssertBindingMedium(
                "<binding template='TileMedium' lang='en-US'/>",
                new TileBinding()
                {
                    Language = "en-US"
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Binding_Arguments_Null()
        {
            AssertBindingMedium(
                "<binding template='TileMedium' />",
                new TileBinding()
                {
                    Arguments = null
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Binding_Arguments_EmptyString()
        {
            AssertBindingMedium(
                "<binding template='TileMedium' arguments='' />",
                new TileBinding()
                {
                    Arguments = string.Empty
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Binding_Arguments_Value()
        {
            AssertBindingMedium(
                "<binding template='TileMedium' arguments='action=viewStory&amp;storyId=52' />",
                new TileBinding()
                {
                    Arguments = "action=viewStory&storyId=52"
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_Root_Defaults()
        {
            AssertBindingMedium(
                "<binding template='TileMedium' />",
                new TileBinding()
                {
                    Content = new TileBindingContentAdaptive()
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_Root_BackgroundImage_Value()
        {
            AssertBindingMedium(
                "<binding template='TileMedium'><image src='http://msn.com/image.png' placement='background' /></binding>",
                new TileBinding()
                {
                    Content = new TileBindingContentAdaptive()
                    {
                        BackgroundImage = new TileBackgroundImage()
                        {
                            Source = "http://msn.com/image.png"
                        }
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_Root_Overlay_Default()
        {
            AssertBindingMedium(
                "<binding template='TileMedium'><image src='Fable.jpg' placement='background' hint-overlay='20'/></binding>",
                new TileBinding()
                {
                    Content = new TileBindingContentAdaptive()
                    {
                        BackgroundImage = new TileBackgroundImage()
                        {
                            HintOverlay = 20,
                            Source = "Fable.jpg"
                        }
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_Root_Overlay_Min()
        {
            AssertBindingMedium(
                "<binding template='TileMedium'><image src='Fable.jpg' placement='background' hint-overlay='0'/></binding>",
                new TileBinding()
                {
                    Content = new TileBindingContentAdaptive()
                    {
                        BackgroundImage = new TileBackgroundImage()
                        {
                            HintOverlay = 0,
                            Source = "Fable.jpg"
                        }
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_Root_Overlay_Max()
        {
            AssertBindingMedium(
                "<binding template='TileMedium'><image src='Fable.jpg' placement='background' hint-overlay='100'/></binding>",
                new TileBinding()
                {
                    Content = new TileBindingContentAdaptive()
                    {
                        BackgroundImage = new TileBackgroundImage()
                        {
                            HintOverlay = 100,
                            Source = "Fable.jpg"
                        }
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_Root_Overlay_AboveDefault()
        {
            AssertBindingMedium(
                "<binding template='TileMedium'><image src='Fable.jpg' placement='background' hint-overlay='40'/></binding>",
                new TileBinding()
                {
                    Content = new TileBindingContentAdaptive()
                    {
                        BackgroundImage = new TileBackgroundImage()
                        {
                            HintOverlay = 40,
                            Source = "Fable.jpg"
                        }
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_Root_Overlay_BelowDefault()
        {
            AssertBindingMedium(
                "<binding template='TileMedium'><image src='Fable.jpg' placement='background' hint-overlay='10' /></binding>",
                new TileBinding()
                {
                    Content = new TileBindingContentAdaptive()
                    {
                        BackgroundImage = new TileBackgroundImage()
                        {
                            HintOverlay = 10,
                            Source = "Fable.jpg"
                        }
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_BackgroundImage_Overlay_BelowMin()
        {
            try
            {
                new TileBackgroundImage()
                {
                    HintOverlay = -1,
                    Source = "Fable.jpg"
                };
            }
            catch
            {
                return;
            }

            Assert.Fail("Exception should have been thrown.");
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_Root_Overlay_AboveMax()
        {
            try
            {
                new TileBackgroundImage()
                {
                    HintOverlay = 101,
                    Source = "Fable.jpg"
                };
            }
            catch
            {
                return;
            }

            Assert.Fail("Exception should have been thrown.");
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_Root_PeekImage_Value()
        {
            AssertBindingMedium(
                "<binding template='TileMedium'><image src='http://msn.com' alt='alt' addImageQuery='true' placement='peek'/></binding>",
                new TileBinding()
                {
                    Content = new TileBindingContentAdaptive()
                    {
                        PeekImage = new TilePeekImage()
                        {
                            Source = "http://msn.com",
                            AlternateText = "alt",
                            AddImageQuery = true
                        }
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_Root_TextStacking_Top()
        {
            AssertBindingMedium(
                "<binding template='TileMedium' />",
                new TileBinding()
                {
                    Content = new TileBindingContentAdaptive()
                    {
                        TextStacking = TileTextStacking.Top
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_Root_TextStacking_Center()
        {
            AssertBindingMedium(
                "<binding template='TileMedium' hint-textStacking='center'/>",
                new TileBinding()
                {
                    Content = new TileBindingContentAdaptive()
                    {
                        TextStacking = TileTextStacking.Center
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_Root_TextStacking_Bottom()
        {
            AssertBindingMedium(
                "<binding template='TileMedium' hint-textStacking='bottom'/>",
                new TileBinding()
                {
                    Content = new TileBindingContentAdaptive()
                    {
                        TextStacking = TileTextStacking.Bottom
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_BackgroundImage_Defaults()
        {
            AssertBindingMediumAdaptive(
                "<binding template='TileMedium'><image src='http://msn.com' placement='background'/></binding>",
                new TileBindingContentAdaptive()
                {
                    BackgroundImage = new TileBackgroundImage()
                    {
                        Source = "http://msn.com"
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_BackgroundImage_Source()
        {
            AssertBindingMediumAdaptive(
                "<binding template='TileMedium'><image src='http://msn.com' placement='background' addImageQuery='true' alt='MSN Image'/></binding>",
                new TileBindingContentAdaptive()
                {
                    BackgroundImage = new TileBackgroundImage()
                    {
                        Source = "http://msn.com",
                        AddImageQuery = true,
                        AlternateText = "MSN Image"
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_BackgroundImage_Crop_None()
        {
            AssertBindingMediumAdaptive(
                "<binding template='TileMedium'><image src='http://msn.com' placement='background' hint-crop='none'/></binding>",
                new TileBindingContentAdaptive()
                {
                    BackgroundImage = new TileBackgroundImage()
                    {
                        Source = "http://msn.com",
                        HintCrop = TileBackgroundImageCrop.None
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_BackgroundImage_Crop_Circle()
        {
            AssertBindingMediumAdaptive(
                "<binding template='TileMedium'><image src='http://msn.com' placement='background' hint-crop='circle'/></binding>",
                new TileBindingContentAdaptive()
                {
                    BackgroundImage = new TileBackgroundImage()
                    {
                        Source = "http://msn.com",
                        HintCrop = TileBackgroundImageCrop.Circle
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_BackgroundImage_Overlay_0()
        {
            AssertBindingMediumAdaptive(
                "<binding template='TileMedium'><image src='http://msn.com' placement='background' hint-overlay='0'/></binding>",
                new TileBindingContentAdaptive()
                {
                    BackgroundImage = new TileBackgroundImage()
                    {
                        Source = "http://msn.com",
                        HintOverlay = 0
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_BackgroundImage_Overlay_20()
        {
            AssertBindingMediumAdaptive(
                "<binding template='TileMedium'><image src='http://msn.com' placement='background' hint-overlay='20'/></binding>",
                new TileBindingContentAdaptive()
                {
                    BackgroundImage = new TileBackgroundImage()
                    {
                        Source = "http://msn.com",
                        HintOverlay = 20
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_BackgroundImage_Overlay_80()
        {
            AssertBindingMediumAdaptive(
                "<binding template='TileMedium'><image src='http://msn.com' placement='background' hint-overlay='80'/></binding>",
                new TileBindingContentAdaptive()
                {
                    BackgroundImage = new TileBackgroundImage()
                    {
                        Source = "http://msn.com",
                        HintOverlay = 80
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_BackgroundImage_NoImageSource()
        {
            try
            {
                TileContent c = new TileContent()
                {
                    Visual = new TileVisual()
                    {
                        TileMedium = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                BackgroundImage = new TileBackgroundImage()
                                {
                                    // No source, which should throw exception
                                }
                            }
                        }
                    }
                };

                c.GetContent();
            }
            catch (NullReferenceException)
            {
                return;
            }

            Assert.Fail("Exception should have been thrown");
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_PeekImage_Defaults()
        {
            AssertBindingMediumAdaptive(
                "<binding template='TileMedium'><image src='http://msn.com' placement='peek'/></binding>",
                new TileBindingContentAdaptive()
                {
                    PeekImage = new TilePeekImage()
                    {
                        Source = "http://msn.com"
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_PeekImage_Source()
        {
            AssertBindingMediumAdaptive(
                "<binding template='TileMedium'><image src='http://msn.com' placement='peek' addImageQuery='true' alt='MSN Image'/></binding>",
                new TileBindingContentAdaptive()
                {
                    PeekImage = new TilePeekImage()
                    {
                        Source = "http://msn.com",
                        AddImageQuery = true,
                        AlternateText = "MSN Image"
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_PeekImage_Crop_None()
        {
            AssertBindingMediumAdaptive(
                "<binding template='TileMedium'><image src='http://msn.com' placement='peek' hint-crop='none'/></binding>",
                new TileBindingContentAdaptive()
                {
                    PeekImage = new TilePeekImage()
                    {
                        Source = "http://msn.com",
                        HintCrop = TilePeekImageCrop.None
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_PeekImage_Crop_Circle()
        {
            AssertBindingMediumAdaptive(
                "<binding template='TileMedium'><image src='http://msn.com' placement='peek' hint-crop='circle'/></binding>",
                new TileBindingContentAdaptive()
                {
                    PeekImage = new TilePeekImage()
                    {
                        Source = "http://msn.com",
                        HintCrop = TilePeekImageCrop.Circle
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_PeekImage_Overlay_0()
        {
            AssertBindingMediumAdaptive(
                "<binding template='TileMedium'><image src='http://msn.com' placement='peek' hint-overlay='0'/></binding>",
                new TileBindingContentAdaptive()
                {
                    PeekImage = new TilePeekImage()
                    {
                        Source = "http://msn.com",
                        HintOverlay = 0
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_PeekImage_Overlay_20()
        {
            AssertBindingMediumAdaptive(
                "<binding template='TileMedium'><image src='http://msn.com' placement='peek' hint-overlay='20'/></binding>",
                new TileBindingContentAdaptive()
                {
                    PeekImage = new TilePeekImage()
                    {
                        Source = "http://msn.com",
                        HintOverlay = 20
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_PeekImage_Overlay_80()
        {
            AssertBindingMediumAdaptive(
                "<binding template='TileMedium'><image src='http://msn.com' placement='peek' hint-overlay='80'/></binding>",
                new TileBindingContentAdaptive()
                {
                    PeekImage = new TilePeekImage()
                    {
                        Source = "http://msn.com",
                        HintOverlay = 80
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_PeekImage_NoImageSource()
        {
            try
            {
                TileContent c = new TileContent()
                {
                    Visual = new TileVisual()
                    {
                        TileMedium = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                PeekImage = new TilePeekImage()
                                {
                                    // No source, which should throw exception when content retrieved
                                }
                            }
                        }
                    }
                };

                c.GetContent();
            }
            catch (NullReferenceException)
            {
                return;
            }

            Assert.Fail("Exception should have been thrown");
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_BackgroundAndPeekImage_Defaults()
        {
            AssertBindingMediumAdaptive(
                "<binding template='TileMedium'><image src='Background.jpg' placement='background'/><image src='Peek.jpg' placement='peek'/></binding>",
                new TileBindingContentAdaptive()
                {
                    BackgroundImage = new TileBackgroundImage()
                    {
                        Source = "Background.jpg"
                    },

                    PeekImage = new TilePeekImage()
                    {
                        Source = "Peek.jpg"
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_BackgroundAndPeekImage_Overlay_0and0()
        {
            AssertBindingMediumAdaptive(
                "<binding template='TileMedium'><image src='Background.jpg' placement='background' hint-overlay='0'/><image src='Peek.jpg' placement='peek' hint-overlay='0'/></binding>",
                new TileBindingContentAdaptive()
                {
                    BackgroundImage = new TileBackgroundImage()
                    {
                        Source = "Background.jpg",
                        HintOverlay = 0
                    },

                    PeekImage = new TilePeekImage()
                    {
                        Source = "Peek.jpg",
                        HintOverlay = 0
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_BackgroundAndPeekImage_Overlay_20and20()
        {
            AssertBindingMediumAdaptive(
                "<binding template='TileMedium'><image src='Background.jpg' placement='background' hint-overlay='20'/><image src='Peek.jpg' placement='peek' hint-overlay='20'/></binding>",
                new TileBindingContentAdaptive()
                {
                    BackgroundImage = new TileBackgroundImage()
                    {
                        Source = "Background.jpg",
                        HintOverlay = 20
                    },

                    PeekImage = new TilePeekImage()
                    {
                        Source = "Peek.jpg",
                        HintOverlay = 20
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_BackgroundAndPeekImage_Overlay_20and30()
        {
            AssertBindingMediumAdaptive(
                "<binding template='TileMedium'><image src='Background.jpg' placement='background' hint-overlay='20'/><image src='Peek.jpg' placement='peek' hint-overlay='30'/></binding>",
                new TileBindingContentAdaptive()
                {
                    BackgroundImage = new TileBackgroundImage()
                    {
                        Source = "Background.jpg",
                        HintOverlay = 20
                    },

                    PeekImage = new TilePeekImage()
                    {
                        Source = "Peek.jpg",
                        HintOverlay = 30
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_BackgroundAndPeekImage_Overlay_30and20()
        {
            AssertBindingMediumAdaptive(
                "<binding template='TileMedium'><image src='Background.jpg' placement='background' hint-overlay='30'/><image src='Peek.jpg' placement='peek' hint-overlay='20'/></binding>",
                new TileBindingContentAdaptive()
                {
                    BackgroundImage = new TileBackgroundImage()
                    {
                        Source = "Background.jpg",
                        HintOverlay = 30
                    },

                    PeekImage = new TilePeekImage()
                    {
                        Source = "Peek.jpg",
                        HintOverlay = 20
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Adaptive_BackgroundAndPeekImage_Overlay_0and20()
        {
            AssertBindingMediumAdaptive(
                "<binding template='TileMedium'><image src='Background.jpg' placement='background' hint-overlay='0'/><image src='Peek.jpg' placement='peek' hint-overlay='20'/></binding>",
                new TileBindingContentAdaptive()
                {
                    BackgroundImage = new TileBackgroundImage()
                    {
                        Source = "Background.jpg",
                        HintOverlay = 0
                    },

                    PeekImage = new TilePeekImage()
                    {
                        Source = "Peek.jpg",
                        HintOverlay = 20
                    }
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Special_Photos_Default()
        {
            TileBindingContentPhotos content = new TileBindingContentPhotos()
            {
            };

            AssertBindingMedium("<binding template='TileMedium' hint-presentation='photos'/>", new TileBinding()
            {
                Content = content
            });
        }

        [TestMethod]
        public void Test_Tile_Xml_Special_Photos_OneImage()
        {
            TileBindingContentPhotos content = new TileBindingContentPhotos()
            {
                Images =
                {
                    new TileBasicImage()
                    {
                        Source = "http://msn.com/1.jpg",
                        AddImageQuery = true,
                        AlternateText = "alternate"
                    }
                }
            };

            AssertBindingMedium("<binding template='TileMedium' hint-presentation='photos'><image src='http://msn.com/1.jpg' addImageQuery='true' alt='alternate'/></binding>", new TileBinding()
            {
                Content = content
            });
        }

        [TestMethod]
        public void Test_Tile_Xml_Special_Photos_TwoImages()
        {
            TileBindingContentPhotos content = new TileBindingContentPhotos()
            {
                Images =
                {
                    new TileBasicImage()
                    {
                        Source = "Assets/1.jpg"
                    },
                    new TileBasicImage()
                    {
                        Source = "Assets/2.jpg"
                    }
                }
            };

            AssertBindingMedium("<binding template='TileMedium' hint-presentation='photos'><image src='Assets/1.jpg'/><image src='Assets/2.jpg'/></binding>", new TileBinding()
            {
                Content = content
            });
        }

        [TestMethod]
        public void Test_Tile_Xml_Special_Photos_MaxImages()
        {
            TileBindingContentPhotos content = new TileBindingContentPhotos()
            {
                Images =
                {
                    new TileBasicImage() { Source = "1.jpg" },
                    new TileBasicImage() { Source = "2.jpg" },
                    new TileBasicImage() { Source = "3.jpg" },
                    new TileBasicImage() { Source = "4.jpg" },
                    new TileBasicImage() { Source = "5.jpg" },
                    new TileBasicImage() { Source = "6.jpg" },
                    new TileBasicImage() { Source = "7.jpg" },
                    new TileBasicImage() { Source = "8.jpg" },
                    new TileBasicImage() { Source = "9.jpg" },
                    new TileBasicImage() { Source = "10.jpg" },
                    new TileBasicImage() { Source = "11.jpg" },
                    new TileBasicImage() { Source = "12.jpg" }
                }
            };

            AssertBindingMedium(
                @"<binding template='TileMedium' hint-presentation='photos'>
                    <image src='1.jpg'/>
                    <image src='2.jpg'/>
                    <image src='3.jpg'/>
                    <image src='4.jpg'/>
                    <image src='5.jpg'/>
                    <image src='6.jpg'/>
                    <image src='7.jpg'/>
                    <image src='8.jpg'/>
                    <image src='9.jpg'/>
                    <image src='10.jpg'/>
                    <image src='11.jpg'/>
                    <image src='12.jpg'/>
                </binding>", new TileBinding()
                {
                    Content = content
                });
        }

        [TestMethod]
        public void Test_Tile_Xml_Special_Photos_TooManyImages()
        {
            try
            {
                new TileBindingContentPhotos()
                {
                    Images =
                    {
                        new TileBasicImage(),
                        new TileBasicImage(),
                        new TileBasicImage(),
                        new TileBasicImage(),
                        new TileBasicImage(),
                        new TileBasicImage(),
                        new TileBasicImage(),
                        new TileBasicImage(),
                        new TileBasicImage(),
                        new TileBasicImage(),
                        new TileBasicImage(),
                        new TileBasicImage(),
                        new TileBasicImage()
                    }
                };
            }
            catch
            {
                return;
            }

            Assert.Fail("Exception should have been thrown, adding more than 12 images isn't supported.");
        }

        [TestMethod]
        public void Test_Tile_Xml_Special_People_Defaults()
        {
            TileBindingContentPeople content = new TileBindingContentPeople();

            AssertBindingMedium("<binding template='TileMedium' hint-presentation='people'/>", new TileBinding()
            {
                Content = content
            });
        }

        [TestMethod]
        public void Test_Tile_Xml_Special_People_OneImage()
        {
            TileBindingContentPeople content = new TileBindingContentPeople()
            {
                Images =
                {
                    new TileBasicImage()
                    {
                        Source = "http://msn.com/1.jpg",
                        AddImageQuery = true,
                        AlternateText = "alternate"
                    }
                }
            };

            AssertBindingMedium("<binding template='TileMedium' hint-presentation='people'><image src='http://msn.com/1.jpg' addImageQuery='true' alt='alternate'/></binding>", new TileBinding()
            {
                Content = content
            });
        }

        [TestMethod]
        public void Test_Tile_Xml_Special_People_TwoImages()
        {
            TileBindingContentPeople content = new TileBindingContentPeople()
            {
                Images =
                {
                    new TileBasicImage() { Source = "Assets/1.jpg" },
                    new TileBasicImage() { Source = "Assets/2.jpg" }
                }
            };

            AssertBindingMedium("<binding template='TileMedium' hint-presentation='people'><image src='Assets/1.jpg'/><image src='Assets/2.jpg'/></binding>", new TileBinding()
            {
                Content = content
            });
        }

        [TestMethod]
        public void Test_Tile_Xml_Special_People_ManyImages()
        {
            string payload = "<binding template='TileMedium' hint-presentation='people'>";

            TileBindingContentPeople content = new TileBindingContentPeople();

            // Add 30 images
            for (int i = 1; i <= 30; i++)
            {
                string src = i + ".jpg";

                content.Images.Add(new TileBasicImage() { Source = src });
                payload += $"<image src='{src}'/>";
            }

            payload += "</binding>";

            AssertBindingMedium(payload, new TileBinding()
            {
                Content = content
            });
        }

        [TestMethod]
        public void Test_Tile_Xml_Special_Contact_Defaults()
        {
            TileBindingContentContact content = new TileBindingContentContact();

            AssertBindingMedium("<binding template='TileMedium' hint-presentation='contact'/>", new TileBinding()
            {
                Content = content
            });
        }

        [TestMethod]
        public void Test_Tile_Xml_Special_Contact_Text()
        {
            TileBindingContentContact content = new TileBindingContentContact()
            {
                Text = new TileBasicText()
                {
                    Text = "Hello world",
                    Lang = "en-US"
                }
            };

            AssertBindingMedium("<binding template='TileMedium' hint-presentation='contact'><text lang='en-US'>Hello world</text></binding>", new TileBinding()
            {
                Content = content
            });
        }

        [TestMethod]
        public void Test_Tile_Xml_Special_Contact_Image()
        {
            TileBindingContentContact content = new TileBindingContentContact()
            {
                Image = new TileBasicImage()
                {
                    Source = "http://msn.com/img.jpg",
                    AddImageQuery = true,
                    AlternateText = "John Smith"
                }
            };

            AssertBindingMedium("<binding template='TileMedium' hint-presentation='contact'><image src='http://msn.com/img.jpg' addImageQuery='true' alt='John Smith'/></binding>", new TileBinding()
            {
                Content = content
            });
        }

        [TestMethod]
        public void Test_Tile_Xml_Special_Contact_Both_Small()
        {
            TileBindingContentContact content = new TileBindingContentContact()
            {
                Text = new TileBasicText()
                {
                    Text = "Hello world"
                },

                Image = new TileBasicImage() { Source = "Assets/img.jpg" }
            };

            // Small doesn't support the text, so it doesn't output the text element when rendered for small
            AssertVisual("<visual><binding template='TileSmall' hint-presentation='contact'><image src='Assets/img.jpg'/></binding></visual>", new TileVisual()
            {
                TileSmall = new TileBinding() { Content = content }
            });
        }

        [TestMethod]
        public void Test_Tile_Xml_Special_Contact_Both_Medium()
        {
            TileBindingContentContact content = new TileBindingContentContact()
            {
                Text = new TileBasicText()
                {
                    Text = "Hello world"
                },

                Image = new TileBasicImage() { Source = "Assets/img.jpg" }
            };

            // Text is written before the image element
            AssertVisual("<visual><binding template='TileMedium' hint-presentation='contact'><text>Hello world</text><image src='Assets/img.jpg'/></binding></visual>", new TileVisual()
            {
                TileMedium = new TileBinding() { Content = content }
            });
        }

        [TestMethod]
        public void Test_Tile_Xml_Special_Iconic_Small()
        {
            AssertVisual("<visual><binding template='TileSquare71x71IconWithBadge'/></visual>", new TileVisual()
            {
                TileSmall = new TileBinding() { Content = new TileBindingContentIconic() }
            });
        }

        [TestMethod]
        public void Test_Tile_Xml_Special_Iconic_Medium()
        {
            AssertVisual("<visual><binding template='TileSquare150x150IconWithBadge'/></visual>", new TileVisual()
            {
                TileMedium = new TileBinding() { Content = new TileBindingContentIconic() }
            });
        }

        [TestMethod]
        public void Test_Tile_Xml_Special_Iconic_Image()
        {
            AssertVisual("<visual><binding template='TileSquare150x150IconWithBadge'><image id='1' src='Assets/Iconic.png' alt='iconic'/></binding></visual>", new TileVisual()
            {
                TileMedium = new TileBinding()
                {
                    Content = new TileBindingContentIconic()
                    {
                        Icon = new TileBasicImage()
                        {
                            Source = "Assets/Iconic.png",
                            AlternateText = "iconic"
                        }
                    }
                }
            });
        }

        private static void AssertBindingMediumAdaptive(string expectedBindingXml, TileBindingContentAdaptive content)
        {
            AssertBindingMedium(expectedBindingXml, new TileBinding() { Content = content });
        }

        private static void AssertBindingMedium(string expectedBindingXml, TileBinding binding)
        {
            AssertVisual("<visual>" + expectedBindingXml + "</visual>", new TileVisual()
            {
                TileMedium = binding
            });
        }

        private static void AssertVisual(string expectedVisualXml, TileVisual visual)
        {
            AssertPayload("<tile>" + expectedVisualXml + "</tile>", new TileContent()
            {
                Visual = visual
            });
        }

        private static void AssertPayload(string expectedXml, TileContent tile)
        {
            AssertHelper.AssertTile(expectedXml, tile);
        }
    }
}