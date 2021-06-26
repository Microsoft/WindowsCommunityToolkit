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
    public class Test_WrapPanel_BasicLayout : VisualUITestBase
    {
        [TestCategory("WrapPanel")]
        [TestMethod]
        public async Task Test_WrapPanel_Normal_Horizontal()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls"">
    <controls:WrapPanel x:Name=""WrapPanel"">
        <Border Width=""100"" Height=""50""/>
        <Border Width=""100"" Height=""50""/>
        <Border Width=""100"" Height=""50""/>
    </controls:WrapPanel>
</Page>") as FrameworkElement;

                var expected = new (int u, int v)[]
                {
                    (0, 0),
                    (100, 0),
                    (200, 0),
                };

                Assert.IsNotNull(treeRoot, "Could not load XAML tree.");

                // Initialize Visual Tree
                await SetTestContentAsync(treeRoot);

                var panel = treeRoot.FindChild("WrapPanel") as WrapPanel;

                Assert.IsNotNull(panel, "Could not find WrapPanel in tree.");

                // Force Layout calculations
                panel.UpdateLayout();

                var children = panel.Children.Cast<FrameworkElement>().ToArray();

                Assert.AreEqual(3, panel.Children.Count);

                // Check all children are in expected places.
                for (int i = 0; i < children.Length; i++)
                {
                    var transform = treeRoot.CoordinatesTo(children[i]);
                    Assert.AreEqual(expected[i].u, transform.X, $"Child {i} not in expected X location.");
                    Assert.AreEqual(expected[i].v, transform.Y, $"Child {i} not in expected Y location.");

                    Assert.AreEqual(100, children[i].ActualWidth, $"Child {i} not of expected width.");
                    Assert.AreEqual(50, children[i].ActualHeight, $"Child {i} not of expected height.");
                }
            });
        }

        [TestCategory("WrapPanel")]
        [TestMethod]
        public async Task Test_WrapPanel_Normal_Vertical()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls"">
    <controls:WrapPanel x:Name=""WrapPanel"" Orientation=""Vertical"">
        <Border Width=""100"" Height=""50""/>
        <Border Width=""100"" Height=""50""/>
        <Border Width=""100"" Height=""50""/>
    </controls:WrapPanel>
</Page>") as FrameworkElement;

                var expected = new (int u, int v)[]
                {
                    (0, 0),
                    (0, 50),
                    (0, 100),
                };

                Assert.IsNotNull(treeRoot, "Could not load XAML tree.");

                // Initialize Visual Tree
                await SetTestContentAsync(treeRoot);

                var panel = treeRoot.FindChild("WrapPanel") as WrapPanel;

                Assert.IsNotNull(panel, "Could not find WrapPanel in tree.");

                // Force Layout calculations
                panel.UpdateLayout();

                var children = panel.Children.Cast<FrameworkElement>().ToArray();

                Assert.AreEqual(3, panel.Children.Count);

                // Check all children are in expected places.
                for (int i = 0; i < children.Length; i++)
                {
                    var transform = treeRoot.CoordinatesTo(children[i]);
                    Assert.AreEqual(expected[i].u, transform.X, $"Child {i} not in expected X location.");
                    Assert.AreEqual(expected[i].v, transform.Y, $"Child {i} not in expected Y location.");

                    Assert.AreEqual(100, children[i].ActualWidth, $"Child {i} not of expected width.");
                    Assert.AreEqual(50, children[i].ActualHeight, $"Child {i} not of expected height.");
                }
            });
        }

        [TestCategory("WrapPanel")]
        [TestMethod]
        public async Task Test_WrapPanel_Normal_Horizontal_WithSpacing()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls"">
    <controls:WrapPanel x:Name=""WrapPanel"" HorizontalSpacing=""10"">
        <Border Width=""100"" Height=""50""/>
        <Border Width=""100"" Height=""50""/>
        <Border Width=""100"" Height=""50""/>
    </controls:WrapPanel>
</Page>") as FrameworkElement;

                var expected = new (int u, int v)[]
                {
                    (0, 0),
                    (110, 0),
                    (220, 0),
                };

                Assert.IsNotNull(treeRoot, "Could not load XAML tree.");

                // Initialize Visual Tree
                await SetTestContentAsync(treeRoot);

                var panel = treeRoot.FindChild("WrapPanel") as WrapPanel;

                Assert.IsNotNull(panel, "Could not find WrapPanel in tree.");

                // Force Layout calculations
                panel.UpdateLayout();

                var children = panel.Children.Cast<FrameworkElement>().ToArray();

                Assert.AreEqual(3, panel.Children.Count);

                // Check all children are in expected places.
                for (int i = 0; i < children.Length; i++)
                {
                    var transform = treeRoot.CoordinatesTo(children[i]);
                    Assert.AreEqual(expected[i].u, transform.X, $"Child {i} not in expected X location.");
                    Assert.AreEqual(expected[i].v, transform.Y, $"Child {i} not in expected Y location.");

                    Assert.AreEqual(100, children[i].ActualWidth, $"Child {i} not of expected width.");
                    Assert.AreEqual(50, children[i].ActualHeight, $"Child {i} not of expected height.");
                }
            });
        }

        [TestCategory("WrapPanel")]
        [TestMethod]
        public async Task Test_WrapPanel_Normal_Vertical_WithSpacing()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls"">
    <controls:WrapPanel x:Name=""WrapPanel"" Orientation=""Vertical"" VerticalSpacing=""10"">
        <Border Width=""100"" Height=""50""/>
        <Border Width=""100"" Height=""50""/>
        <Border Width=""100"" Height=""50""/>
    </controls:WrapPanel>
