// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using FluentAssertions;
using Microsoft.Toolkit.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Collections
{
    [TestClass]
    public class ReadOnlyObservableGroupTests
    {
        [TestCategory("Collections")]
        [TestMethod]
        public void Ctor_WithKeyAndOBservableCollection_ShouldHaveExpectedInitialState()
        {
            var source = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var group = new ReadOnlyObservableGroup<string, int>("key", source);

            group.Key.Should().Be("key");
            group.Should().BeEquivalentTo(new[] { 1, 2, 3 }, option => option.WithStrictOrdering());
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Ctor_ObservableGroup_ShouldHaveExpectedInitialState()
        {
            var source = new[] { 1, 2, 3 };
            var sourceGroup = new ObservableGroup<string, int>("key", source);
            var group = new ReadOnlyObservableGroup<string, int>(sourceGroup);

            group.Key.Should().Be("key");
            group.Should().BeEquivalentTo(new[] { 1, 2, 3 }, option => option.WithStrictOrdering());
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Ctor_WithKeyAndCollection_ShouldHaveExpectedInitialState()
        {
            var source = new[] { 1, 2, 3 };
            var group = new ReadOnlyObservableGroup<string, int>("key", source);

            group.Key.Should().Be("key");
            group.Should().BeEquivalentTo(new[] { 1, 2, 3 }, option => option.WithStrictOrdering());
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Add_ShouldRaiseEvent()
        {
            var collectionChangedEventRaised = false;
            var source = new[] { 1, 2, 3 };
            var sourceGroup = new ObservableGroup<string, int>("key", source);
            var group = new ReadOnlyObservableGroup<string, int>(sourceGroup);
            ((INotifyCollectionChanged)group).CollectionChanged += (s, e) => collectionChangedEventRaised = true;

            sourceGroup.Add(4);

            group.Key.Should().Be("key");
            group.Should().BeEquivalentTo(new[] { 1, 2, 3, 4 }, option => option.WithStrictOrdering());
            collectionChangedEventRaised.Should().BeTrue();
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Update_ShouldRaiseEvent()
        {
            var collectionChangedEventRaised = false;
            var source = new[] { 1, 2, 3 };
            var sourceGroup = new ObservableGroup<string, int>("key", source);
            var group = new ReadOnlyObservableGroup<string, int>(sourceGroup);
            ((INotifyCollectionChanged)group).CollectionChanged += (s, e) => collectionChangedEventRaised = true;

            sourceGroup[1] = 4;

            group.Key.Should().Be("key");
            group.Should().BeEquivalentTo(new[] { 1, 4, 3 }, option => option.WithStrictOrdering());
            collectionChangedEventRaised.Should().BeTrue();
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Remove_ShouldRaiseEvent()
        {
            var collectionChangedEventRaised = false;
            var source = new[] { 1, 2, 3 };
            var sourceGroup = new ObservableGroup<string, int>("key", source);
            var group = new ReadOnlyObservableGroup<string, int>(sourceGroup);
            ((INotifyCollectionChanged)group).CollectionChanged += (s, e) => collectionChangedEventRaised = true;

            sourceGroup.Remove(1);

            group.Key.Should().Be("key");
            group.Should().BeEquivalentTo(new[] { 2, 3 }, option => option.WithStrictOrdering());
            collectionChangedEventRaised.Should().BeTrue();
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Clear_ShouldRaiseEvent()
        {
            var collectionChangedEventRaised = false;
            var source = new[] { 1, 2, 3 };
            var sourceGroup = new ObservableGroup<string, int>("key", source);
            var group = new ReadOnlyObservableGroup<string, int>(sourceGroup);
            ((INotifyCollectionChanged)group).CollectionChanged += (s, e) => collectionChangedEventRaised = true;

            sourceGroup.Clear();

            group.Key.Should().Be("key");
            group.Should().BeEmpty();
            collectionChangedEventRaised.Should().BeTrue();
        }

        [TestCategory("Collections")]
        [DataTestMethod]
        [DataRow(0)]
        [DataRow(3)]
        public void IReadOnlyObservableGroup_ShouldReturnExpectedValues(int count)
        {
            var sourceGroup = new ObservableGroup<string, int>("key", Enumerable.Range(0, count));
            var group = new ReadOnlyObservableGroup<string, int>(sourceGroup);
            var iReadOnlyObservableGroup = (IReadOnlyObservableGroup)group;

            iReadOnlyObservableGroup.Key.Should().Be("key");
            iReadOnlyObservableGroup.Count.Should().Be(count);
        }
    }
}