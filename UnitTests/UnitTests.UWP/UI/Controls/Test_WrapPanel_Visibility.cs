// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace UnitTests.UI.Controls
{
    [TestClass]
    public class Test_WrapPanel_Visibility : VisualUITestBase
    {
        [TestCategory("WrapPanel")]
        [TestMethod]
        public async Task Test_WrapPanel_Visibility_Horizontal_First()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls"">
    <controls:WrapPanel x:Name=""WrapPanel"">
        <Border Width=""150"" Height=""50"" Visibility=""Collapsed""/>
        <Border Width=""100"" Height=""50""/>
        <Border Width=""125"" Height=""50""/>
        <Border Width=""50"" Height=""50""/>
    </controls:WrapPanel>
</Page>") as FrameworkElement;

                treeRoot.UseLayoutRounding = false;

                var expected = new (int u, int v, int w, int h)[]
                {
                    (0, 0, 0, 0), // Collapsed
                    (0, 0, 100, 50),
                    (100, 0, 125, 50),
                    (225, 0, 50, 50),
                };

                Assert.IsNotNull(treeRoot, "Could not load XAML tree.");

                // Initialize Visual Tree
                await SetTestContentAsync(treeRoot);

                var panel = treeRoot.FindChild("WrapPanel") as WrapPanel;

                Assert.IsNotNull(panel, "Could not find WrapPanel in tree.");

                // Force Layout calculations
                panel.UpdateLayout();

                var children = panel.Children.Cast<FrameworkElement>().ToArray();

                Assert.AreEqual(4, panel.Children.Count);

                // Check all children are in expected places.
                for (int i = 0; i < children.Length; i++)
                {
                    var transform = treeRoot.CoordinatesTo(children[i]);
                    Assert.AreEqual(expected[i].u, transform.X, $"Child {i} not in expected X location.");
                    Assert.AreEqual(expected[i].v, transform.Y, $"Child {i} not in expected Y location.");

                    Assert.AreEqual(expected[i].w, children[i].ActualWidth, $"Child {i} not of expected width.");
                    Assert.AreEqual(expected[i].h, children[i].ActualHeight, $"Child {i} not of expected height.");
                }
            });
        }

        //// TODO: Add matching tests with spacing inbetween.

        [TestCategory("WrapPanel")]
        [TestMethod]
        public async Task Test_WrapPanel_Visibility_Horizontal_Middle()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls"">
    <controls:WrapPanel x:Name=""WrapPanel"">
        <Border Width=""150"" Height=""50""/>
        <Border Width=""100"" Height=""50"" Visibility=""Collapsed""/>
        <Border Width=""125"" Height=""50""/>
        <Border Width=""50"" Height=""50""/>
    </controls:WrapPanel>
</Page>") as FrameworkElement;

                treeRoot.UseLayoutRounding = false;

                var expected = new (int u, int v, int w, int h)[]
                {
                    (0, 0, 150, 50),
                    (0, 0, 0, 0),  // Collapsed, 150?
                    (150, 0, 125, 50),
                    (275, 0, 50, 50),
                };

                Assert.IsNotNull(treeRoot, "Could not load XAML tree.");

                // Initialize Visual Tree
                await SetTestContentAsync(treeRoot);

                var panel = treeRoot.FindChild("WrapPanel") as WrapPanel;

                Assert.IsNotNull(panel, "Could not find WrapPanel in tree.");

                // Force Layout calculations
                panel.UpdateLayout();

                var children = panel.Children.Cast<FrameworkElement>().ToArray();

                Assert.AreEqual(4, panel.Children.Count);

                // Check all children are in expected places.
                for (int i = 0; i < children.Length; i++)
                {
                    var transform = treeRoot.CoordinatesTo(children[i]);
                    Assert.AreEqual(expected[i].u, transform.X, $"Child {i} not in expected X location.");
                    Assert.AreEqual(expected[i].v, transform.Y, $"Child {i} not in expected Y location.");

                    Assert.AreEqual(expected[i].w, children[i].ActualWidth, $"Child {i} not of expected width.");
                    Assert.AreEqual(expected[i].h, children[i].ActualHeight, $"Child {i} not of expected height.");
                }
            });
        }

        [TestCategory("WrapPanel")]
        [TestMethod]
        public async Task Test_WrapPanel_Visibility_Horizontal_Last()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls"">
    <controls:WrapPanel x:Name=""WrapPanel"">
        <Border Width=""150"" Height=""50""/>
        <Border Width=""100"" Height=""50""/>
        <Border Width=""125"" Height=""50""/>
        <Border Width=""50"" Height=""50"" Visibility=""Collapsed""/>
    </controls:WrapPanel>
</Page>") as FrameworkElement;

                treeRoot.UseLayoutRounding = false;

                var expected = new (int u, int v, int w, int h)[]
                {
                    (0, 0, 150, 50),
                    (150, 0, 100, 50),
                    (250, 0, 125, 50),
                    (0, 0, 0, 0), // Collapsed, 375?
                };

                Assert.IsNotNull(treeRoot, "Could not load XAML tree.");

                // Initialize Visual Tree
                await SetTestContentAsync(treeRoot);

                var panel = treeRoot.FindChild("WrapPanel") as WrapPanel;

                Assert.IsNotNull(panel, "Could not find WrapPanel in tree.");

                // Force Layout calculations
                panel.UpdateLayout();

                var children = panel.Children.Cast<FrameworkElement>().ToArray();

                Assert.AreEqual(4, panel.Children.Count);

                // Check all children are in expected places.
                for (int i = 0; i < children.Length; i++)
                {
                    var transform = treeRoot.CoordinatesTo(children[i]);
                    Assert.AreEqual(expected[i].u, transform.X, $"Child {i} not in expected X location.");
                    Assert.AreEqual(expected[i].v, transform.Y, $"Child {i} not in expected Y location.");

                    Assert.AreEqual(expected[i].w, children[i].ActualWidth, $"Child {i} not of expected width.");
                    Assert.AreEqual(expected[i].h, children[i].ActualHeight, $"Child {i} not of expected height.");
                }
            });
        }

        //// TODO: Add test for toggling visibility
        //// TODO: Add tests for wrapping lines based on size.
        //// TODO: Add a test with StretchChild behavior as well after...
    }
}