// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using CommunityToolkit.WinUI.Utilities;
using Microsoft.UI.Xaml.Data;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace CommunityToolkit.WinUI.UI.Data.Utilities
{
    internal abstract class CollectionView : ICollectionView, INotifyCollectionChanged, INotifyPropertyChanged
    {
        /// <summary>
        /// Raise this event before changing currency.
        /// </summary>
        public virtual event CurrentChangingEventHandler CurrentChanging;

        /// <summary>
        /// Raise this event after changing currency.
        /// </summary>
        public virtual event EventHandler<object> CurrentChanged;

        /// <summary>
        /// Unused VectorChanged event from the ICollectionView interface.
        /// </summary>
        event VectorChangedEventHandler<object> IObservableVector<object>.VectorChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        public CollectionView(IEnumerable collection)
        {
            _sourceCollection = collection ?? throw new ArgumentNullException("collection");

            // forward collection change events from underlying collection to our listeners.
            INotifyCollectionChanged incc = collection as INotifyCollectionChanged;
            if (incc != null)
            {
                _sourceWeakEventListener =
                    new WeakEventListener<CollectionView, object, NotifyCollectionChangedEventArgs>(this)
                    {
                        // Call the actual collection changed event
                        OnEventAction = (source, changed, arg) => OnCollectionChanged(source, arg),

                        // The source doesn't exist anymore
                        OnDetachAction = (listener) => incc.CollectionChanged -= _sourceWeakEventListener.OnEvent
                    };
                incc.CollectionChanged += _sourceWeakEventListener.OnEvent;
            }

            _currentItem = null;
            _currentPosition = -1;
            SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, _currentPosition < 0);
            SetFlag(CollectionViewFlags.IsCurrentAfterLast, _currentPosition < 0);
            SetFlag(CollectionViewFlags.CachedIsEmpty, _currentPosition < 0);
        }

#if FEATURE_ICOLLECTIONVIEW_FILTER
        /// <summary>
        /// Gets or sets a callback used to determine if an item is suitable for inclusion in the view.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// Simpler implementations do not support filtering and will throw a NotSupportedException.
        /// Use <seealso cref="CanFilter"/> property to test if filtering is supported before
        /// assigning a non-null value.
        /// </exception>
        public virtual Predicate<object> Filter
        {
            get
            {
                return _filter;
            }

            set
            {
                if (!CanFilter)
                {
                    throw new NotSupportedException();
                }

                _filter = value;

                RefreshOrDefer();
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this ICollectionView can do any filtering.
        /// When false, set <seealso cref="Filter"/> will throw an exception.
        /// </summary>
        public abstract bool CanFilter
        {
            get;
        }
#endif

#if FEATURE_ICOLLECTIONVIEW_SORT
        /// <summary>
        /// Collection of Sort criteria to sort items in this view over the SourceCollection.
        /// </summary>
        /// <remarks>
        /// <p>
        /// Simpler implementations do not support sorting and will return an empty
        /// and immutable / read-only SortDescription collection.
        /// Attempting to modify such a collection will cause NotSupportedException.
        /// Use <seealso cref="CanSort"/> property on CollectionView to test if sorting is supported
        /// before modifying the returned collection.
        /// </p>
        /// <p>
        /// One or more sort criteria in form of SortDescription can be added, each specifying a property and direction to sort by.
        /// </p>
        /// </remarks>
        public abstract SortDescriptionCollection SortDescriptions
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether this ICollectionView supports sorting.
        /// </summary>
        public abstract bool CanSort
        {
            get;
        }
#endif

#if FEATURE_ICOLLECTIONVIEW_GROUP
        /// <summary>
        /// Gets a value indicating whether this view really supports grouping.
        /// When this returns false, the rest of the interface is ignored.
        /// </summary>
        public abstract bool CanGroup
        {
            get;
        }

        /// <summary>
        /// Gets the description of grouping, indexed by level.
        /// </summary>
        public abstract ObservableCollection<GroupDescription> GroupDescriptions
        {
            get;
        }

        /// <summary>
        /// Gets the top-level groups, constructed according to the descriptions
        /// given in GroupDescriptions.
        /// </summary>
        public abstract ReadOnlyObservableCollection<object> Groups
        {
            get;
        }
#endif

        public virtual IObservableVector<object> CollectionGroups
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the Culture to use during sorting.
        /// </summary>
        public virtual CultureInfo Culture
        {
            get
            {
                return _culture;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (_culture != value)
                {
                    _culture = value;
                    OnPropertyChanged(CulturePropertyName);
                }
            }
        }

        /// <summary>
        /// Enter a Defer Cycle.
        /// Defer cycles are used to coalesce changes to the ICollectionView.
        /// </summary>
        /// <returns>An IDisposable deferral object.</returns>
        public virtual IDisposable DeferRefresh()
        {
#if FEATURE_IEDITABLECOLLECTIONVIEW
            IEditableCollectionView ecv = this as IEditableCollectionView;
            if (ecv != null && (ecv.IsAddingNew || ecv.IsEditingItem))
            {
                throw CollectionViewsError.CollectionView.MemberNotAllowedDuringAddOrEdit("DeferRefresh");
            }
#endif
            _deferLevel++;
            return new DeferHelper(this);
        }

        /// <summary>
        /// Gets the "current item" for this view
        /// </summary>
        /// <remarks>
        /// Only wrapper classes (those that pass currency handling calls to another internal
        /// CollectionView) should override CurrentItem; all other derived classes
        /// should use SetCurrent() to update the current values stored in the base class.
        /// </remarks>
        public virtual object CurrentItem
        {
            get
            {
                VerifyRefreshNotDeferred();

                return _currentItem;
            }
        }

        /// <summary>
        /// Gets the ordinal position of the <seealso cref="CurrentItem"/> within the (optionally
        /// sorted and filtered) view.
        /// </summary>
        /// <returns>
        /// -1 if the CurrentPosition is unknown, because the collection does not have an
        /// effective notion of indices, or because CurrentPosition is being forcibly changed
        /// due to a CollectionChange.
        /// </returns>
        /// <remarks>
        /// Only wrapper classes (those that pass currency handling calls to another internal
        /// CollectionView) should override CurrenPosition; all other derived classes
        /// should use SetCurrent() to update the current values stored in the base class.
        /// </remarks>
        public virtual int CurrentPosition
        {
            get
            {
                VerifyRefreshNotDeferred();

                return _currentPosition;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the source has more items
        /// </summary>
        public bool HasMoreItems
        {
            get
            {
                ISupportIncrementalLoading sourceAsSupportIncrementalLoading = _sourceCollection as ISupportIncrementalLoading;
                return sourceAsSupportIncrementalLoading?.HasMoreItems ?? false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether <seealso cref="CurrentItem"/> is beyond the end (End-Of-File).
        /// </summary>
        public virtual bool IsCurrentAfterLast
        {
            get
            {
                VerifyRefreshNotDeferred();

                return CheckFlag(CollectionViewFlags.IsCurrentAfterLast);
            }
        }

        /// <summary>
        /// Gets a value indicating whether <seealso cref="CurrentItem"/> is before the beginning (Beginning-Of-File).
        /// </summary>
        public virtual bool IsCurrentBeforeFirst
        {
            get
            {
                VerifyRefreshNotDeferred();

                return CheckFlag(CollectionViewFlags.IsCurrentBeforeFirst);
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the underlying collection.
        /// </summary>
        public virtual IEnumerable SourceCollection
        {
            get
            {
                return _sourceCollection;
            }
        }

        public object this[int index]
        {
            get
            {
                return GetItemAt(index);
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void Add(object item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return true if the item belongs to this view.  No assumptions are
        /// made about the item. This method will behave similarly to IList.Contains().
        /// </summary>
        /// <remarks>
        /// <p>If the caller knows that the item belongs to the
        /// underlying collection, it is more efficient to call PassesFilter.
        /// If the underlying collection is only of type IEnumerable, this method
        /// is a O(N) operation</p>
        /// </remarks>
        /// <returns>True if the item belongs to this view.</returns>
        public abstract bool Contains(object item);

        public void CopyTo(object[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Invoked to load more items from the source.
        /// </summary>
        /// <param name="count">number of items to load</param>
        /// <returns>Async operation of LoadMoreItemsResult</returns>
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            ISupportIncrementalLoading sourceAsSupportIncrementalLoading = _sourceCollection as ISupportIncrementalLoading;
            return sourceAsSupportIncrementalLoading?.LoadMoreItemsAsync(count);
        }

        /// <summary>
        /// Move <seealso cref="CurrentItem"/> to the first item.
        /// </summary>
        /// <returns>True if <seealso cref="CurrentItem"/> points to an item within the view.</returns>
        public virtual bool MoveCurrentToFirst()
        {
            VerifyRefreshNotDeferred();

            return MoveCurrentToPosition(0);
        }

        /// <summary>
        /// Move <seealso cref="CurrentItem"/> to the last item.
        /// </summary>
        /// <returns>True if <seealso cref="CurrentItem"/> points to an item within the view.</returns>
        public virtual bool MoveCurrentToLast()
        {
            VerifyRefreshNotDeferred();

            return MoveCurrentToPosition(Count - 1);
        }

        /// <summary>
        /// Move <seealso cref="CurrentItem"/> to the next item.
        /// </summary>
        /// <returns>True if <seealso cref="CurrentItem"/> points to an item within the view.</returns>
        public virtual bool MoveCurrentToNext()
        {
            VerifyRefreshNotDeferred();

            if (CurrentPosition < Count)
            {
                return MoveCurrentToPosition(CurrentPosition + 1);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Move <seealso cref="CurrentItem"/> to the previous item.
        /// </summary>
        /// <returns>True if <seealso cref="CurrentItem"/> points to an item within the view.</returns>
        public virtual bool MoveCurrentToPrevious()
        {
            VerifyRefreshNotDeferred();

            if (CurrentPosition >= 0)
            {
                return MoveCurrentToPosition(CurrentPosition - 1);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Move <seealso cref="CurrentItem"/> to the given item.
        /// If the item is not found, move to BeforeFirst.
        /// </summary>
        /// <param name="item">Move CurrentItem to this item.</param>
        /// <returns>True if <seealso cref="CurrentItem"/> points to an item within the view.</returns>
        public virtual bool MoveCurrentTo(object item)
        {
            VerifyRefreshNotDeferred();

            // if already on item, don't do anything
            if (object.Equals(CurrentItem, item))
            {
                // also check that we're not fooled by a false null _currentItem
                if (item != null || IsCurrentInView)
                {
                    return IsCurrentInView;
                }
            }

            int index = -1;
#if FEATURE_IEDITABLECOLLECTIONVIEW
            IEditableCollectionView ecv = this as IEditableCollectionView;
            bool isNewItem = ecv != null && ecv.IsAddingNew && object.Equals(item, ecv.CurrentAddItem);
#elif FEATURE_ICOLLECTIONVIEW_FILTER
            bool isNewItem = false;
#endif

#if FEATURE_ICOLLECTIONVIEW_FILTER
            if (isNewItem || item == null || PassesFilter(item))
#endif
            {
                // if the item is not found IndexOf() will return -1, and
                // the MoveCurrentToPosition() below will move current to BeforeFirst
                index = IndexOf(item);
            }

            return MoveCurrentToPosition(index);
        }

        /// <summary>
        /// Move <seealso cref="CurrentItem"/> to the item at the given index.
        /// </summary>
        /// <param name="position">Move CurrentItem to this index</param>
        /// <returns>True if <seealso cref="CurrentItem"/> points to an item within the view.</returns>
        public abstract bool MoveCurrentToPosition(int position);

        /// <summary>
        /// Re-create the view, using any SortDescriptions and/or Filter.
        /// </summary>
        public virtual void Refresh()
        {
#if FEATURE_IEDITABLECOLLECTIONVIEW
            IEditableCollectionView ecv = this as IEditableCollectionView;
            if (ecv != null && (ecv.IsAddingNew || ecv.IsEditingItem))
            {
                throw CollectionViewsError.CollectionView.MemberNotAllowedDuringAddOrEdit("Refresh");
            }
#endif
            RefreshInternal();
        }

        public bool Remove(object item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an object that enumerates the items in this view.
        /// </summary>
        /// <returns>IEnumerator object that enumerates the items in this view.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            throw new NotImplementedException();
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
        public abstract bool PassesFilter(object item);
#endif

        /// <summary>
        /// Return the index where the given item belongs, or -1 if this index is unknown.
        /// </summary>
        /// <remarks>
        /// If this method returns an index other than -1, it must always be true that
        /// view[index-1] &lt; item &lt;= view[index], where the comparisons are done via
        /// the view's IComparer.Compare method (if any).
        /// (This method is used by a listener's (e.g. System.Windows.Controls.ItemsControl)
        /// CollectionChanged event handler to speed up its reaction to insertion and deletion of items.
        /// If IndexOf is  not implemented, a listener does a binary search using IComparer.Compare.)
        /// </remarks>
        /// <param name="item">data item</param>
        /// <returns>The index where the given item belongs, or -1 if this index is unknown.</returns>
        public abstract int IndexOf(object item);

        /// <summary>
        /// Retrieve item at the given zero-based index in this CollectionView.
        /// </summary>
        /// <remarks>
        /// <p>The index is evaluated with any SortDescriptions or Filter being set on this CollectionView.
        /// If the underlying collection is only of type IEnumerable, this method
        /// is a O(N) operation.</p>
        /// <p>When deriving from CollectionView, override this method to provide
        /// a more efficient implementation.</p>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if index is out of range
        /// </exception>
        /// <returns>Item at the given zero-based index in this CollectionView.</returns>
        public abstract object GetItemAt(int index);

        /// <summary>
        /// Gets the number of items (or -1, meaning "don't know");
        /// if a Filter is set, this counts only items that pass the filter.
        /// </summary>
        /// <remarks>
        /// <p>If the underlying collection is only of type IEnumerable, this count
        /// is a O(N) operation; this Count value will be cached until the
        /// collection changes again.</p>
        /// <p>When deriving from CollectionView, override this property to provide
        /// a more efficient implementation.</p>
        /// </remarks>
        public abstract int Count
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the resulting (filtered) view is empty.
        /// </summary>
        public abstract bool IsEmpty
        {
            get;
        }

        /// <summary>
        /// Gets an object that compares items in this view.
        /// </summary>
        public virtual IComparer Comparer
        {
            get
            {
                return this as IComparer;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this view needs to be refreshed.
        /// </summary>
        public virtual bool NeedsRefresh
        {
            get
            {
                return CheckFlag(CollectionViewFlags.NeedsRefresh);
            }
        }

        /// <summary>
        /// Raise this event when the (filtered) view changes
        /// </summary>
        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Raises a PropertyChanged event (per <see cref="INotifyPropertyChanged" />).
        /// </summary>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// PropertyChanged event (per <see cref="INotifyPropertyChanged" />).
        /// </summary>
        public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Re-create the view, using any SortDescriptions and/or Filter.
        /// </summary>
        protected abstract void RefreshOverride();

        /// <summary>
        /// Returns an object that enumerates the items in this view.
        /// </summary>
        /// <returns>An object that enumerates the items in this view.</returns>
        protected abstract IEnumerator GetEnumerator();

        /// <summary>
        /// Notify listeners that this View has changed
        /// </summary>
        /// <remarks>
        /// CollectionViews (and sub-classes) should take their filter/sort/grouping
        /// into account before calling this method to forward CollectionChanged events.
        /// </remarks>
        /// <param name="args">
        /// The NotifyCollectionChangedEventArgs to be passed to the EventHandler
        /// </param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            unchecked
            {
                // invalidate enumerators because of a change
                ++_timestamp;
            }

            CollectionChanged?.Invoke(this, args);

            // Collection changes change the count unless an item is being
            // replaced or moved within the collection.
            if (args.Action != global::System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                OnPropertyChanged(CountPropertyName);
            }

            bool isEmpty = IsEmpty;
            if (isEmpty != CheckFlag(CollectionViewFlags.CachedIsEmpty))
            {
                SetFlag(CollectionViewFlags.CachedIsEmpty, isEmpty);
                OnPropertyChanged(IsEmptyPropertyName);
            }
        }

        /// <summary>
        /// set CurrentItem and CurrentPosition, no questions asked!
        /// </summary>
        /// <remarks>
        /// CollectionViews (and sub-classes) should use this method to update
        /// the Current__ values.
        /// </remarks>
        protected void SetCurrent(object newItem, int newPosition)
        {
            int count = (newItem != null) ? 0 : IsEmpty ? 0 : Count;
            SetCurrent(newItem, newPosition, count);
        }

        /// <summary>
        /// set CurrentItem and CurrentPosition, no questions asked!
        /// </summary>
        /// <remarks>
        /// This method can be called from a constructor - it does not call
        /// any virtuals.  The 'count' parameter is substitute for the real Count,
        /// used only when newItem is null.
        /// In that case, this method sets IsCurrentAfterLast to true if and only
        /// if newPosition >= count.  This distinguishes between a null belonging
        /// to the view and the dummy null when CurrentPosition is past the end.
        /// </remarks>
        protected void SetCurrent(object newItem, int newPosition, int count)
        {
            if (newItem != null)
            {
                // non-null item implies position is within range.
                // We ignore count - it's just a placeholder
                SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, false);
                SetFlag(CollectionViewFlags.IsCurrentAfterLast, false);
            }
            else if (count == 0)
            {
                // empty collection - by convention both flags are true and position is -1
                SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, true);
                SetFlag(CollectionViewFlags.IsCurrentAfterLast, true);
                newPosition = -1;
            }
            else
            {
                // null item, possibly within range.
                SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, newPosition < 0);
                SetFlag(CollectionViewFlags.IsCurrentAfterLast, newPosition >= count);
            }

            _currentItem = newItem;
            _currentPosition = newPosition;
        }

        /// <summary>
        /// Ask listeners (via <seealso cref="ICollectionView.CurrentChanging"/> event) if it's OK to change currency
        /// </summary>
        /// <returns>false if a listener cancels the change, true otherwise</returns>
        protected bool OKToChangeCurrent()
        {
            CurrentChangingEventArgs args = new CurrentChangingEventArgs();
            OnCurrentChanging(args);
            return !args.Cancel;
        }

        /// <summary>
        /// Raise a CurrentChanging event that is not cancelable.
        /// Internally, CurrentPosition is set to -1.
        /// This is called by CollectionChanges (Remove and Refresh) that affect the CurrentItem.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// This CurrentChanging event cannot be canceled.
        /// </exception>
        protected void OnCurrentChanging()
        {
            _currentPosition = -1;
            OnCurrentChanging(UncancelableCurrentChangingEventArgs);
        }

        /// <summary>
        /// Raises the CurrentChanging event
        /// </summary>
        /// <param name="args">
        ///     CancelEventArgs used by the consumer of the event.  args.Cancel will
        ///     be true after this call if the CurrentItem should not be changed for
        ///     any reason.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     This CurrentChanging event cannot be canceled.
        /// </exception>
        protected virtual void OnCurrentChanging(CurrentChangingEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (_currentChangedMonitor.Busy)
            {
                if (args.IsCancelable)
                {
                    args.Cancel = true;
                }

                return;
            }

            CurrentChanging?.Invoke(this, args);
        }

        /// <summary>
        /// Raises the CurrentChanged event
        /// </summary>
        protected virtual void OnCurrentChanged()
        {
            if (CurrentChanged != null && _currentChangedMonitor.Enter())
            {
                using (_currentChangedMonitor)
                {
                    CurrentChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Must be implemented by the derived classes to process a single change on the
        /// UI thread.  The UI thread will have already been entered by now.
        /// </summary>
        /// <param name="args">
        /// The NotifyCollectionChangedEventArgs to be processed.
        /// </param>
        protected abstract void ProcessCollectionChanged(NotifyCollectionChangedEventArgs args);

        /// <summary>
        /// Handle CollectionChanged events.
        /// Calls ProcessCollectionChanged() or posts the change to the Dispatcher to process on the correct thread.
        /// </summary>
        /// <remarks>
        /// User should override <see cref="ProcessCollectionChanged"/>
        /// </remarks>
        protected void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (CheckFlag(CollectionViewFlags.ShouldProcessCollectionChanged))
            {
                ProcessCollectionChanged(args);
            }
        }

        /// <summary>
        ///     Refresh, or mark that refresh is needed when defer cycle completes.
        /// </summary>
        protected void RefreshOrDefer()
        {
            if (IsRefreshDeferred)
            {
                SetFlag(CollectionViewFlags.NeedsRefresh, true);
            }
            else
            {
                RefreshInternal();
            }
        }

        //------------------------------------------------------
        //
        //  Protected Properties
        //
        //------------------------------------------------------

        /// <summary>
        /// Gets a value indicating whether isRefreshDeferred there is still an outstanding
        /// DeferRefresh in use. If at all possible, derived classes should not call Refresh
        /// if IsRefreshDeferred is true.
        /// </summary>
        protected bool IsRefreshDeferred
        {
            get
            {
                return _deferLevel > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether CurrentItem and CurrentPosition are
        /// up-to-date with the state and content of the collection.
        /// </summary>
        protected bool IsCurrentInSync
        {
            get
            {
                if (IsCurrentInView)
                {
                    return GetItemAt(CurrentPosition) == CurrentItem;
                }
                else
                {
                    return CurrentItem == null;
                }
            }
        }

        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------

        /// <summary>
        /// Returns type of the items in the source collection.
        /// </summary>
        /// <returns>Type of the items in the source collection.</returns>
        internal Type GetItemType(bool useRepresentativeItem)
        {
            Type collectionType = SourceCollection.GetType();
            Type[] interfaces = collectionType.GetInterfaces();

            // Look for IEnumerable<T>.  All generic collections should implement
            // this.  We loop through the interface list, rather than call
            // GetInterface(IEnumerableT), so that we handle an ambiguous match
            // (by using the first match) without an exception.
            for (int i = 0; i < interfaces.Length; ++i)
            {
                Type interfaceType = interfaces[i];
                if (interfaceType.Name == IEnumerableT)
                {
                    // found IEnumerable<>, extract T
                    Type[] typeParameters = interfaceType.GetGenericArguments();
                    if (typeParameters.Length == 1)
                    {
                        return typeParameters[0];
                    }
                }
            }

            // No generic information found.  Use a representative item instead.
            if (useRepresentativeItem)
            {
                // get type of a representative item
                object item = GetRepresentativeItem();
                if (item != null)
                {
                    return item.GetType();
                }
            }

            return null;
        }

        internal object GetRepresentativeItem()
        {
            if (IsEmpty)
            {
                return null;
            }

            IEnumerator ie = this.GetEnumerator();
            while (ie.MoveNext())
            {
                object item = ie.Current;
                if (item != null)
                {
                    return item;
                }
            }

            return null;
        }

        internal void RefreshInternal()
        {
            RefreshOverride();

            SetFlag(CollectionViewFlags.NeedsRefresh, false);
        }

        // helper to validate that we are not in the middle of a DeferRefresh
        // and throw if that is the case.
        internal void VerifyRefreshNotDeferred()
        {
            // If the Refresh is being deferred to change filtering or sorting of the
            // data by this CollectionView, then CollectionView will not reflect the correct
            // state of the underlying data.
            if (IsRefreshDeferred)
            {
                throw CollectionViewsError.CollectionView.NoAccessWhileChangesAreDeferred();
            }
        }

        //------------------------------------------------------
        //
        //  Internal Properties
        //
        //------------------------------------------------------
        internal SimpleMonitor CurrentChangedMonitor
        {
            get
            {
                return _currentChangedMonitor;
            }
        }

        internal object SyncRoot
        {
            get
            {
                ICollection collection = SourceCollection as ICollection;
                if (collection != null && collection.SyncRoot != null)
                {
                    return collection.SyncRoot;
                }
                else
                {
                    return SourceCollection;
                }
            }
        }

        // Timestamp is used by the PlaceholderAwareEnumerator to determine if a
        // collection change has occurred since the enumerator began.  (If so,
        // MoveNext should throw.)
        internal int Timestamp
        {
            get
            {
                return _timestamp;
            }
        }

        //------------------------------------------------------
        //
        //  Internal Types
        //
        //------------------------------------------------------
        internal class PlaceholderAwareEnumerator : IEnumerator
        {
            private enum Position
            {
                /// <summary>
                /// Whether the position is before the new item
                /// </summary>
                BeforeNewItem,

                /// <summary>
                /// Whether the position is on the new item that is being created
                /// </summary>
                OnNewItem,

                /// <summary>
                /// Whether the position is after the new item
                /// </summary>
                AfterNewItem
            }

            public PlaceholderAwareEnumerator(CollectionView collectionView, IEnumerator baseEnumerator, object newItem)
            {
                _collectionView = collectionView;
                _timestamp = collectionView.Timestamp;
                _baseEnumerator = baseEnumerator;
                _newItem = newItem;
            }

            public bool MoveNext()
            {
                if (_timestamp != _collectionView.Timestamp)
                {
                    throw CollectionViewsError.CollectionView.EnumeratorVersionChanged();
                }

                if (_position == Position.BeforeNewItem)
                {
                    if (_baseEnumerator.MoveNext() &&
                        (_newItem == NoNewItem || _baseEnumerator.Current != _newItem || _baseEnumerator.MoveNext()))
                    {
                        // advance base, skipping the new item
                    }
                    else if (_newItem != NoNewItem)
                    {
                        // if base has reached the end, move to new item
                        _position = Position.OnNewItem;
                    }
                    else
                    {
                        return false;
                    }

                    return true;
                }

                // in all other cases, simply advance base, skipping the new item
                _position = Position.AfterNewItem;
                return _baseEnumerator.MoveNext() &&
                       (_newItem == NoNewItem || _baseEnumerator.Current != _newItem || _baseEnumerator.MoveNext());
            }

            public object Current
            {
                get
                {
                    return (_position == Position.OnNewItem) ? _newItem : _baseEnumerator.Current;
                }
            }

            public void Reset()
            {
                _position = Position.BeforeNewItem;
                _baseEnumerator.Reset();
            }

            private CollectionView _collectionView;
            private IEnumerator _baseEnumerator;
            private Position _position;
            private object _newItem;
            private int _timestamp;
        }

        //------------------------------------------------------
        //
        //  Private Properties
        //
        //------------------------------------------------------
        private bool IsCurrentInView
        {
            get
            {
                VerifyRefreshNotDeferred();
                return CurrentPosition >= 0 && CurrentPosition < Count;
            }
        }

        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        // returns true if ANY flag in flags is set.
        private bool CheckFlag(CollectionViewFlags flags)
        {
            return (_flags & flags) != 0;
        }

        private void SetFlag(CollectionViewFlags flags, bool value)
        {
            if (value)
            {
                _flags = _flags | flags;
            }
            else
            {
                _flags = _flags & ~flags;
            }
        }

        private void EndDefer()
        {
            _deferLevel--;

            if (_deferLevel == 0 && CheckFlag(CollectionViewFlags.NeedsRefresh))
            {
                Refresh();
            }
        }

        /// <summary>
        /// Helper to raise a PropertyChanged event  />).
        /// </summary>
        private void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        //------------------------------------------------------
        //
        //  Private Types
        //
        //------------------------------------------------------

        /// <summary>
        /// IDisposable deferral object.
        /// </summary>
        private class DeferHelper : IDisposable
        {
            public DeferHelper(CollectionView collectionView)
            {
                _collectionView = collectionView;
            }

            public void Dispose()
            {
                if (_collectionView != null)
                {
                    _collectionView.EndDefer();
                    _collectionView = null;
                }

                GC.SuppressFinalize(this);
            }

            private CollectionView _collectionView;
        }

        // this class helps prevent reentrant calls
        internal class SimpleMonitor : IDisposable
        {
            public bool Enter()
            {
                if (_entered)
                {
                    return false;
                }

                _entered = true;
                return true;
            }

            public void Dispose()
            {
                _entered = false;
                GC.SuppressFinalize(this);
            }

            public bool Busy
            {
                get
                {
                    return _entered;
                }
            }

            private bool _entered;
        }

        // Private members and types
        [Flags]
        private enum CollectionViewFlags
        {
            ShouldProcessCollectionChanged = 0x4,
            IsCurrentBeforeFirst = 0x8,
            IsCurrentAfterLast = 0x10,
            NeedsRefresh = 0x80,
            CachedIsEmpty = 0x200,
        }

        // Property names
        internal const string CountPropertyName = "Count";
        internal const string IsEmptyPropertyName = "IsEmpty";
        internal const string CulturePropertyName = "Culture";
        internal const string CurrentPositionPropertyName = "CurrentPosition";
        internal const string CurrentItemPropertyName = "CurrentItem";
        internal const string IsCurrentBeforeFirstPropertyName = "IsCurrentBeforeFirst";
        internal const string IsCurrentAfterLastPropertyName = "IsCurrentAfterLast";

        // since there's nothing in the uncancelable event args that is mutable,
        // just create one instance to be used universally.
        private static readonly CurrentChangingEventArgs UncancelableCurrentChangingEventArgs = new CurrentChangingEventArgs(false);
        private static readonly string IEnumerableT = typeof(IEnumerable<>).Name;
        internal static readonly object NoNewItem = new object();

        // State
        private IEnumerable _sourceCollection;
        private CollectionViewFlags _flags = CollectionViewFlags.ShouldProcessCollectionChanged | CollectionViewFlags.NeedsRefresh;
        private int _timestamp;
        private object _currentItem;
        private int _currentPosition;
        private CultureInfo _culture;
        private int _deferLevel;
        private SimpleMonitor _currentChangedMonitor = new SimpleMonitor();
        private WeakEventListener<CollectionView, object, NotifyCollectionChangedEventArgs> _sourceWeakEventListener;
#if FEATURE_ICOLLECTIONVIEW_FILTER
        private Predicate<object> _filter;
#endif
    }
}