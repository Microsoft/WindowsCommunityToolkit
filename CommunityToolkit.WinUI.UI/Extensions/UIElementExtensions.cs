// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using Point = Windows.Foundation.Point;

namespace CommunityToolkit.WinUI.UI
{
    /// <summary>
    /// Provides attached dependency properties for the <see cref="Microsoft.UI.Xaml.UIElement"/>
    /// </summary>
    public static class UIElementExtensions
    {
        /// <summary>
        /// Attached <see cref="DependencyProperty"/> that indicates whether or not the contents of the target <see cref="UIElement"/> should always be clipped to their parent's bounds.
        /// </summary>
        public static readonly DependencyProperty ClipToBoundsProperty = DependencyProperty.RegisterAttached(
            "ClipToBounds",
            typeof(bool),
            typeof(UIElementExtensions),
            new PropertyMetadata(null, OnClipToBoundsPropertyChanged));

        /// <summary>
        /// Gets the value of <see cref="ClipToBoundsProperty"/>
        /// </summary>
        /// <param name="element">The <see cref="UIElement"/> to read the property value from</param>
        /// <returns>The <see cref="bool"/> associated with the <see cref="FrameworkElement"/></returns>.
        public static bool GetClipToBounds(UIElement element) => (bool)element.GetValue(ClipToBoundsProperty);

        /// <summary>
        /// Sets the value of <see cref="ClipToBoundsProperty"/>
        /// </summary>
        /// <param name="element">The <see cref="UIElement"/> to set the property to</param>
        /// <param name="value">The new value of the attached property</param>
        public static void SetClipToBounds(UIElement element, bool value) => element.SetValue(ClipToBoundsProperty, value);

        private static void OnClipToBoundsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                var clipToBounds = (bool)e.NewValue;
                var visual = ElementCompositionPreview.GetElementVisual(element);
                visual.Clip = clipToBounds ? visual.Compositor.CreateInsetClip() : null;
            }
        }

        /// <summary>
        /// Provides the distance in a <see cref="Point"/> from the passed in element to the element being called on.
        /// For instance, calling child.CoordinatesFrom(container) will return the position of the child within the container.
        /// Helper for <see cref="UIElement.TransformToVisual(UIElement)"/>.
        /// </summary>
        /// <param name="target">Element to measure distance.</param>
        /// <param name="parent">Starting parent element to provide coordinates from.</param>
        /// <returns><see cref="Point"/> containing difference in position of elements.</returns>
        public static Point CoordinatesFrom(this UIElement target, UIElement parent)
        {
            return target.TransformToVisual(parent).TransformPoint(default(Point));
        }

        /// <summary>
        /// Provides the distance in a <see cref="Point"/> to the passed in element from the element being called on.
        /// For instance, calling container.CoordinatesTo(child) will return the position of the child within the container.
        /// Helper for <see cref="UIElement.TransformToVisual(UIElement)"/>.
        /// </summary>
        /// <param name="parent">Starting parent element to provide coordinates from.</param>
        /// <param name="target">Element to measure distance to.</param>
        /// <returns><see cref="Point"/> containing difference in position of elements.</returns>
        public static Point CoordinatesTo(this UIElement parent, UIElement target)
        {
            return target.TransformToVisual(parent).TransformPoint(default(Point));
        }
    }
}