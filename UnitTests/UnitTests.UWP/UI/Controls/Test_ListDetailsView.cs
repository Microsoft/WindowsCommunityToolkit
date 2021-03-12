// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;

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
            var listDetailsView = new ListDetailsView
            {
                ItemsSource = items
            };
            Assert.AreEqual(-1, listDetailsView.SelectedIndex);
        }

        [TestCategory("ListDetailsView")]
        [UITestMethod]
        public void Test_SelectedItem_Default()
        {
            var items = Enumerable.Range(1, 10).ToArray();
            var listDetailsView = new ListDetailsView
            {
                ItemsSource = items
            };
            Assert.IsNull(listDetailsView.SelectedItem);
        }

        [TestCategory("ListDetailsView")]
        [UITestMethod]
        public void Test_SelectedIndex_Syncs_SelectedItem()
        {
            var items = Enumerable.Range(1, 10).ToArray();
            var listDetailsView = new ListDetailsView
            {
                ItemsSource = items,
                SelectedIndex = 6
            };
            Assert.AreEqual(items[6], listDetailsView.SelectedItem);
        }

        [TestCategory("ListDetailsView")]
        [UITestMethod]
        public void Test_UnselectUsingIndex()
        {
            var items = Enumerable.Range(1, 10).ToArray();
            var listDetailsView = new ListDetailsView
            {
                ItemsSource = items,
                SelectedIndex = 5
            };
            listDetailsView.SelectedIndex = -1;
            Assert.IsNull(listDetailsView.SelectedItem);
        }

        [TestCategory("ListDetailsView")]
        [UITestMethod]
        public void Test_UnselectUsingItem()
        {
            var items = Enumerable.Range(1, 10).ToArray();
            var listDetailsView = new ListDetailsView
            {
                ItemsSource = items,
                SelectedItem = items[5]
            };
            listDetailsView.SelectedItem = null;
            Assert.AreEqual(-1, listDetailsView.SelectedIndex);
        }

        [TestCategory("ListDetailsView")]
        [UITestMethod]
        public void Test_SelectedItem_Syncs_SelectedIndex()
        {
            var items = Enumerable.Range(0, 10).ToArray();
            var listDetailsView = new ListDetailsView
            {
                ItemsSource = items,
                SelectedItem = items[3]
            };
            Assert.AreEqual(3, listDetailsView.SelectedIndex);
        }

        [TestCategory("ListDetailsView")]
        [UITestMethod]
        public void Test_Sorting_Keeps_SelectedIndex()
        {
            var items = Enumerable.Range(0, 10).ToArray();
            var listDetailsView = new ListDetailsView
            {
                ItemsSource = items,
                SelectedItem = items[3]
            };
            Assert.AreEqual(3, listDetailsView.SelectedIndex);
        }

        [TestCategory("ListDetailsView")]
        [UITestMethod]
        public void Test_Sorting_Keeps_SelectedItem()
        {
            var items = new ObservableCollection<int>(Enumerable.Range(0, 10));
            var listDetailsView = new ListDetailsView
            {
                ItemsSource = items,
                SelectedIndex = 3
            };
            var item = listDetailsView.SelectedItem;
            listDetailsView.ItemsSource = new ObservableCollection<int>(items.OrderByDescending(i => i));
            Assert.AreEqual(item, listDetailsView.SelectedItem);
            listDetailsView.ItemsSource = new ObservableCollection<int>(items.OrderBy(i => i));
            Assert.AreEqual(item, listDetailsView.SelectedItem);
        }

        [TestCategory("ListDetailsView")]
        [UITestMethod]
        public void Test_ItemsRemoved()
        {
            var items = new ObservableCollection<int>(Enumerable.Range(0, 10));
            var listDetailsView = new ListDetailsView
            {
                ItemsSource = items,
                SelectedIndex = 3
            };
            listDetailsView.ItemsSource = null;
            Assert.AreEqual(null, listDetailsView.SelectedItem);
            Assert.AreEqual(-1, listDetailsView.SelectedIndex);
        }
    }
}
