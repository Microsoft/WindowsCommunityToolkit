// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using CommunityToolkit.WinUI.UI.Automation.Peers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;

using DiagnosticsDebug = System.Diagnostics.Debug;

namespace CommunityToolkit.WinUI.UI.Controls.Primitives
{
    /// <summary>
    /// Used within the template of a <see cref="DataGrid"/> to specify the
    /// location in the control's visual tree where the rows are to be added.
    /// </summary>
    public sealed class DataGridRowsPresenter : Panel
    {
        private double _preManipulationHorizontalOffset;
        private double _preManipulationVerticalOffset;

        /// <summary>
        ///  Initializes a new instance of the <see cref="DataGridRowsPresenter"/> class.
        /// </summary>
        public DataGridRowsPresenter()
        {
            this.ManipulationStarting += new ManipulationStartingEventHandler(DataGridRowsPresenter_ManipulationStarting);
            this.ManipulationStarted += new ManipulationStartedEventHandler(DataGridRowsPresenter_ManipulationStarted);
            this.ManipulationDelta += new ManipulationDeltaEventHandler(DataGridRowsPresenter_ManipulationDelta);
        }

        internal DataGrid OwningGrid
        {
            get;
            set;
        }

        /// <summary>
        /// Arranges the content of the <see cref="DataGridRowsPresenter"/>.
        /// </summary>
        /// <returns>
        /// The actual size used by the <see cref="DataGridRowsPresenter"/>.
        /// </returns>
        /// <param name="finalSize">
        /// The final area within the parent that this element should use to arrange itself and its children.
        /// </param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (finalSize.Height == 0 || this.OwningGrid == null)
            {
                return base.ArrangeOverride(finalSize);
            }

            this.OwningGrid.OnFillerColumnWidthNeeded(finalSize.Width);

            double rowDesiredWidth = this.OwningGrid.ColumnsInternal.VisibleEdgedColumnsWidth + this.OwningGrid.ColumnsInternal.FillerColumn.FillerWidth;
            double topEdge = -this.OwningGrid.NegVerticalOffset;
            foreach (UIElement element in this.OwningGrid.DisplayData.GetScrollingElements())
            {
                DataGridRow row = element as DataGridRow;
                if (row != null)
                {
                    DiagnosticsDebug.Assert(row.Index != -1, "Expected Index other than -1."); // A displayed row should always have its index

                    // Visibility for all filler cells needs to be set in one place.  Setting it individually in
                    // each CellsPresenter causes an NxN layout cycle (see DevDiv Bugs 211557)
                    row.EnsureFillerVisibility();
                    row.Arrange(new Rect(-this.OwningGrid.HorizontalOffset, topEdge, rowDesiredWidth, element.DesiredSize.Height));
                }
                else
                {
                    DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                    if (groupHeader != null)
                    {
                        double leftEdge = this.OwningGrid.AreRowGroupHeadersFrozen ? 0 : -this.OwningGrid.HorizontalOffset;
                        groupHeader.Arrange(new Rect(leftEdge, topEdge, rowDesiredWidth - leftEdge, element.DesiredSize.Height));
                    }
                }

                topEdge += element.DesiredSize.Height;
            }

            double finalHeight = Math.Max(topEdge + this.OwningGrid.NegVerticalOffset, finalSize.Height);

            // Clip the RowsPresenter so rows cannot overlap other elements in certain styling scenarios
            RectangleGeometry rg = new RectangleGeometry();
            rg.Rect = new Rect(0, 0, finalSize.Width, finalHeight);
            this.Clip = rg;

            return new Size(finalSize.Width, finalHeight);
        }

        /// <summary>
        /// Measures the children of a <see cref="DataGridRowsPresenter"/> to
        /// prepare for arranging them during the <see cref="M:System.Windows.FrameworkElement.ArrangeOverride(System.Windows.Size)"/> pass.
        /// </summary>
        /// <param name="availableSize">
        /// The available size that this element can give to child elements. Indicates an upper limit that child elements should not exceed.
        /// </param>
        /// <returns>
        /// The size that the <see cref="DataGridRowsPresenter"/> determines it needs during layout, based on its calculations of child object allocated sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (availableSize.Height == 0 || this.OwningGrid == null)
            {
                return base.MeasureOverride(availableSize);
            }

