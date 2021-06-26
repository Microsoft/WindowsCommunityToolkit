// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace UnitTests.Extensions
{
    [TestClass]
    public class Test_VisualTreeExtensions : VisualUITestBase
    {
        [TestCategory("VisualTree")]
        [TestMethod]
        public async Task Test_VisualTree_FindDescendantByName_Exists()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""> <!-- Starting Point -->
    <Grid>
        <Grid>
            <Border/>
            <StackPanel>
                <TextBox/>
                <TextBlock x:Name=""TargetElement""/> <!-- Target -->
            </StackPanel>
        </Grid>
    </Grid>
</Page>") as Page;

                // Test Setup
                Assert.IsNotNull(treeRoot, "XAML Failed to Load");

                // Initialize Visual Tree
                await SetTestContentAsync(treeRoot);

                // Main Test
                var textBlock = treeRoot.FindDescendant("TargetElement");

                Assert.IsNotNull(textBlock, "Expected to find something.");
                Assert.IsInstanceOfType(textBlock, typeof(TextBlock), "Didn't find expected typed element.");
                Assert.AreEqual("TargetElement", textBlock.Name, "Didn't find named element.");
            });
        }

        [TestCategory("VisualTree")]
        [TestMethod]
        public async Task Test_VisualTree_FindDescendantByName_NotFound()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""> <!-- Starting Point -->
    <Grid>
        <Grid>
            <Border/>
            <StackPanel>
                <TextBox/>
                <TextBlock/>
            </StackPanel>
        </Grid>
    </Grid>
</Page>") as Page;

                // Test Setup
                Assert.IsNotNull(treeRoot, "XAML Failed to Load");

                // Initialize Visual Tree
                await SetTestContentAsync(treeRoot);

                // Main Test
                var textBlock = treeRoot.FindDescendant("TargetElement");

                Assert.IsNull(textBlock, "Didn't expect to find anything.");
            });
        }

        [TestCategory("VisualTree")]
        [TestMethod]
        public async Task Test_VisualTree_FindDescendant_Exists()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""> <!-- Starting Point -->
    <Grid>
        <Grid>
            <Border/>
            <StackPanel>
                <TextBox/>
                <TextBlock/> <!-- Target -->
            </StackPanel>
        </Grid>
    </Grid>
</Page>") as Page;

                // Test Setup
                Assert.IsNotNull(treeRoot, "XAML Failed to Load");

                // Initialize Visual Tree
                await SetTestContentAsync(treeRoot);

                // Main Test
                var textBlock = treeRoot.FindDescendant<TextBlock>();

                Assert.IsNotNull(textBlock, "Expected to find something.");
                Assert.IsInstanceOfType(textBlock, typeof(TextBlock), "Didn't find expected typed element.");
            });
        }

        [TestCategory("VisualTree")]
        [TestMethod]
        public async Task Test_VisualTree_FindDescendant_ItemsControl_Exists()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""> <!-- Starting Point -->
    <Grid>
        <Grid>
            <Border/>
            <Pivot>
                <PivotItem>
                    <TextBox/>
                </PivotItem>
                <PivotItem>
                    <TextBlock/> <!-- Target -->
                </PivotItem>
            </Pivot>
        </Grid>
    </Grid>
</Page>") as Page;

                // Test Setup
                Assert.IsNotNull(treeRoot, "XAML Failed to Load");

                // Initialize Visual Tree
                await SetTestContentAsync(treeRoot);

                // Main Test
                var textBlock = treeRoot.FindDescendant<TextBlock>();

                Assert.IsNotNull(textBlock, "Expected to find something.");
                Assert.IsInstanceOfType(textBlock, typeof(TextBlock), "Didn't find expected typed element.");
            });
        }

        [TestCategory("VisualTree")]
        [TestMethod]
        public async Task Test_VisualTree_FindDescendant_NotFound()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""> <!-- Starting Point -->
    <Grid>
        <Grid>
            <Border/>
            <StackPanel>
                <TextBox/>
                <TextBlock/>
            </StackPanel>
        </Grid>
    </Grid>
</Page>") as Page;

                // Test Setup
                Assert.IsNotNull(treeRoot, "XAML Failed to Load");

                // Initialize Visual Tree
                await SetTestContentAsync(treeRoot);

                // Main Test
                var uniformGrid = treeRoot.FindDescendant<UniformGrid>();

                Assert.IsNull(uniformGrid, "Didn't expect to find anything.");
            });
        }

        [TestCategory("VisualTree")]
        [TestMethod]
        public async Task Test_VisualTree_FindDescendants_Exists()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""> <!-- Starting Point -->
    <Grid>
        <Grid>
            <Border/>
            <TextBlock x:Name=""One""/> <!-- Target -->
            <StackPanel>
                <TextBox/> <!-- Hidden Target -->
                <TextBlock x:Name=""Two""/> <!-- Target -->
            </StackPanel>
        </Grid>
        <TextBlock x:Name=""Three""/> <!-- Target -->
    </Grid>
