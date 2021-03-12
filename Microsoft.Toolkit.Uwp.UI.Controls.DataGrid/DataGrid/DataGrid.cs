// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security;
using System.Text;
using Microsoft.Toolkit.Uwp.UI.Automation.Peers;
using Microsoft.Toolkit.Uwp.UI.Controls.DataGridInternals;
using Microsoft.Toolkit.Uwp.UI.Controls.Primitives;
using Microsoft.Toolkit.Uwp.UI.Controls.Utilities;
using Microsoft.Toolkit.Uwp.UI.Data.Utilities;
using Microsoft.Toolkit.Uwp.UI.Utilities;
using Microsoft.Toolkit.Uwp.Utilities;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

using DiagnosticsDebug = System.Diagnostics.Debug;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Control to represent data in columns and rows.
    /// </summary>
#if FEATURE_VALIDATION_SUMMARY
    [TemplatePart(Name = DataGrid.DATAGRID_elementValidationSummary, Type = typeof(ValidationSummary))]
#endif
    [TemplatePart(Name = DataGrid.DATAGRID_elementRowsPresenterName, Type = typeof(DataGridRowsPresenter))]
    [TemplatePart(Name = DataGrid.DATAGRID_elementColumnHeadersPresenterName, Type = typeof(DataGridColumnHeadersPresenter))]
    [TemplatePart(Name = DataGrid.DATAGRID_elementFrozenColumnScrollBarSpacerName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = DataGrid.DATAGRID_elementHorizontalScrollBarName, Type = typeof(ScrollBar))]
    [TemplatePart(Name = DataGrid.DATAGRID_elementVerticalScrollBarName, Type = typeof(ScrollBar))]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateTouchIndicator, GroupName = VisualStates.GroupScrollBars)]
    [TemplateVisualState(Name = VisualStates.StateMouseIndicator, GroupName = VisualStates.GroupScrollBars)]
    [TemplateVisualState(Name = VisualStates.StateMouseIndicatorFull, GroupName = VisualStates.GroupScrollBars)]
    [TemplateVisualState(Name = VisualStates.StateNoIndicator, GroupName = VisualStates.GroupScrollBars)]
    [TemplateVisualState(Name = VisualStates.StateSeparatorExpanded, GroupName = VisualStates.GroupScrollBarsSeparator)]
    [TemplateVisualState(Name = VisualStates.StateSeparatorCollapsed, GroupName = VisualStates.GroupScrollBarsSeparator)]
    [TemplateVisualState(Name = VisualStates.StateSeparatorExpandedWithoutAnimation, GroupName = VisualStates.GroupScrollBarsSeparator)]
    [TemplateVisualState(Name = VisualStates.StateSeparatorCollapsedWithoutAnimation, GroupName = VisualStates.GroupScrollBarsSeparator)]
    [TemplateVisualState(Name = VisualStates.StateInvalid, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateValid, GroupName = VisualStates.GroupValidation)]
    [StyleTypedProperty(Property = "CellStyle", StyleTargetType = typeof(DataGridCell))]
    [StyleTypedProperty(Property = "ColumnHeaderStyle", StyleTargetType = typeof(DataGridColumnHeader))]
    [StyleTypedProperty(Property = "DragIndicatorStyle", StyleTargetType = typeof(ContentControl))]
    [StyleTypedProperty(Property = "DropLocationIndicatorStyle", StyleTargetType = typeof(Control))]
    [StyleTypedProperty(Property = "RowHeaderStyle", StyleTargetType = typeof(DataGridRowHeader))]
    [StyleTypedProperty(Property = "RowStyle", StyleTargetType = typeof(DataGridRow))]
    public partial class DataGrid : Control
    {
        private enum ScrollBarVisualState
        {
            NoIndicator,
            TouchIndicator,
            MouseIndicator,
            MouseIndicatorFull
        }

        private enum ScrollBarsSeparatorVisualState
        {
            SeparatorCollapsed,
            SeparatorExpanded,
            SeparatorExpandedWithoutAnimation,
            SeparatorCollapsedWithoutAnimation
        }

#if FEATURE_VALIDATION_SUMMARY
        private const string DATAGRID_elementValidationSummary = "ValidationSummary";
#endif
        private const string DATAGRID_elementRootName = "Root";
        private const string DATAGRID_elementRowsPresenterName = "RowsPresenter";
        private const string DATAGRID_elementColumnHeadersPresenterName = "ColumnHeadersPresenter";
        private const string DATAGRID_elementFrozenColumnScrollBarSpacerName = "FrozenColumnScrollBarSpacer";
        private const string DATAGRID_elementHorizontalScrollBarName = "HorizontalScrollBar";
        private const string DATAGRID_elementRowHeadersPresenterName = "RowHeadersPresenter";
        private const string DATAGRID_elementTopLeftCornerHeaderName = "TopLeftCornerHeader";
        private const string DATAGRID_elementTopRightCornerHeaderName = "TopRightCornerHeader";
        private const string DATAGRID_elementBottomRightCornerHeaderName = "BottomRightCorner";
        private const string DATAGRID_elementVerticalScrollBarName = "VerticalScrollBar";

        private const bool DATAGRID_defaultAutoGenerateColumns = true;
        private const bool DATAGRID_defaultCanUserReorderColumns = true;
        private const bool DATAGRID_defaultCanUserResizeColumns = true;
        private const bool DATAGRID_defaultCanUserSortColumns = true;
        private const DataGridGridLinesVisibility DATAGRID_defaultGridLinesVisibility = DataGridGridLinesVisibility.None;
        private const DataGridHeadersVisibility DATAGRID_defaultHeadersVisibility = DataGridHeadersVisibility.Column;
        private const DataGridRowDetailsVisibilityMode DATAGRID_defaultRowDetailsVisibility = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
        private const DataGridSelectionMode DATAGRID_defaultSelectionMode = DataGridSelectionMode.Extended;
        private const ScrollBarVisibility DATAGRID_defaultScrollBarVisibility = ScrollBarVisibility.Auto;

        /// <summary>
        /// The default order to use for columns when there is no <see cref="DisplayAttribute.Order"/>
        /// value available for the property.
        /// </summary>
        /// <remarks>
        /// The value of 10,000 comes from the DataAnnotations spec, allowing
        /// some properties to be ordered at the beginning and some at the end.
        /// </remarks>
        private const int DATAGRID_defaultColumnDisplayOrder = 10000;

        private const double DATAGRID_horizontalGridLinesThickness = 1;
        private const double DATAGRID_minimumRowHeaderWidth = 4;
        private const double DATAGRID_minimumColumnHeaderHeight = 4;
        internal const double DATAGRID_maximumStarColumnWidth = 10000;
        internal const double DATAGRID_minimumStarColumnWidth = 0.001;
        private const double DATAGRID_mouseWheelDeltaDivider = 4.0;
        private const double DATAGRID_maxHeadersThickness = 32768;

        private const double DATAGRID_defaultRowHeight = 22;
        internal const double DATAGRID_defaultRowGroupSublevelIndent = 20;
        private const double DATAGRID_defaultMinColumnWidth = 20;
        private const double DATAGRID_defaultMaxColumnWidth = double.PositiveInfinity;

        private const double DATAGRID_defaultIncrementalLoadingThreshold = 3.0;
        private const double DATAGRID_defaultDataFetchSize = 3.0;

        // 2 seconds delay used to hide the scroll bars for example when OS animations are turned off.
        private const int DATAGRID_noScrollBarCountdownMs = 2000;

        // Used to work around double arithmetic rounding.
        private const double DATAGRID_roundingDelta = 0.0001;

        // DataGrid Template Parts
#if FEATURE_VALIDATION_SUMMARY
        private ValidationSummary _validationSummary;
#endif
        private UIElement _bottomRightCorner;
        private DataGridColumnHeadersPresenter _columnHeadersPresenter;
        private ScrollBar _hScrollBar;
        private DataGridRowsPresenter _rowsPresenter;
        private ScrollBar _vScrollBar;

        private byte _autoGeneratingColumnOperationCount;
        private bool _autoSizingColumns;
        private List<ValidationResult> _bindingValidationResults;
        private ContentControl _clipboardContentControl;
        private IndexToValueTable<Visibility> _collapsedSlotsTable;
        private bool _columnHeaderHasFocus;
        private DataGridCellCoordinates _currentCellCoordinates;

        // used to store the current column during a Reset
        private int _desiredCurrentColumnIndex;
        private int _editingColumnIndex;
        private RoutedEventArgs _editingEventArgs;
        private bool _executingLostFocusActions;
        private bool _flushCurrentCellChanged;
        private bool _focusEditingControl;
        private FocusInputDeviceKind _focusInputDevice;
        private DependencyObject _focusedObject;
        private DataGridRow _focusedRow;
        private FrameworkElement _frozenColumnScrollBarSpacer;
        private bool _hasNoIndicatorStateStoryboardCompletedHandler;
        private DispatcherQueueTimer _hideScrollBarsTimer;

        // the sum of the widths in pixels of the scrolling columns preceding
        // the first displayed scrolling column
        private double _horizontalOffset;
        private byte _horizontalScrollChangesIgnored;
        private bool _ignoreNextScrollBarsLayout;
        private List<ValidationResult> _indeiValidationResults;
        private bool _initializingNewItem;

        private bool _isHorizontalScrollBarInteracting;
        private bool _isVerticalScrollBarInteracting;

        // Set to True when the pointer is over the optional scroll bars.
        private bool _isPointerOverHorizontalScrollBar;
        private bool _isPointerOverVerticalScrollBar;

        // Set to True to prevent the normal fade-out of the scroll bars.
        private bool _keepScrollBarsShowing;

        // Nth row of rows 0..N that make up the RowHeightEstimate
        private int _lastEstimatedRow;
        private List<DataGridRow> _loadedRows;

        // prevents reentry into the VerticalScroll event handler
        private Queue<Action> _lostFocusActions;
        private bool _makeFirstDisplayedCellCurrentCellPending;
        private bool _measured;

        // the number of pixels of the firstDisplayedScrollingCol which are not displayed
        private double _negHorizontalOffset;

        // the number of pixels of DisplayData.FirstDisplayedScrollingRow which are not displayed
        private int _noCurrentCellChangeCount;
        private int _noFocusedColumnChangeCount;
        private int _noSelectionChangeCount;

        private double _oldEdgedRowsHeightCalculated = 0.0;

        // Set to True to favor mouse indicators over panning indicators for the scroll bars.
        private bool _preferMouseIndicators;

        private DataGridCellCoordinates _previousAutomationFocusCoordinates;
        private DataGridColumn _previousCurrentColumn;
        private object _previousCurrentItem;
        private List<ValidationResult> _propertyValidationResults;
        private ScrollBarVisualState _proposedScrollBarsState;
        private ScrollBarsSeparatorVisualState _proposedScrollBarsSeparatorState;
        private string _rowGroupHeaderPropertyNameAlternative;
        private ObservableCollection<Style> _rowGroupHeaderStyles;

        // To figure out what the old RowGroupHeaderStyle was for each level, we need to keep a copy
        // of the list.  The old style important so we don't blow away styles set directly on the RowGroupHeader
        private List<Style> _rowGroupHeaderStylesOld;
        private double[] _rowGroupHeightsByLevel;
        private double _rowHeaderDesiredWidth;
        private Size? _rowsPresenterAvailableSize;
        private bool _scrollingByHeight;
        private DataGridSelectedItemsCollection _selectedItems;
        private IndexToValueTable<Visibility> _showDetailsTable;

        // Set to True when the mouse scroll bars are currently showing.
        private bool _showingMouseIndicators;
        private bool _successfullyUpdatedSelection;
        private bool _temporarilyResetCurrentCell;
        private bool _isUserSorting; // True if we're currently in a user invoked sorting operation
        private ContentControl _topLeftCornerHeader;
        private ContentControl _topRightCornerHeader;
        private object _uneditedValue; // Represents the original current cell value at the time it enters editing mode.
        private string _updateSourcePath;
        private Dictionary<INotifyDataErrorInfo, string> _validationItems;
        private List<ValidationResult> _validationResults;
        private byte _verticalScrollChangesIgnored;
#if FEATURE_ICOLLECTIONVIEW_GROUP
        private INotifyCollectionChanged _topLevelGroup;
#else
        private IObservableVector<object> _topLevelGroup;
#endif
#if FEATURE_VALIDATION_SUMMARY
        private ValidationSummaryItem _selectedValidationSummaryItem;
#endif

        // An approximation of the sum of the heights in pixels of the scrolling rows preceding
        // the first displayed scrolling row.  Since the scrolled off rows are discarded, the grid
        // does not know their actual height. The heights used for the approximation are the ones
        // set as the rows were scrolled off.
        private double _verticalOffset;

#if FEATURE_ICOLLECTIONVIEW_GROUP
        // Cache event listeners for PropertyChanged and CollectionChanged events from CollectionViewGroups
        private Dictionary<INotifyPropertyChanged, WeakEventListener<DataGrid, object, PropertyChangedEventArgs>> _groupsPropertyChangedListenersTable = new Dictionary<INotifyPropertyChanged, WeakEventListener<DataGrid, object, PropertyChangedEventArgs>>();
        private Dictionary<INotifyCollectionChanged, WeakEventListener<DataGrid, object, NotifyCollectionChangedEventArgs>> _groupsCollectionChangedListenersTable = new Dictionary<INotifyCollectionChanged, WeakEventListener<DataGrid, object, NotifyCollectionChangedEventArgs>>();
#else
        // Cache event listeners for VectorChanged events from ICollectionViewGroup's GroupItems
        private Dictionary<IObservableVector<object>, WeakEventListener<DataGrid, object, IVectorChangedEventArgs>> _groupsVectorChangedListenersTable = new Dictionary<IObservableVector<object>, WeakEventListener<DataGrid, object, IVectorChangedEventArgs>>();
#endif

        /// <summary>
        /// Occurs one time for each public, non-static property in the bound data type when the
        /// <see cref="ItemsSource"/> property is changed and the
        /// <see cref="AutoGenerateColumns"/> property is true.
        /// </summary>
        public event EventHandler<DataGridAutoGeneratingColumnEventArgs> AutoGeneratingColumn;

        /// <summary>
        /// Occurs before a cell or row enters editing mode.
        /// </summary>
        public event EventHandler<DataGridBeginningEditEventArgs> BeginningEdit;

        /// <summary>
        /// Occurs after cell editing has ended.
        /// </summary>
        public event EventHandler<DataGridCellEditEndedEventArgs> CellEditEnded;

        /// <summary>
        /// Occurs immediately before cell editing has ended.
        /// </summary>
        public event EventHandler<DataGridCellEditEndingEventArgs> CellEditEnding;

        /// <summary>
        /// Occurs when the <see cref="Microsoft.Toolkit.Uwp.UI.Controls.DataGridColumn.DisplayIndex"/>
        /// property of a column changes.
        /// </summary>
        public event EventHandler<DataGridColumnEventArgs> ColumnDisplayIndexChanged;

        /// <summary>
        /// Occurs when the user drops a column header that was being dragged using the mouse.
        /// </summary>
        public event EventHandler<DragCompletedEventArgs> ColumnHeaderDragCompleted;

        /// <summary>
        /// Occurs one or more times while the user drags a column header using the mouse.
        /// </summary>
        public event EventHandler<DragDeltaEventArgs> ColumnHeaderDragDelta;

        /// <summary>
        /// Occurs when the user begins dragging a column header using the mouse.
        /// </summary>
        public event EventHandler<DragStartedEventArgs> ColumnHeaderDragStarted;

        /// <summary>
        /// Raised when column reordering ends, to allow subscribers to clean up.
        /// </summary>
        public event EventHandler<DataGridColumnEventArgs> ColumnReordered;

        /// <summary>
        /// Raised when starting a column reordering action.  Subscribers to this event can
        /// set tooltip and caret UIElements, constrain tooltip position, indicate that
        /// a preview should be shown, or cancel reordering.
        /// </summary>
        public event EventHandler<DataGridColumnReorderingEventArgs> ColumnReordering;

        /// <summary>
        /// This event is raised by OnCopyingRowClipboardContent method after the default row content is prepared.
        /// Event listeners can modify or add to the row clipboard content.
        /// </summary>
        public event EventHandler<DataGridRowClipboardEventArgs> CopyingRowClipboardContent;

        /// <summary>
        /// Occurs when a different cell becomes the current cell.
        /// </summary>
        public event EventHandler<EventArgs> CurrentCellChanged;

        /// <summary>
        /// Occurs after a <see cref="DataGridRow"/>
        /// is instantiated, so that you can customize it before it is used.
        /// </summary>
        public event EventHandler<DataGridRowEventArgs> LoadingRow;

        /// <summary>
        /// Occurs when a new row details template is applied to a row, so that you can customize
        /// the details section before it is used.
        /// </summary>
        public event EventHandler<DataGridRowDetailsEventArgs> LoadingRowDetails;

        /// <summary>
        /// Occurs before a DataGridRowGroupHeader header is used.
        /// </summary>
        public event EventHandler<DataGridRowGroupHeaderEventArgs> LoadingRowGroup;

        /// <summary>
        /// Occurs when a cell in a <see cref="DataGridTemplateColumn"/> enters editing mode.
        /// </summary>
        public event EventHandler<DataGridPreparingCellForEditEventArgs> PreparingCellForEdit;

        /// <summary>
        /// Occurs when the <see cref="RowDetailsVisibilityMode"/>
        /// property value changes.
        /// </summary>
        public event EventHandler<DataGridRowDetailsEventArgs> RowDetailsVisibilityChanged;

        /// <summary>
        /// Occurs when the row has been successfully committed or canceled.
        /// </summary>
        public event EventHandler<DataGridRowEditEndedEventArgs> RowEditEnded;

        /// <summary>
        /// Occurs immediately before the row has been successfully committed or canceled.
        /// </summary>
        public event EventHandler<DataGridRowEditEndingEventArgs> RowEditEnding;

        /// <summary>
        /// Occurs when the <see cref="SelectedItem"/> or
        /// <see cref="SelectedItems"/> property value changes.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Occurs when the <see cref="Microsoft.Toolkit.Uwp.UI.Controls.DataGridColumn"/> sorting request is triggered.
        /// </summary>
        public event EventHandler<DataGridColumnEventArgs> Sorting;

        /// <summary>
        /// Occurs when a <see cref="DataGridRow"/>
        /// object becomes available for reuse.
        /// </summary>
        public event EventHandler<DataGridRowEventArgs> UnloadingRow;

        /// <summary>
        /// Occurs when the DataGridRowGroupHeader is available for reuse.
        /// </summary>
        public event EventHandler<DataGridRowGroupHeaderEventArgs> UnloadingRowGroup;

        /// <summary>
        /// Occurs when a row details element becomes available for reuse.
        /// </summary>
        public event EventHandler<DataGridRowDetailsEventArgs> UnloadingRowDetails;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGrid"/> class.
        /// </summary>
        public DataGrid()
        {
            this.TabNavigation = KeyboardNavigationMode.Once;

            _loadedRows = new List<DataGridRow>();
            _lostFocusActions = new Queue<Action>();
            _selectedItems = new DataGridSelectedItemsCollection(this);
            _rowGroupHeaderPropertyNameAlternative = Properties.Resources.DefaultRowGroupHeaderPropertyNameAlternative;
            _rowGroupHeaderStyles = new ObservableCollection<Style>();
            _rowGroupHeaderStyles.CollectionChanged += RowGroupHeaderStyles_CollectionChanged;
            _rowGroupHeaderStylesOld = new List<Style>();
            this.RowGroupHeadersTable = new IndexToValueTable<DataGridRowGroupInfo>();

            _collapsedSlotsTable = new IndexToValueTable<Visibility>();
            _validationItems = new Dictionary<INotifyDataErrorInfo, string>();
            _validationResults = new List<ValidationResult>();
            _bindingValidationResults = new List<ValidationResult>();
            _propertyValidationResults = new List<ValidationResult>();
            _indeiValidationResults = new List<ValidationResult>();

            this.ColumnHeaderInteractionInfo = new DataGridColumnHeaderInteractionInfo();
            this.DisplayData = new DataGridDisplayData(this);
            this.ColumnsInternal = CreateColumnsInstance();

            this.RowHeightEstimate = DATAGRID_defaultRowHeight;
            this.RowDetailsHeightEstimate = 0;
            _rowHeaderDesiredWidth = 0;

            this.DataConnection = new DataGridDataConnection(this);
            _showDetailsTable = new IndexToValueTable<Visibility>();

            _focusInputDevice = FocusInputDeviceKind.None;
            _proposedScrollBarsState = ScrollBarVisualState.NoIndicator;
            _proposedScrollBarsSeparatorState = ScrollBarsSeparatorVisualState.SeparatorCollapsed;

            this.AnchorSlot = -1;
            _lastEstimatedRow = -1;
            _editingColumnIndex = -1;
            this.CurrentCellCoordinates = new DataGridCellCoordinates(-1, -1);

            this.RowGroupHeaderHeightEstimate = DATAGRID_defaultRowHeight;

            this.LastHandledKeyDown = VirtualKey.None;

            this.DefaultStyleKey = typeof(DataGrid);

            HookDataGridEvents();
        }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Windows.Media.Brush"/> that is used to paint the background of odd-numbered rows.
        /// </summary>
        /// <returns>
        /// The brush that is used to paint the background of odd-numbered rows.
        /// </returns>
        public Brush AlternatingRowBackground
        {
            get { return GetValue(AlternatingRowBackgroundProperty) as Brush; }
            set { SetValue(AlternatingRowBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AlternatingRowBackground"/>
        /// dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="AlternatingRowBackground"/>
        /// dependency property.
        /// </returns>
        public static readonly DependencyProperty AlternatingRowBackgroundProperty =
            DependencyProperty.Register(
                "AlternatingRowBackground",
                typeof(Brush),
                typeof(DataGrid),
                new PropertyMetadata(null, OnAlternatingRowBackgroundPropertyChanged));

        private static void OnAlternatingRowBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            foreach (DataGridRow row in dataGrid.GetAllRows())
            {
                row.EnsureBackground();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Windows.Media.Brush"/> that is used to paint the foreground of odd-numbered rows.
        /// </summary>
        /// <returns>
        /// The brush that is used to paint the foreground of odd-numbered rows.
        /// </returns>
        public Brush AlternatingRowForeground
        {
            get { return GetValue(AlternatingRowForegroundProperty) as Brush; }
            set { SetValue(AlternatingRowForegroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AlternatingRowForeground"/>
        /// dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="AlternatingRowForeground"/>
        /// dependency property.
        /// </returns>
        public static readonly DependencyProperty AlternatingRowForegroundProperty =
            DependencyProperty.Register(
                "AlternatingRowForeground",
                typeof(Brush),
                typeof(DataGrid),
                new PropertyMetadata(null, OnAlternatingRowForegroundPropertyChanged));

        private static void OnAlternatingRowForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            foreach (DataGridRow row in dataGrid.GetAllRows())
            {
                row.EnsureForeground();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the row details sections remain
        /// fixed at the width of the display area or can scroll horizontally.
        /// </summary>
        public bool AreRowDetailsFrozen
        {
            get { return (bool)GetValue(AreRowDetailsFrozenProperty); }
            set { SetValue(AreRowDetailsFrozenProperty, value); }
        }

        /// <summary>
        /// Identifies the AreRowDetailsFrozen dependency property.
        /// </summary>
        public static readonly DependencyProperty AreRowDetailsFrozenProperty =
            DependencyProperty.Register(
                "AreRowDetailsFrozen",
                typeof(bool),
                typeof(DataGrid),
                null);

        /// <summary>
        /// Gets or sets a value indicating whether the row group header sections
        /// remain fixed at the width of the display area or can scroll horizontally.
        /// </summary>
        public bool AreRowGroupHeadersFrozen
        {
            get { return (bool)GetValue(AreRowGroupHeadersFrozenProperty); }
            set { SetValue(AreRowGroupHeadersFrozenProperty, value); }
        }

        /// <summary>
        /// Identifies the AreRowDetailsFrozen dependency property.
        /// </summary>
        public static readonly DependencyProperty AreRowGroupHeadersFrozenProperty =
            DependencyProperty.Register(
                "AreRowGroupHeadersFrozen",
                typeof(bool),
                typeof(DataGrid),
                new PropertyMetadata(true, OnAreRowGroupHeadersFrozenPropertyChanged));

        private static void OnAreRowGroupHeadersFrozenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            ProcessFrozenColumnCount(dataGrid);

            // Update elements in the RowGroupHeader that were previously frozen.
            if ((bool)e.NewValue)
            {
                if (dataGrid._rowsPresenter != null)
                {
                    foreach (UIElement element in dataGrid._rowsPresenter.Children)
                    {
                        DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                        if (groupHeader != null)
                        {
                            groupHeader.ClearFrozenStates();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether columns are created
        /// automatically when the <see cref="ItemsSource"/> property is set.
        /// </summary>
        public bool AutoGenerateColumns
        {
            get { return (bool)GetValue(AutoGenerateColumnsProperty); }
            set { SetValue(AutoGenerateColumnsProperty, value); }
        }

        /// <summary>
        /// Identifies the AutoGenerateColumns dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoGenerateColumnsProperty =
            DependencyProperty.Register(
                "AutoGenerateColumns",
                typeof(bool),
                typeof(DataGrid),
                new PropertyMetadata(DATAGRID_defaultAutoGenerateColumns, OnAutoGenerateColumnsPropertyChanged));

        private static void OnAutoGenerateColumnsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            bool value = (bool)e.NewValue;
            if (value)
            {
                dataGrid.InitializeElements(false /*recycleRows*/);
            }
            else
            {
                dataGrid.RemoveAutoGeneratedColumns();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can change
        /// the column display order by dragging column headers with the mouse.
        /// </summary>
        public bool CanUserReorderColumns
        {
            get { return (bool)GetValue(CanUserReorderColumnsProperty); }
            set { SetValue(CanUserReorderColumnsProperty, value); }
        }

        /// <summary>
        /// Identifies the CanUserReorderColumns dependency property.
        /// </summary>
        public static readonly DependencyProperty CanUserReorderColumnsProperty =
            DependencyProperty.Register(
                "CanUserReorderColumns",
                typeof(bool),
                typeof(DataGrid),
                new PropertyMetadata(DATAGRID_defaultCanUserReorderColumns));

        /// <summary>
        /// Gets or sets a value indicating whether the user can adjust column widths using the mouse.
        /// </summary>
        public bool CanUserResizeColumns
        {
            get { return (bool)GetValue(CanUserResizeColumnsProperty); }
            set { SetValue(CanUserResizeColumnsProperty, value); }
        }

        /// <summary>
        /// Identifies the CanUserResizeColumns dependency property.
        /// </summary>
        public static readonly DependencyProperty CanUserResizeColumnsProperty =
            DependencyProperty.Register(
                "CanUserResizeColumns",
                typeof(bool),
                typeof(DataGrid),
                new PropertyMetadata(DATAGRID_defaultCanUserResizeColumns, OnCanUserResizeColumnsPropertyChanged));

        /// <summary>
        /// CanUserResizeColumns property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its CanUserResizeColumns.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnCanUserResizeColumnsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            dataGrid.EnsureHorizontalLayout();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can sort columns by clicking the column header.
        /// </summary>
        public bool CanUserSortColumns
        {
            get { return (bool)GetValue(CanUserSortColumnsProperty); }
            set { SetValue(CanUserSortColumnsProperty, value); }
        }

        /// <summary>
        /// Identifies the CanUserSortColumns dependency property.
        /// </summary>
        public static readonly DependencyProperty CanUserSortColumnsProperty =
            DependencyProperty.Register(
                "CanUserSortColumns",
                typeof(bool),
                typeof(DataGrid),
                new PropertyMetadata(DATAGRID_defaultCanUserSortColumns));

        /// <summary>
        /// Gets or sets the style that is used when rendering the data grid cells.
        /// </summary>
        public Style CellStyle
        {
            get { return GetValue(CellStyleProperty) as Style; }
            set { SetValue(CellStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="CellStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CellStyleProperty =
            DependencyProperty.Register(
                "CellStyle",
                typeof(Style),
                typeof(DataGrid),
                new PropertyMetadata(null, OnCellStylePropertyChanged));

        private static void OnCellStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (dataGrid != null)
            {
                Style previousStyle = e.OldValue as Style;
                foreach (DataGridRow row in dataGrid.GetAllRows())
                {
                    foreach (DataGridCell cell in row.Cells)
                    {
                        cell.EnsureStyle(previousStyle);
                    }

                    row.FillerCell.EnsureStyle(previousStyle);
                }

                dataGrid.InvalidateRowHeightEstimate();
            }
        }

        /// <summary>
        /// Gets or sets the property which determines how DataGrid content is copied to the Clipboard.
        /// </summary>
        public DataGridClipboardCopyMode ClipboardCopyMode
        {
            get { return (DataGridClipboardCopyMode)GetValue(ClipboardCopyModeProperty); }
            set { SetValue(ClipboardCopyModeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ClipboardCopyMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ClipboardCopyModeProperty =
            DependencyProperty.Register(
                "ClipboardCopyMode",
                typeof(DataGridClipboardCopyMode),
                typeof(DataGrid),
                new PropertyMetadata(DataGridClipboardCopyMode.ExcludeHeader));

        /// <summary>
        /// Gets or sets the height of the column headers row.
        /// </summary>
        public double ColumnHeaderHeight
        {
            get { return (double)GetValue(ColumnHeaderHeightProperty); }
            set { SetValue(ColumnHeaderHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the ColumnHeaderHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnHeaderHeightProperty =
            DependencyProperty.Register(
                "ColumnHeaderHeight",
                typeof(double),
                typeof(DataGrid),
                new PropertyMetadata(double.NaN, OnColumnHeaderHeightPropertyChanged));

        /// <summary>
        /// ColumnHeaderHeightProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its ColumnHeaderHeight.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnColumnHeaderHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (!dataGrid.IsHandlerSuspended(e.Property))
            {
                double value = (double)e.NewValue;
                if (value < DATAGRID_minimumColumnHeaderHeight)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "ColumnHeaderHeight", DATAGRID_minimumColumnHeaderHeight);
                }

                if (value > DATAGRID_maxHeadersThickness)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeLessThanOrEqualTo("value", "ColumnHeaderHeight", DATAGRID_maxHeadersThickness);
                }

                dataGrid.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets or sets the style that is used when rendering the column headers.
        /// </summary>
        public Style ColumnHeaderStyle
        {
            get { return GetValue(ColumnHeaderStyleProperty) as Style; }
            set { SetValue(ColumnHeaderStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the ColumnHeaderStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnHeaderStyleProperty =
            DependencyProperty.Register(
                "ColumnHeaderStyle",
                typeof(Style),
                typeof(DataGrid),
                new PropertyMetadata(null, OnColumnHeaderStylePropertyChanged));

        private static void OnColumnHeaderStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // TODO: ColumnHeaderStyle should be applied to the TopLeftCorner and the TopRightCorner as well
            DataGrid dataGrid = d as DataGrid;
            if (dataGrid != null)
            {
                Style previousStyle = e.OldValue as Style;
                foreach (DataGridColumn column in dataGrid.Columns)
                {
                    column.HeaderCell.EnsureStyle(previousStyle);
                }

                if (dataGrid.ColumnsInternal.FillerColumn != null)
                {
                    dataGrid.ColumnsInternal.FillerColumn.HeaderCell.EnsureStyle(previousStyle);
                }
            }
        }

        /// <summary>
        /// Gets or sets the standard width or automatic sizing mode of columns in the control.
        /// </summary>
        public DataGridLength ColumnWidth
        {
            get { return (DataGridLength)GetValue(ColumnWidthProperty); }
            set { SetValue(ColumnWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the ColumnWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnWidthProperty =
            DependencyProperty.Register(
                "ColumnWidth",
                typeof(DataGridLength),
                typeof(DataGrid),
                new PropertyMetadata(DataGridLength.Auto, OnColumnWidthPropertyChanged));

        /// <summary>
        /// ColumnWidthProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its ColumnWidth.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnColumnWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;

            foreach (DataGridColumn column in dataGrid.ColumnsInternal.GetDisplayedColumns())
            {
                if (column.InheritsWidth)
                {
                    column.SetWidthInternalNoCallback(dataGrid.ColumnWidth);
                }
            }

            dataGrid.EnsureHorizontalLayout();
        }

        /// <summary>
        /// Gets or sets the amount of data to fetch for virtualizing/prefetch operations.
        /// </summary>
        /// <returns>
        /// The amount of data to fetch per interval, in pages.
        /// </returns>
        public double DataFetchSize
        {
            get { return (double)GetValue(DataFetchSizeProperty); }
            set { SetValue(DataFetchSizeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DataFetchSize"/> dependency property
        /// </summary>
        public static readonly DependencyProperty DataFetchSizeProperty =
            DependencyProperty.Register(
                nameof(DataFetchSize),
                typeof(double),
                typeof(DataGrid),
                new PropertyMetadata(DATAGRID_defaultDataFetchSize, OnDataFetchSizePropertyChanged));

        /// <summary>
        /// DataFetchSizeProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its DataFetchSize.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnDataFetchSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (!dataGrid.IsHandlerSuspended(e.Property))
            {
                double oldValue = (double)e.OldValue;
                double newValue = (double)e.NewValue;

                if (double.IsNaN(newValue))
                {
                    dataGrid.SetValueNoCallback(e.Property, oldValue);
                    throw DataGridError.DataGrid.ValueCannotBeSetToNAN(nameof(dataGrid.DataFetchSize));
                }

                if (newValue < 0)
                {
                    dataGrid.SetValueNoCallback(e.Property, oldValue);
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", nameof(dataGrid.DataFetchSize), 0);
                }
            }
        }

        /// <summary>
        /// Gets or sets the style that is used when rendering the drag indicator
        /// that is displayed while dragging column headers.
        /// </summary>
        public Style DragIndicatorStyle
        {
            get { return GetValue(DragIndicatorStyleProperty) as Style; }
            set { SetValue(DragIndicatorStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DragIndicatorStyle"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty DragIndicatorStyleProperty =
            DependencyProperty.Register(
                "DragIndicatorStyle",
                typeof(Style),
                typeof(DataGrid),
                null);

        /// <summary>
        /// Gets or sets the style that is used when rendering the column headers.
        /// </summary>
        public Style DropLocationIndicatorStyle
        {
            get { return GetValue(DropLocationIndicatorStyleProperty) as Style; }
            set { SetValue(DropLocationIndicatorStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DropLocationIndicatorStyle"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty DropLocationIndicatorStyleProperty =
            DependencyProperty.Register(
                "DropLocationIndicatorStyle",
                typeof(Style),
                typeof(DataGrid),
                null);

        /// <summary>
        /// Gets or sets the number of columns that the user cannot scroll horizontally.
        /// </summary>
        public int FrozenColumnCount
        {
            get { return (int)GetValue(FrozenColumnCountProperty); }
            set { SetValue(FrozenColumnCountProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="FrozenColumnCount"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty FrozenColumnCountProperty =
            DependencyProperty.Register(
                "FrozenColumnCount",
                typeof(int),
                typeof(DataGrid),
                new PropertyMetadata(0, OnFrozenColumnCountPropertyChanged));

        private static void OnFrozenColumnCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (!dataGrid.IsHandlerSuspended(e.Property))
            {
                if ((int)e.NewValue < 0)
                {
                    dataGrid.SetValueNoCallback(DataGrid.FrozenColumnCountProperty, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "FrozenColumnCount", 0);
                }

                ProcessFrozenColumnCount(dataGrid);
            }
        }

        private static void ProcessFrozenColumnCount(DataGrid dataGrid)
        {
            dataGrid.CorrectColumnFrozenStates();
            dataGrid.ComputeScrollBarsLayout();

            dataGrid.InvalidateColumnHeadersArrange();
            dataGrid.InvalidateCellsArrange();
        }

        /// <summary>
        /// Gets or sets a value indicating which grid lines separating inner cells are shown.
        /// </summary>
        public DataGridGridLinesVisibility GridLinesVisibility
        {
            get { return (DataGridGridLinesVisibility)GetValue(GridLinesVisibilityProperty); }
            set { SetValue(GridLinesVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the GridLinesVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty GridLinesVisibilityProperty =
            DependencyProperty.Register(
                "GridLinesVisibility",
                typeof(DataGridGridLinesVisibility),
                typeof(DataGrid),
                new PropertyMetadata(DATAGRID_defaultGridLinesVisibility, OnGridLinesVisibilityPropertyChanged));

        /// <summary>
        /// GridLinesProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its GridLines.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnGridLinesVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            foreach (DataGridRow row in dataGrid.GetAllRows())
            {
                row.EnsureGridLines();
                row.InvalidateHorizontalArrange();
            }

            foreach (DataGridRowGroupHeader rowGroupHeader in dataGrid.GetAllRowGroupHeaders())
            {
                rowGroupHeader.EnsureGridLine();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the visibility of row and column headers.
        /// </summary>
        public DataGridHeadersVisibility HeadersVisibility
        {
            get { return (DataGridHeadersVisibility)GetValue(HeadersVisibilityProperty); }
            set { SetValue(HeadersVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the HeadersVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty HeadersVisibilityProperty =
            DependencyProperty.Register(
                "HeadersVisibility",
                typeof(DataGridHeadersVisibility),
                typeof(DataGrid),
                new PropertyMetadata(DATAGRID_defaultHeadersVisibility, OnHeadersVisibilityPropertyChanged));

        /// <summary>
        /// HeadersVisibilityProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its HeadersVisibility.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnHeadersVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            DataGridHeadersVisibility newValue = (DataGridHeadersVisibility)e.NewValue;
            DataGridHeadersVisibility oldValue = (DataGridHeadersVisibility)e.OldValue;

            Func<DataGridHeadersVisibility, DataGridHeadersVisibility, bool> hasFlags = (DataGridHeadersVisibility value, DataGridHeadersVisibility flags) => ((value & flags) == flags);

            bool newValueCols = hasFlags(newValue, DataGridHeadersVisibility.Column);
            bool newValueRows = hasFlags(newValue, DataGridHeadersVisibility.Row);
            bool oldValueCols = hasFlags(oldValue, DataGridHeadersVisibility.Column);
            bool oldValueRows = hasFlags(oldValue, DataGridHeadersVisibility.Row);

            // Columns
            if (newValueCols != oldValueCols)
            {
                if (dataGrid._columnHeadersPresenter != null)
                {
                    dataGrid.EnsureColumnHeadersVisibility();
                    if (!newValueCols)
                    {
                        dataGrid._columnHeadersPresenter.Measure(new Size(0.0, 0.0));
                    }
                    else
                    {
                        dataGrid.EnsureVerticalGridLines();
                    }

                    dataGrid.InvalidateMeasure();
                }
            }

            // Rows
            if (newValueRows != oldValueRows && dataGrid._rowsPresenter != null)
            {
                foreach (FrameworkElement element in dataGrid._rowsPresenter.Children)
                {
                    DataGridRow row = element as DataGridRow;
                    if (row != null)
                    {
                        row.EnsureHeaderStyleAndVisibility(null);
                        if (newValueRows)
                        {
                            row.ApplyState(false /*animate*/);
                            row.EnsureHeaderVisibility();
                        }
                    }
                    else
                    {
                        DataGridRowGroupHeader rowGroupHeader = element as DataGridRowGroupHeader;
                        if (rowGroupHeader != null)
                        {
                            rowGroupHeader.EnsureHeaderStyleAndVisibility(null);
                        }
                    }
                }

                dataGrid.InvalidateRowHeightEstimate();
                dataGrid.InvalidateRowsMeasure(true /*invalidateIndividualElements*/);
            }

            // TODO: This isn't necessary if the TopLeftCorner and the TopRightCorner Autosize to 0.
            // See if their templates can be changed to do that.
            if (dataGrid._topLeftCornerHeader != null)
            {
                dataGrid._topLeftCornerHeader.Visibility = (newValueRows && newValueCols) ? Visibility.Visible : Visibility.Collapsed;
                if (dataGrid._topLeftCornerHeader.Visibility == Visibility.Collapsed)
                {
                    dataGrid._topLeftCornerHeader.Measure(new Size(0.0, 0.0));
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Windows.Media.Brush"/> that is used to paint grid lines separating rows.
        /// </summary>
        public Brush HorizontalGridLinesBrush
        {
            get { return GetValue(HorizontalGridLinesBrushProperty) as Brush; }
            set { SetValue(HorizontalGridLinesBrushProperty, value); }
        }

        /// <summary>
        /// Identifies the HorizontalGridLinesBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalGridLinesBrushProperty =
            DependencyProperty.Register(
                "HorizontalGridLinesBrush",
                typeof(Brush),
                typeof(DataGrid),
                new PropertyMetadata(null, OnHorizontalGridLinesBrushPropertyChanged));

        /// <summary>
        /// HorizontalGridLinesBrushProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its HorizontalGridLinesBrush.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnHorizontalGridLinesBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (!dataGrid.IsHandlerSuspended(e.Property) && dataGrid._rowsPresenter != null)
            {
                foreach (DataGridRow row in dataGrid.GetAllRows())
                {
                    row.EnsureGridLines();
                }

                foreach (DataGridRowGroupHeader rowGroupHeader in dataGrid.GetAllRowGroupHeaders())
                {
                    rowGroupHeader.EnsureGridLine();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating how the horizontal scroll bar is displayed.
        /// </summary>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the HorizontalScrollBarVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
            DependencyProperty.Register(
                "HorizontalScrollBarVisibility",
                typeof(ScrollBarVisibility),
                typeof(DataGrid),
                new PropertyMetadata(DATAGRID_defaultScrollBarVisibility, OnHorizontalScrollBarVisibilityPropertyChanged));

        /// <summary>
        /// HorizontalScrollBarVisibilityProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its HorizontalScrollBarVisibility.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnHorizontalScrollBarVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (!dataGrid.IsHandlerSuspended(e.Property) && (ScrollBarVisibility)e.NewValue != (ScrollBarVisibility)e.OldValue)
            {
                dataGrid.UpdateRowsPresenterManipulationMode(true /*horizontalMode*/, false /*verticalMode*/);

                if (dataGrid._hScrollBar != null)
                {
                    if (dataGrid.IsHorizontalScrollBarOverCells)
                    {
                        dataGrid.ComputeScrollBarsLayout();
                    }
                    else
                    {
                        dataGrid.InvalidateMeasure();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can edit the values in the control.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Identifies the IsReadOnly dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                "IsReadOnly",
                typeof(bool),
                typeof(DataGrid),
                new PropertyMetadata(false, OnIsReadOnlyPropertyChanged));

        /// <summary>
        /// IsReadOnlyProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its IsReadOnly.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnIsReadOnlyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (!dataGrid.IsHandlerSuspended(e.Property))
            {
                bool value = (bool)e.NewValue;
                if (value && !dataGrid.CommitEdit(DataGridEditingUnit.Row, true /*exitEditing*/))
                {
                    dataGrid.CancelEdit(DataGridEditingUnit.Row, false /*raiseEvents*/);
                }

#if FEATURE_IEDITABLECOLLECTIONVIEW
                dataGrid.UpdateNewItemPlaceholder();
#endif
            }
        }

        /// <summary>
        /// Gets a value indicating whether data in the grid is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return (bool)GetValue(IsValidProperty);
            }

            internal set
            {
                if (value != this.IsValid)
                {
                    if (value)
                    {
                        VisualStates.GoToState(this, true, VisualStates.StateValid);
                    }
                    else
                    {
                        VisualStates.GoToState(this, true, VisualStates.StateInvalid, VisualStates.StateValid);
                    }

                    this.SetValueNoCallback(DataGrid.IsValidProperty, value);
                }
            }
        }

        /// <summary>
        /// Identifies the IsValid dependency property.
        /// </summary>
        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register(
                "IsValid",
                typeof(bool),
                typeof(DataGrid),
                new PropertyMetadata(true, OnIsValidPropertyChanged));

        /// <summary>
        /// IsValidProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its IsValid.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnIsValidPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (!dataGrid.IsHandlerSuspended(e.Property))
            {
                dataGrid.SetValueNoCallback(DataGrid.IsValidProperty, e.OldValue);
                throw DataGridError.DataGrid.UnderlyingPropertyIsReadOnly("IsValid");
            }
        }

        /// <summary>
        /// Gets or sets the threshold range that governs when the DataGrid class will begin to prefetch more items.
        /// </summary>
        /// <returns>
        /// The loading threshold, in terms of pages.
        /// </returns>
        public double IncrementalLoadingThreshold
        {
            get { return (double)GetValue(IncrementalLoadingThresholdProperty); }
            set { SetValue(IncrementalLoadingThresholdProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IncrementalLoadingThreshold"/> dependency property
        /// </summary>
        public static readonly DependencyProperty IncrementalLoadingThresholdProperty =
            DependencyProperty.Register(
                nameof(IncrementalLoadingThreshold),
                typeof(double),
                typeof(DataGrid),
                new PropertyMetadata(DATAGRID_defaultIncrementalLoadingThreshold, OnIncrementalLoadingThresholdPropertyChanged));

        /// <summary>
        /// IncrementalLoadingThresholdProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its IncrementalLoadingThreshold.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnIncrementalLoadingThresholdPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (!dataGrid.IsHandlerSuspended(e.Property))
            {
                double oldValue = (double)e.OldValue;
                double newValue = (double)e.NewValue;

                if (double.IsNaN(newValue))
                {
                    dataGrid.SetValueNoCallback(e.Property, oldValue);
                    throw DataGridError.DataGrid.ValueCannotBeSetToNAN(nameof(dataGrid.IncrementalLoadingThreshold));
                }

                if (newValue < 0)
                {
                    dataGrid.SetValueNoCallback(e.Property, oldValue);
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", nameof(dataGrid.IncrementalLoadingThreshold), 0);
                }

                if (newValue > oldValue)
                {
                    dataGrid.LoadMoreDataFromIncrementalItemsSource();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates the conditions for prefetch operations by the DataGrid class.
        /// </summary>
        /// <returns>
        /// An enumeration value that indicates the conditions that trigger prefetch operations. The default is **Edge**.
        /// </returns>
        public IncrementalLoadingTrigger IncrementalLoadingTrigger
        {
            get { return (IncrementalLoadingTrigger)GetValue(IncrementalLoadingTriggerProperty); }
            set { SetValue(IncrementalLoadingTriggerProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IncrementalLoadingTrigger"/> dependency property
        /// </summary>
        public static readonly DependencyProperty IncrementalLoadingTriggerProperty =
            DependencyProperty.Register(
                nameof(IncrementalLoadingTrigger),
                typeof(IncrementalLoadingTrigger),
                typeof(DataGrid),
                new PropertyMetadata(IncrementalLoadingTrigger.Edge));

        /// <summary>
        /// Gets or sets a collection that is used to generate the content of the control.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return GetValue(ItemsSourceProperty) as IEnumerable; }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ItemsSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(DataGrid),
                new PropertyMetadata(null, OnItemsSourcePropertyChanged));

        /// <summary>
        /// ItemsSourceProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its ItemsSource.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (!dataGrid.IsHandlerSuspended(e.Property))
            {
                DiagnosticsDebug.Assert(dataGrid.DataConnection != null, "Expected non-null DataConnection.");

                if (dataGrid.LoadingOrUnloadingRow)
                {
                    dataGrid.SetValueNoCallback(DataGrid.ItemsSourceProperty, e.OldValue);
                    throw DataGridError.DataGrid.CannotChangeItemsWhenLoadingRows();
                }

                // Try to commit edit on the old DataSource, but force a cancel if it fails.
                if (!dataGrid.CommitEdit())
                {
                    dataGrid.CancelEdit(DataGridEditingUnit.Row, false);
                }

                dataGrid.DataConnection.UnWireEvents(dataGrid.DataConnection.DataSource);
                dataGrid.DataConnection.ClearDataProperties();

                dataGrid.ClearRowGroupHeadersTable();

                // The old selected indexes are no longer relevant. There's a perf benefit from
                // updating the selected indexes with a null DataSource, because we know that all
                // of the previously selected indexes have been removed from selection.
                dataGrid.DataConnection.DataSource = null;
                dataGrid._selectedItems.UpdateIndexes();
                dataGrid.CoerceSelectedItem();

                // Wrap an IEnumerable in an ICollectionView if it's not already one.
                bool setDefaultSelection = false;
                IEnumerable newItemsSource = e.NewValue as IEnumerable;
                if (newItemsSource != null && !(newItemsSource is ICollectionView))
                {
                    dataGrid.DataConnection.DataSource = DataGridDataConnection.CreateView(newItemsSource);
                }
                else
                {
                    dataGrid.DataConnection.DataSource = newItemsSource;
                    setDefaultSelection = true;
                }

                if (dataGrid.DataConnection.DataSource != null)
                {
                    // Setup the column headers.
                    if (dataGrid.DataConnection.DataType != null)
                    {
                        foreach (DataGridBoundColumn boundColumn in dataGrid.ColumnsInternal.GetDisplayedColumns(column => column is DataGridBoundColumn))
                        {
                            boundColumn.SetHeaderFromBinding();
                        }
                    }

                    dataGrid.DataConnection.WireEvents(dataGrid.DataConnection.DataSource);
                }

                // Wait for the current cell to be set before we raise any SelectionChanged events.
                dataGrid._makeFirstDisplayedCellCurrentCellPending = true;

                // Clear out the old rows and remove the generated columns.
                dataGrid.ClearRows(false /*recycle*/);
                dataGrid.RemoveAutoGeneratedColumns();

                // Set the SlotCount (from the data count and number of row group headers) before we make the default selection.
                dataGrid.PopulateRowGroupHeadersTable();
                dataGrid.RefreshSlotCounts();

                dataGrid.SelectedItem = null;
                if (dataGrid.DataConnection.CollectionView != null && setDefaultSelection)
                {
                    dataGrid.SelectedItem = dataGrid.DataConnection.CollectionView.CurrentItem;
                }

                // Treat this like the DataGrid has never been measured because all calculations at
                // this point are invalid until the next layout cycle.  For instance, the ItemsSource
                // can be set when the DataGrid is not part of the visual tree.
                dataGrid._measured = false;
                dataGrid.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets or sets the maximum width of columns in the <see cref="DataGrid"/>.
        /// </summary>
        public double MaxColumnWidth
        {
            get { return (double)GetValue(MaxColumnWidthProperty); }
            set { SetValue(MaxColumnWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the MaxColumnWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxColumnWidthProperty =
            DependencyProperty.Register(
                "MaxColumnWidth",
                typeof(double),
                typeof(DataGrid),
                new PropertyMetadata(DATAGRID_defaultMaxColumnWidth, OnMaxColumnWidthPropertyChanged));

        /// <summary>
        /// MaxColumnWidthProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its ColumnWidth.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnMaxColumnWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (!dataGrid.IsHandlerSuspended(e.Property))
            {
                double oldValue = (double)e.OldValue;
                double newValue = (double)e.NewValue;

                if (double.IsNaN(newValue))
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueCannotBeSetToNAN("MaxColumnWidth");
                }

                if (newValue < 0)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "MaxColumnWidth", 0);
                }

                if (dataGrid.MinColumnWidth > newValue)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "MaxColumnWidth", "MinColumnWidth");
                }

                foreach (DataGridColumn column in dataGrid.ColumnsInternal.GetDisplayedColumns())
                {
                    dataGrid.OnColumnMaxWidthChanged(column, Math.Min(column.MaxWidth, oldValue));
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum width of columns in the <see cref="DataGrid"/>.
        /// </summary>
        public double MinColumnWidth
        {
            get { return (double)GetValue(MinColumnWidthProperty); }
            set { SetValue(MinColumnWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the MinColumnWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty MinColumnWidthProperty =
            DependencyProperty.Register(
                "MinColumnWidth",
                typeof(double),
                typeof(DataGrid),
                new PropertyMetadata(DATAGRID_defaultMinColumnWidth, OnMinColumnWidthPropertyChanged));

        /// <summary>
        /// MinColumnWidthProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its ColumnWidth.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnMinColumnWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (!dataGrid.IsHandlerSuspended(e.Property))
            {
                double oldValue = (double)e.OldValue;
                double newValue = (double)e.NewValue;

                if (double.IsNaN(newValue))
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueCannotBeSetToNAN("MinColumnWidth");
                }

                if (newValue < 0)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "MinColumnWidth", 0);
                }

                if (double.IsPositiveInfinity(newValue))
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueCannotBeSetToInfinity("MinColumnWidth");
                }

                if (dataGrid.MaxColumnWidth < newValue)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeLessThanOrEqualTo("value", "MinColumnWidth", "MaxColumnWidth");
                }

                foreach (DataGridColumn column in dataGrid.ColumnsInternal.GetDisplayedColumns())
                {
                    dataGrid.OnColumnMinWidthChanged(column, Math.Max(column.MinWidth, oldValue));
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Windows.Media.Brush"/> that is used to paint row backgrounds.
        /// </summary>
        public Brush RowBackground
        {
            get { return GetValue(RowBackgroundProperty) as Brush; }
            set { SetValue(RowBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="RowBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowBackgroundProperty =
            DependencyProperty.Register(
                "RowBackground",
                typeof(Brush),
                typeof(DataGrid),
                new PropertyMetadata(null, OnRowBackgroundPropertyChanged));

        private static void OnRowBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;

            // Go through the Displayed rows and update the background
            foreach (DataGridRow row in dataGrid.GetAllRows())
            {
                row.EnsureBackground();
            }
        }

        /// <summary>
        /// Gets or sets the template that is used to display the content of the details section of rows.
        /// </summary>
        public DataTemplate RowDetailsTemplate
        {
            get { return GetValue(RowDetailsTemplateProperty) as DataTemplate; }
            set { SetValue(RowDetailsTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the RowDetailsTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty RowDetailsTemplateProperty =
            DependencyProperty.Register(
                "RowDetailsTemplate",
                typeof(DataTemplate),
                typeof(DataGrid),
                new PropertyMetadata(null, OnRowDetailsTemplatePropertyChanged));

        private static void OnRowDetailsTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;

            // Update the RowDetails templates if necessary
            if (dataGrid._rowsPresenter != null)
            {
                foreach (DataGridRow row in dataGrid.GetAllRows())
                {
                    if (dataGrid.GetRowDetailsVisibility(row.Index) == Visibility.Visible)
                    {
                        // DetailsPreferredHeight is initialized when the DetailsElement's size changes.
                        row.ApplyDetailsTemplate(false /*initializeDetailsPreferredHeight*/);
                    }
                }
            }

            dataGrid.UpdateRowDetailsHeightEstimate();
            dataGrid.InvalidateMeasure();
        }

        /// <summary>
        /// Gets or sets a value indicating when the details sections of rows are displayed.
        /// </summary>
        public DataGridRowDetailsVisibilityMode RowDetailsVisibilityMode
        {
            get { return (DataGridRowDetailsVisibilityMode)GetValue(RowDetailsVisibilityModeProperty); }
            set { SetValue(RowDetailsVisibilityModeProperty, value); }
        }

        /// <summary>
        /// Identifies the RowDetailsVisibilityMode dependency property.
        /// </summary>
        public static readonly DependencyProperty RowDetailsVisibilityModeProperty =
            DependencyProperty.Register(
                "RowDetailsVisibilityMode",
                typeof(DataGridRowDetailsVisibilityMode),
                typeof(DataGrid),
                new PropertyMetadata(DATAGRID_defaultRowDetailsVisibility, OnRowDetailsVisibilityModePropertyChanged));

        /// <summary>
        /// RowDetailsVisibilityModeProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its RowDetailsVisibilityMode.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnRowDetailsVisibilityModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            dataGrid.UpdateRowDetailsVisibilityMode((DataGridRowDetailsVisibilityMode)e.NewValue);
        }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Windows.Media.Brush"/> that is used as the default cells foreground.
        /// </summary>
        public Brush RowForeground
        {
            get { return GetValue(RowForegroundProperty) as Brush; }
            set { SetValue(RowForegroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="RowForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowForegroundProperty =
            DependencyProperty.Register(
                "RowForeground",
                typeof(Brush),
                typeof(DataGrid),
                new PropertyMetadata(null, OnRowForegroundPropertyChanged));

        private static void OnRowForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;

            // Go through the Displayed rows and update the foreground
            foreach (DataGridRow row in dataGrid.GetAllRows())
            {
                row.EnsureForeground();
            }
        }

        /// <summary>
        /// Gets or sets the standard height of rows in the control.
        /// </summary>
        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the RowHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register(
                "RowHeight",
                typeof(double),
                typeof(DataGrid),
                new PropertyMetadata(double.NaN, OnRowHeightPropertyChanged));

        /// <summary>
        /// RowHeightProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its RowHeight.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnRowHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;

            if (!dataGrid.IsHandlerSuspended(e.Property))
            {
                double value = (double)e.NewValue;

                if (value < DataGridRow.DATAGRIDROW_minimumHeight)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "RowHeight", 0);
                }

                if (value > DataGridRow.DATAGRIDROW_maximumHeight)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeLessThanOrEqualTo("value", "RowHeight", DataGridRow.DATAGRIDROW_maximumHeight);
                }

                dataGrid.InvalidateRowHeightEstimate();

                // Re-measure all the rows due to the Height change
                dataGrid.InvalidateRowsMeasure(true);

                // DataGrid needs to update the layout information and the ScrollBars
                dataGrid.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets or sets the width of the row header column.
        /// </summary>
        public double RowHeaderWidth
        {
            get { return (double)GetValue(RowHeaderWidthProperty); }
            set { SetValue(RowHeaderWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the RowHeaderWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty RowHeaderWidthProperty =
            DependencyProperty.Register(
                "RowHeaderWidth",
                typeof(double),
                typeof(DataGrid),
                new PropertyMetadata(double.NaN, OnRowHeaderWidthPropertyChanged));

        /// <summary>
        /// RowHeaderWidthProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its RowHeaderWidth.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnRowHeaderWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (!dataGrid.IsHandlerSuspended(e.Property))
            {
                double value = (double)e.NewValue;

                if (value < DATAGRID_minimumRowHeaderWidth)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "RowHeaderWidth", DATAGRID_minimumRowHeaderWidth);
                }

                if (value > DATAGRID_maxHeadersThickness)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeLessThanOrEqualTo("value", "RowHeaderWidth", DATAGRID_maxHeadersThickness);
                }

                dataGrid.EnsureRowHeaderWidth();
            }
        }

        /// <summary>
        /// Gets or sets the style that is used when rendering the row headers.
        /// </summary>
        public Style RowHeaderStyle
        {
            get { return GetValue(RowHeaderStyleProperty) as Style; }
            set { SetValue(RowHeaderStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="RowHeaderStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowHeaderStyleProperty =
            DependencyProperty.Register(
                "RowHeaderStyle",
                typeof(Style),
                typeof(DataGrid),
                new PropertyMetadata(null, OnRowHeaderStylePropertyChanged));

        private static void OnRowHeaderStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (dataGrid != null && dataGrid._rowsPresenter != null)
            {
                // Set HeaderStyle for displayed rows
                Style previousStyle = e.OldValue as Style;
                foreach (UIElement element in dataGrid._rowsPresenter.Children)
                {
                    DataGridRow row = element as DataGridRow;
                    if (row != null)
                    {
                        row.EnsureHeaderStyleAndVisibility(previousStyle);
                    }
                    else
                    {
                        DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                        if (groupHeader != null)
                        {
                            groupHeader.EnsureHeaderStyleAndVisibility(previousStyle);
                        }
                    }
                }

                dataGrid.InvalidateRowHeightEstimate();
            }
        }

        /// <summary>
        /// Gets or sets the style that is used when rendering the rows.
        /// </summary>
        public Style RowStyle
        {
            get { return GetValue(RowStyleProperty) as Style; }
            set { SetValue(RowStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="RowStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowStyleProperty =
            DependencyProperty.Register(
                "RowStyle",
                typeof(Style),
                typeof(DataGrid),
                new PropertyMetadata(null, OnRowStylePropertyChanged));

        private static void OnRowStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (dataGrid != null)
            {
                if (dataGrid._rowsPresenter != null)
                {
                    // Set the style for displayed rows if it has not already been set
                    foreach (DataGridRow row in dataGrid.GetAllRows())
                    {
                        EnsureElementStyle(row, e.OldValue as Style, e.NewValue as Style);
                    }
                }

                dataGrid.InvalidateRowHeightEstimate();
            }
        }

        /// <summary>
        /// Gets or sets the selection behavior of the data grid.
        /// </summary>
        public DataGridSelectionMode SelectionMode
        {
            get { return (DataGridSelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectionMode dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(
                "SelectionMode",
                typeof(DataGridSelectionMode),
                typeof(DataGrid),
                new PropertyMetadata(DATAGRID_defaultSelectionMode, OnSelectionModePropertyChanged));

        /// <summary>
        /// SelectionModeProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its SelectionMode.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (!dataGrid.IsHandlerSuspended(e.Property))
            {
                dataGrid.ClearRowSelection(true /*resetAnchorSlot*/);
            }
        }

        /// <summary>
        /// Gets or sets the index of the current selection.
        /// </summary>
        /// <returns>The index of the current selection, or -1 if the selection is empty.</returns>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedIndex dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                "SelectedIndex",
                typeof(int),
                typeof(DataGrid),
                new PropertyMetadata(-1, OnSelectedIndexPropertyChanged));

        /// <summary>
        /// SelectedIndexProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its SelectedIndex.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectedIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (!dataGrid.IsHandlerSuspended(e.Property))
            {
                int index = (int)e.NewValue;

                // GetDataItem returns null if index is >= Count, we do not check newValue
                // against Count here to avoid enumerating through an Enumerable twice
                // Setting SelectedItem coerces the finally value of the SelectedIndex
                object newSelectedItem = (index < 0) ? null : dataGrid.DataConnection.GetDataItem(index);
                dataGrid.SelectedItem = newSelectedItem;
                if (dataGrid.SelectedItem != newSelectedItem)
                {
                    d.SetValueNoCallback(e.Property, e.OldValue);
                }
            }
        }

        /// <summary>
        /// Gets or sets the data item corresponding to the selected row.
        /// </summary>
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty) as object; }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedItem dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                "SelectedItem",
                typeof(object),
                typeof(DataGrid),
                new PropertyMetadata(null, OnSelectedItemPropertyChanged));

        /// <summary>
        /// SelectedItemProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its SelectedItem.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;

            if (!dataGrid.IsHandlerSuspended(e.Property))
            {
                int rowIndex = (e.NewValue == null) ? -1 : dataGrid.DataConnection.IndexOf(e.NewValue);
                if (rowIndex == -1)
                {
                    // If the Item is null or it's not found, clear the Selection
                    if (!dataGrid.CommitEdit(DataGridEditingUnit.Row, true /*exitEditing*/))
                    {
                        // Edited value couldn't be committed or aborted
                        d.SetValueNoCallback(e.Property, e.OldValue);
                        return;
                    }

                    // Clear all row selections
                    dataGrid.ClearRowSelection(true /*resetAnchorSlot*/);
                }
                else
                {
                    int slot = dataGrid.SlotFromRowIndex(rowIndex);
                    if (slot != dataGrid.CurrentSlot)
                    {
                        if (!dataGrid.CommitEdit(DataGridEditingUnit.Row, true /*exitEditing*/))
                        {
                            // Edited value couldn't be committed or aborted
                            d.SetValueNoCallback(e.Property, e.OldValue);
                            return;
                        }

                        if (slot >= dataGrid.SlotCount || slot < -1)
                        {
                            if (dataGrid.DataConnection.CollectionView != null)
                            {
                                dataGrid.DataConnection.CollectionView.MoveCurrentToPosition(rowIndex);
                            }
                        }
                    }

                    int oldSelectedIndex = dataGrid.SelectedIndex;
                    if (oldSelectedIndex != rowIndex)
                    {
                        dataGrid.SetValueNoCallback(DataGrid.SelectedIndexProperty, rowIndex);
                    }

                    try
                    {
                        dataGrid._noSelectionChangeCount++;
                        int columnIndex = dataGrid.CurrentColumnIndex;

                        if (columnIndex == -1)
                        {
                            columnIndex = dataGrid.FirstDisplayedNonFillerColumnIndex;
                        }

                        if (dataGrid.IsSlotOutOfSelectionBounds(slot))
                        {
                            dataGrid.ClearRowSelection(slot /*slotException*/, true /*resetAnchorSlot*/);
                            return;
                        }

                        dataGrid.UpdateSelectionAndCurrency(columnIndex, slot, DataGridSelectionAction.SelectCurrent, false /*scrollIntoView*/);
                    }
                    finally
                    {
                        dataGrid.NoSelectionChangeCount--;
                    }

                    if (!dataGrid._successfullyUpdatedSelection)
                    {
                        dataGrid.SetValueNoCallback(DataGrid.SelectedIndexProperty, oldSelectedIndex);
                        d.SetValueNoCallback(e.Property, e.OldValue);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Windows.Media.Brush"/> that is used to paint grid lines separating columns.
        /// </summary>
        public Brush VerticalGridLinesBrush
        {
            get { return GetValue(VerticalGridLinesBrushProperty) as Brush; }
            set { SetValue(VerticalGridLinesBrushProperty, value); }
        }

        /// <summary>
        /// Identifies the VerticalGridLinesBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalGridLinesBrushProperty =
            DependencyProperty.Register(
                "VerticalGridLinesBrush",
                typeof(Brush),
                typeof(DataGrid),
                new PropertyMetadata(null, OnVerticalGridLinesBrushPropertyChanged));

        /// <summary>
        /// VerticalGridLinesBrushProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its VerticalGridLinesBrush.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnVerticalGridLinesBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (dataGrid._rowsPresenter != null)
            {
                foreach (DataGridRow row in dataGrid.GetAllRows())
                {
                    row.EnsureGridLines();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating how the vertical scroll bar is displayed.
        /// </summary>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the VerticalScrollBarVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.Register(
                "VerticalScrollBarVisibility",
                typeof(ScrollBarVisibility),
                typeof(DataGrid),
                new PropertyMetadata(DATAGRID_defaultScrollBarVisibility, OnVerticalScrollBarVisibilityPropertyChanged));

        /// <summary>
        /// VerticalScrollBarVisibilityProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its VerticalScrollBarVisibility.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnVerticalScrollBarVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (!dataGrid.IsHandlerSuspended(e.Property) && (ScrollBarVisibility)e.NewValue != (ScrollBarVisibility)e.OldValue)
            {
                dataGrid.UpdateRowsPresenterManipulationMode(false /*horizontalMode*/, true /*verticalMode*/);

                if (dataGrid._vScrollBar != null)
                {
                    if (dataGrid.IsVerticalScrollBarOverCells)
                    {
                        dataGrid.ComputeScrollBarsLayout();
                    }
                    else
                    {
                        dataGrid.InvalidateMeasure();
                    }
                }
            }
        }

        /// <summary>
        /// Gets a collection that contains all the columns in the control.
        /// </summary>
        public ObservableCollection<DataGridColumn> Columns
        {
            get
            {
                // we use a backing field here because the field's type
                // is a subclass of the property's
                return this.ColumnsInternal;
            }
        }

        /// <summary>
        /// Gets or sets the column that contains the current cell.
        /// </summary>
        public DataGridColumn CurrentColumn
        {
            get
            {
                if (this.CurrentColumnIndex == -1)
                {
                    return null;
                }

                DiagnosticsDebug.Assert(this.CurrentColumnIndex < this.ColumnsItemsInternal.Count, "Expected CurrentColumnIndex smaller than ColumnsItemsInternal.Count.");
                return this.ColumnsItemsInternal[this.CurrentColumnIndex];
            }

            set
            {
                DataGridColumn dataGridColumn = value;
                if (dataGridColumn == null)
                {
                    throw DataGridError.DataGrid.ValueCannotBeSetToNull("value", "CurrentColumn");
                }

                if (this.CurrentColumn != dataGridColumn)
                {
                    if (dataGridColumn.OwningGrid != this)
                    {
                        // Provided column does not belong to this DataGrid
                        throw DataGridError.DataGrid.ColumnNotInThisDataGrid();
                    }

                    if (dataGridColumn.Visibility == Visibility.Collapsed)
                    {
                        // CurrentColumn cannot be set to an invisible column
                        throw DataGridError.DataGrid.ColumnCannotBeCollapsed();
                    }

                    if (this.CurrentSlot == -1)
                    {
                        // There is no current row so the current column cannot be set
                        throw DataGridError.DataGrid.NoCurrentRow();
                    }

                    bool beginEdit = _editingColumnIndex != -1;
                    if (!EndCellEdit(DataGridEditAction.Commit, true /*exitEditingMode*/, this.ContainsFocus /*keepFocus*/, true /*raiseEvents*/))
                    {
                        // Edited value couldn't be committed or aborted
                        return;
                    }

                    if (_noFocusedColumnChangeCount == 0)
                    {
                        this.ColumnHeaderHasFocus = false;
                    }

                    this.UpdateSelectionAndCurrency(dataGridColumn.Index, this.CurrentSlot, DataGridSelectionAction.None, false /*scrollIntoView*/);
                    DiagnosticsDebug.Assert(_successfullyUpdatedSelection, "Expected _successfullyUpdatedSelection is true.");
                    if (beginEdit &&
                        _editingColumnIndex == -1 &&
                        this.CurrentSlot != -1 &&
                        this.CurrentColumnIndex != -1 &&
                        this.CurrentColumnIndex == dataGridColumn.Index &&
                        dataGridColumn.OwningGrid == this &&
                        !GetColumnEffectiveReadOnlyState(dataGridColumn))
                    {
                        // Returning to editing mode since the grid was in that mode prior to the EndCellEdit call above.
                        BeginCellEdit(new RoutedEventArgs());
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the label to display in a DataGridRowGroupHeader when its PropertyName is not set.
        /// </summary>
        public string RowGroupHeaderPropertyNameAlternative
        {
            get
            {
                return _rowGroupHeaderPropertyNameAlternative;
            }

            set
            {
                _rowGroupHeaderPropertyNameAlternative = value;
            }
        }

        /// <summary>
        /// Gets the style that is used when rendering the row group header.
        /// </summary>
        public ObservableCollection<Style> RowGroupHeaderStyles
        {
            get
            {
                return _rowGroupHeaderStyles;
            }
        }

        /// <summary>
        /// Gets a list that contains the data items corresponding to the selected rows.
        /// </summary>
        public IList SelectedItems
        {
            get { return _selectedItems as IList; }
        }

        /// <summary>
        /// Gets the data item bound to the row that contains the current cell.
        /// </summary>
        protected object CurrentItem
        {
            get
            {
                if (this.CurrentSlot == -1 ||
                    this.RowGroupHeadersTable.Contains(this.CurrentSlot) ||
                    this.ItemsSource /*this.DataConnection.DataSource*/ == null)
                {
                    return null;
                }

                return this.DataConnection.GetDataItem(RowIndexFromSlot(this.CurrentSlot));
            }
        }

        internal static double HorizontalGridLinesThickness
        {
            get
            {
                return DATAGRID_horizontalGridLinesThickness;
            }
        }

        internal int AnchorSlot
        {
            get;
            private set;
        }

        internal double ActualRowHeaderWidth
        {
            get
            {
                if (!this.AreRowHeadersVisible)
                {
                    return 0;
                }
                else
                {
                    return !double.IsNaN(this.RowHeaderWidth) ? this.RowHeaderWidth : this.RowHeadersDesiredWidth;
                }
            }
        }

        internal double ActualRowsPresenterHeight
        {
            get
            {
                if (_rowsPresenter != null)
                {
                    return _rowsPresenter.ActualHeight;
                }

                return 0;
            }
        }

        internal bool AllowsManipulation
        {
            get
            {
                return _rowsPresenter != null &&
                    (_rowsPresenter.ManipulationMode & (ManipulationModes.TranslateX | ManipulationModes.TranslateY)) != ManipulationModes.None;
            }
        }

        internal bool AreColumnHeadersVisible
        {
            get
            {
                return (this.HeadersVisibility & DataGridHeadersVisibility.Column) == DataGridHeadersVisibility.Column;
            }
        }

        internal bool AreRowHeadersVisible
        {
            get
            {
                return (this.HeadersVisibility & DataGridHeadersVisibility.Row) == DataGridHeadersVisibility.Row;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not at least one auto-sizing column is waiting for all the rows
        /// to be measured before its final width is determined.
        /// </summary>
        internal bool AutoSizingColumns
        {
            get
            {
                return _autoSizingColumns;
            }

            set
            {
                if (_autoSizingColumns && value == false && this.ColumnsInternal != null)
                {
                    double adjustment = this.CellsWidth - this.ColumnsInternal.VisibleEdgedColumnsWidth;
                    this.AdjustColumnWidths(0, adjustment, false);
                    foreach (DataGridColumn column in this.ColumnsInternal.GetVisibleColumns())
                    {
                        column.IsInitialDesiredWidthDetermined = true;
                    }

                    this.ColumnsInternal.EnsureVisibleEdgedColumnsWidth();
                    this.ComputeScrollBarsLayout();
                    InvalidateColumnHeadersMeasure();
                    InvalidateRowsMeasure(true);
                }

                _autoSizingColumns = value;
            }
        }

        internal double AvailableSlotElementRoom
        {
            get;
            set;
        }

        // Height currently available for cells this value is smaller.  This height is reduced by the existence of ColumnHeaders
        // or a horizontal scrollbar.  Layout is asynchronous so changes to the ColumnHeaders or the horizontal scrollbar are
        // not reflected immediately.
        internal double CellsHeight
        {
            get
            {
                return this.RowsPresenterAvailableSize.HasValue ? this.RowsPresenterAvailableSize.Value.Height : 0;
            }
        }

        // Width currently available for cells this value is smaller.  This width is reduced by the existence of RowHeaders
        // or a vertical scrollbar.  Layout is asynchronous so changes to the RowHeaders or the vertical scrollbar are
        // not reflected immediately
        internal double CellsWidth
        {
            get
            {
                double rowsWidth = double.PositiveInfinity;
                if (this.RowsPresenterAvailableSize.HasValue)
                {
                    rowsWidth = Math.Max(0, this.RowsPresenterAvailableSize.Value.Width - this.ActualRowHeaderWidth);
                }

                return double.IsPositiveInfinity(rowsWidth) ? this.ColumnsInternal.VisibleEdgedColumnsWidth : rowsWidth;
            }
        }

        /// <summary>
        /// Gets an empty content control that's used during the DataGrid's copy procedure
        /// to determine the value of a ClipboardContentBinding for a particular column and item.
        /// </summary>
        internal ContentControl ClipboardContentControl
        {
            get
            {
                if (_clipboardContentControl == null)
                {
                    _clipboardContentControl = new ContentControl();
                }

                return _clipboardContentControl;
            }
        }

        internal bool ColumnHeaderHasFocus
        {
            get
            {
                return _columnHeaderHasFocus;
            }

            set
            {
                DiagnosticsDebug.Assert(!value || (this.ColumnHeaders != null && this.AreColumnHeadersVisible), "Expected value==False || (non-null ColumnHeaders and AreColumnHeadersVisible==True)");

                if (_columnHeaderHasFocus != value)
                {
                    _columnHeaderHasFocus = value;

                    if (this.CurrentColumn != null && this.IsSlotVisible(this.CurrentSlot))
                    {
                        UpdateCurrentState(this.DisplayData.GetDisplayedElement(this.CurrentSlot), this.CurrentColumnIndex, true /*applyCellState*/);
                    }

                    DataGridColumn oldFocusedColumn = this.FocusedColumn;
                    this.FocusedColumn = null;

                    if (_columnHeaderHasFocus)
                    {
                        this.FocusedColumn = this.CurrentColumn == null ? this.ColumnsInternal.FirstVisibleNonFillerColumn : this.CurrentColumn;
                    }

                    if (oldFocusedColumn != null && oldFocusedColumn.HasHeaderCell)
                    {
                        oldFocusedColumn.HeaderCell.ApplyState(true);
                    }

                    if (this.FocusedColumn != null && this.FocusedColumn.HasHeaderCell)
                    {
                        this.FocusedColumn.HeaderCell.ApplyState(true);
                        ScrollColumnIntoView(this.FocusedColumn.Index);
                    }
                }
            }
        }

        internal DataGridColumnHeaderInteractionInfo ColumnHeaderInteractionInfo
        {
            get;
            set;
        }

        internal DataGridColumnHeadersPresenter ColumnHeaders
        {
            get
            {
                return _columnHeadersPresenter;
            }
        }

        internal DataGridColumnCollection ColumnsInternal
        {
            get;
            private set;
        }

        internal List<DataGridColumn> ColumnsItemsInternal
        {
            get
            {
                return this.ColumnsInternal.ItemsInternal;
            }
        }

        internal bool ContainsFocus
        {
            get;
            private set;
        }

        internal int CurrentColumnIndex
        {
            get
            {
                return this.CurrentCellCoordinates.ColumnIndex;
            }

            private set
            {
                this.CurrentCellCoordinates.ColumnIndex = value;
            }
        }

        internal int CurrentSlot
        {
            get
            {
                return this.CurrentCellCoordinates.Slot;
            }

            private set
            {
                this.CurrentCellCoordinates.Slot = value;
            }
        }

        internal DataGridDataConnection DataConnection
        {
            get;
            private set;
        }

        internal DataGridDisplayData DisplayData
        {
            get;
            private set;
        }

        internal int EditingColumnIndex
        {
            get
            {
                return _editingColumnIndex;
            }
        }

        internal DataGridRow EditingRow
        {
            get;
            private set;
        }

        internal double FirstDisplayedScrollingColumnHiddenWidth
        {
            get
            {
                return _negHorizontalOffset;
            }
        }

        internal DataGridColumn FocusedColumn
        {
            get;
            set;
        }

        internal bool HasColumnUserInteraction
        {
            get
            {
                return this.ColumnHeaderInteractionInfo.HasUserInteraction;
            }
        }

        // When the RowsPresenter's width increases, the HorizontalOffset will be incorrect until
        // the scrollbar's layout is recalculated, which doesn't occur until after the cells are measured.
        // This property exists to account for this scenario, and avoid collapsing the incorrect cells.
        internal double HorizontalAdjustment
        {
            get;
            private set;
        }

        // the sum of the widths in pixels of the scrolling columns preceding
        // the first displayed scrolling column
        internal double HorizontalOffset
        {
            get
            {
                return _horizontalOffset;
            }

            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                double widthNotVisible = Math.Max(0, this.ColumnsInternal.VisibleEdgedColumnsWidth - this.CellsWidth);
                if (value > widthNotVisible)
                {
                    value = widthNotVisible;
                }

                if (value == _horizontalOffset)
                {
                    return;
                }

                SetHorizontalOffset(value);

                _horizontalOffset = value;

                this.DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();

                // update the lastTotallyDisplayedScrollingCol
                ComputeDisplayedColumns();
            }
        }

        internal ScrollBar HorizontalScrollBar
        {
            get
            {
                return _hScrollBar;
            }
        }

        internal bool LoadingOrUnloadingRow
        {
            get;
            private set;
        }

        internal bool InDisplayIndexAdjustments
        {
            get;
            set;
        }

        internal double NegVerticalOffset
        {
            get;
            private set;
        }

        internal int NoCurrentCellChangeCount
        {
            get
            {
                return _noCurrentCellChangeCount;
            }

            set
            {
                DiagnosticsDebug.Assert(value >= 0, "Expected positive NoCurrentCellChangeCount.");
                _noCurrentCellChangeCount = value;
                if (value == 0)
                {
                    FlushCurrentCellChanged();
                }
            }
        }

        internal double RowDetailsHeightEstimate
        {
            get;
            private set;
        }

        internal double RowHeadersDesiredWidth
        {
            get
            {
                return _rowHeaderDesiredWidth;
            }

            set
            {
                // We only auto grow
                if (_rowHeaderDesiredWidth < value)
                {
                    double oldActualRowHeaderWidth = this.ActualRowHeaderWidth;
                    _rowHeaderDesiredWidth = value;
                    if (oldActualRowHeaderWidth != this.ActualRowHeaderWidth)
                    {
                        bool invalidated = EnsureRowHeaderWidth();

                        // If we didn't invalidate in Ensure and we have star columns, force the column widths to be recomputed here.
                        if (!invalidated && this.ColumnsInternal.VisibleStarColumnCount > 0)
                        {
                            this.ColumnsInternal.EnsureVisibleEdgedColumnsWidth();
                            InvalidateMeasure();
                        }
                    }
                }
            }
        }

        internal double RowGroupHeaderHeightEstimate
        {
            get;
            private set;
        }

        internal IndexToValueTable<DataGridRowGroupInfo> RowGroupHeadersTable
        {
            get;
            private set;
        }

        internal double[] RowGroupSublevelIndents
        {
            get;
            private set;
        }

        internal double RowHeightEstimate
        {
            get;
            private set;
        }

        internal Size? RowsPresenterAvailableSize
        {
            get
            {
                return _rowsPresenterAvailableSize;
            }

            set
            {
                if (_rowsPresenterAvailableSize.HasValue && value.HasValue && value.Value.Width > this.RowsPresenterAvailableSize.Value.Width)
                {
                    // When the available cells width increases, the horizontal offset can be incorrect.
                    // Store away an adjustment to use during the CellsPresenter's measure, so that the
                    // ShouldDisplayCell method correctly determines if a cell will be in view.
                    //
                    //     |   h. offset   |       new available cells width          |
                    //     |-------------->|----------------------------------------->|
                    //      __________________________________________________        |
                    //     |           |           |             |            |       |
                    //     |  column0  |  column1  |   column2   |  column3   |<----->|
                    //     |           |           |             |            |  adj. |
                    double adjustment = (_horizontalOffset + value.Value.Width) - this.ColumnsInternal.VisibleEdgedColumnsWidth;
                    this.HorizontalAdjustment = Math.Min(this.HorizontalOffset, Math.Max(0, adjustment));
                }
                else
                {
                    this.HorizontalAdjustment = 0;
                }

                bool loadMoreDataFromIncrementalItemsSource = _rowsPresenterAvailableSize.HasValue && value.HasValue && value.Value.Height > _rowsPresenterAvailableSize.Value.Height;

                _rowsPresenterAvailableSize = value;

                if (loadMoreDataFromIncrementalItemsSource)
                {
                    LoadMoreDataFromIncrementalItemsSource();
                }
            }
        }

        // This flag indicates whether selection has actually changed during a selection operation,
        // and exists to ensure that FlushSelectionChanged doesn't unnecessarily raise SelectionChanged.
        internal bool SelectionHasChanged
        {
            get;
            set;
        }

        internal int SlotCount
        {
            get;
            private set;
        }

        internal bool UpdatedStateOnTapped
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether or not to use star-sizing logic.  If the DataGrid has infinite available space,
        /// then star sizing doesn't make sense.  In this case, all star columns grow to a predefined size of
        /// 10,000 pixels in order to show the developer that star columns shouldn't be used.
        /// </summary>
        internal bool UsesStarSizing
        {
            get
            {
                if (this.ColumnsInternal != null)
                {
                    return this.ColumnsInternal.VisibleStarColumnCount > 0 &&
                        (!this.RowsPresenterAvailableSize.HasValue || !double.IsPositiveInfinity(this.RowsPresenterAvailableSize.Value.Width));
                }

                return false;
            }
        }

        internal double VerticalOffset
        {
            get
            {
                return _verticalOffset;
            }

            set
            {
                bool loadMoreDataFromIncrementalItemsSource = _verticalOffset < value;

                _verticalOffset = value;

                if (loadMoreDataFromIncrementalItemsSource)
                {
                    LoadMoreDataFromIncrementalItemsSource();
                }
            }
        }

        internal ScrollBar VerticalScrollBar
        {
            get
            {
                return _vScrollBar;
            }
        }

        internal int VisibleSlotCount
        {
            get;
            set;
        }

        private bool AreAllScrollBarsCollapsed
        {
            get
            {
                return (_hScrollBar == null || _hScrollBar.Visibility == Visibility.Collapsed) &&
                       (_vScrollBar == null || _vScrollBar.Visibility == Visibility.Collapsed);
            }
        }

        private bool AreBothScrollBarsVisible
        {
            get
            {
                return _hScrollBar != null && _hScrollBar.Visibility == Visibility.Visible &&
                       _vScrollBar != null && _vScrollBar.Visibility == Visibility.Visible;
            }
        }

        private DataGridCellCoordinates CurrentCellCoordinates
        {
            get
            {
                return _currentCellCoordinates;
            }

            set
            {
                _currentCellCoordinates = value;
            }
        }

        private int FirstDisplayedNonFillerColumnIndex
        {
            get
            {
                DataGridColumn column = this.ColumnsInternal.FirstVisibleNonFillerColumn;
                if (column != null)
                {
                    if (column.IsFrozen)
                    {
                        return column.Index;
                    }
                    else
                    {
                        if (this.DisplayData.FirstDisplayedScrollingCol >= column.Index)
                        {
                            return this.DisplayData.FirstDisplayedScrollingCol;
                        }
                        else
                        {
                            return column.Index;
                        }
                    }
                }

                return -1;
            }
        }

        private bool IsHorizontalScrollBarInteracting
        {
            get
            {
                return _isHorizontalScrollBarInteracting;
            }

            set
            {
                if (_isHorizontalScrollBarInteracting != value)
                {
                    _isHorizontalScrollBarInteracting = value;

                    if (_hScrollBar != null)
                    {
                        if (_isHorizontalScrollBarInteracting)
                        {
                            // Prevent the vertical scroll bar from fading out while the user is interacting with the horizontal one.
                            _keepScrollBarsShowing = true;

                            ShowScrollBars();
                        }
                        else
                        {
                            // Make the scroll bars fade out, after the normal delay.
                            _keepScrollBarsShowing = false;

                            HideScrollBars(true /*useTransitions*/);
                        }
                    }
                }
            }
        }

        private bool IsHorizontalScrollBarOverCells
        {
            get
            {
                return _columnHeadersPresenter != null && Grid.GetColumnSpan(_columnHeadersPresenter) == 2;
            }
        }

        private bool IsVerticalScrollBarInteracting
        {
            get
            {
                return _isVerticalScrollBarInteracting;
            }

            set
            {
                if (_isVerticalScrollBarInteracting != value)
                {
                    _isVerticalScrollBarInteracting = value;

                    if (_vScrollBar != null)
                    {
                        if (_isVerticalScrollBarInteracting)
                        {
                            // Prevent the horizontal scroll bar from fading out while the user is interacting with the vertical one.
                            _keepScrollBarsShowing = true;

                            ShowScrollBars();
                        }
                        else
                        {
                            // Make the scroll bars fade out, after the normal delay.
                            _keepScrollBarsShowing = false;

                            HideScrollBars(true /*useTransitions*/);
                        }
                    }
                }
            }
        }

        private bool IsVerticalScrollBarOverCells
        {
            get
            {
                return _rowsPresenter != null && Grid.GetRowSpan(_rowsPresenter) == 2;
            }
        }

        private VirtualKey LastHandledKeyDown
        {
            get;
            set;
        }

        private int NoSelectionChangeCount
        {
            get
            {
                return _noSelectionChangeCount;
            }

            set
            {
                DiagnosticsDebug.Assert(value >= 0, "Expected positive NoSelectionChangeCount.");
                _noSelectionChangeCount = value;
                if (value == 0)
                {
                    FlushSelectionChanged();
                }
            }
        }

        /// <summary>
        /// Enters editing mode for the current cell and current row (if they're not already in editing mode).
        /// </summary>
        /// <returns>True if operation was successful. False otherwise.</returns>
        public bool BeginEdit()
        {
            return BeginEdit(null);
        }

        /// <summary>
        /// Enters editing mode for the current cell and current row (if they're not already in editing mode).
        /// </summary>
        /// <param name="editingEventArgs">Provides information about the user gesture that caused the call to BeginEdit. Can be null.</param>
        /// <returns>True if operation was successful. False otherwise.</returns>
        public bool BeginEdit(RoutedEventArgs editingEventArgs)
        {
            if (this.CurrentColumnIndex == -1 || !GetRowSelection(this.CurrentSlot))
            {
                return false;
            }

            DiagnosticsDebug.Assert(this.CurrentColumnIndex >= 0, "Expected positive CurrentColumnIndex.");
            DiagnosticsDebug.Assert(this.CurrentColumnIndex < this.ColumnsItemsInternal.Count, "Expected CurrentColumnIndex smaller than ColumnsItemsInternal.Count.");
            DiagnosticsDebug.Assert(this.CurrentSlot >= -1, "Expected CurrentSlot greater than or equal to -1.");
            DiagnosticsDebug.Assert(this.CurrentSlot < this.SlotCount, "Expected CurrentSlot smaller than SlotCount.");
            DiagnosticsDebug.Assert(this.EditingRow == null || this.EditingRow.Slot == this.CurrentSlot, "Expected null EditingRow or EditingRow.Slot equal to CurrentSlot.");

            if (GetColumnEffectiveReadOnlyState(this.CurrentColumn))
            {
                // Current column is read-only
                return false;
            }

            return BeginCellEdit(editingEventArgs);
        }

        /// <summary>
        /// Cancels editing mode and restores the original value.
        /// </summary>
        /// <returns>True if operation was successful. False otherwise.</returns>
        public bool CancelEdit()
        {
            return CancelEdit(DataGridEditingUnit.Row);
        }

        /// <summary>
        /// Cancels editing mode for the specified DataGridEditingUnit and restores its original value.
        /// </summary>
        /// <param name="editingUnit">Specifies whether to cancel edit for a Cell or Row.</param>
        /// <returns>True if operation was successful. False otherwise.</returns>
        public bool CancelEdit(DataGridEditingUnit editingUnit)
        {
            return this.CancelEdit(editingUnit, true /*raiseEvents*/);
        }

        /// <summary>
        /// Commits editing mode and pushes changes to the backend.
        /// </summary>
        /// <returns>True if operation was successful. False otherwise.</returns>
        public bool CommitEdit()
        {
            return CommitEdit(DataGridEditingUnit.Row, true);
        }

        /// <summary>
        /// Commits editing mode for the specified DataGridEditingUnit and pushes changes to the backend.
        /// </summary>
        /// <param name="editingUnit">Specifies whether to commit edit for a Cell or Row.</param>
        /// <param name="exitEditingMode">Editing mode is left if True.</param>
        /// <returns>True if operation was successful. False otherwise.</returns>
        public bool CommitEdit(DataGridEditingUnit editingUnit, bool exitEditingMode)
        {
            if (!EndCellEdit(DataGridEditAction.Commit, editingUnit == DataGridEditingUnit.Cell ? exitEditingMode : true, this.ContainsFocus /*keepFocus*/, true /*raiseEvents*/))
            {
                return false;
            }

            if (editingUnit == DataGridEditingUnit.Row)
            {
                return EndRowEdit(DataGridEditAction.Commit, exitEditingMode, true /*raiseEvents*/);
            }

            return true;
        }

        /// <summary>
        /// Returns the Group at the indicated level or null if the item is not in the ItemsSource
        /// </summary>
        /// <param name="item">item</param>
        /// <param name="groupLevel">groupLevel</param>
        /// <returns>The group the given item falls under or null if the item is not in the ItemsSource</returns>
        public ICollectionViewGroup GetGroupFromItem(object item, int groupLevel)
        {
            int itemIndex = this.DataConnection.IndexOf(item);
            if (itemIndex == -1)
            {
                return null;
            }

            int groupHeaderSlot = this.RowGroupHeadersTable.GetPreviousIndex(SlotFromRowIndex(itemIndex));
            DataGridRowGroupInfo rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(groupHeaderSlot);
            while (rowGroupInfo != null && rowGroupInfo.Level != groupLevel)
            {
                groupHeaderSlot = this.RowGroupHeadersTable.GetPreviousIndex(rowGroupInfo.Slot);
                rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(groupHeaderSlot);
            }

            return rowGroupInfo == null ? null : rowGroupInfo.CollectionViewGroup;
        }

        /// <summary>
        /// Scrolls the specified item or RowGroupHeader and/or column into view.
        /// If item is not null: scrolls the row representing the item into view;
        /// If column is not null: scrolls the column into view;
        /// If both item and column are null, the method returns without scrolling.
        /// </summary>
        /// <param name="item">an item from the DataGrid's items source or a CollectionViewGroup from the collection view</param>
        /// <param name="column">a column from the DataGrid's columns collection</param>
        public void ScrollIntoView(object item, DataGridColumn column)
        {
            if ((column == null && (item == null || this.FirstDisplayedNonFillerColumnIndex == -1)) ||
                (column != null && column.OwningGrid != this))
            {
                // no-op
                return;
            }

            if (item == null)
            {
                // scroll column into view
                this.ScrollSlotIntoView(column.Index, this.DisplayData.FirstScrollingSlot, false /*forCurrentCellChange*/, true /*forceHorizontalScroll*/);
            }
            else
            {
                int slot;
                DataGridRowGroupInfo rowGroupInfo = null;
                ICollectionViewGroup collectionViewGroup = item as ICollectionViewGroup;
                if (collectionViewGroup != null)
                {
                    rowGroupInfo = RowGroupInfoFromCollectionViewGroup(collectionViewGroup);
                    if (rowGroupInfo == null)
                    {
                        Debug.Fail("Expected non-null rowGroupInfo.");
                        return;
                    }

                    slot = rowGroupInfo.Slot;
                }
                else
                {
                    // the row index will be set to -1 if the item is null or not in the list
                    int rowIndex = this.DataConnection.IndexOf(item);
                    if (rowIndex == -1 || (this.IsReadOnly && rowIndex == this.DataConnection.NewItemPlaceholderIndex))
                    {
                        return;
                    }

                    slot = SlotFromRowIndex(rowIndex);
                }

                int columnIndex = (column == null) ? this.FirstDisplayedNonFillerColumnIndex : column.Index;

                if (_collapsedSlotsTable.Contains(slot))
                {
                    // We need to expand all parent RowGroups so that the slot is visible
                    if (rowGroupInfo != null)
                    {
                        ExpandRowGroupParentChain(rowGroupInfo.Level - 1, rowGroupInfo.Slot);
                    }
                    else
                    {
                        rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(this.RowGroupHeadersTable.GetPreviousIndex(slot));
                        DiagnosticsDebug.Assert(rowGroupInfo != null, "Expected non-null rowGroupInfo.");
                        if (rowGroupInfo != null)
                        {
                            ExpandRowGroupParentChain(rowGroupInfo.Level, rowGroupInfo.Slot);
                        }
                    }

                    // Update ScrollBar and display information
                    this.NegVerticalOffset = 0;
                    SetVerticalOffset(0);
                    ResetDisplayedRows();
                    this.DisplayData.FirstScrollingSlot = 0;
                    ComputeScrollBarsLayout();
                }

                ScrollSlotIntoView(columnIndex, slot, true /*forCurrentCellChange*/, true /*forceHorizontalScroll*/);
            }
        }

        /// <summary>
        /// Arranges the content of the <see cref="DataGridRow"/>.
        /// </summary>
        /// <param name="finalSize">
        /// The final area within the parent that this element should use to arrange itself and its children.
        /// </param>
        /// <returns>
        /// The actual size used by the <see cref="DataGridRow"/>.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_makeFirstDisplayedCellCurrentCellPending)
            {
                MakeFirstDisplayedCellCurrentCell();
            }

            if (this.ActualWidth != finalSize.Width)
            {
                // If our final width has changed, we might need to update the filler
                InvalidateColumnHeadersArrange();
                InvalidateCellsArrange();
            }

            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// Measures the children of a <see cref="DataGridRow"/> to prepare for
        /// arranging them during the
        /// <see cref="M:System.Windows.Controls.DataGridRow.ArrangeOverride(System.Windows.Size)"/> pass.
        /// </summary>
        /// <returns>
        /// The size that the <see cref="DataGridRow"/> determines it needs during layout, based on its calculations of child object allocated sizes.
        /// </returns>
        /// <param name="availableSize">
        /// The available size that this element can give to child elements. Indicates an upper limit that
        /// child elements should not exceed.
        /// </param>
        protected override Size MeasureOverride(Size availableSize)
        {
            // Delay layout until after the initial measure to avoid invalid calculations when the
            // DataGrid is not part of the visual tree
            if (!_measured)
            {
                _measured = true;

                // We don't need to clear the rows because it was already done when the ItemsSource changed
                RefreshRowsAndColumns(false /*clearRows*/);

                // Update our estimates now that the DataGrid has all of the information necessary
                UpdateRowDetailsHeightEstimate();

                // Update frozen columns to account for columns added prior to loading or auto-generated columns
                if (this.FrozenColumnCountWithFiller > 0)
                {
                    ProcessFrozenColumnCount(this);
                }
            }

            Size desiredSize;

            // This is a shortcut to skip layout if we don't have any columns
            if (this.ColumnsInternal.Count == 0)
            {
                if (_hScrollBar != null && _hScrollBar.Visibility != Visibility.Collapsed)
                {
                    _hScrollBar.Visibility = Visibility.Collapsed;
                }

                if (_vScrollBar != null && _vScrollBar.Visibility != Visibility.Collapsed)
                {
                    _vScrollBar.Visibility = Visibility.Collapsed;
                }

                desiredSize = base.MeasureOverride(availableSize);
            }
            else
            {
                if (_rowsPresenter != null)
                {
                    _rowsPresenter.InvalidateMeasure();
                }

                InvalidateColumnHeadersMeasure();

                desiredSize = base.MeasureOverride(availableSize);

                ComputeScrollBarsLayout();
            }

            return desiredSize;
        }

        /// <summary>
        /// Comparator class so we can sort list by the display index
        /// </summary>
        public class DisplayIndexComparer : IComparer<DataGridColumn>
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer<DataGridColumn>.Compare(DataGridColumn x, DataGridColumn y)
            {
                return (x.DisplayIndexWithFiller < y.DisplayIndexWithFiller) ? -1 : 1;
            }
        }

        /// <summary>
        /// Builds the visual tree for the column header when a new template is applied.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            // The template has changed, so we need to refresh the visuals
            _measured = false;

            _hasNoIndicatorStateStoryboardCompletedHandler = false;
            _keepScrollBarsShowing = false;

            if (_columnHeadersPresenter != null)
            {
                // If we're applying a new template, we want to remove the old column headers first
                _columnHeadersPresenter.Children.Clear();
            }

            _columnHeadersPresenter = GetTemplateChild(DATAGRID_elementColumnHeadersPresenterName) as DataGridColumnHeadersPresenter;
            if (_columnHeadersPresenter != null)
            {
                if (this.ColumnsInternal.FillerColumn != null)
                {
                    this.ColumnsInternal.FillerColumn.IsRepresented = false;
                }

                _columnHeadersPresenter.OwningGrid = this;

                // Columns were added before our Template was applied, add the ColumnHeaders now
                List<DataGridColumn> sortedInternal = new List<DataGridColumn>(this.ColumnsItemsInternal);
                sortedInternal.Sort(new DisplayIndexComparer());
                foreach (DataGridColumn column in sortedInternal)
                {
                    InsertDisplayedColumnHeader(column);
                }
            }

            if (_rowsPresenter != null)
            {
                // If we're applying a new template, we want to remove the old rows first
                this.UnloadElements(false /*recycle*/);
            }

            _rowsPresenter = GetTemplateChild(DATAGRID_elementRowsPresenterName) as DataGridRowsPresenter;
            if (_rowsPresenter != null)
            {
                _rowsPresenter.OwningGrid = this;
                InvalidateRowHeightEstimate();
                UpdateRowDetailsHeightEstimate();
                UpdateRowsPresenterManipulationMode(true /*horizontalMode*/, true /*verticalMode*/);
            }

            _frozenColumnScrollBarSpacer = GetTemplateChild(DATAGRID_elementFrozenColumnScrollBarSpacerName) as FrameworkElement;

            if (_hScrollBar != null)
            {
                _isHorizontalScrollBarInteracting = false;
                _isPointerOverHorizontalScrollBar = false;
                UnhookHorizontalScrollBarEvents();
            }

            _hScrollBar = GetTemplateChild(DATAGRID_elementHorizontalScrollBarName) as ScrollBar;
            if (_hScrollBar != null)
            {
                _hScrollBar.IsTabStop = false;
                _hScrollBar.Maximum = 0.0;
                _hScrollBar.Orientation = Orientation.Horizontal;
                _hScrollBar.Visibility = Visibility.Collapsed;
                HookHorizontalScrollBarEvents();
            }

            if (_vScrollBar != null)
            {
                _isVerticalScrollBarInteracting = false;
                _isPointerOverVerticalScrollBar = false;
                UnhookVerticalScrollBarEvents();
            }

            _vScrollBar = GetTemplateChild(DATAGRID_elementVerticalScrollBarName) as ScrollBar;
            if (_vScrollBar != null)
            {
                _vScrollBar.IsTabStop = false;
                _vScrollBar.Maximum = 0.0;
                _vScrollBar.Orientation = Orientation.Vertical;
                _vScrollBar.Visibility = Visibility.Collapsed;
                HookVerticalScrollBarEvents();
            }

            _topLeftCornerHeader = GetTemplateChild(DATAGRID_elementTopLeftCornerHeaderName) as ContentControl;
            EnsureTopLeftCornerHeader(); // EnsureTopLeftCornerHeader checks for a null _topLeftCornerHeader;
            _topRightCornerHeader = GetTemplateChild(DATAGRID_elementTopRightCornerHeaderName) as ContentControl;
            _bottomRightCorner = GetTemplateChild(DATAGRID_elementBottomRightCornerHeaderName) as UIElement;

#if FEATURE_VALIDATION_SUMMARY
            if (_validationSummary != null)
            {
                _validationSummary.FocusingInvalidControl -= new EventHandler<FocusingInvalidControlEventArgs>(ValidationSummary_FocusingInvalidControl);
                _validationSummary.SelectionChanged -= new EventHandler<SelectionChangedEventArgs>(ValidationSummary_SelectionChanged);
            }

            _validationSummary = GetTemplateChild(DATAGRID_elementValidationSummary) as ValidationSummary;
            if (_validationSummary != null)
            {
                // The ValidationSummary defaults to using its parent if Target is null, so the only
                // way to prevent it from automatically picking up errors is to set it to some useless element.
                if (_validationSummary.Target == null)
                {
                    _validationSummary.Target = new Rectangle();
                }

                _validationSummary.FocusingInvalidControl += new EventHandler<FocusingInvalidControlEventArgs>(ValidationSummary_FocusingInvalidControl);
                _validationSummary.SelectionChanged += new EventHandler<SelectionChangedEventArgs>(ValidationSummary_SelectionChanged);
                if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                    DiagnosticsDebug.Assert(_validationSummary.Errors != null);

                    // Do not add the default design time errors when in design mode.
                    _validationSummary.Errors.Clear();
                }
            }
#endif

            FrameworkElement root = GetTemplateChild(DATAGRID_elementRootName) as FrameworkElement;

            if (root != null)
            {
                IList<VisualStateGroup> rootVisualStateGroups = VisualStateManager.GetVisualStateGroups(root);

                if (rootVisualStateGroups != null)
                {
                    int groupCount = rootVisualStateGroups.Count;

                    for (int groupIndex = 0; groupIndex < groupCount; groupIndex++)
                    {
                        VisualStateGroup group = rootVisualStateGroups[groupIndex];

                        if (group != null)
                        {
                            IList<VisualState> visualStates = group.States;

                            if (visualStates != null)
                            {
                                int stateCount = visualStates.Count;

                                for (int stateIndex = 0; stateIndex < stateCount; stateIndex++)
                                {
                                    VisualState state = visualStates[stateIndex];

                                    if (state != null)
                                    {
                                        string stateName = state.Name;
                                        Storyboard stateStoryboard = state.Storyboard;

                                        if (stateStoryboard != null)
                                        {
                                            if (stateName == VisualStates.StateNoIndicator)
                                            {
                                                stateStoryboard.Completed += NoIndicatorStateStoryboard_Completed;

                                                _hasNoIndicatorStateStoryboardCompletedHandler = true;
                                            }
                                            else if (stateName == VisualStates.StateTouchIndicator || stateName == VisualStates.StateMouseIndicator || stateName == VisualStates.StateMouseIndicatorFull)
                                            {
                                                stateStoryboard.Completed += IndicatorStateStoryboard_Completed;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            HideScrollBars(false /*useTransitions*/);

            UpdateDisabledVisual();
        }

        /// <summary>
        /// Raises the AutoGeneratingColumn event.
        /// </summary>
        protected virtual void OnAutoGeneratingColumn(DataGridAutoGeneratingColumnEventArgs e)
        {
            EventHandler<DataGridAutoGeneratingColumnEventArgs> handler = this.AutoGeneratingColumn;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the BeginningEdit event.
        /// </summary>
        protected virtual void OnBeginningEdit(DataGridBeginningEditEventArgs e)
        {
            EventHandler<DataGridBeginningEditEventArgs> handler = this.BeginningEdit;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the CellEditEnded event.
        /// </summary>
        protected virtual void OnCellEditEnded(DataGridCellEditEndedEventArgs e)
        {
            EventHandler<DataGridCellEditEndedEventArgs> handler = this.CellEditEnded;
            if (handler != null)
            {
                handler(this, e);
            }

            // Raise the automation invoke event for the cell that just ended edit
            DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
            if (peer != null && AutomationPeer.ListenerExists(AutomationEvents.InvokePatternOnInvoked))
            {
                peer.RaiseAutomationInvokeEvents(DataGridEditingUnit.Cell, e.Column, e.Row);
            }
        }

        /// <summary>
        /// Raises the CellEditEnding event.
        /// </summary>
        protected virtual void OnCellEditEnding(DataGridCellEditEndingEventArgs e)
        {
            EventHandler<DataGridCellEditEndingEventArgs> handler = this.CellEditEnding;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// This method raises the CopyingRowClipboardContent event.
        /// </summary>
        /// <param name="e">Contains the necessary information for generating the row clipboard content.</param>
        protected virtual void OnCopyingRowClipboardContent(DataGridRowClipboardEventArgs e)
        {
            EventHandler<DataGridRowClipboardEventArgs> handler = this.CopyingRowClipboardContent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Creates AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
        /// </summary>
        /// <returns>An automation peer for this <see cref="DataGrid"/>.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DataGridAutomationPeer(this);
        }

        /// <summary>
        /// Raises the CurrentCellChanged event.
        /// </summary>
        protected virtual void OnCurrentCellChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = this.CurrentCellChanged;
            if (handler != null)
            {
                handler(this, e);
            }

            if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected))
            {
                DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseAutomationCellSelectedEvent(this.CurrentSlot, this.CurrentColumnIndex);
                }
            }
        }

        /// <summary>
        /// Raises the LoadingRow event for row preparation.
        /// </summary>
        protected virtual void OnLoadingRow(DataGridRowEventArgs e)
        {
            EventHandler<DataGridRowEventArgs> handler = this.LoadingRow;
            if (handler != null)
            {
                DiagnosticsDebug.Assert(!_loadedRows.Contains(e.Row), "Expected e.Rows not contained in _loadedRows.");
                _loadedRows.Add(e.Row);
                this.LoadingOrUnloadingRow = true;
                try
                {
                    handler(this, e);
                }
                finally
                {
                    this.LoadingOrUnloadingRow = false;
                    DiagnosticsDebug.Assert(_loadedRows.Contains(e.Row), "Expected e.Rows contained in _loadedRows.");
                    _loadedRows.Remove(e.Row);
                }
            }
        }

        /// <summary>
        /// Raises the LoadingRowGroup event
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected virtual void OnLoadingRowGroup(DataGridRowGroupHeaderEventArgs e)
        {
            EventHandler<DataGridRowGroupHeaderEventArgs> handler = this.LoadingRowGroup;
            if (handler != null)
            {
                this.LoadingOrUnloadingRow = true;
                try
                {
                    handler(this, e);
                }
                finally
                {
                    this.LoadingOrUnloadingRow = false;
                }
            }
        }

        /// <summary>
        /// Raises the LoadingRowDetails for row details preparation
        /// </summary>
        protected virtual void OnLoadingRowDetails(DataGridRowDetailsEventArgs e)
        {
            EventHandler<DataGridRowDetailsEventArgs> handler = this.LoadingRowDetails;
            if (handler != null)
            {
                this.LoadingOrUnloadingRow = true;
                try
                {
                    handler(this, e);
                }
                finally
                {
                    this.LoadingOrUnloadingRow = false;
                }
            }
        }

        /// <summary>
        /// Scrolls the DataGrid according to the direction of the delta.
        /// </summary>
        /// <param name="e">PointerRoutedEventArgs</param>
        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            base.OnPointerWheelChanged(e);
            if (!e.Handled)
            {
                PointerPoint pointerPoint = e.GetCurrentPoint(this);

                // A horizontal scroll happens if the mouse has a horizontal wheel OR if the horizontal scrollbar is not disabled AND the vertical scrollbar IS disabled
                bool isForHorizontalScroll = pointerPoint.Properties.IsHorizontalMouseWheel ||
                    (this.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled && this.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled);

                if ((isForHorizontalScroll && this.HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled) ||
                    (!isForHorizontalScroll && this.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled))
                {
                    return;
                }

                double offsetDelta = -pointerPoint.Properties.MouseWheelDelta / DATAGRID_mouseWheelDeltaDivider;
                if (isForHorizontalScroll && pointerPoint.Properties.IsHorizontalMouseWheel)
                {
                    offsetDelta *= -1.0;
                }

                e.Handled = ProcessScrollOffsetDelta(offsetDelta, isForHorizontalScroll);
            }
        }

        /// <summary>
        /// Raises the PreparingCellForEdit event.
        /// </summary>
        protected virtual void OnPreparingCellForEdit(DataGridPreparingCellForEditEventArgs e)
        {
            EventHandler<DataGridPreparingCellForEditEventArgs> handler = this.PreparingCellForEdit;
            if (handler != null)
            {
                handler(this, e);
            }

            // Raise the automation invoke event for the cell that just began edit because now
            // its editable content has been loaded
            DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
            if (peer != null && AutomationPeer.ListenerExists(AutomationEvents.InvokePatternOnInvoked))
            {
                peer.RaiseAutomationInvokeEvents(DataGridEditingUnit.Cell, e.Column, e.Row);
            }
        }

        /// <summary>
        /// Raises the RowEditEnded event.
        /// </summary>
        protected virtual void OnRowEditEnded(DataGridRowEditEndedEventArgs e)
        {
            EventHandler<DataGridRowEditEndedEventArgs> handler = this.RowEditEnded;
            if (handler != null)
            {
                handler(this, e);
            }

            // Raise the automation invoke event for the row that just ended edit because the edits
            // to its associated item have either been committed or reverted
            DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
            if (peer != null && AutomationPeer.ListenerExists(AutomationEvents.InvokePatternOnInvoked))
            {
                peer.RaiseAutomationInvokeEvents(DataGridEditingUnit.Row, null, e.Row);
            }
        }

        /// <summary>
        /// Raises the RowEditEnding event.
        /// </summary>
        protected virtual void OnRowEditEnding(DataGridRowEditEndingEventArgs e)
        {
            EventHandler<DataGridRowEditEndingEventArgs> handler = this.RowEditEnding;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the SelectionChanged event and clears the _selectionChanged.
        /// This event won't get raised again until after _selectionChanged is set back to true.
        /// </summary>
        protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            SelectionChangedEventHandler handler = this.SelectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }

            if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected) ||
                AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection) ||
                AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection))
            {
                DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseAutomationSelectionEvents(e);
                }
            }
        }

        /// <summary>
        /// Raises the UnloadingRow event for row recycling.
        /// </summary>
        protected virtual void OnUnloadingRow(DataGridRowEventArgs e)
        {
            EventHandler<DataGridRowEventArgs> handler = this.UnloadingRow;
            if (handler != null)
            {
                this.LoadingOrUnloadingRow = true;
                try
                {
                    handler(this, e);
                }
                finally
                {
                    this.LoadingOrUnloadingRow = false;
                }
            }
        }

        /// <summary>
        /// Raises the UnloadingRowDetails event
        /// </summary>
        protected virtual void OnUnloadingRowDetails(DataGridRowDetailsEventArgs e)
        {
            EventHandler<DataGridRowDetailsEventArgs> handler = this.UnloadingRowDetails;
            if (handler != null)
            {
                this.LoadingOrUnloadingRow = true;
                try
                {
                    handler(this, e);
                }
                finally
                {
                    this.LoadingOrUnloadingRow = false;
                }
            }
        }

        /// <summary>
        /// Raises the UnloadingRowGroup event
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected virtual void OnUnloadingRowGroup(DataGridRowGroupHeaderEventArgs e)
        {
            EventHandler<DataGridRowGroupHeaderEventArgs> handler = this.UnloadingRowGroup;
            if (handler != null)
            {
                this.LoadingOrUnloadingRow = true;
                try
                {
                    handler(this, e);
                }
                finally
                {
                    this.LoadingOrUnloadingRow = false;
                }
            }
        }

        internal static DataGridCell GetOwningCell(FrameworkElement element)
        {
            DiagnosticsDebug.Assert(element != null, "Expected non-null element.");
            DataGridCell cell = element as DataGridCell;
            while (element != null && cell == null)
            {
                element = element.Parent as FrameworkElement;
                cell = element as DataGridCell;
            }

            return cell;
        }

        /// <summary>
        /// Cancels editing mode for the specified DataGridEditingUnit and restores its original value.
        /// </summary>
        /// <param name="editingUnit">Specifies whether to cancel edit for a Cell or Row.</param>
        /// <param name="raiseEvents">Specifies whether or not to raise editing events</param>
        /// <returns>True if operation was successful. False otherwise.</returns>
        internal bool CancelEdit(DataGridEditingUnit editingUnit, bool raiseEvents)
        {
            if (!EndCellEdit(DataGridEditAction.Cancel, true, this.ContainsFocus /*keepFocus*/, raiseEvents))
            {
                return false;
            }

            if (editingUnit == DataGridEditingUnit.Row)
            {
                return EndRowEdit(DataGridEditAction.Cancel, true, raiseEvents);
            }

            return true;
        }

        /// <summary>
        /// call when: selection changes or SelectedItems object changes
        /// </summary>
        internal void CoerceSelectedItem()
        {
            object selectedItem = null;

            if (this.SelectionMode == DataGridSelectionMode.Extended &&
                this.CurrentSlot != -1 &&
                _selectedItems.ContainsSlot(this.CurrentSlot))
            {
                selectedItem = this.CurrentItem;
            }
            else if (_selectedItems.Count > 0)
            {
                selectedItem = _selectedItems[0];
            }

            if (this.SelectedItem != selectedItem)
            {
                this.SetValueNoCallback(DataGrid.SelectedItemProperty, selectedItem);
            }

            // Update the SelectedIndex
            int newIndex = -1;
            if (selectedItem != null)
            {
                newIndex = this.DataConnection.IndexOf(selectedItem);
            }

            if (this.SelectedIndex != newIndex)
            {
                this.SetValueNoCallback(DataGrid.SelectedIndexProperty, newIndex);
            }
        }

        internal IEnumerable<object> GetSelectionInclusive(int startRowIndex, int endRowIndex)
        {
            int endSlot = SlotFromRowIndex(endRowIndex);
            foreach (int slot in _selectedItems.GetSlots(SlotFromRowIndex(startRowIndex)))
            {
                if (slot > endSlot)
                {
                    break;
                }

                yield return this.DataConnection.GetDataItem(RowIndexFromSlot(slot));
            }
        }

        internal void InitializeElements(bool recycleRows)
        {
            try
            {
                _noCurrentCellChangeCount++;

                // The underlying collection has changed and our editing row (if there is one)
                // is no longer relevant, so we should force a cancel edit.
                CancelEdit(DataGridEditingUnit.Row, false /*raiseEvents*/);

                // We want to persist selection throughout a reset, so store away the selected items
                List<object> selectedItemsCache = new List<object>(_selectedItems.SelectedItemsCache);

                if (recycleRows)
                {
                    RefreshRows(recycleRows /*recycleRows*/, true /*clearRows*/);
                }
                else
                {
                    RefreshRowsAndColumns(true /*clearRows*/);
                }

                // Re-select the old items
                _selectedItems.SelectedItemsCache = selectedItemsCache;
                CoerceSelectedItem();
                if (this.RowDetailsVisibilityMode != DataGridRowDetailsVisibilityMode.Collapsed)
                {
                    UpdateRowDetailsVisibilityMode(this.RowDetailsVisibilityMode);
                }

                // The currently displayed rows may have incorrect visual states because of the selection change
                ApplyDisplayedRowsState(this.DisplayData.FirstScrollingSlot, this.DisplayData.LastScrollingSlot);
            }
            finally
            {
                this.NoCurrentCellChangeCount--;
            }
        }

        // Returns the item or the CollectionViewGroup that is used as the DataContext for a given slot.
        // If the DataContext is an item, rowIndex is set to the index of the item within the collection.
        internal object ItemFromSlot(int slot, ref int rowIndex)
        {
            if (this.RowGroupHeadersTable.Contains(slot))
            {
                DataGridRowGroupInfo groupInfo = this.RowGroupHeadersTable.GetValueAt(slot);
                if (groupInfo != null)
                {
                    return groupInfo.CollectionViewGroup;
                }
            }
            else
            {
                rowIndex = RowIndexFromSlot(slot);
                return this.DataConnection.GetDataItem(rowIndex);
            }

            return null;
        }

        internal void LoadMoreDataFromIncrementalItemsSource()
        {
            LoadMoreDataFromIncrementalItemsSource(totalVisibleHeight: EdgedRowsHeightCalculated);
        }

        internal void OnRowDetailsChanged()
        {
            if (!_scrollingByHeight)
            {
                // Update layout when RowDetails are expanded or collapsed, just updating the vertical scroll bar is not enough
                // since rows could be added or removed.
                InvalidateMeasure();
            }
        }

        internal void OnUserSorting()
        {
            _isUserSorting = true;
        }

        internal void OnUserSorted()
        {
            _isUserSorting = false;
        }

        internal bool ProcessDownKey()
        {
            bool shift, ctrl;
            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            return ProcessDownKeyInternal(shift, ctrl);
        }

        internal bool ProcessEndKey()
        {
            bool ctrl;
            bool shift;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            return this.ProcessEndKey(shift, ctrl);
        }

        internal bool ProcessEnterKey()
        {
            bool ctrl, shift;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            return this.ProcessEnterKey(shift, ctrl);
        }

        internal bool ProcessHomeKey()
        {
            bool ctrl;
            bool shift;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            return this.ProcessHomeKey(shift, ctrl);
        }

        internal void ProcessHorizontalScroll(ScrollEventType scrollEventType)
        {
            if (scrollEventType == ScrollEventType.EndScroll)
            {
                this.IsHorizontalScrollBarInteracting = false;
            }
            else if (scrollEventType == ScrollEventType.ThumbTrack)
            {
                this.IsHorizontalScrollBarInteracting = true;
            }

            if (_horizontalScrollChangesIgnored > 0)
            {
                return;
            }

            // If the user scrolls with the buttons, we need to update the new value of the scroll bar since we delay
            // this calculation.  If they scroll in another other way, the scroll bar's correct value has already been set
            double scrollBarValueDifference = 0;
            if (scrollEventType == ScrollEventType.SmallIncrement)
            {
                scrollBarValueDifference = GetHorizontalSmallScrollIncrease();
            }
            else if (scrollEventType == ScrollEventType.SmallDecrement)
            {
                scrollBarValueDifference = -GetHorizontalSmallScrollDecrease();
            }

            _horizontalScrollChangesIgnored++;
            try
            {
                if (scrollBarValueDifference != 0)
                {
                    DiagnosticsDebug.Assert(_horizontalOffset + scrollBarValueDifference >= 0, "Expected positive _horizontalOffset + scrollBarValueDifference.");
                    SetHorizontalOffset(_horizontalOffset + scrollBarValueDifference);
                }

                UpdateHorizontalOffset(_hScrollBar.Value);
            }
            finally
            {
                _horizontalScrollChangesIgnored--;
            }

            DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
            if (peer != null)
            {
                peer.RaiseAutomationScrollEvents();
            }
        }

        internal bool ProcessLeftKey()
        {
            bool ctrl;
            bool shift;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            return this.ProcessLeftKey(shift, ctrl);
        }

        internal bool ProcessNextKey()
        {
            bool ctrl;
            bool shift;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            return this.ProcessNextKey(shift, ctrl);
        }

        internal bool ProcessPriorKey()
        {
            bool ctrl;
            bool shift;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            return this.ProcessPriorKey(shift, ctrl);
        }

        internal bool ProcessRightKey()
        {
            bool ctrl;
            bool shift;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            return this.ProcessRightKey(shift, ctrl);
        }

        internal bool ProcessScrollOffsetDelta(double offsetDelta, bool isForHorizontalScroll)
        {
            if (this.IsEnabled && this.DisplayData.NumDisplayedScrollingElements > 0)
            {
                if (isForHorizontalScroll)
                {
                    double newHorizontalOffset = this.HorizontalOffset + offsetDelta;
                    if (newHorizontalOffset < 0)
                    {
                        newHorizontalOffset = 0;
                    }

                    double maxHorizontalOffset = Math.Max(0, this.ColumnsInternal.VisibleEdgedColumnsWidth - this.CellsWidth);
                    if (newHorizontalOffset > maxHorizontalOffset)
                    {
                        newHorizontalOffset = maxHorizontalOffset;
                    }

                    if (newHorizontalOffset != this.HorizontalOffset)
                    {
                        UpdateHorizontalOffset(newHorizontalOffset);
                        return true;
                    }
                }
                else
                {
                    if (offsetDelta < 0)
                    {
                        offsetDelta = Math.Max(-_verticalOffset, offsetDelta);
                    }
                    else if (offsetDelta > 0)
                    {
                        if (_vScrollBar != null && this.VerticalScrollBarVisibility == ScrollBarVisibility.Visible)
                        {
                            offsetDelta = Math.Min(Math.Max(0, _vScrollBar.Maximum - _verticalOffset), offsetDelta);
                        }
                        else
                        {
                            double maximum = this.EdgedRowsHeightCalculated - this.CellsHeight;
                            offsetDelta = Math.Min(Math.Max(0, maximum - _verticalOffset), offsetDelta);
                        }
                    }

                    if (offsetDelta != 0)
                    {
                        this.DisplayData.PendingVerticalScrollHeight = offsetDelta;
                        InvalidateRowsMeasure(false /*invalidateIndividualRows*/);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Selects items and updates currency based on parameters
        /// </summary>
        /// <param name="columnIndex">column index to make current</param>
        /// <param name="item">data item or CollectionViewGroup to make current</param>
        /// <param name="backupSlot">slot to use in case the item is no longer valid</param>
        /// <param name="action">selection action to perform</param>
        /// <param name="scrollIntoView">whether or not the new current item should be scrolled into view</param>
        internal void ProcessSelectionAndCurrency(int columnIndex, object item, int backupSlot, DataGridSelectionAction action, bool scrollIntoView)
        {
            _noSelectionChangeCount++;
            _noCurrentCellChangeCount++;
            try
            {
                int slot = -1;
                ICollectionViewGroup group = item as ICollectionViewGroup;
                if (group != null)
                {
                    DataGridRowGroupInfo groupInfo = this.RowGroupInfoFromCollectionViewGroup(group);
                    if (groupInfo != null)
                    {
                        slot = groupInfo.Slot;
                    }
                }
                else
                {
                    slot = this.SlotFromRowIndex(this.DataConnection.IndexOf(item));
                }

                if (slot == -1)
                {
                    slot = backupSlot;
                }

                if (slot < 0 || slot > this.SlotCount)
                {
                    return;
                }

                switch (action)
                {
                    case DataGridSelectionAction.AddCurrentToSelection:
                        SetRowSelection(slot, true /*isSelected*/, true /*setAnchorIndex*/);
                        break;
                    case DataGridSelectionAction.RemoveCurrentFromSelection:
                        SetRowSelection(slot, false /*isSelected*/, false /*setAnchorRowIndex*/);
                        break;
                    case DataGridSelectionAction.SelectFromAnchorToCurrent:
                        if (this.SelectionMode == DataGridSelectionMode.Extended && this.AnchorSlot != -1)
                        {
                            int anchorSlot = this.AnchorSlot;
                            ClearRowSelection(slot /*slotException*/, false /*resetAnchorSlot*/);
                            if (slot <= anchorSlot)
                            {
                                SetRowsSelection(slot, anchorSlot);
                            }
                            else
                            {
                                SetRowsSelection(anchorSlot, slot);
                            }
                        }
                        else
                        {
                            goto case DataGridSelectionAction.SelectCurrent;
                        }

                        break;
                    case DataGridSelectionAction.SelectCurrent:
                        ClearRowSelection(slot /*rowIndexException*/, true /*setAnchorRowIndex*/);
                        break;
                    case DataGridSelectionAction.None:
                        break;
                }

                if (this.CurrentSlot != slot || (this.CurrentColumnIndex != columnIndex && columnIndex != -1))
                {
                    if (columnIndex == -1)
                    {
                        if (this.CurrentColumnIndex != -1)
                        {
                            columnIndex = this.CurrentColumnIndex;
                        }
                        else
                        {
                            DataGridColumn firstVisibleColumn = this.ColumnsInternal.FirstVisibleNonFillerColumn;
                            if (firstVisibleColumn != null)
                            {
                                columnIndex = firstVisibleColumn.Index;
                            }
                        }
                    }

                    if (columnIndex != -1)
                    {
                        if (!SetCurrentCellCore(columnIndex, slot, true /*commitEdit*/, SlotFromRowIndex(this.SelectedIndex) != slot /*endRowEdit*/)
                            || (scrollIntoView && !ScrollSlotIntoView(columnIndex, slot, true /*forCurrentCellChange*/, false /*forceHorizontalScroll*/)))
                        {
                            return;
                        }
                    }
                }

                _successfullyUpdatedSelection = true;
            }
            finally
            {
                this.NoCurrentCellChangeCount--;
                this.NoSelectionChangeCount--;
            }
        }

        internal bool ProcessUpKey()
        {
            bool ctrl;
            bool shift;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            return this.ProcessUpKey(shift, ctrl);
        }

        internal void ProcessVerticalScroll(ScrollEventType scrollEventType)
        {
            if (scrollEventType == ScrollEventType.EndScroll)
            {
                this.IsVerticalScrollBarInteracting = false;
            }
            else if (scrollEventType == ScrollEventType.ThumbTrack)
            {
                this.IsVerticalScrollBarInteracting = true;
            }

            if (_verticalScrollChangesIgnored > 0)
            {
                return;
            }

            DiagnosticsDebug.Assert(DoubleUtil.LessThanOrClose(_vScrollBar.Value, _vScrollBar.Maximum), "Expected _vScrollBar.Value smaller than or close to _vScrollBar.Maximum.");

            _verticalScrollChangesIgnored++;
            try
            {
                DiagnosticsDebug.Assert(_vScrollBar != null, "Expected non-null _vScrollBar.");
                if (scrollEventType == ScrollEventType.SmallIncrement)
                {
                    this.DisplayData.PendingVerticalScrollHeight = GetVerticalSmallScrollIncrease();
                    double newVerticalOffset = _verticalOffset + this.DisplayData.PendingVerticalScrollHeight;
                    if (newVerticalOffset > _vScrollBar.Maximum)
                    {
                        this.DisplayData.PendingVerticalScrollHeight -= newVerticalOffset - _vScrollBar.Maximum;
                    }
                }
                else if (scrollEventType == ScrollEventType.SmallDecrement)
                {
                    if (DoubleUtil.GreaterThan(this.NegVerticalOffset, 0))
                    {
                        this.DisplayData.PendingVerticalScrollHeight -= this.NegVerticalOffset;
                    }
                    else
                    {
                        int previousScrollingSlot = this.GetPreviousVisibleSlot(this.DisplayData.FirstScrollingSlot);
                        if (previousScrollingSlot >= 0)
                        {
                            ScrollSlotIntoView(previousScrollingSlot, false /*scrolledHorizontally*/);
                        }

                        return;
                    }
                }
                else
                {
                    this.DisplayData.PendingVerticalScrollHeight = _vScrollBar.Value - _verticalOffset;
                }

                if (!DoubleUtil.IsZero(this.DisplayData.PendingVerticalScrollHeight))
                {
                    // Invalidate so the scroll happens on idle
                    InvalidateRowsMeasure(false /*invalidateIndividualElements*/);
                }
            }
            finally
            {
                _verticalScrollChangesIgnored--;
            }
        }

        internal void RefreshRowsAndColumns(bool clearRows)
        {
            if (_measured)
            {
                try
                {
                    _noCurrentCellChangeCount++;

                    if (clearRows)
                    {
                        ClearRows(false);
                        ClearRowGroupHeadersTable();
                        PopulateRowGroupHeadersTable();
                        RefreshSlotCounts();
                    }

                    if (this.AutoGenerateColumns)
                    {
                        // Column auto-generation refreshes the rows too
                        AutoGenerateColumnsPrivate();
                    }

                    foreach (DataGridColumn column in this.ColumnsItemsInternal)
                    {
                        // We don't need to refresh the state of AutoGenerated column headers because they're up-to-date
                        if (!column.IsAutoGenerated && column.HasHeaderCell)
                        {
                            column.HeaderCell.ApplyState(false);
                        }
                    }

                    RefreshRows(false /*recycleRows*/, false /*clearRows*/);

                    if (this.Columns.Count > 0 && this.CurrentColumnIndex == -1)
                    {
                        MakeFirstDisplayedCellCurrentCell();
                    }
                    else
                    {
                        _makeFirstDisplayedCellCurrentCellPending = false;
                        _desiredCurrentColumnIndex = -1;
                        FlushCurrentCellChanged();
                    }
                }
                finally
                {
                    this.NoCurrentCellChangeCount--;
                }
            }
            else
            {
                if (clearRows)
                {
                    ClearRows(false /*recycle*/);
                }

                ClearRowGroupHeadersTable();
                PopulateRowGroupHeadersTable();
                RefreshSlotCounts();
            }
        }

        internal void ResetColumnHeaderInteractionInfo()
        {
            DataGridColumnHeaderInteractionInfo interactionInfo = this.ColumnHeaderInteractionInfo;

            if (interactionInfo != null)
            {
                interactionInfo.CapturedPointer = null;
                interactionInfo.DragMode = DataGridColumnHeader.DragMode.None;
                interactionInfo.DragPointerId = 0;
                interactionInfo.DragColumn = null;
                interactionInfo.DragStart = null;
                interactionInfo.PressedPointerPositionHeaders = null;
                interactionInfo.LastPointerPositionHeaders = null;
            }

            if (this.ColumnHeaders != null)
            {
                this.ColumnHeaders.DragColumn = null;
                this.ColumnHeaders.DragIndicator = null;
                this.ColumnHeaders.DropLocationIndicator = null;
            }
        }

        internal bool ScrollSlotIntoView(int columnIndex, int slot, bool forCurrentCellChange, bool forceHorizontalScroll)
        {
            DiagnosticsDebug.Assert(columnIndex >= 0, "Expected positive columnIndex.");
            DiagnosticsDebug.Assert(columnIndex < this.ColumnsItemsInternal.Count, "Expected columnIndex smaller than ColumnsItemsInternal.Count.");
            DiagnosticsDebug.Assert(this.DisplayData.FirstDisplayedScrollingCol >= -1, "Expected DisplayData.FirstDisplayedScrollingCol greater than or equal to -1.");
            DiagnosticsDebug.Assert(this.DisplayData.FirstDisplayedScrollingCol < this.ColumnsItemsInternal.Count, "Expected smaller than ColumnsItemsInternal.Count.");
            DiagnosticsDebug.Assert(this.DisplayData.LastTotallyDisplayedScrollingCol >= -1, "Expected DisplayData.LastTotallyDisplayedScrollingCol greater than or equal to -1.");
            DiagnosticsDebug.Assert(this.DisplayData.LastTotallyDisplayedScrollingCol < this.ColumnsItemsInternal.Count, "Expected DisplayData.LastTotallyDisplayedScrollingCol smaller than ColumnsItemsInternal.Count.");
            DiagnosticsDebug.Assert(!IsSlotOutOfBounds(slot), "Expected IsSlotOutOfBounds(slot) is false.");
            DiagnosticsDebug.Assert(this.DisplayData.FirstScrollingSlot >= -1, "Expected DisplayData.FirstScrollingSlot greater than or equal to -1.");
            DiagnosticsDebug.Assert(this.DisplayData.FirstScrollingSlot < this.SlotCount, "Expected DisplayData.FirstScrollingSlot smaller than SlotCount.");
            DiagnosticsDebug.Assert(this.ColumnsItemsInternal[columnIndex].IsVisible, "Expected ColumnsItemsInternal[columnIndex].IsVisible is true.");

            if (this.CurrentColumnIndex >= 0 &&
                (this.CurrentColumnIndex != columnIndex || this.CurrentSlot != slot))
            {
                if (!CommitEditForOperation(columnIndex, slot, forCurrentCellChange) || IsInnerCellOutOfBounds(columnIndex, slot))
                {
                    return false;
                }
            }

            double oldHorizontalOffset = this.HorizontalOffset;
            bool rowGroupHeadersTableContainsSlot = this.RowGroupHeadersTable.Contains(slot);

            // scroll horizontally unless we're on a RowGroupHeader and we're not forcing horizontal scrolling
            if ((forceHorizontalScroll || (slot != -1 && !rowGroupHeadersTableContainsSlot)) &&
                !ScrollColumnIntoView(columnIndex))
            {
                return false;
            }

            // scroll vertically
            if (!ScrollSlotIntoView(slot, oldHorizontalOffset != this.HorizontalOffset /*scrolledHorizontally*/))
            {
                return false;
            }

            // Scrolling horizontally or vertically could cause less rows to be displayed
            this.DisplayData.FullyRecycleElements();

            DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
            if (peer != null)
            {
                peer.RaiseAutomationScrollEvents();
            }

            return true;
        }

        // Convenient overload that commits the current edit.
        internal bool SetCurrentCellCore(int columnIndex, int slot)
        {
            return SetCurrentCellCore(columnIndex, slot, true /*commitEdit*/, true /*endRowEdit*/);
        }

        internal void UpdateHorizontalOffset(double newValue)
        {
            if (this.HorizontalOffset != newValue)
            {
                this.HorizontalOffset = newValue;

                InvalidateColumnHeadersMeasure();
                InvalidateRowsMeasure(true);
            }
        }

        internal bool UpdateSelectionAndCurrency(int columnIndex, int slot, DataGridSelectionAction action, bool scrollIntoView)
        {
            _successfullyUpdatedSelection = false;

            _noSelectionChangeCount++;
            _noCurrentCellChangeCount++;
            try
            {
                if (this.ColumnsInternal.RowGroupSpacerColumn.IsRepresented &&
                    columnIndex == this.ColumnsInternal.RowGroupSpacerColumn.Index)
                {
                    columnIndex = -1;
                }

                if (IsSlotOutOfSelectionBounds(slot) || (columnIndex != -1 && IsColumnOutOfBounds(columnIndex)))
                {
                    return false;
                }

                int newCurrentPosition = -1;
                object item = ItemFromSlot(slot, ref newCurrentPosition);
                if (newCurrentPosition == this.DataConnection.NewItemPlaceholderIndex)
                {
                    newCurrentPosition = -1;
                }

                if (this.EditingRow != null && slot != this.EditingRow.Slot && !CommitEdit(DataGridEditingUnit.Row, true))
                {
                    return false;
                }

                if (this.DataConnection.CollectionView != null &&
                    this.DataConnection.CollectionView.CurrentPosition != newCurrentPosition)
                {
                    this.DataConnection.MoveCurrentTo(item, slot, columnIndex, action, scrollIntoView);
                }
                else
                {
                    this.ProcessSelectionAndCurrency(columnIndex, item, slot, action, scrollIntoView);
                }
            }
            finally
            {
                this.NoCurrentCellChangeCount--;
                this.NoSelectionChangeCount--;
            }

            return _successfullyUpdatedSelection;
        }

        internal void UpdateStateOnCurrentChanged(object currentItem, int currentPosition)
        {
            if ((currentItem == this.CurrentItem) &&
                (_isUserSorting || (currentItem == this.SelectedItem && currentPosition == this.SelectedIndex)))
            {
                // The DataGrid's CurrentItem is already up-to-date, so we don't need to do anything.
                // In the sorting case, we receive a CurrentChanged notification if the current item
                // changes position in the CollectionView.  However, our CurrentItem is already
                // in the correct position in this case, and we do not want to update the selection so
                // we no-op here.
                return;
            }

            int columnIndex = this.CurrentColumnIndex;
            if (columnIndex == -1)
            {
                if (this.IsColumnOutOfBounds(_desiredCurrentColumnIndex))
                {
                    columnIndex = this.FirstDisplayedNonFillerColumnIndex;
                }
                else if (this.ColumnsInternal.RowGroupSpacerColumn.IsRepresented && _desiredCurrentColumnIndex == this.ColumnsInternal.RowGroupSpacerColumn.Index)
                {
                    columnIndex = this.FirstDisplayedNonFillerColumnIndex;
                }
                else
                {
                    columnIndex = _desiredCurrentColumnIndex;
                }
            }

            // The CollectionView will potentially raise multiple CurrentChanged events during a single
            // add operation, so we should avoid resetting our desired column index until it's committed.
            if (!this.DataConnection.IsAddingNew)
            {
                _desiredCurrentColumnIndex = -1;
            }

            try
            {
                _noSelectionChangeCount++;
                _noCurrentCellChangeCount++;

                if (!this.CommitEdit())
                {
                    this.CancelEdit(DataGridEditingUnit.Row, false);
                }

                this.ClearRowSelection(true);
                if (currentItem == null)
                {
                    SetCurrentCellCore(-1, -1);
                }
                else
                {
                    int slot = SlotFromRowIndex(currentPosition);
                    this.ProcessSelectionAndCurrency(columnIndex, currentItem, slot, DataGridSelectionAction.SelectCurrent, false);
                }
            }
            finally
            {
                this.NoCurrentCellChangeCount--;
                this.NoSelectionChangeCount--;
            }
        }

        internal bool UpdateStateOnTapped(TappedRoutedEventArgs args, int columnIndex, int slot, bool allowEdit)
        {
            bool ctrl, shift;
            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            return this.UpdateStateOnTapped(args, columnIndex, slot, allowEdit, shift, ctrl);
        }

        internal void UpdateVerticalScrollBar()
        {
            if (_vScrollBar != null && _vScrollBar.Visibility == Visibility.Visible)
            {
                double cellsHeight = this.CellsHeight;
                double edgedRowsHeightCalculated = this.EdgedRowsHeightCalculated;
                UpdateVerticalScrollBar(
                    edgedRowsHeightCalculated > cellsHeight /*needVertScrollBar*/,
                    this.VerticalScrollBarVisibility == ScrollBarVisibility.Visible /*forceVertScrollBar*/,
                    edgedRowsHeightCalculated,
                    cellsHeight);
            }
        }

        /// <summary>
        /// If the editing element has focus, this method will set focus to the DataGrid itself
        /// in order to force the element to lose focus.  It will then wait for the editing element's
        /// LostFocus event, at which point it will perform the specified action.
        /// NOTE: It is important to understand that the specified action will be performed when the editing
        /// element loses focus only if this method returns true.  If it returns false, then the action
        /// will not be performed later on, and should instead be performed by the caller, if necessary.
        /// </summary>
        /// <param name="action">Action to perform after the editing element loses focus</param>
        /// <returns>True if the editing element had focus and the action was cached away; false otherwise</returns>
        internal bool WaitForLostFocus(Action action)
        {
            if (this.EditingRow != null && this.EditingColumnIndex != -1 && !_executingLostFocusActions)
            {
                DataGridColumn editingColumn = this.ColumnsItemsInternal[this.EditingColumnIndex];
                FrameworkElement editingElement = editingColumn.GetCellContent(this.EditingRow);
                if (editingElement != null && editingElement.ContainsChild(_focusedObject))
                {
                    DiagnosticsDebug.Assert(_lostFocusActions != null, "Expected non-null _lostFocusActions.");
                    _lostFocusActions.Enqueue(action);
                    editingElement.LostFocus += new RoutedEventHandler(EditingElement_LostFocus);
                    this.IsTabStop = true;
                    this.Focus(FocusState.Programmatic);
                    return true;
                }
            }

            return false;
        }

        // Applies the given Style to the Row if it's supposed to use DataGrid.RowStyle
        private static void EnsureElementStyle(FrameworkElement element, Style oldDataGridStyle, Style newDataGridStyle)
        {
            DiagnosticsDebug.Assert(element != null, "Expected non-null element.");

            // Apply the DataGrid style if the row was using the old DataGridRowStyle before
            if (element != null && (element.Style == null || element.Style == oldDataGridStyle))
            {
                element.SetStyleWithType(newDataGridStyle);
            }
        }

        private bool AddNewItem(RoutedEventArgs editingEventArgs)
        {
#if FEATURE_IEDITABLECOLLECTIONVIEW
            if (this.DataConnection.EditableCollectionView != null && this.DataConnection.EditableCollectionView.CanAddNew)
            {
                _desiredCurrentColumnIndex = this.CurrentColumnIndex;
                object addItem = this.DataConnection.EditableCollectionView.AddNew();
                if (this.CurrentItem != this.DataConnection.EditableCollectionView.CurrentAddItem)
                {
                    int newItemSlot = SlotFromRowIndex(this.DataConnection.IndexOf(addItem));
                    SetAndSelectCurrentCell(this.CurrentColumnIndex, newItemSlot, true);
                    if (!_successfullyUpdatedSelection)
                    {
                        return false;
                    }
                }

                return BeginCellEdit(editingEventArgs);
            }
#endif

            return false;
        }

        private void AddNewCellPrivate(DataGridRow row, DataGridColumn column)
        {
            DataGridCell newCell = new DataGridCell();
            PopulateCellContent(false /*isCellEdited*/, column, row, newCell);
            if (row.OwningGrid != null)
            {
                newCell.OwningColumn = column;
                newCell.Visibility = column.Visibility;
            }

            if (column is DataGridFillerColumn)
            {
                Windows.UI.Xaml.Automation.AutomationProperties.SetAccessibilityView(
                    newCell,
                    AccessibilityView.Raw);
            }

            newCell.EnsureStyle(null);
            row.Cells.Insert(column.Index, newCell);
        }

        // TODO: Call this method once the UISettings has a public property for the "Automatically hide scroll bars in Windows" setting
        // private void AutoHideScrollBarsChanged()
        // {
        //    if (UISettingsHelper.AreSettingsAutoHidingScrollBars)
        //    {
        //        SwitchScrollBarsVisualStates(_proposedScrollBarsState, _proposedScrollBarsSeparatorState, true /*useTransitions*/);
        //    }
        //    else
        //    {
        //        if (this.AreBothScrollBarsVisible)
        //        {
        //            if (UISettingsHelper.AreSettingsEnablingAnimations)
        //            {
        //                SwitchScrollBarsVisualStates(ScrollBarVisualState.MouseIndicatorFull, this.IsEnabled ? ScrollBarsSeparatorVisualState.SeparatorExpanded : ScrollBarsSeparatorVisualState.SeparatorCollapsed, true /*useTransitions*/);
        //            }
        //            else
        //            {
        //                SwitchScrollBarsVisualStates(ScrollBarVisualState.MouseIndicatorFull, this.IsEnabled ? ScrollBarsSeparatorVisualState.SeparatorExpandedWithoutAnimation : ScrollBarsSeparatorVisualState.SeparatorCollapsed, true /*useTransitions*/);
        //            }
        //        }
        //        else
        //        {
        //            if (UISettingsHelper.AreSettingsEnablingAnimations)
        //            {
        //                SwitchScrollBarsVisualStates(ScrollBarVisualState.MouseIndicator, ScrollBarsSeparatorVisualState.SeparatorCollapsed, true/*useTransitions*/);
        //            }
        //            else
        //            {
        //                SwitchScrollBarsVisualStates(ScrollBarVisualState.MouseIndicator, this.IsEnabled ? ScrollBarsSeparatorVisualState.SeparatorCollapsedWithoutAnimation : ScrollBarsSeparatorVisualState.SeparatorCollapsed, true /*useTransitions*/);
        //            }
        //        }
        //    }
        // }
        private bool BeginCellEdit(RoutedEventArgs editingEventArgs)
        {
            if (this.CurrentColumnIndex == -1 || !GetRowSelection(this.CurrentSlot))
            {
                return false;
            }

            DiagnosticsDebug.Assert(this.CurrentColumnIndex >= 0, "Expected positive CurrentColumnIndex.");
            DiagnosticsDebug.Assert(this.CurrentColumnIndex < this.ColumnsItemsInternal.Count, "Expected CurrentColumnIndex smaller than ColumnsItemsInternal.Count.");
            DiagnosticsDebug.Assert(this.CurrentSlot >= -1, "Expected CurrentSlot greater than or equal to -1.");
            DiagnosticsDebug.Assert(this.CurrentSlot < this.SlotCount, "Expected CurrentSlot smaller than SlotCount.");
            DiagnosticsDebug.Assert(this.EditingRow == null || this.EditingRow.Slot == this.CurrentSlot, "Expected null EditingRow or EditingRow.Slot equal to CurrentSlot.");
            DiagnosticsDebug.Assert(!GetColumnEffectiveReadOnlyState(this.CurrentColumn), "Expected GetColumnEffectiveReadOnlyState(CurrentColumn) is false.");
            DiagnosticsDebug.Assert(this.CurrentColumn.IsVisible, "Expected CurrentColumn.IsVisible is true.");

            if (_editingColumnIndex != -1)
            {
                // Current cell is already in edit mode
                DiagnosticsDebug.Assert(_editingColumnIndex == this.CurrentColumnIndex, "Expected _editingColumnIndex equals CurrentColumnIndex.");
                return true;
            }

            // When we begin edit on the NewItemPlaceHolder row, we should try to add a new item.
            if (this.CurrentSlot == SlotFromRowIndex(this.DataConnection.NewItemPlaceholderIndex))
            {
                return this.AddNewItem(editingEventArgs);
            }

            // Get or generate the editing row if it doesn't exist
            DataGridRow dataGridRow = this.EditingRow;
            if (dataGridRow == null)
            {
                DiagnosticsDebug.Assert(!this.RowGroupHeadersTable.Contains(this.CurrentSlot), "Expected CurrentSlot not contained in RowGroupHeadersTable.");

                if (this.IsSlotVisible(this.CurrentSlot))
                {
                    dataGridRow = this.DisplayData.GetDisplayedElement(this.CurrentSlot) as DataGridRow;
                    DiagnosticsDebug.Assert(dataGridRow != null, "Expected non-null dataGridRow.");
                }
                else
                {
                    dataGridRow = GenerateRow(RowIndexFromSlot(this.CurrentSlot), this.CurrentSlot);
                    dataGridRow.Clip = new RectangleGeometry();
                }

                if (this.DataConnection.IsAddingNew)
                {
                    // We just began editing the new item row, so set a flag that prevents us from running
                    // full entity validation until the user explicitly attempts to end editing the row.
                    _initializingNewItem = true;
                }
            }

            DiagnosticsDebug.Assert(dataGridRow != null, "Expected non-null dataGridRow.");

            // Cache these to see if they change later
            int currentRowIndex = this.CurrentSlot;
            int currentColumnIndex = this.CurrentColumnIndex;

            // Raise the BeginningEdit event
            DataGridCell dataGridCell = dataGridRow.Cells[this.CurrentColumnIndex];
            DataGridBeginningEditEventArgs e = new DataGridBeginningEditEventArgs(this.CurrentColumn, dataGridRow, editingEventArgs);
            OnBeginningEdit(e);
            if (e.Cancel ||
                currentRowIndex != this.CurrentSlot ||
                currentColumnIndex != this.CurrentColumnIndex ||
                !GetRowSelection(this.CurrentSlot) ||
                (this.EditingRow == null && !BeginRowEdit(dataGridRow)))
            {
                // If either BeginningEdit was canceled, currency/selection was changed in the event handler,
                // or we failed opening the row for edit, then we can no longer continue BeginCellEdit
                return false;
            }

            if (this.EditingRow == null || this.EditingRow.Slot != this.CurrentSlot)
            {
                // This check was added to safeguard against a ListCollectionView bug where the collection changed currency
                // during a CommitNew operation but failed to raise a CurrentChanged event.
                return false;
            }

            // Finally, we can prepare the cell for editing
            _editingColumnIndex = this.CurrentColumnIndex;
            _editingEventArgs = editingEventArgs;
            this.EditingRow.Cells[this.CurrentColumnIndex].ApplyCellState(true /*animate*/);
            PopulateCellContent(true /*isCellEdited*/, this.CurrentColumn, dataGridRow, dataGridCell);
            return true;
        }

        private bool BeginRowEdit(DataGridRow dataGridRow)
        {
            DiagnosticsDebug.Assert(this.EditingRow == null, "Expected non-null EditingRow.");
            DiagnosticsDebug.Assert(dataGridRow != null, "Expected non-null dataGridRow.");

            DiagnosticsDebug.Assert(this.CurrentSlot >= -1, "Expected CurrentSlot greater than or equal to -1.");
            DiagnosticsDebug.Assert(this.CurrentSlot < this.SlotCount, "Expected CurrentSlot smaller than SlotCount.");

            if (this.DataConnection.BeginEdit(dataGridRow.DataContext))
            {
                this.EditingRow = dataGridRow;
                this.GenerateEditingElements();
                this.ValidateEditingRow(false /*scrollIntoView*/, true /*wireEvents*/);

                // Raise the automation invoke event for the row that just began edit
                DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
                if (peer != null && AutomationPeer.ListenerExists(AutomationEvents.InvokePatternOnInvoked))
                {
                    peer.RaiseAutomationInvokeEvents(DataGridEditingUnit.Row, null, dataGridRow);
                }

                return true;
            }

            return false;
        }

        private bool CancelRowEdit(bool exitEditingMode)
        {
            if (this.EditingRow == null)
            {
                return true;
            }

            DiagnosticsDebug.Assert(this.EditingRow != null, "Expected non-null EditingRow.");
            DiagnosticsDebug.Assert(this.EditingRow.Index >= -1, "Expected EditingRow greater or equal to -1.");
            DiagnosticsDebug.Assert(this.EditingRow.Slot < this.SlotCount, "Expected EditingRow smaller than SlotCount.");
            DiagnosticsDebug.Assert(this.CurrentColumn != null, "Expected non-null CurrentColumn.");

            object dataItem = this.EditingRow.DataContext;
            if (!this.DataConnection.CancelEdit(dataItem))
            {
                return false;
            }

            foreach (DataGridColumn column in this.Columns)
            {
                if (!exitEditingMode && column.Index == _editingColumnIndex && column is DataGridBoundColumn)
                {
                    continue;
                }

                PopulateCellContent(!exitEditingMode && column.Index == _editingColumnIndex /*isCellEdited*/, column, this.EditingRow, this.EditingRow.Cells[column.Index]);
            }

            return true;
        }

        private bool CommitEditForOperation(int columnIndex, int slot, bool forCurrentCellChange)
        {
            if (forCurrentCellChange)
            {
                if (!EndCellEdit(DataGridEditAction.Commit, true /*exitEditingMode*/, true /*keepFocus*/, true /*raiseEvents*/))
                {
                    return false;
                }

                if (this.CurrentSlot != slot &&
                    !EndRowEdit(DataGridEditAction.Commit, true /*exitEditingMode*/, true /*raiseEvents*/))
                {
                    return false;
                }
            }

            if (IsColumnOutOfBounds(columnIndex))
            {
                return false;
            }

            if (slot >= this.SlotCount)
            {
                // Current cell was reset because the commit deleted row(s).
                // Since the user wants to change the current cell, we don't
                // want to end up with no current cell. We pick the last row
                // in the grid which may be the 'new row'.
                int lastSlot = this.LastVisibleSlot;
                if (forCurrentCellChange &&
                    this.CurrentColumnIndex == -1 &&
                    lastSlot != -1)
                {
                    SetAndSelectCurrentCell(columnIndex, lastSlot, false /*forceCurrentCellSelection (unused here)*/);
                }

                // Interrupt operation because it has become invalid.
                return false;
            }

            return true;
        }

        private bool CommitRowEdit(bool exitEditingMode)
        {
            if (this.EditingRow == null)
            {
                return true;
            }

            DiagnosticsDebug.Assert(this.EditingRow != null, "Expected non-null EditingRow.");
            DiagnosticsDebug.Assert(this.EditingRow.Index >= -1, "Expected EditingRow.Index greater than or equal to -1.");
            DiagnosticsDebug.Assert(this.EditingRow.Slot < this.SlotCount, "Expected EditingRow.Slot smaller than SlotCount.");

            if (!ValidateEditingRow(true /*scrollIntoView*/, false /*wireEvents*/))
            {
                return false;
            }

            this.DataConnection.EndEdit(this.EditingRow.DataContext);

            if (!exitEditingMode)
            {
                this.DataConnection.BeginEdit(this.EditingRow.DataContext);
            }

            return true;
        }

        private void CompleteCellsCollection(DataGridRow dataGridRow)
        {
            DiagnosticsDebug.Assert(dataGridRow != null, "Expected non-null dataGridRow.");
            int cellsInCollection = dataGridRow.Cells.Count;
            if (this.ColumnsItemsInternal.Count > cellsInCollection)
            {
                for (int columnIndex = cellsInCollection; columnIndex < this.ColumnsItemsInternal.Count; columnIndex++)
                {
                    AddNewCellPrivate(dataGridRow, this.ColumnsItemsInternal[columnIndex]);
                }
            }
        }

        private void ComputeScrollBarsLayout()
        {
            if (_ignoreNextScrollBarsLayout)
            {
                _ignoreNextScrollBarsLayout = false;

                // TODO: This optimization is causing problems with initial layout:
                //       Investigate why horizontal ScrollBar sometimes has incorrect thumb size when
                //       it first appears after adding a row when this perf improvement is turned on.
                // return;
            }

            bool isHorizontalScrollBarOverCells = this.IsHorizontalScrollBarOverCells;
            bool isVerticalScrollBarOverCells = this.IsVerticalScrollBarOverCells;

            double cellsWidth = this.CellsWidth;
            double cellsHeight = this.CellsHeight;

            bool allowHorizScrollBar = false;
            bool forceHorizScrollBar = false;
            double horizScrollBarHeight = 0;
            if (_hScrollBar != null)
            {
                forceHorizScrollBar = this.HorizontalScrollBarVisibility == ScrollBarVisibility.Visible;
                allowHorizScrollBar = forceHorizScrollBar || (this.ColumnsInternal.VisibleColumnCount > 0 &&
                    this.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled &&
                    this.HorizontalScrollBarVisibility != ScrollBarVisibility.Hidden);

                // Compensate if the horizontal scrollbar is already taking up space
                if (!forceHorizScrollBar && _hScrollBar.Visibility == Visibility.Visible)
                {
                    if (!isHorizontalScrollBarOverCells)
                    {
                        cellsHeight += _hScrollBar.DesiredSize.Height;
                    }
                }

                if (!isHorizontalScrollBarOverCells)
                {
                    horizScrollBarHeight = _hScrollBar.Height + _hScrollBar.Margin.Top + _hScrollBar.Margin.Bottom;
                }
            }

            bool allowVertScrollBar = false;
            bool forceVertScrollBar = false;
            double vertScrollBarWidth = 0;
            if (_vScrollBar != null)
            {
                forceVertScrollBar = this.VerticalScrollBarVisibility == ScrollBarVisibility.Visible;
                allowVertScrollBar = forceVertScrollBar || (this.ColumnsItemsInternal.Count > 0 &&
                    this.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled &&
                    this.VerticalScrollBarVisibility != ScrollBarVisibility.Hidden);

                // Compensate if the vertical scrollbar is already taking up space
                if (!forceVertScrollBar && _vScrollBar.Visibility == Visibility.Visible)
                {
                    if (!isVerticalScrollBarOverCells)
                    {
                        cellsWidth += _vScrollBar.DesiredSize.Width;
                    }
                }

                if (!isVerticalScrollBarOverCells)
                {
                    vertScrollBarWidth = _vScrollBar.Width + _vScrollBar.Margin.Left + _vScrollBar.Margin.Right;
                }
            }

            // Now cellsWidth is the width potentially available for displaying data cells.
            // Now cellsHeight is the height potentially available for displaying data cells.
            bool needHorizScrollBar = false;
            bool needVertScrollBar = false;

            double totalVisibleWidth = this.ColumnsInternal.VisibleEdgedColumnsWidth;
            double totalVisibleFrozenWidth = this.ColumnsInternal.GetVisibleFrozenEdgedColumnsWidth();

            UpdateDisplayedRows(this.DisplayData.FirstScrollingSlot, this.CellsHeight);
            double totalVisibleHeight = this.EdgedRowsHeightCalculated;

            if (!forceHorizScrollBar && !forceVertScrollBar)
            {
                bool needHorizScrollBarWithoutVertScrollBar = false;

                if (allowHorizScrollBar &&
                    DoubleUtil.GreaterThan(totalVisibleWidth, cellsWidth) &&
                    DoubleUtil.LessThan(totalVisibleFrozenWidth, cellsWidth) &&
                    DoubleUtil.LessThanOrClose(horizScrollBarHeight, cellsHeight))
                {
                    double oldDataHeight = cellsHeight;
                    cellsHeight -= horizScrollBarHeight;
                    DiagnosticsDebug.Assert(cellsHeight >= 0, "Expected positive cellsHeight.");
                    needHorizScrollBarWithoutVertScrollBar = needHorizScrollBar = true;

                    if (vertScrollBarWidth > 0 &&
                        allowVertScrollBar &&
                        (DoubleUtil.LessThanOrClose(totalVisibleWidth - cellsWidth, vertScrollBarWidth) || DoubleUtil.LessThanOrClose(cellsWidth - totalVisibleFrozenWidth, vertScrollBarWidth)))
                    {
                        // Would we still need a horizontal scrollbar without the vertical one?
                        UpdateDisplayedRows(this.DisplayData.FirstScrollingSlot, cellsHeight);
                        if (this.DisplayData.NumTotallyDisplayedScrollingElements != this.VisibleSlotCount)
                        {
                            needHorizScrollBar = DoubleUtil.LessThan(totalVisibleFrozenWidth, cellsWidth - vertScrollBarWidth);
                        }

                        if (!needHorizScrollBar)
                        {
                            // Restore old data height because turns out a horizontal scroll bar wouldn't make sense
                            cellsHeight = oldDataHeight;
                        }
                    }
                }

                // Store the current FirstScrollingSlot because removing the horizontal scrollbar could scroll
                // the DataGrid up; however, if we realize later that we need to keep the horizontal scrollbar
                // then we should use the first slot stored here which is not scrolled.
                int firstScrollingSlot = this.DisplayData.FirstScrollingSlot;

                UpdateDisplayedRows(firstScrollingSlot, cellsHeight);
                if (allowVertScrollBar &&
                    DoubleUtil.GreaterThan(cellsHeight, 0) &&
                    DoubleUtil.LessThanOrClose(vertScrollBarWidth, cellsWidth) &&
                    this.DisplayData.NumTotallyDisplayedScrollingElements != this.VisibleSlotCount)
                {
                    cellsWidth -= vertScrollBarWidth;
                    DiagnosticsDebug.Assert(cellsWidth >= 0, "Expected positive cellsWidth.");
                    needVertScrollBar = true;
                }

                this.DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();

                // We compute the number of visible columns only after we set up the vertical scroll bar.
                ComputeDisplayedColumns();

                if ((vertScrollBarWidth > 0 || horizScrollBarHeight > 0) &&
                    allowHorizScrollBar &&
                    needVertScrollBar && !needHorizScrollBar &&
                    DoubleUtil.GreaterThan(totalVisibleWidth, cellsWidth) &&
                    DoubleUtil.LessThan(totalVisibleFrozenWidth, cellsWidth) &&
                    DoubleUtil.LessThanOrClose(horizScrollBarHeight, cellsHeight))
                {
                    cellsWidth += vertScrollBarWidth;
                    cellsHeight -= horizScrollBarHeight;
                    DiagnosticsDebug.Assert(cellsHeight >= 0, "Expected positive cellsHeight.");
                    needVertScrollBar = false;

                    UpdateDisplayedRows(firstScrollingSlot, cellsHeight);
                    if (cellsHeight > 0 &&
                        vertScrollBarWidth <= cellsWidth &&
                        this.DisplayData.NumTotallyDisplayedScrollingElements != this.VisibleSlotCount)
                    {
                        cellsWidth -= vertScrollBarWidth;
                        DiagnosticsDebug.Assert(cellsWidth >= 0, "Expected positive cellsWidth.");
                        needVertScrollBar = true;
                    }

                    if (needVertScrollBar)
                    {
                        needHorizScrollBar = true;
                    }
                    else
                    {
                        needHorizScrollBar = needHorizScrollBarWithoutVertScrollBar;
                    }
                }
            }
            else if (forceHorizScrollBar && !forceVertScrollBar)
            {
                if (allowVertScrollBar)
                {
                    if (cellsHeight > 0 &&
                        DoubleUtil.LessThanOrClose(vertScrollBarWidth, cellsWidth) &&
                        this.DisplayData.NumTotallyDisplayedScrollingElements != this.VisibleSlotCount)
                    {
                        cellsWidth -= vertScrollBarWidth;
                        DiagnosticsDebug.Assert(cellsWidth >= 0, "Expected positive cellsWidth.");
                        needVertScrollBar = true;
                    }

                    this.DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
                    ComputeDisplayedColumns();
                }

                needHorizScrollBar = totalVisibleWidth > cellsWidth && totalVisibleFrozenWidth < cellsWidth;
            }
            else if (!forceHorizScrollBar && forceVertScrollBar)
            {
                if (allowHorizScrollBar)
                {
                    if (cellsWidth > 0 &&
                        DoubleUtil.LessThanOrClose(horizScrollBarHeight, cellsHeight) &&
                        DoubleUtil.GreaterThan(totalVisibleWidth, cellsWidth) &&
                        DoubleUtil.LessThan(totalVisibleFrozenWidth, cellsWidth))
                    {
                        cellsHeight -= horizScrollBarHeight;
                        DiagnosticsDebug.Assert(cellsHeight >= 0, "Expected positive cellsHeight.");
                        needHorizScrollBar = true;
                        UpdateDisplayedRows(this.DisplayData.FirstScrollingSlot, cellsHeight);
                    }

                    this.DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
                    ComputeDisplayedColumns();
                }

                needVertScrollBar = this.DisplayData.NumTotallyDisplayedScrollingElements != this.VisibleSlotCount;
            }
            else
            {
                DiagnosticsDebug.Assert(forceHorizScrollBar, "Expected forceHorizScrollBar is true.");
                DiagnosticsDebug.Assert(forceVertScrollBar, "Expected forceVertScrollBar is true.");
                DiagnosticsDebug.Assert(allowHorizScrollBar, "Expected allowHorizScrollBar is true.");
                DiagnosticsDebug.Assert(allowVertScrollBar, "Expected allowVertScrollBar is true.");
                this.DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
                ComputeDisplayedColumns();
                needVertScrollBar = this.DisplayData.NumTotallyDisplayedScrollingElements != this.VisibleSlotCount;
                needHorizScrollBar = totalVisibleWidth > cellsWidth && totalVisibleFrozenWidth < cellsWidth;
            }

            UpdateHorizontalScrollBar(needHorizScrollBar, forceHorizScrollBar, totalVisibleWidth, totalVisibleFrozenWidth, cellsWidth);
            UpdateVerticalScrollBar(needVertScrollBar, forceVertScrollBar, totalVisibleHeight, cellsHeight);

            if (_topRightCornerHeader != null)
            {
                // Show the TopRightHeaderCell based on vertical ScrollBar visibility
                if (this.AreColumnHeadersVisible &&
                    _vScrollBar != null && _vScrollBar.Visibility == Visibility.Visible)
                {
                    _topRightCornerHeader.Visibility = Visibility.Visible;
                }
                else
                {
                    _topRightCornerHeader.Visibility = Visibility.Collapsed;
                }
            }

            if (_bottomRightCorner != null)
            {
                // Show the BottomRightCorner when both scrollbars are visible.
                _bottomRightCorner.Visibility =
                    _hScrollBar != null && _hScrollBar.Visibility == Visibility.Visible &&
                    _vScrollBar != null && _vScrollBar.Visibility == Visibility.Visible ?
                        Visibility.Visible : Visibility.Collapsed;
            }

            this.DisplayData.FullyRecycleElements();
        }

#if FEATURE_VALIDATION_SUMMARY
        /// <summary>
        /// Create an ValidationSummaryItem for a given ValidationResult, by finding all cells related to the
        /// validation error and adding them as separate ValidationSummaryItemSources.
        /// </summary>
        /// <param name="validationResult">ValidationResult</param>
        /// <returns>ValidationSummaryItem</returns>
        private ValidationSummaryItem CreateValidationSummaryItem(ValidationResult validationResult)
        {
            DiagnosticsDebug.Assert(validationResult != null);
            DiagnosticsDebug.Assert(_validationSummary != null);
            DiagnosticsDebug.Assert(this.EditingRow != null, "Expected non-null EditingRow.");

            ValidationSummaryItem validationSummaryItem = new ValidationSummaryItem(validationResult.ErrorMessage);
            validationSummaryItem.Context = validationResult;

            string messageHeader = null;
            foreach (DataGridColumn column in this.ColumnsInternal.GetDisplayedColumns(c => c.IsVisible && !c.IsReadOnly))
            {
                foreach (string property in validationResult.MemberNames)
                {
                    if (!string.IsNullOrEmpty(property) && column.BindingPaths.Contains(property))
                    {
                        validationSummaryItem.Sources.Add(new ValidationSummaryItemSource(property, this.EditingRow.Cells[column.Index]));
                        if (string.IsNullOrEmpty(messageHeader) && column.Header != null)
                        {
                            messageHeader = column.Header.ToString();
                        }
                    }
                }
            }

            DiagnosticsDebug.Assert(validationSummaryItem.ItemType == ValidationSummaryItemType.ObjectError);
            if (_propertyValidationResults.ContainsEqualValidationResult(validationResult))
            {
                validationSummaryItem.MessageHeader = messageHeader;
                validationSummaryItem.ItemType = ValidationSummaryItemType.PropertyError;
            }

            return validationSummaryItem;
        }
#endif

        /// <summary>
        /// Handles the current editing element's LostFocus event by performing any actions that
        /// were cached by the WaitForLostFocus method.
        /// </summary>
        /// <param name="sender">Editing element</param>
        /// <param name="e">RoutedEventArgs</param>
        private void EditingElement_LostFocus(object sender, RoutedEventArgs e)
        {
            FrameworkElement editingElement = sender as FrameworkElement;
            if (editingElement != null)
            {
                editingElement.LostFocus -= new RoutedEventHandler(EditingElement_LostFocus);
                if (this.EditingRow != null && this.EditingColumnIndex != -1)
                {
                    this.FocusEditingCell(true);
                }

                DiagnosticsDebug.Assert(_lostFocusActions != null, "Expected non-null _lostFocusActions.");
                try
                {
                    _executingLostFocusActions = true;
                    while (_lostFocusActions.Count > 0)
                    {
                        _lostFocusActions.Dequeue()();
                    }
                }
                finally
                {
                    _executingLostFocusActions = false;
                }
            }
        }

        // Makes sure horizontal layout is updated to reflect any changes that affect it
        private void EnsureHorizontalLayout()
        {
            this.ColumnsInternal.EnsureVisibleEdgedColumnsWidth();
            InvalidateColumnHeadersMeasure();
            InvalidateRowsMeasure(true);
            InvalidateMeasure();
        }

        /// <summary>
        /// Ensures that the RowHeader widths are properly sized and invalidates them if they are not
        /// </summary>
        /// <returns>True if a RowHeader or RowGroupHeader was invalidated</returns>
        private bool EnsureRowHeaderWidth()
        {
            bool invalidated = false;
            if (this.AreRowHeadersVisible)
            {
                if (this.AreColumnHeadersVisible)
                {
                    EnsureTopLeftCornerHeader();
                }

                if (_rowsPresenter != null)
                {
                    foreach (UIElement element in _rowsPresenter.Children)
                    {
                        DataGridRow row = element as DataGridRow;
                        if (row != null)
                        {
                            // If the RowHeader resulted in a different width the last time it was measured, we need
                            // to re-measure it
                            if (row.HeaderCell != null && row.HeaderCell.DesiredSize.Width != this.ActualRowHeaderWidth)
                            {
                                row.HeaderCell.InvalidateMeasure();
                                invalidated = true;
                            }
                        }
                        else
                        {
                            DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                            if (groupHeader != null && groupHeader.HeaderCell != null && groupHeader.HeaderCell.DesiredSize.Width != this.ActualRowHeaderWidth)
                            {
                                groupHeader.HeaderCell.InvalidateMeasure();
                                invalidated = true;
                            }
                        }
                    }

                    if (invalidated)
                    {
                        // We need to update the width of the horizontal scrollbar if the rowHeaders' width actually changed
                        if (this.ColumnsInternal.VisibleStarColumnCount > 0)
                        {
                            this.ColumnsInternal.EnsureVisibleEdgedColumnsWidth();
                        }

                        InvalidateMeasure();
                    }
                }
            }

            return invalidated;
        }

        private void EnsureRowsPresenterVisibility()
        {
            if (_rowsPresenter != null)
            {
                // RowCount doesn't need to be considered, doing so might cause extra Visibility changes
                _rowsPresenter.Visibility = this.ColumnsInternal.FirstVisibleNonFillerColumn == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void EnsureTopLeftCornerHeader()
        {
            if (_topLeftCornerHeader != null)
            {
                _topLeftCornerHeader.Visibility = this.HeadersVisibility == DataGridHeadersVisibility.All ? Visibility.Visible : Visibility.Collapsed;

                if (_topLeftCornerHeader.Visibility == Visibility.Visible)
                {
                    if (!double.IsNaN(this.RowHeaderWidth))
                    {
                        // RowHeaderWidth is set explicitly so we should use that
                        _topLeftCornerHeader.Width = this.RowHeaderWidth;
                    }
                    else if (this.VisibleSlotCount > 0)
                    {
                        // RowHeaders AutoSize and we have at least 1 row so take the desired width
                        _topLeftCornerHeader.Width = this.RowHeadersDesiredWidth;
                    }
                }
            }
        }

#if FEATURE_VALIDATION_SUMMARY
        /// <summary>
        /// Handles the ValidationSummary's FocusingInvalidControl event and begins edit on the cells
        /// that are associated with the selected error.
        /// </summary>
        /// <param name="sender">ValidationSummary</param>
        /// <param name="e">FocusingInvalidControlEventArgs</param>
        private void ValidationSummary_FocusingInvalidControl(object sender, FocusingInvalidControlEventArgs e)
        {
            DiagnosticsDebug.Assert(_validationSummary != null);
            if (this.EditingRow == null || this.IsSlotOutOfBounds(this.EditingRow.Slot) || this.EditingRow.Slot == -1 || !ScrollSlotIntoView(this.EditingRow.Slot, false /*scrolledHorizontally*/))
            {
                return;
            }

            // We need to focus the DataGrid in case the focused element gets removed when we end edit.
            if ((_editingColumnIndex == -1 || (this.Focus(FocusState.Programmatic) && EndCellEdit(DataGridEditAction.Commit, true, true, true)))
                && e.Item != null && e.Target != null && _validationSummary.Errors.Contains(e.Item))
            {
                DataGridCell cell = e.Target.Control as DataGridCell;
                if (cell != null && cell.OwningGrid == this && cell.OwningColumn != null && cell.OwningColumn.IsVisible)
                {
                    DiagnosticsDebug.Assert(cell.ColumnIndex >= 0 && cell.ColumnIndex < this.ColumnsInternal.Count);

                    // Begin editing the next relevant cell
                    UpdateSelectionAndCurrency(cell.ColumnIndex, this.EditingRow.Slot, DataGridSelectionAction.None, true /*scrollIntoView*/);
                    if (_successfullyUpdatedSelection)
                    {
                        BeginCellEdit(new RoutedEventArgs());
                        if (!IsColumnDisplayed(this.CurrentColumnIndex))
                        {
                            ScrollColumnIntoView(this.CurrentColumnIndex);
                        }
                    }
                }

                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the ValidationSummary's SelectionChanged event and changes which cells are displayed as invalid.
        /// </summary>
        /// <param name="sender">ValidationSummary</param>
        /// <param name="e">SelectionChangedEventArgs</param>
        private void ValidationSummary_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ValidationSummary only supports single-selection mode.
            if (e.AddedItems.Count == 1)
            {
                _selectedValidationSummaryItem = e.AddedItems[0] as ValidationSummaryItem;
            }

            this.UpdateValidationStatus();
        }
#endif

        // Recursively expands parent RowGroupHeaders from the top down
        private void ExpandRowGroupParentChain(int level, int slot)
        {
            if (level < 0)
            {
                return;
            }

            int previousHeaderSlot = this.RowGroupHeadersTable.GetPreviousIndex(slot + 1);
            while (previousHeaderSlot >= 0)
            {
                DataGridRowGroupInfo rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(previousHeaderSlot);
                DiagnosticsDebug.Assert(rowGroupInfo != null, "Expected non-null rowGroupInfo.");
                if (level == rowGroupInfo.Level)
                {
                    if (_collapsedSlotsTable.Contains(rowGroupInfo.Slot))
                    {
                        // Keep going up the chain
                        ExpandRowGroupParentChain(level - 1, rowGroupInfo.Slot - 1);
                    }

                    if (rowGroupInfo.Visibility != Visibility.Visible)
                    {
                        EnsureRowGroupVisibility(rowGroupInfo, Visibility.Visible, false);
                    }

                    return;
                }
                else
                {
                    previousHeaderSlot = this.RowGroupHeadersTable.GetPreviousIndex(previousHeaderSlot);
                }
            }
        }

#if FEATURE_VALIDATION_SUMMARY
        /// <summary>
        /// Searches through the DataGrid's ValidationSummary for any errors that use the given
        /// ValidationResult as the ValidationSummaryItem's Context value.
        /// </summary>
        /// <param name="context">ValidationResult</param>
        /// <returns>ValidationSummaryItem or null if not found</returns>
        private ValidationSummaryItem FindValidationSummaryItem(ValidationResult context)
        {
            DiagnosticsDebug.Assert(context != null);
            DiagnosticsDebug.Assert(_validationSummary != null);
            foreach (ValidationSummaryItem ValidationSummaryItem in _validationSummary.Errors)
            {
                if (context.Equals(ValidationSummaryItem.Context))
                {
                    return ValidationSummaryItem;
                }
            }

            return null;
        }
#endif

        private void InvalidateCellsArrange()
        {
            foreach (DataGridRow row in GetAllRows())
            {
                row.InvalidateHorizontalArrange();
            }
        }

        private void InvalidateColumnHeadersArrange()
        {
            if (_columnHeadersPresenter != null)
            {
                _columnHeadersPresenter.InvalidateArrange();
            }
        }

        private void InvalidateColumnHeadersMeasure()
        {
            if (_columnHeadersPresenter != null)
            {
                EnsureColumnHeadersVisibility();
                _columnHeadersPresenter.InvalidateMeasure();
            }
        }

        private void InvalidateRowsArrange()
        {
            if (_rowsPresenter != null)
            {
                _rowsPresenter.InvalidateArrange();
            }
        }

        private void InvalidateRowsMeasure(bool invalidateIndividualElements)
        {
            if (_rowsPresenter != null)
            {
                _rowsPresenter.InvalidateMeasure();

                if (invalidateIndividualElements)
                {
                    foreach (UIElement element in _rowsPresenter.Children)
                    {
                        element.InvalidateMeasure();
                    }
                }
            }
        }

        private void DataGrid_GettingFocus(UIElement sender, GettingFocusEventArgs e)
        {
            _focusInputDevice = e.InputDevice;
        }

        private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!this.ContainsFocus)
            {
                this.ContainsFocus = true;
                ApplyDisplayedRowsState(this.DisplayData.FirstScrollingSlot, this.DisplayData.LastScrollingSlot);
                if (this.CurrentColumnIndex != -1 && this.IsSlotVisible(this.CurrentSlot))
                {
                    UpdateCurrentState(this.DisplayData.GetDisplayedElement(this.CurrentSlot), this.CurrentColumnIndex, true /*applyCellState*/);
                }
            }

            DependencyObject focusedElement = e.OriginalSource as DependencyObject;
            _focusedObject = focusedElement;
            while (focusedElement != null)
            {
                // Keep track of which row contains the newly focused element
                var focusedRow = focusedElement as DataGridRow;
                if (focusedRow != null && focusedRow.OwningGrid == this && _focusedRow != focusedRow)
                {
                    ResetFocusedRow();
                    _focusedRow = focusedRow.Visibility == Visibility.Visible ? focusedRow : null;
                    break;
                }

                focusedElement = VisualTreeHelper.GetParent(focusedElement);
            }

            _preferMouseIndicators = _focusInputDevice == FocusInputDeviceKind.Mouse || _focusInputDevice == FocusInputDeviceKind.Pen;

            ShowScrollBars();

            // If the DataGrid itself got focus, we actually want the automation focus to be on the current element
            if (e.OriginalSource == this && AutomationPeer.ListenerExists(AutomationEvents.AutomationFocusChanged))
            {
                DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseAutomationFocusChangedEvent(this.CurrentSlot, this.CurrentColumnIndex);
                }
            }
        }

        private void DataGrid_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateDisabledVisual();

            if (!this.IsEnabled)
            {
                HideScrollBars(true /*useTransitions*/);
            }
        }

        private void DataGrid_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = ProcessDataGridKey(e);
                this.LastHandledKeyDown = e.Handled ? e.Key : VirtualKey.None;
            }
        }

        private void DataGrid_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Tab && e.OriginalSource == this)
            {
                if (this.CurrentColumnIndex == -1)
                {
                    if (this.ColumnHeaders != null && this.AreColumnHeadersVisible && !this.ColumnHeaderHasFocus)
                    {
                        this.ColumnHeaderHasFocus = true;
                    }
                }
                else
                {
                    if (this.ColumnHeaders != null && this.AreColumnHeadersVisible)
                    {
                        KeyboardHelper.GetMetaKeyState(out _, out var shift);

                        if (shift && this.LastHandledKeyDown != VirtualKey.Tab)
                        {
                            DiagnosticsDebug.Assert(!this.ColumnHeaderHasFocus, "Expected ColumnHeaderHasFocus is false.");

                            // Show currency on the current column's header as focus is entering the DataGrid backwards.
                            this.ColumnHeaderHasFocus = true;
                        }
                    }

                    bool success = ScrollSlotIntoView(this.CurrentColumnIndex, this.CurrentSlot, false /*forCurrentCellChange*/, true /*forceHorizontalScroll*/);
                    DiagnosticsDebug.Assert(success, "Expected ScrollSlotIntoView returns true.");
                    if (this.CurrentColumnIndex != -1 && this.SelectedItem == null)
                    {
                        SetRowSelection(this.CurrentSlot, true /*isSelected*/, true /*setAnchorSlot*/);
                    }
                }
            }
        }

        private void DataGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            _focusedObject = null;
            if (this.ContainsFocus)
            {
                bool focusLeftDataGrid = true;
                bool dataGridWillReceiveRoutedEvent = true;

                // Walk up the visual tree of the newly focused element
                // to determine if focus is still within DataGrid.
                object focusedObject = GetFocusedElement();
                DependencyObject focusedDependencyObject = focusedObject as DependencyObject;

                while (focusedDependencyObject != null)
                {
                    if (focusedDependencyObject == this)
                    {
                        focusLeftDataGrid = false;
                        break;
                    }

                    // Walk up the visual tree. Try using the framework element's
                    // parent.  We do this because Popups behave differently with respect to the visual tree,
                    // and it could have a parent even if the VisualTreeHelper doesn't find it.
                    DependencyObject parent = null;
                    FrameworkElement element = focusedDependencyObject as FrameworkElement;
                    if (element == null)
                    {
                        parent = VisualTreeHelper.GetParent(focusedDependencyObject);
                    }
                    else
                    {
                        parent = element.Parent;
                        if (parent == null)
                        {
                            parent = VisualTreeHelper.GetParent(focusedDependencyObject);
                        }
                        else
                        {
                            dataGridWillReceiveRoutedEvent = false;
                        }
                    }

                    focusedDependencyObject = parent;
                }

                if (focusLeftDataGrid)
                {
                    this.ContainsFocus = false;
                    if (this.EditingRow != null)
                    {
                        CommitEdit(DataGridEditingUnit.Row, true /*exitEditingMode*/);
                    }

                    ResetFocusedRow();
                    ApplyDisplayedRowsState(this.DisplayData.FirstScrollingSlot, this.DisplayData.LastScrollingSlot);
                    if (this.ColumnHeaderHasFocus)
                    {
                        this.ColumnHeaderHasFocus = false;
                    }
                    else if (this.CurrentColumnIndex != -1 && this.IsSlotVisible(this.CurrentSlot))
                    {
                        UpdateCurrentState(this.DisplayData.GetDisplayedElement(this.CurrentSlot), this.CurrentColumnIndex, true /*applyCellState*/);
                    }
                }
                else if (!dataGridWillReceiveRoutedEvent)
                {
                    FrameworkElement focusedElement = focusedObject as FrameworkElement;
                    if (focusedElement != null)
                    {
                        focusedElement.LostFocus += new RoutedEventHandler(ExternalEditingElement_LostFocus);
                    }
                }
            }
        }

        private object GetFocusedElement()
        {
            if (TypeHelper.IsXamlRootAvailable && XamlRoot != null)
            {
                return FocusManager.GetFocusedElement(XamlRoot);
            }
            else
            {
                return FocusManager.GetFocusedElement();
            }
        }

        private void DataGrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType != PointerDeviceType.Touch)
            {
                // Mouse/Pen inputs dominate. If touch panning indicators are shown, switch to mouse indicators.
                _preferMouseIndicators = true;
                ShowScrollBars();
            }
        }

        private void DataGrid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType != PointerDeviceType.Touch)
            {
                // Mouse/Pen inputs dominate. If touch panning indicators are shown, switch to mouse indicators.
                _isPointerOverHorizontalScrollBar = false;
                _isPointerOverVerticalScrollBar = false;
                _preferMouseIndicators = true;
                ShowScrollBars();
                HideScrollBarsAfterDelay();
            }
        }

        private void DataGrid_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            // Don't process if this is a generated replay of the event.
            if (e.IsGenerated)
            {
                return;
            }

            if (e.Pointer.PointerDeviceType != PointerDeviceType.Touch)
            {
                // Mouse/Pen inputs dominate. If touch panning indicators are shown, switch to mouse indicators.
                _preferMouseIndicators = true;
                ShowScrollBars();

                if (!UISettingsHelper.AreSettingsEnablingAnimations &&
                    _hideScrollBarsTimer != null &&
                    (_isPointerOverHorizontalScrollBar || _isPointerOverVerticalScrollBar))
                {
                    StopHideScrollBarsTimer();
                }
            }
        }

        private void DataGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            // Show the scroll bars as soon as a pointer is pressed on the DataGrid.
            ShowScrollBars();
        }

        private void DataGrid_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (this.CurrentColumnIndex != -1 && this.CurrentSlot != -1)
            {
                e.Handled = true;
            }
        }

        private void DataGrid_Unloaded(object sender, RoutedEventArgs e)
        {
            _showingMouseIndicators = false;
            _keepScrollBarsShowing = false;
        }

#if FEATURE_VALIDATION
        private void EditingElement_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added && e.Error.Exception != null && e.Error.ErrorContent != null)
            {
                ValidationResult validationResult = new ValidationResult(e.Error.ErrorContent.ToString(), new List<string>() { _updateSourcePath });
                _bindingValidationResults.AddIfNew(validationResult);
            }
        }
#endif

        private void EditingElement_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                element.Loaded -= new RoutedEventHandler(EditingElement_Loaded);
            }

            PreparingCellForEditPrivate(element);
        }

        private bool EndCellEdit(DataGridEditAction editAction, bool exitEditingMode, bool keepFocus, bool raiseEvents)
        {
            if (_editingColumnIndex == -1)
            {
                return true;
            }

            DiagnosticsDebug.Assert(this.EditingRow != null, "Expected non-null EditingRow.");
            DiagnosticsDebug.Assert(this.EditingRow.Slot == this.CurrentSlot, "Expected EditingRow.Slot equals CurrentSlot.");
            DiagnosticsDebug.Assert(_editingColumnIndex >= 0, "Expected positive _editingColumnIndex.");
            DiagnosticsDebug.Assert(_editingColumnIndex < this.ColumnsItemsInternal.Count, "Expected _editingColumnIndex smaller than this.ColumnsItemsInternal.Count.");
            DiagnosticsDebug.Assert(_editingColumnIndex == this.CurrentColumnIndex, "Expected _editingColumnIndex equals this.CurrentColumnIndex.");

            // Cache these to see if they change later
            int currentSlot = this.CurrentSlot;
            int currentColumnIndex = this.CurrentColumnIndex;

            // We're ready to start ending, so raise the event
            DataGridCell editingCell = this.EditingRow.Cells[_editingColumnIndex];
            FrameworkElement editingElement = editingCell.Content as FrameworkElement;
            if (editingElement == null)
            {
                return false;
            }

            if (raiseEvents)
            {
                DataGridCellEditEndingEventArgs e = new DataGridCellEditEndingEventArgs(this.CurrentColumn, this.EditingRow, editingElement, editAction);
                OnCellEditEnding(e);
                if (e.Cancel)
                {
                    // CellEditEnding has been canceled
                    return false;
                }

                // Ensure that the current cell wasn't changed in the user's CellEditEnding handler
                if (_editingColumnIndex == -1 ||
                    currentSlot != this.CurrentSlot ||
                    currentColumnIndex != this.CurrentColumnIndex)
                {
                    return true;
                }

                DiagnosticsDebug.Assert(this.EditingRow != null, "Expected non-null EditingRow.");
                DiagnosticsDebug.Assert(this.EditingRow.Slot == currentSlot, "Expected EditingRow.Slot equals currentSlot.");
                DiagnosticsDebug.Assert(_editingColumnIndex != -1, "Expected _editingColumnIndex other than -1.");
                DiagnosticsDebug.Assert(_editingColumnIndex == this.CurrentColumnIndex, "Expected _editingColumnIndex equals CurrentColumnIndex.");
            }

            _bindingValidationResults.Clear();

            // If we're canceling, let the editing column repopulate its old value if it wants
            if (editAction == DataGridEditAction.Cancel)
            {
                this.CurrentColumn.CancelCellEditInternal(editingElement, _uneditedValue);

                // Ensure that the current cell wasn't changed in the user column's CancelCellEdit
                if (_editingColumnIndex == -1 ||
                    currentSlot != this.CurrentSlot ||
                    currentColumnIndex != this.CurrentColumnIndex)
                {
                    return true;
                }

                DiagnosticsDebug.Assert(this.EditingRow != null, "Expected non-null EditingRow.");
                DiagnosticsDebug.Assert(this.EditingRow.Slot == currentSlot, "Expected EditingRow.Slot equals currentSlot.");
                DiagnosticsDebug.Assert(_editingColumnIndex != -1, "Expected _editingColumnIndex other than -1.");
                DiagnosticsDebug.Assert(_editingColumnIndex == this.CurrentColumnIndex, "Expected _editingColumnIndex equals CurrentColumnIndex.");

                // Re-validate
                this.ValidateEditingRow(true /*scrollIntoView*/, false /*wireEvents*/);
            }

            // If we're committing, explicitly update the source but watch out for any validation errors
            if (editAction == DataGridEditAction.Commit)
            {
                foreach (BindingInfo bindingData in this.CurrentColumn.GetInputBindings(editingElement, this.CurrentItem))
                {
                    DiagnosticsDebug.Assert(bindingData.BindingExpression.ParentBinding != null, "Expected non-null bindingData.BindingExpression.ParentBinding.");
                    _updateSourcePath = bindingData.BindingExpression.ParentBinding.Path != null ? bindingData.BindingExpression.ParentBinding.Path.Path : null;
#if FEATURE_VALIDATION
                    bindingData.Element.BindingValidationError += new EventHandler<ValidationErrorEventArgs>(EditingElement_BindingValidationError);
#endif
                    try
                    {
                        bindingData.BindingExpression.UpdateSource();
                    }
                    finally
                    {
#if FEATURE_VALIDATION
                    bindingData.Element.BindingValidationError -= new EventHandler<ValidationErrorEventArgs>(EditingElement_BindingValidationError);
#endif
                    }
                }

                // Re-validate
                this.ValidateEditingRow(true /*scrollIntoView*/, false /*wireEvents*/);

                if (_bindingValidationResults.Count > 0)
                {
                    ScrollSlotIntoView(this.CurrentColumnIndex, this.CurrentSlot, false /*forCurrentCellChange*/, true /*forceHorizontalScroll*/);
                    return false;
                }
            }

            if (exitEditingMode)
            {
                _editingColumnIndex = -1;
                editingCell.ApplyCellState(true /*animate*/);

                // TODO: Figure out if we should restore a cached this.IsTabStop.
                this.IsTabStop = true;
                if (keepFocus && editingElement.ContainsFocusedElement(this))
                {
                    this.Focus(FocusState.Programmatic);
                }

                PopulateCellContent(!exitEditingMode /*isCellEdited*/, this.CurrentColumn, this.EditingRow, editingCell);
            }

            // We're done, so raise the CellEditEnded event
            if (raiseEvents)
            {
                OnCellEditEnded(new DataGridCellEditEndedEventArgs(this.CurrentColumn, this.EditingRow, editAction));
            }

            // There's a chance that somebody reopened this cell for edit within the CellEditEnded handler,
            // so we should return false if we were supposed to exit editing mode, but we didn't
            return !(exitEditingMode && currentColumnIndex == _editingColumnIndex);
        }

        private bool EndRowEdit(DataGridEditAction editAction, bool exitEditingMode, bool raiseEvents)
        {
            // Explicit row end edit has been triggered, so we can no longer be initializing a new item.
            _initializingNewItem = false;

            if (this.EditingRow == null || this.DataConnection.EndingEdit)
            {
                return true;
            }

            if (_editingColumnIndex != -1 || (editAction == DataGridEditAction.Cancel && raiseEvents &&
                !(this.DataConnection.CanCancelEdit || this.EditingRow.DataContext is IEditableObject || this.DataConnection.IsAddingNew)))
            {
                // Ending the row edit will fail immediately under the following conditions:
                // 1. We haven't ended the cell edit yet.
                // 2. We're trying to cancel edit when the underlying DataType is not an IEditableObject,
                //    because we have no way to properly restore the old value.  We will only allow this to occur if:
                //    a. raiseEvents == false, which means we're internally forcing a cancel or
                //    b. we're canceling a new item addition.
                return false;
            }

            DataGridRow editingRow = this.EditingRow;

            if (raiseEvents)
            {
                DataGridRowEditEndingEventArgs e = new DataGridRowEditEndingEventArgs(this.EditingRow, editAction);
                OnRowEditEnding(e);
                if (e.Cancel)
                {
                    // RowEditEnding has been canceled
                    return false;
                }

                // Editing states might have been changed in the RowEditEnding handlers
                if (_editingColumnIndex != -1)
                {
                    return false;
                }

                if (editingRow != this.EditingRow)
                {
                    return true;
                }
            }

            // Call the appropriate commit or cancel methods
            if (editAction == DataGridEditAction.Commit)
            {
                if (!CommitRowEdit(exitEditingMode))
                {
                    return false;
                }
            }
            else
            {
                if (!CancelRowEdit(exitEditingMode) && raiseEvents)
                {
                    // We failed to cancel edit so we should abort unless we're forcing a cancel
                    return false;
                }
            }

            ResetValidationStatus();

            // Update the previously edited row's state
            if (exitEditingMode && editingRow == this.EditingRow)
            {
                // Unwire the INDEI event handlers
                foreach (INotifyDataErrorInfo indei in _validationItems.Keys)
                {
                    indei.ErrorsChanged -= new EventHandler<DataErrorsChangedEventArgs>(ValidationItem_ErrorsChanged);
                }

                _validationItems.Clear();
                this.RemoveEditingElements();
                ResetEditingRow();
            }

            if (this.CurrentSlot == -1 && this.DataConnection.CollectionView != null && this.DataConnection.CollectionView.CurrentItem != null)
            {
                // Some EditableCollectionViews (ListCollectionView in particular) do not raise CurrentChanged when CommitEdit
                // changes the position of the CurrentItem.  Instead, they raise a PropertyChanged event for PositionChanged.
                // We recognize that case here and setup the CurrentItem again if one exists but it was removed and re-added
                // during Commit.  This is better than reacting to PositionChanged which would double the work in most cases
                // and likely introduce regressions.
                UpdateStateOnCurrentChanged(this.DataConnection.CollectionView.CurrentItem, this.DataConnection.CollectionView.CurrentPosition);
            }

            // Raise the RowEditEnded event
            if (raiseEvents)
            {
                OnRowEditEnded(new DataGridRowEditEndedEventArgs(editingRow, editAction));
            }

            return true;
        }

        private void EnsureColumnHeadersVisibility()
        {
            if (_columnHeadersPresenter != null)
            {
                _columnHeadersPresenter.Visibility = this.AreColumnHeadersVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void EnsureVerticalGridLines()
        {
            if (this.AreColumnHeadersVisible)
            {
                double totalColumnsWidth = 0;
                foreach (DataGridColumn column in this.ColumnsInternal)
                {
                    totalColumnsWidth += column.ActualWidth;

                    column.HeaderCell.SeparatorVisibility = (column != this.ColumnsInternal.LastVisibleColumn || totalColumnsWidth < this.CellsWidth) ?
                        Visibility.Visible : Visibility.Collapsed;
                }
            }

            foreach (DataGridRow row in GetAllRows())
            {
                row.EnsureGridLines();
            }
        }

        /// <summary>
        /// Exits editing mode without trying to commit or revert the editing, and
        /// without repopulating the edited row's cell.
        /// </summary>
        private void ExitEdit(bool keepFocus)
        {
            // We're exiting editing mode, so we can no longer be initializing a new item.
            _initializingNewItem = false;

            if (this.EditingRow == null || this.DataConnection.EndingEdit)
            {
                DiagnosticsDebug.Assert(_editingColumnIndex == -1, "Expected _editingColumnIndex equal to -1.");
                return;
            }

            if (_editingColumnIndex != -1)
            {
                DiagnosticsDebug.Assert(this.EditingRow != null, "Expected non-null EditingRow.");
                DiagnosticsDebug.Assert(this.EditingRow.Slot == this.CurrentSlot, "Expected EditingRow.Slot equals CurrentSlot.");
                DiagnosticsDebug.Assert(_editingColumnIndex >= 0, "Expected positive _editingColumnIndex.");
                DiagnosticsDebug.Assert(_editingColumnIndex < this.ColumnsItemsInternal.Count, "Expected _editingColumnIndex smaller than this.ColumnsItemsInternal.Count.");
                DiagnosticsDebug.Assert(_editingColumnIndex == this.CurrentColumnIndex, "Expected _editingColumnIndex equals CurrentColumnIndex.");

                _editingColumnIndex = -1;
                this.EditingRow.Cells[this.CurrentColumnIndex].ApplyCellState(false /*animate*/);
            }

            // TODO: Figure out if we should restore a cached this.IsTabStop.
            this.IsTabStop = true;
            if (this.IsSlotVisible(this.EditingRow.Slot))
            {
                this.EditingRow.ApplyState(true /*animate*/);
            }

            ResetEditingRow();
            if (keepFocus)
            {
                bool success = Focus(FocusState.Programmatic);
                DiagnosticsDebug.Assert(success, "Expected successful Focus call.");
            }
        }

        private void ExternalEditingElement_LostFocus(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                element.LostFocus -= new RoutedEventHandler(ExternalEditingElement_LostFocus);
                DataGrid_LostFocus(sender, e);
            }
        }

        private void FlushCurrentCellChanged()
        {
            if (_makeFirstDisplayedCellCurrentCellPending)
            {
                return;
            }

            if (this.SelectionHasChanged)
            {
                // selection is changing, don't raise CurrentCellChanged until it's done
                _flushCurrentCellChanged = true;
                FlushSelectionChanged();
                return;
            }

            // We don't want to expand all intermediate currency positions, so we only expand
            // the last current item before we flush the event
            if (_collapsedSlotsTable.Contains(this.CurrentSlot) && this.CurrentSlot != this.SlotFromRowIndex(this.DataConnection.NewItemPlaceholderIndex))
            {
                DataGridRowGroupInfo rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(this.RowGroupHeadersTable.GetPreviousIndex(this.CurrentSlot));
                DiagnosticsDebug.Assert(rowGroupInfo != null, "Expected non-null rowGroupInfo.");
                if (rowGroupInfo != null)
                {
                    this.ExpandRowGroupParentChain(rowGroupInfo.Level, rowGroupInfo.Slot);
                }
            }

            if (this.CurrentColumn != _previousCurrentColumn || this.CurrentItem != _previousCurrentItem)
            {
                this.CoerceSelectedItem();
                _previousCurrentColumn = this.CurrentColumn;
                _previousCurrentItem = this.CurrentItem;

                OnCurrentCellChanged(EventArgs.Empty);
            }

            DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
            if (peer != null && this.CurrentCellCoordinates != _previousAutomationFocusCoordinates)
            {
                _previousAutomationFocusCoordinates = new DataGridCellCoordinates(this.CurrentCellCoordinates);

                // If the DataGrid itself has focus, we want to move automation focus to the new current element
                object focusedObject = GetFocusedElement();
                if (focusedObject == this && AutomationPeer.ListenerExists(AutomationEvents.AutomationFocusChanged))
                {
                    peer.RaiseAutomationFocusChangedEvent(this.CurrentSlot, this.CurrentColumnIndex);
                }
            }

            _flushCurrentCellChanged = false;
        }

        private void FlushSelectionChanged()
        {
            if (this.SelectionHasChanged && _noSelectionChangeCount == 0 && !_makeFirstDisplayedCellCurrentCellPending)
            {
                this.CoerceSelectedItem();
                if (this.NoCurrentCellChangeCount != 0)
                {
                    // current cell is changing, don't raise SelectionChanged until it's done
                    return;
                }

                this.SelectionHasChanged = false;

                if (_flushCurrentCellChanged)
                {
                    FlushCurrentCellChanged();
                }

                SelectionChangedEventArgs e = _selectedItems.GetSelectionChangedEventArgs();
                if (e.AddedItems.Count > 0 || e.RemovedItems.Count > 0)
                {
                    OnSelectionChanged(e);
                }
            }
        }

        private bool FocusEditingCell(bool setFocus)
        {
            DiagnosticsDebug.Assert(this.CurrentColumnIndex >= 0, "Expected positive CurrentColumnIndex.");
            DiagnosticsDebug.Assert(this.CurrentColumnIndex < this.ColumnsItemsInternal.Count, "Expected CurrentColumnIndex smaller than ColumnsItemsInternal.Count.");
            DiagnosticsDebug.Assert(this.CurrentSlot >= -1, "Expected CurrentSlot greater than or equal to -1.");
            DiagnosticsDebug.Assert(this.CurrentSlot < this.SlotCount, "Expected CurrentSlot smaller than SlotCount.");
            DiagnosticsDebug.Assert(this.EditingRow != null, "Expected non-null EditingRow.");
            DiagnosticsDebug.Assert(this.EditingRow.Slot == this.CurrentSlot, "Expected EditingRow.Slot equals CurrentSlot.");
            DiagnosticsDebug.Assert(_editingColumnIndex != -1, "Expected _editingColumnIndex other than -1.");

            // TODO: Figure out if we should cache this.IsTabStop in order to restore
            //       it later instead of setting it back to true unconditionally.
            this.IsTabStop = false;
            _focusEditingControl = false;

            bool success = false;
            DataGridCell dataGridCell = this.EditingRow.Cells[_editingColumnIndex];
            if (setFocus)
            {
                if (dataGridCell.ContainsFocusedElement(this))
                {
                    success = true;
                }
                else
                {
                    success = dataGridCell.Focus(FocusState.Programmatic);
                }

                _focusEditingControl = !success;
            }

            return success;
        }

        /// <summary>
        /// This method formats a row (specified by a DataGridRowClipboardEventArgs) into
        /// a single string to be added to the Clipboard when the DataGrid is copying its contents.
        /// </summary>
        /// <param name="e">DataGridRowClipboardEventArgs</param>
        /// <returns>The formatted string.</returns>
        private string FormatClipboardContent(DataGridRowClipboardEventArgs e)
        {
            StringBuilder text = new StringBuilder();
            for (int cellIndex = 0; cellIndex < e.ClipboardRowContent.Count; cellIndex++)
            {
                DataGridClipboardCellContent cellContent = e.ClipboardRowContent[cellIndex];
                if (cellContent != null)
                {
                    text.Append(cellContent.Content);
                }

                if (cellIndex < e.ClipboardRowContent.Count - 1)
                {
                    text.Append('\t');
                }
                else
                {
                    text.Append('\r');
                    text.Append('\n');
                }
            }

            return text.ToString();
        }

        // Calculates the amount to scroll for the ScrollLeft button
        // This is a method rather than a property to emphasize a calculation
        private double GetHorizontalSmallScrollDecrease()
        {
            // If the first column is covered up, scroll to the start of it when the user clicks the left button
            if (_negHorizontalOffset > 0)
            {
                return _negHorizontalOffset;
            }
            else
            {
                // The entire first column is displayed, show the entire previous column when the user clicks
                // the left button
                DataGridColumn previousColumn = this.ColumnsInternal.GetPreviousVisibleScrollingColumn(
                    this.ColumnsItemsInternal[DisplayData.FirstDisplayedScrollingCol]);
                if (previousColumn != null)
                {
                    return GetEdgedColumnWidth(previousColumn);
                }
                else
                {
                    // There's no previous column so don't move
                    return 0;
                }
            }
        }

        // Calculates the amount to scroll for the ScrollRight button
        // This is a method rather than a property to emphasize a calculation
        private double GetHorizontalSmallScrollIncrease()
        {
            if (this.DisplayData.FirstDisplayedScrollingCol >= 0)
            {
                return GetEdgedColumnWidth(this.ColumnsItemsInternal[DisplayData.FirstDisplayedScrollingCol]) - _negHorizontalOffset;
            }

            return 0;
        }

        // Calculates the amount the ScrollDown button should scroll
        // This is a method rather than a property to emphasize that calculations are taking place
        private double GetVerticalSmallScrollIncrease()
        {
            if (this.DisplayData.FirstScrollingSlot >= 0)
            {
                return GetExactSlotElementHeight(this.DisplayData.FirstScrollingSlot) - this.NegVerticalOffset;
            }

            return 0;
        }

        private void HideScrollBars(bool useTransitions)
        {
            if (!_keepScrollBarsShowing)
            {
                _proposedScrollBarsState = ScrollBarVisualState.NoIndicator;
                _proposedScrollBarsSeparatorState = UISettingsHelper.AreSettingsEnablingAnimations ? ScrollBarsSeparatorVisualState.SeparatorCollapsed : ScrollBarsSeparatorVisualState.SeparatorCollapsedWithoutAnimation;
                if (UISettingsHelper.AreSettingsAutoHidingScrollBars)
                {
                    SwitchScrollBarsVisualStates(_proposedScrollBarsState, _proposedScrollBarsSeparatorState, useTransitions);
                }
            }
        }

        private void HideScrollBarsAfterDelay()
        {
            if (!_keepScrollBarsShowing)
            {
                DispatcherQueueTimer hideScrollBarsTimer = null;

                if (_hideScrollBarsTimer != null)
                {
                    hideScrollBarsTimer = _hideScrollBarsTimer;
                    if (hideScrollBarsTimer.IsRunning)
                    {
                        hideScrollBarsTimer.Stop();
                    }
                }
                else
                {
                    hideScrollBarsTimer = DispatcherQueue.GetForCurrentThread().CreateTimer();
                    hideScrollBarsTimer.Interval = TimeSpan.FromMilliseconds(DATAGRID_noScrollBarCountdownMs);
                    hideScrollBarsTimer.Tick += HideScrollBarsTimerTick;
                    _hideScrollBarsTimer = hideScrollBarsTimer;
                }

                hideScrollBarsTimer.Start();
            }
        }

        private void HideScrollBarsTimerTick(object sender, object e)
        {
            StopHideScrollBarsTimer();
            HideScrollBars(true /*useTransitions*/);
        }

        private void HookDataGridEvents()
        {
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(DataGrid_IsEnabledChanged);
            this.KeyDown += new KeyEventHandler(DataGrid_KeyDown);
            this.KeyUp += new KeyEventHandler(DataGrid_KeyUp);
            this.GettingFocus += new TypedEventHandler<UIElement, GettingFocusEventArgs>(DataGrid_GettingFocus);
            this.GotFocus += new RoutedEventHandler(DataGrid_GotFocus);
            this.LostFocus += new RoutedEventHandler(DataGrid_LostFocus);
            this.PointerEntered += new PointerEventHandler(DataGrid_PointerEntered);
            this.PointerExited += new PointerEventHandler(DataGrid_PointerExited);
            this.PointerMoved += new PointerEventHandler(DataGrid_PointerMoved);
            this.PointerPressed += new PointerEventHandler(DataGrid_PointerPressed);
            this.PointerReleased += new PointerEventHandler(DataGrid_PointerReleased);
            this.Unloaded += new RoutedEventHandler(DataGrid_Unloaded);
        }

        private void HookHorizontalScrollBarEvents()
        {
            if (_hScrollBar != null)
            {
                _hScrollBar.Scroll += new ScrollEventHandler(HorizontalScrollBar_Scroll);
                _hScrollBar.PointerEntered += new PointerEventHandler(HorizontalScrollBar_PointerEntered);
                _hScrollBar.PointerExited += new PointerEventHandler(HorizontalScrollBar_PointerExited);
            }
        }

        private void HookVerticalScrollBarEvents()
        {
            if (_vScrollBar != null)
            {
                _vScrollBar.Scroll += new ScrollEventHandler(VerticalScrollBar_Scroll);
                _vScrollBar.PointerEntered += new PointerEventHandler(VerticalScrollBar_PointerEntered);
                _vScrollBar.PointerExited += new PointerEventHandler(VerticalScrollBar_PointerExited);
            }
        }

        private void HorizontalScrollBar_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _isPointerOverHorizontalScrollBar = true;

            if (!UISettingsHelper.AreSettingsEnablingAnimations)
            {
                HideScrollBarsAfterDelay();
            }
        }

        private void HorizontalScrollBar_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            _isPointerOverHorizontalScrollBar = false;
            HideScrollBarsAfterDelay();
        }

        private void VerticalScrollBar_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _isPointerOverVerticalScrollBar = true;

            if (!UISettingsHelper.AreSettingsEnablingAnimations)
            {
                HideScrollBarsAfterDelay();
            }
        }

        private void VerticalScrollBar_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            _isPointerOverVerticalScrollBar = false;
            HideScrollBarsAfterDelay();
        }

        private void HorizontalScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            ProcessHorizontalScroll(e.ScrollEventType);
        }

        private void IndicatorStateStoryboard_Completed(object sender, object e)
        {
            // If the cursor is currently directly over either scroll bar then do not automatically hide the indicators.
            if (!_keepScrollBarsShowing &&
                !_isPointerOverVerticalScrollBar &&
                !_isPointerOverHorizontalScrollBar)
            {
                // Go to the NoIndicator state using transitions.
                if (UISettingsHelper.AreSettingsEnablingAnimations)
                {
                    // By default there is a delay before the NoIndicator state actually shows.
                    HideScrollBars(true /*useTransitions*/);
                }
                else
                {
                    // Since OS animations are turned off, use a timer to delay the scroll bars' hiding.
                    HideScrollBarsAfterDelay();
                }
            }
        }

        private bool IsColumnOutOfBounds(int columnIndex)
        {
            return columnIndex >= this.ColumnsItemsInternal.Count || columnIndex < 0;
        }

        private bool IsInnerCellOutOfBounds(int columnIndex, int slot)
        {
            return IsColumnOutOfBounds(columnIndex) || IsSlotOutOfBounds(slot);
        }

        private bool IsInnerCellOutOfSelectionBounds(int columnIndex, int slot)
        {
            return IsColumnOutOfBounds(columnIndex) || IsSlotOutOfSelectionBounds(slot);
        }

        private bool IsSlotOutOfBounds(int slot)
        {
            return slot >= this.SlotCount || slot < -1 || _collapsedSlotsTable.Contains(slot);
        }

        private bool IsSlotOutOfSelectionBounds(int slot)
        {
            if (this.RowGroupHeadersTable.Contains(slot))
            {
                DiagnosticsDebug.Assert(slot >= 0, "Expected positive slot.");
                DiagnosticsDebug.Assert(slot < this.SlotCount, "Expected slot smaller than this.SlotCount.");
                return false;
            }
            else
            {
                int rowIndex = RowIndexFromSlot(slot);
                return rowIndex < 0 || rowIndex >= this.DataConnection.Count;
            }
        }

        private void LoadMoreDataFromIncrementalItemsSource(double totalVisibleHeight)
        {
            if (IncrementalLoadingTrigger == IncrementalLoadingTrigger.Edge && DataConnection.IsDataSourceIncremental && DataConnection.HasMoreItems && !DataConnection.IsLoadingMoreItems)
            {
                var bottomScrolledOffHeight = Math.Max(0, totalVisibleHeight - CellsHeight - VerticalOffset);

                if ((IncrementalLoadingThreshold * CellsHeight) >= bottomScrolledOffHeight)
                {
                    var numberOfRowsToLoad = Math.Max(1, (int)(DataFetchSize * CellsHeight / RowHeightEstimate));

                    DataConnection.LoadMoreItems((uint)numberOfRowsToLoad);
                }
            }
        }

        private void MakeFirstDisplayedCellCurrentCell()
        {
            if (this.CurrentColumnIndex != -1)
            {
                _makeFirstDisplayedCellCurrentCellPending = false;
                _desiredCurrentColumnIndex = -1;
                this.FlushCurrentCellChanged();
                return;
            }

            if (this.SlotCount != SlotFromRowIndex(this.DataConnection.Count))
            {
                _makeFirstDisplayedCellCurrentCellPending = true;
                return;
            }

            // No current cell, therefore no selection either - try to set the current cell to the
            // ItemsSource's ICollectionView.CurrentItem if it exists, otherwise use the first displayed cell.
            int slot;
            if (this.DataConnection.CollectionView != null)
            {
                if (this.DataConnection.CollectionView.IsCurrentBeforeFirst ||
                    this.DataConnection.CollectionView.IsCurrentAfterLast)
                {
                    slot = this.RowGroupHeadersTable.Contains(0) ? 0 : -1;
                }
                else
                {
                    slot = SlotFromRowIndex(this.DataConnection.CollectionView.CurrentPosition);
                }
            }
            else
            {
                if (this.SelectedIndex == -1)
                {
                    // Try to default to the first row
                    slot = SlotFromRowIndex(0);
                    if (!this.IsSlotVisible(slot))
                    {
                        slot = -1;
                    }
                }
                else
                {
                    slot = SlotFromRowIndex(this.SelectedIndex);
                }
            }

            int columnIndex = this.FirstDisplayedNonFillerColumnIndex;
            if (_desiredCurrentColumnIndex >= 0 && _desiredCurrentColumnIndex < this.ColumnsItemsInternal.Count)
            {
                columnIndex = _desiredCurrentColumnIndex;
            }

            SetAndSelectCurrentCell(
                columnIndex,
                slot,
                false /*forceCurrentCellSelection*/);
            this.AnchorSlot = slot;
            _makeFirstDisplayedCellCurrentCellPending = false;
            _desiredCurrentColumnIndex = -1;
            FlushCurrentCellChanged();
        }

        private void NoIndicatorStateStoryboard_Completed(object sender, object e)
        {
            DiagnosticsDebug.Assert(_hasNoIndicatorStateStoryboardCompletedHandler, "Expected _hasNoIndicatorStateStoryboardCompletedHandler is true.");

            _showingMouseIndicators = false;
        }

        private void PopulateCellContent(
            bool isCellEdited,
            DataGridColumn dataGridColumn,
            DataGridRow dataGridRow,
            DataGridCell dataGridCell)
        {
            DiagnosticsDebug.Assert(dataGridColumn != null, "Expected non-null dataGridColumn.");
            DiagnosticsDebug.Assert(dataGridRow != null, "Expected non-null dataGridRow.");
            DiagnosticsDebug.Assert(dataGridCell != null, "Expected non-null dataGridCell.");

            FrameworkElement element = null;
            DataGridBoundColumn dataGridBoundColumn = dataGridColumn as DataGridBoundColumn;
            if (isCellEdited)
            {
                // Generate EditingElement and apply column style if available
                element = dataGridColumn.GenerateEditingElementInternal(dataGridCell, dataGridRow.DataContext);
                if (element != null)
                {
                    if (dataGridBoundColumn != null && dataGridBoundColumn.EditingElementStyle != null)
                    {
                        element.SetStyleWithType(dataGridBoundColumn.EditingElementStyle);
                    }

                    // Subscribe to the new element's events
                    element.Loaded += new RoutedEventHandler(EditingElement_Loaded);
                }
            }
            else
            {
                // Generate Element and apply column style if available
                element = dataGridColumn.GenerateElementInternal(dataGridCell, dataGridRow.DataContext);
                if (element != null)
                {
                    if (dataGridBoundColumn != null && dataGridBoundColumn.ElementStyle != null)
                    {
                        element.SetStyleWithType(dataGridBoundColumn.ElementStyle);
                    }
                }

#if FEATURE_VALIDATION
                // If we are replacing the editingElement on the cell with the displayElement, and there
                // were validation errors present on the editingElement, we need to manually force the
                // control to go to the InvalidUnfocused state to support Implicit Styles.  The reason
                // is because the editingElement is being removed as part of a keystroke, and it will
                // leave the visual tree before its state is updated.  Since Implicit Styles are
                // disabled when an element is removed from the visual tree, the subsequent GoToState fails
                // and the editingElement cannot make it to the InvalidUnfocused state.  As a result,
                // any popups in the InvalidFocused state would stay around incorrectly.
                if (this.EditingRow != null && dataGridCell.Content != null)
                {
                    Control control = dataGridCell.Content as Control;
                    if (control != null && Validation.GetHasError(control))
                    {
                        VisualStateManager.GoToState(control, VisualStates.StateInvalidUnfocused, useTransitions: false);
                    }
                }
#endif
            }

            dataGridCell.Content = element;
        }

        private void PreparingCellForEditPrivate(FrameworkElement editingElement)
        {
            if (_editingColumnIndex == -1 ||
                this.CurrentColumnIndex == -1 ||
                this.EditingRow.Cells[this.CurrentColumnIndex].Content != editingElement)
            {
                // The current cell has changed since the call to BeginCellEdit, so the fact
                // that this element has loaded is no longer relevant
                return;
            }

            DiagnosticsDebug.Assert(this.EditingRow != null, "Expected non-null EditingRow.");
            DiagnosticsDebug.Assert(this.EditingRow.Slot == this.CurrentSlot, "Expected EditingRow.Slot equals CurrentSlot.");
            DiagnosticsDebug.Assert(_editingColumnIndex >= 0, "Expected positive _editingColumnIndex.");
            DiagnosticsDebug.Assert(_editingColumnIndex < this.ColumnsItemsInternal.Count, "Expected _editingColumnIndex smaller than this.ColumnsItemsInternal.Count.");
            DiagnosticsDebug.Assert(_editingColumnIndex == this.CurrentColumnIndex, "Expected _editingColumnIndex equals CurrentColumnIndex.");

            FocusEditingCell(this.ContainsFocus || _focusEditingControl /*setFocus*/);

            // Prepare the cell for editing and raise the PreparingCellForEdit event for all columns
            DataGridColumn dataGridColumn = this.CurrentColumn;
            _uneditedValue = dataGridColumn.PrepareCellForEditInternal(editingElement, _editingEventArgs);
            OnPreparingCellForEdit(new DataGridPreparingCellForEditEventArgs(dataGridColumn, this.EditingRow, _editingEventArgs, editingElement));
        }

        private bool ProcessAKey()
        {
            bool ctrl, shift, alt;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift, out alt);

            if (ctrl && !shift && !alt && this.SelectionMode == DataGridSelectionMode.Extended)
            {
                SelectAll();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles the case where a 'Copy' key ('C' or 'Insert') has been pressed.  If pressed in combination with
        /// the control key, and the necessary prerequisites are met, the DataGrid will copy its contents
        /// to the Clipboard as text.
        /// </summary>
        /// <returns>Whether or not the DataGrid handled the key press.</returns>
        private bool ProcessCopyKey()
        {
            bool ctrl, shift, alt;
            KeyboardHelper.GetMetaKeyState(out ctrl, out shift, out alt);

            if (ctrl &&
                !shift &&
                !alt &&
                this.ClipboardCopyMode != DataGridClipboardCopyMode.None &&
                this.SelectedItems.Count > 0 &&
                _editingColumnIndex != this.CurrentColumnIndex)
            {
                StringBuilder textBuilder = new StringBuilder();

                if (this.ClipboardCopyMode == DataGridClipboardCopyMode.IncludeHeader)
                {
                    DataGridRowClipboardEventArgs headerArgs = new DataGridRowClipboardEventArgs(null, true);
                    foreach (DataGridColumn column in this.ColumnsInternal.GetVisibleColumns())
                    {
                        headerArgs.ClipboardRowContent.Add(new DataGridClipboardCellContent(null, column, column.Header));
                    }

                    this.OnCopyingRowClipboardContent(headerArgs);
                    textBuilder.Append(FormatClipboardContent(headerArgs));
                }

                for (int index = 0; index < this.SelectedItems.Count; index++)
                {
                    object item = this.SelectedItems[index];
                    DataGridRowClipboardEventArgs itemArgs = new DataGridRowClipboardEventArgs(item, false);
                    foreach (DataGridColumn column in this.ColumnsInternal.GetVisibleColumns())
                    {
                        object content = column.GetCellValue(item, column.ClipboardContentBinding);
                        itemArgs.ClipboardRowContent.Add(new DataGridClipboardCellContent(item, column, content));
                    }

                    this.OnCopyingRowClipboardContent(itemArgs);
                    textBuilder.Append(FormatClipboardContent(itemArgs));
                }

                string text = textBuilder.ToString();

                if (!string.IsNullOrEmpty(text))
                {
                    try
                    {
                        DataPackage content = new DataPackage();
                        content.SetText(text);
                        Clipboard.SetContent(content);
                    }
                    catch (SecurityException)
                    {
                        // We will get a SecurityException if the user does not allow access to the clipboard.
                    }

                    return true;
                }
            }

            return false;
        }

        private bool ProcessDataGridKey(KeyRoutedEventArgs e)
        {
            bool focusDataGrid = false;

            switch (e.Key)
            {
                case VirtualKey.Tab:
                    return ProcessTabKey(e);

                case VirtualKey.Up:
                    focusDataGrid = ProcessUpKey();
                    break;

                case VirtualKey.Down:
                    focusDataGrid = ProcessDownKey();
                    break;

                case VirtualKey.PageDown:
                    focusDataGrid = ProcessNextKey();
                    break;

                case VirtualKey.PageUp:
                    focusDataGrid = ProcessPriorKey();
                    break;

                case VirtualKey.Left:
                    focusDataGrid = this.FlowDirection == FlowDirection.LeftToRight ? ProcessLeftKey() : ProcessRightKey();
                    break;

                case VirtualKey.Right:
                    focusDataGrid = this.FlowDirection == FlowDirection.LeftToRight ? ProcessRightKey() : ProcessLeftKey();
                    break;

                case VirtualKey.F2:
                    return ProcessF2Key(e);

                case VirtualKey.Home:
                    focusDataGrid = ProcessHomeKey();
                    break;

                case VirtualKey.End:
                    focusDataGrid = ProcessEndKey();
                    break;

                case VirtualKey.Enter:
                    focusDataGrid = ProcessEnterKey();
                    break;

                case VirtualKey.Escape:
                    return ProcessEscapeKey();

                case VirtualKey.A:
                    return ProcessAKey();

                case VirtualKey.C:
                    return ProcessCopyKey();

                case VirtualKey.Insert:
                    return ProcessCopyKey();

                case VirtualKey.Space:
                    return ProcessSpaceKey();
            }

            if (focusDataGrid && this.IsTabStop)
            {
                this.Focus(FocusState.Programmatic);
            }

            return focusDataGrid;
        }

        private bool ProcessDownKeyInternal(bool shift, bool ctrl)
        {
            DataGridColumn dataGridColumn = this.ColumnsInternal.FirstVisibleColumn;
            int firstVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            int lastSlot = this.LastVisibleSlot;
            if (firstVisibleColumnIndex == -1 || lastSlot == -1)
            {
                return false;
            }

            if (this.WaitForLostFocus(() => { this.ProcessDownKeyInternal(shift, ctrl); }))
            {
                return true;
            }

            int nextSlot = -1;
            if (this.CurrentSlot != -1)
            {
                nextSlot = this.GetNextVisibleSlot(this.CurrentSlot);
                if (nextSlot >= this.SlotCount)
                {
                    nextSlot = -1;
                }
            }

            _noSelectionChangeCount++;
            try
            {
                int desiredSlot;
                int columnIndex;
                DataGridSelectionAction action;

                if (this.ColumnHeaderHasFocus)
                {
                    if (ctrl || shift)
                    {
                        return false;
                    }

                    if (this.CurrentSlot == this.FirstVisibleSlot)
                    {
                        this.ColumnHeaderHasFocus = false;
                        return true;
                    }

                    DiagnosticsDebug.Assert(this.CurrentColumnIndex != -1, "Expected CurrentColumnIndex other than -1.");
                    desiredSlot = this.FirstVisibleSlot;
                    columnIndex = this.CurrentColumnIndex;
                    action = DataGridSelectionAction.SelectCurrent;
                }
                else if (this.CurrentColumnIndex == -1)
                {
                    desiredSlot = this.FirstVisibleSlot;
                    columnIndex = firstVisibleColumnIndex;
                    action = DataGridSelectionAction.SelectCurrent;
                }
                else if (ctrl)
                {
                    if (shift)
                    {
                        // Both Ctrl and Shift
                        desiredSlot = lastSlot;
                        columnIndex = this.CurrentColumnIndex;
                        action = (this.SelectionMode == DataGridSelectionMode.Extended)
                            ? DataGridSelectionAction.SelectFromAnchorToCurrent
                            : DataGridSelectionAction.SelectCurrent;
                    }
                    else
                    {
                        // Ctrl without Shift
                        desiredSlot = lastSlot;
                        columnIndex = this.CurrentColumnIndex;
                        action = DataGridSelectionAction.SelectCurrent;
                    }
                }
                else
                {
                    if (nextSlot == -1)
                    {
                        return true;
                    }

                    if (shift)
                    {
                        // Shift without Ctrl
                        desiredSlot = nextSlot;
                        columnIndex = this.CurrentColumnIndex;
                        action = DataGridSelectionAction.SelectFromAnchorToCurrent;
                    }
                    else
                    {
                        // Neither Ctrl nor Shift
                        desiredSlot = nextSlot;
                        columnIndex = this.CurrentColumnIndex;
                        action = DataGridSelectionAction.SelectCurrent;
                    }
                }

                UpdateSelectionAndCurrency(columnIndex, desiredSlot, action, true /*scrollIntoView*/);
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }

            return _successfullyUpdatedSelection;
        }

        private bool ProcessEndKey(bool shift, bool ctrl)
        {
            DataGridColumn dataGridColumn = this.ColumnsInternal.LastVisibleColumn;
            int lastVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            int firstVisibleSlot = this.FirstVisibleSlot;
            int lastVisibleSlot = this.LastVisibleSlot;
            if (lastVisibleColumnIndex == -1 || (firstVisibleSlot == -1 && !this.ColumnHeaderHasFocus))
            {
                return false;
            }

            if (this.WaitForLostFocus(() => { this.ProcessEndKey(shift, ctrl); }))
            {
                return true;
            }

            _noSelectionChangeCount++;
            try
            {
                if (!ctrl)
                {
                    return ProcessRightMost(lastVisibleColumnIndex, firstVisibleSlot);
                }
                else if (firstVisibleSlot != -1)
                {
                    DataGridSelectionAction action = (shift && this.SelectionMode == DataGridSelectionMode.Extended)
                        ? DataGridSelectionAction.SelectFromAnchorToCurrent
                        : DataGridSelectionAction.SelectCurrent;
                    UpdateSelectionAndCurrency(lastVisibleColumnIndex, lastVisibleSlot, action, true /*scrollIntoView*/);
                }
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }

            return _successfullyUpdatedSelection;
        }

        private bool ProcessEnterKey(bool shift, bool ctrl)
        {
            int oldCurrentSlot = this.CurrentSlot;

            if (!ctrl)
            {
                if (this.ColumnHeaderHasFocus)
                {
                    this.CurrentColumn.HeaderCell.InvokeProcessSort();
                    return true;
                }
                else if (this.FirstVisibleSlot != -1 && this.RowGroupHeadersTable.Contains(this.CurrentSlot) && ToggleRowGroup())
                {
                    return true;
                }

                // If Enter was used by a TextBox, we shouldn't handle the key
                TextBox focusedTextBox = GetFocusedElement() as TextBox;
                if (focusedTextBox != null && focusedTextBox.AcceptsReturn)
                {
                    return false;
                }

                if (this.WaitForLostFocus(() => { this.ProcessEnterKey(shift, ctrl); }))
                {
                    return true;
                }

                // Enter behaves like down arrow - it commits the potential editing and goes down one cell.
                if (!ProcessDownKeyInternal(false, ctrl))
                {
                    return false;
                }
            }
            else if (this.WaitForLostFocus(() => { this.ProcessEnterKey(shift, ctrl); }))
            {
                return true;
            }

            // Try to commit the potential editing
            if (oldCurrentSlot == this.CurrentSlot && EndCellEdit(DataGridEditAction.Commit, true /*exitEditingMode*/, true /*keepFocus*/, true /*raiseEvents*/) && this.EditingRow != null)
            {
                EndRowEdit(DataGridEditAction.Commit, true /*exitEditingMode*/, true /*raiseEvents*/);
                ScrollIntoView(this.CurrentItem, this.CurrentColumn);
            }

            return true;
        }

        private bool ProcessEscapeKey()
        {
            if (this.WaitForLostFocus(() => { this.ProcessEscapeKey(); }))
            {
                return true;
            }

            if (_editingColumnIndex != -1)
            {
                // Revert the potential cell editing and exit cell editing.
                EndCellEdit(DataGridEditAction.Cancel, true /*exitEditingMode*/, true /*keepFocus*/, true /*raiseEvents*/);
                return true;
            }
            else if (this.EditingRow != null)
            {
                // Revert the potential row editing and exit row editing.
                EndRowEdit(DataGridEditAction.Cancel, true /*exitEditingMode*/, true /*raiseEvents*/);
                return true;
            }

            return false;
        }

        private bool ProcessF2Key(KeyRoutedEventArgs e)
        {
            bool ctrl, shift;
            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);

            if (!shift && !ctrl &&
                _editingColumnIndex == -1 && this.CurrentColumnIndex != -1 && GetRowSelection(this.CurrentSlot) &&
                !GetColumnEffectiveReadOnlyState(this.CurrentColumn))
            {
                if (ScrollSlotIntoView(this.CurrentColumnIndex, this.CurrentSlot, false /*forCurrentCellChange*/, true /*forceHorizontalScroll*/))
                {
                    BeginCellEdit(e);
                }

                return true;
            }

            return false;
        }

        private bool ProcessHomeKey(bool shift, bool ctrl)
        {
            DataGridColumn dataGridColumn = this.ColumnsInternal.FirstVisibleNonFillerColumn;
            int firstVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            int firstVisibleSlot = this.FirstVisibleSlot;
            if (firstVisibleColumnIndex == -1 || (firstVisibleSlot == -1 && !this.ColumnHeaderHasFocus))
            {
                return false;
            }

            if (this.WaitForLostFocus(() => { this.ProcessHomeKey(shift, ctrl); }))
            {
                return true;
            }

            _noSelectionChangeCount++;
            try
            {
                if (!ctrl)
                {
                    return ProcessLeftMost(firstVisibleColumnIndex, firstVisibleSlot);
                }
                else if (firstVisibleSlot != -1)
                {
                    DataGridSelectionAction action = (shift && this.SelectionMode == DataGridSelectionMode.Extended)
                        ? DataGridSelectionAction.SelectFromAnchorToCurrent
                        : DataGridSelectionAction.SelectCurrent;
                    UpdateSelectionAndCurrency(firstVisibleColumnIndex, firstVisibleSlot, action, true /*scrollIntoView*/);
                }
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }

            return _successfullyUpdatedSelection;
        }

        private bool ProcessLeftKey(bool shift, bool ctrl)
        {
            DataGridColumn dataGridColumn = this.ColumnsInternal.FirstVisibleNonFillerColumn;
            int firstVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            int firstVisibleSlot = this.FirstVisibleSlot;
            if (firstVisibleColumnIndex == -1 || (firstVisibleSlot == -1 && !this.ColumnHeaderHasFocus))
            {
                return false;
            }

            if (this.WaitForLostFocus(() => { this.ProcessLeftKey(shift, ctrl); }))
            {
                return true;
            }

            int previousVisibleColumnIndex = -1;
            if (this.CurrentColumnIndex != -1)
            {
                dataGridColumn = this.ColumnsInternal.GetPreviousVisibleNonFillerColumn(this.ColumnsItemsInternal[this.CurrentColumnIndex]);
                if (dataGridColumn != null)
                {
                    previousVisibleColumnIndex = dataGridColumn.Index;
                }
            }

            DataGridColumn oldFocusedColumn = this.FocusedColumn;

            _noSelectionChangeCount++;
            try
            {
                if (ctrl)
                {
                    return ProcessLeftMost(firstVisibleColumnIndex, firstVisibleSlot);
                }
                else if (firstVisibleSlot != -1 && (!this.RowGroupHeadersTable.Contains(this.CurrentSlot) || this.ColumnHeaderHasFocus))
                {
                    if (this.CurrentColumnIndex == -1)
                    {
                        UpdateSelectionAndCurrency(firstVisibleColumnIndex, firstVisibleSlot, DataGridSelectionAction.SelectCurrent, true /*scrollIntoView*/);
                    }
                    else
                    {
                        if (previousVisibleColumnIndex == -1)
                        {
                            return true;
                        }

                        _noFocusedColumnChangeCount++;
                        try
                        {
                            UpdateSelectionAndCurrency(previousVisibleColumnIndex, this.CurrentSlot, DataGridSelectionAction.None, true /*scrollIntoView*/);
                        }
                        finally
                        {
                            _noFocusedColumnChangeCount--;
                        }
                    }
                }
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }

            if (this.ColumnHeaderHasFocus)
            {
                if (this.CurrentColumn == null)
                {
                    dataGridColumn = this.ColumnsInternal.GetPreviousVisibleNonFillerColumn(this.FocusedColumn);
                    if (dataGridColumn != null)
                    {
                        this.FocusedColumn = dataGridColumn;
                    }
                }
                else
                {
                    this.FocusedColumn = this.CurrentColumn;
                }

                if (firstVisibleSlot == -1 && this.FocusedColumn != null)
                {
                    ScrollColumnIntoView(this.FocusedColumn.Index);
                }
            }

            bool focusedColumnChanged = this.ColumnHeaderHasFocus && oldFocusedColumn != this.FocusedColumn;

            if (focusedColumnChanged)
            {
                if (oldFocusedColumn != null && oldFocusedColumn.HasHeaderCell)
                {
                    oldFocusedColumn.HeaderCell.ApplyState(true);
                }

                if (this.FocusedColumn != null && this.FocusedColumn.HasHeaderCell)
                {
                    this.FocusedColumn.HeaderCell.ApplyState(true);
                }
            }

            return focusedColumnChanged || _successfullyUpdatedSelection;
        }

        // Ctrl Left <==> Home
        private bool ProcessLeftMost(int firstVisibleColumnIndex, int firstVisibleSlot)
        {
            DataGridColumn oldFocusedColumn = this.FocusedColumn;

            _noSelectionChangeCount++;
            try
            {
                int desiredSlot;
                DataGridSelectionAction action;
                if (this.CurrentColumnIndex == -1)
                {
                    desiredSlot = firstVisibleSlot;
                    action = DataGridSelectionAction.SelectCurrent;
                    DiagnosticsDebug.Assert(_selectedItems.Count == 0, "Expected _selectedItems.Count equals 0.");
                }
                else
                {
                    desiredSlot = this.CurrentSlot;
                    action = DataGridSelectionAction.None;
                }

                _noFocusedColumnChangeCount++;
                try
                {
                    UpdateSelectionAndCurrency(firstVisibleColumnIndex, desiredSlot, action, true /*scrollIntoView*/);
                }
                finally
                {
                    _noFocusedColumnChangeCount--;
                }
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }

            if (this.ColumnHeaderHasFocus)
            {
                if (this.CurrentColumn == null)
                {
                    this.FocusedColumn = this.ColumnsInternal.FirstVisibleColumn;
                }
                else
                {
                    this.FocusedColumn = this.CurrentColumn;
                }

                if (firstVisibleSlot == -1 && this.FocusedColumn != null)
                {
                    ScrollColumnIntoView(this.FocusedColumn.Index);
                }
            }

            bool focusedColumnChanged = this.ColumnHeaderHasFocus && oldFocusedColumn != this.FocusedColumn;

            if (focusedColumnChanged)
            {
                if (oldFocusedColumn != null && oldFocusedColumn.HasHeaderCell)
                {
                    oldFocusedColumn.HeaderCell.ApplyState(true);
                }

                if (this.FocusedColumn != null && this.FocusedColumn.HasHeaderCell)
                {
                    this.FocusedColumn.HeaderCell.ApplyState(true);
                }
            }

            return focusedColumnChanged || _successfullyUpdatedSelection;
        }

        private bool ProcessNextKey(bool shift, bool ctrl)
        {
            DataGridColumn dataGridColumn = this.ColumnsInternal.FirstVisibleNonFillerColumn;
            int firstVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            if (firstVisibleColumnIndex == -1 || this.DisplayData.FirstScrollingSlot == -1)
            {
                return false;
            }

            if (this.WaitForLostFocus(() => { this.ProcessNextKey(shift, ctrl); }))
            {
                return true;
            }

            int nextPageSlot = this.CurrentSlot == -1 ? this.DisplayData.FirstScrollingSlot : this.CurrentSlot;
            DiagnosticsDebug.Assert(nextPageSlot != -1, "Expected nextPageSlot other than -1.");
            int slot = GetNextVisibleSlot(nextPageSlot);

            int scrollCount = this.DisplayData.NumTotallyDisplayedScrollingElements;
            while (scrollCount > 0 && slot < this.SlotCount)
            {
                nextPageSlot = slot;
                scrollCount--;
                slot = GetNextVisibleSlot(slot);
            }

            _noSelectionChangeCount++;
            try
            {
                DataGridSelectionAction action;
                int columnIndex;
                if (this.CurrentColumnIndex == -1)
                {
                    columnIndex = firstVisibleColumnIndex;
                    action = DataGridSelectionAction.SelectCurrent;
                }
                else
                {
                    columnIndex = this.CurrentColumnIndex;
                    action = (shift && this.SelectionMode == DataGridSelectionMode.Extended)
                        ? action = DataGridSelectionAction.SelectFromAnchorToCurrent
                        : action = DataGridSelectionAction.SelectCurrent;
                }

                UpdateSelectionAndCurrency(columnIndex, nextPageSlot, action, true /*scrollIntoView*/);
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }

            return _successfullyUpdatedSelection;
        }

        private bool ProcessPriorKey(bool shift, bool ctrl)
        {
            DataGridColumn dataGridColumn = this.ColumnsInternal.FirstVisibleNonFillerColumn;
            int firstVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            if (firstVisibleColumnIndex == -1 || this.DisplayData.FirstScrollingSlot == -1)
            {
                return false;
            }

            if (this.WaitForLostFocus(() => { this.ProcessPriorKey(shift, ctrl); }))
            {
                return true;
            }

            int previousPageSlot = (this.CurrentSlot == -1) ? this.DisplayData.FirstScrollingSlot : this.CurrentSlot;
            DiagnosticsDebug.Assert(previousPageSlot != -1, "Expected previousPageSlot other than -1.");

            int scrollCount = this.DisplayData.NumTotallyDisplayedScrollingElements;
            int slot = GetPreviousVisibleSlot(previousPageSlot);
            while (scrollCount > 0 && slot != -1)
            {
                previousPageSlot = slot;
                scrollCount--;
                slot = GetPreviousVisibleSlot(slot);
            }

            DiagnosticsDebug.Assert(previousPageSlot != -1, "Expected previousPageSlot other than -1.");

            _noSelectionChangeCount++;
            try
            {
                int columnIndex;
                DataGridSelectionAction action;
                if (this.CurrentColumnIndex == -1)
                {
                    columnIndex = firstVisibleColumnIndex;
                    action = DataGridSelectionAction.SelectCurrent;
                }
                else
                {
                    columnIndex = this.CurrentColumnIndex;
                    action = (shift && this.SelectionMode == DataGridSelectionMode.Extended)
                        ? DataGridSelectionAction.SelectFromAnchorToCurrent
                        : DataGridSelectionAction.SelectCurrent;
                }

                UpdateSelectionAndCurrency(columnIndex, previousPageSlot, action, true /*scrollIntoView*/);
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }

            return _successfullyUpdatedSelection;
        }

        private bool ProcessRightKey(bool shift, bool ctrl)
        {
            DataGridColumn dataGridColumn = this.ColumnsInternal.LastVisibleColumn;
            int lastVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            int firstVisibleSlot = this.FirstVisibleSlot;
            if (lastVisibleColumnIndex == -1 || (firstVisibleSlot == -1 && !this.ColumnHeaderHasFocus))
            {
                return false;
            }

            if (this.WaitForLostFocus(() => { this.ProcessRightKey(shift, ctrl); }))
            {
                return true;
            }

            int nextVisibleColumnIndex = -1;
            if (this.CurrentColumnIndex != -1)
            {
                dataGridColumn = this.ColumnsInternal.GetNextVisibleColumn(this.ColumnsItemsInternal[this.CurrentColumnIndex]);
                if (dataGridColumn != null)
                {
                    nextVisibleColumnIndex = dataGridColumn.Index;
                }
            }

            DataGridColumn oldFocusedColumn = this.FocusedColumn;

            _noSelectionChangeCount++;
            try
            {
                if (ctrl)
                {
                    return ProcessRightMost(lastVisibleColumnIndex, firstVisibleSlot);
                }
                else if (firstVisibleSlot != -1 && (!this.RowGroupHeadersTable.Contains(this.CurrentSlot) || this.ColumnHeaderHasFocus))
                {
                    if (this.CurrentColumnIndex == -1)
                    {
                        int firstVisibleColumnIndex = this.ColumnsInternal.FirstVisibleColumn == null ? -1 : this.ColumnsInternal.FirstVisibleColumn.Index;
                        UpdateSelectionAndCurrency(firstVisibleColumnIndex, firstVisibleSlot, DataGridSelectionAction.SelectCurrent, true /*scrollIntoView*/);
                    }
                    else
                    {
                        if (nextVisibleColumnIndex == -1)
                        {
                            return true;
                        }

                        _noFocusedColumnChangeCount++;
                        try
                        {
                            UpdateSelectionAndCurrency(nextVisibleColumnIndex, this.CurrentSlot, DataGridSelectionAction.None, true /*scrollIntoView*/);
                        }
                        finally
                        {
                            _noFocusedColumnChangeCount--;
                        }
                    }
                }
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }

            if (this.ColumnHeaderHasFocus)
            {
                if (this.CurrentColumn == null)
                {
                    dataGridColumn = this.ColumnsInternal.GetNextVisibleColumn(this.FocusedColumn);
                    if (dataGridColumn != null)
                    {
                        this.FocusedColumn = dataGridColumn;
                    }
                }
                else
                {
                    this.FocusedColumn = this.CurrentColumn;
                }

                if (firstVisibleSlot == -1 && this.FocusedColumn != null)
                {
                    ScrollColumnIntoView(this.FocusedColumn.Index);
                }
            }

            bool focusedColumnChanged = this.ColumnHeaderHasFocus && oldFocusedColumn != this.FocusedColumn;

            if (focusedColumnChanged)
            {
                if (oldFocusedColumn != null && oldFocusedColumn.HasHeaderCell)
                {
                    oldFocusedColumn.HeaderCell.ApplyState(true);
                }

                if (this.FocusedColumn != null && this.FocusedColumn.HasHeaderCell)
                {
                    this.FocusedColumn.HeaderCell.ApplyState(true);
                }
            }

            return focusedColumnChanged || _successfullyUpdatedSelection;
        }

        // Ctrl Right <==> End
        private bool ProcessRightMost(int lastVisibleColumnIndex, int firstVisibleSlot)
        {
            DataGridColumn oldFocusedColumn = this.FocusedColumn;

            _noSelectionChangeCount++;
            try
            {
                int desiredSlot;
                DataGridSelectionAction action;
                if (this.CurrentColumnIndex == -1)
                {
                    desiredSlot = firstVisibleSlot;
                    action = DataGridSelectionAction.SelectCurrent;
                }
                else
                {
                    desiredSlot = this.CurrentSlot;
                    action = DataGridSelectionAction.None;
                }

                _noFocusedColumnChangeCount++;
                try
                {
                    UpdateSelectionAndCurrency(lastVisibleColumnIndex, desiredSlot, action, true /*scrollIntoView*/);
                }
                finally
                {
                    _noFocusedColumnChangeCount--;
                }
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }

            if (this.ColumnHeaderHasFocus)
            {
                if (this.CurrentColumn == null)
                {
                    this.FocusedColumn = this.ColumnsInternal.LastVisibleColumn;
                }
                else
                {
                    this.FocusedColumn = this.CurrentColumn;
                }

                if (firstVisibleSlot == -1 && this.FocusedColumn != null)
                {
                    ScrollColumnIntoView(this.FocusedColumn.Index);
                }
            }

            bool focusedColumnChanged = this.ColumnHeaderHasFocus && oldFocusedColumn != this.FocusedColumn;

            if (focusedColumnChanged)
            {
                if (oldFocusedColumn != null && oldFocusedColumn.HasHeaderCell)
                {
                    oldFocusedColumn.HeaderCell.ApplyState(true);
                }

                if (this.FocusedColumn != null && this.FocusedColumn.HasHeaderCell)
                {
                    this.FocusedColumn.HeaderCell.ApplyState(true);
                }
            }

            return focusedColumnChanged || _successfullyUpdatedSelection;
        }

        private bool ProcessSpaceKey()
        {
            return ToggleRowGroup();
        }

        private bool ProcessTabKey(KeyRoutedEventArgs e)
        {
            bool ctrl, shift;
            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            return this.ProcessTabKey(e, shift, ctrl);
        }

        private bool ProcessTabKey(KeyRoutedEventArgs e, bool shift, bool ctrl)
        {
            if (ctrl || _editingColumnIndex == -1 || this.IsReadOnly)
            {
                // Go to the next/previous control on the page or the column header when
                // - Ctrl key is used
                // - Potential current cell is not edited, or the datagrid is read-only.
                if (!shift && this.ColumnHeaders != null && this.AreColumnHeadersVisible && !this.ColumnHeaderHasFocus)
                {
                    // Show focus on the current column's header.
                    this.ColumnHeaderHasFocus = true;
                    return true;
                }
                else if (shift && this.ColumnHeaderHasFocus)
                {
                    this.ColumnHeaderHasFocus = false;
                    return this.CurrentColumnIndex != -1;
                }

                this.ColumnHeaderHasFocus = false;
                return false;
            }

            // Try to locate a writable cell before/after the current cell
            DiagnosticsDebug.Assert(this.CurrentColumnIndex != -1, "Expected CurrentColumnIndex other than -1.");
            DiagnosticsDebug.Assert(this.CurrentSlot != -1, "Expected CurrentSlot other than -1.");

            int neighborVisibleWritableColumnIndex, neighborSlot;
            DataGridColumn dataGridColumn;
            if (shift)
            {
                dataGridColumn = this.ColumnsInternal.GetPreviousVisibleWritableColumn(this.ColumnsItemsInternal[this.CurrentColumnIndex]);
                neighborSlot = GetPreviousVisibleSlot(this.CurrentSlot);
                if (this.EditingRow != null)
                {
                    while (neighborSlot != -1 && this.RowGroupHeadersTable.Contains(neighborSlot))
                    {
                        neighborSlot = GetPreviousVisibleSlot(neighborSlot);
                    }
                }
            }
            else
            {
                dataGridColumn = this.ColumnsInternal.GetNextVisibleWritableColumn(this.ColumnsItemsInternal[this.CurrentColumnIndex]);
                neighborSlot = GetNextVisibleSlot(this.CurrentSlot);
                if (this.EditingRow != null)
                {
                    while (neighborSlot < this.SlotCount && this.RowGroupHeadersTable.Contains(neighborSlot))
                    {
                        neighborSlot = GetNextVisibleSlot(neighborSlot);
                    }
                }
            }

            neighborVisibleWritableColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;

            if (neighborVisibleWritableColumnIndex == -1 && (neighborSlot == -1 || neighborSlot >= this.SlotCount))
            {
                // There is no previous/next row and no previous/next writable cell on the current row
                return false;
            }

            if (this.WaitForLostFocus(() => { this.ProcessTabKey(e, shift, ctrl); }))
            {
                return true;
            }

            int targetSlot = -1, targetColumnIndex = -1;

            _noSelectionChangeCount++;
            try
            {
                if (neighborVisibleWritableColumnIndex == -1)
                {
                    targetSlot = neighborSlot;
                    if (shift)
                    {
                        DiagnosticsDebug.Assert(this.ColumnsInternal.LastVisibleWritableColumn != null, "Expected non-null ColumnsInternal.LastVisibleWritableColumn.");
                        targetColumnIndex = this.ColumnsInternal.LastVisibleWritableColumn.Index;
                    }
                    else
                    {
                        DiagnosticsDebug.Assert(this.ColumnsInternal.FirstVisibleWritableColumn != null, "Expected non-null ColumnsInternal.FirstVisibleWritableColumn.");
                        targetColumnIndex = this.ColumnsInternal.FirstVisibleWritableColumn.Index;
                    }
                }
                else
                {
                    targetSlot = this.CurrentSlot;
                    targetColumnIndex = neighborVisibleWritableColumnIndex;
                }

                DataGridSelectionAction action;
                if (targetSlot != this.CurrentSlot || (this.SelectionMode == DataGridSelectionMode.Extended))
                {
                    if (IsSlotOutOfBounds(targetSlot))
                    {
                        return true;
                    }

                    action = DataGridSelectionAction.SelectCurrent;
                }
                else
                {
                    action = DataGridSelectionAction.None;
                }

                UpdateSelectionAndCurrency(targetColumnIndex, targetSlot, action, true /*scrollIntoView*/);
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }

            if (_successfullyUpdatedSelection && !this.RowGroupHeadersTable.Contains(targetSlot))
            {
                BeginCellEdit(e);
            }

            // Return true to say we handled the key event even if the operation was unsuccessful. If we don't
            // say we handled this event, the framework will continue to process the tab key and change focus.
            return true;
        }

        private bool ProcessUpKey(bool shift, bool ctrl)
        {
            DataGridColumn dataGridColumn = this.ColumnsInternal.FirstVisibleNonFillerColumn;
            int firstVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            int firstVisibleSlot = this.FirstVisibleSlot;
            if (firstVisibleColumnIndex == -1 || firstVisibleSlot == -1)
            {
                return false;
            }

            if (this.WaitForLostFocus(() => { this.ProcessUpKey(shift, ctrl); }))
            {
                return true;
            }

            int previousVisibleSlot = (this.CurrentSlot != -1) ? GetPreviousVisibleSlot(this.CurrentSlot) : -1;

            _noSelectionChangeCount++;

            try
            {
                int slot;
                int columnIndex;
                DataGridSelectionAction action;
                if (this.CurrentColumnIndex == -1)
                {
                    slot = firstVisibleSlot;
                    columnIndex = firstVisibleColumnIndex;
                    action = DataGridSelectionAction.SelectCurrent;
                }
                else if (ctrl)
                {
                    if (shift)
                    {
                        // Both Ctrl and Shift
                        slot = firstVisibleSlot;
                        columnIndex = this.CurrentColumnIndex;
                        action = (this.SelectionMode == DataGridSelectionMode.Extended)
                            ? DataGridSelectionAction.SelectFromAnchorToCurrent
                            : DataGridSelectionAction.SelectCurrent;
                    }
                    else
                    {
                        // Ctrl without Shift
                        slot = firstVisibleSlot;
                        columnIndex = this.CurrentColumnIndex;
                        action = DataGridSelectionAction.SelectCurrent;
                    }
                }
                else
                {
                    if (previousVisibleSlot == -1)
                    {
                        return true;
                    }

                    if (shift)
                    {
                        // Shift without Ctrl
                        slot = previousVisibleSlot;
                        columnIndex = this.CurrentColumnIndex;
                        action = DataGridSelectionAction.SelectFromAnchorToCurrent;
                    }
                    else
                    {
                        // Neither Shift nor Ctrl
                        slot = previousVisibleSlot;
                        columnIndex = this.CurrentColumnIndex;
                        action = DataGridSelectionAction.SelectCurrent;
                    }
                }

                UpdateSelectionAndCurrency(columnIndex, slot, action, true /*scrollIntoView*/);
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }

            return _successfullyUpdatedSelection;
        }

        private void RemoveDisplayedColumnHeader(DataGridColumn dataGridColumn)
        {
            if (_columnHeadersPresenter != null)
            {
                _columnHeadersPresenter.Children.Remove(dataGridColumn.HeaderCell);
            }
        }

        private void RemoveDisplayedColumnHeaders()
        {
            if (_columnHeadersPresenter != null)
            {
                _columnHeadersPresenter.Children.Clear();
            }

            this.ColumnsInternal.FillerColumn.IsRepresented = false;
        }

        private bool ResetCurrentCellCore()
        {
            return this.CurrentColumnIndex == -1 || SetCurrentCellCore(-1, -1);
        }

        private void ResetEditingRow()
        {
            DataGridRow oldEditingRow = this.EditingRow;
            if (oldEditingRow != null &&
                oldEditingRow != _focusedRow &&
                !IsSlotVisible(oldEditingRow.Slot))
            {
                // Unload the old editing row if it's off screen
                oldEditingRow.Clip = null;
                UnloadRow(oldEditingRow);
                this.DisplayData.FullyRecycleElements();
            }

            this.EditingRow = null;
            if (oldEditingRow != null && IsSlotVisible(oldEditingRow.Slot))
            {
                // If the row is no longer editing, then its visuals need to change.
                oldEditingRow.ApplyState(true /*animate*/);
            }
        }

        private void ResetFocusedRow()
        {
            if (_focusedRow != null &&
                _focusedRow != this.EditingRow &&
                !IsSlotVisible(_focusedRow.Slot))
            {
                // Unload the old focused row if it's off screen
                _focusedRow.Clip = null;
                UnloadRow(_focusedRow);
                this.DisplayData.FullyRecycleElements();
            }

            _focusedRow = null;
        }

        private void ResetValidationStatus()
        {
            // Clear the invalid status of the Cell, Row and DataGrid
            if (this.EditingRow != null)
            {
                this.EditingRow.IsValid = true;
                if (this.EditingRow.Index != -1)
                {
                    foreach (DataGridCell cell in this.EditingRow.Cells)
                    {
                        if (!cell.IsValid)
                        {
                            cell.IsValid = true;
                            cell.ApplyCellState(true);
                        }
                    }

                    this.EditingRow.ApplyState(true);
                }
            }

            this.IsValid = true;

            // Clear the previous validation results
            _validationResults.Clear();

#if FEATURE_VALIDATION_SUMMARY
            // Hide the error list if validation succeeded
            if (_validationSummary != null && _validationSummary.Errors.Count > 0)
            {
                _validationSummary.Errors.Clear();
                if (this.EditingRow != null)
                {
                    int editingRowSlot = this.EditingRow.Slot;

                    InvalidateMeasure();
                    // TODO: Move to DispatcherQueue when FEATURE_VALIDATION_SUMMARY is enabled
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        // It's possible that the DataContext or ItemsSource has changed by the time we reach this code,
                        // so we need to ensure that the editing row still exists before scrolling it into view
                        if (!IsSlotOutOfBounds(editingRowSlot) && editingRowSlot != -1)
                        {
                            ScrollSlotIntoView(editingRowSlot, false /*scrolledHorizontally*/);
                        }
                    });
                }
            }
#endif
        }

        private void RowGroupHeaderStyles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_rowsPresenter != null)
            {
                Style oldLastStyle = _rowGroupHeaderStylesOld.Count > 0 ? _rowGroupHeaderStylesOld[_rowGroupHeaderStylesOld.Count - 1] : null;
                while (_rowGroupHeaderStylesOld.Count < _rowGroupHeaderStyles.Count)
                {
                    _rowGroupHeaderStylesOld.Add(oldLastStyle);
                }

                Style lastStyle = _rowGroupHeaderStyles.Count > 0 ? _rowGroupHeaderStyles[_rowGroupHeaderStyles.Count - 1] : null;
                foreach (UIElement element in _rowsPresenter.Children)
                {
                    DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                    if (groupHeader != null)
                    {
                        Style oldStyle = groupHeader.Level < _rowGroupHeaderStylesOld.Count ? _rowGroupHeaderStylesOld[groupHeader.Level] : oldLastStyle;
                        Style newStyle = groupHeader.Level < _rowGroupHeaderStyles.Count ? _rowGroupHeaderStyles[groupHeader.Level] : lastStyle;
                        EnsureElementStyle(groupHeader, oldStyle, newStyle);
                    }
                }
            }

            _rowGroupHeaderStylesOld.Clear();
            foreach (Style style in _rowGroupHeaderStyles)
            {
                _rowGroupHeaderStylesOld.Add(style);
            }
        }

        private void SelectAll()
        {
            SetRowsSelection(0, this.SlotCount - 1);
        }

        private void SetAndSelectCurrentCell(
            int columnIndex,
            int slot,
            bool forceCurrentCellSelection)
        {
            DataGridSelectionAction action = forceCurrentCellSelection ? DataGridSelectionAction.SelectCurrent : DataGridSelectionAction.None;
            UpdateSelectionAndCurrency(columnIndex, slot, action, false /*scrollIntoView*/);
        }

        // columnIndex = 2, rowIndex = -1 --> current cell belongs to the 'new row'.
        // columnIndex = 2, rowIndex = 2 --> current cell is an inner cell
        // columnIndex = -1, rowIndex = -1 --> current cell is reset
        // columnIndex = -1, rowIndex = 2 --> Unexpected
        private bool SetCurrentCellCore(int columnIndex, int slot, bool commitEdit, bool endRowEdit)
        {
            DiagnosticsDebug.Assert(columnIndex < this.ColumnsItemsInternal.Count, "Expected columnIndex smaller than ColumnsItemsInternal.Count.");
            DiagnosticsDebug.Assert(slot < this.SlotCount, "Expected slot smaller than this.SlotCount.");
            DiagnosticsDebug.Assert(columnIndex == -1 || this.ColumnsItemsInternal[columnIndex].IsVisible, "Expected columnIndex equals -1 or ColumnsItemsInternal[columnIndex].IsVisible is true.");
            DiagnosticsDebug.Assert(columnIndex <= -1 || slot != -1, "Expected columnIndex smaller than or equal to -1 or slot other than -1.");

            if (columnIndex == this.CurrentColumnIndex &&
                slot == this.CurrentSlot)
            {
                DiagnosticsDebug.Assert(this.DataConnection != null, "Expected non-null DataConnection.");
                DiagnosticsDebug.Assert(_editingColumnIndex == -1 || _editingColumnIndex == this.CurrentColumnIndex, "Expected _editingColumnIndex equals -1 or _editingColumnIndex equals CurrentColumnIndex.");
                DiagnosticsDebug.Assert(this.EditingRow == null || this.EditingRow.Slot == this.CurrentSlot || this.DataConnection.EndingEdit, "Expected EditingRow is null or EditingRow.Slot equals CurrentSlot or DataConnection.EndingEdit is true.");
                return true;
            }

            UIElement oldDisplayedElement = null;
            DataGridCellCoordinates oldCurrentCell = new DataGridCellCoordinates(this.CurrentCellCoordinates);
            object newCurrentItem = null;

            if (!this.RowGroupHeadersTable.Contains(slot))
            {
                int rowIndex = this.RowIndexFromSlot(slot);
                if (rowIndex >= 0 && rowIndex < this.DataConnection.Count)
                {
                    newCurrentItem = this.DataConnection.GetDataItem(rowIndex);
                }
            }

            if (this.CurrentColumnIndex > -1)
            {
                DiagnosticsDebug.Assert(this.CurrentColumnIndex < this.ColumnsItemsInternal.Count, "Expected CurrentColumnIndex smaller than ColumnsItemsInternal.Count.");
                DiagnosticsDebug.Assert(this.CurrentSlot < this.SlotCount, "Expected CurrentSlot smaller than SlotCount.");

                if (!IsInnerCellOutOfBounds(oldCurrentCell.ColumnIndex, oldCurrentCell.Slot) &&
                    this.IsSlotVisible(oldCurrentCell.Slot))
                {
                    oldDisplayedElement = this.DisplayData.GetDisplayedElement(oldCurrentCell.Slot);
                }

                if (!this.RowGroupHeadersTable.Contains(oldCurrentCell.Slot) && !_temporarilyResetCurrentCell)
                {
                    bool keepFocus = this.ContainsFocus;
                    if (commitEdit)
                    {
                        if (!EndCellEdit(DataGridEditAction.Commit, true /*exitEditingMode*/, keepFocus, true /*raiseEvents*/))
                        {
                            return false;
                        }

                        // Resetting the current cell: setting it to (-1, -1) is not considered setting it out of bounds
                        if ((columnIndex != -1 && slot != -1 && IsInnerCellOutOfSelectionBounds(columnIndex, slot)) ||
                            IsInnerCellOutOfSelectionBounds(oldCurrentCell.ColumnIndex, oldCurrentCell.Slot))
                        {
                            return false;
                        }

                        if (endRowEdit && !EndRowEdit(DataGridEditAction.Commit, true /*exitEditingMode*/, true /*raiseEvents*/))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        this.CancelEdit(DataGridEditingUnit.Row, false);
                        ExitEdit(keepFocus);
                    }
                }
            }

            if (newCurrentItem != null)
            {
                slot = this.SlotFromRowIndex(this.DataConnection.IndexOf(newCurrentItem));
            }

            if (slot == -1 && columnIndex != -1)
            {
                return false;
            }

            if (_noFocusedColumnChangeCount == 0)
            {
                this.ColumnHeaderHasFocus = false;
            }

            this.CurrentColumnIndex = columnIndex;
            this.CurrentSlot = slot;

            if (_temporarilyResetCurrentCell)
            {
                if (columnIndex != -1)
                {
                    _temporarilyResetCurrentCell = false;
                }
            }

            if (!_temporarilyResetCurrentCell && _editingColumnIndex != -1)
            {
                _editingColumnIndex = columnIndex;
            }

            if (oldDisplayedElement != null)
            {
                DataGridRow row = oldDisplayedElement as DataGridRow;
                if (row != null)
                {
                    // Don't reset the state of the current cell if we're editing it because that would put it in an invalid state
                    UpdateCurrentState(oldDisplayedElement, oldCurrentCell.ColumnIndex, !(_temporarilyResetCurrentCell && row.IsEditing && _editingColumnIndex == oldCurrentCell.ColumnIndex));
                }
                else
                {
                    UpdateCurrentState(oldDisplayedElement, oldCurrentCell.ColumnIndex, false /*applyCellState*/);
                }
            }

            if (this.CurrentColumnIndex > -1)
            {
                DiagnosticsDebug.Assert(this.CurrentSlot > -1, "Expected CurrentSlot greater than -1.");
                DiagnosticsDebug.Assert(this.CurrentColumnIndex < this.ColumnsItemsInternal.Count, "Expected CurrentColumnIndex smaller than ColumnsItemsInternal.Count.");
                DiagnosticsDebug.Assert(this.CurrentSlot < this.SlotCount, "Expected CurrentSlot smaller than SlotCount.");
                if (this.IsSlotVisible(this.CurrentSlot))
                {
                    UpdateCurrentState(this.DisplayData.GetDisplayedElement(this.CurrentSlot), this.CurrentColumnIndex, true /*applyCellState*/);
                }
            }

            return true;
        }

        private void SetHorizontalOffset(double newHorizontalOffset)
        {
            if (_hScrollBar != null && _hScrollBar.Value != newHorizontalOffset)
            {
                _hScrollBar.Value = newHorizontalOffset;

                // Unless the control is still loading, show the scroll bars when an offset changes. Keep the existing indicator type.
                if (VisualTreeHelper.GetParent(this) != null)
                {
                    ShowScrollBars();
                }
            }
        }

        private void SetVerticalOffset(double newVerticalOffset)
        {
            VerticalOffset = newVerticalOffset;

            if (_vScrollBar != null && !DoubleUtil.AreClose(newVerticalOffset, _vScrollBar.Value))
            {
                _vScrollBar.Value = _verticalOffset;

                // Unless the control is still loading, show the scroll bars when an offset changes. Keep the existing indicator type.
                if (VisualTreeHelper.GetParent(this) != null)
                {
                    ShowScrollBars();
                }
            }
        }

#if FEATURE_VALIDATION_SUMMARY
        /// <summary>
        /// Determines whether or not a specific validation result should be displayed in the ValidationSummary.
        /// </summary>
        /// <param name="validationResult">Validation result to display.</param>
        /// <returns>True if it should be added to the ValidationSummary, false otherwise.</returns>
        private bool ShouldDisplayValidationResult(ValidationResult validationResult)
        {
            if (this.EditingRow != null)
            {
                return !_bindingValidationResults.ContainsEqualValidationResult(validationResult) ||
                    this.EditingRow.DataContext is IDataErrorInfo || this.EditingRow.DataContext is INotifyDataErrorInfo;
            }

            return false;
        }
#endif

        private void ShowScrollBars()
        {
            if (this.AreAllScrollBarsCollapsed)
            {
                _proposedScrollBarsState = ScrollBarVisualState.NoIndicator;
                _proposedScrollBarsSeparatorState = ScrollBarsSeparatorVisualState.SeparatorCollapsedWithoutAnimation;
                SwitchScrollBarsVisualStates(_proposedScrollBarsState, _proposedScrollBarsSeparatorState, false /*useTransitions*/);
            }
            else
            {
                if (_hideScrollBarsTimer != null && _hideScrollBarsTimer.IsRunning)
                {
                    _hideScrollBarsTimer.Stop();
                    _hideScrollBarsTimer.Start();
                }

                // Mouse indicators dominate if they are already showing or if we have set the flag to prefer them.
                if (_preferMouseIndicators || _showingMouseIndicators)
                {
                    if (this.AreBothScrollBarsVisible && (_isPointerOverHorizontalScrollBar || _isPointerOverVerticalScrollBar))
                    {
                        _proposedScrollBarsState = ScrollBarVisualState.MouseIndicatorFull;
                    }
                    else
                    {
                        _proposedScrollBarsState = ScrollBarVisualState.MouseIndicator;
                    }

                    _showingMouseIndicators = true;
                }
                else
                {
                    _proposedScrollBarsState = ScrollBarVisualState.TouchIndicator;
                }

                // Select the proper state for the scroll bars separator square within the GroupScrollBarsSeparator group:
                if (UISettingsHelper.AreSettingsEnablingAnimations)
                {
                    // When OS animations are turned on, show the square when a scroll bar is shown unless the DataGrid is disabled, using an animation.
                    _proposedScrollBarsSeparatorState =
                        this.IsEnabled &&
                        _proposedScrollBarsState == ScrollBarVisualState.MouseIndicatorFull ?
                        ScrollBarsSeparatorVisualState.SeparatorExpanded : ScrollBarsSeparatorVisualState.SeparatorCollapsed;
                }
                else
                {
                    // OS animations are turned off. Show or hide the square depending on the presence of a scroll bars, without an animation.
                    // When the DataGrid is disabled, hide the square in sync with the scroll bar(s).
                    if (_proposedScrollBarsState == ScrollBarVisualState.MouseIndicatorFull)
                    {
                        _proposedScrollBarsSeparatorState = this.IsEnabled ? ScrollBarsSeparatorVisualState.SeparatorExpandedWithoutAnimation : ScrollBarsSeparatorVisualState.SeparatorCollapsed;
                    }
                    else
                    {
                        _proposedScrollBarsSeparatorState = this.IsEnabled ? ScrollBarsSeparatorVisualState.SeparatorCollapsedWithoutAnimation : ScrollBarsSeparatorVisualState.SeparatorCollapsed;
                    }
                }

                if (!UISettingsHelper.AreSettingsAutoHidingScrollBars)
                {
                    if (this.AreBothScrollBarsVisible)
                    {
                        if (UISettingsHelper.AreSettingsEnablingAnimations)
                        {
                            SwitchScrollBarsVisualStates(ScrollBarVisualState.MouseIndicatorFull, this.IsEnabled ? ScrollBarsSeparatorVisualState.SeparatorExpanded : ScrollBarsSeparatorVisualState.SeparatorCollapsed, true /*useTransitions*/);
                        }
                        else
                        {
                            SwitchScrollBarsVisualStates(ScrollBarVisualState.MouseIndicatorFull, this.IsEnabled ? ScrollBarsSeparatorVisualState.SeparatorExpandedWithoutAnimation : ScrollBarsSeparatorVisualState.SeparatorCollapsed, true /*useTransitions*/);
                        }
                    }
                    else
                    {
                        if (UISettingsHelper.AreSettingsEnablingAnimations)
                        {
                            SwitchScrollBarsVisualStates(ScrollBarVisualState.MouseIndicator, ScrollBarsSeparatorVisualState.SeparatorCollapsed, true /*useTransitions*/);
                        }
                        else
                        {
                            SwitchScrollBarsVisualStates(ScrollBarVisualState.MouseIndicator, this.IsEnabled ? ScrollBarsSeparatorVisualState.SeparatorCollapsedWithoutAnimation : ScrollBarsSeparatorVisualState.SeparatorCollapsed, true /*useTransitions*/);
                        }
                    }
                }
                else
                {
                    SwitchScrollBarsVisualStates(_proposedScrollBarsState, _proposedScrollBarsSeparatorState, true /*useTransitions*/);
                }
            }
        }

        private void StopHideScrollBarsTimer()
        {
            if (_hideScrollBarsTimer != null && _hideScrollBarsTimer.IsRunning)
            {
                _hideScrollBarsTimer.Stop();
            }
        }

        private void SwitchScrollBarsVisualStates(ScrollBarVisualState scrollBarsState, ScrollBarsSeparatorVisualState separatorState, bool useTransitions)
        {
            switch (scrollBarsState)
            {
                case ScrollBarVisualState.NoIndicator:
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateNoIndicator);

                    if (!_hasNoIndicatorStateStoryboardCompletedHandler)
                    {
                        _showingMouseIndicators = false;
                    }

                    break;
                case ScrollBarVisualState.TouchIndicator:
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateTouchIndicator);
                    break;
                case ScrollBarVisualState.MouseIndicator:
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateMouseIndicator);
                    break;
                case ScrollBarVisualState.MouseIndicatorFull:
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateMouseIndicatorFull);
                    break;
            }

            switch (separatorState)
            {
                case ScrollBarsSeparatorVisualState.SeparatorCollapsed:
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateSeparatorCollapsed);
                    break;
                case ScrollBarsSeparatorVisualState.SeparatorExpanded:
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateSeparatorExpanded);
                    break;
                case ScrollBarsSeparatorVisualState.SeparatorExpandedWithoutAnimation:
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateSeparatorExpandedWithoutAnimation);
                    break;
                case ScrollBarsSeparatorVisualState.SeparatorCollapsedWithoutAnimation:
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateSeparatorCollapsedWithoutAnimation);
                    break;
            }
        }

        private void UnhookHorizontalScrollBarEvents()
        {
            if (_hScrollBar != null)
            {
                _hScrollBar.Scroll -= new ScrollEventHandler(HorizontalScrollBar_Scroll);
                _hScrollBar.PointerEntered -= new PointerEventHandler(HorizontalScrollBar_PointerEntered);
                _hScrollBar.PointerExited -= new PointerEventHandler(HorizontalScrollBar_PointerExited);
            }
        }

        private void UnhookVerticalScrollBarEvents()
        {
            if (_vScrollBar != null)
            {
                _vScrollBar.Scroll -= new ScrollEventHandler(VerticalScrollBar_Scroll);
                _vScrollBar.PointerEntered -= new PointerEventHandler(VerticalScrollBar_PointerEntered);
                _vScrollBar.PointerExited -= new PointerEventHandler(VerticalScrollBar_PointerExited);
            }
        }

        private void UpdateCurrentState(UIElement displayedElement, int columnIndex, bool applyCellState)
        {
            DataGridRow row = displayedElement as DataGridRow;
            if (row != null)
            {
                if (this.AreRowHeadersVisible)
                {
                    row.ApplyHeaderState(true /*animate*/);
                }

                DataGridCell cell = row.Cells[columnIndex];
                if (applyCellState)
                {
                    cell.ApplyCellState(true /*animate*/);
                }
            }
            else
            {
                DataGridRowGroupHeader groupHeader = displayedElement as DataGridRowGroupHeader;
                if (groupHeader != null)
                {
                    groupHeader.ApplyState(true /*useTransitions*/);
                    if (this.AreRowHeadersVisible)
                    {
                        groupHeader.ApplyHeaderState(true /*animate*/);
                    }
                }
            }
        }

        private void UpdateDisabledVisual()
        {
            if (this.IsEnabled)
            {
                VisualStates.GoToState(this, true, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(this, true, VisualStates.StateDisabled, VisualStates.StateNormal);
            }
        }

        private void UpdateHorizontalScrollBar(bool needHorizScrollBar, bool forceHorizScrollBar, double totalVisibleWidth, double totalVisibleFrozenWidth, double cellsWidth)
        {
            if (_hScrollBar != null)
            {
                if (needHorizScrollBar || forceHorizScrollBar)
                {
                    // ..........viewportSize
                    //         v---v
                    // |<|_____|###|>|
                    //   ^     ^
                    //   min   max

                    // we want to make the relative size of the thumb reflect the relative size of the viewing area
                    // viewportSize / (max + viewportSize) = cellsWidth / max
                    // -> viewportSize = max * cellsWidth / (max - cellsWidth)

                    // always zero
                    _hScrollBar.Minimum = 0;
                    if (needHorizScrollBar)
                    {
                        // maximum travel distance -- not the total width
                        _hScrollBar.Maximum = totalVisibleWidth - cellsWidth;
                        DiagnosticsDebug.Assert(totalVisibleFrozenWidth >= 0, "Expected positive totalVisibleFrozenWidth.");
                        if (_frozenColumnScrollBarSpacer != null)
                        {
                            _frozenColumnScrollBarSpacer.Width = totalVisibleFrozenWidth;
                        }

                        DiagnosticsDebug.Assert(_hScrollBar.Maximum >= 0, "Expected positive _hScrollBar.Maximum.");

                        // width of the scrollable viewing area
                        double viewPortSize = Math.Max(0, cellsWidth - totalVisibleFrozenWidth);
                        _hScrollBar.ViewportSize = viewPortSize;
                        _hScrollBar.LargeChange = viewPortSize;

                        // The ScrollBar should be in sync with HorizontalOffset at this point.  There's a resize case
                        // where the ScrollBar will coerce an old value here, but we don't want that.
                        SetHorizontalOffset(_horizontalOffset);

                        _hScrollBar.IsEnabled = true;
                    }
                    else
                    {
                        _hScrollBar.Maximum = 0;
                        _hScrollBar.ViewportSize = 0;
                        _hScrollBar.IsEnabled = false;
                    }

                    if (_hScrollBar.Visibility != Visibility.Visible)
                    {
                        // This will trigger a call to this method via Cells_SizeChanged for which no processing is needed.
                        _hScrollBar.Visibility = Visibility.Visible;
                        _ignoreNextScrollBarsLayout = true;

                        if (!this.IsHorizontalScrollBarOverCells && _hScrollBar.DesiredSize.Height == 0)
                        {
                            // We need to know the height for the rest of layout to work correctly so measure it now
                            _hScrollBar.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        }
                    }
                }
                else
                {
                    _hScrollBar.Maximum = 0;
                    if (_hScrollBar.Visibility != Visibility.Collapsed)
                    {
                        // This will trigger a call to this method via Cells_SizeChanged for which no processing is needed.
                        _hScrollBar.Visibility = Visibility.Collapsed;
                        _ignoreNextScrollBarsLayout = true;
                    }
                }

                DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseAutomationScrollEvents();
                }
            }
        }

#if FEATURE_IEDITABLECOLLECTIONVIEW
        private void UpdateNewItemPlaceholder()
        {
            int placeholderSlot = SlotFromRowIndex(this.DataConnection.NewItemPlaceholderIndex);
            if (this.DataConnection.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd &&
                _collapsedSlotsTable.Contains(placeholderSlot) != this.IsReadOnly)
            {
                if (this.IsReadOnly)
                {
                    if (this.SelectedIndex == this.DataConnection.NewItemPlaceholderIndex)
                    {
                        this.SelectedIndex = Math.Max(-1, this.DataConnection.Count - 2);
                    }

                    if (this.IsSlotVisible(SlotFromRowIndex(this.DataConnection.NewItemPlaceholderIndex)))
                    {
                        this.RemoveDisplayedElement(placeholderSlot, false, true);
                        this.InvalidateRowsArrange();
                    }

                    _collapsedSlotsTable.AddValue(placeholderSlot, Visibility.Collapsed);
                }
                else
                {
                    _collapsedSlotsTable.RemoveValue(placeholderSlot);
                }

                this.VisibleSlotCount = this.SlotCount - _collapsedSlotsTable.GetIndexCount(0, this.SlotCount - 1);
                this.ComputeScrollBarsLayout();
            }
        }
#endif

        private void UpdateRowDetailsVisibilityMode(DataGridRowDetailsVisibilityMode newDetailsMode)
        {
            if (_rowsPresenter != null && this.DataConnection.Count > 0)
            {
                Visibility newDetailsVisibility = Visibility.Collapsed;

                switch (newDetailsMode)
                {
                    case DataGridRowDetailsVisibilityMode.Visible:
                        newDetailsVisibility = Visibility.Visible;
                        break;
                    case DataGridRowDetailsVisibilityMode.Collapsed:
                        newDetailsVisibility = Visibility.Collapsed;
                        break;
                    case DataGridRowDetailsVisibilityMode.VisibleWhenSelected:
                        break;
                }

                this.ClearShowDetailsTable();

                bool updated = false;
                foreach (DataGridRow row in this.GetAllRows())
                {
                    if (row.Visibility == Visibility.Visible)
                    {
                        if (newDetailsMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected)
                        {
                            // For VisibleWhenSelected, we need to calculate the value for each individual row
                            newDetailsVisibility = _selectedItems.ContainsSlot(row.Slot) && row.Index != this.DataConnection.NewItemPlaceholderIndex ? Visibility.Visible : Visibility.Collapsed;
                        }

                        if (row.DetailsVisibility != newDetailsVisibility)
                        {
                            updated = true;
                            row.SetDetailsVisibilityInternal(
                                newDetailsVisibility,
                                true /*raiseNotification*/);
                        }
                    }
                }

                if (updated)
                {
                    UpdateDisplayedRows(this.DisplayData.FirstScrollingSlot, this.CellsHeight);
                    InvalidateRowsMeasure(false /*invalidateIndividualElements*/);
                }
            }
        }

        private void UpdateRowsPresenterManipulationMode(bool horizontalMode, bool verticalMode)
        {
            if (_rowsPresenter != null)
            {
                ManipulationModes manipulationMode = _rowsPresenter.ManipulationMode;

                if (horizontalMode)
                {
                    if (this.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled)
                    {
                        manipulationMode |= ManipulationModes.TranslateX | ManipulationModes.TranslateInertia;
                    }
                    else
                    {
                        manipulationMode &= ~(ManipulationModes.TranslateX | ManipulationModes.TranslateRailsX);
                    }
                }

                if (verticalMode)
                {
                    if (this.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled)
                    {
                        manipulationMode |= ManipulationModes.TranslateY | ManipulationModes.TranslateInertia;
                    }
                    else
                    {
                        manipulationMode &= ~(ManipulationModes.TranslateY | ManipulationModes.TranslateRailsY);
                    }
                }

                if ((manipulationMode & (ManipulationModes.TranslateX | ManipulationModes.TranslateY)) == (ManipulationModes.TranslateX | ManipulationModes.TranslateY))
                {
                    manipulationMode |= ManipulationModes.TranslateRailsX | ManipulationModes.TranslateRailsY;
                }

                if ((manipulationMode & (ManipulationModes.TranslateX | ManipulationModes.TranslateRailsX | ManipulationModes.TranslateY | ManipulationModes.TranslateRailsY)) ==
                    ManipulationModes.None)
                {
                    manipulationMode &= ~ManipulationModes.TranslateInertia;
                }

                _rowsPresenter.ManipulationMode = manipulationMode;
            }
        }

        private bool UpdateStateOnTapped(TappedRoutedEventArgs args, int columnIndex, int slot, bool allowEdit, bool shift, bool ctrl)
        {
            bool beginEdit;

            DiagnosticsDebug.Assert(slot >= 0, "Expected positive slot.");

            // Before changing selection, check if the current cell needs to be committed, and
            // check if the current row needs to be committed. If any of those two operations are required and fail,
            // do not change selection, and do not change current cell.
            bool wasInEdit = this.EditingColumnIndex != -1;

            if (IsSlotOutOfBounds(slot))
            {
                return true;
            }

            if (wasInEdit && (columnIndex != this.EditingColumnIndex || slot != this.CurrentSlot) &&
                this.WaitForLostFocus(() => { this.UpdateStateOnTapped(args, columnIndex, slot, allowEdit, shift, ctrl); }))
            {
                return true;
            }

            try
            {
                _noSelectionChangeCount++;

                beginEdit = allowEdit &&
                            this.CurrentSlot == slot &&
                            columnIndex != -1 &&
                            (wasInEdit || this.CurrentColumnIndex == columnIndex) &&
                            !GetColumnEffectiveReadOnlyState(this.ColumnsItemsInternal[columnIndex]);

                DataGridSelectionAction action;
                if (this.SelectionMode == DataGridSelectionMode.Extended && shift)
                {
                    // Shift select multiple rows.
                    action = DataGridSelectionAction.SelectFromAnchorToCurrent;
                }
                else if (GetRowSelection(slot))
                {
                    // Unselecting single row or Selecting a previously multi-selected row.
                    if (!ctrl && this.SelectionMode == DataGridSelectionMode.Extended && _selectedItems.Count != 0)
                    {
                        // Unselect everything except the row that was clicked on.
                        action = DataGridSelectionAction.SelectCurrent;
                    }
                    else if (ctrl && this.EditingRow == null)
                    {
                        action = DataGridSelectionAction.RemoveCurrentFromSelection;
                    }
                    else
                    {
                        action = DataGridSelectionAction.None;
                    }
                }
                else
                {
                    // Selecting a single row or multi-selecting with Ctrl.
                    if (this.SelectionMode == DataGridSelectionMode.Single || !ctrl)
                    {
                        // Unselect the correctly selected rows except the new selected row.
                        action = DataGridSelectionAction.SelectCurrent;
                    }
                    else
                    {
                        action = DataGridSelectionAction.AddCurrentToSelection;
                    }
                }

                UpdateSelectionAndCurrency(columnIndex, slot, action, false /*scrollIntoView*/);
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }

            if (_successfullyUpdatedSelection && beginEdit && BeginCellEdit(args))
            {
                FocusEditingCell(true /*setFocus*/);
            }

            return true;
        }

        /// <summary>
        /// Updates the DataGrid's validation results, modifies the ValidationSummary's items,
        /// and sets the IsValid states of the UIElements.
        /// </summary>
        /// <param name="newValidationResults">New validation results.</param>
        /// <param name="scrollIntoView">If the validation results have changed, scrolls the editing row into view.</param>
        private void UpdateValidationResults(List<ValidationResult> newValidationResults, bool scrollIntoView)
        {
            bool validationResultsChanged = false;
            DiagnosticsDebug.Assert(this.EditingRow != null, "Expected non-null EditingRow.");

            // Remove the validation results that have been fixed
            List<ValidationResult> removedValidationResults = new List<ValidationResult>();
            foreach (ValidationResult oldValidationResult in _validationResults)
            {
                if (oldValidationResult != null && !newValidationResults.ContainsEqualValidationResult(oldValidationResult))
                {
                    removedValidationResults.Add(oldValidationResult);
                    validationResultsChanged = true;
                }
            }

            foreach (ValidationResult removedValidationResult in removedValidationResults)
            {
                _validationResults.Remove(removedValidationResult);
#if FEATURE_VALIDATION_SUMMARY
                if (_validationSummary != null)
                {
                    ValidationSummaryItem removedValidationSummaryItem = this.FindValidationSummaryItem(removedValidationResult);
                    if (removedValidationSummaryItem != null)
                    {
                        _validationSummary.Errors.Remove(removedValidationSummaryItem);
                    }
                }
#endif
            }

            // Add any validation results that were just introduced
            foreach (ValidationResult newValidationResult in newValidationResults)
            {
                if (newValidationResult != null && !_validationResults.ContainsEqualValidationResult(newValidationResult))
                {
                    _validationResults.Add(newValidationResult);
#if FEATURE_VALIDATION_SUMMARY
                    if (_validationSummary != null && ShouldDisplayValidationResult(newValidationResult))
                    {
                        ValidationSummaryItem newValidationSummaryItem = this.CreateValidationSummaryItem(newValidationResult);
                        if (newValidationSummaryItem != null)
                        {
                            _validationSummary.Errors.Add(newValidationSummaryItem);
                        }
                    }
#endif

                    validationResultsChanged = true;
                }
            }

            if (validationResultsChanged)
            {
                this.UpdateValidationStatus();
            }

            if (!this.IsValid && scrollIntoView)
            {
                // Scroll the row with the error into view.
                int editingRowSlot = this.EditingRow.Slot;
#if FEATURE_VALIDATION_SUMMARY
                if (_validationSummary != null)
                {
                    // If the number of errors has changed, then the ValidationSummary will be a different size,
                    // and we need to delay our call to ScrollSlotIntoView
                    this.InvalidateMeasure();
                    // TODO: Move to DispatcherQueue when FEATURE_VALIDATION_SUMMARY is enabled
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        // It's possible that the DataContext or ItemsSource has changed by the time we reach this code,
                        // so we need to ensure that the editing row still exists before scrolling it into view
                        if (!this.IsSlotOutOfBounds(editingRowSlot) && editingRowSlot != -1)
                        {
                            this.ScrollSlotIntoView(editingRowSlot, false /*scrolledHorizontally*/);
                        }
                    });
                }
                else
#endif
                {
                    this.ScrollSlotIntoView(editingRowSlot, false /*scrolledHorizontally*/);
                }
            }
        }

        /// <summary>
        /// Updates the IsValid states of the DataGrid, the EditingRow and its cells. All cells related to
        /// property-level errors are set to Invalid.  If there is an object-level error selected in the
        /// ValidationSummary, then its associated cells will also be flagged (if there are any).
        /// </summary>
        private void UpdateValidationStatus()
        {
            if (this.EditingRow != null)
            {
                foreach (DataGridCell cell in this.EditingRow.Cells)
                {
                    bool isCellValid = true;

                    DiagnosticsDebug.Assert(cell.OwningColumn != null, "Expected cell has owning column.");
                    if (!cell.OwningColumn.IsReadOnly)
                    {
                        foreach (ValidationResult validationResult in _validationResults)
                        {
                            bool validationResultIsSelectedValidationSummaryItemContext = false;

#if FEATURE_VALIDATION_SUMMARY
                            validationResultIsSelectedValidationSummaryItemContext = _selectedValidationSummaryItem != null && _selectedValidationSummaryItem.Context == validationResult;
#endif

                            if (_propertyValidationResults.ContainsEqualValidationResult(validationResult) ||
                                validationResultIsSelectedValidationSummaryItemContext)
                            {
                                foreach (string bindingPath in validationResult.MemberNames)
                                {
                                    if (cell.OwningColumn.BindingPaths.Contains(bindingPath))
                                    {
                                        isCellValid = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (cell.IsValid != isCellValid)
                    {
                        cell.IsValid = isCellValid;
                        cell.ApplyCellState(true /*animate*/);
                    }
                }

                bool isRowValid = _validationResults.Count == 0;
                if (this.EditingRow.IsValid != isRowValid)
                {
                    this.EditingRow.IsValid = isRowValid;
                    this.EditingRow.ApplyState(true /*animate*/);
                }

                this.IsValid = isRowValid;
            }
            else
            {
                this.IsValid = true;
            }
        }

        private void UpdateVerticalScrollBar(bool needVertScrollBar, bool forceVertScrollBar, double totalVisibleHeight, double cellsHeight)
        {
            if (_vScrollBar != null)
            {
                if (needVertScrollBar || forceVertScrollBar)
                {
                    // ..........viewportSize
                    //         v---v
                    // |<|_____|###|>|
                    //   ^     ^
                    //   min   max

                    // we want to make the relative size of the thumb reflect the relative size of the viewing area
                    // viewportSize / (max + viewportSize) = cellsWidth / max
                    // -> viewportSize = max * cellsHeight / (totalVisibleHeight - cellsHeight)
                    // ->              = max * cellsHeight / (totalVisibleHeight - cellsHeight)
                    // ->              = max * cellsHeight / max
                    // ->              = cellsHeight

                    // always zero
                    _vScrollBar.Minimum = 0;
                    if (needVertScrollBar && !double.IsInfinity(cellsHeight))
                    {
                        // maximum travel distance -- not the total height
                        _vScrollBar.Maximum = totalVisibleHeight - cellsHeight;
                        DiagnosticsDebug.Assert(_vScrollBar.Maximum >= 0, "Expected positive _vScrollBar.Maximum.");

                        // total height of the display area
                        _vScrollBar.ViewportSize = cellsHeight;
                        _vScrollBar.LargeChange = cellsHeight;
                        _vScrollBar.IsEnabled = true;
                    }
                    else
                    {
                        _vScrollBar.Maximum = 0;
                        _vScrollBar.ViewportSize = 0;
                        _vScrollBar.IsEnabled = false;
                    }

                    if (_vScrollBar.Visibility != Visibility.Visible)
                    {
                        // This will trigger a call to this method via Cells_SizeChanged for which no processing is needed.
                        _vScrollBar.Visibility = Visibility.Visible;
                        _ignoreNextScrollBarsLayout = true;

                        if (!this.IsVerticalScrollBarOverCells && _vScrollBar.DesiredSize.Width == 0)
                        {
                            // We need to know the width for the rest of layout to work correctly so measure it now.
                            _vScrollBar.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        }
                    }
                }
                else
                {
                    _vScrollBar.Maximum = 0;
                    if (_vScrollBar.Visibility != Visibility.Collapsed)
                    {
                        // This will trigger a call to this method via Cells_SizeChanged for which no processing is needed.
                        _vScrollBar.Visibility = Visibility.Collapsed;
                        _ignoreNextScrollBarsLayout = true;
                    }
                }

                DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseAutomationScrollEvents();
                }
            }
        }

        /// <summary>
        /// Validates the current editing row and updates the visual states.
        /// </summary>
        /// <param name="scrollIntoView">If true, will scroll the editing row into view when a new error is introduced.</param>
        /// <param name="wireEvents">If true, subscribes to the asynchronous INDEI ErrorsChanged events.</param>
        /// <returns>True if the editing row is valid, false otherwise.</returns>
        private bool ValidateEditingRow(bool scrollIntoView, bool wireEvents)
        {
            List<ValidationResult> validationResults;
            if (_initializingNewItem)
            {
                // We only want to run property validation if we're initializing a new item. Instead of
                // clearing all the errors, we will only remove those associated with the current column.
                validationResults = new List<ValidationResult>(_validationResults);
            }
            else
            {
                // We're going to run full entity-level validation, so throw away the
                // old errors since they will be recreated if they're still active.
                _propertyValidationResults.Clear();
                _indeiValidationResults.Clear();
                validationResults = new List<ValidationResult>();
            }

            if (this.EditingRow != null)
            {
                object dataItem = this.EditingRow.DataContext;
                DiagnosticsDebug.Assert(dataItem != null, "Expected non-null dataItem.");

                if (!_initializingNewItem)
                {
                    // Validate using the Validator.
                    ValidationContext context = new ValidationContext(dataItem, null, null);
                    Validator.TryValidateObject(dataItem, context, validationResults, true);

#if FEATURE_IDATAERRORINFO
                    // IDEI entity validation.
                    this.ValidateIdei(dataItem as IDataErrorInfo, null, null, validationResults);
#endif

                    // INDEI entity validation.
                    this.ValidateIndei(dataItem as INotifyDataErrorInfo, null, null, null, validationResults, wireEvents);
                }

                // IDEI and INDEI property validation.
                foreach (DataGridColumn column in this.ColumnsInternal.GetDisplayedColumns(c => c.IsVisible && !c.IsReadOnly))
                {
                    if (!_initializingNewItem || column == this.CurrentColumn)
                    {
                        foreach (string bindingPath in column.BindingPaths)
                        {
                            string declaringPath = null;
                            object declaringItem = dataItem;
                            string bindingProperty = bindingPath;

                            // Check for nested paths.
                            int lastIndexOfSeparator = bindingPath.LastIndexOfAny(new char[] { TypeHelper.PropertyNameSeparator, TypeHelper.LeftIndexerToken });
                            if (lastIndexOfSeparator >= 0)
                            {
                                declaringPath = bindingPath.Substring(0, lastIndexOfSeparator);
                                declaringItem = TypeHelper.GetNestedPropertyValue(dataItem, declaringPath);
                                if (bindingProperty[lastIndexOfSeparator] == TypeHelper.LeftIndexerToken)
                                {
                                    bindingProperty = TypeHelper.PrependDefaultMemberName(declaringItem, bindingPath.Substring(lastIndexOfSeparator));
                                }
                                else
                                {
                                    bindingProperty = bindingPath.Substring(lastIndexOfSeparator + 1);
                                }
                            }

                            if (_initializingNewItem)
                            {
                                // We're only re-validating the current column, so remove its old errors
                                // because we're about to check if they're still relevant.
                                foreach (ValidationResult oldValidationResult in _validationResults)
                                {
                                    if (oldValidationResult != null && oldValidationResult.ContainsMemberName(bindingPath))
                                    {
                                        validationResults.Remove(oldValidationResult);
                                        _indeiValidationResults.Remove(oldValidationResult);
                                        _propertyValidationResults.Remove(oldValidationResult);
                                    }
                                }
                            }

#if FEATURE_IDATAERRORINFO
                            // IDEI property validation.
                            this.ValidateIdei(declaringItem as IDataErrorInfo, bindingProperty, bindingPath, validationResults);
#endif

                            // INDEI property validation.
                            this.ValidateIndei(declaringItem as INotifyDataErrorInfo, bindingProperty, bindingPath, declaringPath, validationResults, wireEvents);
                        }
                    }
                }

                // Add any existing exception errors (in case we're editing a cell).
                // Note: these errors will only be displayed in the ValidationSummary if the
                // editing data item implements IDEI or INDEI.
                foreach (ValidationResult validationResult in _bindingValidationResults)
                {
                    validationResults.AddIfNew(validationResult);
                    _propertyValidationResults.AddIfNew(validationResult);
                }

                // Merge the new validation results with the existing ones.
                this.UpdateValidationResults(validationResults, scrollIntoView);

                // Return false if there are validation errors.
                if (!this.IsValid)
                {
                    return false;
                }
            }

            // Return true if there are no errors or there is no editing row.
            this.ResetValidationStatus();
            return true;
        }

#if FEATURE_IDATAERRORINFO
        /// <summary>
        /// Checks an IDEI data object for errors for the specified property. New errors are added to the
        /// list of validation results.
        /// </summary>
        /// <param name="idei">IDEI object to validate.</param>
        /// <param name="bindingProperty">Name of the property to validate.</param>
        /// <param name="bindingPath">Path of the binding.</param>
        /// <param name="validationResults">List of results to add to.</param>
        private void ValidateIdei(IDataErrorInfo idei, string bindingProperty, string bindingPath, List<ValidationResult> validationResults)
        {
            if (idei != null)
            {
                string errorString = null;
                if (string.IsNullOrEmpty(bindingProperty))
                {
                    DiagnosticsDebug.Assert(string.IsNullOrEmpty(bindingPath));
                    ValidationUtil.CatchNonCriticalExceptions(() => { errorString = idei.Error; });
                    if (!string.IsNullOrEmpty(errorString))
                    {
                        validationResults.AddIfNew(new ValidationResult(errorString));
                    }
                }
                else
                {
                    ValidationUtil.CatchNonCriticalExceptions(() => { errorString = idei[bindingProperty]; });
                    if (!string.IsNullOrEmpty(errorString))
                    {
                        ValidationResult validationResult = new ValidationResult(errorString, new List<string>() { bindingPath });
                        validationResults.AddIfNew(validationResult);
                        _propertyValidationResults.Add(validationResult);
                    }
                }
            }
        }
#endif

        /// <summary>
        /// Checks an INDEI data object for errors on the specified path. New errors are added to the
        /// list of validation results.
        /// </summary>
        /// <param name="indei">INDEI object to validate.</param>
        /// <param name="bindingProperty">Name of the property to validate.</param>
        /// <param name="bindingPath">Path of the binding.</param>
        /// <param name="declaringPath">Path of the INDEI object.</param>
        /// <param name="validationResults">List of results to add to.</param>
        /// <param name="wireEvents">True if the ErrorsChanged event should be subscribed to.</param>
        private void ValidateIndei(INotifyDataErrorInfo indei, string bindingProperty, string bindingPath, string declaringPath, List<ValidationResult> validationResults, bool wireEvents)
        {
            if (indei != null)
            {
                if (indei.HasErrors)
                {
                    IEnumerable errors = null;
                    ValidationUtil.CatchNonCriticalExceptions(() => { errors = indei.GetErrors(bindingProperty); });
                    if (errors != null)
                    {
                        foreach (object errorItem in errors)
                        {
                            if (errorItem != null)
                            {
                                string errorString = null;
                                ValidationUtil.CatchNonCriticalExceptions(() => { errorString = errorItem.ToString(); });
                                if (!string.IsNullOrEmpty(errorString))
                                {
                                    ValidationResult validationResult;
                                    if (!string.IsNullOrEmpty(bindingProperty))
                                    {
                                        validationResult = new ValidationResult(errorString, new List<string>() { bindingPath });
                                        _propertyValidationResults.Add(validationResult);
                                    }
                                    else
                                    {
                                        DiagnosticsDebug.Assert(string.IsNullOrEmpty(bindingPath), "Expected bindingPath is null or empty.");
                                        validationResult = new ValidationResult(errorString);
                                    }

                                    validationResults.AddIfNew(validationResult);
                                    _indeiValidationResults.AddIfNew(validationResult);
                                }
                            }
                        }
                    }
                }

                if (wireEvents && !_validationItems.ContainsKey(indei))
                {
                    _validationItems.Add(indei, declaringPath);
                    indei.ErrorsChanged += new EventHandler<DataErrorsChangedEventArgs>(ValidationItem_ErrorsChanged);
                }
            }
        }

        /// <summary>
        /// Handles the asynchronous INDEI errors that occur while the DataGrid is in editing mode.
        /// </summary>
        /// <param name="sender">INDEI item whose errors changed.</param>
        /// <param name="e">Error event arguments.</param>
        private void ValidationItem_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            INotifyDataErrorInfo indei = sender as INotifyDataErrorInfo;
            if (_validationItems.ContainsKey(indei))
            {
                DiagnosticsDebug.Assert(this.EditingRow != null, "Expected non-null EditingRow.");

                // Determine the binding path.
                string bindingPath = _validationItems[indei];
                if (string.IsNullOrEmpty(bindingPath))
                {
                    bindingPath = e.PropertyName;
                }
                else if (!string.IsNullOrEmpty(e.PropertyName) && e.PropertyName.IndexOf(TypeHelper.LeftIndexerToken) >= 0)
                {
                    bindingPath += TypeHelper.RemoveDefaultMemberName(e.PropertyName);
                }
                else
                {
                    bindingPath += TypeHelper.PropertyNameSeparator + e.PropertyName;
                }

                // Remove the old errors.
                List<ValidationResult> validationResults = new List<ValidationResult>();
                foreach (ValidationResult validationResult in _validationResults)
                {
                    ValidationResult oldValidationResult = _indeiValidationResults.FindEqualValidationResult(validationResult);
                    if (oldValidationResult != null && oldValidationResult.ContainsMemberName(bindingPath))
                    {
                        _indeiValidationResults.Remove(oldValidationResult);
                    }
                    else
                    {
                        validationResults.Add(validationResult);
                    }
                }

                // Find any new errors and update the visuals.
                this.ValidateIndei(indei, e.PropertyName, bindingPath, null, validationResults, false /*wireEvents*/);
                this.UpdateValidationResults(validationResults, false /*scrollIntoView*/);

                // If we're valid now then reset our status.
                if (this.IsValid)
                {
                    this.ResetValidationStatus();
                }
            }
            else if (indei != null)
            {
                indei.ErrorsChanged -= new EventHandler<DataErrorsChangedEventArgs>(ValidationItem_ErrorsChanged);
            }
        }

        private void VerticalScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            ProcessVerticalScroll(e.ScrollEventType);
        }
    }
}