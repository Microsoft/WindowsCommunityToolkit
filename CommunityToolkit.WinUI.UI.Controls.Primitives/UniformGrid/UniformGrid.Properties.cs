// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// The UniformGrid spaces items evenly.
    /// </summary>
    public partial class UniformGrid
    {
        /// <summary>
        /// Determines if this element in the grid participates in the auto-layout algorithm.
        /// </summary>
        public static readonly DependencyProperty AutoLayoutProperty =
            DependencyProperty.RegisterAttached(
              "AutoLayout",
              typeof(bool?),
              typeof(UniformGrid),
              new PropertyMetadata(null));

        /// <summary>
        /// Sets the AutoLayout Attached Property Value. Used internally to track items which need to be arranged vs. fixed in place.
        /// Though it its required to use this property to force an element to the 0, 0 position.
        /// </summary>
        /// <param name="element"><see cref="FrameworkElement"/></param>
        /// <param name="value">A true value indicates this item should be automatically arranged.</param>
        public static void SetAutoLayout(FrameworkElement element, bool? value)
        {
            element.SetValue(AutoLayoutProperty, value);
        }

        /// <summary>
        /// Gets the AutoLayout Attached Property Value. Used internally to track items which need to be arranged vs. fixed in place.
        /// </summary>
        /// <param name="element"><see cref="FrameworkElement"/></param>
        /// <returns>A true value indicates this item should be automatically arranged.</returns>
        public static bool? GetAutoLayout(FrameworkElement element)
        {
            return (bool?)element.GetValue(AutoLayoutProperty);
        }

        /// <summary>
        /// Sets the AutoLayout Attached Property Value. Used internally to track items which need to be arranged vs. fixed in place.
        /// </summary>
        /// <param name="element"><see cref="ColumnDefinition"/></param>
        /// <param name="value">A true value indicates this item should be automatically arranged.</param>
        internal static void SetAutoLayout(ColumnDefinition element, bool? value)
        {
            element.SetValue(AutoLayoutProperty, value);
        }

        /// <summary>
        /// Gets the AutoLayout Attached Property Value. Used internally to track items which need to be arranged vs. fixed in place.
        /// </summary>
        /// <param name="element"><see cref="ColumnDefinition"/></param>
        /// <returns>A true value indicates this item should be automatically arranged.</returns>
        internal static bool? GetAutoLayout(ColumnDefinition element)
        {
            return (bool?)element.GetValue(AutoLayoutProperty);
        }

        /// <summary>
        /// Sets the AutoLayout Attached Property Value. Used internally to track items which need to be arranged vs. fixed in place.
        /// </summary>
        /// <param name="element"><see cref="RowDefinition"/></param>
        /// <param name="value">A true value indicates this item should be automatically arranged.</param>
        internal static void SetAutoLayout(RowDefinition element, bool? value)
        {
            element.SetValue(AutoLayoutProperty, value);
        }

        /// <summary>
        /// Gets the AutoLayout Attached Property Value. Used internally to track items which need to be arranged vs. fixed in place.
        /// </summary>
        /// <param name="element"><see cref="RowDefinition"/></param>
        /// <returns>A true value indicates this item should be automatically arranged.</returns>
        internal static bool? GetAutoLayout(RowDefinition element)
        {
            return (bool?)element.GetValue(AutoLayoutProperty);
        }

        /// <summary>
        /// Identifies the <see cref="Columns"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(nameof(Columns), typeof(int), typeof(UniformGrid), new PropertyMetadata(0, OnPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="FirstColumn"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FirstColumnProperty =
            DependencyProperty.Register(nameof(FirstColumn), typeof(int), typeof(UniformGrid), new PropertyMetadata(0, OnPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(UniformGrid), new PropertyMetadata(Orientation.Horizontal, OnPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Rows"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register(nameof(Rows), typeof(int), typeof(UniformGrid), new PropertyMetadata(0, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, object newValue)
        {
            var self = d as UniformGrid;

            if (self != null)
            {
                self.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets or sets the number of columns in the UniformGrid.
        /// </summary>
        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the starting column offset for the first row of the UniformGrid.
        /// </summary>
        public int FirstColumn
        {
            get { return (int)GetValue(FirstColumnProperty); }
            set { SetValue(FirstColumnProperty, value); }
        }

        /// <summary>
        /// Gets or sets the orientation of the grid. When <see cref="Orientation.Vertical"/>,
        /// will transpose the layout of automatically arranged items such that they start from
        /// top to bottom then based on <see cref="FlowDirection"/>.
        /// Defaults to <see cref="Orientation.Horizontal"/>.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Gets or sets the number of rows in the UniformGrid.
        /// </summary>
        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }
    }
}