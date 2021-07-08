// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
#if FEATURE_IEDITABLECOLLECTIONVIEW
using System.Reflection; // ConstructorInfo
#endif

using DiagnosticsDebug = System.Diagnostics.Debug;

namespace CommunityToolkit.WinUI.UI.Data.Utilities
{
    /// <summary>
    /// A collection view implementation that supports an IList source.
    /// </summary>
    internal class ListCollectionView : CollectionView, IComparer<object>
#if FEATURE_IEDITABLECOLLECTIONVIEW
        , IEditableCollectionView
#endif
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCollectionView"/> class.
        /// </summary>
        /// <param name="list">Underlying IList</param>
        public ListCollectionView(IList list)
            : base(list)
        {
            _internalList = list;

#if FEATURE_IEDITABLECOLLECTIONVIEW
            RefreshCanAddNew();
            RefreshCanRemove();
            RefreshCanCancelEdit();
#endif
            if (InternalList.Count == 0)
            {
                // don't call virtual IsEmpty in ctor
                SetCurrent(null, -1, 0);
            }
            else
            {
                SetCurrent(InternalList[0], 0, 1);
            }

#if FEATURE_ICOLLECTIONVIEW_GROUP
            _group = new CollectionViewGroupRoot(this);
            _group.GroupDescriptionChanged += new EventHandler(OnGroupDescriptionChanged);
            ((INotifyCollectionChanged)_group).CollectionChanged += new NotifyCollectionChangedEventHandler(OnGroupChanged);
            ((INotifyCollectionChanged)_group.GroupDescriptions).CollectionChanged += new NotifyCollectionChangedEventHandler(OnGroupByChanged);
#endif
        }

        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        /// <summary>
        /// Re-create the view over the associated IList
        /// </summary>
        /// <remarks>
        /// Any sorting and filtering will take effect during Refresh.
        /// </remarks>
        protected override void RefreshOverride()
        {
            object oldCurrentItem = CurrentItem;
            int oldCurrentPosition = IsEmpty ? -1 : CurrentPosition;
            bool oldIsCurrentAfterLast = IsCurrentAfterLast;
            bool oldIsCurrentBeforeFirst = IsCurrentBeforeFirst;

            // force currency off the collection (gives user a chance to save dirty information)
            OnCurrentChanging();

            IList list = SourceCollection as IList;
#if FEATURE_ICOLLECTIONVIEW_SORT_OR_FILTER
            PrepareSortAndFilter();

            // if there's no sort/filter, just use the collection's array
            if (!UsesLocalArray)
            {
#endif
#pragma warning disable SA1137 // Elements should have the same indentation
            _internalList = list;
#pragma warning restore SA1137 // Elements should have the same indentation
#if FEATURE_ICOLLECTIONVIEW_SORT_OR_FILTER
            }
            else
            {
                _internalList = PrepareLocalArray(list);
            }
#endif

#if FEATURE_ICOLLECTIONVIEW_GROUP
            PrepareGroups();
#endif

            if (oldIsCurrentBeforeFirst || IsEmpty)
            {
                SetCurrent(null, -1);
            }
            else if (oldIsCurrentAfterLast)
            {
                SetCurrent(null, InternalCount);
            }
            else
            {
                // set currency back to old current item
                // oldCurrentItem may be null

                // if there are duplicates, use the position of the first matching item
                int newPosition = InternalIndexOf(oldCurrentItem);

                if (newPosition < 0)
                {
                    // oldCurrentItem not found: move to first item
                    SetCurrent(InternalItemAt(0), 0);
                }
                else
                {
                    SetCurrent(oldCurrentItem, newPosition);
                }
            }

            // tell listeners everything has changed
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            RaiseCurrencyChanges(
                true /*raiseCurrentChanged*/,
                CurrentItem != oldCurrentItem /*raiseCurrentItem*/,
                CurrentPosition != oldCurrentPosition /*raiseCurrentPosition*/,
                IsCurrentBeforeFirst != oldIsCurrentBeforeFirst /*raiseIsCurrentBeforeFirst*/,
                IsCurrentAfterLast != oldIsCurrentAfterLast /*raiseIsCurrentAfterLast*/);
        }

        /// <summary>
        /// Return true if the item belongs to this view.  No assumptions are
        /// made about the item. This method will behave similarly to IList.Contains()
        /// and will do an exhaustive search through all items in this view.
        /// If the caller knows that the item belongs to the
        /// underlying collection, it is more efficient to call PassesFilter.
        /// </summary>
        /// <returns>True if collection contains item.</returns>
        public override bool Contains(object item)
        {
            VerifyRefreshNotDeferred();

            return InternalContains(item);
        }

        /// <summary>
        /// Move CurrentItem to the item at the given index.
        /// </summary>
        /// <param name="position">Move CurrentItem to this index</param>
        /// <returns>true if CurrentItem points to an item within the view.</returns>
        public override bool MoveCurrentToPosition(int position)
        {
            VerifyRefreshNotDeferred();

            if (position < -1 || position > InternalCount)
            {
                throw new ArgumentOutOfRangeException("position");
            }

            if ((position != CurrentPosition || !IsCurrentInSync) && OKToChangeCurrent())
            {
                object proposedCurrentItem = (position >= 0 && position < InternalCount) ? InternalItemAt(position) : null;
                bool oldIsCurrentAfterLast = IsCurrentAfterLast;
                bool oldIsCurrentBeforeFirst = IsCurrentBeforeFirst;

                SetCurrent(proposedCurrentItem, position);

                // notify that the properties have changed.
                RaiseCurrencyChanges(
                    true /*raiseCurrentChanged*/,
                    true /*raiseCurrentItem*/,
                    true /*raiseCurrentPosition*/,
                    IsCurrentBeforeFirst != oldIsCurrentBeforeFirst /*raiseIsCurrentBeforeFirst*/,
                    IsCurrentAfterLast != oldIsCurrentAfterLast /*raiseIsCurrentAfterLast*/);
            }

            return IsCurrentInView;
        }

#if FEATURE_ICOLLECTIONVIEW_GROUP
        /// <summary>
        /// Gets a value indicating whether this view really supports grouping.
        /// When this returns false, the rest of the interface is ignored.
        /// </summary>
        public override bool CanGroup
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the description of grouping, indexed by level.
        /// </summary>
        public override ObservableCollection<GroupDescription> GroupDescriptions
        {
            get
            {
                return _group.GroupDescriptions;
            }
        }

