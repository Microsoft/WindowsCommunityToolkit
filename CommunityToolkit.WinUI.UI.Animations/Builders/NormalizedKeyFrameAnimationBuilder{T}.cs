// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using CommunityToolkit.WinUI.UI.Animations.Builders.Helpers;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Media.Animation;

#nullable enable

namespace CommunityToolkit.WinUI.UI.Animations
{
    /// <summary>
    /// A generic keyframe animation builder.
    /// </summary>
    /// <typeparam name="T">The type of values being set by the animation being constructed.</typeparam>
    internal abstract partial class NormalizedKeyFrameAnimationBuilder<T> : INormalizedKeyFrameAnimationBuilder<T>
        where T : unmanaged
    {
        /// <summary>
        /// The target property to animate.
        /// </summary>
        private readonly string property;

        /// <summary>
        /// The target delay for the animation, if any.
        /// </summary>
        private readonly TimeSpan? delay;

        /// <summary>
        /// The target duration for the animation.
        /// </summary>
        private readonly TimeSpan duration;

        /// <summary>
        /// The repeat options for the animation.
        /// </summary>
        private readonly RepeatOption repeat;

        /// <summary>
        /// The list builder of keyframes to use.
        /// </summary>
        private ListBuilder<KeyFrameInfo> keyFrames = ListBuilder<KeyFrameInfo>.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="NormalizedKeyFrameAnimationBuilder{T}"/> class.
        /// </summary>
        /// <param name="property">The target property to animate.</param>
        /// <param name="delay">The target delay for the animation.</param>
        /// <param name="duration">The target duration for the animation.</param>
        /// <param name="repeat">The repeat options for the animation.</param>
        protected NormalizedKeyFrameAnimationBuilder(string property, TimeSpan? delay, TimeSpan duration, RepeatOption repeat)
        {
            this.property = property;
            this.delay = delay;
            this.duration = duration;
            this.repeat = repeat;
        }

        /// <inheritdoc/>
        public INormalizedKeyFrameAnimationBuilder<T> KeyFrame(
            double progress,
            T value,
            EasingType easingType,
            EasingMode easingMode)
        {
            this.keyFrames.Append(new(progress, value, easingType, easingMode));

            return this;
        }

        /// <inheritdoc/>
        public abstract INormalizedKeyFrameAnimationBuilder<T> ExpressionKeyFrame(
            double progress,
            string expression,
            EasingType easingType,
            EasingMode easingMode);

        /// <summary>
        /// The abstracted info for a normalized animation keyframe.
        /// </summary>
        protected readonly struct KeyFrameInfo : IKeyFrameInfo
        {
            /// <summary>
            /// The normalized progress for the keyframe.
            /// </summary>
            private readonly double progress;

            /// <summary>
            /// The value for the current keyframe.
            /// </summary>
            private readonly T value;

            /// <summary>
            /// The expression for the current keyframe, if present.
            /// </summary>
            private readonly string? expression;

            /// <summary>
            /// Initializes a new instance of the <see cref="KeyFrameInfo"/> struct.
            /// </summary>
            /// <param name="progress">The normalized progress for the keyframe.</param>
            /// <param name="value">The value for the new keyframe to add.</param>
            /// <param name="easingType">The easing type to use to reach the new keyframe.</param>
            /// <param name="easingMode">The easing mode to use to reach the new keyframe.</param>
            public KeyFrameInfo(
                double progress,
                T value,
                EasingType easingType,
                EasingMode easingMode)
            {
                this.progress = progress;
                this.value = value;
                this.expression = null;

                EasingType = easingType;
                EasingMode = easingMode;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="KeyFrameInfo"/> struct.
            /// </summary>
            /// <param name="progress">The normalized progress for the keyframe.</param>
            /// <param name="expression">The expression for the new keyframe to add.</param>
            /// <param name="easingType">The easing type to use to reach the new keyframe.</param>
            /// <param name="easingMode">The easing mode to use to reach the new keyframe.</param>
            public KeyFrameInfo(
                double progress,
                string expression,
                EasingType easingType,
                EasingMode easingMode)
            {
                this.progress = progress;
                this.value = default;
                this.expression = expression;

                EasingType = easingType;
                EasingMode = easingMode;
            }

            /// <inheritdoc/>
            public EasingType EasingType { get; }

            /// <inheritdoc/>
            public EasingMode EasingMode { get; }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TValue GetValueAs<TValue>()
            {
                return (TValue)(object)this.value;
            }

            /// <inheritdoc/>
            public bool TryInsertExpressionKeyFrame(KeyFrameAnimation animation, TimeSpan duration)
            {
                if (this.expression is null)
                {
                    return false;
                }

                CompositionEasingFunction? easingFunction = animation.Compositor.TryCreateEasingFunction(EasingType, EasingMode);

                if (easingFunction is null)
                {
                    animation.InsertExpressionKeyFrame((float)this.progress, this.expression);
                }
                else
                {
                    animation.InsertExpressionKeyFrame((float)this.progress, this.expression, easingFunction);
                }

                return true;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetNormalizedProgress(TimeSpan duration)
            {
                return (float)this.progress;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TimeSpan GetTimedProgress(TimeSpan duration)
            {
                return TimeSpan.FromMilliseconds(duration.TotalMilliseconds * this.progress);
            }
        }
    }
}