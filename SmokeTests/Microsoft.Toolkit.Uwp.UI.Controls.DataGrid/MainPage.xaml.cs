// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml.Data;

namespace SmokeTest
{
    public sealed partial class MainPage
    {
        private DataGridDataSource viewModel = new DataGridDataSource();

        public MainPage()
        {
            InitializeComponent();

            dataGrid.ItemsSource = viewModel.GetData();
        }

        private void DataGrid_Sorting(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridColumnEventArgs e)
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

        private void DataGrid_LoadingRowGroup(object sender, DataGridRowGroupHeaderEventArgs e)
        {
            ICollectionViewGroup group = e.RowGroupHeader.CollectionViewGroup;
            DataGridDataItem item = group.GroupItems[0] as DataGridDataItem;
            e.RowGroupHeader.PropertyValue = item.Range;
        }
    }

    public class DataGridDataItem : INotifyDataErrorInfo, IComparable
    {
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        private uint _rank;
        private string _mountain;
        private uint _height;
        private string _range;
        private string _parentMountain;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public uint Rank
        {
            get
            {
                return _rank;
            }

            set
            {
                if (_rank != value)
                {
                    _rank = value;
                }
            }
        }

        public string Mountain
        {
            get
            {
                return _mountain;
            }

            set
            {
                if (_mountain != value)
                {
                    _mountain = value;

                    bool isMountainValid = !_errors.Keys.Contains("Mountain");
                    if (_mountain == string.Empty && isMountainValid)
                    {
                        List<string> errors = new List<string>();
                        errors.Add("Mountain name cannot be empty");
                        _errors.Add("Mountain", errors);
                        this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("Mountain"));
                    }
                    else if (_mountain != string.Empty && !isMountainValid)
                    {
                        _errors.Remove("Mountain");
                        this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("Mountain"));
                    }
                }
            }
        }

        public uint Height_m
        {
            get
            {
                return _height;
            }

            set
            {
                if (_height != value)
                {
                    _height = value;
                }
            }
        }

        public string Range
        {
            get
            {
                return _range;
            }

            set
            {
                if (_range != value)
                {
                    _range = value;

                    bool isRangeValid = !_errors.Keys.Contains("Range");
                    if (_range == string.Empty && isRangeValid)
                    {
                        List<string> errors = new List<string>();
                        errors.Add("Range name cannot be empty");
                        _errors.Add("Range", errors);
                        this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("Range"));
                    }
                    else if (_range != string.Empty && !isRangeValid)
                    {
                        _errors.Remove("Range");
                        this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("Range"));
                    }
                }
            }
        }

        public string Parent_mountain
        {
            get
            {
                return _parentMountain;
            }

            set
            {
                if (_parentMountain != value)
                {
                    _parentMountain = value;

                    bool isParentValid = !_errors.Keys.Contains("Parent_mountain");
                    if (_parentMountain == string.Empty && isParentValid)
                    {
                        List<string> errors = new List<string>();
                        errors.Add("Parent_mountain name cannot be empty");
                        _errors.Add("Parent_mountain", errors);
                        this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("Parent_mountain"));
                    }
                    else if (_parentMountain != string.Empty && !isParentValid)
                    {
                        _errors.Remove("Parent_mountain");
                        this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("Parent_mountain"));
                    }
                }
            }
        }

        public string Coordinates { get; set; }

        public uint Prominence { get; set; }

        public uint First_ascent { get; set; }

        public string Ascents { get; set; }

        bool INotifyDataErrorInfo.HasErrors
        {
            get
            {
                return _errors.Keys.Count > 0;
            }
        }

        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            if (propertyName == null)
            {
                propertyName = string.Empty;
            }

            if (_errors.Keys.Contains(propertyName))
            {
                return _errors[propertyName];
            }
            else
            {
                return null;
            }
        }

        int IComparable.CompareTo(object obj)
        {
            int lnCompare = Range.CompareTo((obj as DataGridDataItem).Range);

            if (lnCompare == 0)
            {
                return Parent_mountain.CompareTo((obj as DataGridDataItem).Parent_mountain);
            }
            else
            {
                return lnCompare;
            }
        }
    }

    public class DataGridDataSource
    {
        private const string File = @"1,Mount Everest,8848,Mahalangur Himalaya,27d59m17sN 86d55m31sE,8848,none,1953,>>145 (121)
2, K2/Qogir,8611,Baltoro Karakoram,35d52m53sN 76d30m48sE,4017,Mount Everest,1954,45 (44)
3,Kangchenjunga,8586,Kangchenjunga Himalaya,27d42m12sN 88d08m51sE*,3922,Mount Everest,1955,38 (24)
4,Lhotse,8516,Mahalangur Himalaya,27d57m42sN 86d55m59sE,610,Mount Everest,1956,26 (26)
5,Makalu,8485,Mahalangur Himalaya,27d53m23sN 87d5m20sE,2386,Mount Everest,1955,45 (52)
6,Cho Oyu,8188, Mahalangur Himalaya,28d05m39sN 86d39m39sE,2340,Mount Everest,1954,79 (28)
7,Dhaulagiri I,8167, Dhaulagiri Himalaya,28d41m48sN 83d29m35sE,3357,K2,1960,51 (39)
8,Manaslu,8163,Manaslu Himalaya,28d33m00sN 84d33m35sE,3092,Cho Oyu,1956,49 (45)
9,Nanga Parbat,8126, Nanga Parbat Himalaya,35d14m14sN 74d35m21sE,4608,Dhaulagiri,1953,52 (67)
10,Annapurna I,8091, Annapurna Himalaya,28d35m44sN 83d49m13sE,2984,Cho Oyu,1950,36 (47)
11,Gasherbrum I,8080, Baltoro Karakoram,35d43m28sN 76d41m47sE,2155,K2,1958,31 (16)
12,Broad Peak/K3,8051,Baltoro Karakoram,35d48m38sN 76d34m06sE,1701,Gasherbrum I,1957,39 (19)
13,Gasherbrum II/K4,8034,Baltoro Karakoram,35d45m28sN 76d39m12sE,1523,Gasherbrum I,1956,54 (12)
14,Shishapangma,8027,Jugal Himalaya,28d21m12sN 85d46m43sE,2897,Cho Oyu,1964,43 (19)
15,Gyachung Kang,7952, Mahalangur Himalaya,28d05m53sN 86d44m42sE,700,Cho Oyu,1964,5 (3)
15,Gasherbrum III,7946, Baltoro Karakoram,35d45m33sN 76d38m30sE,355,Gasherbrum II,1975,2 (2)
16,Annapurna II,7937, Annapurna Himalaya,28d32m05sN 84d07m19sE,2437,Annapurna I,1960,6 (19)
17,Gasherbrum IV,7932, Baltoro Karakoram,35d45m38sN 76d36m58sE,715,Gasherbrum III,1958,4 (11)
18,Himalchuli,7893,Manaslu Himalaya,28d26m12sN 84d38m23sE*,1633,Manaslu,1960,6 (12)
19,Distaghil Sar,7884, Hispar Karakoram,36d19m33sN 75d11m16sE,2525,K2,1960,3 (5)
20,Ngadi Chuli,7871, Manaslu Himalaya,28d30m12sN 84d34m00sE,1020,Manaslu,1970,2 (6)";

        private static ObservableCollection<DataGridDataItem> _items;
        private static List<string> _mountains;
        private static CollectionViewSource groupedItems;
        private string _cachedSortedColumn = string.Empty;

        // Loading data
        public IEnumerable<DataGridDataItem> GetData()
        {
            _items = new ObservableCollection<DataGridDataItem>();

            using (StringReader sr = new StringReader(File))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    string[] values = line.Split(',');

                    _items.Add(
                        new DataGridDataItem()
                        {
                            Rank = uint.Parse(values[0]),
                            Mountain = values[1],
                            Height_m = uint.Parse(values[2]),
                            Range = values[3],
                            Coordinates = values[4],
                            Prominence = uint.Parse(values[5]),
                            Parent_mountain = values[6],
                            First_ascent = uint.Parse(values[7]),
                            Ascents = values[8]
                        });
                }
            }

            return _items;
        }

        // Load mountains into separate collection for use in combobox column
        public IEnumerable<string> GetMountains()
        {
            if (_items == null || !_items.Any())
            {
                GetData();
            }

            _mountains = _items?.OrderBy(x => x.Mountain).Select(x => x.Mountain).Distinct().ToList();

            return _mountains;
        }

        // Sorting implementation using LINQ
        public string CachedSortedColumn
        {
            get
            {
                return _cachedSortedColumn;
            }

            set
            {
                _cachedSortedColumn = value;
            }
        }

        public ObservableCollection<DataGridDataItem> SortData(string sortBy, bool ascending)
        {
            _cachedSortedColumn = sortBy;
            switch (sortBy)
            {
                case "Rank":
                    if (ascending)
                    {
                        return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                          orderby item.Rank ascending
                                                                          select item);
                    }
                    else
                    {
                        return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                          orderby item.Rank descending
                                                                          select item);
                    }

                case "Parent_mountain":
                    if (ascending)
                    {
                        return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                          orderby item.Parent_mountain ascending
                                                                          select item);
                    }
                    else
                    {
                        return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                          orderby item.Parent_mountain descending
                                                                          select item);
                    }

                case "Mountain":
                    if (ascending)
                    {
                        return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                          orderby item.Mountain ascending
                                                                          select item);
                    }
                    else
                    {
                        return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                          orderby item.Mountain descending
                                                                          select item);
                    }

                case "Height_m":
                    if (ascending)
                    {
                        return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                          orderby item.Height_m ascending
                                                                          select item);
                    }
                    else
                    {
                        return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                          orderby item.Height_m descending
                                                                          select item);
                    }

                case "Range":
                    if (ascending)
                    {
                        return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                          orderby item.Range ascending
                                                                          select item);
                    }
                    else
                    {
                        return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                          orderby item.Range descending
                                                                          select item);
                    }
            }

            return _items;
        }

        // Grouping implementation using LINQ
        public CollectionViewSource GroupData()
        {
            ObservableCollection<GroupInfoCollection<DataGridDataItem>> groups = new ObservableCollection<GroupInfoCollection<DataGridDataItem>>();
            var query = from item in _items
                        orderby item
                        group item by item.Range into g
                        select new { GroupName = g.Key, Items = g };
            foreach (var g in query)
            {
                GroupInfoCollection<DataGridDataItem> info = new GroupInfoCollection<DataGridDataItem>();
                info.Key = g.GroupName;
                foreach (var item in g.Items)
                {
                    info.Add(item);
                }

                groups.Add(info);
            }

            groupedItems = new CollectionViewSource();
            groupedItems.IsSourceGrouped = true;
            groupedItems.Source = groups;

            return groupedItems;
        }

        public class GroupInfoCollection<T> : ObservableCollection<T>
        {
            public object Key { get; set; }

            public new IEnumerator<T> GetEnumerator()
            {
                return (IEnumerator<T>)base.GetEnumerator();
            }
        }

        // Filtering implementation using LINQ
        public enum FilterOptions
        {
            All = -1,
            Rank_Low = 0,
            Rank_High = 1,
            Height_Low = 2,
            Height_High = 3
        }

        public ObservableCollection<DataGridDataItem> FilterData(FilterOptions filterBy)
        {
            switch (filterBy)
            {
                case FilterOptions.All:
                    return new ObservableCollection<DataGridDataItem>(_items);

                case FilterOptions.Rank_Low:
                    return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                      where item.Rank < 50
                                                                      select item);

                case FilterOptions.Rank_High:
                    return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                      where item.Rank > 50
                                                                      select item);

                case FilterOptions.Height_High:
                    return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                      where item.Height_m > 8000
                                                                      select item);

                case FilterOptions.Height_Low:
                    return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                      where item.Height_m < 8000
                                                                      select item);
            }

            return _items;
        }
    }
}