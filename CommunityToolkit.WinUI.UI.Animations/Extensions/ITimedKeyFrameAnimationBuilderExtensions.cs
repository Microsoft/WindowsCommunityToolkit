// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;
using Microsoft.UI.Xaml.Media.Animation;
using static CommunityToolkit.WinUI.UI.Animations.AnimationExtensions;

namespace CommunityToolkit.WinUI.UI.Animations
{
    /// <summary>
    /// An extension <see langword="class"/> for the <see cref="ITimedKeyFrameAnimationBuilder{T}"/> type.
    /// </summary>
    public static class ITimedKeyFrameAnimationBuilderExtensions
    {
        /// <summary>
        /// Adds a new timed keyframe to the builder in use.
        /// </summary>
        /// <typeparam name="T">The type of values being set by the animation being constructed.</typeparam>
        /// <param name="builder">The target <see cref="ITimedKeyFrameAnimationBuilder{T}"/> instance in use.</param>
        /// <param name="progress">The timed progress for the keyframe (in milliseconds), relative to the start of the animation.</param>
        /// <param name="value">The value for the new keyframe to add.</param>
        /// <param name="easingType">The easing type to use to reach the new keyframe.</param>
        /// <param name="easingMode">The easing mode to use to reach the new keyframe.</param>
        /// <returns>The same <see cref="INormalizedKeyFrameAnimationBuilder{T}"/> instance that the method was invoked upon.</returns>
        public static ITimedKeyFrameAnimationBuilder<T> KeyFrame<T>(
            this ITimedKeyFrameAnimationBuilder<T> builder,
            int progress,
            T value,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode)
            where T : unmanaged
        {
            return builder.KeyFrame(TimeSpan.FromMilliseconds(progress), value, easingType, easingMode);
        }
    }
}