</Page>") as Page;

                // Test Setup
                Assert.IsNotNull(treeRoot, "XAML Failed to Load");

                // Initialize Visual Tree
                await SetTestContentAsync(treeRoot);

                // Main Test
                var textBlocks = treeRoot.FindDescendants().OfType<TextBlock>();

                Assert.IsNotNull(textBlocks, "Expected to find something.");

                var array = textBlocks.ToArray();

                Assert.AreEqual(4, array.Length, "Expected to find 4 TextBlock elements.");

                // I don't think we want to guarantee order here, so just care that we can find each one.
                Assert.IsTrue(array.Any((tb) => tb.Name == "One"), "Couldn't find TextBlock 'One'");
                Assert.IsTrue(array.Any((tb) => tb.Name == "Two"), "Couldn't find TextBlock 'Two'");
                Assert.IsTrue(array.Any((tb) => tb.Name == "Three"), "Couldn't find TextBlock 'Three'");

                // TextBox has one in its template!
                Assert.IsTrue(array.Any((tb) => tb.Name == "PlaceholderTextContentPresenter"), "Couldn't find hidden TextBlock from TextBox.");
            });
        }

        [TestCategory("VisualTree")]
        [TestMethod]
        public async Task Test_VisualTree_FindDescendants_NotFound()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""> <!-- Starting Point -->
    <Grid>
        <Grid>
            <Border/>
            <TextBlock x:Name=""One""/>
            <StackPanel>
                <TextBox/>
                <TextBlock x:Name=""Two""/>
            </StackPanel>
        </Grid>
        <TextBlock x:Name=""Three""/>
    </Grid>
</Page>") as Page;

                // Test Setup
                Assert.IsNotNull(treeRoot, "XAML Failed to Load");

                // Initialize Visual Tree
                await SetTestContentAsync(treeRoot);

                // Main Test
                var thing = treeRoot.FindDescendants().OfType<UniformGrid>();

                Assert.IsNotNull(thing, "Expected to find something.");

                var array = thing.ToArray();

                Assert.AreEqual(0, array.Length, "Expected to find no elements.");
            });
        }

        [TestCategory("VisualTree")]
        [TestMethod]
        public async Task Test_VisualTree_FindAscendant_Exists()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <Grid>
        <Grid> <!-- Target -->
            <Border/>
            <Border>
                <TextBlock/> <!-- Starting Point -->
            </Border>
        </Grid>
    </Grid>
</Page>") as Page;

                // Test Setup
                Assert.IsNotNull(treeRoot, "XAML Failed to Load");

                // Initialize Visual Tree
                await SetTestContentAsync(treeRoot);

                var outerGrid = treeRoot.Content as Grid;

                Assert.IsNotNull(outerGrid, "Couldn't find Page content.");

                var targetGrid = outerGrid.Children.FirstOrDefault() as Grid;
                Assert.IsNotNull(targetGrid, "Couldn't find Target Grid");
                Assert.AreEqual(2, targetGrid.Children.Count, "Grid doesn't have right number of children.");

                var secondBorder = targetGrid.Children[1] as Border;
                Assert.IsNotNull(secondBorder, "Border not found.");

                var startingPoint = secondBorder.Child as FrameworkElement;
                Assert.IsNotNull(startingPoint, "Could not find starting element.");

                // Main Test
                var grid = startingPoint.FindAscendant<Grid>();

                Assert.IsNotNull(grid, "Expected to find Grid");
                Assert.AreEqual(targetGrid, grid, "Grid didn't match expected.");
            });
        }

        [TestCategory("VisualTree")]
        [TestMethod]
        public async Task Test_VisualTree_FindAscendant_NotFound()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <Grid>
        <Grid>
            <Border/>
            <Border>
                <TextBlock/> <!-- Starting Point -->
            </Border>
        </Grid>
    </Grid>
</Page>") as Page;

                // Test Setup
                Assert.IsNotNull(treeRoot, "XAML Failed to Load");

                // Initialize Visual Tree
                await SetTestContentAsync(treeRoot);

                var outerGrid = treeRoot.Content as Grid;

                Assert.IsNotNull(outerGrid, "Couldn't find Page content.");

                var innerGrid = outerGrid.Children.FirstOrDefault() as Grid;
                Assert.IsNotNull(innerGrid, "Couldn't find Target Grid");
                Assert.AreEqual(2, innerGrid.Children.Count, "Grid doesn't have right number of children.");

                var secondBorder = innerGrid.Children[1] as Border;
                Assert.IsNotNull(secondBorder, "Border not found.");

                var startingPoint = secondBorder.Child as FrameworkElement;
                Assert.IsNotNull(startingPoint, "Could not find starting element.");

                // Main Test
                var grid = startingPoint.FindAscendant<TextBlock>();

                Assert.IsNull(grid, "Didn't expect to find anything.");
            });
        }

        [TestCategory("VisualTree")]
        [TestMethod]
        public async Task Test_VisualTree_FindAscendantByName_Exists()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <Grid>
        <Grid x:Name=""TargetElement""> <!-- Target -->
            <Border/>
            <Border>
                <TextBlock/> <!-- Starting Point -->
            </Border>
        </Grid>
    </Grid>
