// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using Microsoft.UI.Xaml.Data;

namespace CommunityToolkit.WinUI.UI.Data.Utilities
{
    /// <summary>
    /// A collection view implementation that supports an IEnumerable source.
    /// </summary>
    internal class EnumerableCollectionView : CollectionView
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        // Set up a ListCollectionView over the snapshot.
        // We will delegate all CollectionView functionality to this view.
        internal EnumerableCollectionView(IEnumerable source)
            : base(source)
        {
            _snapshot = new ObservableCollection<object>();

            LoadSnapshotCore(source);

            if (_snapshot.Count > 0)
            {
                SetCurrent(_snapshot[0], 0, 1);
            }
            else
            {
                SetCurrent(null, -1, 0);
            }

            // If the source doesn't raise collection change events, try to detect changes by polling the enumerator.
            _pollForChanges = !(source is INotifyCollectionChanged);

            _view = new ListCollectionView(_snapshot);

            INotifyCollectionChanged incc = _view as INotifyCollectionChanged;
            incc.CollectionChanged += new NotifyCollectionChangedEventHandler(EnumerableCollectionView_OnViewChanged);

            INotifyPropertyChanged ipc = _view as INotifyPropertyChanged;
            ipc.PropertyChanged += new PropertyChangedEventHandler(EnumerableCollectionView_OnPropertyChanged);

            _view.CurrentChanging += new CurrentChangingEventHandler(EnumerableCollectionView_OnCurrentChanging);
            _view.CurrentChanged += new EventHandler<object>(EnumerableCollectionView_OnCurrentChanged);
        }

        //------------------------------------------------------
        //
        //  Interfaces
        //
        //------------------------------------------------------

        /// <summary>
        /// Gets or sets culture to use during sorting.
        /// </summary>
        public override CultureInfo Culture
        {
            get
            {
                return _view.Culture;
            }

            set
            {
                _view.Culture = value;
            }
        }

        /// <summary>
        /// Return true if the item belongs to this view.  No assumptions are
        /// made about the item. This method will behave similarly to IList.Contains().
        /// If the caller knows that the item belongs to the
        /// underlying collection, it is more efficient to call PassesFilter.
        /// </summary>
        /// <returns>True if the item belongs to this view.</returns>
        public override bool Contains(object item)
        {
            EnsureSnapshot();
            return _view.Contains(item);
        }

#if FEATURE_ICOLLECTIONVIEW_FILTER
        /// <summary>
        /// Set/get a filter callback to filter out items in collection.
        /// This property will always accept a filter, but the collection view for the
        /// underlying InnerList or ItemsSource may not actually support filtering.
        /// Please check <seealso cref="CanFilter"/>
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// Collections assigned to ItemsSource may not support filtering and could throw a NotSupportedException.
        /// Use <seealso cref="CanSort"/> property to test if sorting is supported before adding
        /// to SortDescriptions.
        /// </exception>
        public override Predicate<object> Filter
        {
            get
            {
                return _view.Filter;
            }

            set
            {
                _view.Filter = value;
            }
        }

        /// <summary>
        /// Test if this ICollectionView supports filtering before assigning
        /// a filter callback to <seealso cref="Filter"/>.
        /// </summary>
        public override bool CanFilter
        {
            get
            {
                return _view.CanFilter;
            }
        }
#endif

#if FEATURE_ICOLLECTIONVIEW_SORT
        /// <summary>
        /// Set/get Sort criteria to sort items in collection.
        /// </summary>
        /// <remarks>
        /// <p>
        /// Clear a sort criteria by assigning SortDescription.Empty to this property.
        /// One or more sort criteria in form of <seealso cref="SortDescription"/>
        /// can be used, each specifying a property and direction to sort by.
        /// </p>
        /// </remarks>
        /// <exception cref="NotSupportedException">
        /// Simpler implementations do not support sorting and will throw a NotSupportedException.
        /// Use <seealso cref="CanSort"/> property to test if sorting is supported before adding
        /// to SortDescriptions.
        /// </exception>
        public override SortDescriptionCollection SortDescriptions
        {
            get
            {
                return _view.SortDescriptions;
            }
        }

        /// <summary>
        /// Test if this ICollectionView supports sorting before adding
        /// to <seealso cref="SortDescriptions"/>.
        /// </summary>
        public override bool CanSort
        {
            get
            {
                return _view.CanSort;
            }
        }
#endif

#if FEATURE_ICOLLECTIONVIEW_GROUP
        /// <summary>
        /// Returns true if this view really supports grouping.
        /// When this returns false, the rest of the interface is ignored.
        /// </summary>
        public override bool CanGroup
        {
            get
            {
                return _view.CanGroup;
            }
        }

