// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.Toolkit.Uwp.UI.Controls.DataGridInternals;
using Windows.Foundation;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Microsoft.Toolkit.Uwp.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer for an item in a DataGrid
    /// </summary>
    public class DataGridItemAutomationPeer : FrameworkElementAutomationPeer,
        IInvokeProvider, IScrollItemProvider, ISelectionItemProvider, ISelectionProvider
    {
        private object _item;
        private AutomationPeer _dataGridAutomationPeer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridItemAutomationPeer"/> class.
        /// </summary>
        public DataGridItemAutomationPeer(object item, DataGrid dataGrid)
            : base(dataGrid)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (dataGrid == null)
            {
                throw new ArgumentNullException("dataGrid");
            }

            _item = item;
            _dataGridAutomationPeer = FrameworkElementAutomationPeer.CreatePeerForElement(dataGrid);
        }

        private DataGrid OwningDataGrid
        {
            get
            {
                DataGridAutomationPeer gridPeer = _dataGridAutomationPeer as DataGridAutomationPeer;
                return gridPeer.Owner as DataGrid;
            }
        }

        private DataGridRow OwningRow
        {
            get
            {
                int index = this.OwningDataGrid.DataConnection.IndexOf(_item);
                int slot = this.OwningDataGrid.SlotFromRowIndex(index);

                if (this.OwningDataGrid.IsSlotVisible(slot))
                {
                    return this.OwningDataGrid.DisplayData.GetDisplayedElement(slot) as DataGridRow;
                }

                return null;
            }
        }

        internal DataGridRowAutomationPeer OwningRowPeer
        {
            get
            {
                DataGridRowAutomationPeer rowPeer = null;
                DataGridRow row = this.OwningRow;
                if (row != null)
                {
                    rowPeer = FrameworkElementAutomationPeer.CreatePeerForElement(row) as DataGridRowAutomationPeer;
                }

                return rowPeer;
            }
        }

        /// <summary>
        /// Returns the accelerator key for the UIElement that is associated with this DataGridItemAutomationPeer.
        /// </summary>
        /// <returns>The accelerator key for the UIElement that is associated with this DataGridItemAutomationPeer.</returns>
        protected override string GetAcceleratorKeyCore()
        {
            return this.OwningRowPeer != null ? this.OwningRowPeer.GetAcceleratorKey() : string.Empty;
        }

        /// <summary>
        /// Returns the access key for the UIElement that is associated with this DataGridItemAutomationPeer.
        /// </summary>
        /// <returns>The access key for the UIElement that is associated with this DataGridItemAutomationPeer.</returns>
        protected override string GetAccessKeyCore()
        {
            return this.OwningRowPeer != null ? this.OwningRowPeer.GetAccessKey() : string.Empty;
        }

        /// <summary>
        /// Returns the control type for the UIElement that is associated with this DataGridItemAutomationPeer.
        /// </summary>
        /// <returns>The control type for the UIElement that is associated with this DataGridItemAutomationPeer.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.DataItem;
        }

        /// <summary>
        /// Returns the string that uniquely identifies the FrameworkElement that is associated with this DataGridItemAutomationPeer.
        /// </summary>
        /// <returns>The string that uniquely identifies the FrameworkElement that is associated with this DataGridItemAutomationPeer.</returns>
        protected override string GetAutomationIdCore()
        {
            // The AutomationId should be unset for dynamic content.
            return string.Empty;
        }

        /// <summary>
        /// Returns the Rect that represents the bounding rectangle of the UIElement that is associated with this DataGridItemAutomationPeer.
        /// </summary>
        /// <returns>The Rect that represents the bounding rectangle of the UIElement that is associated with this DataGridItemAutomationPeer.</returns>
        protected override Rect GetBoundingRectangleCore()
        {
            return this.OwningRowPeer != null ? this.OwningRowPeer.GetBoundingRectangle() : default(Rect);
        }

        /// <summary>
        /// Returns the collection of elements that are represented in the UI Automation tree as immediate
        /// child elements of the automation peer.
        /// </summary>
        /// <returns>The children elements.</returns>
        protected override IList<AutomationPeer> GetChildrenCore()
        {
            if (this.OwningRowPeer != null)
            {
                this.OwningRowPeer.InvalidatePeer();
                return this.OwningRowPeer.GetChildren();
            }

            return new List<AutomationPeer>();
        }

        /// <summary>
        /// Called by GetClassName that gets a human readable name that, in addition to AutomationControlType,
        /// differentiates the control represented by this AutomationPeer.
        /// </summary>
        /// <returns>The string that contains the name.</returns>
        protected override string GetClassNameCore()
        {
            string classNameCore = (this.OwningRowPeer != null) ? this.OwningRowPeer.GetClassName() : string.Empty;
#if DEBUG_AUTOMATION
            System.Diagnostics.Debug.WriteLine("DataGridItemAutomationPeer.GetClassNameCore returns " + classNameCore);
#endif
            return classNameCore;
        }

        /// <summary>
        /// Returns a Point that represents the clickable space that is on the UIElement that is associated with this DataGridItemAutomationPeer.
        /// </summary>
        /// <returns>A Point that represents the clickable space that is on the UIElement that is associated with this DataGridItemAutomationPeer.</returns>
        protected override Point GetClickablePointCore()
        {
            return this.OwningRowPeer != null ? this.OwningRowPeer.GetClickablePoint() : new Point(double.NaN, double.NaN);
        }

        /// <summary>
        /// Returns the string that describes the functionality of the control that is associated with the automation peer.
        /// </summary>
        /// <returns>The string that contains the help text.</returns>
        protected override string GetHelpTextCore()
        {
            return this.OwningRowPeer != null ? this.OwningRowPeer.GetHelpText() : string.Empty;
        }

        /// <summary>
        /// Returns a string that communicates the visual status of the UIElement that is associated with this DataGridItemAutomationPeer.
        /// </summary>
        /// <returns>A string that communicates the visual status of the UIElement that is associated with this DataGridItemAutomationPeer.</returns>
        protected override string GetItemStatusCore()
        {
            return this.OwningRowPeer != null ? this.OwningRowPeer.GetItemStatus() : string.Empty;
        }

        /// <summary>
        /// Returns a human-readable string that contains the item type that the UIElement for this DataGridItemAutomationPeer represents.
        /// </summary>
        /// <returns>A human-readable string that contains the item type that the UIElement for this DataGridItemAutomationPeer represents.</returns>
        protected override string GetItemTypeCore()
        {
            return this.OwningRowPeer != null ? this.OwningRowPeer.GetItemType() : string.Empty;
        }

        /// <summary>
        /// Returns the AutomationPeer for the element that is targeted to the UIElement for this DataGridItemAutomationPeer.
        /// </summary>
        /// <returns>The AutomationPeer for the element that is targeted to the UIElement for this DataGridItemAutomationPeer.</returns>
        protected override AutomationPeer GetLabeledByCore()
        {
            return this.OwningRowPeer != null ? this.OwningRowPeer.GetLabeledBy() : null;
        }

        /// <summary>
        /// Returns a localized human readable string for this control type.
        /// </summary>
        /// <returns>A localized human readable string for this control type.</returns>
        protected override string GetLocalizedControlTypeCore()
        {
            return this.OwningRowPeer != null ? this.OwningRowPeer.GetLocalizedControlType() : string.Empty;
        }

        /// <summary>
        /// Returns the string that describes the functionality of the control that is associated with this DataGridItemAutomationPeer.
        /// </summary>
        /// <returns>The string that contains the help text.</returns>
        protected override string GetNameCore()
        {
            if (this.OwningRowPeer != null)
            {
                string owningRowPeerName = this.OwningRowPeer.GetName();
                if (!string.IsNullOrEmpty(owningRowPeerName))
                {
#if DEBUG_AUTOMATION
                    System.Diagnostics.Debug.WriteLine("DataGridItemAutomationPeer.GetNameCore returns " + owningRowPeerName);
#endif
                    return owningRowPeerName;
                }
            }

            string name = UI.Controls.Properties.Resources.DataGridRowAutomationPeer_ItemType;
#if DEBUG_AUTOMATION
            System.Diagnostics.Debug.WriteLine("DataGridItemAutomationPeer.GetNameCore returns " + name);
#endif
            return name;
        }

        /// <summary>
        /// Returns a value indicating whether the element associated with this DataGridItemAutomationPeer is laid out in a specific direction.
        /// </summary>
        /// <returns>A value indicating whether the element associated with this DataGridItemAutomationPeer is laid out in a specific direction.</returns>
        protected override AutomationOrientation GetOrientationCore()
        {
            return this.OwningRowPeer != null ? this.OwningRowPeer.GetOrientation() : AutomationOrientation.None;
        }

        /// <summary>
        /// Returns the control pattern that is associated with the specified Windows.UI.Xaml.Automation.Peers.PatternInterface.
        /// </summary>
        /// <param name="patternInterface">A value from the Windows.UI.Xaml.Automation.Peers.PatternInterface enumeration.</param>
        /// <returns>The object that supports the specified pattern, or null if unsupported.</returns>
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            switch (patternInterface)
            {
                case PatternInterface.Invoke:
                    if (!this.OwningDataGrid.IsReadOnly)
                    {
                        return this;
                    }

                    break;
                case PatternInterface.ScrollItem:
                    if (this.OwningDataGrid.VerticalScrollBar != null &&
                        this.OwningDataGrid.VerticalScrollBar.Maximum > 0)
                    {
                        return this;
                    }

                    break;
                case PatternInterface.Selection:
                case PatternInterface.SelectionItem:
                    return this;
            }

            return base.GetPatternCore(patternInterface);
        }

        /// <summary>
        /// Returns a value indicating whether the UIElement associated with this DataGridItemAutomationPeer can accept keyboard focus.
        /// </summary>
        /// <returns>True if the element is focusable by the keyboard; otherwise false.</returns>
        protected override bool HasKeyboardFocusCore()
        {
            return this.OwningRowPeer != null ? this.OwningRowPeer.HasKeyboardFocus() : false;
        }

        /// <summary>
        /// Returns a value indicating whether the element associated with this DataGridItemAutomationPeer is an element that contains data that is presented to the user.
        /// </summary>
        /// <returns>True if the element contains data for the user to read; otherwise, false.</returns>
        protected override bool IsContentElementCore()
        {
            return this.OwningRowPeer != null ? this.OwningRowPeer.IsContentElement() : true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the UIElement associated with this DataGridItemAutomationPeer
        /// is understood by the end user as interactive.
        /// </summary>
        /// <returns>True if the UIElement associated with this DataGridItemAutomationPeer
        /// is understood by the end user as interactive.</returns>
        protected override bool IsControlElementCore()
        {
            return this.OwningRowPeer != null ? this.OwningRowPeer.IsControlElement() : true;
        }

        /// <summary>
        /// Gets a value indicating whether this DataGridItemAutomationPeer can receive and send events to the associated element.
        /// </summary>
        /// <returns>True if this DataGridItemAutomationPeer can receive and send events; otherwise, false.</returns>
        protected override bool IsEnabledCore()
        {
            return this.OwningRowPeer != null ? this.OwningRowPeer.IsEnabled() : false;
        }

        /// <summary>
        /// Gets a value indicating whether the UIElement associated with this DataGridItemAutomationPeer can accept keyboard focus.
        /// </summary>
        /// <returns>True if the UIElement associated with this DataGridItemAutomationPeer can accept keyboard focus.</returns>
        protected override bool IsKeyboardFocusableCore()
        {
            return this.OwningRowPeer != null ? this.OwningRowPeer.IsKeyboardFocusable() : false;
        }

        /// <summary>
        /// Gets a value indicating whether the UIElement associated with this DataGridItemAutomationPeer is off the screen.
        /// </summary>
        /// <returns>True if the element is not on the screen; otherwise, false.</returns>
        protected override bool IsOffscreenCore()
        {
            return this.OwningRowPeer != null ? this.OwningRowPeer.IsOffscreen() : true;
        }

        /// <summary>
        /// Gets a value indicating whether the UIElement associated with this DataGridItemAutomationPeer contains protected content.
        /// </summary>
        /// <returns>True if the UIElement contains protected content.</returns>
        protected override bool IsPasswordCore()
        {
            return this.OwningRowPeer != null ? this.OwningRowPeer.IsPassword() : false;
        }

        /// <summary>
        /// Gets a value indicating whether the UIElement associated with this DataGridItemAutomationPeer is required to be completed on a form.
        /// </summary>
        /// <returns>True if the UIElement is required to be completed on a form.</returns>
        protected override bool IsRequiredForFormCore()
        {
            return this.OwningRowPeer != null ? this.OwningRowPeer.IsRequiredForForm() : false;
        }

        /// <summary>
        /// Sets the keyboard input focus on the UIElement associated with this DataGridItemAutomationPeer.
        /// </summary>
        protected override void SetFocusCore()
        {
            if (this.OwningRowPeer != null)
            {
                this.OwningRowPeer.SetFocus();
            }
        }

        void IInvokeProvider.Invoke()
        {
            EnsureEnabled();

            if (this.OwningRowPeer == null)
            {
                this.OwningDataGrid.ScrollIntoView(_item, null);
            }

            bool success = false;
            if (this.OwningRow != null)
            {
                if (this.OwningDataGrid.WaitForLostFocus(() => { ((IInvokeProvider)this).Invoke(); }))
                {
                    return;
                }

                if (this.OwningDataGrid.EditingRow == this.OwningRow)
                {
                    success = this.OwningDataGrid.CommitEdit(DataGridEditingUnit.Row, true /*exitEditing*/);
                }
                else if (this.OwningDataGrid.UpdateSelectionAndCurrency(this.OwningDataGrid.CurrentColumnIndex, this.OwningRow.Slot, DataGridSelectionAction.SelectCurrent, false))
                {
                    success = this.OwningDataGrid.BeginEdit();
                }
            }
        }

        void IScrollItemProvider.ScrollIntoView()
        {
            this.OwningDataGrid.ScrollIntoView(_item, null);
        }

        bool ISelectionItemProvider.IsSelected
        {
            get
            {
                return this.OwningDataGrid.SelectedItems.Contains(_item);
            }
        }

        IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
        {
            get
            {
                return ProviderFromPeer(_dataGridAutomationPeer);
            }
        }

        void ISelectionItemProvider.AddToSelection()
        {
            EnsureEnabled();

            if (this.OwningDataGrid.SelectionMode == DataGridSelectionMode.Single &&
                this.OwningDataGrid.SelectedItems.Count > 0 &&
                !this.OwningDataGrid.SelectedItems.Contains(_item))
            {
                throw DataGridError.DataGridAutomationPeer.OperationCannotBePerformed();
            }

            int index = this.OwningDataGrid.DataConnection.IndexOf(_item);
            if (index != -1)
            {
                this.OwningDataGrid.SetRowSelection(this.OwningDataGrid.SlotFromRowIndex(index), true, false);
                return;
            }

            throw DataGridError.DataGridAutomationPeer.OperationCannotBePerformed();
        }

        void ISelectionItemProvider.RemoveFromSelection()
        {
            EnsureEnabled();

            int index = this.OwningDataGrid.DataConnection.IndexOf(_item);
            if (index != -1)
            {
                bool success = true;
                if (this.OwningDataGrid.EditingRow != null && this.OwningDataGrid.EditingRow.Index == index)
                {
                    if (this.OwningDataGrid.WaitForLostFocus(() => { ((ISelectionItemProvider)this).RemoveFromSelection(); }))
                    {
                        return;
                    }

                    success = this.OwningDataGrid.CommitEdit(DataGridEditingUnit.Row, true /*exitEditing*/);
                }

                if (success)
                {
                    this.OwningDataGrid.SetRowSelection(this.OwningDataGrid.SlotFromRowIndex(index), false, false);
                    return;
                }

                throw DataGridError.DataGridAutomationPeer.OperationCannotBePerformed();
            }
        }

        void ISelectionItemProvider.Select()
        {
            EnsureEnabled();

            int index = this.OwningDataGrid.DataConnection.IndexOf(_item);
            if (index != -1)
            {
                bool success = true;
                if (this.OwningDataGrid.EditingRow != null && this.OwningDataGrid.EditingRow.Index != index)
                {
                    if (this.OwningDataGrid.WaitForLostFocus(() => { ((ISelectionItemProvider)this).Select(); }))
                    {
                        return;
                    }

                    success = this.OwningDataGrid.CommitEdit(DataGridEditingUnit.Row, true /*exitEditing*/);
                }

                if (success)
                {
                    // Clear all the other selected items and select this one
                    int slot = this.OwningDataGrid.SlotFromRowIndex(index);
                    this.OwningDataGrid.UpdateSelectionAndCurrency(this.OwningDataGrid.CurrentColumnIndex, slot, DataGridSelectionAction.SelectCurrent, false);
                    return;
                }

                throw DataGridError.DataGridAutomationPeer.OperationCannotBePerformed();
            }
        }

        bool ISelectionProvider.CanSelectMultiple
        {
            get
            {
                return false;
            }
        }

        bool ISelectionProvider.IsSelectionRequired
        {
            get
            {
                return false;
            }
        }

        IRawElementProviderSimple[] ISelectionProvider.GetSelection()
        {
            if (this.OwningRow != null &&
                this.OwningDataGrid.IsSlotVisible(this.OwningRow.Slot) &&
                this.OwningDataGrid.CurrentSlot == this.OwningRow.Slot)
            {
                DataGridCell cell = this.OwningRow.Cells[this.OwningRow.OwningGrid.CurrentColumnIndex];
                AutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(cell);
                if (peer != null)
                {
                    return new IRawElementProviderSimple[] { ProviderFromPeer(peer) };
                }
            }

            return null;
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