        /// <summary>
        /// Gets the top-level groups, constructed according to the descriptions
        /// given in GroupDescriptions.
        /// </summary>
        public override ReadOnlyObservableCollection<object> Groups
        {
            get
            {
                return IsGrouping ? _group.Items : null;
            }
        }
#endif

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
            return ActiveFilter == null || ActiveFilter(item);
        }
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
        public override int IndexOf(object item)
        {
            VerifyRefreshNotDeferred();

            return InternalIndexOf(item);
        }

        /// <summary>
        /// Retrieve item at the given zero-based index in this CollectionView.
        /// </summary>
        /// <remarks>
        /// <p>The index is evaluated with any SortDescriptions or Filter being set on this CollectionView.</p>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if index is out of range
        /// </exception>
        /// <returns>Item at the given zero-based index in this CollectionView.</returns>
        public override object GetItemAt(int index)
        {
            VerifyRefreshNotDeferred();

            return InternalItemAt(index);
        }

        /// <summary>
        /// Return -, 0, or +, according to whether o1 occurs before, at, or after o2 (respectively)
        /// </summary>
        /// <param name="o1">first object</param>
        /// <param name="o2">second object</param>
        /// <remarks>
        /// Compares items by their resp. index in the IList.
        /// </remarks>
        /// <returns>-, 0, or +, according to whether o1 occurs before, at, or after o2 (respectively).</returns>
        int IComparer<object>.Compare(object o1, object o2)
        {
            return Compare(o1, o2);
        }

        /// <summary>
        /// Return -, 0, or +, according to whether o1 occurs before, at, or after o2 (respectively)
        /// </summary>
        /// <param name="o1">first object</param>
        /// <param name="o2">second object</param>
        /// <remarks>
        /// Compares items by their resp. index in the IList.
        /// </remarks>
        /// <returns>Return -, 0, or +, according to whether o1 occurs before, at, or after o2 (respectively).</returns>
        protected virtual int Compare(object o1, object o2)
        {
#if FEATURE_ICOLLECTIONVIEW_GROUP
            if (!IsGrouping)
            {
#endif
#if FEATURE_ICOLLECTIONVIEW_SORT
                if (ActiveComparer != null)
                {
                    return ActiveComparer.Compare(o1, o2);
                }
#endif
            int i1 = InternalList.IndexOf(o1);
            int i2 = InternalList.IndexOf(o2);
            return i1 - i2;
#if FEATURE_ICOLLECTIONVIEW_GROUP
            }
            else
            {
                int i1 = InternalIndexOf(o1);
                int i2 = InternalIndexOf(o2);
                return i1 - i2;
            }
#endif
        }

        /// <summary>
        /// Implementation of IEnumerable.GetEnumerator().
        /// This provides a way to enumerate the members of the collection
        /// without changing the currency.
        /// </summary>
        /// <returns>IEnumerator</returns>
        protected override IEnumerator GetEnumerator()
        {
            VerifyRefreshNotDeferred();

            return InternalGetEnumerator();
        }

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------
#if FEATURE_ICOLLECTIONVIEW_SORT
        /// <summary>
        /// Gets a collection of Sort criteria to sort items in this view over the SourceCollection.
        /// </summary>
        /// <remarks>
        /// <p>
        /// One or more sort criteria in form of SortDescription
        /// can be added, each specifying a property and direction to sort by.
        /// </p>
        /// </remarks>
        public override SortDescriptionCollection SortDescriptions
        {
            get
            {
                if (_sort == null)
                {
                    SetSortDescriptions(new SortDescriptionCollection());
                }

                return _sort;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this ICollectionView supports sorting.
        /// </summary>
        /// <remarks>
        /// ListCollectionView does implement an IComparer based sorting.
        /// </remarks>
        public override bool CanSort
        {
            get
            {
                return true;
            }
        }
#endif

#if FEATURE_ICOLLECTIONVIEW_FILTER
        /// <summary>
        /// Gets a value indicating whether ICollectionView supports filtering.
        /// </summary>
        public override bool CanFilter
        {
            get
            {
                return true;
            }
        }
#endif

#if FEATURE_IEDITABLECOLLECTIONVIEW
        /// <summary>
        /// Gets or sets the callback used by the implementation of the ICollectionView to determine if an
        /// item is suitable for inclusion in the view.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// Simpler implementations do not support filtering and will throw a NotSupportedException.
        /// Use <seealso cref="CanFilter"/> property to test if filtering is supported before
        /// assigning a non-null value.
        /// </exception>
        public override Predicate<object> Filter
        {
            get
            {
                return base.Filter;
            }

            set
            {
                if (IsAddingNew || IsEditingItem)
                {
                    throw CollectionViewsError.CollectionView.MemberNotAllowedDuringAddOrEdit("Filter");
                }
                base.Filter = value;
            }
        }
#endif

        /// <summary>
        /// Gets the estimated number of records (or -1, meaning "don't know").
        /// </summary>
        public override int Count
        {
            get
            {
                VerifyRefreshNotDeferred();

                return InternalCount;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the resulting (filtered) view is empty.
        /// </summary>
        public override bool IsEmpty
        {
            get
            {
                return InternalCount == 0;
            }
        }

#if FEATURE_IEDITABLECOLLECTIONVIEW
        /// <summary>
        /// Gets or sets a value that indicates whether to include a placeholder for a new item, and if so,
        /// where to put it.
        /// </summary>
        public NewItemPlaceholderPosition NewItemPlaceholderPosition
        {
            get
            {
                return NewItemPlaceholderPosition.None;
            }

            set
            {
                if ((NewItemPlaceholderPosition)value != NewItemPlaceholderPosition.None)
                {
                    throw new ArgumentException("value");
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the view supports AddNew.
        /// </summary>
        public bool CanAddNew
        {
            get
            {
                return _canAddNew;
            }

            private set
            {
                if (_canAddNew != value)
                {
                    _canAddNew = value;
                    OnPropertyChanged(CanAddNewPropertyName);
                }
            }
        }

        private void RefreshCanAddNew()
        {
            this.CanAddNew = !IsEditingItem && SourceList != null && !SourceList.IsFixedSize && CanConstructItem;
        }

        private bool CanConstructItem
        {
            get
            {
                if (!_isItemConstructorValid)
                {
                    EnsureItemConstructor();
                }

                return _itemConstructor != null;
            }
        }

        private void EnsureItemConstructor()
        {
            if (!_isItemConstructorValid)
            {
                Type itemType = GetItemType(true);
                if (itemType != null)
                {
                    _itemConstructor = itemType.GetConstructor(Type.EmptyTypes);
                    _isItemConstructorValid = true;
                }
            }
        }

        /// <summary>
        /// Add a new item to the underlying collection.  Returns the new item.
        /// After calling AddNew and changing the new item as desired, either
        /// <seealso cref="CommitNew"/> or <seealso cref="CancelNew"/> should be
        /// called to complete the transaction.
        /// </summary>
        /// <returns>The new item.</returns>
        public object AddNew()
        {
            VerifyRefreshNotDeferred();

            if (IsEditingItem)
            {
                CommitEdit();   // implicitly close a previous EditItem
            }

            CommitNew();        // implicitly close a previous AddNew

            if (!CanAddNew)
            {
                throw CollectionViewsError.ListCollectionView.MemberNotAllowedForView("AddNew");
            }

            object newItem = _itemConstructor.Invoke(null);

            _newItemIndex = -2; // this is a signal that the next Add event comes from AddNew
            int index = SourceList.Add(newItem);

            // if the source doesn't raise collection change events, fake one
            if (!(SourceList is INotifyCollectionChanged))
            {
                // the index returned by IList.Add isn't always reliable
                if (!object.Equals(newItem, SourceList[index]))
                {
                    index = SourceList.IndexOf(newItem);
                }

                BeginAddNew(newItem, index);
            }

            DiagnosticsDebug.Assert(_newItemIndex != -2 && object.Equals(newItem, _newItem), "AddNew did not raise expected events");

            MoveCurrentTo(newItem);

            ISupportInitialize isi = newItem as ISupportInitialize;
            if (isi != null)
            {
                isi.BeginInit();
            }

            IEditableObject ieo = newItem as IEditableObject;
            if (ieo != null)
            {
                ieo.BeginEdit();
            }

            return newItem;
        }

        // Calling IList.Add() will raise an ItemAdded event.  We handle this specially
        // to adjust the position of the new item in the view (it should be adjacent
        // to the placeholder), and cache the new item for use by the other APIs
        // related to AddNew.  This method is called from ProcessCollectionChanged.
        private void BeginAddNew(object newItem, int index)
        {
            DiagnosticsDebug.Assert(_newItemIndex == -2 && _newItem == NoNewItem, "unexpected call to BeginAddNew");

            // remember the new item and its position in the underlying list
            SetNewItem(newItem);
            _newItemIndex = index;

            // adjust the position of the new item
            int position = UsesLocalArray ? InternalCount - 1 : _newItemIndex;

            // raise events as if the new item appeared in the adjusted position
            ProcessCollectionChangedWithAdjustedIndex(
                                        new NotifyCollectionChangedEventArgs(
                                                NotifyCollectionChangedAction.Add,
                                                newItem,
                                                position),
                                        -1,
                                        position);
        }

        /// <summary>
        /// Complete the transaction started by <seealso cref="AddNew"/>.  The new
        /// item remains in the collection, and the view's sort, filter, and grouping
        /// specifications (if any) are applied to the new item.
        /// </summary>
        public void CommitNew()
        {
            if (IsEditingItem)
            {
                throw CollectionViewsError.ListCollectionView.MemberNotAllowedDuringTransaction("CommitNew", "EditItem");
            }

            VerifyRefreshNotDeferred();

            if (_newItem == NoNewItem)
            {
                return;
            }

#if FEATURE_ICOLLECTIONVIEW_GROUP
            // grouping works differently
            if (IsGrouping)
            {
                CommitNewForGrouping();
                return;
            }
#endif

            // Remember its current position (have to do this before calling EndNew,
            // because InternalCount depends on "adding-new" mode).
            int fromIndex = UsesLocalArray ? InternalCount - 1 : _newItemIndex;

            // End the AddNew transaction
            object newItem = EndAddNew(false);

            // Tell the view clients what happened to the new item
            int toIndex = AdjustBefore(NotifyCollectionChangedAction.Add, newItem, _newItemIndex);

            if (toIndex < 0)
            {
                // item is effectively removed (due to filter), raise a Remove event
                ProcessCollectionChangedWithAdjustedIndex(
                            new NotifyCollectionChangedEventArgs(
                                                NotifyCollectionChangedAction.Remove,
                                                newItem,
                                                fromIndex),
                            fromIndex,
                            -1);
            }
            else if (fromIndex == toIndex)
            {
                // item isn't moving, so no events are needed.  But the item does need
                // to be added to the local array.
                if (UsesLocalArray)
                {
                    InternalList.Insert(toIndex, newItem);
                }
            }
            else
            {
                // item is moving
                ProcessCollectionChangedWithAdjustedIndex(newItem, fromIndex, toIndex);
            }
        }

#if FEATURE_ICOLLECTIONVIEW_GROUP
        private void CommitNewForGrouping()
        {
            // for grouping we cannot pretend that the new item moves to a different position,
            // since it may actually appear in several new positions (belonging to several groups).
            // Instead, we remove the item from its temporary position, then add it to the groups
            // as if it had just been added to the underlying collection.
            int index = _group.Items.Count - 1;

            // End the AddNew transaction
            int newItemIndex = _newItemIndex;
            object newItem = EndAddNew(false);

            try
            {
                _newGroupedItem = newItem;

                // remove item from its temporary position
                _group.RemoveSpecialItem(index, newItem, false /*loading*/);

                // now pretend it just got added to the collection.  This will add it
                // to the internal list with sort/filter, and to the groups
                ProcessCollectionChanged(
                        new NotifyCollectionChangedEventArgs(
                                    NotifyCollectionChangedAction.Add,
                                    newItem,
                                    newItemIndex));
            }
            finally
            {
                _newGroupedItem = null;
            }

            // Check if currency was already set to a particular item
            if (CurrentPosition == -1)
            {
                // Attempt to move to the new item. CurrentPosition will remain -1
                // if the new item cannot be found or it does not pass the filter.
                MoveCurrentTo(newItem);
            }
        }
#endif

        /// <summary>
        /// Complete the transaction started by <seealso cref="AddNew"/>.  The new
        /// item is removed from the collection.
        /// </summary>
        public void CancelNew()
        {
            if (IsEditingItem)
            {
                throw CollectionViewsError.ListCollectionView.MemberNotAllowedDuringTransaction("CancelNew", "EditItem");
            }

            VerifyRefreshNotDeferred();

            if (_newItem == NoNewItem)
            {
                return;
            }

            // remove the new item from the underlying collection.  Normally the
            // collection will raise a Remove event, which we'll handle by calling
            // EndNew to leave AddNew mode.
            SourceList.RemoveAt(_newItemIndex);

            // if the collection doesn't raise events, do the work explicitly on its behalf
            if (_newItem != NoNewItem)
            {
                int index = AdjustBefore(NotifyCollectionChangedAction.Remove, _newItem, _newItemIndex);
                object newItem = EndAddNew(true);

                ProcessCollectionChangedWithAdjustedIndex(
                            new NotifyCollectionChangedEventArgs(
                                                NotifyCollectionChangedAction.Remove,
                                                newItem,
                                                index),
                            index,
                            -1);
            }
        }

        // Common functionality used by CommitNew, CancelNew, and when the
        // new item is removed by Remove or Refresh.
        private object EndAddNew(bool cancel)
        {
            object newItem = _newItem;

            SetNewItem(NoNewItem);  // leave "adding-new" mode

            IEditableObject ieo = newItem as IEditableObject;
            if (ieo != null)
            {
                if (cancel)
                {
                    ieo.CancelEdit();
                }
                else
                {
                    ieo.EndEdit();
                }
            }

            ISupportInitialize isi = newItem as ISupportInitialize;
            if (isi != null)
            {
                isi.EndInit();
            }

            return newItem;
        }

        /// <summary>
        /// Gets a value indicating whether an AddNew transaction is in progress.
        /// </summary>
        public bool IsAddingNew
        {
            get
            {
                return _newItem != NoNewItem;
            }
        }

        /// <summary>
        /// Gets the new item when an AddNew transaction is in progress. Otherwise it returns null.
        /// </summary>
        public object CurrentAddItem
        {
            get
            {
                return IsAddingNew ? _newItem : null;
            }
        }

        private void SetNewItem(object item)
        {
            if (!object.Equals(item, _newItem))
            {
                DiagnosticsDebug.Assert(item == NoNewItem || this._newItem == NoNewItem, "Old and new _newItem values are unexpectedly different from NoNewItem");
                _newItem = item;

                OnPropertyChanged(CurrentAddItemPropertyName);
                OnPropertyChanged(IsAddingNewPropertyName);
                RefreshCanRemove();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the view supports Remove and RemoveAt.
        /// </summary>
        public bool CanRemove
        {
            get
            {
                return _canRemove;
            }

            private set
            {
                if (_canRemove != value)
                {
                    _canRemove = value;
                    OnPropertyChanged(CanRemovePropertyName);
                }
            }
        }

        private void RefreshCanRemove()
        {
            this.CanRemove = !IsEditingItem && !IsAddingNew && !SourceList.IsFixedSize;
        }

        /// <summary>
        /// Remove the item at the given index from the underlying collection.
        /// The index is interpreted with respect to the view (not with respect to
        /// the underlying collection).
        /// </summary>
        public void RemoveAt(int index)
        {
            if (IsEditingItem || IsAddingNew)
            {
                throw CollectionViewsError.CollectionView.MemberNotAllowedDuringAddOrEdit("RemoveAt");
            }
            else if (!this.CanRemove)
            {
                throw CollectionViewsError.ListCollectionView.MemberNotAllowedForView("RemoveAt");
            }

            VerifyRefreshNotDeferred();

            // convert the index from "view-relative" to "list-relative"
            object item = GetItemAt(index);

            int listIndex = index;
            bool raiseEvent = !(SourceList is INotifyCollectionChanged);

            // remove the item from the list
            if (UsesLocalArray || IsGrouping)
            {
                if (raiseEvent)
                {
                    listIndex = SourceList.IndexOf(item);
                    SourceList.RemoveAt(listIndex);
                }
                else
                {
                    SourceList.Remove(item);
                }
            }
            else
            {
                SourceList.RemoveAt(listIndex);
            }

            // if the list doesn't raise CollectionChanged events, fake one
            if (raiseEvent)
            {
                ProcessCollectionChanged(new NotifyCollectionChangedEventArgs(
                                            NotifyCollectionChangedAction.Remove,
                                            item,
                                            listIndex));
            }
        }

        /// <summary>
        /// Remove the given item from the underlying collection.
        /// </summary>
        public void Remove(object item)
        {
            int index = InternalIndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
            }
        }

        /// <summary>
        /// Begins an editing transaction on the given item.  The transaction is
        /// completed by calling either <seealso cref="CommitEdit"/> or
        /// <seealso cref="CancelEdit"/>.  Any changes made to the item during
        /// the transaction are considered "pending", provided that the view supports
        /// the notion of "pending changes" for the given item.
        /// </summary>
        public void EditItem(object item)
        {
            VerifyRefreshNotDeferred();

            if (IsAddingNew)
            {
                if (object.Equals(item, _newItem))
                {
                    return; // EditItem(newItem) is a no-op
                }

                CommitNew(); // implicitly close a previous AddNew
            }

            CommitEdit(); // implicitly close a previous EditItem transaction

            SetEditItem(item);

            IEditableObject ieo = item as IEditableObject;
            if (ieo != null)
            {
                ieo.BeginEdit();
            }
        }

        /// <summary>
        /// Complete the transaction started by <seealso cref="EditItem"/>.
        /// The pending changes (if any) to the item are committed.
        /// </summary>
        public void CommitEdit()
        {
            if (IsAddingNew)
            {
                throw CollectionViewsError.ListCollectionView.MemberNotAllowedDuringTransaction("CommitEdit", "AddNew");
            }

            VerifyRefreshNotDeferred();

            if (_editItem == null)
            {
                return;
            }

            object editItem = _editItem;
            SetEditItem(null);

            IEditableObject ieo = editItem as IEditableObject;
            if (ieo != null)
            {
                ieo.EndEdit();
            }

            int fromIndex = -1;
            bool wasInView = false, isInView = false;
            if (IsGrouping || UsesLocalArray)
            {
                // see if the item is entering or leaving the view
                fromIndex = InternalIndexOf(editItem);
                wasInView = fromIndex >= 0;
                isInView = wasInView ? PassesFilter(editItem)
                               : SourceList.Contains(editItem) && PassesFilter(editItem);
            }

#if FEATURE_ICOLLECTIONVIEW_GROUP
            // editing may change the item's group names (and we can't tell whether
            // it really did).  The best we can do is remove the item and re-insert
            // it.
            if (IsGrouping)
            {
                // Check whether to restore currency to the item being edited
                object restoreCurrencyTo = (editItem == CurrentItem) ? editItem : null;

                if (wasInView)
                {
                    RemoveItemFromGroups(editItem);
                }

                // Cache currency values so the appropriate PropertyChanged events can be raised later
                // Values are cached after calling RemoveItemFromGroups since that method may change
                // currency and raise events itself.
                object oldCurrentItem = CurrentItem;
                int oldCurrentPosition = CurrentPosition;
                bool oldIsCurrentAfterLast = IsCurrentAfterLast;
                bool oldIsCurrentBeforeFirst = IsCurrentBeforeFirst;

                if (isInView)
                {
                    AddItemToGroups(editItem);
                }

                // Check if currency was already set to a particular item,
                // if the edited item was the current item and may need to be restored
                if (CurrentPosition == -1 && restoreCurrencyTo != null)
                {
                    // Check if edited item ended up in the view and if it's OK to change currency
                    int newPosition = InternalIndexOf(restoreCurrencyTo);
                    if (newPosition >= 0 && PassesFilter(restoreCurrencyTo) && OKToChangeCurrent())
                    {
                        // Restore the original currency
                        SetCurrent(restoreCurrencyTo, newPosition);
                    }
                }

                RaiseCurrencyChanges(
                    oldCurrentItem != CurrentItem /*raiseCurrentChanged*/,
                    CurrentItem != oldCurrentItem /*raiseCurrentItem*/,
                    CurrentPosition != oldCurrentPosition /*raiseCurrentPosition*/,
                    IsCurrentBeforeFirst != oldIsCurrentBeforeFirst /*raiseIsCurrentBeforeFirst*/,
                    IsCurrentAfterLast != oldIsCurrentAfterLast /*raiseIsCurrentAfterLast*/);
                return;
            }
#endif

            // the edit may cause the item to move.  If so, report it.
            if (UsesLocalArray)
            {
                List<object> list = InternalList as List<object>;
                int toIndex = -1;

                if (wasInView)
                {
                    if (!isInView)
                    {
                        // the item has been effectively removed
                        ProcessCollectionChangedWithAdjustedIndex(
                                    new NotifyCollectionChangedEventArgs(
                                                        NotifyCollectionChangedAction.Remove,
                                                        editItem,
                                                        fromIndex),
                                    fromIndex,
                                    -1);
                    }
                    else if (ActiveComparer != null)
                    {
                        // the item may have moved within the view
                        int localIndex = fromIndex;
                        if (localIndex > 0 && ActiveComparer.Compare(list[localIndex - 1], editItem) > 0)
                        {
                            // the item has moved toward the front of the list
                            toIndex = list.BinarySearch(0, localIndex, editItem, ActiveComparer);
                            if (toIndex < 0)
                            {
                                toIndex = ~toIndex;
                            }
                        }
                        else if (localIndex < list.Count - 1 && ActiveComparer.Compare(editItem, list[localIndex + 1]) > 0)
                        {
                            // the item has moved toward the back of the list
                            toIndex = list.BinarySearch(localIndex + 1, list.Count - localIndex - 1, editItem, ActiveComparer);
                            if (toIndex < 0)
                            {
                                toIndex = ~toIndex;
                            }

                            --toIndex;      // because the item is leaving its old position
                        }

                        if (toIndex >= 0)
                        {
                            // the item has effectively moved
                            ProcessCollectionChangedWithAdjustedIndex(editItem, fromIndex, toIndex);
                        }
                    }
                }
                else if (isInView)
                {
                    // the item has effectively been added
                    toIndex = AdjustBefore(NotifyCollectionChangedAction.Add, editItem, SourceList.IndexOf(editItem));
                    ProcessCollectionChangedWithAdjustedIndex(
                                new NotifyCollectionChangedEventArgs(
                                            NotifyCollectionChangedAction.Add,
                                            editItem,
                                            toIndex),
                                -1,
                                toIndex);
                }
            }
        }

        /// <summary>
        /// Complete the transaction started by <seealso cref="EditItem"/>.
        /// The pending changes (if any) to the item are discarded.
        /// </summary>
        public void CancelEdit()
        {
            if (IsAddingNew)
            {
                throw CollectionViewsError.ListCollectionView.MemberNotAllowedDuringTransaction("CancelEdit", "AddNew");
            }

            VerifyRefreshNotDeferred();

            if (_editItem == null)
            {
                return;
            }

            object editItem = _editItem;
            SetEditItem(null);

            IEditableObject ieo = editItem as IEditableObject;
            if (ieo != null)
            {
                ieo.CancelEdit();
            }
            else
            {
                throw CollectionViewsError.ListCollectionView.CancelEditNotSupported();
            }
        }

        private void ImplicitlyCancelEdit()
        {
            IEditableObject ieo = _editItem as IEditableObject;
            SetEditItem(null);

            if (ieo != null)
            {
                ieo.CancelEdit();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the view supports the notion of "pending changes" on the
        /// current edit item.  This may vary, depending on the view and the particular
        /// item.  For example, a view might return true if the current edit item
        /// implements <seealso cref="IEditableObject"/>, or if the view has special
        /// knowledge about the item that it can use to support rollback of pending
        /// changes.
        /// </summary>
        public bool CanCancelEdit
        {
            get
            {
                return _canCancelEdit;
            }

            private set
            {
                if (_canCancelEdit != value)
                {
                    _canCancelEdit = value;
                    OnPropertyChanged(CanCancelEditPropertyName);
                }
            }
        }

        private void RefreshCanCancelEdit()
        {
            CanCancelEdit = _editItem is IEditableObject;
        }

        /// <summary>
        /// Gets a value indicating whether an EditItem transaction is in progress.
        /// </summary>
        public bool IsEditingItem
        {
            get
            {
                return _editItem != null;
            }
        }

        /// <summary>
        /// Gets the affected item when an EditItem" transaction is in progress. Otherwise it returns null.
        /// </summary>
        public object CurrentEditItem
        {
            get
            {
                return _editItem;
            }
        }

        private void SetEditItem(object item)
        {
            if (!object.Equals(item, _editItem))
            {
                DiagnosticsDebug.Assert(item == null || _editItem == null, "Old and new _editItem values are unexpectedly non null");
                _editItem = item;

                OnPropertyChanged(CurrentEditItemPropertyName);
                OnPropertyChanged(IsEditingItemPropertyName);
                RefreshCanCancelEdit();
                RefreshCanAddNew();
                RefreshCanRemove();
            }
        }
#endif

        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        /// <summary>
        /// Handle CollectionChange events
        /// </summary>
        protected override void ProcessCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

#if DEBUG
            Debug_ValidateCollectionChangedEventArgs(args);
#endif

#if FEATURE_IEDITABLECOLLECTIONVIEW
            // adding or replacing an item can change CanAddNew, by providing a
            // non-null representative
            if (!_isItemConstructorValid)
            {
                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Reset:
                    case NotifyCollectionChangedAction.Add:
                    case NotifyCollectionChangedAction.Replace:
                        RefreshCanAddNew();
                        break;
                }
            }
#endif
            int adjustedOldIndex = -1;
            int adjustedNewIndex = -1;

            // If the Action is Reset then we do a Refresh.
            if (args.Action == NotifyCollectionChangedAction.Reset)
            {
#if FEATURE_IEDITABLECOLLECTIONVIEW
                // implicitly cancel EditItem transactions
                if (IsEditingItem)
                {
                    ImplicitlyCancelEdit();
                }

                // adjust AddNew transactions, depending on whether the new item
                // survived the Reset
                if (IsAddingNew)
                {
                    _newItemIndex = SourceList.IndexOf(_newItem);
                    if (_newItemIndex < 0)
                    {
                        EndAddNew(true);
                    }
                }
#endif
                RefreshOrDefer();

                // the Refresh raises collection change event, so there's nothing left to do
                return;
            }

#if FEATURE_IEDITABLECOLLECTIONVIEW
            if (args.Action == NotifyCollectionChangedAction.Add && _newItemIndex == -2)
            {
                // The Add event came from AddNew.
                BeginAddNew(args.NewItems[0], args.NewStartingIndex);
                return;
            }
#endif

            // If the Action is one that can be expected to have a valid NewItems[0] and NewStartingIndex then
            // adjust the index for filtering and sorting.
            if (args.Action != NotifyCollectionChangedAction.Remove)
            {
                adjustedNewIndex = AdjustBefore(NotifyCollectionChangedAction.Add, args.NewItems[0], args.NewStartingIndex);
            }

            // If the Action is one that can be expected to have a valid OldItems[0] and OldStartingIndex then
            // adjust the index for filtering and sorting.
            if (args.Action != NotifyCollectionChangedAction.Add)
            {
                adjustedOldIndex = AdjustBefore(NotifyCollectionChangedAction.Remove, args.OldItems[0], args.OldStartingIndex);

#if FEATURE_ICOLLECTIONVIEW_SORT_OR_FILTER
                // the new index needs further adjustment if the action removes (or moves)
                // something before it.
                if (UsesLocalArray && adjustedOldIndex >= 0 && adjustedOldIndex < adjustedNewIndex)
                {
                    adjustedNewIndex--;
                }
#endif
            }

#if FEATURE_IEDITABLECOLLECTIONVIEW
            // handle interaction with AddNew and EditItem
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (args.NewStartingIndex <= _newItemIndex)
                    {
                        ++_newItemIndex;
                    }

                    break;

            case NotifyCollectionChangedAction.Remove:
                    if (args.OldStartingIndex < _newItemIndex)
                    {
                        --_newItemIndex;
                    }

                    // implicitly cancel AddNew and/or EditItem transactions if the relevant item is removed
                    object item = args.OldItems[0];

                    if (item == CurrentEditItem)
                    {
                        ImplicitlyCancelEdit();
                    }
                    else if (item == CurrentAddItem)
                    {
                        EndAddNew(true);
                    }

                    break;
            }
#endif

            ProcessCollectionChangedWithAdjustedIndex(args, adjustedOldIndex, adjustedNewIndex);
        }

        private void ProcessCollectionChangedWithAdjustedIndex(NotifyCollectionChangedEventArgs args, int adjustedOldIndex, int adjustedNewIndex)
        {
            ProcessCollectionChangedWithAdjustedIndex(
                (EffectiveNotifyCollectionChangedAction)args.Action,
                (args.OldItems == null || args.OldItems.Count == 0) ? null : args.OldItems[0],
                (args.NewItems == null || args.NewItems.Count == 0) ? null : args.NewItems[0],
                adjustedOldIndex,
                adjustedNewIndex);
        }

        private void ProcessCollectionChangedWithAdjustedIndex(object movedItem, int adjustedOldIndex, int adjustedNewIndex)
        {
            ProcessCollectionChangedWithAdjustedIndex(
                EffectiveNotifyCollectionChangedAction.Move,
                movedItem,
                movedItem,
                adjustedOldIndex,
                adjustedNewIndex);
        }

        private void ProcessCollectionChangedWithAdjustedIndex(EffectiveNotifyCollectionChangedAction action, object oldItem, object newItem, int adjustedOldIndex, int adjustedNewIndex)
        {
            // Finding out the effective Action after filtering and sorting.
            EffectiveNotifyCollectionChangedAction effectiveAction = action;
            if (adjustedOldIndex == adjustedNewIndex && adjustedOldIndex >= 0)
            {
                effectiveAction = EffectiveNotifyCollectionChangedAction.Replace;
            }
            else if (adjustedOldIndex == -1)
            {
                // old index is unknown
                // we weren't told the old index, but it may have been in the view.
                if (adjustedNewIndex < 0)
                {
                    // The new item will not be in the filtered view,
                    // so an Add is a no-op and anything else is a Remove.
                    if (action == EffectiveNotifyCollectionChangedAction.Add)
                    {
                        return;
                    }

                    effectiveAction = EffectiveNotifyCollectionChangedAction.Remove;
                }
            }
            else if (adjustedOldIndex < -1)
            {
                // old item is known to be NOT in filtered view
                if (adjustedNewIndex < 0)
                {
                    // since the old item wasn't in the filtered view, and the new
                    // item would not be in the filtered view, this is a no-op.
                    return;
                }
                else
                {
                    effectiveAction = EffectiveNotifyCollectionChangedAction.Add;
                }
            }
            else
            {
                // old item was in view
                if (adjustedNewIndex < 0)
                {
                    effectiveAction = EffectiveNotifyCollectionChangedAction.Remove;
                }
                else
                {
                    effectiveAction = EffectiveNotifyCollectionChangedAction.Move;
                }
            }

            int originalCurrentPosition = CurrentPosition;
            int oldCurrentPosition = CurrentPosition;
            object oldCurrentItem = CurrentItem;
            bool oldIsCurrentAfterLast = IsCurrentAfterLast;
            bool oldIsCurrentBeforeFirst = IsCurrentBeforeFirst;

            // in the case of a replace that has a new adjustedPosition
            // (likely caused by sorting), the only way to effectively communicate
            // this change is through raising Remove followed by Insert.
            NotifyCollectionChangedEventArgs args = null, args2 = null;

            switch (effectiveAction)
            {
                case EffectiveNotifyCollectionChangedAction.Add:
                    // insert into private view
#if FEATURE_ICOLLECTIONVIEW_SORT_OR_FILTER
#if FEATURE_IEDITABLECOLLECTIONVIEW
                    // (unless it's a special item (i.e. new item))
                    if (UsesLocalArray && (!IsAddingNew || !object.Equals(_newItem, newItem)))
#else
                    if (UsesLocalArray)
#endif
                    {
                        InternalList.Insert(adjustedNewIndex, newItem);
                    }
#endif
                    if (!IsGrouping)
                    {
                        AdjustCurrencyForAdd(adjustedNewIndex);
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem, adjustedNewIndex);
                    }
#if FEATURE_ICOLLECTIONVIEW_GROUP
                    else
                    {
                        AddItemToGroups(newItem);
                    }
#endif
                    break;

                case EffectiveNotifyCollectionChangedAction.Remove:
#if FEATURE_ICOLLECTIONVIEW_SORT_OR_FILTER
                    // remove from private view, unless it's not there to start with
                    // (e.g. when CommitNew is applied to an item that fails the filter)
                    if (UsesLocalArray)
                    {
                        int localOldIndex = adjustedOldIndex;

                        if (localOldIndex < InternalList.Count && localOldIndex >= 0 &&
                            object.Equals(InternalList[localOldIndex], oldItem))
                        {
                            InternalList.RemoveAt(localOldIndex);
                        }
                    }
#endif

                    if (!IsGrouping)
                    {
                        AdjustCurrencyForRemove(adjustedOldIndex);
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, adjustedOldIndex);
                    }
#if FEATURE_ICOLLECTIONVIEW_GROUP
                    else
                    {
                        RemoveItemFromGroups(oldItem);
                    }
#endif
                    break;

                case EffectiveNotifyCollectionChangedAction.Replace:
#if FEATURE_ICOLLECTIONVIEW_SORT_OR_FILTER
                    // replace item in private view
                    if (UsesLocalArray)
                    {
                        InternalList[adjustedOldIndex] = newItem;
                    }
#endif

                    if (!IsGrouping)
                    {
                        AdjustCurrencyForReplace(adjustedOldIndex);
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, adjustedOldIndex);
                    }
#if FEATURE_ICOLLECTIONVIEW_GROUP
                    else
                    {
                        RemoveItemFromGroups(oldItem);
                        AddItemToGroups(newItem);
                    }
#endif
                    break;

                case EffectiveNotifyCollectionChangedAction.Move:
                    // remove from private view
#if FEATURE_ICOLLECTIONVIEW_GROUP
                    bool simpleMove = oldItem == newItem;
#endif
#if FEATURE_ICOLLECTIONVIEW_SORT_OR_FILTER
                    if (UsesLocalArray)
                    {
                        int localOldIndex = adjustedOldIndex;
                        int localNewIndex = adjustedNewIndex;

                        // remove the item from its old position, unless it's not there
                        // (which happens when the item is the object of CommitNew)
                        if (localOldIndex < InternalList.Count && localOldIndex >= 0 &&
                            object.Equals(InternalList[localOldIndex], oldItem))
                        {
                            InternalList.RemoveAt(localOldIndex);
                        }

                        // put the item into its new position
                        InternalList.Insert(localNewIndex, newItem);
                    }
#endif

                    if (!IsGrouping)
                    {
                        AdjustCurrencyForMove(adjustedOldIndex, adjustedNewIndex);

                        // move/replace
                        args2 = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem, adjustedNewIndex);
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, adjustedOldIndex);
                    }
#if FEATURE_ICOLLECTIONVIEW_GROUP
                    else if (!simpleMove)
                    {
                        RemoveItemFromGroups(oldItem);
                        AddItemToGroups(newItem);
                    }
#endif
                    break;

                default:
                    DiagnosticsDebug.Assert(false, "Unexpected Effective Collection Change Action");
                    break;
            }

            // remember whether scalar properties of the view have changed.
            // They may change again during the collection change event, so we
            // need to do the test before raising that event.
            bool afterLastHasChanged = IsCurrentAfterLast != oldIsCurrentAfterLast;
            bool beforeFirstHasChanged = IsCurrentBeforeFirst != oldIsCurrentBeforeFirst;
            bool currentPositionHasChanged = CurrentPosition != oldCurrentPosition;
            bool currentItemHasChanged = CurrentItem != oldCurrentItem;

            // take a new snapshot of the scalar properties, so that we can detect
            // changes made during the collection change event
            oldIsCurrentAfterLast = IsCurrentAfterLast;
            oldIsCurrentBeforeFirst = IsCurrentBeforeFirst;
            oldCurrentPosition = CurrentPosition;
            oldCurrentItem = CurrentItem;

            // base class will raise an event to our listeners
            if (!IsGrouping)
            {
                // we've already returned if (action == NotifyCollectionChangedAction.Reset) above

                // To avoid notification reentrancy we need to mark this collection view as processing a change
                // so any changes to the current item will only be raised once, and from this method
                // _currentChangedMonitor is used to guard whether the CurrentChanged and CurrentChanging event can be fired
                // so by entering it we're preventing the base calls from firing those events.
                DiagnosticsDebug.Assert(!CurrentChangedMonitor.Busy, "Expected _currentChangedMonitor.Busy is false.");

                CurrentChangedMonitor.Enter();
                using (CurrentChangedMonitor)
                {
                    OnCollectionChanged(args);
                    if (args2 != null)
                    {
                        OnCollectionChanged(args2);
                    }
                }

                // Any scalar properties that changed don't need a further notification,
                // but do need a new snapshot
                if (IsCurrentAfterLast != oldIsCurrentAfterLast)
                {
                    afterLastHasChanged = false;
                    oldIsCurrentAfterLast = IsCurrentAfterLast;
                }

                if (IsCurrentBeforeFirst != oldIsCurrentBeforeFirst)
                {
                    beforeFirstHasChanged = false;
                    oldIsCurrentBeforeFirst = IsCurrentBeforeFirst;
                }

                if (CurrentPosition != oldCurrentPosition)
                {
                    currentPositionHasChanged = false;
                    oldCurrentPosition = CurrentPosition;
                }

                if (CurrentItem != oldCurrentItem)
                {
                    currentItemHasChanged = false;
                    oldCurrentItem = CurrentItem;
                }
            }

            // currency has to change after firing the deletion event,
            // so event handlers have the right picture
            if (_currentElementWasRemoved)
            {
                int oldCurPos = originalCurrentPosition;

#if FEATURE_ICOLLECTIONVIEW_GROUP
                if (_newGroupedItem != null)
                {
                    oldCurPos = IndexOf(_newGroupedItem);
                }
#endif
                MoveCurrencyOffDeletedElement(oldCurPos);

                // changes to the scalar properties need notification
                afterLastHasChanged = afterLastHasChanged || (IsCurrentAfterLast != oldIsCurrentAfterLast);
                beforeFirstHasChanged = beforeFirstHasChanged || (IsCurrentBeforeFirst != oldIsCurrentBeforeFirst);
                currentPositionHasChanged = currentPositionHasChanged || (CurrentPosition != oldCurrentPosition);
                currentItemHasChanged = currentItemHasChanged || (CurrentItem != oldCurrentItem);
            }

            // notify that the properties have changed.  We may end up doing
            // double notification for properties that change during the collection
            // change event, but that's not harmful.  Detecting the double change
            // is more trouble than it's worth.
            RaiseCurrencyChanges(
                false /*raiseCurrentChanged*/,
                currentItemHasChanged,
                currentPositionHasChanged,
                beforeFirstHasChanged,
                afterLastHasChanged);
        }

        /// <summary>
        /// Return index of item in the internal list.
        /// </summary>
        /// <returns>Index of item in the internal list.</returns>
        protected int InternalIndexOf(object item)
        {
#if FEATURE_ICOLLECTIONVIEW_GROUP
            if (IsGrouping)
            {
                return _group.LeafIndexOf(item);
            }
#endif
#if FEATURE_ICOLLECTIONVIEW_SORT_OR_FILTER
#if FEATURE_IEDITABLECOLLECTIONVIEW
            if (IsAddingNew && object.Equals(item, _newItem) && UsesLocalArray)
            {
                return InternalCount - 1;
            }
#endif
#endif
            return InternalList.IndexOf(item);
        }

        /// <summary>
        /// Return item at the given index in the internal list.
        /// </summary>
        /// <returns>Item at the given index in the internal list.</returns>
        protected object InternalItemAt(int index)
        {
#if FEATURE_ICOLLECTIONVIEW_GROUP
            if (IsGrouping)
            {
                return _group.LeafAt(index);
            }
#endif
#if FEATURE_IEDITABLECOLLECTIONVIEW
#if FEATURE_ICOLLECTIONVIEW_SORT_OR_FILTER
            if (IsAddingNew && UsesLocalArray && index == InternalCount - 1)
            {
                return _newItem;
            }
#endif
#endif
            return InternalList[index];
        }

        /// <summary>
        /// Return true if internal list contains the item.
        /// </summary>
        /// <returns>True if internal list contains the item.</returns>
        protected bool InternalContains(object item)
        {
#if FEATURE_ICOLLECTIONVIEW_GROUP
            if (!IsGrouping)
            {
#endif
            return InternalList.Contains(item);
#if FEATURE_ICOLLECTIONVIEW_GROUP
            }
            else
            {
                return _group.LeafIndexOf(item) >= 0;
            }
#endif
        }

        /// <summary>
        /// Return an enumerator for the internal list.
        /// </summary>
        /// <returns>An enumerator for the internal list.</returns>
        protected IEnumerator InternalGetEnumerator()
        {
#if FEATURE_ICOLLECTIONVIEW_GROUP
            if (!IsGrouping)
            {
#endif
#if FEATURE_IEDITABLECOLLECTIONVIEW
                return new PlaceholderAwareEnumerator(this, InternalList.GetEnumerator(), _newItem);
#else
            return new PlaceholderAwareEnumerator(this, InternalList.GetEnumerator(), NoNewItem);
#endif
#if FEATURE_ICOLLECTIONVIEW_GROUP
            }
            else
            {
                return _group.GetLeafEnumerator();
            }
#endif
        }

#if FEATURE_ICOLLECTIONVIEW_SORT_OR_FILTER
        /// <summary>
        /// Gets a value indicating whether a private copy of the data is needed for sorting and filtering
        /// </summary>
        protected bool UsesLocalArray
        {
            get
            {
                return ActiveComparer != null || ActiveFilter != null;
            }
        }
#endif

        /// <summary>
        /// Gets a protected accessor to private _internalList field.
        /// </summary>
        protected IList InternalList
        {
            get
            {
                return _internalList;
            }
        }

#if FEATURE_ICOLLECTIONVIEW_SORT
        /// <summary>
        /// Gets or sets a protected accessor to private _activeComparer field.
        /// </summary>
        protected IComparer<object> ActiveComparer
        {
            get
            {
                return _activeComparer;
            }

            set
            {
                _activeComparer = value;
            }
        }
#endif

#if FEATURE_ICOLLECTIONVIEW_FILTER
        /// <summary>
        /// Gets or sets a protected accessor to private _activeFilter field.
        /// </summary>
        protected Predicate<object> ActiveFilter
        {
            get
            {
                return _activeFilter;
            }

            set
            {
                _activeFilter = value;
            }
        }
#endif

        /// <summary>
        /// Gets a value indicating whether grouping is supported.
        /// </summary>
        protected bool IsGrouping
        {
            get
            {
#if FEATURE_ICOLLECTIONVIEW_GROUP
                return _isGrouping;
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// Gets a protected accessor to private count.
        /// </summary>
        protected int InternalCount
        {
            get
            {
#if FEATURE_ICOLLECTIONVIEW_GROUP
                if (IsGrouping)
                {
                    return _group.ItemCount;
                }
#endif

#if FEATURE_ICOLLECTIONVIEW_SORT_OR_FILTER
                bool usesLocalArray = UsesLocalArray;
#else
                bool usesLocalArray = false;
#endif
#if FEATURE_IEDITABLECOLLECTIONVIEW
                bool isAddingNew = IsAddingNew;
#else
                bool isAddingNew = false;
#endif
                int delta = (usesLocalArray && isAddingNew) ? 1 : 0;
                return delta + InternalList.Count;
            }
        }

#if FEATURE_ICOLLECTIONVIEW_SORT
        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------

        // returns true if this ListCollectionView has sort descriptions,
        // without tripping off lazy creation of SortDescriptions collection.
        internal bool HasSortDescriptions
        {
            get
            {
                return _sort != null && _sort.Count > 0;
            }
        }
#endif

        //------------------------------------------------------
        //
        //  Private Properties
        //
        //------------------------------------------------------

        // true if CurrentPosition points to item within view
        private bool IsCurrentInView
        {
            get
            {
                return CurrentPosition >= 0 && CurrentPosition < InternalCount;
            }
        }

#if FEATURE_ICOLLECTIONVIEW_GROUP
        // can the group name(s) for an item change after we've grouped the item?
        private bool CanGroupNamesChange
        {
            // There's no way we can deduce this - the app has to tell us.
            // If this is true, removing a grouped item is quite difficult.
            // We cannot rely on its group names to tell us which group we inserted
            // it into (they may have been different at insertion time), so we
            // have to do a linear search.
            get
            {
                return true;
            }
        }
#endif

        private IList SourceList
        {
            get
            {
                return SourceCollection as IList;
            }
        }

        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        /// <summary>
        /// Validates provided NotifyCollectionChangedEventArgs
        /// </summary>
        /// <param name="e">NotifyCollectionChangedEventArgs to validate.</param>
        [Conditional("DEBUG")]
        private void Debug_ValidateCollectionChangedEventArgs(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    DiagnosticsDebug.Assert(e.NewItems.Count == 1, "Unexpected NotifyCollectionChangedEventArgs.NewItems.Count for Add action");
                    break;

                case NotifyCollectionChangedAction.Remove:
                    DiagnosticsDebug.Assert(e.OldItems.Count == 1, "Unexpected NotifyCollectionChangedEventArgs.OldItems.Count for Remove action");
                    break;

                case NotifyCollectionChangedAction.Replace:
                    DiagnosticsDebug.Assert(e.OldItems.Count == 1, "Unexpected NotifyCollectionChangedEventArgs.OldItems.Count for Replace action");
                    DiagnosticsDebug.Assert(e.NewItems.Count == 1, "Unexpected NotifyCollectionChangedEventArgs.NewItems.Count for Replace action");
                    break;

                case NotifyCollectionChangedAction.Reset:
                    break;

                default:
                    DiagnosticsDebug.Assert(false, "Unexpected NotifyCollectionChangedEventArgs action");
                    break;
            }
        }

#if FEATURE_ICOLLECTIONVIEW_SORT_OR_FILTER
        /// <summary>
        /// Create, filter and sort the local index array.
        /// called from Refresh(), override in derived classes as needed.
        /// </summary>
        /// <param name="list">new ILIst to associate this view with</param>
        /// <returns>new local array to use for this view</returns>
        private IList PrepareLocalArray(IList list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            // filter the collection's array into the local array
            List<object> al;

            if (ActiveFilter == null)
            {
                al = new List<object>();
                foreach (var o in list)
                {
                    al.Add(o);
                }
            }
            else
            {
                al = new List<object>(list.Count);
                for (int k = 0; k < list.Count; ++k)
                {
                    if (ActiveFilter(list[k]))
                    {
                        al.Add(list[k]);
                    }
                }
            }

            // sort the local array
            if (ActiveComparer != null)
            {
                al.Sort(ActiveComparer);
            }

            return al;
        }
#endif

        private void MoveCurrencyOffDeletedElement(int oldCurrentPosition)
        {
            int lastPosition = InternalCount - 1;   // OK if last is -1

            // if position falls beyond last position, move back to last position
            int newPosition = (oldCurrentPosition < lastPosition) ? oldCurrentPosition : lastPosition;

            // reset this to false before raising events to avoid problems in re-entrancy
            _currentElementWasRemoved = false;

            OnCurrentChanging();

            if (newPosition < 0)
            {
                SetCurrent(null, newPosition);
            }
            else
            {
                SetCurrent(InternalItemAt(newPosition), newPosition);
            }

            OnCurrentChanged();
        }

        // Convert the collection's index to an index into the view.
        // Return -1 if the index is unknown or moot (Reset events).
        // Return -2 if the event doesn't apply to this view.
        private int AdjustBefore(NotifyCollectionChangedAction action, object item, int index)
        {
            // index is not relevant to Reset events
            if (action == NotifyCollectionChangedAction.Reset)
            {
                return -1;
            }

            IList ilFull = SourceCollection as IList;

            // validate input
            if (index < -1 || index > ilFull.Count)
            {
                throw CollectionViewsError.ListCollectionView.CollectionChangedOutOfRange();
            }

            if (action == NotifyCollectionChangedAction.Add)
            {
                if (index >= 0)
                {
                    if (!object.Equals(item, ilFull[index]))
                    {
                        throw CollectionViewsError.CollectionView.ItemNotAtIndex("added");
                    }
                }
                else
                {
                    // event didn't specify index - determine it the hard way
                    index = ilFull.IndexOf(item);
                    if (index < 0)
                    {
                        throw CollectionViewsError.ListCollectionView.AddedItemNotInCollection();
                    }
                }
            }

#if FEATURE_ICOLLECTIONVIEW_SORT_OR_FILTER
            // If there's no sort or filter, use the index into the full array
            if (!UsesLocalArray)
#endif
            {
#if FEATURE_IEDITABLECOLLECTIONVIEW
                if (IsAddingNew)
                {
                    if (index > _newItemIndex)
                    {
                        index--; // the new item has been artificially moved elsewhere
                    }
                }
#endif
                return index;
            }

#if FEATURE_ICOLLECTIONVIEW_SORT_OR_FILTER
            if (action == NotifyCollectionChangedAction.Add)
            {
#if FEATURE_ICOLLECTIONVIEW_FILTER
                // if the item isn't in the filter, return -2
                if (!PassesFilter(item))
                {
                    return -2;
                }
#endif
                // search the local array
                List<object> al = InternalList as List<object>;
                if (al == null)
                {
                    index = -1;
                }
#if FEATURE_ICOLLECTIONVIEW_SORT
                else if (ActiveComparer != null)
                {
                    // if there's a sort order, use binary search
                    index = al.BinarySearch(item, ActiveComparer);
                    if (index < 0)
                    {
                        index = ~index;
                    }
                }
#endif
                else
                {
                    // otherwise, do a linear search of the full array, advancing
                    // localIndex past elements that appear in the local array,
                    // until either (a) reaching the position of the item in the
                    // full array, or (b) falling off the end of the local array.
                    // localIndex is now the desired index.
                    // One small wrinkle:  we have to ignore the target item in
                    // the local array (this arises in a Move event).
                    int fullIndex = 0, localIndex = 0;

                    while (fullIndex < index && localIndex < al.Count)
                    {
                        if (object.Equals(ilFull[fullIndex], al[localIndex]))
                        {
                            // match - current item passes filter.  Skip it.
                            ++fullIndex;
                            ++localIndex;
                        }
                        else if (object.Equals(item, al[localIndex]))
                        {
                            // skip over an unmatched copy of the target item
                            // (this arises in a Move event)
                            ++localIndex;
                        }
                        else
                        {
                            // no match - current item fails filter.  Ignore it.
                            ++fullIndex;
                        }
                    }

                    index = localIndex;
                }
            }
            else if (action == NotifyCollectionChangedAction.Remove)
            {
#if FEATURE_IEDITABLECOLLECTIONVIEW
                if (!IsAddingNew || item != _newItem)
                {
#endif
                    // a deleted item should already be in the local array
                    index = InternalList.IndexOf(item);

                    // but may not be, if it was already filtered out (can't use
                    // PassesFilter here, because the item could have changed
                    // while it was out of our sight)
                    if (index < 0)
                    {
                        return -2;
                    }
#if FEATURE_IEDITABLECOLLECTIONVIEW
                }
                else
                {
                    // the new item is in a special position
                    return InternalCount - 1;
                }
#endif
            }
            else
            {
                index = -1;
            }

            return (index < 0) ? index : index;
#endif
        }

        // fix up CurrentPosition and CurrentItem after a collection change
        private void AdjustCurrencyForAdd(int index)
        {
            if (InternalCount == 1)
            {
                if (CurrentItem != null || CurrentPosition != -1)
                {
                    // fire current changing notification
                    OnCurrentChanging();
                }

                // added first item; set current at BeforeFirst
                SetCurrent(null, -1);
            }
            else if (index <= CurrentPosition)
            {
                // adjust current index if insertion is earlier
                int newPosition = CurrentPosition + 1;

                if (newPosition < InternalCount)
                {
                    // CurrentItem might be out of sync if underlying list is not INCC
                    // or if this Add is the result of a Replace (Rem + Add)
                    SetCurrent(GetItemAt(newPosition), newPosition);
                }
                else
                {
                    SetCurrent(null, InternalCount);
                }
            }
            else if (!IsCurrentInSync)
            {
                // Make sure current item and position are in sync.
                SetCurrent(CurrentItem, InternalIndexOf(CurrentItem));
            }
        }

        // fix up CurrentPosition and CurrentItem after a collection change
        private void AdjustCurrencyForRemove(int index)
        {
            // adjust current index if deletion is earlier
            if (index < CurrentPosition)
            {
                SetCurrent(CurrentItem, CurrentPosition - 1);
            }

            // remember to move currency off the deleted element
            else if (index == CurrentPosition)
            {
                _currentElementWasRemoved = true;
            }
        }

        // fix up CurrentPosition and CurrentItem after a collection change
        private void AdjustCurrencyForMove(int oldIndex, int newIndex)
        {
            if (oldIndex == CurrentPosition)
            {
                // moving the current item - currency moves with the item (bug 1942184)
                SetCurrent(GetItemAt(newIndex), newIndex);
            }
            else if (oldIndex < CurrentPosition && CurrentPosition <= newIndex)
            {
                // moving an item from before current position to after -
                // current item shifts back one position
                SetCurrent(CurrentItem, CurrentPosition - 1);
            }
            else if (newIndex <= CurrentPosition && CurrentPosition < oldIndex)
            {
                // moving an item from after current position to before -
                // current item shifts ahead one position
                SetCurrent(CurrentItem, CurrentPosition + 1);
            }

            // else no change necessary
        }

        // fix up CurrentPosition and CurrentItem after a collection change
        private void AdjustCurrencyForReplace(int index)
        {
            // remember to move currency off the deleted element
            if (index == CurrentPosition)
            {
                _currentElementWasRemoved = true;
            }
        }

        private void RaiseCurrencyChanges(
            bool raiseCurrentChanged,
            bool raiseCurrentItem,
            bool raiseCurrentPosition,
            bool raiseIsCurrentBeforeFirst,
            bool raiseIsCurrentAfterLast)
        {
            if (raiseCurrentChanged)
            {
                OnCurrentChanged();
            }

            if (raiseIsCurrentAfterLast)
            {
                OnPropertyChanged(IsCurrentAfterLastPropertyName);
            }

            if (raiseIsCurrentBeforeFirst)
            {
                OnPropertyChanged(IsCurrentBeforeFirstPropertyName);
            }

            if (raiseCurrentPosition)
            {
                OnPropertyChanged(CurrentPositionPropertyName);
            }

            if (raiseCurrentItem)
            {
                OnPropertyChanged(CurrentItemPropertyName);
            }
        }

        // build the sort and filter information from the relevant properties
        private void PrepareSortAndFilter()
        {
#if FEATURE_ICOLLECTIONVIEW_SORT
            // sort:  prepare the comparer
            if (_sort != null && _sort.Count > 0)
            {
                ActiveComparer = new SortFieldComparer(_sort, Culture);
            }
            else
            {
                ActiveComparer = null;
            }
#endif

#if FEATURE_ICOLLECTIONVIEW_FILTER
            // filter:  prepare the Predicate<object> filter
            ActiveFilter = Filter;
#endif
        }

#if FEATURE_ICOLLECTIONVIEW_SORT
        // set new SortDescription collection; rehook collection change notification handler
        private void SetSortDescriptions(SortDescriptionCollection descriptions)
        {
            if (_sort != null)
            {
                ((INotifyCollectionChanged)_sort).CollectionChanged -= new NotifyCollectionChangedEventHandler(SortDescriptionsChanged);
            }

            _sort = descriptions;

            if (_sort != null)
            {
                DiagnosticsDebug.Assert(_sort.Count == 0, "must be empty SortDescription collection");
                ((INotifyCollectionChanged)_sort).CollectionChanged += new NotifyCollectionChangedEventHandler(SortDescriptionsChanged);
            }
        }

        // SortDescription was added/removed, refresh CollectionView
        private void SortDescriptionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
#if FEATURE_IEDITABLECOLLECTIONVIEW
            if (IsAddingNew || IsEditingItem)
            {
                throw CollectionViewsError.CollectionView.MemberNotAllowedDuringAddOrEdit("Sorting");
            }
#endif
            RefreshOrDefer();
        }
#endif

#if FEATURE_ICOLLECTIONVIEW_GROUP
        // divide the data items into groups
        private void PrepareGroups()
        {
            // discard old groups
            _group.Clear();

            // initialize the synthetic top level group
            _group.Initialize();

            // if there's no grouping, there's nothing to do
            _isGrouping = _group.GroupBy != null;
            if (!_isGrouping)
            {
                return;
            }

            // reset the grouping comparer
            IComparer<object> comparer = ActiveComparer;
            if (comparer != null)
            {
                _group.ActiveComparer = comparer;
            }
            else
            {
                CollectionViewGroupInternal.IListComparer ilc = _group.ActiveComparer as CollectionViewGroupInternal.IListComparer;
                if (ilc != null)
                {
                    ilc.ResetList(InternalList);
                }
                else
                {
                    _group.ActiveComparer = new CollectionViewGroupInternal.IListComparer(InternalList);
                }
            }

            // loop through the sorted/filtered list of items, dividing them
            // into groups (with special cases for new item)
            for (int k = 0, n = InternalList.Count; k < n; ++k)
            {
                object item = InternalList[k];
                if (!IsAddingNew || !object.Equals(_newItem, item))
                {
                    _group.AddToSubgroups(item, true /*loading*/);
                }
            }

            if (IsAddingNew)
            {
                _group.InsertSpecialItem(_group.Items.Count, _newItem, true /*loading*/);
            }
        }

        // For the Group to report collection changed
        private void OnGroupChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                AdjustCurrencyForAdd(e.NewStartingIndex);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                AdjustCurrencyForRemove(e.OldStartingIndex);
            }

            OnCollectionChanged(e);
        }

        // The GroupDescriptions collection changed
        private void OnGroupByChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsAddingNew || IsEditingItem)
            {
                throw CollectionViewsError.CollectionView.MemberNotAllowedDuringAddOrEdit("Grouping");
            }

            // This is a huge change.  Just refresh the view.
            RefreshOrDefer();
        }

        // A group description for one of the subgroups changed
        private void OnGroupDescriptionChanged(object sender, EventArgs e)
        {
            if (IsAddingNew || IsEditingItem)
            {
                throw CollectionViewsError.CollectionView.MemberNotAllowedDuringAddOrEdit("Grouping");
            }

            // This is a huge change.  Just refresh the view.
            RefreshOrDefer();
        }

        // An item was inserted into the collection.  Update the groups.
        private void AddItemToGroups(object item)
        {
            if (IsAddingNew && item == _newItem)
            {
                _group.InsertSpecialItem(_group.Items.Count, item, false /*loading*/);
            }
            else
            {
                _group.AddToSubgroups(item, false /*loading*/);
            }
        }

        // An item was removed from the collection.  Update the groups.
        private void RemoveItemFromGroups(object item)
        {
            if (CanGroupNamesChange || _group.RemoveFromSubgroups(item))
            {
                // the item didn't appear where we expected it to.
                _group.RemoveItemFromSubgroupsByExhaustiveSearch(item);
            }
        }
