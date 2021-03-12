// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;
using Windows.UI.Xaml.Media.Animation;
using static Microsoft.Toolkit.Uwp.UI.Animations.AnimationExtensions;

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// A custom animation types, that can target both the composition and the XAML layer, and that
    /// can adapt based on the context it is being used from (eg. explicit or implicit animation).
    /// </summary>
    /// <inheritdoc cref="Animation{TValue, TKeyFrame}"/>
    public abstract class CustomAnimation<TValue, TKeyFrame> : ImplicitAnimation<TValue, TKeyFrame>
        where TKeyFrame : unmanaged
    {
        /// <summary>
        /// Gets or sets the target property for the animation.
        /// </summary>
        public string? Target { get; set; }

        /// <summary>
        /// Gets or sets the target framework layer for the animation. This is only supported
        /// for a set of animation types (see the docs for more on this). Furthermore, this is
        /// ignored when the animation is being used as an implicit composition animation.
        /// The default value is <see cref="FrameworkLayer.Composition"/>.
        /// </summary>
        public FrameworkLayer Layer { get; set; }

        /// <inheritdoc/>
        protected override string ExplicitTarget => Target!;

        /// <inheritdoc/>
        public override AnimationBuilder AppendToBuilder(AnimationBuilder builder, TimeSpan? delayHint, TimeSpan? durationHint, EasingType? easingTypeHint, EasingMode? easingModeHint)
        {
            return builder.NormalizedKeyFrames<TKeyFrame, (CustomAnimation<TValue, TKeyFrame> This, EasingType? EasingTypeHint, EasingMode? EasingModeHint)>(
                property: ExplicitTarget,
                state: (this, easingTypeHint, easingModeHint),
                delay: Delay ?? delayHint ?? DefaultDelay,
                duration: Duration ?? durationHint ?? DefaultDuration,
                delayBehavior: DelayBehavior,
                layer: Layer,
                build: static (b, s) => s.This.AppendToBuilder(b, s.EasingTypeHint, s.EasingModeHint));
        }
    }
}
