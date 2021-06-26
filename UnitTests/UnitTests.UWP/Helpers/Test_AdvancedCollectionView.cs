// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UnitTests.Helpers
{
    [TestClass]
    public class Test_AdvancedCollectionView
    {
        private class SampleClass : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private int val;

            public int Val
            {
                get
                {
                    return val;
                }

                set
                {
                    val = value;
                    OnPropertyChanged();
                }
            }

            public int GetPropertyChangedEventHandlerSubscriberLength()
            {
                return PropertyChanged is null ? 0 : PropertyChanged.GetInvocationList().Length;
            }

            public SampleClass(int val)
            {
                this.Val = val;
            }

            private void OnPropertyChanged([CallerMemberName] string name = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        [TestCategory("Helpers")]
        [UITestMethod]
        public void Test_SourceNcc_CollectionChanged_Add()
        {
            // Create ref list with all test items:
            List<SampleClass> refList = new List<SampleClass>();
            for (int e = 0; e < 100; e++)
            {
                refList.Add(new SampleClass(e));
            }

            ObservableCollection<SampleClass> col = new ObservableCollection<SampleClass>();
            AdvancedCollectionView acv = new AdvancedCollectionView(col, true);

            // Add all items to collection while DeferRefresh() is active:
            using (acv.DeferRefresh())
            {
                foreach (var item in refList)
                {
                    col.Add(item);
                }
            }

            // Check if subscribed to all items:
            foreach (var item in refList)
            {
                Assert.IsTrue(item.GetPropertyChangedEventHandlerSubscriberLength() == 1);
            }
        }

        [TestCategory("Helpers")]
        [UITestMethod]
        public void Test_SourceNcc_CollectionChanged_Remove()
        {
            // Create ref list with all test items:
            List<SampleClass> refList = new List<SampleClass>();
            for (int e = 0; e < 100; e++)
            {
                refList.Add(new SampleClass(e));
            }

            ObservableCollection<SampleClass> col = new ObservableCollection<SampleClass>();
            AdvancedCollectionView acv = new AdvancedCollectionView(col, true);

            // Add all items to collection:
            foreach (var item in refList)
            {
                col.Add(item);
            }

            // Remove all items from collection while DeferRefresh() is active:
            using (acv.DeferRefresh())
            {
                while (col.Count > 0)
                {
                    col.RemoveAt(0);
                }
            }

            // Check if unsubscribed from all items:
            foreach (var item in refList)
            {
                Assert.IsTrue(item.GetPropertyChangedEventHandlerSubscriberLength() == 0);
            }
        }
    }
}