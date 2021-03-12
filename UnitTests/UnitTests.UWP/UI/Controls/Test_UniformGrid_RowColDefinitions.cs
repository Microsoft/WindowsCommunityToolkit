// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace UnitTests.UI.Controls
{
    [TestClass]
    public class Test_UniformGrid_RowColDefinitions
    {
        [TestCategory("UniformGrid")]
        [UITestMethod]
        public void Test_UniformGrid_SetupRowDefinitions_AllAutomatic()
        {
            var treeroot = XamlReader.Load(@"<Page
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

            Assert.IsNotNull(treeroot, "Could not load XAML tree.");

            var grid = treeroot.FindChild("UniformGrid") as UniformGrid;

            Assert.IsNotNull(grid, "Could not find UniformGrid in tree.");

            // Normal Grid's don't have any definitions to start
            Assert.AreEqual(0, grid.RowDefinitions.Count);

            //// DO IT AGAIN
            //// This is so we can check that it will behave the same again on another pass.

            // We'll have three rows in this setup
            grid.SetupRowDefinitions(3);

            // We should now have our rows created
            Assert.AreEqual(3, grid.RowDefinitions.Count);

            for (int i = 0; i < grid.RowDefinitions.Count; i++)
            {
                var rd = grid.RowDefinitions[i];

                // Check if we've setup our row to automatically layout
                Assert.AreEqual(true, UniformGrid.GetAutoLayout(rd));

                // We need to be using '*' layout for all our rows to be even.
                Assert.AreEqual(GridUnitType.Star, rd.Height.GridUnitType);

                Assert.AreEqual(1.0, rd.Height.Value);
            }

            // We'll have three rows in this setup
            grid.SetupRowDefinitions(3);

            // We should now have our rows created
            Assert.AreEqual(3, grid.RowDefinitions.Count);

            for (int i = 0; i < grid.RowDefinitions.Count; i++)
            {
                var rd = grid.RowDefinitions[i];

                // Check if we've setup our row to automatically layout
                Assert.AreEqual(true, UniformGrid.GetAutoLayout(rd));

                // We need to be using '*' layout for all our rows to be even.
                Assert.AreEqual(GridUnitType.Star, rd.Height.GridUnitType);

                Assert.AreEqual(1.0, rd.Height.Value);
            }
        }

        [TestCategory("UniformGrid")]
        [UITestMethod]
        public void Test_UniformGrid_SetupRowDefinitions_FirstFixed()
        {
            var treeroot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls"">
    <controls:UniformGrid x:Name=""UniformGrid"">
        <controls:UniformGrid.RowDefinitions>
            <RowDefinition Height=""48""/>
        </controls:UniformGrid.RowDefinitions>
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

            Assert.IsNotNull(treeroot, "Could not load XAML tree.");

            var grid = treeroot.FindChild("UniformGrid") as UniformGrid;

            Assert.IsNotNull(grid, "Could not find UniformGrid in tree.");

            // We should find our first definition
            Assert.AreEqual(1, grid.RowDefinitions.Count);
            Assert.AreEqual(48, grid.RowDefinitions[0].Height.Value);

            // We'll have three rows in this setup
            grid.SetupRowDefinitions(3);

            // We should now have our rows created
            Assert.AreEqual(3, grid.RowDefinitions.Count);

            var rdo = grid.RowDefinitions[0];

            // Did we mark that our row is special?
            Assert.AreEqual(false, UniformGrid.GetAutoLayout(rdo));

            Assert.AreNotEqual(GridUnitType.Star, rdo.Height.GridUnitType);

            Assert.AreNotEqual(1.0, rdo.Height.Value);

            // Check that we filled in the other two.
            for (int i = 1; i < grid.RowDefinitions.Count; i++)
            {
                var rd = grid.RowDefinitions[i];

                // Check if we've setup our row to automatically layout
                Assert.AreEqual(true, UniformGrid.GetAutoLayout(rd));

                // We need to be using '*' layout for all our rows to be even.
                Assert.AreEqual(GridUnitType.Star, rd.Height.GridUnitType);

                Assert.AreEqual(1.0, rd.Height.Value);
            }

            //// DO IT AGAIN
            //// This is so we can check that it will behave the same again on another pass.

            // We'll have three rows in this setup
            grid.SetupRowDefinitions(3);

            // We should now have our rows created
            Assert.AreEqual(3, grid.RowDefinitions.Count);

            rdo = grid.RowDefinitions[0];

            // Did we mark that our row is special?
            Assert.AreEqual(false, UniformGrid.GetAutoLayout(rdo));

            Assert.AreNotEqual(GridUnitType.Star, rdo.Height.GridUnitType);

            Assert.AreNotEqual(1.0, rdo.Height.Value);

            // Check that we filled in the other two.
            for (int i = 1; i < grid.RowDefinitions.Count; i++)
            {
                var rd = grid.RowDefinitions[i];

                // Check if we've setup our row to automatically layout
                Assert.AreEqual(true, UniformGrid.GetAutoLayout(rd));

                // We need to be using '*' layout for all our rows to be even.
                Assert.AreEqual(GridUnitType.Star, rd.Height.GridUnitType);

                Assert.AreEqual(1.0, rd.Height.Value);
            }
        }

        [TestCategory("UniformGrid")]
        [UITestMethod]
        public void Test_UniformGrid_SetupRowDefinitions_MiddleFixed()
        {
            var treeroot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls"">
    <controls:UniformGrid x:Name=""UniformGrid"" Rows=""5"">
        <controls:UniformGrid.RowDefinitions>
            <RowDefinition Height=""48""/>
        </controls:UniformGrid.RowDefinitions>
        <Border/>
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

            Assert.IsNotNull(treeroot, "Could not load XAML tree.");

            var grid = treeroot.FindChild("UniformGrid") as UniformGrid;

            Assert.IsNotNull(grid, "Could not find UniformGrid in tree.");

            // We should find our first definition
            Assert.AreEqual(1, grid.RowDefinitions.Count, "Expected to find our row definition.");
            Assert.AreEqual(48, grid.RowDefinitions[0].Height.Value);

            Assert.AreEqual(5, grid.Rows, "Rows not set to 5");

            // We'll have five rows in this setup
            grid.SetupRowDefinitions(5);

            // We should now have our rows created
            Assert.AreEqual(5, grid.RowDefinitions.Count, "5 RowDefinitions weren't created.");

            // Our original definition should be at index 2
            var rdo = grid.RowDefinitions[0];

            // Did we mark that our row is special?
            Assert.AreEqual(false, UniformGrid.GetAutoLayout(rdo));

            Assert.AreNotEqual(GridUnitType.Star, rdo.Height.GridUnitType);

            Assert.AreNotEqual(1.0, rdo.Height.Value);

            // Check that we filled in the other two.
            for (int i = 1; i < grid.RowDefinitions.Count; i++)
            {
                var rd = grid.RowDefinitions[i];

                // Check if we've setup our row to automatically layout
                Assert.AreEqual(true, UniformGrid.GetAutoLayout(rd));

                // We need to be using '*' layout for all our rows to be even.
                Assert.AreEqual(GridUnitType.Star, rd.Height.GridUnitType);

                Assert.AreEqual(1.0, rd.Height.Value);
            }

            //// DO IT AGAIN
            //// This is so we can check that it will behave the same again on another pass.

            // We'll have five rows in this setup
            grid.SetupRowDefinitions(5);

            // We should now have our rows created
            Assert.AreEqual(5, grid.RowDefinitions.Count);

            // Our original definition should be at index 2
            rdo = grid.RowDefinitions[0];

            // Did we mark that our row is special?
            Assert.AreEqual(false, UniformGrid.GetAutoLayout(rdo));

            Assert.AreNotEqual(GridUnitType.Star, rdo.Height.GridUnitType);

            Assert.AreNotEqual(1.0, rdo.Height.Value);

            // Check that we filled in the other two.
            for (int i = 1; i < grid.RowDefinitions.Count; i++)
            {
                var rd = grid.RowDefinitions[i];

                // Check if we've setup our row to automatically layout
                Assert.AreEqual(true, UniformGrid.GetAutoLayout(rd));

                // We need to be using '*' layout for all our rows to be even.
                Assert.AreEqual(GridUnitType.Star, rd.Height.GridUnitType);

                Assert.AreEqual(1.0, rd.Height.Value);
            }
        }

        [TestCategory("UniformGrid")]
        [UITestMethod]
        public void Test_UniformGrid_SetupRowDefinitions_MiddleAndEndFixed()
        {
            var treeroot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls"">
    <controls:UniformGrid x:Name=""UniformGrid"" Rows=""5"">
        <controls:UniformGrid.RowDefinitions>
            <RowDefinition/>
            <RowDefinition/>
            <RowDefinition Height=""48""/>
            <RowDefinition/>
            <RowDefinition Height=""128""/>
        </controls:UniformGrid.RowDefinitions>
        <Border/>
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

            Assert.IsNotNull(treeroot, "Could not load XAML tree.");

            var grid = treeroot.FindChild("UniformGrid") as UniformGrid;

            Assert.IsNotNull(grid, "Could not find UniformGrid in tree.");

            // We should find our first definition
            Assert.AreEqual(5, grid.RowDefinitions.Count, "Expected to find two row definitions.");
            Assert.AreEqual(48, grid.RowDefinitions[2].Height.Value);
            Assert.AreEqual(128, grid.RowDefinitions[4].Height.Value);

            Assert.AreEqual(5, grid.Rows, "Rows not set to 5");

            // We'll have five rows in this setup
            grid.SetupRowDefinitions(5);

            // We should now have our rows created
            Assert.AreEqual(5, grid.RowDefinitions.Count, "5 RowDefinitions weren't created.");

            // Our original definition should be at index 2
            var rdo = grid.RowDefinitions[2];

            // Did we mark that our row is special?
            Assert.AreEqual(false, UniformGrid.GetAutoLayout(rdo));

            Assert.AreNotEqual(GridUnitType.Star, rdo.Height.GridUnitType);

            Assert.AreNotEqual(1.0, rdo.Height.Value);

            // Our 2nd original definition should be at index 4
            var rdo2 = grid.RowDefinitions[4];

            // Did we mark that our row is special?
            Assert.AreEqual(false, UniformGrid.GetAutoLayout(rdo2));

            Assert.AreNotEqual(GridUnitType.Star, rdo2.Height.GridUnitType);

            Assert.AreNotEqual(1.0, rdo2.Height.Value);

            // Check that we filled in the other two.
            for (int i = 0; i < grid.RowDefinitions.Count; i++)
            {
                if (i == 2 || i == 4)
                {
                    continue;
                }

                var rd = grid.RowDefinitions[i];

                // Check if we've setup our row to automatically layout
                Assert.AreEqual(false, UniformGrid.GetAutoLayout(rd));

                // We need to be using '*' layout for all our rows to be even.
                Assert.AreEqual(GridUnitType.Star, rd.Height.GridUnitType);

                Assert.AreEqual(1.0, rd.Height.Value);
            }

            //// DO IT AGAIN
            //// This is so we can check that it will behave the same again on another pass.

            // We'll have five rows in this setup
            grid.SetupRowDefinitions(5);

            // We should now have our rows created
            Assert.AreEqual(5, grid.RowDefinitions.Count, "5 RowDefinitions weren't created.");

            // Our original definition should be at index 2
            rdo = grid.RowDefinitions[2];

            // Did we mark that our row is special?
            Assert.AreEqual(false, UniformGrid.GetAutoLayout(rdo));

            Assert.AreNotEqual(GridUnitType.Star, rdo.Height.GridUnitType);

            Assert.AreNotEqual(1.0, rdo.Height.Value);

            // Our 2nd original definition should be at index 4
            rdo2 = grid.RowDefinitions[4];

            // Did we mark that our row is special?
            Assert.AreEqual(false, UniformGrid.GetAutoLayout(rdo2));

            Assert.AreNotEqual(GridUnitType.Star, rdo2.Height.GridUnitType);

            Assert.AreNotEqual(1.0, rdo2.Height.Value);

            // Check that we filled in the other two.
            for (int i = 0; i < grid.RowDefinitions.Count; i++)
            {
                if (i == 2 || i == 4)
                {
                    continue;
                }

                var rd = grid.RowDefinitions[i];

                // Check if we've setup our row to automatically layout
                Assert.AreEqual(false, UniformGrid.GetAutoLayout(rd));

                // We need to be using '*' layout for all our rows to be even.
                Assert.AreEqual(GridUnitType.Star, rd.Height.GridUnitType);

                Assert.AreEqual(1.0, rd.Height.Value);
            }
        }

        [TestCategory("UniformGrid")]
        [UITestMethod]
        public void Test_UniformGrid_SetupColumnDefinitions_AllAutomatic()
        {
            var treeroot = XamlReader.Load(@"<Page
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

            Assert.IsNotNull(treeroot, "Could not load XAML tree.");

            var grid = treeroot.FindChild("UniformGrid") as UniformGrid;

            Assert.IsNotNull(grid, "Could not find UniformGrid in tree.");

            // Normal Grid's don't have any definitions to start
            Assert.AreEqual(0, grid.ColumnDefinitions.Count);

            // We'll have three columns in this setup
            grid.SetupColumnDefinitions(3);

            // We should now have our columns created
            Assert.AreEqual(3, grid.ColumnDefinitions.Count);

            for (int i = 0; i < grid.ColumnDefinitions.Count; i++)
            {
                var cd = grid.ColumnDefinitions[i];

                // Check if we've setup our row to automatically layout
                Assert.AreEqual(true, UniformGrid.GetAutoLayout(cd));

                // We need to be using '*' layout for all our rows to be even.
                Assert.AreEqual(GridUnitType.Star, cd.Width.GridUnitType);

                Assert.AreEqual(1.0, cd.Width.Value);
            }

            //// DO IT AGAIN
            //// This is so we can check that it will behave the same again on another pass.

            // We'll have three columns in this setup
            grid.SetupColumnDefinitions(3);

            // We should now have our columns created
            Assert.AreEqual(3, grid.ColumnDefinitions.Count);

            for (int i = 0; i < grid.ColumnDefinitions.Count; i++)
            {
                var cd = grid.ColumnDefinitions[i];

                // Check if we've setup our row to automatically layout
                Assert.AreEqual(true, UniformGrid.GetAutoLayout(cd));

                // We need to be using '*' layout for all our rows to be even.
                Assert.AreEqual(GridUnitType.Star, cd.Width.GridUnitType);

                Assert.AreEqual(1.0, cd.Width.Value);
            }
        }

        [TestCategory("UniformGrid")]
        [UITestMethod]
        public void Test_UniformGrid_SetupColumnDefinitions_FirstFixed()
        {
            var treeroot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls"">
    <controls:UniformGrid x:Name=""UniformGrid"">
        <controls:UniformGrid.ColumnDefinitions>
            <ColumnDefinition Width=""48""/>
        </controls:UniformGrid.ColumnDefinitions>
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

            Assert.IsNotNull(treeroot, "Could not load XAML tree.");

            var grid = treeroot.FindChild("UniformGrid") as UniformGrid;

            Assert.IsNotNull(grid, "Could not find UniformGrid in tree.");

            // We should find our first definition
            Assert.AreEqual(1, grid.ColumnDefinitions.Count);
            Assert.AreEqual(48, grid.ColumnDefinitions[0].Width.Value);

            // We'll have three columns in this setup
            grid.SetupColumnDefinitions(3);

            // We should now have our columns created
            Assert.AreEqual(3, grid.ColumnDefinitions.Count);

            var cdo = grid.ColumnDefinitions[0];

            // Did we mark that our column is special?
            Assert.AreEqual(false, UniformGrid.GetAutoLayout(cdo));

            Assert.AreNotEqual(GridUnitType.Star, cdo.Width.GridUnitType);

            Assert.AreNotEqual(1.0, cdo.Width.Value);

            // Check that we filled in the other two.
            for (int i = 1; i < grid.ColumnDefinitions.Count; i++)
            {
                var cd = grid.ColumnDefinitions[i];

                // Check if we've setup our row to automatically layout
                Assert.AreEqual(true, UniformGrid.GetAutoLayout(cd));

                // We need to be using '*' layout for all our rows to be even.
                Assert.AreEqual(GridUnitType.Star, cd.Width.GridUnitType);

                Assert.AreEqual(1.0, cd.Width.Value);
            }

            //// DO IT AGAIN
            //// This is so we can check that it will behave the same again on another pass.

            // We'll have three columns in this setup
            grid.SetupColumnDefinitions(3);

            // We should now have our columns created
            Assert.AreEqual(3, grid.ColumnDefinitions.Count);

            cdo = grid.ColumnDefinitions[0];

            // Did we mark that our column is special?
            Assert.AreEqual(false, UniformGrid.GetAutoLayout(cdo));

            Assert.AreNotEqual(GridUnitType.Star, cdo.Width.GridUnitType);

            Assert.AreNotEqual(1.0, cdo.Width.Value);

            // Check that we filled in the other two.
            for (int i = 1; i < grid.ColumnDefinitions.Count; i++)
            {
                var cd = grid.ColumnDefinitions[i];

                // Check if we've setup our row to automatically layout
                Assert.AreEqual(true, UniformGrid.GetAutoLayout(cd));

                // We need to be using '*' layout for all our rows to be even.
                Assert.AreEqual(GridUnitType.Star, cd.Width.GridUnitType);

                Assert.AreEqual(1.0, cd.Width.Value);
            }
        }

        [TestCategory("UniformGrid")]
        [UITestMethod]
        public void Test_UniformGrid_SetupColumnDefinitions_MiddleFixed()
        {
            var treeroot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls"">
    <controls:UniformGrid x:Name=""UniformGrid"" Columns=""5"">
        <controls:UniformGrid.ColumnDefinitions>
            <ColumnDefinition/>
            <ColumnDefinition/>
            <ColumnDefinition Width=""48""/>
        </controls:UniformGrid.ColumnDefinitions>
        <Border/>
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

            Assert.IsNotNull(treeroot, "Could not load XAML tree.");

            var grid = treeroot.FindChild("UniformGrid") as UniformGrid;

            Assert.IsNotNull(grid, "Could not find UniformGrid in tree.");

            // We should find our first definition
            Assert.AreEqual(3, grid.ColumnDefinitions.Count, "Expected to find our Column definition.");
            Assert.AreEqual(48, grid.ColumnDefinitions[2].Width.Value);

            Assert.AreEqual(5, grid.Columns, "Columns not set to 5");

            // We'll have five Columns in this setup
            grid.SetupColumnDefinitions(5);

            // We should now have our Columns created
            Assert.AreEqual(5, grid.ColumnDefinitions.Count, "5 ColumnDefinitions weren't created.");

            // Our original definition should be at index 2
            var rdo = grid.ColumnDefinitions[2];

            // Did we mark that our Column is special?
            Assert.AreEqual(false, UniformGrid.GetAutoLayout(rdo), "AutoLayout of 48 Width should be False.");

            Assert.AreNotEqual(GridUnitType.Star, rdo.Width.GridUnitType);

            Assert.AreNotEqual(1.0, rdo.Width.Value);

            // Check that we filled in the other two.
            for (int i = 3; i < grid.ColumnDefinitions.Count; i++)
            {
                var rd = grid.ColumnDefinitions[i];

                // Check if we've setup our Column to automatically layout
                Assert.AreEqual(true, UniformGrid.GetAutoLayout(rd), "AutoLayout should be true. {0}", i);

                // We need to be using '*' layout for all our Columns to be even.
                Assert.AreEqual(GridUnitType.Star, rd.Width.GridUnitType);

                Assert.AreEqual(1.0, rd.Width.Value);
            }

            //// DO IT AGAIN
            //// This is so we can check that it will behave the same again on another pass.

            // We'll have five Columns in this setup
            grid.SetupColumnDefinitions(5);

            // We should now have our Columns created
            Assert.AreEqual(5, grid.ColumnDefinitions.Count);

            // Our original definition should be at index 2
            rdo = grid.ColumnDefinitions[2];

            // Did we mark that our Column is special?
            Assert.AreEqual(false, UniformGrid.GetAutoLayout(rdo), "AutoLayout Width 48 should be false 2nd time.");

            Assert.AreNotEqual(GridUnitType.Star, rdo.Width.GridUnitType);

            Assert.AreNotEqual(1.0, rdo.Width.Value);

            // Check that we filled in the other two.
            for (int i = 3; i < grid.ColumnDefinitions.Count; i++)
            {
                var rd = grid.ColumnDefinitions[i];

                // Check if we've setup our Column to automatically layout
                Assert.AreEqual(true, UniformGrid.GetAutoLayout(rd), "AutoLayout should be true 2nd time. {0}", i);

                // We need to be using '*' layout for all our Columns to be even.
                Assert.AreEqual(GridUnitType.Star, rd.Width.GridUnitType);

                Assert.AreEqual(1.0, rd.Width.Value);
            }
        }

        [TestCategory("UniformGrid")]
        [UITestMethod]
        public void Test_UniformGrid_SetupColumnDefinitions_FirstAndEndFixed()
        {
            var treeroot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls"">
    <controls:UniformGrid x:Name=""UniformGrid"" Columns=""5"">
        <controls:UniformGrid.ColumnDefinitions>
            <ColumnDefinition Width=""48""/>
            <ColumnDefinition/>
            <ColumnDefinition/>
            <ColumnDefinition/>
            <ColumnDefinition Width=""128""/>
        </controls:UniformGrid.ColumnDefinitions>
        <Border/>
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

            Assert.IsNotNull(treeroot, "Could not load XAML tree.");

            var grid = treeroot.FindChild("UniformGrid") as UniformGrid;

            Assert.IsNotNull(grid, "Could not find UniformGrid in tree.");

            // We should find our first definition
            Assert.AreEqual(5, grid.ColumnDefinitions.Count, "Expected to find two Column definitions.");
            Assert.AreEqual(48, grid.ColumnDefinitions[0].Width.Value);
            Assert.AreEqual(128, grid.ColumnDefinitions[4].Width.Value);

            Assert.AreEqual(5, grid.Columns, "Columns not set to 5");

            // We'll have five Columns in this setup
            grid.SetupColumnDefinitions(5);

            // We should now have our Columns created
            Assert.AreEqual(5, grid.ColumnDefinitions.Count, "5 ColumnDefinitions weren't created.");

            // Our original definition should be at index 2
            var rdo = grid.ColumnDefinitions[0];

            // Did we mark that our Column is special?
            Assert.AreEqual(false, UniformGrid.GetAutoLayout(rdo));

            Assert.AreNotEqual(GridUnitType.Star, rdo.Width.GridUnitType);

            Assert.AreNotEqual(1.0, rdo.Width.Value);

            // Our 2nd original definition should be at index 4
            var rdo2 = grid.ColumnDefinitions[4];

            // Did we mark that our Column is special?
            Assert.AreEqual(false, UniformGrid.GetAutoLayout(rdo2));
            ////Assert.AreEqual(4, UniformGrid.GetColumn(rdo2));

            Assert.AreNotEqual(GridUnitType.Star, rdo2.Width.GridUnitType);

            Assert.AreNotEqual(1.0, rdo2.Width.Value);

            // Check that we filled in the other two.
            for (int i = 1; i < grid.ColumnDefinitions.Count - 1; i++)
            {
                var rd = grid.ColumnDefinitions[i];

                // Check if we've setup our Column to automatically layout
                Assert.AreEqual(false, UniformGrid.GetAutoLayout(rd));

                // We need to be using '*' layout for all our Columns to be even.
                Assert.AreEqual(GridUnitType.Star, rd.Width.GridUnitType);

                Assert.AreEqual(1.0, rd.Width.Value);
            }

            //// DO IT AGAIN
            //// This is so we can check that it will behave the same again on another pass.

            // We'll have five Columns in this setup
            grid.SetupColumnDefinitions(5);

            // We should now have our Columns created
            Assert.AreEqual(5, grid.ColumnDefinitions.Count, "5 ColumnDefinitions weren't created.");

            // Our original definition should be at index 0
            rdo = grid.ColumnDefinitions[0];

            // Did we mark that our Column is special?
            Assert.AreEqual(false, UniformGrid.GetAutoLayout(rdo));

            Assert.AreNotEqual(GridUnitType.Star, rdo.Width.GridUnitType);

            Assert.AreNotEqual(1.0, rdo.Width.Value);

            // Our 2nd original definition should be at index 4
            rdo2 = grid.ColumnDefinitions[4];

            // Did we mark that our Column is special?
            Assert.AreEqual(false, UniformGrid.GetAutoLayout(rdo2));

            Assert.AreNotEqual(GridUnitType.Star, rdo2.Width.GridUnitType);

            Assert.AreNotEqual(1.0, rdo2.Width.Value);

            // Check that we filled in the other two.
            for (int i = 1; i < grid.ColumnDefinitions.Count - 1; i++)
            {
                var rd = grid.ColumnDefinitions[i];

                // Check if we've setup our Column to automatically layout
                Assert.AreEqual(false, UniformGrid.GetAutoLayout(rd));

                // We need to be using '*' layout for all our Columns to be even.
                Assert.AreEqual(GridUnitType.Star, rd.Width.GridUnitType);

                Assert.AreEqual(1.0, rd.Width.Value);
            }
        }
    }
}