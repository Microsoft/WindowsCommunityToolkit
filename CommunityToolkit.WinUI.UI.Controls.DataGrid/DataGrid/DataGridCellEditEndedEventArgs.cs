// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Provides information just after a cell has exited editing mode.
    /// </summary>
    public class DataGridCellEditEndedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridCellEditEndedEventArgs"/> class.
        /// </summary>
        /// <param name="column">The column of the cell that has just exited edit mode.</param>
        /// <param name="row">The row container of the cell container that has just exited edit mode.</param>
        /// <param name="editAction">The editing action that has been taken.</param>
        public DataGridCellEditEndedEventArgs(DataGridColumn column, DataGridRow row, DataGridEditAction editAction)
        {
            this.Column = column;
            this.Row = row;
            this.EditAction = editAction;
        }

        /// <summary>
        /// Gets the column of the cell that has just exited edit mode.
        /// </summary>
        public DataGridColumn Column
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the edit action taken when leaving edit mode.
        /// </summary>
        public DataGridEditAction EditAction
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the row container of the cell container that has just exited edit mode.
        /// </summary>
        public DataGridRow Row
        {
            get;
            private set;
        }
    }
}