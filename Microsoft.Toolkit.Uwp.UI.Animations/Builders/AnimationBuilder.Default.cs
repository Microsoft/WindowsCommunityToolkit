// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
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
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public AnimationBuilder AnchorPoint(
            Axis axis,
            double to,
            double? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode)
        {
            AddCompositionAnimationFactory(Properties.Composition.AnchorPoint(axis), (float)to, (float?)from, delay, duration, repeat, easingType, easingMode);

            return this;
        }

        /// <summary>
        /// Adds a new anchor point animation for the X and Y axes to the current schedule.
        /// </summary>
        /// <param name="to">The final point for the animation.</param>
        /// <param name="from">The optional starting point for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public AnimationBuilder AnchorPoint(
            Vector2 to,
            Vector2? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode)
        {
            AddCompositionAnimationFactory(nameof(Visual.AnchorPoint), to, from, delay, duration, repeat, easingType, easingMode);

            return this;
        }

        /// <summary>
        /// Adds a new opacity animation to the current schedule.
        /// </summary>
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <param name="layer">The target framework layer to animate.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        public AnimationBuilder Opacity(
            double to,
            double? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode,
            FrameworkLayer layer = FrameworkLayer.Composition)
        {
            if (layer == FrameworkLayer.Composition)
            {
                AddCompositionAnimationFactory(nameof(Visual.Opacity), (float)to, (float?)from, delay, duration, repeat, easingType, easingMode);
            }
            else
            {
                AddXamlAnimationFactory(nameof(UIElement.Opacity), to, from, delay, duration, repeat, easingType, easingMode);
            }

            return this;
        }

        /// <summary>
        /// Adds a new translation animation for a single axis to the current schedule.
        /// </summary>
        /// <param name="axis">The target translation axis to animate.</param>
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <param name="layer">The target framework layer to animate.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        public AnimationBuilder Translation(
            Axis axis,
            double to,
            double? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode,
            FrameworkLayer layer = FrameworkLayer.Composition)
        {
            if (layer == FrameworkLayer.Composition)
            {
                AddCompositionAnimationFactory(Properties.Composition.Translation(axis), (float)to, (float?)from, delay, duration, repeat, easingType, easingMode);
            }
            else
            {
                AddXamlAnimationFactory(Properties.Xaml.Translation(axis), to, from, delay, duration, repeat, easingType, easingMode);
            }

            return this;
        }

        /// <summary>
        /// Adds a new translation animation for the X and Y axes to the current schedule.
        /// </summary>
        /// <param name="to">The final point for the animation.</param>
        /// <param name="from">The optional starting point for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <param name="layer">The target framework layer to animate.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        public AnimationBuilder Translation(
            Vector2 to,
            Vector2? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode,
            FrameworkLayer layer = FrameworkLayer.Composition)
        {
            if (layer == FrameworkLayer.Composition)
            {
                AddCompositionAnimationFactory(Properties.Composition.TranslationXY(), to, from, delay, duration, repeat, easingType, easingMode);
            }
            else
            {
                AddXamlTransformDoubleAnimationFactory(nameof(CompositeTransform.TranslateX), to.X, from?.X, delay, duration, repeat, easingType, easingMode);
                AddXamlTransformDoubleAnimationFactory(nameof(CompositeTransform.TranslateY), to.Y, from?.Y, delay, duration, repeat, easingType, easingMode);
            }

            return this;
        }

        /// <summary>
        /// Adds a new composition translation animation for all axes to the current schedule.
        /// </summary>
        /// <param name="to">The final point for the animation.</param>
        /// <param name="from">The optional starting point for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public AnimationBuilder Translation(
            Vector3 to,
            Vector3? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode)
        {
            return AddCompositionAnimationFactory(Properties.Composition.Translation(), to, from, delay, duration, repeat, easingType, easingMode);
        }

        /// <summary>
        /// Adds a new composition offset animation for a single axis to the current schedule.
        /// </summary>
        /// <param name="axis">The target translation axis to animate.</param>
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public AnimationBuilder Offset(
            Axis axis,
            double to,
            double? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode)
        {
            return AddCompositionAnimationFactory(Properties.Composition.Offset(axis), (float)to, (float?)from, delay, duration, repeat, easingType, easingMode);
        }

        /// <summary>
        /// Adds a new composition offset animation for the X and Y axes to the current schedule.
        /// </summary>
        /// <param name="to">The final point for the animation.</param>
        /// <param name="from">The optional starting point for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public AnimationBuilder Offset(
            Vector2 to,
            Vector2? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode)
        {
            AddCompositionAnimationFactory(Properties.Composition.OffsetXY(), to, from, delay, duration, repeat, easingType, easingMode);

            return this;
        }

        /// <summary>
        /// Adds a new composition offset translation animation for all axes to the current schedule.
        /// </summary>
        /// <param name="to">The final point for the animation.</param>
        /// <param name="from">The optional starting point for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public AnimationBuilder Offset(
            Vector3 to,
            Vector3? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode)
        {
            return AddCompositionAnimationFactory(nameof(Visual.Offset), to, from, delay, duration, repeat, easingType, easingMode);
        }

        /// <summary>
        /// Adds a new uniform scale animation for all axes to the current schedule.
        /// </summary>
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <param name="layer">The target framework layer to animate.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        public AnimationBuilder Scale(
            double to,
            double? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode,
            FrameworkLayer layer = FrameworkLayer.Composition)
        {
            if (layer == FrameworkLayer.Composition)
            {
                Vector3? from3 = from is null ? null : new((float)(double)from);
                Vector3 to3 = new((float)to);

                AddCompositionAnimationFactory(nameof(Visual.Scale), to3, from3, delay, duration, repeat, easingType, easingMode);
            }
            else
            {
                AddXamlTransformDoubleAnimationFactory(nameof(CompositeTransform.ScaleX), to, from, delay, duration, repeat, easingType, easingMode);
                AddXamlTransformDoubleAnimationFactory(nameof(CompositeTransform.ScaleY), to, from, delay, duration, repeat, easingType, easingMode);
            }

            return this;
        }

        /// <summary>
        /// Adds a new scale animation on a specified axis to the current schedule.
        /// </summary>
        /// <param name="axis">The target scale axis to animate.</param>
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <param name="layer">The target framework layer to animate.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        public AnimationBuilder Scale(
            Axis axis,
            double to,
            double? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode,
            FrameworkLayer layer = FrameworkLayer.Composition)
        {
            if (layer == FrameworkLayer.Composition)
            {
                AddCompositionAnimationFactory(Properties.Composition.Scale(axis), (float)to, (float?)from, delay, duration, repeat, easingType, easingMode);
            }
            else
            {
                AddXamlTransformDoubleAnimationFactory(Properties.Xaml.Scale(axis), to, from, delay, duration, repeat, easingType, easingMode);
            }

            return this;
        }

        /// <summary>
        /// Adds a new scale animation for the X and Y axes to the current schedule.
        /// </summary>
        /// <param name="to">The final point for the animation.</param>
        /// <param name="from">The optional starting point for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <param name="layer">The target framework layer to animate.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        public AnimationBuilder Scale(
            Vector2 to,
            Vector2? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode,
            FrameworkLayer layer = FrameworkLayer.Composition)
        {
            if (layer == FrameworkLayer.Composition)
            {
                AddCompositionAnimationFactory(Properties.Composition.ScaleXY(), to, from, delay, duration, repeat, easingType, easingMode);
            }
            else
            {
                AddXamlTransformDoubleAnimationFactory(nameof(CompositeTransform.ScaleX), to.X, from?.X, delay, duration, repeat, easingType, easingMode);
                AddXamlTransformDoubleAnimationFactory(nameof(CompositeTransform.ScaleY), to.Y, from?.Y, delay, duration, repeat, easingType, easingMode);
            }

            return this;
        }

        /// <summary>
        /// Adds a new scale animation for all axes to the current schedule.
        /// </summary>
        /// <param name="to">The final point for the animation.</param>
        /// <param name="from">The optional starting point for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public AnimationBuilder Scale(
            Vector3 to,
            Vector3? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode)
        {
            return AddCompositionAnimationFactory(nameof(Visual.Scale), to, from, delay, duration, repeat, easingType, easingMode);
        }

        /// <summary>
        /// Adds a new center point animation on a specified axis to the current schedule.
        /// </summary>
        /// <param name="axis">The target scale axis to animate.</param>
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <param name="layer">The target framework layer to animate.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        public AnimationBuilder CenterPoint(
            Axis axis,
            double to,
            double? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode,
            FrameworkLayer layer = FrameworkLayer.Composition)
        {
            if (layer == FrameworkLayer.Composition)
            {
                AddCompositionAnimationFactory(Properties.Composition.CenterPoint(axis), (float)to, (float?)from, delay, duration, repeat, easingType, easingMode);
            }
            else
            {
                AddXamlTransformDoubleAnimationFactory(Properties.Xaml.CenterPoint(axis), to, from, delay, duration, repeat, easingType, easingMode);
            }

            return this;
        }

        /// <summary>
        /// Adds a new center point animation for the X and Y axes to the current schedule.
        /// </summary>
        /// <param name="to">The final point for the animation.</param>
        /// <param name="from">The optional starting point for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <param name="layer">The target framework layer to animate.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        public AnimationBuilder CenterPoint(
            Vector2 to,
            Vector2? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode,
            FrameworkLayer layer = FrameworkLayer.Composition)
        {
            if (layer == FrameworkLayer.Composition)
            {
                AddCompositionAnimationFactory(Properties.Composition.CenterPointXY(), to, from, delay, duration, repeat, easingType, easingMode);
            }
            else
            {
                AddXamlTransformDoubleAnimationFactory(nameof(CompositeTransform.CenterX), to.X, from?.X, delay, duration, repeat, easingType, easingMode);
                AddXamlTransformDoubleAnimationFactory(nameof(CompositeTransform.CenterY), to.Y, from?.Y, delay, duration, repeat, easingType, easingMode);
            }

            return this;
        }

        /// <summary>
        /// Adds a new center point animation for all axes to the current schedule.
        /// </summary>
        /// <param name="to">The final point for the animation.</param>
        /// <param name="from">The optional starting point for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public AnimationBuilder CenterPoint(
            Vector3 to,
            Vector3? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode)
        {
            return AddCompositionAnimationFactory(nameof(Visual.CenterPoint), to, from, delay, duration, repeat, easingType, easingMode);
        }

        /// <summary>
        /// Adds a new rotation animation to the current schedule.
        /// </summary>
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <param name="layer">The target framework layer to animate.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        public AnimationBuilder Rotation(
            double to,
            double? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode,
            FrameworkLayer layer = FrameworkLayer.Composition)
        {
            if (layer == FrameworkLayer.Composition)
            {
                AddCompositionAnimationFactory(nameof(Visual.RotationAngle), (float)to, (float?)from, delay, duration, repeat, easingType, easingMode);
            }
            else
            {
                double? fromDegrees = from * Math.PI / 180;
                double toDegrees = to * Math.PI / 180;

                AddXamlTransformDoubleAnimationFactory(nameof(CompositeTransform.Rotation), toDegrees, fromDegrees, delay, duration, repeat, easingType, easingMode);
            }

            return this;
        }

        /// <summary>
        /// Adds a new rotation animation in degrees to the current schedule.
        /// </summary>
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <param name="layer">The target framework layer to animate.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        public AnimationBuilder RotationInDegrees(
            double to,
            double? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode,
            FrameworkLayer layer = FrameworkLayer.Composition)
        {
            if (layer == FrameworkLayer.Composition)
            {
                AddCompositionAnimationFactory(nameof(Visual.RotationAngleInDegrees), (float)to, (float?)from, delay, duration, repeat, easingType, easingMode);
            }
            else
            {
                AddXamlTransformDoubleAnimationFactory(nameof(CompositeTransform.Rotation), to, from, delay, duration, repeat, easingType, easingMode);
            }

            return this;
        }

        /// <summary>
        /// Adds a new rotation axis animation to the current schedule.
        /// </summary>
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public AnimationBuilder RotationAxis(
            Vector3 to,
            Vector3? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode)
        {
            return AddCompositionAnimationFactory(nameof(Visual.RotationAxis), to, from, delay, duration, repeat, easingType, easingMode);
        }

        /// <summary>
        /// Adds a new orientation animation to the current schedule.
        /// </summary>
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public AnimationBuilder Orientation(
            Quaternion to,
            Quaternion? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode)
        {
            return AddCompositionAnimationFactory(nameof(Visual.Orientation), to, from, delay, duration, repeat, easingType, easingMode);
        }

        /// <summary>
        /// Adds a new transform animation to the current schedule.
        /// </summary>
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public AnimationBuilder Transform(
            Matrix4x4 to,
            Matrix4x4? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode)
        {
            if (!Matrix4x4.Decompose(to, out Vector3 toScale, out Quaternion toRotation, out Vector3 toTranslation))
            {
                ThrowThrowArgumentExceptionForToDecompose();
            }

            Vector3? fromScale = null;
            Quaternion? fromRotation = null;
            Vector3? fromTranslation = null;

            if (from.HasValue)
            {
                if (!Matrix4x4.Decompose(from.GetValueOrDefault(), out Vector3 scale3, out Quaternion rotation4, out Vector3 translation3))
                {
                    ThrowThrowArgumentExceptionForFromDecompose();
                }

                fromScale = scale3;
                fromRotation = rotation4;
                fromTranslation = translation3;
            }

            Scale(toScale, fromScale, delay, duration, repeat, easingType, easingMode);
            Orientation(toRotation, fromRotation, delay, duration, repeat, easingType, easingMode);
            Translation(toTranslation, fromTranslation, delay, duration, repeat, easingType, easingMode);

            return this;

            static void ThrowThrowArgumentExceptionForToDecompose() => throw new ArgumentException("The destination matrix could not be decomposed");
            static void ThrowThrowArgumentExceptionForFromDecompose() => throw new ArgumentException("The initial matrix could not be decomposed");
        }

        /// <summary>
        /// Adds a new clip animation to the current schedule.
        /// </summary>
        /// <param name="side">The clip size to animate.</param>
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public AnimationBuilder Clip(
            Side side,
            double to,
            double? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode)
        {
            CompositionClipScalarAnimation animation = new(
                Properties.Composition.Clip(side),
                (float)to,
                (float?)from,
                delay ?? DefaultDelay,
                duration ?? DefaultDuration,
                repeat ?? RepeatOption.Once,
                easingType,
                easingMode);

            this.compositionAnimationFactories.Add(animation);

            return this;
        }

        /// <summary>
        /// Adds a new clip animation to the current schedule.
        /// </summary>
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        /// <remarks>This animation is only available on the composition layer.</remarks>
        public AnimationBuilder Clip(
            Thickness to,
            Thickness? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode)
        {
            this.compositionAnimationFactories.Add(new CompositionClipScalarAnimation(
                nameof(InsetClip.LeftInset),
                (float)to.Left,
                (float?)from?.Left,
                delay ?? DefaultDelay,
                duration ?? DefaultDuration,
                repeat ?? RepeatOption.Once,
                easingType,
                easingMode));

            this.compositionAnimationFactories.Add(new CompositionClipScalarAnimation(
                nameof(InsetClip.TopInset),
                (float)to.Top,
                (float?)from?.Top,
                delay ?? DefaultDelay,
                duration ?? DefaultDuration,
                repeat ?? RepeatOption.Once,
                easingType,
                easingMode));

            this.compositionAnimationFactories.Add(new CompositionClipScalarAnimation(
                nameof(InsetClip.RightInset),
                (float)to.Right,
                (float?)from?.Right,
                delay ?? DefaultDelay,
                duration ?? DefaultDuration,
                repeat ?? RepeatOption.Once,
                easingType,
                easingMode));

            this.compositionAnimationFactories.Add(new CompositionClipScalarAnimation(
                nameof(InsetClip.BottomInset),
                (float)to.Bottom,
                (float?)from?.Bottom,
                delay ?? DefaultDelay,
                duration ?? DefaultDuration,
                repeat ?? RepeatOption.Once,
                easingType,
                easingMode));

            return this;
        }

        /// <summary>
        /// Adds a new size animation for a single axis to the current schedule.
        /// </summary>
        /// <param name="axis">The target size axis to animate.</param>
        /// <param name="to">The final value for the animation.</param>
        /// <param name="from">The optional starting value for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <param name="layer">The target framework layer to animate.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        public AnimationBuilder Size(
            Axis axis,
            double to,
            double? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode,
            FrameworkLayer layer = FrameworkLayer.Composition)
        {
            if (layer == FrameworkLayer.Composition)
            {
                AddCompositionAnimationFactory(Properties.Composition.Size(axis), (float)to, (float?)from, delay, duration, repeat, easingType, easingMode);
            }
            else
            {
                AddXamlAnimationFactory(Properties.Xaml.Size(axis), to, from, delay, duration, repeat, easingType, easingMode);
            }

            return this;
        }

        /// <summary>
        /// Adds a new size animation for the X and Y axes to the current schedule.
        /// </summary>
        /// <param name="to">The final point for the animation.</param>
        /// <param name="from">The optional starting point for the animation.</param>
        /// <param name="delay">The optional initial delay for the animation.</param>
        /// <param name="duration">The optional animation duration.</param>
        /// <param name="repeat">The optional repeat mode (defaults to once).</param>
        /// <param name="easingType">The optional easing function type for the animation.</param>
        /// <param name="easingMode">The optional easing function mode for the animation.</param>
        /// <param name="layer">The target framework layer to animate.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        public AnimationBuilder Size(
            Vector2 to,
            Vector2? from = null,
            TimeSpan? delay = null,
            TimeSpan? duration = null,
            RepeatOption? repeat = null,
            EasingType easingType = DefaultEasingType,
            EasingMode easingMode = DefaultEasingMode,
            FrameworkLayer layer = FrameworkLayer.Composition)
        {
            if (layer == FrameworkLayer.Composition)
            {
                AddCompositionAnimationFactory(Properties.Composition.Size(), to, from, delay, duration, repeat, easingType, easingMode);
            }
            else
            {
                AddXamlAnimationFactory(nameof(FrameworkElement.Width), to.X, from?.X, delay, duration, repeat, easingType, easingMode);
                AddXamlAnimationFactory(nameof(FrameworkElement.Height), to.Y, from?.Y, delay, duration, repeat, easingType, easingMode);
            }

            return this;
        }
    }
}