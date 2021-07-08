// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.WinUI.UI.Controls.DataGridInternals;
using CommunityToolkit.WinUI.UI.Utilities;
using CommunityToolkit.WinUI.Utilities;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Represents a <see cref="DataGrid"/> column that hosts textual content in its cells. In edit mode data can be changed to a value from a collection hosted in a ComboBox.
    /// </summary>
    [StyleTypedProperty(Property = "ElementStyle", StyleTargetType = typeof(TextBlock))]
    [StyleTypedProperty(Property = "EditingElementStyle", StyleTargetType = typeof(ComboBox))]
    public class DataGridComboBoxColumn : DataGridBoundColumn
    {
        private const string DATAGRIDCOMBOBOXCOLUMN_fontFamilyName = "FontFamily";
        private const string DATAGRIDCOMBOBOXCOLUMN_fontSizeName = "FontSize";
        private const string DATAGRIDCOMBOBOXCOLUMN_fontStyleName = "FontStyle";
        private const string DATAGRIDCOMBOBOXCOLUMN_fontWeightName = "FontWeight";
        private const string DATAGRIDCOMBOBOXCOLUMN_foregroundName = "Foreground";
        private const string DATAGRIDCOMBOBOXCOLUMN_itemsSourceName = "ItemsSource";
        private const string DATAGRIDCOMBOBOXCOLUMN_displayMemberPathName = "DisplayMemberPath";
        private const double DATAGRIDCOMBOBOXCOLUMN_leftMargin = 12.0;
        private const double DATAGRIDCOMBOBOXCOLUMN_rightMargin = 12.0;

        private double? _fontSize;
        private DataGrid _owningGrid;
        private FontStyle? _fontStyle;
        private FontWeight? _fontWeight;
        private Brush _foreground;
        private HashSet<object> _notifyingDataItems;

        /// <summary>
        /// Identifies the ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                DATAGRIDCOMBOBOXCOLUMN_itemsSourceName,
                typeof(IEnumerable),
                typeof(DataGridComboBoxColumn),
                new PropertyMetadata(default(IEnumerable), OnItemSourcePropertyChanged));

        /// <summary>
        /// Gets or sets a collection that is used to generate the content of the ComboBox while in editing mode.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void OnItemSourcePropertyChanged(DependencyObject comboBoxDependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var comboColumn = comboBoxDependencyObject as DataGridComboBoxColumn;
            comboColumn.NotifyPropertyChanged(DATAGRIDCOMBOBOXCOLUMN_itemsSourceName);
        }

        /// <summary>
        /// Identifies the DisplayMemberPath dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(
                DATAGRIDCOMBOBOXCOLUMN_displayMemberPathName,
                typeof(string),
                typeof(DataGridComboBoxColumn),
                new PropertyMetadata(default(string)));

        /// <summary>
        /// Gets or sets the name or path of the property that is displayed in the ComboBox.
        /// </summary>
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        /// <summary>
        /// Gets or sets the font name.
        /// </summary>
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        /// <summary>
        /// Identifies the FontFamily dependency property.
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register(
                DATAGRIDCOMBOBOXCOLUMN_fontFamilyName,
                typeof(FontFamily),
                typeof(DataGridComboBoxColumn),
                new PropertyMetadata(null, OnFontFamilyPropertyChanged));

        private static void OnFontFamilyPropertyChanged(DependencyObject comboBoxColumnDependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var comboColumn = comboBoxColumnDependencyObject as DataGridComboBoxColumn;
            comboColumn.NotifyPropertyChanged(DATAGRIDCOMBOBOXCOLUMN_fontFamilyName);
        }

        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        // Use DefaultValue here so undo in the Designer will set this to NaN
        [DefaultValue(double.NaN)]
        public double FontSize
        {
            get
            {
                return _fontSize ?? double.NaN;
            }

            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    NotifyPropertyChanged(DATAGRIDCOMBOBOXCOLUMN_fontSizeName);
                }
            }
        }

        /// <summary>
        /// Gets or sets the font style.
        /// </summary>
        public FontStyle FontStyle
        {
            get
            {
                return _fontStyle ?? FontStyle.Normal;
            }

            set
            {
                if (_fontStyle != value)
                {
                    _fontStyle = value;
                    NotifyPropertyChanged(DATAGRIDCOMBOBOXCOLUMN_fontStyleName);
                }
            }
        }

        /// <summary>
        /// Gets or sets the font weight or thickness.
        /// </summary>
        public FontWeight FontWeight
        {
            get
            {
                return _fontWeight ?? FontWeights.Normal;
            }

            set
            {
                if (!_fontWeight.HasValue || _fontWeight.Value.Weight != value.Weight)
                {
                    _fontWeight = value;
                    NotifyPropertyChanged(DATAGRIDCOMBOBOXCOLUMN_fontWeightName);
                }
            }
        }

        /// <summary>
        /// Gets or sets a brush that describes the foreground of the column cells.
        /// </summary>
        public Brush Foreground
        {
            get
            {
                return _foreground;
            }

            set
            {
                if (_foreground != value)
                {
                    _foreground = value;
                    NotifyPropertyChanged(DATAGRIDCOMBOBOXCOLUMN_foregroundName);
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="T:Microsoft.UI.Xaml.Controls.ComboBox"/> control that is bound to the column's ItemsSource collection.
        /// </summary>
        /// <param name="cell">The cell that will contain the generated element.</param>
        /// <param name="dataItem">The data item represented by the row that contains the intended cell.</param>
        /// <returns>A new <see cref="T:Microsoft.UI.Xaml.Controls.ComboBox"/> control that is bound to the column's ItemsSource collection.</returns>
        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            EnsureColumnBinding(dataItem);

            EnsureDisplayMemberPathExists();

            EnsureItemsSourceBinding();

            EnsureColumnTypeAgreement(dataItem);

            var comboBox = new ComboBox
            {
                Margin = default(Thickness),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center
            };

            if (dataItem != null)
            {
                var value = TypeHelper.GetNestedPropertyValue(dataItem, Binding.Path.Path);

                var selection = !string.IsNullOrEmpty(DisplayMemberPath)
                    ? ItemsSource?.Cast<object>().FirstOrDefault(x => TypeHelper.GetNestedPropertyValue(x, Binding.GetBindingPropertyName()).Equals(value))
                    : ItemsSource?.Cast<object>().FirstOrDefault(x => x.Equals(value));

                comboBox.SelectedItem = selection;
            }

            var itemsSourceBinding = new Binding
            {
                Source = this,
                Path = new PropertyPath(DATAGRIDCOMBOBOXCOLUMN_itemsSourceName)
            };

            var displayMemberPathBinding = new Binding
            {
                Source = this,
                Path = new PropertyPath(DATAGRIDCOMBOBOXCOLUMN_displayMemberPathName)
            };

            comboBox.SetBinding(ComboBox.ItemsSourceProperty, itemsSourceBinding);

            comboBox.SetBinding(ComboBox.DisplayMemberPathProperty, displayMemberPathBinding);

            if (DependencyProperty.UnsetValue != ReadLocalValue(DataGridComboBoxColumn.FontFamilyProperty))
            {
                comboBox.FontFamily = FontFamily;
            }

            if (_fontSize.HasValue)
            {
                comboBox.FontSize = _fontSize.Value;
            }

            if (_fontStyle.HasValue)
            {
                comboBox.FontStyle = _fontStyle.Value;
            }

            if (_fontWeight.HasValue)
            {
                comboBox.FontWeight = _fontWeight.Value;
            }

            RefreshForeground(comboBox, (cell != null & cell.OwningRow != null) ? cell.OwningRow.ComputedForeground : null);

            comboBox.SelectionChanged += (sender, args) =>
            {
                var item = args.AddedItems.FirstOrDefault();

                if (item != null)
                {
                    var newValue = !string.IsNullOrEmpty(DisplayMemberPath)
                        ? item.GetType().GetProperty(Binding.GetBindingPropertyName())?.GetValue(item)
                        : item;

                    TypeHelper.SetNestedPropertyValue(ref dataItem, newValue, Binding.Path.Path);
                }
            };

            return comboBox;
        }

        /// <summary>
        /// Gets a read-only <see cref="T:Microsoft.UI.Xaml.Controls.TextBlock"/> element that is bound to the column's <see cref="P:CommunityToolkit.WinUI.UI.Controls.DataGridBoundColumn.Binding"/> property value.
        /// </summary>
        /// <param name="cell">The cell that will contain the generated element.</param>
        /// <param name="dataItem">The data item represented by the row that contains the intended cell.</param>
        /// <returns>A new, read-only <see cref="T:Microsoft.UI.Xaml.Controls.TextBlock"/> element that is bound to the column's <see cref="P:CommunityToolkit.WinUI.UI.Controls.DataGridBoundColumn.Binding"/> property value.</returns>
        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            EnsureColumnBinding(dataItem);
            EnsureColumnTypeAgreement(dataItem);
            EnsureDisplayMemberPathExists();
            EnsureItemsSourceBinding();

            var textBlockElement = new TextBlock
            {
                Margin = new Thickness(DATAGRIDCOMBOBOXCOLUMN_leftMargin, 0.0, DATAGRIDCOMBOBOXCOLUMN_rightMargin, 0.0),
                VerticalAlignment = VerticalAlignment.Center
            };

            if (DependencyProperty.UnsetValue != ReadLocalValue(DataGridComboBoxColumn.FontFamilyProperty))
            {
                textBlockElement.FontFamily = FontFamily;
            }

            if (_fontSize.HasValue)
            {
                textBlockElement.FontSize = _fontSize.Value;
            }

            if (_fontStyle.HasValue)
            {
                textBlockElement.FontStyle = _fontStyle.Value;
            }

            if (_fontWeight.HasValue)
            {
                textBlockElement.FontWeight = _fontWeight.Value;
            }

            RefreshForeground(textBlockElement, (cell != null & cell.OwningRow != null) ? cell.OwningRow.ComputedForeground : null);

            if (Binding != null && EnsureOwningGrid())
            {
                if (string.IsNullOrEmpty(DisplayMemberPath))
                {
                    textBlockElement.SetBinding(TextBlock.TextProperty, Binding);
                }
                else
                {
                    textBlockElement.Text = GetDisplayValue(dataItem);

                    HookDataItemPropertyChanged(dataItem);
                }
            }

            return textBlockElement;
        }

        /// <summary>
        /// Causes the column cell being edited to revert to the specified value.
        /// </summary>
        /// <param name="editingElement">The element that the column displays for a cell in editing mode.</param>
        /// <param name="uneditedValue">The previous, unedited value in the cell being edited.</param>
        protected override void CancelCellEdit(FrameworkElement editingElement, object uneditedValue)
        {
            var comboBox = editingElement as ComboBox;

            if (comboBox != null)
            {
                if (uneditedValue != null)
                {
                    var property = uneditedValue.GetType().GetNestedProperty(Binding.GetBindingPropertyName());

                    if (property == null)
                    {
                        comboBox.SelectedItem = uneditedValue;
                    }
                    else
                    {
                        var value = TypeHelper.GetNestedPropertyValue(uneditedValue, Binding.GetBindingPropertyName());
                        var selection = ItemsSource?.Cast<object>().FirstOrDefault(x => TypeHelper.GetNestedPropertyValue(x, Binding.GetBindingPropertyName()).Equals(value));

                        comboBox.SelectedItem = selection;
                    }
                }
                else
                {
                    comboBox.SelectedItem = null;
                }
            }
        }

        /// <summary>
        /// Called when the cell in the column enters editing mode.
        /// </summary>
        /// <param name="editingElement">The element that the column displays for a cell in editing mode.</param>
        /// <param name="editingEventArgs">Information about the user gesture that is causing a cell to enter editing mode.</param>
        /// <returns>The unedited value. </returns>
        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            return (editingElement as ComboBox)?.SelectedItem;
        }

        /// <summary>
        /// Called by the DataGrid control when this column asks for its elements to be updated, because a property changed.
        /// </summary>
        protected internal override void RefreshCellContent(FrameworkElement element, Brush computedRowForeground, string propertyName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            var comboBox = element as ComboBox;
            if (comboBox == null)
            {
                var textBlock = element as TextBlock;
                if (textBlock == null)
                {
                    throw DataGridError.DataGrid.ValueIsNotAnInstanceOfEitherOr(nameof(element), typeof(ComboBox), typeof(TextBlock));
                }

                if (propertyName == DATAGRIDCOMBOBOXCOLUMN_fontFamilyName)
                {
                    textBlock.FontFamily = FontFamily;
                }
                else if (propertyName == DATAGRIDCOMBOBOXCOLUMN_fontSizeName)
                {
                    SetTextFontSize(textBlock, TextBlock.FontSizeProperty);
                }
                else if (propertyName == DATAGRIDCOMBOBOXCOLUMN_fontStyleName)
                {
                    textBlock.FontStyle = FontStyle;
                }
                else if (propertyName == DATAGRIDCOMBOBOXCOLUMN_fontWeightName)
                {
                    textBlock.FontWeight = FontWeight;
                }
                else if (propertyName == DATAGRIDCOMBOBOXCOLUMN_foregroundName)
                {
                    RefreshForeground(textBlock, computedRowForeground);
                }
                else if (propertyName == DATAGRIDCOMBOBOXCOLUMN_itemsSourceName)
                {
                    OwningGrid.OnColumnBindingChanged(this);
                }
                else
                {
                    if (FontFamily != null)
                    {
                        textBlock.FontFamily = FontFamily;
                    }

                    SetTextFontSize(textBlock, TextBlock.FontSizeProperty);
                    textBlock.FontStyle = FontStyle;
                    textBlock.FontWeight = FontWeight;
                    RefreshForeground(textBlock, computedRowForeground);
                }

                return;
            }

            if (propertyName == DATAGRIDCOMBOBOXCOLUMN_fontFamilyName)
            {
                comboBox.FontFamily = FontFamily;
            }
            else if (propertyName == DATAGRIDCOMBOBOXCOLUMN_fontSizeName)
            {
                SetTextFontSize(comboBox, ComboBox.FontSizeProperty);
            }
            else if (propertyName == DATAGRIDCOMBOBOXCOLUMN_fontStyleName)
            {
                comboBox.FontStyle = FontStyle;
            }
            else if (propertyName == DATAGRIDCOMBOBOXCOLUMN_fontWeightName)
            {
                comboBox.FontWeight = FontWeight;
            }
            else if (propertyName == DATAGRIDCOMBOBOXCOLUMN_foregroundName)
            {
                RefreshForeground(comboBox, computedRowForeground);
            }
            else
            {
                if (FontFamily != null)
                {
                    comboBox.FontFamily = FontFamily;
                }

                SetTextFontSize(comboBox, ComboBox.FontSizeProperty);
                comboBox.FontStyle = FontStyle;
                comboBox.FontWeight = FontWeight;
                RefreshForeground(comboBox, computedRowForeground);
            }
        }

        /// <summary>
        /// Called when the computed foreground of a row changed.
        /// </summary>
        protected internal override void RefreshForeground(FrameworkElement element, Brush computedRowForeground)
        {
            if (element is ComboBox comboBox)
            {
                RefreshForeground(comboBox, computedRowForeground);
            }
            else if (element is TextBlock textBlock)
            {
                RefreshForeground(textBlock, computedRowForeground);
            }
        }

        private void RefreshForeground(ComboBox comboBox, Brush computedRowForeground)
        {
            if (Foreground == null)
            {
                if (computedRowForeground != null)
                {
                    comboBox.Foreground = computedRowForeground;
                }
            }
            else
            {
                comboBox.Foreground = Foreground;
            }
        }

        private void RefreshForeground(TextBlock textBlock, Brush computedRowForeground)
        {
            if (Foreground == null)
            {
                if (computedRowForeground != null)
                {
                    textBlock.Foreground = computedRowForeground;
                }
            }
            else
            {
                textBlock.Foreground = Foreground;
            }
        }

        private void SetTextFontSize(DependencyObject textElement, DependencyProperty fontSizeProperty)
        {
            double newFontSize = FontSize;
            if (double.IsNaN(newFontSize))
            {
                textElement.ClearValue(fontSizeProperty);
            }
            else
            {
                textElement.SetValue(fontSizeProperty, newFontSize);
            }
        }

        private bool EnsureOwningGrid()
        {
            if (OwningGrid != null)
            {
                if (OwningGrid != _owningGrid)
                {
                    _owningGrid = OwningGrid;
                    _owningGrid.Columns.CollectionChanged += new NotifyCollectionChangedEventHandler(Columns_CollectionChanged);
                    _owningGrid.LoadingRow += OwningGrid_LoadingRow;
                    _owningGrid.UnloadingRow += OwningGrid_UnloadingRow;
                    _owningGrid.CellEditEnded += OwningGrid_CellEditEnded;
                }

                return true;
            }

            return false;
        }

        private void OwningGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            HookDataItemPropertyChanged(e.Row.DataContext);
            SetDisplayMemberPathValue(e.Row);
        }

        private void OwningGrid_UnloadingRow(object sender, DataGridRowEventArgs e)
        {
            UnhookDataItemPropertyChanged(e.Row.DataContext);
        }

        private void OwningGrid_CellEditEnded(object sender, DataGridCellEditEndedEventArgs e)
        {
            SetDisplayMemberPathValue(e.Row);
        }

        private void Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (OwningGrid == null && _owningGrid != null)
            {
                _notifyingDataItems?.Clear();
                _owningGrid.Columns.CollectionChanged -= new NotifyCollectionChangedEventHandler(Columns_CollectionChanged);
                _owningGrid.LoadingRow -= OwningGrid_LoadingRow;
                _owningGrid.UnloadingRow -= this.OwningGrid_UnloadingRow;
                _owningGrid.CellEditEnded -= OwningGrid_CellEditEnded;
                _owningGrid = null;
            }
        }

        private void SetDisplayMemberPathValue(DataGridRow row)
        {
            if (OwningGrid != null && !string.IsNullOrEmpty(DisplayMemberPath))
            {
                var textBlock = GetCellContent(row) as TextBlock;
                if (textBlock != null)
                {
                    var displayValue = GetDisplayValue(row.DataContext);

                    textBlock.Text = displayValue;
                }
            }
        }

        private string GetDisplayValue(object dataItem)
        {
            if (Binding?.Path != null && dataItem != null)
            {
                var value = TypeHelper.GetNestedPropertyValue(dataItem, Binding.Path.Path);

                var item = ItemsSource?.Cast<object>().FirstOrDefault(x => TypeHelper.GetNestedPropertyValue(x, Binding.GetBindingPropertyName()).Equals(value));

                var displayValue = item?.GetType().GetProperty(DisplayMemberPath).GetValue(item) ?? string.Empty;

                return displayValue as string ?? displayValue.ToString();
            }

            return string.Empty;
        }

        private void EnsureColumnBinding(object dataItem)
        {
            if (Binding?.Path == null)
            {
                if (!string.IsNullOrEmpty(Header as string))
                {
                    throw DataGridError.DataGridComboBoxColumn.UnsetBinding(Header as string);
                }

                throw DataGridError.DataGridComboBoxColumn.UnsetBinding(GetType());
            }

            var property = dataItem?.GetType().GetNestedProperty(Binding?.Path?.Path);

            if (property == null && dataItem != null)
            {
                throw DataGridError.DataGridComboBoxColumn.UnknownBindingPath(Binding, dataItem?.GetType());
            }
        }

        private void EnsureColumnTypeAgreement(object dataItem)
        {
            if (string.IsNullOrEmpty(DisplayMemberPath))
            {
                var itemsSourceType = ItemsSource?.GetType().GetEnumerableItemType();
                var dataItemType = dataItem?.GetType().GetNestedPropertyType(Binding?.Path?.Path);

                if (dataItemType != null && itemsSourceType != null && itemsSourceType != dataItemType)
                {
                    throw DataGridError.DataGridComboBoxColumn.BindingTypeMismatch(dataItemType, itemsSourceType);
                }
            }
        }

        private void EnsureDisplayMemberPathExists()
        {
            if (!string.IsNullOrEmpty(DisplayMemberPath))
            {
                var type = ItemsSource?.GetItemType();

                if (ItemsSource != null && !type.GetProperties().Any(x => x.Name.Equals(DisplayMemberPath)))
                {
                    throw DataGridError.DataGridComboBoxColumn.UnknownDisplayMemberPath(DisplayMemberPath, type);
                }
            }
        }

        private void EnsureItemsSourceBinding()
        {
            if (!string.IsNullOrEmpty(DisplayMemberPath) && ItemsSource != null)
            {
                var item = ItemsSource.Cast<object>().FirstOrDefault();

                if (item != null && !item.GetType().GetProperties().Any(y => y.Name.Equals(Binding.GetBindingPropertyName())))
                {
                    throw DataGridError.DataGridComboBoxColumn.UnknownItemsSourcePath(Binding);
                }
            }
        }

        private void HookDataItemPropertyChanged(object dataItem)
        {
            if (Binding.Mode == BindingMode.OneTime)
            {
                return;
            }

            var notifyingDataItem = dataItem as INotifyPropertyChanged;

            if (notifyingDataItem == null)
            {
                return;
            }

            if (_notifyingDataItems == null)
            {
                _notifyingDataItems = new HashSet<object>();
            }

            if (!_notifyingDataItems.Contains(dataItem))
            {
                notifyingDataItem.PropertyChanged += DataItem_PropertyChanged;
                _notifyingDataItems.Add(dataItem);
            }
        }

        private void UnhookDataItemPropertyChanged(object dataItem)
        {
            if (_notifyingDataItems == null)
            {
                return;
            }

            var notifyingDataItem = dataItem as INotifyPropertyChanged;

            if (notifyingDataItem == null)
            {
                return;
            }

            if (_notifyingDataItems.Contains(dataItem))
            {
                notifyingDataItem.PropertyChanged -= DataItem_PropertyChanged;
                _notifyingDataItems.Remove(dataItem);
            }
        }

        private void DataItem_PropertyChanged(object dataItem, PropertyChangedEventArgs e)
        {
            if (this.OwningGrid != null && Binding?.Path != null && this.Binding.Path.Path == e.PropertyName)
            {
                var dataGridRow = OwningGrid.GetRowFromItem(dataItem);

                if (dataGridRow != null && this.GetCellContent(dataGridRow) is TextBlock textBlockElement)
                {
                    textBlockElement.Text = GetDisplayValue(dataItem);
                }
            }
        }
    }
}