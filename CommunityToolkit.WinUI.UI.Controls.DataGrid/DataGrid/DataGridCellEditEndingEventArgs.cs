// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Microsoft.UI.Xaml;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Provides information just before a cell exits editing mode.
    /// </summary>
    public class DataGridCellEditEndingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridCellEditEndingEventArgs"/> class.
        /// </summary>
        /// <param name="column">The column of the cell that is about to exit edit mode.</param>
        /// <param name="row">The row container of the cell container that is about to exit edit mode.</param>
        /// <param name="editingElement">The editing element within the cell.</param>
        /// <param name="editAction">The editing action that will be taken.</param>
        public DataGridCellEditEndingEventArgs(
            DataGridColumn column,
            DataGridRow row,
            FrameworkElement editingElement,
            DataGridEditAction editAction)
        {
            this.Column = column;
            this.Row = row;
            this.EditingElement = editingElement;
            this.EditAction = editAction;
        }

        /// <summary>
        /// Gets the column of the cell that is about to exit edit mode.
        /// </summary>
        public DataGridColumn Column
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the edit action to take when leaving edit mode.
        /// </summary>
        public DataGridEditAction EditAction
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the editing element within the cell.
        /// </summary>
        public FrameworkElement EditingElement
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the row container of the cell container that is about to exit edit mode.
        /// </summary>
        public DataGridRow Row
        {
            get;
            private set;
        }
    }
}