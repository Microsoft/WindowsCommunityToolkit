// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

using DiagnosticsDebug = System.Diagnostics.Debug;

namespace CommunityToolkit.WinUI.UI.Controls.DataGridInternals
{
    internal class DataGridDisplayData
    {
        private Stack<DataGridRow> _fullyRecycledRows; // list of Rows that have been fully recycled (Collapsed)
        private int _headScrollingElements; // index of the row in _scrollingRows that is the first displayed row
        private DataGrid _owner;
        private Stack<DataGridRow> _recyclableRows; // list of Rows which have not been fully recycled (avoids Measure in several cases)
        private List<UIElement> _scrollingElements; // circular list of displayed elements
        private Stack<DataGridRowGroupHeader> _fullyRecycledGroupHeaders; // list of GroupHeaders that have been fully recycled (Collapsed)
        private Stack<DataGridRowGroupHeader> _recyclableGroupHeaders; // list of GroupHeaders which have not been fully recycled (avoids Measure in several cases)

        public DataGridDisplayData(DataGrid owner)
        {
            _owner = owner;

            ResetSlotIndexes();
            this.FirstDisplayedScrollingCol = -1;
            this.LastTotallyDisplayedScrollingCol = -1;

            _scrollingElements = new List<UIElement>();
            _recyclableRows = new Stack<DataGridRow>();
            _fullyRecycledRows = new Stack<DataGridRow>();
            _recyclableGroupHeaders = new Stack<DataGridRowGroupHeader>();
            _fullyRecycledGroupHeaders = new Stack<DataGridRowGroupHeader>();
        }

        public int FirstDisplayedScrollingCol
        {
            get;
            set;
        }

        public int FirstScrollingSlot
        {
            get;
            set;
        }

        public int LastScrollingSlot
        {
            get;
            set;
        }

        public int LastTotallyDisplayedScrollingCol
        {
            get;
            set;
        }

        public int NumDisplayedScrollingElements
        {
            get
            {
                return _scrollingElements.Count;
            }
        }

        public int NumTotallyDisplayedScrollingElements
        {
            get;
            set;
        }

        internal double PendingVerticalScrollHeight
        {
            get;
            set;
        }

        internal void AddRecylableRow(DataGridRow row)
        {
            DiagnosticsDebug.Assert(!_recyclableRows.Contains(row), "Expected row parameter to be non-recyclable.");

            row.DetachFromDataGrid(true);
            _recyclableRows.Push(row);
        }

        internal void AddRecylableRowGroupHeader(DataGridRowGroupHeader groupHeader)
        {
            DiagnosticsDebug.Assert(!_recyclableGroupHeaders.Contains(groupHeader), "Expected groupHeader parameter to be non-recyclable.");

            groupHeader.PropertyName = null;
            groupHeader.PropertyValue = null;
            groupHeader.IsRecycled = true;
            _recyclableGroupHeaders.Push(groupHeader);
        }

        internal void ClearElements(bool recycle)
        {
            ResetSlotIndexes();
            if (recycle)
            {
                foreach (UIElement element in _scrollingElements)
                {
                    DataGridRow row = element as DataGridRow;
                    if (row != null)
                    {
                        if (row.IsRecyclable)
                        {
                            AddRecylableRow(row);
                        }
                        else
                        {
                            row.Clip = new RectangleGeometry();
                        }
                    }
                    else
                    {
                        DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                        if (groupHeader != null)
                        {
                            AddRecylableRowGroupHeader(groupHeader);
                        }
                    }
                }
            }
            else
            {
                _recyclableRows.Clear();
                _fullyRecycledRows.Clear();
                _recyclableGroupHeaders.Clear();
                _fullyRecycledGroupHeaders.Clear();
            }

            _scrollingElements.Clear();
        }

        internal void CorrectSlotsAfterDeletion(int slot, bool wasCollapsed)
        {
            if (wasCollapsed)
            {
                if (slot > this.FirstScrollingSlot)
                {
                    this.LastScrollingSlot--;
                }
            }
            else if (_owner.IsSlotVisible(slot))
            {
                UnloadScrollingElement(slot, true /*updateSlotInformation*/, true /*wasDeleted*/);
            }

            // This cannot be an else condition because if there are 2 rows left, and you delete the first one
            // then these indexes need to be updated as well
            if (slot < this.FirstScrollingSlot)
            {
                this.FirstScrollingSlot--;
                this.LastScrollingSlot--;
            }
        }

