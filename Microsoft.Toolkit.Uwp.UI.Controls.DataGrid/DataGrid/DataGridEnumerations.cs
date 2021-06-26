// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Defines modes that indicates how DataGrid content is copied to the Clipboard.
    /// </summary>
    public enum DataGridClipboardCopyMode
    {
        /// <summary>
        /// Disable the DataGrid's ability to copy selected items as text.
        /// </summary>
        None,

        /// <summary>
        /// Enable the DataGrid's ability to copy selected items as text, but do not include
        /// the column header content as the first line in the text that gets copied to the Clipboard.
        /// </summary>
        ExcludeHeader,

        /// <summary>
        /// Enable the DataGrid's ability to copy selected items as text, and include
        /// the column header content as the first line in the text that gets copied to the Clipboard.
        /// </summary>
        IncludeHeader
    }

    /// <summary>
    /// Used to specify action to take out of edit mode.
    /// </summary>
    public enum DataGridEditAction
    {
        /// <summary>
        /// Cancel the changes.
        /// </summary>
        Cancel,

        /// <summary>
        /// Commit edited value.
        /// </summary>
        Commit
    }

    // Determines the location and visibility of the editing row.
    internal enum DataGridEditingRowLocation
    {
        Bottom = 0, // The editing row is collapsed below the displayed rows
        Inline = 1, // The editing row is visible and displayed
        Top = 2 // The editing row is collapsed above the displayed rows
    }

    /// <summary>
    /// Determines whether the inner cells' vertical/horizontal gridlines are shown or not.
    /// </summary>
    [Flags]
    public enum DataGridGridLinesVisibility
    {
        /// <summary>
        /// None DataGridGridLinesVisibility
        /// </summary>
        None = 0,

        /// <summary>
        /// Horizontal DataGridGridLinesVisibility
        /// </summary>
        Horizontal = 1,

        /// <summary>
        /// Vertical DataGridGridLinesVisibility
        /// </summary>
        Vertical = 2,

        /// <summary>
        /// All DataGridGridLinesVisibility
        /// </summary>
        All = 3,
    }

    /// <summary>
    /// Determines whether the current cell or row is edited.
    /// </summary>
    public enum DataGridEditingUnit
    {
        /// <summary>
        /// Cell DataGridEditingUnit
        /// </summary>
        Cell = 0,

        /// <summary>
        /// Row DataGridEditingUnit
        /// </summary>
        Row = 1,
    }

    /// <summary>
    /// Determines whether the row/column headers are shown or not.
    /// </summary>
    [Flags]
    public enum DataGridHeadersVisibility
    {
        /// <summary>
        /// Show Row, Column, and Corner Headers
        /// </summary>
        All = Row | Column,

        /// <summary>
        /// Show only Column Headers with top-right corner Header
        /// </summary>
        Column = 0x01,

        /// <summary>
        /// Show only Row Headers with bottom-left corner
        /// </summary>
        Row = 0x02,

        /// <summary>
        /// Don’t show any Headers
        /// </summary>
        None = 0x00
    }

    /// <summary>
    /// Determines the visibility of the row details.
    /// </summary>
    public enum DataGridRowDetailsVisibilityMode
    {
        /// <summary>
        /// Collapsed DataGridRowDetailsVisibilityMode
        /// </summary>
        Collapsed = 2, // Show no details.  Developer is in charge of toggling visibility.

        /// <summary>
        /// Visible DataGridRowDetailsVisibilityMode
        /// </summary>
        Visible = 1, // Show the details section for all rows.

        /// <summary>
        /// VisibleWhenSelected DataGridRowDetailsVisibilityMode
        /// </summary>
        VisibleWhenSelected = 0 // Show the details section only for the selected row(s).
    }

    /// <summary>
    /// Determines the type of action to take when selecting items.
    /// </summary>
    internal enum DataGridSelectionAction
    {
        AddCurrentToSelection,
        None,
        RemoveCurrentFromSelection,
        SelectCurrent,
        SelectFromAnchorToCurrent
    }

    /// <summary>
    /// Determines the selection model.
    /// </summary>
    public enum DataGridSelectionMode
    {
        /// <summary>
        /// Extended DataGridSelectionMode
        /// </summary>
        Extended = 0,

        /// <summary>
        /// Single DataGridSelectionMode
        /// </summary>
        Single = 1
    }

    /// <summary>
    /// Determines the sort direction of a column.
    /// </summary>
    public enum DataGridSortDirection
    {
        /// <summary>
        /// Sorts in ascending order.
        /// </summary>
        Ascending = 0,

        /// <summary>
        /// Sorts in descending order.
        /// </summary>
        Descending = 1
    }
}