#endif

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
        /// Private EffectiveNotifyCollectionChangedAction enum
        /// </summary>
        private enum EffectiveNotifyCollectionChangedAction
        {
            Add = 0,
            Remove = 1,
            Replace = 2,
            Move = 3,
            Reset = 4
        }

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------
        private IList _internalList;
        private bool _currentElementWasRemoved;  // true if we need to MoveCurrencyOffDeletedElement
#if FEATURE_ICOLLECTIONVIEW_FILTER
        private Predicate<object> _activeFilter;
#endif
#if FEATURE_ICOLLECTIONVIEW_SORT
        private IComparer<object> _activeComparer;
        private SortDescriptionCollection _sort;
#endif
#if FEATURE_ICOLLECTIONVIEW_GROUP
        private object _newGroupedItem; // used when a CommitNew is called in grouping scenarios
        private CollectionViewGroupRoot _group;
        private bool _isGrouping;
#endif
#if FEATURE_IEDITABLECOLLECTIONVIEW
        private bool _canAddNew;
        private bool _canRemove;
        private bool _canCancelEdit;
        private bool _isItemConstructorValid;
        private ConstructorInfo _itemConstructor;
        private object _editItem;
        private object _newItem = NoNewItem;
        private int _newItemIndex;  // position _newItem in the source collection
#endif

#if FEATURE_IEDITABLECOLLECTIONVIEW
        //------------------------------------------------------
        //
        //  Constants
        //
        //------------------------------------------------------

        // ListCollectionView-specific property names, introduced by IEditableCollectionView
        internal const string CanAddNewPropertyName = "CanAddNew";
        internal const string CanCancelEditPropertyName = "CanCancelEdit";
        internal const string CanRemovePropertyName = "CanRemove";
        internal const string CurrentAddItemPropertyName = "CurrentAddItem";
        internal const string CurrentEditItemPropertyName = "CurrentEditItem";
        internal const string IsAddingNewPropertyName = "IsAddingNew";
        internal const string IsEditingItemPropertyName = "IsEditingItem";
#endif
    }
}