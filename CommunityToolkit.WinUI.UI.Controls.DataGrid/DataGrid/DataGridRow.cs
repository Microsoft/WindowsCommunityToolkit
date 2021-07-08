// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Reflection;
using CommunityToolkit.WinUI.UI.Automation.Peers;
using CommunityToolkit.WinUI.UI.Controls.DataGridInternals;
using CommunityToolkit.WinUI.UI.Controls.Primitives;
using CommunityToolkit.WinUI.UI.Controls.Utilities;
using CommunityToolkit.WinUI.UI.Utilities;
using CommunityToolkit.WinUI.Utilities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.Devices.Input;
using Windows.Foundation;

using DiagnosticsDebug = System.Diagnostics.Debug;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Represents a <see cref="DataGrid"/> row.
    /// </summary>
    [TemplatePart(Name = DATAGRIDROW_elementBottomGridLine, Type = typeof(Rectangle))]
    [TemplatePart(Name = DATAGRIDROW_elementCells, Type = typeof(DataGridCellsPresenter))]
    [TemplatePart(Name = DATAGRIDROW_elementDetails, Type = typeof(DataGridDetailsPresenter))]
    [TemplatePart(Name = DATAGRIDROW_elementRoot, Type = typeof(Panel))]
    [TemplatePart(Name = DATAGRIDROW_elementRowHeader, Type = typeof(DataGridRowHeader))]

    [TemplateVisualState(Name = DATAGRIDROW_stateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = DATAGRIDROW_stateAlternate, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = DATAGRIDROW_stateNormalEditing, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = DATAGRIDROW_stateNormalEditingFocused, GroupName = VisualStates.GroupCommon)]

    [TemplateVisualState(Name = DATAGRIDROW_stateSelected, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = DATAGRIDROW_stateSelectedFocused, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = DATAGRIDROW_statePointerOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = DATAGRIDROW_statePointerOverEditing, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = DATAGRIDROW_statePointerOverEditingFocused, GroupName = VisualStates.GroupCommon)]

    [TemplateVisualState(Name = DATAGRIDROW_statePointerOverSelected, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = DATAGRIDROW_statePointerOverSelectedFocused, GroupName = VisualStates.GroupCommon)]

    [TemplateVisualState(Name = VisualStates.StateInvalid, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateValid, GroupName = VisualStates.GroupValidation)]
    [StyleTypedProperty(Property = "HeaderStyle", StyleTargetType = typeof(DataGridRowHeader))]
    public partial class DataGridRow : Control
    {
        private const byte DATAGRIDROW_defaultMinHeight = 0;
        internal const int DATAGRIDROW_maximumHeight = 65536;
        internal const double DATAGRIDROW_minimumHeight = 0;

        private const string DATAGRIDROW_elementBottomGridLine = "BottomGridLine";
        private const string DATAGRIDROW_elementCells = "CellsPresenter";
        private const string DATAGRIDROW_elementDetails = "DetailsPresenter";
        internal const string DATAGRIDROW_elementRoot = "RowRoot";
        internal const string DATAGRIDROW_elementRowHeader = "RowHeader";

        private const string DATAGRIDROW_stateAlternate = "NormalAlternatingRow";
        private const string DATAGRIDROW_statePointerOver = "PointerOver";
        private const string DATAGRIDROW_statePointerOverEditing = "PointerOverUnfocusedEditing";
        private const string DATAGRIDROW_statePointerOverEditingFocused = "PointerOverEditing";
        private const string DATAGRIDROW_statePointerOverSelected = "PointerOverUnfocusedSelected";
        private const string DATAGRIDROW_statePointerOverSelectedFocused = "PointerOverSelected";
        private const string DATAGRIDROW_stateNormal = "Normal";
        private const string DATAGRIDROW_stateNormalEditing = "UnfocusedEditing";
        private const string DATAGRIDROW_stateNormalEditingFocused = "NormalEditing";
        private const string DATAGRIDROW_stateSelected = "UnfocusedSelected";
        private const string DATAGRIDROW_stateSelectedFocused = "NormalSelected";

        private const byte DATAGRIDROW_statePointerOverCode = 0;
        private const byte DATAGRIDROW_statePointerOverEditingCode = 1;
        private const byte DATAGRIDROW_statePointerOverEditingFocusedCode = 2;
        private const byte DATAGRIDROW_statePointerOverSelectedCode = 3;
        private const byte DATAGRIDROW_statePointerOverSelectedFocusedCode = 4;
        private const byte DATAGRIDROW_stateNormalCode = 5;
        private const byte DATAGRIDROW_stateNormalEditingCode = 6;
        private const byte DATAGRIDROW_stateNormalEditingFocusedCode = 7;
        private const byte DATAGRIDROW_stateSelectedCode = 8;
        private const byte DATAGRIDROW_stateSelectedFocusedCode = 9;
        private const byte DATAGRIDROW_stateNullCode = 255;

        // Static arrays to handle state transitions:
        private static byte[] _idealStateMapping = new byte[]
        {
            DATAGRIDROW_stateNormalCode,
            DATAGRIDROW_stateNormalCode,
            DATAGRIDROW_statePointerOverCode,
            DATAGRIDROW_statePointerOverCode,
            DATAGRIDROW_stateNullCode,
            DATAGRIDROW_stateNullCode,
            DATAGRIDROW_stateNullCode,
            DATAGRIDROW_stateNullCode,
            DATAGRIDROW_stateSelectedCode,
            DATAGRIDROW_stateSelectedFocusedCode,
            DATAGRIDROW_statePointerOverSelectedCode,
            DATAGRIDROW_statePointerOverSelectedFocusedCode,
            DATAGRIDROW_stateNormalEditingCode,
            DATAGRIDROW_stateNormalEditingFocusedCode,
            DATAGRIDROW_statePointerOverEditingCode,
            DATAGRIDROW_statePointerOverEditingFocusedCode
        };

        private static byte[] _fallbackStateMapping = new byte[]
        {
            DATAGRIDROW_stateNormalCode, // DATAGRIDROW_statePointerOverCode's fallback
            DATAGRIDROW_statePointerOverEditingFocusedCode, // DATAGRIDROW_statePointerOverEditingCode's fallback
            DATAGRIDROW_stateNormalEditingFocusedCode, // DATAGRIDROW_statePointerOverEditingFocusedCode's fallback
            DATAGRIDROW_statePointerOverSelectedFocusedCode, // DATAGRIDROW_statePointerOverSelectedCode's fallback
            DATAGRIDROW_stateSelectedFocusedCode, // DATAGRIDROW_statePointerOverSelectedFocusedCode's fallback
            DATAGRIDROW_stateNullCode, // DATAGRIDROW_stateNormalCode's fallback
            DATAGRIDROW_stateNormalEditingFocusedCode, // DATAGRIDROW_stateNormalEditingCode's fallback
            DATAGRIDROW_stateSelectedFocusedCode, // DATAGRIDROW_stateNormalEditingFocusedCode's fallback
            DATAGRIDROW_stateSelectedFocusedCode, // DATAGRIDROW_stateSelectedCode's fallback
            DATAGRIDROW_stateNormalCode // DATAGRIDROW_stateSelectedFocusedCode's fallback
        };

        private static string[] _stateNames = new string[]
        {
            DATAGRIDROW_statePointerOver,
            DATAGRIDROW_statePointerOverEditing,
            DATAGRIDROW_statePointerOverEditingFocused,
            DATAGRIDROW_statePointerOverSelected,
            DATAGRIDROW_statePointerOverSelectedFocused,
            DATAGRIDROW_stateNormal,
            DATAGRIDROW_stateNormalEditing,
            DATAGRIDROW_stateNormalEditingFocused,
            DATAGRIDROW_stateSelected,
            DATAGRIDROW_stateSelectedFocused
        };

        // Locally cache whether or not details are visible so we don't run redundant storyboards
        // The Details Template that is actually applied to the Row
        private DataTemplate _appliedDetailsTemplate;
        private Visibility? _appliedDetailsVisibility;
        private Rectangle _bottomGridLine;
        private DataGridCellsPresenter _cellsElement;

        // In the case where Details scales vertically when it's arranged at a different width, we
        // get the wrong height measurement so we need to check it again after arrange
        private bool _checkDetailsContentHeight;

        private Brush _computedForeground;

        // Optimal height of the details based on the Element created by the DataTemplate
        private double _detailsDesiredHeight;
        private bool _detailsLoaded;
        private bool _detailsVisibilityNotificationPending;
        private FrameworkElement _detailsContent;
        private DataGridDetailsPresenter _detailsElement;
        private DataGridCell _fillerCell;
        private DataGridRowHeader _headerElement;
        private double _lastHorizontalOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridRow"/> class.
        /// </summary>
        public DataGridRow()
        {
            this.MinHeight = DATAGRIDROW_defaultMinHeight;
            this.IsTapEnabled = true;
            this.Index = -1;
            this.IsValid = true;
            this.Slot = -1;
            _detailsDesiredHeight = double.NaN;
            _detailsLoaded = false;
            _appliedDetailsVisibility = Visibility.Collapsed;
            _computedForeground = this.Foreground;
            this.Cells = new DataGridCellCollection(this);
            this.Cells.CellAdded += new EventHandler<DataGridCellEventArgs>(DataGridCellCollection_CellAdded);
            this.Cells.CellRemoved += new EventHandler<DataGridCellEventArgs>(DataGridCellCollection_CellRemoved);

            this.AddHandler(UIElement.TappedEvent, new TappedEventHandler(DataGridRow_Tapped), true /*handledEventsToo*/);

            this.PointerCanceled += new PointerEventHandler(DataGridRow_PointerCanceled);
            this.PointerCaptureLost += new PointerEventHandler(DataGridRow_PointerCaptureLost);
            this.PointerPressed += new PointerEventHandler(DataGridRow_PointerPressed);
            this.PointerReleased += new PointerEventHandler(DataGridRow_PointerReleased);
            this.PointerEntered += new PointerEventHandler(DataGridRow_PointerEntered);
            this.PointerExited += new PointerEventHandler(DataGridRow_PointerExited);
            this.PointerMoved += new PointerEventHandler(DataGridRow_PointerMoved);

            RegisterPropertyChangedCallback(ForegroundProperty, OnDependencyPropertyChanged);

            DefaultStyleKey = typeof(DataGridRow);
        }

        /// <summary>
        /// Gets or sets the template that is used to display the details section of the row.
        /// </summary>
        public DataTemplate DetailsTemplate
        {
            get { return GetValue(DetailsTemplateProperty) as DataTemplate; }
            set { SetValue(DetailsTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the DetailsTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty DetailsTemplateProperty =
            DependencyProperty.Register(
                "DetailsTemplate",
                typeof(DataTemplate),
                typeof(DataGridRow),
                new PropertyMetadata(null, OnDetailsTemplatePropertyChanged));

        /// <summary>
        /// DetailsTemplateProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGridRow that changed its DetailsTemplate.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnDetailsTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridRow source = d as DataGridRow;
            DiagnosticsDebug.Assert(source != null, "The source is not an instance of DataGridRow!");

            DiagnosticsDebug.Assert(
                (e.NewValue == null) ||
                typeof(DataTemplate).IsInstanceOfType(e.NewValue),
                "The e.NewValue is not an instance of DataTemplate.");
            DataTemplate newValue = (DataTemplate)e.NewValue;
            DataTemplate oldValue = (DataTemplate)e.OldValue;

            if (!source.IsHandlerSuspended(e.Property) && source.OwningGrid != null)
            {
                Func<DataTemplate, DataTemplate> actualDetailsTemplate = template => (template != null ? template : source.OwningGrid.RowDetailsTemplate);

                // We don't always want to apply the new Template because they might have set the same one
                // we inherited from the DataGrid
                if (actualDetailsTemplate(newValue) != actualDetailsTemplate(oldValue))
                {
                    source.ApplyDetailsTemplate(false /*initializeDetailsPreferredHeight*/);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates when the details section of the row is displayed.
        /// </summary>
        public Visibility DetailsVisibility
        {
            get { return (Visibility)GetValue(DetailsVisibilityProperty); }
            set { SetValue(DetailsVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the DetailsTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty DetailsVisibilityProperty =
            DependencyProperty.Register(
                "DetailsVisibility",
                typeof(Visibility),
                typeof(DataGridRow),
                new PropertyMetadata(Visibility.Collapsed, OnDetailsVisibilityPropertyChanged));

        /// <summary>
        /// DetailsVisibilityProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGridRow that changed its DetailsTemplate.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnDetailsVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridRow row = d as DataGridRow;

            if (!row.IsHandlerSuspended(e.Property))
            {
                if (row.OwningGrid == null)
                {
                    throw DataGridError.DataGrid.NoOwningGrid(row.GetType());
                }

                if (row.Index == -1)
                {
                    throw DataGridError.DataGridRow.InvalidRowIndexCannotCompleteOperation();
                }

                Visibility newValue = (Visibility)e.NewValue;
                row.OwningGrid.OnRowDetailsVisibilityPropertyChanged(row.Index, newValue);
                row.SetDetailsVisibilityInternal(
                    newValue,
                    true /*raiseNotification*/);
            }
        }

        /// <summary>
        /// Gets or sets the row header.
        /// </summary>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the Header dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                "Header",
                typeof(object),
                typeof(DataGridRow),
                new PropertyMetadata(null, OnHeaderPropertyChanged));

        /// <summary>
        /// HeaderProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGridRow that changed its Header.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridRow row = d as DataGridRow;
            if (row._headerElement != null)
            {
                row._headerElement.Content = e.NewValue;
            }
        }

        /// <summary>
        /// Gets or sets the style that is used when rendering the row header.
        /// </summary>
        public Style HeaderStyle
        {
            get { return GetValue(HeaderStyleProperty) as Style; }
            set { SetValue(HeaderStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="CommunityToolkit.WinUI.UI.Controls.DataGridRow.HeaderStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderStyleProperty =
            DependencyProperty.Register(
                "HeaderStyle",
                typeof(Style),
                typeof(DataGridRow),
                new PropertyMetadata(null, OnHeaderStylePropertyChanged));

        private static void OnHeaderStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridRow row = d as DataGridRow;
            if (row != null && row._headerElement != null)
            {
                row._headerElement.EnsureStyle(e.OldValue as Style);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the data in a row is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return (bool)GetValue(IsValidProperty);
            }

            internal set
            {
                this.SetValueNoCallback(IsValidProperty, value);
            }
        }

        /// <summary>
        /// Identifies the IsValid dependency property.
        /// </summary>
        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register(
                "IsValid",
                typeof(bool),
                typeof(DataGridRow),
                new PropertyMetadata(true, OnIsValidPropertyChanged));

        /// <summary>
        /// IsValidProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGridRow that changed its IsValid.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnIsValidPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridRow dataGridRow = d as DataGridRow;
            if (!dataGridRow.IsHandlerSuspended(e.Property))
            {
                dataGridRow.SetValueNoCallback(DataGridRow.IsValidProperty, e.OldValue);
                throw DataGridError.DataGrid.UnderlyingPropertyIsReadOnly("IsValid");
            }
        }

        internal double ActualBottomGridLineHeight
        {
            get
            {
                if (_bottomGridLine != null && this.OwningGrid != null && this.OwningGrid.AreRowBottomGridLinesRequired)
                {
                    // Unfortunately, _bottomGridLine has no size yet so we can't get its actual height
                    return DataGrid.HorizontalGridLinesThickness;
                }

                return 0;
            }
        }

        internal DataGridCellCollection Cells
        {
            get;
            private set;
        }

        internal Brush ComputedForeground
        {
            get
            {
                return _computedForeground;
            }

            set
            {
                if (_computedForeground != value)
                {
                    _computedForeground = value;

                    if (this.Cells != null)
                    {
                        foreach (DataGridCell dataGridCell in this.Cells)
                        {
                            FrameworkElement element = dataGridCell.Content as FrameworkElement;
                            if (element != null)
                            {
                                dataGridCell.OwningColumn.RefreshForeground(element, _computedForeground);
                            }
                        }
                    }
                }
            }
        }

        internal double DetailsContentHeight
        {
            get
            {
                if (_detailsElement != null)
                {
                    return _detailsElement.ContentHeight;
                }

                return 0;
            }
        }

        internal DataGridCell FillerCell
        {
            get
            {
                DiagnosticsDebug.Assert(this.OwningGrid != null, "Expected non-null owning DataGrid.");

                if (_fillerCell == null)
                {
                    _fillerCell = new DataGridCell();

                    Microsoft.UI.Xaml.Automation.AutomationProperties.SetAccessibilityView(
                        _fillerCell,
                        AccessibilityView.Raw);

                    _fillerCell.Visibility = Visibility.Collapsed;
                    _fillerCell.OwningRow = this;
                    _fillerCell.EnsureStyle(null);
                    if (_cellsElement != null)
                    {
                        _cellsElement.Children.Add(_fillerCell);
                    }
                }

                return _fillerCell;
            }
        }

        internal bool HasBottomGridLine
        {
            get
            {
                return _bottomGridLine != null;
            }
        }

        internal bool HasHeaderCell
        {
            get
            {
                return _headerElement != null;
            }
        }

        internal DataGridRowHeader HeaderCell
        {
            get
            {
                return _headerElement;
            }
        }

        /// <summary>
        /// Gets or sets the index of the row.
        /// </summary>
        internal int Index
        {
            get;
            set;
        }

        internal bool IsEditing
        {
            get
            {
                return this.OwningGrid != null && this.OwningGrid.EditingRow == this;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the layout when template is applied.
        /// </summary>
        internal bool IsLayoutDelayed
        {
            get;
            private set;
        }

        internal bool IsPointerOver
        {
            get
            {
                return this.InteractionInfo != null && this.InteractionInfo.IsPointerOver;
            }

            set
            {
                if (value && this.InteractionInfo == null)
                {
                    this.InteractionInfo = new DataGridInteractionInfo();
                }

                if (this.InteractionInfo != null)
                {
                    this.InteractionInfo.IsPointerOver = value;
                }

                ApplyState(true /*animate*/);
            }
        }

        internal bool IsRecycled
        {
            get;
            private set;
        }

        internal bool IsRecyclable
        {
            get
            {
                if (this.OwningGrid != null)
                {
                    return this.OwningGrid.IsRowRecyclable(this);
                }

                return true;
            }
        }

        internal bool IsSelected
        {
            get
            {
                if (this.OwningGrid == null || this.Slot == -1)
                {
                    // The Slot can be -1 if we're about to reuse or recycle this row, but the layout cycle has not
                    // passed so we don't know the outcome yet.  We don't care whether or not it's selected in this case
                    return false;
                }

                DiagnosticsDebug.Assert(this.Index != -1, "Expected Index other than -1.");
                return this.OwningGrid.GetRowSelection(this.Slot);
            }
        }

        internal DataGrid OwningGrid
        {
            get;
            set;
        }

        internal Panel RootElement
        {
            get;
            private set;
        }

        internal int Slot
        {
            get;
            set;
        }

        // Height that the row will eventually end up at after a possible details animation has completed
        internal double TargetHeight
        {
            get
            {
                if (!double.IsNaN(this.Height))
                {
                    return this.Height;
                }
                else
                {
                    this.EnsureMeasured();
                    if (_detailsElement != null && _appliedDetailsVisibility == Visibility.Visible && _appliedDetailsTemplate != null)
                    {
                        DiagnosticsDebug.Assert(!double.IsNaN(_detailsElement.ContentHeight), "Expected _detailsElement.ContentHeight different from double.NaN.");
                        DiagnosticsDebug.Assert(!double.IsNaN(_detailsDesiredHeight), "Expected _detailsDesiredHeight different from double.NaN.");
                        return this.DesiredSize.Height + _detailsDesiredHeight - _detailsElement.ContentHeight;
                    }
                    else
                    {
                        return this.DesiredSize.Height;
                    }
                }
            }
        }

        // Returns the actual template that should be sued for Details: either explicitly set on this row
        // or inherited from the DataGrid
        private DataTemplate ActualDetailsTemplate
        {
            get
            {
                DiagnosticsDebug.Assert(this.OwningGrid != null, "Expected non-null owning DataGrid.");
                DataTemplate currentDetailsTemplate = this.DetailsTemplate;

                return currentDetailsTemplate != null ? currentDetailsTemplate : this.OwningGrid.RowDetailsTemplate;
            }
        }

        private Visibility ActualDetailsVisibility
        {
            get
            {
                if (this.OwningGrid == null)
                {
                    throw DataGridError.DataGrid.NoOwningGrid(this.GetType());
                }

                if (this.Index == -1)
                {
                    throw DataGridError.DataGridRow.InvalidRowIndexCannotCompleteOperation();
                }

                return this.OwningGrid.GetRowDetailsVisibility(this.Index);
            }
        }

        private bool AreDetailsVisible
        {
            get
            {
                return this.DetailsVisibility == Visibility.Visible;
            }
        }

        private DataGridInteractionInfo InteractionInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the row which contains the given element
        /// </summary>
        /// <param name="element">element contained in a row</param>
        /// <returns>Row that contains the element, or null if not found
        /// </returns>
        public static DataGridRow GetRowContainingElement(FrameworkElement element)
        {
            // Walk up the tree to find the DataGridRow that contains the element
            DependencyObject parent = element;
            DataGridRow row = parent as DataGridRow;
            while (parent != null && row == null)
            {
                parent = VisualTreeHelper.GetParent(parent);
                row = parent as DataGridRow;
            }

            return row;
        }

        /// <summary>
        /// Returns the index of the current row.
        /// </summary>
        /// <returns>
        /// The index of the current row.
        /// </returns>
        public int GetIndex()
        {
            return this.Index;
        }

        /// <summary>
        /// Arranges the content of the <see cref="DataGridRow"/>.
        /// </summary>
        /// <returns>
        /// The actual size used by the <see cref="DataGridRow"/>.
        /// </returns>
        /// <param name="finalSize">
        /// The final area within the parent that this element should use to arrange itself and its children.
        /// </param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.OwningGrid == null)
            {
                return base.ArrangeOverride(finalSize);
            }

            // If the DataGrid was scrolled horizontally after our last Arrange, we need to make sure
            // the Cells and Details are Arranged again
            if (_lastHorizontalOffset != this.OwningGrid.HorizontalOffset)
            {
                _lastHorizontalOffset = this.OwningGrid.HorizontalOffset;
                InvalidateHorizontalArrange();
            }

            Size size = base.ArrangeOverride(finalSize);

            if (_checkDetailsContentHeight)
            {
                _checkDetailsContentHeight = false;
                EnsureDetailsContentHeight();
            }

            if (this.RootElement != null)
            {
                foreach (UIElement child in this.RootElement.Children)
                {
                    if (DataGridFrozenGrid.GetIsFrozen(child))
                    {
                        TranslateTransform transform = new TranslateTransform();

                        // Automatic layout rounding doesn't apply to transforms so we need to Round this
                        transform.X = Math.Round(this.OwningGrid.HorizontalOffset);
                        child.RenderTransform = transform;
                    }
                }
            }

            if (_bottomGridLine != null)
            {
                RectangleGeometry gridlineClipGeometry = new RectangleGeometry();
                gridlineClipGeometry.Rect = new Rect(this.OwningGrid.HorizontalOffset, 0, Math.Max(0, this.DesiredSize.Width - this.OwningGrid.HorizontalOffset), _bottomGridLine.DesiredSize.Height);
                _bottomGridLine.Clip = gridlineClipGeometry;
            }

            return size;
        }

        /// <summary>
        /// Measures the children of a <see cref="DataGridRow"/> to
        /// prepare for arranging them during the <see cref="M:System.Windows.FrameworkElement.ArrangeOverride(System.Windows.Size)"/> pass.
        /// </summary>
        /// <param name="availableSize">
        /// The available size that this element can give to child elements. Indicates an upper limit that child elements should not exceed.
        /// </param>
        /// <returns>
        /// The size that the <see cref="DataGridRow"/> determines it needs during layout, based on its calculations of child object allocated sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.OwningGrid == null)
            {
                return base.MeasureOverride(availableSize);
            }

            // Allow the DataGrid specific components to adjust themselves based on new values
            if (_headerElement != null)
            {
                _headerElement.InvalidateMeasure();
            }

            if (_cellsElement != null)
            {
                _cellsElement.InvalidateMeasure();
            }

            if (_detailsElement != null)
            {
                _detailsElement.InvalidateMeasure();
            }

            bool currentAddItemIsDataContext = false;
#if FEATURE_IEDITABLECOLLECTIONVIEW
            currentAddItemIsDataContext = this.OwningGrid.DataConnection.EditableCollectionView.CurrentAddItem == this.DataContext;
#endif
            Size desiredSize = default;
            try
            {
                desiredSize = base.MeasureOverride(availableSize);
            }
            catch
            {
            }

            desiredSize.Width = Math.Max(desiredSize.Width, this.OwningGrid.CellsWidth);
            if (double.IsNaN(this.Height) && DoubleUtil.IsZero(this.MinHeight) &&
                (this.Index == this.OwningGrid.DataConnection.NewItemPlaceholderIndex ||
                (this.OwningGrid.DataConnection.IsAddingNew && currentAddItemIsDataContext)))
            {
                // In order to avoid auto-sizing the placeholder or new item row to an unusable height, we will
                // measure it at the DataGrid's average RowHeightEstimate if its Height has not been explicitly set.
                desiredSize.Height = Math.Max(desiredSize.Height, this.OwningGrid.RowHeightEstimate);
            }

            return desiredSize;
        }

        /// <summary>
        /// Builds the visual tree for the column header when a new template is applied.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            this.RootElement = GetTemplateChild(DATAGRIDROW_elementRoot) as Panel;

            if (this.RootElement != null)
            {
                EnsureBackground();
                ApplyState(false /*animate*/);
            }

            if (_cellsElement != null)
            {
                // If we're applying a new template, we  want to remove the cells from the previous _cellsElement
                _cellsElement.Children.Clear();
            }

            _cellsElement = GetTemplateChild(DATAGRIDROW_elementCells) as DataGridCellsPresenter;
            if (_cellsElement != null)
            {
                _cellsElement.OwningRow = this;

                // Cells that were already added before the Template was applied need to
                // be added to the Canvas
                if (this.Cells.Count > 0)
                {
                    foreach (DataGridCell cell in this.Cells)
                    {
                        _cellsElement.Children.Add(cell);
                    }
                }
            }

            _detailsElement = GetTemplateChild(DATAGRIDROW_elementDetails) as DataGridDetailsPresenter;
            if (_detailsElement != null && this.OwningGrid != null)
            {
                _detailsElement.OwningRow = this;
                if (this.ActualDetailsVisibility == Visibility.Visible && this.ActualDetailsTemplate != null && _appliedDetailsTemplate == null)
                {
                    // Apply the DetailsTemplate now that the row template is applied.
                    SetDetailsVisibilityInternal(
                        this.ActualDetailsVisibility,
                        _detailsVisibilityNotificationPending /*raiseNotification*/);
                    _detailsVisibilityNotificationPending = false;
                }
            }

            _bottomGridLine = GetTemplateChild(DATAGRIDROW_elementBottomGridLine) as Rectangle;

            _headerElement = GetTemplateChild(DATAGRIDROW_elementRowHeader) as DataGridRowHeader;
            if (_headerElement != null)
            {
                _headerElement.Owner = this;
                if (this.Header != null)
                {
                    _headerElement.Content = Header;
                }

                EnsureHeaderStyleAndVisibility(null);
            }

            EnsureGridLines();
            EnsureForeground();
        }

        /// <summary>
        /// Creates AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
        /// </summary>
        /// <returns>An automation peer for this <see cref="DataGridRow"/>.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DataGridRowAutomationPeer(this);
        }

        internal void ApplyCellsState(bool animate)
        {
            foreach (DataGridCell dataGridCell in this.Cells)
            {
                dataGridCell.ApplyCellState(animate);
            }
        }

        internal void ApplyDetailsTemplate(bool initializeDetailsPreferredHeight)
        {
            if (_detailsElement != null && this.AreDetailsVisible)
            {
                DataTemplate oldDetailsTemplate = _appliedDetailsTemplate;
                if (this.ActualDetailsTemplate != _appliedDetailsTemplate)
                {
                    if (this.ActualDetailsTemplate == null)
                    {
                        UnloadDetailsTemplate(false /*recycle*/, false /*setDetailsVisibilityToCollapsed*/);
                    }
                    else
                    {
                        if (_detailsContent != null)
                        {
                            _detailsContent.SizeChanged -= new SizeChangedEventHandler(DetailsContent_SizeChanged);
                            if (_detailsLoaded)
                            {
                                this.OwningGrid.OnUnloadingRowDetails(this, _detailsContent);
                                _detailsLoaded = false;
                            }
                        }

                        _detailsElement.Children.Clear();

                        _detailsContent = this.ActualDetailsTemplate.LoadContent() as FrameworkElement;
                        _appliedDetailsTemplate = this.ActualDetailsTemplate;

                        if (_detailsContent != null)
                        {
                            _detailsContent.SizeChanged += new SizeChangedEventHandler(DetailsContent_SizeChanged);
                            _detailsElement.Children.Add(_detailsContent);
                        }
                    }
                }

                if (_detailsContent != null && !_detailsLoaded)
                {
                    _detailsLoaded = true;
                    _detailsContent.DataContext = this.DataContext;
                    this.OwningGrid.OnLoadingRowDetails(this, _detailsContent);
                }

                if (initializeDetailsPreferredHeight && double.IsNaN(_detailsDesiredHeight) &&
                    _appliedDetailsTemplate != null && _detailsElement.Children.Count > 0)
                {
                    EnsureDetailsDesiredHeight();
                }
                else if (oldDetailsTemplate == null)
                {
                    _detailsDesiredHeight = double.NaN;
                    EnsureDetailsDesiredHeight();
                    _detailsElement.ContentHeight = _detailsDesiredHeight;
                }
            }
        }

        internal void ApplyHeaderState(bool animate)
        {
            if (_headerElement != null && this.OwningGrid.AreRowHeadersVisible)
            {
                _headerElement.ApplyOwnerState(animate);
            }
        }

        /// <summary>
        /// Updates the background brush of the row, using a storyboard if available.
        /// </summary>
        internal void ApplyState(bool animate)
        {
            if (this.RootElement != null && this.OwningGrid != null && this.Visibility == Visibility.Visible)
            {
                DiagnosticsDebug.Assert(this.Index != -1, "Expected Index other than -1.");
                byte idealStateMappingIndex = 0;
                if (this.IsSelected || this.IsEditing)
                {
                    idealStateMappingIndex += 8;
                }

                if (this.IsEditing)
                {
                    idealStateMappingIndex += 4;
                }

                if (this.IsPointerOver)
                {
                    idealStateMappingIndex += 2;
                }

                if (this.OwningGrid.ContainsFocus)
                {
                    idealStateMappingIndex += 1;
                }

                byte stateCode = _idealStateMapping[idealStateMappingIndex];
                DiagnosticsDebug.Assert(stateCode != DATAGRIDROW_stateNullCode, "stateCode other than DATAGRIDROW_stateNullCode.");

                string storyboardName;
                while (stateCode != DATAGRIDROW_stateNullCode)
                {
                    if (stateCode == DATAGRIDROW_stateNormalCode)
                    {
                        if (this.Index % 2 == 1)
                        {
                            storyboardName = DATAGRIDROW_stateAlternate;
                        }
                        else
                        {
                            storyboardName = DATAGRIDROW_stateNormal;
                        }
                    }
                    else
                    {
                        storyboardName = _stateNames[stateCode];
                    }

                    if (VisualStateManager.GoToState(this, storyboardName, animate))
                    {
                        break;
                    }
                    else
                    {
                        // The state wasn't implemented so fall back to the next one.
                        stateCode = _fallbackStateMapping[stateCode];
                    }
                }

                if (this.IsValid)
                {
                    VisualStates.GoToState(this, animate, VisualStates.StateValid);
                }
                else
                {
                    VisualStates.GoToState(this, animate, VisualStates.StateInvalid, VisualStates.StateValid);
                }

                ApplyHeaderState(animate);
            }
        }

        internal void DetachFromDataGrid(bool recycle)
        {
            UnloadDetailsTemplate(recycle, true /*setDetailsVisibilityToCollapsed*/);

            if (recycle)
            {
                Recycle();

                if (_cellsElement != null)
                {
                    _cellsElement.Recycle();
                }

                _checkDetailsContentHeight = false;

                // Clear out the old Details cache so it won't be reused for other data
                _detailsDesiredHeight = double.NaN;
                if (_detailsElement != null)
                {
                    _detailsElement.ClearValue(DataGridDetailsPresenter.ContentHeightProperty);
                }
            }

            this.Slot = -1;
        }

        // Make sure the row's background is set to its correct value.  It could be explicitly set or inherit
        // DataGrid.RowBackground or DataGrid.AlternatingRowBackground
        internal void EnsureBackground()
        {
            // Inherit the DataGrid's RowBackground properties only if this row doesn't explicitly have a background set
            if (this.RootElement != null && this.OwningGrid != null)
            {
                DiagnosticsDebug.Assert(this.Index != -1, "Expected Index other than -1.");

                Brush newBackground = null;
                if (this.Background == null)
                {
                    if (this.Index % 2 == 0 || this.OwningGrid.AlternatingRowBackground == null)
                    {
                        // Use OwningGrid.RowBackground if the index is even or if the OwningGrid.AlternatingRowBackground is null
                        if (this.OwningGrid.RowBackground != null)
                        {
                            newBackground = this.OwningGrid.RowBackground;
                        }
                    }
                    else
                    {
                        // Alternate row
                        if (this.OwningGrid.AlternatingRowBackground != null)
                        {
                            newBackground = this.OwningGrid.AlternatingRowBackground;
                        }
                    }
                }
                else
                {
                    newBackground = this.Background;
                }

                if (this.RootElement.Background != newBackground)
                {
                    this.RootElement.Background = newBackground;
                }
            }
        }

        // Make sure the row's foreground is set to its correct value.  It could be explicitly set or inherit
        // DataGrid.RowForeground or DataGrid.AlternatingRowForeground
        internal void EnsureForeground()
        {
            // Inherit the DataGrid's RowForeground properties only if this row doesn't explicitly have a foreground set
            if (this.OwningGrid != null)
            {
                DiagnosticsDebug.Assert(this.Index != -1, "Expected Index other than -1.");

                PropertyMetadata metadataInfo = DataGridRow.ForegroundProperty.GetMetadata(typeof(DataGridRow));
                Brush defaultForeground = metadataInfo == null ? null : metadataInfo.DefaultValue as Brush;
                Brush newForeground = null;

                if (this.Foreground.Equals(defaultForeground))
                {
                    if (this.Index % 2 == 0 || this.OwningGrid.AlternatingRowForeground == null)
                    {
                        // Use OwningGrid.RowForeground if the index is even or if the OwningGrid.AlternatingRowForeground is null
                        if (this.OwningGrid.RowForeground != null)
                        {
                            newForeground = this.OwningGrid.RowForeground;
                        }
                    }
                    else
                    {
                        // Alternate row
                        if (this.OwningGrid.AlternatingRowForeground != null)
                        {
                            newForeground = this.OwningGrid.AlternatingRowForeground;
                        }
                    }

                    if (newForeground == null)
                    {
                        newForeground = this.Foreground;
                    }
                }
                else
                {
                    newForeground = this.Foreground;
                }

                this.ComputedForeground = newForeground;
            }
            else
            {
                this.ComputedForeground = this.Foreground;
            }
        }

        internal void EnsureFillerVisibility()
        {
            if (_cellsElement != null)
            {
                _cellsElement.EnsureFillerVisibility();
            }
        }

        internal void EnsureGridLines()
        {
            if (this.OwningGrid != null)
            {
                if (_bottomGridLine != null)
                {
                    Visibility newVisibility = this.OwningGrid.GridLinesVisibility == DataGridGridLinesVisibility.Horizontal || this.OwningGrid.GridLinesVisibility == DataGridGridLinesVisibility.All
                        ? Visibility.Visible : Visibility.Collapsed;

                    if (newVisibility != _bottomGridLine.Visibility)
                    {
                        _bottomGridLine.Visibility = newVisibility;
                    }

                    EnsureHeaderGridLines(newVisibility);

                    _bottomGridLine.Fill = this.OwningGrid.HorizontalGridLinesBrush;
                }

                foreach (DataGridCell cell in this.Cells)
                {
                    cell.EnsureGridLine(this.OwningGrid.ColumnsInternal.LastVisibleColumn);
                }
            }
        }

        // Set the proper style for the Header by walking up the Style hierarchy
        internal void EnsureHeaderStyleAndVisibility(Style previousStyle)
        {
            if (_headerElement != null && this.OwningGrid != null)
            {
                if (this.OwningGrid.AreRowHeadersVisible)
                {
                    _headerElement.EnsureStyle(previousStyle);
                    _headerElement.Visibility = Visibility.Visible;
                }
                else
                {
                    _headerElement.Visibility = Visibility.Collapsed;
                }
            }
        }

        internal void EnsureHeaderVisibility()
        {
            if (_headerElement != null && this.OwningGrid != null)
            {
                _headerElement.Visibility = this.OwningGrid.AreRowHeadersVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void EnsureHeaderGridLines(Visibility visibility)
        {
            if (_headerElement != null)
            {
                _headerElement.SeparatorVisibility = visibility;
            }
        }

        internal void InvalidateHorizontalArrange()
        {
            if (_cellsElement != null)
            {
                _cellsElement.InvalidateArrange();
            }

            if (_detailsElement != null)
            {
                _detailsElement.InvalidateArrange();
            }
        }

        // Sets AreDetailsVisible on the row and animates if necessary
        internal void SetDetailsVisibilityInternal(
            Visibility visibility,
            bool raiseNotification)
        {
            DiagnosticsDebug.Assert(this.OwningGrid != null, "Expected non-null owning DataGrid.");
            DiagnosticsDebug.Assert(this.Index != -1, "Expected Index other than -1.");

            if (_appliedDetailsVisibility != visibility)
            {
                if (_detailsElement == null)
                {
                    if (raiseNotification)
                    {
                        _detailsVisibilityNotificationPending = true;
                    }

                    return;
                }

                _appliedDetailsVisibility = visibility;
                this.SetValueNoCallback(DetailsVisibilityProperty, visibility);

                // Applies a new DetailsTemplate only if it has changed either here or at the DataGrid level
                ApplyDetailsTemplate(true /*initializeDetailsPreferredHeight*/);

                // no template to show
                if (_appliedDetailsTemplate == null)
                {
                    if (_detailsElement.ContentHeight > 0)
                    {
                        _detailsElement.ContentHeight = 0;
                    }

                    return;
                }

                if (this.AreDetailsVisible)
                {
                    // Set the details height directly
                    _detailsElement.ContentHeight = _detailsDesiredHeight;
                    _checkDetailsContentHeight = true;
                }
                else
                {
                    _detailsElement.ContentHeight = 0;
                }

                OnRowDetailsChanged();

                if (raiseNotification)
                {
                    this.OwningGrid.OnRowDetailsVisibilityChanged(new DataGridRowDetailsEventArgs(this, _detailsContent));
                }
            }
        }

        private void CancelPointer(PointerRoutedEventArgs e)
        {
            if (this.InteractionInfo != null && this.InteractionInfo.CapturedPointerId == e.Pointer.PointerId)
            {
                this.InteractionInfo.CapturedPointerId = 0u;
            }

            this.IsPointerOver = false;
        }

        private void DataGridCellCollection_CellAdded(object sender, DataGridCellEventArgs e)
        {
            if (_cellsElement != null)
            {
                _cellsElement.Children.Add(e.Cell);
            }
        }

        private void DataGridCellCollection_CellRemoved(object sender, DataGridCellEventArgs e)
        {
            if (_cellsElement != null)
            {
                _cellsElement.Children.Remove(e.Cell);
            }
        }

        private void DataGridRow_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            CancelPointer(e);
        }

        private void DataGridRow_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            CancelPointer(e);
        }

        private void DataGridRow_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch &&
                this.OwningGrid != null &&
                this.OwningGrid.AllowsManipulation &&
                (this.InteractionInfo == null || this.InteractionInfo.CapturedPointerId == 0u) &&
                this.CapturePointer(e.Pointer))
            {
                if (this.InteractionInfo == null)
                {
                    this.InteractionInfo = new DataGridInteractionInfo();
                }

                this.InteractionInfo.CapturedPointerId = e.Pointer.PointerId;
            }
        }

        private void DataGridRow_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (this.InteractionInfo != null && this.InteractionInfo.CapturedPointerId == e.Pointer.PointerId)
            {
                ReleasePointerCapture(e.Pointer);
            }
        }

        private void DataGridRow_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            UpdateIsPointerOver(true);
        }

        private void DataGridRow_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            UpdateIsPointerOver(false);
        }

        private void DataGridRow_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            UpdateIsPointerOver(true);
        }

        private void DataGridRow_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.OwningGrid != null && !this.OwningGrid.HasColumnUserInteraction)
            {
                if (this.OwningGrid.UpdatedStateOnTapped)
                {
                    this.OwningGrid.UpdatedStateOnTapped = false;
                }
                else
                {
                    e.Handled = this.OwningGrid.UpdateStateOnTapped(e, -1, this.Slot, false /*allowEdit*/);
                }
            }
        }

        private void DetailsContent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Height != e.PreviousSize.Height && e.NewSize.Height != _detailsDesiredHeight)
            {
                // Update the new desired height for RowDetails
                _detailsDesiredHeight = e.NewSize.Height;

                if (this.AreDetailsVisible && _appliedDetailsTemplate != null)
                {
                    _detailsElement.ContentHeight = e.NewSize.Height;

                    // Calling this when details are not visible invalidates during layout when we have no work
                    // to do.  In certain scenarios, this could cause a layout cycle
                    OnRowDetailsChanged();
                }
            }
        }

        // Makes sure the _detailsDesiredHeight is initialized.  We need to measure it to know what
        // height we want to animate to.  Subsequently, we just update that height in response to SizeChanged.
        private void EnsureDetailsDesiredHeight()
        {
            DiagnosticsDebug.Assert(_detailsElement != null, "Expected non-null _detailsElement.");
            DiagnosticsDebug.Assert(this.OwningGrid != null, "Expected non-null owning DataGrid.");

            if (_detailsContent != null)
            {
                DiagnosticsDebug.Assert(_detailsElement.Children.Contains(_detailsContent), "Expected _detailsElement parent of _detailsContent.");

                _detailsContent.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                _detailsDesiredHeight = _detailsContent.DesiredSize.Height;
            }
            else
            {
                _detailsDesiredHeight = 0;
            }
        }

        private void EnsureDetailsContentHeight()
        {
            if (_detailsElement != null &&
                _detailsContent != null &&
                double.IsNaN(_detailsContent.Height) &&
                this.AreDetailsVisible &&
                !double.IsNaN(_detailsDesiredHeight) &&
                !DoubleUtil.AreClose(_detailsContent.ActualHeight, _detailsDesiredHeight) &&
                this.Slot != -1)
            {
                _detailsDesiredHeight = _detailsContent.ActualHeight;
                _detailsElement.ContentHeight = _detailsDesiredHeight;
            }
        }

        private void OnDependencyPropertyChanged(DependencyObject dependencyObject, DependencyProperty dependencyProperty)
        {
            if (dependencyProperty == Control.ForegroundProperty)
            {
                EnsureForeground();
            }
        }

        private void OnRowDetailsChanged()
        {
            if (this.OwningGrid != null)
            {
                this.OwningGrid.OnRowDetailsChanged();
            }
        }

        private void Recycle()
        {
            this.InteractionInfo = null;
            this.IsRecycled = true;
        }

        private void UnloadDetailsTemplate(bool recycle, bool setDetailsVisibilityToCollapsed)
        {
            if (_detailsElement != null)
            {
                if (_detailsContent != null)
                {
                    if (_detailsLoaded)
                    {
                        this.OwningGrid.OnUnloadingRowDetails(this, _detailsContent);
                    }

                    _detailsContent.DataContext = null;
                    if (!recycle)
                    {
                        _detailsContent.SizeChanged -= new SizeChangedEventHandler(DetailsContent_SizeChanged);
                        _detailsContent = null;
                    }
                }

                if (!recycle)
                {
                    _detailsElement.Children.Clear();
                }

                _detailsElement.ContentHeight = 0;
            }

            if (!recycle)
            {
                _appliedDetailsTemplate = null;
                this.SetValueNoCallback(DetailsTemplateProperty, null);
            }

            _detailsLoaded = false;
            _appliedDetailsVisibility = null;

            if (setDetailsVisibilityToCollapsed)
            {
                this.SetValueNoCallback(DetailsVisibilityProperty, Visibility.Collapsed);
            }
        }

        private void UpdateIsPointerOver(bool isPointerOver)
        {
            if (this.InteractionInfo != null && this.InteractionInfo.CapturedPointerId != 0u)
            {
                return;
            }

            this.IsPointerOver = isPointerOver;
        }

#if DEBUG
        /// <summary>
        /// Gets the row's Index.
        /// </summary>
        public int Debug_Index
        {
            get
            {
                return this.Index;
            }
        }
#endif
    }
}