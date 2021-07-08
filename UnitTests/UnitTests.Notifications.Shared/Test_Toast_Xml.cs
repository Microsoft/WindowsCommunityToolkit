// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.WinUI.Notifications;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Notifications
{
    [TestClass]
    public class Test_Toast_Xml
    {
        [TestMethod]
        public void Test_Toast_XML_Toast_Defaults()
        {
            AssertPayload("<toast/>", new ToastContent());
        }

        [TestMethod]
        public void Test_Toast_XML_Toast_Launch_Value()
        {
            var toast = new ToastContent()
            {
                Launch = "tacos"
            };

            AssertPayload("<toast launch='tacos'/>", toast);
        }

        [TestMethod]
        public void Test_Toast_XML_Toast_ActivationType_Foreground()
        {
            var toast = new ToastContent()
            {
                ActivationType = ToastActivationType.Foreground
            };

            AssertPayload("<toast />", toast);
        }

        [TestMethod]
        public void Test_Toast_XML_Toast_ActivationType_Background()
        {
            var toast = new ToastContent()
            {
                ActivationType = ToastActivationType.Background
            };

            AssertPayload("<toast activationType='background' />", toast);
        }

        [TestMethod]
        public void Test_Toast_XML_Toast_ActivationType_Protocol()
        {
            var toast = new ToastContent()
            {
                ActivationType = ToastActivationType.Protocol
            };

            AssertPayload("<toast activationType='protocol' />", toast);
        }

        [TestMethod]
        public void Test_Toast_XML_Toast_Duration_Short()
        {
            var toast = new ToastContent()
            {
                Duration = ToastDuration.Short
            };

            AssertPayload("<toast />", toast);
        }

        [TestMethod]
        public void Test_Toast_XML_Toast_Duration_Long()
        {
            var toast = new ToastContent()
            {
                Duration = ToastDuration.Long
            };

            AssertPayload("<toast duration='long' />", toast);
        }

        [TestMethod]
        public void Test_Toast_XML_Toast_HintToastId()
        {
            var toast = new ToastContent()
            {
                HintToastId = "AppointmentReminder"
            };

            AssertPayload("<toast hint-toastId='AppointmentReminder' />", toast);
        }

        [TestMethod]
        public void Test_Toast_XML_Toast_HintPeople_RemoteId()
        {
            var toast = new ToastContent()
            {
                HintPeople = new ToastPeople()
                {
                    RemoteId = "1234"
                }
            };

            AssertPayload("<toast hint-people='remoteid:1234' />", toast);
        }

        [TestMethod]
        public void Test_Toast_XML_Toast_HintPeople_EmailAddress()
        {
            var toast = new ToastContent()
            {
                HintPeople = new ToastPeople()
                {
                    EmailAddress = "johndoe@mydomain.com"
                }
            };

            AssertPayload("<toast hint-people='mailto:johndoe@mydomain.com' />", toast);
        }

        [TestMethod]
        public void Test_Toast_XML_Toast_HintPeople_PhoneNumber()
        {
            var toast = new ToastContent()
            {
                HintPeople = new ToastPeople()
                {
                    PhoneNumber = "888-888-8888"
                }
            };

            AssertPayload("<toast hint-people='tel:888-888-8888' />", toast);
        }

        [TestMethod]
        public void Test_Toast_XML_Toast_HintPeople_Precedence()
        {
            // Email should take precedence over phone number
            var toast = new ToastContent()
            {
                HintPeople = new ToastPeople()
                {
                    EmailAddress = "johndoe@mydomain.com",
                    PhoneNumber = "888-888-8888"
                }
            };

            AssertPayload("<toast hint-people='mailto:johndoe@mydomain.com' />", toast);

            // RemoteId should take precedence over phone number
            toast.HintPeople = new ToastPeople()
            {
                RemoteId = "1234",
                PhoneNumber = "888-888-8888"
            };

            AssertPayload("<toast hint-people='remoteid:1234' />", toast);

            // RemoteId should take precedence over all
            toast.HintPeople = new ToastPeople()
            {
                RemoteId = "1234",
                PhoneNumber = "888-888-8888",
                EmailAddress = "johndoe@mydomain.com"
            };

            AssertPayload("<toast hint-people='remoteid:1234' />", toast);
        }

        [TestMethod]
        public void Test_Toast_XML_Toast_AdditionalProperties()
        {
            AssertPayload("<toast hint-tacos='yummy://kind=beans,c=0' />", new ToastContent()
            {
                AdditionalProperties =
                {
                    { "hint-tacos", "yummy://kind=beans,c=0" }
                }
            });

            // Multiple
            AssertPayload("<toast avacado='definitely' burrito='true' />", new ToastContent()
            {
                AdditionalProperties =
                {
                    { "burrito", "true" },
                    { "avacado", "definitely" }
                }
            });

            // XML encoding
            AssertPayload("<toast request='eggs &amp; beans' />", new ToastContent()
            {
                AdditionalProperties =
                {
                    { "request", "eggs & beans" }
                }
            });
        }

        [TestMethod]
        public void Test_ToastV2_Visual_Defaults()
        {
            AssertPayload("<toast></toast>", new ToastContent());
        }

        [TestMethod]
        public void Test_ToastV2_Visual_AddImageQuery_False()
        {
            var visual = new ToastVisual()
            {
                AddImageQuery = false
            };

            AssertVisualPayloadProperties(@"addImageQuery='false'", visual);
        }

        [TestMethod]
        public void Test_ToastV2_Visual_AddImageQuery_True()
        {
            var visual = new ToastVisual()
            {
                AddImageQuery = true
            };

            AssertVisualPayloadProperties(@"addImageQuery=""true""", visual);
        }

        [TestMethod]
        public void Test_ToastV2_Visual_BaseUri_Value()
        {
            var visual = new ToastVisual()
            {
                BaseUri = new Uri("http://msn.com")
            };

            AssertVisualPayloadProperties(@"baseUri=""http://msn.com/""", visual);
        }

        [TestMethod]
        public void Test_ToastV2_Visual_Language_Value()
        {
            var visual = new ToastVisual()
            {
                Language = "en-US"
            };

            AssertVisualPayloadProperties(@"lang=""en-US""", visual);
        }

        [TestMethod]
        public void Test_ToastV2_Visual_AdaptiveText_Defaults()
        {
            AssertAdaptiveText(@"<text/>", new AdaptiveText());
        }

        [TestMethod]
        public void Test_ToastV2_Visual_AdaptiveText_All()
        {
            AssertAdaptiveText(@"<text lang=""en-US"" hint-align=""right"" hint-maxLines=""3"" hint-minLines=""2"" hint-style=""header"" hint-wrap=""true"">Hi, I am a title</text>", new AdaptiveText()
            {
                Text = "Hi, I am a title",
                Language = "en-US",
                HintAlign = AdaptiveTextAlign.Right,
                HintMaxLines = 3,
                HintMinLines = 2,
                HintStyle = AdaptiveTextStyle.Header,
                HintWrap = true
            });
        }

        [TestMethod]
        public void Test_ToastV2_Xml_Attribution()
        {
            var visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = "My title"
                        },

                        new AdaptiveText()
                        {
                            Text = "My body 1"
                        }
                    },

                    Attribution = new ToastGenericAttributionText()
                    {
                        Text = "cnn.com"
                    }
                }
            };

            AssertVisualPayload(@"<visual><binding template=""ToastGeneric""><text>My title</text><text>My body 1</text><text placement='attribution'>cnn.com</text></binding></visual>", visual);
        }

        [TestMethod]
        public void Test_ToastV2_Xml_Attribution_Lang()
        {
            var visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = "My title"
                        },

                        new AdaptiveText()
                        {
                            Text = "My body 1"
                        }
                    },

                    Attribution = new ToastGenericAttributionText()
                    {
                        Text = "cnn.com",
                        Language = "en-US"
                    }
                }
            };

            AssertVisualPayload(@"<visual><binding template=""ToastGeneric""><text>My title</text><text>My body 1</text><text placement='attribution' lang='en-US'>cnn.com</text></binding></visual>", visual);
        }

        [TestMethod]
        public void Test_ToastV2_BindingGeneric_BaseUri()
        {
            AssertBindingGenericProperty("baseUri", "http://msn.com/images/", new ToastBindingGeneric()
            {
                BaseUri = new Uri("http://msn.com/images/", UriKind.Absolute)
            });
        }

        [TestMethod]
        public void Test_ToastV2_BindingGeneric_AddImageQuery()
        {
            AssertBindingGenericProperty("addImageQuery", "false", new ToastBindingGeneric()
            {
                AddImageQuery = false
            });

            AssertBindingGenericProperty("addImageQuery", "true", new ToastBindingGeneric()
            {
                AddImageQuery = true
            });
        }

        [TestMethod]
        public void Test_ToastV2_BindingGeneric_Language()
        {
            AssertBindingGenericProperty("lang", "en-US", new ToastBindingGeneric()
            {
                Language = "en-US"
            });
        }

        [TestMethod]
        public void Test_ToastV2_BindingGeneric_DefaultNullValues()
        {
            AssertBindingGenericPayload("<binding template='ToastGeneric' />", new ToastBindingGeneric()
            {
                AddImageQuery = null,
                AppLogoOverride = null,
                BaseUri = null,
                Language = null,
                HeroImage = null,
                Attribution = null
            });
        }

        private static void AssertBindingGenericProperty(string expectedPropertyName, string expectedPropertyValue, ToastBindingGeneric binding)
        {
            AssertBindingGenericPayload($"<binding template='ToastGeneric' {expectedPropertyName}='{expectedPropertyValue}'/>", binding);
        }

        [TestMethod]
        public void Test_ToastV2_BindingShoulderTap_BaseUri()
        {
            AssertBindingShoulderTapProperty("baseUri", "http://msn.com/images/", new ToastBindingShoulderTap()
            {
                BaseUri = new Uri("http://msn.com/images/", UriKind.Absolute)
            });
        }

        [TestMethod]
        public void Test_ToastV2_BindingShoulderTap_AddImageQuery()
        {
            AssertBindingShoulderTapProperty("addImageQuery", "false", new ToastBindingShoulderTap()
            {
                AddImageQuery = false
            });

            AssertBindingShoulderTapProperty("addImageQuery", "true", new ToastBindingShoulderTap()
            {
                AddImageQuery = true
            });
        }

        [TestMethod]
        public void Test_ToastV2_BindingShoulderTap_Language()
        {
            AssertBindingShoulderTapProperty("lang", "en-US", new ToastBindingShoulderTap()
            {
                Language = "en-US"
            });
        }

        private static void AssertBindingShoulderTapProperty(string expectedPropertyName, string expectedPropertyValue, ToastBindingShoulderTap binding)
        {
            AssertBindingShoulderTapPayload($"<binding template='ToastGeneric' experienceType='shoulderTap' {expectedPropertyName}='{expectedPropertyValue}'/>", binding);
        }

        [TestMethod]
        public void Test_ToastV2_ShoulderTapImage()
        {
            AssertShoulderTapImagePayload("<image src='img.png' addImageQuery='true' alt='alt text'/>", new ToastShoulderTapImage()
            {
                Source = "img.png",
                AddImageQuery = true,
                AlternateText = "alt text"
            });

            // Defaults shouldn't have anything assigned
            AssertShoulderTapImagePayload("<image src='img.png'/>", new ToastShoulderTapImage()
            {
                Source = "img.png"
            });
        }

        [TestMethod]
        public void Test_ToastV2_ShoulderTapImage_SourceRequired()
        {
            Assert.ThrowsException<NullReferenceException>(() =>
            {
                AssertShoulderTapImagePayload("exception should be thrown", new ToastShoulderTapImage());
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new ToastShoulderTapImage()
                {
                    Source = null
                };
            });
        }

        private static void AssertShoulderTapImagePayload(string expectedImageXml, ToastShoulderTapImage image)
        {
            AssertBindingShoulderTapPayload($"<binding template='ToastGeneric' experienceType='shoulderTap'>{expectedImageXml}</binding>", new ToastBindingShoulderTap()
            {
                Image = image
            });
        }

        [TestMethod]
        public void Test_ToastV2_SpriteSheet()
        {
            AssertSpriteSheetProperties("spritesheet-src='sprite.png' spritesheet-height='80' spritesheet-fps='25' spritesheet-startingFrame='15'", new ToastSpriteSheet()
            {
                Source = "sprite.png",
                FrameHeight = 80,
                Fps = 25,
                StartingFrame = 15
            });

            // Defaults shouldn't have anything assigned
            AssertSpriteSheetProperties("spritesheet-src='sprite.png'", new ToastSpriteSheet()
            {
                Source = "sprite.png"
            });

            // Can assign invalid values
            AssertSpriteSheetProperties("spritesheet-src='sprite.png' spritesheet-height='0' spritesheet-fps='150' spritesheet-startingFrame='15'", new ToastSpriteSheet()
            {
                Source = "sprite.png",
                FrameHeight = 0,
                Fps = 150,
                StartingFrame = 15
            });
        }

        [TestMethod]
        public void Test_ToastV2_SpriteSheet_SourceRequired()
        {
            Assert.ThrowsException<NullReferenceException>(() =>
            {
                AssertSpriteSheetProperties("exception should be thrown", new ToastSpriteSheet());
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new ToastSpriteSheet()
                {
                    Source = null
                };
            });
        }

        private static void AssertSpriteSheetProperties(string expectedProperties, ToastSpriteSheet spriteSheet)
        {
            AssertShoulderTapImagePayload($"<image src='img.png' {expectedProperties} />", new ToastShoulderTapImage()
            {
                Source = "img.png",
                SpriteSheet = spriteSheet
            });
        }

        private static void AssertBindingShoulderTapPayload(string expectedBindingXml, ToastBindingShoulderTap binding)
        {
            AssertVisualPayload("<visual><binding template='ToastGeneric'/>" + expectedBindingXml + "</visual>", new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric(),
                BindingShoulderTap = binding
            });
        }

        [TestMethod]
        public void Test_ToastV2_AppLogo_Crop_None()
        {
            var appLogo = new ToastGenericAppLogo()
            {
                HintCrop = ToastGenericAppLogoCrop.None,
                Source = "img.png"
            };

            AssertAppLogoPayload(@"<image src='img.png' placement=""appLogoOverride"" hint-crop='none'/>", appLogo);
        }

        [TestMethod]
        public void Test_ToastV2_AppLogo_Crop_Circle()
        {
            var appLogo = new ToastGenericAppLogo()
            {
                HintCrop = ToastGenericAppLogoCrop.Circle,
                Source = "img.png"
            };

            AssertAppLogoPayload(@"<image src=""img.png"" placement=""appLogoOverride"" hint-crop=""circle""/>", appLogo);
        }

        [TestMethod]
        public void Test_ToastV2_AppLogo_Source_Defaults()
        {
            var appLogo = new ToastGenericAppLogo()
            {
                Source = "http://xbox.com/Avatar.jpg"
            };

            AssertAppLogoPayload(@"<image placement=""appLogoOverride"" src=""http://xbox.com/Avatar.jpg""/>", appLogo);
        }

        [TestMethod]
        public void Test_ToastV2_AppLogo_Source_Alt()
        {
            var appLogo = new ToastGenericAppLogo()
            {
                Source = "http://xbox.com/Avatar.jpg",
                AlternateText = "alternate"
            };

            AssertAppLogoPayload(@"<image placement=""appLogoOverride"" src=""http://xbox.com/Avatar.jpg"" alt=""alternate""/>", appLogo);
        }

        [TestMethod]
        public void Test_ToastV2_AppLogo_Source_AddImageQuery_False()
        {
            var appLogo = new ToastGenericAppLogo()
            {
                Source = "http://xbox.com/Avatar.jpg",
                AddImageQuery = false
            };

            AssertAppLogoPayload(@"<image placement=""appLogoOverride"" src=""http://xbox.com/Avatar.jpg"" addImageQuery='false'/>", appLogo);
        }

        [TestMethod]
        public void Test_ToastV2_AppLogo_Source_AddImageQuery_True()
        {
            var appLogo = new ToastGenericAppLogo()
            {
                Source = "http://xbox.com/Avatar.jpg",
                AddImageQuery = true
            };

            AssertAppLogoPayload(@"<image placement=""appLogoOverride"" src=""http://xbox.com/Avatar.jpg"" addImageQuery=""true""/>", appLogo);
        }

        [TestMethod]
        public void Test_ToastV2_Xml_HeroImage_Default()
        {
            var hero = new ToastGenericHeroImage();

            try
            {
                AssertHeroImagePayload("<image placement='hero'/>", hero);
            }
            catch (NullReferenceException)
            {
                return;
            }

            Assert.Fail("Exception should have been thrown since Source wasn't provided.");
        }

        [TestMethod]
        public void Test_ToastV2_Xml_HeroImage_WithSource()
        {
            var hero = new ToastGenericHeroImage()
            {
                Source = "http://food.com/peanuts.jpg"
            };

            AssertHeroImagePayload("<image placement='hero' src='http://food.com/peanuts.jpg'/>", hero);
        }

        [TestMethod]
        public void Test_ToastV2_Xml_HeroImage_Alt()
        {
            var hero = new ToastGenericHeroImage()
            {
                Source = "http://food.com/peanuts.jpg",
                AlternateText = "peanuts"
            };

            AssertHeroImagePayload("<image placement='hero' src='http://food.com/peanuts.jpg' alt='peanuts'/>", hero);
        }

        [TestMethod]
        public void Test_ToastV2_Xml_HeroImage_AddImageQuery()
        {
            var hero = new ToastGenericHeroImage()
            {
                Source = "http://food.com/peanuts.jpg",
                AddImageQuery = true
            };

            AssertHeroImagePayload("<image placement='hero' src='http://food.com/peanuts.jpg' addImageQuery='true'/>", hero);
        }

        [TestMethod]
        public void Test_ToastV2_Xml_HeroImage_AllProperties()
        {
            var hero = new ToastGenericHeroImage()
            {
                Source = "http://food.com/peanuts.jpg",
                AddImageQuery = true,
                AlternateText = "peanuts"
            };

            AssertHeroImagePayload("<image placement='hero' src='http://food.com/peanuts.jpg' addImageQuery='true' alt='peanuts'/>", hero);
        }

        private static ToastContent GenerateFromVisual(ToastVisual visual)
        {
            return new ToastContent()
            {
                Visual = visual
            };
        }

        /// <summary>
        /// Used for testing properties of visual without needing to specify the Generic binding
        /// </summary>
        private static void AssertVisualPayloadProperties(string expectedVisualProperties, ToastVisual visual)
        {
            visual.BindingGeneric = new ToastBindingGeneric();

            AssertVisualPayload("<visual " + expectedVisualProperties + "><binding template='ToastGeneric'></binding></visual>", visual);
        }

        private static void AssertBindingGenericPayload(string expectedBindingXml, ToastBindingGeneric binding)
        {
            AssertVisualPayload("<visual>" + expectedBindingXml + "</visual>", new ToastVisual()
            {
                BindingGeneric = binding
            });
        }

        private static void AssertAdaptiveText(string expectedAdaptiveTextXml, AdaptiveText text)
        {
            AssertBindingGenericPayload("<binding template='ToastGeneric'>" + expectedAdaptiveTextXml + "</binding>", new ToastBindingGeneric()
            {
                Children =
                {
                    text
                }
            });
        }

        private static void AssertAppLogoPayload(string expectedAppLogoXml, ToastGenericAppLogo appLogo)
        {
            AssertVisualPayload(@"<visual><binding template=""ToastGeneric"">" + expectedAppLogoXml + "</binding></visual>", new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    AppLogoOverride = appLogo
                }
            });
        }

        private static void AssertHeroImagePayload(string expectedHeroXml, ToastGenericHeroImage heroImage)
        {
            AssertVisualPayload(@"<visual><binding template=""ToastGeneric"">" + expectedHeroXml + "</binding></visual>", new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    HeroImage = heroImage
                }
            });
        }

        [TestMethod]
        public void Test_Toast_Xml_Audio_Defaults()
        {
            var audio = new ToastAudio();

            AssertAudioPayload("<audio />", audio);
        }

        [TestMethod]
        public void Test_Toast_Xml_Audio_Loop_False()
        {
            var audio = new ToastAudio()
            {
                Loop = false
            };

            AssertAudioPayload("<audio />", audio);
        }

        [TestMethod]
        public void Test_Toast_Xml_Audio_Loop_True()
        {
            var audio = new ToastAudio()
            {
                Loop = true
            };

            AssertAudioPayload("<audio loop='true'/>", audio);
        }

        [TestMethod]
        public void Test_Toast_Xml_Audio_Silent_False()
        {
            var audio = new ToastAudio()
            {
                Silent = false
            };

            AssertAudioPayload("<audio/>", audio);
        }

        [TestMethod]
        public void Test_Toast_Xml_Audio_Silent_True()
        {
            var audio = new ToastAudio()
            {
                Silent = true
            };

            AssertAudioPayload("<audio silent='true'/>", audio);
        }

        [TestMethod]
        public void Test_Toast_Xml_Audio_Src_Value()
        {
            var audio = new ToastAudio()
            {
                Src = new Uri("ms-appx:///Assets/audio.mp3")
            };

            AssertAudioPayload("<audio src='ms-appx:///Assets/audio.mp3'/>", audio);
        }

        [TestMethod]
        public void Test_Toast_Xml_Actions_SnoozeAndDismiss()
        {
            AssertActionsPayload("<actions hint-systemCommands='SnoozeAndDismiss'/>", new ToastActionsSnoozeAndDismiss());
        }

        [TestMethod]
        public void Test_Toast_Xml_Actions_Custom_Defaults()
        {
            AssertActionsPayload("<actions/>", new ToastActionsCustom());
        }

        [TestMethod]
        public void Test_Toast_Xml_Actions_TextBoxAndButton()
        {
            AssertActionsPayload("<actions><input id='tb1' type='text'/><action content='Click me!' arguments='clickArgs'/></actions>", new ToastActionsCustom()
            {
                Buttons =
                {
                    new ToastButton("Click me!", "clickArgs")
                },

                Inputs =
                {
                    new ToastTextBox("tb1")
                }
            });
        }

        [TestMethod]
        public void Test_Toast_Xml_Actions_TwoTextBoxes()
        {
            AssertActionsPayload("<actions><input id='tb1' type='text'/><input id='tb2' type='text'/></actions>", new ToastActionsCustom()
            {
                Inputs =
                {
                    new ToastTextBox("tb1"),
                    new ToastTextBox("tb2")
                }
            });
        }

        [TestMethod]
        public void Test_Toast_Xml_Actions_FiveTextBoxes()
        {
            AssertActionsPayload("<actions><input id='tb1' type='text'/><input id='tb2' type='text'/><input id='tb3' type='text'/><input id='tb4' type='text'/><input id='tb5' type='text'/></actions>", new ToastActionsCustom()
            {
                Inputs =
                {
                    new ToastTextBox("tb1"),
                    new ToastTextBox("tb2"),
                    new ToastTextBox("tb3"),
                    new ToastTextBox("tb4"),
                    new ToastTextBox("tb5")
                }
            });
        }

        [TestMethod]
        public void Test_Toast_Xml_Actions_SixTextBoxes()
        {
            try
            {
                new ToastActionsCustom()
                {
                    Inputs =
                    {
                        new ToastTextBox("tb1"),
                        new ToastTextBox("tb2"),
                        new ToastTextBox("tb3"),
                        new ToastTextBox("tb4"),
                        new ToastTextBox("tb5"),
                        new ToastTextBox("tb6")
                    }
                };
            }
            catch
            {
                return;
            }

            Assert.Fail("Exception should have been thrown.");
        }

        [TestMethod]
        public void Test_Toast_Xml_Actions_SelectionAndButton()
        {
            AssertActionsPayload("<actions><input id='s1' type='selection'><selection id='1' content='First'/><selection id='2' content='Second'/></input><action content='Click me!' arguments='clickArgs'/></actions>", new ToastActionsCustom()
            {
                Inputs =
                {
                    new ToastSelectionBox("s1")
                    {
                        Items =
                        {
                            new ToastSelectionBoxItem("1", "First"),
                            new ToastSelectionBoxItem("2", "Second")
                        }
                    }
                },

                Buttons =
                {
                    new ToastButton("Click me!", "clickArgs")
                }
            });
        }

        [TestMethod]
        public void Test_Toast_Xml_Actions_TwoButtons()
        {
            AssertActionsPayload("<actions><action content='Button 1' arguments='1'/><action content='Button 2' arguments='2'/></actions>", new ToastActionsCustom()
            {
                Buttons =
                {
                    new ToastButton("Button 1", "1"),
                    new ToastButton("Button 2", "2")
                }
            });
        }

        [TestMethod]
        public void Test_Toast_Xml_Actions_FiveButtons()
        {
            AssertActionsPayload("<actions><action content='Button 1' arguments='1'/><action content='Button 2' arguments='2'/><action content='Button 3' arguments='3'/><action content='Button 4' arguments='4'/><action content='Button 5' arguments='5'/></actions>", new ToastActionsCustom()
            {
                Buttons =
                {
                    new ToastButton("Button 1", "1"),
                    new ToastButton("Button 2", "2"),
                    new ToastButton("Button 3", "3"),
                    new ToastButton("Button 4", "4"),
                    new ToastButton("Button 5", "5")
                }
            });
        }

        [TestMethod]
        public void Test_Toast_Xml_Actions_SixButtons()
        {
            try
            {
                new ToastActionsCustom()
                {
                    Buttons =
                    {
                        new ToastButton("Button 1", "1"),
                        new ToastButton("Button 2", "2"),
                        new ToastButton("Button 3", "3"),
                        new ToastButton("Button 4", "4"),
                        new ToastButton("Button 5", "5"),
                        new ToastButton("Button 6", "6")
                    }
                };
            }
            catch
            {
                return;
            }

            Assert.Fail("Exception should have been thrown.");
        }

        [TestMethod]
        public void Test_Toast_Xml_Actions_SixTotal()
        {
            try
            {
                AssertActionsPayload("doesn't matter", new ToastActionsCustom()
                {
                    Buttons =
                    {
                        new ToastButton("Button 1", "1"),
                        new ToastButton("Button 2", "2"),
                        new ToastButton("Button 3", "3"),
                        new ToastButton("Button 4", "4"),
                        new ToastButton("Button 5", "5")
                    },

                    ContextMenuItems =
                    {
                        new ToastContextMenuItem("Menu item 1", "1")
                    }
                });
            }
            catch (InvalidOperationException)
            {
                return;
            }

            Assert.Fail("Exception should have been thrown, only 5 actions are allowed.");
        }

        [TestMethod]
        public void Test_Toast_Xml_Actions_SnoozeAndDismissUsingBuilders()
        {
            AssertActionsPayload("<actions><action content='' activationType='system' arguments='snooze'/><action content='' activationType='system' arguments='dismiss'/><action content='Hide' activationType='system' arguments='dismiss'/><action content='Later' activationType='system' arguments='snooze'/><action content='Remind me' activationType='system' arguments='snooze' hint-inputId='snoozePicker'/></actions>", new ToastActionsCustom()
            {
                Buttons =
                {
                    // Allowing system to auto-generate text content
                    new ToastButton()
                        .SetSnoozeActivation(),

                    // Allowing system to auto-generate text content
                    new ToastButton()
                        .SetDismissActivation(),

                    // Specifying specific content
                    new ToastButton()
                        .SetContent("Hide")
                        .SetDismissActivation(),

                    new ToastButton()
                        .SetContent("Later")
                        .SetSnoozeActivation(),

                    new ToastButton()
                        .SetContent("Remind me")
                        .SetSnoozeActivation("snoozePicker")
                }
            });
        }

        [TestMethod]
        public void Test_Toast_Xml_Actions_TwoContextMenuItems()
        {
            AssertActionsPayload("<actions><action placement='contextMenu' content='Menu item 1' arguments='1'/><action placement='contextMenu' content='Menu item 2' arguments='2'/></actions>", new ToastActionsCustom()
            {
                ContextMenuItems =
                {
                    new ToastContextMenuItem("Menu item 1", "1"),
                    new ToastContextMenuItem("Menu item 2", "2")
                }
            });
        }

        [TestMethod]
        public void Test_Toast_Xml_Actions_FiveContextMenuItems()
        {
            AssertActionsPayload("<actions><action placement='contextMenu' content='Menu item 1' arguments='1'/><action placement='contextMenu' content='Menu item 2' arguments='2'/><action placement='contextMenu' content='Menu item 3' arguments='3'/><action placement='contextMenu' content='Menu item 4' arguments='4'/><action placement='contextMenu' content='Menu item 5' arguments='5'/></actions>", new ToastActionsCustom()
            {
                ContextMenuItems =
                {
                    new ToastContextMenuItem("Menu item 1", "1"),
                    new ToastContextMenuItem("Menu item 2", "2"),
                    new ToastContextMenuItem("Menu item 3", "3"),
                    new ToastContextMenuItem("Menu item 4", "4"),
                    new ToastContextMenuItem("Menu item 5", "5")
                }
            });
        }

        [TestMethod]
        public void Test_Toast_Xml_Actions_SixContextMenuItems()
        {
            try
            {
                AssertActionsPayload("doesn't matter", new ToastActionsCustom()
                {
                    ContextMenuItems =
                    {
                        new ToastContextMenuItem("Menu item 1", "1"),
                        new ToastContextMenuItem("Menu item 2", "2"),
                        new ToastContextMenuItem("Menu item 3", "3"),
                        new ToastContextMenuItem("Menu item 4", "4"),
                        new ToastContextMenuItem("Menu item 5", "5"),
                        new ToastContextMenuItem("Menu item 6", "6")
                    }
                });
            }
            catch
            {
                return;
            }

            Assert.Fail("Exception should have been thrown.");
        }

        [TestMethod]
        public void Test_Toast_Xml_ActionsSnoozeDismiss_TwoContextMenuItems()
        {
            AssertActionsPayload("<actions hint-systemCommands='SnoozeAndDismiss'><action placement='contextMenu' content='Menu item 1' arguments='1'/><action placement='contextMenu' content='Menu item 2' arguments='2'/></actions>", new ToastActionsSnoozeAndDismiss()
            {
                ContextMenuItems =
                {
                    new ToastContextMenuItem("Menu item 1", "1"),
                    new ToastContextMenuItem("Menu item 2", "2")
                }
            });
        }

        [TestMethod]
        public void Test_Toast_Xml_Actions_ActionsSnoozeDismiss_FiveContextMenuItems()
        {
            AssertActionsPayload("<actions hint-systemCommands='SnoozeAndDismiss'><action placement='contextMenu' content='Menu item 1' arguments='1'/><action placement='contextMenu' content='Menu item 2' arguments='2'/><action placement='contextMenu' content='Menu item 3' arguments='3'/><action placement='contextMenu' content='Menu item 4' arguments='4'/><action placement='contextMenu' content='Menu item 5' arguments='5'/></actions>", new ToastActionsSnoozeAndDismiss()
            {
                ContextMenuItems =
                {
                    new ToastContextMenuItem("Menu item 1", "1"),
                    new ToastContextMenuItem("Menu item 2", "2"),
                    new ToastContextMenuItem("Menu item 3", "3"),
                    new ToastContextMenuItem("Menu item 4", "4"),
                    new ToastContextMenuItem("Menu item 5", "5")
                }
            });
        }

        [TestMethod]
        public void Test_Toast_Xml_Actions_ActionsSnoozeDismiss_SixContextMenuItems()
        {
            try
            {
                AssertActionsPayload("doesn't matter", new ToastActionsSnoozeAndDismiss()
                {
                    ContextMenuItems =
                    {
                        new ToastContextMenuItem("Menu item 1", "1"),
                        new ToastContextMenuItem("Menu item 2", "2"),
                        new ToastContextMenuItem("Menu item 3", "3"),
                        new ToastContextMenuItem("Menu item 4", "4"),
                        new ToastContextMenuItem("Menu item 5", "5"),
                        new ToastContextMenuItem("Menu item 6", "6")
                    }
                });
            }
            catch
            {
                return;
            }

            Assert.Fail("Exception should have been thrown.");
        }

        [TestMethod]
        public void Test_Toast_Xml_Button_Defaults()
        {
            ToastButton button = new ToastButton("my content", "myArgs");

            AssertButtonPayload("<action content='my content' arguments='myArgs' />", button);
        }

        [TestMethod]
        public void Test_Toast_Xml_Button_NullContent()
        {
            try
            {
                new ToastButton(null, "args");
            }
            catch
            {
                return;
            }

            Assert.Fail("Exception should have been thrown.");
        }

        [TestMethod]
        public void Test_Toast_Xml_Button_NullArguments()
        {
            try
            {
                new ToastButton("content", null);
            }
            catch
            {
                return;
            }

            Assert.Fail("Exception should have been thrown.");
        }

        [TestMethod]
        public void Test_Toast_Xml_Button_ActivationType_Foreground()
        {
            ToastButton button = new ToastButton("my content", "myArgs")
            {
                ActivationType = ToastActivationType.Foreground
            };

            AssertButtonPayload("<action content='my content' arguments='myArgs' />", button);
        }

        [TestMethod]
        public void Test_Toast_Xml_Button_ActivationType_Background()
        {
            ToastButton button = new ToastButton("my content", "myArgs")
            {
                ActivationType = ToastActivationType.Background
            };

            AssertButtonPayload("<action content='my content' arguments='myArgs' activationType='background' />", button);
        }

        [TestMethod]
        public void Test_Toast_Xml_Button_ActivationType_Protocol()
        {
            ToastButton button = new ToastButton("my content", "myArgs")
            {
                ActivationType = ToastActivationType.Protocol
            };

            AssertButtonPayload("<action content='my content' arguments='myArgs' activationType='protocol' />", button);
        }

        [TestMethod]
        public void Test_Toast_Xml_Button_ImageUri_Value()
        {
            ToastButton button = new ToastButton("my content", "myArgs")
            {
                ImageUri = "Assets/button.png"
            };

            AssertButtonPayload("<action content='my content' arguments='myArgs' imageUri='Assets/button.png' />", button);
        }

        [TestMethod]
        public void Test_Toast_Xml_Button_TextBoxId_Value()
        {
            ToastButton button = new ToastButton("my content", "myArgs")
            {
                TextBoxId = "myTextBox"
            };

            AssertButtonPayload("<action content='my content' arguments='myArgs' hint-inputId='myTextBox' />", button);
        }

        [TestMethod]
        public void Test_Toast_Xml_Button_HintActionId_Value()
        {
            ToastButton button = new ToastButton("my content", "myArgs")
            {
                HintActionId = "MyAction1"
            };

            AssertButtonPayload("<action content='my content' arguments='myArgs' hint-actionId='MyAction1' />", button);
        }

        [TestMethod]
        public void Test_Toast_Xml_ButtonSnooze_Defaults()
        {
            ToastButtonSnooze button = new ToastButtonSnooze();

            AssertButtonPayload("<action activationType='system' arguments='snooze' content=''/>", button);
        }

        [TestMethod]
        public void Test_Toast_Xml_ButtonSnooze_CustomContent()
        {
            ToastButtonSnooze button = new ToastButtonSnooze("my snooze");

            AssertButtonPayload("<action activationType='system' arguments='snooze' content='my snooze'/>", button);
        }

        [TestMethod]
        public void Test_Toast_Xml_ButtonSnooze_Image()
        {
            ToastButtonSnooze button = new ToastButtonSnooze()
            {
                ImageUri = "Assets/Snooze.png"
            };

            AssertButtonPayload("<action activationType='system' arguments='snooze' content='' imageUri='Assets/Snooze.png'/>", button);
        }

        [TestMethod]
        public void Test_Toast_Xml_ButtonSnooze_SelectionId()
        {
            ToastButtonSnooze button = new ToastButtonSnooze()
            {
                SelectionBoxId = "snoozeId"
            };

            AssertButtonPayload("<action activationType='system' arguments='snooze' content='' hint-inputId='snoozeId'/>", button);
        }

        [TestMethod]
        public void Test_Toast_Xml_ButtonSnooze_HintActionId()
        {
            ToastButtonSnooze button = new ToastButtonSnooze()
            {
                HintActionId = "MySnoozeButton1"
            };

            AssertButtonPayload("<action activationType='system' arguments='snooze' content='' hint-actionId='MySnoozeButton1'/>", button);
        }

        [TestMethod]
        public void Test_Toast_Xml_ButtonDismiss_Defaults()
        {
            ToastButtonDismiss button = new ToastButtonDismiss();

            AssertButtonPayload("<action activationType='system' arguments='dismiss' content=''/>", button);
        }

        [TestMethod]
        public void Test_Toast_Xml_ButtonDismiss_CustomContent()
        {
            ToastButtonDismiss button = new ToastButtonDismiss("my dismiss");

            AssertButtonPayload("<action activationType='system' arguments='dismiss' content='my dismiss'/>", button);
        }

        [TestMethod]
        public void Test_Toast_Xml_ButtonDismiss_Image()
        {
            ToastButtonDismiss button = new ToastButtonDismiss()
            {
                ImageUri = "Assets/Dismiss.png"
            };

            AssertButtonPayload("<action activationType='system' arguments='dismiss' content='' imageUri='Assets/Dismiss.png'/>", button);
        }

        [TestMethod]
        public void Test_Toast_Xml_ButtonDismiss_HintActionId()
        {
            ToastButtonDismiss button = new ToastButtonDismiss()
            {
                HintActionId = "MyDismissButton1"
            };

            AssertButtonPayload("<action activationType='system' arguments='dismiss' content='' hint-actionId='MyDismissButton1'/>", button);
        }

        [TestMethod]
        public void Test_Toast_Xml_ContextMenuItem_Defaults()
        {
            ToastContextMenuItem item = new ToastContextMenuItem("content", "args");

            AssertContextMenuItemPayload("<action placement='contextMenu' content='content' arguments='args'/>", item);
        }

        [TestMethod]
        public void Test_Toast_Xml_ContextMenuItem_NullContent()
        {
            try
            {
                new ToastContextMenuItem(null, "args");
            }
            catch
            {
                return;
            }

            Assert.Fail("Exception should have been thrown.");
        }

        [TestMethod]
        public void Test_Toast_Xml_ContextMenuItem_NullArguments()
        {
            try
            {
                new ToastContextMenuItem("content", null);
            }
            catch
            {
                return;
            }

            Assert.Fail("Exception should have been thrown.");
        }

        [TestMethod]
        public void Test_Toast_Xml_ContextMenuItem_ActivationType_Foreground()
        {
            ToastContextMenuItem item = new ToastContextMenuItem("content", "args")
            {
                ActivationType = ToastActivationType.Foreground
            };

            AssertContextMenuItemPayload("<action placement='contextMenu' content='content' arguments='args'/>", item);
        }

        [TestMethod]
        public void Test_Toast_Xml_ContextMenuItem_ActivationType_Background()
        {
            ToastContextMenuItem item = new ToastContextMenuItem("content", "args")
            {
                ActivationType = ToastActivationType.Background
            };

            AssertContextMenuItemPayload("<action placement='contextMenu' content='content' arguments='args' activationType='background'/>", item);
        }

        [TestMethod]
        public void Test_Toast_Xml_ContextMenuItem_ActivationType_Protocol()
        {
            ToastContextMenuItem item = new ToastContextMenuItem("content", "args")
            {
                ActivationType = ToastActivationType.Protocol
            };

            AssertContextMenuItemPayload("<action placement='contextMenu' content='content' arguments='args' activationType='protocol'/>", item);
        }

        [TestMethod]
        public void Test_Toast_Xml_ContextMenuItem_HintActionId()
        {
            ToastContextMenuItem item = new ToastContextMenuItem("content", "args")
            {
                HintActionId = "MyContextMenu1"
            };

            AssertContextMenuItemPayload("<action placement='contextMenu' content='content' arguments='args' hint-actionId='MyContextMenu1'/>", item);
        }

        [TestMethod]
        public void Test_Toast_Xml_TextBox_Defaults()
        {
            var textBox = new ToastTextBox("myId");

            AssertInputPayload("<input id='myId' type='text' />", textBox);
        }

        [TestMethod]
        public void Test_Toast_Xml_TextBox_DefaultTextInput_Value()
        {
            var textBox = new ToastTextBox("myId")
            {
                DefaultInput = "Default text input"
            };

            AssertInputPayload("<input id='myId' type='text' defaultInput='Default text input' />", textBox);
        }

        [TestMethod]
        public void Test_Toast_Xml_TextBox_PlaceholderContent_Value()
        {
            var textBox = new ToastTextBox("myId")
            {
                PlaceholderContent = "My placeholder content"
            };

            AssertInputPayload("<input id='myId' type='text' placeHolderContent='My placeholder content' />", textBox);
        }

        [TestMethod]
        public void Test_Toast_Xml_TextBox_Title_Value()
        {
            var textBox = new ToastTextBox("myId")
            {
                Title = "My title"
            };

            AssertInputPayload("<input id='myId' type='text' title='My title' />", textBox);
        }

        [TestMethod]
        public void Test_Toast_Xml_TextBox_NullId()
        {
            try
            {
                new ToastTextBox(null);
            }
            catch
            {
                return;
            }

            Assert.Fail("Exception should have been thrown.");
        }

        [TestMethod]
        public void Test_Toast_Xml_TextBox_EmptyId()
        {
            var textBox = new ToastTextBox(string.Empty);

            AssertInputPayload("<input id='' type='text' />", textBox);
        }

        [TestMethod]
        public void Test_Toast_Xml_SelectionBox_Defaults()
        {
            var selectionBox = new ToastSelectionBox("myId");

            AssertInputPayload("<input id='myId' type='selection' />", selectionBox);
        }

        [TestMethod]
        public void Test_Toast_Xml_SelectionBox_EmptyId()
        {
            var selectionBox = new ToastSelectionBox(string.Empty);

            AssertInputPayload("<input id='' type='selection' />", selectionBox);
        }

        [TestMethod]
        public void Test_Toast_Xml_SelectionBox_NullId()
        {
            try
            {
                new ToastSelectionBox(null);
            }
            catch
            {
                return;
            }

            Assert.Fail("Exception should have been thrown.");
        }

        [TestMethod]
        public void Test_Toast_Xml_SelectionBox_DefaultSelectionBoxItemId_Value()
        {
            var selectionBox = new ToastSelectionBox("myId")
            {
                DefaultSelectionBoxItemId = "2"
            };

            AssertInputPayload("<input id='myId' type='selection' defaultInput='2' />", selectionBox);
        }

        [TestMethod]
        public void Test_Toast_Xml_SelectionBox_Title_Value()
        {
            var selectionBox = new ToastSelectionBox("myId")
            {
                Title = "My title"
            };

            AssertInputPayload("<input id='myId' type='selection' title='My title' />", selectionBox);
        }

        [TestMethod]
        public void Test_Toast_Xml_SelectionBoxItem()
        {
            var selectionBoxItem = new ToastSelectionBoxItem("myId", "My content");

            AssertSelectionPayload("<selection id='myId' content='My content' />", selectionBoxItem);
        }

        [TestMethod]
        public void Test_Toast_Xml_SelectionBoxItem_NullId()
        {
            try
            {
                new ToastSelectionBoxItem(null, "My content");
            }
            catch
            {
                return;
            }

            Assert.Fail("Exception should have been thrown.");
        }

        [TestMethod]
        public void Test_Toast_Xml_SelectionBoxItem_NullContent()
        {
            try
            {
                new ToastSelectionBoxItem("myId", null);
            }
            catch
            {
                return;
            }

            Assert.Fail("Exception should have been thrown.");
        }

        [TestMethod]
        public void Test_Toast_Xml_SelectionBoxItem_EmptyId()
        {
            var selectionBoxItem = new ToastSelectionBoxItem(string.Empty, "My content");

            AssertSelectionPayload("<selection id='' content='My content' />", selectionBoxItem);
        }

        [TestMethod]
        public void Test_Toast_Xml_SelectionBoxItem_EmptyContent()
        {
            var selectionBoxItem = new ToastSelectionBoxItem("myId", string.Empty);

            AssertSelectionPayload("<selection id='myId' content='' />", selectionBoxItem);
        }

        [TestMethod]
        public void Test_Toast_Header_AllValues()
        {
            AssertHeaderPayload("<header id='myId' title='My header' arguments='myArgs' activationType='protocol' />", new ToastHeader("myId", "My header", "myArgs")
            {
                ActivationType = ToastActivationType.Protocol
            });
        }

        [TestMethod]
        public void Test_Toast_Header_NullId()
        {
            try
            {
                new ToastHeader(null, "Title", "Args");
            }
            catch (ArgumentNullException)
            {
                try
                {
                    new ToastHeader("Id", "Title", "Args")
                    {
                        Id = null
                    };
                }
                catch (ArgumentNullException)
                {
                    return;
                }
            }

            Assert.Fail("ArgumentNullException for Id should have been thrown.");
        }

        [TestMethod]
        public void Test_Toast_Header_NullTitle()
        {
            try
            {
                new ToastHeader("id", null, "Args");
            }
            catch (ArgumentNullException)
            {
                try
                {
                    new ToastHeader("Id", "Title", "Args")
                    {
                        Title = null
                    };
                }
                catch (ArgumentNullException)
                {
                    return;
                }
            }

            Assert.Fail("ArgumentNullException for Title should have been thrown.");
        }

        [TestMethod]
        public void Test_Toast_Header_NullArguments()
        {
            try
            {
                new ToastHeader("id", "Title", null);
            }
            catch (ArgumentNullException)
            {
                try
                {
                    new ToastHeader("id", "Title", "args")
                    {
                        Arguments = null
                    };
                }
                catch (ArgumentNullException)
                {
                    return;
                }
            }

            Assert.Fail("ArgumentNullException for Arguments should have been thrown.");
        }

        [TestMethod]
        public void Test_Toast_Header_EmptyStrings()
        {
            AssertHeaderPayload("<header id='' title='' arguments='' />", new ToastHeader(string.Empty, string.Empty, string.Empty));
        }

        [TestMethod]
        public void Test_Toast_Header_ActivationTypes()
        {
            AssertHeaderActivationType("foreground", ToastActivationType.Foreground);

            try
            {
                AssertHeaderActivationType("background", ToastActivationType.Background);
                throw new Exception("ArgumentException should have been thrown, since activation type of background isn't allowed.");
            }
            catch (ArgumentException)
            {
            }

            AssertHeaderActivationType("protocol", ToastActivationType.Protocol);
        }

        [TestMethod]
        public void Test_Toast_DisplayTimestamp()
        {
            AssertPayload("<toast displayTimestamp='2016-10-19T09:00:00Z' />", new ToastContent()
            {
                DisplayTimestamp = new DateTime(2016, 10, 19, 9, 0, 0, DateTimeKind.Utc)
            });

            AssertPayload("<toast displayTimestamp='2016-10-19T09:00:00-08:00' />", new ToastContent()
            {
                DisplayTimestamp = new DateTimeOffset(2016, 10, 19, 9, 0, 0, TimeSpan.FromHours(-8))
            });

            // If devs use DateTime.Now, or directly use ticks (like this code), they can actually end up with a seconds decimal
            // value that is more than 3 decimal places. The platform notification parser will fail if there are
            // more than three decimal places. Hence this test normally would produce "2017-04-04T10:28:34.7047925Z"
            // but we've added code to ensure it strips to only at most 3 decimal places.
            AssertPayload("<toast displayTimestamp='2017-04-04T10:28:34.704Z' />", new ToastContent()
            {
                DisplayTimestamp = new DateTimeOffset(636268985147047925, TimeSpan.FromHours(0))
            });
        }

        [TestMethod]
        public void Test_Toast_Button_ActivationOptions()
        {
            AssertButtonPayload("<action content='My content' arguments='myArgs' activationType='protocol' afterActivationBehavior='pendingUpdate' protocolActivationTargetApplicationPfn='Microsoft.Settings' />", new ToastButton("My content", "myArgs")
            {
                ActivationType = ToastActivationType.Protocol,
                ActivationOptions = new ToastActivationOptions()
                {
                    AfterActivationBehavior = ToastAfterActivationBehavior.PendingUpdate,
                    ProtocolActivationTargetApplicationPfn = "Microsoft.Settings"
                }
            });

            // Empty class should do nothing
            AssertButtonPayload("<action content='My content' arguments='myArgs' activationType='background' />", new ToastButton("My content", "myArgs")
            {
                ActivationType = ToastActivationType.Background,
                ActivationOptions = new ToastActivationOptions()
            });

            // Default should be ignored
            AssertButtonPayload("<action content='My content' arguments='myArgs' activationType='background' />", new ToastButton("My content", "myArgs")
            {
                ActivationType = ToastActivationType.Background,
                ActivationOptions = new ToastActivationOptions()
                {
                    AfterActivationBehavior = ToastAfterActivationBehavior.Default
                }
            });

            // Specifying protocol PFN without using protocol activation should throw exception
            try
            {
                AssertButtonPayload("Exception should be thrown", new ToastButton("My content", "myArgs")
                {
                    ActivationOptions = new ToastActivationOptions()
                    {
                        ProtocolActivationTargetApplicationPfn = "Microsoft.Settings"
                    }
                });
                Assert.Fail("InvalidOperationException should have been thrown.");
            }
            catch (InvalidOperationException)
            {
            }
        }

        [TestMethod]
        public void Test_Toast_ContextMenuItem_ActivationOptions()
        {
            ToastContextMenuItem item = new ToastContextMenuItem("My content", "myArgs")
            {
                ActivationType = ToastActivationType.Protocol,
                ActivationOptions = new ToastActivationOptions()
                {
                    AfterActivationBehavior = ToastAfterActivationBehavior.PendingUpdate,
                    ProtocolActivationTargetApplicationPfn = "Microsoft.Settings"
                }
            };

            AssertContextMenuItemPayload("<action placement='contextMenu' content='My content' arguments='myArgs' activationType='protocol' afterActivationBehavior='pendingUpdate' protocolActivationTargetApplicationPfn='Microsoft.Settings' />", item);

            // Empty class should do nothing
            item.ActivationOptions = new ToastActivationOptions();

            AssertContextMenuItemPayload("<action placement='contextMenu' content='My content' arguments='myArgs' activationType='protocol' />", item);

            // Default should be ignored
            item.ActivationOptions.AfterActivationBehavior = ToastAfterActivationBehavior.Default;

            AssertContextMenuItemPayload("<action placement='contextMenu' content='My content' arguments='myArgs' activationType='protocol' />", item);

            // Specifying protocol PFN without using protocol activation should throw exception
            try
            {
                AssertContextMenuItemPayload("Exception should be thrown", new ToastContextMenuItem("My content", "myArgs")
                {
                    ActivationOptions = new ToastActivationOptions()
                    {
                        ProtocolActivationTargetApplicationPfn = "Microsoft.Settings"
                    }
                });
                Assert.Fail("InvalidOperationException should have been thrown.");
            }
            catch (InvalidOperationException)
            {
            }
        }

        [TestMethod]
        public void Test_Toast_ActivationOptions()
        {
            AssertPayload("<toast launch='settings:about' activationType='protocol' protocolActivationTargetApplicationPfn='Microsoft.Settings' />", new ToastContent()
            {
                Launch = "settings:about",
                ActivationType = ToastActivationType.Protocol,
                ActivationOptions = new ToastActivationOptions()
                {
                    ProtocolActivationTargetApplicationPfn = "Microsoft.Settings"
                }
            });

            // Empty class should do nothing
            AssertPayload("<toast launch='myArgs' activationType='background' />", new ToastContent()
            {
                Launch = "myArgs",
                ActivationType = ToastActivationType.Background,
                ActivationOptions = new ToastActivationOptions()
            });

            // Default should be ignored
            AssertPayload("<toast launch='myArgs' activationType='background' />", new ToastContent()
            {
                Launch = "myArgs",
                ActivationType = ToastActivationType.Background,
                ActivationOptions = new ToastActivationOptions()
                {
                    AfterActivationBehavior = ToastAfterActivationBehavior.Default
                }
            });

            // Setting anything other than default should throw exception
            try
            {
                AssertPayload("XML shouldn't matter", new ToastContent()
                {
                    Launch = "myArgs",
                    ActivationOptions = new ToastActivationOptions()
                    {
                        AfterActivationBehavior = ToastAfterActivationBehavior.PendingUpdate
                    }
                });
                Assert.Fail("InvalidOperationException should have been thrown.");
            }
            catch (InvalidOperationException)
            {
            }

            // Specifying protocol PFN without using protocol activation should throw exception
            try
            {
                AssertPayload("Exception should be thrown", new ToastContent()
                {
                    ActivationOptions = new ToastActivationOptions()
                    {
                        ProtocolActivationTargetApplicationPfn = "Microsoft.Settings"
                    }
                });
                Assert.Fail("InvalidOperationException should have been thrown.");
            }
            catch (InvalidOperationException)
            {
            }
        }

        [TestMethod]
        public void Test_Toast_Header_ActivationOptions()
        {
            var header = new ToastHeader("myId", "My title", "settings:about")
            {
                ActivationType = ToastActivationType.Protocol,
                ActivationOptions = new ToastActivationOptions()
                {
                    ProtocolActivationTargetApplicationPfn = "Microsoft.Settings"
                }
            };

            AssertHeaderPayload("<header id='myId' title='My title' arguments='settings:about' activationType='protocol' protocolActivationTargetApplicationPfn='Microsoft.Settings' />", header);

            // Empty class should do nothing
            header.ActivationOptions = new ToastActivationOptions();
            AssertHeaderPayload("<header id='myId' title='My title' arguments='settings:about' activationType='protocol' />", header);

            // Default should be ignored
            header.ActivationOptions.AfterActivationBehavior = ToastAfterActivationBehavior.Default;
            AssertHeaderPayload("<header id='myId' title='My title' arguments='settings:about' activationType='protocol' />", header);

            // Using anything other than default should throw exception
            try
            {
                header.ActivationOptions.AfterActivationBehavior = ToastAfterActivationBehavior.PendingUpdate;
                AssertHeaderPayload("Exception should be thrown", header);
                Assert.Fail("InvalidOperationException should have been thrown.");
            }
            catch (InvalidOperationException)
            {
            }

            // Specifying protocol PFN without using protocol activation should throw exception
            try
            {
                header.ActivationType = ToastActivationType.Foreground;
                header.ActivationOptions = new ToastActivationOptions()
                {
                    ProtocolActivationTargetApplicationPfn = "Microsoft.Settings"
                };
                AssertHeaderPayload("Exception should be thrown", header);
                Assert.Fail("InvalidOperationException should have been thrown.");
            }
            catch (InvalidOperationException)
            {
            }
        }

        [TestMethod]
        public void Test_Toast_ProgressBar_Value()
        {
            AssertProgressBar("<progress value='0' status='Downloading...'/>", new AdaptiveProgressBar() { Status = "Downloading..." });

            // Only non-WinRT supports implicit converters
#if !WINRT
            AssertProgressBar("<progress value='0.3' status='Downloading...'/>", new AdaptiveProgressBar()
            {
                Value = 0.3,
                Status = "Downloading..."
            });
#endif

            AssertProgressBar("<progress value='0.3' status='Downloading...'/>", new AdaptiveProgressBar()
            {
                Value = AdaptiveProgressBarValue.FromValue(0.3),
                Status = "Downloading..."
            });
            AssertProgressBar("<progress value='indeterminate' status='Downloading...'/>", new AdaptiveProgressBar()
            {
                Value = AdaptiveProgressBarValue.Indeterminate,
                Status = "Downloading..."
            });
            AssertProgressBar("<progress value='{progressValue}' status='Downloading...'/>", new AdaptiveProgressBar()
            {
#if WINRT
                Bindings =
                {
                    { AdaptiveProgressBarBindableProperty.Value, "progressValue" }
                },
#else
                Value = new BindableProgressBarValue("progressValue"),
#endif
                Status = "Downloading..."
            });

            try
            {
                new AdaptiveProgressBar()
                {
                    Value = AdaptiveProgressBarValue.FromValue(-4),
                    Status = "Downloading..."
                };
                Assert.Fail("Exception should have been thrown, only values 0-1 allowed");
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            try
            {
                new AdaptiveProgressBar()
                {
                    Value = AdaptiveProgressBarValue.FromValue(1.3),
                    Status = "Downloading..."
                };
                Assert.Fail("Exception should have been thrown, only values 0-1 allowed");
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }

        [TestMethod]
        public void Test_Toast_ProgressBar_NonEscapingString()
        {
            // There is NOT an escape string. Guidance to developers is if you're using data binding,
            // use data binding for ALL your user-generated strings.
            AssertProgressBar("<progress value='0' title='{I like tacos}' status='Downloading...'/>", new AdaptiveProgressBar()
            {
                Title = "{I like tacos}",
                Status = "Downloading..."
            });
        }

        [TestMethod]
        public void Test_Toast_ProgressBar()
        {
            try
            {
                AssertProgressBar("Exception should be thrown", new AdaptiveProgressBar());
                Assert.Fail("Exception should have been thrown, Status property is required");
            }
            catch (NullReferenceException)
            {
            }

            AssertProgressBar("<progress value='0.3' title='Katy Perry' valueStringOverride='3/10 songs' status='Adding music...'/>", new AdaptiveProgressBar()
            {
                Value = AdaptiveProgressBarValue.FromValue(0.3),
                Title = "Katy Perry",
                ValueStringOverride = "3/10 songs",
                Status = "Adding music..."
            });

            AssertProgressBar("<progress value='{progressValue}' title='{progressTitle}' valueStringOverride='{progressValueOverride}' status='{progressStatus}'/>", new AdaptiveProgressBar()
            {
#if WINRT
                Bindings =
                {
                    { AdaptiveProgressBarBindableProperty.Value, "progressValue" },
                    { AdaptiveProgressBarBindableProperty.Title, "progressTitle" },
                    { AdaptiveProgressBarBindableProperty.ValueStringOverride, "progressValueOverride" },
                    { AdaptiveProgressBarBindableProperty.Status, "progressStatus" }
                }
#else
                Value = new BindableProgressBarValue("progressValue"),
                Title = new BindableString("progressTitle"),
                ValueStringOverride = new BindableString("progressValueOverride"),
                Status = new BindableString("progressStatus")
#endif
            });

            AssertProgressBar("<progress value='0' status='Downloading...'/>", new AdaptiveProgressBar()
            {
                Value = null,
                Title = null,
                ValueStringOverride = null,
                Status = "Downloading..."
            });
        }

        private static void AssertProgressBar(string expectedProgressBarXml, AdaptiveProgressBar progressBar)
        {
            AssertBindingGenericPayload("<binding template='ToastGeneric'>" + expectedProgressBarXml + "</binding>", new ToastBindingGeneric()
            {
                Children =
                {
                    progressBar
                }
            });
        }

        [TestMethod]
        public void Test_Toast_FullPayload_ShoulderTap()
        {
            var content = new ToastContent()
            {
                HintPeople = new ToastPeople()
                {
                    EmailAddress = "johndoe@mydomain.com"
                },

                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = "Toast fallback"
                            },

                            new AdaptiveText()
                            {
                                Text = "Add your fallback toast content here"
                            }
                        }
                    },

                    BindingShoulderTap = new ToastBindingShoulderTap()
                    {
                        Image = new ToastShoulderTapImage()
                        {
                            Source = "img.png",
                            SpriteSheet = new ToastSpriteSheet()
                            {
                                Source = "sprite.png",
                                FrameHeight = 80,
                                Fps = 25,
                                StartingFrame = 15
                            }
                        }
                    }
                }
            };

            AssertPayload(
                @"<toast hint-people='mailto:johndoe@mydomain.com'>
                    <visual>
                        <binding template='ToastGeneric'>
                            <text>Toast fallback</text>
                            <text>Add your fallback toast content here</text>
                        </binding>
                        <binding template='ToastGeneric' experienceType='shoulderTap'>
                            <image src='img.png'
                                spritesheet-src='sprite.png'
                                spritesheet-height='80'
                                spritesheet-fps='25'
                                spritesheet-startingFrame='15'/>
                        </binding>
                    </visual>
                </toast>", content);
        }

        private static void AssertSelectionPayload(string expectedSelectionXml, ToastSelectionBoxItem selectionItem)
        {
            AssertInputPayload("<input id='myId' type='selection'>" + expectedSelectionXml + "</input>", new ToastSelectionBox("myId")
            {
                Items = { selectionItem }
            });
        }

        private static void AssertInputPayload(string expectedInputXml, IToastInput textBox)
        {
            AssertActionsPayload("<actions>" + expectedInputXml + "</actions>", new ToastActionsCustom()
            {
                Inputs = { textBox }
            });
        }

        private static void AssertButtonPayload(string expectedButtonXml, IToastButton button)
        {
            AssertActionsPayload("<actions>" + expectedButtonXml + "</actions>", new ToastActionsCustom()
            {
                Buttons = { button }
            });
        }

        private static void AssertContextMenuItemPayload(string expectedContextMenuItemXml, ToastContextMenuItem item)
        {
            AssertActionsPayload("<actions>" + expectedContextMenuItemXml + "</actions>", new ToastActionsCustom()
            {
                ContextMenuItems = { item }
            });
        }

        private static void AssertActionsPayload(string expectedActionsXml, IToastActions actions)
        {
            AssertPayload("<toast>" + expectedActionsXml + "</toast>", new ToastContent()
            {
                Actions = actions
            });
        }

        private static void AssertAudioPayload(string expectedAudioXml, ToastAudio audio)
        {
            AssertPayload("<toast>" + expectedAudioXml + "</toast>", new ToastContent()
            {
                Audio = audio
            });
        }

        private static void AssertVisualPayload(string expectedVisualXml, ToastVisual visual)
        {
            AssertPayload("<toast>" + expectedVisualXml + "</toast>", new ToastContent()
            {
                Visual = visual
            });
        }

        private static void AssertHeaderActivationType(string expectedPropertyValue, ToastActivationType activationType)
        {
            ToastHeader header = new ToastHeader("myId", "My title", "myArgs")
            {
                ActivationType = activationType
            };

            if (activationType == ToastActivationType.Foreground)
            {
                AssertHeaderPayload("<header id='myId' title='My title' arguments='myArgs' />", header);
            }
            else
            {
                AssertHeaderPayload($"<header id='myId' title='My title' arguments='myArgs' activationType='{expectedPropertyValue}' />", header);
            }
        }

        private static void AssertHeaderPayload(string expectedHeaderXml, ToastHeader header)
        {
            AssertPayload("<toast>" + expectedHeaderXml + "</toast>", new ToastContent()
            {
                Header = header
            });
        }

        private static void AssertPayload(string expectedXml, ToastContent toast)
        {
            AssertHelper.AssertToast(expectedXml, toast);
        }
    }
}