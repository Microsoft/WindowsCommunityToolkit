// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using CommunityToolkit.WinUI.UI.Automation.Peers;
using CommunityToolkit.WinUI.UI.Controls.DataGridInternals;
using CommunityToolkit.WinUI.UI.Controls.Utilities;
using CommunityToolkit.WinUI.UI.Utilities;
using CommunityToolkit.WinUI.Utilities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Core;

using DiagnosticsDebug = System.Diagnostics.Debug;

namespace CommunityToolkit.WinUI.UI.Controls.Primitives
{
    /// <summary>
    /// Represents an individual <see cref="DataGrid"/> column header.
    /// </summary>
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePointerOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnsorted, GroupName = VisualStates.GroupSort)]
    [TemplateVisualState(Name = VisualStates.StateSortAscending, GroupName = VisualStates.GroupSort)]
    [TemplateVisualState(Name = VisualStates.StateSortDescending, GroupName = VisualStates.GroupSort)]
    public partial class DataGridColumnHeader : ContentControl
    {
        internal enum DragMode
        {
            None = 0,
            PointerPressed = 1,
            Drag = 2,
            Resize = 3,
            Reorder = 4
        }

        private const int DATAGRIDCOLUMNHEADER_dragThreshold = 2;
        private const int DATAGRIDCOLUMNHEADER_resizeRegionWidthStrict = 5;
        private const int DATAGRIDCOLUMNHEADER_resizeRegionWidthLoose = 9;
        private const double DATAGRIDCOLUMNHEADER_separatorThickness = 1;

        private Visibility _desiredSeparatorVisibility;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridColumnHeader"/> class.
        /// </summary>
        public DataGridColumnHeader()
        {
            this.PointerCanceled += new PointerEventHandler(DataGridColumnHeader_PointerCanceled);
            this.PointerCaptureLost += new PointerEventHandler(DataGridColumnHeader_PointerCaptureLost);
            this.PointerPressed += new PointerEventHandler(DataGridColumnHeader_PointerPressed);
            this.PointerReleased += new PointerEventHandler(DataGridColumnHeader_PointerReleased);
            this.PointerMoved += new PointerEventHandler(DataGridColumnHeader_PointerMoved);
            this.PointerEntered += new PointerEventHandler(DataGridColumnHeader_PointerEntered);
            this.PointerExited += new PointerEventHandler(DataGridColumnHeader_PointerExited);
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(DataGridColumnHeader_IsEnabledChanged);

            DefaultStyleKey = typeof(DataGridColumnHeader);
        }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Windows.Media.Brush"/> used to paint the column header separator lines.
        /// </summary>
        public Brush SeparatorBrush
        {
            get { return GetValue(SeparatorBrushProperty) as Brush; }
            set { SetValue(SeparatorBrushProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="CommunityToolkit.WinUI.UI.Controls.Primitives.DataGridColumnHeader.SeparatorBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SeparatorBrushProperty =
            DependencyProperty.Register(
                "SeparatorBrush",
                typeof(Brush),
                typeof(DataGridColumnHeader),
                null);

        /// <summary>
        /// Gets or sets a value indicating whether the column header separator lines are visible.
        /// </summary>
        public Visibility SeparatorVisibility
        {
            get { return (Visibility)GetValue(SeparatorVisibilityProperty); }
            set { SetValue(SeparatorVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="CommunityToolkit.WinUI.UI.Controls.Primitives.DataGridColumnHeader.SeparatorVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SeparatorVisibilityProperty =
            DependencyProperty.Register(
                "SeparatorVisibility",
                typeof(Visibility),
                typeof(DataGridColumnHeader),
                new PropertyMetadata(Visibility.Visible, OnSeparatorVisibilityPropertyChanged));

        private static void OnSeparatorVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridColumnHeader columnHeader = d as DataGridColumnHeader;

            if (!columnHeader.IsHandlerSuspended(e.Property))
            {
                columnHeader._desiredSeparatorVisibility = (Visibility)e.NewValue;
                if (columnHeader.OwningGrid != null)
                {
                    columnHeader.UpdateSeparatorVisibility(columnHeader.OwningGrid.ColumnsInternal.LastVisibleColumn);
                }
                else
                {
                    columnHeader.UpdateSeparatorVisibility(null);
                }
            }
        }

        internal int ColumnIndex
        {
            get
            {
                if (this.OwningColumn == null)
                {
                    return -1;
                }

                return this.OwningColumn.Index;
            }
        }

#if FEATURE_ICOLLECTIONVIEW_SORT
        internal ListSortDirection? CurrentSortingState
        {
            get;
            private set;
        }
#endif

        internal DataGrid OwningGrid
        {
            get
            {
                if (this.OwningColumn != null && this.OwningColumn.OwningGrid != null)
                {
                    return this.OwningColumn.OwningGrid;
                }

                return null;
            }
        }

        internal DataGridColumn OwningColumn
        {
            get;
            set;
        }

        private bool HasFocus
        {
            get
            {
                return this.OwningGrid != null &&
                    this.OwningColumn == this.OwningGrid.FocusedColumn &&
                    this.OwningGrid.ColumnHeaderHasFocus;
            }
        }

        private bool IsPointerOver
        {
            get;
            set;
        }

        private bool IsPressed
        {
            get;
            set;
        }

        /// <summary>
        /// Builds the visual tree for the column header when a new template is applied.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ApplyState(false /*useTransitions*/);
        }

        /// <summary>
        /// Called when the value of the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property changes.
        /// </summary>
        /// <param name="oldContent">The old value of the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property.</param>
        /// <param name="newContent">The new value of the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// <paramref name="newContent"/> is not a UIElement.
        /// </exception>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            if (newContent is UIElement)
            {
                throw DataGridError.DataGridColumnHeader.ContentDoesNotSupportUIElements();
            }

            base.OnContentChanged(oldContent, newContent);
        }

        /// <summary>
        /// Creates AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
        /// </summary>
        /// <returns>An automation peer for this <see cref="DataGridColumnHeader"/>.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            if (this.OwningGrid != null && this.OwningColumn != this.OwningGrid.ColumnsInternal.FillerColumn)
            {
                return new DataGridColumnHeaderAutomationPeer(this);
            }

            return base.OnCreateAutomationPeer();
        }

        internal void ApplyState(bool useTransitions)
        {
            DragMode dragMode = this.OwningGrid == null ? DragMode.None : this.OwningGrid.ColumnHeaderInteractionInfo.DragMode;

            // Common States
            if (this.IsPressed && dragMode != DragMode.Resize)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StatePressed, VisualStates.StatePointerOver, VisualStates.StateNormal);
            }
            else if (this.IsPointerOver && dragMode != DragMode.Resize)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StatePointerOver, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateNormal);
            }

            // Focus States
            if (this.HasFocus)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateFocused, VisualStates.StateRegular);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateUnfocused);
            }

            // Sort States
            if (this.OwningColumn != null)
            {
                switch (this.OwningColumn.SortDirection)
                {
                    case null:
                        VisualStates.GoToState(this, useTransitions, VisualStates.StateUnsorted);
                        break;
                    case DataGridSortDirection.Ascending:
                        VisualStates.GoToState(this, useTransitions, VisualStates.StateSortAscending, VisualStates.StateUnsorted);
                        break;
                    case DataGridSortDirection.Descending:
                        VisualStates.GoToState(this, useTransitions, VisualStates.StateSortDescending, VisualStates.StateUnsorted);
                        break;
                }
            }
        }

        /// <summary>
        /// Ensures that the correct Style is applied to this object.
        /// </summary>
        /// <param name="previousStyle">Caller's previous associated Style</param>
        internal void EnsureStyle(Style previousStyle)
        {
            if (this.Style != null &&
                this.Style != previousStyle &&
                (this.OwningColumn == null || this.Style != this.OwningColumn.HeaderStyle) &&
                (this.OwningGrid == null || this.Style != this.OwningGrid.ColumnHeaderStyle))
            {
                return;
            }

            Style style = null;
            if (this.OwningColumn != null)
            {
                style = this.OwningColumn.HeaderStyle;
            }

            if (style == null && this.OwningGrid != null)
            {
                style = this.OwningGrid.ColumnHeaderStyle;
            }

            this.SetStyleWithType(style);
        }

        internal void InvokeProcessSort()
        {
            DiagnosticsDebug.Assert(this.OwningGrid != null, "Expected non-null owning DataGrid.");

            if (this.OwningGrid.WaitForLostFocus(() => { this.InvokeProcessSort(); }))
            {
                return;
            }

            if (this.OwningGrid.CommitEdit(DataGridEditingUnit.Row, true /*exitEditingMode*/))
            {
                DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () => { ProcessSort(); });
            }
        }

        private void ProcessSort()
        {
            if (this.OwningColumn != null &&
                this.OwningGrid != null &&
                this.OwningGrid.EditingRow == null &&
                this.OwningColumn != this.OwningGrid.ColumnsInternal.FillerColumn &&
                this.OwningGrid.CanUserSortColumns &&
                this.OwningColumn.CanUserSort)
            {
                DataGridColumnEventArgs ea = new DataGridColumnEventArgs(this.OwningColumn);
                this.OwningGrid.OnColumnSorting(ea);

#if FEATURE_ICOLLECTIONVIEW_SORT
                if (!ea.Handled && this.OwningGrid.DataConnection.AllowSort && this.OwningGrid.DataConnection.SortDescriptions != null)
                {
                    // - DataConnection.AllowSort is true, and
                    // - SortDescriptionsCollection exists, and
                    // - the column's data type is comparable
                    DataGrid owningGrid = this.OwningGrid;
                    ListSortDirection newSortDirection;
                    SortDescription newSort;

                    bool ctrl;
                    bool shift;

                    KeyboardHelper.GetMetaKeyState(out ctrl, out shift);

                    SortDescription? sort = this.OwningColumn.GetSortDescription();
                    ICollectionView collectionView = owningGrid.DataConnection.CollectionView;
                    DiagnosticsDebug.Assert(collectionView != null);
                    try
                    {
                        owningGrid.OnUserSorting();
                        using (collectionView.DeferRefresh())
                        {
                            // If shift is held down, we multi-sort, therefore if it isn't, we'll clear the sorts beforehand
                            if (!shift || owningGrid.DataConnection.SortDescriptions.Count == 0)
                            {
                                if (collectionView.CanGroup && collectionView.GroupDescriptions != null)
                                {
                                    // Make sure we sort by the GroupDescriptions first
                                    for (int i = 0; i < collectionView.GroupDescriptions.Count; i++)
                                    {
                                        PropertyGroupDescription groupDescription = collectionView.GroupDescriptions[i] as PropertyGroupDescription;
                                        if (groupDescription != null && collectionView.SortDescriptions.Count <= i || collectionView.SortDescriptions[i].PropertyName != groupDescription.PropertyName)
                                        {
                                            collectionView.SortDescriptions.Insert(Math.Min(i, collectionView.SortDescriptions.Count), new SortDescription(groupDescription.PropertyName, ListSortDirection.Ascending));
                                        }
                                    }
                                    while (collectionView.SortDescriptions.Count > collectionView.GroupDescriptions.Count)
                                    {
                                        collectionView.SortDescriptions.RemoveAt(collectionView.GroupDescriptions.Count);
                                    }
                                }
                                else if (!shift)
                                {
                                    owningGrid.DataConnection.SortDescriptions.Clear();
                                }
                            }

                            if (sort.HasValue)
                            {
                                // swap direction
                                switch (sort.Value.Direction)
                                {
                                    case ListSortDirection.Ascending:
                                        newSortDirection = ListSortDirection.Descending;
                                        break;
                                    default:
                                        newSortDirection = ListSortDirection.Ascending;
                                        break;
                                }

                                newSort = new SortDescription(sort.Value.PropertyName, newSortDirection);

                                // changing direction should not affect sort order, so we replace this column's
                                // sort description instead of just adding it to the end of the collection
                                int oldIndex = owningGrid.DataConnection.SortDescriptions.IndexOf(sort.Value);
                                if (oldIndex >= 0)
                                {
                                    owningGrid.DataConnection.SortDescriptions.Remove(sort.Value);
                                    owningGrid.DataConnection.SortDescriptions.Insert(oldIndex, newSort);
                                }
                                else
                                {
                                    owningGrid.DataConnection.SortDescriptions.Add(newSort);
                                }
                            }
                            else
                            {
                                // start new sort
                                newSortDirection = ListSortDirection.Ascending;

                                string propertyName = this.OwningColumn.GetSortPropertyName();

                                // no-opt if we couldn't find a property to sort on
                                if (string.IsNullOrEmpty(propertyName))
                                {
                                    return;
                                }

                                newSort = new SortDescription(propertyName, newSortDirection);

                                owningGrid.DataConnection.SortDescriptions.Add(newSort);
                            }
                        }
                    }
                    finally
                    {
                        owningGrid.OnUserSorted();
                    }

                    sortProcessed = true;
                }
#endif

                // Send the Invoked event for the column header's automation peer.
                DataGridAutomationPeer.RaiseAutomationInvokeEvent(this);
            }
        }

        internal void UpdateSeparatorVisibility(DataGridColumn lastVisibleColumn)
        {
            Visibility newVisibility = _desiredSeparatorVisibility;

            // Collapse separator for the last column if there is no filler column
            if (this.OwningColumn != null &&
                this.OwningGrid != null &&
                _desiredSeparatorVisibility == Visibility.Visible &&
                this.OwningColumn == lastVisibleColumn &&
                !this.OwningGrid.ColumnsInternal.FillerColumn.IsActive)
            {
                newVisibility = Visibility.Collapsed;
            }

            // Update the public property if it has changed
            if (this.SeparatorVisibility != newVisibility)
            {
                this.SetValueNoCallback(DataGridColumnHeader.SeparatorVisibilityProperty, newVisibility);
            }
        }

        /// <summary>
        /// Determines whether a column can be resized by dragging the border of its header.  If star sizing
        /// is being used, there are special conditions that can prevent a column from being resized:
        /// 1. The column is the last visible column.
        /// 2. All columns are constrained by either their maximum or minimum values.
        /// </summary>
        /// <param name="column">Column to check.</param>
        /// <returns>Whether or not the column can be resized by dragging its header.</returns>
        private static bool CanResizeColumn(DataGridColumn column)
        {
            if (column.OwningGrid != null && column.OwningGrid.ColumnsInternal != null && column.OwningGrid.UsesStarSizing &&
                (column.OwningGrid.ColumnsInternal.LastVisibleColumn == column || !DoubleUtil.AreClose(column.OwningGrid.ColumnsInternal.VisibleEdgedColumnsWidth, column.OwningGrid.CellsWidth)))
            {
                return false;
            }

            return column.ActualCanUserResize;
        }

        private bool TrySetResizeColumn(uint pointerId, DataGridColumn column)
        {
            // If Datagrid.CanUserResizeColumns == false, then the column can still override it
            if (this.OwningGrid != null && CanResizeColumn(column))
            {
                DataGridColumnHeaderInteractionInfo interactionInfo = this.OwningGrid.ColumnHeaderInteractionInfo;

                DiagnosticsDebug.Assert(interactionInfo.DragMode != DragMode.None, "Expected _dragMode other than None.");

                interactionInfo.DragColumn = column;
                interactionInfo.DragMode = DragMode.Resize;
                interactionInfo.DragPointerId = pointerId;

                return true;
            }

            return false;
        }

        private bool CanReorderColumn(DataGridColumn column)
        {
            return this.OwningGrid.CanUserReorderColumns &&
                !(column is DataGridFillerColumn) &&
                ((column.CanUserReorderInternal.HasValue && column.CanUserReorderInternal.Value) || !column.CanUserReorderInternal.HasValue);
        }

        private void DataGridColumnHeader_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            CancelPointer(e);
        }

        private void DataGridColumnHeader_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            CancelPointer(e);
        }

        private void CancelPointer(PointerRoutedEventArgs e)
        {
            // When the user stops interacting with the column headers, the drag mode needs to be reset and any open popups closed.
            if (this.OwningGrid != null)
            {
                this.IsPressed = false;
                this.IsPointerOver = false;

                DataGridColumnHeaderInteractionInfo interactionInfo = this.OwningGrid.ColumnHeaderInteractionInfo;
                bool setResizeCursor = false;

                if (this.OwningGrid.ColumnHeaders != null)
                {
                    Point pointerPositionHeaders = e.GetCurrentPoint(this.OwningGrid.ColumnHeaders).Position;
                    setResizeCursor = interactionInfo.DragMode == DragMode.Resize && pointerPositionHeaders.X > 0 && pointerPositionHeaders.X < this.OwningGrid.ActualWidth;
                }

                if (!setResizeCursor)
                {
                    SetOriginalCursor();
                }

                if (interactionInfo.DragPointerId == e.Pointer.PointerId)
                {
                    this.OwningGrid.ResetColumnHeaderInteractionInfo();
                }

                if (setResizeCursor)
                {
                    SetResizeCursor(e.Pointer, e.GetCurrentPoint(this).Position);
                }

                ApplyState(false /*useTransitions*/);
            }
        }

        private void DataGridColumnHeader_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.OwningGrid != null && !(bool)e.NewValue)
            {
                this.IsPressed = false;
                this.IsPointerOver = false;

                DataGridColumnHeaderInteractionInfo interactionInfo = this.OwningGrid.ColumnHeaderInteractionInfo;

                if (interactionInfo.CapturedPointer != null)
                {
                    ReleasePointerCapture(interactionInfo.CapturedPointer);
                }

                ApplyState(false /*useTransitions*/);
            }
        }

        private void DataGridColumnHeader_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!this.IsEnabled || this.OwningGrid == null)
            {
                return;
            }

            this.IsPointerOver = true;

            SetResizeCursor(e.Pointer, e.GetCurrentPoint(this).Position);

            ApplyState(true /*useTransitions*/);
        }

        private void DataGridColumnHeader_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!this.IsEnabled || this.OwningGrid == null)
            {
                return;
            }

            this.IsPointerOver = false;

            DataGridColumnHeaderInteractionInfo interactionInfo = this.OwningGrid.ColumnHeaderInteractionInfo;

            if (interactionInfo.DragMode == DragMode.None && interactionInfo.ResizePointerId == e.Pointer.PointerId)
            {
                SetOriginalCursor();
            }

            ApplyState(true /*useTransitions*/);
        }

        private void DataGridColumnHeader_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (this.OwningGrid == null || this.OwningColumn == null || e.Handled || !this.IsEnabled || this.OwningGrid.ColumnHeaderInteractionInfo.DragMode != DragMode.None)
            {
                return;
            }

            var pointerPoint = e.GetCurrentPoint(this);
            DataGridColumnHeaderInteractionInfo interactionInfo = this.OwningGrid.ColumnHeaderInteractionInfo;

            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse && !pointerPoint.Properties.IsLeftButtonPressed)
            {
                return;
            }

            DiagnosticsDebug.Assert(interactionInfo.DragPointerId == 0, "Expected _dragPointerId is 0.");

            bool handled = e.Handled;

            this.IsPressed = true;

            if (this.OwningGrid.ColumnHeaders != null)
            {
                Point pointerPosition = pointerPoint.Position;

                if (this.CapturePointer(e.Pointer))
                {
                    interactionInfo.CapturedPointer = e.Pointer;
                }
                else
                {
                    interactionInfo.CapturedPointer = null;
                }

                DiagnosticsDebug.Assert(interactionInfo.DragMode == DragMode.None, "Expected _dragMode equals None.");
                DiagnosticsDebug.Assert(interactionInfo.DragColumn == null, "Expected _dragColumn is null.");
                interactionInfo.DragMode = DragMode.PointerPressed;
                interactionInfo.DragPointerId = e.Pointer.PointerId;
                interactionInfo.FrozenColumnsWidth = this.OwningGrid.ColumnsInternal.GetVisibleFrozenEdgedColumnsWidth();
                interactionInfo.PressedPointerPositionHeaders = interactionInfo.LastPointerPositionHeaders = this.Translate(this.OwningGrid.ColumnHeaders, pointerPosition);

                double distanceFromLeft = pointerPosition.X;
                double distanceFromRight = this.ActualWidth - distanceFromLeft;
                DataGridColumn currentColumn = this.OwningColumn;
                DataGridColumn previousColumn = null;
                if (!(this.OwningColumn is DataGridFillerColumn))
                {
                    previousColumn = this.OwningGrid.ColumnsInternal.GetPreviousVisibleNonFillerColumn(currentColumn);
                }

                int resizeRegionWidth = e.Pointer.PointerDeviceType == PointerDeviceType.Touch ? DATAGRIDCOLUMNHEADER_resizeRegionWidthLoose : DATAGRIDCOLUMNHEADER_resizeRegionWidthStrict;

                if (distanceFromRight <= resizeRegionWidth)
                {
                    handled = TrySetResizeColumn(e.Pointer.PointerId, currentColumn);
                }
                else if (distanceFromLeft <= resizeRegionWidth && previousColumn != null)
                {
                    handled = TrySetResizeColumn(e.Pointer.PointerId, previousColumn);
                }

                if (interactionInfo.DragMode == DragMode.Resize && interactionInfo.DragColumn != null)
                {
                    interactionInfo.DragStart = interactionInfo.LastPointerPositionHeaders;
                    interactionInfo.OriginalWidth = interactionInfo.DragColumn.ActualWidth;
                    interactionInfo.OriginalHorizontalOffset = this.OwningGrid.HorizontalOffset;

                    handled = true;
                }
            }

            e.Handled = handled;

            ApplyState(true /*useTransitions*/);
        }

        private void DataGridColumnHeader_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (this.OwningGrid == null || this.OwningColumn == null || e.Handled || !this.IsEnabled)
            {
                return;
            }

            var pointerPoint = e.GetCurrentPoint(this);

            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse && pointerPoint.Properties.IsLeftButtonPressed)
            {
                return;
            }

            DataGridColumnHeaderInteractionInfo interactionInfo = this.OwningGrid.ColumnHeaderInteractionInfo;

            if (interactionInfo.DragPointerId != 0 && interactionInfo.DragPointerId != e.Pointer.PointerId)
            {
                return;
            }

            Point pointerPosition = pointerPoint.Position;
            Point pointerPositionHeaders = e.GetCurrentPoint(this.OwningGrid.ColumnHeaders).Position;
            bool handled = e.Handled;

            this.IsPressed = false;

            if (this.OwningGrid.ColumnHeaders != null)
            {
                switch (interactionInfo.DragMode)
                {
                    case DragMode.PointerPressed:
                    {
                        // Completed a click or tap without dragging, so raise the DataGrid.Sorting event.
                        InvokeProcessSort();
                        break;
                    }

                    case DragMode.Reorder:
                    {
                        // Find header hovered over
                        int targetIndex = this.GetReorderingTargetDisplayIndex(pointerPositionHeaders);

                        if ((!this.OwningColumn.IsFrozen && targetIndex >= this.OwningGrid.FrozenColumnCount) ||
                            (this.OwningColumn.IsFrozen && targetIndex < this.OwningGrid.FrozenColumnCount))
                        {
                            this.OwningColumn.DisplayIndex = targetIndex;

                            DataGridColumnEventArgs ea = new DataGridColumnEventArgs(this.OwningColumn);
                            this.OwningGrid.OnColumnReordered(ea);
                        }

                        DragCompletedEventArgs dragCompletedEventArgs = new DragCompletedEventArgs(pointerPosition.X - interactionInfo.DragStart.Value.X, pointerPosition.Y - interactionInfo.DragStart.Value.Y, false);
                        this.OwningGrid.OnColumnHeaderDragCompleted(dragCompletedEventArgs);
                        break;
                    }

                    case DragMode.Drag:
                    {
                        DragCompletedEventArgs dragCompletedEventArgs = new DragCompletedEventArgs(0, 0, false);
                        this.OwningGrid.OnColumnHeaderDragCompleted(dragCompletedEventArgs);
                        break;
                    }
                }

                SetResizeCursor(e.Pointer, pointerPosition);

                // Variables that track drag mode states get reset in DataGridColumnHeader_LostPointerCapture
                if (interactionInfo.CapturedPointer != null)
                {
                    ReleasePointerCapture(interactionInfo.CapturedPointer);
                }

                this.OwningGrid.ResetColumnHeaderInteractionInfo();
                handled = true;
            }

            e.Handled = handled;

            ApplyState(true /*useTransitions*/);
        }

        private void DataGridColumnHeader_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (this.OwningColumn == null || this.OwningGrid == null || this.OwningGrid.ColumnHeaders == null || !this.IsEnabled)
            {
                return;
            }

            var pointerPoint = e.GetCurrentPoint(this);
            Point pointerPosition = pointerPoint.Position;
            DataGridColumnHeaderInteractionInfo interactionInfo = this.OwningGrid.ColumnHeaderInteractionInfo;

            if (pointerPoint.IsInContact && (interactionInfo.DragPointerId == 0 || interactionInfo.DragPointerId == e.Pointer.PointerId))
            {
                Point pointerPositionHeaders = e.GetCurrentPoint(this.OwningGrid.ColumnHeaders).Position;
                bool handled = false;

                DiagnosticsDebug.Assert(this.OwningGrid.Parent is UIElement, "Expected owning DataGrid's parent to be a UIElement.");

                double distanceFromLeft = pointerPosition.X;
                double distanceFromRight = this.ActualWidth - distanceFromLeft;

                OnPointerMove_Resize(ref handled, pointerPositionHeaders);
                OnPointerMove_Reorder(ref handled, e.Pointer, pointerPosition, pointerPositionHeaders, distanceFromLeft, distanceFromRight);

                // If nothing was done about moving the pointer while the pointer is down, remember the dragging, but do not
                // claim the event was actually handled.
                if (interactionInfo.DragMode == DragMode.PointerPressed &&
                    interactionInfo.PressedPointerPositionHeaders.HasValue &&
                    Math.Abs(interactionInfo.PressedPointerPositionHeaders.Value.X - pointerPositionHeaders.X) + Math.Abs(interactionInfo.PressedPointerPositionHeaders.Value.Y - pointerPositionHeaders.Y) > DATAGRIDCOLUMNHEADER_dragThreshold)
                {
                    interactionInfo.DragMode = DragMode.Drag;
                    interactionInfo.DragPointerId = e.Pointer.PointerId;
                }

                if (interactionInfo.DragMode == DragMode.Drag)
                {
                    DragDeltaEventArgs dragDeltaEventArgs = new DragDeltaEventArgs(pointerPositionHeaders.X - interactionInfo.LastPointerPositionHeaders.Value.X, pointerPositionHeaders.Y - interactionInfo.LastPointerPositionHeaders.Value.Y);
                    this.OwningGrid.OnColumnHeaderDragDelta(dragDeltaEventArgs);
                }

                interactionInfo.LastPointerPositionHeaders = pointerPositionHeaders;
            }

            SetResizeCursor(e.Pointer, pointerPosition);

            if (!this.IsPointerOver)
            {
                this.IsPointerOver = true;
                ApplyState(true /*useTransitions*/);
            }
        }

        /// <summary>
        /// Returns the column against whose top-left the reordering caret should be positioned
        /// </summary>
        /// <param name="pointerPositionHeaders">Pointer position within the ColumnHeadersPresenter</param>
        /// <param name="scroll">Whether or not to scroll horizontally when a column is dragged out of bounds</param>
        /// <param name="scrollAmount">If scroll is true, returns the horizontal amount that was scrolled</param>
        /// <returns>The column against whose top-left the reordering caret should be positioned.</returns>
        private DataGridColumn GetReorderingTargetColumn(Point pointerPositionHeaders, bool scroll, out double scrollAmount)
        {
            DiagnosticsDebug.Assert(this.OwningGrid != null, "Expected non-null OwningGrid.");

            scrollAmount = 0;
            double leftEdge = 0;

            if (this.OwningGrid.ColumnsInternal.RowGroupSpacerColumn.IsRepresented)
            {
                leftEdge = this.OwningGrid.ColumnsInternal.RowGroupSpacerColumn.ActualWidth;
            }

            DataGridColumnHeaderInteractionInfo interactionInfo = this.OwningGrid.ColumnHeaderInteractionInfo;
            double rightEdge = this.OwningGrid.CellsWidth;
            if (this.OwningColumn.IsFrozen)
            {
                rightEdge = Math.Min(rightEdge, interactionInfo.FrozenColumnsWidth);
            }
            else if (this.OwningGrid.FrozenColumnCount > 0)
            {
                leftEdge = interactionInfo.FrozenColumnsWidth;
            }

            if (pointerPositionHeaders.X < leftEdge)
            {
                if (scroll &&
                    this.OwningGrid.HorizontalScrollBar != null &&
                    this.OwningGrid.HorizontalScrollBar.Visibility == Visibility.Visible &&
                    this.OwningGrid.HorizontalScrollBar.Value > 0)
                {
                    double newVal = pointerPositionHeaders.X - leftEdge;
                    scrollAmount = Math.Min(newVal, this.OwningGrid.HorizontalScrollBar.Value);
                    this.OwningGrid.UpdateHorizontalOffset(scrollAmount + this.OwningGrid.HorizontalScrollBar.Value);
                }

                pointerPositionHeaders.X = leftEdge;
            }
            else if (pointerPositionHeaders.X >= rightEdge)
            {
                if (scroll &&
                    this.OwningGrid.HorizontalScrollBar != null &&
                    this.OwningGrid.HorizontalScrollBar.Visibility == Visibility.Visible &&
                    this.OwningGrid.HorizontalScrollBar.Value < this.OwningGrid.HorizontalScrollBar.Maximum)
                {
                    double newVal = pointerPositionHeaders.X - rightEdge;
                    scrollAmount = Math.Min(newVal, this.OwningGrid.HorizontalScrollBar.Maximum - this.OwningGrid.HorizontalScrollBar.Value);
                    this.OwningGrid.UpdateHorizontalOffset(scrollAmount + this.OwningGrid.HorizontalScrollBar.Value);
                }

                pointerPositionHeaders.X = rightEdge - 1;
            }

            foreach (DataGridColumn column in this.OwningGrid.ColumnsInternal.GetDisplayedColumns())
            {
                Point pointerPosition = this.OwningGrid.ColumnHeaders.Translate(column.HeaderCell, pointerPositionHeaders);
                double columnMiddle = column.HeaderCell.ActualWidth / 2;
                if (pointerPosition.X >= 0 && pointerPosition.X <= columnMiddle)
                {
                    return column;
                }
                else if (pointerPosition.X > columnMiddle && pointerPosition.X < column.HeaderCell.ActualWidth)
                {
                    return this.OwningGrid.ColumnsInternal.GetNextVisibleColumn(column);
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the display index to set the column to
        /// </summary>
        /// <param name="pointerPositionHeaders">Pointer position relative to the column headers presenter</param>
        /// <returns>The display index to set the column to.</returns>
        private int GetReorderingTargetDisplayIndex(Point pointerPositionHeaders)
        {
            DiagnosticsDebug.Assert(this.OwningGrid != null, "Expected non-null OwningGrid.");

            DataGridColumn targetColumn = GetReorderingTargetColumn(pointerPositionHeaders, false /*scroll*/, out _);
            if (targetColumn != null)
            {
                return targetColumn.DisplayIndex > this.OwningColumn.DisplayIndex ? targetColumn.DisplayIndex - 1 : targetColumn.DisplayIndex;
            }
            else
            {
                return this.OwningGrid.Columns.Count - 1;
            }
        }

        private void OnPointerMove_BeginReorder(uint pointerId, Point pointerPosition)
        {
            DiagnosticsDebug.Assert(this.OwningGrid != null, "Expected non-null OwningGrid.");

            DataGridColumnHeader dragIndicator = new DataGridColumnHeader();
            dragIndicator.OwningColumn = this.OwningColumn;
            dragIndicator.IsEnabled = false;
            dragIndicator.Content = this.Content;
            dragIndicator.ContentTemplate = this.ContentTemplate;

            Control dropLocationIndicator = new ContentControl();
            dropLocationIndicator.SetStyleWithType(this.OwningGrid.DropLocationIndicatorStyle);

            if (this.OwningColumn.DragIndicatorStyle != null)
            {
                dragIndicator.SetStyleWithType(this.OwningColumn.DragIndicatorStyle);
            }
            else if (this.OwningGrid.DragIndicatorStyle != null)
            {
                dragIndicator.SetStyleWithType(this.OwningGrid.DragIndicatorStyle);
            }

            // If the user didn't style the dragIndicator's Width, default it to the column header's width.
            if (double.IsNaN(dragIndicator.Width))
            {
                dragIndicator.Width = this.ActualWidth;
            }

            // If the user didn't style the dropLocationIndicator's Height, default to the column header's height.
            if (double.IsNaN(dropLocationIndicator.Height))
            {
                dropLocationIndicator.Height = this.ActualHeight;
            }

            // pass the caret's data template to the user for modification.
            DataGridColumnReorderingEventArgs columnReorderingEventArgs = new DataGridColumnReorderingEventArgs(this.OwningColumn)
            {
                DropLocationIndicator = dropLocationIndicator,
                DragIndicator = dragIndicator
            };
            this.OwningGrid.OnColumnReordering(columnReorderingEventArgs);
            if (columnReorderingEventArgs.Cancel)
            {
                return;
            }

            DataGridColumnHeaderInteractionInfo interactionInfo = this.OwningGrid.ColumnHeaderInteractionInfo;

            // The app didn't cancel, so prepare for the reorder.
            interactionInfo.DragColumn = this.OwningColumn;
            DiagnosticsDebug.Assert(interactionInfo.DragMode != DragMode.None, "Expected _dragMode other than None.");
            interactionInfo.DragMode = DragMode.Reorder;
            interactionInfo.DragPointerId = pointerId;
            interactionInfo.DragStart = pointerPosition;

            // Display the reordering thumb.
            this.OwningGrid.ColumnHeaders.DragColumn = this.OwningColumn;
            this.OwningGrid.ColumnHeaders.DragIndicator = columnReorderingEventArgs.DragIndicator;
            this.OwningGrid.ColumnHeaders.DropLocationIndicator = columnReorderingEventArgs.DropLocationIndicator;
        }

        private void OnPointerMove_Reorder(ref bool handled, Pointer pointer, Point pointerPosition, Point pointerPositionHeaders, double distanceFromLeft, double distanceFromRight)
        {
            DiagnosticsDebug.Assert(this.OwningGrid != null, "Expected non-null OwningGrid.");

            if (handled)
            {
                return;
            }

            DataGridColumnHeaderInteractionInfo interactionInfo = this.OwningGrid.ColumnHeaderInteractionInfo;
            int resizeRegionWidth = pointer.PointerDeviceType == PointerDeviceType.Touch ? DATAGRIDCOLUMNHEADER_resizeRegionWidthLoose : DATAGRIDCOLUMNHEADER_resizeRegionWidthStrict;

            // Handle entry into reorder mode
            if (interactionInfo.DragMode == DragMode.PointerPressed &&
                interactionInfo.DragColumn == null &&
                distanceFromRight > resizeRegionWidth &&
                distanceFromLeft > resizeRegionWidth &&
                interactionInfo.PressedPointerPositionHeaders.HasValue &&
                Math.Abs(interactionInfo.PressedPointerPositionHeaders.Value.X - pointerPositionHeaders.X) + Math.Abs(interactionInfo.PressedPointerPositionHeaders.Value.Y - pointerPositionHeaders.Y) > DATAGRIDCOLUMNHEADER_dragThreshold)
            {
                DragStartedEventArgs dragStartedEventArgs =
                    new DragStartedEventArgs(pointerPositionHeaders.X - interactionInfo.LastPointerPositionHeaders.Value.X, pointerPositionHeaders.Y - interactionInfo.LastPointerPositionHeaders.Value.Y);
                this.OwningGrid.OnColumnHeaderDragStarted(dragStartedEventArgs);

                handled = CanReorderColumn(this.OwningColumn);

                if (handled)
                {
                    OnPointerMove_BeginReorder(pointer.PointerId, pointerPosition);
                }
            }

            // Handle reorder mode (eg, positioning of the popup)
            if (interactionInfo.DragMode == DragMode.Reorder && this.OwningGrid.ColumnHeaders.DragIndicator != null)
            {
                DragDeltaEventArgs dragDeltaEventArgs = new DragDeltaEventArgs(pointerPositionHeaders.X - interactionInfo.LastPointerPositionHeaders.Value.X, pointerPositionHeaders.Y - interactionInfo.LastPointerPositionHeaders.Value.Y);
                this.OwningGrid.OnColumnHeaderDragDelta(dragDeltaEventArgs);

                // Find header we're hovering over
                DataGridColumn targetColumn = GetReorderingTargetColumn(pointerPositionHeaders, !this.OwningColumn.IsFrozen /*scroll*/, out var scrollAmount);

                this.OwningGrid.ColumnHeaders.DragIndicatorOffset = pointerPosition.X - interactionInfo.DragStart.Value.X + scrollAmount;
                this.OwningGrid.ColumnHeaders.InvalidateArrange();

                if (this.OwningGrid.ColumnHeaders.DropLocationIndicator != null)
                {
                    Point targetPosition = new Point(0, 0);
                    if (targetColumn == null || targetColumn == this.OwningGrid.ColumnsInternal.FillerColumn || targetColumn.IsFrozen != this.OwningColumn.IsFrozen)
                    {
                        targetColumn = this.OwningGrid.ColumnsInternal.GetLastColumn(true /*isVisible*/, this.OwningColumn.IsFrozen /*isFrozen*/, null /*isReadOnly*/);
                        targetPosition = targetColumn.HeaderCell.Translate(this.OwningGrid.ColumnHeaders, targetPosition);
                        targetPosition.X += targetColumn.ActualWidth;
                    }
                    else
                    {
                        targetPosition = targetColumn.HeaderCell.Translate(this.OwningGrid.ColumnHeaders, targetPosition);
                    }

                    this.OwningGrid.ColumnHeaders.DropLocationIndicatorOffset = targetPosition.X - scrollAmount;
                }

                handled = true;
            }
        }

        private void OnPointerMove_Resize(ref bool handled, Point pointerPositionHeaders)
        {
            DiagnosticsDebug.Assert(this.OwningGrid != null, "Expected non-null OwningGrid.");

            DataGridColumnHeaderInteractionInfo interactionInfo = this.OwningGrid.ColumnHeaderInteractionInfo;

            if (!handled && interactionInfo.DragMode == DragMode.Resize && interactionInfo.DragColumn != null && interactionInfo.DragStart.HasValue)
            {
                DiagnosticsDebug.Assert(interactionInfo.ResizePointerId != 0, "Expected interactionInfo.ResizePointerId other than 0.");

                // Resize column
                double pointerDelta = pointerPositionHeaders.X - interactionInfo.DragStart.Value.X;
                double desiredWidth = interactionInfo.OriginalWidth + pointerDelta;

                desiredWidth = Math.Max(interactionInfo.DragColumn.ActualMinWidth, Math.Min(interactionInfo.DragColumn.ActualMaxWidth, desiredWidth));
                interactionInfo.DragColumn.Resize(interactionInfo.DragColumn.Width.Value, interactionInfo.DragColumn.Width.UnitType, interactionInfo.DragColumn.Width.DesiredValue, desiredWidth, true);

                this.OwningGrid.UpdateHorizontalOffset(interactionInfo.OriginalHorizontalOffset);

                handled = true;
            }
        }

        private void SetOriginalCursor()
        {
            DiagnosticsDebug.Assert(this.OwningGrid != null, "Expected non-null OwningGrid.");

            DataGridColumnHeaderInteractionInfo interactionInfo = this.OwningGrid.ColumnHeaderInteractionInfo;

            if (interactionInfo.ResizePointerId != 0)
            {
                DiagnosticsDebug.Assert(interactionInfo.OriginalCursor != null, "Expected non-null interactionInfo.OriginalCursor.");

                ProtectedCursor = interactionInfo.OriginalCursor;

                interactionInfo.ResizePointerId = 0;
            }
        }

        private void SetResizeCursor(Pointer pointer, Point pointerPosition)
        {
            DiagnosticsDebug.Assert(this.OwningGrid != null, "Expected non-null OwningGrid.");

            DataGridColumnHeaderInteractionInfo interactionInfo = this.OwningGrid.ColumnHeaderInteractionInfo;

            if (interactionInfo.DragMode != DragMode.None || this.OwningGrid == null || this.OwningColumn == null)
            {
                return;
            }

            // Set mouse cursor if the column can be resized.
            double distanceFromLeft = pointerPosition.X;
            double distanceFromTop = pointerPosition.Y;
            double distanceFromRight = this.ActualWidth - distanceFromLeft;
            DataGridColumn currentColumn = this.OwningColumn;
            DataGridColumn previousColumn = null;

            if (!(this.OwningColumn is DataGridFillerColumn))
            {
                previousColumn = this.OwningGrid.ColumnsInternal.GetPreviousVisibleNonFillerColumn(currentColumn);
            }

            int resizeRegionWidth = pointer.PointerDeviceType == PointerDeviceType.Touch ? DATAGRIDCOLUMNHEADER_resizeRegionWidthLoose : DATAGRIDCOLUMNHEADER_resizeRegionWidthStrict;
            bool nearCurrentResizableColumnRightEdge = distanceFromRight <= resizeRegionWidth && currentColumn != null && CanResizeColumn(currentColumn) && distanceFromTop < this.ActualHeight;
            bool nearPreviousResizableColumnLeftEdge = distanceFromLeft <= resizeRegionWidth && previousColumn != null && CanResizeColumn(previousColumn) && distanceFromTop < this.ActualHeight;

            if (this.OwningGrid.IsEnabled && (nearCurrentResizableColumnRightEdge || nearPreviousResizableColumnLeftEdge))
            {
                CoreCursor currentCursor = ProtectedCursor;
                if (currentCursor == null)
                {
                    currentCursor = new CoreCursor(CoreCursorType.Arrow, 0);
                }

                if (currentCursor != null && currentCursor.Type != CoreCursorType.SizeWestEast)
                {
                    interactionInfo.OriginalCursor = currentCursor;
                    interactionInfo.ResizePointerId = pointer.PointerId;

                    var newCursor = new CoreCursor(CoreCursorType.SizeWestEast, 0);
                    ProtectedCursor = newCursor;
                }
            }
            else if (interactionInfo.ResizePointerId == pointer.PointerId)
            {
                SetOriginalCursor();
            }
        }
    }
}