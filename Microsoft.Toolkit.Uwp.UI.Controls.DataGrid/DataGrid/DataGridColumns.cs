// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Toolkit.Uwp.UI.Controls.DataGridInternals;
using Microsoft.Toolkit.Uwp.UI.Utilities;
using Microsoft.Toolkit.Uwp.Utilities;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

using DiagnosticsDebug = System.Diagnostics.Debug;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Control to represent data in columns and rows.
    /// </summary>
    public partial class DataGrid
    {
        /// <summary>
        /// OnColumnDisplayIndexChanged
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnColumnDisplayIndexChanged(DataGridColumnEventArgs e)
        {
            this.ColumnDisplayIndexChanged?.Invoke(this, e);
        }

        /// <summary>
        /// OnColumnReordered
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected internal virtual void OnColumnReordered(DataGridColumnEventArgs e)
        {
            this.EnsureVerticalGridLines();

            this.ColumnReordered?.Invoke(this, e);
        }

        /// <summary>
        /// OnColumnReordering
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected internal virtual void OnColumnReordering(DataGridColumnReorderingEventArgs e)
        {
            this.ColumnReordering?.Invoke(this, e);
        }

        /// <summary>
        /// OnColumnSorting
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected internal virtual void OnColumnSorting(DataGridColumnEventArgs e)
        {
            this.Sorting?.Invoke(this, e);
        }

        // Returns the column's width
        internal static double GetEdgedColumnWidth(DataGridColumn dataGridColumn)
        {
            DiagnosticsDebug.Assert(dataGridColumn != null, "Expected non-null dataGridColumn.");
            return dataGridColumn.ActualWidth;
        }

        /// <summary>
        /// Adjusts the widths of all columns with DisplayIndex >= displayIndex such that the total
        /// width is adjusted by the given amount, if possible.  If the total desired adjustment amount
        /// could not be met, the remaining amount of adjustment is returned.
        /// </summary>
        /// <param name="displayIndex">Starting column DisplayIndex.</param>
        /// <param name="amount">Adjustment amount (positive for increase, negative for decrease).</param>
        /// <param name="userInitiated">Whether or not this adjustment was initiated by a user action.</param>
        /// <returns>The remaining amount of adjustment.</returns>
        internal double AdjustColumnWidths(int displayIndex, double amount, bool userInitiated)
        {
            if (!DoubleUtil.IsZero(amount))
            {
                if (amount < 0)
                {
                    amount = DecreaseColumnWidths(displayIndex, amount, userInitiated);
                }
                else
                {
                    amount = IncreaseColumnWidths(displayIndex, amount, userInitiated);
                }
            }

            return amount;
        }

        /// <summary>
        /// Grows an auto-column's width to the desired width.
        /// </summary>
        /// <param name="column">Auto-column to adjust.</param>
        /// <param name="desiredWidth">The new desired width of the column.</param>
        internal void AutoSizeColumn(DataGridColumn column, double desiredWidth)
        {
            DiagnosticsDebug.Assert(
                column.Width.IsAuto || column.Width.IsSizeToCells || column.Width.IsSizeToHeader || (!this.UsesStarSizing && column.Width.IsStar),
                "Expected column.Width.IsAuto or column.Width.IsSizeToCells or column.Width.IsSizeToHeader or (!UsesStarSizing && column.Width.IsStar).");

            // If we're using star sizing and this is the first time we've measured this particular auto-column,
            // we want to allow all rows to get measured before we setup the star widths.  We won't know the final
            // desired value of the column until all rows have been measured.  Because of this, we wait until
            // an Arrange occurs before we adjust star widths.
            if (this.UsesStarSizing && !column.IsInitialDesiredWidthDetermined)
            {
                this.AutoSizingColumns = true;
            }

            // Update the column's DesiredValue if it needs to grow to fit the new desired value
            if (desiredWidth > column.Width.DesiredValue || double.IsNaN(column.Width.DesiredValue))
            {
                // If this auto-growth occurs after the column's initial desired width has been determined,
                // then the growth should act like a resize (squish columns to the right).  Otherwise, if
                // this column is newly added, we'll just set its display value directly.
                if (this.UsesStarSizing && column.IsInitialDesiredWidthDetermined)
                {
                    column.Resize(column.Width.Value, column.Width.UnitType, desiredWidth, desiredWidth, false);
                }
                else
                {
                    column.SetWidthInternalNoCallback(new DataGridLength(column.Width.Value, column.Width.UnitType, desiredWidth, desiredWidth));
                    this.OnColumnWidthChanged(column);
                }
            }
        }

        internal bool ColumnRequiresRightGridLine(DataGridColumn dataGridColumn, bool includeLastRightGridLineWhenPresent)
        {
            return (this.GridLinesVisibility == DataGridGridLinesVisibility.Vertical || this.GridLinesVisibility == DataGridGridLinesVisibility.All) && this.VerticalGridLinesBrush != null &&
                   (dataGridColumn != this.ColumnsInternal.LastVisibleColumn || (includeLastRightGridLineWhenPresent && this.ColumnsInternal.FillerColumn.IsActive));
        }

        internal DataGridColumnCollection CreateColumnsInstance()
        {
            return new DataGridColumnCollection(this);
        }

        /// <summary>
        /// Decreases the widths of all columns with DisplayIndex >= displayIndex such that the total
        /// width is decreased by the given amount, if possible.  If the total desired adjustment amount
        /// could not be met, the remaining amount of adjustment is returned.
        /// </summary>
        /// <param name="displayIndex">Starting column DisplayIndex.</param>
        /// <param name="amount">Amount to decrease (in pixels).</param>
        /// <param name="userInitiated">Whether or not this adjustment was initiated by a user action.</param>
        /// <returns>The remaining amount of adjustment.</returns>
        internal double DecreaseColumnWidths(int displayIndex, double amount, bool userInitiated)
        {
            // 1. Take space from non-star columns with widths larger than desired widths (left to right).
            amount = DecreaseNonStarColumnWidths(displayIndex, c => c.Width.DesiredValue, amount, false, false);

            // 2. Take space from star columns until they reach their min.
            amount = AdjustStarColumnWidths(displayIndex, amount, userInitiated);

            // 3. Take space from non-star columns that have already been initialized, until they reach their min (right to left).
            amount = DecreaseNonStarColumnWidths(displayIndex, c => c.ActualMinWidth, amount, true, false);

            // 4. Take space from all non-star columns until they reach their min, even if they are new (right to left).
            amount = DecreaseNonStarColumnWidths(displayIndex, c => c.ActualMinWidth, amount, true, true);

            return amount;
        }

        internal bool GetColumnReadOnlyState(DataGridColumn dataGridColumn, bool isReadOnly)
        {
            DiagnosticsDebug.Assert(dataGridColumn != null, "Expected non-null dataGridColumn.");

            DataGridBoundColumn dataGridBoundColumn = dataGridColumn as DataGridBoundColumn;
            if (dataGridBoundColumn != null && dataGridBoundColumn.Binding != null)
            {
                string path = null;
                if (dataGridBoundColumn.Binding.Path != null)
                {
                    path = dataGridBoundColumn.Binding.Path.Path;
                }

                if (!string.IsNullOrEmpty(path))
                {
                    return this.DataConnection.GetPropertyIsReadOnly(path) || isReadOnly;
                }
            }

            return isReadOnly;
        }

        /// <summary>
        /// Increases the widths of all columns with DisplayIndex >= displayIndex such that the total
        /// width is increased by the given amount, if possible.  If the total desired adjustment amount
        /// could not be met, the remaining amount of adjustment is returned.
        /// </summary>
        /// <param name="displayIndex">Starting column DisplayIndex.</param>
        /// <param name="amount">Amount of increase (in pixels).</param>
        /// <param name="userInitiated">Whether or not this adjustment was initiated by a user action.</param>
        /// <returns>The remaining amount of adjustment.</returns>
        internal double IncreaseColumnWidths(int displayIndex, double amount, bool userInitiated)
        {
            // 1. Give space to non-star columns that are smaller than their desired widths (left to right).
            amount = IncreaseNonStarColumnWidths(displayIndex, c => c.Width.DesiredValue, amount, false, false);

            // 2. Give space to star columns until they reach their max.
            amount = AdjustStarColumnWidths(displayIndex, amount, userInitiated);

            // 3. Give space to non-star columns that have already been initialized, until they reach their max (right to left).
            amount = IncreaseNonStarColumnWidths(displayIndex, c => c.ActualMaxWidth, amount, true, false);

            // 4. Give space to all non-star columns until they reach their max, even if they are new (right to left).
            amount = IncreaseNonStarColumnWidths(displayIndex, c => c.ActualMaxWidth, amount, true, false);

            return amount;
        }

        internal void OnClearingColumns()
        {
            // Rows need to be cleared first. There cannot be rows without also having columns.
            ClearRows(false);

            // Removing all the column header cells
            RemoveDisplayedColumnHeaders();

            _horizontalOffset = _negHorizontalOffset = 0;

            if (_hScrollBar != null && _hScrollBar.Visibility == Visibility.Visible)
            {
                _hScrollBar.Value = 0;
            }
        }

        /// <summary>
        /// Invalidates the widths of all columns because the resizing behavior of an individual column has changed.
        /// </summary>
        /// <param name="column">Column with CanUserResize property that has changed.</param>
        internal void OnColumnCanUserResizeChanged(DataGridColumn column)
        {
            if (column.IsVisible)
            {
                EnsureHorizontalLayout();
            }
        }

        internal void OnColumnCellStyleChanged(DataGridColumn column, Style previousStyle)
        {
            // Set HeaderCell.Style for displayed rows if HeaderCell.Style is not already set
            foreach (DataGridRow row in GetAllRows())
            {
                row.Cells[column.Index].EnsureStyle(previousStyle);
            }

            InvalidateRowHeightEstimate();
        }

        internal void OnColumnCollectionChanged_PostNotification(bool columnsGrew)
        {
            if (columnsGrew &&
                this.CurrentColumnIndex == -1)
            {
                MakeFirstDisplayedCellCurrentCell();
            }

            if (_autoGeneratingColumnOperationCount == 0)
            {
                EnsureRowsPresenterVisibility();
                InvalidateRowHeightEstimate();
            }
        }

        internal void OnColumnCollectionChanged_PreNotification(bool columnsGrew)
        {
            // dataGridColumn==null means the collection was refreshed.
            if (columnsGrew && _autoGeneratingColumnOperationCount == 0 && this.ColumnsItemsInternal.Count == 1)
            {
                RefreshRows(false /*recycleRows*/, true /*clearRows*/);
            }
            else
            {
                InvalidateMeasure();
            }
        }

        internal void OnColumnDisplayIndexChanged(DataGridColumn dataGridColumn)
        {
            DiagnosticsDebug.Assert(dataGridColumn != null, "Expected non-null dataGridColumn.");
            DataGridColumnEventArgs e = new DataGridColumnEventArgs(dataGridColumn);

            // Call protected method to raise event
            if (dataGridColumn != this.ColumnsInternal.RowGroupSpacerColumn)
            {
                OnColumnDisplayIndexChanged(e);
            }
        }

        internal void OnColumnDisplayIndexChanged_PostNotification()
        {
            // Notifications for adjusted display indexes.
            FlushDisplayIndexChanged(true /*raiseEvent*/);

            // Our displayed columns may have changed so recompute them
            UpdateDisplayedColumns();

            // Invalidate layout
            CorrectColumnFrozenStates();
            EnsureHorizontalLayout();
        }

        internal void OnColumnDisplayIndexChanging(DataGridColumn targetColumn, int newDisplayIndex)
        {
            DiagnosticsDebug.Assert(targetColumn != null, "Expected non-null targetColumn.");
            DiagnosticsDebug.Assert(newDisplayIndex != targetColumn.DisplayIndexWithFiller, "Expected newDisplayIndex other than targetColumn.DisplayIndexWithFiller.");

            if (InDisplayIndexAdjustments)
            {
                // We are within columns display indexes adjustments. We do not allow changing display indexes while adjusting them.
                throw DataGridError.DataGrid.CannotChangeColumnCollectionWhileAdjustingDisplayIndexes();
            }

            try
            {
                InDisplayIndexAdjustments = true;

                bool trackChange = targetColumn != this.ColumnsInternal.RowGroupSpacerColumn;
                DataGridColumn column;

                // Move is legal - let's adjust the affected display indexes.
                if (newDisplayIndex < targetColumn.DisplayIndexWithFiller)
                {
                    // DisplayIndex decreases. All columns with newDisplayIndex <= DisplayIndex < targetColumn.DisplayIndex
                    // get their DisplayIndex incremented.
                    for (int i = newDisplayIndex; i < targetColumn.DisplayIndexWithFiller; i++)
                    {
                        column = this.ColumnsInternal.GetColumnAtDisplayIndex(i);
                        column.DisplayIndexWithFiller = column.DisplayIndexWithFiller + 1;
                        if (trackChange)
                        {
                            column.DisplayIndexHasChanged = true; // OnColumnDisplayIndexChanged needs to be raised later on
                        }
                    }
                }
                else
                {
                    // DisplayIndex increases. All columns with targetColumn.DisplayIndex < DisplayIndex <= newDisplayIndex
                    // get their DisplayIndex decremented.
                    for (int i = newDisplayIndex; i > targetColumn.DisplayIndexWithFiller; i--)
                    {
                        column = this.ColumnsInternal.GetColumnAtDisplayIndex(i);
                        column.DisplayIndexWithFiller = column.DisplayIndexWithFiller - 1;
                        if (trackChange)
                        {
                            column.DisplayIndexHasChanged = true; // OnColumnDisplayIndexChanged needs to be raised later on
                        }
                    }
                }

                // Now let's actually change the order of the DisplayIndexMap
                if (targetColumn.DisplayIndexWithFiller != -1)
                {
                    this.ColumnsInternal.DisplayIndexMap.Remove(targetColumn.Index);
                }

                this.ColumnsInternal.DisplayIndexMap.Insert(newDisplayIndex, targetColumn.Index);
            }
            finally
            {
                InDisplayIndexAdjustments = false;
            }

            // Note that displayIndex of moved column is updated by caller.
        }

        internal void OnColumnBindingChanged(DataGridBoundColumn column)
        {
            // Update Binding in Displayed rows by regenerating the affected elements
            if (_rowsPresenter != null)
            {
                foreach (DataGridRow row in GetAllRows())
                {
                    PopulateCellContent(false /*isCellEdited*/, column, row, row.Cells[column.Index]);
                }
            }
        }

        internal void OnColumnElementStyleChanged(DataGridBoundColumn column)
        {
            // Update Element Style in Displayed rows
            foreach (DataGridRow row in GetAllRows())
            {
                FrameworkElement element = column.GetCellContent(row);
                if (element != null)
                {
                    element.SetStyleWithType(column.ElementStyle);
                }
            }

            InvalidateRowHeightEstimate();
        }

        internal void OnColumnHeaderDragStarted(DragStartedEventArgs e)
        {
            if (this.ColumnHeaderDragStarted != null)
            {
                this.ColumnHeaderDragStarted(this, e);
            }
        }

        internal void OnColumnHeaderDragDelta(DragDeltaEventArgs e)
        {
            if (this.ColumnHeaderDragDelta != null)
            {
                this.ColumnHeaderDragDelta(this, e);
            }
        }

        internal void OnColumnHeaderDragCompleted(DragCompletedEventArgs e)
        {
            if (this.ColumnHeaderDragCompleted != null)
            {
                this.ColumnHeaderDragCompleted(this, e);
            }
        }

        /// <summary>
        /// Adjusts the specified column's width according to its new maximum value.
        /// </summary>
        /// <param name="column">The column to adjust.</param>
        /// <param name="oldValue">The old ActualMaxWidth of the column.</param>
        internal void OnColumnMaxWidthChanged(DataGridColumn column, double oldValue)
        {
            DiagnosticsDebug.Assert(column != null, "Expected non-null column.");

            if (column.Visibility == Visibility.Visible && oldValue != column.ActualMaxWidth)
            {
                if (column.ActualMaxWidth < column.Width.DisplayValue)
                {
                    // If the maximum width has caused the column to decrease in size, try first to resize
                    // the columns to the right to make up for the difference in width, but don't limit the column's
                    // final display value to how much they could be resized.
                    AdjustColumnWidths(column.DisplayIndex + 1, column.Width.DisplayValue - column.ActualMaxWidth, false);
                    column.SetWidthDisplayValue(column.ActualMaxWidth);
                }
                else if (column.Width.DisplayValue == oldValue && column.Width.DesiredValue > column.Width.DisplayValue)
                {
                    // If the column was previously limited by its maximum value but has more room now,
                    // attempt to resize the column to its desired width.
                    column.Resize(column.Width.Value, column.Width.UnitType, column.Width.DesiredValue, column.Width.DesiredValue, false);
                }

                OnColumnWidthChanged(column);
            }
        }

        /// <summary>
        /// Adjusts the specified column's width according to its new minimum value.
        /// </summary>
        /// <param name="column">The column to adjust.</param>
        /// <param name="oldValue">The old ActualMinWidth of the column.</param>
        internal void OnColumnMinWidthChanged(DataGridColumn column, double oldValue)
        {
            DiagnosticsDebug.Assert(column != null, "Expected non-null column.");

            if (column.Visibility == Visibility.Visible && oldValue != column.ActualMinWidth)
            {
                if (column.ActualMinWidth > column.Width.DisplayValue)
                {
                    // If the minimum width has caused the column to increase in size, try first to resize
                    // the columns to the right to make up for the difference in width, but don't limit the column's
                    // final display value to how much they could be resized.
                    AdjustColumnWidths(column.DisplayIndex + 1, column.Width.DisplayValue - column.ActualMinWidth, false);
                    column.SetWidthDisplayValue(column.ActualMinWidth);
                }
                else if (column.Width.DisplayValue == oldValue && column.Width.DesiredValue < column.Width.DisplayValue)
                {
                    // If the column was previously limited by its minimum value but can be smaller now,
                    // attempt to resize the column to its desired width.
                    column.Resize(column.Width.Value, column.Width.UnitType, column.Width.DesiredValue, column.Width.DesiredValue, false);
                }

                OnColumnWidthChanged(column);
            }
        }

        internal void OnColumnReadOnlyStateChanging(DataGridColumn dataGridColumn, bool isReadOnly)
        {
            DiagnosticsDebug.Assert(dataGridColumn != null, "Expected non-null dataGridColumn.");
            if (isReadOnly && this.CurrentColumnIndex == dataGridColumn.Index)
            {
                // Edited column becomes read-only. Exit editing mode.
                if (!EndCellEdit(DataGridEditAction.Commit, true /*exitEditingMode*/, this.ContainsFocus /*keepFocus*/, true /*raiseEvents*/))
                {
                    EndCellEdit(DataGridEditAction.Cancel, true /*exitEditingMode*/, this.ContainsFocus /*keepFocus*/, false /*raiseEvents*/);
                }
            }
        }

        internal void OnColumnVisibleStateChanged(DataGridColumn updatedColumn)
        {
            DiagnosticsDebug.Assert(updatedColumn != null, "Expected non-null updatedColumn.");

            CorrectColumnFrozenStates();
            UpdateDisplayedColumns();
            EnsureRowsPresenterVisibility();
            EnsureHorizontalLayout();
            InvalidateColumnHeadersMeasure();

            if (updatedColumn.IsVisible &&
                this.ColumnsInternal.VisibleColumnCount == 1 && this.CurrentColumnIndex == -1)
            {
                DiagnosticsDebug.Assert(this.SelectedIndex == this.DataConnection.IndexOf(this.SelectedItem), "Expected SelectedIndex equals DataConnection.IndexOf(this.SelectedItem).");
                if (this.SelectedIndex != -1)
                {
                    SetAndSelectCurrentCell(updatedColumn.Index, this.SelectedIndex, true /*forceCurrentCellSelection*/);
                }
                else
                {
                    MakeFirstDisplayedCellCurrentCell();
                }
            }

            // We need to explicitly collapse the cells of the invisible column because layout only goes through
            // visible ones
            if (updatedColumn.Visibility == Visibility.Collapsed)
            {
                foreach (DataGridRow row in GetAllRows())
                {
                    row.Cells[updatedColumn.Index].Visibility = Visibility.Collapsed;
                }
            }
        }

        internal void OnColumnVisibleStateChanging(DataGridColumn targetColumn)
        {
            DiagnosticsDebug.Assert(targetColumn != null, "Expected non-null targetColumn.");

            if (targetColumn.IsVisible && this.CurrentColumn == targetColumn)
            {
                // Column of the current cell is made invisible. Trying to move the current cell to a neighbor column. May throw an exception.
                DataGridColumn dataGridColumn = this.ColumnsInternal.GetNextVisibleColumn(targetColumn);
                if (dataGridColumn == null)
                {
                    dataGridColumn = this.ColumnsInternal.GetPreviousVisibleNonFillerColumn(targetColumn);
                }

                if (dataGridColumn == null)
                {
                    SetCurrentCellCore(-1, -1);
                }
                else
                {
                    SetCurrentCellCore(dataGridColumn.Index, this.CurrentSlot);
                }
            }
        }

        internal void OnColumnWidthChanged(DataGridColumn updatedColumn)
        {
            DiagnosticsDebug.Assert(updatedColumn != null, "Expected non-null updatedColumn.");
            if (updatedColumn.IsVisible)
            {
                EnsureHorizontalLayout();
            }
        }

        internal void OnFillerColumnWidthNeeded(double finalWidth)
        {
            DataGridFillerColumn fillerColumn = this.ColumnsInternal.FillerColumn;
            double totalColumnsWidth = this.ColumnsInternal.VisibleEdgedColumnsWidth;
            if (finalWidth - totalColumnsWidth > DATAGRID_roundingDelta)
            {
                fillerColumn.FillerWidth = finalWidth - totalColumnsWidth;
            }
            else
            {
                fillerColumn.FillerWidth = 0;
            }
        }

        internal void OnInsertedColumn_PostNotification(DataGridCellCoordinates newCurrentCellCoordinates, int newDisplayIndex)
        {
            // Update current cell if needed
            if (newCurrentCellCoordinates.ColumnIndex != -1)
            {
                DiagnosticsDebug.Assert(this.CurrentColumnIndex == -1, "Expected CurrentColumnIndex equals -1.");
                SetAndSelectCurrentCell(
                    newCurrentCellCoordinates.ColumnIndex,
                    newCurrentCellCoordinates.Slot,
                    this.ColumnsInternal.VisibleColumnCount == 1 /*forceCurrentCellSelection*/);

                if (newDisplayIndex < this.FrozenColumnCountWithFiller)
                {
                    CorrectColumnFrozenStates();
                }
            }
        }

        internal void OnInsertedColumn_PreNotification(DataGridColumn insertedColumn)
        {
            // Fix the Index of all following columns
            CorrectColumnIndexesAfterInsertion(insertedColumn, 1);

            DiagnosticsDebug.Assert(insertedColumn.Index >= 0, "Expected positive insertedColumn.Index.");
            DiagnosticsDebug.Assert(insertedColumn.Index < this.ColumnsItemsInternal.Count, "insertedColumn.Index smaller than ColumnsItemsInternal.Count.");
            DiagnosticsDebug.Assert(insertedColumn.OwningGrid == this, "Expected insertedColumn.OwningGrid equals this DataGrid.");

            CorrectColumnDisplayIndexesAfterInsertion(insertedColumn);

            InsertDisplayedColumnHeader(insertedColumn);

            // Insert the missing data cells
            if (this.SlotCount > 0)
            {
                int newColumnCount = this.ColumnsItemsInternal.Count;

                foreach (DataGridRow row in GetAllRows())
                {
                    if (row.Cells.Count < newColumnCount)
                    {
                        AddNewCellPrivate(row, insertedColumn);
                    }
                }
            }

            if (insertedColumn.IsVisible)
            {
                EnsureHorizontalLayout();
            }

            DataGridBoundColumn boundColumn = insertedColumn as DataGridBoundColumn;
            if (boundColumn != null && !boundColumn.IsAutoGenerated)
            {
                boundColumn.SetHeaderFromBinding();
            }
        }

        internal DataGridCellCoordinates OnInsertingColumn(int columnIndexInserted, DataGridColumn insertColumn)
        {
            DataGridCellCoordinates newCurrentCellCoordinates;
            DiagnosticsDebug.Assert(insertColumn != null, "Expected non-null insertColumn.");

            if (insertColumn.OwningGrid != null && insertColumn != this.ColumnsInternal.RowGroupSpacerColumn)
            {
                throw DataGridError.DataGrid.ColumnCannotBeReassignedToDifferentDataGrid();
            }

            // Reset current cell if there is one, no matter the relative position of the columns involved
            if (this.CurrentColumnIndex != -1)
            {
                _temporarilyResetCurrentCell = true;
                newCurrentCellCoordinates = new DataGridCellCoordinates(
                    columnIndexInserted <= this.CurrentColumnIndex ? this.CurrentColumnIndex + 1 : this.CurrentColumnIndex,
                    this.CurrentSlot);
                ResetCurrentCellCore();
            }
            else
            {
                newCurrentCellCoordinates = new DataGridCellCoordinates(-1, -1);
            }

            return newCurrentCellCoordinates;
        }

        internal void OnRemovedColumn_PostNotification(DataGridCellCoordinates newCurrentCellCoordinates)
        {
            // Update current cell if needed
            if (newCurrentCellCoordinates.ColumnIndex != -1)
            {
                DiagnosticsDebug.Assert(this.CurrentColumnIndex == -1, "Expected CurrentColumnIndex equals -1.");
                SetAndSelectCurrentCell(newCurrentCellCoordinates.ColumnIndex, newCurrentCellCoordinates.Slot, false /*forceCurrentCellSelection*/);
            }
        }

        internal void OnRemovedColumn_PreNotification(DataGridColumn removedColumn)
        {
            DiagnosticsDebug.Assert(removedColumn.Index >= 0, "Expected positive removedColumn.Index.");
            DiagnosticsDebug.Assert(removedColumn.OwningGrid == null, "Expected null removedColumn.OwningGrid.");

            // Intentionally keep the DisplayIndex intact after detaching the column.
            CorrectColumnIndexesAfterDeletion(removedColumn);

            CorrectColumnDisplayIndexesAfterDeletion(removedColumn);

            // If the detached column was frozen, a new column needs to take its place
            if (removedColumn.IsFrozen)
            {
                removedColumn.IsFrozen = false;
                CorrectColumnFrozenStates();
            }

            UpdateDisplayedColumns();

            // Fix the existing rows by removing cells at correct index
            int newColumnCount = this.ColumnsItemsInternal.Count;

            if (_rowsPresenter != null)
            {
                foreach (DataGridRow row in GetAllRows())
                {
                    if (row.Cells.Count > newColumnCount)
                    {
                        row.Cells.RemoveAt(removedColumn.Index);
                    }
                }

                _rowsPresenter.InvalidateArrange();
            }

            RemoveDisplayedColumnHeader(removedColumn);
        }

        internal DataGridCellCoordinates OnRemovingColumn(DataGridColumn dataGridColumn)
        {
            DiagnosticsDebug.Assert(dataGridColumn != null, "Expected non-null dataGridColumn.");
            DiagnosticsDebug.Assert(dataGridColumn.Index >= 0, "Expected positive dataGridColumn.Index.");
            DiagnosticsDebug.Assert(dataGridColumn.Index < this.ColumnsItemsInternal.Count, "Expected dataGridColumn.Index smaller than ColumnsItemsInternal.Count.");

            DataGridCellCoordinates newCurrentCellCoordinates;

            _temporarilyResetCurrentCell = false;
            int columnIndex = dataGridColumn.Index;

            // Reset the current cell's address if there is one.
            if (this.CurrentColumnIndex != -1)
            {
                int newCurrentColumnIndex = this.CurrentColumnIndex;
                if (columnIndex == newCurrentColumnIndex)
                {
                    DataGridColumn dataGridColumnNext = this.ColumnsInternal.GetNextVisibleColumn(this.ColumnsItemsInternal[columnIndex]);
                    if (dataGridColumnNext != null)
                    {
                        if (dataGridColumnNext.Index > columnIndex)
                        {
                            newCurrentColumnIndex = dataGridColumnNext.Index - 1;
                        }
                        else
                        {
                            newCurrentColumnIndex = dataGridColumnNext.Index;
                        }
                    }
                    else
                    {
                        DataGridColumn dataGridColumnPrevious = this.ColumnsInternal.GetPreviousVisibleNonFillerColumn(this.ColumnsItemsInternal[columnIndex]);
                        if (dataGridColumnPrevious != null)
                        {
                            if (dataGridColumnPrevious.Index > columnIndex)
                            {
                                newCurrentColumnIndex = dataGridColumnPrevious.Index - 1;
                            }
                            else
                            {
                                newCurrentColumnIndex = dataGridColumnPrevious.Index;
                            }
                        }
                        else
                        {
                            newCurrentColumnIndex = -1;
                        }
                    }
                }
                else if (columnIndex < newCurrentColumnIndex)
                {
                    newCurrentColumnIndex--;
                }

                newCurrentCellCoordinates = new DataGridCellCoordinates(newCurrentColumnIndex, (newCurrentColumnIndex == -1) ? -1 : this.CurrentSlot);
                if (columnIndex == this.CurrentColumnIndex)
                {
                    // If the commit fails, force a cancel edit
                    if (!this.CommitEdit(DataGridEditingUnit.Row, false /*exitEditingMode*/))
                    {
                        this.CancelEdit(DataGridEditingUnit.Row, false /*raiseEvents*/);
                    }
                }
                else
                {
                    // Underlying data of deleted column is gone. It cannot be accessed anymore.
                    // Do not end editing mode so that CellValidation doesn't get raised, since that event needs the current formatted value.
                    _temporarilyResetCurrentCell = true;
                }

                bool success = this.SetCurrentCellCore(-1, -1);
                DiagnosticsDebug.Assert(success, "Expected successful call to SetCurrentCellCore.");
            }
            else
            {
                newCurrentCellCoordinates = new DataGridCellCoordinates(-1, -1);
            }

            // If the last column is removed, delete all the rows first.
            if (this.ColumnsItemsInternal.Count == 1)
            {
                ClearRows(false);
            }

            // Is deleted column scrolled off screen?
            if (dataGridColumn.IsVisible &&
                !dataGridColumn.IsFrozen &&
                this.DisplayData.FirstDisplayedScrollingCol >= 0)
            {
                // Deleted column is part of scrolling columns.
                if (this.DisplayData.FirstDisplayedScrollingCol == dataGridColumn.Index)
                {
                    // Deleted column is first scrolling column
                    _horizontalOffset -= _negHorizontalOffset;
                    _negHorizontalOffset = 0;
                }
                else if (!this.ColumnsInternal.DisplayInOrder(this.DisplayData.FirstDisplayedScrollingCol, dataGridColumn.Index))
                {
                    // Deleted column is displayed before first scrolling column
                    DiagnosticsDebug.Assert(_horizontalOffset >= GetEdgedColumnWidth(dataGridColumn), "Expected _horizontalOffset greater than or equal to GetEdgedColumnWidth(dataGridColumn).");
                    _horizontalOffset -= GetEdgedColumnWidth(dataGridColumn);
                }

                if (_hScrollBar != null && _hScrollBar.Visibility == Visibility.Visible)
                {
                    _hScrollBar.Value = _horizontalOffset;
                }
            }

            return newCurrentCellCoordinates;
        }

        /// <summary>
        /// Called when a column property changes, and its cells need to adjust that column change.
        /// </summary>
        internal void RefreshColumnElements(DataGridColumn dataGridColumn, string propertyName)
        {
            DiagnosticsDebug.Assert(dataGridColumn != null, "Expected non-null dataGridColumn.");

            // Take care of the non-displayed loaded rows
            for (int index = 0; index < _loadedRows.Count;)
            {
                DataGridRow dataGridRow = _loadedRows[index];
                DiagnosticsDebug.Assert(dataGridRow != null, "Expected non-null dataGridRow.");
                if (!this.IsSlotVisible(dataGridRow.Slot))
                {
                    RefreshCellElement(dataGridColumn, dataGridRow, propertyName);
                }

                index++;
            }

            // Take care of the displayed rows
            if (_rowsPresenter != null)
            {
                foreach (DataGridRow row in GetAllRows())
                {
                    RefreshCellElement(dataGridColumn, row, propertyName);
                }

                // This update could change layout so we need to update our estimate and invalidate
                InvalidateRowHeightEstimate();
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Decreases the width of a non-star column by the given amount, if possible.  If the total desired
        /// adjustment amount could not be met, the remaining amount of adjustment is returned.  The adjustment
        /// stops when the column's target width has been met.
        /// </summary>
        /// <param name="column">Column to adjust.</param>
        /// <param name="targetWidth">The target width of the column (in pixels).</param>
        /// <param name="amount">Amount to decrease (in pixels).</param>
        /// <returns>The remaining amount of adjustment.</returns>
        private static double DecreaseNonStarColumnWidth(DataGridColumn column, double targetWidth, double amount)
        {
            DiagnosticsDebug.Assert(amount < 0, "Expected negative amount.");
            DiagnosticsDebug.Assert(column.Width.UnitType != DataGridLengthUnitType.Star, "column.Width.UnitType other than DataGridLengthUnitType.Star.");

            if (DoubleUtil.GreaterThanOrClose(targetWidth, column.Width.DisplayValue))
            {
                return amount;
            }

            double adjustment = Math.Max(
                column.ActualMinWidth - column.Width.DisplayValue,
                Math.Max(targetWidth - column.Width.DisplayValue, amount));

            column.SetWidthDisplayValue(column.Width.DisplayValue + adjustment);
            return amount - adjustment;
        }

        private static DataGridAutoGeneratingColumnEventArgs GenerateColumn(Type propertyType, string propertyName, string header)
        {
            // Create a new DataBoundColumn for the Property
            DataGridBoundColumn newColumn = GetDataGridColumnFromType(propertyType);
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            newColumn.Binding = binding;
            newColumn.Header = header;
            newColumn.IsAutoGenerated = true;
            return new DataGridAutoGeneratingColumnEventArgs(propertyName, propertyType, newColumn);
        }

        private static DataGridBoundColumn GetDataGridColumnFromType(Type type)
        {
            DiagnosticsDebug.Assert(type != null, "Expected non-null type.");
            if (type == typeof(bool))
            {
                return new DataGridCheckBoxColumn();
            }
            else if (type == typeof(bool?))
            {
                DataGridCheckBoxColumn column = new DataGridCheckBoxColumn();
                column.IsThreeState = true;
                return column;
            }

            return new DataGridTextColumn();
        }

        /// <summary>
        /// Increases the width of a non-star column by the given amount, if possible.  If the total desired
        /// adjustment amount could not be met, the remaining amount of adjustment is returned.  The adjustment
        /// stops when the column's target width has been met.
        /// </summary>
        /// <param name="column">Column to adjust.</param>
        /// <param name="targetWidth">The target width of the column (in pixels).</param>
        /// <param name="amount">Amount to increase (in pixels).</param>
        /// <returns>The remaining amount of adjustment.</returns>
        private static double IncreaseNonStarColumnWidth(DataGridColumn column, double targetWidth, double amount)
        {
            DiagnosticsDebug.Assert(amount > 0, "Expected strictly positive amount.");
            DiagnosticsDebug.Assert(column.Width.UnitType != DataGridLengthUnitType.Star, "Expected column.Width.UnitType other than DataGridLengthUnitType.Star.");

            if (targetWidth <= column.Width.DisplayValue)
            {
                return amount;
            }

            double adjustment = Math.Min(
                column.ActualMaxWidth - column.Width.DisplayValue,
                Math.Min(targetWidth - column.Width.DisplayValue, amount));

            column.SetWidthDisplayValue(column.Width.DisplayValue + adjustment);
            return amount - adjustment;
        }

        private static void RefreshCellElement(DataGridColumn dataGridColumn, DataGridRow dataGridRow, string propertyName)
        {
            DiagnosticsDebug.Assert(dataGridColumn != null, "Expected non-null dataGridColumn.");
            DiagnosticsDebug.Assert(dataGridRow != null, "Expected non-null dataGridRow.");

            DataGridCell dataGridCell = dataGridRow.Cells[dataGridColumn.Index];
            DiagnosticsDebug.Assert(dataGridCell != null, "Expected non-null dataGridCell.");
            FrameworkElement element = dataGridCell.Content as FrameworkElement;
            if (element != null)
            {
                dataGridColumn.RefreshCellContent(element, dataGridRow.ComputedForeground, propertyName);
            }
        }

        private bool AddGeneratedColumn(DataGridAutoGeneratingColumnEventArgs e)
        {
            // Raise the AutoGeneratingColumn event in case the user wants to Cancel or Replace the
            // column being generated
            OnAutoGeneratingColumn(e);
            if (e.Cancel)
            {
                return false;
            }
            else
            {
                if (e.Column != null)
                {
                    // Set the IsAutoGenerated flag here in case the user provides a custom auto-generated column
                    e.Column.IsAutoGenerated = true;
                }

                this.ColumnsInternal.Add(e.Column);
                this.ColumnsInternal.AutogeneratedColumnCount++;
                return true;
            }
        }

        /// <summary>
        /// Adjusts the widths of all star columns with DisplayIndex >= displayIndex such that the total
        /// width is adjusted by the given amount, if possible.  If the total desired adjustment amount
        /// could not be met, the remaining amount of adjustment is returned.
        /// </summary>
        /// <param name="displayIndex">Starting column DisplayIndex.</param>
        /// <param name="adjustment">Adjustment amount (positive for increase, negative for decrease).</param>
        /// <param name="userInitiated">Whether or not this adjustment was initiated by a user action.</param>
        /// <returns>The remaining amount of adjustment.</returns>
        private double AdjustStarColumnWidths(int displayIndex, double adjustment, bool userInitiated)
        {
            double remainingAdjustment = adjustment;
            if (DoubleUtil.IsZero(remainingAdjustment))
            {
                return remainingAdjustment;
            }

            bool increase = remainingAdjustment > 0;

            // Make an initial pass through the star columns to total up some values.
            bool scaleStarWeights = false;
            double totalStarColumnsWidth = 0;
            double totalStarColumnsWidthLimit = 0;
            double totalStarWeights = 0;
            List<DataGridColumn> starColumns = new List<DataGridColumn>();
            foreach (DataGridColumn column in this.ColumnsInternal.GetDisplayedColumns(c => c.Width.IsStar && c.IsVisible && (c.ActualCanUserResize || !userInitiated)))
            {
                if (column.DisplayIndex < displayIndex)
                {
                    scaleStarWeights = true;
                    continue;
                }

                starColumns.Add(column);
                totalStarWeights += column.Width.Value;
                totalStarColumnsWidth += column.Width.DisplayValue;
                totalStarColumnsWidthLimit += increase ? column.ActualMaxWidth : column.ActualMinWidth;
            }

            // Set the new desired widths according to how much all the star columns can be adjusted without any
            // of them being limited by their minimum or maximum widths (as that would distort their ratios).
            double adjustmentLimit = totalStarColumnsWidthLimit - totalStarColumnsWidth;
            adjustmentLimit = increase ? Math.Min(adjustmentLimit, adjustment) : Math.Max(adjustmentLimit, adjustment);
            foreach (DataGridColumn starColumn in starColumns)
            {
                starColumn.SetWidthDesiredValue((totalStarColumnsWidth + adjustmentLimit) * starColumn.Width.Value / totalStarWeights);
            }

            // Adjust the star column widths first towards their desired values, and then towards their limits.
            remainingAdjustment = AdjustStarColumnWidths(displayIndex, remainingAdjustment, userInitiated, c => c.Width.DesiredValue);
            remainingAdjustment = AdjustStarColumnWidths(displayIndex, remainingAdjustment, userInitiated, c => increase ? c.ActualMaxWidth : c.ActualMinWidth);

            // Set the new star value weights according to how much the total column widths have changed.
            // Only do this if there were other star columns to the left, though.  If there weren't any then that means
            // all the star columns were adjusted at the same time, and therefore, their ratios have not changed.
            if (scaleStarWeights)
            {
                double starRatio = (totalStarColumnsWidth + adjustment - remainingAdjustment) / totalStarColumnsWidth;
                foreach (DataGridColumn starColumn in starColumns)
                {
                    starColumn.SetWidthStarValue(Math.Min(double.MaxValue, starRatio * starColumn.Width.Value));
                }
            }

            return remainingAdjustment;
        }

        /// <summary>
        /// Adjusts the widths of all star columns with DisplayIndex >= displayIndex such that the total
        /// width is adjusted by the given amount, if possible.  If the total desired adjustment amount
        /// could not be met, the remaining amount of adjustment is returned.  The columns will stop adjusting
        /// once they hit their target widths.
        /// </summary>
        /// <param name="displayIndex">Starting column DisplayIndex.</param>
        /// <param name="remainingAdjustment">Adjustment amount (positive for increase, negative for decrease).</param>
        /// <param name="userInitiated">Whether or not this adjustment was initiated by a user action.</param>
        /// <param name="targetWidth">The target width of the column.</param>
        /// <returns>The remaining amount of adjustment.</returns>
        private double AdjustStarColumnWidths(int displayIndex, double remainingAdjustment, bool userInitiated, Func<DataGridColumn, double> targetWidth)
        {
            if (DoubleUtil.IsZero(remainingAdjustment))
            {
                return remainingAdjustment;
            }

            bool increase = remainingAdjustment > 0;

            double totalStarWeights = 0;
            double totalStarColumnsWidth = 0;

            // Order the star columns according to which one will hit their target width (or min/max limit) first.
            // Each KeyValuePair represents a column (as the key) and an ordering factor (as the value).  The ordering factor
            // is computed based on the distance from each column's current display width to its target width.  Because each column
            // could have different star ratios, though, this distance is then adjusted according to its star value.  A column with
            // a larger star value, for example, will change size more rapidly than a column with a lower star value.
            List<KeyValuePair<DataGridColumn, double>> starColumnPairs = new List<KeyValuePair<DataGridColumn, double>>();
            foreach (DataGridColumn column in this.ColumnsInternal.GetDisplayedColumns(
                c => c.Width.IsStar && c.DisplayIndex >= displayIndex && c.IsVisible && c.Width.Value > 0 && (c.ActualCanUserResize || !userInitiated)))
            {
                int insertIndex = 0;
                double distanceToTarget = Math.Min(column.ActualMaxWidth, Math.Max(targetWidth(column), column.ActualMinWidth)) - column.Width.DisplayValue;
                double factor = (increase ? Math.Max(0, distanceToTarget) : Math.Min(0, distanceToTarget)) / column.Width.Value;
                foreach (KeyValuePair<DataGridColumn, double> starColumnPair in starColumnPairs)
                {
                    if (increase ? factor <= starColumnPair.Value : factor >= starColumnPair.Value)
                    {
                        break;
                    }

                    insertIndex++;
                }

                starColumnPairs.Insert(insertIndex, new KeyValuePair<DataGridColumn, double>(column, factor));
                totalStarWeights += column.Width.Value;
                totalStarColumnsWidth += column.Width.DisplayValue;
            }

            // Adjust the column widths one at a time until they either hit their individual target width
            // or the total remaining amount to adjust has been depleted.
            foreach (KeyValuePair<DataGridColumn, double> starColumnPair in starColumnPairs)
            {
                double distanceToTarget = starColumnPair.Value * starColumnPair.Key.Width.Value;
                double distanceAvailable = (starColumnPair.Key.Width.Value * remainingAdjustment) / totalStarWeights;
                double adjustment = increase ? Math.Min(distanceToTarget, distanceAvailable) : Math.Max(distanceToTarget, distanceAvailable);

                remainingAdjustment -= adjustment;
                totalStarWeights -= starColumnPair.Key.Width.Value;
                starColumnPair.Key.SetWidthDisplayValue(Math.Max(DataGrid.DATAGRID_minimumStarColumnWidth, starColumnPair.Key.Width.DisplayValue + adjustment));
            }

            return remainingAdjustment;
        }

        private void AutoGenerateColumnsPrivate()
        {
            if (!_measured || (_autoGeneratingColumnOperationCount > 0))
            {
                // Reading the DataType when we generate columns could cause the CollectionView to
                // raise a Reset if its Enumeration changed.  In that case, we don't want to generate again.
                return;
            }

            _autoGeneratingColumnOperationCount++;
            try
            {
                // Always remove existing auto-generated columns before generating new ones
                RemoveAutoGeneratedColumns();
                GenerateColumnsFromProperties();
                EnsureRowsPresenterVisibility();
                InvalidateRowHeightEstimate();
            }
            finally
            {
                _autoGeneratingColumnOperationCount--;
            }
        }

        private bool ComputeDisplayedColumns()
        {
            bool invalidate = false;
            int visibleScrollingColumnsTmp = 0;
            double displayWidth = this.CellsWidth;
            double cx = 0;
            int firstDisplayedFrozenCol = -1;
            int firstDisplayedScrollingCol = this.DisplayData.FirstDisplayedScrollingCol;

            // the same problem with negative numbers:
            // if the width passed in is negative, then return 0
            if (displayWidth <= 0 || this.ColumnsInternal.VisibleColumnCount == 0)
            {
                this.DisplayData.FirstDisplayedScrollingCol = -1;
                this.DisplayData.LastTotallyDisplayedScrollingCol = -1;
                return invalidate;
            }

            foreach (DataGridColumn dataGridColumn in this.ColumnsInternal.GetVisibleFrozenColumns())
            {
                if (firstDisplayedFrozenCol == -1)
                {
                    firstDisplayedFrozenCol = dataGridColumn.Index;
                }

                cx += GetEdgedColumnWidth(dataGridColumn);
                if (cx >= displayWidth)
                {
                    break;
                }
            }

            DiagnosticsDebug.Assert(cx <= this.ColumnsInternal.GetVisibleFrozenEdgedColumnsWidth(), "cx smaller than or equal to ColumnsInternal.GetVisibleFrozenEdgedColumnsWidth().");

            if (cx < displayWidth && firstDisplayedScrollingCol >= 0)
            {
                DataGridColumn dataGridColumn = this.ColumnsItemsInternal[firstDisplayedScrollingCol];
                if (dataGridColumn.IsFrozen)
                {
                    dataGridColumn = this.ColumnsInternal.FirstVisibleScrollingColumn;
                    _negHorizontalOffset = 0;
                    if (dataGridColumn == null)
                    {
                        this.DisplayData.FirstDisplayedScrollingCol = this.DisplayData.LastTotallyDisplayedScrollingCol = -1;
                        return invalidate;
                    }
                    else
                    {
                        firstDisplayedScrollingCol = dataGridColumn.Index;
                    }
                }

                cx -= _negHorizontalOffset;
                while (cx < displayWidth && dataGridColumn != null)
                {
                    cx += GetEdgedColumnWidth(dataGridColumn);
                    visibleScrollingColumnsTmp++;
                    dataGridColumn = this.ColumnsInternal.GetNextVisibleColumn(dataGridColumn);
                }

                var numVisibleScrollingCols = visibleScrollingColumnsTmp;

                // if we inflate the data area then we paint columns to the left of firstDisplayedScrollingCol
                if (cx < displayWidth)
                {
                    DiagnosticsDebug.Assert(firstDisplayedScrollingCol >= 0, "Expected positive firstDisplayedScrollingCol.");

                    // first minimize value of _negHorizontalOffset
                    if (_negHorizontalOffset > 0)
                    {
                        invalidate = true;
                        if (displayWidth - cx > _negHorizontalOffset)
                        {
                            cx += _negHorizontalOffset;
                            _horizontalOffset -= _negHorizontalOffset;
                            if (_horizontalOffset < DATAGRID_roundingDelta)
                            {
                                // Snap to zero to avoid trying to partially scroll in first scrolled off column below
                                _horizontalOffset = 0;
                            }

                            _negHorizontalOffset = 0;
                        }
                        else
                        {
                            _horizontalOffset -= displayWidth - cx;
                            _negHorizontalOffset -= displayWidth - cx;
                            cx = displayWidth;
                        }

                        // Make sure the HorizontalAdjustment is not greater than the new HorizontalOffset
                        // since it would cause an assertion failure in DataGridCellsPresenter.ShouldDisplayCell
                        // called by DataGridCellsPresenter.MeasureOverride.
                        this.HorizontalAdjustment = Math.Min(this.HorizontalAdjustment, _horizontalOffset);
                    }

                    // second try to scroll entire columns
                    if (cx < displayWidth && _horizontalOffset > 0)
                    {
                        DiagnosticsDebug.Assert(_negHorizontalOffset == 0, "Expected _negHorizontalOffset equals 0.");
                        dataGridColumn = this.ColumnsInternal.GetPreviousVisibleScrollingColumn(this.ColumnsItemsInternal[firstDisplayedScrollingCol]);
                        while (dataGridColumn != null && cx + GetEdgedColumnWidth(dataGridColumn) <= displayWidth)
                        {
                            cx += GetEdgedColumnWidth(dataGridColumn);
                            visibleScrollingColumnsTmp++;
                            invalidate = true;
                            firstDisplayedScrollingCol = dataGridColumn.Index;
                            _horizontalOffset -= GetEdgedColumnWidth(dataGridColumn);
                            dataGridColumn = this.ColumnsInternal.GetPreviousVisibleScrollingColumn(dataGridColumn);
                        }
                    }

                    // third try to partially scroll in first scrolled off column
                    if (cx < displayWidth && _horizontalOffset > 0)
                    {
                        DiagnosticsDebug.Assert(_negHorizontalOffset == 0, "Expected _negHorizontalOffset equals 0.");
                        dataGridColumn = this.ColumnsInternal.GetPreviousVisibleScrollingColumn(this.ColumnsItemsInternal[firstDisplayedScrollingCol]);
                        DiagnosticsDebug.Assert(dataGridColumn != null, "Expected non-null dataGridColumn.");
                        DiagnosticsDebug.Assert(GetEdgedColumnWidth(dataGridColumn) > displayWidth - cx, "Expected GetEdgedColumnWidth(dataGridColumn) greater than displayWidth - cx.");
                        firstDisplayedScrollingCol = dataGridColumn.Index;
                        _negHorizontalOffset = GetEdgedColumnWidth(dataGridColumn) - displayWidth + cx;
                        _horizontalOffset -= displayWidth - cx;
                        visibleScrollingColumnsTmp++;
                        invalidate = true;
                        cx = displayWidth;
                        DiagnosticsDebug.Assert(_negHorizontalOffset == GetNegHorizontalOffsetFromHorizontalOffset(_horizontalOffset), "Expected _negHorizontalOffset equals GetNegHorizontalOffsetFromHorizontalOffset(_horizontalOffset).");
                    }

                    // update the number of visible columns to the new reality
                    DiagnosticsDebug.Assert(numVisibleScrollingCols <= visibleScrollingColumnsTmp, "Expected numVisibleScrollingCols less than or equal to visibleScrollingColumnsTmp.");
                    numVisibleScrollingCols = visibleScrollingColumnsTmp;
                }

                int jumpFromFirstVisibleScrollingCol = numVisibleScrollingCols - 1;
                if (cx > displayWidth)
                {
                    jumpFromFirstVisibleScrollingCol--;
                }

                DiagnosticsDebug.Assert(jumpFromFirstVisibleScrollingCol >= -1, "Expected jumpFromFirstVisibleScrollingCol greater than or equal to -1.");

                if (jumpFromFirstVisibleScrollingCol < 0)
                {
                    this.DisplayData.LastTotallyDisplayedScrollingCol = -1; // no totally visible scrolling column at all
                }
                else
                {
                    DiagnosticsDebug.Assert(firstDisplayedScrollingCol >= 0, "Expected positive firstDisplayedScrollingCol.");
                    dataGridColumn = this.ColumnsItemsInternal[firstDisplayedScrollingCol];
                    for (int jump = 0; jump < jumpFromFirstVisibleScrollingCol; jump++)
                    {
                        dataGridColumn = this.ColumnsInternal.GetNextVisibleColumn(dataGridColumn);
                        DiagnosticsDebug.Assert(dataGridColumn != null, "Expected non-null dataGridColumn.");
                    }

                    this.DisplayData.LastTotallyDisplayedScrollingCol = dataGridColumn.Index;
                }
            }
            else
            {
                this.DisplayData.LastTotallyDisplayedScrollingCol = -1;
            }

            this.DisplayData.FirstDisplayedScrollingCol = firstDisplayedScrollingCol;

            return invalidate;
        }

        private int ComputeFirstVisibleScrollingColumn()
        {
            if (this.ColumnsInternal.GetVisibleFrozenEdgedColumnsWidth() >= this.CellsWidth)
            {
                // Not enough room for scrolling columns.
                _negHorizontalOffset = 0;
                return -1;
            }

            DataGridColumn dataGridColumn = this.ColumnsInternal.FirstVisibleScrollingColumn;

            if (_horizontalOffset == 0)
            {
                _negHorizontalOffset = 0;
                return (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            }

            double cx = 0;
            while (dataGridColumn != null)
            {
                cx += GetEdgedColumnWidth(dataGridColumn);
                if (cx > _horizontalOffset)
                {
                    break;
                }

                dataGridColumn = this.ColumnsInternal.GetNextVisibleColumn(dataGridColumn);
            }

            if (dataGridColumn == null)
            {
                DiagnosticsDebug.Assert(cx <= _horizontalOffset, "Expected cx less than or equal to _horizontalOffset.");
                dataGridColumn = this.ColumnsInternal.FirstVisibleScrollingColumn;
                if (dataGridColumn == null)
                {
                    _negHorizontalOffset = 0;
                    return -1;
                }
                else
                {
                    if (_negHorizontalOffset != _horizontalOffset)
                    {
                        _negHorizontalOffset = 0;
                    }

                    return dataGridColumn.Index;
                }
            }
            else
            {
                _negHorizontalOffset = GetEdgedColumnWidth(dataGridColumn) - (cx - _horizontalOffset);
                return dataGridColumn.Index;
            }
        }

        private void CorrectColumnDisplayIndexesAfterDeletion(DataGridColumn deletedColumn)
        {
            // Column indexes have already been adjusted.
            // This column has already been detached and has retained its old Index and DisplayIndex
            DiagnosticsDebug.Assert(deletedColumn != null, "Expected non-null deletedColumn.");
            DiagnosticsDebug.Assert(deletedColumn.OwningGrid == null, "Expected null deletedColumn.OwningGrid.");
            DiagnosticsDebug.Assert(deletedColumn.Index >= 0, "Expected positive deletedColumn.Index.");
            DiagnosticsDebug.Assert(deletedColumn.DisplayIndexWithFiller >= 0, "Expected positive deletedColumn.DisplayIndexWithFiller.");

            try
            {
                InDisplayIndexAdjustments = true;

                // The DisplayIndex of columns greater than the deleted column need to be decremented,
                // as do the DisplayIndexMap values of modified column Indexes
                DataGridColumn column;
                this.ColumnsInternal.DisplayIndexMap.RemoveAt(deletedColumn.DisplayIndexWithFiller);
                for (int displayIndex = 0; displayIndex < this.ColumnsInternal.DisplayIndexMap.Count; displayIndex++)
                {
                    if (this.ColumnsInternal.DisplayIndexMap[displayIndex] > deletedColumn.Index)
                    {
                        this.ColumnsInternal.DisplayIndexMap[displayIndex]--;
                    }

                    if (displayIndex >= deletedColumn.DisplayIndexWithFiller)
                    {
                        column = this.ColumnsInternal.GetColumnAtDisplayIndex(displayIndex);
                        column.DisplayIndexWithFiller = column.DisplayIndexWithFiller - 1;
                        column.DisplayIndexHasChanged = true; // OnColumnDisplayIndexChanged needs to be raised later on
                    }
                }

#if DEBUG
                DiagnosticsDebug.Assert(this.ColumnsInternal.Debug_VerifyColumnDisplayIndexes(), "Expected ColumnsInternal.Debug_VerifyColumnDisplayIndexes() is true.");
#endif

                // Now raise all the OnColumnDisplayIndexChanged events
                FlushDisplayIndexChanged(true /*raiseEvent*/);
            }
            finally
            {
                InDisplayIndexAdjustments = false;
                FlushDisplayIndexChanged(false /*raiseEvent*/);
            }
        }

        private void CorrectColumnDisplayIndexesAfterInsertion(DataGridColumn insertedColumn)
        {
            DiagnosticsDebug.Assert(insertedColumn != null, "Expected non-null insertedColumn.");
            DiagnosticsDebug.Assert(insertedColumn.OwningGrid == this, "Expected insertedColumn.OwningGrid equals this DataGrid.");
            if (insertedColumn.DisplayIndexWithFiller == -1 || insertedColumn.DisplayIndexWithFiller >= this.ColumnsItemsInternal.Count)
            {
                // Developer did not assign a DisplayIndex or picked a large number.
                // Choose the Index as the DisplayIndex.
                insertedColumn.DisplayIndexWithFiller = insertedColumn.Index;
            }

            try
            {
                InDisplayIndexAdjustments = true;

                // The DisplayIndex of columns greater than the inserted column need to be incremented,
                // as do the DisplayIndexMap values of modified column Indexes
                DataGridColumn column;
                for (int displayIndex = 0; displayIndex < this.ColumnsInternal.DisplayIndexMap.Count; displayIndex++)
                {
                    if (this.ColumnsInternal.DisplayIndexMap[displayIndex] >= insertedColumn.Index)
                    {
                        this.ColumnsInternal.DisplayIndexMap[displayIndex]++;
                    }

                    if (displayIndex >= insertedColumn.DisplayIndexWithFiller)
                    {
                        column = this.ColumnsInternal.GetColumnAtDisplayIndex(displayIndex);
                        column.DisplayIndexWithFiller++;
                        column.DisplayIndexHasChanged = true; // OnColumnDisplayIndexChanged needs to be raised later on
                    }
                }

                this.ColumnsInternal.DisplayIndexMap.Insert(insertedColumn.DisplayIndexWithFiller, insertedColumn.Index);

#if DEBUG
                DiagnosticsDebug.Assert(this.ColumnsInternal.Debug_VerifyColumnDisplayIndexes(), "Expected ColumnsInternal.Debug_VerifyColumnDisplayIndexes() is true.");
#endif

                // Now raise all the OnColumnDisplayIndexChanged events
                FlushDisplayIndexChanged(true /*raiseEvent*/);
            }
            finally
            {
                InDisplayIndexAdjustments = false;
                FlushDisplayIndexChanged(false /*raiseEvent*/);
            }
        }

        private void CorrectColumnFrozenStates()
        {
            int index = 0;
            double frozenColumnWidth = 0;
            double oldFrozenColumnWidth = 0;
            foreach (DataGridColumn column in this.ColumnsInternal.GetDisplayedColumns())
            {
                if (column.IsFrozen)
                {
                    oldFrozenColumnWidth += column.ActualWidth;
                }

                column.IsFrozen = index < this.FrozenColumnCountWithFiller;
                if (column.IsFrozen)
                {
                    frozenColumnWidth += column.ActualWidth;
                }

                index++;
            }

            if (this.HorizontalOffset > Math.Max(0, frozenColumnWidth - oldFrozenColumnWidth))
            {
                UpdateHorizontalOffset(this.HorizontalOffset - frozenColumnWidth + oldFrozenColumnWidth);
            }
            else
            {
                UpdateHorizontalOffset(0);
            }
        }

        private void CorrectColumnIndexesAfterDeletion(DataGridColumn deletedColumn)
        {
            DiagnosticsDebug.Assert(deletedColumn != null, "Expected non-null deletedColumn.");
            for (int columnIndex = deletedColumn.Index; columnIndex < this.ColumnsItemsInternal.Count; columnIndex++)
            {
                this.ColumnsItemsInternal[columnIndex].Index = this.ColumnsItemsInternal[columnIndex].Index - 1;
                DiagnosticsDebug.Assert(this.ColumnsItemsInternal[columnIndex].Index == columnIndex, "Expected ColumnsItemsInternal[columnIndex].Index equals columnIndex.");
            }
        }

        private void CorrectColumnIndexesAfterInsertion(DataGridColumn insertedColumn, int insertionCount)
        {
            DiagnosticsDebug.Assert(insertedColumn != null, "Expected non-null insertedColumn.");
            DiagnosticsDebug.Assert(insertionCount > 0, "Expected strictly positive insertionCount.");
            for (int columnIndex = insertedColumn.Index + insertionCount; columnIndex < this.ColumnsItemsInternal.Count; columnIndex++)
            {
                this.ColumnsItemsInternal[columnIndex].Index = columnIndex;
            }
        }

        /// <summary>
        /// Decreases the widths of all non-star columns with DisplayIndex >= displayIndex such that the total
        /// width is decreased by the given amount, if possible.  If the total desired adjustment amount
        /// could not be met, the remaining amount of adjustment is returned.  The adjustment stops when
        /// the column's target width has been met.
        /// </summary>
        /// <param name="displayIndex">Starting column DisplayIndex.</param>
        /// <param name="targetWidth">The target width of the column (in pixels).</param>
        /// <param name="amount">Amount to decrease (in pixels).</param>
        /// <param name="reverse">Whether or not to reverse the order in which the columns are traversed.</param>
        /// <param name="affectNewColumns">Whether or not to adjust widths of columns that do not yet have their initial desired width.</param>
        /// <returns>The remaining amount of adjustment.</returns>
        private double DecreaseNonStarColumnWidths(int displayIndex, Func<DataGridColumn, double> targetWidth, double amount, bool reverse, bool affectNewColumns)
        {
            if (DoubleUtil.GreaterThanOrClose(amount, 0))
            {
                return amount;
            }

            foreach (DataGridColumn column in this.ColumnsInternal.GetDisplayedColumns(
                reverse,
                column =>
                    column.IsVisible &&
                    column.Width.UnitType != DataGridLengthUnitType.Star &&
                    column.DisplayIndex >= displayIndex &&
                    column.ActualCanUserResize &&
                    (affectNewColumns || column.IsInitialDesiredWidthDetermined)))
            {
                amount = DecreaseNonStarColumnWidth(column, Math.Max(column.ActualMinWidth, targetWidth(column)), amount);
                if (DoubleUtil.IsZero(amount))
                {
                    break;
                }
            }

            return amount;
        }

        private void FlushDisplayIndexChanged(bool raiseEvent)
        {
            foreach (DataGridColumn column in this.ColumnsItemsInternal)
            {
                if (column.DisplayIndexHasChanged)
                {
                    column.DisplayIndexHasChanged = false;
                    if (raiseEvent)
                    {
                        DiagnosticsDebug.Assert(column != this.ColumnsInternal.RowGroupSpacerColumn, "Expected column other than ColumnsInternal.RowGroupSpacerColumn.");
                        OnColumnDisplayIndexChanged(column);
                    }
                }
            }
        }

        private void GenerateColumnsFromProperties()
        {
            // Auto-generated Columns are added at the end so the user columns appear first
            if (this.DataConnection.DataProperties != null && this.DataConnection.DataProperties.Length > 0)
            {
                List<KeyValuePair<int, DataGridAutoGeneratingColumnEventArgs>> columnOrderPairs = new List<KeyValuePair<int, DataGridAutoGeneratingColumnEventArgs>>();

                // Generate the columns
                foreach (PropertyInfo propertyInfo in this.DataConnection.DataProperties)
                {
                    string columnHeader = propertyInfo.Name;
                    int columnOrder = DATAGRID_defaultColumnDisplayOrder;

                    // Check if DisplayAttribute is defined on the property
                    DisplayAttribute displayAttribute = propertyInfo.GetCustomAttributes().OfType<DisplayAttribute>().FirstOrDefault();
                    if (displayAttribute != null)
                    {
                        bool? autoGenerateField = displayAttribute.GetAutoGenerateField();
                        if (autoGenerateField.HasValue && autoGenerateField.Value == false)
                        {
                            // Abort column generation because we aren't supposed to auto-generate this field
                            continue;
                        }

                        string header = displayAttribute.GetShortName();
                        if (header != null)
                        {
                            columnHeader = header;
                        }

                        int? order = displayAttribute.GetOrder();
                        if (order.HasValue)
                        {
                            columnOrder = order.Value;
                        }
                    }

                    // Generate a single column and determine its relative order
                    int insertIndex = 0;
                    if (columnOrder == int.MaxValue)
                    {
                        insertIndex = columnOrderPairs.Count;
                    }
                    else
                    {
                        foreach (KeyValuePair<int, DataGridAutoGeneratingColumnEventArgs> columnOrderPair in columnOrderPairs)
                        {
                            if (columnOrderPair.Key > columnOrder)
                            {
                                break;
                            }

                            insertIndex++;
                        }
                    }

                    DataGridAutoGeneratingColumnEventArgs columnArgs = GenerateColumn(propertyInfo.PropertyType, propertyInfo.Name, columnHeader);
                    columnOrderPairs.Insert(insertIndex, new KeyValuePair<int, DataGridAutoGeneratingColumnEventArgs>(columnOrder, columnArgs));
                }

                // Add the columns to the DataGrid in the correct order
                foreach (KeyValuePair<int, DataGridAutoGeneratingColumnEventArgs> columnOrderPair in columnOrderPairs)
                {
                    AddGeneratedColumn(columnOrderPair.Value);
                }
            }
            else if (this.DataConnection.DataIsPrimitive)
            {
                AddGeneratedColumn(GenerateColumn(this.DataConnection.DataType, string.Empty, this.DataConnection.DataType.Name));
            }
        }

        private bool GetColumnEffectiveReadOnlyState(DataGridColumn dataGridColumn)
        {
            DiagnosticsDebug.Assert(dataGridColumn != null, "Expected non-null dataGridColumn.");

            return this.IsReadOnly || dataGridColumn.IsReadOnly || dataGridColumn is DataGridFillerColumn;
        }

        /// <summary>
        ///      Returns the absolute coordinate of the left edge of the given column (including
        ///      the potential gridline - that is the left edge of the gridline is returned). Note that
        ///      the column does not need to be in the display area.
        /// </summary>
        /// <returns>Absolute coordinate of the left edge of the given column.</returns>
        private double GetColumnXFromIndex(int index)
        {
            DiagnosticsDebug.Assert(index < this.ColumnsItemsInternal.Count, "Expected index smaller than this.ColumnsItemsInternal.Count.");
            DiagnosticsDebug.Assert(this.ColumnsItemsInternal[index].IsVisible, "Expected ColumnsItemsInternal[index].IsVisible is true.");

            double x = 0;
            foreach (DataGridColumn column in this.ColumnsInternal.GetVisibleColumns())
            {
                if (index == column.Index)
                {
                    break;
                }

                x += GetEdgedColumnWidth(column);
            }

            return x;
        }

        private double GetNegHorizontalOffsetFromHorizontalOffset(double horizontalOffset)
        {
            foreach (DataGridColumn column in this.ColumnsInternal.GetVisibleScrollingColumns())
            {
                if (GetEdgedColumnWidth(column) > horizontalOffset)
                {
                    break;
                }

                horizontalOffset -= GetEdgedColumnWidth(column);
            }

            return horizontalOffset;
        }

        /// <summary>
        /// Increases the widths of all non-star columns with DisplayIndex >= displayIndex such that the total
        /// width is increased by the given amount, if possible.  If the total desired adjustment amount
        /// could not be met, the remaining amount of adjustment is returned.  The adjustment stops when
        /// the column's target width has been met.
        /// </summary>
        /// <param name="displayIndex">Starting column DisplayIndex.</param>
        /// <param name="targetWidth">The target width of the column (in pixels).</param>
        /// <param name="amount">Amount to increase (in pixels).</param>
        /// <param name="reverse">Whether or not to reverse the order in which the columns are traversed.</param>
        /// <param name="affectNewColumns">Whether or not to adjust widths of columns that do not yet have their initial desired width.</param>
        /// <returns>The remaining amount of adjustment.</returns>
        private double IncreaseNonStarColumnWidths(int displayIndex, Func<DataGridColumn, double> targetWidth, double amount, bool reverse, bool affectNewColumns)
        {
            if (DoubleUtil.LessThanOrClose(amount, 0))
            {
                return amount;
            }

            foreach (DataGridColumn column in this.ColumnsInternal.GetDisplayedColumns(
                reverse,
                column =>
                    column.IsVisible &&
                    column.Width.UnitType != DataGridLengthUnitType.Star &&
                    column.DisplayIndex >= displayIndex &&
                    column.ActualCanUserResize &&
                    (affectNewColumns || column.IsInitialDesiredWidthDetermined)))
            {
                amount = IncreaseNonStarColumnWidth(column, Math.Min(column.ActualMaxWidth, targetWidth(column)), amount);
                if (DoubleUtil.IsZero(amount))
                {
                    break;
                }
            }

            return amount;
        }

        private void InsertDisplayedColumnHeader(DataGridColumn dataGridColumn)
        {
            DiagnosticsDebug.Assert(dataGridColumn != null, "Expected non-null dataGridColumn.");
            if (_columnHeadersPresenter != null)
            {
                dataGridColumn.HeaderCell.Visibility = dataGridColumn.Visibility;
                DiagnosticsDebug.Assert(!_columnHeadersPresenter.Children.Contains(dataGridColumn.HeaderCell), "Expected dataGridColumn.HeaderCell not contained in _columnHeadersPresenter.Children.");
                _columnHeadersPresenter.Children.Insert(dataGridColumn.DisplayIndexWithFiller, dataGridColumn.HeaderCell);
            }
        }

        private void RemoveAutoGeneratedColumns()
        {
            int index = 0;
            _autoGeneratingColumnOperationCount++;
            try
            {
                while (index < this.ColumnsInternal.Count)
                {
                    // Skip over the user columns
                    while (index < this.ColumnsInternal.Count && !this.ColumnsInternal[index].IsAutoGenerated)
                    {
                        index++;
                    }

                    // Remove the auto-generated columns
                    while (index < this.ColumnsInternal.Count && this.ColumnsInternal[index].IsAutoGenerated)
                    {
                        this.ColumnsInternal.RemoveAt(index);
                    }
                }

                this.ColumnsInternal.AutogeneratedColumnCount = 0;
            }
            finally
            {
                _autoGeneratingColumnOperationCount--;
            }
        }

        private bool ScrollColumnIntoView(int columnIndex)
        {
            DiagnosticsDebug.Assert(columnIndex >= 0, "Expected positive columnIndex.");
            DiagnosticsDebug.Assert(columnIndex < this.ColumnsItemsInternal.Count, "Expected columnIndex smaller than this.ColumnsItemsInternal.Count.");

            if (this.DisplayData.FirstDisplayedScrollingCol != -1 &&
                !this.ColumnsItemsInternal[columnIndex].IsFrozen &&
                (columnIndex != this.DisplayData.FirstDisplayedScrollingCol || _negHorizontalOffset > 0))
            {
                int columnsToScroll;
                if (this.ColumnsInternal.DisplayInOrder(columnIndex, this.DisplayData.FirstDisplayedScrollingCol))
                {
                    columnsToScroll = this.ColumnsInternal.GetColumnCount(true /*isVisible*/, false /*isFrozen*/, columnIndex, this.DisplayData.FirstDisplayedScrollingCol);
                    if (_negHorizontalOffset > 0)
                    {
                        columnsToScroll++;
                    }

                    ScrollColumns(-columnsToScroll);
                }
                else if (columnIndex == this.DisplayData.FirstDisplayedScrollingCol && _negHorizontalOffset > 0)
                {
                    ScrollColumns(-1);
                }
                else if (this.DisplayData.LastTotallyDisplayedScrollingCol == -1 ||
                         (this.DisplayData.LastTotallyDisplayedScrollingCol != columnIndex &&
                          this.ColumnsInternal.DisplayInOrder(this.DisplayData.LastTotallyDisplayedScrollingCol, columnIndex)))
                {
                    double xColumnLeftEdge = GetColumnXFromIndex(columnIndex);
                    double xColumnRightEdge = xColumnLeftEdge + GetEdgedColumnWidth(this.ColumnsItemsInternal[columnIndex]);
                    double change = xColumnRightEdge - this.HorizontalOffset - this.CellsWidth;
                    double widthRemaining = change;

                    DataGridColumn newFirstDisplayedScrollingCol = this.ColumnsItemsInternal[this.DisplayData.FirstDisplayedScrollingCol];
                    DataGridColumn nextColumn = this.ColumnsInternal.GetNextVisibleColumn(newFirstDisplayedScrollingCol);
                    double newColumnWidth = GetEdgedColumnWidth(newFirstDisplayedScrollingCol) - _negHorizontalOffset;
                    while (nextColumn != null && widthRemaining >= newColumnWidth)
                    {
                        widthRemaining -= newColumnWidth;
                        newFirstDisplayedScrollingCol = nextColumn;
                        newColumnWidth = GetEdgedColumnWidth(newFirstDisplayedScrollingCol);
                        nextColumn = this.ColumnsInternal.GetNextVisibleColumn(newFirstDisplayedScrollingCol);
                        _negHorizontalOffset = 0;
                    }

                    _negHorizontalOffset += widthRemaining;
                    this.DisplayData.LastTotallyDisplayedScrollingCol = columnIndex;
                    if (newFirstDisplayedScrollingCol.Index == columnIndex)
                    {
                        _negHorizontalOffset = 0;
                        double frozenColumnWidth = this.ColumnsInternal.GetVisibleFrozenEdgedColumnsWidth();

                        // If the entire column cannot be displayed, we want to start showing it from its LeftEdge.
                        if (newColumnWidth > (this.CellsWidth - frozenColumnWidth))
                        {
                            this.DisplayData.LastTotallyDisplayedScrollingCol = -1;
                            change = xColumnLeftEdge - this.HorizontalOffset - frozenColumnWidth;
                        }
                    }

                    this.DisplayData.FirstDisplayedScrollingCol = newFirstDisplayedScrollingCol.Index;

                    // At this point DisplayData.FirstDisplayedScrollingColumn and LastDisplayedScrollingColumn should be correct.
                    if (change != 0)
                    {
                        UpdateHorizontalOffset(this.HorizontalOffset + change);
                    }
                }
            }

            return true;
        }

        private void ScrollColumns(int columns)
        {
            DataGridColumn newFirstVisibleScrollingCol = null;
            DataGridColumn dataGridColumnTmp;
            int colCount = 0;
            if (columns > 0)
            {
                if (this.DisplayData.LastTotallyDisplayedScrollingCol >= 0)
                {
                    dataGridColumnTmp = this.ColumnsItemsInternal[this.DisplayData.LastTotallyDisplayedScrollingCol];
                    while (colCount < columns && dataGridColumnTmp != null)
                    {
                        dataGridColumnTmp = this.ColumnsInternal.GetNextVisibleColumn(dataGridColumnTmp);
                        colCount++;
                    }

                    if (dataGridColumnTmp == null)
                    {
                        // no more column to display on the right of the last totally seen column
                        return;
                    }
                }

                DiagnosticsDebug.Assert(this.DisplayData.FirstDisplayedScrollingCol >= 0, "Expected positive DisplayData.FirstDisplayedScrollingCol.");
                dataGridColumnTmp = this.ColumnsItemsInternal[this.DisplayData.FirstDisplayedScrollingCol];
                colCount = 0;
                while (colCount < columns && dataGridColumnTmp != null)
                {
                    dataGridColumnTmp = this.ColumnsInternal.GetNextVisibleColumn(dataGridColumnTmp);
                    colCount++;
                }

                newFirstVisibleScrollingCol = dataGridColumnTmp;
            }

            if (columns < 0)
            {
                DiagnosticsDebug.Assert(this.DisplayData.FirstDisplayedScrollingCol >= 0, "Expected positive DisplayData.FirstDisplayedScrollingCol.");
                dataGridColumnTmp = this.ColumnsItemsInternal[this.DisplayData.FirstDisplayedScrollingCol];
                if (_negHorizontalOffset > 0)
                {
                    colCount++;
                }

                while (colCount < -columns && dataGridColumnTmp != null)
                {
                    dataGridColumnTmp = this.ColumnsInternal.GetPreviousVisibleScrollingColumn(dataGridColumnTmp);
                    colCount++;
                }

                newFirstVisibleScrollingCol = dataGridColumnTmp;
                if (newFirstVisibleScrollingCol == null)
                {
                    if (_negHorizontalOffset == 0)
                    {
                        // no more column to display on the left of the first seen column
                        return;
                    }
                    else
                    {
                        newFirstVisibleScrollingCol = this.ColumnsItemsInternal[this.DisplayData.FirstDisplayedScrollingCol];
                    }
                }
            }

            double newColOffset = 0;
            foreach (DataGridColumn dataGridColumn in this.ColumnsInternal.GetVisibleScrollingColumns())
            {
                if (dataGridColumn == newFirstVisibleScrollingCol)
                {
                    break;
                }

                newColOffset += GetEdgedColumnWidth(dataGridColumn);
            }

            UpdateHorizontalOffset(newColOffset);
        }

        private void UpdateDisplayedColumns()
        {
            this.DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
            ComputeDisplayedColumns();
        }
    }
}