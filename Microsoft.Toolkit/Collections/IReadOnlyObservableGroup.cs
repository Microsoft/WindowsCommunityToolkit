// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace Microsoft.Toolkit.Collections
{
    /// <summary>
    /// An interface for a grouped collection of items.
    /// It allows us to use x:Bind with <see cref="ObservableGroup{TKey, TValue}"/> and <see cref="ReadOnlyObservableGroup{TKey, TValue}"/> by providing
    /// a non-generic type that we can declare using x:DataType.
    /// </summary>
    public interface IReadOnlyObservableGroup : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the key for the current collection, as an <see cref="object"/>.
        /// It is immutable.
        /// </summary>
        object Key { get; }

        /// <summary>
        /// Gets the number of items currently in the grouped collection.
        /// </summary>
        int Count { get; }
    }
}