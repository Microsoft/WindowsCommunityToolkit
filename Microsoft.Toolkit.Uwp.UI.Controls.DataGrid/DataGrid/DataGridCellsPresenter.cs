// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using Microsoft.Toolkit.Uwp.Utilities;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

using DiagnosticsDebug = System.Diagnostics.Debug;

namespace Microsoft.Toolkit.Uwp.UI.Controls.Primitives
{
    /// <summary>
    /// Used within the template of a <see cref="DataGrid"/>
    /// to specify the location in the control's visual tree where the cells are to be added.
    /// </summary>
    public sealed class DataGridCellsPresenter : Panel
    {
        private double _fillerLeftEdge;

        // The desired height needs to be cached due to column virtualization; otherwise, the cells
        // would grow and shrink as the DataGrid scrolls horizontally
        private double DesiredHeight
        {
            get;
            set;
        }

        private DataGrid OwningGrid
        {
            get
            {
                if (this.OwningRow != null)
                {
                    return this.OwningRow.OwningGrid;
                }

                return null;
            }
        }

        internal DataGridRow OwningRow
        {
            get;
            set;
        }

        /// <summary>
        /// Arranges the content of the <see cref="T:System.Windows.Controls.Primitives.DataGridCellsPresenter"/>.
        /// </summary>
        /// <returns>
        /// The actual size used by the <see cref="T:System.Windows.Controls.Primitives.DataGridCellsPresenter"/>.
        /// </returns>
        /// <param name="finalSize">
        /// The final area within the parent that this element should use to arrange itself and its children.
        /// </param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.OwningGrid == null)
            {
                return base.ArrangeOverride(finalSize);
            }

            if (this.OwningGrid.AutoSizingColumns)
            {
                // When we initially load an auto-column, we have to wait for all the rows to be measured
                // before we know its final desired size.  We need to trigger a new round of measures now
                // that the final sizes have been calculated.
                this.OwningGrid.AutoSizingColumns = false;
                return base.ArrangeOverride(finalSize);
            }

            double frozenLeftEdge = 0;
            double scrollingLeftEdge = -this.OwningGrid.HorizontalOffset;

            double cellLeftEdge;
            foreach (DataGridColumn column in this.OwningGrid.ColumnsInternal.GetVisibleColumns())
            {
                DataGridCell cell = this.OwningRow.Cells[column.Index];
                DiagnosticsDebug.Assert(cell.OwningColumn == column, "Expected column owner.");
                DiagnosticsDebug.Assert(column.IsVisible, "Expected visible column.");

                if (column.IsFrozen)
                {
                    cellLeftEdge = frozenLeftEdge;

                    // This can happen before or after clipping because frozen cells aren't clipped
                    frozenLeftEdge += column.ActualWidth;
                }
                else
                {
                    cellLeftEdge = scrollingLeftEdge;
                }

                if (cell.Visibility == Visibility.Visible)
                {
                    cell.Arrange(new Rect(cellLeftEdge, 0, column.LayoutRoundedWidth, finalSize.Height));
                    EnsureCellClip(cell, column.ActualWidth, finalSize.Height, frozenLeftEdge, scrollingLeftEdge);
                }

                scrollingLeftEdge += column.ActualWidth;
                column.IsInitialDesiredWidthDetermined = true;
            }

            _fillerLeftEdge = scrollingLeftEdge;

            // FillerColumn.Width == 0 when the filler column is not active
            this.OwningRow.FillerCell.Arrange(new Rect(_fillerLeftEdge, 0, this.OwningGrid.ColumnsInternal.FillerColumn.FillerWidth, finalSize.Height));

            return finalSize;
        }

        private static void EnsureCellClip(DataGridCell cell, double width, double height, double frozenLeftEdge, double cellLeftEdge)
        {
            // Clip the cell only if it's scrolled under frozen columns.  Unfortunately, we need to clip in this case
            // because cells could be transparent
            if (!cell.OwningColumn.IsFrozen && frozenLeftEdge > cellLeftEdge)
            {
                RectangleGeometry rg = new RectangleGeometry();
                double xClip = Math.Round(Math.Min(width, frozenLeftEdge - cellLeftEdge));
                rg.Rect = new Rect(xClip, 0, Math.Max(0, width - xClip), height);
                cell.Clip = rg;
            }
            else
            {
                cell.Clip = null;
            }
        }

