// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Toolkit.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Collections
{
    [TestClass]
    public class ObservableGroupedCollectionTests
    {
        [TestCategory("Collections")]
        [TestMethod]
        public void Ctor_ShouldHaveExpectedValues()
        {
            var groupCollection = new ObservableGroupedCollection<string, int>();

            groupCollection.Should().BeEmpty();
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Ctor_WithGroups_ShouldHaveExpectedValues()
        {
            var groups = new List<IGrouping<string, int>>
            {
                new IntGroup("A", new[] { 1, 3, 5 }),
                new IntGroup("B", new[] { 2, 4, 6 }),
            };
            var groupCollection = new ObservableGroupedCollection<string, int>(groups);

            groupCollection.Should().HaveCount(2);
            groupCollection.ElementAt(0).Key.Should().Be("A");
            groupCollection.ElementAt(0).Should().BeEquivalentTo(new[] { 1, 3, 5 }, o => o.WithStrictOrdering());
            groupCollection.ElementAt(1).Key.Should().Be("B");
            groupCollection.ElementAt(1).Should().BeEquivalentTo(new[] { 2, 4, 6 }, o => o.WithStrictOrdering());
        }
    }
}