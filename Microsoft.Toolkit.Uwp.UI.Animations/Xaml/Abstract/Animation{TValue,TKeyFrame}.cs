// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Animation;
using static Microsoft.Toolkit.Uwp.UI.Animations.AnimationExtensions;

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// A base model representing a typed animation that can be used in XAML.
    /// </summary>
    /// <typeparam name="TValue">
    /// The type to use for the public <see cref="To"/> and <see cref="From"/> properties.
    /// This can differ from <typeparamref name="TKeyFrame"/> to facilitate XAML parsing.
    /// </typeparam>
    /// <typeparam name="TKeyFrame">The actual type of keyframe values in use.</typeparam>
    [ContentProperty(Name = nameof(KeyFrames))]
    public abstract class Animation<TValue, TKeyFrame> : Animation
        where TKeyFrame : unmanaged
    {
        /// <summary>
        /// Gets or sets the final value for the animation.
        /// </summary>
        public TValue? To
        {
            get => (TValue?)GetValue(ToProperty);
            set => SetValue(ToProperty, value);
        }

        /// <summary>
        /// Identifies the <seealso cref="To"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ToProperty = DependencyProperty.Register(
            nameof(To),
            typeof(TValue?),
            typeof(Animation<TValue, TKeyFrame>),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the optional starting value for the animation.
        /// </summary>
        public TValue? From
        {
            get => (TValue?)GetValue(FromProperty);
            set => SetValue(FromProperty, value);
        }

        /// <summary>
        /// Identifies the <seealso cref="From"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FromProperty = DependencyProperty.Register(
            nameof(From),
            typeof(TValue?),
            typeof(Animation<TValue, TKeyFrame>),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the optional keyframe collection for the current animation.
        /// Setting this will overwrite the <see cref="To"/> and <see cref="From"/> values.
        /// </summary>
        public IList<IKeyFrame<TKeyFrame>> KeyFrames
        {
            get
            {
                if (GetValue(KeyFramesProperty) is not IList<IKeyFrame<TKeyFrame>> keyFrames)
                {
                    keyFrames = new List<IKeyFrame<TKeyFrame>>();

                    SetValue(KeyFramesProperty, keyFrames);
                }

                return keyFrames;
            }
            set => SetValue(KeyFramesProperty, value);
        }

        /// <summary>
        /// Identifies the <seealso cref="KeyFrames"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty KeyFramesProperty = DependencyProperty.Register(
            nameof(KeyFrames),
            typeof(IList<IKeyFrame<TKeyFrame>>),
            typeof(Animation<TValue, TKeyFrame>),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets the explicit target for the animation. This is the primary target property that is animated.
        /// </summary>
        protected abstract string ExplicitTarget { get; }

        /// <inheritdoc/>
        public override AnimationBuilder AppendToBuilder(AnimationBuilder builder, TimeSpan? delayHint, TimeSpan? durationHint, EasingType? easingTypeHint, EasingMode? easingModeHint)
        {
            return builder.NormalizedKeyFrames<TKeyFrame, (Animation<TValue, TKeyFrame> This, EasingType? EasingTypeHint, EasingMode? EasingModeHint)>(
                property: ExplicitTarget,
                state: (this, easingTypeHint, easingModeHint),
                delay: Delay ?? delayHint ?? DefaultDelay,
                duration: Duration ?? durationHint ?? DefaultDuration,
                repeatOption: Repeat,
                delayBehavior: DelayBehavior,
                build: static (b, s) => s.This.AppendToBuilder(b, s.EasingTypeHint, s.EasingModeHint));
        }

        /// <summary>
        /// Gets the parsed <typeparamref name="TKeyFrame"/> values from <see cref="Animation{TValue,TKeyFrame}"/>.
        /// </summary>
        /// <returns>The parsed animation values as <typeparamref name="TKeyFrame"/>.</returns>
        protected abstract (TKeyFrame? To, TKeyFrame? From) GetParsedValues();

        /// <summary>
        /// Appends the current keyframe values to a target <see cref="INormalizedKeyFrameAnimationBuilder{T}"/> instance.
        /// This method will also automatically generate keyframes for <see cref="To"/> and <see cref="From"/>.
        /// </summary>
        /// <param name="builder">The target <see cref="INormalizedKeyFrameAnimationBuilder{T}"/> instance to add the keyframe to.</param>
        /// <param name="easingTypeHint">A hint for the easing type, if present.</param>
        /// <param name="easingModeHint">A hint for the easing mode, if present.</param>
        /// <returns>The same <see cref="INormalizedKeyFrameAnimationBuilder{T}"/> instance as <paramref name="builder"/>.</returns>
        protected INormalizedKeyFrameAnimationBuilder<TKeyFrame> AppendToBuilder(INormalizedKeyFrameAnimationBuilder<TKeyFrame> builder, EasingType? easingTypeHint, EasingMode? easingModeHint)
        {
            foreach (var keyFrame in KeyFrames)
            {
                builder = keyFrame.AppendToBuilder(builder);
            }

            var (to, from) = GetParsedValues();

            if (to is not null)
            {
                builder.KeyFrame(
                    1.0,
                    to.Value,
                    EasingType ?? easingTypeHint ?? DefaultEasingType,
                    EasingMode ?? easingModeHint ?? DefaultEasingMode);
            }

            if (from is not null)
            {
                builder.KeyFrame(0.0, from.Value, default, default);
            }

            return builder;
        }
    }
}