// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using CommunityToolkit.WinUI.Notifications;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Notifications
{
#if !WINRT
    [TestClass]
    public class TestToastContentBuilder
    {
        [TestMethod]
        public void AddCustomTimeStampTest_WithCustomTimeStamp_ReturnSelfWithCustomTimeStampAdded()
        {
            // Arrange
            DateTime testCustomTimeStamp = DateTime.UtcNow;

            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder.AddCustomTimeStamp(testCustomTimeStamp);

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.AreEqual(testCustomTimeStamp, builder.Content.DisplayTimestamp);
        }

        [TestMethod]
        public void AddHeaderTest_WithExpectedArgs_ReturnSelfWithHeaderAdded()
        {
            // Arrange
            string testToastHeaderId = "Test Header ID";
            string testToastTitle = "Test Toast Title";
            string testToastArguments = "Test Toast Arguments";

            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder.AddHeader(testToastHeaderId, testToastTitle, testToastArguments);

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.AreEqual(testToastHeaderId, builder.Content.Header.Id);
            Assert.AreEqual(testToastTitle, builder.Content.Header.Title);
            Assert.AreEqual(testToastArguments, builder.Content.Header.Arguments);
        }

        [TestMethod]
        public void AddHeaderTest_WithExpectedArgsAndToastArguments_ReturnSelfWithHeaderAdded()
        {
            // Arrange
            string testToastHeaderId = "Test Header ID";
            string testToastTitle = "Test Toast Title";
            ToastArguments testToastArguments = new ToastArguments()
                .Add("arg1", 5)
                .Add("arg2", "tacos");

            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder.AddHeader(testToastHeaderId, testToastTitle, testToastArguments);

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.AreEqual(testToastHeaderId, builder.Content.Header.Id);
            Assert.AreEqual(testToastTitle, builder.Content.Header.Title);
            Assert.AreEqual(testToastArguments.ToString(), builder.Content.Header.Arguments);
        }

        [TestMethod]
        public void AddToastActivationInfoTest_WithExpectedArgs_ReturnSelfWithActivationInfoAdded()
        {
            // Arrange
            string testToastLaunchArugments = "Test Toast Launch Args";
            ToastActivationType testToastActivationType = ToastActivationType.Background;

            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder.AddToastActivationInfo(testToastLaunchArugments, testToastActivationType);

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.AreEqual(testToastLaunchArugments, builder.Content.Launch);
            Assert.AreEqual(testToastActivationType, builder.Content.ActivationType);
        }

        [TestMethod]
        public void AddArgumentTest_Basic_ReturnSelfWithArgumentsAdded()
        {
            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder
                .AddArgument("userId", 542)
                .AddArgument("name", "Andrew");

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.AreEqual("userId=542;name=Andrew", builder.Content.Launch);
        }

        [TestMethod]
        public void AddArgumentTest_NoValue_ReturnSelfWithArgumentsAdded()
        {
            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder
                .AddArgument("isPurelyInformational");

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.AreEqual("isPurelyInformational", builder.Content.Launch);
        }

        [TestMethod]
        public void AddArgumentTest_Escaping_ReturnSelfWithArgumentsAdded()
        {
            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder
                .AddArgument("user;Id", "andrew=leader%26bares");

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.AreEqual("user%3BId=andrew%3Dleader%2526bares", builder.Content.Launch);
        }

        [TestMethod]
        public void AddArgumentTest_Replacing_ReturnSelfWithArgumentsAdded()
        {
            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder
                .AddArgument("userId", 542)
                .AddArgument("name", "Andrew")
                .AddArgument("userId", 601);

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.AreEqual("userId=601;name=Andrew", builder.Content.Launch);
        }

        [TestMethod]
        public void AddArgumentTest_Generic_ReturnSelfWithArgumentsAdded()
        {
            // Arrange
            const string userIdKey = "userId";
            const int userIdValue = 542;

            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder
                .AddButton(new ToastButton()
                    .SetContent("Accept")
                    .AddArgument("action", "accept")
                    .SetBackgroundActivation())
                .AddButton(new ToastButtonSnooze())
                .AddButton("View", ToastActivationType.Protocol, "https://msn.com")

                // Add generic arguments halfway through (should be applied to existing buttons and to any subsequent buttons added later)
                .AddArgument(userIdKey, userIdValue)

                .AddButton(new ToastButton()
                    .SetContent("Decline")
                    .AddArgument("action", "decline")
                    .SetBackgroundActivation())
                .AddButton(new ToastButton()
                    .SetContent("Report")
                    .SetProtocolActivation(new Uri("https://microsoft.com")));

            // Assert
            Assert.AreSame(builder, anotherReference);

            // Top level arguments should be present
            Assert.AreEqual("userId=542", builder.Content.Launch);

            // All foreground/background activation buttons should have received generic arguments. Protocol and system activation buttons shouldn't have had any arguments changed.
            var actions = builder.Content.Actions as ToastActionsCustom;

            var button1 = actions.Buttons[0] as ToastButton;
            Assert.AreEqual("Accept", button1.Content);
            Assert.AreEqual("action=accept;userId=542", button1.Arguments);

            var button2 = actions.Buttons[1];
            Assert.IsInstanceOfType(button2, typeof(ToastButtonSnooze));

            var button3 = actions.Buttons[2] as ToastButton;
            Assert.AreEqual("View", button3.Content);
            Assert.AreEqual("https://msn.com", button3.Arguments);

            var button4 = actions.Buttons[3] as ToastButton;
            Assert.AreEqual("Decline", button4.Content);
            Assert.AreEqual("action=decline;userId=542", button4.Arguments);

            var button5 = actions.Buttons[4] as ToastButton;
            Assert.AreEqual("Report", button5.Content);
            Assert.AreEqual("https://microsoft.com/", button5.Arguments);
        }

        [TestMethod]
        public void AddArgumentTest_ReplacingWithinButton_ReturnSelfWithArgumentsAdded()
        {
            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder
                .AddButton(new ToastButton()
                    .SetContent("Accept")
                    .AddArgument("action", "accept")
                    .AddArgument("userId", 601)
                    .SetBackgroundActivation())

                // Add generic arguments halfway through (in this case shouldn't overwrite anything)
                .AddArgument("userId", 542)

                .AddButton(new ToastButton()
                    .SetContent("Decline")
                    .AddArgument("action", "decline")
                    .AddArgument("userId", 601)
                    .SetBackgroundActivation());

            // Assert
            Assert.AreSame(builder, anotherReference);

            // Top level arguments should be present
            Assert.AreEqual("userId=542", builder.Content.Launch);

            // Buttons should have overridden the generic userId
            var actions = builder.Content.Actions as ToastActionsCustom;

            var button1 = actions.Buttons[0] as ToastButton;
            Assert.AreEqual("Accept", button1.Content);
            Assert.AreEqual("action=accept;userId=601", button1.Arguments);

            var button2 = actions.Buttons[1] as ToastButton;
            Assert.AreEqual("Decline", button2.Content);
            Assert.AreEqual("action=decline;userId=601", button2.Arguments);
        }

        [TestMethod]
        public void AddArgumentTest_AvoidModifyingCustomButtons_ReturnSelfWithArgumentsAdded()
        {
            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder
                .AddToastActivationInfo("myCustomLaunchStr", ToastActivationType.Foreground)

                .AddButton("Accept", ToastActivationType.Background, "myAcceptStr")

                // userId shouldn't be added to any of these except view
                .AddArgument("userId", 542)

                .AddButton("Decline", ToastActivationType.Background, "myDeclineStr")

                .AddButton(new ToastButton()
                    .SetContent("View")
                    .AddArgument("action", "view"));

            // Assert
            Assert.AreSame(builder, anotherReference);

            // Top level arguments should be the custom string since user set that
            Assert.AreEqual("myCustomLaunchStr", builder.Content.Launch);

            // Buttons should have their custom strings except the last
            var actions = builder.Content.Actions as ToastActionsCustom;

            var button1 = actions.Buttons[0] as ToastButton;
            Assert.AreEqual("Accept", button1.Content);
            Assert.AreEqual("myAcceptStr", button1.Arguments);

            var button2 = actions.Buttons[1] as ToastButton;
            Assert.AreEqual("Decline", button2.Content);
            Assert.AreEqual("myDeclineStr", button2.Arguments);

            var button3 = actions.Buttons[2] as ToastButton;
            Assert.AreEqual("View", button3.Content);
            Assert.AreEqual("action=view;userId=542", button3.Arguments);
        }

        [TestMethod]
        public void AddArgumentTest_BackgroundActivation_ReturnSelfWithArgumentsAdded()
        {
            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder
                .AddArgument("userId", 542)
                .SetBackgroundActivation();

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.AreEqual("userId=542", builder.Content.Launch);
            Assert.AreEqual(ToastActivationType.Background, builder.Content.ActivationType);
        }

        [TestMethod]
        public void SetProtocolActivationTest_ReturnSelfWithArgumentsAdded()
        {
            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder
                .AddButton(new ToastButton()
                    .SetContent("Accept")
                    .AddArgument("action", "accept")
                    .SetBackgroundActivation())

                .AddArgument("userId", 542)

                .SetProtocolActivation(new Uri("https://msn.com/"))

                .AddArgument("name", "Andrew")

                .AddButton(new ToastButton()
                    .SetContent("Decline")
                    .AddArgument("action", "decline")
                    .SetBackgroundActivation());

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.AreEqual("https://msn.com/", builder.Content.Launch);
            Assert.AreEqual(ToastActivationType.Protocol, builder.Content.ActivationType);

            var actions = builder.Content.Actions as ToastActionsCustom;

            var button1 = actions.Buttons[0] as ToastButton;
            Assert.AreEqual("Accept", button1.Content);
            Assert.AreEqual("action=accept;userId=542;name=Andrew", button1.Arguments);

            var button2 = actions.Buttons[1] as ToastButton;
            Assert.AreEqual("Decline", button2.Content);
            Assert.AreEqual("action=decline;userId=542;name=Andrew", button2.Arguments);
        }

        [TestMethod]
        public void ToastButtonBuilders_General_ReturnSelf()
        {
            ToastButton button = new ToastButton();
            ToastButton anotherReference = button
                .SetContent("View")
                .AddArgument("action", "view")
                .AddArgument("imageId", 601);

            Assert.AreSame(button, anotherReference);
            Assert.AreEqual("View", button.Content);
            Assert.AreEqual("action=view;imageId=601", button.Arguments);
            Assert.AreEqual(ToastActivationType.Foreground, button.ActivationType);
        }

        [TestMethod]
        public void ToastButtonBuilders_AllProperties_ReturnSelf()
        {
            ToastButton button = new ToastButton();
            ToastButton anotherReference = button
                .SetContent("View")
                .SetImageUri(new Uri("ms-appx:///Assets/view.png"))
                .AddArgument("action", "view")
                .SetBackgroundActivation()
                .SetAfterActivationBehavior(ToastAfterActivationBehavior.PendingUpdate)
                .SetHintActionId("viewImage");

            Assert.AreSame(button, anotherReference);
            Assert.AreEqual("View", button.Content);
            Assert.AreEqual("action=view", button.Arguments);
            Assert.AreEqual("ms-appx:///Assets/view.png", button.ImageUri);
            Assert.AreEqual(ToastActivationType.Background, button.ActivationType);
            Assert.AreEqual(ToastAfterActivationBehavior.PendingUpdate, button.ActivationOptions.AfterActivationBehavior);
            Assert.AreEqual("viewImage", button.HintActionId);
        }

        [TestMethod]
        public void ToastButtonBuilders_ProtocolActivation_ReturnSelf()
        {
            ToastButton button = new ToastButton();
            ToastButton anotherReference = button
                .SetContent("View")
                .SetProtocolActivation(new Uri("https://msn.com"));

            Assert.AreSame(button, anotherReference);
            Assert.AreEqual("View", button.Content);
            Assert.AreEqual("https://msn.com/", button.Arguments);
            Assert.AreEqual(ToastActivationType.Protocol, button.ActivationType);
        }

        [TestMethod]
        public void ToastButtonBuilders_ProtocolActivationWithPfn_ReturnSelf()
        {
            ToastButton button = new ToastButton();
            ToastButton anotherReference = button
                .SetContent("View")
                .SetProtocolActivation(new Uri("https://msn.com"), "MyPfn");

            Assert.AreSame(button, anotherReference);
            Assert.AreEqual("View", button.Content);
            Assert.AreEqual("https://msn.com/", button.Arguments);
            Assert.AreEqual(ToastActivationType.Protocol, button.ActivationType);
            Assert.AreEqual("MyPfn", button.ActivationOptions.ProtocolActivationTargetApplicationPfn);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ToastButtonBuilders_InvalidProtocolAfterArguments_ReturnSelf()
        {
            new ToastButton()
                .SetContent("View")
                .AddArgument("action", "view")
                .SetProtocolActivation(new Uri("https://msn.com"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ToastButtonBuilders_InvalidDismissAfterArguments_ReturnSelf()
        {
            new ToastButton()
                .SetContent("View")
                .AddArgument("action", "view")
                .SetDismissActivation();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ToastButtonBuilders_InvalidSnoozeAfterArguments_ReturnSelf()
        {
            new ToastButton()
                .SetContent("View")
                .AddArgument("action", "view")
                .SetSnoozeActivation();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ToastButtonBuilders_InvalidSnoozeWithIdAfterArguments_ReturnSelf()
        {
            new ToastButton()
                .SetContent("View")
                .AddArgument("action", "view")
                .SetSnoozeActivation("snoozeId");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ToastButtonBuilders_InvalidArgumentsAfterProtocol_ReturnSelf()
        {
            new ToastButton()
                .SetContent("View")
                .SetProtocolActivation(new Uri("https://msn.com"))
                .AddArgument("action", "view");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ToastButtonBuilders_InvalidArgumentsAfterCustomArguments_ReturnSelf()
        {
            var button = new ToastButton("View", "viewArgs");

            button.AddArgument("action", "view");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ToastButtonBuilders_InvalidArgumentsAfterSnooze_ReturnSelf()
        {
            new ToastButton()
                .SetContent("Later")
                .SetSnoozeActivation()
                .AddArgument("action", "later");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ToastButtonBuilders_InvalidArgumentsAfterSnoozeWithId_ReturnSelf()
        {
            new ToastButton()
                .SetContent("Later")
                .SetSnoozeActivation("myId")
                .AddArgument("action", "later");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ToastButtonBuilders_InvalidArgumentsAfterDismissActivation_ReturnSelf()
        {
            new ToastButton()
                .SetContent("Later")
                .SetDismissActivation()
                .AddArgument("action", "later");
        }

        [TestMethod]
        public void SetToastDurationTest_WithCustomToastDuration_ReturnSelfWithCustomToastDurationSet()
        {
            // Arrange
            ToastDuration testToastDuration = ToastDuration.Long;

            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder.SetToastDuration(testToastDuration);

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.AreEqual(testToastDuration, builder.Content.Duration);
        }

        [TestMethod]
        public void SetToastScenarioTest_WithCustomToastScenario_ReturnSelfWithCustomToastScenarioSet()
        {
            // Arrange
            ToastScenario testToastScenario = ToastScenario.Default;

            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder.SetToastScenario(testToastScenario);

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.AreEqual(testToastScenario, builder.Content.Scenario);
        }

        [TestMethod]
        public void AddAudioTest_WithAudioUriOnly_ReturnSelfWithCustomAudioAdded()
        {
            // Arrange
            Uri testAudioUriSrc = new Uri("C:/justatesturi.mp3");

            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder.AddAudio(testAudioUriSrc);

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.AreEqual(testAudioUriSrc.OriginalString, builder.Content.Audio.Src.OriginalString);
        }

        [TestMethod]
        public void AddAudioTest_WithFullArgs_ReturnSelfWithCustomAudioAddedWithAllOptionsSet()
        {
            // Arrange
            Uri testAudioUriSrc = new Uri("C:/justatesturi.mp3");
            bool testToastAudioLoop = true;
            bool testToastAudioSilent = true;

            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder.AddAudio(testAudioUriSrc, testToastAudioLoop, testToastAudioSilent);

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.AreEqual(testAudioUriSrc.OriginalString, builder.Content.Audio.Src.OriginalString);
            Assert.AreEqual(testToastAudioLoop, builder.Content.Audio.Loop);
            Assert.AreEqual(testToastAudioSilent, builder.Content.Audio.Silent);
        }

        [TestMethod]
        public void AddAudioTest_WithMsWinSoundEvent_ReturnSelfWithCustomAudioAdded()
        {
            // Arrange
            Uri testAudioUriSrc = new Uri("ms-winsoundevent:Notification.Reminder");

            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder.AddAudio(testAudioUriSrc);

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.AreEqual(testAudioUriSrc.OriginalString, builder.Content.Audio.Src.OriginalString);
        }

        [TestMethod]
        public void AddAudioTest_WithMsAppx_ReturnSelfWithCustomAudioAdded()
        {
            // Arrange
            Uri testAudioUriSrc = new Uri("ms-appx:///Assets/Audio.mp3");

            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder.AddAudio(testAudioUriSrc);

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.AreEqual(testAudioUriSrc.OriginalString, builder.Content.Audio.Src.OriginalString);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddAudioTest_WithInvalidMsUri_ThrowException()
        {
            // Arrange
            Uri testAudioUriSrc = new Uri("ms-doesntexist:Notification.Reminder");

            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            builder.AddAudio(testAudioUriSrc);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddAudioTest_WithInvalidAppDataUri_ThrowException()
        {
            // Arrange (ms-appdata isn't currently supported)
            Uri testAudioUriSrc = new Uri("ms-appdata:///local/Sound.mp3");

            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            builder.AddAudio(testAudioUriSrc);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddAudioTest_WithInvalidHttpUri_ThrowException()
        {
            // Arrange
            Uri testAudioUriSrc = new Uri("https://myaudio.com/song.mp3");

            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            builder.AddAudio(testAudioUriSrc);
        }

        [TestMethod]
        public void AddAudioTest_WithAudioObject_ReturnSelfWithCustomAudioAdded()
        {
            // Arrange
            var audio = new ToastAudio()
            {
                Silent = true
            };

            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder.AddAudio(audio);

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.AreSame(audio, builder.Content.Audio);
        }

        [TestMethod]
        public void AddAttributionTextTest_WithSimpleText_ReturnSelfWithCustomAttributionTextAdded()
        {
            // Arrange
            string testAttributionText = "Test Attribution Text";

            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder.AddAttributionText(testAttributionText);

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.AreEqual(testAttributionText, builder.Content.Visual.BindingGeneric.Attribution.Text);
        }

        [TestMethod]
        public void AddAttributionTextTest_WithTextAndLanguage_ReturnSelfWithCustomAttributionTextAndLanguageAdded()
        {
            // Arrange
            string testAttributionText = "Test Attribution Text";
            string testAttributionTextLanguage = "en-US";

            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder.AddAttributionText(testAttributionText, testAttributionTextLanguage);

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.AreEqual(testAttributionText, builder.Content.Visual.BindingGeneric.Attribution.Text);
            Assert.AreEqual(testAttributionTextLanguage, builder.Content.Visual.BindingGeneric.Attribution.Language);
        }

        [TestMethod]
        public void AddAppLogoOverrideTest_WithLogoUriOnly_ReturnSelfWithCustomLogoAdded()
        {
            // Arrange
            Uri testAppLogoUriSrc = new Uri("C:/justatesturi.jpg");

            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder.AddAppLogoOverride(testAppLogoUriSrc);

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.AreEqual(testAppLogoUriSrc.OriginalString, builder.Content.Visual.BindingGeneric.AppLogoOverride.Source);
        }

        [TestMethod]
        public void AddAppLogoOverrideTest_WithCustomLogoAndFullOptions_ReturnSelfWithCustomLogoAndOptionsAdded()
        {
            // Arrange
            Uri testAppLogoUriSrc = new Uri("C:/justatesturi.jpg");
            ToastGenericAppLogoCrop testCropOption = ToastGenericAppLogoCrop.Circle;
            string testLogoAltText = "Test Logo Alt Text";
            bool testLogoAddImageQuery = true;

            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder.AddAppLogoOverride(testAppLogoUriSrc, testCropOption, testLogoAltText, testLogoAddImageQuery);

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.AreEqual(testAppLogoUriSrc.OriginalString, builder.Content.Visual.BindingGeneric.AppLogoOverride.Source);
            Assert.AreEqual(testCropOption, builder.Content.Visual.BindingGeneric.AppLogoOverride.HintCrop);
            Assert.AreEqual(testLogoAltText, builder.Content.Visual.BindingGeneric.AppLogoOverride.AlternateText);
            Assert.AreEqual(testLogoAddImageQuery, builder.Content.Visual.BindingGeneric.AppLogoOverride.AddImageQuery);
        }

        [TestMethod]
        public void AddHeroImageTest_WithHeroImageUriOnly_ReturnSelfWithHeroImageAdded()
        {
            // Arrange
            Uri testHeroImageUriSrc = new Uri("C:/justatesturi.jpg");

            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder.AddHeroImage(testHeroImageUriSrc);

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.AreEqual(testHeroImageUriSrc.OriginalString, builder.Content.Visual.BindingGeneric.HeroImage.Source);
        }

        [TestMethod]
        public void AddHeroImageTest_WithHeroImageUriAndFullOptions_ReturnSelfWithHeroImageAndOptionsAdded()
        {
            // Arrange
            Uri testHeroImageUriSrc = new Uri("C:/justatesturi.jpg");
            string testHeroImageAltText = "Test Hero Image Text";
            bool testHeroImageAddImageQuery = true;

            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder.AddHeroImage(testHeroImageUriSrc, testHeroImageAltText, testHeroImageAddImageQuery);

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.AreEqual(testHeroImageUriSrc.OriginalString, builder.Content.Visual.BindingGeneric.HeroImage.Source);
            Assert.AreEqual(testHeroImageAltText, builder.Content.Visual.BindingGeneric.HeroImage.AlternateText);
            Assert.AreEqual(testHeroImageAddImageQuery, builder.Content.Visual.BindingGeneric.HeroImage.AddImageQuery);
        }

        [TestMethod]
        public void AddInlineImageTest_WithInlineImageUriOnly_ReturnSelfWithInlineImageAdded()
        {
            // Arrange
            Uri testInlineImageUriSrc = new Uri("C:/justatesturi.jpg");

            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder.AddInlineImage(testInlineImageUriSrc);

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.AreEqual(testInlineImageUriSrc.OriginalString, (builder.Content.Visual.BindingGeneric.Children.First() as AdaptiveImage).Source);
        }

        [TestMethod]
        public void AddInlineImageTest_WithInlineImageAndFullOptions_ReturnSelfWithInlineImageAndOptionsAdded()
        {
            // Arrange
            Uri testInlineImageUriSrc = new Uri("C:/justatesturi.jpg");
            string testInlineImageAltText = "Test Inline Image Text";
            bool testInlineImageAddImageQuery = true;

            // Act
            ToastContentBuilder builder = new ToastContentBuilder();
            ToastContentBuilder anotherReference = builder.AddInlineImage(testInlineImageUriSrc, testInlineImageAltText, testInlineImageAddImageQuery);

            // Assert
            Assert.AreSame(builder, anotherReference);

            var image = builder.Content.Visual.BindingGeneric.Children.First() as AdaptiveImage;

            Assert.AreEqual(testInlineImageUriSrc.OriginalString, image.Source);
            Assert.AreEqual(testInlineImageAltText, image.AlternateText);
            Assert.AreEqual(testInlineImageAddImageQuery, image.AddImageQuery);
        }

        [TestMethod]
        public void AddProgressBarTest_WithoutInputArgs_ReturnSelfWithNonIndeterminateBindableProgressBarAdded()
        {
            // Arrange
            ToastContentBuilder builder = new ToastContentBuilder();

            // Act
            ToastContentBuilder anotherReference = builder.AddProgressBar();

            // Assert
            Assert.AreSame(builder, anotherReference);
            var progressBar = builder.Content.Visual.BindingGeneric.Children.First() as AdaptiveProgressBar;

            Assert.IsNotNull(progressBar.Title.BindingName);
            Assert.IsNotNull(progressBar.Value.BindingName);
            Assert.IsNotNull(progressBar.ValueStringOverride.BindingName);
            Assert.IsNotNull(progressBar.Status.BindingName);
        }

        [TestMethod]
        public void AddProgressBarTest_WithFixedPropertiesAndDeterminateValue_ReturnSelfWithFixedValueAndPropertiesProgressBarAdded()
        {
            // Arrange
            string testProgressBarTitle = "Test Progress Bar Title";
            double testProgressBarValue = 0.25;
            string testValueStringOverride = "Test Value String Override";
            string testProgressBarStatus = "Test Progress Bar Status";

            ToastContentBuilder builder = new ToastContentBuilder();

            // Act
            ToastContentBuilder anotherReference = builder.AddProgressBar(testProgressBarTitle, testProgressBarValue, false, testValueStringOverride, testProgressBarStatus);

            // Assert
            Assert.AreSame(builder, anotherReference);
            var progressBar = builder.Content.Visual.BindingGeneric.Children.First() as AdaptiveProgressBar;

            Assert.IsNull(progressBar.Title.BindingName);
            Assert.AreEqual(testProgressBarTitle, (string)progressBar.Title);

            Assert.IsNull(progressBar.Value.BindingName);
            Assert.AreEqual(testProgressBarValue, ((AdaptiveProgressBarValue)progressBar.Value).Value);

            Assert.IsNull(progressBar.ValueStringOverride.BindingName);
            Assert.AreEqual(testValueStringOverride, (string)progressBar.ValueStringOverride);

            Assert.IsNull(progressBar.Status.BindingName);
            Assert.AreEqual(testProgressBarStatus, (string)progressBar.Status);
        }

        [TestMethod]
        public void AddProgressBarTest_WithIndeterminateValue_ReturnSelfWithIndeterminateProgressBarAdded()
        {
            // Arrange
            ToastContentBuilder builder = new ToastContentBuilder();

            // Act
            ToastContentBuilder anotherReference = builder.AddProgressBar(isIndeterminate: true);

            // Assert
            Assert.AreSame(builder, anotherReference);
            var progressBar = builder.Content.Visual.BindingGeneric.Children.First() as AdaptiveProgressBar;

            Assert.IsTrue(((AdaptiveProgressBarValue)progressBar.Value).IsIndeterminate);
        }

        [TestMethod]
        public void AddTextTest_WithSimpleText_ReturnSelfWithTextAdded()
        {
            // Arrange
            string testText = "Test Text";
            ToastContentBuilder builder = new ToastContentBuilder();

            // Act
            ToastContentBuilder anotherReference = builder.AddText(testText);

            // Assert
            Assert.AreSame(builder, anotherReference);
            var text = builder.Content.Visual.BindingGeneric.Children.First() as AdaptiveText;

            Assert.AreEqual(testText, (string)text.Text);
        }

        [TestMethod]
        public void AddTextTest_WithMultipleTexts_ReturnSelfWithAllTextsAdded()
        {
            // Arrange
            string testText1 = "Test Header";
            string testText2 = "Test Text";
            string testText3 = "Test Text Again";
            ToastContentBuilder builder = new ToastContentBuilder();

            // Act
            ToastContentBuilder anotherReference = builder.AddText(testText1)
                .AddText(testText2)
                .AddText(testText3);

            // Assert
            Assert.AreSame(builder, anotherReference);
            var texts = builder.Content.Visual.BindingGeneric.Children.Take(3).Cast<AdaptiveText>().ToList();

            Assert.AreEqual(testText1, (string)texts[0].Text);
            Assert.AreEqual(testText2, (string)texts[1].Text);
            Assert.AreEqual(testText3, (string)texts[2].Text);
        }

        [TestMethod]
        public void AddTextTest_WithTextAndFullOptions_ReturnSelfWithTextAndAllOptionsAdded()
        {
            // Arrange
            string testText = "Test Text";
            AdaptiveTextStyle testStyle = AdaptiveTextStyle.Header;
            bool testWrapHint = true;
            int testHintMaxLine = 2;
            int testHintMinLine = 1;
            AdaptiveTextAlign testAlign = AdaptiveTextAlign.Auto;
            string testLanguage = "en-US";

            ToastContentBuilder builder = new ToastContentBuilder();

            // Act
            ToastContentBuilder anotherReference = builder.AddText(testText, testStyle, testWrapHint, testHintMaxLine, testHintMinLine, testAlign, testLanguage);

            // Assert
            Assert.AreSame(builder, anotherReference);
            var text = builder.Content.Visual.BindingGeneric.Children.First() as AdaptiveText;

            Assert.AreEqual(testText, (string)text.Text);
            Assert.AreEqual(testHintMaxLine, text.HintMaxLines);
            Assert.AreEqual(testLanguage, text.Language);

            // These values should still be the default values, since they aren't used for top-level text
            Assert.AreEqual(AdaptiveTextStyle.Default, text.HintStyle);
            Assert.IsNull(text.HintWrap);
            Assert.IsNull(text.HintMinLines);
            Assert.AreEqual(AdaptiveTextAlign.Default, text.HintAlign);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddTextTest_WithMoreThan4LinesOfText_ThrowInvalidOperationException()
        {
            // Arrange
            string testText1 = "Test Header";
            string testText2 = "Test Text";
            string testText3 = "Test Text Again";
            string testText4 = "Test Text Again x2";
            ToastContentBuilder builder = new ToastContentBuilder();

            // Act
            _ = builder.AddText(testText1)
                .AddText(testText2)
                .AddText(testText3)
                .AddText(testText4);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddTextTest_WithMaxLinesValueLargerThan2_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            string testText1 = "Test Header";
            ToastContentBuilder builder = new ToastContentBuilder();

            // Act
            _ = builder.AddText(testText1, hintMaxLines: 3);
        }

        [TestMethod]
        public void AddVisualChildTest_WithCustomVisual_ReturnSelfWithCustomVisualAdded()
        {
            // Arrange
            // Taken from : https://docs.microsoft.com/en-us/windows/uwp/design/shell/tiles-and-notifications/adaptive-interactive-toasts#adaptive-content
            AdaptiveGroup group = new AdaptiveGroup()
            {
                Children =
                {
                    new AdaptiveSubgroup()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = "52 attendees",
                                HintStyle = AdaptiveTextStyle.Base
                            },
                            new AdaptiveText()
                            {
                                Text = "23 minute drive",
                                HintStyle = AdaptiveTextStyle.CaptionSubtle
                            }
                        }
                    },
                    new AdaptiveSubgroup()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = "1 Microsoft Way",
                                HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                HintAlign = AdaptiveTextAlign.Right
                            },
                            new AdaptiveText()
                            {
                                Text = "Bellevue, WA 98008",
                                HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                HintAlign = AdaptiveTextAlign.Right
                            }
                        }
                    }
                }
            };
            ToastContentBuilder builder = new ToastContentBuilder();

            // Act
            ToastContentBuilder anotherReference = builder.AddVisualChild(group);

            // Assert
            Assert.AreSame(builder, anotherReference);
            Assert.IsInstanceOfType(builder.Content.Visual.BindingGeneric.Children.First(), typeof(AdaptiveGroup));
        }

        [TestMethod]
        public void AddButtonTest_WithTextOnlyButton_ReturnSelfWithButtonAdded()
        {
            // Arrange
            string testButtonContent = "Test Button Content";
            ToastActivationType testToastActivationType = ToastActivationType.Background;
            string testButtonLaunchArgs = "Test Launch Args";
            ToastContentBuilder builder = new ToastContentBuilder();

            // Act
            ToastContentBuilder anotherReference = builder.AddButton(testButtonContent, testToastActivationType, testButtonLaunchArgs);

            // Assert
            Assert.AreSame(builder, anotherReference);

            var button = (builder.Content.Actions as ToastActionsCustom).Buttons.First() as ToastButton;
            Assert.AreEqual(testButtonContent, button.Content);
            Assert.AreEqual(testToastActivationType, button.ActivationType);
            Assert.AreEqual(testButtonLaunchArgs, button.Arguments);
        }

        [TestMethod]
        public void AddButtonTest_WithCustomImageAndTextButton_ReturnSelfWithButtonAdded()
        {
            // Arrange
            string testButtonContent = "Test Button Content";
            ToastActivationType testToastActivationType = ToastActivationType.Background;
            string testButtonLaunchArgs = "Test Launch Args";
            Uri testImageUriSrc = new Uri("C:/justatesturi.jpg");

            ToastContentBuilder builder = new ToastContentBuilder();

            // Act
            ToastContentBuilder anotherReference = builder.AddButton(testButtonContent, testToastActivationType, testButtonLaunchArgs, testImageUriSrc);

            // Assert
            Assert.AreSame(builder, anotherReference);

            var button = (builder.Content.Actions as ToastActionsCustom).Buttons.First() as ToastButton;
            Assert.AreEqual(testButtonContent, button.Content);
            Assert.AreEqual(testToastActivationType, button.ActivationType);
            Assert.AreEqual(testButtonLaunchArgs, button.Arguments);
            Assert.AreEqual(testImageUriSrc.OriginalString, button.ImageUri);
        }

        [TestMethod]
        public void AddButtonTest_WithTextBoxId_ReturnSelfWithButtonAdded()
        {
            // Arrange
            string testInputTextBoxId = Guid.NewGuid().ToString();
            string testButtonContent = "Test Button Content";
            ToastActivationType testToastActivationType = ToastActivationType.Background;
            string testButtonLaunchArgs = "Test Launch Args";
            Uri testImageUriSrc = new Uri("C:/justatesturi.jpg");

            ToastContentBuilder builder = new ToastContentBuilder();

            // Act
            ToastContentBuilder anotherReference = builder.AddButton(testInputTextBoxId, testButtonContent, testToastActivationType, testButtonLaunchArgs, testImageUriSrc);

            // Assert
            Assert.AreSame(builder, anotherReference);

            var button = (builder.Content.Actions as ToastActionsCustom).Buttons.First() as ToastButton;
            Assert.AreEqual(testInputTextBoxId, button.TextBoxId);
            Assert.AreEqual(testButtonContent, button.Content);
            Assert.AreEqual(testToastActivationType, button.ActivationType);
            Assert.AreEqual(testButtonLaunchArgs, button.Arguments);
            Assert.AreEqual(testImageUriSrc.OriginalString, button.ImageUri);
        }

        [TestMethod]
        public void AddInputTextBoxTest_WithStringIdOnly_ReturnSelfWithInputTextBoxAdded()
        {
            // Arrange
            string testInputTextBoxId = Guid.NewGuid().ToString();
            ToastContentBuilder builder = new ToastContentBuilder();

            // Act
            ToastContentBuilder anotherReference = builder.AddInputTextBox(testInputTextBoxId);

            // Assert
            Assert.AreSame(builder, anotherReference);
            var inputTextBox = (builder.Content.Actions as ToastActionsCustom).Inputs.First() as ToastTextBox;

            Assert.AreEqual(testInputTextBoxId, inputTextBox.Id);
        }

        [TestMethod]
        public void AddInputTextBoxTest_WithPlaceHolderContentAndTitle_ReturnSelfWithInputTextBoxAndAllOptionsAdded()
        {
            // Arrange
            string testInputTextBoxId = Guid.NewGuid().ToString();
            string testInputTextBoxPlaceHolderContent = "Placeholder Content";
            string testInputTextBoxTitle = "Test Title";
            ToastContentBuilder builder = new ToastContentBuilder();

            // Act
            ToastContentBuilder anotherReference = builder.AddInputTextBox(testInputTextBoxId, testInputTextBoxPlaceHolderContent, testInputTextBoxTitle);

            // Assert
            Assert.AreSame(builder, anotherReference);
            var inputTextBox = (builder.Content.Actions as ToastActionsCustom).Inputs.First() as ToastTextBox;

            Assert.AreEqual(testInputTextBoxId, inputTextBox.Id);
            Assert.AreEqual(testInputTextBoxPlaceHolderContent, inputTextBox.PlaceholderContent);
            Assert.AreEqual(testInputTextBoxTitle, inputTextBox.Title);
        }

        [TestMethod]
        public void AddComboBoxTest_WithMultipleChoices_ReturnSelfWithComboBoxAndAllChoicesAdded()
        {
            // Arrange
            string testComboBoxId = Guid.NewGuid().ToString();
            var choice1 = (Guid.NewGuid().ToString(), "Test Choice 1");
            var choice2 = (Guid.NewGuid().ToString(), "Test Choice 2");
            var choice3 = (Guid.NewGuid().ToString(), "Test Choice 3");

            ToastContentBuilder builder = new ToastContentBuilder();

            // Act
            ToastContentBuilder anotherReference = builder.AddComboBox(testComboBoxId, choice1, choice2, choice3);

            // Assert
            Assert.AreSame(builder, anotherReference);
            var comboBox = (builder.Content.Actions as ToastActionsCustom).Inputs.First() as ToastSelectionBox;

            Assert.AreEqual(testComboBoxId, comboBox.Id);
            Assert.AreEqual(choice1.Item1, comboBox.Items[0].Id);
            Assert.AreEqual(choice2.Item1, comboBox.Items[1].Id);
            Assert.AreEqual(choice3.Item1, comboBox.Items[2].Id);

            Assert.AreEqual(choice1.Item2, comboBox.Items[0].Content);
            Assert.AreEqual(choice2.Item2, comboBox.Items[1].Content);
            Assert.AreEqual(choice3.Item2, comboBox.Items[2].Content);
        }

        [TestMethod]
        public void AddComboBoxTest_WithMultipleChoicesAndDefaultSelected_ReturnSelfWithComboBoxAddedWithAllChoicesAndDefaultSelection()
        {
            // Arrange
            string testComboBoxId = Guid.NewGuid().ToString();
            var choice1 = (Guid.NewGuid().ToString(), "Test Choice 1");
            var choice2 = (Guid.NewGuid().ToString(), "Test Choice 2");
            var choice3 = (Guid.NewGuid().ToString(), "Test Choice 3");
            string defaultChoice = choice2.Item1;

            ToastContentBuilder builder = new ToastContentBuilder();

            // Act
            ToastContentBuilder anotherReference = builder.AddComboBox(testComboBoxId, defaultChoice, choice1, choice2, choice3);

            // Assert
            Assert.AreSame(builder, anotherReference);
            var comboBox = (builder.Content.Actions as ToastActionsCustom).Inputs.First() as ToastSelectionBox;

            Assert.AreEqual(testComboBoxId, comboBox.Id);
            Assert.AreEqual(choice1.Item1, comboBox.Items[0].Id);
            Assert.AreEqual(choice2.Item1, comboBox.Items[1].Id);
            Assert.AreEqual(choice3.Item1, comboBox.Items[2].Id);

            Assert.AreEqual(choice1.Item2, comboBox.Items[0].Content);
            Assert.AreEqual(choice2.Item2, comboBox.Items[1].Content);
            Assert.AreEqual(choice3.Item2, comboBox.Items[2].Content);

            Assert.AreEqual(defaultChoice, comboBox.DefaultSelectionBoxItemId);
        }

        [TestMethod]
        public void AddComboBoxTest_WithMultipleChoiceAndDefaultSelectedAndTitle_ReturnSelfWithComboBoxAddedWithAllChoicesAndDefaultSelectionAndTitle()
        {
            // Arrange
            string testComboBoxId = Guid.NewGuid().ToString();
            var choice1 = (Guid.NewGuid().ToString(), "Test Choice 1");
            var choice2 = (Guid.NewGuid().ToString(), "Test Choice 2");
            var choice3 = (Guid.NewGuid().ToString(), "Test Choice 3");
            string defaultChoice = choice2.Item1;
            string testComboBoxTitle = "Test Title";

            ToastContentBuilder builder = new ToastContentBuilder();

            // Act
            ToastContentBuilder anotherReference = builder.AddComboBox(testComboBoxId, testComboBoxTitle, defaultChoice, choice1, choice2, choice3);

            // Assert
            Assert.AreSame(builder, anotherReference);
            var comboBox = (builder.Content.Actions as ToastActionsCustom).Inputs.First() as ToastSelectionBox;

            Assert.AreEqual(testComboBoxId, comboBox.Id);
            Assert.AreEqual(choice1.Item1, comboBox.Items[0].Id);
            Assert.AreEqual(choice2.Item1, comboBox.Items[1].Id);
            Assert.AreEqual(choice3.Item1, comboBox.Items[2].Id);

            Assert.AreEqual(choice1.Item2, comboBox.Items[0].Content);
            Assert.AreEqual(choice2.Item2, comboBox.Items[1].Content);
            Assert.AreEqual(choice3.Item2, comboBox.Items[2].Content);

            Assert.AreEqual(defaultChoice, comboBox.DefaultSelectionBoxItemId);
            Assert.AreEqual(testComboBoxTitle, comboBox.Title);
        }
    }

#endif
}