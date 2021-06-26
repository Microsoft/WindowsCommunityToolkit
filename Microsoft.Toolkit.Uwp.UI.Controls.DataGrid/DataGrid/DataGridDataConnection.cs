// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Toolkit.Uwp.UI.Data.Utilities;
using Microsoft.Toolkit.Uwp.UI.Utilities;
using Microsoft.Toolkit.Uwp.Utilities;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;

using DiagnosticsDebug = System.Diagnostics.Debug;

namespace Microsoft.Toolkit.Uwp.UI.Controls.DataGridInternals
{
    internal class DataGridDataConnection
    {
        private int _backupSlotForCurrentChanged;
        private int _columnForCurrentChanged;
        private PropertyInfo[] _dataProperties;
        private IEnumerable _dataSource;
        private Type _dataType;
        private bool _expectingCurrentChanged;
        private ISupportIncrementalLoading _incrementalItemsSource;
        private object _itemToSelectOnCurrentChanged;
        private IAsyncOperation<LoadMoreItemsResult> _loadingOperation;
        private DataGrid _owner;
        private bool _scrollForCurrentChanged;
        private DataGridSelectionAction _selectionActionForCurrentChanged;
        private WeakEventListener<DataGridDataConnection, object, NotifyCollectionChangedEventArgs> _weakCollectionChangedListener;
        private WeakEventListener<DataGridDataConnection, object, IVectorChangedEventArgs> _weakVectorChangedListener;
        private WeakEventListener<DataGridDataConnection, object, CurrentChangingEventArgs> _weakCurrentChangingListener;
        private WeakEventListener<DataGridDataConnection, object, object> _weakCurrentChangedListener;
        private WeakEventListener<DataGridDataConnection, object, PropertyChangedEventArgs> _weakIncrementalItemsSourcePropertyChangedListener;

#if FEATURE_ICOLLECTIONVIEW_SORT
        private WeakEventListener<DataGridDataConnection, object, NotifyCollectionChangedEventArgs> _weakSortDescriptionsCollectionChangedListener;
#endif

        public DataGridDataConnection(DataGrid owner)
        {
            _owner = owner;
        }