        /// <summary>
        /// The description of grouping, indexed by level.
        /// </summary>
        public override ObservableCollection<GroupDescription> GroupDescriptions
        {
            get
            {
                return _view.GroupDescriptions;
            }
        }

        /// <summary>
        /// The top-level groups, constructed according to the descriptions
        /// given in GroupDescriptions.
        /// </summary>
        public override ReadOnlyObservableCollection<object> Groups
        {
            get
            {
                return _view.Groups;
            }
        }
#endif

        /// <summary>
        /// Enter a Defer Cycle.
        /// Defer cycles are used to coalesce changes to the ICollectionView.
        /// </summary>
        /// <returns>An IDisposable deferral object.</returns>
        public override IDisposable DeferRefresh()
        {
            return _view.DeferRefresh();
        }

        /// <summary>
        /// Gets the current item.
        /// </summary>
        public override object CurrentItem
        {
            get
            {
                return _view.CurrentItem;
            }
        }

        /// <summary>
        /// Gets the ordinal position of the <seealso cref="CurrentItem"/> within the (optionally
        /// sorted and filtered) view.
        /// </summary>
        public override int CurrentPosition
        {
            get
            {
                return _view.CurrentPosition;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the currency is beyond the end (End-Of-File).
        /// </summary>
        public override bool IsCurrentAfterLast
        {
            get
            {
                return _view.IsCurrentAfterLast;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the currency is before the beginning (Beginning-Of-File).
        /// </summary>
        public override bool IsCurrentBeforeFirst
        {
            get
            {
                return _view.IsCurrentBeforeFirst;
            }
        }

        /// <summary>
        /// Move <seealso cref="CurrentItem"/> to the first item.
        /// </summary>
        /// <returns>True if <seealso cref="CurrentItem"/> points to an item within the view.</returns>
        public override bool MoveCurrentToFirst()
        {
            return _view.MoveCurrentToFirst();
        }

        /// <summary>
        /// Move <seealso cref="CurrentItem"/> to the previous item.
        /// </summary>
        /// <returns>True if <seealso cref="CurrentItem"/> points to an item within the view.</returns>
        public override bool MoveCurrentToPrevious()
        {
            return _view.MoveCurrentToPrevious();
        }

        /// <summary>
        /// Move <seealso cref="CurrentItem"/> to the next item.
        /// </summary>
        /// <returns>True if <seealso cref="CurrentItem"/> points to an item within the view.</returns>
        public override bool MoveCurrentToNext()
        {
            return _view.MoveCurrentToNext();
        }

        /// <summary>
        /// Move <seealso cref="CurrentItem"/> to the last item.
        /// </summary>
        /// <returns>True if <seealso cref="CurrentItem"/> points to an item within the view.</returns>
        public override bool MoveCurrentToLast()
        {
            return _view.MoveCurrentToLast();
        }

        /// <summary>
        /// Move <seealso cref="CurrentItem"/> to the given item.
        /// If the item is not found, move to BeforeFirst.
        /// </summary>
        /// <param name="item">Move CurrentItem to this item.</param>
        /// <returns>True if <seealso cref="CurrentItem"/> points to an item within the view.</returns>
        public override bool MoveCurrentTo(object item)
        {
            return _view.MoveCurrentTo(item);
        }

        /// <summary>
        /// Move <seealso cref="CurrentItem"/> to the item at the given index.
        /// </summary>
        /// <param name="position">Move CurrentItem to this index</param>
        /// <returns>True if <seealso cref="CurrentItem"/> points to an item within the view.</returns>
        public override bool MoveCurrentToPosition(int position)
        {
            // If the index is out of range here, I'll let the
            // _view be the one to make that determination.
            return _view.MoveCurrentToPosition(position);
        }

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        /// <summary>
        /// Gets the number of records (or -1, meaning "don't know").
        /// A virtualizing view should return the best estimate it can
        /// without de-virtualizing all the data.  A non-virtualizing view
        /// should return the exact count of its (filtered) data.
        /// </summary>
        public override int Count
        {
            get
            {
                EnsureSnapshot();
                return _view.Count;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                EnsureSnapshot();
                return (_view != null) ? _view.IsEmpty : true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this view needs to be refreshed.
        /// </summary>
        public override bool NeedsRefresh
        {
            get
            {
                return _view.NeedsRefresh;
            }
        }

        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        /// <summary>
        /// Return the index where the given item appears, or -1 if doesn't appear.
        /// </summary>
        /// <param name="item">data item</param>
        /// <returns>The index where the given item belongs, or -1 if this index is unknown.</returns>
        public override int IndexOf(object item)
        {
            EnsureSnapshot();
            return _view.IndexOf(item);
        }

#if FEATURE_ICOLLECTIONVIEW_FILTER
        /// <summary>
        /// Return true if the item belongs to this view.  The item is assumed to belong to the
        /// underlying DataCollection;  this method merely takes filters into account.
        /// It is commonly used during collection-changed notifications to determine if the added/removed
        /// item requires processing.
        /// Returns true if no filter is set on collection view.
        /// </summary>
        /// <returns>True if the item belongs to this view.</returns>
        public override bool PassesFilter(object item)
        {
            if (_view.CanFilter && _view.Filter != null)
            {
                return _view.Filter(item);
            }

            return true;
        }
#endif

        /// <summary>
        /// Retrieve item at the given zero-based index in this CollectionView.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if index is out of range
        /// </exception>
        /// <returns>Item at the given zero-based index in this CollectionView.</returns>
        public override object GetItemAt(int index)
        {
            EnsureSnapshot();
            return _view.GetItemAt(index);
        }

        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        /// <summary> Implementation of IEnumerable.GetEnumerator().
        /// This provides a way to enumerate the members of the collection
        /// without changing the currency.
        /// </summary>
        /// <returns>IEnumerator object that enumerates the items in this view.</returns>
        protected override IEnumerator GetEnumerator()
        {
            EnsureSnapshot();
            return (_view as IEnumerable).GetEnumerator();
        }

        /// <summary>
        /// Re-create the view over the associated IList
        /// </summary>
        /// <remarks>
        /// Any sorting and filtering will take effect during Refresh.
        /// </remarks>
        protected override void RefreshOverride()
        {
            LoadSnapshot(SourceCollection);
        }

        /// <summary>
        /// Processes a single collection change on the UI thread.
        /// </summary>
        /// <param name="args">
        /// The NotifyCollectionChangedEventArgs to be processed.
        /// </param>
        protected override void ProcessCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            // Apply the change to the snapshot
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (args.NewStartingIndex < 0 || _snapshot.Count <= args.NewStartingIndex)
                    { // Append
                        for (int i = 0; i < args.NewItems.Count; ++i)
                        {
                            _snapshot.Add(args.NewItems[i]);
                        }
                    }
                    else
                    { // Insert
                        for (int i = args.NewItems.Count - 1; i >= 0; --i)
                        {
                            _snapshot.Insert(args.NewStartingIndex, args.NewItems[i]);
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (args.OldStartingIndex < 0)
                    {
                        throw CollectionViewsError.EnumerableCollectionView.RemovedItemNotFound();
                    }

                    for (int i = args.OldItems.Count - 1, index = args.OldStartingIndex + i; i >= 0; --i, --index)
                    {
                        if (!object.Equals(args.OldItems[i], _snapshot[index]))
                        {
                            throw CollectionViewsError.CollectionView.ItemNotAtIndex("removed");
                        }

                        _snapshot.RemoveAt(index);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    for (int i = args.NewItems.Count - 1, index = args.NewStartingIndex + i; i >= 0; --i, --index)
                    {
                        if (!object.Equals(args.OldItems[i], _snapshot[index]))
                        {
                            throw CollectionViewsError.CollectionView.ItemNotAtIndex("replaced");
                        }

                        _snapshot[index] = args.NewItems[i];
                    }

                    break;

                case NotifyCollectionChangedAction.Reset:
                    LoadSnapshot(SourceCollection);
                    break;
            }
        }

        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        // Load a snapshot of the contents of the IEnumerable into the ObservableCollection.
        private void LoadSnapshot(IEnumerable source)
        {
            // Force currency off the collection (gives user a chance to save dirty information).
            OnCurrentChanging();

            // Remember the values of the scalar properties, so that we can restore
            // them and raise events after reloading the data
            object oldCurrentItem = CurrentItem;
            int oldCurrentPosition = CurrentPosition;
            bool oldIsCurrentBeforeFirst = IsCurrentBeforeFirst;
            bool oldIsCurrentAfterLast = IsCurrentAfterLast;

            // Reload the data
            LoadSnapshotCore(source);

            // Tell listeners everything has changed
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            OnCurrentChanged();

            if (IsCurrentAfterLast != oldIsCurrentAfterLast)
            {
                OnPropertyChanged(new PropertyChangedEventArgs(IsCurrentAfterLastPropertyName));
            }

            if (IsCurrentBeforeFirst != oldIsCurrentBeforeFirst)
            {
                OnPropertyChanged(new PropertyChangedEventArgs(IsCurrentBeforeFirstPropertyName));
            }

            if (oldCurrentPosition != CurrentPosition)
            {
                OnPropertyChanged(new PropertyChangedEventArgs(CurrentPositionPropertyName));
            }

            if (oldCurrentItem != CurrentItem)
            {
                OnPropertyChanged(new PropertyChangedEventArgs(CurrentItemPropertyName));
            }
        }

        private void LoadSnapshotCore(IEnumerable source)
        {
            _trackingEnumerator = source.GetEnumerator();

            using (IgnoreViewEvents())
            {
                _snapshot.Clear();

                while (_trackingEnumerator.MoveNext())
                {
                    _snapshot.Add(_trackingEnumerator.Current);
                }
            }
        }

        // If the IEnumerable has changed, bring the snapshot up to date.
        // (This isn't necessary if the IEnumerable is also INotifyCollectionChanged
        // because we keep the snapshot in sync incrementally.)
        private void EnsureSnapshot()
        {
            if (_pollForChanges)
            {
                try
                {
                    _trackingEnumerator.MoveNext();
                }
                catch (InvalidOperationException)
                {
                    // This "feature" is necessarily incomplete (we cannot detect
                    // the changes when they happen, only as a side-effect of some
                    // later operation), and inconsistent (none of the other
                    // collection views does this).  Changing a collection without
                    // raising a notification is not supported.

                    // Collection was changed - start over with a new enumerator
                    LoadSnapshotCore(SourceCollection);
                }
            }
        }

        private IDisposable IgnoreViewEvents()
        {
            return new IgnoreViewEventsHelper(this);
        }

        private void BeginIgnoreEvents()
        {
            _ignoreEventsLevel++;
        }

        private void EndIgnoreEvents()
        {
            _ignoreEventsLevel--;
        }

        // forward events from the internal view to our own listeners
        private void EnumerableCollectionView_OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_ignoreEventsLevel != 0)
            {
                return;
            }

#if FEATURE_IEDITABLECOLLECTIONVIEW
            // Also ignore ListCollectionView's property change notifications for
            // IEditableCollectionView's properties
            switch (args.PropertyName)
            {
                case ListCollectionView.CanAddNewPropertyName:
                case ListCollectionView.CanCancelEditPropertyName:
                case ListCollectionView.CanRemovePropertyName:
                case ListCollectionView.CurrentAddItemPropertyName:
                case ListCollectionView.CurrentEditItemPropertyName:
                case ListCollectionView.IsAddingNewPropertyName:
                case ListCollectionView.IsEditingItemPropertyName:
                    return;
            }
#endif
            OnPropertyChanged(args);
        }

        private void EnumerableCollectionView_OnViewChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (_ignoreEventsLevel != 0)
            {
                return;
            }

            OnCollectionChanged(args);
        }

        private void EnumerableCollectionView_OnCurrentChanging(object sender, CurrentChangingEventArgs args)
        {
            if (_ignoreEventsLevel != 0)
            {
                return;
            }

            OnCurrentChanging(args);
        }

        private void EnumerableCollectionView_OnCurrentChanged(object sender, object args)
        {
            if (_ignoreEventsLevel != 0)
            {
                return;
            }

            OnCurrentChanged();
        }

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------
        private ListCollectionView _view;
        private ObservableCollection<object> _snapshot;
        private IEnumerator _trackingEnumerator;
        private int _ignoreEventsLevel;
        private bool _pollForChanges;

        private class IgnoreViewEventsHelper : IDisposable
        {
            public IgnoreViewEventsHelper(EnumerableCollectionView parent)
            {
                _parent = parent;
                _parent.BeginIgnoreEvents();
            }

            public void Dispose()
            {
                if (_parent != null)
                {
                    _parent.EndIgnoreEvents();
                    _parent = null;
                }

                GC.SuppressFinalize(this);
            }

            private EnumerableCollectionView _parent;
        }
    }
}