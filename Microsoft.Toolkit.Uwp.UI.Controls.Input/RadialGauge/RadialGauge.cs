// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Numerics;
using Microsoft.Toolkit.Uwp.UI.Helpers;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// A Modern UI Radial Gauge using XAML and Composition API.
    /// The scale of the gauge is a clockwise arc that sweeps from MinAngle (default lower left, at -150°) to MaxAngle (default lower right, at +150°).
    /// </summary>
    //// All calculations are for a 200x200 square. The viewbox will do the rest.
    [TemplatePart(Name = ContainerPartName, Type = typeof(Grid))]
    [TemplatePart(Name = ScalePartName, Type = typeof(Path))]
    [TemplatePart(Name = TrailPartName, Type = typeof(Path))]
    [TemplatePart(Name = ValueTextPartName, Type = typeof(TextBlock))]
    public class RadialGauge : RangeBase
    {
        /// <summary>
        /// Identifies the optional StepSize property.
        /// </summary>
        public static readonly DependencyProperty StepSizeProperty =
            DependencyProperty.Register(nameof(StepSize), typeof(double), typeof(RadialGauge), new PropertyMetadata(0.0));

        /// <summary>
        /// Identifies the <see cref="IsInteractive"/> property.
        /// </summary>
        public static readonly DependencyProperty IsInteractiveProperty =
            DependencyProperty.Register(nameof(IsInteractive), typeof(bool), typeof(RadialGauge), new PropertyMetadata(false, OnInteractivityChanged));

        /// <summary>
        /// Identifies the ScaleWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty ScaleWidthProperty =
            DependencyProperty.Register(nameof(ScaleWidth), typeof(double), typeof(RadialGauge), new PropertyMetadata(26.0, OnScaleChanged));

        /// <summary>
        /// Identifies the NeedleBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty NeedleBrushProperty =
            DependencyProperty.Register(nameof(NeedleBrush), typeof(SolidColorBrush), typeof(RadialGauge), new PropertyMetadata(null, OnFaceChanged));

        /// <summary>
        /// Identifies the Unit dependency property.
        /// </summary>
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register(nameof(Unit), typeof(string), typeof(RadialGauge), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Identifies the TrailBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty TrailBrushProperty =
            DependencyProperty.Register(nameof(TrailBrush), typeof(Brush), typeof(RadialGauge), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the ScaleBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty ScaleBrushProperty =
            DependencyProperty.Register(nameof(ScaleBrush), typeof(Brush), typeof(RadialGauge), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the ScaleTickBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty ScaleTickBrushProperty =
            DependencyProperty.Register(nameof(ScaleTickBrush), typeof(Brush), typeof(RadialGauge), new PropertyMetadata(null, OnFaceChanged));

        /// <summary>
        /// Identifies the TickBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty TickBrushProperty =
            DependencyProperty.Register(nameof(TickBrush), typeof(SolidColorBrush), typeof(RadialGauge), new PropertyMetadata(null, OnFaceChanged));

        /// <summary>
        /// Identifies the ValueStringFormat dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueStringFormatProperty =
            DependencyProperty.Register(nameof(ValueStringFormat), typeof(string), typeof(RadialGauge), new PropertyMetadata("N0", (s, e) => OnValueChanged(s)));

        /// <summary>
        /// Identifies the TickSpacing dependency property.
        /// </summary>
        public static readonly DependencyProperty TickSpacingProperty =
        DependencyProperty.Register(nameof(TickSpacing), typeof(int), typeof(RadialGauge), new PropertyMetadata(10, OnFaceChanged));

        /// <summary>
        /// Identifies the NeedleLength dependency property.
        /// </summary>
        public static readonly DependencyProperty NeedleLengthProperty =
            DependencyProperty.Register(nameof(NeedleLength), typeof(double), typeof(RadialGauge), new PropertyMetadata(100d, OnFaceChanged));

        /// <summary>
        /// Identifies the NeedleWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty NeedleWidthProperty =
            DependencyProperty.Register(nameof(NeedleWidth), typeof(double), typeof(RadialGauge), new PropertyMetadata(5d, OnFaceChanged));

        /// <summary>
        /// Identifies the ScalePadding dependency property.
        /// </summary>
        public static readonly DependencyProperty ScalePaddingProperty =
            DependencyProperty.Register(nameof(ScalePadding), typeof(double), typeof(RadialGauge), new PropertyMetadata(23d, OnFaceChanged));

        /// <summary>
        /// Identifies the ScaleTickWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty ScaleTickWidthProperty =
            DependencyProperty.Register(nameof(ScaleTickWidth), typeof(double), typeof(RadialGauge), new PropertyMetadata(2.5, OnFaceChanged));

        /// <summary>
        /// Identifies the TickWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty TickWidthProperty =
            DependencyProperty.Register(nameof(TickWidth), typeof(double), typeof(RadialGauge), new PropertyMetadata(5d, OnFaceChanged));

        /// <summary>
        /// Identifies the TickLength dependency property.
        /// </summary>
        public static readonly DependencyProperty TickLengthProperty =
            DependencyProperty.Register(nameof(TickLength), typeof(double), typeof(RadialGauge), new PropertyMetadata(18d, OnFaceChanged));

        /// <summary>
        /// Identifies the MinAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty MinAngleProperty =
            DependencyProperty.Register(nameof(MinAngle), typeof(int), typeof(RadialGauge), new PropertyMetadata(-150, OnScaleChanged));

        /// <summary>
        /// Identifies the MaxAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxAngleProperty =
            DependencyProperty.Register(nameof(MaxAngle), typeof(int), typeof(RadialGauge), new PropertyMetadata(150, OnScaleChanged));

        /// <summary>
        /// Identifies the ValueAngle dependency property.
        /// </summary>
        protected static readonly DependencyProperty ValueAngleProperty =
            DependencyProperty.Register(nameof(ValueAngle), typeof(double), typeof(RadialGauge), new PropertyMetadata(null));

        // Template Parts.
        private const string ContainerPartName = "PART_Container";
        private const string ScalePartName = "PART_Scale";
        private const string TrailPartName = "PART_Trail";
        private const string ValueTextPartName = "PART_ValueText";

        // For convenience.
        private const double Degrees2Radians = Math.PI / 180;

        // High-contrast accessibility
        private static readonly ThemeListener ThemeListener = new ThemeListener();
        private SolidColorBrush _needleBrush;
        private Brush _trailBrush;
        private Brush _scaleBrush;
        private SolidColorBrush _scaleTickBrush;
        private SolidColorBrush _tickBrush;
        private Brush _foreground;

        private double _normalizedMinAngle;
        private double _normalizedMaxAngle;

        private Compositor _compositor;
        private ContainerVisual _root;
        private SpriteVisual _needle;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadialGauge"/> class.
        /// Create a default radial gauge control.
        /// </summary>
        public RadialGauge()
        {
            DefaultStyleKey = typeof(RadialGauge);

            SmallChange = 1;
            LargeChange = 10;
            Unloaded += RadialGauge_Unloaded;
        }

        private void ThemeListener_ThemeChanged(ThemeListener sender)
        {
            OnColorsChanged();
        }

        private void RadialGauge_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            double step = SmallChange;
            var ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
            if (ctrl.HasFlag(CoreVirtualKeyStates.Down))
            {
                step = LargeChange;
            }

            step = Math.Max(StepSize, step);
            if ((e.Key == VirtualKey.Left) || (e.Key == VirtualKey.Down))
            {
                Value = Math.Max(Minimum, Value - step);
                e.Handled = true;
                return;
            }

            if ((e.Key == VirtualKey.Right) || (e.Key == VirtualKey.Up))
            {
                Value = Math.Min(Maximum, Value + step);
                e.Handled = true;
            }
        }

        private void RadialGauge_Unloaded(object sender, RoutedEventArgs e)
        {
            // Unregister event handlers.
            KeyDown -= RadialGauge_KeyDown;
            ThemeListener.ThemeChanged -= ThemeListener_ThemeChanged;
            PointerReleased -= RadialGauge_PointerReleased;
            Unloaded -= RadialGauge_Unloaded;
        }

        /// <summary>
        /// Gets or sets the rounding interval for the Value.
        /// </summary>
        public double StepSize
        {
            get { return (double)GetValue(StepSizeProperty); }
            set { SetValue(StepSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control accepts setting its value through interaction.
        /// </summary>
        public bool IsInteractive
        {
            get { return (bool)GetValue(IsInteractiveProperty); }
            set { SetValue(IsInteractiveProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the scale, in percentage of the gauge radius.
        /// </summary>
        public double ScaleWidth
        {
            get { return (double)GetValue(ScaleWidthProperty); }
            set { SetValue(ScaleWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the displayed unit measure.
        /// </summary>
        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        /// <summary>
        /// Gets or sets the needle brush.
        /// </summary>
        public SolidColorBrush NeedleBrush
        {
            get { return (SolidColorBrush)GetValue(NeedleBrushProperty); }
            set { SetValue(NeedleBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the trail brush.
        /// </summary>
        public Brush TrailBrush
        {
            get { return (Brush)GetValue(TrailBrushProperty); }
            set { SetValue(TrailBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the scale brush.
        /// </summary>
        public Brush ScaleBrush
        {
            get { return (Brush)GetValue(ScaleBrushProperty); }
            set { SetValue(ScaleBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the scale tick brush.
        /// </summary>
        public SolidColorBrush ScaleTickBrush
        {
            get { return (SolidColorBrush)GetValue(ScaleTickBrushProperty); }
            set { SetValue(ScaleTickBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the outer tick brush.
        /// </summary>
        public SolidColorBrush TickBrush
        {
            get { return (SolidColorBrush)GetValue(TickBrushProperty); }
            set { SetValue(TickBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value string format.
        /// </summary>
        public string ValueStringFormat
        {
            get { return (string)GetValue(ValueStringFormatProperty); }
            set { SetValue(ValueStringFormatProperty, value); }
        }

        /// <summary>
        /// Gets or sets the tick spacing, in units. Values of zero or less will be ignored when drawing.
        /// </summary>
        public int TickSpacing
        {
            get { return (int)GetValue(TickSpacingProperty); }
            set { SetValue(TickSpacingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the needle length, in percentage of the gauge radius.
        /// </summary>
        public double NeedleLength
        {
            get { return (double)GetValue(NeedleLengthProperty); }
            set { SetValue(NeedleLengthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the needle width, in percentage of the gauge radius.
        /// </summary>
        public double NeedleWidth
        {
            get { return (double)GetValue(NeedleWidthProperty); }
            set { SetValue(NeedleWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the distance of the scale from the outside of the control, in percentage of the gauge radius.
        /// </summary>
        public double ScalePadding
        {
            get { return (double)GetValue(ScalePaddingProperty); }
            set { SetValue(ScalePaddingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the scale ticks, in percentage of the gauge radius.
        /// </summary>
        public double ScaleTickWidth
        {
            get { return (double)GetValue(ScaleTickWidthProperty); }
            set { SetValue(ScaleTickWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the length of the ticks, in percentage of the gauge radius.
        /// </summary>
        public double TickLength
        {
            get { return (double)GetValue(TickLengthProperty); }
            set { SetValue(TickLengthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the ticks, in percentage of the gauge radius.
        /// </summary>
        public double TickWidth
        {
            get { return (double)GetValue(TickWidthProperty); }
            set { SetValue(TickWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the start angle of the scale, which corresponds with the Minimum value, in degrees.
        /// </summary>
        /// <remarks>Changing MinAngle may require retemplating the control.</remarks>
        public int MinAngle
        {
            get { return (int)GetValue(MinAngleProperty); }
            set { SetValue(MinAngleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the end angle of the scale, which corresponds with the Maximum value, in degrees.
        /// </summary>
        /// <remarks>Changing MaxAngle may require retemplating the control.</remarks>
        public int MaxAngle
        {
            get { return (int)GetValue(MaxAngleProperty); }
            set { SetValue(MaxAngleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the current angle of the needle (between MinAngle and MaxAngle). Setting the angle will update the Value.
        /// </summary>
        protected double ValueAngle
        {
            get { return (double)GetValue(ValueAngleProperty); }
            set { SetValue(ValueAngleProperty, value); }
        }

        /// <summary>
        /// Gets the normalized minimum angle.
        /// </summary>
        /// <value>The minimum angle in the range from -180 to 180.</value>
        protected double NormalizedMinAngle => _normalizedMinAngle;

        /// <summary>
        /// Gets the normalized maximum angle.
        /// </summary>
        /// <value>The maximum angle, in the range from -180 to 540.</value>
        protected double NormalizedMaxAngle => _normalizedMaxAngle;

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadialGaugeAutomationPeer(this);
        }

        /// <summary>
        /// Update the visual state of the control when its template is changed.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            // Remember local brushes.
            _needleBrush = ReadLocalValue(NeedleBrushProperty) as SolidColorBrush;
            _trailBrush = ReadLocalValue(TrailBrushProperty) as SolidColorBrush;
            _scaleBrush = ReadLocalValue(ScaleBrushProperty) as SolidColorBrush;
            _scaleTickBrush = ReadLocalValue(ScaleTickBrushProperty) as SolidColorBrush;
            _tickBrush = ReadLocalValue(TickBrushProperty) as SolidColorBrush;
            _foreground = ReadLocalValue(ForegroundProperty) as SolidColorBrush;

            // Register event handlers.
            PointerReleased += RadialGauge_PointerReleased;
            ThemeListener.ThemeChanged += ThemeListener_ThemeChanged;
            KeyDown += RadialGauge_KeyDown;

            // Apply color scheme.
            OnColorsChanged();

            base.OnApplyTemplate();
        }

        /// <inheritdoc/>
        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);
            OnScaleChanged(this);
        }

        /// <inheritdoc/>
        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);
            OnScaleChanged(this);
        }

        /// <inheritdoc/>
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            OnValueChanged(this);
            base.OnValueChanged(oldValue, newValue);
            if (AutomationPeer.ListenerExists(AutomationEvents.LiveRegionChanged))
            {
                var peer = FrameworkElementAutomationPeer.FromElement(this) as RadialGaugeAutomationPeer;
                peer?.RaiseValueChangedEvent(oldValue, newValue);
            }
        }

        private static void OnValueChanged(DependencyObject d)
        {
            RadialGauge radialGauge = (RadialGauge)d;
            if (!double.IsNaN(radialGauge.Value))
            {
                if (radialGauge.StepSize != 0)
                {
                    radialGauge.Value = radialGauge.RoundToMultiple(radialGauge.Value, radialGauge.StepSize);
                }

                var middleOfScale = 100 - radialGauge.ScalePadding - (radialGauge.ScaleWidth / 2);
                var valueText = radialGauge.GetTemplateChild(ValueTextPartName) as TextBlock;
                radialGauge.ValueAngle = radialGauge.ValueToAngle(radialGauge.Value);

                // Needle
                if (radialGauge._needle != null)
                {
                    radialGauge._needle.RotationAngleInDegrees = (float)radialGauge.ValueAngle;
                }

                // Trail
                var trail = radialGauge.GetTemplateChild(TrailPartName) as Path;
                if (trail != null)
                {
                    if (radialGauge.ValueAngle > radialGauge.NormalizedMinAngle)
                    {
                        trail.Visibility = Visibility.Visible;

                        if (radialGauge.ValueAngle - radialGauge.NormalizedMinAngle == 360)
                        {
                            // Draw full circle.
                            var eg = new EllipseGeometry();
                            eg.Center = new Point(100, 100);
                            eg.RadiusX = 100 - radialGauge.ScalePadding - (radialGauge.ScaleWidth / 2);
                            eg.RadiusY = eg.RadiusX;
                            trail.Data = eg;
                        }
                        else
                        {
                            // Draw arc.
                            var pg = new PathGeometry();
                            var pf = new PathFigure();
                            pf.IsClosed = false;
                            pf.StartPoint = radialGauge.ScalePoint(radialGauge.NormalizedMinAngle, middleOfScale);
                            var seg = new ArcSegment();
                            seg.SweepDirection = SweepDirection.Clockwise;
                            seg.IsLargeArc = radialGauge.ValueAngle > (180 + radialGauge.NormalizedMinAngle);
                            seg.Size = new Size(middleOfScale, middleOfScale);
                            seg.Point = radialGauge.ScalePoint(Math.Min(radialGauge.ValueAngle, radialGauge.NormalizedMaxAngle), middleOfScale);  // On overflow, stop trail at MaxAngle.
                            pf.Segments.Add(seg);
                            pg.Figures.Add(pf);
                            trail.Data = pg;
                        }
                    }
                    else
                    {
                        trail.Visibility = Visibility.Collapsed;
                    }
                }

                // Value Text
                if (valueText != null)
                {
                    valueText.Text = radialGauge.Value.ToString(radialGauge.ValueStringFormat);
                }
            }
        }

        private static void OnInteractivityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadialGauge radialGauge = (RadialGauge)d;

            if (radialGauge.IsInteractive)
            {
                radialGauge.Tapped += radialGauge.RadialGauge_Tapped;
                radialGauge.ManipulationDelta += radialGauge.RadialGauge_ManipulationDelta;
                radialGauge.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            }
            else
            {
                radialGauge.Tapped -= radialGauge.RadialGauge_Tapped;
                radialGauge.ManipulationDelta -= radialGauge.RadialGauge_ManipulationDelta;
                radialGauge.ManipulationMode = ManipulationModes.None;
            }
        }

        private static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnScaleChanged(d);
        }

        private static void OnScaleChanged(DependencyObject d)
        {
            RadialGauge radialGauge = (RadialGauge)d;

            radialGauge.UpdateNormalizedAngles();

            var scale = radialGauge.GetTemplateChild(ScalePartName) as Path;
            if (scale != null)
            {
                if (radialGauge.NormalizedMaxAngle - radialGauge.NormalizedMinAngle == 360)
                {
                    // Draw full circle.
                    var eg = new EllipseGeometry();
                    eg.Center = new Point(100, 100);
                    eg.RadiusX = 100 - radialGauge.ScalePadding - (radialGauge.ScaleWidth / 2);
                    eg.RadiusY = eg.RadiusX;
                    scale.Data = eg;
                }
                else
                {
                    // Draw arc.
                    var pg = new PathGeometry();
                    var pf = new PathFigure();
                    pf.IsClosed = false;
                    var middleOfScale = 100 - radialGauge.ScalePadding - (radialGauge.ScaleWidth / 2);
                    pf.StartPoint = radialGauge.ScalePoint(radialGauge.NormalizedMinAngle, middleOfScale);
                    var seg = new ArcSegment();
                    seg.SweepDirection = SweepDirection.Clockwise;
                    seg.IsLargeArc = radialGauge.NormalizedMaxAngle > (radialGauge.NormalizedMinAngle + 180);
                    seg.Size = new Size(middleOfScale, middleOfScale);
                    seg.Point = radialGauge.ScalePoint(radialGauge.NormalizedMaxAngle, middleOfScale);
                    pf.Segments.Add(seg);
                    pg.Figures.Add(pf);
                    scale.Data = pg;
                }

                if (!DesignTimeHelpers.IsRunningInLegacyDesignerMode)
                {
                    OnFaceChanged(radialGauge);
                }
            }
        }

        private static void OnFaceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignTimeHelpers.IsRunningInLegacyDesignerMode)
            {
                OnFaceChanged(d);
            }
        }

        private static void OnFaceChanged(DependencyObject d)
        {
            RadialGauge radialGauge = (RadialGauge)d;

            var container = radialGauge.GetTemplateChild(ContainerPartName) as Grid;
            if (container == null || DesignTimeHelpers.IsRunningInLegacyDesignerMode)
            {
                // Bad template.
                return;
            }

            radialGauge._root = container.GetVisual();
            radialGauge._root.Children.RemoveAll();
            radialGauge._compositor = radialGauge._root.Compositor;

            if (radialGauge.TickSpacing > 0)
            {
                // Ticks.
                SpriteVisual tick;
                for (double i = radialGauge.Minimum; i <= radialGauge.Maximum; i += radialGauge.TickSpacing)
                {
                    tick = radialGauge._compositor.CreateSpriteVisual();
                    tick.Size = new Vector2((float)radialGauge.TickWidth, (float)radialGauge.TickLength);
                    tick.Brush = radialGauge._compositor.CreateColorBrush(radialGauge.TickBrush.Color);
                    tick.Offset = new Vector3(100 - ((float)radialGauge.TickWidth / 2), 0.0f, 0);
                    tick.CenterPoint = new Vector3((float)radialGauge.TickWidth / 2, 100.0f, 0);
                    tick.RotationAngleInDegrees = (float)radialGauge.ValueToAngle(i);
                    radialGauge._root.Children.InsertAtTop(tick);
                }

                // Scale Ticks.
                for (double i = radialGauge.Minimum; i <= radialGauge.Maximum; i += radialGauge.TickSpacing)
                {
                    tick = radialGauge._compositor.CreateSpriteVisual();
                    tick.Size = new Vector2((float)radialGauge.ScaleTickWidth, (float)radialGauge.ScaleWidth);
                    tick.Brush = radialGauge._compositor.CreateColorBrush(radialGauge.ScaleTickBrush.Color);
                    tick.Offset = new Vector3(100 - ((float)radialGauge.ScaleTickWidth / 2), (float)radialGauge.ScalePadding, 0);
                    tick.CenterPoint = new Vector3((float)radialGauge.ScaleTickWidth / 2, 100 - (float)radialGauge.ScalePadding, 0);
                    tick.RotationAngleInDegrees = (float)radialGauge.ValueToAngle(i);
                    radialGauge._root.Children.InsertAtTop(tick);
                }
            }

            // Needle.
            radialGauge._needle = radialGauge._compositor.CreateSpriteVisual();
            radialGauge._needle.Size = new Vector2((float)radialGauge.NeedleWidth, (float)radialGauge.NeedleLength);
            radialGauge._needle.Brush = radialGauge._compositor.CreateColorBrush(radialGauge.NeedleBrush.Color);
            radialGauge._needle.CenterPoint = new Vector3((float)radialGauge.NeedleWidth / 2, (float)radialGauge.NeedleLength, 0);
            radialGauge._needle.Offset = new Vector3(100 - ((float)radialGauge.NeedleWidth / 2), 100 - (float)radialGauge.NeedleLength, 0);
            radialGauge._root.Children.InsertAtTop(radialGauge._needle);

            OnValueChanged(radialGauge);
        }

        private void OnColorsChanged()
        {
            if (ThemeListener.IsHighContrast)
            {
                // Apply High Contrast Theme.
                ClearBrush(_needleBrush, NeedleBrushProperty);
                ClearBrush(_trailBrush, TrailBrushProperty);
                ClearBrush(_scaleBrush, ScaleBrushProperty);
                ClearBrush(_scaleBrush, ScaleTickBrushProperty);
                ClearBrush(_tickBrush, TickBrushProperty);
                ClearBrush(_foreground, ForegroundProperty);
            }
            else
            {
                // Apply User Defined or Default Theme.
                RestoreBrush(_needleBrush, NeedleBrushProperty);
                RestoreBrush(_trailBrush, TrailBrushProperty);
                RestoreBrush(_scaleBrush, ScaleBrushProperty);
                RestoreBrush(_scaleBrush, ScaleTickBrushProperty);
                RestoreBrush(_tickBrush, TickBrushProperty);
                RestoreBrush(_foreground, ForegroundProperty);
            }

            OnScaleChanged(this);
        }

        private void ClearBrush(Brush brush, DependencyProperty prop)
        {
            if (brush != null)
            {
                ClearValue(prop);
            }
        }

        private void RestoreBrush(Brush source, DependencyProperty prop)
        {
            if (source != null)
            {
                SetValue(prop, source);
            }
        }

        private void RadialGauge_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            SetGaugeValueFromPoint(e.Position);
        }

        private void RadialGauge_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SetGaugeValueFromPoint(e.GetPosition(this));
        }

        private void RadialGauge_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (IsInteractive)
            {
                e.Handled = true;
            }
        }

        private void UpdateNormalizedAngles()
        {
            var result = Mod(MinAngle, 360);

            if (result >= 180)
            {
                result = result - 360;
            }

            _normalizedMinAngle = result;

            result = Mod(MaxAngle, 360);

            if (result < 180)
            {
                result = result + 360;
            }

            if (result > NormalizedMinAngle + 360)
            {
                result = result - 360;
            }

            _normalizedMaxAngle = result;
        }

        private void SetGaugeValueFromPoint(Point p)
        {
            var pt = new Point(p.X - (ActualWidth / 2), -p.Y + (ActualHeight / 2));

            var angle = Math.Atan2(pt.X, pt.Y) / Degrees2Radians;
            var divider = Mod(NormalizedMaxAngle - NormalizedMinAngle, 360);
            if (divider == 0)
            {
                divider = 360;
            }

            var value = Minimum + ((Maximum - Minimum) * Mod(angle - NormalizedMinAngle, 360) / divider);
            if (value < Minimum || value > Maximum)
            {
                // Ignore positions outside the scale angle.
                return;
            }

            Value = RoundToMultiple(value, StepSize);
        }

        private Point ScalePoint(double angle, double middleOfScale)
        {
            return new Point(100 + (Math.Sin(Degrees2Radians * angle) * middleOfScale), 100 - (Math.Cos(Degrees2Radians * angle) * middleOfScale));
        }

        private double ValueToAngle(double value)
        {
            // Off-scale on the left.
            if (value < Minimum)
            {
                return MinAngle;
            }

            // Off-scale on the right.
            if (value > Maximum)
            {
                return MaxAngle;
            }

            return ((value - Minimum) / (Maximum - Minimum) * (NormalizedMaxAngle - NormalizedMinAngle)) + NormalizedMinAngle;
        }

        private double Mod(double number, double divider)
        {
            var result = number % divider;
            result = result < 0 ? result + divider : result;
            return result;
        }

        private double RoundToMultiple(double number, double multiple)
        {
            double modulo = number % multiple;
            if (double.IsNaN(modulo))
            {
                return number;
            }

            if ((multiple - modulo) <= modulo)
            {
                modulo = multiple - modulo;
            }
            else
            {
                modulo *= -1;
            }

            return number + modulo;
        }
    }
}