// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.UI.Composition;
using Windows.UI.Xaml;

#nullable enable

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// An interface representing a XAML model for a custom implicit composition animation.
    /// </summary>
    public interface IImplicitTimeline
    {
        /// <summary>
        /// Raised whenever a property that influences the animation changes.
        /// This event is used by <see cref="ImplicitAnimationSet"/> to update the animations collection
        /// assigned to a target <see cref="UIElement"/> when any of the individual animations is modified.
        /// </summary>
        event EventHandler? AnimationPropertyChanged;

        /// <summary>
        /// Gets a <see cref="CompositionAnimation"/> from the current node. This animation might
        /// be used either as an implicit show/hide animation, or as a direct implicit animation.
        /// </summary>
        /// <param name="element">The target <see cref="UIElement"/> the animation will be applied to.</param>
        /// <param name="target">
        /// The optional target property for the animation. This might be used for direct implicit
        /// animations that target a property but want to be triggered according to a separate property.
        /// </param>
        /// <returns>A new <see cref="CompositionAnimation"/> instance.</returns>
        CompositionAnimation GetAnimation(UIElement element, out string? target);
    }
}