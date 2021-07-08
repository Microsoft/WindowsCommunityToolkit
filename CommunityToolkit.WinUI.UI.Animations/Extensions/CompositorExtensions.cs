// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;
using System.Diagnostics.Contracts;
using System.Numerics;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.UI;
using static CommunityToolkit.WinUI.UI.Animations.AnimationExtensions;

namespace CommunityToolkit.WinUI.UI.Animations
{
    /// <summary>
    /// An extension <see langword="class"/> for the <see cref="Compositor"/> type.
    /// </summary>
    public static class CompositorExtensions
    {
        /// <summary>
        /// Creates the appropriate <see cref="CompositionEasingFunction"/> from the given easing type and mode.
        /// </summary>
        /// <param name="compositor">The source <see cref="Compositor"/> used to create the easing function.</param>
        /// <param name="easingType">The target easing function to use.</param>
        /// <param name="easingMode">The target easing mode to use.</param>
        /// <returns>
        /// A <see cref="CompositionEasingFunction"/> instance with the specified easing, or <see langword="null"/>
        /// when the input parameters refer to the built-in easing, which means no instance is needed.
        /// </returns>
        [Pure]
        public static CompositionEasingFunction? TryCreateEasingFunction(this Compositor compositor, EasingType easingType = DefaultEasingType, EasingMode easingMode = DefaultEasingMode)
        {
            if (easingType == DefaultEasingType && easingMode == DefaultEasingMode)
            {
                return null;
            }

            if (easingType == EasingType.Linear)
            {
                return compositor.CreateLinearEasingFunction();
            }

            var (a, b) = EasingMaps[(easingType, easingMode)];

            return compositor.CreateCubicBezierEasingFunction(a, b);
        }

        /// <summary>
        /// Creates a <see cref="CubicBezierEasingFunction"/> from the input control points.
        /// </summary>
        /// <param name="compositor">The source <see cref="CompositionObject"/> used to create the easing function.</param>
        /// <param name="x1">The X coordinate of the first control point.</param>
        /// <param name="y1">The Y coordinate of the first control point.</param>
        /// <param name="x2">The X coordinate of the second control point.</param>
        /// <param name="y2">The Y coordinate of the second control point.</param>
        /// <returns>A <see cref="CubicBezierEasingFunction"/> instance with the given control points.</returns>
        [Pure]
        public static CubicBezierEasingFunction CreateCubicBezierEasingFunction(this Compositor compositor, float x1, float y1, float x2, float y2)
        {
            return compositor.CreateCubicBezierEasingFunction(new(x1, y1), new(x2, y2));
        }

        /// <summary>
        /// Creates a <see cref="BooleanKeyFrameAnimation"/> instance with the given parameters to on a target element.
        /// </summary>
        /// <param name="compositor">The current <see cref="Compositor"/> instance used to create the animation.</param>
        /// <param name="target">The optional target property to animate.</param>
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="delayBehavior">The delay behavior to use for the animation.</param>
        /// <param name="direction">The direction to use when playing the animation.</param>
        /// <param name="iterationBehavior">The iteration behavior to use for the animation.</param>
        /// <param name="iterationCount">The iteration count to use for the animation.</param>
        /// <returns>A <see cref="BooleanKeyFrameAnimation"/> instance with the specified parameters.</returns>
        [Pure]
        public static BooleanKeyFrameAnimation CreateBooleanKeyFrameAnimation(
            this Compositor compositor,
            string? target,
            bool to,
            bool? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            AnimationDelayBehavior delayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay,
            AnimationDirection direction = AnimationDirection.Normal,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count,
            int iterationCount = 1)
        {
            BooleanKeyFrameAnimation animation = compositor.CreateBooleanKeyFrameAnimation();

            animation.Duration = duration ?? DefaultDuration;

            if (delay.HasValue)
            {
                animation.DelayTime = delay.Value;
                animation.DelayBehavior = delayBehavior;
            }

            animation.InsertKeyFrame(1, to);

            if (from.HasValue)
            {
                animation.InsertKeyFrame(0, from.Value);
            }

            animation.Target = target;
            animation.Direction = direction;
            animation.IterationBehavior = iterationBehavior;
            animation.IterationCount = iterationCount;

            return animation;
        }

