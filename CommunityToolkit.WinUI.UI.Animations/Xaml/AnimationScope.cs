// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;

namespace CommunityToolkit.WinUI.UI.Animations
{
    /// <summary>
    /// A container of <see cref="ITimeline"/> elements that can be used to conceptually group animations
    /// together and to assign shared properties to be applied to all the contained items automatically.
    /// </summary>
    public sealed class AnimationScope : DependencyObjectCollection, ITimeline
    {
        /// <summary>
        /// Gets or sets the optional initial delay for the animation.
        /// </summary>
        public TimeSpan? Delay
        {
            get => (TimeSpan?)GetValue(DelayProperty);
            set => SetValue(DelayProperty, value);
        }

        /// <summary>
        /// Identifies the <seealso cref="Delay"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DelayProperty = DependencyProperty.Register(
            nameof(Delay),
            typeof(TimeSpan?),
            typeof(Animation),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the animation duration.
        /// </summary>
        public TimeSpan? Duration
        {
            get => (TimeSpan?)GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }

        /// <summary>
        /// Identifies the <seealso cref="Duration"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(
            nameof(Duration),
            typeof(TimeSpan?),
            typeof(Animation),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the optional easing function type for the animation.
        /// </summary>
        public EasingType? EasingType
        {
            get => (EasingType?)GetValue(EasingTypeProperty);
            set => SetValue(EasingTypeProperty, value);
        }

        /// <summary>
        /// Identifies the <seealso cref="EasingType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EasingTypeProperty = DependencyProperty.Register(
            nameof(EasingType),
            typeof(EasingType?),
            typeof(Animation),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the optional easing function mode for the animation.
        /// </summary>
        public EasingMode? EasingMode
        {
            get => (EasingMode?)GetValue(EasingModeProperty);
            set => SetValue(EasingModeProperty, value);
        }

        /// <summary>
        /// Identifies the <seealso cref="EasingMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EasingModeProperty = DependencyProperty.Register(
            nameof(EasingMode),
            typeof(EasingMode?),
            typeof(Animation),
            new PropertyMetadata(null));

        /// <inheritdoc/>
        public AnimationBuilder AppendToBuilder(AnimationBuilder builder, TimeSpan? delayHint, TimeSpan? durationHint, EasingType? easingTypeHint, EasingMode? easingModeHint)
        {
            foreach (ITimeline element in this)
            {
                builder = element.AppendToBuilder(builder, Delay ?? delayHint, Duration ?? durationHint, EasingType ?? easingTypeHint, EasingMode ?? easingModeHint);
            }

            return builder;
        }
    }
}