// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Data;

namespace CommunityToolkit.WinUI.UI
{
    /// <summary>
    /// Extended ICollectionView with filtering and sorting
    /// </summary>
    public interface IAdvancedCollectionView : ICollectionView
    {
        /// <summary>
        /// Gets a value indicating whether this CollectionView can filter its items
        /// </summary>
        bool CanFilter { get; }

        /// <summary>
        /// Gets or sets the predicate used to filter the visible items
        /// </summary>
        Predicate<object> Filter { get; set; }

        /// <summary>
        /// Gets a value indicating whether this CollectionView can sort its items
        /// </summary>
        bool CanSort { get; }

        /// <summary>
        /// Gets SortDescriptions to sort the visible items
        /// </summary>
        IList<SortDescription> SortDescriptions { get; }

        /*
        /// <summary>
        /// Gets a value indicating whether this CollectionView can group its items
        /// </summary>
        bool CanGroup { get; }

        /// <summary>
        /// Gets GroupDescriptions to group the visible items
        /// </summary>
        IList<object> GroupDescriptions { get; }
        */

        /// <summary>
        /// Gets the source collection
        /// </summary>
        IEnumerable SourceCollection { get; }

        /// <summary>
        /// Stops refreshing until it is disposed
        /// </summary>
        /// <returns>An disposable object</returns>
        IDisposable DeferRefresh();

        /// <summary>
        /// Manually refreshes the view
        /// </summary>
        void Refresh();

        /// <summary>
        /// Manually refreshes the filter on the view
        /// </summary>
        void RefreshFilter();

        /// <summary>
        /// Manually refreshes the sorting on the view
        /// </summary>
        void RefreshSorting();

        /// <summary>
        /// Add a property to re-filter an item on when it is changed
        /// </summary>
        void ObserveFilterProperty(string propertyName);

        /// <summary>
        /// Clears all properties items are re-filtered on
        /// </summary>
        void ClearObservedFilterProperties();
    }
}