        /// <summary>
        /// Creates a <see cref="ScalarKeyFrameAnimation"/> instance with the given parameters to on a target element.
        /// </summary>
        /// <param name="compositor">The current <see cref="Compositor"/> instance used to create the animation.</param>
        /// <param name="target">The optional target property to animate.</param>
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="easing">The optional easing function for the animation.</param>
        /// <param name="delayBehavior">The delay behavior to use for the animation.</param>
        /// <param name="direction">The direction to use when playing the animation.</param>
        /// <param name="iterationBehavior">The iteration behavior to use for the animation.</param>
        /// <param name="iterationCount">The iteration count to use for the animation.</param>
        /// <returns>A <see cref="ScalarKeyFrameAnimation"/> instance with the specified parameters.</returns>
        [Pure]
        public static ScalarKeyFrameAnimation CreateScalarKeyFrameAnimation(
            this Compositor compositor,
            string? target,
            float to,
            float? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            CompositionEasingFunction? easing = null,
            AnimationDelayBehavior delayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay,
            AnimationDirection direction = AnimationDirection.Normal,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count,
            int iterationCount = 1)
        {
            ScalarKeyFrameAnimation animation = compositor.CreateScalarKeyFrameAnimation();

            animation.Duration = duration ?? DefaultDuration;

            if (delay.HasValue)
            {
                animation.DelayTime = delay.Value;
                animation.DelayBehavior = delayBehavior;
            }

            if (easing is null)
            {
                animation.InsertKeyFrame(1, to);
            }
            else
            {
                animation.InsertKeyFrame(1, to, easing);
            }

            if (from.HasValue)
            {
                animation.InsertKeyFrame(0, from.Value);
            }

            animation.Target = target;
            animation.Direction = direction;
            animation.IterationBehavior = iterationBehavior;
            animation.IterationCount = iterationCount;

            return animation;
        }

        /// <summary>
        /// Creates a <see cref="ScalarKeyFrameAnimation"/> instance with the given parameters to on a target element.
        /// </summary>
        /// <param name="compositor">The current <see cref="Compositor"/> instance used to create the animation.</param>
        /// <param name="target">The optional target property to animate.</param>
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="easing">The optional easing function for the animation.</param>
        /// <param name="delayBehavior">The delay behavior to use for the animation.</param>
        /// <param name="direction">The direction to use when playing the animation.</param>
        /// <param name="iterationBehavior">The iteration behavior to use for the animation.</param>
        /// <param name="iterationCount">The iteration count to use for the animation.</param>
        /// <returns>A <see cref="Vector2KeyFrameAnimation"/> instance with the specified parameters.</returns>
        [Pure]
        public static Vector2KeyFrameAnimation CreateVector2KeyFrameAnimation(
            this Compositor compositor,
            string? target,
            Vector2 to,
            Vector2? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            CompositionEasingFunction? easing = null,
            AnimationDelayBehavior delayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay,
            AnimationDirection direction = AnimationDirection.Normal,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count,
            int iterationCount = 1)
        {
            Vector2KeyFrameAnimation animation = compositor.CreateVector2KeyFrameAnimation();

            animation.Duration = duration ?? DefaultDuration;

            if (delay.HasValue)
            {
                animation.DelayTime = delay.Value;
                animation.DelayBehavior = delayBehavior;
            }

            if (easing is null)
            {
                animation.InsertKeyFrame(1, to);
            }
            else
            {
                animation.InsertKeyFrame(1, to, easing);
            }

            if (from.HasValue)
            {
                animation.InsertKeyFrame(0, from.Value);
            }

            animation.Target = target;
            animation.Direction = direction;
            animation.IterationBehavior = iterationBehavior;
            animation.IterationCount = iterationCount;

            return animation;
        }

