// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml.Media;
using static Microsoft.Toolkit.Uwp.UI.Animations.AnimationExtensions;

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <inheritdoc cref="AnimationBuilder"/>
    public sealed partial class AnimationBuilder
    {
        /// <summary>
        /// Adds a new anchor point animation for a single axis to the current schedule.
        /// </summary>
        /// <param name="axis">The target anchor point axis to animate.</param>
        /// <returns>An <see cref="IPropertyAnimationBuilder{T}"/> instance to configure the animation.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public IPropertyAnimationBuilder<double> AnchorPoint(Axis axis)
        {
            return new PropertyAnimationBuilder<double>(this, Properties.Composition.AnchorPoint(axis), FrameworkLayer.Composition);
        }

        /// <summary>
        /// Adds a new anchor point animation for the X and Y axes to the current schedule.
        /// </summary>
        /// <returns>An <see cref="IPropertyAnimationBuilder{T}"/> instance to configure the animation.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public IPropertyAnimationBuilder<Vector2> AnchorPoint()
        {
            return new PropertyAnimationBuilder<Vector2>(this, nameof(Visual.AnchorPoint), FrameworkLayer.Composition);
        }

        /// <summary>
        /// Adds a new opacity animation to the current schedule.
        /// </summary>
        /// <param name="layer">The target framework layer to animate.</param>
        /// <returns>An <see cref="IPropertyAnimationBuilder{T}"/> instance to configure the animation.</returns>
        public IPropertyAnimationBuilder<double> Opacity(FrameworkLayer layer = FrameworkLayer.Composition)
        {
            return new PropertyAnimationBuilder<double>(this, nameof(Visual.Opacity), layer);
        }

        /// <summary>
        /// Adds a new translation animation for a single axis to the current schedule.
        /// </summary>
        /// <param name="axis">The target translation axis to animate.</param>
        /// <param name="layer">The target framework layer to animate.</param>
        /// <returns>An <see cref="IPropertyAnimationBuilder{T}"/> instance to configure the animation.</returns>
        public IPropertyAnimationBuilder<double> Translation(Axis axis, FrameworkLayer layer = FrameworkLayer.Composition)
        {
            if (layer == FrameworkLayer.Composition)
            {
                return new PropertyAnimationBuilder<double>(this, Properties.Composition.Translation(axis), layer);
            }

            return new XamlTransformPropertyAnimationBuilder(this, Properties.Xaml.Translation(axis));
        }

        /// <summary>
        /// Adds a new composition translation animation for all axes to the current schedule.
        /// </summary>
        /// <returns>An <see cref="IPropertyAnimationBuilder{T}"/> instance to configure the animation.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public IPropertyAnimationBuilder<Vector3> Translation()
        {
            return new PropertyAnimationBuilder<Vector3>(this, Properties.Composition.Translation(), FrameworkLayer.Composition);
        }

        /// <summary>
        /// Adds a new composition offset animation for a single axis to the current schedule.
        /// </summary>
        /// <param name="axis">The target translation axis to animate.</param>
        /// <returns>An <see cref="IPropertyAnimationBuilder{T}"/> instance to configure the animation.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public IPropertyAnimationBuilder<double> Offset(Axis axis)
        {
            return new PropertyAnimationBuilder<double>(this, Properties.Composition.Offset(axis), FrameworkLayer.Composition);
        }

        /// <summary>
        /// Adds a new composition offset translation animation for all axes to the current schedule.
        /// </summary>
        /// <returns>An <see cref="IPropertyAnimationBuilder{T}"/> instance to configure the animation.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public IPropertyAnimationBuilder<Vector3> Offset()
        {
            return new PropertyAnimationBuilder<Vector3>(this, nameof(Visual.Offset), FrameworkLayer.Composition);
        }

        /// <summary>
        /// Adds a new scale animation on a specified axis to the current schedule.
        /// </summary>
        /// <param name="axis">The target scale axis to animate.</param>
        /// <param name="layer">The target framework layer to animate.</param>
        /// <returns>An <see cref="IPropertyAnimationBuilder{T}"/> instance to configure the animation.</returns>
        public IPropertyAnimationBuilder<double> Scale(Axis axis, FrameworkLayer layer = FrameworkLayer.Composition)
        {
            if (layer == FrameworkLayer.Composition)
            {
                return new PropertyAnimationBuilder<double>(this, Properties.Composition.Scale(axis), layer);
            }

            return new XamlTransformPropertyAnimationBuilder(this, Properties.Xaml.Scale(axis));
        }

        /// <summary>
        /// Adds a new scale animation for all axes to the current schedule.
        /// </summary>
        /// <returns>An <see cref="IPropertyAnimationBuilder{T}"/> instance to configure the animation.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public IPropertyAnimationBuilder<Vector3> Scale()
        {
            return new PropertyAnimationBuilder<Vector3>(this, nameof(Visual.Scale), FrameworkLayer.Composition);
        }

        /// <summary>
        /// Adds a new center point animation on a specified axis to the current schedule.
        /// </summary>
        /// <param name="axis">The target scale axis to animate.</param>
        /// <param name="layer">The target framework layer to animate.</param>
        /// <returns>An <see cref="IPropertyAnimationBuilder{T}"/> instance to configure the animation.</returns>
        public IPropertyAnimationBuilder<double> CenterPoint(Axis axis, FrameworkLayer layer = FrameworkLayer.Composition)
        {
            if (layer == FrameworkLayer.Composition)
            {
                return new PropertyAnimationBuilder<double>(this, Properties.Composition.CenterPoint(axis), layer);
            }

            return new XamlTransformPropertyAnimationBuilder(this, Properties.Xaml.CenterPoint(axis));
        }

        /// <summary>
        /// Adds a new center point animation for all axes to the current schedule.
        /// </summary>
        /// <returns>An <see cref="IPropertyAnimationBuilder{T}"/> instance to configure the animation.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public IPropertyAnimationBuilder<Vector3> CenterPoint()
        {
            return new PropertyAnimationBuilder<Vector3>(this, nameof(Visual.CenterPoint), FrameworkLayer.Composition);
        }

        /// <summary>
        /// Adds a new rotation animation to the current schedule.
        /// </summary>
        /// <returns>An <see cref="IPropertyAnimationBuilder{T}"/> instance to configure the animation.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public IPropertyAnimationBuilder<double> Rotation()
        {
            return new PropertyAnimationBuilder<double>(this, nameof(Visual.RotationAngle), FrameworkLayer.Composition);
        }

        /// <summary>
        /// Adds a new rotation animation in degrees to the current schedule.
        /// </summary>
        /// <param name="layer">The target framework layer to animate.</param>
        /// <returns>An <see cref="IPropertyAnimationBuilder{T}"/> instance to configure the animation.</returns>
        public IPropertyAnimationBuilder<double> RotationInDegrees(FrameworkLayer layer = FrameworkLayer.Composition)
        {
            if (layer == FrameworkLayer.Composition)
            {
                return new PropertyAnimationBuilder<double>(this, nameof(Visual.RotationAngleInDegrees), layer);
            }

            return new XamlTransformPropertyAnimationBuilder(this, nameof(CompositeTransform.Rotation));
        }

        /// <summary>
        /// Adds a new rotation axis animation to the current schedule.
        /// </summary>
        /// <returns>An <see cref="IPropertyAnimationBuilder{T}"/> instance to configure the animation.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public IPropertyAnimationBuilder<Vector3> RotationAxis()
        {
            return new PropertyAnimationBuilder<Vector3>(this, nameof(Visual.RotationAxis), FrameworkLayer.Composition);
        }

        /// <summary>
        /// Adds a new orientation animation to the current schedule.
        /// </summary>
        /// <returns>An <see cref="IPropertyAnimationBuilder{T}"/> instance to configure the animation.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public IPropertyAnimationBuilder<Quaternion> Orientation()
        {
            return new PropertyAnimationBuilder<Quaternion>(this, nameof(Visual.Orientation), FrameworkLayer.Composition);
        }

        /// <summary>
        /// Adds a new clip animation to the current schedule.
        /// </summary>
        /// <param name="side">The clip size to animate.</param>
        /// <returns>An <see cref="IPropertyAnimationBuilder{T}"/> instance to configure the animation.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public IPropertyAnimationBuilder<double> Clip(Side side)
        {
            return new CompositionClipAnimationBuilder(this, Properties.Composition.Clip(side));
        }

        /// <summary>
        /// Adds a new size animation for a single axis to the current schedule.
        /// </summary>
        /// <param name="axis">The target size axis to animate.</param>
        /// <param name="layer">The target framework layer to animate.</param>
        /// <returns>An <see cref="IPropertyAnimationBuilder{T}"/> instance to configure the animation.</returns>
        public IPropertyAnimationBuilder<double> Size(Axis axis, FrameworkLayer layer = FrameworkLayer.Composition)
        {
            if (layer == FrameworkLayer.Composition)
            {
                return new PropertyAnimationBuilder<double>(this, Properties.Composition.Size(axis), layer);
            }

            return new PropertyAnimationBuilder<double>(this, Properties.Xaml.Size(axis), layer);
        }

        /// <summary>
        /// Adds a new composition size translation animation for all axes to the current schedule.
        /// </summary>
        /// <returns>An <see cref="IPropertyAnimationBuilder{T}"/> instance to configure the animation.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public IPropertyAnimationBuilder<Vector2> Size()
        {
            return new PropertyAnimationBuilder<Vector2>(this, nameof(Visual.Size), FrameworkLayer.Composition);
        }

        /// <summary>
        /// Adds a custom animation based on normalized keyframes to the current schedule.
        /// </summary>
        /// <typeparam name="T">The type of values to animate.</typeparam>
        /// <param name="property">The target property to animate.</param>
        /// <param name="build">The callback to use to construct the custom animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The animation duration.</param>
        /// <param name="repeatOption">The repeat option for the animation (defaults to one iteration).</param>
        /// <param name="delayBehavior">The delay behavior to use (ignored if <paramref name="layer"/> is <see cref="FrameworkLayer.Xaml"/>).</param>
        /// <param name="layer">The target framework layer to animate.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        public AnimationBuilder NormalizedKeyFrames<T>(
            string property,
            Action<INormalizedKeyFrameAnimationBuilder<T>> build,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeatOption = null,
            AnimationDelayBehavior? delayBehavior = null,
            FrameworkLayer layer = FrameworkLayer.Composition)
            where T : unmanaged
        {
            if (layer == FrameworkLayer.Composition)
            {
                NormalizedKeyFrameAnimationBuilder<T>.Composition builder = new(
                    property,
                    delay,
                    duration ?? DefaultDuration,
                    repeatOption ?? RepeatOption.Once,
                    delayBehavior ?? DefaultDelayBehavior);

                build(builder);

                this.compositionAnimationFactories.Add(builder);
            }
            else
            {
                NormalizedKeyFrameAnimationBuilder<T>.Xaml builder = new(
                    property,
                    delay,
                    duration ?? DefaultDuration,
                    repeatOption ?? RepeatOption.Once);

                build(builder);

                this.xamlAnimationFactories.Add(builder);
            }

            return this;
        }

        /// <summary>
        /// Adds a custom animation based on normalized keyframes to the current schedule.
        /// </summary>
        /// <typeparam name="T">The type of values to animate.</typeparam>
        /// <typeparam name="TState">The type of state to pass to the builder.</typeparam>
        /// <param name="property">The target property to animate.</param>
        /// <param name="state">The state to pass to the builder.</param>
        /// <param name="build">The callback to use to construct the custom animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The animation duration.</param>
        /// <param name="repeatOption">The repeat option for the animation (defaults to one iteration).</param>
        /// <param name="delayBehavior">The delay behavior to use (ignored if <paramref name="layer"/> is <see cref="FrameworkLayer.Xaml"/>).</param>
        /// <param name="layer">The target framework layer to animate.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        public AnimationBuilder NormalizedKeyFrames<T, TState>(
            string property,
            TState state,
            Action<INormalizedKeyFrameAnimationBuilder<T>, TState> build,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeatOption = null,
            AnimationDelayBehavior? delayBehavior = null,
            FrameworkLayer layer = FrameworkLayer.Composition)
            where T : unmanaged
        {
            if (layer == FrameworkLayer.Composition)
            {
                NormalizedKeyFrameAnimationBuilder<T>.Composition builder = new(
                    property,
                    delay,
                    duration ?? DefaultDuration,
                    repeatOption ?? RepeatOption.Once,
                    delayBehavior ?? DefaultDelayBehavior);

                build(builder, state);

                this.compositionAnimationFactories.Add(builder);
            }
            else
            {
                NormalizedKeyFrameAnimationBuilder<T>.Xaml builder = new(
                    property,
                    delay,
                    duration ?? DefaultDuration,
                    repeatOption ?? RepeatOption.Once);

                build(builder, state);

                this.xamlAnimationFactories.Add(builder);
            }

            return this;
        }

        /// <summary>
        /// Adds a custom animation based on timed keyframes to the current schedule.
        /// </summary>
        /// <typeparam name="T">The type of values to animate.</typeparam>
        /// <param name="property">The target property to animate.</param>
        /// <param name="build">The callback to use to construct the custom animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="repeat">The repeat option for the animation (defaults to one iteration).</param>
        /// <param name="delayBehavior">The delay behavior to use (ignored if <paramref name="layer"/> is <see cref="FrameworkLayer.Xaml"/>).</param>
        /// <param name="layer">The target framework layer to animate.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        public AnimationBuilder TimedKeyFrames<T>(
            string property,
            Action<ITimedKeyFrameAnimationBuilder<T>> build,
            TimeSpan? delay = null,
            RepeatOption? repeat = null,
            AnimationDelayBehavior? delayBehavior = null,
            FrameworkLayer layer = FrameworkLayer.Composition)
            where T : unmanaged
        {
            if (layer == FrameworkLayer.Composition)
            {
                TimedKeyFrameAnimationBuilder<T>.Composition builder = new(
                    property,
                    delay,
                    repeat ?? RepeatOption.Once,
                    delayBehavior ?? DefaultDelayBehavior);

                build(builder);

                this.compositionAnimationFactories.Add(builder);
            }
            else
            {
                TimedKeyFrameAnimationBuilder<T>.Xaml builder = new(property, delay, repeat ?? RepeatOption.Once);

                build(builder);

                this.xamlAnimationFactories.Add(builder);
            }

            return this;
        }

        /// <summary>
        /// Adds a custom animation based on timed keyframes to the current schedule.
        /// </summary>
        /// <typeparam name="T">The type of values to animate.</typeparam>
        /// <typeparam name="TState">The type of state to pass to the builder.</typeparam>
        /// <param name="property">The target property to animate.</param>
        /// <param name="state">The state to pass to the builder.</param>
        /// <param name="build">The callback to use to construct the custom animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="repeatOption">The repeat option for the animation (defaults to one iteration).</param>
        /// <param name="delayBehavior">The delay behavior to use (ignored if <paramref name="layer"/> is <see cref="FrameworkLayer.Xaml"/>).</param>
        /// <param name="layer">The target framework layer to animate.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        public AnimationBuilder TimedKeyFrames<T, TState>(
            string property,
            TState state,
            Action<ITimedKeyFrameAnimationBuilder<T>, TState> build,
            TimeSpan? delay = null,
            RepeatOption? repeatOption = null,
            AnimationDelayBehavior? delayBehavior = null,
            FrameworkLayer layer = FrameworkLayer.Composition)
            where T : unmanaged
        {
            if (layer == FrameworkLayer.Composition)
            {
                TimedKeyFrameAnimationBuilder<T>.Composition builder = new(
                    property,
                    delay,
                    repeatOption ?? RepeatOption.Once,
                    delayBehavior ?? DefaultDelayBehavior);

                build(builder, state);

                this.compositionAnimationFactories.Add(builder);
            }
            else
            {
                TimedKeyFrameAnimationBuilder<T>.Xaml builder = new(property, delay, repeatOption ?? RepeatOption.Once);

                build(builder, state);

                this.xamlAnimationFactories.Add(builder);
            }

            return this;
        }
    }
}
