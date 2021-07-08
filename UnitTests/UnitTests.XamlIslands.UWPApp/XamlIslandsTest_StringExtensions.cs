// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI;
using CommunityToolkit.WinUI.UI.Controls;
using CommunityToolkit.WinUI.UI.Controls.TextToolbarButtons.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace UnitTests.XamlIslands.UWPApp
{
    [STATestClass]
    public partial class XamlIslandsTest_StringExtensions
    {
        [TestMethod]
        public async Task StringExtensions_GetViewLocalized()
        {
            await App.Dispatcher.EnqueueAsync(() =>
            {
                var xamlRoot = App.XamlRoot;
                var str = "abc".GetViewLocalized(xamlRoot.UIContext);
                Assert.AreEqual("ABCDEF", str);
            });
        }

        [TestMethod]
        public async Task StringExtensions_GetLocalized()
        {
            await App.Dispatcher.EnqueueAsync(() =>
            {
                var xamlRoot = App.XamlRoot;
                var str = "abc".GetLocalized(xamlRoot.UIContext);
                Assert.AreEqual("ABCDEF", str);
            });
        }

        [TestMethod]
        public void StringExtensions_GetLocalizedWithResourcePath()
        {
            var str = "WCT_TextToolbar_OkLabel".GetLocalized("CommunityToolkit.WinUI.UI.Controls.Core/Resources");
            Assert.AreEqual("Ok", str);
        }

        /// <summary>
        /// Tests the general ability to look-up a resource from the UI control as a base-case.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task Test_TextToolbar_Localization_Retrieve()
        {
            await App.Dispatcher.EnqueueAsync(() =>
            {
                var treeRoot = XamlReader.Load(
@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:controls=""using:CommunityToolkit.WinUI.UI.Controls"">

    <controls:TextToolbar x:Name=""TextToolbarControl"">
    </controls:TextToolbar>

</Page>") as FrameworkElement;

                Assert.IsNotNull(treeRoot, "Could not load XAML tree.");

                var toolbar = treeRoot.FindChild("TextToolbarControl") as TextToolbar;

                Assert.IsNotNull(toolbar, "Could not find TextToolbar in tree.");

                var commonButtons = new CommonButtons(toolbar);
                var boldButton = commonButtons.Bold;

                Assert.IsNotNull(boldButton, "Bold Button not found.");

                Assert.AreEqual("Bold", boldButton.ToolTip, "Label doesn't match expected default value.");
            });
        }

        /// <summary>
        /// Tests the ability to override the resource lookup for a toolkit component in the app resource dictionary.
        /// See Link:Strings/en-us/Resources.resw
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task Test_TextToolbar_Localization_Override()
        {
            await App.Dispatcher.EnqueueAsync(() =>
            {
                var commonButtons = new CommonButtons(new TextToolbar());
                var italicsButton = commonButtons.Italics;

                Assert.IsNotNull(italicsButton, "Italics Button not found.");

                Assert.AreEqual("ItalicsOverride", italicsButton.ToolTip, "Label doesn't match expected default value.");
            });
        }

        /// <summary>
        /// Tests the ability to have different overrides in different languages.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [TestMethod]
        public async Task Test_TextToolbar_Localization_Override_Fr()
        {
            await App.Dispatcher.EnqueueAsync(async () =>
            {
                // Just double-check we've got the right environment setup in our tests.
                //// Note: This seems to fail on XAML Islands, but the rest of the test works fine...?
                ////CollectionAssert.AreEquivalent(new string[] { "en-US", "fr" }, ApplicationLanguages.ManifestLanguages.ToArray(), "Missing locales for test");

                // Override the default language for this test only (we'll set it back after).
                var defaultLanguage = ApplicationLanguages.PrimaryLanguageOverride;
                ApplicationLanguages.PrimaryLanguageOverride = "fr";

                // Need to add a delay for release mode as otherwise the language switch doesn't kickover quick enough
                // This should be sufficient as we're just using this as a test-harness.
                await Task.Delay(3000);

                var commonButtons = new CommonButtons(new TextToolbar());
                var italicsButton = commonButtons.Italics;

                // Note: When running locally if the test somehow fails before the default is reset, then
                // the tests will be in a bad state as PrimaryLanguageOverride is persisted.
                // To fix this, uninstall the UnitTests UWP app and run the tests again.
                ApplicationLanguages.PrimaryLanguageOverride = defaultLanguage;

                // Check for expected values.
                Assert.IsNotNull(italicsButton, "Italics Button not found.");

                Assert.AreEqual("ItalicsFr", italicsButton.ToolTip, "Label doesn't match expected default value.");
            });
        }
    }
}