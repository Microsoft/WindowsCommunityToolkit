// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Microsoft.Toolkit.Uwp.SampleApp.Data;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    public sealed partial class DataGridPage : Page, IXamlRenderListener
    {
        private DataGrid dataGrid;
        private AppBarButton groupButton;
        private MenuFlyoutItem rankLowItem;
        private MenuFlyoutItem rankHighItem;
        private MenuFlyoutItem heightLowItem;
        private MenuFlyoutItem heightHighItem;
        private DataGridDataSource viewModel = new DataGridDataSource();

        public DataGridPage()
        {
            InitializeComponent();
        }

        public async void OnXamlRendered(FrameworkElement control)
        {
            if (dataGrid != null)
            {
                dataGrid.Sorting -= DataGrid_Sorting;
                dataGrid.LoadingRowGroup -= DataGrid_LoadingRowGroup;
            }

            dataGrid = control.FindDescendant("dataGrid") as DataGrid;
            if (dataGrid != null)
            {
                dataGrid.Sorting += DataGrid_Sorting;
                dataGrid.LoadingRowGroup += DataGrid_LoadingRowGroup;
                dataGrid.ItemsSource = await viewModel.GetDataAsync();

                var comboBoxColumn = dataGrid.Columns.FirstOrDefault(x => x.Tag.Equals("Mountain")) as DataGridComboBoxColumn;
                if (comboBoxColumn != null)
                {
                    comboBoxColumn.ItemsSource = await viewModel.GetMountains();
                }
            }

            if (groupButton != null)
            {
                groupButton.Click -= GroupButton_Click;
            }

            groupButton = control.FindDescendant("groupButton") as AppBarButton;
            if (groupButton != null)
            {
                groupButton.Click += GroupButton_Click;
            }

            if (rankLowItem != null)
            {
                rankLowItem.Click -= RankLowItem_Click;
            }

            rankLowItem = control.FindName("rankLow") as MenuFlyoutItem;
            if (rankLowItem != null)
            {
                rankLowItem.Click += RankLowItem_Click;
            }

            if (rankHighItem != null)
            {
                rankHighItem.Click -= RankHigh_Click;
            }

            rankHighItem = control.FindName("rankHigh") as MenuFlyoutItem;
            if (rankHighItem != null)
            {
                rankHighItem.Click += RankHigh_Click;
            }

            if (heightLowItem != null)
            {
                heightLowItem.Click -= HeightLow_Click;
            }

            heightLowItem = control.FindName("heightLow") as MenuFlyoutItem;
            if (heightLowItem != null)
            {
                heightLowItem.Click += HeightLow_Click;
            }

            if (heightHighItem != null)
            {
                heightHighItem.Click -= HeightHigh_Click;
            }

            heightHighItem = control.FindName("heightHigh") as MenuFlyoutItem;
            if (heightHighItem != null)
            {
                heightHighItem.Click += HeightHigh_Click;
            }

            var clearFilter = control.FindName("clearFilter") as MenuFlyoutItem;
            if (clearFilter != null)
            {
                clearFilter.Click += this.ClearFilter_Click;
            }
        }

        private void DataGrid_LoadingRowGroup(object sender, DataGridRowGroupHeaderEventArgs e)
        {
            ICollectionViewGroup group = e.RowGroupHeader.CollectionViewGroup;
            DataGridDataItem item = group.GroupItems[0] as DataGridDataItem;
            e.RowGroupHeader.PropertyValue = item.Range;
        }

        private void GroupButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid != null)
            {
                dataGrid.ItemsSource = viewModel.GroupData().View;
            }
        }

        private void DataGrid_Sorting(object sender, DataGridColumnEventArgs e)
        {
            // Clear previous sorted column if we start sorting a different column
            string previousSortedColumn = viewModel.CachedSortedColumn;
            if (previousSortedColumn != string.Empty)
            {
                foreach (DataGridColumn dataGridColumn in dataGrid.Columns)
                {
                    if (dataGridColumn.Tag != null && dataGridColumn.Tag.ToString() == previousSortedColumn &&
                        (e.Column.Tag == null || previousSortedColumn != e.Column.Tag.ToString()))
                    {
                        dataGridColumn.SortDirection = null;
                    }
                }
            }

            // Toggle clicked column's sorting method
            if (e.Column.Tag != null)
            {
                if (e.Column.SortDirection == null)
                {
                    dataGrid.ItemsSource = viewModel.SortData(e.Column.Tag.ToString(), true);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else if (e.Column.SortDirection == DataGridSortDirection.Ascending)
                {
                    dataGrid.ItemsSource = viewModel.SortData(e.Column.Tag.ToString(), false);
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
                else
                {
                    dataGrid.ItemsSource = viewModel.FilterData(DataGridDataSource.FilterOptions.All);
                    e.Column.SortDirection = null;
                }
            }
        }

        private void RankLowItem_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid != null)
            {
                dataGrid.ItemsSource = viewModel.FilterData(DataGridDataSource.FilterOptions.Rank_Low);
            }
        }

        private void RankHigh_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid != null)
            {
                dataGrid.ItemsSource = viewModel.FilterData(DataGridDataSource.FilterOptions.Rank_High);
            }
        }

        private void HeightLow_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid != null)
            {
                dataGrid.ItemsSource = viewModel.FilterData(DataGridDataSource.FilterOptions.Height_Low);
            }
        }

        private void HeightHigh_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid != null)
            {
                dataGrid.ItemsSource = viewModel.FilterData(DataGridDataSource.FilterOptions.Height_High);
            }
        }

        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid != null)
            {
                dataGrid.ItemsSource = viewModel.FilterData(DataGridDataSource.FilterOptions.All);
            }
        }
    }
}
