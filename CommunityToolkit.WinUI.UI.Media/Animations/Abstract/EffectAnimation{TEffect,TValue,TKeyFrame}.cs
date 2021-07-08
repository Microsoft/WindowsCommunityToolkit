// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.WinUI.UI.Media;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;
using static CommunityToolkit.WinUI.UI.Animations.AnimationExtensions;

#nullable enable

namespace CommunityToolkit.WinUI.UI.Animations
{
    /// <summary>
    /// A custom animation targeting a property on an <see cref="IPipelineEffect"/> instance.
    /// </summary>
    /// <typeparam name="TEffect">The type of effect to animate.</typeparam>
    /// <typeparam name="TValue">
    /// The type to use for the public <see cref="Animation{TValue,TKeyFrame}.To"/> and <see cref="Animation{TValue,TKeyFrame}.From"/>
    /// properties. This can differ from <typeparamref name="TKeyFrame"/> to facilitate XAML parsing.
    /// </typeparam>
    /// <typeparam name="TKeyFrame">The actual type of keyframe values in use.</typeparam>
    public abstract class EffectAnimation<TEffect, TValue, TKeyFrame> : Animation<TValue, TKeyFrame>
        where TEffect : class, IPipelineEffect
        where TKeyFrame : unmanaged
    {
        /// <summary>
        /// Gets or sets the linked <typeparamref name="TEffect"/> instance to animate.
        /// </summary>
        public TEffect? Target
        {
            get => (TEffect?)GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }

        /// <summary>
        /// Identifies the <seealso cref="Target"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
            nameof(Target),
            typeof(TEffect),
            typeof(EffectAnimation<TEffect, TValue, TKeyFrame>),
            new PropertyMetadata(null));

        /// <inheritdoc/>
        public override AnimationBuilder AppendToBuilder(AnimationBuilder builder, TimeSpan? delayHint, TimeSpan? durationHint, EasingType? easingTypeHint, EasingMode? easingModeHint)
        {
            if (Target is not TEffect target)
            {
                static AnimationBuilder ThrowArgumentNullException() => throw new ArgumentNullException("The target effect is null, make sure to set the Target property");

                return ThrowArgumentNullException();
            }

            if (ExplicitTarget is not string explicitTarget)
            {
                static AnimationBuilder ThrowArgumentNullException()
                {
                    throw new ArgumentNullException(
                        "The target effect cannot be animated at this time. If you're targeting one of the " +
                        "built-in effects, make sure that the PipelineEffect.IsAnimatable property is set to true.");
                }

                return ThrowArgumentNullException();
            }

            NormalizedKeyFrameAnimationBuilder<TKeyFrame>.Composition keyFrameBuilder = new(
                explicitTarget,
                Delay ?? delayHint ?? DefaultDelay,
                Duration ?? durationHint ?? DefaultDuration,
                Repeat,
                DelayBehavior);

            AppendToBuilder(keyFrameBuilder, easingTypeHint, easingModeHint);

            CompositionAnimation animation = keyFrameBuilder.GetAnimation(target.Brush!, out _);

            return builder.ExternalAnimation(target.Brush!, animation);
        }
    }
}