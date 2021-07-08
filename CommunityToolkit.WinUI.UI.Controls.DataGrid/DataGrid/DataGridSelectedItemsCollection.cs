// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using CommunityToolkit.WinUI.UI.Controls.DataGridInternals;
using CommunityToolkit.WinUI.Utilities;
using Microsoft.UI.Xaml.Controls;

using DiagnosticsDebug = System.Diagnostics.Debug;

namespace CommunityToolkit.WinUI.UI.Controls
{
    internal class DataGridSelectedItemsCollection : IList
    {
        private List<object> _oldSelectedItemsCache;
        private IndexToValueTable<bool> _oldSelectedSlotsTable;
        private List<object> _selectedItemsCache;
        private IndexToValueTable<bool> _selectedSlotsTable;

        public DataGridSelectedItemsCollection(DataGrid owningGrid)
        {
            this.OwningGrid = owningGrid;
            _oldSelectedItemsCache = new List<object>();
            _oldSelectedSlotsTable = new IndexToValueTable<bool>();
            _selectedItemsCache = new List<object>();
            _selectedSlotsTable = new IndexToValueTable<bool>();
        }

        public object this[int index]
        {
            get
            {
                if (index < 0 || index >= _selectedSlotsTable.IndexCount)
                {
                    throw DataGridError.DataGrid.ValueMustBeBetween("index", "Index", 0, true, _selectedSlotsTable.IndexCount, false);
                }

                int slot = _selectedSlotsTable.GetNthIndex(index);
                DiagnosticsDebug.Assert(slot >= 0, "Expected positive slot.");
                return this.OwningGrid.DataConnection.GetDataItem(this.OwningGrid.RowIndexFromSlot(slot));
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public int Add(object dataItem)
        {
            if (this.OwningGrid.SelectionMode == DataGridSelectionMode.Single)
            {
                throw DataGridError.DataGridSelectedItemsCollection.CannotChangeSelectedItemsCollectionInSingleMode();
            }

            int itemIndex = this.OwningGrid.DataConnection.IndexOf(dataItem);
            if (itemIndex == -1)
            {
                throw DataGridError.DataGrid.ItemIsNotContainedInTheItemsSource("dataItem");
            }

            DiagnosticsDebug.Assert(itemIndex >= 0, "Expected positive itemIndex.");

            int slot = this.OwningGrid.SlotFromRowIndex(itemIndex);
            if (_selectedSlotsTable.RangeCount == 0)
            {
                this.OwningGrid.SelectedItem = dataItem;
            }
            else
            {
                this.OwningGrid.SetRowSelection(slot, true /*isSelected*/, false /*setAnchorSlot*/);
            }

            return _selectedSlotsTable.IndexOf(slot);
        }

        public void Clear()
        {
            if (this.OwningGrid.SelectionMode == DataGridSelectionMode.Single)
            {
                throw DataGridError.DataGridSelectedItemsCollection.CannotChangeSelectedItemsCollectionInSingleMode();
            }

            if (_selectedSlotsTable.RangeCount > 0)
            {
                // Clearing the selection does not reset the potential current cell.
                if (!this.OwningGrid.CommitEdit(DataGridEditingUnit.Row, true /*exitEditing*/))
                {
                    // Edited value couldn't be committed or aborted
                    return;
                }

                this.OwningGrid.ClearRowSelection(true /*resetAnchorSlot*/);
            }
        }

        public bool Contains(object dataItem)
        {
            int itemIndex = this.OwningGrid.DataConnection.IndexOf(dataItem);
            if (itemIndex == -1)
            {
                return false;
            }

            DiagnosticsDebug.Assert(itemIndex >= 0, "Expected positive itemIndex.");

            return ContainsSlot(this.OwningGrid.SlotFromRowIndex(itemIndex));
        }

        public int IndexOf(object dataItem)
        {
            int itemIndex = this.OwningGrid.DataConnection.IndexOf(dataItem);
            if (itemIndex == -1)
            {
                return -1;
            }

            DiagnosticsDebug.Assert(itemIndex >= 0, "Expected positive itemIndex.");

            int slot = this.OwningGrid.SlotFromRowIndex(itemIndex);
            return _selectedSlotsTable.IndexOf(slot);
        }

        public void Insert(int index, object dataItem)
        {
            throw new NotSupportedException();
        }

        public void Remove(object dataItem)
        {
            if (this.OwningGrid.SelectionMode == DataGridSelectionMode.Single)
            {
                throw DataGridError.DataGridSelectedItemsCollection.CannotChangeSelectedItemsCollectionInSingleMode();
            }

            int itemIndex = this.OwningGrid.DataConnection.IndexOf(dataItem);
            if (itemIndex == -1)
            {
                return;
            }

            DiagnosticsDebug.Assert(itemIndex >= 0, "Expected positive itemIndex.");

            if (itemIndex == this.OwningGrid.CurrentSlot &&
                !this.OwningGrid.CommitEdit(DataGridEditingUnit.Row, true /*exitEditing*/))
            {
                // Edited value couldn't be committed or aborted
                return;
            }

            this.OwningGrid.SetRowSelection(itemIndex, false /*isSelected*/, false /*setAnchorSlot*/);
        }

        public void RemoveAt(int index)
        {
            if (this.OwningGrid.SelectionMode == DataGridSelectionMode.Single)
            {
                throw DataGridError.DataGridSelectedItemsCollection.CannotChangeSelectedItemsCollectionInSingleMode();
            }

            if (index < 0 || index >= _selectedSlotsTable.IndexCount)
            {
                throw DataGridError.DataGrid.ValueMustBeBetween("index", "Index", 0, true, _selectedSlotsTable.IndexCount, false);
            }

            int rowIndex = _selectedSlotsTable.GetNthIndex(index);
            DiagnosticsDebug.Assert(rowIndex > -1, "Expected positive itemIndex.");

            if (rowIndex == this.OwningGrid.CurrentSlot &&
                !this.OwningGrid.CommitEdit(DataGridEditingUnit.Row, true /*exitEditing*/))
            {
                // Edited value couldn't be committed or aborted
                return;
            }

            this.OwningGrid.SetRowSelection(rowIndex, false /*isSelected*/, false /*setAnchorSlot*/);
        }

        public int Count
        {
            get
            {
                return _selectedSlotsTable.IndexCount;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

        public void CopyTo(Array array, int index)
        {
            // TODO: Not supported yet.
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            DiagnosticsDebug.Assert(this.OwningGrid != null, "Expected non-null owning DataGrid.");
            DiagnosticsDebug.Assert(this.OwningGrid.DataConnection != null, "Expected non-null owning DataGrid.DataConnection.");
            DiagnosticsDebug.Assert(_selectedSlotsTable != null, "Expected non-null _selectedSlotsTable.");

            foreach (int slot in _selectedSlotsTable.GetIndexes())
            {
                int rowIndex = this.OwningGrid.RowIndexFromSlot(slot);
                DiagnosticsDebug.Assert(rowIndex > -1, "Expected positive rowIndex.");
                yield return this.OwningGrid.DataConnection.GetDataItem(rowIndex);
            }
        }

        internal DataGrid OwningGrid
        {
            get;
            private set;
        }

        internal List<object> SelectedItemsCache
        {
            get
            {
                return _selectedItemsCache;
            }

            set
            {
                _selectedItemsCache = value;
                UpdateIndexes();
            }
        }

        internal void ClearRows()
        {
            _selectedSlotsTable.Clear();
            _selectedItemsCache.Clear();
        }

        internal bool ContainsSlot(int slot)
        {
            return _selectedSlotsTable.Contains(slot);
        }

        internal bool ContainsAll(int startSlot, int endSlot)
        {
            int itemSlot = this.OwningGrid.RowGroupHeadersTable.GetNextGap(startSlot - 1);
            while (itemSlot <= endSlot)
            {
                // Skip over the RowGroupHeaderSlots
                int nextRowGroupHeaderSlot = this.OwningGrid.RowGroupHeadersTable.GetNextIndex(itemSlot);
                int lastItemSlot = nextRowGroupHeaderSlot == -1 ? endSlot : Math.Min(endSlot, nextRowGroupHeaderSlot - 1);
                if (!_selectedSlotsTable.ContainsAll(itemSlot, lastItemSlot))
                {
                    return false;
                }

                itemSlot = this.OwningGrid.RowGroupHeadersTable.GetNextGap(lastItemSlot);
            }

            return true;
        }

        // Called when an item is deleted from the ItemsSource as opposed to just being unselected
        internal void Delete(int slot, object item)
        {
            if (_oldSelectedSlotsTable.Contains(slot))
            {
                this.OwningGrid.SelectionHasChanged = true;
            }

            DeleteSlot(slot);
            _selectedItemsCache.Remove(item);
        }

        internal void DeleteSlot(int slot)
        {
            _selectedSlotsTable.RemoveIndex(slot);
            _oldSelectedSlotsTable.RemoveIndex(slot);
        }

        // Returns the inclusive index count between lowerBound and upperBound of all indexes with the given value
        internal int GetIndexCount(int lowerBound, int upperBound)
        {
            return _selectedSlotsTable.GetIndexCount(lowerBound, upperBound, true);
        }

        internal IEnumerable<int> GetIndexes()
        {
            return _selectedSlotsTable.GetIndexes();
        }

        internal IEnumerable<int> GetSlots(int startSlot)
        {
            return _selectedSlotsTable.GetIndexes(startSlot);
        }

        internal SelectionChangedEventArgs GetSelectionChangedEventArgs()
        {
            List<object> addedSelectedItems = new List<object>();
            List<object> removedSelectedItems = new List<object>();

            // Compare the old selected indexes with the current selection to determine which items
            // have been added and removed since the last time this method was called
            foreach (int newSlot in _selectedSlotsTable.GetIndexes())
            {
                object newItem = this.OwningGrid.DataConnection.GetDataItem(this.OwningGrid.RowIndexFromSlot(newSlot));
                if (_oldSelectedSlotsTable.Contains(newSlot))
                {
                    _oldSelectedSlotsTable.RemoveValue(newSlot);
                    _oldSelectedItemsCache.Remove(newItem);
                }
                else
                {
                    addedSelectedItems.Add(newItem);
                }
            }

            foreach (object oldItem in _oldSelectedItemsCache)
            {
                removedSelectedItems.Add(oldItem);
            }

            // The current selection becomes the old selection
            _oldSelectedSlotsTable = _selectedSlotsTable.Copy();
            _oldSelectedItemsCache = new List<object>(_selectedItemsCache);

            return new SelectionChangedEventArgs(removedSelectedItems, addedSelectedItems);
        }

        internal void InsertIndex(int slot)
        {
            _selectedSlotsTable.InsertIndex(slot);
            _oldSelectedSlotsTable.InsertIndex(slot);

            // It's possible that we're inserting an item that was just removed.  If that's the case,
            // and the re-inserted item used to be selected, we want to update the _oldSelectedSlotsTable
            // to include the item's new index within the collection.
            int rowIndex = this.OwningGrid.RowIndexFromSlot(slot);
            if (rowIndex != -1)
            {
                object insertedItem = this.OwningGrid.DataConnection.GetDataItem(rowIndex);
                if (insertedItem != null && _oldSelectedItemsCache.Contains(insertedItem))
                {
                    _oldSelectedSlotsTable.AddValue(slot, true);
                }
            }
        }

        internal void SelectSlot(int slot, bool select)
        {
            if (this.OwningGrid.RowGroupHeadersTable.Contains(slot))
            {
                return;
            }

            if (select)
            {
                if (!_selectedSlotsTable.Contains(slot))
                {
                    _selectedItemsCache.Add(this.OwningGrid.DataConnection.GetDataItem(this.OwningGrid.RowIndexFromSlot(slot)));
                }

                _selectedSlotsTable.AddValue(slot, true);
            }
            else
            {
                if (_selectedSlotsTable.Contains(slot))
                {
                    _selectedItemsCache.Remove(this.OwningGrid.DataConnection.GetDataItem(this.OwningGrid.RowIndexFromSlot(slot)));
                }

                _selectedSlotsTable.RemoveValue(slot);
            }
        }

        internal void SelectSlots(int startSlot, int endSlot, bool select)
        {
            int itemSlot = this.OwningGrid.RowGroupHeadersTable.GetNextGap(startSlot - 1);
            int endItemSlot = this.OwningGrid.RowGroupHeadersTable.GetPreviousGap(endSlot + 1);

            if (select)
            {
                while (itemSlot <= endItemSlot)
                {
                    // Add the newly selected item slots by skipping over the RowGroupHeaderSlots
                    int nextRowGroupHeaderSlot = this.OwningGrid.RowGroupHeadersTable.GetNextIndex(itemSlot);
                    int lastItemSlot = nextRowGroupHeaderSlot == -1 ? endItemSlot : Math.Min(endItemSlot, nextRowGroupHeaderSlot - 1);

                    for (int slot = itemSlot; slot <= lastItemSlot; slot++)
                    {
                        if (!_selectedSlotsTable.Contains(slot))
                        {
                            _selectedItemsCache.Add(this.OwningGrid.DataConnection.GetDataItem(this.OwningGrid.RowIndexFromSlot(slot)));
                        }
                    }

                    _selectedSlotsTable.AddValues(itemSlot, lastItemSlot - itemSlot + 1, true);
                    itemSlot = this.OwningGrid.RowGroupHeadersTable.GetNextGap(lastItemSlot);
                }
            }
            else
            {
                while (itemSlot <= endItemSlot)
                {
                    // Remove the unselected item slots by skipping over the RowGroupHeaderSlots
                    int nextRowGroupHeaderSlot = this.OwningGrid.RowGroupHeadersTable.GetNextIndex(itemSlot);
                    int lastItemSlot = nextRowGroupHeaderSlot == -1 ? endItemSlot : Math.Min(endItemSlot, nextRowGroupHeaderSlot - 1);

                    for (int slot = itemSlot; slot <= lastItemSlot; slot++)
                    {
                        if (_selectedSlotsTable.Contains(slot))
                        {
                            _selectedItemsCache.Remove(this.OwningGrid.DataConnection.GetDataItem(this.OwningGrid.RowIndexFromSlot(slot)));
                        }
                    }

                    _selectedSlotsTable.RemoveValues(itemSlot, lastItemSlot - itemSlot + 1);
                    itemSlot = this.OwningGrid.RowGroupHeadersTable.GetNextGap(lastItemSlot);
                }
            }
        }

        internal void UpdateIndexes()
        {
            _oldSelectedSlotsTable.Clear();
            _selectedSlotsTable.Clear();

            if (this.OwningGrid.DataConnection.DataSource == null)
            {
                if (this.SelectedItemsCache.Count > 0)
                {
                    this.OwningGrid.SelectionHasChanged = true;
                    this.SelectedItemsCache.Clear();
                }
            }
            else
            {
                List<object> tempSelectedItemsCache = new List<object>();
                foreach (object item in _selectedItemsCache)
                {
                    int index = this.OwningGrid.DataConnection.IndexOf(item);
                    if (index != -1)
                    {
                        tempSelectedItemsCache.Add(item);
                        _selectedSlotsTable.AddValue(this.OwningGrid.SlotFromRowIndex(index), true);
                    }
                }

                foreach (object item in _oldSelectedItemsCache)
                {
                    int index = this.OwningGrid.DataConnection.IndexOf(item);
                    if (index == -1)
                    {
                        this.OwningGrid.SelectionHasChanged = true;
                    }
                    else
                    {
                        _oldSelectedSlotsTable.AddValue(this.OwningGrid.SlotFromRowIndex(index), true);
                    }
                }

                _selectedItemsCache = tempSelectedItemsCache;
            }
        }
    }
}