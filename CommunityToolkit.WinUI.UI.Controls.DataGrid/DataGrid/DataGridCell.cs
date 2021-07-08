// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using CommunityToolkit.WinUI.UI.Automation.Peers;
using CommunityToolkit.WinUI.UI.Controls.DataGridInternals;
using CommunityToolkit.WinUI.UI.Controls.Utilities;
using CommunityToolkit.WinUI.UI.Utilities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using Windows.Devices.Input;

using DiagnosticsDebug = System.Diagnostics.Debug;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Represents an individual <see cref="DataGrid"/> cell.
    /// </summary>
    [TemplatePart(Name = DATAGRIDCELL_elementRightGridLine, Type = typeof(Rectangle))]

    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePointerOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateUnselected, GroupName = VisualStates.GroupSelection)]
    [TemplateVisualState(Name = VisualStates.StateSelected, GroupName = VisualStates.GroupSelection)]
    [TemplateVisualState(Name = VisualStates.StateRegular, GroupName = VisualStates.GroupCurrent)]
    [TemplateVisualState(Name = VisualStates.StateCurrent, GroupName = VisualStates.GroupCurrent)]
    [TemplateVisualState(Name = VisualStates.StateCurrentWithFocus, GroupName = VisualStates.GroupCurrent)]
    [TemplateVisualState(Name = VisualStates.StateDisplay, GroupName = VisualStates.GroupInteraction)]
    [TemplateVisualState(Name = VisualStates.StateEditing, GroupName = VisualStates.GroupInteraction)]
    [TemplateVisualState(Name = VisualStates.StateInvalid, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateValid, GroupName = VisualStates.GroupValidation)]
    public sealed partial class DataGridCell : ContentControl
    {
        private const string DATAGRIDCELL_elementRightGridLine = "RightGridLine";

        private Rectangle _rightGridLine;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridCell"/> class.
        /// </summary>
        public DataGridCell()
        {
            this.IsTapEnabled = true;
            this.AddHandler(UIElement.TappedEvent, new TappedEventHandler(DataGridCell_PointerTapped), true /*handledEventsToo*/);

            this.PointerCanceled += new PointerEventHandler(DataGridCell_PointerCanceled);
            this.PointerCaptureLost += new PointerEventHandler(DataGridCell_PointerCaptureLost);
            this.PointerPressed += new PointerEventHandler(DataGridCell_PointerPressed);
            this.PointerReleased += new PointerEventHandler(DataGridCell_PointerReleased);
            this.PointerEntered += new PointerEventHandler(DataGridCell_PointerEntered);
            this.PointerExited += new PointerEventHandler(DataGridCell_PointerExited);
            this.PointerMoved += new PointerEventHandler(DataGridCell_PointerMoved);

            DefaultStyleKey = typeof(DataGridCell);
        }

        /// <summary>
        /// Gets a value indicating whether the data in a cell is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return (bool)GetValue(IsValidProperty);
            }

            internal set
            {
                this.SetValueNoCallback(IsValidProperty, value);
            }
        }

        /// <summary>
        /// Identifies the IsValid dependency property.
        /// </summary>
        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register(
                "IsValid",
                typeof(bool),
                typeof(DataGridCell),
                new PropertyMetadata(true, OnIsValidPropertyChanged));

        /// <summary>
        /// IsValidProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGridCell that changed its IsValid.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnIsValidPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridCell dataGridCell = d as DataGridCell;
            if (!dataGridCell.IsHandlerSuspended(e.Property))
            {
                dataGridCell.SetValueNoCallback(DataGridCell.IsValidProperty, e.OldValue);
                throw DataGridError.DataGrid.UnderlyingPropertyIsReadOnly("IsValid");
            }
        }

        internal double ActualRightGridLineWidth
        {
            get
            {
                if (_rightGridLine != null)
                {
                    return _rightGridLine.ActualWidth;
                }

                return 0;
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

        internal bool IsCurrent
        {
            get
            {
                DiagnosticsDebug.Assert(this.OwningGrid != null && this.OwningColumn != null && this.OwningRow != null, "Expected non-null owning DataGrid, DataGridColumn and DataGridRow.");

                return this.OwningGrid.CurrentColumnIndex == this.OwningColumn.Index &&
                       this.OwningGrid.CurrentSlot == this.OwningRow.Slot;
            }
        }

        internal bool IsPointerOver
        {
            get
            {
                return this.InteractionInfo != null && this.InteractionInfo.IsPointerOver;
            }

            set
            {
                if (value && this.InteractionInfo == null)
                {
                    this.InteractionInfo = new DataGridInteractionInfo();
                }

                if (this.InteractionInfo != null)
                {
                    this.InteractionInfo.IsPointerOver = value;
                }

                ApplyCellState(true /*animate*/);
            }
        }

        internal DataGridColumn OwningColumn
        {
            get;
            set;
        }

        internal DataGrid OwningGrid
        {
            get
            {
                if (this.OwningRow != null && this.OwningRow.OwningGrid != null)
                {
                    return this.OwningRow.OwningGrid;
                }

                if (this.OwningColumn != null)
                {
                    return this.OwningColumn.OwningGrid;
                }

                return null;
            }
        }

        internal DataGridRow OwningRow
        {
            get;
            set;
        }

        internal int RowIndex
        {
            get
            {
                if (this.OwningRow == null)
                {
                    return -1;
                }

                return this.OwningRow.Index;
            }
        }

        private DataGridInteractionInfo InteractionInfo
        {
            get;
            set;
        }

        private bool IsEdited
        {
            get
            {
                DiagnosticsDebug.Assert(this.OwningGrid != null, "Expected non-null owning DataGrid.");

                return this.OwningGrid.EditingRow == this.OwningRow &&
                       this.OwningGrid.EditingColumnIndex == this.ColumnIndex;
            }
        }

        /// <summary>
        /// Builds the visual tree for the row header when a new template is applied.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ApplyCellState(false /*animate*/);

            _rightGridLine = GetTemplateChild(DATAGRIDCELL_elementRightGridLine) as Rectangle;
            if (_rightGridLine != null && this.OwningColumn == null)
            {
                // Turn off the right GridLine for filler cells
                _rightGridLine.Visibility = Visibility.Collapsed;
            }
            else
            {
                EnsureGridLine(null);
            }
        }

        /// <summary>
        /// Creates AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
        /// </summary>
        /// <returns>An automation peer for this <see cref="DataGridCell"/>.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            if (this.OwningGrid != null &&
                this.OwningColumn != null &&
                this.OwningColumn != this.OwningGrid.ColumnsInternal.FillerColumn)
            {
                return new DataGridCellAutomationPeer(this);
            }

            return base.OnCreateAutomationPeer();
        }

        internal void ApplyCellState(bool animate)
        {
            if (this.OwningGrid == null || this.OwningColumn == null || this.OwningRow == null || this.OwningRow.Visibility == Visibility.Collapsed || this.OwningRow.Slot == -1)
            {
                return;
            }

            // CommonStates
            if (this.IsPointerOver)
            {
                VisualStates.GoToState(this, animate, VisualStates.StatePointerOver, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(this, animate, VisualStates.StateNormal);
            }

            // SelectionStates
            if (this.OwningRow.IsSelected)
            {
                VisualStates.GoToState(this, animate, VisualStates.StateSelected, VisualStates.StateUnselected);
            }
            else
            {
                VisualStates.GoToState(this, animate, VisualStates.StateUnselected);
            }

            // CurrentStates
            if (this.IsCurrent && !this.OwningGrid.ColumnHeaderHasFocus)
            {
                if (this.OwningGrid.ContainsFocus)
                {
                    VisualStates.GoToState(this, animate, VisualStates.StateCurrentWithFocus, VisualStates.StateCurrent, VisualStates.StateRegular);
                }
                else
                {
                    VisualStates.GoToState(this, animate, VisualStates.StateCurrent, VisualStates.StateRegular);
                }
            }
            else
            {
                VisualStates.GoToState(this, animate, VisualStates.StateRegular);
            }

            // Interaction states
            if (this.IsEdited)
            {
                VisualStates.GoToState(this, animate, VisualStates.StateEditing, VisualStates.StateDisplay);
            }
            else
            {
                VisualStates.GoToState(this, animate, VisualStates.StateDisplay);
            }

            // Validation states
            if (this.IsValid)
            {
                VisualStates.GoToState(this, animate, VisualStates.StateValid);
            }
            else
            {
                VisualStates.GoToState(this, animate, VisualStates.StateInvalid, VisualStates.StateValid);
            }
        }

        // Makes sure the right gridline has the proper stroke and visibility. If lastVisibleColumn is specified, the
        // right gridline will be collapsed if this cell belongs to the lastVisibileColumn and there is no filler column
        internal void EnsureGridLine(DataGridColumn lastVisibleColumn)
        {
            if (this.OwningGrid != null && _rightGridLine != null)
            {
                if (!(this.OwningColumn is DataGridFillerColumn) && this.OwningGrid.VerticalGridLinesBrush != null && this.OwningGrid.VerticalGridLinesBrush != _rightGridLine.Fill)
                {
                    _rightGridLine.Fill = this.OwningGrid.VerticalGridLinesBrush;
                }

                Visibility newVisibility =
                    (this.OwningGrid.GridLinesVisibility == DataGridGridLinesVisibility.Vertical || this.OwningGrid.GridLinesVisibility == DataGridGridLinesVisibility.All) &&
                    (this.OwningGrid.ColumnsInternal.FillerColumn.IsActive || this.OwningColumn != lastVisibleColumn)
                    ? Visibility.Visible : Visibility.Collapsed;

                if (newVisibility != _rightGridLine.Visibility)
                {
                    _rightGridLine.Visibility = newVisibility;
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
                (this.OwningColumn == null || this.Style != this.OwningColumn.CellStyle) &&
                (this.OwningGrid == null || this.Style != this.OwningGrid.CellStyle) &&
                this.Style != previousStyle)
            {
                return;
            }

            Style style = null;
            if (this.OwningColumn != null)
            {
                style = this.OwningColumn.CellStyle;
            }

            if (style == null && this.OwningGrid != null)
            {
                style = this.OwningGrid.CellStyle;
            }

            this.SetStyleWithType(style);
        }

        internal void Recycle()
        {
            this.InteractionInfo = null;
        }

        private void CancelPointer(PointerRoutedEventArgs e)
        {
            if (this.InteractionInfo != null && this.InteractionInfo.CapturedPointerId == e.Pointer.PointerId)
            {
                this.InteractionInfo.CapturedPointerId = 0u;
            }

            this.IsPointerOver = false;
        }

        private void DataGridCell_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            CancelPointer(e);
        }

        private void DataGridCell_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            CancelPointer(e);
        }

        private void DataGridCell_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch &&
                this.OwningGrid != null &&
                this.OwningGrid.AllowsManipulation &&
                (this.InteractionInfo == null || this.InteractionInfo.CapturedPointerId == 0u) &&
                this.CapturePointer(e.Pointer))
            {
                if (this.InteractionInfo == null)
                {
                    this.InteractionInfo = new DataGridInteractionInfo();
                }

                this.InteractionInfo.CapturedPointerId = e.Pointer.PointerId;
            }
        }

        private void DataGridCell_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (this.InteractionInfo != null && this.InteractionInfo.CapturedPointerId == e.Pointer.PointerId)
            {
                ReleasePointerCapture(e.Pointer);
            }
        }

        private void DataGridCell_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            UpdateIsPointerOver(true);
        }

        private void DataGridCell_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            UpdateIsPointerOver(false);
        }

        private void DataGridCell_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            UpdateIsPointerOver(true);
        }

        private void DataGridCell_PointerTapped(object sender, TappedRoutedEventArgs e)
        {
            // OwningGrid is null for TopLeftHeaderCell and TopRightHeaderCell because they have no OwningRow
            if (this.OwningGrid != null && !this.OwningGrid.HasColumnUserInteraction)
            {
                if (!e.Handled && this.OwningGrid.IsTabStop)
                {
                    bool success = this.OwningGrid.Focus(FocusState.Programmatic);
                    DiagnosticsDebug.Assert(success, "Expected successful focus change.");
                }

                if (this.OwningRow != null)
                {
                    DiagnosticsDebug.Assert(sender is DataGridCell, "Expected sender is DataGridCell.");
                    DiagnosticsDebug.Assert(sender == this, "Expected sender is this.");
                    e.Handled = this.OwningGrid.UpdateStateOnTapped(e, this.ColumnIndex, this.OwningRow.Slot, !e.Handled /*allowEdit*/);
                    this.OwningGrid.UpdatedStateOnTapped = true;
                }
            }
        }

        private void UpdateIsPointerOver(bool isPointerOver)
        {
            if (this.InteractionInfo != null && this.InteractionInfo.CapturedPointerId != 0u)
            {
                return;
            }

            this.IsPointerOver = isPointerOver;
        }
    }
}