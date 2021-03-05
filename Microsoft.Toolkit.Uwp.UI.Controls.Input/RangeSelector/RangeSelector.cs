// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Shapes;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// RangeSelector is a "double slider" control for range values.
    /// </summary>
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "MinPressed", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "MaxPressed", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplatePart(Name = "OutOfRangeContentContainer", Type = typeof(Border))]
    [TemplatePart(Name = "ActiveRectangle", Type = typeof(Rectangle))]
    [TemplatePart(Name = "MinThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "MaxThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "ContainerCanvas", Type = typeof(Canvas))]
    [TemplatePart(Name = "ControlGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "ToolTip", Type = typeof(Grid))]
    [TemplatePart(Name = "ToolTipText", Type = typeof(TextBlock))]

    public partial class RangeSelector : Control
    {
        private const double Epsilon = 0.01;
        private const double DefaultMinimum = 0.0;
        private const double DefaultMaximum = 1.0;
        private const double DefaultStepFrequency = 1;
        private static readonly TimeSpan TimeToHideToolTipOnKeyUp = TimeSpan.FromSeconds(1);

        private Rectangle _activeRectangle;
        private Thumb _minThumb;
        private Thumb _maxThumb;
        private Canvas _containerCanvas;
        private double _oldValue;
        private bool _minSet;
        private bool _maxSet;
        private bool _pointerManipulatingMin;
        private bool _pointerManipulatingMax;
        private double _absolutePosition;
        private Grid _toolTip;
        private TextBlock _toolTipText;

        private static string FormatForToolTip(double newValue)
    => string.Format("{0:0.##}", newValue);

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeSelector"/> class.
        /// Create a default range selector control.
        /// </summary>
        public RangeSelector()
        {
            DefaultStyleKey = typeof(RangeSelector);
        }

        /// <summary>
        /// Update the visual state of the control when its template is changed.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            if (_minThumb is Thumb oldMinThumb)
            {
                oldMinThumb.DragCompleted -= MinThumb_DragCompleted;
                oldMinThumb.DragDelta -= MinThumb_DragDelta;
                oldMinThumb.DragStarted -= MinThumb_DragStarted;
                oldMinThumb.KeyDown -= MinThumb_KeyDown;
            }

            if (_maxThumb is Thumb oldMaxThumb)
            {
                oldMaxThumb.DragCompleted -= MaxThumb_DragCompleted;
                oldMaxThumb.DragDelta -= MaxThumb_DragDelta;
                oldMaxThumb.DragStarted -= MaxThumb_DragStarted;
                oldMaxThumb.KeyDown -= MaxThumb_KeyDown;
            }

            if (_containerCanvas is Canvas oldContainerCanvas)
            {
                oldContainerCanvas.SizeChanged -= ContainerCanvas_SizeChanged;
                oldContainerCanvas.PointerPressed -= ContainerCanvas_PointerPressed;
                oldContainerCanvas.PointerMoved -= ContainerCanvas_PointerMoved;
                oldContainerCanvas.PointerReleased -= ContainerCanvas_PointerReleased;
                oldContainerCanvas.PointerExited -= ContainerCanvas_PointerExited;
            }

            IsEnabledChanged -= RangeSelector_IsEnabledChanged;

            // Need to make sure the values can be set in XAML and don't overwrite each other
            VerifyValues();

            _activeRectangle = GetTemplateChild("ActiveRectangle") as Rectangle;
            _minThumb = GetTemplateChild("MinThumb") as Thumb;
            _maxThumb = GetTemplateChild("MaxThumb") as Thumb;
            _containerCanvas = GetTemplateChild("ContainerCanvas") as Canvas;
            _toolTip = GetTemplateChild("ToolTip") as Grid;
            _toolTipText = GetTemplateChild("ToolTipText") as TextBlock;

            if (_minThumb != null)
            {
                _minThumb.DragCompleted += MinThumb_DragCompleted;
                _minThumb.DragDelta += MinThumb_DragDelta;
                _minThumb.DragStarted += MinThumb_DragStarted;
                _minThumb.KeyDown += MinThumb_KeyDown;
                _minThumb.KeyUp += Thumb_KeyUp;
            }

            if (_maxThumb != null)
            {
                _maxThumb.DragCompleted += MaxThumb_DragCompleted;
                _maxThumb.DragDelta += MaxThumb_DragDelta;
                _maxThumb.DragStarted += MaxThumb_DragStarted;
                _maxThumb.KeyDown += MaxThumb_KeyDown;
                _maxThumb.KeyUp += Thumb_KeyUp;
            }

            if (_containerCanvas != null)
            {
                _containerCanvas.SizeChanged += ContainerCanvas_SizeChanged;
                _containerCanvas.PointerEntered += ContainerCanvas_PointerEntered;
                _containerCanvas.PointerPressed += ContainerCanvas_PointerPressed;
                _containerCanvas.PointerMoved += ContainerCanvas_PointerMoved;
                _containerCanvas.PointerReleased += ContainerCanvas_PointerReleased;
                _containerCanvas.PointerExited += ContainerCanvas_PointerExited;
            }

            VisualStateManager.GoToState(this, IsEnabled ? "Normal" : "Disabled", false);

            IsEnabledChanged += RangeSelector_IsEnabledChanged;

            // Measure our min/max text longest value so we can avoid the length of the scrolling reason shifting in size during use.
            var tb = new TextBlock { Text = Maximum.ToString() };
            tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            base.OnApplyTemplate();
        }

        private void ContainerCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SyncThumbs();
        }

        private void VerifyValues()
        {
            if (Minimum > Maximum)
            {
                Minimum = Maximum;
                Maximum = Maximum;
            }

            if (Minimum == Maximum)
            {
                Maximum += Epsilon;
            }

            if (!_maxSet)
            {
                RangeMax = Maximum;
            }

            if (!_minSet)
            {
                RangeMin = Minimum;
            }

            if (RangeMin < Minimum)
            {
                RangeMin = Minimum;
            }

            if (RangeMax < Minimum)
            {
                RangeMax = Minimum;
            }

            if (RangeMin > Maximum)
            {
                RangeMin = Maximum;
            }

            if (RangeMax > Maximum)
            {
                RangeMax = Maximum;
            }

            if (RangeMax < RangeMin)
            {
                RangeMin = RangeMax;
            }
        }

        private void SyncThumbs(bool fromMinKeyDown = false, bool fromMaxKeyDown = false)
        {
            if (_containerCanvas == null)
            {
                return;
            }

            var relativeLeft = ((RangeMin - Minimum) / (Maximum - Minimum)) * DragWidth;
            var relativeRight = ((RangeMax - Minimum) / (Maximum - Minimum)) * DragWidth;

            Canvas.SetLeft(_minThumb, relativeLeft);
            Canvas.SetLeft(_maxThumb, relativeRight);

            if (fromMinKeyDown || fromMaxKeyDown)
            {
                DragThumb(
                    fromMinKeyDown ? _minThumb : _maxThumb,
                    fromMinKeyDown ? 0 : Canvas.GetLeft(_minThumb),
                    fromMinKeyDown ? Canvas.GetLeft(_maxThumb) : DragWidth,
                    fromMinKeyDown ? relativeLeft : relativeRight);
                if (_toolTipText != null)
                {
                    _toolTipText.Text = FormatForToolTip(fromMinKeyDown ? RangeMin : RangeMax);
                }
            }

            SyncActiveRectangle();
        }

        private void SyncActiveRectangle()
        {
            if (_containerCanvas == null)
            {
                return;
            }

            if (_minThumb == null)
            {
                return;
            }

            if (_maxThumb == null)
            {
                return;
            }

            var relativeLeft = Canvas.GetLeft(_minThumb);
            Canvas.SetLeft(_activeRectangle, relativeLeft);
            Canvas.SetTop(_activeRectangle, (_containerCanvas.ActualHeight - _activeRectangle.ActualHeight) / 2);
            _activeRectangle.Width = Math.Max(0, Canvas.GetLeft(_maxThumb) - Canvas.GetLeft(_minThumb));
        }

        private void RangeSelector_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            VisualStateManager.GoToState(this, IsEnabled ? "Normal" : "Disabled", true);
        }
    }
}
