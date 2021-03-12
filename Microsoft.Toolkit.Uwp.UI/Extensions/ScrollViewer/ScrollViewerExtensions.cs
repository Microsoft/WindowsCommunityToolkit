// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Microsoft.Toolkit.Uwp.UI
{
    /// <summary>
    /// Provides attached dependency properties and methods for the <see cref="ScrollViewer"/> control.
    /// </summary>
    public static partial class ScrollViewerExtensions
    {
        private static void OnHorizontalScrollBarMarginPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is FrameworkElement baseElement)
            {
                // If it didn't work it means that we need to wait for the component to be loaded before getting its ScrollViewer
                if (ChangeHorizontalScrollBarMarginProperty(sender as FrameworkElement))
                {
                    return;
                }

                // We need to wait for the component to be loaded before getting its ScrollViewer
                baseElement.Loaded -= ChangeHorizontalScrollBarMarginProperty;

                if (HorizontalScrollBarMarginProperty != null)
                {
                    baseElement.Loaded += ChangeHorizontalScrollBarMarginProperty;
                }
            }
        }

        private static bool ChangeHorizontalScrollBarMarginProperty(FrameworkElement sender)
        {
            if (sender == null)
            {
                return false;
            }

            var scrollViewer = sender as ScrollViewer ?? sender.FindDescendant<ScrollViewer>();

            // Last scrollbar with "HorizontalScrollBar" as name is our target to set its margin and avoid it overlapping the header
            var scrollBar = scrollViewer?.FindDescendants().OfType<ScrollBar>().LastOrDefault(bar => bar.Name == "HorizontalScrollBar");

            if (scrollBar == null)
            {
                return false;
            }

            var newMargin = GetHorizontalScrollBarMargin(sender);

            scrollBar.Margin = newMargin;

            return true;
        }

        private static void ChangeHorizontalScrollBarMarginProperty(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is FrameworkElement baseElement)
            {
                ChangeHorizontalScrollBarMarginProperty(baseElement);

                // Handling Loaded event is only required the first time the property is set, so we can stop handling it now
                baseElement.Loaded -= ChangeHorizontalScrollBarMarginProperty;
            }
        }

        private static void OnVerticalScrollBarMarginPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is FrameworkElement baseElement)
            {
                // We try to update the value, if it works we may exit
                if (ChangeVerticalScrollBarMarginProperty(sender as FrameworkElement))
                {
                    return;
                }

                // If it didn't work it means that we need to wait for the component to be loaded before getting its ScrollViewer
                baseElement.Loaded -= ChangeVerticalScrollBarMarginProperty;

                if (VerticalScrollBarMarginProperty != null)
                {
                    baseElement.Loaded += ChangeVerticalScrollBarMarginProperty;
                }
            }
        }

        private static bool ChangeVerticalScrollBarMarginProperty(FrameworkElement sender)
        {
            if (sender == null)
            {
                return false;
            }

            var scrollViewer = sender as ScrollViewer ?? sender.FindDescendant<ScrollViewer>();

            // Last scrollbar with "HorizontalScrollBar" as name is our target to set its margin and avoid it overlapping the header
            var scrollBar = scrollViewer?.FindDescendants().OfType<ScrollBar>().LastOrDefault(bar => bar.Name == "VerticalScrollBar");

            if (scrollBar == null)
            {
                return false;
            }

            var newMargin = GetVerticalScrollBarMargin(sender);

            scrollBar.Margin = newMargin;

            return true;
        }

        private static void ChangeVerticalScrollBarMarginProperty(object sender, RoutedEventArgs routedEventArgs)
        {
            ChangeVerticalScrollBarMarginProperty(sender as FrameworkElement);

            if (sender is FrameworkElement baseElement)
            {
                ChangeVerticalScrollBarMarginProperty(baseElement);

                // Handling Loaded event is only required the first time the property is set, so we can stop handling it now
                baseElement.Loaded -= ChangeVerticalScrollBarMarginProperty;
            }
        }
    }
}