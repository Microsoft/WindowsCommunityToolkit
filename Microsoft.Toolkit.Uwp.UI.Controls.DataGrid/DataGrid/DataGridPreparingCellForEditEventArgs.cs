// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Provides data for the <see cref="E:Microsoft.Toolkit.Uwp.UI.Controls.DataGrid.PreparingCellForEdit"/> event.
    /// </summary>
    public class DataGridPreparingCellForEditEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridPreparingCellForEditEventArgs"/> class.
        /// </summary>
        /// <param name="column">The column that contains the cell to be edited.</param>
        /// <param name="row">The row that contains the cell to be edited.</param>
        /// <param name="editingEventArgs">Information about the user gesture that caused the cell to enter edit mode.</param>
        /// <param name="editingElement">The element that the column displays for a cell in editing mode.</param>
        public DataGridPreparingCellForEditEventArgs(
            DataGridColumn column,
            DataGridRow row,
            RoutedEventArgs editingEventArgs,
            FrameworkElement editingElement)
        {
            this.Column = column;
            this.Row = row;
            this.EditingEventArgs = editingEventArgs;
            this.EditingElement = editingElement;
        }

        /// <summary>
        /// Gets the column that contains the cell to be edited.
        /// </summary>
        public DataGridColumn Column
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the element that the column displays for a cell in editing mode.
        /// </summary>
        public FrameworkElement EditingElement
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets information about the user gesture that caused the cell to enter edit mode.
        /// </summary>
        public RoutedEventArgs EditingEventArgs
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the row that contains the cell to be edited.
        /// </summary>
        public DataGridRow Row
        {
            get;
            private set;
        }
    }
}