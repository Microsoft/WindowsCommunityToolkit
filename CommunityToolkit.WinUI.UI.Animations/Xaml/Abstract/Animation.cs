// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;

namespace CommunityToolkit.WinUI.UI.Animations
{
    /// <summary>
    /// A base model representing an animation that can be used in XAML.
    /// </summary>
    public abstract class Animation : DependencyObject, ITimeline
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

        /// <summary>
        /// Gets or sets the repeat option for the animation.
        /// </summary>
        public RepeatOption Repeat
        {
            get => (RepeatOption)GetValue(RepeatProperty);
            set => SetValue(RepeatProperty, value);
        }

        /// <summary>
        /// Identifies the <seealso cref="Repeat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RepeatProperty = DependencyProperty.Register(
            nameof(Repeat),
            typeof(RepeatOption),
            typeof(Animation),
            new PropertyMetadata(RepeatOption.Once));

        /// <summary>
        /// Gets or sets the delay behavior for the animation. The default value is set to <see cref="AnimationDelayBehavior.SetInitialValueBeforeDelay"/>.
        /// This value is applicable when the current animation is used as either an implicit composition animation, or an explicit composition animation.
        /// If the current animation is instead running on the XAML layer (if used through <see cref="CustomAnimation{TValue, TKeyFrame}"/>), it will be ignored.
        /// </summary>
        public AnimationDelayBehavior DelayBehavior
        {
            get => (AnimationDelayBehavior)GetValue(DelayBehaviorProperty);
            set => SetValue(DelayBehaviorProperty, value);
        }

        /// <summary>
        /// Identifies the <seealso cref="DelayBehavior"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DelayBehaviorProperty = DependencyProperty.Register(
            nameof(DelayBehavior),
            typeof(AnimationDelayBehavior),
            typeof(Animation),
            new PropertyMetadata(AnimationDelayBehavior.SetInitialValueBeforeDelay));

        /// <inheritdoc/>
        public abstract AnimationBuilder AppendToBuilder(AnimationBuilder builder, TimeSpan? delayHint, TimeSpan? durationHint, EasingType? easingTypeHint, EasingMode? easingModeHint);
    }
}