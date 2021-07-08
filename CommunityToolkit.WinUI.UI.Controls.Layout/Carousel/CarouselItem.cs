// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.WinUI.UI.Automation.Peers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Represents the container for an item in a Carousel control.
    /// </summary>
    public partial class CarouselItem : SelectorItem
    {
        private const string PointerOverState = "PointerOver";
        private const string PointerOverSelectedState = "PointerOverSelected";
        private const string PressedState = "Pressed";
        private const string PressedSelectedState = "PressedSelected";
        private const string SelectedState = "Selected";
        private const string NormalState = "Normal";

        private WeakReference<Carousel> parentCarousel;

        /// <summary>
        /// Initializes a new instance of the <see cref="CarouselItem"/> class.
        /// </summary>
        public CarouselItem()
        {
            // Set style
            DefaultStyleKey = typeof(CarouselItem);

            RegisterPropertyChangedCallback(SelectorItem.IsSelectedProperty, OnIsSelectedChanged);
        }

        internal Carousel ParentCarousel
        {
            get
            {
                this.parentCarousel.TryGetTarget(out var carousel);
                return carousel;
            }
            set => this.parentCarousel = new WeakReference<Carousel>(value);
        }

        /// <inheritdoc/>
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);

            VisualStateManager.GoToState(this, IsSelected ? PointerOverSelectedState : PointerOverState, true);
        }

        /// <inheritdoc/>
        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);

            VisualStateManager.GoToState(this, IsSelected ? SelectedState : NormalState, true);
        }

        /// <inheritdoc/>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);

            VisualStateManager.GoToState(this, IsSelected ? PressedSelectedState : PressedState, true);
        }

        /// <summary>
        /// Creates AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
        /// </summary>
        /// <returns>An automation peer for this <see cref="CarouselItem"/>.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CarouselItemAutomationPeer(this);
        }

        internal event EventHandler Selected;

        private void OnIsSelectedChanged(DependencyObject sender, DependencyProperty dp)
        {
            var item = (CarouselItem)sender;

            if (item.IsSelected)
            {
                Selected?.Invoke(this, EventArgs.Empty);
                VisualStateManager.GoToState(item, SelectedState, true);
            }
            else
            {
                VisualStateManager.GoToState(item, NormalState, true);
            }
        }
    }
}