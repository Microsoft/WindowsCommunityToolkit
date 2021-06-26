// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Microsoft.Toolkit.Uwp.UI.Controls.Design.Properties;

using Microsoft.VisualStudio.DesignTools.Extensibility;
using Microsoft.VisualStudio.DesignTools.Extensibility.Features;
using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;
using Microsoft.VisualStudio.DesignTools.Extensibility.Model;

namespace Microsoft.Toolkit.Uwp.UI.Controls.Design
{
    internal class DataGridDefaults : DefaultInitializer
    {
        public override void InitializeDefaults(ModelItem item)
        {
            item.Properties[nameof(DataGrid.Height)].SetValue(50d);
            item.Properties[nameof(DataGrid.Width)].SetValue(100d);
        }
    }

    internal class DataGridMetadata : AttributeTableBuilder
    {
        public DataGridMetadata() : base()
        {
            AddCallback(ControlTypes.DataGrid,
                b =>
                {
                    b.AddCustomAttributes(new FeatureAttribute(typeof(DataGridDefaults)));
                    b.AddCustomAttributes(nameof(DataGrid.AlternatingRowBackground), new CategoryAttribute(Resources.CategoryRows));
                    b.AddCustomAttributes(nameof(DataGrid.AreRowDetailsFrozen), new CategoryAttribute(Resources.CategoryRows));
                    b.AddCustomAttributes(nameof(DataGrid.AreRowGroupHeadersFrozen), new CategoryAttribute(Resources.CategoryHeaders));
                    b.AddCustomAttributes(nameof(DataGrid.AutoGenerateColumns), new CategoryAttribute(Resources.CategoryColumns));
                    b.AddCustomAttributes(nameof(DataGrid.CanUserReorderColumns), new CategoryAttribute(Resources.CategoryColumns));
                    b.AddCustomAttributes(nameof(DataGrid.CanUserResizeColumns), new CategoryAttribute(Resources.CategoryColumns));
                    b.AddCustomAttributes(nameof(DataGrid.CanUserSortColumns), new CategoryAttribute(Resources.CategoryColumns));
                    b.AddCustomAttributes(nameof(DataGrid.CellStyle), new CategoryAttribute(Resources.CategoryColumns));
                    b.AddCustomAttributes(nameof(DataGrid.ClipboardCopyMode), new CategoryAttribute(Resources.CategoryText));
                    b.AddCustomAttributes(nameof(DataGrid.ColumnHeaderHeight), new CategoryAttribute(Resources.CategoryHeaders));
                    b.AddCustomAttributes(nameof(DataGrid.ColumnHeaderStyle), new CategoryAttribute(Resources.CategoryHeaders));
                    b.AddCustomAttributes(nameof(DataGrid.Columns), new CategoryAttribute(Resources.CategoryColumns));
                    b.AddCustomAttributes(nameof(DataGrid.ColumnWidth), new CategoryAttribute(Resources.CategoryColumns));
                    b.AddCustomAttributes(nameof(DataGrid.CurrentColumn), new CategoryAttribute(Resources.CategoryText));
                    b.AddCustomAttributes(nameof(DataGrid.DragIndicatorStyle), new CategoryAttribute(Resources.CategoryText));
                    b.AddCustomAttributes(nameof(DataGrid.DropLocationIndicatorStyle), new CategoryAttribute(Resources.CategoryText));
                    b.AddCustomAttributes(nameof(DataGrid.FrozenColumnCount), new CategoryAttribute(Resources.CategoryColumns));
                    b.AddCustomAttributes(nameof(DataGrid.GridLinesVisibility), new CategoryAttribute(Resources.CategoryGridLines));
                    b.AddCustomAttributes(nameof(DataGrid.HeadersVisibility), new CategoryAttribute(Resources.CategoryHeaders));
                    b.AddCustomAttributes(nameof(DataGrid.HorizontalGridLinesBrush), new CategoryAttribute(Resources.CategoryGridLines));
                    b.AddCustomAttributes(nameof(DataGrid.HorizontalScrollBarVisibility), new CategoryAttribute(Resources.CategoryLayout));
                    b.AddCustomAttributes(nameof(DataGrid.IsReadOnly), new CategoryAttribute(Resources.CategoryText));
                    b.AddCustomAttributes(nameof(DataGrid.IsValid), new CategoryAttribute(Resources.CategoryText));
                    b.AddCustomAttributes(nameof(DataGrid.ItemsSource), new CategoryAttribute(Resources.CategoryColumns));
                    b.AddCustomAttributes(nameof(DataGrid.MaxColumnWidth), new CategoryAttribute(Resources.CategoryColumns));
                    b.AddCustomAttributes(nameof(DataGrid.MinColumnWidth), new CategoryAttribute(Resources.CategoryColumns));
                    b.AddCustomAttributes(nameof(DataGrid.RowBackground), new CategoryAttribute(Resources.CategoryRows));
                    b.AddCustomAttributes(nameof(DataGrid.RowDetailsTemplate), new CategoryAttribute(Resources.CategoryRows));
                    b.AddCustomAttributes(nameof(DataGrid.RowDetailsVisibilityMode), new CategoryAttribute(Resources.CategoryRows));
                    b.AddCustomAttributes(nameof(DataGrid.RowGroupHeaderPropertyNameAlternative), new CategoryAttribute(Resources.CategoryHeaders));
                    b.AddCustomAttributes(nameof(DataGrid.RowGroupHeaderStyles), new CategoryAttribute(Resources.CategoryHeaders));
                    b.AddCustomAttributes(nameof(DataGrid.RowHeaderStyle), new CategoryAttribute(Resources.CategoryHeaders));
                    b.AddCustomAttributes(nameof(DataGrid.RowHeaderWidth), new CategoryAttribute(Resources.CategoryHeaders));
                    b.AddCustomAttributes(nameof(DataGrid.RowHeight), new CategoryAttribute(Resources.CategoryRows));
                    b.AddCustomAttributes(nameof(DataGrid.RowStyle), new CategoryAttribute(Resources.CategoryRows));
                    b.AddCustomAttributes(nameof(DataGrid.SelectedIndex), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(DataGrid.SelectedItem), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(DataGrid.SelectedItems), new CategoryAttribute(Resources.CategoryAppearance));
                    b.AddCustomAttributes(nameof(DataGrid.SelectionMode), new CategoryAttribute(Resources.CategoryRows));
                    b.AddCustomAttributes(nameof(DataGrid.VerticalGridLinesBrush), new CategoryAttribute(Resources.CategoryGridLines));
                    b.AddCustomAttributes(nameof(DataGrid.VerticalScrollBarVisibility), new CategoryAttribute(Resources.CategoryLayout));
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Toolkit, false));
                });

