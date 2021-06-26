// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using Windows.UI.Composition;
using Windows.UI.Xaml.Media.Animation;

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// An interface representing a generic model containing info for an abstract keyframe.
    /// </summary>
    internal interface IKeyFrameInfo
    {
        /// <summary>
        /// Gets the easing type to use to reach the new keyframe.
        /// </summary>
        EasingType EasingType { get; }

        /// <summary>
        /// Gets the easing mode to use to reach the new keyframe.
        /// </summary>
        EasingMode EasingMode { get; }

        /// <summary>
        /// Gets the value for the new keyframe to add.
        /// </summary>
        /// <typeparam name="T">The type of values being set by the animation being constructed.</typeparam>
        /// <returns>The value for the current keyframe.</returns>
        [Pure]
        T GetValueAs<T>();

        /// <summary>
        /// Tries to insert an expression keyframe into the target animation, if possible.
        /// </summary>
        /// <param name="animation">The target <see cref="KeyFrameAnimation"/> instance.</param>
        /// <param name="duration">The total duration for the full animation.</param>
        /// <returns>Whether or not the curreent <see cref="IKeyFrameInfo"/> instance contained an expression.</returns>
        bool TryInsertExpressionKeyFrame(KeyFrameAnimation animation, TimeSpan duration);

        /// <summary>
        /// Gets the normalized progress for the current keyframe.
        /// </summary>
        /// <param name="duration">The total duration for the full animation.</param>
        /// <returns>The normalized progress for the current keyframe.</returns>
        [Pure]
        float GetNormalizedProgress(TimeSpan duration);

        /// <summary>
        /// Gets the timed progress for the current keyframe.
        /// </summary>
        /// <param name="duration">The total duration for the full animation.</param>
        /// <returns>The timed progress for the current keyframe.</returns>
        [Pure]
        TimeSpan GetTimedProgress(TimeSpan duration);
    }
}