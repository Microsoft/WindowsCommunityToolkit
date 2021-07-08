// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System.Linq;

namespace UnitTests.UI.Controls
{
    [TestClass]
    public class Test_ListDetailsView
    {
        [TestCategory("ListDetailsView")]
        [UITestMethod]
        public void Test_SelectedIndex_Default()
        {
            var items = Enumerable.Range(1, 10).ToArray();
            var listDetailsView = new ListDetailsView();
            listDetailsView.ItemsSource = items;
            Assert.AreEqual(-1, listDetailsView.SelectedIndex);
        }

        [TestCategory("ListDetailsView")]
        [UITestMethod]
        public void Test_SelectedItem_Default()
        {
            var items = Enumerable.Range(1, 10).ToArray();
            var listDetailsView = new ListDetailsView();
            listDetailsView.ItemsSource = items;
            Assert.IsNull(listDetailsView.SelectedItem);
        }

        [TestCategory("ListDetailsView")]
        [UITestMethod]
        public void Test_SelectedIndex_Syncs_SelectedItem()
        {
            var items = Enumerable.Range(1, 10).ToArray();
            var listDetailsView = new ListDetailsView();
            listDetailsView.ItemsSource = items;
            listDetailsView.SelectedIndex = 6;
            Assert.AreEqual(items[6], listDetailsView.SelectedItem);
        }

        [TestCategory("ListDetailsView")]
        [UITestMethod]
        public void Test_UnselectUsingIndex()
        {
            var items = Enumerable.Range(1, 10).ToArray();
            var listDetailsView = new ListDetailsView();
            listDetailsView.ItemsSource = items;
            listDetailsView.SelectedIndex = 5;
            listDetailsView.SelectedIndex = -1;
            Assert.IsNull(listDetailsView.SelectedItem);
        }

        [TestCategory("ListDetailsView")]
        [UITestMethod]
        public void Test_UnselectUsingItem()
        {
            var items = Enumerable.Range(1, 10).ToArray();
            var listDetailsView = new ListDetailsView();
            listDetailsView.ItemsSource = items;
            listDetailsView.SelectedItem = items[5];
            listDetailsView.SelectedItem = null;
            Assert.AreEqual(-1, listDetailsView.SelectedIndex);
        }

        [TestCategory("ListDetailsView")]
        [UITestMethod]
        public void Test_SelectedItem_Syncs_SelectedIndex()
        {
            var items = Enumerable.Range(0, 10).ToArray();
            var listDetailsView = new ListDetailsView();
            listDetailsView.ItemsSource = items;
            listDetailsView.SelectedItem = items[3];
            Assert.AreEqual(3, listDetailsView.SelectedIndex);
        }
    }
}