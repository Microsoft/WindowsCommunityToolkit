// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Collections.Extensions
{
    /// <summary>
    /// An interface providing key type contravariant access to a <see cref="DictionarySlim{TKey,TValue}"/> instance.
    /// </summary>
    /// <typeparam name="TKey">The contravariant type of keys in the dictionary.</typeparam>
    internal interface IDictionarySlim<in TKey> : IDictionarySlim
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Tries to remove a value with a specified key, if present.
        /// </summary>
        /// <param name="key">The key of the value to remove.</param>
        /// <returns>Whether or not the key was present.</returns>
        bool TryRemove(TKey key);
    }
}