        /// <summary>
        /// Creates a <see cref="ScalarKeyFrameAnimation"/> instance with the given parameters to on a target element.
        /// </summary>
        /// <param name="compositor">The current <see cref="Compositor"/> instance used to create the animation.</param>
        /// <param name="target">The optional target property to animate.</param>
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="easing">The optional easing function for the animation.</param>
        /// <param name="delayBehavior">The delay behavior to use for the animation.</param>
        /// <param name="direction">The direction to use when playing the animation.</param>
        /// <param name="iterationBehavior">The iteration behavior to use for the animation.</param>
        /// <param name="iterationCount">The iteration count to use for the animation.</param>
        /// <returns>A <see cref="Vector3KeyFrameAnimation"/> instance with the specified parameters.</returns>
        [Pure]
        public static Vector3KeyFrameAnimation CreateVector3KeyFrameAnimation(
            this Compositor compositor,
            string? target,
            Vector3 to,
            Vector3? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            CompositionEasingFunction? easing = null,
            AnimationDelayBehavior delayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay,
            AnimationDirection direction = AnimationDirection.Normal,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count,
            int iterationCount = 1)
        {
            Vector3KeyFrameAnimation animation = compositor.CreateVector3KeyFrameAnimation();

            animation.Duration = duration ?? DefaultDuration;

            if (delay.HasValue)
            {
                animation.DelayTime = delay.Value;
                animation.DelayBehavior = delayBehavior;
            }

            if (easing is null)
            {
                animation.InsertKeyFrame(1, to);
            }
            else
            {
                animation.InsertKeyFrame(1, to, easing);
            }

            if (from.HasValue)
            {
                animation.InsertKeyFrame(0, from.Value);
            }

            animation.Target = target;
            animation.Direction = direction;
            animation.IterationBehavior = iterationBehavior;
            animation.IterationCount = iterationCount;

            return animation;
        }

        /// <summary>
        /// Creates a <see cref="Vector4KeyFrameAnimation"/> instance with the given parameters to on a target element.
        /// </summary>
        /// <param name="compositor">The current <see cref="Compositor"/> instance used to create the animation.</param>
        /// <param name="target">The optional target property to animate.</param>
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="easing">The optional easing function for the animation.</param>
        /// <param name="delayBehavior">The delay behavior to use for the animation.</param>
        /// <param name="direction">The direction to use when playing the animation.</param>
        /// <param name="iterationBehavior">The iteration behavior to use for the animation.</param>
        /// <param name="iterationCount">The iteration count to use for the animation.</param>
        /// <returns>A <see cref="Vector4KeyFrameAnimation"/> instance with the specified parameters.</returns>
        [Pure]
        public static Vector4KeyFrameAnimation CreateVector4KeyFrameAnimation(
            this Compositor compositor,
            string? target,
            Vector4 to,
            Vector4? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            CompositionEasingFunction? easing = null,
            AnimationDelayBehavior delayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay,
            AnimationDirection direction = AnimationDirection.Normal,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count,
            int iterationCount = 1)
        {
            Vector4KeyFrameAnimation animation = compositor.CreateVector4KeyFrameAnimation();

            animation.Duration = duration ?? DefaultDuration;

            if (delay.HasValue)
            {
                animation.DelayTime = delay.Value;
                animation.DelayBehavior = delayBehavior;
            }

            if (easing is null)
            {
                animation.InsertKeyFrame(1, to);
            }
            else
            {
                animation.InsertKeyFrame(1, to, easing);
            }

            if (from.HasValue)
            {
                animation.InsertKeyFrame(0, from.Value);
            }

            animation.Target = target;
            animation.Direction = direction;
            animation.IterationBehavior = iterationBehavior;
            animation.IterationCount = iterationCount;

            return animation;
        }

