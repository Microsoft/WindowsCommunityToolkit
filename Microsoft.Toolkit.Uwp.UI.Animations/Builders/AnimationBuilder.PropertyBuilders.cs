// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using static Microsoft.Toolkit.Uwp.UI.Animations.AnimationExtensions;

#nullable enable

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <inheritdoc cref="AnimationBuilder"/>
    public sealed partial class AnimationBuilder
    {
        /// <summary>
        /// A custom <see cref="IPropertyAnimationBuilder{T}"/> for a shared animation.
        /// </summary>
        /// <typeparam name="T">The type of property to animate.</typeparam>
        private sealed record PropertyAnimationBuilder<T>(
            AnimationBuilder Builder,
            string Property,
            FrameworkLayer Layer)
            : IPropertyAnimationBuilder<T>
            where T : unmanaged
        {
            /// <inheritdoc/>
            public AnimationBuilder NormalizedKeyFrames(
                Action<INormalizedKeyFrameAnimationBuilder<T>> build,
                TimeSpan? delay,
                TimeSpan? duration,
                RepeatOption? repeatOption,
                AnimationDelayBehavior? delayBehavior)
            {
                return Builder.NormalizedKeyFrames(Property, build, delay, duration, repeatOption, delayBehavior, Layer);
            }

            /// <inheritdoc/>
            public AnimationBuilder NormalizedKeyFrames<TState>(
                TState state,
                Action<INormalizedKeyFrameAnimationBuilder<T>, TState> build,
                TimeSpan? delay,
                TimeSpan? duration,
                RepeatOption? repeatOption,
                AnimationDelayBehavior? delayBehavior)
            {
                return Builder.NormalizedKeyFrames(Property, state, build, delay, duration, repeatOption, delayBehavior, Layer);
            }

            /// <inheritdoc/>
            public AnimationBuilder TimedKeyFrames(
                Action<ITimedKeyFrameAnimationBuilder<T>> build,
                TimeSpan? delay,
                RepeatOption? repeatOption,
                AnimationDelayBehavior? delayBehavior)
            {
                return Builder.TimedKeyFrames(Property, build, delay, repeatOption, delayBehavior, Layer);
            }

            /// <inheritdoc/>
            public AnimationBuilder TimedKeyFrames<TState>(
                TState state,
                Action<ITimedKeyFrameAnimationBuilder<T>, TState> build,
                TimeSpan? delay,
                RepeatOption? repeatOption,
                AnimationDelayBehavior? delayBehavior)
            {
                return Builder.TimedKeyFrames(Property, state, build, delay, repeatOption, delayBehavior, Layer);
            }
        }

        /// <summary>
        /// A custom <see cref="IPropertyAnimationBuilder{T}"/> for a composition clip animation.
        /// </summary>
        private sealed record CompositionClipAnimationBuilder(
            AnimationBuilder Builder,
            string Property)
            : IPropertyAnimationBuilder<double>
        {
            /// <inheritdoc/>
            public AnimationBuilder NormalizedKeyFrames(
                Action<INormalizedKeyFrameAnimationBuilder<double>> build,
                TimeSpan? delay,
                TimeSpan? duration,
                RepeatOption? repeatOption,
                AnimationDelayBehavior? delayBehavior)
            {
                NormalizedKeyFrameAnimationBuilder<double>.Composition builder = new(
                    Property,
                    delay,
                    duration ?? DefaultDuration,
                    repeatOption ?? RepeatOption.Once,
                    delayBehavior ?? DefaultDelayBehavior);

                build(builder);

                Builder.compositionAnimationFactories.Add(new Factory(builder));

                return Builder;
            }

            /// <inheritdoc/>
            public AnimationBuilder NormalizedKeyFrames<TState>(
                TState state,
                Action<INormalizedKeyFrameAnimationBuilder<double>, TState> build,
                TimeSpan? delay,
                TimeSpan? duration,
                RepeatOption? repeatOption,
                AnimationDelayBehavior? delayBehavior)
            {
                NormalizedKeyFrameAnimationBuilder<double>.Composition builder = new(
                    Property,
                    delay,
                    duration ?? DefaultDuration,
                    repeatOption ?? RepeatOption.Once,
                    delayBehavior ?? DefaultDelayBehavior);

                build(builder, state);

                Builder.compositionAnimationFactories.Add(new Factory(builder));

                return Builder;
            }

            /// <inheritdoc/>
            public AnimationBuilder TimedKeyFrames(
                Action<ITimedKeyFrameAnimationBuilder<double>> build,
                TimeSpan? delay,
                RepeatOption? repeatOption,
                AnimationDelayBehavior? delayBehavior)
            {
                TimedKeyFrameAnimationBuilder<double>.Composition builder = new(
                    Property,
                    delay,
                    repeatOption ?? RepeatOption.Once,
                    delayBehavior ?? DefaultDelayBehavior);

                build(builder);

                Builder.compositionAnimationFactories.Add(new Factory(builder));

                return Builder;
            }

            /// <inheritdoc/>
            public AnimationBuilder TimedKeyFrames<TState>(
                TState state,
                Action<ITimedKeyFrameAnimationBuilder<double>, TState> build,
                TimeSpan? delay,
                RepeatOption? repeatOption,
                AnimationDelayBehavior? delayBehavior)
            {
                TimedKeyFrameAnimationBuilder<double>.Composition builder = new(
                    Property,
                    delay,
                    repeatOption ?? RepeatOption.Once,
                    delayBehavior ?? DefaultDelayBehavior);

                build(builder, state);

                Builder.compositionAnimationFactories.Add(new Factory(builder));

                return Builder;
            }

            /// <summary>
            /// A private factory implementing <see cref="ICompositionAnimationFactory"/>.
            /// </summary>
            private sealed record Factory(ICompositionAnimationFactory Builder) : ICompositionAnimationFactory
            {
                /// <inheritdoc/>
                public CompositionAnimation GetAnimation(CompositionObject targetHint, out CompositionObject? target)
                {
                    Visual visual = (Visual)targetHint;
                    InsetClip clip = visual.Clip as InsetClip ?? (InsetClip)(visual.Clip = visual.Compositor.CreateInsetClip());
                    CompositionAnimation animation = Builder.GetAnimation(clip, out _);

                    target = clip;

                    return animation;
                }
            }
        }

        /// <summary>
        /// A custom <see cref="IPropertyAnimationBuilder{T}"/> for a XAML transform animation.
        /// </summary>
        private sealed record XamlTransformPropertyAnimationBuilder(
            AnimationBuilder Builder,
            string Property)
            : IPropertyAnimationBuilder<double>
        {
            /// <inheritdoc/>
            public AnimationBuilder NormalizedKeyFrames(
                Action<INormalizedKeyFrameAnimationBuilder<double>> build,
                TimeSpan? delay,
                TimeSpan? duration,
                RepeatOption? repeatOption,
                AnimationDelayBehavior? _)
            {
                NormalizedKeyFrameAnimationBuilder<double>.Xaml builder = new(
                    Property,
                    delay,
                    duration ?? DefaultDuration,
                    repeatOption ?? RepeatOption.Once);

                build(builder);

                Builder.xamlAnimationFactories.Add(new Factory(builder));

                return Builder;
            }

            /// <inheritdoc/>
            public AnimationBuilder NormalizedKeyFrames<TState>(
                TState state,
                Action<INormalizedKeyFrameAnimationBuilder<double>, TState> build,
                TimeSpan? delay,
                TimeSpan? duration,
                RepeatOption? repeatOption,
                AnimationDelayBehavior? _)
            {
                NormalizedKeyFrameAnimationBuilder<double>.Xaml builder = new(
                    Property,
                    delay,
                    duration ?? DefaultDuration,
                    repeatOption ?? RepeatOption.Once);

                build(builder, state);

                Builder.xamlAnimationFactories.Add(new Factory(builder));

                return Builder;
            }

            /// <inheritdoc/>
            public AnimationBuilder TimedKeyFrames(
                Action<ITimedKeyFrameAnimationBuilder<double>> build,
                TimeSpan? delay,
                RepeatOption? repeatOption,
                AnimationDelayBehavior? _)
            {
                TimedKeyFrameAnimationBuilder<double>.Xaml builder = new(Property, delay, repeatOption ?? RepeatOption.Once);

                build(builder);

                Builder.xamlAnimationFactories.Add(new Factory(builder));

                return Builder;
            }

            /// <inheritdoc/>
            public AnimationBuilder TimedKeyFrames<TState>(
                TState state,
                Action<ITimedKeyFrameAnimationBuilder<double>, TState> build,
                TimeSpan? delay,
                RepeatOption? repeatOption,
                AnimationDelayBehavior? _)
            {
                TimedKeyFrameAnimationBuilder<double>.Xaml builder = new(Property, delay, repeatOption ?? RepeatOption.Once);

                build(builder, state);

                Builder.xamlAnimationFactories.Add(new Factory(builder));

                return Builder;
            }

            /// <summary>
            /// A private factory implementing <see cref="IXamlAnimationFactory"/>.
            /// </summary>
            private sealed record Factory(IXamlAnimationFactory Builder) : IXamlAnimationFactory
            {
                /// <inheritdoc/>
                public Timeline GetAnimation(DependencyObject targetHint)
                {
                    UIElement element = (UIElement)targetHint;

                    if (element.RenderTransform is not CompositeTransform transform)
                    {
                        element.RenderTransform = transform = new CompositeTransform();
                    }

                    return Builder.GetAnimation(transform);
                }
            }
        }
    }
}