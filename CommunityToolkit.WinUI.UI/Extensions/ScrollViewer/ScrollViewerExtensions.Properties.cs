// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml;

namespace CommunityToolkit.WinUI.UI
{
    /// <summary>
    /// Provides attached dependency properties and methods for the <see cref="Microsoft.UI.Xaml.Controls.ScrollViewer"/> control.
    /// </summary>
    public partial class ScrollViewerExtensions
    {
#pragma warning disable CS0419 // Ambiguous reference in cref attribute
        /// <summary>
        /// Attached <see cref="DependencyProperty"/> for binding a <see cref="Microsoft.UI.Xaml.Thickness"/> for the horizontal <see cref="Microsoft.UI.Xaml.Controls.Primitives.ScrollBar"/> of a <see cref="Microsoft.UI.Xaml.Controls.ScrollViewer"/>
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarMarginProperty = DependencyProperty.RegisterAttached("HorizontalScrollBarMargin", typeof(Thickness), typeof(ScrollViewerExtensions), new PropertyMetadata(null, OnHorizontalScrollBarMarginPropertyChanged));

        /// <summary>
        /// Attached <see cref="DependencyProperty"/> for binding a <see cref="Microsoft.UI.Xaml.Thickness"/> for the vertical <see cref="Microsoft.UI.Xaml.Controls.Primitives.ScrollBar"/> of a <see cref="Microsoft.UI.Xaml.Controls.ScrollViewer"/>
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarMarginProperty = DependencyProperty.RegisterAttached("VerticalScrollBarMargin", typeof(Thickness), typeof(ScrollViewerExtensions), new PropertyMetadata(null, OnVerticalScrollBarMarginPropertyChanged));

        /// <summary>
        /// Attached <see cref="DependencyProperty"/> for enabling middle click scrolling
        /// </summary>
        public static readonly DependencyProperty EnableMiddleClickScrollingProperty =
            DependencyProperty.RegisterAttached("EnableMiddleClickScrolling", typeof(bool), typeof(ScrollViewerExtensions), new PropertyMetadata(false, OnEnableMiddleClickScrollingChanged));

        /// <summary>
        /// Gets the <see cref="Microsoft.UI.Xaml.Thickness"/> associated with the specified vertical <see cref="Microsoft.UI.Xaml.Controls.Primitives.ScrollBar"/> of a <see cref="Microsoft.UI.Xaml.Controls.ScrollViewer"/>
        /// </summary>
        /// <param name="obj">The <see cref="FrameworkElement"/> to get the associated <see cref="Microsoft.UI.Xaml.Thickness"/> from</param>
        /// <returns>The <see cref="Microsoft.UI.Xaml.Thickness"/> associated with the <see cref="FrameworkElement"/></returns>
        public static Thickness GetVerticalScrollBarMargin(FrameworkElement obj)
        {
            return (Thickness)obj.GetValue(VerticalScrollBarMarginProperty);
        }

        /// <summary>
        /// Sets the <see cref="Microsoft.UI.Xaml.Thickness"/> associated with the specified vertical <see cref="Microsoft.UI.Xaml.Controls.Primitives.ScrollBar"/> of a <see cref="Microsoft.UI.Xaml.Controls.ScrollViewer"/>
        /// </summary>
        /// <param name="obj">The <see cref="FrameworkElement"/> to associate the <see cref="Microsoft.UI.Xaml.Thickness"/> with</param>
        /// <param name="value">The <see cref="Microsoft.UI.Xaml.Thickness"/> for binding to the <see cref="FrameworkElement"/></param>
        public static void SetVerticalScrollBarMargin(FrameworkElement obj, Thickness value)
        {
            obj.SetValue(VerticalScrollBarMarginProperty, value);
        }

        /// <summary>
        /// Gets the <see cref="Microsoft.UI.Xaml.Thickness"/> associated with the specified horizontal <see cref="Microsoft.UI.Xaml.Controls.Primitives.ScrollBar"/> of a <see cref="Microsoft.UI.Xaml.Controls.ScrollViewer"/>
        /// </summary>
        /// <param name="obj">The <see cref="FrameworkElement"/> to get the associated <see cref="Microsoft.UI.Xaml.Thickness"/> from</param>
        /// <returns>The <see cref="Microsoft.UI.Xaml.Thickness"/> associated with the <see cref="FrameworkElement"/></returns>
        public static Thickness GetHorizontalScrollBarMargin(FrameworkElement obj)
        {
            return (Thickness)obj.GetValue(HorizontalScrollBarMarginProperty);
        }

        /// <summary>
        /// Sets the <see cref="Microsoft.UI.Xaml.Thickness"/> associated with the specified horizontal <see cref="Microsoft.UI.Xaml.Controls.Primitives.ScrollBar"/> of a <see cref="Microsoft.UI.Xaml.Controls.ScrollViewer"/>
        /// </summary>
        /// <param name="obj">The <see cref="FrameworkElement"/> to associate the <see cref="Microsoft.UI.Xaml.Thickness"/> with</param>
        /// <param name="value">The <see cref="Microsoft.UI.Xaml.Thickness"/> for binding to the <see cref="FrameworkElement"/></param>
        public static void SetHorizontalScrollBarMargin(FrameworkElement obj, Thickness value)
        {
            obj.SetValue(HorizontalScrollBarMarginProperty, value);
        }

        /// <summary>
        /// Get <see cref="EnableMiddleClickScrollingProperty"/>. Returns `true` if middle click scrolling is enabled else return `false`
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> to get the associated `bool`</param>
        /// <returns>The `bool` associated with the <see cref="DependencyObject"/></returns>
        public static bool GetEnableMiddleClickScrolling(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableMiddleClickScrollingProperty);
        }

        /// <summary>
        /// Set <see cref="EnableMiddleClickScrollingProperty"/>. `true` to enable middle click scrolling
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> to associate the `bool` with</param>
        /// <param name="value">The `bool` for binding to the <see cref="DependencyObject"/></param>
        public static void SetEnableMiddleClickScrolling(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableMiddleClickScrollingProperty, value);
        }
#pragma warning restore CS0419 // Ambiguous reference in cref attribute
    }
}