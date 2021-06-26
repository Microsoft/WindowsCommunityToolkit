// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;
using System.Diagnostics.Contracts;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using XamlColorAnimation = Windows.UI.Xaml.Media.Animation.ColorAnimation;

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// An extension <see langword="class"/> for the <see cref="DependencyObject"/> type.
    /// </summary>
    public static class DependencyObjectExtensions
    {
        /// <summary>
        /// Prepares a <see cref="DoubleAnimation"/> with the given info.
        /// </summary>
        /// <param name="target">The target <see cref="DependencyObject"/> to animate.</param>
        /// <param name="property">The property to animate inside the target <see cref="DependencyObject"/>.</param>
        /// <param name="to">The final property value.</param>
        /// <param name="from">The optional initial property value.</param>
        /// <param name="delay">The optional delay for the animation.</param>
        /// <param name="duration">The duration of the <see cref="DoubleAnimation"/>.</param>
        /// <param name="easing">The easing function to use inside the <see cref="DoubleAnimation"/>.</param>
        /// <param name="repeatBehavior">The repeat behavior for the animation (defaults to one iteration).</param>
        /// <param name="fillBehavior">The behavior to use when the animation reaches the end of its schedule.</param>
        /// <param name="autoReverse">Indicates whether the animation plays in reverse after each forward iteration.</param>
        /// <param name="enableDependecyAnimations">Indicates whether or not to apply this animation to elements that need the visual tree to be rearranged.</param>
        /// <returns>A <see cref="DoubleAnimation"/> instance with the specified parameters.</returns>
        [Pure]
        public static DoubleAnimation CreateDoubleAnimation(
            this DependencyObject target,
            string property,
            double to,
            double? from,
            TimeSpan? delay,
            TimeSpan duration,
            EasingFunctionBase? easing = null,
            RepeatBehavior? repeatBehavior = null,
            FillBehavior fillBehavior = FillBehavior.HoldEnd,
            bool autoReverse = false,
            bool enableDependecyAnimations = false)
        {
            DoubleAnimation animation = new()
            {
                To = to,
                From = from,
                BeginTime = delay,
                Duration = duration,
                EasingFunction = easing,
                RepeatBehavior = repeatBehavior ?? new RepeatBehavior(1),
                FillBehavior = fillBehavior,
                AutoReverse = autoReverse,
                EnableDependentAnimation = enableDependecyAnimations
            };

            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, property);

            return animation;
        }

        /// <summary>
        /// Prepares a <see cref="PointAnimation"/> with the given info.
        /// </summary>
        /// <param name="target">The target <see cref="DependencyObject"/> to animate.</param>
        /// <param name="property">The property to animate inside the target <see cref="DependencyObject"/>.</param>
        /// <param name="to">The final property value.</param>
        /// <param name="from">The optional initial property value.</param>
        /// <param name="delay">The optional delay for the animation.</param>
        /// <param name="duration">The duration of the <see cref="PointAnimation"/>.</param>
        /// <param name="easing">The easing function to use inside the <see cref="PointAnimation"/>.</param>
        /// <param name="repeatBehavior">The repeat behavior for the animation (defaults to one iteration).</param>
        /// <param name="fillBehavior">The behavior to use when the animation reaches the end of its schedule.</param>
        /// <param name="autoReverse">Indicates whether the animation plays in reverse after each forward iteration.</param>
        /// <param name="enableDependecyAnimations">Indicates whether or not to apply this animation to elements that need the visual tree to be rearranged.</param>
        /// <returns>A <see cref="PointAnimation"/> instance with the specified parameters.</returns>
        [Pure]
        public static PointAnimation CreatePointAnimation(
            this DependencyObject target,
            string property,
            Point to,
            Point? from,
            TimeSpan? delay,
            TimeSpan duration,
            EasingFunctionBase? easing = null,
            RepeatBehavior? repeatBehavior = null,
            FillBehavior fillBehavior = FillBehavior.HoldEnd,
            bool autoReverse = false,
            bool enableDependecyAnimations = false)
        {
            PointAnimation animation = new()
            {
                To = to,
                From = from,
                BeginTime = delay,
                Duration = duration,
                EasingFunction = easing,
                RepeatBehavior = repeatBehavior ?? new RepeatBehavior(1),
                FillBehavior = fillBehavior,
                AutoReverse = autoReverse,
                EnableDependentAnimation = enableDependecyAnimations
            };

            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, property);

            return animation;
        }

        /// <summary>
        /// Prepares a <see cref="XamlColorAnimation"/> with the given info.
        /// </summary>
        /// <param name="target">The target <see cref="DependencyObject"/> to animate.</param>
        /// <param name="property">The property to animate inside the target <see cref="DependencyObject"/>.</param>
        /// <param name="to">The final property value.</param>
        /// <param name="from">The optional initial property value.</param>
        /// <param name="delay">The optional delay for the animation.</param>
        /// <param name="duration">The duration of the <see cref="XamlColorAnimation"/>.</param>
        /// <param name="easing">The easing function to use inside the <see cref="XamlColorAnimation"/>.</param>
        /// <param name="repeatBehavior">The repeat behavior for the animation (defaults to one iteration).</param>
        /// <param name="fillBehavior">The behavior to use when the animation reaches the end of its schedule.</param>
        /// <param name="autoReverse">Indicates whether the animation plays in reverse after each forward iteration.</param>
        /// <returns>A <see cref="XamlColorAnimation"/> instance with the specified parameters.</returns>
        [Pure]
        public static XamlColorAnimation CreateColorAnimation(
            this DependencyObject target,
            string property,
            Color to,
            Color? from,
            TimeSpan? delay,
            TimeSpan duration,
            EasingFunctionBase? easing = null,
            RepeatBehavior? repeatBehavior = null,
            FillBehavior fillBehavior = FillBehavior.HoldEnd,
            bool autoReverse = false)
        {
            XamlColorAnimation animation = new()
            {
                To = to,
                From = from,
                BeginTime = delay,
                Duration = duration,
                EasingFunction = easing,
                RepeatBehavior = repeatBehavior ?? new RepeatBehavior(1),
                FillBehavior = fillBehavior,
                AutoReverse = autoReverse
            };

            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, property);

            return animation;
        }
    }
}