// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTests.UI
{
    public class DataSource<T> : IIncrementalSource<T>
    {
        private readonly IEnumerable<T> items;
        private readonly Queue<PageOperation> pageRequestOperations;

        public delegate IEnumerable<T> PageOperation(IEnumerable<T> page);

        public DataSource(IEnumerable<T> items, IEnumerable<PageOperation> pageOps)
            : this(items, new Queue<PageOperation>(pageOps))
        {
        }

        public DataSource(IEnumerable<T> items, params PageOperation[] pageOps)
            : this(items, new Queue<PageOperation>(pageOps))
        {
        }

        public DataSource(IEnumerable<T> items, Queue<PageOperation> pageOps = default)
        {
            this.items = items ?? throw new ArgumentNullException(nameof(items));
            this.pageRequestOperations = pageOps ?? new Queue<PageOperation>();
        }

        public static PageOperation MakeDelayOp(int delay)
            => new (page =>
            {
                Thread.Sleep(delay);
                return page;
            });

        public static IEnumerable<T> ThrowException(IEnumerable<T> page) => throw new Exception();

        public static IEnumerable<T> PassThrough(IEnumerable<T> page) => page;

        public async Task<IEnumerable<T>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            // Gets items from the collection according to pageIndex and pageSize parameters.
            var result = (from p in items
                          select p).Skip(pageIndex * pageSize).Take(pageSize);

            return this.pageRequestOperations.TryDequeue(out var op)
                ? await Task.Factory.StartNew(new Func<object, IEnumerable<T>>(o => op(o as IEnumerable<T>)), state: result)
                : result;
        }
    }
}