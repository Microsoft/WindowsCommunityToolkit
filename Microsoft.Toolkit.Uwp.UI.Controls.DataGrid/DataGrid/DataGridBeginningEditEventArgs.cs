// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Provides data for the <see cref="E:Microsoft.Toolkit.Uwp.UI.Controls.DataGrid.BeginningEdit" /> event.
    /// </summary>
    public class DataGridBeginningEditEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridBeginningEditEventArgs"/> class.
        /// </summary>
        /// <param name="column">
        /// The column that contains the cell to be edited.
        /// </param>
        /// <param name="row">
        /// The row that contains the cell to be edited.
        /// </param>
        /// <param name="editingEventArgs">
        /// Information about the user gesture that caused the cell to enter edit mode.
        /// </param>
        public DataGridBeginningEditEventArgs(
            DataGridColumn column,
            DataGridRow row,
            RoutedEventArgs editingEventArgs)
        {
            this.Column = column;
            this.Row = row;
            this.EditingEventArgs = editingEventArgs;
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