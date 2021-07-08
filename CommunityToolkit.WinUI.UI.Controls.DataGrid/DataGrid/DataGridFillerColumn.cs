// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using CommunityToolkit.WinUI.UI.Controls.Primitives;
using Microsoft.UI.Xaml;

using DiagnosticsDebug = System.Diagnostics.Debug;

namespace CommunityToolkit.WinUI.UI.Controls
{
    internal class DataGridFillerColumn : DataGridColumn
    {
        public DataGridFillerColumn(DataGrid owningGrid)
        {
            this.IsReadOnly = true;
            this.OwningGrid = owningGrid;
            this.MinWidth = 0;
            this.MaxWidth = int.MaxValue;
        }

        internal double FillerWidth
        {
            get;
            set;
        }

        // True if there is room for the filler column; otherwise, false
        internal bool IsActive
        {
            get
            {
                return this.FillerWidth > 0;
            }
        }

        // True if the FillerColumn's header cell is contained in the visual tree
        internal bool IsRepresented
        {
            get;
            set;
        }

        internal override DataGridColumnHeader CreateHeader()
        {
            DataGridColumnHeader headerCell = base.CreateHeader();
            if (headerCell != null)
            {
                Microsoft.UI.Xaml.Automation.AutomationProperties.SetAccessibilityView(
                    headerCell,
                    Microsoft.UI.Xaml.Automation.Peers.AccessibilityView.Raw);
                headerCell.IsEnabled = false;
            }

            return headerCell;
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            return null;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return null;
        }

        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            DiagnosticsDebug.Assert(false, "Unexpected call to DataGridFillerColumn.PrepareCellForEdit.");

            return null;
        }
    }
}