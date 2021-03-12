// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Microsoft.Toolkit.Uwp.UI;

namespace UnitTests.Extensions
{
    [TestClass]
    public class Test_FontIconExtensionMarkupExtension
    {
        [TestCategory("FontIconExtensionMarkupExtension")]
        [UITestMethod]
        public void Test_FontIconExtension_MarkupExtension_ProvideSegoeMdl2Asset()
        {
            var treeroot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:ui=""using:Microsoft.Toolkit.Uwp.UI"">
        <AppBarButton x:Name=""Check"" Icon=""{ui:FontIcon Glyph=&#xE105;}""/>
</Page>") as FrameworkElement;

            var button = treeroot.FindChild("Check") as AppBarButton;

            Assert.IsNotNull(button, $"Could not find the {nameof(AppBarButton)} control in tree.");

            var icon = button.Icon as FontIcon;

            Assert.IsNotNull(icon, $"Could not find the {nameof(FontIcon)} element in button.");

            Assert.AreEqual(icon.Glyph, "\uE105", "Expected icon glyph to be E105.");
            Assert.AreEqual(icon.FontFamily.Source, "Segoe MDL2 Assets", "Expected font family to be Segoe MDL2 Assets");
        }

        [TestCategory("FontIconExtensionMarkupExtension")]
        [UITestMethod]
        public void Test_FontIconExtension_MarkupExtension_ProvideSegoeUI()
        {
            var treeroot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:ui=""using:Microsoft.Toolkit.Uwp.UI"">
        <AppBarButton x:Name=""Check"" Icon=""{ui:FontIcon Glyph=&#xE14D;, FontFamily='Segoe UI'}""/>
</Page>") as FrameworkElement;

            var button = treeroot.FindChild("Check") as AppBarButton;

            Assert.IsNotNull(button, $"Could not find the {nameof(AppBarButton)} control in tree.");

            var icon = button.Icon as FontIcon;

            Assert.IsNotNull(icon, $"Could not find the {nameof(FontIcon)} element in button.");

            Assert.AreEqual(icon.Glyph, "\uE14D", "Expected icon glyph to be E14D.");
            Assert.AreEqual(icon.FontFamily.Source, "Segoe UI", "Expected font family to be Segoe UI");
        }

        [TestCategory("FontIconExtensionMarkupExtension")]
        [UITestMethod]
        public void Test_FontIconExtension_MarkupExtension_ProvideCustomFontIcon()
        {
            var treeroot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:ui=""using:Microsoft.Toolkit.Uwp.UI"">
        <AppBarButton x:Name=""Check"" Icon=""{ui:FontIcon Glyph=&#xE14D;, FontSize=7, FontFamily='Segoe MDL2 Assets', FontWeight=Bold, FontStyle=Italic, IsTextScaleFactorEnabled=True, MirroredWhenRightToLeft=True}""/>
</Page>") as FrameworkElement;

            var button = treeroot.FindChild("Check") as AppBarButton;

            Assert.IsNotNull(button, $"Could not find the {nameof(AppBarButton)} control in tree.");

            var icon = button.Icon as FontIcon;

            Assert.IsNotNull(icon, $"Could not find the {nameof(FontIcon)} element in button.");

            Assert.AreEqual(icon.Glyph, "\uE14D", "Expected icon glyph to be E14D.");
            Assert.AreEqual(icon.FontSize, 7.0, "Expected font size of 7");
            Assert.AreEqual(icon.FontFamily.Source, "Segoe MDL2 Assets", "Expected font family to be Segoe MDL2 Assets");
            Assert.AreEqual(icon.FontWeight, FontWeights.Bold, "Expected bold font weight");
            Assert.AreEqual(icon.FontStyle, FontStyle.Italic, "Expected italic font style");
            Assert.AreEqual(icon.IsTextScaleFactorEnabled, true, "Expected IsTextScaleFactorEnabled set to true");
            Assert.AreEqual(icon.MirroredWhenRightToLeft, true, "Expected MirroredWhenRightToLeft set to true");
        }
    }
}
