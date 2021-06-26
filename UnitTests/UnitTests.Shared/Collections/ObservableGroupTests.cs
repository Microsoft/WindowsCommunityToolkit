// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Specialized;
using System.Linq;
using FluentAssertions;
using Microsoft.Toolkit.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Collections
{
    [TestClass]
    public class ObservableGroupTests
    {
        [TestCategory("Collections")]
        [TestMethod]
        public void Ctor_ShouldHaveExpectedState()
        {
            var group = new ObservableGroup<string, int>("key");

            group.Key.Should().Be("key");
            group.Should().BeEmpty();
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Ctor_WithGrouping_ShouldHaveExpectedState()
        {
            var source = new IntGroup("Key", new[] { 1, 2, 3 });
            var group = new ObservableGroup<string, int>(source);

            group.Key.Should().Be("Key");
            group.Should().BeEquivalentTo(new[] { 1, 2, 3 }, option => option.WithStrictOrdering());
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Ctor_WithCollection_ShouldHaveExpectedState()
        {
            var source = new[] { 1, 2, 3 };
            var group = new ObservableGroup<string, int>("key", source);

            group.Key.Should().Be("key");
            group.Should().BeEquivalentTo(new[] { 1, 2, 3 }, option => option.WithStrictOrdering());
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Add_ShouldRaiseEvent()
        {
            var collectionChangedEventRaised = false;
            var source = new[] { 1, 2, 3 };
            var group = new ObservableGroup<string, int>("key", source);
            ((INotifyCollectionChanged)group).CollectionChanged += (s, e) => collectionChangedEventRaised = true;

            group.Add(4);

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
            var group = new ObservableGroup<string, int>("key", source);
            ((INotifyCollectionChanged)group).CollectionChanged += (s, e) => collectionChangedEventRaised = true;

            group[1] = 4;

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
            var group = new ObservableGroup<string, int>("key", source);
            ((INotifyCollectionChanged)group).CollectionChanged += (s, e) => collectionChangedEventRaised = true;

            group.Remove(1);

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
            var group = new ObservableGroup<string, int>("key", source);
            ((INotifyCollectionChanged)group).CollectionChanged += (s, e) => collectionChangedEventRaised = true;

            group.Clear();

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
            var group = new ObservableGroup<string, int>("key", Enumerable.Range(0, count));
            var iReadOnlyObservableGroup = (IReadOnlyObservableGroup)group;

            iReadOnlyObservableGroup.Key.Should().Be("key");
            iReadOnlyObservableGroup.Count.Should().Be(count);
        }
    }
}