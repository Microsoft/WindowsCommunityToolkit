// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.UI.Xaml.Media.Animation;

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// An interface representing a XAML model for a custom animation.
    /// </summary>
    public interface ITimeline
    {
        /// <summary>
        /// Appens the current animation to a target <see cref="AnimationBuilder"/> instance.
        /// This method is used when the current <see cref="ITimeline"/> instance is explicitly triggered.
        /// </summary>
        /// <param name="builder">The target <see cref="AnimationBuilder"/> instance to schedule the animation on.</param>
        /// <param name="delayHint">A hint for the animation delay, if present.</param>
        /// <param name="durationHint">A hint for the animation duration, if present.</param>
        /// <param name="easingTypeHint">A hint for the easing type, if present.</param>
        /// <param name="easingModeHint">A hint for the easing mode, if present.</param>
        /// <returns>The same <see cref="AnimationBuilder"/> instance as <paramref name="builder"/>.</returns>
        AnimationBuilder AppendToBuilder(
            AnimationBuilder builder,
            TimeSpan? delayHint = null,
            TimeSpan? durationHint = null,
            EasingType? easingTypeHint = null,
            EasingMode? easingModeHint = null);
    }
}