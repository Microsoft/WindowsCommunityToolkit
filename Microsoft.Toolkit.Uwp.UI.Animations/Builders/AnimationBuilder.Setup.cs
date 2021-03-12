// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Media.Animation;
using static Microsoft.Toolkit.Uwp.UI.Animations.AnimationExtensions;

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// A <see langword="class"/> that allows to build custom animations targeting both the XAML and composition layers.
    /// </summary>
    public sealed partial class AnimationBuilder
    {
        /// <summary>
        /// The list of <see cref="ICompositionAnimationFactory"/> instances representing factories for composition animations to run.
        /// </summary>
        private readonly List<ICompositionAnimationFactory> compositionAnimationFactories = new();

        /// <summary>
        /// The list of <see cref="IXamlAnimationFactory"/> instances representing factories for XAML animations to run.
        /// </summary>
        private readonly List<IXamlAnimationFactory> xamlAnimationFactories = new();

        /// <summary>
        /// Adds a new composition <typeparamref name="T"/> animation to the current schedule.
        /// </summary>
        /// <typeparam name="T">The type of values to animate.</typeparam>
        /// <param name="property">The target property to animate.</param>
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat option for the animation.</param>
        /// <param name="easingType">The easing function for the animation.</param>
        /// <param name="easingMode">The easing mode for the animation.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        private AnimationBuilder AddCompositionAnimationFactory<T>(
            string property,
            T to,
            T? from,
            TimeSpan? delay,
            TimeSpan? duration,
            RepeatOption? repeat,
            EasingType easingType,
            EasingMode easingMode)
            where T : unmanaged
        {
            AnimationFactory<T> animation = new(
                property,
                to,
                from,
                delay ?? DefaultDelay,
                duration ?? DefaultDuration,
                repeat ?? RepeatOption.Once,
                easingType,
                easingMode);

            this.compositionAnimationFactories.Add(animation);

            return this;
        }

        /// <summary>
        /// Adds a new XAML <typeparamref name="T"/> animation to the current schedule.
        /// </summary>
        /// <typeparam name="T">The type of values to animate.</typeparam>
        /// <param name="property">The target property to animate.</param>
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode for the animation.</param>
        /// <param name="easingType">The easing function for the animation.</param>
        /// <param name="easingMode">The easing mode for the animation.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        private AnimationBuilder AddXamlAnimationFactory<T>(
            string property,
            T to,
            T? from,
            TimeSpan? delay,
            TimeSpan? duration,
            RepeatOption? repeat,
            EasingType easingType,
            EasingMode easingMode)
            where T : unmanaged
        {
            AnimationFactory<T> animation = new(
                property,
                to,
                from,
                delay ?? DefaultDelay,
                duration ?? DefaultDuration,
                repeat ?? RepeatOption.Once,
                easingType,
                easingMode);

            this.xamlAnimationFactories.Add(animation);

            return this;
        }

        /// <summary>
        /// Adds a new XAML transform <see cref="double"/> animation to the current schedule.
        /// </summary>
        /// <param name="property">The target property to animate.</param>
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode for the animation.</param>
        /// <param name="easingType">The easing function for the animation.</param>
        /// <param name="easingMode">The easing mode for the animation.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        private AnimationBuilder AddXamlTransformDoubleAnimationFactory(
            string property,
            double to,
            double? from,
            TimeSpan? delay,
            TimeSpan? duration,
            RepeatOption? repeat,
            EasingType easingType,
            EasingMode easingMode)
        {
            XamlTransformDoubleAnimationFactory animation = new(
                property,
                to,
                from,
                delay ?? DefaultDelay,
                duration ?? DefaultDuration,
                repeat ?? RepeatOption.Once,
                easingType,
                easingMode);

            this.xamlAnimationFactories.Add(animation);

            return this;
        }
    }
}
