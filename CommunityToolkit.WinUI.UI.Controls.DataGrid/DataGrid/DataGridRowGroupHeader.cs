// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Globalization;
using CommunityToolkit.WinUI.UI.Automation.Peers;
using CommunityToolkit.WinUI.UI.Controls.DataGridInternals;
using CommunityToolkit.WinUI.UI.Controls.Primitives;
using CommunityToolkit.WinUI.UI.Controls.Utilities;
using CommunityToolkit.WinUI.UI.Utilities;
using CommunityToolkit.WinUI.Utilities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;

using DiagnosticsDebug = System.Diagnostics.Debug;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Represents the header of a <see cref="DataGrid"/> row group.
    /// </summary>
    [TemplatePart(Name = DataGridRow.DATAGRIDROW_elementRoot, Type = typeof(Panel))]
    [TemplatePart(Name = DataGridRow.DATAGRIDROW_elementRowHeader, Type = typeof(DataGridRowHeader))]
    [TemplatePart(Name = DATAGRIDROWGROUPHEADER_bottomGridLine, Type = typeof(Rectangle))]
    [TemplatePart(Name = DATAGRIDROWGROUPHEADER_expanderButton, Type = typeof(ToggleButton))]
    [TemplatePart(Name = DATAGRIDROWGROUPHEADER_indentSpacer, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = DATAGRIDROWGROUPHEADER_itemCountElement, Type = typeof(TextBlock))]
    [TemplatePart(Name = DATAGRIDROWGROUPHEADER_propertyNameElement, Type = typeof(TextBlock))]
    [TemplatePart(Name = DATAGRIDROWGROUPHEADER_propertyValueElement, Type = typeof(TextBlock))]
    [StyleTypedProperty(Property = "HeaderStyle", StyleTargetType = typeof(DataGridRowHeader))]
    public class DataGridRowGroupHeader : Control
    {
        private const string DATAGRIDROWGROUPHEADER_bottomGridLine = "BottomGridLine";
        private const string DATAGRIDROWGROUPHEADER_expanderButton = "ExpanderButton";
        private const string DATAGRIDROWGROUPHEADER_indentSpacer = "IndentSpacer";
        private const string DATAGRIDROWGROUPHEADER_itemCountElement = "ItemCountElement";
        private const string DATAGRIDROWGROUPHEADER_propertyNameElement = "PropertyNameElement";
        private const string DATAGRIDROWGROUPHEADER_propertyValueElement = "PropertyValueElement";

        private bool _areIsCheckedHandlersSuspended;
        private Rectangle _bottomGridLine;
        private ToggleButton _expanderButton;
        private FrameworkElement _indentSpacer;
        private TextBlock _itemCountElement;
        private TextBlock _propertyNameElement;
        private TextBlock _propertyValueElement;
        private Panel _rootElement;
        private double _totalIndent;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridRowGroupHeader"/> class.
        /// </summary>
        public DataGridRowGroupHeader()
        {
            DefaultStyleKey = typeof(DataGridRowGroupHeader);

            this.AddHandler(UIElement.TappedEvent, new TappedEventHandler(DataGridRowGroupHeader_Tapped), true /*handledEventsToo*/);
            this.AddHandler(UIElement.DoubleTappedEvent, new DoubleTappedEventHandler(DataGridRowGroupHeader_DoubleTapped), true /*handledEventsToo*/);

            this.PointerCanceled += new PointerEventHandler(DataGridRowGroupHeader_PointerCanceled);
            this.PointerEntered += new PointerEventHandler(DataGridRowGroupHeader_PointerEntered);
            this.PointerExited += new PointerEventHandler(DataGridRowGroupHeader_PointerExited);
            this.PointerMoved += new PointerEventHandler(DataGridRowGroupHeader_PointerMoved);
            this.PointerPressed += new PointerEventHandler(DataGridRowGroupHeader_PointerPressed);
            this.PointerReleased += new PointerEventHandler(DataGridRowGroupHeader_PointerReleased);
        }

        /// <summary>
        /// Gets or sets the style applied to the header cell of a <see cref="DataGridRowGroupHeader"/>.
        /// </summary>
        public Style HeaderStyle
        {
            get { return GetValue(HeaderStyleProperty) as Style; }
            set { SetValue(HeaderStyleProperty, value); }
        }

        /// <summary>
        /// Dependency Property for HeaderStyle
        /// </summary>
        public static readonly DependencyProperty HeaderStyleProperty =
            DependencyProperty.Register(
                "HeaderStyle",
                typeof(Style),
                typeof(DataGridRowGroupHeader),
                new PropertyMetadata(null, OnHeaderStylePropertyChanged));

        private static void OnHeaderStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridRowGroupHeader groupHeader = d as DataGridRowGroupHeader;
            if (groupHeader.HeaderElement != null)
            {
                groupHeader.HeaderElement.EnsureStyle(e.OldValue as Style);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the item count is visible.
        /// </summary>
        public Visibility ItemCountVisibility
        {
            get { return (Visibility)GetValue(ItemCountVisibilityProperty); }
            set { SetValue(ItemCountVisibilityProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for ItemCountVisibility
        /// </summary>
        public static readonly DependencyProperty ItemCountVisibilityProperty =
            DependencyProperty.Register(
                "ItemCountVisibility",
                typeof(Visibility),
                typeof(DataGridRowGroupHeader),
                new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets the nesting level of the associated group.
        /// </summary>
        public int Level
        {
            get { return (int)GetValue(LevelProperty); }
            internal set { SetValue(LevelProperty, value); }
        }

        /// <summary>
        /// Identifies the Level dependency property.
        /// </summary>
        public static readonly DependencyProperty LevelProperty =
            DependencyProperty.Register(
                "Level",
                typeof(int),
                typeof(DataGridRowGroupHeader),
                new PropertyMetadata(0));

        /// <summary>
        /// Gets or sets the name of the property that this <see cref="DataGrid"/> row is bound to.
        /// </summary>
        public string PropertyName
        {
            get { return GetValue(PropertyNameProperty) as string; }
            set { SetValue(PropertyNameProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for PropertyName
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register(
                "PropertyName",
                typeof(string),
                typeof(DataGridRowGroupHeader),
                new PropertyMetadata(null, OnPropertyNameChanged));

        private static void OnPropertyNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridRowGroupHeader groupHeader = d as DataGridRowGroupHeader;
            groupHeader.UpdateTitleElements();
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the property name is visible.
        /// </summary>
        public Visibility PropertyNameVisibility
        {
            get { return (Visibility)GetValue(PropertyNameVisibilityProperty); }
            set { SetValue(PropertyNameVisibilityProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for PropertyNameVisibility
        /// </summary>
        public static readonly DependencyProperty PropertyNameVisibilityProperty =
            DependencyProperty.Register(
                "PropertyNameVisibility",
                typeof(Visibility),
                typeof(DataGridRowGroupHeader),
                new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets the value of the property that this <see cref="DataGrid"/> row is bound to.
        /// </summary>
        public string PropertyValue
        {
            get { return GetValue(PropertyValueProperty) as string; }
            set { SetValue(PropertyValueProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for PropertyName
        /// </summary>
        public static readonly DependencyProperty PropertyValueProperty =
            DependencyProperty.Register(
                "PropertyValue",
                typeof(string),
                typeof(DataGridRowGroupHeader),
                new PropertyMetadata(null, OnPropertyValueChanged));

        private static void OnPropertyValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridRowGroupHeader groupHeader = d as DataGridRowGroupHeader;
            groupHeader.UpdateTitleElements();
        }

        /// <summary>
        /// Gets or sets a value that indicates the amount that the
        /// children of the <see cref="DataGridRowGroupHeader"/> are indented.
        /// </summary>
        public double SublevelIndent
        {
            get { return (double)GetValue(SublevelIndentProperty); }
            set { SetValue(SublevelIndentProperty, value); }
        }

        /// <summary>
        /// SublevelIndent Dependency property
        /// </summary>
        public static readonly DependencyProperty SublevelIndentProperty =
            DependencyProperty.Register(
                "SublevelIndent",
                typeof(double),
                typeof(DataGridRowGroupHeader),
                new PropertyMetadata(DataGrid.DATAGRID_defaultRowGroupSublevelIndent, OnSublevelIndentPropertyChanged));

        private static void OnSublevelIndentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridRowGroupHeader groupHeader = d as DataGridRowGroupHeader;
            double newValue = (double)e.NewValue;

            // We don't need to revert to the old value if our input is bad because we never read this property value
            if (double.IsNaN(newValue))
            {
                throw DataGridError.DataGrid.ValueCannotBeSetToNAN("SublevelIndent");
            }
            else if (double.IsInfinity(newValue))
            {
                throw DataGridError.DataGrid.ValueCannotBeSetToInfinity("SublevelIndent");
            }
            else if (newValue < 0)
            {
                throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "SublevelIndent", 0);
            }

            if (groupHeader.OwningGrid != null)
            {
                groupHeader.OwningGrid.OnSublevelIndentUpdated(groupHeader, newValue);
            }
        }

        /// <summary>
        /// Gets the ICollectionViewGroup implementation associated with this <see cref="DataGridRowGroupHeader"/>.
        /// </summary>
        public ICollectionViewGroup CollectionViewGroup
        {
            get
            {
                return this.RowGroupInfo == null ? null : this.RowGroupInfo.CollectionViewGroup;
            }
        }

        internal DataGridRowHeader HeaderCell
        {
            get
            {
                return this.HeaderElement;
            }
        }

        private DataGridRowHeader HeaderElement
        {
            get;
            set;
        }

        private bool IsCurrent
        {
            get
            {
                DiagnosticsDebug.Assert(this.OwningGrid != null, "Expected non-null OwningGrid.");
                return this.RowGroupInfo.Slot == this.OwningGrid.CurrentSlot;
            }
        }

        private bool IsPointerOver
        {
            get;
            set;
        }

        private bool IsPressed
        {
            get;
            set;
        }

        internal bool IsRecycled
        {
            get;
            set;
        }

        internal DataGrid OwningGrid
        {
            get;
            set;
        }

        internal DataGridRowGroupInfo RowGroupInfo
        {
            get;
            set;
        }

        internal double TotalIndent
        {
            set
            {
                _totalIndent = value;
                if (_indentSpacer != null)
                {
                    _indentSpacer.Width = _totalIndent;
                }
            }
        }

        internal void ApplyHeaderState(bool animate)
        {
            if (this.HeaderElement != null && this.OwningGrid.AreRowHeadersVisible)
            {
                this.HeaderElement.ApplyOwnerState(animate);
            }
        }

        internal void ApplyState(bool useTransitions)
        {
            // Common States
            if (this.IsPressed)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StatePressed, VisualStates.StatePointerOver, VisualStates.StateNormal);
            }
            else if (this.IsPointerOver)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StatePointerOver, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateNormal);
            }

            // Current States
            if (this.IsCurrent && !this.OwningGrid.ColumnHeaderHasFocus)
            {
                if (this.OwningGrid.ContainsFocus)
                {
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateCurrentWithFocus, VisualStates.StateCurrent, VisualStates.StateRegular);
                }
                else
                {
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateCurrent, VisualStates.StateRegular);
                }
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateRegular);
            }

            // Expanded States
            if (this.RowGroupInfo.CollectionViewGroup.GroupItems.Count == 0)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateEmpty);
            }
            else
            {
                if (this.RowGroupInfo.Visibility == Visibility.Visible)
                {
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateExpanded, VisualStates.StateEmpty);
                }
                else
                {
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateCollapsed, VisualStates.StateEmpty);
                }
            }
        }

        /// <summary>
        /// ArrangeOverride
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        /// <returns>The actual size that is used after the element is arranged in layout.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.OwningGrid == null)
            {
                return base.ArrangeOverride(finalSize);
            }

            Size size = base.ArrangeOverride(finalSize);
            if (_rootElement != null)
            {
                if (this.OwningGrid.AreRowGroupHeadersFrozen)
                {
                    foreach (UIElement child in _rootElement.Children)
                    {
                        child.Clip = null;
                    }
                }
                else
                {
                    double frozenLeftEdge = 0;
                    foreach (UIElement child in _rootElement.Children)
                    {
                        if (DataGridFrozenGrid.GetIsFrozen(child) && child.Visibility == Visibility.Visible)
                        {
                            TranslateTransform transform = new TranslateTransform();

                            // Automatic layout rounding doesn't apply to transforms so we need to Round this
                            transform.X = Math.Round(this.OwningGrid.HorizontalOffset);
                            child.RenderTransform = transform;

                            double childLeftEdge = child.Translate(this, new Point(child.RenderSize.Width, 0)).X - transform.X;
                            frozenLeftEdge = Math.Max(frozenLeftEdge, childLeftEdge + this.OwningGrid.HorizontalOffset);
                        }
                    }

                    // Clip the non-frozen elements so they don't overlap the frozen ones
                    foreach (UIElement child in _rootElement.Children)
                    {
                        if (!DataGridFrozenGrid.GetIsFrozen(child))
                        {
                            EnsureChildClip(child, frozenLeftEdge);
                        }
                    }
                }
            }

            return size;
        }

        internal void ClearFrozenStates()
        {
            if (_rootElement != null)
            {
                foreach (UIElement child in _rootElement.Children)
                {
                    child.RenderTransform = null;
                }
            }
        }

        private void DataGridRowGroupHeader_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.OwningGrid != null && !this.OwningGrid.HasColumnUserInteraction)
            {
                if (!e.Handled && this.OwningGrid.IsTabStop)
                {
                    bool success = this.OwningGrid.Focus(FocusState.Programmatic);
                    DiagnosticsDebug.Assert(success, "Expected successful focus change.");
                }

                e.Handled = this.OwningGrid.UpdateStateOnTapped(e, this.OwningGrid.CurrentColumnIndex, this.RowGroupInfo.Slot, false /*allowEdit*/);
            }
        }

        private void DataGridRowGroupHeader_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (this.OwningGrid != null && !this.OwningGrid.HasColumnUserInteraction && !e.Handled)
            {
                ToggleExpandCollapse(this.RowGroupInfo.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible, true);
                e.Handled = true;
            }
        }

        private void EnsureChildClip(UIElement child, double frozenLeftEdge)
        {
            double childLeftEdge = child.Translate(this, new Point(0, 0)).X;
            if (frozenLeftEdge > childLeftEdge)
            {
                double xClip = Math.Round(frozenLeftEdge - childLeftEdge);
                RectangleGeometry rg = new RectangleGeometry();
                rg.Rect = new Rect(xClip, 0, Math.Max(0, child.RenderSize.Width - xClip), child.RenderSize.Height);
                child.Clip = rg;
            }
            else
            {
                child.Clip = null;
            }
        }

        internal void EnsureExpanderButtonIsChecked()
        {
            if (_expanderButton != null &&
                this.RowGroupInfo != null &&
                this.RowGroupInfo.CollectionViewGroup != null &&
                this.RowGroupInfo.CollectionViewGroup.GroupItems != null &&
                this.RowGroupInfo.CollectionViewGroup.GroupItems.Count != 0)
            {
                SetIsCheckedNoCallBack(this.RowGroupInfo.Visibility == Visibility.Visible);
            }
        }

        internal void EnsureHeaderStyleAndVisibility(Style previousStyle)
        {
            if (this.HeaderElement != null && this.OwningGrid != null)
            {
                if (this.OwningGrid.AreRowHeadersVisible)
                {
                    this.HeaderElement.EnsureStyle(previousStyle);
                    this.HeaderElement.Visibility = Visibility.Visible;
                }
                else
                {
                    this.HeaderElement.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ExpanderButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!_areIsCheckedHandlersSuspended)
            {
                ToggleExpandCollapse(Visibility.Visible, true);
            }
        }

        private void ExpanderButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!_areIsCheckedHandlersSuspended)
            {
                ToggleExpandCollapse(Visibility.Collapsed, true);
            }
        }

        internal void LoadVisualsForDisplay()
        {
            EnsureExpanderButtonIsChecked();

            EnsureHeaderStyleAndVisibility(null);
            ApplyState(false /*useTransitions*/);
            ApplyHeaderState(false);
        }

        /// <summary>
        /// Builds the visual tree for the row group header when a new template is applied.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            _rootElement = GetTemplateChild(DataGridRow.DATAGRIDROW_elementRoot) as Panel;

            if (_expanderButton != null)
            {
                _expanderButton.Checked -= ExpanderButton_Checked;
                _expanderButton.Unchecked -= ExpanderButton_Unchecked;
            }

            _bottomGridLine = GetTemplateChild(DATAGRIDROWGROUPHEADER_bottomGridLine) as Rectangle;

            _expanderButton = GetTemplateChild(DATAGRIDROWGROUPHEADER_expanderButton) as ToggleButton;
            if (_expanderButton != null)
            {
                EnsureExpanderButtonIsChecked();
                _expanderButton.Checked += new RoutedEventHandler(ExpanderButton_Checked);
                _expanderButton.Unchecked += new RoutedEventHandler(ExpanderButton_Unchecked);
            }

            this.HeaderElement = GetTemplateChild(DataGridRow.DATAGRIDROW_elementRowHeader) as DataGridRowHeader;
            if (this.HeaderElement != null)
            {
                this.HeaderElement.Owner = this;
                EnsureHeaderStyleAndVisibility(null);
            }

            _indentSpacer = GetTemplateChild(DATAGRIDROWGROUPHEADER_indentSpacer) as FrameworkElement;
            if (_indentSpacer != null)
            {
                _indentSpacer.Width = _totalIndent;
            }

            _itemCountElement = GetTemplateChild(DATAGRIDROWGROUPHEADER_itemCountElement) as TextBlock;
            _propertyNameElement = GetTemplateChild(DATAGRIDROWGROUPHEADER_propertyNameElement) as TextBlock;
            _propertyValueElement = GetTemplateChild(DATAGRIDROWGROUPHEADER_propertyValueElement) as TextBlock;
            UpdateTitleElements();
            EnsureGridLine();
        }

        /// <summary>
        /// Creates AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
        /// </summary>
        /// <returns>An automation peer for this <see cref="DataGridRowGroupHeader"/>.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DataGridRowGroupHeaderAutomationPeer(this);
        }

        private void SetIsCheckedNoCallBack(bool value)
        {
            if (_expanderButton != null && _expanderButton.IsChecked != value)
            {
                _areIsCheckedHandlersSuspended = true;
                try
                {
                    _expanderButton.IsChecked = value;
                }
                finally
                {
                    _areIsCheckedHandlersSuspended = false;
                }
            }
        }

        internal void ToggleExpandCollapse(Visibility newVisibility, bool setCurrent)
        {
            if (this.RowGroupInfo.CollectionViewGroup.GroupItems.Count != 0)
            {
                if (this.OwningGrid == null)
                {
                    // Do these even if the OwningGrid is null in case it could improve the Designer experience for a standalone DataGridRowGroupHeader
                    this.RowGroupInfo.Visibility = newVisibility;
                }
                else
                {
                    this.OwningGrid.OnRowGroupHeaderToggled(this, newVisibility, setCurrent);
                }

                EnsureExpanderButtonIsChecked();
                ApplyState(true /*useTransitions*/);
            }
        }

        internal void UpdateTitleElements()
        {
            string propertyName = this.PropertyName;
            bool hasPropertyValue = _propertyValueElement != null && !string.IsNullOrEmpty(this.PropertyValue);

            if (_propertyNameElement != null)
            {
                if (!string.IsNullOrWhiteSpace(propertyName) && this.OwningGrid.DataConnection.DataType != null)
                {
                    string displayName = this.OwningGrid.DataConnection.DataType.GetDisplayName(propertyName);
                    if (!string.IsNullOrWhiteSpace(displayName))
                    {
                        propertyName = displayName;
                    }
                }

                if (string.IsNullOrEmpty(propertyName))
                {
                    propertyName = this.OwningGrid.RowGroupHeaderPropertyNameAlternative;
                }

                if (!string.IsNullOrEmpty(propertyName) && hasPropertyValue)
                {
                    propertyName = string.Format(CultureInfo.CurrentCulture, Properties.Resources.DataGridRowGroupHeader_PropertyName, propertyName);
                }

                if (!string.IsNullOrEmpty(propertyName))
                {
                    _propertyNameElement.Text = propertyName;
                }
            }

            if (hasPropertyValue)
            {
                _propertyValueElement.Text = this.PropertyValue;
            }

            if (_itemCountElement != null && this.RowGroupInfo != null && this.RowGroupInfo.CollectionViewGroup != null)
            {
                _itemCountElement.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    this.RowGroupInfo.CollectionViewGroup.GroupItems.Count == 1 ? Properties.Resources.DataGridRowGroupHeader_ItemCountSingular : Properties.Resources.DataGridRowGroupHeader_ItemCountPlural,
                    this.RowGroupInfo.CollectionViewGroup.GroupItems.Count);
            }
        }

        private void DataGridRowGroupHeader_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            UpdateIsPointerOver(false);
            UpdateIsPressed(false);
        }

        private void DataGridRowGroupHeader_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            UpdateIsPointerOver(true);
        }

        private void DataGridRowGroupHeader_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            UpdateIsPointerOver(false);
        }

        private void DataGridRowGroupHeader_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            UpdateIsPointerOver(true);
        }

        private void DataGridRowGroupHeader_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            UpdateIsPressed(true);
        }

        private void DataGridRowGroupHeader_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            UpdateIsPressed(false);
        }

        internal void EnsureGridLine()
        {
            if (this.OwningGrid != null && _bottomGridLine != null)
            {
                Visibility newVisibility = this.OwningGrid.GridLinesVisibility == DataGridGridLinesVisibility.Horizontal || this.OwningGrid.GridLinesVisibility == DataGridGridLinesVisibility.All
                    ? Visibility.Visible : Visibility.Collapsed;

                if (newVisibility != _bottomGridLine.Visibility)
                {
                    _bottomGridLine.Visibility = newVisibility;
                }

                _bottomGridLine.Fill = this.OwningGrid.HorizontalGridLinesBrush;
            }
        }

        private void UpdateIsPointerOver(bool isPointerOver)
        {
            if (!this.IsEnabled || isPointerOver == this.IsPointerOver)
            {
                return;
            }

            this.IsPointerOver = isPointerOver;
            ApplyState(true /*useTransitions*/);
        }

        private void UpdateIsPressed(bool isPressed)
        {
            if (!this.IsEnabled || isPressed == this.IsPressed)
            {
                return;
            }

            this.IsPressed = isPressed;
            ApplyState(true /*useTransitions*/);
        }
    }
}