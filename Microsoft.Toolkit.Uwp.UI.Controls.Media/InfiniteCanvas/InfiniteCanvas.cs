// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// InfiniteCanvas is a canvas that supports Ink, Text, Format Text, Zoom in/out, Redo, Undo, Export canvas data, Import canvas data.
    /// </summary>
    [TemplatePart(Name = CanvasTextBoxToolsName, Type = typeof(StackPanel))]
    [TemplatePart(Name = CanvasTextBoxColorPickerName, Type = typeof(Windows.UI.Xaml.Controls.ColorPicker))]
    [TemplatePart(Name = CanvasComboBoxFontSizeTextBoxName, Type = typeof(TextBox))]
    [TemplatePart(Name = CanvasTextBoxItalicButtonName, Type = typeof(ToggleButton))]
    [TemplatePart(Name = CanvasTextBoxBoldButtonName, Type = typeof(ToggleButton))]
    [TemplatePart(Name = DrawingSurfaceRendererName, Type = typeof(InfiniteCanvasVirtualDrawingSurface))]
    [TemplatePart(Name = MainContainerName, Type = typeof(Canvas))]
    [TemplatePart(Name = InfiniteCanvasScrollViewerName, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = EraseAllButtonName, Type = typeof(Button))]
    [TemplatePart(Name = CanvasTextBoxName, Type = typeof(InfiniteCanvasTextBox))]
    [TemplatePart(Name = EnableTextButtonName, Type = typeof(InkToolbarCustomToolButton))]
    [TemplatePart(Name = EnableTouchInkingButtonName, Type = typeof(InkToolbarCustomToggleButton))]
    [TemplatePart(Name = InkCanvasToolBarName, Type = typeof(InkToolbar))]
    [TemplatePart(Name = CanvasToolbarContainerName, Type = typeof(StackPanel))]
    [TemplatePart(Name = DrawingInkCanvasName, Type = typeof(InkCanvas))]
    [TemplatePart(Name = UndoButtonName, Type = typeof(Button))]
    [TemplatePart(Name = RedoButtonName, Type = typeof(Button))]
    [TemplatePart(Name = FontColorIconName, Type = typeof(FontIcon))]
    public partial class InfiniteCanvas : Control
    {
        private const double DefaultMaxZoomFactor = 4.0;
        private const double DefaultMinZoomFactor = .25;
        private const double LargeCanvasWidthHeight = 1 << 21;

        private const string CanvasTextBoxToolsName = "CanvasTextBoxTools";
        private const string CanvasTextBoxColorPickerName = "CanvasTextBoxColorPicker";
        private const string CanvasComboBoxFontSizeTextBoxName = "CanvasComboBoxFontSizeTextBox";
        private const string CanvasTextBoxItalicButtonName = "CanvasTextBoxItalicButton";
        private const string CanvasTextBoxBoldButtonName = "CanvasTextBoxBoldButton";
        private const string DrawingSurfaceRendererName = "DrawingSurfaceRenderer";
        private const string MainContainerName = "MainContainer";
        private const string InfiniteCanvasScrollViewerName = "InfiniteCanvasScrollViewer";
        private const string EraseAllButtonName = "EraseAllButton";
        private const string CanvasTextBoxName = "CanvasTextBox";
        private const string EnableTextButtonName = "EnableTextButton";
        private const string EnableTouchInkingButtonName = "EnableTouchInkingButton";
        private const string InkCanvasToolBarName = "InkCanvasToolBar";
        private const string CanvasToolbarContainerName = "CanvasToolbarContainer";
        private const string DrawingInkCanvasName = "DrawingInkCanvas";
        private const string UndoButtonName = "UndoButton";
        private const string RedoButtonName = "RedoButton";
        private const string FontColorIconName = "FontColorIcon";

        private InkCanvas _inkCanvas;
        private InfiniteCanvasVirtualDrawingSurface _drawingSurfaceRenderer;
        private InkSynchronizer _inkSync;
        private InkToolbarCustomToolButton _enableTextButton;
        private InkToolbarCustomToggleButton _enableTouchInkingButton;
        private InfiniteCanvasTextBox _canvasTextBox;
        private StackPanel _canvasTextBoxTools;
        private Windows.UI.Xaml.Controls.ColorPicker _canvasTextBoxColorPicker;

        private ComboBox _canvasComboBoxFontSizeTextBox;
        private ToggleButton _canvasTextBoxItalicButton;
        private ToggleButton _canvasTextBoxBoldButton;
        private Button _undoButton;
        private Button _redoButton;
        private Button _eraseAllButton;

        private InkToolbar _inkCanvasToolBar;
        private Canvas _mainContainer;
        private ScrollViewer _infiniteCanvasScrollViewer;
        private StackPanel _canvasToolbarContainer;
        private FontIcon _fontColorIcon;

        /// <summary>
        /// Gets or sets the width of the canvas, default value is the max value 2097152
        /// </summary>
        public double CanvasWidth
        {
            get { return (double)GetValue(CanvasWidthProperty); }
            set { SetValue(CanvasWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="CanvasWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanvasWidthProperty =
            DependencyProperty.Register(
                nameof(CanvasWidth),
                typeof(double),
                typeof(InfiniteCanvas),
                new PropertyMetadata(LargeCanvasWidthHeight, CanvasWidthHeightPropertyChanged));

        /// <summary>
        /// Gets or sets the height of the canvas, default value is the max value 2097152
        /// </summary>
        public double CanvasHeight
        {
            get { return (double)GetValue(CanvasHeightProperty); }
            set { SetValue(CanvasHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="CanvasHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanvasHeightProperty =
            DependencyProperty.Register(
                nameof(CanvasHeight),
                typeof(double),
                typeof(InfiniteCanvas),
                new PropertyMetadata(LargeCanvasWidthHeight, CanvasWidthHeightPropertyChanged));

        /// <summary>
        /// Gets or sets a value indicating whether the toolbar is visible or not.
        /// </summary>
        public bool IsToolbarVisible
        {
            get { return (bool)GetValue(IsToolbarVisibleProperty); }
            set { SetValue(IsToolbarVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsToolbarVisible "/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsToolbarVisibleProperty =
            DependencyProperty.Register(
                nameof(IsToolbarVisible),
                typeof(bool),
                typeof(InfiniteCanvas),
                new PropertyMetadata(true, IsToolbarVisiblePropertyChanged));

        /// <summary>
        /// Gets or sets the MaxZoomFactor for the canvas, range between 1 to 10 and the default value is 4
        /// </summary>
        public double MaxZoomFactor
        {
            get { return (double)GetValue(MaxZoomFactorProperty); }
            set { SetValue(MaxZoomFactorProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MaxZoomFactor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxZoomFactorProperty =
            DependencyProperty.Register(
                nameof(MaxZoomFactor),
                typeof(double),
                typeof(InfiniteCanvas),
                new PropertyMetadata(DefaultMaxZoomFactor, MinMaxZoomChangedPropertyChanged));

        /// <summary>
        /// Gets or sets the MinZoomFactor for the canvas, range between .1 to 1 the default value is .25
        /// </summary>
        public double MinZoomFactor
        {
            get { return (double)GetValue(MinZoomFactorProperty); }
            set { SetValue(MinZoomFactorProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MinZoomFactor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinZoomFactorProperty =
            DependencyProperty.Register(
                nameof(MinZoomFactor),
                typeof(double),
                typeof(InfiniteCanvas),
                new PropertyMetadata(DefaultMinZoomFactor, MinMaxZoomChangedPropertyChanged));

        private Rect ViewPort => new Rect(_infiniteCanvasScrollViewer.HorizontalOffset / _infiniteCanvasScrollViewer.ZoomFactor, _infiniteCanvasScrollViewer.VerticalOffset / _infiniteCanvasScrollViewer.ZoomFactor, ViewPortWidth, ViewPortHeight);

        private double ViewPortHeight
        {
            get
            {
                double height;
                if (double.IsNaN(_infiniteCanvasScrollViewer.Height))
                {
                    if (ControlHelpers.IsXamlRootAvailable && _infiniteCanvasScrollViewer.XamlRoot != null)
                    {
                        height = _infiniteCanvasScrollViewer.XamlRoot.Size.Height;
                    }
                    else
                    {
                        height = Window.Current.Bounds.Height;
                    }
                }
                else
                {
                    height = _infiniteCanvasScrollViewer.ViewportHeight;
                }

                return height / _infiniteCanvasScrollViewer.ZoomFactor;
            }
        }

        private double ViewPortWidth
        {
            get
            {
                double width;
                if (double.IsNaN(_infiniteCanvasScrollViewer.Width))
                {
                    if (ControlHelpers.IsXamlRootAvailable && _infiniteCanvasScrollViewer.XamlRoot != null)
                    {
                        width = _infiniteCanvasScrollViewer.XamlRoot.Size.Width;
                    }
                    else
                    {
                        width = Window.Current.Bounds.Width;
                    }
                }
                else
                {
                    width = _infiniteCanvasScrollViewer.ViewportWidth;
                }

                return width / _infiniteCanvasScrollViewer.ZoomFactor;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfiniteCanvas"/> class.
        /// </summary>
        public InfiniteCanvas()
        {
            DefaultStyleKey = typeof(InfiniteCanvas);
        }

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            _canvasTextBoxTools = (StackPanel)GetTemplateChild(CanvasTextBoxToolsName);
            this._canvasTextBoxColorPicker = (Windows.UI.Xaml.Controls.ColorPicker)GetTemplateChild(CanvasTextBoxColorPickerName);
            _canvasComboBoxFontSizeTextBox = (ComboBox)GetTemplateChild(CanvasComboBoxFontSizeTextBoxName);
            _canvasTextBoxItalicButton = (ToggleButton)GetTemplateChild(CanvasTextBoxItalicButtonName);
            _canvasTextBoxBoldButton = (ToggleButton)GetTemplateChild(CanvasTextBoxBoldButtonName);
            _drawingSurfaceRenderer = (InfiniteCanvasVirtualDrawingSurface)GetTemplateChild(DrawingSurfaceRendererName);
            _mainContainer = (Canvas)GetTemplateChild(MainContainerName);
            _infiniteCanvasScrollViewer = (ScrollViewer)GetTemplateChild(InfiniteCanvasScrollViewerName);
            _eraseAllButton = (Button)GetTemplateChild(EraseAllButtonName);
            _canvasTextBox = (InfiniteCanvasTextBox)GetTemplateChild(CanvasTextBoxName);
            _enableTextButton = (InkToolbarCustomToolButton)GetTemplateChild(EnableTextButtonName);
            _enableTouchInkingButton = (InkToolbarCustomToggleButton)GetTemplateChild(EnableTouchInkingButtonName);
            _inkCanvasToolBar = (InkToolbar)GetTemplateChild(InkCanvasToolBarName);
            _canvasToolbarContainer = (StackPanel)GetTemplateChild(CanvasToolbarContainerName);

            _inkCanvas = (InkCanvas)GetTemplateChild(DrawingInkCanvasName);
            _undoButton = (Button)GetTemplateChild(UndoButtonName);
            _redoButton = (Button)GetTemplateChild(RedoButtonName);
            _fontColorIcon = (FontIcon)GetTemplateChild(FontColorIconName);

            UnRegisterEvents();
            RegisterEvents();

            ConfigureControls();

            if (double.IsNaN(_infiniteCanvasScrollViewer.Width))
            {
                if (ControlHelpers.IsXamlRootAvailable && _infiniteCanvasScrollViewer.XamlRoot != null)
                {
                    _infiniteCanvasScrollViewer.Width = _infiniteCanvasScrollViewer.XamlRoot.Size.Width;
                }
                else
                {
                    _infiniteCanvasScrollViewer.Width = Window.Current.Bounds.Width;
                }
            }

            if (double.IsNaN(_infiniteCanvasScrollViewer.Height))
            {
                if (ControlHelpers.IsXamlRootAvailable && _infiniteCanvasScrollViewer.XamlRoot != null)
                {
                    _infiniteCanvasScrollViewer.Height = _infiniteCanvasScrollViewer.XamlRoot.Size.Height;
                }
                else
                {
                    _infiniteCanvasScrollViewer.Height = Window.Current.Bounds.Height;
                }
            }

            base.OnApplyTemplate();
        }

        private void UnRegisterEvents()
        {
            _canvasComboBoxFontSizeTextBox.SelectionChanged -= CanvasComboBoxFontSizeTextBox_SelectionChanged;
            _canvasTextBoxItalicButton.Click -= CanvasTextBoxItalicButton_Clicked;
            _canvasTextBoxBoldButton.Click -= CanvasTextBoxBoldButton_Clicked;
            _canvasTextBoxColorPicker.ColorChanged -= CanvasTextBoxColorPicker_ColorChanged;
            _enableTouchInkingButton.Checked -= EnableTouchInkingButton_Checked;
            _enableTouchInkingButton.Unchecked -= EnableTouchInkingButton_Unchecked;
            _enableTextButton.Checked -= EnableTextButton_Checked;
            _enableTextButton.Unchecked -= EnableTextButton_Unchecked;
            _eraseAllButton.Click -= EraseAllButton_Click;
            _infiniteCanvasScrollViewer.PointerPressed -= InkScrollViewer_PointerPressed;
            _infiniteCanvasScrollViewer.PreviewKeyDown -= InkScrollViewer_PreviewKeyDown;
            _canvasTextBox.TextChanged -= CanvasTextBox_TextChanged;
            _canvasTextBox.SizeChanged -= CanvasTextBox_SizeChanged;
            _undoButton.Click -= UndoButton_Click;
            _redoButton.Click -= RedoButton_Click;
            Unloaded -= InfiniteCanvas_Unloaded;
            Application.Current.LeavingBackground -= Current_LeavingBackground;
            _drawingSurfaceRenderer.CommandExecuted -= DrawingSurfaceRenderer_CommandExecuted;
            _canvasComboBoxFontSizeTextBox.PreviewKeyDown -= CanvasComboBoxFontSizeTextBox_PreviewKeyDown;
            _canvasComboBoxFontSizeTextBox.TextSubmitted -= CanvasComboBoxFontSizeTextBox_TextSubmitted;
            Loaded -= InfiniteCanvas_Loaded;
        }

        private void RegisterEvents()
        {
            _canvasComboBoxFontSizeTextBox.SelectionChanged += CanvasComboBoxFontSizeTextBox_SelectionChanged;
            _canvasTextBoxItalicButton.Click += CanvasTextBoxItalicButton_Clicked;
            _canvasTextBoxBoldButton.Click += CanvasTextBoxBoldButton_Clicked;
            _canvasTextBoxColorPicker.ColorChanged += CanvasTextBoxColorPicker_ColorChanged;
            _enableTouchInkingButton.Checked += EnableTouchInkingButton_Checked;
            _enableTouchInkingButton.Unchecked += EnableTouchInkingButton_Unchecked;
            _enableTextButton.Checked += EnableTextButton_Checked;
            _enableTextButton.Unchecked += EnableTextButton_Unchecked;
            _eraseAllButton.Click += EraseAllButton_Click;
            _infiniteCanvasScrollViewer.PointerPressed += InkScrollViewer_PointerPressed;
            _infiniteCanvasScrollViewer.PreviewKeyDown += InkScrollViewer_PreviewKeyDown;
            _canvasTextBox.TextChanged += CanvasTextBox_TextChanged;
            _canvasTextBox.SizeChanged += CanvasTextBox_SizeChanged;
            _undoButton.Click += UndoButton_Click;
            _redoButton.Click += RedoButton_Click;
            Unloaded += InfiniteCanvas_Unloaded;
            Application.Current.LeavingBackground += Current_LeavingBackground;
            _drawingSurfaceRenderer.CommandExecuted += DrawingSurfaceRenderer_CommandExecuted;
            _canvasComboBoxFontSizeTextBox.PreviewKeyDown += CanvasComboBoxFontSizeTextBox_PreviewKeyDown;
            _canvasComboBoxFontSizeTextBox.TextSubmitted += CanvasComboBoxFontSizeTextBox_TextSubmitted;
            Loaded += InfiniteCanvas_Loaded;
        }

        private void InfiniteCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            _infiniteCanvasScrollViewer.Width = double.NaN;
            _infiniteCanvasScrollViewer.Height = double.NaN;
        }

        private void ConfigureControls()
        {
            _inkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Pen;
            _inkSync = _inkCanvas.InkPresenter.ActivateCustomDrying();
            _inkCanvas.InkPresenter.StrokesCollected += OnStrokesCollected;

            _inkCanvas.InkPresenter.UnprocessedInput.PointerMoved -= UnprocessedInput_PointerMoved;
            _inkCanvas.InkPresenter.UnprocessedInput.PointerMoved += UnprocessedInput_PointerMoved;

            SetZoomFactor();

            _infiniteCanvasScrollViewer.ViewChanged -= InkScrollViewer_ViewChanged;
            _infiniteCanvasScrollViewer.SizeChanged -= InkScrollViewer_SizeChanged;
            _infiniteCanvasScrollViewer.ViewChanged += InkScrollViewer_ViewChanged;
            _infiniteCanvasScrollViewer.SizeChanged += InkScrollViewer_SizeChanged;

            SetCanvasWidthHeight();

            SetFontSize(_textFontSize);
        }

        private void SetZoomFactor()
        {
            var maxZoomFactor = DefaultMaxZoomFactor;
            var minZoomFactor = DefaultMinZoomFactor;

            if (MaxZoomFactor >= 1 && MaxZoomFactor <= 10)
            {
                maxZoomFactor = MaxZoomFactor;
            }

            if (MinZoomFactor >= .1 && MinZoomFactor <= 1)
            {
                minZoomFactor = MinZoomFactor;
            }

            _infiniteCanvasScrollViewer.MaxZoomFactor = (float)maxZoomFactor;
            _infiniteCanvasScrollViewer.MinZoomFactor = (float)minZoomFactor;
        }

        private void SetCanvasWidthHeight()
        {
            if (_mainContainer == null || _inkCanvas == null || _drawingSurfaceRenderer == null)
            {
                return;
            }

            _mainContainer.Width = CanvasWidth;
            _mainContainer.Height = CanvasHeight;
            _inkCanvas.Width = CanvasWidth;
            _inkCanvas.Height = CanvasHeight;
            _drawingSurfaceRenderer.Width = CanvasWidth;
            _drawingSurfaceRenderer.Height = CanvasHeight;
            _drawingSurfaceRenderer.ConfigureSpriteVisual(CanvasWidth, CanvasHeight, _infiniteCanvasScrollViewer.ZoomFactor);
        }

        private void ReDrawCanvas()
        {
            _drawingSurfaceRenderer.ReDraw(ViewPort, _infiniteCanvasScrollViewer.ZoomFactor);
        }

        /// <summary>
        /// Redo the last action.
        /// </summary>
        public void Redo()
        {
            _drawingSurfaceRenderer.Redo(ViewPort, _infiniteCanvasScrollViewer.ZoomFactor);
        }

        /// <summary>
        /// Undo the last action.
        /// </summary>
        public void Undo()
        {
            _drawingSurfaceRenderer.Undo(ViewPort, _infiniteCanvasScrollViewer.ZoomFactor);
        }

        /// <summary>
        /// Export the InfinitCanvas as json string.
        /// </summary>
        /// <returns>json string</returns>
        public string ExportAsJson()
        {
            // We need to introduce versioning in the next release.
            return _drawingSurfaceRenderer.GetSerializedList();
        }

        /// <summary>
        /// Export the InfiniteCanvas ink strokes.
        /// </summary>
        /// <returns>list of InkStrokes</returns>
        public List<InkStroke> ExportInkStrokes()
        {
            return _drawingSurfaceRenderer.ExportInkStrokes();
        }

        /// <summary>
        /// Export the InfiniteCanvas raw text.
        /// </summary>
        /// <returns>list of strings</returns>
        public List<string> ExportText()
        {
            return _drawingSurfaceRenderer.ExportText();
        }

        /// <summary>
        /// Import InfiniteCanvas from json string and render the new canvas, this function will empty the Redo/Undo queue.
        /// </summary>
        /// <param name="json">InfiniteCanvas json representation</param>
        public void ImportFromJson(string json)
        {
            _drawingSurfaceRenderer.RenderFromJsonAndDraw(ViewPort, json, _infiniteCanvasScrollViewer.ZoomFactor);
        }

        /// <summary>
        /// This method exports the max possible view of the InfiniteCanvas drawings as offScreen drawings that can be converted to image.
        /// Max is calculated using CanvasDevice.MaximumBitmapSizeInPixels
        /// </summary>
        /// <param name="stream">The target stream.</param>
        /// <param name="bitmapFileFormat">the specified format.</param>
        /// <returns>Task</returns>
        public async Task SaveBitmapAsync(IRandomAccessStream stream, BitmapFileFormat bitmapFileFormat)
        {
            var offScreen = _drawingSurfaceRenderer.ExportMaxOffScreenDrawings();
            await offScreen.SaveAsync(stream, MapToCanvasBitmapFileFormat(bitmapFileFormat));
        }

        /// <summary>
        /// This event triggered after each render happened because of any change in the canvas elements.
        /// </summary>
        public event EventHandler ReRenderCompleted;

        private static CanvasBitmapFileFormat MapToCanvasBitmapFileFormat(BitmapFileFormat bitmapFileFormat)
        {
            switch (bitmapFileFormat)
            {
                case BitmapFileFormat.Bmp:
                    return CanvasBitmapFileFormat.Bmp;
                case BitmapFileFormat.Png:
                    return CanvasBitmapFileFormat.Png;
                case BitmapFileFormat.Jpeg:
                    return CanvasBitmapFileFormat.Jpeg;
                case BitmapFileFormat.Tiff:
                    return CanvasBitmapFileFormat.Tiff;
                case BitmapFileFormat.Gif:
                    return CanvasBitmapFileFormat.Gif;
                case BitmapFileFormat.JpegXR:
                    return CanvasBitmapFileFormat.JpegXR;
                default: return CanvasBitmapFileFormat.Auto;
            }
        }
    }
}