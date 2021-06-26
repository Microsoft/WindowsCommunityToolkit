// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.UI.Xaml.Media.Animation;
using static Microsoft.Toolkit.Uwp.UI.Animations.AnimationExtensions;

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// An interface for an animation builder using timed keyframes.
    /// </summary>
    /// <typeparam name="T">The type of values being set by the animation being constructed.</typeparam>
    public interface ITimedKeyFrameAnimationBuilder<in T>
    {
        /// <summary>
        /// Adds a new timed keyframe to the builder in use.
        /// </summary>
        /// <param name="progress">The timed progress for the keyframe, relative to the start of the animation.</param>
        /// <param name="value">The value for the new keyframe to add.</param>
        /// <param name="easingType">The easing type to use to reach the new keyframe.</param>
        /// <param name="easingMode">The easing mode to use to reach the new keyframe.</param>
        /// <returns>The same <see cref="INormalizedKeyFrameAnimationBuilder{T}"/> instance that the method was invoked upon.</returns>
        ITimedKeyFrameAnimationBuilder<T> KeyFrame(
            TimeSpan progress,
            T value,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode);

        /// <summary>
        /// Adds a new timed expressionkeyframe to the builder in use.
        /// This method can only be used when the animation being built targets the composition layer.
        /// </summary>
        /// <param name="progress">The timed progress for the keyframe, relative to the start of the animation.</param>
        /// <param name="expression">The expression for the new keyframe to add.</param>
        /// <param name="easingType">The easing type to use to reach the new keyframe.</param>
        /// <param name="easingMode">The easing mode to use to reach the new keyframe.</param>
        /// <returns>The same <see cref="INormalizedKeyFrameAnimationBuilder{T}"/> instance that the method was invoked upon.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the animation being built targets the XAML layer.</exception>
        ITimedKeyFrameAnimationBuilder<T> ExpressionKeyFrame(
            TimeSpan progress,
            string expression,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode);
    }
}