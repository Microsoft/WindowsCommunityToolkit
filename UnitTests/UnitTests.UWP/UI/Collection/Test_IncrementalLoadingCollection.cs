// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.UI
{
    [TestClass]
    public class Test_IncrementalLoadingCollection
    {
        private static readonly DataSource<int>.PageOperation[] FailPassSequence
            = new DataSource<int>.PageOperation[]
            {
                DataSource<int>.ThrowException, DataSource<int>.PassThrough,
                DataSource<int>.ThrowException, DataSource<int>.PassThrough,
                DataSource<int>.ThrowException, DataSource<int>.PassThrough,
                DataSource<int>.ThrowException, DataSource<int>.PassThrough,
                DataSource<int>.ThrowException, DataSource<int>.PassThrough,
            };

        [TestMethod]
        [DataRow(20, 5)]
        [DataRow(5, 5)]
        [DataRow(20, 5, 2500, 1000, 1000, 1000, 1000)]
        public async Task Requests(int pageSize, int pages, params int[] pageDelays)
        {
            var allData = Enumerable.Range(0, pages * pageSize).ToArray();
            var source = new DataSource<int>(allData, pageDelays.Select(DataSource<int>.MakeDelayOp));
            var collection = new IncrementalLoadingCollection<DataSource<int>, int>(source, pageSize);

            for (var pageNum = 1; pageNum <= pages; pageNum++)
            {
                await collection.LoadMoreItemsAsync(0);
                CollectionAssert.AreEqual(Enumerable.Range(0, pageSize * pageNum).ToArray(), collection);
            }
        }

        [TestMethod]
        [DataRow(20, 5)]
        [DataRow(5, 5)]
        [DataRow(20, 5, 2500, 1000, 1000, 1000, 1000)]
        public async Task RequestsAsync(int pageSize, int pages, params int[] pageDelays)
        {
            var allData = Enumerable.Range(0, pages * pageSize).ToArray();
            var source = new DataSource<int>(allData, pageDelays.Select(DataSource<int>.MakeDelayOp));
            var collection = new IncrementalLoadingCollection<DataSource<int>, int>(source, pageSize);

            var requests = new List<Task>();

            for (var pageNum = 1; pageNum <= pages; pageNum++)
            {
                requests.Add(collection.LoadMoreItemsAsync(0).AsTask()
                    .ContinueWith(t => Assert.IsTrue(t.IsCompletedSuccessfully)));
            }

            await Task.WhenAll(requests);

            CollectionAssert.AreEqual(allData, collection);
        }

        [TestMethod]
        [DataRow(5, 5)]
        [DataRow(20, 5)]
        public async Task FirstRequestFails(int pageSize, int pages)
        {
            var allData = Enumerable.Range(0, pages * pageSize).ToArray();
            var source = new DataSource<int>(allData, DataSource<int>.ThrowException);
            var collection = new IncrementalLoadingCollection<DataSource<int>, int>(source, pageSize);

            await Assert.ThrowsExceptionAsync<AggregateException>(collection.LoadMoreItemsAsync(0).AsTask);

            Assert.IsTrue(!collection.Any());

            var requests = new List<Task>();

            for (var pageNum = 1; pageNum <= pages; pageNum++)
            {
                requests.Add(collection.LoadMoreItemsAsync(0).AsTask()
                    .ContinueWith(t => Assert.IsTrue(t.IsCompletedSuccessfully)));
            }

            await Task.WhenAll(requests);

            CollectionAssert.AreEqual(allData, collection);
        }

        [TestMethod]
        [DataRow(5, 5)]
        [DataRow(20, 5)]
        public async Task EveryOtherRequestFails(int pageSize, int pages)
        {
            var allData = Enumerable.Range(0, pages * pageSize).ToArray();
            var source = new DataSource<int>(allData, FailPassSequence);
            var collection = new IncrementalLoadingCollection<DataSource<int>, int>(source, pageSize);

            var willFail = true;
            for (var submitedRequests = 0; submitedRequests < pages * 2; submitedRequests++)
            {
                if (willFail)
                {
                    await Assert.ThrowsExceptionAsync<AggregateException>(collection.LoadMoreItemsAsync(0).AsTask);
                }
                else
                {
                    await collection.LoadMoreItemsAsync(0);
                }

                willFail = !willFail;
            }

            CollectionAssert.AreEqual(allData, collection);
        }

        [TestMethod]
        [DataRow(5, 5)]
        [DataRow(20, 5)]
        public async Task EveryOtherRequestFailsAsync(int pageSize, int pages)
        {
            var allData = Enumerable.Range(0, pages * pageSize).ToArray();
            var source = new DataSource<int>(allData, FailPassSequence);
            var collection = new IncrementalLoadingCollection<DataSource<int>, int>(source, pageSize);

            var requests = new List<Task>();

            var willFail = true;
            for (var submitedRequests = 0; submitedRequests < pages * 2; submitedRequests++)
            {
                if (willFail)
                {
                    requests.Add(Assert.ThrowsExceptionAsync<AggregateException>(collection.LoadMoreItemsAsync(0).AsTask));
                }
                else
                {
                    requests.Add(collection.LoadMoreItemsAsync(0).AsTask().ContinueWith(t => Assert.IsTrue(t.IsCompletedSuccessfully)));
                }

                willFail = !willFail;
            }

            await Task.WhenAll(requests);

            CollectionAssert.AreEqual(allData, collection);
        }
    }
}