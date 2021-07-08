// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.UI.Controls.Design
{
    internal static partial class ControlTypes
    {
        internal const string DataGrid = RootNamespace + "." + nameof(DataGrid);
        internal const string DataGridColumn = RootNamespace + "." + nameof(DataGridColumn);
        internal const string DataGridBoundColumn = RootNamespace + "." + nameof(DataGridBoundColumn);
        internal const string DataGridTextColumn = RootNamespace + "." + nameof(DataGridTextColumn);
        internal const string DataGridCheckBoxColumn = RootNamespace + "." + nameof(DataGridCheckBoxColumn);
        internal const string DataGridTemplateColumn = RootNamespace + "." + nameof(DataGridTemplateColumn);
    }

    internal static class DataGrid
    {
        internal const string AlternatingRowBackground = nameof(AlternatingRowBackground);
        internal const string AreRowDetailsFrozen = nameof(AreRowDetailsFrozen);
        internal const string AreRowGroupHeadersFrozen = nameof(AreRowGroupHeadersFrozen);
        internal const string AutoGenerateColumns = nameof(AutoGenerateColumns);
        internal const string CanUserReorderColumns = nameof(CanUserReorderColumns);
        internal const string CanUserResizeColumns = nameof(CanUserResizeColumns);
        internal const string CanUserSortColumns = nameof(CanUserSortColumns);
        internal const string CellStyle = nameof(CellStyle);
        internal const string ClipboardCopyMode = nameof(ClipboardCopyMode);
        internal const string ColumnHeaderHeight = nameof(ColumnHeaderHeight);
        internal const string ColumnHeaderStyle = nameof(ColumnHeaderStyle);
        internal const string Columns = nameof(Columns);
        internal const string ColumnWidth = nameof(ColumnWidth);
        internal const string CurrentColumn = nameof(CurrentColumn);
        internal const string DragIndicatorStyle = nameof(DragIndicatorStyle);
        internal const string DropLocationIndicatorStyle = nameof(DropLocationIndicatorStyle);
        internal const string FrozenColumnCount = nameof(FrozenColumnCount);
        internal const string GridLinesVisibility = nameof(GridLinesVisibility);
        internal const string HeadersVisibility = nameof(HeadersVisibility);
        internal const string Height = nameof(Height);
        internal const string HorizontalGridLinesBrush = nameof(HorizontalGridLinesBrush);
        internal const string HorizontalScrollBarVisibility = nameof(HorizontalScrollBarVisibility);
        internal const string IsReadOnly = nameof(IsReadOnly);
        internal const string IsValid = nameof(IsValid);
        internal const string ItemsSource = nameof(ItemsSource);
        internal const string MaxColumnWidth = nameof(MaxColumnWidth);
        internal const string MinColumnWidth = nameof(MinColumnWidth);
        internal const string RowBackground = nameof(RowBackground);
        internal const string RowDetailsTemplate = nameof(RowDetailsTemplate);
        internal const string RowDetailsVisibilityMode = nameof(RowDetailsVisibilityMode);
        internal const string RowGroupHeaderPropertyNameAlternative = nameof(RowGroupHeaderPropertyNameAlternative);
        internal const string RowGroupHeaderStyles = nameof(RowGroupHeaderStyles);
        internal const string RowHeaderStyle = nameof(RowHeaderStyle);
        internal const string RowHeaderWidth = nameof(RowHeaderWidth);
        internal const string RowHeight = nameof(RowHeight);
        internal const string RowStyle = nameof(RowStyle);
        internal const string SelectedIndex = nameof(SelectedIndex);
        internal const string SelectedItem = nameof(SelectedItem);
        internal const string SelectedItems = nameof(SelectedItems);
        internal const string SelectionMode = nameof(SelectionMode);
        internal const string VerticalGridLinesBrush = nameof(VerticalGridLinesBrush);
        internal const string VerticalScrollBarVisibility = nameof(VerticalScrollBarVisibility);
        internal const string Width = nameof(Width);
    }

    internal static class DataGridColumn
    {
        internal const string CanUserResize = nameof(CanUserResize);
        internal const string CanUserSort = nameof(CanUserSort);
        internal const string Header = nameof(Header);
        internal const string HeaderStyle = nameof(HeaderStyle);
        internal const string MaxWidth = nameof(MaxWidth);
        internal const string MinWidth = nameof(MinWidth);
        internal const string SortDirection = nameof(SortDirection);
        internal const string Visibility = nameof(Visibility);
        internal const string Width = nameof(Width);
    }

    internal static class DataGridBoundColumn
    {
        internal const string Binding = nameof(Binding);
    }

    internal static class DataGridTextColumn
    {
        internal const string FontFamily = nameof(FontFamily);
        internal const string FontSize = nameof(FontSize);
        internal const string FontStyle = nameof(FontStyle);
        internal const string FontWeight = nameof(FontWeight);
        internal const string Foreground = nameof(Foreground);
    }

    internal static class DataGridCheckBoxColumn
    {
        internal const string IsThreeState = nameof(IsThreeState);
    }

    internal static class DataGridTemplateColumn
    {
        internal const string CellEditingTemplate = nameof(CellEditingTemplate);
        internal const string CellTemplate = nameof(CellTemplate);
    }
}