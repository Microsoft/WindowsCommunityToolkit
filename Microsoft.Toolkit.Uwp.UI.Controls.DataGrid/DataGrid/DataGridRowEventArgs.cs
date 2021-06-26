// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Provides data for <see cref="DataGrid"/> row-related events.
    /// </summary>
    public class DataGridRowEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridRowEventArgs"/> class.
        /// </summary>
        /// <param name="dataGridRow">The row that the event occurs for.</param>
        public DataGridRowEventArgs(DataGridRow dataGridRow)
        {
            this.Row = dataGridRow;
        }

        /// <summary>
        /// Gets the row that the event occurs for.
        /// </summary>
        public DataGridRow Row
        {
            get;
            private set;
        }
    }
}