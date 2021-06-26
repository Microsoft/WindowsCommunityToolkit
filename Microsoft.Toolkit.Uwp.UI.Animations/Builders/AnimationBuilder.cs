// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Animation;

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// A <see langword="class"/> that allows to build custom animations targeting both the XAML and composition layers.
    /// </summary>
    public sealed partial class AnimationBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationBuilder"/> class.
        /// </summary>
        /// <remarks>This is private as the public entry point is the <see cref="Create"/> method.</remarks>
        private AnimationBuilder()
        {
        }

        /// <summary>
        /// <para>
        /// Creates a new <see cref="AnimationBuilder"/> instance to setup an animation schedule.
        /// This can be used as the entry point to construct a custom animation sequence.
        /// </para>
        /// For instance:
        /// <code>
        /// AnimationBuilder.Create()<br/>
        ///     .Opacity(from: 0, to: 1)<br/>
        ///     .Translation(Axis.X, from: -40, to: 0)<br/>
        ///     .Start(MyButton);
        /// </code>
        /// <para>
        /// Configured <see cref="AnimationBuilder"/> instances are also reusable, meaning that the same
        /// one can be used to start an animation sequence on multiple elements as well.
        /// </para>
        /// For instance:
        /// <code>
        /// var animation = AnimationBuilder.Create().Opacity(0, 1).Size(1.2, 1);<br/>
        /// <br/>
        /// animation.Start(MyButton);<br/>
        /// animation.Start(MyGrid);
        /// </code>
        /// Alternatively, the <see cref="AnimationSet"/> type can be used to configure animations directly from XAML.
        /// The same <see cref="AnimationBuilder"/> APIs will still be used behind the scenes to handle animations.
        /// </summary>
        /// <returns>An empty <see cref="AnimationBuilder"/> instance to use to construct an animation sequence.</returns>
        [Pure]
        public static AnimationBuilder Create() => new();

        /// <summary>
        /// Starts the animations present in the current <see cref="AnimationBuilder"/> instance.
        /// </summary>
        /// <param name="element">The target <see cref="UIElement"/> to animate.</param>
        public void Start(UIElement element)
        {
            if (this.compositionAnimationFactories.Count > 0)
            {
                ElementCompositionPreview.SetIsTranslationEnabled(element, true);

                Visual visual = ElementCompositionPreview.GetElementVisual(element);

                foreach (var factory in this.compositionAnimationFactories)
                {
                    var animation = factory.GetAnimation(visual, out var target);

                    if (target is null)
                    {
                        visual.StartAnimation(animation.Target, animation);
                    }
                    else
                    {
                        target.StartAnimation(animation.Target, animation);
                    }
                }
            }

            if (this.xamlAnimationFactories.Count > 0)
            {
                Storyboard storyboard = new();

                foreach (var factory in this.xamlAnimationFactories)
                {
                    storyboard.Children.Add(factory.GetAnimation(element));
                }

                storyboard.Begin();
            }
        }

        /// <summary>
        /// Starts the animations present in the current <see cref="AnimationBuilder"/> instance.
        /// </summary>
        /// <param name="element">The target <see cref="UIElement"/> to animate.</param>
        /// <param name="callback">The callback to invoke when the animation completes.</param>
        public void Start(UIElement element, Action callback)
        {
            // The point of this overload is to allow consumers to invoke a callback when an animation
            // completes, without having to create an async state machine. There are three different possible
            // scenarios to handle, and each can have a specialized code path to ensure the implementation
            // is as lean and efficient as possible. Specifically, for a given AnimationBuilder instance:
            //   1) There are only Composition animations
            //   2) There are only XAML animations
            //   3) There are both Composition and XAML animations
            // The implementation details of each of these paths is described below.
            if (this.compositionAnimationFactories.Count > 0)
            {
                if (this.xamlAnimationFactories.Count == 0)
                {
                    // There are only Composition animations. In this case we can just use a Composition scoped batch,
                    // capture the user-provided callback and invoke it directly when the batch completes. There is no
                    // additional overhead here, since we would've had to create a closure regardless to be able to monitor
                    // the completion of the animation (eg. to capture a TaskCompletionSource like we're doing below).
                    static void Start(AnimationBuilder builder, UIElement element, Action callback)
                    {
                        ElementCompositionPreview.SetIsTranslationEnabled(element, true);

                        Visual visual = ElementCompositionPreview.GetElementVisual(element);
                        CompositionScopedBatch batch = visual.Compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

                        batch.Completed += (_, _) => callback();

                        foreach (var factory in builder.compositionAnimationFactories)
                        {
                            var animation = factory.GetAnimation(visual, out var target);

                            if (target is null)
                            {
                                visual.StartAnimation(animation.Target, animation);
                            }
                            else
                            {
                                target.StartAnimation(animation.Target, animation);
                            }
                        }

                        batch.End();
                    }

                    Start(this, element, callback);
                }
                else
                {
                    // In this case we need to wait for both the Composition and XAML animation groups to complete. These two
                    // groups use different APIs and can have a different duration, so we need to synchronize between them
                    // without creating an async state machine (as that'd defeat the point of this separate overload).
                    //
                    // The code below relies on a mutable boxed counter that's shared across the two closures for the Completed
                    // events for both the Composition scoped batch and the XAML Storyboard. The counter is initialized to 2, and
                    // when each group completes, the counter is decremented (we don't need an interlocked decrement as the delegates
                    // will already be invoked on the current DispatcherQueue instance, which acts as the synchronization context here.
                    // The handlers for the Composition batch and the Storyboard will never execute concurrently). If the counter has
                    // reached zero, it means that both groups have completed, so the user-provided callback is triggered, otherwise
                    // the handler just does nothing. This ensures that the callback is executed exactly once when all the animation
                    // complete, but without the need to create TaskCompletionSource-s and an async state machine to await for that.
                    //
                    // Note: we're using StrongBox<T> here because that exposes a mutable field of the type we need (int).
                    // We can't just mutate a boxed int in-place with Unsafe.Unbox<T> as that's against the ECMA spec, since
                    // that API uses the unbox IL opcode (§III.4.32) which returns a "controlled-mutability managed pointer"
                    // (§III.1.8.1.2.2), which is not "verifier-assignable-to" (ie. directly assigning to it is not legal).
                    static void Start(AnimationBuilder builder, UIElement element, Action callback)
                    {
                        StrongBox<int> counter = new(2);

                        ElementCompositionPreview.SetIsTranslationEnabled(element, true);

                        Visual visual = ElementCompositionPreview.GetElementVisual(element);
                        CompositionScopedBatch batch = visual.Compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

                        batch.Completed += (_, _) =>
                        {
                            if (--counter.Value == 0)
                            {
                                callback();
                            }
                        };

                        foreach (var factory in builder.compositionAnimationFactories)
                        {
                            var animation = factory.GetAnimation(visual, out var target);

                            if (target is null)
                            {
                                visual.StartAnimation(animation.Target, animation);
                            }
                            else
                            {
                                target.StartAnimation(animation.Target, animation);
                            }
                        }

                        batch.End();

                        Storyboard storyboard = new();

                        foreach (var factory in builder.xamlAnimationFactories)
                        {
                            storyboard.Children.Add(factory.GetAnimation(element));
                        }

                        storyboard.Completed += (_, _) =>
                        {
                            if (--counter.Value == 0)
                            {
                                callback();
                            }
                        };
                        storyboard.Begin();
                    }

                    Start(this, element, callback);
                }
            }
            else
            {
                // There are only XAML animations. This case is extremely similar to that where we only have Composition
                // animations, with the main difference being that the Completed event is directly exposed from the
                // Storyboard type, so we don't need a separate type to track the animation completion. The same
                // considerations regarding the closure to capture the provided callback apply here as well.
                static void Start(AnimationBuilder builder, UIElement element, Action callback)
                {
                    Storyboard storyboard = new();

                    foreach (var factory in builder.xamlAnimationFactories)
                    {
                        storyboard.Children.Add(factory.GetAnimation(element));
                    }

                    storyboard.Completed += (_, _) => callback();
                    storyboard.Begin();
                }

                Start(this, element, callback);
            }
        }

        /// <summary>
        /// Starts the animations present in the current <see cref="AnimationBuilder"/> instance, and
        /// registers a given cancellation token to stop running animations before they complete.
        /// </summary>
        /// <param name="element">The target <see cref="UIElement"/> to animate.</param>
        /// <param name="token">The cancellation token to stop animations while they're running.</param>
        public void Start(UIElement element, CancellationToken token)
        {
            List<(CompositionObject Target, string Path)>? compositionAnimations = null;

            if (this.compositionAnimationFactories.Count > 0)
            {
                compositionAnimations = new List<(CompositionObject Target, string Path)>(this.compositionAnimationFactories.Count);

                ElementCompositionPreview.SetIsTranslationEnabled(element, true);

                Visual visual = ElementCompositionPreview.GetElementVisual(element);

                foreach (var factory in this.compositionAnimationFactories)
                {
                    var animation = factory.GetAnimation(visual, out var target);

                    if (target is null)
                    {
                        visual.StartAnimation(animation.Target, animation);
                    }
                    else
                    {
                        target.StartAnimation(animation.Target, animation);
                    }

                    compositionAnimations.Add((target ?? visual, animation.Target));
                }
            }

            Storyboard? storyboard = null;

            if (this.xamlAnimationFactories.Count > 0)
            {
                storyboard = new Storyboard();

                foreach (var factory in this.xamlAnimationFactories)
                {
                    storyboard.Children.Add(factory.GetAnimation(element));
                }

                storyboard.Begin();
            }

            static void Stop(object state)
            {
                (List<(CompositionObject Target, string Path)>? animations, Storyboard? storyboard) = ((List<(CompositionObject, string)>?, Storyboard?))state;

                if (animations is not null)
                {
                    foreach (var (target, path) in animations)
                    {
                        target.StopAnimation(path);
                    }
                }

                storyboard?.Stop();
            }

            token.Register(static obj => Stop(obj), (compositionAnimations, storyboard));
        }

        /// <summary>
        /// Starts the animations present in the current <see cref="AnimationBuilder"/> instance.
        /// </summary>
        /// <param name="element">The target <see cref="UIElement"/> to animate.</param>
        /// <returns>A <see cref="Task"/> that completes when all animations have completed.</returns>
        public Task StartAsync(UIElement element)
        {
            Task
                compositionTask = Task.CompletedTask,
                xamlTask = Task.CompletedTask;

            if (this.compositionAnimationFactories.Count > 0)
            {
                ElementCompositionPreview.SetIsTranslationEnabled(element, true);

                Visual visual = ElementCompositionPreview.GetElementVisual(element);
                CompositionScopedBatch batch = visual.Compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                TaskCompletionSource<object?> taskCompletionSource = new();

                batch.Completed += (_, _) => taskCompletionSource.SetResult(null);

                foreach (var factory in this.compositionAnimationFactories)
                {
                    var animation = factory.GetAnimation(visual, out var target);

                    if (target is null)
                    {
                        visual.StartAnimation(animation.Target, animation);
                    }
                    else
                    {
                        target.StartAnimation(animation.Target, animation);
                    }
                }

                batch.End();

                compositionTask = taskCompletionSource.Task;
            }

            if (this.xamlAnimationFactories.Count > 0)
            {
                Storyboard storyboard = new();
                TaskCompletionSource<object?> taskCompletionSource = new();

                foreach (var factory in this.xamlAnimationFactories)
                {
                    storyboard.Children.Add(factory.GetAnimation(element));
                }

                storyboard.Completed += (_, _) => taskCompletionSource.SetResult(null);
                storyboard.Begin();

                xamlTask = taskCompletionSource.Task;
            }

            return Task.WhenAll(compositionTask, xamlTask);
        }

        /// <summary>
        /// Starts the animations present in the current <see cref="AnimationBuilder"/> instance, and
        /// registers a given cancellation token to stop running animations before they complete.
        /// </summary>
        /// <param name="element">The target <see cref="UIElement"/> to animate.</param>
        /// <param name="token">The cancellation token to stop animations while they're running.</param>
        /// <returns>A <see cref="Task"/> that completes when all animations have completed.</returns>
        public Task StartAsync(UIElement element, CancellationToken token)
        {
            Task
                compositionTask = Task.CompletedTask,
                xamlTask = Task.CompletedTask;
            List<(CompositionObject Target, string Path)>? compositionAnimations = null;

            if (this.compositionAnimationFactories.Count > 0)
            {
                compositionAnimations = new List<(CompositionObject Target, string Path)>(this.compositionAnimationFactories.Count);

                ElementCompositionPreview.SetIsTranslationEnabled(element, true);

                Visual visual = ElementCompositionPreview.GetElementVisual(element);
                CompositionScopedBatch batch = visual.Compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                TaskCompletionSource<object?> taskCompletionSource = new();

                batch.Completed += (_, _) => taskCompletionSource.SetResult(null);

                foreach (var factory in this.compositionAnimationFactories)
                {
                    var animation = factory.GetAnimation(visual, out var target);

                    if (target is null)
                    {
                        visual.StartAnimation(animation.Target, animation);
                    }
                    else
                    {
                        target.StartAnimation(animation.Target, animation);
                    }

                    compositionAnimations.Add((target ?? visual, animation.Target));
                }

                batch.End();

                compositionTask = taskCompletionSource.Task;
            }

            Storyboard? storyboard = null;

            if (this.xamlAnimationFactories.Count > 0)
            {
                storyboard = new Storyboard();

                TaskCompletionSource<object?> taskCompletionSource = new();

                foreach (var factory in this.xamlAnimationFactories)
                {
                    storyboard.Children.Add(factory.GetAnimation(element));
                }

                storyboard.Completed += (_, _) => taskCompletionSource.SetResult(null);
                storyboard.Begin();

                xamlTask = taskCompletionSource.Task;
            }

            static void Stop(object state)
            {
                (List<(CompositionObject Target, string Path)>? animations, Storyboard? storyboard) = ((List<(CompositionObject, string)>?, Storyboard?))state;

                if (animations is not null)
                {
                    foreach (var (target, path) in animations)
                    {
                        target.StopAnimation(path);
                    }
                }

                storyboard?.Stop();
            }

            token.Register(static obj => Stop(obj), (compositionAnimations, storyboard));

            return Task.WhenAll(compositionTask, xamlTask);
        }
    }
}