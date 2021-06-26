// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Toolkit.Uwp.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// A modern UI Carousel control. Really flexible. Works with touch, keyboard, mouse.
    /// </summary>
    public partial class Carousel : ItemsControl
    {
        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <returns>The selected item. The default is null.</returns>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="SelectedItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(Carousel), new PropertyMetadata(null, OnCarouselPropertyChanged));

        /// <summary>
        /// Gets or sets the index of the selected item.
        /// </summary>
        /// <returns>The index of the selected item. The default is -1.</returns>
        public int SelectedIndex
        {
            get
            {
                return (int)GetValue(SelectedIndexProperty);
            }

            set
            {
                SetValue(SelectedIndexProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="SelectedIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Carousel), new PropertyMetadata(-1, OnCarouselPropertyChanged));

        /// <summary>
        /// Gets or sets duration of the easing function animation (ms).
        /// </summary>
        public int TransitionDuration
        {
            get { return (int)GetValue(TransitionDurationProperty); }
            set { SetValue(TransitionDurationProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="TransitionDuration"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TransitionDurationProperty = DependencyProperty.Register("TransitionDuration", typeof(int), typeof(Carousel), new PropertyMetadata(200));

        /// <summary>
        /// Gets or sets the depth of items that are not selected.
        /// </summary>
        public int ItemDepth
        {
            get { return (int)GetValue(ItemDepthProperty); }
            set { SetValue(ItemDepthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ItemDepth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemDepthProperty = DependencyProperty.Register("ItemDepth", typeof(int), typeof(Carousel), new PropertyMetadata(0, OnCarouselPropertyChanged));

        /// <summary>
        /// Gets or sets easing function to apply for each transition.
        /// </summary>
        public EasingFunctionBase EasingFunction
        {
            get { return (EasingFunctionBase)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="EasingFunction"/> dependency property
        /// </summary>
        public static readonly DependencyProperty EasingFunctionProperty = DependencyProperty.Register("EasingFunction", typeof(EasingFunctionBase), typeof(Carousel), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the margin for items.
        /// </summary>
        public int ItemMargin
        {
            get { return (int)GetValue(ItemMarginProperty); }
            set { SetValue(ItemMarginProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ItemMargin"/> dependency property
        /// </summary>
        public static readonly DependencyProperty ItemMarginProperty = DependencyProperty.Register("ItemMargin", typeof(int), typeof(Carousel), new PropertyMetadata(0, OnCarouselPropertyChanged));

        /// <summary>
        /// Gets or sets a value indicating whether the items rendered transformations should be opposite compared to the selected item
        /// If false, all the items (except the selected item) will have the exact same transformations
        /// If true, all the items where index > selected index will have an opposite transformation (Rotation X Y and Z will be multiply by -1)
        /// </summary>
        public bool InvertPositive
        {
            get { return (bool)GetValue(InvertPositiveProperty); }
            set { SetValue(InvertPositiveProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="InvertPositive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InvertPositiveProperty =
            DependencyProperty.Register("InvertPositive", typeof(bool), typeof(Carousel), new PropertyMetadata(true, OnCarouselPropertyChanged));

        /// <summary>
        /// Gets or sets rotation angle on X
        /// </summary>
        public double ItemRotationX
        {
            get { return (double)GetValue(ItemRotationXProperty); }
            set { SetValue(ItemRotationXProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ItemRotationX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemRotationXProperty = DependencyProperty.Register("ItemRotationX", typeof(double), typeof(Carousel), new PropertyMetadata(0d, OnCarouselPropertyChanged));

        /// <summary>
        /// Gets or sets rotation angle on Y
        /// </summary>
        public double ItemRotationY
        {
            get { return (double)GetValue(ItemRotationYProperty); }
            set { SetValue(ItemRotationYProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ItemRotationY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemRotationYProperty = DependencyProperty.Register("ItemRotationY", typeof(double), typeof(Carousel), new PropertyMetadata(0d, OnCarouselPropertyChanged));

        /// <summary>
        /// Gets or sets rotation angle on Z
        /// </summary>
        public double ItemRotationZ
        {
            get { return (double)GetValue(ItemRotationZProperty); }
            set { SetValue(ItemRotationZProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ItemRotationZ"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemRotationZProperty = DependencyProperty.Register("ItemRotationZ", typeof(double), typeof(Carousel), new PropertyMetadata(0d, OnCarouselPropertyChanged));

        /// <summary>
        /// Gets or sets the Carousel orientation. Horizontal or Vertical
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Carousel), new PropertyMetadata(Orientation.Horizontal, OnCarouselPropertyChanged));

        /// <summary>
        /// Each time a property change, we have to reposition
        /// </summary>
        private static void OnCarouselPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Carousel carouselControl = (Carousel)d;

            if (e.Property == SelectedIndexProperty)
            {
                var newValue = (int)e.NewValue == -1 ? null : carouselControl.Items[(int)e.NewValue];
                var oldValue = e.OldValue == null ? null : carouselControl.Items.ElementAtOrDefault((int)e.OldValue);
                if (oldValue != null)
                {
                    var item = (CarouselItem)carouselControl.ContainerFromIndex((int)e.OldValue);
                    if (item != null)
                    {
                        item.IsSelected = false;
                    }
                }

                bool isNewSelectedItem = carouselControl.SelectedItem != newValue;

                if (newValue != null)
                {
                    var item = (CarouselItem)carouselControl.ContainerFromIndex((int)e.NewValue);
                    if (item != null)
                    {
                        item.IsSelected = true;
                    }

                    carouselControl.FocusContainerFromIndex((int)e.NewValue);
                }

                // double check
                if (isNewSelectedItem)
                {
                    carouselControl.SetValue(SelectedItemProperty, newValue);
                    var newValues = new List<object>() { newValue };
                    var oldValues = oldValue == null ? new List<object>() : new List<object>() { oldValue };

                    var args = new SelectionChangedEventArgs(oldValues, newValues);
                    carouselControl.SelectionChanged?.Invoke(carouselControl, args);
                    return;
                }
            }
            else if (e.Property == SelectedItemProperty)
            {
                var index = e.NewValue == null ? -1 : carouselControl.Items.IndexOf(e.NewValue);

                // double check
                if (carouselControl.SelectedIndex != index)
                {
                    carouselControl.SetValue(SelectedIndexProperty, index);
                    var newValues = new List<object>() { e.NewValue };
                    var oldValues = e.OldValue == null ? new List<object>() : new List<object>() { e.OldValue };

                    var args = new SelectionChangedEventArgs(oldValues, newValues);
                    carouselControl.SelectionChanged?.Invoke(carouselControl, args);
                    return;
                }
            }

            carouselControl.UpdatePositions();
        }

        private void FocusContainerFromIndex(int index)
        {
            ContentControl oldElem;

            if (ControlHelpers.IsXamlRootAvailable && XamlRoot != null)
            {
                oldElem = FocusManager.GetFocusedElement(XamlRoot) as ContentControl;
            }
            else
            {
                oldElem = FocusManager.GetFocusedElement() as ContentControl;
            }

            var newElem = ContainerFromIndex(index) as ContentControl;

            if (oldElem == newElem)
            {
                return;
            }

            if (newElem != null)
            {
                newElem.IsTabStop = true;
                newElem.Focus(oldElem != null && oldElem.FocusState != FocusState.Unfocused ? oldElem.FocusState : FocusState.Programmatic);
            }

            if (oldElem != null && oldElem is CarouselItem)
            {
                oldElem.IsTabStop = false;
            }
        }

        /// <summary>
        /// Occurs when the selected item changes.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Return ItemsPanel
        /// </summary>
        /// <returns>Gets the ItemsPanel used for the carousel</returns>
        internal CarouselPanel GetItemsPanel()
        {
            return this.FindDescendant<CarouselPanel>();
        }

        /// <summary>
        /// Returns the container used for each item
        /// </summary>
        /// <returns>Returns always a CarouselItem</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CarouselItem();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Carousel"/> class.
        /// </summary>
        public Carousel()
        {
            // Set style
            DefaultStyleKey = typeof(Carousel);
            SetValue(AutomationProperties.NameProperty, "Carousel");
            IsHitTestVisible = true;

            IsTabStop = false;
            TabNavigation = KeyboardNavigationMode.Once;

            // Events registered
            PointerWheelChanged += OnPointerWheelChanged;
            PointerReleased += CarouselControl_PointerReleased;
            KeyDown += Keyboard_KeyUp;

            // Register ItemSource changed to get correct SelectedItem and SelectedIndex
            RegisterPropertyChangedCallback(ItemsSourceProperty, (d, dp) =>
            {
                var carouselControl = (Carousel)d;
                var itemsPanel = carouselControl.GetItemsPanel();

                // Prioritize the SelectedItem over the SelectedIndex
                if (SelectedItem != null && (Items.Count() <= carouselControl.SelectedIndex || Items[carouselControl.SelectedIndex] != SelectedItem))
                {
                    var index = carouselControl.Items.IndexOf(SelectedItem);

                    // Double check
                    if (carouselControl.SelectedIndex != index)
                    {
                        carouselControl.SetValue(SelectedIndexProperty, index);
                    }
                }
                else if (SelectedItem == null && SelectedIndex >= 0 && carouselControl.Items != null && carouselControl.Items.Count > 0)
                {
                    var item = carouselControl.Items[SelectedIndex];

                    // Double check
                    if (carouselControl.SelectedItem != item)
                    {
                        carouselControl.SetValue(SelectedItemProperty, item);
                    }
                }
            });
        }

        /// <summary>
        /// Launch an update positions (and animations) on the ItemsPanel
        /// </summary>
        private void UpdatePositions()
        {
            if (ItemsPanel == null)
            {
                return;
            }

            var itemsPanel = GetItemsPanel();

            if (itemsPanel == null)
            {
                return;
            }

            itemsPanel.UpdatePosition();
        }

        private void CarouselControl_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            Focus(FocusState.Pointer);
            e.Handled = true;
        }

        private void Keyboard_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Down:
                case Windows.System.VirtualKey.GamepadDPadDown:
                case Windows.System.VirtualKey.GamepadLeftThumbstickDown:
                    if (Orientation == Orientation.Vertical)
                    {
                        if (SelectedIndex < Items.Count - 1)
                        {
                            SelectedIndex++;
                        }
                        else if (e.OriginalKey != Windows.System.VirtualKey.Down)
                        {
                            FocusManager.TryMoveFocus(FocusNavigationDirection.Down);
                        }

                        e.Handled = true;
                    }

                    break;
                case Windows.System.VirtualKey.Up:
                case Windows.System.VirtualKey.GamepadDPadUp:
                case Windows.System.VirtualKey.GamepadLeftThumbstickUp:
                    if (Orientation == Orientation.Vertical)
                    {
                        if (SelectedIndex > 0)
                        {
                            SelectedIndex--;
                        }
                        else if (e.OriginalKey != Windows.System.VirtualKey.Up)
                        {
                            FocusManager.TryMoveFocus(FocusNavigationDirection.Up);
                        }

                        e.Handled = true;
                    }

                    break;
                case Windows.System.VirtualKey.Left:
                case Windows.System.VirtualKey.GamepadDPadLeft:
                case Windows.System.VirtualKey.GamepadLeftThumbstickLeft:
                    if (Orientation == Orientation.Horizontal)
                    {
                        if (SelectedIndex > 0)
                        {
                            SelectedIndex--;
                        }
                        else if (e.OriginalKey != Windows.System.VirtualKey.Left)
                        {
                            FocusManager.TryMoveFocus(FocusNavigationDirection.Left);
                        }

                        e.Handled = true;
                    }

                    break;
                case Windows.System.VirtualKey.Right:
                case Windows.System.VirtualKey.GamepadDPadRight:
                case Windows.System.VirtualKey.GamepadLeftThumbstickRight:
                    if (Orientation == Orientation.Horizontal)
                    {
                        if (SelectedIndex < Items.Count - 1)
                        {
                            SelectedIndex++;
                        }
                        else if (e.OriginalKey != Windows.System.VirtualKey.Right)
                        {
                            FocusManager.TryMoveFocus(FocusNavigationDirection.Right);
                        }

                        e.Handled = true;
                    }

                    break;
            }
        }

        internal void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            var index = e.GetCurrentPoint(null).Properties.MouseWheelDelta > 0 ? -1 : 1;
            if (index == -1 && SelectedIndex > 0)
            {
                SelectedIndex--;
            }

            if (index == 1 && SelectedIndex < Items.Count - 1)
            {
                SelectedIndex++;
            }

            e.Handled = true;
        }

        /// <inheritdoc/>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is CarouselItem;
        }

        /// <inheritdoc/>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
            var carouselItem = (CarouselItem)element;
            carouselItem.Selected -= OnCarouselItemSelected;
        }

        /// <inheritdoc/>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            var carouselItem = (CarouselItem)element;
            carouselItem.Selected += OnCarouselItemSelected;

            carouselItem.RenderTransformOrigin = new Point(0.5, 0.5);

            carouselItem.IsTabStop = Items.IndexOf(item) == SelectedIndex;
            carouselItem.UseSystemFocusVisuals = true;

            PlaneProjection planeProjection = new PlaneProjection();
            planeProjection.CenterOfRotationX = 0.5;
            planeProjection.CenterOfRotationY = 0.5;
            planeProjection.CenterOfRotationZ = 0.5;

            var compositeTransform = new CompositeTransform();
            compositeTransform.CenterX = 0.5;
            compositeTransform.CenterY = 0.5;

            carouselItem.Projection = planeProjection;
            carouselItem.RenderTransform = compositeTransform;

            if (item == SelectedItem)
            {
                carouselItem.IsSelected = true;
            }

            carouselItem.ParentCarousel = this;
        }

        /// <summary>
        /// Creates AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
        /// </summary>
        /// <returns>An automation peer for this <see cref="Carousel"/>.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CarouselAutomationPeer(this);
        }

        private void OnCarouselItemSelected(object sender, EventArgs e)
        {
            var item = (CarouselItem)sender;

            SelectedItem = ItemFromContainer(item);
        }

        internal void SetSelectedItem(CarouselItem owner)
        {
            var item = ItemFromContainer(owner);
            SelectedItem = item;
        }
    }
}