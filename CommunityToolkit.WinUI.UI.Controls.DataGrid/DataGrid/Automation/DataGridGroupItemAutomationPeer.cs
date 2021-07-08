// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using CommunityToolkit.WinUI.UI.Controls;
using CommunityToolkit.WinUI.UI.Controls.DataGridInternals;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
using Microsoft.UI.Xaml.Data;
using Windows.Foundation;

using DiagnosticsDebug = System.Diagnostics.Debug;

namespace CommunityToolkit.WinUI.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer for a group of items in a DataGrid
    /// </summary>
    public class DataGridGroupItemAutomationPeer : FrameworkElementAutomationPeer,
        IExpandCollapseProvider, IGridProvider, IScrollItemProvider, ISelectionProvider
    {
        private ICollectionViewGroup _group;
        private AutomationPeer _dataGridAutomationPeer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridGroupItemAutomationPeer"/> class.
        /// </summary>
        public DataGridGroupItemAutomationPeer(ICollectionViewGroup group, DataGrid dataGrid)
            : base(dataGrid)
        {
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }

            if (dataGrid == null)
            {
                throw new ArgumentNullException("dataGrid");
            }

            _group = group;
            _dataGridAutomationPeer = FrameworkElementAutomationPeer.CreatePeerForElement(dataGrid);
        }

        /// <summary>
        /// Gets the owning DataGrid
        /// </summary>
        private DataGrid OwningDataGrid
        {
            get
            {
                DataGridAutomationPeer gridPeer = _dataGridAutomationPeer as DataGridAutomationPeer;
                return gridPeer.Owner as DataGrid;
            }
        }

        /// <summary>
        /// Gets the owning DataGrid's Automation Peer
        /// </summary>
        private DataGridAutomationPeer OwningDataGridPeer
        {
            get
            {
                return _dataGridAutomationPeer as DataGridAutomationPeer;
            }
        }

        /// <summary>
        /// Gets the owning DataGridRowGroupHeader
        /// </summary>
        private DataGridRowGroupHeader OwningRowGroupHeader
        {
            get
            {
                if (this.OwningDataGrid != null)
                {
                    DataGridRowGroupInfo groupInfo = this.OwningDataGrid.RowGroupInfoFromCollectionViewGroup(_group);
                    if (groupInfo != null && this.OwningDataGrid.IsSlotVisible(groupInfo.Slot))
                    {
                        return this.OwningDataGrid.DisplayData.GetDisplayedElement(groupInfo.Slot) as DataGridRowGroupHeader;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the owning DataGridRowGroupHeader's Automation Peer
        /// </summary>
        internal DataGridRowGroupHeaderAutomationPeer OwningRowGroupHeaderPeer
        {
            get
            {
                DataGridRowGroupHeaderAutomationPeer rowGroupHeaderPeer = null;
                DataGridRowGroupHeader rowGroupHeader = this.OwningRowGroupHeader;
                if (rowGroupHeader != null)
                {
                    rowGroupHeaderPeer = FrameworkElementAutomationPeer.FromElement(rowGroupHeader) as DataGridRowGroupHeaderAutomationPeer;
                    if (rowGroupHeaderPeer == null)
                    {
                        rowGroupHeaderPeer = FrameworkElementAutomationPeer.CreatePeerForElement(rowGroupHeader) as DataGridRowGroupHeaderAutomationPeer;
                    }
                }

                return rowGroupHeaderPeer;
            }
        }

        /// <summary>
        /// Returns the accelerator key for the UIElement that is associated with this DataGridGroupItemAutomationPeer.
        /// </summary>
        /// <returns>The accelerator key for the UIElement that is associated with this DataGridGroupItemAutomationPeer.</returns>
        protected override string GetAcceleratorKeyCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.GetAcceleratorKey() : string.Empty;
        }

        /// <summary>
        /// Returns the access key for the UIElement that is associated with this DataGridGroupItemAutomationPeer.
        /// </summary>
        /// <returns>The access key for the UIElement that is associated with this DataGridGroupItemAutomationPeer.</returns>
        protected override string GetAccessKeyCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.GetAccessKey() : string.Empty;
        }

        /// <summary>
        /// Returns the control type for the UIElement that is associated with this DataGridGroupItemAutomationPeer.
        /// </summary>
        /// <returns>The control type for the UIElement that is associated with this DataGridGroupItemAutomationPeer.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Group;
        }

        /// <summary>
        /// Returns the string that uniquely identifies the FrameworkElement that is associated with this DataGridGroupItemAutomationPeer.
        /// </summary>
        /// <returns>The string that uniquely identifies the FrameworkElement that is associated with this DataGridGroupItemAutomationPeer.</returns>
        protected override string GetAutomationIdCore()
        {
            // The AutomationId should be unset for dynamic content.
            return string.Empty;
        }

        /// <summary>
        /// Returns the Rect that represents the bounding rectangle of the UIElement that is associated with this DataGridGroupItemAutomationPeer.
        /// </summary>
        /// <returns>The Rect that represents the bounding rectangle of the UIElement that is associated with this DataGridGroupItemAutomationPeer.</returns>
        protected override Rect GetBoundingRectangleCore()
        {
            return this.OwningRowGroupHeaderPeer != null ? this.OwningRowGroupHeaderPeer.GetBoundingRectangle() : default(Rect);
        }

        /// <summary>
        /// Returns the collection of elements that are represented in the UI Automation tree as immediate
        /// child elements of the automation peer.
        /// </summary>
        /// <returns>The children elements.</returns>
        protected override IList<AutomationPeer> GetChildrenCore()
        {
            List<AutomationPeer> children = new List<AutomationPeer>();
            if (this.OwningRowGroupHeaderPeer != null)
            {
                this.OwningRowGroupHeaderPeer.InvalidatePeer();
                children.AddRange(this.OwningRowGroupHeaderPeer.GetChildren());
            }

#if FEATURE_ICOLLECTIONVIEW_GROUP
            if (_group.IsBottomLevel)
            {
#endif
#pragma warning disable SA1137 // Elements should have the same indentation
                foreach (object item in _group.GroupItems /*Items*/)
                {
                    children.Add(this.OwningDataGridPeer.GetOrCreateItemPeer(item));
                }
#pragma warning restore SA1137 // Elements should have the same indentation
#if FEATURE_ICOLLECTIONVIEW_GROUP
            }
            else
            {
                foreach (object group in _group.Items)
                {
                    children.Add(this.OwningDataGridPeer.GetOrCreateGroupItemPeer(group));
                }
            }
#endif
            return children;
        }

        /// <summary>
        /// Called by GetClassName that gets a human readable name that, in addition to AutomationControlType,
        /// differentiates the control represented by this AutomationPeer.
        /// </summary>
        /// <returns>The string that contains the name.</returns>
        protected override string GetClassNameCore()
        {
            string classNameCore = this.OwningRowGroupHeaderPeer != null ? this.OwningRowGroupHeaderPeer.GetClassName() : string.Empty;
#if DEBUG_AUTOMATION
            Debug.WriteLine("DataGridGroupItemAutomationPeer.GetClassNameCore returns " + classNameCore);
#endif
            return classNameCore;
        }

        /// <summary>
        /// Returns a Point that represents the clickable space that is on the UIElement that is associated with this DataGridGroupItemAutomationPeer.
        /// </summary>
        /// <returns>A Point that represents the clickable space that is on the UIElement that is associated with this DataGridGroupItemAutomationPeer.</returns>
        protected override Point GetClickablePointCore()
        {
            return this.OwningRowGroupHeaderPeer != null ? this.OwningRowGroupHeaderPeer.GetClickablePoint() : new Point(double.NaN, double.NaN);
        }

        /// <summary>
        /// Returns the string that describes the functionality of the control that is associated with the automation peer.
        /// </summary>
        /// <returns>The string that contains the help text.</returns>
        protected override string GetHelpTextCore()
        {
            return this.OwningRowGroupHeaderPeer != null ? this.OwningRowGroupHeaderPeer.GetHelpText() : string.Empty;
        }

        /// <summary>
        /// Returns a string that communicates the visual status of the UIElement that is associated with this DataGridGroupItemAutomationPeer.
        /// </summary>
        /// <returns>A string that communicates the visual status of the UIElement that is associated with this DataGridGroupItemAutomationPeer.</returns>
        protected override string GetItemStatusCore()
        {
            return this.OwningRowGroupHeaderPeer != null ? this.OwningRowGroupHeaderPeer.GetItemStatus() : string.Empty;
        }

        /// <summary>
        /// Returns a human-readable string that contains the item type that the UIElement for this DataGridGroupItemAutomationPeer represents.
        /// </summary>
        /// <returns>A human-readable string that contains the item type that the UIElement for this DataGridGroupItemAutomationPeer represents.</returns>
        protected override string GetItemTypeCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.GetItemType() : string.Empty;
        }

        /// <summary>
        /// Returns the AutomationPeer for the element that is targeted to the UIElement for this DataGridGroupItemAutomationPeer.
        /// </summary>
        /// <returns>The AutomationPeer for the element that is targeted to the UIElement for this DataGridGroupItemAutomationPeer.</returns>
        protected override AutomationPeer GetLabeledByCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.GetLabeledBy() : null;
        }

        /// <summary>
        /// Returns a localized human readable string for this control type.
        /// </summary>
        /// <returns>A localized human readable string for this control type.</returns>
        protected override string GetLocalizedControlTypeCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.GetLocalizedControlType() : string.Empty;
        }

        /// <summary>
        /// Returns the string that describes the functionality of the control that is associated with this DataGridGroupItemAutomationPeer.
        /// </summary>
        /// <returns>The string that contains the help text.</returns>
        protected override string GetNameCore()
        {
#if FEATURE_ICOLLECTIONVIEW_GROUP
            if (_group.Name != null)
            {
                string name = _group.Name.ToString();
                if (!string.IsNullOrEmpty(name))
                {
                    return name;
                }
            }
#endif
            return base.GetNameCore();
        }

        /// <summary>
        /// Returns a value indicating whether the element associated with this DataGridGroupItemAutomationPeer is laid out in a specific direction.
        /// </summary>
        /// <returns>A value indicating whether the element associated with this DataGridGroupItemAutomationPeer is laid out in a specific direction.</returns>
        protected override AutomationOrientation GetOrientationCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.GetOrientation() : AutomationOrientation.None;
        }

        /// <summary>
        /// Gets the control pattern that is associated with the specified Microsoft.UI.Xaml.Automation.Peers.PatternInterface.
        /// </summary>
        /// <param name="patternInterface">A value from the Microsoft.UI.Xaml.Automation.Peers.PatternInterface enumeration.</param>
        /// <returns>The object that supports the specified pattern, or null if unsupported.</returns>
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            switch (patternInterface)
            {
                case PatternInterface.ExpandCollapse:
                case PatternInterface.Grid:
                case PatternInterface.Selection:
                case PatternInterface.Table:
                    return this;
                case PatternInterface.ScrollItem:
                {
                    if (this.OwningDataGrid.VerticalScrollBar != null &&
                        this.OwningDataGrid.VerticalScrollBar.Maximum > 0)
                    {
                        return this;
                    }

                    break;
                }
            }

            return base.GetPatternCore(patternInterface);
        }

        /// <summary>
        /// Returns a value indicating whether the UIElement associated with this DataGridGroupItemAutomationPeer can accept keyboard focus.
        /// </summary>
        /// <returns>True if the element is focusable by the keyboard; otherwise false.</returns>
        protected override bool HasKeyboardFocusCore()
        {
            return this.OwningRowGroupHeaderPeer != null ? this.OwningRowGroupHeaderPeer.HasKeyboardFocus() : false;
        }

        /// <summary>
        /// Returns a value indicating whether the element associated with this DataGridGroupItemAutomationPeer is an element that contains data that is presented to the user.
        /// </summary>
        /// <returns>True if the element contains data for the user to read; otherwise, false.</returns>
        protected override bool IsContentElementCore()
        {
            return this.OwningRowGroupHeaderPeer != null ? this.OwningRowGroupHeaderPeer.IsContentElement() : true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the UIElement associated with this DataGridGroupItemAutomationPeer
        /// is understood by the end user as interactive.
        /// </summary>
        /// <returns>True if the UIElement associated with this DataGridGroupItemAutomationPeer
        /// is understood by the end user as interactive.</returns>
        protected override bool IsControlElementCore()
        {
            return this.OwningRowGroupHeaderPeer != null ? this.OwningRowGroupHeaderPeer.IsControlElement() : true;
        }

        /// <summary>
        /// Gets a value indicating whether this DataGridGroupItemAutomationPeer can receive and send events to the associated element.
        /// </summary>
        /// <returns>True if this DataGridGroupItemAutomationPeer can receive and send events; otherwise, false.</returns>
        protected override bool IsEnabledCore()
        {
            return this.OwningRowGroupHeaderPeer != null ? this.OwningRowGroupHeaderPeer.IsEnabled() : false;
        }

        /// <summary>
        /// Gets a value indicating whether the UIElement associated with this DataGridGroupItemAutomationPeer can accept keyboard focus.
        /// </summary>
        /// <returns>True if the UIElement associated with this DataGridGroupItemAutomationPeer can accept keyboard focus.</returns>
        protected override bool IsKeyboardFocusableCore()
        {
            return this.OwningRowGroupHeaderPeer != null ? this.OwningRowGroupHeaderPeer.IsKeyboardFocusable() : false;
        }

        /// <summary>
        /// Gets a value indicating whether the UIElement associated with this DataGridGroupItemAutomationPeer is off the screen.
        /// </summary>
        /// <returns>True if the element is not on the screen; otherwise, false.</returns>
        protected override bool IsOffscreenCore()
        {
            return this.OwningRowGroupHeaderPeer != null ? this.OwningRowGroupHeaderPeer.IsOffscreen() : true;
        }

        /// <summary>
        /// Gets a value indicating whether the UIElement associated with this DataGridGroupItemAutomationPeer contains protected content.
        /// </summary>
        /// <returns>True if the UIElement contains protected content.</returns>
        protected override bool IsPasswordCore()
        {
            return this.OwningRowGroupHeaderPeer != null ? this.OwningRowGroupHeaderPeer.IsPassword() : false;
        }

        /// <summary>
        /// Gets a value indicating whether the UIElement associated with this DataGridGroupItemAutomationPeer is required to be completed on a form.
        /// </summary>
        /// <returns>True if the UIElement is required to be completed on a form.</returns>
        protected override bool IsRequiredForFormCore()
        {
            return this.OwningRowGroupHeaderPeer != null ? this.OwningRowGroupHeaderPeer.IsRequiredForForm() : false;
        }

        /// <summary>
        /// Sets the keyboard input focus on the UIElement associated with this DataGridGroupItemAutomationPeer.
        /// </summary>
        protected override void SetFocusCore()
        {
            if (this.OwningRowGroupHeaderPeer != null)
            {
                this.OwningRowGroupHeaderPeer.SetFocus();
            }
        }

        void IExpandCollapseProvider.Collapse()
        {
            EnsureEnabled();

            if (this.OwningDataGrid != null)
            {
                this.OwningDataGrid.CollapseRowGroup(_group, false /*collapseAllSubgroups*/);
            }
        }

        void IExpandCollapseProvider.Expand()
        {
            EnsureEnabled();

            if (this.OwningDataGrid != null)
            {
                this.OwningDataGrid.ExpandRowGroup(_group, false /*expandAllSubgroups*/);
            }
        }

        ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState
        {
            get
            {
                if (this.OwningDataGrid != null)
                {
                    DataGridRowGroupInfo groupInfo = this.OwningDataGrid.RowGroupInfoFromCollectionViewGroup(_group);
                    if (groupInfo != null && groupInfo.Visibility == Visibility.Visible)
                    {
                        return ExpandCollapseState.Expanded;
                    }
                }

                return ExpandCollapseState.Collapsed;
            }
        }

        int IGridProvider.ColumnCount
        {
            get
            {
                if (this.OwningDataGrid != null)
                {
                    return this.OwningDataGrid.Columns.Count;
                }

                return 0;
            }
        }

        IRawElementProviderSimple IGridProvider.GetItem(int row, int column)
        {
            EnsureEnabled();

            if (this.OwningDataGrid != null &&
                this.OwningDataGrid.DataConnection != null &&
                row >= 0 && row < _group.GroupItems.Count /*ItemCount*/ &&
                column >= 0 && column < this.OwningDataGrid.Columns.Count)
            {
                DataGridRowGroupInfo groupInfo = this.OwningDataGrid.RowGroupInfoFromCollectionViewGroup(_group);
                if (groupInfo != null)
                {
                    // Adjust the row index to be relative to the DataGrid instead of the group
                    row = groupInfo.Slot - this.OwningDataGrid.RowGroupHeadersTable.GetIndexCount(0, groupInfo.Slot) + row + 1;
                    DiagnosticsDebug.Assert(row >= 0, "Expected positive row.");
                    DiagnosticsDebug.Assert(row < this.OwningDataGrid.DataConnection.Count, "Expected row smaller than this.OwningDataGrid.DataConnection.Count.");
                    int slot = this.OwningDataGrid.SlotFromRowIndex(row);

                    if (!this.OwningDataGrid.IsSlotVisible(slot))
                    {
                        object item = this.OwningDataGrid.DataConnection.GetDataItem(row);
                        this.OwningDataGrid.ScrollIntoView(item, this.OwningDataGrid.Columns[column]);
                    }

                    DiagnosticsDebug.Assert(this.OwningDataGrid.IsSlotVisible(slot), "Expected OwningDataGrid.IsSlotVisible(slot) is true.");

                    DataGridRow dgr = this.OwningDataGrid.DisplayData.GetDisplayedElement(slot) as DataGridRow;

                    // the first cell is always the indentation filler cell if grouping is enabled, so skip it
                    DiagnosticsDebug.Assert(column + 1 < dgr.Cells.Count, "Expected column + 1 smaller than dgr.Cells.Count.");
                    DataGridCell cell = dgr.Cells[column + 1];
                    AutomationPeer peer = CreatePeerForElement(cell);
                    if (peer != null)
                    {
                        return ProviderFromPeer(peer);
                    }
                }
            }

            return null;
        }

        int IGridProvider.RowCount
        {
            get
            {
                return _group.GroupItems.Count /*ItemCount*/;
            }
        }

        void IScrollItemProvider.ScrollIntoView()
        {
            EnsureEnabled();

            if (this.OwningDataGrid != null)
            {
                DataGridRowGroupInfo groupInfo = this.OwningDataGrid.RowGroupInfoFromCollectionViewGroup(_group);
                if (groupInfo != null)
                {
                    this.OwningDataGrid.ScrollIntoView(groupInfo.CollectionViewGroup, null);
                }
            }
        }

        IRawElementProviderSimple[] ISelectionProvider.GetSelection()
        {
            EnsureEnabled();

            if (this.OwningDataGrid != null &&
                this.OwningDataGridPeer != null &&
                this.OwningDataGrid.SelectedItems != null &&
                _group.GroupItems.Count /*ItemCount*/ > 0)
            {
                DataGridRowGroupInfo groupInfo = this.OwningDataGrid.RowGroupInfoFromCollectionViewGroup(_group);
                if (groupInfo != null)
                {
                    // See which of the selected items are contained within this group
                    List<IRawElementProviderSimple> selectedProviders = new List<IRawElementProviderSimple>();
                    int startRowIndex = groupInfo.Slot - this.OwningDataGrid.RowGroupHeadersTable.GetIndexCount(0, groupInfo.Slot) + 1;
                    foreach (object item in this.OwningDataGrid.GetSelectionInclusive(startRowIndex, startRowIndex + _group.GroupItems.Count /*ItemCount*/ - 1))
                    {
                        DataGridItemAutomationPeer peer = this.OwningDataGridPeer.GetOrCreateItemPeer(item);
                        if (peer != null)
                        {
                            selectedProviders.Add(ProviderFromPeer(peer));
                        }
                    }

                    return selectedProviders.ToArray();
                }
            }

            return null;
        }

        bool ISelectionProvider.CanSelectMultiple
        {
            get
            {
                return this.OwningDataGrid != null && this.OwningDataGrid.SelectionMode == DataGridSelectionMode.Extended;
            }
        }

        bool ISelectionProvider.IsSelectionRequired
        {
            get
            {
                return false;
            }
        }

        private void EnsureEnabled()
        {
            if (!_dataGridAutomationPeer.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }
        }
    }
}