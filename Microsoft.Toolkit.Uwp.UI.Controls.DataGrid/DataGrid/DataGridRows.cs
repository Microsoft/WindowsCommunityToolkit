// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Toolkit.Uwp.UI.Automation.Peers;
using Microsoft.Toolkit.Uwp.UI.Controls.DataGridInternals;
using Microsoft.Toolkit.Uwp.UI.Utilities;
using Microsoft.Toolkit.Uwp.Utilities;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

using DiagnosticsDebug = System.Diagnostics.Debug;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Control to represent data in columns and rows.
    /// </summary>
    public partial class DataGrid
    {
        internal bool AreRowBottomGridLinesRequired
        {
            get
            {
                return (this.GridLinesVisibility == DataGridGridLinesVisibility.Horizontal || this.GridLinesVisibility == DataGridGridLinesVisibility.All) && this.HorizontalGridLinesBrush != null;
            }
        }

        internal int FirstVisibleSlot
        {
            get
            {
                return (this.SlotCount > 0) ? GetNextVisibleSlot(-1) : -1;
            }
        }

        internal int FrozenColumnCountWithFiller
        {
            get
            {
                int count = this.FrozenColumnCount;
                if (this.ColumnsInternal.RowGroupSpacerColumn.IsRepresented && (this.AreRowGroupHeadersFrozen || count > 0))
                {
                    // Either the RowGroupHeaders are frozen by default or the user set a frozen column count.  In both cases, we need to freeze
                    // one more column than the what the public value says
                    count++;
                }

                return count;
            }
        }

        internal int LastVisibleSlot
        {
            get
            {
                return (this.SlotCount > 0) ? this.GetPreviousVisibleSlot(this.SlotCount) : -1;
            }
        }

        // Cumulated height of all known rows, including the gridlines and details section.
        // This property returns an approximation of the actual total row heights and also
        // updates the RowHeightEstimate
        private double EdgedRowsHeightCalculated
        {
            get
            {
                // If we're not displaying any rows or if we have infinite space the, relative height of our rows is 0
                if (this.DisplayData.LastScrollingSlot == -1 || double.IsPositiveInfinity(this.AvailableSlotElementRoom))
                {
                    if (_oldEdgedRowsHeightCalculated > 0)
                    {
                        _oldEdgedRowsHeightCalculated = 0;

                        LoadMoreDataFromIncrementalItemsSource(0);
                    }

                    return 0;
                }

                DiagnosticsDebug.Assert(this.DisplayData.LastScrollingSlot >= 0, "Expected positive DisplayData.LastScrollingSlot.");
                DiagnosticsDebug.Assert(_verticalOffset >= 0, "Expected positive _verticalOffset.");
                DiagnosticsDebug.Assert(this.NegVerticalOffset >= 0, "Expected positive NegVerticalOffset.");

                // Height of all rows above the viewport
                double totalRowsHeight = _verticalOffset - this.NegVerticalOffset;

                // Add the height of all the rows currently displayed, AvailableRowRoom
                // is not always up to date enough for this
                foreach (UIElement element in this.DisplayData.GetScrollingElements())
                {
                    DataGridRow row = element as DataGridRow;
                    if (row != null)
                    {
                        totalRowsHeight += row.TargetHeight;
                    }
                    else
                    {
                        totalRowsHeight += element.EnsureMeasured().DesiredSize.Height;
                    }
                }

                // Details up to and including viewport
                int detailsCount = GetDetailsCountInclusive(0, this.DisplayData.LastScrollingSlot);

                // Subtract details that were accounted for from the totalRowsHeight
                totalRowsHeight -= detailsCount * this.RowDetailsHeightEstimate;

                // Update the RowHeightEstimate if we have more row information
                if (this.DisplayData.LastScrollingSlot >= _lastEstimatedRow)
                {
                    _lastEstimatedRow = this.DisplayData.LastScrollingSlot;
                    this.RowHeightEstimate = totalRowsHeight / (_lastEstimatedRow + 1 - _collapsedSlotsTable.GetIndexCount(0, _lastEstimatedRow));
                }

                // Calculate estimates for what's beyond the viewport
                if (this.VisibleSlotCount > this.DisplayData.NumDisplayedScrollingElements)
                {
                    int remainingRowCount = this.SlotCount - this.DisplayData.LastScrollingSlot - _collapsedSlotsTable.GetIndexCount(this.DisplayData.LastScrollingSlot, this.SlotCount - 1) - 1;

                    // Add estimation for the cell heights of all rows beyond our viewport
                    totalRowsHeight += this.RowHeightEstimate * remainingRowCount;

                    // Add the rest of the details beyond the viewport
                    detailsCount += GetDetailsCountInclusive(this.DisplayData.LastScrollingSlot + 1, this.SlotCount - 1);
                }

                // TODO: Update the DetailsHeightEstimate
                double totalDetailsHeight = detailsCount * this.RowDetailsHeightEstimate;
                double newEdgedRowsHeightCalculated = totalRowsHeight + totalDetailsHeight;
                bool loadMoreDataFromIncrementalItemsSource = newEdgedRowsHeightCalculated < _oldEdgedRowsHeightCalculated;

                _oldEdgedRowsHeightCalculated = newEdgedRowsHeightCalculated;

                if (loadMoreDataFromIncrementalItemsSource)
                {
                    LoadMoreDataFromIncrementalItemsSource(newEdgedRowsHeightCalculated);
                }

                return newEdgedRowsHeightCalculated;
            }
        }

        /// <summary>
        /// Collapses the DataGridRowGroupHeader that represents a given CollectionViewGroup
        /// </summary>
        /// <param name="collectionViewGroup">CollectionViewGroup</param>
        /// <param name="collapseAllSubgroups">Set to true to collapse all Subgroups</param>
        public void CollapseRowGroup(ICollectionViewGroup collectionViewGroup, bool collapseAllSubgroups)
        {
            if (this.WaitForLostFocus(() => { this.CollapseRowGroup(collectionViewGroup, collapseAllSubgroups); }) ||
                collectionViewGroup == null || !this.CommitEdit())
            {
                return;
            }

            EnsureRowGroupVisibility(RowGroupInfoFromCollectionViewGroup(collectionViewGroup), Visibility.Collapsed, true);

            if (collapseAllSubgroups)
            {
                foreach (object groupObj in collectionViewGroup.GroupItems)
                {
                    ICollectionViewGroup subGroup = groupObj as ICollectionViewGroup;
                    if (subGroup != null)
                    {
                        CollapseRowGroup(subGroup, collapseAllSubgroups);
                    }
                }
            }
        }

        /// <summary>
        /// Expands the DataGridRowGroupHeader that represents a given CollectionViewGroup
        /// </summary>
        /// <param name="collectionViewGroup">CollectionViewGroup</param>
        /// <param name="expandAllSubgroups">Set to true to expand all Subgroups</param>
        public void ExpandRowGroup(ICollectionViewGroup collectionViewGroup, bool expandAllSubgroups)
        {
            if (this.WaitForLostFocus(() => { this.ExpandRowGroup(collectionViewGroup, expandAllSubgroups); }) ||
                collectionViewGroup == null || !this.CommitEdit())
            {
                if (collectionViewGroup == null || !this.CommitEdit())
                {
                    return;
                }
            }

            EnsureRowGroupVisibility(RowGroupInfoFromCollectionViewGroup(collectionViewGroup), Visibility.Visible, true);

            if (expandAllSubgroups)
            {
                foreach (object groupObj in collectionViewGroup.GroupItems)
                {
                    ICollectionViewGroup subGroup = groupObj as ICollectionViewGroup;
                    if (subGroup != null)
                    {
                        ExpandRowGroup(subGroup, expandAllSubgroups);
                    }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.DataGrid.RowDetailsVisibilityChanged" /> event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected internal virtual void OnRowDetailsVisibilityChanged(DataGridRowDetailsEventArgs e)
        {
            this.RowDetailsVisibilityChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Clears the entire selection. Displayed rows are deselected explicitly to visualize
        /// potential transition effects
        /// </summary>
        internal void ClearRowSelection(bool resetAnchorSlot)
        {
            if (resetAnchorSlot)
            {
                this.AnchorSlot = -1;
            }

            if (_selectedItems.Count > 0)
            {
                _noSelectionChangeCount++;
                try
                {
                    // Individually deselecting displayed rows to view potential transitions
                    for (int slot = this.DisplayData.FirstScrollingSlot;
                         slot > -1 && slot <= this.DisplayData.LastScrollingSlot;
                         slot++)
                    {
                        DataGridRow row = this.DisplayData.GetDisplayedElement(slot) as DataGridRow;
                        if (row != null)
                        {
                            if (_selectedItems.ContainsSlot(row.Slot))
                            {
                                SelectSlot(row.Slot, false);
                            }
                        }
                    }

                    _selectedItems.ClearRows();
                    this.SelectionHasChanged = true;
                }
                finally
                {
                    this.NoSelectionChangeCount--;
                }
            }
        }

        /// <summary>
        /// Clears the entire selection except the indicated row. Displayed rows are deselected explicitly to
        /// visualize potential transition effects. The row indicated is selected if it is not already.
        /// </summary>
        internal void ClearRowSelection(int slotException, bool setAnchorSlot)
        {
            _noSelectionChangeCount++;
            try
            {
                bool exceptionAlreadySelected = false;
                if (_selectedItems.Count > 0)
                {
                    // Individually deselecting displayed rows to view potential transitions
                    for (int slot = this.DisplayData.FirstScrollingSlot;
                         slot > -1 && slot <= this.DisplayData.LastScrollingSlot;
                         slot++)
                    {
                        if (slot != slotException && _selectedItems.ContainsSlot(slot))
                        {
                            SelectSlot(slot, false);
                            this.SelectionHasChanged = true;
                        }
                    }

                    exceptionAlreadySelected = _selectedItems.ContainsSlot(slotException);
                    int selectedCount = _selectedItems.Count;
                    if (selectedCount > 0)
                    {
                        if (selectedCount > 1)
                        {
                            this.SelectionHasChanged = true;
                        }
                        else
                        {
                            int currentlySelectedSlot = _selectedItems.GetIndexes().First();
                            if (currentlySelectedSlot != slotException)
                            {
                                this.SelectionHasChanged = true;
                            }
                        }

                        _selectedItems.ClearRows();
                    }
                }

                if (exceptionAlreadySelected)
                {
                    // Exception row was already selected. It just needs to be marked as selected again.
                    // No transition involved.
                    _selectedItems.SelectSlot(slotException, true /*select*/);
                    if (setAnchorSlot)
                    {
                        this.AnchorSlot = slotException;
                    }
                }
                else
                {
                    // Exception row was not selected. It needs to be selected with potential transition
                    SetRowSelection(slotException, true /*isSelected*/, setAnchorSlot);
                }
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
        }

        internal int GetCollapsedSlotCount(int startSlot, int endSlot)
        {
            return _collapsedSlotsTable.GetIndexCount(startSlot, endSlot);
        }

        internal int GetNextVisibleSlot(int slot)
        {
            return _collapsedSlotsTable.GetNextGap(slot);
        }

        internal int GetPreviousVisibleSlot(int slot)
        {
            return _collapsedSlotsTable.GetPreviousGap(slot);
        }

        internal Visibility GetRowDetailsVisibility(int rowIndex)
        {
            return GetRowDetailsVisibility(rowIndex, this.RowDetailsVisibilityMode);
        }

        internal Visibility GetRowDetailsVisibility(int rowIndex, DataGridRowDetailsVisibilityMode gridLevelRowDetailsVisibility)
        {
            DiagnosticsDebug.Assert(rowIndex != -1, "Expected rowIndex other than -1.");
            if (_showDetailsTable.Contains(rowIndex))
            {
                // The user explicitly set DetailsVisibility on a row so we should respect that
                return _showDetailsTable.GetValueAt(rowIndex);
            }
            else
            {
                if (gridLevelRowDetailsVisibility == DataGridRowDetailsVisibilityMode.Visible ||
                    (gridLevelRowDetailsVisibility == DataGridRowDetailsVisibilityMode.VisibleWhenSelected &&
                     _selectedItems.ContainsSlot(SlotFromRowIndex(rowIndex))))
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Returns the row associated to the provided backend data item.
        /// </summary>
        /// <param name="dataItem">backend data item</param>
        /// <returns>null if the DataSource is null, the provided item in not in the source, or the item is not displayed; otherwise, the associated Row</returns>
        internal DataGridRow GetRowFromItem(object dataItem)
        {
            int rowIndex = this.DataConnection.IndexOf(dataItem);
            if (rowIndex < 0)
            {
                return null;
            }

            int slot = SlotFromRowIndex(rowIndex);
            return IsSlotVisible(slot) ? this.DisplayData.GetDisplayedElement(slot) as DataGridRow : null;
        }

        internal bool GetRowSelection(int slot)
        {
            DiagnosticsDebug.Assert(slot != -1, "Expected slot other than -1.");
            return _selectedItems.ContainsSlot(slot);
        }

        internal void InsertElementAt(
            int slot,
            int rowIndex,
            object item,
            DataGridRowGroupInfo groupInfo,
            bool isCollapsed)
        {
            DiagnosticsDebug.Assert(slot >= 0, "Expected positive slot.");
            DiagnosticsDebug.Assert(slot <= this.SlotCount, "Expected slot smaller than or equal to SlotCount.");

            bool isRow = rowIndex != -1;
            if (isCollapsed || (this.IsReadOnly && rowIndex == this.DataConnection.NewItemPlaceholderIndex))
            {
                InsertElement(slot, null /*element*/, true /*updateVerticalScrollBarOnly*/, true /*isCollapsed*/, isRow);
            }
            else if (SlotIsDisplayed(slot))
            {
                // Row at that index needs to be displayed
                if (isRow)
                {
                    InsertElement(slot, GenerateRow(rowIndex, slot, item), false /*updateVerticalScrollBarOnly*/, false /*isCollapsed*/, isRow);
                }
                else
                {
                    InsertElement(slot, GenerateRowGroupHeader(slot, groupInfo), false /*updateVerticalScrollBarOnly*/, false /*isCollapsed*/, isRow);
                }
            }
            else
            {
                InsertElement(slot, null, _vScrollBar == null || _vScrollBar.Visibility == Visibility.Visible /*updateVerticalScrollBarOnly*/, false /*isCollapsed*/, isRow);
            }
        }

        internal void InsertRowAt(int rowIndex)
        {
            int slot = SlotFromRowIndex(rowIndex);
            object item = this.DataConnection.GetDataItem(rowIndex);

            // isCollapsed below is always false because we only use the method if we're not grouping
            InsertElementAt(
                slot,
                rowIndex,
                item,
                null /*DataGridRowGroupInfo*/,
                false /*isCollapsed*/);
        }

        internal bool IsColumnDisplayed(int columnIndex)
        {
            return columnIndex >= this.FirstDisplayedNonFillerColumnIndex && columnIndex <= this.DisplayData.LastTotallyDisplayedScrollingCol;
        }

        internal bool IsRowRecyclable(DataGridRow row)
        {
            return row != this.EditingRow && row != _focusedRow;
        }

        internal bool IsSlotVisible(int slot)
        {
            return slot >= this.DisplayData.FirstScrollingSlot &&
                slot <= this.DisplayData.LastScrollingSlot &&
                slot != -1 &&
                !_collapsedSlotsTable.Contains(slot);
        }

        // detailsElement is the FrameworkElement created by the DetailsTemplate
        internal void OnUnloadingRowDetails(DataGridRow row, FrameworkElement detailsElement)
        {
            OnUnloadingRowDetails(new DataGridRowDetailsEventArgs(row, detailsElement));
        }

        // detailsElement is the FrameworkElement created by the DetailsTemplate
        internal void OnLoadingRowDetails(DataGridRow row, FrameworkElement detailsElement)
        {
            OnLoadingRowDetails(new DataGridRowDetailsEventArgs(row, detailsElement));
        }

        internal void OnRowDetailsVisibilityPropertyChanged(int rowIndex, Visibility visibility)
        {
            DiagnosticsDebug.Assert(rowIndex >= 0, "Expected positive rowIndex.");
            DiagnosticsDebug.Assert(rowIndex < this.SlotCount, "Expected rowIndex smaller than SlotCount.");

            _showDetailsTable.AddValue(rowIndex, visibility);
        }

        internal void OnRowGroupHeaderToggled(DataGridRowGroupHeader groupHeader, Visibility newVisibility, bool setCurrent)
        {
            DiagnosticsDebug.Assert(groupHeader.RowGroupInfo.CollectionViewGroup.GroupItems.Count > 0, "Expected positive groupHeader.RowGroupInfo.CollectionViewGroup.GroupItems.Count.");

            if (this.WaitForLostFocus(() => { this.OnRowGroupHeaderToggled(groupHeader, newVisibility, setCurrent); }) || !this.CommitEdit())
            {
                return;
            }

            if (setCurrent && this.CurrentSlot != groupHeader.RowGroupInfo.Slot)
            {
                // Most of the time this is set by the MouseLeftButtonDown handler but validation could cause that code path to fail
                UpdateSelectionAndCurrency(this.CurrentColumnIndex, groupHeader.RowGroupInfo.Slot, DataGridSelectionAction.SelectCurrent, false /*scrollIntoView*/);
            }

            UpdateRowGroupVisibility(groupHeader.RowGroupInfo, newVisibility, true /*isHeaderDisplayed*/);

            ComputeScrollBarsLayout();

            // We need force arrange since our Scrollings Rows could update without automatically triggering layout
            InvalidateRowsArrange();
        }

        internal void OnRowsMeasure()
        {
            if (!DoubleUtil.IsZero(this.DisplayData.PendingVerticalScrollHeight))
            {
                ScrollSlotsByHeight(this.DisplayData.PendingVerticalScrollHeight);
                this.DisplayData.PendingVerticalScrollHeight = 0;
            }
        }

        internal void OnSublevelIndentUpdated(DataGridRowGroupHeader groupHeader, double newValue)
        {
            DiagnosticsDebug.Assert(this.DataConnection.CollectionView != null, "Expected non-null DataConnection.CollectionView.");
            DiagnosticsDebug.Assert(this.DataConnection.CollectionView.CollectionGroups != null, "Expected non-null DataConnection.CollectionView.CollectionGroups.");
            DiagnosticsDebug.Assert(this.RowGroupSublevelIndents != null, "Expected non-null RowGroupSublevelIndents.");

#if FEATURE_ICOLLECTIONVIEW_GROUP
            int groupLevelCount = this.DataConnection.CollectionView.GroupDescriptions.Count;
#else
            int groupLevelCount = 1;
#endif
            DiagnosticsDebug.Assert(groupHeader.Level >= 0, "Expected positive groupHeader.Level.");
            DiagnosticsDebug.Assert(groupHeader.Level < groupLevelCount, "Expected groupHeader.Level smaller than groupLevelCount.");

            double oldValue = this.RowGroupSublevelIndents[groupHeader.Level];
            if (groupHeader.Level > 0)
            {
                oldValue -= this.RowGroupSublevelIndents[groupHeader.Level - 1];
            }

            // Update the affected values in our table by the amount affected
            double change = newValue - oldValue;
            for (int i = groupHeader.Level; i < groupLevelCount; i++)
            {
                this.RowGroupSublevelIndents[i] += change;
                DiagnosticsDebug.Assert(this.RowGroupSublevelIndents[i] >= 0, "Expected positive RowGroupSublevelIndents[i].");
            }

            EnsureRowGroupSpacerColumnWidth(groupLevelCount);
        }

        internal void RefreshRows(bool recycleRows, bool clearRows)
        {
            if (_measured)
            {
                // _desiredCurrentColumnIndex is used in MakeFirstDisplayedCellCurrentCell to set the
                // column position back to what it was before the refresh
                _desiredCurrentColumnIndex = this.CurrentColumnIndex;
                double verticalOffset = _verticalOffset;
                if (this.DisplayData.PendingVerticalScrollHeight > 0)
                {
                    // Use the pending vertical scrollbar position if there is one, in the case that the collection
                    // has been reset multiple times in a row.
                    verticalOffset = this.DisplayData.PendingVerticalScrollHeight;
                }

                VerticalOffset = 0;
                this.NegVerticalOffset = 0;

                if (clearRows)
                {
                    ClearRows(recycleRows);
                    ClearRowGroupHeadersTable();
                    PopulateRowGroupHeadersTable();
                    RefreshSlotCounts();
                }

                RefreshRowGroupHeaders();

                // Update the CurrentSlot because it might have changed
                if (recycleRows && this.DataConnection.CollectionView != null)
                {
                    this.CurrentSlot = this.DataConnection.CollectionView.CurrentPosition == -1
                        ? -1 : SlotFromRowIndex(this.DataConnection.CollectionView.CurrentPosition);
                    if (this.CurrentSlot == -1)
                    {
                        SetCurrentCellCore(-1, -1);
                    }
                }

                if (this.DataConnection != null && this.ColumnsItemsInternal.Count > 0)
                {
                    int slotCount = this.DataConnection.Count;
                    slotCount += this.RowGroupHeadersTable.IndexCount;
                    AddSlots(slotCount);
                    InvalidateMeasure();
                }

                EnsureRowGroupSpacerColumn();

                if (this.VerticalScrollBar != null)
                {
                    this.DisplayData.PendingVerticalScrollHeight = Math.Min(verticalOffset, this.VerticalScrollBar.Maximum);
                }
            }
            else
            {
                if (clearRows)
                {
                    ClearRows(recycleRows /*recycle*/);
                }

                ClearRowGroupHeadersTable();
                PopulateRowGroupHeadersTable();
                RefreshSlotCounts();
            }
        }

        internal void RemoveRowAt(int rowIndex, object item)
        {
            RemoveElementAt(SlotFromRowIndex(rowIndex), item, true);
        }

        internal DataGridRowGroupInfo RowGroupInfoFromCollectionViewGroup(ICollectionViewGroup collectionViewGroup)
        {
            foreach (int slot in this.RowGroupHeadersTable.GetIndexes())
            {
                DataGridRowGroupInfo rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(slot);
                if (rowGroupInfo.CollectionViewGroup == collectionViewGroup)
                {
                    return rowGroupInfo;
                }
            }

            return null;
        }

        internal int RowIndexFromSlot(int slot)
        {
            return slot - this.RowGroupHeadersTable.GetIndexCount(0, slot);
        }

        internal bool ScrollSlotIntoView(int slot, bool scrolledHorizontally)
        {
            DiagnosticsDebug.Assert(_collapsedSlotsTable.Contains(slot) || !IsSlotOutOfBounds(slot), "Expected _collapsedSlotsTable.Contains(slot) is true or IsSlotOutOfBounds(slot) is false.");

            if (scrolledHorizontally && this.DisplayData.FirstScrollingSlot <= slot && this.DisplayData.LastScrollingSlot >= slot)
            {
                // If the slot is displayed and we scrolled horizontally, column virtualization could cause the rows to grow.
                // As a result we need to force measure on the rows we're displaying and recalculate our First and Last slots
                // so they're accurate
                foreach (DataGridRow row in this.DisplayData.GetScrollingElements(true /*onlyRows*/))
                {
                    row.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                }

                UpdateDisplayedRows(this.DisplayData.FirstScrollingSlot, this.CellsHeight);
            }

            if (this.DisplayData.FirstScrollingSlot < slot && this.DisplayData.LastScrollingSlot > slot)
            {
                // The row is already displayed in its entirety
                return true;
            }
            else if (this.DisplayData.FirstScrollingSlot == slot && slot != -1)
            {
                if (!DoubleUtil.IsZero(this.NegVerticalOffset))
                {
                    // First displayed row is partially scrolled of. Let's scroll it so that this.NegVerticalOffset becomes 0.
                    this.DisplayData.PendingVerticalScrollHeight = -this.NegVerticalOffset;
                    InvalidateRowsMeasure(false /*invalidateIndividualRows*/);
                }

                return true;
            }

            double deltaY = 0;
            int firstFullSlot;
            if (this.DisplayData.FirstScrollingSlot > slot)
            {
                // Scroll up to the new row so it becomes the first displayed row
                firstFullSlot = this.DisplayData.FirstScrollingSlot - 1;
                if (DoubleUtil.GreaterThan(this.NegVerticalOffset, 0))
                {
                    deltaY = -this.NegVerticalOffset;
                }

                deltaY -= GetSlotElementsHeight(slot, firstFullSlot);
                if (this.DisplayData.FirstScrollingSlot - slot > 1)
                {
                    // TODO: This will likely discard and create a small number of the same rows so we could probably
                    // optimize this.  The optimization would only affect the PageUp key.
                    ResetDisplayedRows();
                }

                this.NegVerticalOffset = 0;
                UpdateDisplayedRows(slot, this.CellsHeight);
            }
            else if (this.DisplayData.LastScrollingSlot <= slot)
            {
                // Scroll down to the new row so it's entirely displayed.  If the height of the row
                // is greater than the height of the DataGrid, then show the top of the row at the top
                // of the grid.
                firstFullSlot = this.DisplayData.LastScrollingSlot;

                // Figure out how much of the last row is cut off.
                double rowHeight = GetExactSlotElementHeight(this.DisplayData.LastScrollingSlot);
                double availableHeight = this.AvailableSlotElementRoom + rowHeight;
                if (DoubleUtil.AreClose(rowHeight, availableHeight))
                {
                    if (this.DisplayData.LastScrollingSlot == slot)
                    {
                        // We're already at the very bottom so we don't need to scroll down further.
                        return true;
                    }
                    else
                    {
                        // We're already showing the entire last row so don't count it as part of the delta.
                        firstFullSlot++;
                    }
                }
                else if (rowHeight > availableHeight)
                {
                    firstFullSlot++;
                    deltaY += rowHeight - availableHeight;
                }

                // sum up the height of the rest of the full rows.
                if (slot >= firstFullSlot)
                {
                    deltaY += GetSlotElementsHeight(firstFullSlot, slot);
                }

                // If the first row we're displaying is no longer adjacent to the rows we have
                // simply discard the ones we have.
                if (slot - this.DisplayData.LastScrollingSlot > 1)
                {
                    ResetDisplayedRows();
                }

                if (DoubleUtil.GreaterThanOrClose(GetExactSlotElementHeight(slot), this.CellsHeight))
                {
                    // The entire row won't fit in the DataGrid so we start showing it from the top.
                    this.NegVerticalOffset = 0;
                    UpdateDisplayedRows(slot, this.CellsHeight);
                }
                else
                {
                    UpdateDisplayedRowsFromBottom(slot);
                }
            }

            VerticalOffset += deltaY;
            if (_verticalOffset < 0 || this.DisplayData.FirstScrollingSlot == 0)
            {
                // We scrolled too far because a row's height was larger than its approximation.
                VerticalOffset = this.NegVerticalOffset;
            }

            // TODO: in certain cases (eg, variable row height), this may not be true
            DiagnosticsDebug.Assert(DoubleUtil.LessThanOrClose(this.NegVerticalOffset, _verticalOffset), "Expected NegVerticalOffset is less than or close to _verticalOffset.");

            SetVerticalOffset(_verticalOffset);

            InvalidateMeasure();
            InvalidateRowsMeasure(false /*invalidateIndividualRows*/);

            return true;
        }

        internal void SetRowSelection(int slot, bool isSelected, bool setAnchorSlot)
        {
            DiagnosticsDebug.Assert(isSelected || !setAnchorSlot, "Expected isSelected is true or setAnchorSlot is false.");
            DiagnosticsDebug.Assert(!IsSlotOutOfSelectionBounds(slot), "Expected IsSlotOutOfSelectionBounds(slot) is false.");
            _noSelectionChangeCount++;
            try
            {
                if (this.SelectionMode == DataGridSelectionMode.Single && isSelected)
                {
                    DiagnosticsDebug.Assert(_selectedItems.Count <= 1, "Expected _selectedItems.Count smaller than or equal to 1.");
                    if (_selectedItems.Count > 0)
                    {
                        int currentlySelectedSlot = _selectedItems.GetIndexes().First();
                        if (currentlySelectedSlot != slot)
                        {
                            SelectSlot(currentlySelectedSlot, false);
                            this.SelectionHasChanged = true;
                        }
                    }
                }

                if (_selectedItems.ContainsSlot(slot) != isSelected)
                {
                    SelectSlot(slot, isSelected);
                    this.SelectionHasChanged = true;
                }

                if (setAnchorSlot)
                {
                    this.AnchorSlot = slot;
                }
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
        }

        // For now, all scenarios are for isSelected == true.
        internal void SetRowsSelection(int startSlot, int endSlot, bool isSelected = true)
        {
            DiagnosticsDebug.Assert(startSlot >= 0, "Expected startSlot is positive.");
            DiagnosticsDebug.Assert(startSlot < this.SlotCount, "Expected startSlot is smaller than SlotCount.");
            DiagnosticsDebug.Assert(endSlot >= 0, "Expected endSlot is positive.");
            DiagnosticsDebug.Assert(endSlot < this.SlotCount, "Expected endSlot is smaller than SlotCount.");
            DiagnosticsDebug.Assert(startSlot <= endSlot, "Expected startSlot is smaller than or equal to endSlot.");

            _noSelectionChangeCount++;
            try
            {
                if (isSelected && !_selectedItems.ContainsAll(startSlot, endSlot))
                {
                    // At least one row gets selected
                    SelectSlots(startSlot, endSlot, true);
                    this.SelectionHasChanged = true;
                }
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
        }

        internal int SlotFromRowIndex(int rowIndex)
        {
            return rowIndex + this.RowGroupHeadersTable.GetIndexCountBeforeGap(0, rowIndex);
        }

        private static void CorrectRowAfterDeletion(DataGridRow row, bool rowDeleted)
        {
            row.Slot--;
            if (rowDeleted)
            {
                row.Index--;
            }
        }

        private static void CorrectRowAfterInsertion(DataGridRow row, bool rowInserted)
        {
            row.Slot++;
            if (rowInserted)
            {
                row.Index++;
            }
        }

        private void AddSlotElement(int slot, UIElement element)
        {
#if DEBUG
            DataGridRow row = element as DataGridRow;
            if (row != null)
            {
                DiagnosticsDebug.Assert(row.OwningGrid == this, "Expected row.OwningGrid equals this DataGrid.");
                DiagnosticsDebug.Assert(row.Cells.Count == this.ColumnsItemsInternal.Count, "Expected row.Cells.Count equals this.ColumnsItemsInternal.Count.");

                int columnIndex = 0;
                foreach (DataGridCell dataGridCell in row.Cells)
                {
                    DiagnosticsDebug.Assert(dataGridCell.OwningRow == row, "Expected dataGridCell.OwningRow equals row.");
                    DiagnosticsDebug.Assert(dataGridCell.OwningColumn == this.ColumnsItemsInternal[columnIndex], "Expected dataGridCell.OwningColumn equals this.ColumnsItemsInternal[columnIndex].");
                    columnIndex++;
                }
            }
#endif
            DiagnosticsDebug.Assert(slot == this.SlotCount, "Expected slot equals this.SlotCount.");

            OnAddedElement_Phase1(slot, element);
            this.SlotCount++;
            this.VisibleSlotCount++;
            OnAddedElement_Phase2(slot, false /*updateVerticalScrollBarOnly*/);
            OnElementsChanged(true /*grew*/);
        }

        private void AddSlots(int totalSlots)
        {
            this.SlotCount = 0;
            this.VisibleSlotCount = 0;
            IEnumerator<int> groupSlots = null;
            int nextGroupSlot = -1;
            if (this.RowGroupHeadersTable.RangeCount > 0)
            {
                groupSlots = this.RowGroupHeadersTable.GetIndexes().GetEnumerator();
                if (groupSlots != null && groupSlots.MoveNext())
                {
                    nextGroupSlot = groupSlots.Current;
                }
            }

            int slot = 0;
            int addedRows = 0;
            while (slot < totalSlots && this.AvailableSlotElementRoom > 0)
            {
                if (slot == nextGroupSlot)
                {
                    DataGridRowGroupInfo groupRowInfo = this.RowGroupHeadersTable.GetValueAt(slot);
                    AddSlotElement(slot, GenerateRowGroupHeader(slot, groupRowInfo));
                    nextGroupSlot = groupSlots.MoveNext() ? groupSlots.Current : -1;
                }
                else
                {
                    AddSlotElement(slot, GenerateRow(addedRows, slot));
                    addedRows++;
                }

                slot++;
            }

            if (slot < totalSlots)
            {
                this.SlotCount += totalSlots - slot;
                this.VisibleSlotCount += totalSlots - slot;
                OnAddedElement_Phase2(0, _vScrollBar == null || _vScrollBar.Visibility == Visibility.Visible /*updateVerticalScrollBarOnly*/);
                OnElementsChanged(true /*grew*/);
            }
        }

        private void ApplyDisplayedRowsState(int startSlot, int endSlot)
        {
            int firstSlot = Math.Max(this.DisplayData.FirstScrollingSlot, startSlot);
            int lastSlot = Math.Min(this.DisplayData.LastScrollingSlot, endSlot);

            if (firstSlot >= 0)
            {
                DiagnosticsDebug.Assert(lastSlot >= firstSlot, "lastSlot greater than or equal to firstSlot.");
                int slot = GetNextVisibleSlot(firstSlot - 1);
                while (slot <= lastSlot)
                {
                    DataGridRow row = this.DisplayData.GetDisplayedElement(slot) as DataGridRow;
                    if (row != null)
                    {
                        row.ApplyState(true /*animate*/);
                    }

                    slot = GetNextVisibleSlot(slot);
                }
            }
        }

        private void ClearRowGroupHeadersTable()
        {
            // Detach existing handlers on CollectionViewGroup.Items.CollectionChanged
            foreach (int slot in this.RowGroupHeadersTable.GetIndexes())
            {
                DataGridRowGroupInfo groupInfo = this.RowGroupHeadersTable.GetValueAt(slot);
                if (groupInfo.CollectionViewGroup.GroupItems != null)
                {
#if FEATURE_ICOLLECTIONVIEW_GROUP
                    UnhookCollectionChangedListenerFromGroup(groupInfo.CollectionViewGroup.GroupItems as INotifyCollectionChanged, false /*removeFromTable*/);
#else
                    UnhookVectorChangedListenerFromGroup(groupInfo.CollectionViewGroup.GroupItems, false /*removeFromTable*/);
#endif
                }

#if FEATURE_ICOLLECTIONVIEW_GROUP
                WeakEventListener<DataGrid, object, PropertyChangedEventArgs> weakPropertyChangedListener;
                INotifyPropertyChanged inpc = groupInfo.CollectionViewGroup as INotifyPropertyChanged;
                if (inpc != null && _groupsPropertyChangedListenersTable.TryGetValue(inpc, out weakPropertyChangedListener))
                {
                    weakPropertyChangedListener.Detach();
                }
#endif
            }

            if (_topLevelGroup != null)
            {
#if FEATURE_ICOLLECTIONVIEW_GROUP
                UnhookCollectionChangedListenerFromGroup(_topLevelGroup as INotifyCollectionChanged, false /*removeFromTable*/);
#else
                UnhookVectorChangedListenerFromGroup(_topLevelGroup, false /*removeFromTable*/);
#endif
                _topLevelGroup = null;
            }

#if FEATURE_ICOLLECTIONVIEW_GROUP
            _groupsPropertyChangedListenersTable.Clear();
            _groupsCollectionChangedListenersTable.Clear();
#endif

            this.RowGroupHeadersTable.Clear();
            _collapsedSlotsTable.Clear();

            _rowGroupHeightsByLevel = null;
            RowGroupSublevelIndents = null;
        }

        private void ClearRows(bool recycle)
        {
            // Need to clean up recycled rows even if the RowCount is 0
            SetCurrentCellCore(-1, -1, false /*commitEdit*/, false /*endRowEdit*/);
            ClearRowSelection(true /*resetAnchorSlot*/);
            UnloadElements(recycle);

            this.ClearShowDetailsTable();
            this.SlotCount = 0;
            this.NegVerticalOffset = 0;
            SetVerticalOffset(0);
            ComputeScrollBarsLayout();
        }

        private void ClearShowDetailsTable()
        {
            _showDetailsTable.Clear();
#if FEATURE_IEDITABLECOLLECTIONVIEW
            if (this.DataConnection.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd)
            {
                _showDetailsTable.AddValue(this.DataConnection.NewItemPlaceholderIndex, Visibility.Collapsed);
            }
#endif
        }

        // Updates _collapsedSlotsTable and returns the number of pixels that were collapsed
        private double CollapseSlotsInTable(int startSlot, int endSlot, ref int slotsExpanded, int lastDisplayedSlot, ref double heightChangeBelowLastDisplayedSlot)
        {
            int firstSlot = startSlot;
            int lastSlot;
            double totalHeightChange = 0;

            // Figure out which slots actually need to be expanded since some might already be collapsed
            while (firstSlot <= endSlot)
            {
                firstSlot = _collapsedSlotsTable.GetNextGap(firstSlot - 1);
                int nextCollapsedSlot = _collapsedSlotsTable.GetNextIndex(firstSlot) - 1;
                lastSlot = nextCollapsedSlot == -2 ? endSlot : Math.Min(endSlot, nextCollapsedSlot);

                if (firstSlot <= lastSlot)
                {
                    double heightChange = GetHeightEstimate(firstSlot, lastSlot);
                    totalHeightChange -= heightChange;
                    slotsExpanded -= lastSlot - firstSlot + 1;

                    if (lastSlot > lastDisplayedSlot)
                    {
                        if (firstSlot > lastDisplayedSlot)
                        {
                            heightChangeBelowLastDisplayedSlot -= heightChange;
                        }
                        else
                        {
                            heightChangeBelowLastDisplayedSlot -= GetHeightEstimate(lastDisplayedSlot + 1, lastSlot);
                        }
                    }

                    firstSlot = lastSlot + 1;
                }
            }

            // Update _collapsedSlotsTable in one bulk operation
            _collapsedSlotsTable.AddValues(startSlot, endSlot - startSlot + 1, Visibility.Collapsed);

            return totalHeightChange;
        }

#if FEATURE_ICOLLECTIONVIEW_GROUP
        private void CollectionViewGroup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ItemCount")
            {
                DataGridRowGroupInfo rowGroupInfo = RowGroupInfoFromCollectionViewGroup(sender as CollectionViewGroup);
                if (rowGroupInfo != null && IsSlotVisible(rowGroupInfo.Slot))
                {
                    DataGridRowGroupHeader rowGroupHeader = this.DisplayData.GetDisplayedElement(rowGroupInfo.Slot) as DataGridRowGroupHeader;
                    if (rowGroupHeader != null)
                    {
                        rowGroupHeader.UpdateTitleElements();
                    }
                }
            }
        }
#endif

        private void CorrectEditingRow()
        {
            if (this.EditingRow != null)
            {
                this.EditingRow.Index = this.DataConnection.IndexOf(this.EditingRow.DataContext);
                this.EditingRow.Slot = this.SlotFromRowIndex(this.EditingRow.Index);
                if (this.EditingRow.Index > 0)
                {
                    // The collection actually removes and re-inserts the edited item during a commit operation.
                    // We recycle the editing row in this case in order to avoid generating a new element, but we don't
                    // care about refreshing its background/foreground until the item is added back (i.e. Index > 0).
                    this.EditingRow.EnsureBackground();
                    this.EditingRow.EnsureForeground();
                }
            }
        }

        // This method is necessary for incrementing the LastSubItemSlot property of the group ancestors
        // because CorrectSlotsAfterInsertion only increments those that come after the specified group.
        private void CorrectLastSubItemSlotsAfterInsertion(DataGridRowGroupInfo subGroupInfo)
        {
            int subGroupSlot;
            int subGroupLevel;
            while (subGroupInfo != null)
            {
                subGroupLevel = subGroupInfo.Level;
                subGroupInfo.LastSubItemSlot++;

                while (subGroupInfo != null && subGroupInfo.Level >= subGroupLevel)
                {
                    subGroupSlot = this.RowGroupHeadersTable.GetPreviousIndex(subGroupInfo.Slot);
                    subGroupInfo = this.RowGroupHeadersTable.GetValueAt(subGroupSlot);
                }
            }
        }

        /// <summary>
        /// Adjusts the index of all displayed, loaded and edited rows after a row was deleted.
        /// Removes the deleted row from the list of loaded rows if present.
        /// </summary>
        private void CorrectSlotsAfterDeletion(int slotDeleted, bool wasRow)
        {
            DiagnosticsDebug.Assert(slotDeleted >= 0, "Expected positive slotDeleted.");

            // Take care of the non-visible loaded rows
            for (int index = 0; index < _loadedRows.Count;)
            {
                DataGridRow dataGridRow = _loadedRows[index];
                if (this.IsSlotVisible(dataGridRow.Slot))
                {
                    index++;
                }
                else
                {
                    if (dataGridRow.Slot > slotDeleted)
                    {
                        CorrectRowAfterDeletion(dataGridRow, wasRow);
                        index++;
                    }
                    else if (dataGridRow.Slot == slotDeleted)
                    {
                        _loadedRows.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
            }

            // Take care of the non-visible edited row
            this.CorrectEditingRow();

            // Take care of the non-visible focused row
            if (_focusedRow != null &&
                _focusedRow != this.EditingRow &&
                !this.IsSlotVisible(_focusedRow.Slot) &&
                _focusedRow.Slot > slotDeleted)
            {
                CorrectRowAfterDeletion(_focusedRow, wasRow);
                _focusedRow.EnsureBackground();
                _focusedRow.EnsureForeground();
            }

            // Take care of the visible rows
            foreach (DataGridRow row in this.DisplayData.GetScrollingElements(true /*onlyRows*/))
            {
                if (row.Slot > slotDeleted)
                {
                    CorrectRowAfterDeletion(row, wasRow);
                    row.EnsureBackground();
                    row.EnsureForeground();
                }
            }

            // Update the RowGroupHeaders
            foreach (int slot in this.RowGroupHeadersTable.GetIndexes())
            {
                DataGridRowGroupInfo rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(slot);
                if (rowGroupInfo.Slot > slotDeleted)
                {
                    rowGroupInfo.Slot--;
                }

                if (rowGroupInfo.LastSubItemSlot >= slotDeleted)
                {
                    rowGroupInfo.LastSubItemSlot--;
                }
            }

            // Update which row we've calculated the RowHeightEstimate up to
            if (_lastEstimatedRow >= slotDeleted)
            {
                _lastEstimatedRow--;
            }
        }

        /// <summary>
        /// Adjusts the index of all displayed, loaded and edited rows after rows were deleted.
        /// </summary>
        private void CorrectSlotsAfterInsertion(int slotInserted, bool isCollapsed, bool rowInserted)
        {
            DiagnosticsDebug.Assert(slotInserted >= 0, "Expected positive slotInserted.");

            // Take care of the non-visible loaded rows
            foreach (DataGridRow dataGridRow in _loadedRows)
            {
                if (!this.IsSlotVisible(dataGridRow.Slot) && dataGridRow.Slot >= slotInserted)
                {
                    DataGrid.CorrectRowAfterInsertion(dataGridRow, rowInserted);
                }
            }

            // Take care of the non-visible focused row
            if (_focusedRow != null &&
                _focusedRow != EditingRow &&
                !(this.IsSlotVisible(_focusedRow.Slot) || ((_focusedRow.Slot == slotInserted) && isCollapsed)) &&
                _focusedRow.Slot >= slotInserted)
            {
                DataGrid.CorrectRowAfterInsertion(_focusedRow, rowInserted);
                _focusedRow.EnsureBackground();
                _focusedRow.EnsureForeground();
            }

            // Take care of the visible rows
            foreach (DataGridRow row in this.DisplayData.GetScrollingElements(true /*onlyRows*/))
            {
                if (row.Slot >= slotInserted)
                {
                    DataGrid.CorrectRowAfterInsertion(row, rowInserted);
                    row.EnsureBackground();
                    row.EnsureForeground();
                }
            }

            // Re-calculate the EditingRow's Slot and Index and ensure that it is still selected.
            this.CorrectEditingRow();

            // Update the RowGroupHeaders
            foreach (int slot in this.RowGroupHeadersTable.GetIndexes(slotInserted))
            {
                DataGridRowGroupInfo rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(slot);
                if (rowGroupInfo.Slot >= slotInserted)
                {
                    rowGroupInfo.Slot++;
                }

                // We are purposefully checking GT and not GTE because the equality case is handled
                // by the CorrectLastSubItemSlotsAfterInsertion method.
                if (rowGroupInfo.LastSubItemSlot > slotInserted)
                {
                    rowGroupInfo.LastSubItemSlot++;
                }
            }

            // Update which row we've calculated the RowHeightEstimate up to.
            if (_lastEstimatedRow >= slotInserted)
            {
                _lastEstimatedRow++;
            }
        }

        private int CountAndPopulateGroupHeaders(object group, int rootSlot, int level)
        {
            int treeCount = 1;

            ICollectionViewGroup collectionViewGroup = group as ICollectionViewGroup;
            if (collectionViewGroup != null)
            {
                if (collectionViewGroup.GroupItems != null && collectionViewGroup.GroupItems.Count > 0)
                {
#if FEATURE_ICOLLECTIONVIEW_GROUP
                    HookupCollectionChangedListenerToGroup(collectionViewGroup.GroupItems as INotifyCollectionChanged);
#else
                    HookupVectorChangedListenerToGroup(collectionViewGroup.GroupItems);
#endif
                    if (collectionViewGroup.GroupItems[0] is ICollectionViewGroup)
                    {
                        foreach (object subGroup in collectionViewGroup.GroupItems)
                        {
                            treeCount += CountAndPopulateGroupHeaders(subGroup, rootSlot + treeCount, level + 1);
                        }
                    }
                    else
                    {
                        // Optimization: don't walk to the bottom level nodes
                        treeCount += collectionViewGroup.GroupItems.Count;
                    }
                }

                this.RowGroupHeadersTable.AddValue(rootSlot, new DataGridRowGroupInfo(collectionViewGroup, Visibility.Visible, level, rootSlot, rootSlot + treeCount - 1));
            }

            return treeCount;
        }

#if FEATURE_ICOLLECTIONVIEW_GROUP
        private void CollectionViewGroup_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // If we receive this event when the number of GroupDescriptions is different than what we have already
            // accounted for, that means the ICollectionView is still in the process of updating its groups. It will
            // send a reset notification when it's done, at which point we can update our visuals.
            if (_rowGroupHeightsByLevel != null && this.DataConnection.CollectionView != null)
            {
                if (this.DataConnection.CollectionView.GroupDescriptions != null && this.DataConnection.CollectionView.GroupDescriptions.Count == _rowGroupHeightsByLevel.Length)
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            this.CollectionViewGroup_CollectionChanged_Add(sender, e);
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            this.CollectionViewGroup_CollectionChanged_Remove(sender, e);
                            break;
                    }
                }
            }
        }

        private void CollectionViewGroup_CollectionChanged_Add(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                OnCollectionViewGroupItemInserted(sender, e.NewItems[0], e.NewStartingIndex);
            }
        }

        private void CollectionViewGroup_CollectionChanged_Remove(object sender, NotifyCollectionChangedEventArgs e)
        {
            DiagnosticsDebug.Assert(e.OldItems.Count == 1, "Expected e.OldItems.Count equals 1.");
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                OnCollectionViewGroupItemRemoved(sender, e.OldItems[0], e.OldStartingIndex);
            }
        }
#else
        private void CollectionViewGroupItems_VectorChanged(IObservableVector<object> groupItems, IVectorChangedEventArgs e)
        {
            int index = (int)e.Index;
            switch (e.CollectionChange)
            {
                case CollectionChange.ItemChanged:
                    break;
                case CollectionChange.ItemInserted:
                    OnCollectionViewGroupItemInserted(groupItems, groupItems[index], index);
                    break;
                case CollectionChange.ItemRemoved:
                    OnCollectionViewGroupItemRemoved(groupItems, groupItems[index], index);
                    break;
                case CollectionChange.Reset:
                    break;
            }
        }
#endif

        private void EnsureAnscestorsExpanderButtonChecked(DataGridRowGroupInfo parentGroupInfo)
        {
            if (IsSlotVisible(parentGroupInfo.Slot))
            {
                DataGridRowGroupHeader ancestorGroupHeader = this.DisplayData.GetDisplayedElement(parentGroupInfo.Slot) as DataGridRowGroupHeader;
                while (ancestorGroupHeader != null)
                {
                    ancestorGroupHeader.EnsureExpanderButtonIsChecked();
                    if (ancestorGroupHeader.Level > 0)
                    {
                        int slot = this.RowGroupHeadersTable.GetPreviousIndex(ancestorGroupHeader.RowGroupInfo.Slot);
                        if (IsSlotVisible(slot))
                        {
                            ancestorGroupHeader = this.DisplayData.GetDisplayedElement(slot) as DataGridRowGroupHeader;
                            continue;
                        }
                    }

                    break;
                }
            }
        }

        private void EnsureRowDetailsVisibility(
            DataGridRow row,
            bool raiseNotification)
        {
            // Show or hide RowDetails based on DataGrid settings
            row.SetDetailsVisibilityInternal(
                GetRowDetailsVisibility(row.Index),
                raiseNotification);
        }

        private IEnumerable<DataGridRow> GetAllRows()
        {
            if (_rowsPresenter != null)
            {
                foreach (UIElement element in _rowsPresenter.Children)
                {
                    DataGridRow row = element as DataGridRow;
                    if (row != null)
                    {
                        yield return row;
                    }
                }
            }
        }

        private IEnumerable<DataGridRowGroupHeader> GetAllRowGroupHeaders()
        {
            if (_rowsPresenter != null)
            {
                foreach (UIElement element in _rowsPresenter.Children)
                {
                    DataGridRowGroupHeader rowGroupHeader = element as DataGridRowGroupHeader;
                    if (rowGroupHeader != null)
                    {
                        yield return rowGroupHeader;
                    }
                }
            }
        }

        // Returns the number of rows with details visible between lowerBound and upperBound exclusive.
        // As of now, the caller needs to account for Collapsed slots.  This method assumes everything
        // is visible
        private int GetDetailsCountInclusive(int lowerBound, int upperBound)
        {
            // Convert from slots to indexes.
            lowerBound = this.RowGroupHeadersTable.GetNextGap(lowerBound - 1);
            upperBound = this.RowGroupHeadersTable.GetPreviousGap(upperBound + 1);

            lowerBound = this.RowIndexFromSlot(lowerBound);
            upperBound = this.RowIndexFromSlot(upperBound);

            int indexCount = upperBound - lowerBound + 1;
            if (indexCount <= 0)
            {
                return 0;
            }

            if (this.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Visible)
            {
                // Total rows minus ones which explicitly turned details off
                return indexCount - _showDetailsTable.GetIndexCount(lowerBound, upperBound, Visibility.Collapsed);
            }
            else if (this.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Collapsed)
            {
                // Total rows with details explicitly turned on
                return _showDetailsTable.GetIndexCount(lowerBound, upperBound, Visibility.Visible);
            }
            else if (this.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected)
            {
                // Total number of remaining rows that are selected
                return _selectedItems.GetIndexCount(lowerBound, upperBound);
            }

            DiagnosticsDebug.Assert(false, "Expected known RowDetailsVisibilityMode value."); // Shouldn't ever happen
            return 0;
        }

        private void EnsureRowGroupSpacerColumn()
        {
            bool spacerColumnChanged = this.ColumnsInternal.EnsureRowGrouping(!this.RowGroupHeadersTable.IsEmpty);
            if (spacerColumnChanged)
            {
                if (this.ColumnsInternal.RowGroupSpacerColumn.IsRepresented && this.CurrentColumnIndex == 0)
                {
                    this.CurrentColumn = this.ColumnsInternal.FirstVisibleNonFillerColumn;
                }

                ProcessFrozenColumnCount(this);
            }
        }

        private void EnsureRowGroupSpacerColumnWidth(int groupLevelCount)
        {
            if (groupLevelCount == 0)
            {
                this.ColumnsInternal.RowGroupSpacerColumn.Width = new DataGridLength(0);
            }
            else
            {
                this.ColumnsInternal.RowGroupSpacerColumn.Width = new DataGridLength(this.RowGroupSublevelIndents[groupLevelCount - 1]);
            }
        }

        private void EnsureRowGroupVisibility(DataGridRowGroupInfo rowGroupInfo, Visibility visibility, bool setCurrent)
        {
            if (rowGroupInfo == null)
            {
                return;
            }

            if (rowGroupInfo.Visibility != visibility)
            {
                if (this.IsSlotVisible(rowGroupInfo.Slot))
                {
                    DataGridRowGroupHeader rowGroupHeader = this.DisplayData.GetDisplayedElement(rowGroupInfo.Slot) as DataGridRowGroupHeader;
                    DiagnosticsDebug.Assert(rowGroupHeader != null, "Expected non-null rowGroupHeader.");
                    rowGroupHeader.ToggleExpandCollapse(visibility, setCurrent);
                }
                else
                {
                    if (_collapsedSlotsTable.Contains(rowGroupInfo.Slot))
                    {
                        // Somewhere up the parent chain, there's a collapsed header so all the slots remain the same and
                        // we just need to mark this header with the new visibility
                        rowGroupInfo.Visibility = visibility;
                    }
                    else
                    {
                        if (rowGroupInfo.Slot < this.DisplayData.FirstScrollingSlot)
                        {
                            double heightChange = UpdateRowGroupVisibility(rowGroupInfo, visibility, false /*isHeaderDisplayed*/);

                            // Use epsilon instead of 0 here so that in the off chance that our estimates put the vertical offset negative
                            // the user can still scroll to the top since the offset is non-zero
                            SetVerticalOffset(Math.Max(DoubleUtil.DBL_EPSILON, _verticalOffset + heightChange));

                            this.DisplayData.FullyRecycleElements();
                        }
                        else
                        {
                            UpdateRowGroupVisibility(rowGroupInfo, visibility, false /*isHeaderDisplayed*/);
                        }

                        UpdateVerticalScrollBar();
                    }
                }
            }
        }

        // Expands slots from startSlot to endSlot inclusive and adds the amount expanded in this suboperation to
        // the given totalHeightChanged of the entire operation
        private void ExpandSlots(int startSlot, int endSlot, bool isHeaderDisplayed, ref int slotsExpanded, ref double totalHeightChange)
        {
            double heightAboveStartSlot = 0;
            if (isHeaderDisplayed)
            {
                int slot = this.DisplayData.FirstScrollingSlot;
                while (slot < startSlot)
                {
                    heightAboveStartSlot += GetExactSlotElementHeight(slot);
                    slot = GetNextVisibleSlot(slot);
                }

                // First make the bottom rows available for recycling so we minimize element creation when expanding
                for (int i = 0; (i < endSlot - startSlot + 1) && (this.DisplayData.LastScrollingSlot > endSlot); i++)
                {
                    RemoveDisplayedElement(this.DisplayData.LastScrollingSlot, false /*wasDeleted*/, true /*updateSlotInformation*/);
                }
            }

            // Figure out which slots actually need to be expanded since some might already be collapsed
            double currentHeightChange = 0;
            int firstSlot = startSlot;
            int lastSlot;
            while (firstSlot <= endSlot)
            {
                firstSlot = _collapsedSlotsTable.GetNextIndex(firstSlot - 1);
                if (firstSlot == -1)
                {
                    break;
                }

                lastSlot = Math.Min(endSlot, _collapsedSlotsTable.GetNextGap(firstSlot) - 1);

                if (firstSlot <= lastSlot)
                {
                    if (!isHeaderDisplayed)
                    {
                        // Estimate the height change if the slots aren't displayed.  If they are displayed, we can add real values
                        double rowCount = lastSlot - firstSlot + 1;
                        rowCount -= GetRowGroupHeaderCount(firstSlot, lastSlot, Visibility.Collapsed, out var headerHeight);
                        double detailsCount = GetDetailsCountInclusive(firstSlot, lastSlot);
                        currentHeightChange += headerHeight + (detailsCount * this.RowDetailsHeightEstimate) + (rowCount * this.RowHeightEstimate);
                    }

                    slotsExpanded += lastSlot - firstSlot + 1;
                    firstSlot = lastSlot + 1;
                }
            }

            // Update _collapsedSlotsTable in one bulk operation
            _collapsedSlotsTable.RemoveValues(startSlot, endSlot - startSlot + 1);

            if (isHeaderDisplayed)
            {
                double availableHeight = this.CellsHeight - heightAboveStartSlot;

                // Actually expand the displayed slots up to what we can display
                int lastExpandedSlot = -1;
                for (int i = startSlot; (i <= endSlot) && (currentHeightChange < availableHeight); i++)
                {
                    FrameworkElement insertedElement = InsertDisplayedElement(i, false /*updateSlotInformation*/);
                    lastExpandedSlot = i;
                    currentHeightChange += insertedElement.EnsureMeasured().DesiredSize.Height;
                    if (i > this.DisplayData.LastScrollingSlot)
                    {
                        this.DisplayData.LastScrollingSlot = i;
                    }
                }

                // We were unable to expand the slots from (lastExpandedSlot + 1) to endSlot because we ran out of space;
                // however, we also have extra visible elements below endSlot.  In this case, we need to remove the
                // extra elements.  While we remove these, we need to mark (lastExpandedSlot + 1) to endSlot collapsed
                // because that is a temporary gap that is not accounted for.
                if ((lastExpandedSlot != -1) && (lastExpandedSlot < endSlot) && (this.DisplayData.LastScrollingSlot > endSlot))
                {
                    // Temporarily account for the slots we couldn't expand by marking them collapsed
                    _collapsedSlotsTable.AddValues(lastExpandedSlot + 1, endSlot - lastExpandedSlot, Visibility.Collapsed);

                    // Remove the extra elements below our lastExpandedSlot
                    RemoveNonDisplayedRows(this.DisplayData.FirstScrollingSlot, lastExpandedSlot);

                    // Remove the temporarily marked collapsed rows
                    _collapsedSlotsTable.RemoveValues(lastExpandedSlot + 1, endSlot - lastExpandedSlot);
                }
            }

            // Update the total height for the entire Expand operation
            totalHeightChange += currentHeightChange;
        }

        /// <summary>
        /// Creates all the editing elements for the current editing row, so the bindings
        /// all exist during validation.
        /// </summary>
        private void GenerateEditingElements()
        {
            if (this.EditingRow != null && this.EditingRow.Cells != null)
            {
                DiagnosticsDebug.Assert(this.EditingRow.Cells.Count == this.ColumnsItemsInternal.Count, "Expected EditingRow.Cells.Count equals this.ColumnsItemsInternal.Count.");
                foreach (DataGridColumn column in this.ColumnsInternal.GetDisplayedColumns(c => c.IsVisible && !c.IsReadOnly))
                {
                    column.GenerateEditingElementInternal(this.EditingRow.Cells[column.Index], this.EditingRow.DataContext);
                }
            }
        }

        /// <summary>
        /// Returns a row for the provided index. The row gets first loaded through the LoadingRow event.
        /// </summary>
        /// <returns>A row for the provided index.</returns>
        private DataGridRow GenerateRow(int rowIndex, int slot)
        {
            return GenerateRow(rowIndex, slot, this.DataConnection.GetDataItem(rowIndex));
        }

        /// <summary>
        /// Returns a row for the provided index. The row gets first loaded through the LoadingRow event.
        /// </summary>
        /// <returns>A row for the provided index.</returns>
        private DataGridRow GenerateRow(int rowIndex, int slot, object dataContext)
        {
            DiagnosticsDebug.Assert(rowIndex >= 0, "Expected positive rowIndex.");
            DataGridRow dataGridRow = GetGeneratedRow(dataContext);
            if (dataGridRow == null)
            {
                dataGridRow = this.DisplayData.GetUsedRow() ?? new DataGridRow();
                dataGridRow.Index = rowIndex;
                dataGridRow.Slot = slot;
                dataGridRow.OwningGrid = this;
                dataGridRow.DataContext = dataContext;
                CompleteCellsCollection(dataGridRow);

                OnLoadingRow(new DataGridRowEventArgs(dataGridRow));

                DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
                if (peer != null)
                {
                    peer.UpdateRowPeerEventsSource(dataGridRow);
                }
            }

            return dataGridRow;
        }

        private DataGridRowGroupHeader GenerateRowGroupHeader(int slot, DataGridRowGroupInfo rowGroupInfo)
        {
            DiagnosticsDebug.Assert(slot >= 0, "Expected positive slot.");
            DiagnosticsDebug.Assert(rowGroupInfo != null, "Expected non-null rowGroupInfo.");

            DataGridRowGroupHeader groupHeader = this.DisplayData.GetUsedGroupHeader() ?? new DataGridRowGroupHeader();
            groupHeader.OwningGrid = this;
            groupHeader.RowGroupInfo = rowGroupInfo;
            groupHeader.DataContext = rowGroupInfo.CollectionViewGroup;
            groupHeader.Level = rowGroupInfo.Level;

#if FEATURE_ICOLLECTIONVIEW_GROUP
            DiagnosticsDebug.Assert(this.DataConnection.CollectionView != null && groupHeader.Level < this.DataConnection.CollectionView.GroupDescriptions.Count);
            PropertyGroupDescription propertyGroupDescription = this.DataConnection.CollectionView.GroupDescriptions[groupHeader.Level] as PropertyGroupDescription;
            if (propertyGroupDescription != null)
            {
                groupHeader.PropertyName = propertyGroupDescription.PropertyName;
            }

            // Listen for CollectionViewGroup.PropertyChanged in order to update Title when ItemCount changes
            if (rowGroupInfo.CollectionViewGroup != null)
            {
                INotifyPropertyChanged inpc = rowGroupInfo.CollectionViewGroup as INotifyPropertyChanged;
                if (inpc != null && !_groupsPropertyChangedListenersTable.ContainsKey(inpc))
                {
                    WeakEventListener<DataGrid, object, PropertyChangedEventArgs> weakPropertyChangedListener = new WeakEventListener<DataGrid, object, PropertyChangedEventArgs>(this);
                    weakPropertyChangedListener.OnEventAction = (instance, source, eventArgs) => instance.CollectionViewGroup_PropertyChanged(source, eventArgs);
                    weakPropertyChangedListener.OnDetachAction = (weakEventListener) => inpc.PropertyChanged -= weakEventListener.OnEvent;
                    inpc.PropertyChanged += weakPropertyChangedListener.OnEvent;

                    _groupsPropertyChangedListenersTable.Add(inpc, weakPropertyChangedListener);
                }
            }
#endif

            OnLoadingRowGroup(new DataGridRowGroupHeaderEventArgs(groupHeader));

            if (!string.IsNullOrWhiteSpace(groupHeader.PropertyName) &&
                string.IsNullOrEmpty(groupHeader.PropertyValue) &&
                rowGroupInfo.CollectionViewGroup.GroupItems != null &&
                rowGroupInfo.CollectionViewGroup.GroupItems.Count > 0)
            {
                object propertyValue = TypeHelper.GetNestedPropertyValue(rowGroupInfo.CollectionViewGroup.GroupItems[0], groupHeader.PropertyName);

                if (propertyValue != null)
                {
                    groupHeader.PropertyValue = propertyValue.ToString();
                }
            }

            groupHeader.UpdateTitleElements();

            DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
            if (peer != null)
            {
                peer.UpdateRowGroupHeaderPeerEventsSource(groupHeader);
            }

            return groupHeader;
        }

        /// <summary>
        /// Returns the exact row height, whether it is currently displayed or not.
        /// The row is generated and added to the displayed rows in case it is not already displayed.
        /// The horizontal gridlines thickness are added.
        /// </summary>
        /// <returns>Exact row height with gridlines thickness.</returns>
        private double GetExactSlotElementHeight(int slot)
        {
            DiagnosticsDebug.Assert(slot >= 0, "Expected positive slot.");
            DiagnosticsDebug.Assert(slot < this.SlotCount, "Expected slot  smaller than SlotCount.");

            if (this.IsSlotVisible(slot))
            {
                DiagnosticsDebug.Assert(this.DisplayData.GetDisplayedElement(slot) != null, "Expected non-null DisplayData.GetDisplayedElement(slot).");
                return this.DisplayData.GetDisplayedElement(slot).EnsureMeasured().DesiredSize.Height;
            }

            // InsertDisplayedElement automatically measures the element
            FrameworkElement slotElement = InsertDisplayedElement(slot, true /*updateSlotInformation*/);
            DiagnosticsDebug.Assert(slotElement != null, "Expected non-null slotElement.");
            return slotElement.DesiredSize.Height;
        }

        // Returns an estimate for the height of the slots between fromSlot and toSlot
        private double GetHeightEstimate(int fromSlot, int toSlot)
        {
            double rowCount = toSlot - fromSlot + 1;
            rowCount -= GetRowGroupHeaderCount(fromSlot, toSlot, Visibility.Visible, out var headerHeight);
            double detailsCount = GetDetailsCountInclusive(fromSlot, toSlot);

            return headerHeight + (detailsCount * this.RowDetailsHeightEstimate) + (rowCount * this.RowHeightEstimate);
        }

        private DataGridRowGroupInfo GetParentGroupInfo(object collection)
        {
            if (collection == this.DataConnection.CollectionView.CollectionGroups)
            {
                // If the new item is a root level element, it has no parent group, so create an empty RowGroupInfo
                return new DataGridRowGroupInfo(null, Visibility.Visible, -1, -1, -1);
            }
            else
            {
                foreach (int slot in this.RowGroupHeadersTable.GetIndexes())
                {
                    DataGridRowGroupInfo groupInfo = this.RowGroupHeadersTable.GetValueAt(slot);
                    if (groupInfo.CollectionViewGroup.GroupItems == collection)
                    {
                        return groupInfo;
                    }
                }
            }

            return null;
        }

        // Returns the inclusive count of expanded RowGroupHeaders from startSlot to endSlot
        private int GetRowGroupHeaderCount(int startSlot, int endSlot, Visibility? visibility, out double headersHeight)
        {
            int count = 0;
            headersHeight = 0;
            foreach (int slot in this.RowGroupHeadersTable.GetIndexes(startSlot))
            {
                if (slot > endSlot)
                {
                    return count;
                }

                DataGridRowGroupInfo rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(slot);
                if (!visibility.HasValue ||
                    (visibility == Visibility.Visible && !_collapsedSlotsTable.Contains(slot)) ||
                    (visibility == Visibility.Collapsed && _collapsedSlotsTable.Contains(slot)))
                {
                    count++;
                    headersHeight += _rowGroupHeightsByLevel[rowGroupInfo.Level];
                }
            }

            return count;
        }

        /// <summary>
        /// If the provided slot is displayed, returns the exact height.
        /// If the slot is not displayed, returns a default height.
        /// </summary>
        /// <returns>Exact height of displayed slot, or default height otherwise.</returns>
        private double GetSlotElementHeight(int slot)
        {
            DiagnosticsDebug.Assert(slot >= 0, "Expected positive slot.");
            DiagnosticsDebug.Assert(slot < this.SlotCount, "Expected slot  smaller than SlotCount.");
            if (this.IsSlotVisible(slot))
            {
                DiagnosticsDebug.Assert(this.DisplayData.GetDisplayedElement(slot) != null, "Expected non-null DisplayData.GetDisplayedElement(slot).");
                return this.DisplayData.GetDisplayedElement(slot).EnsureMeasured().DesiredSize.Height;
            }
            else
            {
                DataGridRowGroupInfo rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(slot);
                if (rowGroupInfo != null)
                {
                    return _rowGroupHeightsByLevel[rowGroupInfo.Level];
                }

                // Assume it's a row since we're either not grouping or it wasn't a RowGroupHeader.
                return this.RowHeightEstimate + (GetRowDetailsVisibility(this.RowIndexFromSlot(slot)) == Visibility.Visible ? this.RowDetailsHeightEstimate : 0);
            }
        }

        /// <summary>
        /// Cumulates the approximate height of the non-collapsed slots from fromSlot to toSlot inclusive.
        /// Including the potential gridline thickness.
        /// </summary>
        /// <returns>Cumulated approximate height of the non-collapsed slots from fromSlot to toSlot inclusive including the potential gridline thickness.</returns>
        private double GetSlotElementsHeight(int fromSlot, int toSlot)
        {
            DiagnosticsDebug.Assert(toSlot >= fromSlot, "Expected toSlot greater or equal to fromSlot.");

            double height = 0;
            for (int slot = GetNextVisibleSlot(fromSlot - 1); slot <= toSlot; slot = GetNextVisibleSlot(slot))
            {
                height += GetSlotElementHeight(slot);
            }

            return height;
        }

        /// <summary>
        /// Checks if the row for the provided dataContext has been generated and is present
        /// in either the loaded rows, pre-fetched rows, or editing row.
        /// The displayed rows are *not* searched. Returns null if the row does not belong to those 3 categories.
        /// </summary>
        /// <returns>Either a loaded, or pre-fetched or editing row.</returns>
        private DataGridRow GetGeneratedRow(object dataContext)
        {
            // Check the list of rows being loaded via the LoadingRow event.
            DataGridRow dataGridRow = GetLoadedRow(dataContext);
            if (dataGridRow != null)
            {
                return dataGridRow;
            }

            // Check the potential editing row.
            if (this.EditingRow != null && dataContext == this.EditingRow.DataContext)
            {
                return this.EditingRow;
            }

            // Check the potential focused row.
            if (_focusedRow != null && dataContext == _focusedRow.DataContext)
            {
                return _focusedRow;
            }

            return null;
        }

        private DataGridRow GetLoadedRow(object dataContext)
        {
            foreach (DataGridRow dataGridRow in _loadedRows)
            {
                if (dataGridRow.DataContext == dataContext)
                {
                    return dataGridRow;
                }
            }

            return null;
        }

        private FrameworkElement InsertDisplayedElement(int slot, bool updateSlotInformation)
        {
            FrameworkElement slotElement;
            if (this.RowGroupHeadersTable.Contains(slot))
            {
                slotElement = GenerateRowGroupHeader(slot, this.RowGroupHeadersTable.GetValueAt(slot) /*rowGroupInfo*/);
            }
            else
            {
                // If we're grouping, the GroupLevel needs to be fixed later by methods calling this
                // which end up inserting rows. We don't do it here because elements could be inserted
                // from top to bottom or bottom to up so it's better to do in one pass
                slotElement = GenerateRow(RowIndexFromSlot(slot), slot);
            }

            InsertDisplayedElement(slot, slotElement, false /*wasNewlyAdded*/, updateSlotInformation);
            return slotElement;
        }

        private void InsertDisplayedElement(int slot, UIElement element, bool wasNewlyAdded, bool updateSlotInformation)
        {
            // We can only support creating new rows that are adjacent to the currently visible rows
            // since they need to be added to the visual tree for us to Measure them.
            DiagnosticsDebug.Assert(
                this.DisplayData.FirstScrollingSlot == -1 || (slot >= GetPreviousVisibleSlot(this.DisplayData.FirstScrollingSlot) && slot <= GetNextVisibleSlot(this.DisplayData.LastScrollingSlot)),
                "Expected DisplayData.FirstScrollingSlot equals -1 or (slot greater than or equal to GetPreviousVisibleSlot(DisplayData.FirstScrollingSlot) and slot smaller than or equal to GetNextVisibleSlot(DisplayData.LastScrollingSlot)).");
            DiagnosticsDebug.Assert(element != null, "Expected non-null element.");

            if (_rowsPresenter != null)
            {
                DataGridRowGroupHeader groupHeader = null;
                DataGridRow row = element as DataGridRow;
                if (row != null)
                {
                    LoadRowVisualsForDisplay(row);

                    if (IsRowRecyclable(row))
                    {
                        if (!row.IsRecycled)
                        {
                            DiagnosticsDebug.Assert(!_rowsPresenter.Children.Contains(element), "Expected element not contained in _rowsPresenter.Children.");
                            _rowsPresenter.Children.Add(row);
                        }
                    }
                    else
                    {
                        element.Clip = null;
                        DiagnosticsDebug.Assert(row.Index == RowIndexFromSlot(slot), "Expected row.Index equals RowIndexFromSlot(slot).");
                        if (!_rowsPresenter.Children.Contains(row))
                        {
                            _rowsPresenter.Children.Add(row);
                        }
                    }
                }
                else
                {
                    groupHeader = element as DataGridRowGroupHeader;
                    DiagnosticsDebug.Assert(groupHeader != null, "Expected non-null grouHeader.");
                    if (groupHeader != null)
                    {
                        groupHeader.TotalIndent = (groupHeader.Level == 0) ? 0 : this.RowGroupSublevelIndents[groupHeader.Level - 1];
                        if (!groupHeader.IsRecycled)
                        {
                            _rowsPresenter.Children.Add(element);
                        }

                        groupHeader.LoadVisualsForDisplay();

                        Style lastStyle = _rowGroupHeaderStyles.Count > 0 ? _rowGroupHeaderStyles[_rowGroupHeaderStyles.Count - 1] : null;
                        EnsureElementStyle(groupHeader, groupHeader.Style, groupHeader.Level < _rowGroupHeaderStyles.Count ? _rowGroupHeaderStyles[groupHeader.Level] : lastStyle);
                    }
                }

                // Measure the element and update AvailableRowRoom
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                this.AvailableSlotElementRoom -= element.DesiredSize.Height;

                if (groupHeader != null)
                {
                    _rowGroupHeightsByLevel[groupHeader.Level] = groupHeader.DesiredSize.Height;
                }

                if (row != null && this.RowHeightEstimate == DataGrid.DATAGRID_defaultRowHeight && double.IsNaN(row.Height))
                {
                    this.RowHeightEstimate = row.DesiredSize.Height - row.DetailsContentHeight;
                }
            }

            if (wasNewlyAdded)
            {
                this.DisplayData.CorrectSlotsAfterInsertion(slot, element, false /*isCollapsed*/);
            }
            else
            {
                this.DisplayData.LoadScrollingSlot(slot, element, updateSlotInformation);
            }
        }

        private void InsertElement(int slot, UIElement element, bool updateVerticalScrollBarOnly, bool isCollapsed, bool isRow)
        {
            DiagnosticsDebug.Assert(slot >= 0, "Expected positive slot.");
            DiagnosticsDebug.Assert(slot <= this.SlotCount, "Expected slot smaller than or equal to SlotCount.");

            OnInsertingElement(slot, true /*firstInsertion*/, isCollapsed, isRow);   // will throw an exception if the insertion is illegal

            OnInsertedElement_Phase1(slot, element, isCollapsed, isRow);
            this.SlotCount++;
            if (!isCollapsed)
            {
                this.VisibleSlotCount++;
            }

            OnInsertedElement_Phase2(slot, updateVerticalScrollBarOnly, isCollapsed);
        }

        private void InvalidateRowHeightEstimate()
        {
            // Start from scratch and assume that we haven't estimated any rows
            _lastEstimatedRow = -1;
        }

        private void OnAddedElement_Phase1(int slot, UIElement element)
        {
            DiagnosticsDebug.Assert(slot >= 0, "Expected positive slot.");

            // Row needs to be potentially added to the displayed rows
            if (SlotIsDisplayed(slot))
            {
                InsertDisplayedElement(slot, element, true /*wasNewlyAdded*/, true);
            }
        }

        private void OnAddedElement_Phase2(int slot, bool updateVerticalScrollBarOnly)
        {
            if (slot < this.DisplayData.FirstScrollingSlot - 1)
            {
                // The element was added above our viewport so it pushes the VerticalOffset down
                double elementHeight = this.RowGroupHeadersTable.Contains(slot) ? this.RowGroupHeaderHeightEstimate : this.RowHeightEstimate;

                SetVerticalOffset(_verticalOffset + elementHeight);
            }

            if (updateVerticalScrollBarOnly)
            {
                UpdateVerticalScrollBar();
            }
            else
            {
                ComputeScrollBarsLayout();

                // Reposition rows in case we use a recycled one
                InvalidateRowsArrange();
            }
        }

        private void OnCollectionViewGroupItemInserted(object groupItems, object insertedItem, int insertedIndex)
        {
            // We need to figure out the CollectionViewGroup that the sender belongs to.  We could cache
            // it by tagging the collections ahead of time, but I think the extra storage might not be worth
            // it since this lookup should be performant enough
            int insertSlot = -1;
            DataGridRowGroupInfo parentGroupInfo = GetParentGroupInfo(groupItems);
            ICollectionViewGroup group = insertedItem as ICollectionViewGroup;

            if (parentGroupInfo != null)
            {
                if (group != null || parentGroupInfo.Level == -1)
                {
                    insertSlot = parentGroupInfo.Slot + 1;

                    // For groups, we need to skip over subgroups to find the correct slot
                    DataGridRowGroupInfo groupInfo;
                    for (int i = 0; i < insertedIndex; i++)
                    {
                        do
                        {
                            insertSlot = this.RowGroupHeadersTable.GetNextIndex(insertSlot);
                            groupInfo = this.RowGroupHeadersTable.GetValueAt(insertSlot);
                        }
                        while (groupInfo != null && groupInfo.Level > parentGroupInfo.Level + 1);

                        if (groupInfo == null)
                        {
#if FEATURE_IEDITABLECOLLECTIONVIEW
                            // We couldn't find the subchild so this should go at the end
                            // if it's the placeholder, or second from the end if it's a new group or item.
                            if (this.DataConnection.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd &&
                                this.DataConnection.IndexOf(e.NewItems[0]) != this.DataConnection.NewItemPlaceholderIndex &&
                                this.SlotCount > 0)
                            {
                                insertSlot = this.SlotCount - 1;
                            }
                            else
#endif
                            {
                                insertSlot = this.SlotCount;
                            }
                        }
                    }
                }
                else
                {
                    // For items the slot is a simple calculation
                    insertSlot = parentGroupInfo.Slot + insertedIndex + 1;
                }
            }

            if (insertSlot != -1)
            {
                bool isCollapsed = parentGroupInfo != null && (parentGroupInfo.Visibility == Visibility.Collapsed || _collapsedSlotsTable.Contains(parentGroupInfo.Slot));
                if (group != null)
                {
                    if (group.GroupItems != null)
                    {
#if FEATURE_ICOLLECTIONVIEW_GROUP
                        HookupCollectionChangedListenerToGroup(group.GroupItems as INotifyCollectionChanged);
#else
                        HookupVectorChangedListenerToGroup(group.GroupItems);
#endif
                    }

                    DataGridRowGroupInfo newGroupInfo = new DataGridRowGroupInfo(group, Visibility.Visible, parentGroupInfo.Level + 1, insertSlot, insertSlot);
                    InsertElementAt(insertSlot, -1 /*rowIndex*/, null /*item*/, newGroupInfo, isCollapsed);
                    this.RowGroupHeadersTable.AddValue(insertSlot, newGroupInfo);
                }
                else
                {
                    // Assume we're adding a new row
                    int rowIndex = this.DataConnection.IndexOf(insertedItem);
                    DiagnosticsDebug.Assert(rowIndex != -1, "Expected rowIndex other than -1.");
                    if (this.SlotCount == 0 && this.DataConnection.ShouldAutoGenerateColumns)
                    {
                        AutoGenerateColumnsPrivate();
                    }

                    InsertElementAt(insertSlot, rowIndex, insertedItem /*item*/, null /*rowGroupInfo*/, isCollapsed);
                }

                CorrectLastSubItemSlotsAfterInsertion(parentGroupInfo);
                if (parentGroupInfo.LastSubItemSlot - parentGroupInfo.Slot == 1)
                {
                    // We just added the first item to a RowGroup so the header should transition from Empty to either Expanded or Collapsed
                    EnsureAnscestorsExpanderButtonChecked(parentGroupInfo);
                }
            }
        }

        private void OnCollectionViewGroupItemRemoved(object groupItems, object removedItem, int removedIndex)
        {
            ICollectionViewGroup removedGroup = removedItem as ICollectionViewGroup;
            if (removedGroup != null)
            {
                if (removedGroup.GroupItems != null)
                {
#if FEATURE_ICOLLECTIONVIEW_GROUP
                    UnhookCollectionChangedListenerFromGroup(removedGroup.GroupItems as INotifyCollectionChanged, true /*removeFromTable*/);
#else
                    UnhookVectorChangedListenerFromGroup(removedGroup.GroupItems, true /*removeFromTable*/);
#endif
                }

#if FEATURE_ICOLLECTIONVIEW_GROUP
                WeakEventListener<DataGrid, object, PropertyChangedEventArgs> weakPropertyChangedListener;
                INotifyPropertyChanged inpc = removedGroup as INotifyPropertyChanged;
                if (inpc != null && _groupsPropertyChangedListenersTable.TryGetValue(inpc, out weakPropertyChangedListener))
                {
                    weakPropertyChangedListener.Detach();
                    _groupsPropertyChangedListenersTable.Remove(inpc);
                }
#else
#endif

                DataGridRowGroupInfo groupInfo = RowGroupInfoFromCollectionViewGroup(removedGroup);
                DiagnosticsDebug.Assert(groupInfo != null, "Expected non-null groupInfo.");
                if ((groupInfo.Level == _rowGroupHeightsByLevel.Length - 1) && (removedGroup.GroupItems != null) && (removedGroup.GroupItems.Count > 0))
                {
                    DiagnosticsDebug.Assert(groupInfo.LastSubItemSlot - groupInfo.Slot == removedGroup.GroupItems.Count, "Expected groupInfo.LastSubItemSlot - groupInfo.Slot equals removedGroup.GroupItems.Count.");

                    // If we're removing a leaf Group then remove all of its items before removing the Group.
                    for (int i = 0; i < removedGroup.GroupItems.Count; i++)
                    {
                        RemoveElementAt(groupInfo.Slot + 1, removedGroup.GroupItems[i] /*item*/, true /*isRow*/);
                    }
                }

                RemoveElementAt(groupInfo.Slot, null /*item*/, false /*isRow*/);
            }
            else
            {
                // A single item was removed from a leaf group
                DataGridRowGroupInfo parentGroupInfo = GetParentGroupInfo(groupItems);
                if (parentGroupInfo != null)
                {
                    int slot;
                    if (parentGroupInfo.CollectionViewGroup == null && this.RowGroupHeadersTable.IndexCount > 0)
                    {
#if FEATURE_IEDITABLECOLLECTIONVIEW
                        // In this case, we're removing from the root group.  If there are other groups, then this must
                        // be either the new item row or the placeholder that doesn't belong to any group.
                        if (this.DataConnection.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd && this.SlotCount > 1)
                        {
                            slot = this.SlotCount - 2;
                        }
                        else
#endif
                        {
                            slot = this.SlotCount - 1;
                        }
                    }
                    else
                    {
                        slot = parentGroupInfo.Slot + removedIndex + 1;
                    }

                    RemoveElementAt(slot, removedItem, true /*isRow*/);
                }
            }
        }

        private void OnElementsChanged(bool grew)
        {
            if (grew &&
                this.ColumnsItemsInternal.Count > 0 &&
                this.CurrentColumnIndex == -1)
            {
                MakeFirstDisplayedCellCurrentCell();
            }
        }

        private void OnInsertedElement_Phase1(int slot, UIElement element, bool isCollapsed, bool isRow)
        {
            DiagnosticsDebug.Assert(slot >= 0, "Expected positive slot.");

            // Fix the Index of all following rows
            CorrectSlotsAfterInsertion(slot, isCollapsed, isRow);

            // Next, same effect as adding a row
            if (element != null)
            {
#if DEBUG
                DataGridRow dataGridRow = element as DataGridRow;
                if (dataGridRow != null)
                {
                    DiagnosticsDebug.Assert(dataGridRow.Cells.Count == this.ColumnsItemsInternal.Count, "Expected dataGridRow.Cells.Count equals ColumnsItemsInternal.Count.");

                    int columnIndex = 0;
                    foreach (DataGridCell dataGridCell in dataGridRow.Cells)
                    {
                        DiagnosticsDebug.Assert(dataGridCell.OwningRow == dataGridRow, "Expected dataGridRow owns dataGridCell.");
                        DiagnosticsDebug.Assert(dataGridCell.OwningColumn == this.ColumnsItemsInternal[columnIndex], "Expected ColumnsItemsInternal[columnIndex] owns dataGridCell.");
                        columnIndex++;
                    }
                }
#endif
                DiagnosticsDebug.Assert(!isCollapsed, "Expected isCollapsed is false.");
                OnAddedElement_Phase1(slot, element);
            }
            else if ((slot <= this.DisplayData.FirstScrollingSlot) || (isCollapsed && (slot <= this.DisplayData.LastScrollingSlot)))
            {
                this.DisplayData.CorrectSlotsAfterInsertion(slot, null /*row*/, isCollapsed);
            }
        }

        private void OnInsertedElement_Phase2(int slot, bool updateVerticalScrollBarOnly, bool isCollapsed)
        {
            DiagnosticsDebug.Assert(slot >= 0, "Expected positive slot.");

            if (!isCollapsed)
            {
                // Same effect as adding a row
                OnAddedElement_Phase2(slot, updateVerticalScrollBarOnly);
            }
        }

        private void OnInsertingElement(int slotInserted, bool firstInsertion, bool isCollapsed, bool isRow)
        {
            // Reset the current cell's address if it's after the inserted row.
            if (firstInsertion)
            {
                if (this.CurrentSlot != -1 && slotInserted <= this.CurrentSlot)
                {
                    // The underlying data was already added, therefore we need to avoid accessing any back-end data since we might be off by 1 row.
                    _temporarilyResetCurrentCell = true;
                    bool success = SetCurrentCellCore(-1, -1);
                    DiagnosticsDebug.Assert(success, "Expected successful SetCurrentCellCore call.");
                }
            }

            // Update the slot ranges for the RowGroupHeaders before updating the _selectedItems table,
            // because it's dependent on the slots being correct with regards to grouping.
            this.RowGroupHeadersTable.InsertIndex(slotInserted);

            _selectedItems.InsertIndex(slotInserted);
            if (isRow)
            {
                // Since details are only visible for rows, the showDetailsTable only contains row indexes.
                int rowIndex = this.RowIndexFromSlot(slotInserted);
                if (rowIndex == this.DataConnection.NewItemPlaceholderIndex)
                {
                    _showDetailsTable.InsertIndexAndValue(rowIndex, Visibility.Collapsed);
                }
                else
                {
                    _showDetailsTable.InsertIndex(rowIndex);
                }
            }

            if (isCollapsed)
            {
                _collapsedSlotsTable.InsertIndexAndValue(slotInserted, Visibility.Collapsed);
            }
            else
            {
                _collapsedSlotsTable.InsertIndex(slotInserted);
            }

            // If we've inserted rows before the current selected item, update its index
            if (slotInserted <= this.SelectedIndex)
            {
                this.SetValueNoCallback(SelectedIndexProperty, this.SelectedIndex + 1);
            }
        }

        private void OnRemovedElement(int slotDeleted, object itemDeleted, bool isRow)
        {
            this.SlotCount--;
            bool wasCollapsed = _collapsedSlotsTable.Contains(slotDeleted);
            if (!wasCollapsed)
            {
                this.VisibleSlotCount--;
            }

            // If we're deleting the focused row, we need to clear the cached value
            if (_focusedRow != null && _focusedRow.DataContext == itemDeleted)
            {
                ResetFocusedRow();
            }

            // The element needs to be potentially removed from the displayed elements
            UIElement elementDeleted = null;
            if (slotDeleted <= this.DisplayData.LastScrollingSlot)
            {
                if ((slotDeleted >= this.DisplayData.FirstScrollingSlot) && !wasCollapsed)
                {
                    elementDeleted = this.DisplayData.GetDisplayedElement(slotDeleted);

                    // Make sure we have the correct height for the calculation below
                    elementDeleted.EnsureMeasured();

                    // We need to retrieve the Element before updating the tables, but we need
                    // to update the tables before updating DisplayData in RemoveDisplayedElement
                    UpdateTablesForRemoval(slotDeleted, itemDeleted);

                    // Displayed row is removed
                    RemoveDisplayedElement(elementDeleted, slotDeleted, true /*wasDeleted*/, true /*updateSlotInformation*/);
                }
                else
                {
                    UpdateTablesForRemoval(slotDeleted, itemDeleted);

                    // Removed row is not in view, just update the DisplayData
                    this.DisplayData.CorrectSlotsAfterDeletion(slotDeleted, wasCollapsed);
                }
            }
            else
            {
                // The element was removed beyond the viewport so we just need to update the tables
                UpdateTablesForRemoval(slotDeleted, itemDeleted);
            }

            // If a row was removed before the currently selected row, update its index
            if (slotDeleted < this.SelectedIndex)
            {
                this.SetValueNoCallback(SelectedIndexProperty, this.SelectedIndex - 1);
            }

            if (!wasCollapsed)
            {
                if (slotDeleted >= this.DisplayData.LastScrollingSlot && elementDeleted == null)
                {
                    // Deleted Row is below our Viewport, we just need to adjust the scrollbar
                    UpdateVerticalScrollBar();
                }
                else
                {
                    double verticalOffsetAdjustment = 0;
                    if (elementDeleted != null)
                    {
                        // Deleted Row is within our Viewport, update the AvailableRowRoom
                        this.AvailableSlotElementRoom += elementDeleted.DesiredSize.Height;

                        // When we delete a row in view, we also need to adjust the verticalOffset
                        // in the cases where the deletion causes us to be scrolled further down than
                        // what is possible.
                        double newVerticalScrollBarMax = _vScrollBar.Maximum - elementDeleted.DesiredSize.Height;
                        if (_verticalOffset > newVerticalScrollBarMax)
                        {
                            verticalOffsetAdjustment = elementDeleted.DesiredSize.Height;
                        }
                    }
                    else
                    {
                        // Deleted element is above our Viewport, update the vertical offset
                        verticalOffsetAdjustment = isRow ? this.RowHeightEstimate : this.RowGroupHeaderHeightEstimate;
                    }

                    if (verticalOffsetAdjustment > 0)
                    {
                        SetVerticalOffset(Math.Max(0, _verticalOffset - verticalOffsetAdjustment));

                        // If we've adjusted the vertical offset so that is less than the amount that the first element
                        // is covered up, we need to uncover the first element appropriately.
                        if (this.NegVerticalOffset > _verticalOffset)
                        {
                            this.NegVerticalOffset = _verticalOffset;
                        }
                    }

                    ComputeScrollBarsLayout();

                    // Reposition rows in case we use a recycled one
                    InvalidateRowsArrange();
                }
            }
        }

        private void OnRemovingElement(int slotDeleted)
        {
            // Note that the row needs to be deleted no matter what. The underlying data row was already deleted.
            DiagnosticsDebug.Assert(slotDeleted >= 0, "Expected positive slotDeleted.");
            DiagnosticsDebug.Assert(slotDeleted < this.SlotCount, "Expected slotDeleted smaller than SlotCount.");
            _temporarilyResetCurrentCell = false;

            // Reset the current cell's address if it's on the deleted row, or after it.
            if (this.CurrentSlot != -1 && slotDeleted <= this.CurrentSlot)
            {
                _desiredCurrentColumnIndex = this.CurrentColumnIndex;
                if (slotDeleted == this.CurrentSlot)
                {
                    // No editing is committed since the underlying entity was already deleted.
                    bool success = SetCurrentCellCore(-1, -1, false /*commitEdit*/, false /*endRowEdit*/);
                    DiagnosticsDebug.Assert(success, "Expected successful SetCurrentCellCore call.");
                }
                else
                {
                    // Underlying data of deleted row is gone. It cannot be accessed anymore. Skip the commit of the editing.
                    _temporarilyResetCurrentCell = true;
                    bool success = SetCurrentCellCore(-1, -1);
                    DiagnosticsDebug.Assert(success, "Expected successful SetCurrentCellCore call.");
                }
            }
        }

        // Makes sure the row shows the proper visuals for selection, currency, details, etc.
        private void LoadRowVisualsForDisplay(DataGridRow row)
        {
            // If the row has been recycled, reapply the BackgroundBrush
            if (row.IsRecycled)
            {
                row.EnsureBackground();
                row.EnsureForeground();
                row.ApplyCellsState(false /*animate*/);
            }
            else if (row == this.EditingRow)
            {
                row.ApplyCellsState(false /*animate*/);
            }

            // Set the Row's Style if we one's defined at the DataGrid level and the user didn't
            // set one at the row level
            EnsureElementStyle(row, null, this.RowStyle);
            row.EnsureHeaderStyleAndVisibility(null);

            // Check to see if the row contains the CurrentCell, apply its state.
            if (this.CurrentColumnIndex != -1 &&
                this.CurrentSlot != -1 &&
                row.Index == this.CurrentSlot)
            {
                row.Cells[this.CurrentColumnIndex].ApplyCellState(false /*animate*/);
            }

            if (row.IsSelected || row.IsRecycled)
            {
                row.ApplyState(false);
            }

            // Show or hide RowDetails based on DataGrid settings
            EnsureRowDetailsVisibility(
                row,
                false /*raiseNotification*/);
        }

        private void PopulateRowGroupHeadersTable()
        {
            if (this.DataConnection.CollectionView != null &&
#if FEATURE_ICOLLECTIONVIEW_GROUP
                this.DataConnection.CollectionView.CanGroup &&
#endif
                this.DataConnection.CollectionView.CollectionGroups != null)
            {
                int totalSlots = 0;
                _topLevelGroup = this.DataConnection.CollectionView.CollectionGroups;
#if FEATURE_ICOLLECTIONVIEW_GROUP
                HookupCollectionChangedListenerToGroup(_topLevelGroup as INotifyCollectionChanged);
#else
                HookupVectorChangedListenerToGroup(_topLevelGroup);
#endif
                foreach (object group in this.DataConnection.CollectionView.CollectionGroups)
                {
                    totalSlots += CountAndPopulateGroupHeaders(group, totalSlots, 0);
                }
            }
#if FEATURE_IEDITABLECOLLECTIONVIEW
            if (this.IsReadOnly && this.DataConnection.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd)
            {
                _collapsedSlotsTable.AddValue(this.SlotFromRowIndex(this.DataConnection.NewItemPlaceholderIndex), Visibility.Collapsed);
            }
#endif
        }

#if FEATURE_ICOLLECTIONVIEW_GROUP
        private void HookupCollectionChangedListenerToGroup(INotifyCollectionChanged incc)
        {
            if (incc != null && !_groupsCollectionChangedListenersTable.ContainsKey(incc))
            {
                WeakEventListener<DataGrid, object, NotifyCollectionChangedEventArgs> weakCollectionChangedListener = new WeakEventListener<DataGrid, object, NotifyCollectionChangedEventArgs>(this);
                weakCollectionChangedListener.OnEventAction = (instance, source, eventArgs) => instance.CollectionViewGroup_CollectionChanged(source, eventArgs);
                weakCollectionChangedListener.OnDetachAction = (weakEventListener) => incc.CollectionChanged -= weakCollectionChangedListener.OnEvent;
                incc.CollectionChanged += weakCollectionChangedListener.OnEvent;

                _groupsCollectionChangedListenersTable.Add(incc, weakCollectionChangedListener);
            }
        }

        private void UnhookCollectionChangedListenerFromGroup(INotifyCollectionChanged incc, bool removeFromTable)
        {
            WeakEventListener<DataGrid, object, NotifyCollectionChangedEventArgs> weakCollectionChangedListener;
            if (incc != null && _groupsCollectionChangedListenersTable.TryGetValue(incc, out weakCollectionChangedListener))
            {
                weakCollectionChangedListener.Detach();
                if (removeFromTable)
                {
                    _groupsCollectionChangedListenersTable.Remove(incc);
                }
            }
        }
#else
        private void HookupVectorChangedListenerToGroup(IObservableVector<object> groupItems)
        {
            if (groupItems != null && !_groupsVectorChangedListenersTable.ContainsKey(groupItems))
            {
                WeakEventListener<DataGrid, object, IVectorChangedEventArgs> weakVectorChangedListener = new WeakEventListener<DataGrid, object, IVectorChangedEventArgs>(this);
                weakVectorChangedListener.OnEventAction = (instance, source, eventArgs) => instance.CollectionViewGroupItems_VectorChanged(source as IObservableVector<object>, eventArgs);
                weakVectorChangedListener.OnDetachAction = (weakEventListener) => groupItems.VectorChanged -= weakVectorChangedListener.OnEvent;
                groupItems.VectorChanged += weakVectorChangedListener.OnEvent;

                _groupsVectorChangedListenersTable.Add(groupItems, weakVectorChangedListener);
            }
        }

        private void UnhookVectorChangedListenerFromGroup(IObservableVector<object> groupItems, bool removeFromTable)
        {
            WeakEventListener<DataGrid, object, IVectorChangedEventArgs> weakVectorChangedListener;
            if (groupItems != null && _groupsVectorChangedListenersTable.TryGetValue(groupItems, out weakVectorChangedListener))
            {
                weakVectorChangedListener.Detach();
                if (removeFromTable)
                {
                    _groupsVectorChangedListenersTable.Remove(groupItems);
                }
            }
        }
#endif

        private void RefreshRowGroupHeaders()
        {
            if (this.DataConnection.CollectionView != null &&
#if FEATURE_ICOLLECTIONVIEW_GROUP
                this.DataConnection.CollectionView.CanGroup &&
#endif
                this.DataConnection.CollectionView.CollectionGroups != null)
            {
                // Initialize our array for the height of the RowGroupHeaders by Level.
                // If the Length is the same, we can reuse the old array.
#if FEATURE_ICOLLECTIONVIEW_GROUP
                int groupLevelCount = this.DataConnection.CollectionView.GroupDescriptions.Count;
#else
                int groupLevelCount = 1;
#endif
                if (_rowGroupHeightsByLevel == null || _rowGroupHeightsByLevel.Length != groupLevelCount)
                {
                    _rowGroupHeightsByLevel = new double[groupLevelCount];
                    for (int i = 0; i < groupLevelCount; i++)
                    {
                        // Default height for now, the actual heights are updated as the RowGroupHeaders
                        // are added and measured
                        _rowGroupHeightsByLevel[i] = DATAGRID_defaultRowHeight;
                    }
                }

                if (this.RowGroupSublevelIndents == null || this.RowGroupSublevelIndents.Length != groupLevelCount)
                {
                    this.RowGroupSublevelIndents = new double[groupLevelCount];
                    double indent;
                    for (int i = 0; i < groupLevelCount; i++)
                    {
                        DataGridRowGroupHeader rowGroupHeader = null;
                        indent = DATAGRID_defaultRowGroupSublevelIndent;
                        if (i < this.RowGroupHeaderStyles.Count && this.RowGroupHeaderStyles[i] != null)
                        {
                            if (rowGroupHeader == null)
                            {
                                rowGroupHeader = new DataGridRowGroupHeader();
                            }

                            rowGroupHeader.Style = this.RowGroupHeaderStyles[i];
                            if (rowGroupHeader.SublevelIndent != DataGrid.DATAGRID_defaultRowGroupSublevelIndent)
                            {
                                indent = rowGroupHeader.SublevelIndent;
                            }
                        }

                        this.RowGroupSublevelIndents[i] = indent;
                        if (i > 0)
                        {
                            this.RowGroupSublevelIndents[i] += this.RowGroupSublevelIndents[i - 1];
                        }
                    }
                }

                EnsureRowGroupSpacerColumnWidth(groupLevelCount);
            }
        }

        private void RefreshSlotCounts()
        {
            this.SlotCount = this.DataConnection.Count;
            this.SlotCount += this.RowGroupHeadersTable.IndexCount;
            this.VisibleSlotCount = this.SlotCount - _collapsedSlotsTable.GetIndexCount(0, this.SlotCount - 1);
        }

        private void RemoveDisplayedElement(int slot, bool wasDeleted, bool updateSlotInformation)
        {
            DiagnosticsDebug.Assert(slot >= this.DisplayData.FirstScrollingSlot, "Expected slot larger or equal to DisplayData.FirstScrollingSlot.");
            DiagnosticsDebug.Assert(slot <= this.DisplayData.LastScrollingSlot, "Expected slot smaller or equal to DisplayData.LastScrollingSlot.");

            RemoveDisplayedElement(this.DisplayData.GetDisplayedElement(slot), slot, wasDeleted, updateSlotInformation);
        }

        // Removes an element from display either because it was deleted or it was scrolled out of view.
        // If the element was provided, it will be the element removed; otherwise, the element will be
        // retrieved from the slot information
        private void RemoveDisplayedElement(UIElement element, int slot, bool wasDeleted, bool updateSlotInformation)
        {
            DataGridRow dataGridRow = element as DataGridRow;
            if (dataGridRow != null)
            {
                if (IsRowRecyclable(dataGridRow))
                {
                    UnloadRow(dataGridRow);
                }
                else
                {
                    dataGridRow.Clip = new RectangleGeometry();
                }
            }
            else
            {
                DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                if (groupHeader != null)
                {
                    OnUnloadingRowGroup(new DataGridRowGroupHeaderEventArgs(groupHeader));
                    this.DisplayData.AddRecylableRowGroupHeader(groupHeader);
                }
                else if (_rowsPresenter != null)
                {
                    _rowsPresenter.Children.Remove(element);
                }
            }

            // Update DisplayData
            if (wasDeleted)
            {
                this.DisplayData.CorrectSlotsAfterDeletion(slot, false /*wasCollapsed*/);
            }
            else
            {
                this.DisplayData.UnloadScrollingElement(slot, updateSlotInformation, false /*wasDeleted*/);
            }
        }

        /// <summary>
        /// Removes all of the editing elements for the row that is just leaving editing mode.
        /// </summary>
        private void RemoveEditingElements()
        {
            if (this.EditingRow != null && this.EditingRow.Cells != null)
            {
                DiagnosticsDebug.Assert(this.EditingRow.Cells.Count == this.ColumnsItemsInternal.Count, "Expected EditingRow.Cells.Count equals ColumnsItemsInternal.Count.");
                foreach (DataGridColumn column in this.Columns)
                {
                    column.RemoveEditingElement();
                }
            }
        }

        private void RemoveElementAt(int slot, object item, bool isRow)
        {
            DiagnosticsDebug.Assert(slot >= 0, "Expected positive slot.");
            DiagnosticsDebug.Assert(slot < this.SlotCount, "Expected slot  smaller than SlotCount.");

            OnRemovingElement(slot);

            CorrectSlotsAfterDeletion(slot, isRow);

            OnRemovedElement(slot, item, isRow);

            // Synchronize CurrentCellCoordinates, CurrentColumn, CurrentColumnIndex, CurrentItem
            // and CurrentSlot with the currently edited cell, since OnRemovingElement called
            // SetCurrentCellCore(-1, -1) to temporarily reset the current cell.
            if (_temporarilyResetCurrentCell &&
                _editingColumnIndex != -1 &&
                _previousCurrentItem != null &&
                this.EditingRow != null &&
                this.EditingRow.Slot != -1)
            {
                ProcessSelectionAndCurrency(
                    columnIndex: _editingColumnIndex,
                    item: _previousCurrentItem,
                    backupSlot: this.EditingRow.Slot,
                    action: DataGridSelectionAction.None,
                    scrollIntoView: false);
            }
        }

        private void RemoveNonDisplayedRows(int newFirstDisplayedSlot, int newLastDisplayedSlot)
        {
            while (this.DisplayData.FirstScrollingSlot < newFirstDisplayedSlot)
            {
                // Need to add rows above the lastDisplayedScrollingRow
                RemoveDisplayedElement(this.DisplayData.FirstScrollingSlot, false /*wasDeleted*/, true /*updateSlotInformation*/);
            }

            while (this.DisplayData.LastScrollingSlot > newLastDisplayedSlot)
            {
                // Need to remove rows below the lastDisplayedScrollingRow
                RemoveDisplayedElement(this.DisplayData.LastScrollingSlot, false /*wasDeleted*/, true /*updateSlotInformation*/);
            }
        }

        private void ResetDisplayedRows()
        {
            if (this.UnloadingRow != null || this.UnloadingRowGroup != null)
            {
                foreach (UIElement element in this.DisplayData.GetScrollingElements())
                {
                    // Raise Unloading Row for all the rows we're displaying
                    DataGridRow row = element as DataGridRow;
                    if (row != null)
                    {
                        if (IsRowRecyclable(row))
                        {
                            OnUnloadingRow(new DataGridRowEventArgs(row));
                        }
                    }
                    else
                    {
                        // Raise Unloading Row for all the RowGroupHeaders we're displaying
                        DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                        if (groupHeader != null)
                        {
                            OnUnloadingRowGroup(new DataGridRowGroupHeaderEventArgs(groupHeader));
                        }
                    }
                }
            }

            this.DisplayData.ClearElements(true /*recycleRows*/);
            this.AvailableSlotElementRoom = this.CellsHeight;
        }

        /// <summary>
        /// Determines whether the row at the provided index must be displayed or not.
        /// </summary>
        /// <returns>True when the slot is displayed.</returns>
        private bool SlotIsDisplayed(int slot)
        {
            DiagnosticsDebug.Assert(slot >= 0, "Expected positive slot.");

            if (slot >= this.DisplayData.FirstScrollingSlot &&
                slot <= this.DisplayData.LastScrollingSlot)
            {
                // Additional row takes the spot of a displayed row - it is necessarily displayed
                return true;
            }
            else if (this.DisplayData.FirstScrollingSlot == -1 &&
                     this.CellsHeight > 0 &&
                     this.CellsWidth > 0)
            {
                return true;
            }
            else if (slot == GetNextVisibleSlot(this.DisplayData.LastScrollingSlot))
            {
                if (this.AvailableSlotElementRoom > 0)
                {
                    // There is room for this additional row
                    return true;
                }
            }

            return false;
        }

        // Updates display information and displayed rows after scrolling the given number of pixels
        private void ScrollSlotsByHeight(double height)
        {
            DiagnosticsDebug.Assert(this.DisplayData.FirstScrollingSlot >= 0, "Expected positive DisplayData.FirstScrollingSlot.");
            DiagnosticsDebug.Assert(!DoubleUtil.IsZero(height), "DoubleUtil.IsZero(height) is false.");

            _scrollingByHeight = true;
            try
            {
                double deltaY = 0;
                int newFirstScrollingSlot = this.DisplayData.FirstScrollingSlot;
                double newVerticalOffset = _verticalOffset + height;
                if (height > 0)
                {
                    // Scrolling Down
                    int lastVisibleSlot = GetPreviousVisibleSlot(this.SlotCount);
                    if (_vScrollBar != null && DoubleUtil.AreClose(_vScrollBar.Maximum, newVerticalOffset))
                    {
                        // We've scrolled to the bottom of the ScrollBar, automatically place the user at the very bottom
                        // of the DataGrid.  If this produces very odd behavior, evaluate the coping strategy used by
                        // OnRowMeasure(Size).  For most data, this should be unnoticeable.
                        ResetDisplayedRows();
                        UpdateDisplayedRowsFromBottom(lastVisibleSlot);
                        newFirstScrollingSlot = this.DisplayData.FirstScrollingSlot;
                    }
                    else
                    {
                        deltaY = GetSlotElementHeight(newFirstScrollingSlot) - this.NegVerticalOffset;
                        if (DoubleUtil.LessThan(height, deltaY))
                        {
                            // We've merely covered up more of the same row we're on
                            this.NegVerticalOffset += height;
                        }
                        else
                        {
                            // Figure out what row we've scrolled down to and update the value for this.NegVerticalOffset
                            this.NegVerticalOffset = 0;

                            if (height > 2 * this.CellsHeight &&
                                (this.RowDetailsVisibilityMode != DataGridRowDetailsVisibilityMode.VisibleWhenSelected || this.RowDetailsTemplate == null))
                            {
                                // Very large scroll occurred. Instead of determining the exact number of scrolled off rows,
                                // let's estimate the number based on this.RowHeight.
                                ResetDisplayedRows();
                                double singleRowHeightEstimate = this.RowHeightEstimate + (this.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Visible ? this.RowDetailsHeightEstimate : 0);
                                int scrolledToSlot = newFirstScrollingSlot + (int)(height / singleRowHeightEstimate);
                                scrolledToSlot += _collapsedSlotsTable.GetIndexCount(newFirstScrollingSlot, newFirstScrollingSlot + scrolledToSlot);
                                newFirstScrollingSlot = Math.Min(GetNextVisibleSlot(scrolledToSlot), lastVisibleSlot);
                            }
                            else
                            {
                                while (DoubleUtil.LessThanOrClose(deltaY, height))
                                {
                                    if (newFirstScrollingSlot < lastVisibleSlot)
                                    {
                                        if (this.IsSlotVisible(newFirstScrollingSlot))
                                        {
                                            // Make the top row available for reuse
                                            RemoveDisplayedElement(newFirstScrollingSlot, false /*wasDeleted*/, true /*updateSlotInformation*/);
                                        }

                                        newFirstScrollingSlot = GetNextVisibleSlot(newFirstScrollingSlot);
                                    }
                                    else
                                    {
                                        // We're being told to scroll beyond the last row, ignore the extra
                                        this.NegVerticalOffset = 0;
                                        break;
                                    }

                                    double rowHeight = GetExactSlotElementHeight(newFirstScrollingSlot);
                                    double remainingHeight = height - deltaY;
                                    if (DoubleUtil.LessThanOrClose(rowHeight, remainingHeight))
                                    {
                                        deltaY += rowHeight;
                                    }
                                    else
                                    {
                                        this.NegVerticalOffset = remainingHeight;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Scrolling Up
                    if (DoubleUtil.GreaterThanOrClose(height + this.NegVerticalOffset, 0))
                    {
                        // We've merely exposing more of the row we're on
                        this.NegVerticalOffset += height;
                    }
                    else
                    {
                        // Figure out what row we've scrolled up to and update the value for this.NegVerticalOffset
                        deltaY = -this.NegVerticalOffset;
                        this.NegVerticalOffset = 0;

                        if (height < -2 * this.CellsHeight &&
                            (this.RowDetailsVisibilityMode != DataGridRowDetailsVisibilityMode.VisibleWhenSelected || this.RowDetailsTemplate == null))
                        {
                            // Very large scroll occurred. Instead of determining the exact number of scrolled off rows,
                            // let's estimate the number based on this.RowHeight.
                            if (newVerticalOffset == 0)
                            {
                                newFirstScrollingSlot = 0;
                            }
                            else
                            {
                                double singleRowHeightEstimate = this.RowHeightEstimate + (this.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Visible ? this.RowDetailsHeightEstimate : 0);
                                int scrolledToSlot = newFirstScrollingSlot + (int)(height / singleRowHeightEstimate);
                                scrolledToSlot -= _collapsedSlotsTable.GetIndexCount(scrolledToSlot, newFirstScrollingSlot);

                                newFirstScrollingSlot = Math.Max(0, GetPreviousVisibleSlot(scrolledToSlot + 1));
                            }

                            ResetDisplayedRows();
                        }
                        else
                        {
                            int lastScrollingSlot = this.DisplayData.LastScrollingSlot;
                            while (DoubleUtil.GreaterThan(deltaY, height))
                            {
                                if (newFirstScrollingSlot > 0)
                                {
                                    if (this.IsSlotVisible(lastScrollingSlot))
                                    {
                                        // Make the bottom row available for reuse
                                        RemoveDisplayedElement(lastScrollingSlot, false /*wasDeleted*/, true /*updateSlotInformation*/);
                                        lastScrollingSlot = GetPreviousVisibleSlot(lastScrollingSlot);
                                    }

                                    newFirstScrollingSlot = GetPreviousVisibleSlot(newFirstScrollingSlot);
                                }
                                else
                                {
                                    this.NegVerticalOffset = 0;
                                    break;
                                }

                                double rowHeight = GetExactSlotElementHeight(newFirstScrollingSlot);
                                double remainingHeight = height - deltaY;
                                if (DoubleUtil.LessThanOrClose(rowHeight + remainingHeight, 0))
                                {
                                    deltaY -= rowHeight;
                                }
                                else
                                {
                                    this.NegVerticalOffset = rowHeight + remainingHeight;
                                    break;
                                }
                            }
                        }
                    }

                    if (DoubleUtil.GreaterThanOrClose(0, newVerticalOffset) && newFirstScrollingSlot != 0)
                    {
                        // We've scrolled to the top of the ScrollBar, automatically place the user at the very top
                        // of the DataGrid.  If this produces very odd behavior, evaluate the RowHeight estimate.
                        // strategy. For most data, this should be unnoticeable.
                        ResetDisplayedRows();
                        this.NegVerticalOffset = 0;
                        UpdateDisplayedRows(0, this.CellsHeight);
                        newFirstScrollingSlot = 0;
                    }
                }

                double firstRowHeight = GetExactSlotElementHeight(newFirstScrollingSlot);
                if (DoubleUtil.LessThan(firstRowHeight, this.NegVerticalOffset))
                {
                    // We've scrolled off more of the first row than what's possible.  This can happen
                    // if the first row got shorter (Ex: Collapsing RowDetails) or if the user has a recycling
                    // cleanup issue.  In this case, simply try to display the next row as the first row instead
                    if (newFirstScrollingSlot < this.SlotCount - 1)
                    {
                        newFirstScrollingSlot = GetNextVisibleSlot(newFirstScrollingSlot);
                        DiagnosticsDebug.Assert(newFirstScrollingSlot != -1, "Expected newFirstScrollingSlot other than -1.");
                    }

                    this.NegVerticalOffset = 0;
                }

                UpdateDisplayedRows(newFirstScrollingSlot, this.CellsHeight);

                double firstElementHeight = GetExactSlotElementHeight(this.DisplayData.FirstScrollingSlot);
                if (DoubleUtil.GreaterThan(this.NegVerticalOffset, firstElementHeight))
                {
                    int firstElementSlot = this.DisplayData.FirstScrollingSlot;

                    // We filled in some rows at the top and now we have a NegVerticalOffset that's greater than the first element
                    while (newFirstScrollingSlot > 0 && DoubleUtil.GreaterThan(this.NegVerticalOffset, firstElementHeight))
                    {
                        int previousSlot = GetPreviousVisibleSlot(firstElementSlot);
                        if (previousSlot == -1)
                        {
                            this.NegVerticalOffset = 0;
                            VerticalOffset = 0;
                        }
                        else
                        {
                            this.NegVerticalOffset -= firstElementHeight;
                            VerticalOffset = Math.Max(0, _verticalOffset - firstElementHeight);
                            firstElementSlot = previousSlot;
                            firstElementHeight = GetExactSlotElementHeight(firstElementSlot);
                        }
                    }

                    // We could be smarter about this, but it's not common so we wouldn't gain much from optimizing here
                    if (firstElementSlot != this.DisplayData.FirstScrollingSlot)
                    {
                        UpdateDisplayedRows(firstElementSlot, this.CellsHeight);
                    }
                }

                DiagnosticsDebug.Assert(this.DisplayData.FirstScrollingSlot >= 0, "Expected positive DisplayData.FirstScrollingSlot.");
                DiagnosticsDebug.Assert(GetExactSlotElementHeight(this.DisplayData.FirstScrollingSlot) > this.NegVerticalOffset, "Expected GetExactSlotElementHeight(DisplayData.FirstScrollingSlot) larger than this.NegVerticalOffset.");

                if (this.DisplayData.FirstScrollingSlot == 0)
                {
                    VerticalOffset = this.NegVerticalOffset;
                }
                else if (DoubleUtil.GreaterThan(this.NegVerticalOffset, newVerticalOffset))
                {
                    // The scrolled-in row was larger than anticipated. Adjust the DataGrid so the ScrollBar thumb
                    // can stay in the same place
                    this.NegVerticalOffset = newVerticalOffset;
                    VerticalOffset = newVerticalOffset;
                }
                else
                {
                    VerticalOffset = newVerticalOffset;
                }

                DiagnosticsDebug.Assert(
                    _verticalOffset != 0 || this.NegVerticalOffset != 0 || this.DisplayData.FirstScrollingSlot <= 0,
                    "Expected _verticalOffset other than 0 or this.NegVerticalOffset other than 0 or this.DisplayData.FirstScrollingSlot smaller than or equal to 0.");

                SetVerticalOffset(_verticalOffset);

                this.DisplayData.FullyRecycleElements();

                DiagnosticsDebug.Assert(DoubleUtil.GreaterThanOrClose(this.NegVerticalOffset, 0), "Expected NegVerticalOffset greater than or close to 0.");
                DiagnosticsDebug.Assert(DoubleUtil.GreaterThanOrClose(_verticalOffset, this.NegVerticalOffset), "Expected _verticalOffset greater than or close to NegVerticalOffset.");

                DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseAutomationScrollEvents();
                }
            }
            finally
            {
                _scrollingByHeight = false;
            }
        }

        private void SelectDisplayedElement(int slot)
        {
            DiagnosticsDebug.Assert(IsSlotVisible(slot), "Expected IsSlotVisible(slot) is true.");
            FrameworkElement element = this.DisplayData.GetDisplayedElement(slot) as FrameworkElement;
            DataGridRow row = this.DisplayData.GetDisplayedElement(slot) as DataGridRow;
            if (row != null)
            {
                row.ApplyState(true /*animate*/);
                EnsureRowDetailsVisibility(
                    row,
                    true /*raiseNotification*/);
            }
            else
            {
                // Assume it's a RowGroupHeader.
                DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                groupHeader.ApplyState(true /*useTransitions*/);
            }
        }

        private void SelectSlot(int slot, bool isSelected)
        {
            _selectedItems.SelectSlot(slot, isSelected);
            if (this.IsSlotVisible(slot))
            {
                SelectDisplayedElement(slot);
            }
        }

        private void SelectSlots(int startSlot, int endSlot, bool isSelected)
        {
            _selectedItems.SelectSlots(startSlot, endSlot, isSelected);

            // Apply the correct row state for display rows and also expand or collapse detail accordingly
            int firstSlot = Math.Max(this.DisplayData.FirstScrollingSlot, startSlot);
            int lastSlot = Math.Min(this.DisplayData.LastScrollingSlot, endSlot);

            for (int slot = firstSlot; slot <= lastSlot; slot++)
            {
                if (IsSlotVisible(slot))
                {
                    SelectDisplayedElement(slot);
                }
            }
        }

        private bool ToggleRowGroup()
        {
            if (!this.ColumnHeaderHasFocus && this.FirstVisibleSlot != -1 && this.RowGroupHeadersTable.Contains(this.CurrentSlot))
            {
                ICollectionViewGroup collectionViewGroup = this.RowGroupHeadersTable.GetValueAt(this.CurrentSlot).CollectionViewGroup;

                if (collectionViewGroup != null)
                {
                    DataGridRowGroupInfo dataGridRowGroupInfo = RowGroupInfoFromCollectionViewGroup(collectionViewGroup);

                    if (dataGridRowGroupInfo != null)
                    {
                        if (dataGridRowGroupInfo.Visibility == Visibility.Collapsed)
                        {
                            ExpandRowGroup(collectionViewGroup, false /*expandAllSubgroups*/);
                        }
                        else
                        {
                            CollapseRowGroup(collectionViewGroup, false /*collapseAllSubgroups*/);
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        private void UnloadElements(bool recycle)
        {
            // Since we're unloading all the elements, we can't be in editing mode anymore,
            // so commit if we can, otherwise force cancel.
            if (!this.CommitEdit())
            {
                this.CancelEdit(DataGridEditingUnit.Row, false);
            }

            this.ResetEditingRow();

            // Make sure to clear the focused row (because it's no longer relevant).
            if (_focusedRow != null)
            {
                ResetFocusedRow();
                Focus(FocusState.Programmatic);
            }

            if (_rowsPresenter != null)
            {
                foreach (UIElement element in _rowsPresenter.Children)
                {
                    DataGridRow row = element as DataGridRow;
                    if (row != null)
                    {
                        // Raise UnloadingRow for any row that was visible
                        if (this.IsSlotVisible(row.Slot))
                        {
                            OnUnloadingRow(new DataGridRowEventArgs(row));
                        }

                        row.DetachFromDataGrid(recycle && row.IsRecyclable /*recycle*/);
                    }
                    else
                    {
                        DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                        if (groupHeader != null && this.IsSlotVisible(groupHeader.RowGroupInfo.Slot))
                        {
                            OnUnloadingRowGroup(new DataGridRowGroupHeaderEventArgs(groupHeader));
                        }
                    }
                }

                if (!recycle)
                {
                    _rowsPresenter.Children.Clear();
                }
            }

            this.DisplayData.ClearElements(recycle);

            // Update the AvailableRowRoom since we're displaying 0 rows now
            this.AvailableSlotElementRoom = this.CellsHeight;
            this.VisibleSlotCount = 0;
        }

        private void UnloadRow(DataGridRow dataGridRow)
        {
            DiagnosticsDebug.Assert(dataGridRow != null, "Expected non-null dataGridRow.");
            DiagnosticsDebug.Assert(_rowsPresenter != null, "Expected non-null _rowsPresenter.");
            DiagnosticsDebug.Assert(_rowsPresenter.Children.Contains(dataGridRow), "Expected dataGridRow contained in _rowsPresenter.Children.");

            if (_loadedRows.Contains(dataGridRow))
            {
                return; // The row is still referenced, we can't release it.
            }

            // Raise UnloadingRow regardless of whether the row will be recycled
            OnUnloadingRow(new DataGridRowEventArgs(dataGridRow));

            // TODO: Be able to recycle the current row
            bool recycleRow = this.CurrentSlot != dataGridRow.Index;

            // Don't recycle if the row has a custom Style set
            recycleRow &= dataGridRow.Style == null || dataGridRow.Style == this.RowStyle;

            if (recycleRow)
            {
                this.DisplayData.AddRecylableRow(dataGridRow);
            }
            else
            {
                // TODO: Should we raise Unloading for rows that are not going to be recycled as well
                _rowsPresenter.Children.Remove(dataGridRow);
                dataGridRow.DetachFromDataGrid(false);
            }
        }

        private void UpdateDisplayedRows(int newFirstDisplayedSlot, double displayHeight)
        {
            DiagnosticsDebug.Assert(!_collapsedSlotsTable.Contains(newFirstDisplayedSlot), "Expected newFirstDisplayedSlot not contained in _collapsedSlotsTable.");

            int firstDisplayedScrollingSlot = newFirstDisplayedSlot;
            int lastDisplayedScrollingSlot = -1;
            double deltaY = -this.NegVerticalOffset;
            int visibleScrollingRows = 0;

            if (DoubleUtil.LessThanOrClose(displayHeight, 0) || this.SlotCount == 0 || this.ColumnsItemsInternal.Count == 0)
            {
                return;
            }

            if (firstDisplayedScrollingSlot == -1)
            {
                // 0 is fine because the element in the first slot cannot be collapsed
                firstDisplayedScrollingSlot = 0;
            }

            int slot = firstDisplayedScrollingSlot;
            while (slot < this.SlotCount && !DoubleUtil.GreaterThanOrClose(deltaY, displayHeight))
            {
                deltaY += GetExactSlotElementHeight(slot);
                visibleScrollingRows++;
                lastDisplayedScrollingSlot = slot;
                slot = GetNextVisibleSlot(slot);
            }

            while (DoubleUtil.LessThan(deltaY, displayHeight) && slot >= 0)
            {
                slot = GetPreviousVisibleSlot(firstDisplayedScrollingSlot);
                if (slot >= 0)
                {
                    deltaY += GetExactSlotElementHeight(slot);
                    firstDisplayedScrollingSlot = slot;
                    visibleScrollingRows++;
                }
            }

            // If we're up to the first row, and we still have room left, uncover as much of the first row as we can
            if (firstDisplayedScrollingSlot == 0 && DoubleUtil.LessThan(deltaY, displayHeight))
            {
                double newNegVerticalOffset = Math.Max(0, this.NegVerticalOffset - displayHeight + deltaY);
                deltaY += this.NegVerticalOffset - newNegVerticalOffset;
                this.NegVerticalOffset = newNegVerticalOffset;
            }

            if (DoubleUtil.GreaterThan(deltaY, displayHeight) || (DoubleUtil.AreClose(deltaY, displayHeight) && DoubleUtil.GreaterThan(this.NegVerticalOffset, 0)))
            {
                this.DisplayData.NumTotallyDisplayedScrollingElements = visibleScrollingRows - 1;
            }
            else
            {
                this.DisplayData.NumTotallyDisplayedScrollingElements = visibleScrollingRows;
            }

            if (visibleScrollingRows == 0)
            {
                firstDisplayedScrollingSlot = -1;
                DiagnosticsDebug.Assert(lastDisplayedScrollingSlot == -1, "Expected lastDisplayedScrollingSlot equal to -1.");
            }

            DiagnosticsDebug.Assert(lastDisplayedScrollingSlot < this.SlotCount, "lastDisplayedScrollingRow larger than number of rows");

            RemoveNonDisplayedRows(firstDisplayedScrollingSlot, lastDisplayedScrollingSlot);

            DiagnosticsDebug.Assert(this.DisplayData.NumDisplayedScrollingElements >= 0, "the number of visible scrolling rows can't be negative");
            DiagnosticsDebug.Assert(this.DisplayData.NumTotallyDisplayedScrollingElements >= 0, "the number of totally visible scrolling rows can't be negative");
            DiagnosticsDebug.Assert(this.DisplayData.FirstScrollingSlot < this.SlotCount, "firstDisplayedScrollingRow larger than number of rows");
            DiagnosticsDebug.Assert(this.DisplayData.FirstScrollingSlot == firstDisplayedScrollingSlot, "Expected DisplayData.FirstScrollingSlot equal to firstDisplayedScrollingSlot.");
            DiagnosticsDebug.Assert(this.DisplayData.LastScrollingSlot == lastDisplayedScrollingSlot, "DisplayData.LastScrollingSlot equal to lastDisplayedScrollingSlot.");
        }

        // Similar to UpdateDisplayedRows except that it starts with the LastDisplayedScrollingRow
        // and computes the FirstDisplayScrollingRow instead of doing it the other way around.  We use this
        // when scrolling down to a full row
        private void UpdateDisplayedRowsFromBottom(int newLastDisplayedScrollingRow)
        {
            DiagnosticsDebug.Assert(!_collapsedSlotsTable.Contains(newLastDisplayedScrollingRow), "Expected newLastDisplayedScrollingRow not contained in _collapsedSlotsTable.");

            int lastDisplayedScrollingRow = newLastDisplayedScrollingRow;
            int firstDisplayedScrollingRow = -1;
            double displayHeight = this.CellsHeight;
            double deltaY = 0;
            int visibleScrollingRows = 0;

            if (DoubleUtil.LessThanOrClose(displayHeight, 0) || this.SlotCount == 0 || this.ColumnsItemsInternal.Count == 0)
            {
                this.ResetDisplayedRows();
                return;
            }

            if (lastDisplayedScrollingRow == -1)
            {
                lastDisplayedScrollingRow = 0;
            }

            int slot = lastDisplayedScrollingRow;
            while (DoubleUtil.LessThan(deltaY, displayHeight) && slot >= 0)
            {
                deltaY += GetExactSlotElementHeight(slot);
                visibleScrollingRows++;
                firstDisplayedScrollingRow = slot;
                slot = GetPreviousVisibleSlot(slot);
            }

            this.DisplayData.NumTotallyDisplayedScrollingElements = deltaY > displayHeight ? visibleScrollingRows - 1 : visibleScrollingRows;

            DiagnosticsDebug.Assert(this.DisplayData.NumTotallyDisplayedScrollingElements >= 0, "Expected positive DisplayData.NumTotallyDisplayedScrollingElements.");
            DiagnosticsDebug.Assert(lastDisplayedScrollingRow < this.SlotCount, "lastDisplayedScrollingRow larger than number of rows");

            this.NegVerticalOffset = Math.Max(0, deltaY - displayHeight);

            RemoveNonDisplayedRows(firstDisplayedScrollingRow, lastDisplayedScrollingRow);

            DiagnosticsDebug.Assert(this.DisplayData.NumDisplayedScrollingElements >= 0, "the number of visible scrolling rows can't be negative");
            DiagnosticsDebug.Assert(this.DisplayData.NumTotallyDisplayedScrollingElements >= 0, "the number of totally visible scrolling rows can't be negative");
            DiagnosticsDebug.Assert(this.DisplayData.FirstScrollingSlot < this.SlotCount, "firstDisplayedScrollingRow larger than number of rows");
        }

        private void UpdateRowDetailsHeightEstimate()
        {
            if (_rowsPresenter != null && _measured && this.RowDetailsTemplate != null)
            {
                FrameworkElement detailsContent = this.RowDetailsTemplate.LoadContent() as FrameworkElement;
                if (detailsContent != null)
                {
                    if (this.VisibleSlotCount > 0)
                    {
                        detailsContent.DataContext = this.DataConnection.GetDataItem(0);
                    }

                    _rowsPresenter.Children.Add(detailsContent);

                    detailsContent.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    this.RowDetailsHeightEstimate = detailsContent.DesiredSize.Height;
                    _rowsPresenter.Children.Remove(detailsContent);
                }
            }
        }

        // This method does not check the state of the parent RowGroupHeaders, it assumes they're ready for this newVisibility to
        // be applied this header.
        // Returns the number of pixels that were expanded or (collapsed); however, if we're expanding displayed rows, we only expand up
        // to what we can display.
        private double UpdateRowGroupVisibility(DataGridRowGroupInfo targetRowGroupInfo, Visibility newVisibility, bool isHeaderDisplayed)
        {
            double heightChange = 0;
            int slotsExpanded = 0;
            int startSlot = targetRowGroupInfo.Slot + 1;
            int endSlot;

            targetRowGroupInfo.Visibility = newVisibility;
            if (newVisibility == Visibility.Visible)
            {
                // Expand
                foreach (int slot in this.RowGroupHeadersTable.GetIndexes(targetRowGroupInfo.Slot + 1))
                {
                    if (slot >= startSlot)
                    {
                        DataGridRowGroupInfo rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(slot);
                        if (rowGroupInfo.Level <= targetRowGroupInfo.Level)
                        {
                            break;
                        }

                        if (rowGroupInfo.Visibility == Visibility.Collapsed)
                        {
                            // Skip over the items in collapsed subgroups
                            endSlot = rowGroupInfo.Slot;
                            ExpandSlots(startSlot, endSlot, isHeaderDisplayed, ref slotsExpanded, ref heightChange);
                            startSlot = rowGroupInfo.LastSubItemSlot + 1;
                        }
                    }
                }

                if (targetRowGroupInfo.LastSubItemSlot >= startSlot)
                {
                    ExpandSlots(startSlot, targetRowGroupInfo.LastSubItemSlot, isHeaderDisplayed, ref slotsExpanded, ref heightChange);
                }

                if (isHeaderDisplayed)
                {
                    UpdateDisplayedRows(this.DisplayData.FirstScrollingSlot, this.CellsHeight);
                }
            }
            else
            {
                // Collapse
                endSlot = this.SlotCount - 1;
#if FEATURE_IEDITABLECOLLECTIONVIEW
                if (this.DataConnection.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd)
                {
                    endSlot--;
                }
#endif

                if (this.DataConnection.IsAddingNew)
                {
                    endSlot--;
                }

                DiagnosticsDebug.Assert(endSlot >= 0, "Expected positive endSlot.");
                foreach (int slot in this.RowGroupHeadersTable.GetIndexes(targetRowGroupInfo.Slot + 1))
                {
                    DataGridRowGroupInfo rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(slot);
                    if (rowGroupInfo.Level <= targetRowGroupInfo.Level)
                    {
                        endSlot = slot - 1;
                        break;
                    }
                }

                int oldLastDisplayedSlot = this.DisplayData.LastScrollingSlot;

                // onlyChildrenDisplayed is true if the RowGroupHeader is not displayed but some of its children are
                bool onlyChildrenDisplayed = !isHeaderDisplayed && (endSlot >= this.DisplayData.FirstScrollingSlot);
                double partialGroupDisplayedHeight = 0;
                if (isHeaderDisplayed || onlyChildrenDisplayed)
                {
                    // If the RowGroupHeader is displayed or its children are partially displayed,
                    // we need to remove all the displayed slots that aren't already collapsed
                    int startDisplayedSlot = Math.Max(startSlot, this.DisplayData.FirstScrollingSlot);
                    int endDisplayedSlot = Math.Min(endSlot, this.DisplayData.LastScrollingSlot);

                    int elementsToRemove = endDisplayedSlot - startDisplayedSlot + 1 - _collapsedSlotsTable.GetIndexCount(startDisplayedSlot, endDisplayedSlot);
                    if (_focusedRow != null && _focusedRow.Slot >= startSlot && _focusedRow.Slot <= endSlot)
                    {
                        DiagnosticsDebug.Assert(this.EditingRow == null, "Expected null EditingRow.");

                        // Don't call ResetFocusedRow here because we're already cleaning it up below, and we don't want to FullyRecycle yet
                        _focusedRow = null;
                    }

                    for (int i = 0; i < elementsToRemove; i++)
                    {
                        UIElement displayedElement = this.DisplayData.GetDisplayedElement(startDisplayedSlot);
                        displayedElement.EnsureMeasured();

                        // For partially displayed groups, we need to update the slot information right away.  For groups
                        // where the RowGroupHeader is displayed, we can just mark them collapsed later.
                        RemoveDisplayedElement(displayedElement, startDisplayedSlot, false /*wasDeleted*/, onlyChildrenDisplayed /*updateSlotInformation*/);
                        if (onlyChildrenDisplayed)
                        {
                            startDisplayedSlot = this.DisplayData.FirstScrollingSlot;
                            partialGroupDisplayedHeight += displayedElement.DesiredSize.Height;
                        }
                    }

                    if (onlyChildrenDisplayed)
                    {
                        partialGroupDisplayedHeight -= this.NegVerticalOffset;
                    }
                }

                // If part of the group we collapsed was partially displayed, we only collapsed the amount that was not displayed.
                heightChange += partialGroupDisplayedHeight;

                double heightChangeBelowLastDisplayedSlot = 0;
                if (this.DisplayData.FirstScrollingSlot >= startSlot && this.DisplayData.FirstScrollingSlot <= endSlot)
                {
                    // Our first visible slot was collapsed, find the replacement
                    int collapsedSlotsAbove = this.DisplayData.FirstScrollingSlot - startSlot - _collapsedSlotsTable.GetIndexCount(startSlot, this.DisplayData.FirstScrollingSlot);
                    DiagnosticsDebug.Assert(collapsedSlotsAbove > 0, "Expected positive collapsedSlotsAbove.");
                    int newFirstScrollingSlot = GetNextVisibleSlot(this.DisplayData.FirstScrollingSlot);
                    while (collapsedSlotsAbove > 1 && newFirstScrollingSlot < this.SlotCount)
                    {
                        collapsedSlotsAbove--;
                        newFirstScrollingSlot = GetNextVisibleSlot(newFirstScrollingSlot);
                    }

                    heightChange += CollapseSlotsInTable(startSlot, endSlot, ref slotsExpanded, oldLastDisplayedSlot, ref heightChangeBelowLastDisplayedSlot);
                    if (isHeaderDisplayed || onlyChildrenDisplayed)
                    {
                        if (newFirstScrollingSlot >= this.SlotCount)
                        {
                            // No visible slots below, look up
                            UpdateDisplayedRowsFromBottom(targetRowGroupInfo.Slot);
                        }
                        else
                        {
                            UpdateDisplayedRows(newFirstScrollingSlot, this.CellsHeight);
                        }
                    }
                }
                else
                {
                    heightChange += CollapseSlotsInTable(startSlot, endSlot, ref slotsExpanded, oldLastDisplayedSlot, ref heightChangeBelowLastDisplayedSlot);
                }

                if (this.DisplayData.LastScrollingSlot >= startSlot && this.DisplayData.LastScrollingSlot <= endSlot)
                {
                    // Collapsed the last scrolling row, we need to update it
                    this.DisplayData.LastScrollingSlot = GetPreviousVisibleSlot(this.DisplayData.LastScrollingSlot);
                }

                // Collapsing could cause the vertical offset to move up if we collapsed a lot of slots
                // near the bottom of the DataGrid.  To do this, we compare the height we collapsed to
                // the distance to the last visible row and adjust the scrollbar if we collapsed more
                if (isHeaderDisplayed && _verticalOffset > 0)
                {
                    int lastVisibleSlot = GetPreviousVisibleSlot(this.SlotCount);
                    int slot = GetNextVisibleSlot(oldLastDisplayedSlot);

                    // AvailableSlotElementRoom ends up being the amount of the last slot that is partially scrolled off
                    // as a negative value, heightChangeBelowLastDisplayed slot is also a negative value since we're collapsing
                    double heightToLastVisibleSlot = this.AvailableSlotElementRoom + heightChangeBelowLastDisplayedSlot;
                    while ((heightToLastVisibleSlot > heightChange) && (slot < lastVisibleSlot))
                    {
                        heightToLastVisibleSlot -= GetSlotElementHeight(slot);
                        slot = GetNextVisibleSlot(slot);
                    }

                    if (heightToLastVisibleSlot > heightChange)
                    {
                        double newVerticalOffset = _verticalOffset + heightChange - heightToLastVisibleSlot;
                        if (newVerticalOffset > 0)
                        {
                            SetVerticalOffset(newVerticalOffset);
                        }
                        else
                        {
                            // Collapsing causes the vertical offset to go to 0 so we should go back to the first row.
                            ResetDisplayedRows();
                            this.NegVerticalOffset = 0;
                            SetVerticalOffset(0);
                            int firstDisplayedRow = GetNextVisibleSlot(-1);
                            UpdateDisplayedRows(firstDisplayedRow, this.CellsHeight);
                        }
                    }
                }
            }

            // Update VisibleSlotCount
            this.VisibleSlotCount += slotsExpanded;

            return heightChange;
        }

        private void UpdateTablesForRemoval(int slotDeleted, object itemDeleted)
        {
            if (this.RowGroupHeadersTable.Contains(slotDeleted))
            {
                // A RowGroupHeader was removed
                this.RowGroupHeadersTable.RemoveIndexAndValue(slotDeleted);
                _collapsedSlotsTable.RemoveIndexAndValue(slotDeleted);
                _selectedItems.DeleteSlot(slotDeleted);
            }
            else
            {
                // Update the ranges of selected rows
                if (_selectedItems.ContainsSlot(slotDeleted))
                {
                    this.SelectionHasChanged = true;
                }

                _selectedItems.Delete(slotDeleted, itemDeleted);
                _showDetailsTable.RemoveIndex(RowIndexFromSlot(slotDeleted));
                this.RowGroupHeadersTable.RemoveIndex(slotDeleted);
                _collapsedSlotsTable.RemoveIndex(slotDeleted);
            }
        }

#if DEBUG
        internal void PrintRowGroupInfo()
        {
            Debug.WriteLine("-----------------------------------------------RowGroupHeaders");
            foreach (int slot in this.RowGroupHeadersTable.GetIndexes())
            {
                DataGridRowGroupInfo info = this.RowGroupHeadersTable.GetValueAt(slot);
#if FEATURE_ICOLLECTIONVIEW_GROUP
                Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} {1} Slot:{2} Last:{3} Level:{4}", info.CollectionViewGroup.Name, info.Visibility.ToString(), slot, info.LastSubItemSlot, info.Level));
#else
                Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} Slot:{1} Last:{2} Level:{3}", info.Visibility.ToString(), slot, info.LastSubItemSlot, info.Level));
#endif
            }

            Debug.WriteLine("-----------------------------------------------CollapsedSlots");
            _collapsedSlotsTable.PrintIndexes();
        }
#endif
    }
}