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
        private const int PageSize = 20;
        private const int Pages = 5;

        private static readonly DataSource<int>.PageOperation[] FailPassSequence
            = new DataSource<int>.PageOperation[]
            {
                DataSource<int>.ThrowException, DataSource<int>.PassThrough,
                DataSource<int>.ThrowException, DataSource<int>.PassThrough,
                DataSource<int>.ThrowException, DataSource<int>.PassThrough,
                DataSource<int>.ThrowException, DataSource<int>.PassThrough,
                DataSource<int>.ThrowException, DataSource<int>.PassThrough,
            };

        private static readonly int[] AllData
            = Enumerable.Range(0, Pages * PageSize).ToArray();

        [DataRow]
        [DataRow(2500, 1000, 1000, 1000, 1000)]
        [TestMethod]
        public async Task Requests(params int[] pageDelays)
        {
            var source = new DataSource<int>(AllData, pageDelays.Select(DataSource<int>.MakeDelayOp));
            var collection = new IncrementalLoadingCollection<DataSource<int>, int>(source, PageSize);

            for (int pageNum = 1; pageNum <= Pages; pageNum++)
            {
                await collection.LoadMoreItemsAsync(0);
                CollectionAssert.AreEqual(Enumerable.Range(0, PageSize * pageNum).ToArray(), collection);
            }
        }

        [DataRow]
        [DataRow(2500, 1000, 1000, 1000, 1000)]
        [TestMethod]
        public async Task RequestsAsync(params int[] pageDelays)
        {
            var source = new DataSource<int>(AllData, pageDelays.Select(DataSource<int>.MakeDelayOp));
            var collection = new IncrementalLoadingCollection<DataSource<int>, int>(source, PageSize);

            var requests = new List<Task>();

            for (int pageNum = 1; pageNum <= Pages; pageNum++)
            {
                requests.Add(collection.LoadMoreItemsAsync(0).AsTask()
                    .ContinueWith(t => Assert.IsTrue(t.IsCompletedSuccessfully)));
            }

            await Task.WhenAll(requests);

            CollectionAssert.AreEqual(AllData, collection);
        }

        [TestMethod]
        public async Task FirstRequestFails()
        {
            var source = new DataSource<int>(AllData, DataSource<int>.ThrowException);
            var collection = new IncrementalLoadingCollection<DataSource<int>, int>(source, PageSize);

            await Assert.ThrowsExceptionAsync<AggregateException>(collection.LoadMoreItemsAsync(0).AsTask);

            Assert.IsTrue(!collection.Any());

            var requests = new List<Task>();

            for (int pageNum = 1; pageNum <= Pages; pageNum++)
            {
                requests.Add(collection.LoadMoreItemsAsync(0).AsTask()
                    .ContinueWith(t => Assert.IsTrue(t.IsCompletedSuccessfully)));
            }

            await Task.WhenAll(requests);

            CollectionAssert.AreEqual(AllData, collection);
        }

        [TestMethod]
        public async Task EveryOtherRequestFails()
        {
            var source = new DataSource<int>(AllData, FailPassSequence);
            var collection = new IncrementalLoadingCollection<DataSource<int>, int>(source, PageSize);

            var willFail = true;
            for (int submitedRequests = 0; submitedRequests < Pages * 2; submitedRequests++)
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

            CollectionAssert.AreEqual(AllData, collection);
        }

        [TestMethod]
        public async Task EveryOtherRequestFailsAsync()
        {
            var source = new DataSource<int>(AllData, FailPassSequence);
            var collection = new IncrementalLoadingCollection<DataSource<int>, int>(source, PageSize);

            var requests = new List<Task>();

            var willFail = true;
            for (int submitedRequests = 0; submitedRequests < Pages * 2; submitedRequests++)
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

            CollectionAssert.AreEqual(AllData, collection);
        }
    }
}