</Page>") as FrameworkElement;

                var expected = new (int u, int v)[]
                {
                    (0, 0),
                    (0, 60),
                    (0, 120),
                };

                Assert.IsNotNull(treeRoot, "Could not load XAML tree.");

                // Initialize Visual Tree
                await SetTestContentAsync(treeRoot);

                var panel = treeRoot.FindChild("WrapPanel") as WrapPanel;

                Assert.IsNotNull(panel, "Could not find WrapPanel in tree.");

                // Force Layout calculations
                panel.UpdateLayout();

                var children = panel.Children.Cast<FrameworkElement>().ToArray();

                Assert.AreEqual(3, panel.Children.Count);

                // Check all children are in expected places.
                for (int i = 0; i < children.Length; i++)
                {
                    var transform = treeRoot.CoordinatesTo(children[i]);
                    Assert.AreEqual(expected[i].u, transform.X, $"Child {i} not in expected X location.");
                    Assert.AreEqual(expected[i].v, transform.Y, $"Child {i} not in expected Y location.");

                    Assert.AreEqual(100, children[i].ActualWidth, $"Child {i} not of expected width.");
                    Assert.AreEqual(50, children[i].ActualHeight, $"Child {i} not of expected height.");
                }
            });
        }

        [TestCategory("WrapPanel")]
        [TestMethod]
        public async Task Test_WrapPanel_Normal_Horizontal_WithSpacing_AndPadding()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls"">
    <controls:WrapPanel x:Name=""WrapPanel"" HorizontalSpacing=""10"" Padding=""20"">
        <Border Width=""100"" Height=""50""/>
        <Border Width=""100"" Height=""50""/>
        <Border Width=""100"" Height=""50""/>
    </controls:WrapPanel>
</Page>") as FrameworkElement;

                var expected = new (int u, int v)[]
                {
                    (20, 20),
                    (130, 20),
                    (240, 20),
                };

                Assert.IsNotNull(treeRoot, "Could not load XAML tree.");

                // Initialize Visual Tree
                await SetTestContentAsync(treeRoot);

                var panel = treeRoot.FindChild("WrapPanel") as WrapPanel;

                Assert.IsNotNull(panel, "Could not find WrapPanel in tree.");

                // Force Layout calculations
                panel.UpdateLayout();

                var children = panel.Children.Cast<FrameworkElement>().ToArray();

                Assert.AreEqual(3, panel.Children.Count);

                // Check all children are in expected places.
                for (int i = 0; i < children.Length; i++)
                {
                    var transform = treeRoot.CoordinatesTo(children[i]);
                    Assert.AreEqual(expected[i].u, transform.X, $"Child {i} not in expected X location.");
                    Assert.AreEqual(expected[i].v, transform.Y, $"Child {i} not in expected Y location.");

                    Assert.AreEqual(100, children[i].ActualWidth, $"Child {i} not of expected width.");
                    Assert.AreEqual(50, children[i].ActualHeight, $"Child {i} not of expected height.");
                }
            });
        }

        [TestCategory("WrapPanel")]
        [TestMethod]
        public async Task Test_WrapPanel_Normal_Vertical_WithSpacing_AndPadding()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls"">
    <controls:WrapPanel x:Name=""WrapPanel"" Orientation=""Vertical"" VerticalSpacing=""10"" Padding=""20"">
        <Border Width=""100"" Height=""50""/>
        <Border Width=""100"" Height=""50""/>
        <Border Width=""100"" Height=""50""/>
    </controls:WrapPanel>
</Page>") as FrameworkElement;

                var expected = new (int u, int v)[]
                {
                    (20, 20),
                    (20, 80),
                    (20, 140),
                };

                Assert.IsNotNull(treeRoot, "Could not load XAML tree.");

                // Initialize Visual Tree
                await SetTestContentAsync(treeRoot);

                var panel = treeRoot.FindChild("WrapPanel") as WrapPanel;

                Assert.IsNotNull(panel, "Could not find WrapPanel in tree.");

                // Force Layout calculations
                panel.UpdateLayout();

                var children = panel.Children.Cast<FrameworkElement>().ToArray();

                Assert.AreEqual(3, panel.Children.Count);

                // Check all children are in expected places.
                for (int i = 0; i < children.Length; i++)
                {
                    var transform = treeRoot.CoordinatesTo(children[i]);
                    Assert.AreEqual(expected[i].u, transform.X, $"Child {i} not in expected X location.");
                    Assert.AreEqual(expected[i].v, transform.Y, $"Child {i} not in expected Y location.");

                    Assert.AreEqual(100, children[i].ActualWidth, $"Child {i} not of expected width.");
                    Assert.AreEqual(50, children[i].ActualHeight, $"Child {i} not of expected height.");
                }
            });
        }
    }
}