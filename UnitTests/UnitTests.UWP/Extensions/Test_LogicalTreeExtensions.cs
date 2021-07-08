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
    public class Test_LogicalTreeExtensions : VisualUITestBase
    {
        [TestCategory("LogicalTree")]
        [TestMethod]
        public async Task Test_LogicalTree_FindChildByName_Exists()
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
                var textBlock = treeRoot.FindChild("TargetElement");

                Assert.IsNotNull(textBlock, "Expected to find something.");
                Assert.IsInstanceOfType(textBlock, typeof(TextBlock), "Didn't find expected typed element.");
                Assert.AreEqual("TargetElement", textBlock.Name, "Didn't find named element.");
            });
        }

        [TestCategory("LogicalTree")]
        [TestMethod]
        public async Task Test_LogicalTree_FindChildByName_NotFound()
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
                var textBlock = treeRoot.FindChild("TargetElement");

                Assert.IsNull(textBlock, "Didn't expect to find anything.");
            });
        }

        [TestCategory("LogicalTree")]
        [TestMethod]
        public async Task Test_LogicalTree_FindChild_Exists()
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
                var textBlock = treeRoot.FindChild<TextBlock>();

                Assert.IsNotNull(textBlock, "Expected to find something.");
                Assert.IsInstanceOfType(textBlock, typeof(TextBlock), "Didn't find expected typed element.");
            });
        }

        [TestCategory("LogicalTree")]
        [TestMethod]
        public async Task Test_LogicalTree_FindChild_NoVisualTree_Exists()
        {
            await App.DispatcherQueue.EnqueueAsync(() =>
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

                // Main Test
                var textBlock = treeRoot.FindChild<TextBlock>();

                Assert.IsNotNull(textBlock, "Expected to find something.");
                Assert.IsInstanceOfType(textBlock, typeof(TextBlock), "Didn't find expected typed element.");
            });
        }

        [TestCategory("LogicalTree")]
        [TestMethod]
        public async Task Test_LogicalTree_FindChild_ItemsControl_Exists()
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
                var textBlock = treeRoot.FindChild<TextBlock>();

                Assert.IsNotNull(textBlock, "Expected to find something.");
                Assert.IsInstanceOfType(textBlock, typeof(TextBlock), "Didn't find expected typed element.");
            });
        }

        [TestCategory("LogicalTree")]
        [TestMethod]
        public async Task Test_LogicalTree_FindChild_NotFound()
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
                var uniformGrid = treeRoot.FindChild<UniformGrid>();

                Assert.IsNull(uniformGrid, "Didn't expect to find anything.");
            });
        }

        [TestCategory("LogicalTree")]
        [TestMethod]
        public async Task Test_LogicalTree_FindChildren_Exists()
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
                <TextBox/>
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
                var textBlocks = treeRoot.FindChildren().OfType<TextBlock>();

                Assert.IsNotNull(textBlocks, "Expected to find something.");

                var array = textBlocks.ToArray();

                Assert.AreEqual(3, array.Length, "Expected to find 3 TextBlock elements.");

                // I don't think we want to guarantee order here, so just care that we can find each one.
                Assert.IsTrue(array.Any((tb) => tb.Name == "One"), "Couldn't find TextBlock 'One'");
                Assert.IsTrue(array.Any((tb) => tb.Name == "Two"), "Couldn't find TextBlock 'Two'");
                Assert.IsTrue(array.Any((tb) => tb.Name == "Three"), "Couldn't find TextBlock 'Three'");
            });
        }

        [TestCategory("LogicalTree")]
        [TestMethod]
        public async Task Test_LogicalTree_FindChildren_NotFound()
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
                var thing = treeRoot.FindChildren().OfType<UniformGrid>();

                Assert.IsNotNull(thing, "Expected to still have enumerable.");

                var array = thing.ToArray();

                Assert.AreEqual(0, array.Length, "Expected to have 0 elements.");
            });
        }

        [TestCategory("LogicalTree")]
        [TestMethod]
        public async Task Test_LogicalTree_FindParent_Exists()
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
                var grid = startingPoint.FindParent<Grid>();

                Assert.IsNotNull(grid, "Expected to find Grid");
                Assert.AreEqual(targetGrid, grid, "Grid didn't match expected.");
            });
        }

        [TestCategory("LogicalTree")]
        [TestMethod]
        public async Task Test_LogicalTree_FindParent_NotFound()
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
                var grid = startingPoint.FindParent<TextBlock>();

                Assert.IsNull(grid, "Didn't expect to find anything.");
            });
        }

        [TestCategory("LogicalTree")]
        [TestMethod]
        public async Task Test_LogicalTree_FindParentByName_Exists()
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
                var grid = startingPoint.FindParent("TargetElement");

                Assert.IsNotNull(grid, "Expected to find Grid");
                Assert.AreEqual(targetGrid, grid, "Grid didn't match expected.");
            });
        }

        [TestCategory("LogicalTree")]
        [TestMethod]
        public async Task Test_LogicalTree_FindParentByName_NotFound()
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
                var grid = startingPoint.FindParent("TargetElement");

                Assert.IsNull(grid, "Didn't expect to find anything.");
            });
        }
    }
}