            AddCallback(ControlTypes.DataGridColumn,
                b =>
                {
                    b.AddCustomAttributes(nameof(DataGridColumn.CanUserResize), new CategoryAttribute(Resources.CategoryLayout));
                    b.AddCustomAttributes(nameof(DataGridColumn.CanUserSort), new CategoryAttribute(Resources.CategorySort));
                    b.AddCustomAttributes(nameof(DataGridColumn.Header), new CategoryAttribute(Resources.CategoryHeader));
                    b.AddCustomAttributes(nameof(DataGridColumn.HeaderStyle), new CategoryAttribute(Resources.CategoryHeader));
                    b.AddCustomAttributes(nameof(DataGridColumn.MaxWidth), new CategoryAttribute(Resources.CategoryLayout));
                    b.AddCustomAttributes(nameof(DataGridColumn.MinWidth), new CategoryAttribute(Resources.CategoryLayout));
                    b.AddCustomAttributes(nameof(DataGridColumn.SortDirection), new CategoryAttribute(Resources.CategorySort));
                    b.AddCustomAttributes(nameof(DataGridColumn.Visibility), new CategoryAttribute(Resources.CategoryAppearance));
                    b.AddCustomAttributes(nameof(DataGridColumn.Width), new CategoryAttribute(Resources.CategoryLayout));
                });

            AddCallback(ControlTypes.DataGridBoundColumn,
                b =>
                {
                    b.AddCustomAttributes(nameof(DataGridBoundColumn.Binding), new CategoryAttribute(Resources.CategoryCellBinding));
                });

            AddCallback(ControlTypes.DataGridTextColumn,
                b =>
                {
                    b.AddCustomAttributes(nameof(DataGridTextColumn.FontFamily), new CategoryAttribute(Resources.CategoryText));
                    b.AddCustomAttributes(nameof(DataGridTextColumn.FontSize), new CategoryAttribute(Resources.CategoryText));
                    b.AddCustomAttributes(nameof(DataGridTextColumn.FontStyle), new CategoryAttribute(Resources.CategoryText));
                    b.AddCustomAttributes(nameof(DataGridTextColumn.FontWeight), new CategoryAttribute(Resources.CategoryText));
                    b.AddCustomAttributes(nameof(DataGridTextColumn.Foreground), new CategoryAttribute(Resources.CategoryText));
                });

            AddCallback(ControlTypes.DataGridCheckBoxColumn,
                b =>
                {
                    b.AddCustomAttributes(nameof(DataGridCheckBoxColumn.IsThreeState), new CategoryAttribute(Resources.CategoryCommon));
                });

            AddCallback(ControlTypes.DataGridTemplateColumn,
                b =>
                {
                    b.AddCustomAttributes(nameof(DataGridTemplateColumn.CellEditingTemplate), new CategoryAttribute(Resources.CategoryCellTemplate));
                    b.AddCustomAttributes(nameof(DataGridTemplateColumn.CellTemplate), new CategoryAttribute(Resources.CategoryCellTemplate));
                });
        }
    }
}