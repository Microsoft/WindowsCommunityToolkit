// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.UI.Composition;
using Windows.UI.Xaml.Media.Animation;

#nullable enable

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <inheritdoc cref="TimedKeyFrameAnimationBuilder{T}"/>
    internal abstract partial class TimedKeyFrameAnimationBuilder<T>
        where T : unmanaged
    {
        /// <summary>
        /// A custom <see cref="TimedKeyFrameAnimationBuilder{T}"/> class targeting the composition layer.
        /// </summary>
        public sealed class Composition : TimedKeyFrameAnimationBuilder<T>, AnimationBuilder.ICompositionAnimationFactory
        {
            /// <summary>
            /// The target delay behavior to use.
            /// </summary>
            private readonly AnimationDelayBehavior delayBehavior;

            /// <summary>
            /// Initializes a new instance of the <see cref="TimedKeyFrameAnimationBuilder{T}.Composition"/> class.
            /// </summary>
            /// <param name="property">The target property to animate.</param>
            /// <param name="delay">The target delay for the animation.</param>
            /// <param name="repeat">The repeat options for the animation.</param>
            /// <param name="delayBehavior">The delay behavior mode to use.</param>
            public Composition(string property, TimeSpan? delay, RepeatOption repeat, AnimationDelayBehavior delayBehavior)
                : base(property, delay, repeat)
            {
                this.delayBehavior = delayBehavior;
            }

            /// <inheritdoc/>
            public override ITimedKeyFrameAnimationBuilder<T> ExpressionKeyFrame(
                TimeSpan progress,
                string expression,
                EasingType easingType,
                EasingMode easingMode)
            {
                this.keyFrames.Append(new(progress, expression, easingType, easingMode));

                return this;
            }

            /// <inheritdoc/>
            public CompositionAnimation GetAnimation(CompositionObject targetHint, out CompositionObject? target)
            {
                target = null;

                // We can retrieve the total duration from the last timed keyframe, and then set
                // this as the target duration and use it to normalize the keyframe progresses.
                ArraySegment<KeyFrameInfo> keyFrames = this.keyFrames.GetArraySegment();
                TimeSpan duration = keyFrames[keyFrames.Count - 1].GetTimedProgress(default);

                return NormalizedKeyFrameAnimationBuilder<T>.GetAnimation(
                    targetHint,
                    this.property,
                    this.delay,
                    duration,
                    this.repeat,
                    this.delayBehavior,
                    keyFrames);
            }
        }
    }
}