        /// <summary>
        /// Creates a <see cref="ColorKeyFrameAnimation"/> instance with the given parameters to on a target element.
        /// </summary>
        /// <param name="compositor">The current <see cref="Compositor"/> instance used to create the animation.</param>
        /// <param name="target">The optional target property to animate.</param>
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="easing">The optional easing function for the animation.</param>
        /// <param name="delayBehavior">The delay behavior to use for the animation.</param>
        /// <param name="direction">The direction to use when playing the animation.</param>
        /// <param name="iterationBehavior">The iteration behavior to use for the animation.</param>
        /// <param name="iterationCount">The iteration count to use for the animation.</param>
        /// <returns>A <see cref="ColorKeyFrameAnimation"/> instance with the specified parameters.</returns>
        [Pure]
        public static ColorKeyFrameAnimation CreateColorKeyFrameAnimation(
            this Compositor compositor,
            string? target,
            Color to,
            Color? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            CompositionEasingFunction? easing = null,
            AnimationDelayBehavior delayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay,
            AnimationDirection direction = AnimationDirection.Normal,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count,
            int iterationCount = 1)
        {
            ColorKeyFrameAnimation animation = compositor.CreateColorKeyFrameAnimation();

            animation.Duration = duration ?? DefaultDuration;

            if (delay.HasValue)
            {
                animation.DelayTime = delay.Value;
                animation.DelayBehavior = delayBehavior;
            }

            if (easing is null)
            {
                animation.InsertKeyFrame(1, to);
            }
            else
            {
                animation.InsertKeyFrame(1, to, easing);
            }

            if (from.HasValue)
            {
                animation.InsertKeyFrame(0, from.Value);
            }

            animation.Target = target;
            animation.Direction = direction;
            animation.IterationBehavior = iterationBehavior;
            animation.IterationCount = iterationCount;

            return animation;
        }

        /// <summary>
        /// Creates a <see cref="QuaternionKeyFrameAnimation"/> instance with the given parameters to on a target element.
        /// </summary>
        /// <param name="compositor">The current <see cref="Compositor"/> instance used to create the animation.</param>
        /// <param name="target">The optional target property to animate.</param>
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="easing">The optional easing function for the animation.</param>
        /// <param name="delayBehavior">The delay behavior to use for the animation.</param>
        /// <param name="direction">The direction to use when playing the animation.</param>
        /// <param name="iterationBehavior">The iteration behavior to use for the animation.</param>
        /// <param name="iterationCount">The iteration count to use for the animation.</param>
        /// <returns>A <see cref="QuaternionKeyFrameAnimation"/> instance with the specified parameters.</returns>
        [Pure]
        public static QuaternionKeyFrameAnimation CreateQuaternionKeyFrameAnimation(
            this Compositor compositor,
            string? target,
            Quaternion to,
            Quaternion? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            CompositionEasingFunction? easing = null,
            AnimationDelayBehavior delayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay,
            AnimationDirection direction = AnimationDirection.Normal,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count,
            int iterationCount = 1)
        {
            QuaternionKeyFrameAnimation animation = compositor.CreateQuaternionKeyFrameAnimation();

            animation.Duration = duration ?? DefaultDuration;

            if (delay.HasValue)
            {
                animation.DelayTime = delay.Value;
                animation.DelayBehavior = delayBehavior;
            }

            if (easing is null)
            {
                animation.InsertKeyFrame(1, to);
            }
            else
            {
                animation.InsertKeyFrame(1, to, easing);
            }

            if (from.HasValue)
            {
                animation.InsertKeyFrame(0, from.Value);
            }

            animation.Target = target;
            animation.Direction = direction;
            animation.IterationBehavior = iterationBehavior;
            animation.IterationCount = iterationCount;

            return animation;
        }
    }
}