</Page>") as Page;

                // Test Setup
                Assert.IsNotNull(treeRoot, "XAML Failed to Load");

                // Initialize Visual Tree
                await SetTestContentAsync(treeRoot);

                var outerGrid = treeRoot.Content as Grid;

                Assert.IsNotNull(outerGrid, "Couldn't find Page content.");

                var targetGrid = outerGrid.Children.FirstOrDefault() as Grid;
                Assert.IsNotNull(targetGrid, "Couldn't find Target Grid");
                Assert.AreEqual("TargetElement", targetGrid.Name, "Named element not found.");
                Assert.AreEqual(2, targetGrid.Children.Count, "Grid doesn't have right number of children.");

                var secondBorder = targetGrid.Children[1] as Border;
                Assert.IsNotNull(secondBorder, "Border not found.");

                var startingPoint = secondBorder.Child as FrameworkElement;
                Assert.IsNotNull(startingPoint, "Could not find starting element.");

                // Main Test
                var grid = startingPoint.FindAscendant("TargetElement");

                Assert.IsNotNull(grid, "Expected to find Grid");
                Assert.AreEqual(targetGrid, grid, "Grid didn't match expected.");
            });
        }

        [TestCategory("VisualTree")]
        [TestMethod]
        public async Task Test_VisualTree_FindAscendantByName_NotFound()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <Grid>
        <Grid>
            <Border/>
            <Border>
                <TextBlock/> <!-- Starting Point -->
            </Border>
        </Grid>
    </Grid>
</Page>") as Page;

                // Test Setup
                Assert.IsNotNull(treeRoot, "XAML Failed to Load");

                // Initialize Visual Tree
                await SetTestContentAsync(treeRoot);

                var outerGrid = treeRoot.Content as Grid;

                Assert.IsNotNull(outerGrid, "Couldn't find Page content.");

                var innerGrid = outerGrid.Children.FirstOrDefault() as Grid;
                Assert.IsNotNull(innerGrid, "Couldn't find Target Grid");
                Assert.AreEqual(2, innerGrid.Children.Count, "Grid doesn't have right number of children.");

                var secondBorder = innerGrid.Children[1] as Border;
                Assert.IsNotNull(secondBorder, "Border not found.");

                var startingPoint = secondBorder.Child as FrameworkElement;
                Assert.IsNotNull(startingPoint, "Could not find starting element.");

                // Main Test
                var grid = startingPoint.FindAscendant("TargetElement");

                Assert.IsNull(grid, "Didn't expect to find anything.");
            });
        }

        [TestCategory("VisualTree")]
        [TestMethod]
        public async Task Test_VisualTree_FindAscendants_Simple_Exists()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <Grid>
        <StackPanel>
            <TextBox/>
            <Border>
                <TextBlock/> <!-- Starting Point -->
            </Border>
        </StackPanel>
    </Grid>
</Page>") as Page;

                // Test Setup
                Assert.IsNotNull(treeRoot, "XAML Failed to Load");

                // Initialize Visual Tree
                await SetTestContentAsync(treeRoot);

                var outerGrid = treeRoot.Content as Grid;

                Assert.IsNotNull(outerGrid, "Couldn't find Page content.");

                var innerStackPanel = outerGrid.Children.FirstOrDefault() as StackPanel;
                Assert.IsNotNull(innerStackPanel, "Couldn't find inner StackPanel");
                Assert.AreEqual(2, innerStackPanel.Children.Count, "StackPanel doesn't have right number of children.");

                var secondBorder = innerStackPanel.Children[1] as Border;
                Assert.IsNotNull(secondBorder, "Border not found.");

                var startingPoint = secondBorder.Child as FrameworkElement;
                Assert.IsNotNull(startingPoint, "Could not find starting element.");

                // Main Test
                var ascendants = startingPoint.FindAscendants()
                                              .TakeWhile(el => el.GetType() != typeof(ContentPresenter)); // Otherwise we break beyond our page into the Test App, so this is kind of a hack to the test.

                Assert.IsNotNull(ascendants, "Expected to find something.");

                var array = ascendants.ToArray();

                Assert.AreEqual(4, array.Length, "Expected to find 4 parent elements.");

                // We should expect these to have been enumerated in order
                Assert.IsInstanceOfType(array[0], typeof(Border), "Didn't find immediate Border Parent");
                Assert.IsInstanceOfType(array[1], typeof(StackPanel), "Didn't find expected StackPanel");
                Assert.IsInstanceOfType(array[2], typeof(Grid), "Didn't find expected Grid");
                Assert.IsInstanceOfType(array[3], typeof(Page), "Didn't find expected Page");
            });
        }

        // TODO: Add another Ascendants test where we have something like a ListView which creates a container.
    }
}