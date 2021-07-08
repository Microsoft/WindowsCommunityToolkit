// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// ItemsControl that lays out items in a circle with support for orbits and anchors
    /// </summary>
    [TemplatePart(Name = "AnchorCanvas", Type = typeof(Canvas))]
    [TemplatePart(Name = "OrbitGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "CenterContent", Type = typeof(ContentPresenter))]
    public partial class OrbitView : ItemsControl
    {
        private const double AnimationDuration = 200;

        private OrbitViewPanel _panel;
        private Grid _orbitsContainer;
        private Canvas _anchorCanvas;
        private ContentPresenter _centerContent;
        private Compositor _compositor;

        private Dictionary<object, Ellipse> _orbits;
        private Dictionary<object, Line> _anchors;

        /// <summary>
        /// Raised when an item has been clicked or activated with keyboard/controller
        /// </summary>
        public event EventHandler<OrbitViewItemClickedEventArgs> ItemClick;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrbitView"/> class.
        /// Creates a new instance of <see cref="OrbitView"/>
        /// </summary>
        public OrbitView()
        {
            DefaultStyleKey = typeof(OrbitView);

            IsTabStop = false;
            TabNavigation = KeyboardNavigationMode.Once;
            KeyDown += OrbitView_KeyDown;
            _orbits = new Dictionary<object, Ellipse>();
            _anchors = new Dictionary<object, Line>();

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                var items = new List<OrbitViewDataItem>
                {
                    new OrbitViewDataItem() { Distance = 0.1, Diameter = 0.5, Label = "test" },
                    new OrbitViewDataItem() { Distance = 0.1, Diameter = 0.5, Label = "test" },
                    new OrbitViewDataItem() { Distance = 0.1, Diameter = 0.5, Label = "test" }
                };
                ItemsSource = items;
            }
        }

        /// <summary>
        /// Invoked whenever application code or internal processes call ApplyTemplate
        /// </summary>
        protected override void OnApplyTemplate()
        {
            _centerContent = GetTemplateChild("CenterContent") as ContentPresenter;
            if (_centerContent == null)
            {
                return;
            }

            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Gets or sets a value indicating whether orbits are enabled or not.
        /// </summary>
        public bool OrbitsEnabled
        {
            get { return (bool)GetValue(OrbitsEnabledProperty); }
            set { SetValue(OrbitsEnabledProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="OrbitsEnabled"/> property
        /// </summary>
        public static readonly DependencyProperty OrbitsEnabledProperty =
            DependencyProperty.Register(nameof(OrbitsEnabled), typeof(bool), typeof(OrbitView), new PropertyMetadata(false, OnOrbitsEnabledChanged));

        /// <summary>
        /// Gets or sets a value indicating whether elements are clickable.
        /// </summary>
        public bool IsItemClickEnabled
        {
            get { return (bool)GetValue(IsItemClickEnabledProperty); }
            set { SetValue(IsItemClickEnabledProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsItemClickEnabled"/> property
        /// </summary>
        public static readonly DependencyProperty IsItemClickEnabledProperty =
            DependencyProperty.Register(nameof(IsItemClickEnabled), typeof(bool), typeof(OrbitView), new PropertyMetadata(false, OnItemClickEnabledChanged));

        /// <summary>
        /// Gets or sets a value indicating whether anchors are enabled.
        /// </summary>
        public bool AnchorsEnabled
        {
            get { return (bool)GetValue(AnchorsEnabledProperty); }
            set { SetValue(AnchorsEnabledProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AnchorsEnabled"/> property
        /// </summary>
        public static readonly DependencyProperty AnchorsEnabledProperty =
            DependencyProperty.Register(nameof(AnchorsEnabled), typeof(bool), typeof(OrbitView), new PropertyMetadata(false, OnAchorsEnabledChanged));

        /// <summary>
        /// Gets or sets a value indicating the minimum size of items
        /// Note: for this property to work, Data Context must be derived from OrbitViewItems
        /// and Diameter must be between 0 and 1
        /// </summary>
        public double MinItemSize
        {
            get { return (double)GetValue(MinItemSizeProperty); }
            set { SetValue(MinItemSizeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MinItemSize"/> property
        /// </summary>
        public static readonly DependencyProperty MinItemSizeProperty =
            DependencyProperty.Register(nameof(MinItemSize), typeof(double), typeof(OrbitView), new PropertyMetadata(20d, OnItemSizePropertyChanged));

        /// <summary>
        /// Gets or sets a value indicating the maximum size of items
        /// Note: for this property to work, Data Context must be derived from OrbitViewItems
        /// and Diameter must be between 0 and 1
        /// </summary>
        public double MaxItemSize
        {
            get { return (double)GetValue(MaxItemSizeProperty); }
            set { SetValue(MaxItemSizeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MaxItemSize"/> property
        /// </summary>
        public static readonly DependencyProperty MaxItemSizeProperty =
            DependencyProperty.Register(nameof(MaxItemSize), typeof(double), typeof(OrbitView), new PropertyMetadata(50d, OnItemSizePropertyChanged));

        /// <summary>
        /// Gets or sets a value indicating the color of anchors
        /// </summary>
        public Brush AnchorColor
        {
            get { return (Brush)GetValue(AnchorColorProperty); }
            set { SetValue(AnchorColorProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AnchorColor"/> property
        /// </summary>
        public static readonly DependencyProperty AnchorColorProperty =
            DependencyProperty.Register(nameof(AnchorColor), typeof(Brush), typeof(OrbitView), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnAnchorPropertyChanged));

        /// <summary>
        /// Gets or sets a value indicating the color of orbits
        /// </summary>
        public Brush OrbitColor
        {
            get { return (Brush)GetValue(OrbitColorProperty); }
            set { SetValue(OrbitColorProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="OrbitColor"/> property
        /// </summary>
        public static readonly DependencyProperty OrbitColorProperty =
            DependencyProperty.Register(nameof(OrbitColor), typeof(Brush), typeof(OrbitView), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnOrbitPropertyChanged));

        /// <summary>
        /// Gets or sets a value indicating the dash array for the orbit
        /// </summary>
        public DoubleCollection OrbitDashArray
        {
            get { return (DoubleCollection)GetValue(OrbitDashArrayProperty); }
            set { SetValue(OrbitDashArrayProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="OrbitDashArray"/> property
        /// </summary>
        public static readonly DependencyProperty OrbitDashArrayProperty =
            DependencyProperty.Register(nameof(OrbitDashArray), typeof(DoubleCollection), typeof(OrbitView), new PropertyMetadata(null, OnOrbitPropertyChanged));

        /// <summary>
        /// Gets or sets a value indicating the thickness of the anchors
        /// </summary>
        public double AnchorThickness
        {
            get { return (double)GetValue(AnchorThicknessProperty); }
            set { SetValue(AnchorThicknessProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AnchorThickness"/> property
        /// </summary>
        public static readonly DependencyProperty AnchorThicknessProperty =
            DependencyProperty.Register(nameof(AnchorThickness), typeof(double), typeof(double), new PropertyMetadata(1d, OnAnchorPropertyChanged));

        /// <summary>
        /// Gets or sets a value indicating the thickness of the orbits
        /// </summary>
        public double OrbitThickness
        {
            get { return (double)GetValue(OrbitThicknessProperty); }
            set { SetValue(OrbitThicknessProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="OrbitThickness"/> property
        /// </summary>
        public static readonly DependencyProperty OrbitThicknessProperty =
            DependencyProperty.Register(nameof(OrbitThickness), typeof(double), typeof(OrbitView), new PropertyMetadata(1d, OnOrbitPropertyChanged));

        /// <summary>
        /// Gets or sets a value representing the center element
        /// </summary>
        public object CenterContent
        {
            get { return (object)GetValue(CenterContentProperty); }
            set { SetValue(CenterContentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="CenterContent"/> property
        /// </summary>
        public static readonly DependencyProperty CenterContentProperty =
            DependencyProperty.Register(nameof(CenterContent), typeof(object), typeof(OrbitView), new PropertyMetadata(null));

        /// <inheritdoc/>
        protected override DependencyObject GetContainerForItemOverride()
        {
            var element = new OrbitViewItem();
            ApplyImplicitOffsetAnimation(element);
            return element;
        }

        /// <summary>
        /// Prepares the specified element to display the specified item
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (_panel == null && ItemsPanelRoot != null)
            {
                _panel = ItemsPanelRoot as OrbitViewPanel;
                _panel.ItemArranged -= OrbitViewPanel_ItemArranged;
                _panel.ItemsArranged -= OrbitViewPanel_ItemsArranged;
                _panel.ItemArranged += OrbitViewPanel_ItemArranged;
                _panel.ItemsArranged += OrbitViewPanel_ItemsArranged;
            }

            var control = element as OrbitViewItem;
            var orbitViewDataItem = item as OrbitViewDataItem;
            var orbitViewElement = element as FrameworkElement;

            if (control != null)
            {
                orbitViewElement = ItemTemplate?.LoadContent() as FrameworkElement;
                if (orbitViewElement == null)
                {
                    var itemEllipse = new Ellipse()
                    {
                        Fill = Foreground,
                    };

                    if (orbitViewDataItem != null && orbitViewDataItem.Image != null)
                    {
                        itemEllipse.Fill = new ImageBrush() { ImageSource = orbitViewDataItem.Image };
                    }

                    orbitViewElement = itemEllipse;
                }

                control.Content = orbitViewElement;
                control.DataContext = item;
                control.KeyUp += OrbitViewItem_KeyUp;
                control.PointerReleased += OrbitViewItem_PointerReleased;

                control.IsClickEnabled = IsItemClickEnabled;
            }
            else if (orbitViewElement != null && orbitViewElement.DataContext is OrbitViewDataItem)
            {
                orbitViewDataItem = (OrbitViewDataItem)orbitViewElement.DataContext;
            }

            if (orbitViewDataItem != null)
            {
                element.SetValue(AutomationProperties.NameProperty, orbitViewDataItem.Label);
                if (orbitViewDataItem.Diameter >= 0)
                {
                    double diameter = Math.Min(orbitViewDataItem.Diameter, 1d);
                    orbitViewElement.Width = orbitViewElement.Height = (diameter * (MaxItemSize - MinItemSize)) + MinItemSize;
                }
            }
            else
            {
                element.SetValue(AutomationProperties.NameProperty, item.ToString());
            }
        }

        /// <summary>
        /// Undoes the effects of the PrepareContainerForItemOverride method
        /// </summary>
        /// <param name="element">The container element</param>
        /// <param name="item">The item</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            _orbits.TryGetValue(element, out Ellipse orbit);

            if (orbit != null)
            {
                _orbits.Remove(element);
                _orbitsContainer.Children.Remove(orbit);
            }

            _anchors.TryGetValue(element, out Line anchor);

            if (anchor != null)
            {
                _anchors.Remove(element);
                _anchorCanvas.Children.Remove(anchor);
            }

            if (element is OrbitViewItem control)
            {
                control.KeyUp -= OrbitViewItem_KeyUp;
                control.PointerReleased -= OrbitViewItem_PointerReleased;
            }
        }

        private static void OnAchorsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var orbitView = d as OrbitView;

            if (e.NewValue == e.OldValue)
            {
                return;
            }

            if (!(bool)e.NewValue)
            {
                orbitView.ClearAnchors();
            }
            else
            {
                orbitView.ItemsPanelRoot?.InvalidateArrange();
            }
        }

        private static void OnOrbitsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var orbitView = d as OrbitView;

            if (e.NewValue == e.OldValue)
            {
                return;
            }

            if (!(bool)e.NewValue)
            {
                orbitView.ClearOrbits();
            }
            else
            {
                orbitView.ItemsPanelRoot?.InvalidateArrange();
            }
        }

        private static void OnItemSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var orbitView = d as OrbitView;

            if (orbitView.ItemsPanelRoot != null)
            {
                foreach (var element in orbitView.ItemsPanelRoot.Children)
                {
                    if (element is ContentControl control && control.DataContext is OrbitViewDataItem)
                    {
                        var item = (OrbitViewDataItem)control.DataContext;
                        if (item.Diameter >= 0)
                        {
                            double diameter = Math.Min(item.Diameter, 1d);
                            var content = (FrameworkElement)control.Content;
                            content.Width = content.Height = (diameter * (orbitView.MaxItemSize - orbitView.MinItemSize)) + orbitView.MinItemSize;
                        }
                    }
                }
            }

            orbitView.ItemsPanelRoot?.InvalidateArrange();
        }

        private static void OnItemClickEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var orbitView = d as OrbitView;
            if (orbitView.Items == null || orbitView.Items.Count == 0 || orbitView.ItemsPanelRoot == null)
            {
                return;
            }

            foreach (var control in orbitView.ItemsPanelRoot.Children)
            {
                (control as OrbitViewItem).IsClickEnabled = (bool)e.NewValue;
            }
        }

        private static void OnOrbitPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var orbitView = d as OrbitView;
            if (orbitView._orbitsContainer == null)
            {
                return;
            }

            foreach (var orbit in orbitView._orbitsContainer.Children)
            {
                orbitView.SetOrbitProperties(orbit as Ellipse);
            }
        }

        private static void OnAnchorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var orbitView = d as OrbitView;
            if (orbitView._anchorCanvas == null)
            {
                return;
            }

            foreach (var anchor in orbitView._anchorCanvas.Children)
            {
                orbitView.SetAnchorProperties(anchor as Line);
            }
        }

        private void OrbitViewItem_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            OnItemClicked((OrbitViewItem)sender);
        }

        private void OrbitViewItem_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Space || e.Key == Windows.System.VirtualKey.GamepadA)
            {
                OnItemClicked((OrbitViewItem)sender);
            }
        }

        private void OrbitViewPanel_ItemsArranged(object sender, OrbitViewPanelItemsArrangedArgs e)
        {
            if (AnchorsEnabled)
            {
                if (_anchorCanvas == null)
                {
                    _anchorCanvas = (Canvas)GetTemplateChild("AnchorCanvas");
                    if (_anchorCanvas == null)
                    {
                        return;
                    }
                }

                if (_anchorCanvas.Children.Count != e.Elements.Count)
                {
                    _anchorCanvas.Children.Clear();
                    foreach (var element in e.Elements)
                    {
                        var anchor = CreateAnchor(element.Element, element.XYFromCenter.X, element.XYFromCenter.Y);
                        _anchorCanvas.Children.Add(anchor);
                        _anchors.Add(element, anchor);
                    }
                }
            }
        }

        private void OrbitViewPanel_ItemArranged(object sender, OrbitViewPanelItemArrangedArgs e)
        {
            if (OrbitsEnabled)
            {
                _orbits.TryGetValue(e.ElementProperties.Element, out Ellipse orbit);

                if (orbit == null)
                {
                    if (_orbitsContainer == null)
                    {
                        _orbitsContainer = (Grid)GetTemplateChild("OrbitGrid");
                        if (_orbitsContainer == null)
                        {
                            return;
                        }
                    }

                    orbit = CreateOrbit();
                    _orbits.Add(e.ElementProperties.Element, orbit);
                    _orbitsContainer.Children.Add(orbit);
                }

                orbit.Height = orbit.Width = e.ElementProperties.DistanceFromCenter * 2;
            }
        }

        private void OrbitView_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (ItemsPanelRoot == null)
            {
                return;
            }

            if (e.Key == Windows.System.VirtualKey.Left)
            {
                e.Handled = true;
                if (GetFocusedElement() is ContentControl currentEllement)
                {
                    var index = ItemsPanelRoot.Children.IndexOf(currentEllement);
                    var nextIndex = (index + 1) % Items.Count;

                    (ItemsPanelRoot.Children.ElementAt(nextIndex) as ContentControl).Focus(FocusState.Keyboard);
                }
            }
            else if (e.Key == Windows.System.VirtualKey.Right)
            {
                e.Handled = true;
                if (GetFocusedElement() is ContentControl currentEllement)
                {
                    var index = ItemsPanelRoot.Children.IndexOf(currentEllement);
                    var nextIndex = index > 0 ? index - 1 : Items.Count - 1;

                    (ItemsPanelRoot.Children.ElementAt(nextIndex) as ContentControl).Focus(FocusState.Keyboard);
                }
            }
        }

        private object GetFocusedElement()
        {
            if (XamlRoot != null)
            {
                return FocusManager.GetFocusedElement(XamlRoot);
            }
            else
            {
                return FocusManager.GetFocusedElement();
            }
        }

        private void OnItemClicked(OrbitViewItem item)
        {
            if (IsItemClickEnabled)
            {
                ItemClick?.Invoke(this, new OrbitViewItemClickedEventArgs(item.DataContext));
            }
        }

        private void ClearOrbits()
        {
            if (_orbitsContainer == null)
            {
                return;
            }

            _orbitsContainer.Children.Clear();
            _orbits.Clear();
        }

        private Ellipse CreateOrbit()
        {
            var orbit = new Ellipse()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            SetOrbitProperties(orbit);

            return orbit;
        }

        private void SetOrbitProperties(Ellipse orbit)
        {
            orbit.StrokeDashArray = OrbitDashArray;
            orbit.Stroke = OrbitColor;
            orbit.StrokeThickness = OrbitThickness;
        }

        private void SetAnchorProperties(Line anchor)
        {
            anchor.Stroke = AnchorColor;
            anchor.StrokeThickness = AnchorThickness;
        }

        private Line CreateAnchor(UIElement element, double x, double y)
        {
            var anchor = new Line()
            {
                X1 = 0,
                Y1 = 0,
                X2 = 80,
                Y2 = 0
            };

            SetAnchorProperties(anchor);

            var anchorVisual = ElementCompositionPreview.GetElementVisual(anchor);
            var elementVisual = ElementCompositionPreview.GetElementVisual(element);
            var centerVisual = ElementCompositionPreview.GetElementVisual(_centerContent);

            string expression = string.Empty;
            var elementY = "(elementVisual.Offset.Y + elementVisual.Size.Y / 2)";
            var centerY = "(centerVisual.Offset.Y + centerVisual.Size.Y / 2)";
            var elementX = "(elementVisual.Offset.X + elementVisual.Size.X / 2)";
            var centerX = "(centerVisual.Offset.X + centerVisual.Size.X / 2)";

            var startingAngle = Math.Atan2(y, x);

            if (startingAngle > Math.PI / 4 && startingAngle < 3 * Math.PI / 4)
            {
                expression = $"Atan((-1 * ({elementX} - {centerX})) / ( {elementY} - {centerY})) - PI / 2";
            }
            else if (startingAngle >= 3 * Math.PI / 4 || startingAngle < -3 * Math.PI / 4)
            {
                expression = $"Atan(({elementY} - {centerY}) / ({elementX} - {centerX})) + PI";
            }
            else if (startingAngle >= -3 * Math.PI / 4 && startingAngle < Math.PI / -4)
            {
                expression = $"Atan(({elementX} - {centerX}) / (-1 * ({elementY} - {centerY}))) + PI  / 2";
            }
            else
            {
                expression = $"Atan(({elementY} - {centerY}) / ({elementX} - {centerX}))";
            }

            anchorVisual.CenterPoint = new Vector3(0);
            var rotationExpression = _compositor.CreateExpressionAnimation();
            rotationExpression.Expression = expression;
            rotationExpression.SetReferenceParameter("centerVisual", centerVisual);
            rotationExpression.SetReferenceParameter("elementVisual", elementVisual);
            anchorVisual.StartAnimation(nameof(anchorVisual.RotationAngle), rotationExpression);

            var offsetExpression = _compositor.CreateExpressionAnimation();
            offsetExpression.Expression = "Vector3(centerVisual.Offset.X + centerVisual.Size.X / 2, centerVisual.Offset.Y + centerVisual.Size.Y / 2, 0)";
            offsetExpression.SetReferenceParameter("centerVisual", centerVisual);
            anchorVisual.StartAnimation(nameof(anchorVisual.Offset), offsetExpression);

            var scaleExpression = _compositor.CreateExpressionAnimation();
            scaleExpression.Expression = $"Vector3(Pow(Pow({elementX} - {centerX}, 2) + Pow({elementY} - {centerY}, 2), 0.5)/80, 1, 1)";
            scaleExpression.SetReferenceParameter("centerVisual", centerVisual);
            scaleExpression.SetReferenceParameter("elementVisual", elementVisual);
            anchorVisual.StartAnimation(nameof(anchorVisual.Scale), scaleExpression);

            return anchor;
        }

        private void ClearAnchors()
        {
            if (_anchorCanvas == null)
            {
                return;
            }

            _anchorCanvas.Children.Clear();
            _anchors.Clear();
        }

        private void ApplyImplicitOffsetAnimation(UIElement element, double delay = 0)
        {
            // don't use animations if running in designer
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                return;
            }

            if (_compositor == null)
            {
                return;
            }

            var easeIn = _compositor.CreateCubicBezierEasingFunction(new Vector2(0.03f, 1.11f), new Vector2(.66f, 1.11f));

            var offsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
            offsetAnimation.Target = nameof(Visual.Offset);
            offsetAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue", easeIn);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(AnimationDuration);
            offsetAnimation.DelayTime = TimeSpan.FromMilliseconds(delay);

            var implicitAnimations = _compositor.CreateImplicitAnimationCollection();
            implicitAnimations[nameof(Visual.Offset)] = offsetAnimation;

            ElementCompositionPreview.GetElementVisual(element).ImplicitAnimations = implicitAnimations;
        }
    }
}