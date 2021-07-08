// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Windows.UI.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Text;

namespace UnitTests.Extensions
{
    [TestClass]
    public class Test_FontIconSourceExtensionMarkupExtension
    {
        [TestCategory("FontIconSourceExtensionMarkupExtension")]
        [UITestMethod]
        public void Test_FontIconSourceExtension_MarkupExtension_ProvideSegoeMdl2Asset()
        {
            var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:ui=""using:CommunityToolkit.WinUI.UI""
    xmlns:controls=""using:UnitTests.Extensions"">
        <controls:MockSwipeItem x:Name=""Check"" IconSource=""{ui:FontIconSource Glyph=&#xE105;}""/>
</Page>") as FrameworkElement;

            var button = treeRoot.FindChild("Check") as MockSwipeItem;

            Assert.IsNotNull(button, $"Could not find the {nameof(MockSwipeItem)} control in tree.");

            var icon = button.IconSource as FontIconSource;

            Assert.IsNotNull(icon, $"Could not find the {nameof(FontIcon)} element in button.");

            Assert.AreEqual(icon.Glyph, "\uE105", "Expected icon glyph to be E105.");
            Assert.AreEqual(icon.FontFamily.Source, "Segoe MDL2 Assets", "Expected font family to be Segoe MDL2 Assets");
        }

        [TestCategory("FontIconSourceExtensionMarkupExtension")]
        [UITestMethod]
        public void Test_FontIconSourceExtension_MarkupExtension_ProvideSegoeUI()
        {
            var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:ui=""using:CommunityToolkit.WinUI.UI""
    xmlns:controls=""using:UnitTests.Extensions"">
        <controls:MockSwipeItem x:Name=""Check"" IconSource=""{ui:FontIconSource Glyph=&#xE14D;, FontFamily='Segoe UI'}""/>
</Page>") as FrameworkElement;

            var button = treeRoot.FindChild("Check") as MockSwipeItem;

            Assert.IsNotNull(button, $"Could not find the {nameof(MockSwipeItem)} control in tree.");

            var icon = button.IconSource as FontIconSource;

            Assert.IsNotNull(icon, $"Could not find the {nameof(FontIcon)} element in button.");

            Assert.AreEqual(icon.Glyph, "\uE14D", "Expected icon glyph to be E14D.");
            Assert.AreEqual(icon.FontFamily.Source, "Segoe UI", "Expected font family to be Segoe UI");
        }

        [TestCategory("FontIconSourceExtensionMarkupExtension")]
        [UITestMethod]
        public void Test_FontIconSourceExtension_MarkupExtension_ProvideCustomFontIcon()
        {
            var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:ui=""using:CommunityToolkit.WinUI.UI""
    xmlns:controls=""using:UnitTests.Extensions"">
        <controls:MockSwipeItem x:Name=""Check"" IconSource=""{ui:FontIconSource Glyph=&#xE14D;, FontSize=7, FontFamily='Segoe MDL2 Assets', FontWeight=Bold, FontStyle=Italic, IsTextScaleFactorEnabled=True, MirroredWhenRightToLeft=True}""/>
</Page>") as FrameworkElement;

            var button = treeRoot.FindChild("Check") as MockSwipeItem;

            Assert.IsNotNull(button, $"Could not find the {nameof(MockSwipeItem)} control in tree.");

            var icon = button.IconSource as FontIconSource;

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

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402", Justification = "Mock control for tests")]
    public class MockSwipeItem : Control
    {
        public IconSource IconSource
        {
            get => (IconSource)GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register(
                nameof(IconSource),
                typeof(IconSource),
                typeof(MockSwipeItem),
                new PropertyMetadata(default(IconSource)));
    }
}