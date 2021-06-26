// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// EventArgs used for the DataGrid's LoadingRowGroup and UnloadingRowGroup events
    /// </summary>
    public class DataGridRowGroupHeaderEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridRowGroupHeaderEventArgs"/> class.
        /// </summary>
        /// <param name="rowGroupHeader">The row group header that the event occurs for.</param>
        public DataGridRowGroupHeaderEventArgs(DataGridRowGroupHeader rowGroupHeader)
        {
            this.RowGroupHeader = rowGroupHeader;
        }

        /// <summary>
        /// Gets the <see cref="DataGridRowGroupHeader"/> associated with this instance.
        /// </summary>
        public DataGridRowGroupHeader RowGroupHeader
        {
            get;
            private set;
        }
    }
}