        internal void CorrectSlotsAfterInsertion(int slot, UIElement element, bool isCollapsed)
        {
            if (slot < this.FirstScrollingSlot)
            {
                // The row was inserted above our viewport, just update our indexes
                this.FirstScrollingSlot++;
                this.LastScrollingSlot++;
            }
            else if (isCollapsed && (slot <= this.LastScrollingSlot))
            {
                this.LastScrollingSlot++;
            }
            else if ((_owner.GetPreviousVisibleSlot(slot) <= this.LastScrollingSlot) || (this.LastScrollingSlot == -1))
            {
                DiagnosticsDebug.Assert(element != null, "Expected non-null element.");

                // The row was inserted in our viewport, add it as a scrolling row
                LoadScrollingSlot(slot, element, true /*updateSlotInformation*/);
            }
        }

        private int GetCircularListIndex(int slot, bool wrap)
        {
            int index = slot - this.FirstScrollingSlot - _headScrollingElements - _owner.GetCollapsedSlotCount(this.FirstScrollingSlot, slot);
            return wrap ? index % _scrollingElements.Count : index;
        }

        internal void FullyRecycleElements()
        {
            // Fully recycle Recycleable rows and transfer them to Recycled rows
            while (_recyclableRows.Count > 0)
            {
                DataGridRow row = _recyclableRows.Pop();
                DiagnosticsDebug.Assert(row != null, "Expected non-null row.");
                row.Visibility = Visibility.Collapsed;
                DiagnosticsDebug.Assert(!_fullyRecycledRows.Contains(row), "Expected row not in _fullyRecycledRows.");
                _fullyRecycledRows.Push(row);
            }

            // Fully recycle recyclable GroupHeaders and transfer them to Recycled GroupHeaders
            while (_recyclableGroupHeaders.Count > 0)
            {
                DataGridRowGroupHeader groupHeader = _recyclableGroupHeaders.Pop();
                DiagnosticsDebug.Assert(groupHeader != null, "Expected non-null groupHeader.");
                groupHeader.Visibility = Visibility.Collapsed;
                DiagnosticsDebug.Assert(!_fullyRecycledGroupHeaders.Contains(groupHeader), "Expected groupHeader not in _fullyRecycledGroupHeaders.");
                _fullyRecycledGroupHeaders.Push(groupHeader);
            }
        }

        internal UIElement GetDisplayedElement(int slot)
        {
            DiagnosticsDebug.Assert(slot >= this.FirstScrollingSlot, "Expected slot greater than or equal to FirstScrollingSlot.");
            DiagnosticsDebug.Assert(slot <= this.LastScrollingSlot, "Expected slot less than or equal to LastScrollingSlot.");

            return _scrollingElements[GetCircularListIndex(slot, true /*wrap*/)];
        }

        internal DataGridRow GetDisplayedRow(int rowIndex)
        {
            return GetDisplayedElement(_owner.SlotFromRowIndex(rowIndex)) as DataGridRow;
        }

        // Returns an enumeration of the displayed scrolling rows in order starting with the FirstDisplayedScrollingRow
        // Only DataGridRow instances are returned when onlyRows = true
        internal IEnumerable<UIElement> GetScrollingElements(bool onlyRows = false)
        {
            for (int i = 0; i < _scrollingElements.Count; i++)
            {
                UIElement element = _scrollingElements[(_headScrollingElements + i) % _scrollingElements.Count];
                if (!onlyRows || element is DataGridRow)
                {
                    // _scrollingRows is a circular list that wraps
                    yield return element;
                }
            }
        }

        internal DataGridRowGroupHeader GetUsedGroupHeader()
        {
            if (_recyclableGroupHeaders.Count > 0)
            {
                return _recyclableGroupHeaders.Pop();
            }
            else if (_fullyRecycledGroupHeaders.Count > 0)
            {
                // For fully recycled rows, we need to set the Visibility back to Visible
                DataGridRowGroupHeader groupHeader = _fullyRecycledGroupHeaders.Pop();
                groupHeader.Visibility = Visibility.Visible;
                return groupHeader;
            }

            return null;
        }

        internal DataGridRow GetUsedRow()
        {
            if (_recyclableRows.Count > 0)
            {
                return _recyclableRows.Pop();
            }
            else if (_fullyRecycledRows.Count > 0)
            {
                // For fully recycled rows, we need to set the Visibility back to Visible
                DataGridRow row = _fullyRecycledRows.Pop();
                row.Visibility = Visibility.Visible;
                return row;
            }

            return null;
        }

