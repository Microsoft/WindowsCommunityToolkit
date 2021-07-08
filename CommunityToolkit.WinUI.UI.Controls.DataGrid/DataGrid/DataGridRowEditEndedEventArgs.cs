// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Provides information just after a row has exited edit mode.
    /// </summary>
    public class DataGridRowEditEndedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridRowEditEndedEventArgs"/> class.
        /// </summary>
        /// <param name="row">The row container of the cell container that has just exited edit mode.</param>
        /// <param name="editAction">The editing action that has been taken.</param>
        public DataGridRowEditEndedEventArgs(DataGridRow row, DataGridEditAction editAction)
        {
            this.Row = row;
            this.EditAction = editAction;
        }

        /// <summary>
        /// Gets the editing action that has been taken.
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