// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using FluentAssertions;
using Microsoft.Toolkit.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Collections
{
    [TestClass]
    public class ReadOnlyObservableGroupedCollectionTests
    {
        [TestCategory("Collections")]
        [TestMethod]
        public void Ctor_WithEmptySource_ShoudInitializeObject()
        {
            var source = new ObservableGroupedCollection<string, int>();
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);

            readOnlyGroup.Should().BeEmpty();
            readOnlyGroup.Count.Should().Be(0);
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Ctor_WithObservableGroupedCollection_ShoudInitializeObject()
        {
            var groups = new List<IGrouping<string, int>>
            {
                new IntGroup("A", new[] { 1, 3, 5 }),
                new IntGroup("B", new[] { 2, 4, 6 }),
            };
            var source = new ObservableGroupedCollection<string, int>(groups);
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);

            readOnlyGroup.Should().HaveCount(2);
            readOnlyGroup.Count.Should().Be(2);
            readOnlyGroup.ElementAt(0).Key.Should().Be("A");
            readOnlyGroup.ElementAt(0).Should().BeEquivalentTo(new[] { 1, 3, 5 }, o => o.WithoutStrictOrdering());
            readOnlyGroup.ElementAt(1).Key.Should().Be("B");
            readOnlyGroup.ElementAt(1).Should().BeEquivalentTo(new[] { 2, 4, 6 }, o => o.WithoutStrictOrdering());
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Ctor_WithListOfIGroupingSource_ShoudInitializeObject()
        {
            var source = new List<IGrouping<string, int>>
            {
                new IntGroup("A", new[] { 1, 3, 5 }),
                new IntGroup("B", new[] { 2, 4, 6 }),
            };
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);

            readOnlyGroup.Should().HaveCount(2);
            readOnlyGroup.Count.Should().Be(2);
            readOnlyGroup.ElementAt(0).Key.Should().Be("A");
            readOnlyGroup.ElementAt(0).Should().BeEquivalentTo(new[] { 1, 3, 5 }, o => o.WithoutStrictOrdering());
            readOnlyGroup.ElementAt(1).Key.Should().Be("B");
            readOnlyGroup.ElementAt(1).Should().BeEquivalentTo(new[] { 2, 4, 6 }, o => o.WithoutStrictOrdering());
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Ctor_WithListOfReadOnlyObservableGroupSource_ShoudInitializeObject()
        {
            var source = new List<ReadOnlyObservableGroup<string, int>>
            {
                new ReadOnlyObservableGroup<string, int>("A", new[] { 1, 3, 5 }),
                new ReadOnlyObservableGroup<string, int>("B", new[] { 2, 4, 6 }),
            };
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);

            readOnlyGroup.Should().HaveCount(2);
            readOnlyGroup.Count.Should().Be(2);
            readOnlyGroup.ElementAt(0).Key.Should().Be("A");
            readOnlyGroup.ElementAt(0).Should().BeEquivalentTo(new[] { 1, 3, 5 }, o => o.WithoutStrictOrdering());
            readOnlyGroup.ElementAt(1).Key.Should().Be("B");
            readOnlyGroup.ElementAt(1).Should().BeEquivalentTo(new[] { 2, 4, 6 }, o => o.WithoutStrictOrdering());
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void IListImplementation_Properties_ShoudReturnExpectedValues()
        {
            var groups = new List<IGrouping<string, int>>
            {
                new IntGroup("A", new[] { 1, 3, 5 }),
                new IntGroup("B", new[] { 2, 4, 6 }),
            };
            var source = new ObservableGroupedCollection<string, int>(groups);
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);
            var list = (IList)readOnlyGroup;

            list.Count.Should().Be(2);
            var group0 = (ReadOnlyObservableGroup<string, int>)list[0];
            group0.Key.Should().Be("A");
            group0.Should().BeEquivalentTo(new[] { 1, 3, 5 }, o => o.WithoutStrictOrdering());
            var group1 = (ReadOnlyObservableGroup<string, int>)list[1];
            group1.Key.Should().Be("B");
            group1.Should().BeEquivalentTo(new[] { 2, 4, 6 }, o => o.WithoutStrictOrdering());

            list.SyncRoot.Should().NotBeNull();
            list.IsFixedSize.Should().BeTrue();
            list.IsReadOnly.Should().BeTrue();
            list.IsSynchronized.Should().BeFalse();
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void IListImplementation_MutableMethods_ShoudThrow()
        {
            var groups = new List<IGrouping<string, int>>
            {
                new IntGroup("A", new[] { 1, 3, 5 }),
                new IntGroup("B", new[] { 2, 4, 6 }),
            };
            var source = new ObservableGroupedCollection<string, int>(groups);
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);
            var list = (IList)readOnlyGroup;

            var testGroup = new ReadOnlyObservableGroup<string, int>("test", new ObservableCollection<int>());
            Action add = () => list.Add(testGroup);
            add.Should().Throw<NotSupportedException>();

            Action clear = () => list.Clear();
            clear.Should().Throw<NotSupportedException>();

            Action insert = () => list.Insert(2, testGroup);
            insert.Should().Throw<NotSupportedException>();

            Action remove = () => list.Remove(testGroup);
            remove.Should().Throw<NotSupportedException>();

            Action removeAt = () => list.RemoveAt(2);
            removeAt.Should().Throw<NotSupportedException>();

            Action set = () => list[2] = testGroup;
            set.Should().Throw<NotSupportedException>();

            var array = new object[5];
            Action copyTo = () => list.CopyTo(array, 0);
            copyTo.Should().NotThrow();
        }

        [TestCategory("Collections")]
        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void IListImplementation_IndexOf_ShoudReturnExpectedValue(int groupIndex)
        {
            var groups = new List<IGrouping<string, int>>
            {
                new IntGroup("A", new[] { 1, 3, 5 }),
                new IntGroup("B", new[] { 2, 4, 6 }),
                new IntGroup("C", new[] { 7, 8, 9 }),
            };
            var source = new ObservableGroupedCollection<string, int>(groups);
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);
            var list = (IList)readOnlyGroup;

            var groupToSearch = groupIndex >= 0 ? list[groupIndex] : null;

            var index = list.IndexOf(groupToSearch);

            index.Should().Be(groupIndex);
        }

        [TestCategory("Collections")]
        [DataTestMethod]
        [DataRow(-1, false)]
        [DataRow(0, true)]
        [DataRow(1, true)]
        public void IListImplementation_Contains_ShoudReturnExpectedValue(int groupIndex, bool expectedResult)
        {
            var groups = new List<IGrouping<string, int>>
            {
                new IntGroup("A", new[] { 1, 3, 5 }),
                new IntGroup("B", new[] { 2, 4, 6 }),
            };
            var source = new ObservableGroupedCollection<string, int>(groups);
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);
            var list = (IList)readOnlyGroup;

            var groupToSearch = groupIndex >= 0 ? list[groupIndex] : null;

            var result = list.Contains(groupToSearch);

            result.Should().Be(expectedResult);
        }

        [TestCategory("Collections")]
        [DataTestMethod]
        [DataRow(0, 0)]
        [DataRow(3, 3)]
        public void AddGroupInSource_ShouldAddGroup(int sourceInitialItemsCount, int expectedInsertionIndex)
        {
            NotifyCollectionChangedEventArgs collectionChangedEventArgs = null;
            var collectionChangedEventsCount = 0;
            var isCountPropertyChangedEventRaised = false;
            var itemsList = new[] { 1, 2, 3 };
            var source = new ObservableGroupedCollection<string, int>();
            for (var i = 0; i < sourceInitialItemsCount; i++)
            {
                source.Add(new ObservableGroup<string, int>($"group {i}", Enumerable.Empty<int>()));
            }

            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);
            ((INotifyCollectionChanged)readOnlyGroup).CollectionChanged += (s, e) =>
            {
                collectionChangedEventArgs = e;
                collectionChangedEventsCount++;
            };
            ((INotifyPropertyChanged)readOnlyGroup).PropertyChanged += (s, e) => isCountPropertyChangedEventRaised = isCountPropertyChangedEventRaised || e.PropertyName == nameof(readOnlyGroup.Count);

            source.Add(new ObservableGroup<string, int>("Add", itemsList));

            var expectedReadOnlyGroupCount = sourceInitialItemsCount + 1;
            readOnlyGroup.Should().HaveCount(expectedReadOnlyGroupCount);
            readOnlyGroup.Count.Should().Be(expectedReadOnlyGroupCount);
            readOnlyGroup.Last().Key.Should().Be("Add");
            readOnlyGroup.Last().Should().BeEquivalentTo(itemsList, o => o.WithoutStrictOrdering());

            isCountPropertyChangedEventRaised.Should().BeTrue();
            collectionChangedEventArgs.Should().NotBeNull();
            collectionChangedEventsCount.Should().Be(1);
            IsAddEventValid(collectionChangedEventArgs, itemsList, expectedInsertionIndex).Should().BeTrue();
        }

        [TestCategory("Collections")]
        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void InsertGroupInSource_ShouldAddGroup(int insertionIndex)
        {
            NotifyCollectionChangedEventArgs collectionChangedEventArgs = null;
            var collectionChangedEventsCount = 0;
            var isCountPropertyChangedEventRaised = false;
            var itemsList = new[] { 1, 2, 3 };
            var source = new ObservableGroupedCollection<string, int>
            {
                new ObservableGroup<string, int>("Group0", new[] { 10, 20, 30 }),
                new ObservableGroup<string, int>("Group1", new[] { 40, 50, 60 })
            };
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);
            ((INotifyCollectionChanged)readOnlyGroup).CollectionChanged += (s, e) =>
            {
                collectionChangedEventArgs = e;
                collectionChangedEventsCount++;
            };
            ((INotifyPropertyChanged)readOnlyGroup).PropertyChanged += (s, e) => isCountPropertyChangedEventRaised = isCountPropertyChangedEventRaised || e.PropertyName == nameof(readOnlyGroup.Count);

            source.Insert(insertionIndex, new ObservableGroup<string, int>("Add", itemsList));

            readOnlyGroup.Should().HaveCount(3);
            readOnlyGroup.Count.Should().Be(3);
            readOnlyGroup.ElementAt(insertionIndex).Key.Should().Be("Add");
            readOnlyGroup.ElementAt(insertionIndex).Should().BeEquivalentTo(itemsList, o => o.WithoutStrictOrdering());

            isCountPropertyChangedEventRaised.Should().BeTrue();
            collectionChangedEventArgs.Should().NotBeNull();
            collectionChangedEventsCount.Should().Be(1);
            IsAddEventValid(collectionChangedEventArgs, itemsList, addIndex: insertionIndex).Should().BeTrue();
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void RemoveGroupInSource_ShoudRemoveGroup()
        {
            NotifyCollectionChangedEventArgs collectionChangedEventArgs = null;
            var collectionChangedEventsCount = 0;
            var isCountPropertyChangedEventRaised = false;
            var aItemsList = new[] { 1, 2, 3 };
            var bItemsList = new[] { 2, 4, 6 };
            var groups = new List<IGrouping<string, int>>
            {
                new IntGroup("A", aItemsList),
                new IntGroup("B", bItemsList),
            };
            var source = new ObservableGroupedCollection<string, int>(groups);
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);
            ((INotifyCollectionChanged)readOnlyGroup).CollectionChanged += (s, e) =>
            {
                collectionChangedEventArgs = e;
                collectionChangedEventsCount++;
            };
            ((INotifyPropertyChanged)readOnlyGroup).PropertyChanged += (s, e) => isCountPropertyChangedEventRaised = isCountPropertyChangedEventRaised || e.PropertyName == nameof(readOnlyGroup.Count);

            source.RemoveAt(1);

            readOnlyGroup.Should().ContainSingle();
            readOnlyGroup.Count.Should().Be(1);
            readOnlyGroup.ElementAt(0).Key.Should().Be("A");
            readOnlyGroup.ElementAt(0).Should().BeEquivalentTo(aItemsList, o => o.WithoutStrictOrdering());

            isCountPropertyChangedEventRaised.Should().BeTrue();
            collectionChangedEventArgs.Should().NotBeNull();
            collectionChangedEventsCount.Should().Be(1);
            IsRemoveEventValid(collectionChangedEventArgs, bItemsList, 1).Should().BeTrue();
        }

        [TestCategory("Collections")]
        [DataTestMethod]
        [DataRow(1, 0)]
        [DataRow(0, 1)]
        public void MoveGroupInSource_ShoudMoveGroup(int oldIndex, int newIndex)
        {
            NotifyCollectionChangedEventArgs collectionChangedEventArgs = null;
            var collectionChangedEventsCount = 0;
            var isCountPropertyChangedEventRaised = false;
            var aItemsList = new[] { 1, 2, 3 };
            var bItemsList = new[] { 2, 4, 6 };
            var groups = new List<IGrouping<string, int>>
            {
                new IntGroup("A", aItemsList),
                new IntGroup("B", bItemsList),
            };
            var source = new ObservableGroupedCollection<string, int>(groups);
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);
            ((INotifyCollectionChanged)readOnlyGroup).CollectionChanged += (s, e) =>
            {
                collectionChangedEventArgs = e;
                collectionChangedEventsCount++;
            };
            ((INotifyPropertyChanged)readOnlyGroup).PropertyChanged += (s, e) => isCountPropertyChangedEventRaised = isCountPropertyChangedEventRaised || e.PropertyName == nameof(readOnlyGroup.Count);

            source.Move(oldIndex, newIndex);

            readOnlyGroup.Should().HaveCount(2);
            readOnlyGroup.Count.Should().Be(2);
            readOnlyGroup.ElementAt(0).Key.Should().Be("B");
            readOnlyGroup.ElementAt(0).Should().BeEquivalentTo(bItemsList, o => o.WithoutStrictOrdering());
            readOnlyGroup.ElementAt(1).Key.Should().Be("A");
            readOnlyGroup.ElementAt(1).Should().BeEquivalentTo(aItemsList, o => o.WithoutStrictOrdering());

            isCountPropertyChangedEventRaised.Should().BeFalse();
            collectionChangedEventArgs.Should().NotBeNull();
            collectionChangedEventsCount.Should().Be(1);
            IsMoveEventValid(collectionChangedEventArgs, groups[oldIndex], oldIndex, newIndex).Should().BeTrue();
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void ClearSource_ShoudClear()
        {
            NotifyCollectionChangedEventArgs collectionChangedEventArgs = null;
            var collectionChangedEventsCount = 0;
            var isCountPropertyChangedEventRaised = false;
            var aItemsList = new[] { 1, 2, 3 };
            var bItemsList = new[] { 2, 4, 6 };
            var groups = new List<IGrouping<string, int>>
            {
                new IntGroup("A", aItemsList),
                new IntGroup("B", bItemsList),
            };
            var source = new ObservableGroupedCollection<string, int>(groups);
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);
            ((INotifyCollectionChanged)readOnlyGroup).CollectionChanged += (s, e) =>
            {
                collectionChangedEventArgs = e;
                collectionChangedEventsCount++;
            };
            ((INotifyPropertyChanged)readOnlyGroup).PropertyChanged += (s, e) => isCountPropertyChangedEventRaised = isCountPropertyChangedEventRaised || e.PropertyName == nameof(readOnlyGroup.Count);

            source.Clear();

            readOnlyGroup.Should().BeEmpty();
            readOnlyGroup.Count.Should().Be(0);

            isCountPropertyChangedEventRaised.Should().BeTrue();
            collectionChangedEventArgs.Should().NotBeNull();
            collectionChangedEventsCount.Should().Be(1);
            IsResetEventValid(collectionChangedEventArgs).Should().BeTrue();
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void ReplaceGroupInSource_ShoudReplaceGroup()
        {
            NotifyCollectionChangedEventArgs collectionChangedEventArgs = null;
            var collectionChangedEventsCount = 0;
            var isCountPropertyChangedEventRaised = false;
            var aItemsList = new[] { 1, 2, 3 };
            var bItemsList = new[] { 2, 4, 6 };
            var cItemsList = new[] { 7, 8, 9 };
            var groups = new List<IGrouping<string, int>>
            {
                new IntGroup("A", aItemsList),
                new IntGroup("B", bItemsList),
            };
            var source = new ObservableGroupedCollection<string, int>(groups);
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);
            ((INotifyCollectionChanged)readOnlyGroup).CollectionChanged += (s, e) =>
            {
                collectionChangedEventArgs = e;
                collectionChangedEventsCount++;
            };
            ((INotifyPropertyChanged)readOnlyGroup).PropertyChanged += (s, e) => isCountPropertyChangedEventRaised = isCountPropertyChangedEventRaised || e.PropertyName == nameof(readOnlyGroup.Count);

            source[0] = new ObservableGroup<string, int>("C", cItemsList);

            readOnlyGroup.Should().HaveCount(2);
            readOnlyGroup.Count.Should().Be(2);
            readOnlyGroup.ElementAt(0).Key.Should().Be("C");
            readOnlyGroup.ElementAt(0).Should().BeEquivalentTo(cItemsList, o => o.WithoutStrictOrdering());
            readOnlyGroup.ElementAt(1).Key.Should().Be("B");
            readOnlyGroup.ElementAt(1).Should().BeEquivalentTo(bItemsList, o => o.WithoutStrictOrdering());

            isCountPropertyChangedEventRaised.Should().BeFalse();
            collectionChangedEventArgs.Should().NotBeNull();
            collectionChangedEventsCount.Should().Be(1);
            IsReplaceEventValid(collectionChangedEventArgs, aItemsList, cItemsList).Should().BeTrue();
        }

        private static bool IsAddEventValid(NotifyCollectionChangedEventArgs args, IEnumerable<int> expectedGroupItems, int addIndex)
        {
            var newItems = args.NewItems?.Cast<IEnumerable<int>>();
            return args.Action == NotifyCollectionChangedAction.Add &&
                    args.NewStartingIndex == addIndex &&
                    args.OldItems == null &&
                    newItems?.Count() == 1 &&
                    Enumerable.SequenceEqual(newItems.ElementAt(0), expectedGroupItems);
        }

        private static bool IsRemoveEventValid(NotifyCollectionChangedEventArgs args, IEnumerable<int> expectedGroupItems, int oldIndex)
        {
            var oldItems = args.OldItems?.Cast<IEnumerable<int>>();
            return args.Action == NotifyCollectionChangedAction.Remove &&
                    args.NewItems == null &&
                    args.OldStartingIndex == oldIndex &&
                    oldItems?.Count() == 1 &&
                    Enumerable.SequenceEqual(oldItems.ElementAt(0), expectedGroupItems);
        }

        private static bool IsMoveEventValid(NotifyCollectionChangedEventArgs args, IEnumerable<int> expectedGroupItems, int oldIndex, int newIndex)
        {
            var oldItems = args.OldItems?.Cast<IEnumerable<int>>();
            var newItems = args.NewItems?.Cast<IEnumerable<int>>();
            return args.Action == NotifyCollectionChangedAction.Move &&
                    args.OldStartingIndex == oldIndex &&
                    args.NewStartingIndex == newIndex &&
                    oldItems?.Count() == 1 &&
                    Enumerable.SequenceEqual(oldItems.ElementAt(0), expectedGroupItems) &&
                    newItems?.Count() == 1 &&
                    Enumerable.SequenceEqual(newItems.ElementAt(0), expectedGroupItems);
        }

        private static bool IsReplaceEventValid(NotifyCollectionChangedEventArgs args, IEnumerable<int> expectedRemovedItems, IEnumerable<int> expectedAddItems)
        {
            var oldItems = args.OldItems?.Cast<IEnumerable<int>>();
            var newItems = args.NewItems?.Cast<IEnumerable<int>>();
            return args.Action == NotifyCollectionChangedAction.Replace &&
                    oldItems?.Count() == 1 &&
                    Enumerable.SequenceEqual(oldItems.ElementAt(0), expectedRemovedItems) &&
                    newItems?.Count() == 1 &&
                    Enumerable.SequenceEqual(newItems.ElementAt(0), expectedAddItems);
        }

        private static bool IsResetEventValid(NotifyCollectionChangedEventArgs args) => args.Action == NotifyCollectionChangedAction.Reset && args.NewItems == null && args.OldItems == null;
    }
}