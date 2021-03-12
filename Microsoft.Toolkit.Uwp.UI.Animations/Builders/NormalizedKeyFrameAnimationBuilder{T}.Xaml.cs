// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <inheritdoc cref="NormalizedKeyFrameAnimationBuilder{T}"/>
    internal abstract partial class NormalizedKeyFrameAnimationBuilder<T>
        where T : unmanaged
    {
        /// <summary>
        /// A custom <see cref="NormalizedKeyFrameAnimationBuilder{T}"/> class targeting the XAML layer.
        /// </summary>
        public sealed class Xaml : NormalizedKeyFrameAnimationBuilder<T>, AnimationBuilder.IXamlAnimationFactory
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="NormalizedKeyFrameAnimationBuilder{T}.Xaml"/> class.
            /// </summary>
            /// <param name="property">The target property to animate.</param>
            /// <param name="delay">The target delay for the animation.</param>
            /// <param name="duration">The target duration for the animation.</param>
            /// <param name="repeat">The repeat options for the animation.</param>
            public Xaml(string property, TimeSpan? delay, TimeSpan duration, RepeatOption repeat)
                : base(property, delay, duration, repeat)
            {
            }

            /// <inheritdoc/>
            public override INormalizedKeyFrameAnimationBuilder<T> ExpressionKeyFrame(
                double progress,
                string expression,
                EasingType easingType,
                EasingMode easingMode)
            {
                throw new InvalidOperationException("Expression keyframes can only be used on the composition layer");
            }

            /// <inheritdoc/>
            public Timeline GetAnimation(DependencyObject targetHint)
            {
                return TimedKeyFrameAnimationBuilder<T>.GetAnimation(
                    targetHint,
                    this.property,
                    this.delay,
                    this.duration,
                    this.repeat,
                    this.keyFrames.GetArraySegment());
            }
        }
    }
}
