// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace UnitTests.UI.Controls
{
    [TestClass]
    public class Test_UniformGrid_Dimensions
    {
        [TestCategory("UniformGrid")]
        [UITestMethod]
        public void Test_UniformGrid_GetDimensions_NoElements()
        {
            var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls"">
    <controls:UniformGrid x:Name=""UniformGrid"">
    </controls:UniformGrid>
</Page>") as FrameworkElement;

            Assert.IsNotNull(treeRoot, "Could not load XAML tree.");

            var grid = treeRoot.FindChild("UniformGrid") as UniformGrid;

            Assert.IsNotNull(grid, "Could not find UniformGrid in tree.");

            var children = grid.Children.Select(item => item as FrameworkElement).ToArray();

            Assert.AreEqual(0, grid.Children.Count());

            var (rows, columns) = UniformGrid.GetDimensions(children, 0, 0, 0);

            Assert.AreEqual(1, rows);
            Assert.AreEqual(1, columns);
        }

        [TestCategory("UniformGrid")]
        [UITestMethod]
        public void Test_UniformGrid_GetDimensions_AllVisible()
        {
            var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls"">
    <controls:UniformGrid x:Name=""UniformGrid"">
        <Border/>
        <Border/>
        <Border/>
        <Border/>
        <Border/>
        <Border/>
        <Border/>
        <Border/>
    </controls:UniformGrid>
</Page>") as FrameworkElement;

            Assert.IsNotNull(treeRoot, "Could not load XAML tree.");

            var grid = treeRoot.FindChild("UniformGrid") as UniformGrid;

            Assert.IsNotNull(grid, "Could not find UniformGrid in tree.");

            var children = grid.Children.Select(item => item as FrameworkElement).ToArray();

            Assert.AreEqual(8, grid.Children.Count());

            var (rows, columns) = UniformGrid.GetDimensions(children, 0, 0, 0);
            
            Assert.AreEqual(3, rows);
            Assert.AreEqual(3, columns);
        }

        [TestCategory("UniformGrid")]
        [UITestMethod]
        public void Test_UniformGrid_GetDimensions_SomeVisible()
        {
            var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls"">
    <controls:UniformGrid x:Name=""UniformGrid"">
        <Border/>
        <Border Visibility=""Collapsed""/>
        <Border/>
        <Border Visibility=""Collapsed""/>
        <Border/>
        <Border Visibility=""Collapsed""/>
        <Border/>
        <Border Visibility=""Collapsed""/>
    </controls:UniformGrid>
</Page>") as FrameworkElement;

            Assert.IsNotNull(treeRoot, "Could not load XAML tree.");

            var grid = treeRoot.FindChild("UniformGrid") as UniformGrid;

            Assert.IsNotNull(grid, "Could not find UniformGrid in tree.");

            var children = grid.Children.Select(item => item as FrameworkElement);

            Assert.AreEqual(8, grid.Children.Count());

            // TODO: We don't expose this piece of the UniformGrid, but want to test this here for now.
            var visible = grid.Children.Where(item => item.Visibility != Visibility.Collapsed && item is FrameworkElement).Select(item => item as FrameworkElement).ToArray();

            Assert.AreEqual(4, visible.Count());

            var (rows, columns) = UniformGrid.GetDimensions(visible, 0, 0, 0);

            Assert.AreEqual(2, rows);
            Assert.AreEqual(2, columns);
        }

        [TestCategory("UniformGrid")]
        [UITestMethod]
        public void Test_UniformGrid_GetDimensions_FirstColumn()
        {
            var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls"">
    <controls:UniformGrid x:Name=""UniformGrid"">
        <Border/>
        <Border/>
        <Border/>
        <Border/>
        <Border/>
        <Border/>
        <Border/>
        <Border/>
    </controls:UniformGrid>
</Page>") as FrameworkElement;

            Assert.IsNotNull(treeRoot, "Could not load XAML tree.");

            var grid = treeRoot.FindChild("UniformGrid") as UniformGrid;

            Assert.IsNotNull(grid, "Could not find UniformGrid in tree.");

            var children = grid.Children.Select(item => item as FrameworkElement).ToArray();

            Assert.AreEqual(8, grid.Children.Count());

            var (rows, columns) = UniformGrid.GetDimensions(children, 0, 0, 2);

            Assert.AreEqual(4, rows);
            Assert.AreEqual(4, columns);
        }

        [TestCategory("UniformGrid")]
        [UITestMethod]
        public void Test_UniformGrid_GetDimensions_ElementLarger()
        {
            var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls"">
    <controls:UniformGrid x:Name=""UniformGrid"">
        <Border/>
        <Border/>
        <Border Grid.RowSpan=""3"" Grid.ColumnSpan=""2""/>
        <Border/>
        <Border/>
        <Border/>
        <Border/>
        <Border/>
    </controls:UniformGrid>
</Page>") as FrameworkElement;

            Assert.IsNotNull(treeRoot, "Could not load XAML tree.");

            var grid = treeRoot.FindChild("UniformGrid") as UniformGrid;

            Assert.IsNotNull(grid, "Could not find UniformGrid in tree.");

            var children = grid.Children.Select(item => item as FrameworkElement).ToArray();

            Assert.AreEqual(8, grid.Children.Count());

            var (rows, columns) = UniformGrid.GetDimensions(children, 0, 0, 0);

            Assert.AreEqual(4, rows);
            Assert.AreEqual(4, columns);
        }

        [TestCategory("UniformGrid")]
        [UITestMethod]
        public void Test_UniformGrid_GetDimensions_FirstColumnEqualsColumns()
        {
            var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls"">
    <controls:UniformGrid x:Name=""UniformGrid"">
        <Border/>
        <Border/>
        <Border/>
        <Border/>
        <Border/>
        <Border/>
        <Border/>
    </controls:UniformGrid>
</Page>") as FrameworkElement;

            Assert.IsNotNull(treeRoot, "Could not load XAML tree.");

            var grid = treeRoot.FindChild("UniformGrid") as UniformGrid;

            Assert.IsNotNull(grid, "Could not find UniformGrid in tree.");

            var children = grid.Children.Select(item => item as FrameworkElement).ToArray();

            Assert.AreEqual(7, grid.Children.Count());
            
            // columns == first column
            // In WPF, First Column is ignored and we have a 1x7 layout.
            var (rows, columns) = UniformGrid.GetDimensions(children, 0, 7, 7);

            Assert.AreEqual(1, rows, "Expected single row.");
            Assert.AreEqual(7, columns, "Expected seven columns.");
        }
    }
}