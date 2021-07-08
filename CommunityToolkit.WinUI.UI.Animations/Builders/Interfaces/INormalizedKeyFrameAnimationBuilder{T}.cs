// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml.Media.Animation;
using static CommunityToolkit.WinUI.UI.Animations.AnimationExtensions;

namespace CommunityToolkit.WinUI.UI.Animations
{
    /// <summary>
    /// An interface for an animation builder using normalized keyframes.
    /// </summary>
    /// <typeparam name="T">The type of values being set by the animation being constructed.</typeparam>
    public interface INormalizedKeyFrameAnimationBuilder<in T>
    {
        /// <summary>
        /// Adds a new normalized keyframe to the builder in use.
        /// </summary>
        /// <param name="progress">The normalized progress for the keyframe (must be in the [0, 1] range).</param>
        /// <param name="value">The value for the new keyframe to add.</param>
        /// <param name="easingType">The easing type to use to reach the new keyframe.</param>
        /// <param name="easingMode">The easing mode to use to reach the new keyframe.</param>
        /// <returns>The same <see cref="INormalizedKeyFrameAnimationBuilder{T}"/> instance that the method was invoked upon.</returns>
        INormalizedKeyFrameAnimationBuilder<T> KeyFrame(
            double progress,
            T value,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode);

        /// <summary>
        /// Adds a new normalized expression keyframe to the builder in use.
        /// This method can only be used when the animation being built targets the composition layer.
        /// </summary>
        /// <param name="progress">The normalized progress for the keyframe (must be in the [0, 1] range).</param>
        /// <param name="expression">The expression for the new keyframe to add.</param>
        /// <param name="easingType">The easing type to use to reach the new keyframe.</param>
        /// <param name="easingMode">The easing mode to use to reach the new keyframe.</param>
        /// <returns>The same <see cref="INormalizedKeyFrameAnimationBuilder{T}"/> instance that the method was invoked upon.</returns>
        /// <exception cref="global::System.InvalidOperationException">Thrown when the animation being built targets the XAML layer.</exception>
        INormalizedKeyFrameAnimationBuilder<T> ExpressionKeyFrame(
            double progress,
            string expression,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode);
    }
}