        public bool AllowEdit
        {
            get
            {
                if (this.List == null)
                {
                    return true;
                }
                else
                {
                    return !this.List.IsReadOnly;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the collection view says it can sort.
        /// </summary>
        public bool AllowSort
        {
            get
            {
                if (this.CollectionView == null)
                {
                    return false;
                }

#if FEATURE_IEDITABLECOLLECTIONVIEW
                if (this.EditableCollectionView != null && (this.EditableCollectionView.IsAddingNew || this.EditableCollectionView.IsEditingItem))
                {
                    return false;
                }
#endif

#if FEATURE_ICOLLECTIONVIEW_SORT
                return this.CollectionView.CanSort;
#else
                return false;
#endif
            }
        }

        public bool CanCancelEdit
        {
            get
            {
#if FEATURE_IEDITABLECOLLECTIONVIEW
                return this.EditableCollectionView != null && this.EditableCollectionView.CanCancelEdit;
#else
                return false;
#endif
            }
        }

        public ICollectionView CollectionView
        {
            get
            {
                return this.DataSource as ICollectionView;
            }
        }

        public int Count
        {
            get
            {
                IList list = this.List;
                if (list != null)
                {
                    return list.Count;
                }

#if FEATURE_PAGEDCOLLECTIONVIEW
                PagedCollectionView collectionView = this.DataSource as PagedCollectionView;
                if (collectionView != null)
                {
                    return collectionView.Count;
                }
#endif

                int count = 0;
                IEnumerable enumerable = this.DataSource;
                if (enumerable != null)
                {
                    IEnumerator enumerator = enumerable.GetEnumerator();
                    if (enumerator != null)
                    {
                        while (enumerator.MoveNext())
                        {
                            count++;
                        }
                    }
                }

                return count;
            }
        }

        public bool DataIsPrimitive
        {
            get
            {
                return DataTypeIsPrimitive(this.DataType);
            }
        }

        public PropertyInfo[] DataProperties
        {
            get
            {
                if (_dataProperties == null)
                {
                    UpdateDataProperties();
                }

                return _dataProperties;
            }
        }

        public IEnumerable DataSource
        {
            get
            {
                return _dataSource;
            }

            set
            {
                _dataSource = value;

                // Because the DataSource is changing, we need to reset our cached values for DataType and DataProperties,
                // which are dependent on the current DataSource
                _dataType = null;
                UpdateDataProperties();
                UpdateIncrementalItemsSource();
            }
        }

        public Type DataType
        {
            get
            {
                // We need to use the raw ItemsSource as opposed to DataSource because DataSource
                // may be the ItemsSource wrapped in a collection view, in which case we wouldn't
                // be able to take T to be the type if we're given IEnumerable<T>
                if (_dataType == null && _owner.ItemsSource != null)
                {
                    _dataType = _owner.ItemsSource.GetItemType();
                }

                return _dataType;
            }
        }

        public bool HasMoreItems
        {
            get { return IsDataSourceIncremental && _incrementalItemsSource.HasMoreItems; }
        }

        public bool IsDataSourceIncremental
        {
            get { return _incrementalItemsSource != null; }
        }

        public bool IsLoadingMoreItems
        {
            get { return _loadingOperation != null; }
        }

#if FEATURE_IEDITABLECOLLECTIONVIEW
        public IEditableCollectionView EditableCollectionView
        {
            get
            {
                return this.DataSource as IEditableCollectionView;
            }
        }
#endif

        public bool EndingEdit
        {
            get;
            private set;
        }

        public bool EventsWired
        {
            get;
            private set;
        }

        public bool IsAddingNew
        {
            get
            {
#if FEATURE_IEDITABLECOLLECTIONVIEW
                return this.EditableCollectionView != null && this.EditableCollectionView.IsAddingNew;
#else
                return false;
#endif
            }
        }

        private bool IsGrouping
        {
            get
            {
                return this.CollectionView != null &&
#if FEATURE_ICOLLECTIONVIEW_GROUP
                    this.CollectionView.CanGroup &&
#endif
                    this.CollectionView.CollectionGroups != null &&
                    this.CollectionView.CollectionGroups.Count > 0;
            }
        }

        public IList List
        {
            get
            {
                return this.DataSource as IList;
            }
        }

        public int NewItemPlaceholderIndex
        {
            get
            {
#if FEATURE_IEDITABLECOLLECTIONVIEW
                if (this.EditableCollectionView != null && this.EditableCollectionView.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd)
                {
                    return this.Count - 1;
                }
#endif

                return -1;
            }
        }

#if FEATURE_IEDITABLECOLLECTIONVIEW
        public NewItemPlaceholderPosition NewItemPlaceholderPosition
        {
            get
            {
                if (this.EditableCollectionView != null)
                {
                    return this.EditableCollectionView.NewItemPlaceholderPosition;
                }

                return NewItemPlaceholderPosition.None;
            }
        }
#endif

        public bool ShouldAutoGenerateColumns
        {
            get
            {
                return _owner.AutoGenerateColumns
                    && (_owner.ColumnsInternal.AutogeneratedColumnCount == 0)
                    && ((this.DataProperties != null && this.DataProperties.Length > 0) || this.DataIsPrimitive);
            }
        }

#if FEATURE_ICOLLECTIONVIEW_SORT
        public SortDescriptionCollection SortDescriptions
        {
            get
            {
                if (this.CollectionView != null && this.CollectionView.CanSort)
                {
                    return this.CollectionView.SortDescriptions;
                }
                else
                {
                    return (SortDescriptionCollection)null;
                }
            }
        }
#endif

        public static bool CanEdit(Type type)
        {
            DiagnosticsDebug.Assert(type != null, "Expected non-null type.");

            type = type.GetNonNullableType();

            return
                type.GetTypeInfo().IsEnum
                || type == typeof(string)
                || type == typeof(char)
                || type == typeof(bool)
                || type == typeof(byte)
                || type == typeof(sbyte)
                || type == typeof(float)
                || type == typeof(double)
                || type == typeof(decimal)
                || type == typeof(short)
                || type == typeof(int)
                || type == typeof(long)
                || type == typeof(ushort)
                || type == typeof(uint)
                || type == typeof(ulong)
                || type == typeof(DateTime);
        }

        /// <summary>
        /// Puts the entity into editing mode if possible
        /// </summary>
        /// <param name="dataItem">The entity to edit</param>
        /// <returns>True if editing was started</returns>
        public bool BeginEdit(object dataItem)
        {
            if (dataItem == null)
            {
                return false;
            }

#if FEATURE_IEDITABLECOLLECTIONVIEW
            IEditableCollectionView editableCollectionView = this.EditableCollectionView;
            if (editableCollectionView != null)
            {
                if ((editableCollectionView.IsEditingItem && (dataItem == editableCollectionView.CurrentEditItem)) ||
                    (editableCollectionView.IsAddingNew && (dataItem == editableCollectionView.CurrentAddItem)))
                {
                    return true;
                }
                else
                {
                    editableCollectionView.EditItem(dataItem);
                    return editableCollectionView.IsEditingItem;
                }
            }
#endif

            IEditableObject editableDataItem = dataItem as IEditableObject;
            if (editableDataItem != null)
            {
                editableDataItem.BeginEdit();
                return true;
            }

            return true;
        }

        /// <summary>
        /// Cancels the current entity editing and exits the editing mode.
        /// </summary>
        /// <param name="dataItem">The entity being edited</param>
        /// <returns>True if a cancellation operation was invoked.</returns>
        public bool CancelEdit(object dataItem)
        {
#if FEATURE_IEDITABLECOLLECTIONVIEW
            IEditableCollectionView editableCollectionView = this.EditableCollectionView;
            if (editableCollectionView != null)
            {
                _owner.NoCurrentCellChangeCount++;
                this.EndingEdit = true;
                try
                {
                    if (editableCollectionView.IsAddingNew && dataItem == editableCollectionView.CurrentAddItem)
                    {
                        editableCollectionView.CancelNew();
                        return true;
                    }
                    else if (editableCollectionView.CanCancelEdit)
                    {
                        editableCollectionView.CancelEdit();
                        return true;
                    }
                }
                finally
                {
                    _owner.NoCurrentCellChangeCount--;
                    this.EndingEdit = false;
                }

                return false;
            }
#endif

            IEditableObject editableDataItem = dataItem as IEditableObject;
            if (editableDataItem != null)
            {
                editableDataItem.CancelEdit();
                return true;
            }

            return true;
        }

        /// <summary>
        /// Commits the current entity editing and exits the editing mode.
        /// </summary>
        /// <param name="dataItem">The entity being edited</param>
        /// <returns>True if a commit operation was invoked.</returns>
        public bool EndEdit(object dataItem)
        {
#if FEATURE_IEDITABLECOLLECTIONVIEW
            IEditableCollectionView editableCollectionView = this.EditableCollectionView;
            if (editableCollectionView != null)
            {
                // IEditableCollectionView.CommitEdit can potentially change currency. If it does,
                // we don't want to attempt a second commit inside our CurrentChanging event handler.
                _owner.NoCurrentCellChangeCount++;
                this.EndingEdit = true;
                try
                {
                    if (editableCollectionView.IsAddingNew && dataItem == editableCollectionView.CurrentAddItem)
                    {
                        editableCollectionView.CommitNew();
                    }
                    else
                    {
                        editableCollectionView.CommitEdit();
                    }
                }
                finally
                {
                    _owner.NoCurrentCellChangeCount--;
                    this.EndingEdit = false;
                }

                return true;
            }
#endif

            IEditableObject editableDataItem = dataItem as IEditableObject;
            if (editableDataItem != null)
            {
                editableDataItem.EndEdit();
            }

            return true;
        }

        // Assumes index >= 0, returns null if index >= Count
        public object GetDataItem(int index)
        {
            DiagnosticsDebug.Assert(index >= 0, "Expected positive index.");

            IList list = this.List;
            if (list != null)
            {
                return (index < list.Count) ? list[index] : null;
            }

#if FEATURE_PAGEDCOLLECTIONVIEW
            PagedCollectionView collectionView = this.DataSource as PagedCollectionView;
            if (collectionView != null)
            {
                return (index < collectionView.Count) ? collectionView.GetItemAt(index) : null;
            }
#endif

            IEnumerable enumerable = this.DataSource;
            if (enumerable != null)
            {
                IEnumerator enumerator = enumerable.GetEnumerator();
                int i = -1;
                while (enumerator.MoveNext() && i < index)
                {
                    i++;
                    if (i == index)
                    {
                        return enumerator.Current;
                    }
                }
            }

            return null;
        }

        public bool GetPropertyIsReadOnly(string propertyName)
        {
            if (this.DataType != null)
            {
                if (!string.IsNullOrEmpty(propertyName))
                {
                    Type propertyType = this.DataType;
                    PropertyInfo propertyInfo = null;
                    List<string> propertyNames = TypeHelper.SplitPropertyPath(propertyName);
                    for (int i = 0; i < propertyNames.Count; i++)
                    {
                        if (propertyType.GetTypeInfo().GetIsReadOnly())
                        {
                            return true;
                        }

                        propertyInfo = propertyType.GetPropertyOrIndexer(propertyNames[i], out _);
                        if (propertyInfo == null || propertyInfo.GetIsReadOnly())
                        {
                            // Either the property doesn't exist or it does exist but is read-only.
                            return true;
                        }

                        // Check if EditableAttribute is defined on the property and if it indicates uneditable
                        var editableAttribute = propertyInfo.GetCustomAttributes().OfType<EditableAttribute>().FirstOrDefault();
                        if (editableAttribute != null && !editableAttribute.AllowEdit)
                        {
                            return true;
                        }

                        propertyType = propertyInfo.PropertyType.GetNonNullableType();
                    }

                    return propertyInfo == null || !propertyInfo.CanWrite || !this.AllowEdit || !CanEdit(propertyType);
                }
                else
                {
                    if (this.DataType.GetTypeInfo().GetIsReadOnly())
                    {
                        return true;
                    }
                }
            }

            return !this.AllowEdit;
        }

        public int IndexOf(object dataItem)
        {
            IList list = this.List;
            if (list != null)
            {
                return list.IndexOf(dataItem);
            }

#if FEATURE_PAGEDCOLLECTIONVIEW
            PagedCollectionView cv = this.DataSource as PagedCollectionView;
            if (cv != null)
            {
                return cv.IndexOf(dataItem);
            }
#endif

            IEnumerable enumerable = this.DataSource;
            if (enumerable != null && dataItem != null)
            {
                int index = 0;
                foreach (object dataItemTmp in enumerable)
                {
                    if ((dataItem == null && dataItemTmp == null) ||
                        dataItem.Equals(dataItemTmp))
                    {
                        return index;
                    }

                    index++;
                }
            }

            return -1;
        }

        public void LoadMoreItems(uint count)
        {
            DiagnosticsDebug.Assert(_loadingOperation == null, "Expected _loadingOperation == null.");

            _loadingOperation = _incrementalItemsSource.LoadMoreItemsAsync(count);

            if (_loadingOperation != null)
            {
                _loadingOperation.Completed = OnLoadingOperationCompleted;
            }
        }

#if FEATURE_PAGEDCOLLECTIONVIEW
        /// <summary>
        /// Creates a collection view around the DataGrid's source. ICollectionViewFactory is
        /// used if the source implements it. Otherwise a PagedCollectionView is returned.
        /// </summary>
        /// <param name="source">Enumerable source for which to create a view</param>
        /// <returns>ICollectionView view over the provided source</returns>
#else
        /// <summary>
        /// Creates a collection view around the DataGrid's source. ICollectionViewFactory is
        /// used if the source implements it.
        /// </summary>
        /// <param name="source">Enumerable source for which to create a view</param>
        /// <returns>ICollectionView view over the provided source</returns>
#endif
        internal static ICollectionView CreateView(IEnumerable source)
        {
            DiagnosticsDebug.Assert(source != null, "source unexpectedly null");
            DiagnosticsDebug.Assert(!(source is ICollectionView), "source is an ICollectionView");

            ICollectionView collectionView = null;

            ICollectionViewFactory collectionViewFactory = source as ICollectionViewFactory;
            if (collectionViewFactory != null)
            {
                // If the source is a collection view factory, give it a chance to produce a custom collection view.
                collectionView = collectionViewFactory.CreateView();

                // Intentionally not catching potential exception thrown by ICollectionViewFactory.CreateView().
            }

#if FEATURE_PAGEDCOLLECTIONVIEW
            if (collectionView == null)
            {
                collectionView = new PagedCollectionView(source);
            }
#endif

            if (collectionView == null)
            {
                IList sourceAsList = source as IList;
                if (sourceAsList != null)
                {
                    collectionView = new ListCollectionView(sourceAsList);
                }
                else
                {
                    collectionView = new EnumerableCollectionView(source);
                }
            }

            return collectionView;
        }

        internal static bool DataTypeIsPrimitive(Type dataType)
        {
            if (dataType != null)
            {
                Type type = TypeHelper.GetNonNullableType(dataType);  // no-opt if dataType isn't nullable
                return
                    type.GetTypeInfo().IsPrimitive ||
                    type == typeof(string) ||
                    type == typeof(decimal) ||
                    type == typeof(DateTime);
            }
            else
            {
                return false;
            }
        }

        internal void ClearDataProperties()
        {
            _dataProperties = null;
        }

        internal void MoveCurrentTo(object item, int backupSlot, int columnIndex, DataGridSelectionAction action, bool scrollIntoView)
        {
            if (this.CollectionView != null)
            {
                _expectingCurrentChanged = true;
                _columnForCurrentChanged = columnIndex;
                _itemToSelectOnCurrentChanged = item;
                _selectionActionForCurrentChanged = action;
                _scrollForCurrentChanged = scrollIntoView;
                _backupSlotForCurrentChanged = backupSlot;

                var itemIsCollectionViewGroup = item is ICollectionViewGroup;
                this.CollectionView.MoveCurrentTo((itemIsCollectionViewGroup || this.IndexOf(item) == this.NewItemPlaceholderIndex) ? null : item);

                _expectingCurrentChanged = false;
            }
        }

        internal void UnWireEvents(IEnumerable value)
        {
            INotifyCollectionChanged notifyingDataSource1 = value as INotifyCollectionChanged;
            if (notifyingDataSource1 != null && _weakCollectionChangedListener != null)
            {
                _weakCollectionChangedListener.Detach();
                _weakCollectionChangedListener = null;
            }

            IObservableVector<object> notifyingDataSource2 = value as IObservableVector<object>;
            if (notifyingDataSource2 != null && _weakVectorChangedListener != null)
            {
                _weakVectorChangedListener.Detach();
                _weakVectorChangedListener = null;
            }

#if FEATURE_ICOLLECTIONVIEW_SORT
            if (this.SortDescriptions != null && _weakSortDescriptionsCollectionChangedListener != null)
            {
                _weakSortDescriptionsCollectionChangedListener.Detach();
                _weakSortDescriptionsCollectionChangedListener = null;
            }
#endif

            if (this.CollectionView != null)
            {
                if (_weakCurrentChangedListener != null)
                {
                    _weakCurrentChangedListener.Detach();
                    _weakCurrentChangedListener = null;
                }

                if (_weakCurrentChangingListener != null)
                {
                    _weakCurrentChangingListener.Detach();
                    _weakCurrentChangingListener = null;
                }
            }

            this.EventsWired = false;
        }

        internal void WireEvents(IEnumerable value)
        {
            INotifyCollectionChanged notifyingDataSource1 = value as INotifyCollectionChanged;
            if (notifyingDataSource1 != null)
            {
                _weakCollectionChangedListener = new WeakEventListener<DataGridDataConnection, object, NotifyCollectionChangedEventArgs>(this);
                _weakCollectionChangedListener.OnEventAction = (instance, source, eventArgs) => instance.NotifyingDataSource_CollectionChanged(source, eventArgs);
                _weakCollectionChangedListener.OnDetachAction = (weakEventListener) => notifyingDataSource1.CollectionChanged -= weakEventListener.OnEvent;
                notifyingDataSource1.CollectionChanged += _weakCollectionChangedListener.OnEvent;
            }
            else
            {
                IObservableVector<object> notifyingDataSource2 = value as IObservableVector<object>;
                if (notifyingDataSource2 != null)
                {
                    _weakVectorChangedListener = new WeakEventListener<DataGridDataConnection, object, IVectorChangedEventArgs>(this);
                    _weakVectorChangedListener.OnEventAction = (instance, source, eventArgs) => instance.NotifyingDataSource_VectorChanged(source as IObservableVector<object>, eventArgs);
                    _weakVectorChangedListener.OnDetachAction = (weakEventListener) => notifyingDataSource2.VectorChanged -= _weakVectorChangedListener.OnEvent;
                    notifyingDataSource2.VectorChanged += _weakVectorChangedListener.OnEvent;
                }
            }

#if FEATURE_ICOLLECTIONVIEW_SORT
            if (this.SortDescriptions != null)
            {
                INotifyCollectionChanged sortDescriptionsINCC = (INotifyCollectionChanged)this.SortDescriptions;
                _weakSortDescriptionsCollectionChangedListener = new WeakEventListener<DataGridDataConnection, object, NotifyCollectionChangedEventArgs>(this);
                _weakSortDescriptionsCollectionChangedListener.OnEventAction = (instance, source, eventArgs) => instance.CollectionView_SortDescriptions_CollectionChanged(source, eventArgs);
                _weakSortDescriptionsCollectionChangedListener.OnDetachAction = (weakEventListener) => sortDescriptionsINCC.CollectionChanged -= weakEventListener.OnEvent;
                sortDescriptionsINCC.CollectionChanged += _weakSortDescriptionsCollectionChangedListener.OnEvent;
            }
#endif

            if (this.CollectionView != null)
            {
                // A local variable must be used in the lambda expression or the CollectionView will leak
                ICollectionView collectionView = this.CollectionView;

                _weakCurrentChangedListener = new WeakEventListener<DataGridDataConnection, object, object>(this);
                _weakCurrentChangedListener.OnEventAction = (instance, source, eventArgs) => instance.CollectionView_CurrentChanged(source, null);
                _weakCurrentChangedListener.OnDetachAction = (weakEventListener) => collectionView.CurrentChanged -= weakEventListener.OnEvent;
                this.CollectionView.CurrentChanged += _weakCurrentChangedListener.OnEvent;

                _weakCurrentChangingListener = new WeakEventListener<DataGridDataConnection, object, CurrentChangingEventArgs>(this);
                _weakCurrentChangingListener.OnEventAction = (instance, source, eventArgs) => instance.CollectionView_CurrentChanging(source, eventArgs);
                _weakCurrentChangingListener.OnDetachAction = (weakEventListener) => collectionView.CurrentChanging -= weakEventListener.OnEvent;
                this.CollectionView.CurrentChanging += _weakCurrentChangingListener.OnEvent;
            }

            this.EventsWired = true;
        }

        private void CollectionView_CurrentChanged(object sender, object e)
        {
            if (_expectingCurrentChanged)
            {
                // Committing Edit could cause our item to move to a group that no longer exists.  In
                // this case, we need to update the item.
                ICollectionViewGroup collectionViewGroup = _itemToSelectOnCurrentChanged as ICollectionViewGroup;
                if (collectionViewGroup != null)
                {
                    DataGridRowGroupInfo groupInfo = _owner.RowGroupInfoFromCollectionViewGroup(collectionViewGroup);
                    if (groupInfo == null)
                    {
                        // Move to the next slot if the target slot isn't visible
                        if (!_owner.IsSlotVisible(_backupSlotForCurrentChanged))
                        {
                            _backupSlotForCurrentChanged = _owner.GetNextVisibleSlot(_backupSlotForCurrentChanged);
                        }

                        // Move to the next best slot if we've moved past all the slots.  This could happen if multiple
                        // groups were removed.
                        if (_backupSlotForCurrentChanged >= _owner.SlotCount)
                        {
                            _backupSlotForCurrentChanged = _owner.GetPreviousVisibleSlot(_owner.SlotCount);
                        }

                        // Update the itemToSelect
                        int newCurrentPosition = -1;
                        _itemToSelectOnCurrentChanged = _owner.ItemFromSlot(_backupSlotForCurrentChanged, ref newCurrentPosition);
                    }
                }

                _owner.ProcessSelectionAndCurrency(
                    _columnForCurrentChanged,
                    _itemToSelectOnCurrentChanged,
                    _backupSlotForCurrentChanged,
                    _selectionActionForCurrentChanged,
                    _scrollForCurrentChanged);
            }
            else if (this.CollectionView != null)
            {
                _owner.UpdateStateOnCurrentChanged(this.CollectionView.CurrentItem, this.CollectionView.CurrentPosition);
            }
        }

        private void CollectionView_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            if (_owner.NoCurrentCellChangeCount == 0 &&
                !_expectingCurrentChanged &&
                !this.EndingEdit &&
                !_owner.CommitEdit())
            {
                // If CommitEdit failed, then the user has most likely input invalid data.
                // Cancel the current change if possible, otherwise abort the edit.
                if (e.IsCancelable)
                {
                    e.Cancel = true;
                }
                else
                {
                    _owner.CancelEdit(DataGridEditingUnit.Row, false);
                }
            }
        }

#if FEATURE_ICOLLECTIONVIEW_SORT
        private void CollectionView_SortDescriptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_owner.ColumnsItemsInternal.Count == 0)
            {
                return;
            }

            // Refresh sort description
            foreach (DataGridColumn column in _owner.ColumnsItemsInternal)
            {
                column.HeaderCell.ApplyState(true);
            }
        }
#endif

