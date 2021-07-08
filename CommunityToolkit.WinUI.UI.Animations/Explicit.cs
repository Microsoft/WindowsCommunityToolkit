// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml;

namespace CommunityToolkit.WinUI.UI.Animations
{
    /// <summary>
    /// Attached properties to support explicitly triggered animations for <see cref="UIElement"/> instances.
    /// </summary>
    public static class Explicit
    {
        /// <summary>
        /// The attached "Animations" property.
        /// </summary>
        public static readonly DependencyProperty AnimationsProperty = DependencyProperty.RegisterAttached(
            "Animations",
            typeof(AnimationDictionary),
            typeof(Explicit),
            new PropertyMetadata(null, OnAnimationsPropertyChanged));

        /// <summary>
        /// Gets the value of the <see cref="AnimationsProperty"/> property.
        /// </summary>
        /// <param name="element">The <see cref="UIElement"/> to get the value for.</param>
        /// <returns>The retrieved <see cref="AnimationDictionary"/> item.</returns>
        public static AnimationDictionary GetAnimations(UIElement element)
        {
            if (element.GetValue(AnimationsProperty) is AnimationDictionary collection)
            {
                return collection;
            }

            collection = new AnimationDictionary();

            element.SetValue(AnimationsProperty, collection);

            return collection;
        }

        /// <summary>
        /// Sets the value of the <see cref="AnimationsProperty"/> property.
        /// </summary>
        /// <param name="element">The <see cref="UIElement"/> to set the value for.</param>
        /// <param name="value">The <see cref="AnimationDictionary"/> value to set.</param>
        public static void SetAnimations(UIElement element, AnimationDictionary value)
        {
            element.SetValue(AnimationsProperty, value);
        }

        /// <summary>
        /// Callback to keep the attached parent in sync for animations linked to the <see cref="Animations"/> property.
        /// </summary>
        /// <param name="d">The target object the property was changed for.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance for the current event.</param>
        private static void OnAnimationsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not UIElement element)
            {
                return;
            }

            if (e.OldValue is AnimationDictionary oldDictionary)
            {
                oldDictionary.Parent = null;
            }

            if (e.NewValue is AnimationDictionary newDictionary)
            {
                newDictionary.Parent = element;
            }
        }
    }
}