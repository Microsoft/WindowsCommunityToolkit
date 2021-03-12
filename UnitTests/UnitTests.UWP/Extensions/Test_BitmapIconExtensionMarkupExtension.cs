// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace UnitTests.Extensions
{
    [TestClass]
    public class Test_BitmapIconExtensionMarkupExtension
    {
        [TestCategory("BitmapIconExtensionMarkupExtension")]
        [UITestMethod]
        public void Test_BitmapIconExtension_MarkupExtension_ProvideImage()
        {
            var treeroot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:ui=""using:Microsoft.Toolkit.Uwp.UI"">
        <Button x:Name=""RootButton"">
            <Button.Flyout>
                <MenuFlyout>
                    <MenuFlyoutItem Icon=""{ui:BitmapIcon Source=/Assets/StoreLogo.png}"" />
                </MenuFlyout>
            </Button.Flyout>
        </Button>
</Page>") as FrameworkElement;

            var button = treeroot.FindChild("RootButton") as Button;

            Assert.IsNotNull(button, $"Could not find the {nameof(Button)} control in tree.");

            var item = ((MenuFlyout)button.Flyout)?.Items?.FirstOrDefault() as MenuFlyoutItem;

            Assert.IsNotNull(button, $"Could not find the target {nameof(MenuFlyoutItem)} control.");

            var icon = item.Icon as BitmapIcon;

            Assert.IsNotNull(icon, $"Could not find the {nameof(BitmapIcon)} element in button.");

            Assert.AreEqual(icon.UriSource, new Uri("ms-resource:///Files/Assets/StoreLogo.png"), "Expected ms-resource:///Files/Assets/StoreLogo.png uri.");
            Assert.AreEqual(icon.ShowAsMonochrome, false, "Expected icon not to be monochrome");
        }

        [TestCategory("BitmapIconExtensionMarkupExtension")]
        [UITestMethod]
        public void Test_BitmapIconExtension_MarkupExtension_ProvideImageAndMonochrome()
        {
            var treeroot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:ui=""using:Microsoft.Toolkit.Uwp.UI"">
        <Button x:Name=""RootButton"">
            <Button.Flyout>
                <MenuFlyout>
                    <MenuFlyoutItem Icon=""{ui:BitmapIcon Source=/Assets/StoreLogo.png, ShowAsMonochrome=True}"" />
                </MenuFlyout>
            </Button.Flyout>
        </Button>
</Page>") as FrameworkElement;

            var button = treeroot.FindChild("RootButton") as Button;

            Assert.IsNotNull(button, $"Could not find the {nameof(Button)} control in tree.");

            var item = ((MenuFlyout)button.Flyout)?.Items?.FirstOrDefault() as MenuFlyoutItem;

            Assert.IsNotNull(button, $"Could not find the target {nameof(MenuFlyoutItem)} control.");

            var icon = item.Icon as BitmapIcon;

            Assert.IsNotNull(icon, $"Could not find the {nameof(BitmapIcon)} element in button.");

            Assert.AreEqual(icon.UriSource, new Uri("ms-resource:///Files/Assets/StoreLogo.png"), "Expected ms-resource:///Files/Assets/StoreLogo.png uri.");
            Assert.AreEqual(icon.ShowAsMonochrome, true, "Expected icon to be monochrome");
        }
    }
}