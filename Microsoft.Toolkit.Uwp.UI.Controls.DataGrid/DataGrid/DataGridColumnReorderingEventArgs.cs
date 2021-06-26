// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Provides data for the <see cref="E:Microsoft.Toolkit.Uwp.UI.Controls.DataGrid.ColumnReordering" /> event.
    /// </summary>
    public class DataGridColumnReorderingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridColumnReorderingEventArgs"/> class.
        /// </summary>
        /// <param name="dataGridColumn">The column that the event occurs for.</param>
        public DataGridColumnReorderingEventArgs(DataGridColumn dataGridColumn)
        {
            this.Column = dataGridColumn;
        }

        /// <summary>
        /// Gets the column being moved.
        /// </summary>
        public DataGridColumn Column
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the popup indicator displayed while dragging. If null and Handled = true, then do not display a tooltip.
        /// </summary>
        public Control DragIndicator
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Control to display at the insertion position. If null and Handled = true, then do not display an insertion indicator.
        /// </summary>
        public Control DropLocationIndicator
        {
            get;
            set;
        }
    }
}