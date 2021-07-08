// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using Microsoft.UI.Composition;

namespace CommunityToolkit.WinUI.UI.Media.Helpers.Cache
{
    /// <summary>
    /// A <see langword="class"/> used to cache reusable <see cref="CompositionObject"/> instances in each UI thread
    /// </summary>
    /// <typeparam name="T">The type of instances to cache</typeparam>
    internal sealed class CompositionObjectCache<T>
        where T : CompositionObject
    {
        /// <summary>
        /// The cache of weak references of type <typeparamref name="T"/>, to avoid memory leaks
        /// </summary>
        private readonly ConditionalWeakTable<Compositor, WeakReference<T>> cache = new ConditionalWeakTable<Compositor, WeakReference<T>>();

        /// <summary>
        /// Tries to retrieve a valid <typeparamref name="T"/> instance from the cache, and uses the provided factory if an existing item is not found
        /// </summary>
        /// <param name="compositor">The current <see cref="Compositor"/> instance to get the value for</param>
        /// <param name="producer">A <see cref="Func{TResult}"/> instance used to produce a <typeparamref name="T"/> instance</param>
        /// <returns>A <typeparamref name="T"/> instance that is linked to <paramref name="compositor"/></returns>
        public T GetValue(Compositor compositor, Func<Compositor, T> producer)
        {
            lock (cache)
            {
                if (this.cache.TryGetValue(compositor, out var reference) &&
                    reference.TryGetTarget(out var instance))
                {
                    return instance;
                }

                // Create a new instance when needed
                var fallback = producer(compositor);
                this.cache.AddOrUpdate(compositor, new WeakReference<T>(fallback));

                return fallback;
            }
        }
    }
}