        private static void EnsureCellDisplay(DataGridCell cell, bool displayColumn)
        {
            if (cell.IsCurrent)
            {
                if (displayColumn)
                {
                    cell.Visibility = Visibility.Visible;
                    cell.Clip = null;
                }
                else
                {
                    // Clip
                    RectangleGeometry rg = new RectangleGeometry();
                    rg.Rect = Rect.Empty;
                    cell.Clip = rg;
                }
            }
            else
            {
                cell.Visibility = displayColumn ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        internal void EnsureFillerVisibility()
        {
            DataGridFillerColumn fillerColumn = this.OwningGrid.ColumnsInternal.FillerColumn;
            Visibility newVisibility = fillerColumn.IsActive ? Visibility.Visible : Visibility.Collapsed;
            if (this.OwningRow.FillerCell.Visibility != newVisibility)
            {
                this.OwningRow.FillerCell.Visibility = newVisibility;
                if (newVisibility == Visibility.Visible)
                {
                    this.OwningRow.FillerCell.Arrange(new Rect(_fillerLeftEdge, 0, fillerColumn.FillerWidth, this.ActualHeight));
                }
            }

            // This must be done after the Filler visibility is determined.  This also must be done
            // regardless of whether or not the filler visibility actually changed values because
            // we could scroll in a cell that didn't have EnsureGridLine called yet
            DataGridColumn lastVisibleColumn = this.OwningGrid.ColumnsInternal.LastVisibleColumn;
            if (lastVisibleColumn != null)
            {
                DataGridCell cell = this.OwningRow.Cells[lastVisibleColumn.Index];
                cell.EnsureGridLine(lastVisibleColumn);
            }
        }

        /// <summary>
        /// Measures the children of a <see cref="T:System.Windows.Controls.Primitives.DataGridCellsPresenter"/> to
        /// prepare for arranging them during the <see cref="M:System.Windows.FrameworkElement.ArrangeOverride(System.Windows.Size)"/> pass.
        /// </summary>
        /// <param name="availableSize">
        /// The available size that this element can give to child elements. Indicates an upper limit that child elements should not exceed.
        /// </param>
        /// <returns>
        /// The size that the <see cref="T:System.Windows.Controls.Primitives.DataGridCellsPresenter"/> determines it needs during layout, based on its calculations of child object allocated sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.OwningGrid == null)
            {
                return base.MeasureOverride(availableSize);
            }

            bool autoSizeHeight;
            double measureHeight;
            if (double.IsNaN(this.OwningGrid.RowHeight))
            {
                // No explicit height values were set so we can autosize
                autoSizeHeight = true;
                measureHeight = double.PositiveInfinity;
            }
            else
            {
                this.DesiredHeight = this.OwningGrid.RowHeight;
                measureHeight = this.DesiredHeight;
                autoSizeHeight = false;
            }

            double frozenLeftEdge = 0;
            double totalDisplayWidth = 0;
            double scrollingLeftEdge = -this.OwningGrid.HorizontalOffset;
            this.OwningGrid.ColumnsInternal.EnsureVisibleEdgedColumnsWidth();
            DataGridColumn lastVisibleColumn = this.OwningGrid.ColumnsInternal.LastVisibleColumn;
            foreach (DataGridColumn column in this.OwningGrid.ColumnsInternal.GetVisibleColumns())
            {
                DataGridCell cell = this.OwningRow.Cells[column.Index];

                // Measure the entire first row to make the horizontal scrollbar more accurate
                bool shouldDisplayCell = ShouldDisplayCell(column, frozenLeftEdge, scrollingLeftEdge) || this.OwningRow.Index == 0;
                EnsureCellDisplay(cell, shouldDisplayCell);
                if (shouldDisplayCell)
                {
                    DataGridLength columnWidth = column.Width;
                    bool autoGrowWidth = columnWidth.IsSizeToCells || columnWidth.IsAuto;
                    if (column != lastVisibleColumn)
                    {
                        cell.EnsureGridLine(lastVisibleColumn);
                    }

                    // If we're not using star sizing or the current column can't be resized,
                    // then just set the display width according to the column's desired width
                    if (!this.OwningGrid.UsesStarSizing || (!column.ActualCanUserResize && !column.Width.IsStar))
                    {
                        // In the edge-case where we're given infinite width and we have star columns, the
                        // star columns grow to their predefined limit of 10,000 (or their MaxWidth)
                        double newDisplayWidth = column.Width.IsStar ?
                            Math.Min(column.ActualMaxWidth, DataGrid.DATAGRID_maximumStarColumnWidth) :
                            Math.Max(column.ActualMinWidth, Math.Min(column.ActualMaxWidth, column.Width.DesiredValue));
                        column.SetWidthDisplayValue(newDisplayWidth);
                    }

                    // If we're auto-growing the column based on the cell content, we want to measure it at its maximum value
                    if (autoGrowWidth)
                    {
                        cell.Measure(new Size(column.ActualMaxWidth, measureHeight));
                        this.OwningGrid.AutoSizeColumn(column, cell.DesiredSize.Width);
                        column.ComputeLayoutRoundedWidth(totalDisplayWidth);
                    }
                    else if (!this.OwningGrid.UsesStarSizing)
                    {
                        column.ComputeLayoutRoundedWidth(scrollingLeftEdge);
                        cell.Measure(new Size(column.LayoutRoundedWidth, measureHeight));
                    }

                    // We need to track the largest height in order to auto-size
                    if (autoSizeHeight)
                    {
                        this.DesiredHeight = Math.Max(this.DesiredHeight, cell.DesiredSize.Height);
                    }
                }

                if (column.IsFrozen)
                {
                    frozenLeftEdge += column.ActualWidth;
                }

                scrollingLeftEdge += column.ActualWidth;
                totalDisplayWidth += column.ActualWidth;
            }

            // If we're using star sizing (and we're not waiting for an auto-column to finish growing)
            // then we will resize all the columns to fit the available space.
            if (this.OwningGrid.UsesStarSizing && !this.OwningGrid.AutoSizingColumns)
            {
                double adjustment = this.OwningGrid.CellsWidth - totalDisplayWidth;
                this.OwningGrid.AdjustColumnWidths(0, adjustment, false);

                // Since we didn't know the final widths of the columns until we resized,
                // we waited until now to measure each cell
                double leftEdge = 0;
                foreach (DataGridColumn column in this.OwningGrid.ColumnsInternal.GetVisibleColumns())
                {
                    DataGridCell cell = this.OwningRow.Cells[column.Index];
                    column.ComputeLayoutRoundedWidth(leftEdge);
                    cell.Measure(new Size(column.LayoutRoundedWidth, measureHeight));
                    if (autoSizeHeight)
                    {
                        this.DesiredHeight = Math.Max(this.DesiredHeight, cell.DesiredSize.Height);
                    }

                    leftEdge += column.ActualWidth;
                }
            }

            // Measure FillerCell, we're doing it unconditionally here because we don't know if we'll need the filler
            // column and we don't want to cause another Measure if we do
            this.OwningRow.FillerCell.Measure(new Size(double.PositiveInfinity, this.DesiredHeight));

            this.OwningGrid.ColumnsInternal.EnsureVisibleEdgedColumnsWidth();
            return new Size(this.OwningGrid.ColumnsInternal.VisibleEdgedColumnsWidth, this.DesiredHeight);
        }

        internal void Recycle()
        {
            // Clear out the cached desired height so it is not reused for other rows
            this.DesiredHeight = 0;

            if (this.OwningGrid != null && this.OwningRow != null)
            {
                foreach (DataGridColumn column in this.OwningGrid.ColumnsInternal)
                {
                    this.OwningRow.Cells[column.Index].Recycle();
                }
            }
        }

        private bool ShouldDisplayCell(DataGridColumn column, double frozenLeftEdge, double scrollingLeftEdge)
        {
            DiagnosticsDebug.Assert(this.OwningGrid != null, "Expected non-null owning DataGrid.");
            DiagnosticsDebug.Assert(this.OwningGrid.HorizontalAdjustment >= 0, "Expected owning positive DataGrid.HorizontalAdjustment.");
            DiagnosticsDebug.Assert(this.OwningGrid.HorizontalAdjustment <= this.OwningGrid.HorizontalOffset, "Expected owning DataGrid.HorizontalAdjustment smaller than or equal to DataGrid.HorizontalOffset.");

            if (column.Visibility != Visibility.Visible)
            {
                return false;
            }

            scrollingLeftEdge += this.OwningGrid.HorizontalAdjustment;
            double leftEdge = column.IsFrozen ? frozenLeftEdge : scrollingLeftEdge;
            double rightEdge = leftEdge + column.ActualWidth;
            return DoubleUtil.GreaterThan(rightEdge, 0) &&
                DoubleUtil.LessThanOrClose(leftEdge, this.OwningGrid.CellsWidth) &&
                DoubleUtil.GreaterThan(rightEdge, frozenLeftEdge); // scrolling column covered up by frozen column(s)
        }
    }
}