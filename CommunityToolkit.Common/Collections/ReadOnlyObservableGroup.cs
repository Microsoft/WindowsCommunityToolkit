// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CommunityToolkit.Common.Collections
{
    /// <summary>
    /// A read-only observable group. It associates a <see cref="Key"/> to a <see cref="ReadOnlyObservableCollection{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TValue">The type of the items in the collection.</typeparam>
    public sealed class ReadOnlyObservableGroup<TKey, TValue> : ReadOnlyObservableCollection<TValue>, IGrouping<TKey, TValue>, IReadOnlyObservableGroup
        where TKey : notnull
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyObservableGroup{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="key">The key of the group.</param>
        /// <param name="collection">The collection of items to add in the group.</param>
        public ReadOnlyObservableGroup(TKey key, ObservableCollection<TValue> collection)
            : base(collection)
        {
            Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyObservableGroup{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="group">The <see cref="ObservableGroup{TKey, TValue}"/> to wrap.</param>
        public ReadOnlyObservableGroup(ObservableGroup<TKey, TValue> group)
            : base(group)
        {
            Key = group.Key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyObservableGroup{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="key">The key of the group.</param>
        /// <param name="collection">The collection of items to add in the group.</param>
        public ReadOnlyObservableGroup(TKey key, IEnumerable<TValue> collection)
            : base(new ObservableCollection<TValue>(collection))
        {
            Key = key;
        }

        /// <inheritdoc/>
        public TKey Key { get; }

        /// <inheritdoc/>
        object IReadOnlyObservableGroup.Key => Key;
    }
}