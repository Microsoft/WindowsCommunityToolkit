// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Xaml;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Provides data for the <see cref="E:CommunityToolkit.WinUI.UI.Controls.DataGrid.LoadingRowDetails"/>, <see cref="E:CommunityToolkit.WinUI.UI.Controls.DataGrid.UnloadingRowDetails"/>,
    /// and <see cref="E:CommunityToolkit.WinUI.UI.Controls.DataGrid.RowDetailsVisibilityChanged"/> events.
    /// </summary>
    public class DataGridRowDetailsEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridRowDetailsEventArgs"/> class.
        /// </summary>
        /// <param name="row">The row that the event occurs for.</param>
        /// <param name="detailsElement">The row details section as a framework element.</param>
        public DataGridRowDetailsEventArgs(DataGridRow row, FrameworkElement detailsElement)
        {
            this.Row = row;
            this.DetailsElement = detailsElement;
        }

        /// <summary>
        /// Gets the row details section as a framework element.
        /// </summary>
        public FrameworkElement DetailsElement
        {
            get;
            private set;
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