        // Tracks the row at index rowIndex as a scrolling row
        internal void LoadScrollingSlot(int slot, UIElement element, bool updateSlotInformation)
        {
            if (_scrollingElements.Count == 0)
            {
                SetScrollingSlots(slot);
                _scrollingElements.Add(element);
            }
            else
            {
                // The slot should be adjacent to the other slots being displayed
                DiagnosticsDebug.Assert(slot >= _owner.GetPreviousVisibleSlot(this.FirstScrollingSlot), "Expected slot greater than or equal to _owner.GetPreviousVisibleSlot(this.FirstScrollingSlot).");
                DiagnosticsDebug.Assert(slot <= _owner.GetNextVisibleSlot(this.LastScrollingSlot), "Expected slot smaller than or equal to _owner.GetNextVisibleSlot(this.LastScrollingSlot).");
                if (updateSlotInformation)
                {
                    if (slot < this.FirstScrollingSlot)
                    {
                        this.FirstScrollingSlot = slot;
                    }
                    else
                    {
                        this.LastScrollingSlot = _owner.GetNextVisibleSlot(this.LastScrollingSlot);
                    }
                }

                int insertIndex = GetCircularListIndex(slot, false /*wrap*/);
                if (insertIndex > _scrollingElements.Count)
                {
                    // We need to wrap around from the bottom to the top of our circular list; as a result the head of the list moves forward
                    insertIndex -= _scrollingElements.Count;
                    _headScrollingElements++;
                }

                _scrollingElements.Insert(insertIndex, element);
            }
        }

        private void ResetSlotIndexes()
        {
            SetScrollingSlots(-1);
            this.NumTotallyDisplayedScrollingElements = 0;
            _headScrollingElements = 0;
        }

        private void SetScrollingSlots(int newValue)
        {
            this.FirstScrollingSlot = newValue;
            this.LastScrollingSlot = newValue;
        }

        // Stops tracking the element at the given slot as a scrolling element
        internal void UnloadScrollingElement(int slot, bool updateSlotInformation, bool wasDeleted)
        {
            DiagnosticsDebug.Assert(_owner.IsSlotVisible(slot), "Expected slot is visible.");

            int elementIndex = GetCircularListIndex(slot, false /*wrap*/);
            if (elementIndex > _scrollingElements.Count)
            {
                // We need to wrap around from the top to the bottom of our circular list
                elementIndex -= _scrollingElements.Count;
                _headScrollingElements--;
            }

            _scrollingElements.RemoveAt(elementIndex);

            if (updateSlotInformation)
            {
                if (slot == this.FirstScrollingSlot && !wasDeleted)
                {
                    this.FirstScrollingSlot = _owner.GetNextVisibleSlot(this.FirstScrollingSlot);
                }
                else
                {
                    this.LastScrollingSlot = _owner.GetPreviousVisibleSlot(this.LastScrollingSlot);
                }

                if (this.LastScrollingSlot < this.FirstScrollingSlot)
                {
                    ResetSlotIndexes();
                }
            }
        }

#if DEBUG
        internal void PrintDisplay()
        {
            foreach (UIElement element in this.GetScrollingElements())
            {
                DataGridRow row = element as DataGridRow;
                if (row != null)
                {
                    Debug.WriteLine(string.Format(global::System.Globalization.CultureInfo.InvariantCulture, "Slot: {0} Row: {1} ", row.Slot, row.Index));
                }
                else
                {
                    DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                    if (groupHeader != null)
                    {
#if FEATURE_ICOLLECTIONVIEW_GROUP
                        Debug.WriteLine(string.Format(
                            global::System.Globalization.CultureInfo.InvariantCulture,
                            "Slot: {0} GroupHeader: {1}",
                            groupHeader.RowGroupInfo.Slot,
                            groupHeader.RowGroupInfo.CollectionViewGroup.Name));
#else
                        Debug.WriteLine(string.Format(
                            global::System.Globalization.CultureInfo.InvariantCulture,
                            "Slot: {0} GroupHeader: {1}",
                            groupHeader.RowGroupInfo.Slot,
                            groupHeader.RowGroupInfo.ToString()));
#endif
                    }
                }
            }
        }
#endif
    }
}