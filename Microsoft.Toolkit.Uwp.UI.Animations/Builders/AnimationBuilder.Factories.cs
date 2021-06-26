// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

#nullable enable

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <inheritdoc cref="AnimationBuilder"/>
    public sealed partial class AnimationBuilder
    {
        /// <summary>
        /// An interface for factories of XAML animations.
        /// </summary>
        internal interface IXamlAnimationFactory
        {
            /// <summary>
            /// Gets a <see cref="Timeline"/> instance representing the animation to start.
            /// </summary>
            /// <param name="targetHint">The suggested target <see cref="DependencyObject"/> instance to animate.</param>
            /// <returns>A <see cref="Timeline"/> instance with the specified animation.</returns>
            Timeline GetAnimation(DependencyObject targetHint);
        }

        /// <summary>
        /// An interface for factories of composition animations.
        /// </summary>
        internal interface ICompositionAnimationFactory
        {
            /// <summary>
            /// Gets a <see cref="CompositionAnimation"/> instance representing the animation to start.
            /// </summary>
            /// <param name="targetHint">The suggested target <see cref="CompositionObject"/> instance to animate.</param>
            /// <param name="target">An optional <see cref="CompositionObject"/> instance to animate instead of the suggested one.</param>
            /// <returns>A <see cref="CompositionAnimation"/> instance with the specified animation.</returns>
            /// <remarks>
            /// The separate <paramref name="target"/> parameter is needed because unlike with XAML animations, composition animations
            /// can't store the target instance internally, and need to be started on the target object directly. This means that custom
            /// animation factories that want to target an external object need to return that object separately to inform the callers.
            /// </remarks>
            CompositionAnimation GetAnimation(CompositionObject targetHint, out CompositionObject? target);
        }

        /// <summary>
        /// A model representing a generic animation for a target object.
        /// </summary>
        private sealed record AnimationFactory<T>(
            string Property,
            T To,
            T? From,
            TimeSpan Delay,
            TimeSpan Duration,
            RepeatOption Repeat,
            EasingType EasingType,
            EasingMode EasingMode)
            : ICompositionAnimationFactory, IXamlAnimationFactory
            where T : unmanaged
        {
            /// <inheritdoc/>
            public CompositionAnimation GetAnimation(CompositionObject targetHint, out CompositionObject? target)
            {
                CompositionEasingFunction? easingFunction = targetHint.Compositor.TryCreateEasingFunction(EasingType, EasingMode);
                (AnimationIterationBehavior iterationBehavior, int iterationCount) = Repeat.ToBehaviorAndCount();

                target = null;

                if (typeof(T) == typeof(bool))
                {
                    return targetHint.Compositor.CreateBooleanKeyFrameAnimation(
                        Property,
                        GetToAs<bool>(),
                        GetFromAs<bool>(),
                        Delay,
                        Duration,
                        iterationBehavior: iterationBehavior,
                        iterationCount: iterationCount);
                }

                if (typeof(T) == typeof(float))
                {
                    return targetHint.Compositor.CreateScalarKeyFrameAnimation(
                        Property,
                        GetToAs<float>(),
                        GetFromAs<float>(),
                        Delay,
                        Duration,
                        easingFunction,
                        iterationBehavior: iterationBehavior,
                        iterationCount: iterationCount);
                }

                if (typeof(T) == typeof(double))
                {
                    return targetHint.Compositor.CreateScalarKeyFrameAnimation(
                        Property,
                        (float)GetToAs<double>(),
                        (float?)GetFromAs<double>(),
                        Delay,
                        Duration,
                        easingFunction,
                        iterationBehavior: iterationBehavior,
                        iterationCount: iterationCount);
                }

                if (typeof(T) == typeof(Vector2))
                {
                    return targetHint.Compositor.CreateVector2KeyFrameAnimation(
                        Property,
                        GetToAs<Vector2>(),
                        GetFromAs<Vector2>(),
                        Delay,
                        Duration,
                        easingFunction,
                        iterationBehavior: iterationBehavior,
                        iterationCount: iterationCount);
                }

                if (typeof(T) == typeof(Vector3))
                {
                    return targetHint.Compositor.CreateVector3KeyFrameAnimation(
                        Property,
                        GetToAs<Vector3>(),
                        GetFromAs<Vector3>(),
                        Delay,
                        Duration,
                        easingFunction,
                        iterationBehavior: iterationBehavior,
                        iterationCount: iterationCount);
                }

                if (typeof(T) == typeof(Vector4))
                {
                    return targetHint.Compositor.CreateVector4KeyFrameAnimation(
                        Property,
                        GetToAs<Vector4>(),
                        GetFromAs<Vector4>(),
                        Delay,
                        Duration,
                        easingFunction,
                        iterationBehavior: iterationBehavior,
                        iterationCount: iterationCount);
                }

                if (typeof(T) == typeof(Color))
                {
                    return targetHint.Compositor.CreateColorKeyFrameAnimation(
                        Property,
                        GetToAs<Color>(),
                        GetFromAs<Color>(),
                        Delay,
                        Duration,
                        easingFunction,
                        iterationBehavior: iterationBehavior,
                        iterationCount: iterationCount);
                }

                if (typeof(T) == typeof(Quaternion))
                {
                    return targetHint.Compositor.CreateQuaternionKeyFrameAnimation(
                        Property,
                        GetToAs<Quaternion>(),
                        GetFromAs<Quaternion>(),
                        Delay,
                        Duration,
                        easingFunction,
                        iterationBehavior: iterationBehavior,
                        iterationCount: iterationCount);
                }

                throw new InvalidOperationException("Invalid animation type");
            }

            /// <inheritdoc/>
            public Timeline GetAnimation(DependencyObject targetHint)
            {
                EasingFunctionBase? easingFunction = EasingType.ToEasingFunction(EasingMode);

                if (typeof(T) == typeof(float))
                {
                    return targetHint.CreateDoubleAnimation(
                        Property,
                        GetToAs<float>(),
                        GetFromAs<float>(),
                        Delay,
                        Duration,
                        easingFunction,
                        Repeat.ToRepeatBehavior(),
                        enableDependecyAnimations: true);
                }

                if (typeof(T) == typeof(double))
                {
                    return targetHint.CreateDoubleAnimation(
                        Property,
                        GetToAs<double>(),
                        GetFromAs<double>(),
                        Delay,
                        Duration,
                        easingFunction,
                        Repeat.ToRepeatBehavior(),
                        enableDependecyAnimations: true);
                }

                if (typeof(T) == typeof(Point))
                {
                    return targetHint.CreatePointAnimation(
                        Property,
                        GetToAs<Point>(),
                        GetFromAs<Point>(),
                        Delay,
                        Duration,
                        easingFunction,
                        Repeat.ToRepeatBehavior(),
                        enableDependecyAnimations: true);
                }

                if (typeof(T) == typeof(Color))
                {
                    return targetHint.CreateColorAnimation(
                        Property,
                        GetToAs<Color>(),
                        GetFromAs<Color>(),
                        Delay,
                        Duration,
                        easingFunction,
                        Repeat.ToRepeatBehavior());
                }

                throw new InvalidOperationException("Invalid animation type");
            }

            /// <summary>
            /// Gets the current target value as <typeparamref name="TValue"/>.
            /// </summary>
            /// <typeparam name="TValue">The target value type to use.</typeparam>
            /// <returns>The target type cast to <typeparamref name="TValue"/>.</returns>
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private TValue GetToAs<TValue>()
                where TValue : unmanaged
            {
                // We employ this (T2)(object)t1 pattern multiple times in this library to alter generics.
                // This is an equivalent but safer alternative to using Unsafe.As<TFrom, TTo>(ref TFrom).
                // For instance, this method will result in the following IL being emitted:
                // =============================
                // IL_0000: ldarg.0
                // IL_0001: call instance !0 class AnimationFactory`1<!T>::get_To()
                // IL_0006: box !T
                // IL_000b: unbox.any !!TValue
                // IL_0010: ret
                // =============================
                // The key point is that the JIT (and AOT compilers such as .NET Native) can recognize this
                // pattern and optimize the boxing away in case the types match. This is the case whenever
                // the generic arguments are value types, which due to generic types in .NET being reified
                // results in a completely different generic instantiation of the same method, making the
                // type arguments effectively constant values known at compile time, ie. at JIT time.
                // As a result of this, the boxing is completely avoided and the value is returned directly.
                // Leveraging this pattern lets us keep the same optimal codegen while avoiding the extra
                // NuGet package dependency on UWP, and the more dangerous path using the Unsafe APIs.
                // As an example, assuming T is float, the JIT will produce the following codegen on x64:
                // =============================
                // L0000: vzeroupper
                // L0003: vmovss xmm0, [rcx+8]
                // L0008: ret
                // =============================
                // We can see how the property value is loaded directly from the underlying field and
                // then returned to the caller: no boxing or unwanted overhead is introduced at all.
                return (TValue)(object)To;
            }

            /// <summary>
            /// Gets the current starting value as <typeparamref name="TValue"/>.
            /// </summary>
            /// <typeparam name="TValue">The starting value type to use.</typeparam>
            /// <returns>The starting type cast to nullable <typeparamref name="TValue"/>.</returns>
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private TValue? GetFromAs<TValue>()
                where TValue : unmanaged
            {
                if (From is null)
                {
                    return null;
                }

                T from = From.GetValueOrDefault();

                return (TValue)(object)from;
            }
        }

        /// <summary>
        /// A model representing a specified composition scalar animation factory targeting a clip.
        /// </summary>
        private sealed record CompositionClipScalarAnimation(
            string Property,
            float To,
            float? From,
            TimeSpan Delay,
            TimeSpan Duration,
            RepeatOption Repeat,
            EasingType EasingType,
            EasingMode EasingMode)
            : ICompositionAnimationFactory
        {
            /// <inheritdoc/>
            public CompositionAnimation GetAnimation(CompositionObject targetHint, out CompositionObject? target)
            {
                Visual visual = (Visual)targetHint;
                InsetClip clip = visual.Clip as InsetClip ?? (InsetClip)(visual.Clip = visual.Compositor.CreateInsetClip());
                CompositionEasingFunction? easingFunction = clip.Compositor.TryCreateEasingFunction(EasingType, EasingMode);
                (AnimationIterationBehavior iterationBehavior, int iterationCount) = Repeat.ToBehaviorAndCount();
                ScalarKeyFrameAnimation animation = clip.Compositor.CreateScalarKeyFrameAnimation(
                    Property,
                    To,
                    From,
                    Delay,
                    Duration,
                    easingFunction,
                    iterationBehavior: iterationBehavior,
                    iterationCount: iterationCount);

                target = clip;

                return animation;
            }
        }

        /// <summary>
        /// A model representing a specified XAML <see cref="double"/> animation factory targeting a transform.
        /// </summary>
        private sealed record XamlTransformDoubleAnimationFactory(
            string Property,
            double To,
            double? From,
            TimeSpan Delay,
            TimeSpan Duration,
            RepeatOption Repeat,
            EasingType EasingType,
            EasingMode EasingMode)
            : IXamlAnimationFactory
        {
            /// <inheritdoc/>
            public Timeline GetAnimation(DependencyObject targetHint)
            {
                UIElement element = (UIElement)targetHint;

                if (element.RenderTransform is not CompositeTransform transform)
                {
                    element.RenderTransform = transform = new CompositeTransform();
                }

                return transform.CreateDoubleAnimation(
                    Property,
                    To,
                    From,
                    Duration,
                    Delay,
                    EasingType.ToEasingFunction(EasingMode),
                    Repeat.ToRepeatBehavior());
            }
        }

        /// <summary>
        /// A model representing an external composition animation with an optional target <see cref="CompositionObject"/>.
        /// </summary>
        private sealed record ExternalCompositionAnimation(CompositionObject? Target, CompositionAnimation Animation) : ICompositionAnimationFactory
        {
            /// <inheritdoc/>
            public CompositionAnimation GetAnimation(CompositionObject targetHint, out CompositionObject? target)
            {
                target = Target;

                return Animation;
            }
        }

        /// <summary>
        /// A model representing an external composition animation with an optional target <see cref="CompositionObject"/>.
        /// </summary>
        private sealed record ExternalXamlAnimation(Timeline Animation) : IXamlAnimationFactory
        {
            /// <inheritdoc/>
            public Timeline GetAnimation(DependencyObject targetHint)
            {
                return Animation;
            }
        }
    }
}