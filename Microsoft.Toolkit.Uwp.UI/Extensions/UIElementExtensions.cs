// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace Microsoft.Toolkit.Uwp.UI
{
    /// <summary>
    /// Provides attached dependency properties for the <see cref="Windows.UI.Xaml.UIElement"/>
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
            new PropertyMetadata(DependencyProperty.UnsetValue, OnClipToBoundsPropertyChanged));

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
    }
}
