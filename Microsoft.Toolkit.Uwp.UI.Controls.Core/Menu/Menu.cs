// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Menu Control defines a menu of choices for users to invoke.
    /// </summary>
    [Obsolete("This control will be removed in a future major release. Please use the MenuBar control from the WinUI Library instead.")]
    public partial class Menu : ItemsControl
    {
        private WrapPanel _wrapPanel;

        /// <summary>
        /// Initializes a new instance of the <see cref="Menu"/> class.
        /// </summary>
        public Menu()
        {
            DefaultStyleKey = typeof(Menu);
        }

        // even if we have multiple menus in the same page we need only one cache because only one menu item will have certain short cut.
        private static readonly Dictionary<string, DependencyObject> MenuItemInputGestureCache = new Dictionary<string, DependencyObject>();

        /// <summary>
        /// Gets or sets the orientation of the Menu, Horizontal or vertical means that child controls will be added horizontally
        /// until the width of the panel can't fit more control then a new row is added to fit new horizontal added child controls,
        /// vertical means that child will be added vertically until the height of the panel is received then a new column is added
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                typeof(Menu),
                new PropertyMetadata(Orientation.Horizontal, OrientationPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="MenuFlyoutStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MenuFlyoutStyleProperty = DependencyProperty.Register(nameof(MenuFlyoutStyle), typeof(Style), typeof(Menu), new PropertyMetadata(default(Style)));

        /// <summary>
        /// Gets or sets the menu style for MenuItem
        /// </summary>
        public Style MenuFlyoutStyle
        {
            get { return (Style)GetValue(MenuFlyoutStyleProperty); }
            set { SetValue(MenuFlyoutStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="TooltipStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TooltipStyleProperty = DependencyProperty.Register(nameof(TooltipStyle), typeof(Style), typeof(Menu), new PropertyMetadata(default(Style)));

        /// <summary>
        /// Gets or sets the tooltip styles for menu
        /// </summary>
        public Style TooltipStyle
        {
            get { return (Style)GetValue(TooltipStyleProperty); }
            set { SetValue(TooltipStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="TooltipPlacement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TooltipPlacementProperty = DependencyProperty.Register(nameof(TooltipPlacement), typeof(PlacementMode), typeof(Menu), new PropertyMetadata(default(PlacementMode)));

        /// <summary>
        /// Gets or sets the tooltip placement on menu
        /// </summary>
        public PlacementMode TooltipPlacement
        {
            get { return (PlacementMode)GetValue(TooltipPlacementProperty); }
            set { SetValue(TooltipPlacementProperty, value); }
        }

        /// <summary>
        /// Gets the current selected menu header item
        /// </summary>
        public MenuItem SelectedMenuItem { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the menu is opened or not
        /// </summary>
        public bool IsOpened { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the menu is in transition state between menu closing state and menu opened state.
        /// </summary>
        internal bool IsInTransitionState { get; set; }

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            Loaded -= Menu_Loaded;
            Unloaded -= Menu_Unloaded;

            Loaded += Menu_Loaded;
            Unloaded += Menu_Unloaded;

            base.OnApplyTemplate();
        }

        /// <inheritdoc />
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is MenuItem;
        }

        /// <inheritdoc />
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MenuItem();
        }
    }
}