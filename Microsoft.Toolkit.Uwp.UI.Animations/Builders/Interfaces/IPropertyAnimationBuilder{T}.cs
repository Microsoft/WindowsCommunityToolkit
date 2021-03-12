// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.UI.Composition;

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// An animation for an animation builder using keyframes, targeting a specific property.
    /// </summary>
    /// <typeparam name="T">The type of values being set by the animation being constructed.</typeparam>
    public interface IPropertyAnimationBuilder<in T>
    {
        /// <summary>
        /// Adds a custom animation based on normalized keyframes ot the current schedule.
        /// </summary>
        /// <param name="build">The callback to use to construct the custom animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The animation duration.</param>
        /// <param name="repeat">The repeat option for the animation (defaults to one iteration).</param>
        /// <param name="delayBehavior">The delay behavior to use (ignored if the animation is not being executed on the composition layer).</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        AnimationBuilder NormalizedKeyFrames(
            Action<INormalizedKeyFrameAnimationBuilder<T>> build,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            AnimationDelayBehavior? delayBehavior = null);

        /// <summary>
        /// Adds a custom animation based on normalized keyframes ot the current schedule.
        /// </summary>
        /// <typeparam name="TState">The type of state to pass to the builder.</typeparam>
        /// <param name="state">The state to pass to the builder.</param>
        /// <param name="build">The callback to use to construct the custom animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The animation duration.</param>
        /// <param name="repeat">The repeat option for the animation (defaults to one iteration).</param>
        /// <param name="delayBehavior">The delay behavior to use (ignored if the animation is not being executed on the composition layer).</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        AnimationBuilder NormalizedKeyFrames<TState>(
            TState state,
            Action<INormalizedKeyFrameAnimationBuilder<T>, TState> build,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            AnimationDelayBehavior? delayBehavior = null);

        /// <summary>
        /// Adds a custom animation based on timed keyframes to the current schedule.
        /// </summary>
        /// <param name="build">The callback to use to construct the custom animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="repeat">The repeat option for the animation (defaults to one iteration).</param>
        /// <param name="delayBehavior">The delay behavior to use (ignored if the animation is not being executed on the composition layer).</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        AnimationBuilder TimedKeyFrames(
            Action<ITimedKeyFrameAnimationBuilder<T>> build,
            TimeSpan? delay = null,
            RepeatOption? repeat = null,
            AnimationDelayBehavior? delayBehavior = null);

        /// <summary>
        /// Adds a custom animation based on timed keyframes to the current schedule.
        /// </summary>
        /// <typeparam name="TState">The type of state to pass to the builder.</typeparam>
        /// <param name="state">The state to pass to the builder.</param>
        /// <param name="build">The callback to use to construct the custom animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="repeat">The repeat option for the animation (defaults to one iteration).</param>
        /// <param name="delayBehavior">The delay behavior to use (ignored if the animation is not being executed on the composition layer).</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        AnimationBuilder TimedKeyFrames<TState>(
            TState state,
            Action<ITimedKeyFrameAnimationBuilder<T>, TState> build,
            TimeSpan? delay = null,
            RepeatOption? repeat = null,
            AnimationDelayBehavior? delayBehavior = null);
    }
}