            // If the Width of our RowsPresenter changed then we need to invalidate our rows
            bool invalidateRows =
                (!this.OwningGrid.RowsPresenterAvailableSize.HasValue || availableSize.Width != this.OwningGrid.RowsPresenterAvailableSize.Value.Width) &&
                !double.IsInfinity(availableSize.Width);

            // The DataGrid uses the RowsPresenter available size in order to autogrow
            // and calculate the scrollbars
            this.OwningGrid.RowsPresenterAvailableSize = availableSize;

            this.OwningGrid.OnRowsMeasure();

            double totalHeight = -this.OwningGrid.NegVerticalOffset;
            double totalCellsWidth = this.OwningGrid.ColumnsInternal.VisibleEdgedColumnsWidth;

            double headerWidth = 0;
            foreach (UIElement element in this.OwningGrid.DisplayData.GetScrollingElements())
            {
                DataGridRow row = element as DataGridRow;
                if (row != null)
                {
                    if (invalidateRows)
                    {
                        row.InvalidateMeasure();
                    }
                }

                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                if (row != null && row.HeaderCell != null)
                {
                    headerWidth = Math.Max(headerWidth, row.HeaderCell.DesiredSize.Width);
                }
                else
                {
                    DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                    if (groupHeader != null && groupHeader.HeaderCell != null)
                    {
                        headerWidth = Math.Max(headerWidth, groupHeader.HeaderCell.DesiredSize.Width);
                    }
                }

                totalHeight += element.DesiredSize.Height;
            }

            this.OwningGrid.RowHeadersDesiredWidth = headerWidth;

            // Could be positive infinity depending on the DataGrid's bounds
            this.OwningGrid.AvailableSlotElementRoom = availableSize.Height - totalHeight;

            // TODO: totalHeight can be negative if we've just collapsed details.  This is a workaround,
            // the real fix is to correct NegVerticalOffset.
            totalHeight = Math.Max(0, totalHeight);

            return new Size(totalCellsWidth + headerWidth, totalHeight);
        }

        /// <summary>
        /// Creates AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
        /// </summary>
        /// <returns>An automation peer for this <see cref="DataGridRowsPresenter"/>.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DataGridRowsPresenterAutomationPeer(this);
        }

        private void DataGridRowsPresenter_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
        {
            if (this.OwningGrid != null)
            {
                DiagnosticsDebug.Assert(this.OwningGrid.IsEnabled, "Expected OwningGrid.IsEnabled is true.");

                _preManipulationHorizontalOffset = this.OwningGrid.HorizontalOffset;
                _preManipulationVerticalOffset = this.OwningGrid.VerticalOffset;
            }
        }

        private void DataGridRowsPresenter_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
            {
                e.Complete();
            }
        }

        private void DataGridRowsPresenter_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (this.OwningGrid != null)
            {
                e.Handled =
                    this.OwningGrid.ProcessScrollOffsetDelta(_preManipulationHorizontalOffset - e.Cumulative.Translation.X - this.OwningGrid.HorizontalOffset, true /*isForHorizontalScroll*/) ||
                    this.OwningGrid.ProcessScrollOffsetDelta(_preManipulationVerticalOffset - e.Cumulative.Translation.Y - this.OwningGrid.VerticalOffset, false /*isForHorizontalScroll*/);
            }
        }

#if DEBUG
        internal void PrintChildren()
        {
            foreach (UIElement element in this.Children)
            {
                DataGridRow row = element as DataGridRow;
                if (row != null)
                {
                    Debug.WriteLine(string.Format(global::System.Globalization.CultureInfo.InvariantCulture, "Slot: {0} Row: {1} Visibility: {2} ", row.Slot, row.Index, row.Visibility));
                }
                else
                {
                    DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                    if (groupHeader != null)
                    {
#if FEATURE_ICOLLECTIONVIEW_GROUP
                        Debug.WriteLine(string.Format(global::System.Globalization.CultureInfo.InvariantCulture, "Slot: {0} GroupHeader: {1} Visibility: {2}", groupHeader.RowGroupInfo.Slot, groupHeader.RowGroupInfo.CollectionViewGroup.Name, groupHeader.Visibility));
#else
                        Debug.WriteLine(string.Format(global::System.Globalization.CultureInfo.InvariantCulture, "Slot: {0} Visibility: {1}", groupHeader.RowGroupInfo.Slot, groupHeader.Visibility));
#endif
                    }
                }
            }
        }
#endif
    }
}