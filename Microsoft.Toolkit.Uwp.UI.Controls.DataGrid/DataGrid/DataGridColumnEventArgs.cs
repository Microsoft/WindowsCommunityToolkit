// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Provides data for <see cref="DataGrid"/> column-related events.
    /// </summary>
    public class DataGridColumnEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridColumnEventArgs"/> class.
        /// </summary>
        /// <param name="column">The column that the event occurs for.</param>
        public DataGridColumnEventArgs(DataGridColumn column)
        {
            if (column == null)
            {
                throw new ArgumentNullException("column");
            }

            this.Column = column;
        }

        /// <summary>
        /// Gets the column that the event occurs for.
        /// </summary>
        public DataGridColumn Column
        {
            get;
            private set;
        }
    }
}