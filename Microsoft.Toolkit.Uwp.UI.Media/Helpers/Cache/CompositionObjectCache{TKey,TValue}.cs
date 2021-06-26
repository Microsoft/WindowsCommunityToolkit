// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Windows.UI.Composition;

namespace Microsoft.Toolkit.Uwp.UI.Media.Helpers.Cache
{
    /// <summary>
    /// A <see langword="class"/> used to cache reusable <see cref="CompositionObject"/> instances with an associated key
    /// </summary>
    /// <typeparam name="TKey">The type of key to classify the items in the cache</typeparam>
    /// <typeparam name="TValue">The type of items stored in the cache</typeparam>
    internal sealed class CompositionObjectCache<TKey, TValue>
        where TValue : CompositionObject
    {
        /// <summary>
        /// The cache of weak references of type <typeparamref name="TValue"/> to <typeparamref name="TKey"/> instances, to avoid memory leaks
        /// </summary>
        private readonly ConditionalWeakTable<Compositor, Dictionary<TKey, WeakReference<TValue>>> cache = new ConditionalWeakTable<Compositor, Dictionary<TKey, WeakReference<TValue>>>();

        /// <summary>
        /// Tries to retrieve a valid instance from the cache, and uses the provided factory if an existing item is not found
        /// </summary>
        /// <param name="compositor">The current <see cref="Compositor"/> instance to get the value for</param>
        /// <param name="key">The key to look for</param>
        /// <param name="result">The resulting value, if existing</param>
        /// <returns><see langword="true"/> if the target value has been found, <see langword="false"/> otherwise</returns>
        public bool TryGetValue(Compositor compositor, TKey key, out TValue result)
        {
            lock (this.cache)
            {
                if (this.cache.TryGetValue(compositor, out var map) &&
                    map.TryGetValue(key, out var reference) &&
                    reference.TryGetTarget(out result))
                {
                    return true;
                }

                result = null;
                return false;
            }
        }

        /// <summary>
        /// Adds or updates a value with the specified key to the cache
        /// </summary>
        /// <param name="compositor">The current <see cref="Compositor"/> instance to get the value for</param>
        /// <param name="key">The key of the item to add</param>
        /// <param name="value">The value to add</param>
        public void AddOrUpdate(Compositor compositor, TKey key, TValue value)
        {
            lock (this.cache)
            {
                if (this.cache.TryGetValue(compositor, out var map))
                {
                    _ = map.Remove(key);

                    map.Add(key, new WeakReference<TValue>(value));
                }
                else
                {
                    map = new Dictionary<TKey, WeakReference<TValue>> { [key] = new WeakReference<TValue>(value) };

                    this.cache.Add(compositor, map);
                }
            }
        }
    }
}