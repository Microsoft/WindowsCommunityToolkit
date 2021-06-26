// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace UnitTests.UI.Controls
{
    [TestClass]
    public class Test_TokenizingTextBox_InterspersedCollection
    {
        /// <summary>
        /// Tests the mapping of original indices in the collection to the projected outer collection.
        /// </summary>
        [TestMethod]
        public void TestIndexMappingOuter()
        {
            var originalSource = new List<object>(new object[] { 0, 2 });
            var ioc = new InterspersedObservableCollection(originalSource);
            var pioc = new PrivateObject(ioc); // TODO: Look into source generator helper for toolkit?

            ioc.Insert(1, "1");

            Assert.AreEqual(0, pioc.Invoke("ToOuterIndex", (object)0), "0 Index not at 0");
            Assert.AreEqual(2, pioc.Invoke("ToOuterIndex", (object)1), "1 Index not at 2");

            // Inserting new item at end shouldn't effect our result.
            ioc.Insert(3, "3");

            Assert.AreEqual(2, pioc.Invoke("ToOuterIndex", (object)1), "1 Index not at 2 (2)");
        }

        /// <summary>
        /// Tests the mapping of original indices in the collection to the projected outer collection.
        /// </summary>
        [TestMethod]
        public void TestIndexMappingOuterBanded()
        {
            var originalSource = new List<object>(new object[] { 0, 2, 4 });
            var ioc = new InterspersedObservableCollection(originalSource);
            var pioc = new PrivateObject(ioc); // TODO: Look into source generator helper for toolkit?

            ioc.Insert(1, "1");
            ioc.Insert(3, "3");
            ioc.Insert(5, "5");

            CollectionAssert.AreEqual(new object[] { 0, "1", 2, "3", 4, "5" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(0, pioc.Invoke("ToOuterIndex", (object)0), "0 Index not at 0");
            Assert.AreEqual(2, pioc.Invoke("ToOuterIndex", (object)1), "1 Index not at 2");
            Assert.AreEqual(4, pioc.Invoke("ToOuterIndex", (object)2), "2 Index not at 4");
        }

        /// <summary>
        /// Tests the mapping of original indices in the collection to the projected outer collection.
        /// </summary>
        [TestMethod]
        public void TestIndexMappingOuterBandedOpposite()
        {
            var originalSource = new List<object>(new object[] { 1, 3, 5 });
            var ioc = new InterspersedObservableCollection(originalSource);
            var pioc = new PrivateObject(ioc); // TODO: Look into source generator helper for toolkit?

            ioc.Insert(0, "0");
            ioc.Insert(2, "2");
            ioc.Insert(4, "4");

            CollectionAssert.AreEqual(new object[] { "0", 1, "2", 3, "4", 5 }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(1, pioc.Invoke("ToOuterIndex", (object)0), "0 Index not at 1");
            Assert.AreEqual(3, pioc.Invoke("ToOuterIndex", (object)1), "1 Index not at 3");
            Assert.AreEqual(5, pioc.Invoke("ToOuterIndex", (object)2), "2 Index not at 5");
        }

        /// <summary>
        /// Tests the mapping of original indices in the collection to the projected outer collection.
        /// </summary>
        [TestMethod]
        public void TestIndexMappingOuterDifferentOrder()
        {
            var originalSource = new List<object>(new object[] { 0, 2 });
            var ioc = new InterspersedObservableCollection(originalSource);
            var pioc = new PrivateObject(ioc); // TODO: Look into source generator helper for toolkit?

            ioc.Insert(2, "3");

            Assert.AreEqual(0, pioc.Invoke("ToOuterIndex", (object)0), "0 Index not at 0");
            Assert.AreEqual(1, pioc.Invoke("ToOuterIndex", (object)1), "1 Index not at 1");

            CollectionAssert.AreEqual(new object[] { 0, 2, "3" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            // Insert new earlier key
            ioc.Insert(1, "1");

            CollectionAssert.AreEqual(new object[] { 0, "1", 2, "3" }, ioc, string.Format("Collection not as expected, received {0} - (2)", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(0, pioc.Invoke("ToOuterIndex", (object)0), "0 Index not at 0 (2)");
            Assert.AreEqual(2, pioc.Invoke("ToOuterIndex", (object)1), "1 Index not at 2");
        }

#if DEBUG
        /// <summary>
        /// Tests the mapping of original indices in the collection to the projected outer collection.
        /// </summary>
        [TestMethod]
        public void TestIndexMappingOuterBounds()
        {
            var originalSource = new List<object>(new object[] { 0, 1 });
            var ioc = new InterspersedObservableCollection(originalSource);
            var pioc = new PrivateObject(ioc); // TODO: Look into source generator helper for toolkit?

            ioc.Insert(2, "2");

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => pioc.Invoke("ToOuterIndex", (object)-1), "-1 not throwing exception");
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => pioc.Invoke("ToOuterIndex", (object)2), "2 not throwing exception");

            // Inserting new item at end shouldn't effect our result.
            ioc.Insert(3, "3");

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => pioc.Invoke("ToOuterIndex", (object)2), "2 not throwing exception (2)");
        }
#endif

        /// <summary>
        /// Tests the mapping indices of the projected outer collection back to the inner source.
        /// </summary>
        [TestMethod]
        public void TestIndexMappingInner()
        {
            var originalSource = new List<object>(new object[] { 0, 2 });
            var ioc = new InterspersedObservableCollection(originalSource);
            var pioc = new PrivateObject(ioc);

            ioc.Insert(1, "1");

            Assert.AreEqual(0, pioc.Invoke("ToInnerIndex", (object)0), "0 Index not at 0");
            Assert.AreEqual(1, pioc.Invoke("ToInnerIndex", (object)2), "2 Index not at 1");

            // Inserting new item at end shouldn't effect our result.
            ioc.Insert(3, "3");

            Assert.AreEqual(1, pioc.Invoke("ToInnerIndex", (object)2), "2 Index not at 1 (2)");
        }

        /// <summary>
        /// Tests the mapping indices of the projected outer collection back to the inner source.
        /// </summary>
        [TestMethod]
        public void TestIndexMappingInnerDifferentOrder()
        {
            var originalSource = new List<object>(new object[] { 0, 2 });
            var ioc = new InterspersedObservableCollection(originalSource);
            var pioc = new PrivateObject(ioc);

            ioc.Insert(2, "3");

            Assert.AreEqual(0, pioc.Invoke("ToInnerIndex", (object)0), "0 Index not at 0");
            Assert.AreEqual(1, pioc.Invoke("ToInnerIndex", (object)1), "2 Index not at 1");

            CollectionAssert.AreEqual(new object[] { 0, 2, "3" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            // Inserting new item
            ioc.Insert(1, "1");

            CollectionAssert.AreEqual(new object[] { 0, "1", 2, "3" }, ioc, string.Format("Collection not as expected, received {0} - (2)", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(1, pioc.Invoke("ToInnerIndex", (object)2), "2 Index not at 1 (2)");
        }

#if DEBUG
        /// <summary>
        /// Tests the mapping indices of the projected outer collection back to the inner source.
        /// </summary>
        [TestMethod]
        public void TestIndexMappingInnerBounds()
        {
            var originalSource = new List<object>(new object[] { 0, 1 });
            var ioc = new InterspersedObservableCollection(originalSource);
            var pioc = new PrivateObject(ioc);

            ioc.Insert(2, "2");

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => pioc.Invoke("ToInnerIndex", (object)-1), "-1 should be out of bounds");
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => pioc.Invoke("ToInnerIndex", (object)3), "3 should be out of bounds");

            // Inserting new item at end shouldn't effect our result.
            ioc.Insert(3, "3");

            CollectionAssert.AreEqual(new object[] { 0, 1, "2", "3" }, ioc, string.Format("Collection not as expected, received {0} - (2)", ioc.ToArray().ToArrayString()));

            Assert.ThrowsException<ArgumentException>(() => pioc.Invoke("ToInnerIndex", (object)2), "2 should fail as can't map key");
            Assert.ThrowsException<ArgumentException>(() => pioc.Invoke("ToInnerIndex", (object)3), "4 should fail as can't map key");
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => pioc.Invoke("ToInnerIndex", (object)4), "4 should be out of bounds");
        }
#endif

        /// <summary>
        /// Tests the internal search used to look up values.
        /// </summary>
        [TestMethod]
        public void TestItemKeySearchNull()
        {
            var originalSource = new List<object>(new object[] { 0, 2 });
            var ioc = new InterspersedObservableCollection(originalSource);
            var pioc = new PrivateObject(ioc); // TODO: Look into source generator helper for toolkit?

            ioc.Insert(1, null);
            ioc.Insert(3, "3");

            Assert.AreEqual(new KeyValuePair<int?, object>(1, null), pioc.Invoke("ItemKeySearch", (object)null), "null Index not at 1");
            Assert.AreEqual(new KeyValuePair<int?, object>(3, "3"), pioc.Invoke("ItemKeySearch", (object)"3"), "\"3\" Index not at 3");
        }

        [TestMethod]
        public void TestMoveKeysBackward()
        {
            var originalSource = new ObservableCollection<object>(new object[] { 0, 1 });
            var ioc = new InterspersedObservableCollection(originalSource);

            var pioc = new PrivateObject(ioc); // TODO: Look into source generator helper for toolkit?

            ioc.Insert(3, "2"); // Add one of our interspersed items too far forward

            pioc.Invoke("MoveKeysBackward", new object[] { 0, 1 });

            Assert.AreEqual(2, ioc.IndexOf("2"), "Key didn't move backward.");

            ioc.Insert(1, "0.5"); // Add one of our interspersed items, this will move keys forward 1

            Assert.AreEqual(1, ioc.IndexOf("0.5"), "Key didn't remain in position.");
            Assert.AreEqual(3, ioc.IndexOf("2"), "Key didn't move forward again.");

            ioc.Remove("0.5"); // Remove our first key, we should see other key move back

            Assert.AreEqual(2, ioc.IndexOf("2"), "Key didn't move backwards (2).");

            CollectionAssert.AreEqual(new object[] { 0, 1, "2" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));
        }

        [TestMethod]
        public void TestMoveKeysBackwardAdjacentNearPivot()
        {
            var originalSource = new ObservableCollection<object>(new object[] { 0, 3 });
            var ioc = new InterspersedObservableCollection(originalSource);

            var pioc = new PrivateObject(ioc);

            ioc.Insert(1, "2"); // Add one of our interspersed items
            ioc.Insert(1, "1"); // Add one of our interspersed items

            Assert.AreEqual(1, ioc.IndexOf("1"), "Key 1 not in right position.");
            Assert.AreEqual(2, ioc.IndexOf("2"), "Key 2 not in right position.");

            CollectionAssert.AreEqual(new object[] { 0, "1", "2", 3 }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            ioc.Remove(0); // Remove our first key, we should see other key move back

            CollectionAssert.AreEqual(new object[] { "1", "2", 3 }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(0, ioc.IndexOf("1"), "Key didn't move backwards.");
            Assert.AreEqual(1, ioc.IndexOf("2"), "Key didn't move backwards (2).");
        }

        [TestMethod]
        public void TestMoveKeysForward()
        {
            var originalSource = new ObservableCollection<object>(new object[] { 0, 1 });
            var ioc = new InterspersedObservableCollection(originalSource);

            var pioc = new PrivateObject(ioc); // TODO: Look into source generator helper for toolkit?

            ioc.Insert(2, "3"); // Add one of our interspersed items

            pioc.Invoke("MoveKeysForward", 2, 1);

            Assert.AreEqual(3, ioc.IndexOf("3"), "Key didn't move forward.");

            ioc.Insert(1, "0.5"); // Add one of our interspersed items, this will move keys forward 1

            Assert.AreEqual(1, ioc.IndexOf("0.5"), "Key didn't remain in position.");
            Assert.AreEqual(4, ioc.IndexOf("3"), "Key didn't move forward again.");

            CollectionAssert.AreEqual(new object[] { 0, "0.5", 1, "3" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));
        }

        /// <summary>
        /// Basic functionality test of the collection of interspersing an item into a collection.
        /// </summary>
        [TestMethod]
        public void TestBasicInsertionEnumeration()
        {
            var originalSource = new List<object>(new object[] { 0, 1, 3 });
            var ioc = new InterspersedObservableCollection(originalSource);

            ioc.Insert(2, "2");

            // Check value both ways
            Assert.AreEqual(2, ioc.IndexOf("2"), "Expected \"2\" to be at index 2");
            Assert.AreEqual("2", ioc[2], "Expected index 2 to contain \"2\"");

            // Check Count
            Assert.AreEqual(4, ioc.Count, "Count not as expected");

            // Enumerate collection
            CollectionAssert.AreEqual(new object[] { 0, 1, "2", 3 }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            // Original Collection remains unchanged
            CollectionAssert.AreEqual(new object[] { 0, 1, 3 }, originalSource, string.Format("Original collection was modified, received {0}", originalSource.ToArray().ToArrayString()));
        }

        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1008:Opening parenthesis should be spaced correctly", Justification = "Alignment/readability")]
        public void TestBasicIndexing()
        {
            var originalSource = new List<object>(new object[] { 1, 3, 5 });
            var ioc = new InterspersedObservableCollection(originalSource);

            ioc.Insert(0, "0");
            ioc.Insert(2, "2");
            ioc.Insert(4, "4");

            CollectionAssert.AreEqual(new object[] { "0", 1, "2", 3, "4", 5 }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual("0", ioc[0], "0 index not as expected");
            Assert.AreEqual( 1,  ioc[1], "1 index not as expected");
            Assert.AreEqual("2", ioc[2], "2 index not as expected");
            Assert.AreEqual( 3 , ioc[3], "3 index not as expected");
            Assert.AreEqual("4", ioc[4], "4 index not as expected");
            Assert.AreEqual( 5,  ioc[5], "5 index not as expected");
        }

        /// <summary>
        /// Tests multiple items being interspersed and how they enumerate.
        /// </summary>
        [TestMethod]
        public void TestMultipleInterspersedItemEnumeration()
        {
            var originalSource = new List<object>(new object[] { 0, 2, 5 });
            var ioc = new InterspersedObservableCollection(originalSource);

            ioc.Insert(1, "1");
            ioc.Insert(3, "3");
            ioc.Insert(4, "4");
            ioc.Insert(6, "6");

            CollectionAssert.AreEqual(new object[] { 0, "1", 2, "3", "4", 5, "6" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));
        }

        /// <summary>
        /// Tests multiple items being interspersed and how they enumerate.
        /// </summary>
        [TestMethod]
        public void TestInterspersedItemsAfterEnumeration()
        {
            var originalSource = new List<object>(new object[] { });
            var ioc = new InterspersedObservableCollection(originalSource);

            ioc.Insert(0, "0");
            ioc.Insert(1, "1");
            ioc.Insert(2, "2"); // TODO: How do we handle outside bounds test?

            CollectionAssert.AreEqual(new object[] { "0", "1", "2" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));
        }

        /// <summary>
        /// Tests modifying the original collection and seeing the interspered index auto-update.
        /// </summary>
        [TestMethod]
        public void TestInsertToOriginalCollectionBefore()
        {
            var originalSource = new ObservableCollection<object>(new object[] { 1, 2 });
            var ioc = new InterspersedObservableCollection(originalSource);

            ioc.Insert(2, "3"); // Add one of our interspersed items after '2'

            CollectionAssert.AreEqual(new object[] { 1, 2, "3" }, ioc, string.Format("Intermediate Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            originalSource.Insert(0, "0");

            CollectionAssert.AreEqual(new object[] { "0", 1, 2 }, originalSource, "Original collection doesn't contain new item");

            CollectionAssert.AreEqual(new object[] { "0", 1, 2, "3" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(3, ioc.IndexOf("3"), "Internal index not updated.");
        }

        /// <summary>
        /// Tests modifying the original collection and seeing the interspered index auto-update.
        /// </summary>
        [TestMethod]
        public void TestInsertToOriginalCollectionAfter()
        {
            var originalSource = new ObservableCollection<object>(new object[] { 0, 2, 4 });
            var ioc = new InterspersedObservableCollection(originalSource);

            ioc.Insert(1, "1"); // Add one of our interspersed item after '0'

            CollectionAssert.AreEqual(new object[] { 0, "1", 2, 4 }, ioc, string.Format("Intermediate Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            originalSource.Insert(2, "3"); // Insert after '2'

            CollectionAssert.AreEqual(new object[] { 0, 2, "3", 4 }, originalSource, "Original collection doesn't contain new item");

            CollectionAssert.AreEqual(new object[] { 0, "1", 2, "3", 4 }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(1, ioc.IndexOf("1"), "Internal index was modified.");
        }

        /// <summary>
        /// Tests modifying the original collection and seeing the interspered index auto-update.
        /// </summary>
        [TestMethod]
        public void TestInsertToOriginalCollectionAtEnd()
        {
            var originalSource = new ObservableCollection<object>(new object[] { 0, 1 });
            var ioc = new InterspersedObservableCollection(originalSource);

            ioc.Insert(2, "3"); // Add one of our interspersed items

            CollectionAssert.AreEqual(new object[] { 0, 1, "3" }, ioc, string.Format("Intermediate Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            originalSource.Insert(2, "2");

            CollectionAssert.AreEqual(new object[] { 0, 1, "2" }, originalSource, "Original collection doesn't contain new item");

            CollectionAssert.AreEqual(new object[] { 0, 1, "2", "3" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(3, ioc.IndexOf("3"), "Internal index not updated.");
        }

        [TestMethod]
        public void TestInsertAtBeforeInterspersed()
        {
            var originalSource = new ObservableCollection<object>(new object[] { });
            var ioc = new InterspersedObservableCollection(originalSource);
            var notified = false;

            ioc.Insert(0, "1");

            ioc.CollectionChanged += (s, args) =>
            {
                Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action, "Action not correct");
                Assert.AreEqual(0, args.NewStartingIndex, "Index not expected");
                CollectionAssert.AreEqual(new object[] { 0 }, args.NewItems, string.Format("New item not expected, received {0}", args.NewItems[0]));

                notified = true;
            };

            ioc.InsertAt(0, 0);

            Assert.IsTrue(notified, "CollectionChanged not fired.");

            CollectionAssert.AreEqual(new object[] { 0 }, originalSource, "Original collection doesn't contain new item");

            CollectionAssert.AreEqual(new object[] { 0, "1" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(1, ioc.IndexOf("1"), "Internal index not updated.");
        }

        [TestMethod]
        public void TestInsertAtBeforeMultiple()
        {
            var originalSource = new ObservableCollection<object>(new object[] { 2 });
            var ioc = new InterspersedObservableCollection(originalSource);
            var notified = false;

            ioc.Insert(0, "1");

            ioc.CollectionChanged += (s, args) =>
            {
                Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action, "Action not correct");
                Assert.AreEqual(0, args.NewStartingIndex, "Index not expected");
                CollectionAssert.AreEqual(new object[] { 0 }, args.NewItems, string.Format("New item not expected, received {0}", args.NewItems[0]));

                notified = true;
            };

            ioc.InsertAt(0, 0);

            Assert.IsTrue(notified, "CollectionChanged not fired.");

            CollectionAssert.AreEqual(new object[] { 0, 2 }, originalSource, "Original collection doesn't contain new item");

            CollectionAssert.AreEqual(new object[] { 0, "1", 2 }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(1, ioc.IndexOf("1"), "Internal index not updated.");
        }

        [TestMethod]
        public void TestInsertAtBetweenInterspersed()
        {
            var originalSource = new ObservableCollection<object>(new object[] { 0 });
            var ioc = new InterspersedObservableCollection(originalSource);
            var notified = false;

            ioc.Insert(1, "2");

            ioc.CollectionChanged += (s, args) =>
            {
                Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action, "Action not correct");
                Assert.AreEqual(1, args.NewStartingIndex, "Index not expected");
                CollectionAssert.AreEqual(new object[] { 1 }, args.NewItems, string.Format("New item not expected, received {0}", args.NewItems[0]));

                notified = true;
            };

            ioc.InsertAt(1, 1);

            Assert.IsTrue(notified, "CollectionChanged not fired.");

            CollectionAssert.AreEqual(new object[] { 0, 1 }, originalSource, "Original collection doesn't contain new item");

            CollectionAssert.AreEqual(new object[] { 0, 1, "2" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(2, ioc.IndexOf("2"), "Internal index not updated.");
        }

        /// <summary>
        /// Tests modifying the original collection and seeing the interspered index auto-update.
        /// </summary>
        [TestMethod]
        public void TestInsertAtOriginalCollectionStart()
        {
            var originalSource = new ObservableCollection<object>(new object[] { 2, 4 });
            var ioc = new InterspersedObservableCollection(originalSource);
            var notified = false;

            ioc.Insert(0, "1"); // Add one of our interspersed items
            ioc.Insert(2, "3"); // Add one of our interspersed items

            CollectionAssert.AreEqual(new object[] { "1", 2, "3", 4 }, ioc, string.Format("Intermediate Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            ioc.CollectionChanged += (s, args) =>
            {
                Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action, "Action not correct");
                Assert.AreEqual(0, args.NewStartingIndex, "Index not expected");
                CollectionAssert.AreEqual(new object[] { 0 }, args.NewItems, string.Format("New item not expected, received {0}", args.NewItems[0]));

                notified = true;
            };

            ioc.InsertAt(0, 0);

            Assert.IsTrue(notified, "CollectionChanged not fired.");

            CollectionAssert.AreEqual(new object[] { 0, 2, 4 }, originalSource, "Original collection doesn't contain new item");

            CollectionAssert.AreEqual(new object[] { 0, "1", 2, "3", 4 }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(1, ioc.IndexOf("1"), "Internal index not updated.");
            Assert.AreEqual(3, ioc.IndexOf("3"), "Internal index not updated (2).");
        }

        /// <summary>
        /// Tests modifying the original collection and seeing the interspered index auto-update.
        /// </summary>
        [TestMethod]
        public void TestInsertAtOriginalCollectionBetween()
        {
            var originalSource = new ObservableCollection<object>(new object[] { });
            var ioc = new InterspersedObservableCollection(originalSource);
            var notified = false;

            ioc.Insert(0, "0"); // Add one of our interspersed items
            ioc.Insert(1, "2"); // Add one of our interspersed items

            CollectionAssert.AreEqual(new object[] { "0", "2" }, ioc, string.Format("Intermediate Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            ioc.CollectionChanged += (s, args) =>
            {
                Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action, "Action not correct");
                Assert.AreEqual(1, args.NewStartingIndex, "Index not expected");
                CollectionAssert.AreEqual(new object[] { 1 }, args.NewItems, string.Format("New item not expected, received {0}", args.NewItems[0]));

                notified = true;
            };

            ioc.InsertAt(1, 1);

            Assert.IsTrue(notified, "CollectionChanged not fired.");

            CollectionAssert.AreEqual(new object[] { 1 }, originalSource, "Original collection doesn't contain new item");

            CollectionAssert.AreEqual(new object[] { "0", 1, "2" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(0, ioc.IndexOf("0"), "Internal index was updated.");
            Assert.AreEqual(2, ioc.IndexOf("2"), "Internal index not updated.");
        }

        /// <summary>
        /// Tests modifying the original collection and seeing the interspered index auto-update.
        /// </summary>
        [TestMethod]
        public void TestInsertAtOriginalCollectionMiddle()
        {
            var originalSource = new ObservableCollection<object>(new object[] { 0, 4 });
            var ioc = new InterspersedObservableCollection(originalSource);
            var notified = false;

            ioc.Insert(1, "1"); // Add one of our interspersed items
            ioc.Insert(2, "3"); // Add one of our interspersed items
            ioc.Insert(4, "5"); // Add one of our interspersed items

            CollectionAssert.AreEqual(new object[] { 0, "1", "3", 4, "5" }, ioc, string.Format("Intermediate Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            ioc.CollectionChanged += (s, args) =>
            {
                Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action, "Action not correct");
                Assert.AreEqual(2, args.NewStartingIndex, "Index not expected");
                CollectionAssert.AreEqual(new object[] { 2 }, args.NewItems, string.Format("New item not expected, received {0}", args.NewItems[0]));

                notified = true;
            };

            ioc.InsertAt(2, 2);

            Assert.IsTrue(notified, "CollectionChanged not fired.");

            CollectionAssert.AreEqual(new object[] { 0, 2, 4 }, originalSource, "Original collection doesn't contain new item");

            CollectionAssert.AreEqual(new object[] { 0, "1", 2, "3", 4, "5" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(1, ioc.IndexOf("1"), "Internal index was updated.");
            Assert.AreEqual(3, ioc.IndexOf("3"), "Internal index not updated.");
            Assert.AreEqual(5, ioc.IndexOf("5"), "Internal index not updated (2).");
        }

        /// <summary>
        /// Tests modifying the original collection and seeing the interspered index auto-update.
        /// </summary>
        [TestMethod]
        public void TestInsertAtOriginalCollectionMiddleMultiple()
        {
            var originalSource = new ObservableCollection<object>(new object[] { 0, 5 });
            var ioc = new InterspersedObservableCollection(originalSource);
            var notified = false;

            ioc.Insert(1, "1"); // Add one of our interspersed items
            ioc.Insert(2, "2"); // Add one of our interspersed items
            ioc.Insert(3, "4"); // Add one of our interspersed items
            ioc.Insert(5, "6"); // Add one of our interspersed items

            CollectionAssert.AreEqual(new object[] { 0, "1", "2", "4", 5, "6" }, ioc, string.Format("Intermediate Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            ioc.CollectionChanged += (s, args) =>
            {
                Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action, "Action not correct");
                Assert.AreEqual(3, args.NewStartingIndex, "Index not expected");
                CollectionAssert.AreEqual(new object[] { 3 }, args.NewItems, string.Format("New item not expected, received {0}", args.NewItems[0]));

                notified = true;
            };

            ioc.InsertAt(3, 3);

            Assert.IsTrue(notified, "CollectionChanged not fired.");

            CollectionAssert.AreEqual(new object[] { 0, 3, 5 }, originalSource, "Original collection doesn't contain new item");

            CollectionAssert.AreEqual(new object[] { 0, "1", "2", 3, "4", 5, "6" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(1, ioc.IndexOf("1"), "Internal index was updated.");
            Assert.AreEqual(2, ioc.IndexOf("2"), "Internal index was updated (2).");
            Assert.AreEqual(4, ioc.IndexOf("4"), "Internal index not updated.");
            Assert.AreEqual(6, ioc.IndexOf("6"), "Internal index not updated (2).");
        }

        /// <summary>
        /// Tests modifying the original collection and seeing the interspered index auto-update.
        /// </summary>
        [TestMethod]
        public void TestInsertAtOriginalCollectionNearEnd()
        {
            var originalSource = new ObservableCollection<object>(new object[] { 0, 1 });
            var ioc = new InterspersedObservableCollection(originalSource);
            var notified = false;

            ioc.Insert(2, "2"); // Add one of our interspersed items
            ioc.Insert(3, "4"); // Add one of our interspersed items

            CollectionAssert.AreEqual(new object[] { 0, 1, "2", "4" }, ioc, string.Format("Intermediate Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            ioc.CollectionChanged += (s, args) =>
            {
                Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action, "Action not correct");
                Assert.AreEqual(3, args.NewStartingIndex, "Index not expected");
                CollectionAssert.AreEqual(new object[] { 3 }, args.NewItems, string.Format("New item not expected, received {0}", args.NewItems[0]));

                notified = true;
            };

            ioc.InsertAt(3, 3);

            Assert.IsTrue(notified, "CollectionChanged not fired.");

            CollectionAssert.AreEqual(new object[] { 0, 1, 3 }, originalSource, "Original collection doesn't contain new item");

            CollectionAssert.AreEqual(new object[] { 0, 1, "2", 3, "4" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(2, ioc.IndexOf("2"), "Internal index was updated.");
            Assert.AreEqual(4, ioc.IndexOf("4"), "Internal index not updated.");
        }

        /// <summary>
        /// Tests modifying the original collection and seeing the interspered index auto-update.
        /// </summary>
        [TestMethod]
        public void TestInsertAtOriginalCollectionAtEnd()
        {
            var originalSource = new ObservableCollection<object>(new object[] { 0, 1 });
            var ioc = new InterspersedObservableCollection(originalSource);
            var notified = false;

            ioc.Insert(2, "2"); // Add one of our interspersed items
            ioc.Insert(3, "3"); // Add one of our interspersed items

            CollectionAssert.AreEqual(new object[] { 0, 1, "2", "3" }, ioc, string.Format("Intermediate Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            ioc.CollectionChanged += (s, args) =>
            {
                Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action, "Action not correct");
                Assert.AreEqual(4, args.NewStartingIndex, "Index not expected");
                CollectionAssert.AreEqual(new object[] { 4 }, args.NewItems, string.Format("New item not expected, received {0}", args.NewItems[0]));

                notified = true;
            };

            ioc.InsertAt(4, 4);

            Assert.IsTrue(notified, "CollectionChanged not fired.");

            CollectionAssert.AreEqual(new object[] { 0, 1, 4 }, originalSource, "Original collection doesn't contain new item");

            CollectionAssert.AreEqual(new object[] { 0, 1, "2", "3", 4 }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(2, ioc.IndexOf("2"), "Internal index was updated.");
            Assert.AreEqual(3, ioc.IndexOf("3"), "Internal index was updated (2).");
        }

        /// <summary>
        /// Tests modifying the original collection and seeing the interspered index auto-update.
        /// </summary>
        [TestMethod]
        public void TestRemoveItemOriginalCollectionBefore()
        {
            var originalSource = new ObservableCollection<object>(new object[] { -1, 0 });
            var ioc = new InterspersedObservableCollection(originalSource);

            ioc.Insert(2, "1"); // Add one of our interspersed items after '2'

            CollectionAssert.AreEqual(new object[] { -1, 0, "1" }, ioc, string.Format("Intermediate Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            originalSource.RemoveAt(0);

            CollectionAssert.AreEqual(new object[] { 0 }, originalSource, "Original collection still contains item");

            CollectionAssert.AreEqual(new object[] { 0, "1" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(1, ioc.IndexOf("1"), "Internal index not updated.");
        }

        /// <summary>
        /// Tests modifying the original collection and seeing the interspered index auto-update.
        /// </summary>
        [TestMethod]
        public void TestRemoveItemOriginalCollectionAfter()
        {
            var originalSource = new ObservableCollection<object>(new object[] { 0, -1, 2 });
            var ioc = new InterspersedObservableCollection(originalSource);

            ioc.Insert(1, "1"); // Add one of our interspersed item after '0'

            CollectionAssert.AreEqual(new object[] { 0, "1", -1, 2 }, ioc, string.Format("Intermediate Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            originalSource.RemoveAt(1);

            CollectionAssert.AreEqual(new object[] { 0, 2 }, originalSource, "Original collection still contains extra item");

            CollectionAssert.AreEqual(new object[] { 0, "1", 2 }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(1, ioc.IndexOf("1"), "Internal index was updated.");
        }

        /// <summary>
        /// Tests modifying the original collection and seeing the interspered index auto-update.
        /// </summary>
        [TestMethod]
        public void TestRemoveItemOriginalCollectionAtEnd()
        {
            var originalSource = new ObservableCollection<object>(new object[] { 0, 1 });
            var ioc = new InterspersedObservableCollection(originalSource);

            ioc.Insert(2, "1"); // Add one of our interspersed items

            CollectionAssert.AreEqual(new object[] { 0, 1, "1" }, ioc, string.Format("Intermediate Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            originalSource.RemoveAt(1);

            CollectionAssert.AreEqual(new object[] { 0 }, originalSource, "Original collection still contains original item");

            CollectionAssert.AreEqual(new object[] { 0, "1" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(1, ioc.IndexOf("1"), "Internal index not updated.");
        }

        [TestMethod]
        public void TestCollectionNotificationAddAtStart()
        {
            var originalSource = new ObservableCollection<object>(new object[] { });
            var ioc = new InterspersedObservableCollection(originalSource);
            var notified = false;

            ioc.Insert(0, "1"); // Add an initial item

            CollectionAssert.AreEqual(new object[] { "1" }, ioc, string.Format("Intermediate Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            ioc.CollectionChanged += (s, args) =>
            {
                Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action, "Action not correct");
                Assert.AreEqual(0, args.NewStartingIndex, "Index not expected");
                CollectionAssert.AreEqual(new object[] { 0 }, args.NewItems, string.Format("New item not expected, received {0}", args.NewItems[0]));

                notified = true;
            };

            originalSource.Add(0);

            Assert.IsTrue(notified, "CollectionChanged not fired.");

            CollectionAssert.AreEqual(new object[] { 0 }, originalSource, "Original doesn't contain item");

            CollectionAssert.AreEqual(new object[] { 0, "1" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(1, ioc.IndexOf("1"), "Internal index not updated.");
        }

        [TestMethod]
        public void TestCollectionNotificationRemoveAtStart()
        {
            var originalSource = new ObservableCollection<object>(new object[] { -1 });
            var ioc = new InterspersedObservableCollection(originalSource);
            var notified = false;

            ioc.Insert(1, "0"); // Add an initial item at end

            CollectionAssert.AreEqual(new object[] { -1, "0" }, ioc, string.Format("Intermediate Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            ioc.CollectionChanged += (s, args) =>
            {
                Assert.AreEqual(NotifyCollectionChangedAction.Remove, args.Action, "Action not correct");
                Assert.AreEqual(0, args.OldStartingIndex, "Index not expected");
                CollectionAssert.AreEqual(new object[] { -1 }, args.OldItems, string.Format("Old item not expected, received {0}", args.OldItems[0]));

                notified = true;
            };

            originalSource.Remove(-1);

            Assert.IsTrue(notified, "CollectionChanged not fired.");

            CollectionAssert.AreEqual(new object[] { }, originalSource, "Original contains item");

            CollectionAssert.AreEqual(new object[] { "0" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(0, ioc.IndexOf("0"), "Internal index not updated.");
        }

        [TestMethod]
        public void TestCollectionNotificationRemoveInterspersed()
        {
            var originalSource = new ObservableCollection<object>(new object[] { 0, 2, 3 });
            var ioc = new InterspersedObservableCollection(originalSource);
            var notified = false;

            ioc.Insert(1, "1"); // Add an initial item
            ioc.Insert(4, "4"); // Add an item at end

            CollectionAssert.AreEqual(new object[] { 0, "1", 2, 3, "4" }, ioc, string.Format("Intermediate Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            ioc.CollectionChanged += (s, args) =>
            {
                Assert.AreEqual(NotifyCollectionChangedAction.Remove, args.Action, "Action not correct");
                Assert.AreEqual(1, args.OldStartingIndex, "Index not expected");
                CollectionAssert.AreEqual(new object[] { "1" }, args.OldItems, string.Format("Old item not expected, received {0}", args.OldItems[0]));

                notified = true;
            };

            ioc.Remove("1");

            Assert.IsTrue(notified, "CollectionChanged not fired.");

            CollectionAssert.AreEqual(new object[] { 0, 2, 3 }, originalSource, "Original was modified");

            CollectionAssert.AreEqual(new object[] { 0, 2, 3, "4" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(3, ioc.IndexOf("4"), "Internal index not updated.");
        }

        [TestMethod]
        public void TestCollectionNotificationRemoveInterspersedOriginal()
        {
            var originalSource = new ObservableCollection<object>(new object[] { 0, 1, 3 });
            var ioc = new InterspersedObservableCollection(originalSource);
            var notified = false;

            ioc.Insert(2, "2"); // Add an initial item
            ioc.Insert(4, "4"); // Add an item at end

            CollectionAssert.AreEqual(new object[] { 0, 1, "2", 3, "4" }, ioc, string.Format("Intermediate Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            ioc.CollectionChanged += (s, args) =>
            {
                Assert.AreEqual(NotifyCollectionChangedAction.Remove, args.Action, "Action not correct");
                Assert.AreEqual(3, args.OldStartingIndex, "Index not expected");
                CollectionAssert.AreEqual(new object[] { 3 }, args.OldItems, string.Format("Old item not expected, received {0}", args.OldItems[0]));

                notified = true;
            };

            ioc.Remove(3);

            Assert.IsTrue(notified, "CollectionChanged not fired.");

            CollectionAssert.AreEqual(new object[] { 0, 1 }, originalSource, "Original not modified");

            CollectionAssert.AreEqual(new object[] { 0, 1, "2", "4" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(2, ioc.IndexOf("2"), "Internal index was updated.");
            Assert.AreEqual(3, ioc.IndexOf("4"), "Internal index not updated.");
        }

        [TestMethod]
        public void TestCollectionNotificationRemovalSequence()
        {
            var originalSource = new ObservableCollection<object>(new object[] { 0, 1, 2, 3 });
            var ioc = new InterspersedObservableCollection(originalSource);
            var notified = false;

            ioc.Insert(4, "4"); // Add an item at end

            CollectionAssert.AreEqual(new object[] { 0, 1, 2, 3, "4" }, ioc, string.Format("Intermediate Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            ioc.Remove(1);
            ioc.Insert(1, "1");

            ioc.Remove(2);
            ioc.Insert(2, "2");

            CollectionAssert.AreEqual(new object[] { 0, "1", "2", 3, "4" }, ioc, string.Format("Intermediate Collection (2) not as expected, received {0}", ioc.ToArray().ToArrayString()));

            ioc.CollectionChanged += (s, args) =>
            {
                Assert.AreEqual(NotifyCollectionChangedAction.Remove, args.Action, "Action not correct");
                Assert.AreEqual(3, args.OldStartingIndex, "Index not expected");
                CollectionAssert.AreEqual(new object[] { 3 }, args.OldItems, string.Format("Old item not expected, received {0}", args.OldItems[0]));

                notified = true;
            };

            ioc.Remove(3);

            Assert.IsTrue(notified, "CollectionChanged not fired.");

            CollectionAssert.AreEqual(new object[] { 0 }, originalSource, "Original not modified");

            CollectionAssert.AreEqual(new object[] { 0, "1", "2", "4" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(1, ioc.IndexOf("1"), "Internal index was updated.");
            Assert.AreEqual(2, ioc.IndexOf("2"), "Internal index was updated (2).");
            Assert.AreEqual(3, ioc.IndexOf("4"), "Internal index not updated.");
        }

        [TestMethod]
        public void TestCollectionInsertOriginalFirstItem()
        {
            var originalSource = new ObservableCollection<object>(new object[] { });
            var ioc = new InterspersedObservableCollection(originalSource);
            var notified = false;

            ioc.Insert(0, 1);
            ioc.Insert(1, 2);

            CollectionAssert.AreEqual(new object[] { 1, 2 }, ioc, string.Format("Intermediate Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            ioc.CollectionChanged += (s, args) =>
            {
                Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action, "Action not correct");
                Assert.AreEqual(0, args.NewStartingIndex, "Index not expected");
                CollectionAssert.AreEqual(new object[] { "0" }, args.NewItems, string.Format("New item not expected, received {0}", args.NewItems[0]));

                notified = true;
            };

            originalSource.Add("0");

            Assert.IsTrue(notified, "CollectionChanged not fired.");

            CollectionAssert.AreEqual(new object[] { "0" }, originalSource, "Original doesn't contain item");

            CollectionAssert.AreEqual(new object[] { "0", 1, 2 }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(0, ioc.IndexOf("0"), "Original index not correct.");
            Assert.AreEqual(1, ioc.IndexOf(1), "Internal index not updated.");
            Assert.AreEqual(2, ioc.IndexOf(2), "Internal index not updated (2).");
        }

        [TestMethod]
        public void TestCollectionInsertInterspersedBefore()
        {
            var originalSource = new ObservableCollection<object>(new object[] { 0, 2 });
            var ioc = new InterspersedObservableCollection(originalSource);
            var notified = false;

            ioc.Insert(2, "3");

            CollectionAssert.AreEqual(new object[] { 0, 2, "3" }, ioc, string.Format("Intermediate Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            ioc.CollectionChanged += (s, args) =>
            {
                Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action, "Action not correct");
                Assert.AreEqual(1, args.NewStartingIndex, "Index not expected");
                CollectionAssert.AreEqual(new object[] { "1" }, args.NewItems, string.Format("New item not expected, received {0}", args.NewItems[0]));

                notified = true;
            };

            ioc.Insert(1, "1");

            Assert.IsTrue(notified, "CollectionChanged not fired.");

            CollectionAssert.AreEqual(new object[] { 0, 2 }, originalSource, "Original source changed");

            CollectionAssert.AreEqual(new object[] { 0, "1", 2, "3" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(0, ioc.IndexOf(0), "Original index not correct.");
            Assert.AreEqual(1, ioc.IndexOf("1"), "Internal index not correct.");
            Assert.AreEqual(2, ioc.IndexOf(2), "Original index not correct (2).");
            Assert.AreEqual(3, ioc.IndexOf("3"), "Internal index not updated.");
        }

        [TestMethod]
        public void TestOriginalCollectionCleared()
        {
            var originalSource = new ObservableCollection<object>(new object[] { 0, 2 });
            var ioc = new InterspersedObservableCollection(originalSource);
            var notified = false;

            ioc.Insert(1, "1");
            ioc.Insert(3, "3");

            CollectionAssert.AreEqual(new object[] { 0, "1", 2, "3" }, ioc, string.Format("Intermediate Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            ioc.CollectionChanged += (s, args) =>
            {
                Assert.AreEqual(NotifyCollectionChangedAction.Reset, args.Action, "Action not correct");

                notified = true;
            };

            originalSource.Clear();

            Assert.IsTrue(notified, "CollectionChanged not fired.");

            CollectionAssert.AreEqual(new object[] { }, originalSource, "Original contains items");

            CollectionAssert.AreEqual(new object[] { "1", "3" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(0, ioc.IndexOf("1"), "Internal index not updated.");
            Assert.AreEqual(1, ioc.IndexOf("3"), "Internal index not updated (2).");
        }

        /// <summary>
        /// Tests the internal condensing method.
        /// </summary>
        [TestMethod]
        public void TestReadjustKeysNoItems()
        {
            var originalSource = new List<object>(new object[] { });
            var ioc = new InterspersedObservableCollection(originalSource);
            var pioc = new PrivateObject(ioc); // TODO: Look into source generator helper for toolkit?

            ioc.Insert(1, "1");
            ioc.Insert(3, "3");

            CollectionAssert.AreEqual(new object[] { "1", "3" }, ioc, string.Format("Starting Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            pioc.Invoke("ReadjustKeys"); // Call our condensing method

            CollectionAssert.AreEqual(new object[] { "1", "3" }, ioc, string.Format("Collection not as expected, received {0}", ioc.ToArray().ToArrayString()));

            Assert.AreEqual(0, ioc.IndexOf("1"), "Internal index not updated.");
            Assert.AreEqual(1, ioc.IndexOf("3"), "Internal index not updated (2).");
        }

        // TODO: Unit tests for InsertTo with non-observable
    }
}