        private void NotifyingDataSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_owner.LoadingOrUnloadingRow)
            {
                throw DataGridError.DataGrid.CannotChangeItemsWhenLoadingRows();
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    DiagnosticsDebug.Assert(e.NewItems != null, "Unexpected NotifyCollectionChangedAction.Add notification");
                    DiagnosticsDebug.Assert(this.ShouldAutoGenerateColumns || this.IsGrouping || e.NewItems.Count == 1, "Expected NewItems.Count equals 1.");
                    NotifyingDataSource_Add(e.NewStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    IList removedItems = e.OldItems;
                    if (removedItems == null || e.OldStartingIndex < 0)
                    {
                        DiagnosticsDebug.Assert(false, "Unexpected NotifyCollectionChangedAction.Remove notification");
                        return;
                    }

                    if (!this.IsGrouping)
                    {
                        // If we're grouping then we handle this through the CollectionViewGroup notifications.
                        // Remove is a single item operation.
                        foreach (object item in removedItems)
                        {
                            DiagnosticsDebug.Assert(item != null, "Expected non-null item.");
                            _owner.RemoveRowAt(e.OldStartingIndex, item);
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    throw new NotSupportedException();

                case NotifyCollectionChangedAction.Reset:
                    NotifyingDataSource_Reset();
                    break;
            }
        }

        private void NotifyingDataSource_VectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs e)
        {
            if (_owner.LoadingOrUnloadingRow)
            {
                throw DataGridError.DataGrid.CannotChangeItemsWhenLoadingRows();
            }

            int index = (int)e.Index;

            switch (e.CollectionChange)
            {
                case CollectionChange.ItemChanged:
                    throw new NotSupportedException();

                case CollectionChange.ItemInserted:
                    NotifyingDataSource_Add(index);
                    break;

                case CollectionChange.ItemRemoved:
                    if (!this.IsGrouping)
                    {
                        // If we're grouping then we handle this through the CollectionViewGroup notifications.
                        // Remove is a single item operation.
                        _owner.RemoveRowAt(index, sender[index]);
                    }

                    break;

                case CollectionChange.Reset:
                    NotifyingDataSource_Reset();
                    break;
            }
        }

        private void NotifyingDataSource_Add(int index)
        {
            if (this.ShouldAutoGenerateColumns)
            {
                // The columns are also affected (not just rows) in this case, so reset everything.
                _owner.InitializeElements(false /*recycleRows*/);
            }
            else if (!this.IsGrouping)
            {
                // If we're grouping then we handle this through the CollectionViewGroup notifications.
                // Add is a single item operation.
                _owner.InsertRowAt(index);
            }
        }

        private void NotifyingDataSource_Reset()
        {
            // Did the data type change during the reset?  If not, we can recycle
            // the existing rows instead of having to clear them all.  We still need to clear our cached
            // values for DataType and DataProperties, though, because the collection has been reset.
            Type previousDataType = _dataType;
            _dataType = null;
            if (previousDataType != this.DataType)
            {
                ClearDataProperties();
                _owner.InitializeElements(false /*recycleRows*/);
            }
            else
            {
                _owner.InitializeElements(!this.ShouldAutoGenerateColumns /*recycleRows*/);
            }
        }

        private void NotifyingIncrementalItemsSource(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HasMoreItems))
            {
                _owner.LoadMoreDataFromIncrementalItemsSource();
            }
        }

        private void OnLoadingOperationCompleted(object info, AsyncStatus status)
        {
            if (status != AsyncStatus.Started)
            {
                _loadingOperation = null;
            }
        }

        private void UpdateDataProperties()
        {
            Type dataType = this.DataType;

            if (this.DataSource != null && dataType != null && !DataTypeIsPrimitive(dataType))
            {
                _dataProperties = dataType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                DiagnosticsDebug.Assert(_dataProperties != null, "Expected non-null _dataProperties.");
            }
            else
            {
                _dataProperties = null;
            }
        }

        private void UpdateIncrementalItemsSource()
        {
            if (_weakIncrementalItemsSourcePropertyChangedListener != null)
            {
                _weakIncrementalItemsSourcePropertyChangedListener.Detach();
                _weakIncrementalItemsSourcePropertyChangedListener = null;
            }

            // Determine if incremental loading should be used
            if (_dataSource is ISupportIncrementalLoading incrementalDataSource)
            {
                _incrementalItemsSource = incrementalDataSource;
            }
            else if (_owner.ItemsSource is ISupportIncrementalLoading incrementalItemsSource)
            {
                _incrementalItemsSource = incrementalItemsSource;
            }
            else
            {
                _incrementalItemsSource = default(ISupportIncrementalLoading);
            }

            if (_incrementalItemsSource != null && _incrementalItemsSource is INotifyPropertyChanged inpc)
            {
                _weakIncrementalItemsSourcePropertyChangedListener = new WeakEventListener<DataGridDataConnection, object, PropertyChangedEventArgs>(this);
                _weakIncrementalItemsSourcePropertyChangedListener.OnEventAction = (instance, source, eventArgs) => instance.NotifyingIncrementalItemsSource(source, eventArgs);
                _weakIncrementalItemsSourcePropertyChangedListener.OnDetachAction = (weakEventListener) => inpc.PropertyChanged -= weakEventListener.OnEvent;
                inpc.PropertyChanged += _weakIncrementalItemsSourcePropertyChangedListener.OnEvent;
            }

            if (_loadingOperation != null)
            {
                _loadingOperation.Cancel();
                _loadingOperation = null;
            }
        }
    }
}