// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace UnitTests.Extensions
{
    [TestClass]
    public class Test_UIElementExtensions_Coordinates : VisualUITestBase
    {
        [TestCategory("UIElementExtensions")]
        [TestMethod]
        public async Task Test_UIElement_Extensions_CoordinatesTo()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""> <!-- Starting Point -->
        <Border x:Name=""Target"" Margin=""150,100,0,0""/>
</Page>") as Page;

                // Test Setup
                Assert.IsNotNull(treeRoot, "XAML Failed to Load");

                // Initialize Visual Tree
                await SetTestContentAsync(treeRoot);

                // Main Test
                var border = treeRoot.FindDescendant("Target");

                Assert.IsNotNull(border, "Expected to find something.");
                Assert.IsInstanceOfType(border, typeof(Border), "Didn't find expected typed element.");

                var point = treeRoot.CoordinatesTo(border);

                Assert.AreEqual(150, point.X);
                Assert.AreEqual(100, point.Y);

                // And otherway
                point = border.CoordinatesTo(treeRoot);

                Assert.AreEqual(-150, point.X);
                Assert.AreEqual(-100, point.Y);
            });
        }

        [TestCategory("UIElementExtensions")]
        [TestMethod]
        public async Task Test_UIElement_Extensions_CoordinatesFrom()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""> <!-- Starting Point -->
        <Border x:Name=""Target"" Margin=""100,150,0,0""/>
</Page>") as Page;

                // Test Setup
                Assert.IsNotNull(treeRoot, "XAML Failed to Load");

                // Initialize Visual Tree
                await SetTestContentAsync(treeRoot);

                // Main Test
                var border = treeRoot.FindDescendant("Target");

                Assert.IsNotNull(border, "Expected to find something.");
                Assert.IsInstanceOfType(border, typeof(Border), "Didn't find expected typed element.");

                var point = border.CoordinatesFrom(treeRoot);

                Assert.AreEqual(100, point.X);
                Assert.AreEqual(150, point.Y);

                // And Backwards
                point = treeRoot.CoordinatesFrom(border);

                Assert.AreEqual(-100, point.X);
                Assert.AreEqual(-150, point.Y);
            });
        }
    }
}