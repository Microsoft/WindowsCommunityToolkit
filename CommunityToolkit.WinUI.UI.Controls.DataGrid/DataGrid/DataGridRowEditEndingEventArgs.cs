// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Provides information just before a row exits editing mode.
    /// </summary>
    public class DataGridRowEditEndingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridRowEditEndingEventArgs"/> class.
        /// </summary>
        /// <param name="row">The row container of the cell container that is about to exit edit mode.</param>
        /// <param name="editAction">The editing action that will be taken.</param>
        public DataGridRowEditEndingEventArgs(DataGridRow row, DataGridEditAction editAction)
        {
            this.Row = row;
            this.EditAction = editAction;
        }

        /// <summary>
        /// Gets the editing action that will be taken.
        /// </summary>
        public DataGridEditAction EditAction
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