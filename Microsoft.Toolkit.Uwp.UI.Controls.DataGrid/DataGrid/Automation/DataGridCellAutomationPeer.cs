// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.Toolkit.Uwp.UI.Controls.DataGridInternals;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer for DataGridCell
    /// </summary>
    public class DataGridCellAutomationPeer : FrameworkElementAutomationPeer,
        IGridItemProvider, IInvokeProvider, IScrollItemProvider, ISelectionItemProvider, ITableItemProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridCellAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">DataGridCell</param>
        public DataGridCellAutomationPeer(DataGridCell owner)
            : base(owner)
        {
        }

        private IRawElementProviderSimple ContainingGrid
        {
            get
            {
                AutomationPeer peer = CreatePeerForElement(this.OwningGrid);
                if (peer != null)
                {
                    return ProviderFromPeer(peer);
                }

                return null;
            }
        }

        private DataGridCell OwningCell
        {
            get
            {
                return Owner as DataGridCell;
            }
        }

        private DataGridColumn OwningColumn
        {
            get
            {
                return this.OwningCell.OwningColumn;
            }
        }

        private DataGrid OwningGrid
        {
            get
            {
                return this.OwningCell.OwningGrid;
            }
        }

        private DataGridRow OwningRow
        {
            get
            {
                return this.OwningCell.OwningRow;
            }
        }

        /// <summary>
        /// Gets the control type for the element that is associated with the UI Automation peer.
        /// </summary>
        /// <returns>The control type.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            if (this.OwningColumn != null)
            {
                if (this.OwningColumn is DataGridCheckBoxColumn)
                {
                    return AutomationControlType.CheckBox;
                }

                if (this.OwningColumn is DataGridTextColumn)
                {
                    return AutomationControlType.Text;
                }

                if (this.OwningColumn is DataGridComboBoxColumn)
                {
                    return AutomationControlType.ComboBox;
                }
            }

            return AutomationControlType.Custom;
        }

        /// <summary>
        /// Called by GetClassName that gets a human readable name that, in addition to AutomationControlType,
        /// differentiates the control represented by this AutomationPeer.
        /// </summary>
        /// <returns>The string that contains the name.</returns>
        protected override string GetClassNameCore()
        {
            string classNameCore = Owner.GetType().Name;
#if DEBUG_AUTOMATION
            System.Diagnostics.Debug.WriteLine("DataGridCellAutomationPeer.GetClassNameCore returns " + classNameCore);
#endif
            return classNameCore;
        }

        /// <summary>
        /// Gets the name of the element.
        /// </summary>
        /// <returns>The string that contains the name.</returns>
        protected override string GetNameCore()
        {
            TextBlock textBlock = this.OwningCell.Content as TextBlock;
            if (textBlock != null)
            {
                return textBlock.Text;
            }

            TextBox textBox = this.OwningCell.Content as TextBox;
            if (textBox != null)
            {
                return textBox.Text;
            }

            if (this.OwningColumn != null && this.OwningRow != null)
            {
                object cellContent = null;
                DataGridBoundColumn boundColumn = this.OwningColumn as DataGridBoundColumn;
                if (boundColumn != null && boundColumn.Binding != null)
                {
                    cellContent = boundColumn.GetCellValue(this.OwningRow.DataContext, boundColumn.Binding);
                }

                if (cellContent == null && this.OwningColumn.ClipboardContentBinding != null)
                {
                    cellContent = this.OwningColumn.GetCellValue(this.OwningRow.DataContext, this.OwningColumn.ClipboardContentBinding);
                }

                if (cellContent != null)
                {
                    string cellName = cellContent.ToString();
                    if (!string.IsNullOrEmpty(cellName))
                    {
                        return cellName;
                    }
                }
            }

            return base.GetNameCore();
        }

        /// <summary>
        /// Gets the control pattern that is associated with the specified Windows.UI.Xaml.Automation.Peers.PatternInterface.
        /// </summary>
        /// <param name="patternInterface">A value from the Windows.UI.Xaml.Automation.Peers.PatternInterface enumeration.</param>
        /// <returns>The object that supports the specified pattern, or null if unsupported.</returns>
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (this.OwningGrid != null)
            {
                switch (patternInterface)
                {
                    case PatternInterface.Invoke:
                    {
                        if (!this.OwningGrid.IsReadOnly &&
                            this.OwningColumn != null &&
                            !this.OwningColumn.IsReadOnly)
                        {
                            return this;
                        }

                        break;
                    }

                    case PatternInterface.ScrollItem:
                    {
                        if (this.OwningGrid.HorizontalScrollBar != null &&
                            this.OwningGrid.HorizontalScrollBar.Maximum > 0)
                        {
                            return this;
                        }

                        break;
                    }

                    case PatternInterface.GridItem:
                    case PatternInterface.SelectionItem:
                    case PatternInterface.TableItem:
                        return this;
                }
            }

            return base.GetPatternCore(patternInterface);
        }

        /// <summary>
        /// Gets a value that indicates whether the element can accept keyboard focus.
        /// </summary>
        /// <returns>true if the element can accept keyboard focus; otherwise, false</returns>
        protected override bool IsKeyboardFocusableCore()
        {
            return true;
        }

        int IGridItemProvider.Column
        {
            get
            {
                int column = this.OwningCell.ColumnIndex;
                if (column >= 0 && this.OwningGrid != null && this.OwningGrid.ColumnsInternal.RowGroupSpacerColumn.IsRepresented)
                {
                    column--;
                }

                return column;
            }
        }

        int IGridItemProvider.ColumnSpan
        {
            get
            {
                return 1;
            }
        }

        IRawElementProviderSimple IGridItemProvider.ContainingGrid
        {
            get
            {
                return this.ContainingGrid;
            }
        }

        int IGridItemProvider.Row
        {
            get
            {
                return this.OwningCell.RowIndex;
            }
        }

        int IGridItemProvider.RowSpan
        {
            get
            {
                return 1;
            }
        }

        void IInvokeProvider.Invoke()
        {
            EnsureEnabled();

            if (this.OwningGrid != null)
            {
                if (this.OwningGrid.WaitForLostFocus(() => { ((IInvokeProvider)this).Invoke(); }))
                {
                    return;
                }

                if (this.OwningGrid.EditingRow == this.OwningRow && this.OwningGrid.EditingColumnIndex == this.OwningCell.ColumnIndex)
                {
                    this.OwningGrid.CommitEdit(DataGridEditingUnit.Cell, true /*exitEditingMode*/);
                }
                else if (this.OwningGrid.UpdateSelectionAndCurrency(this.OwningCell.ColumnIndex, this.OwningRow.Slot, DataGridSelectionAction.SelectCurrent, true))
                {
                    this.OwningGrid.BeginEdit();
                }
            }
        }

        void IScrollItemProvider.ScrollIntoView()
        {
            if (this.OwningGrid != null)
            {
                this.OwningGrid.ScrollIntoView(this.OwningCell.DataContext, this.OwningColumn);
            }
            else
            {
                throw DataGridError.DataGridAutomationPeer.OperationCannotBePerformed();
            }
        }

        bool ISelectionItemProvider.IsSelected
        {
            get
            {
                if (this.OwningGrid != null && this.OwningRow != null)
                {
                    return this.OwningRow.IsSelected;
                }

                throw DataGridError.DataGridAutomationPeer.OperationCannotBePerformed();
            }
        }

        IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
        {
            get
            {
                AutomationPeer peer = CreatePeerForElement(this.OwningRow);
                if (peer != null)
                {
                    return ProviderFromPeer(peer);
                }

                return null;
            }
        }

        void ISelectionItemProvider.AddToSelection()
        {
            EnsureEnabled();
            if (this.OwningCell.OwningGrid == null ||
                this.OwningCell.OwningGrid.CurrentSlot != this.OwningCell.RowIndex ||
                this.OwningCell.OwningGrid.CurrentColumnIndex != this.OwningCell.ColumnIndex)
            {
                throw DataGridError.DataGridAutomationPeer.OperationCannotBePerformed();
            }
        }

        void ISelectionItemProvider.RemoveFromSelection()
        {
            EnsureEnabled();
            if (this.OwningCell.OwningGrid == null ||
                (this.OwningCell.OwningGrid.CurrentSlot == this.OwningCell.RowIndex &&
                 this.OwningCell.OwningGrid.CurrentColumnIndex == this.OwningCell.ColumnIndex))
            {
                throw DataGridError.DataGridAutomationPeer.OperationCannotBePerformed();
            }
        }

        void ISelectionItemProvider.Select()
        {
            EnsureEnabled();

            if (this.OwningGrid != null)
            {
                if (this.OwningGrid.WaitForLostFocus(() => { ((ISelectionItemProvider)this).Select(); }))
                {
                    return;
                }

                this.OwningGrid.UpdateSelectionAndCurrency(this.OwningCell.ColumnIndex, this.OwningRow.Slot, DataGridSelectionAction.SelectCurrent, false);
            }
        }

        IRawElementProviderSimple[] ITableItemProvider.GetColumnHeaderItems()
        {
            if (this.OwningGrid != null &&
                this.OwningGrid.AreColumnHeadersVisible &&
                this.OwningColumn.HeaderCell != null)
            {
                AutomationPeer peer = CreatePeerForElement(this.OwningColumn.HeaderCell);
                if (peer != null)
                {
                    List<IRawElementProviderSimple> providers = new List<IRawElementProviderSimple>(1);
                    providers.Add(ProviderFromPeer(peer));
                    return providers.ToArray();
                }
            }

            return null;
        }

        IRawElementProviderSimple[] ITableItemProvider.GetRowHeaderItems()
        {
            if (this.OwningGrid != null &&
                this.OwningGrid.AreRowHeadersVisible &&
                this.OwningRow.HeaderCell != null)
            {
                AutomationPeer peer = CreatePeerForElement(this.OwningRow.HeaderCell);
                if (peer != null)
                {
                    List<IRawElementProviderSimple> providers = new List<IRawElementProviderSimple>(1);
                    providers.Add(ProviderFromPeer(peer));
                    return providers.ToArray();
                }
            }

            return null;
        }

        private void EnsureEnabled()
        {
            if (!IsEnabled())
            {
                throw new ElementNotEnabledException();
            }
        }
    }
}