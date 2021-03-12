// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Microsoft.Toolkit.Uwp.UI.Media
{
    /// <summary>
    /// An extension <see langword="class"/> for the <see cref="System.Collections.Generic"/> <see langword="namespace"/>
    /// </summary>
    internal static class GenericExtensions
    {
        /// <summary>
        /// Merges the two input <see cref="IReadOnlyDictionary{TKey,TValue}"/> instances and makes sure no duplicate keys are present
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the input dictionaries</typeparam>
        /// <typeparam name="TValue">The type of values in the input dictionaries</typeparam>
        /// <param name="a">The first <see cref="IReadOnlyDictionary{TKey,TValue}"/> to merge</param>
        /// <param name="b">The second <see cref="IReadOnlyDictionary{TKey,TValue}"/> to merge</param>
        /// <returns>An <see cref="IReadOnlyDictionary{TKey,TValue}"/> instance with elements from both <paramref name="a"/> and <paramref name="b"/></returns>
        [Pure]
        public static IReadOnlyDictionary<TKey, TValue> Merge<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> a,
            IReadOnlyDictionary<TKey, TValue> b)
        {
            if (a.Keys.FirstOrDefault(b.ContainsKey) is TKey key)
            {
                throw new InvalidOperationException($"The key {key} already exists in the current pipeline");
            }

            return new Dictionary<TKey, TValue>(a.Concat(b));
        }

        /// <summary>
        /// Merges the two input <see cref="IReadOnlyCollection{T}"/> instances and makes sure no duplicate items are present
        /// </summary>
        /// <typeparam name="T">The type of elements in the input collections</typeparam>
        /// <param name="a">The first <see cref="IReadOnlyCollection{T}"/> to merge</param>
        /// <param name="b">The second <see cref="IReadOnlyCollection{T}"/> to merge</param>
        /// <returns>An <see cref="IReadOnlyCollection{T}"/> instance with elements from both <paramref name="a"/> and <paramref name="b"/></returns>
        [Pure]
        public static IReadOnlyCollection<T> Merge<T>(this IReadOnlyCollection<T> a, IReadOnlyCollection<T> b)
        {
            if (a.Any(b.Contains))
            {
                throw new InvalidOperationException("The input collection has at least an item already present in the second collection");
            }

            return a.Concat(b).ToArray();
        }
    }
}
