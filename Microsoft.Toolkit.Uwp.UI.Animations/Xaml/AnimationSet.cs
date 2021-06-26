// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// A collection of animations that can be grouped together. This type represents a composite animation
    /// (such as <see cref="Windows.UI.Xaml.Media.Animation.Storyboard"/>) that can be executed on a given element.
    /// </summary>
    public sealed class AnimationSet : DependencyObjectCollection
    {
        /// <summary>
        /// A conditional weak table storing <see cref="CancellationTokenSource"/> instances associated with animations
        /// that have been started from the current set. This can be used to defer stopping running animations for any
        /// target <see cref="UIElement"/> instance that originated from the current <see cref="AnimationSet"/>.
        /// </summary>
        private readonly ConditionalWeakTable<UIElement, CancellationTokenSource> cancellationTokenMap = new();

        /// <summary>
        /// Raised whenever the current animation is started.
        /// </summary>
        public event EventHandler? Started;

        /// <summary>
        /// Raised whenever the current animation completes.
        /// </summary>
        public event EventHandler? Completed;

        /// <summary>
        /// Gets or sets a value indicating whether top level animation nodes in this collection are invoked
        /// sequentially. This applies to both <see cref="AnimationScope"/> nodes (which will still trigger
        /// contained animations at the same time), and other top level animation nodes. The default value
        /// is <see langword="false"/>, which means that all contained animations will start at the same time.
        /// <para>
        /// Note that this property will also cause a change in behavior for the animation. With the default
        /// configuration, with all animations starting at the same time, it's not possible to use multiple
        /// animations targeting the same property (as they'll cause a conflict and be ignored when on the
        /// composition layer, or cause a crash when on the XAML layer). When animations are started sequentially
        /// instead, each sequential block will be able to share target properties with animations from other
        /// sequential blocks, without issues. Note that especially for simple scenarios (eg. an opacity animation
        /// that just transitions to a state and then back, or between two states), it is recommended to use a single
        /// keyframe animation instead, which will result in less overhead when creating and starting the animation.
        /// </para>
        /// </summary>
        public bool IsSequential { get; set; }

        /// <summary>
        /// Gets or sets the weak reference to the parent that owns the current animation collection.
        /// </summary>
        internal WeakReference<UIElement>? ParentReference { get; set; }

        /// <inheritdoc cref="AnimationBuilder.Start(UIElement)"/>
        /// <exception cref="InvalidOperationException">Thrown when there is no attached <see cref="UIElement"/> instance.</exception>
        public async void Start()
        {
            // Here we're using an async void method on purpose, in order to be able to await
            // the completion of the animation and rethrow exceptions. We can't just use the
            // synchronous AnimationBuilder.Start method here, as we also need to await for the
            // animation to complete in either case in order to raise the Completed event when that
            // happens. So we add an async state machine here to work around this.
            await StartAsync();
        }

        /// <inheritdoc cref="AnimationBuilder.Start(UIElement)"/>
        public async void Start(UIElement element)
        {
            await StartAsync(element);
        }

        /// <inheritdoc cref="AnimationBuilder.Start(UIElement)"/>
        /// <exception cref="InvalidOperationException">Thrown when there is no attached <see cref="UIElement"/> instance.</exception>
        public async void Start(CancellationToken token)
        {
            await StartAsync(token);
        }

        /// <inheritdoc cref="AnimationBuilder.Start(UIElement)"/>
        /// <exception cref="InvalidOperationException">Thrown when there is no attached <see cref="UIElement"/> instance.</exception>
        public Task StartAsync()
        {
            return StartAsync(GetParent());
        }

        /// <inheritdoc cref="AnimationBuilder.Start(UIElement)"/>
        public Task StartAsync(UIElement element)
        {
            Stop(element);

            CancellationTokenSource cancellationTokenSource = new();

            this.cancellationTokenMap.AddOrUpdate(element, cancellationTokenSource);

            return StartAsync(element, cancellationTokenSource.Token);
        }

        /// <inheritdoc cref="AnimationBuilder.Start(UIElement)"/>
        /// <exception cref="InvalidOperationException">Thrown when there is no attached <see cref="UIElement"/> instance.</exception>
        public Task StartAsync(CancellationToken token)
        {
            return StartAsync(GetParent(), token);
        }

        /// <inheritdoc cref="AnimationBuilder.Start(UIElement)"/>
        public async Task StartAsync(UIElement element, CancellationToken token)
        {
            Started?.Invoke(this, EventArgs.Empty);

            if (IsSequential)
            {
                foreach (object node in this)
                {
                    if (node is ITimeline timeline)
                    {
                        var builder = AnimationBuilder.Create();

                        timeline.AppendToBuilder(builder);

                        await builder.StartAsync(element, token);
                    }
                    else if (node is IActivity activity)
                    {
                        try
                        {
                            // Unlike with animations, activities can potentially throw if they execute
                            // an await operation on a task that was linked to a cancellation token. For
                            // instance, this is the case for the await operation for the initial delay,
                            // and the same can apply to 3rd party activities that would just integrate
                            // the input token into their logic. We can just catch these exceptions and
                            // stop the sequential execution immediately from the handler.
                            await activity.InvokeAsync(element, token);
                        }
                        catch (OperationCanceledException)
                        {
                            break;
                        }
                    }
                    else
                    {
                        ThrowArgumentException();
                    }

                    // This should in theory only be necessary in the timeline branch, but doing this check
                    // after running activities too help guard against 3rd party activities that might not
                    // properly monitor the token being in use, and still run fine after a cancellation.
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }
            else
            {
                var builder = AnimationBuilder.Create();

                foreach (object node in this)
                {
                    switch (node)
                    {
                        case ITimeline timeline:
                            builder = timeline.AppendToBuilder(builder);
                            break;
                        case IActivity activity:
                            _ = activity.InvokeAsync(element, token);
                            break;
                        default:
                            ThrowArgumentException();
                            break;
                    }
                }

                await builder.StartAsync(element, token);
            }

            Completed?.Invoke(this, EventArgs.Empty);

            static void ThrowArgumentException() => throw new ArgumentException($"An animation set can only contain nodes implementing either ITimeline or IActivity");
        }

        /// <summary>
        /// Cancels the current animation on the attached <see cref="UIElement"/> instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when there is no attached <see cref="UIElement"/> instance.</exception>
        public void Stop()
        {
            Stop(GetParent());
        }

        /// <summary>
        ///  Cancels the current animation for a target <see cref="UIElement"/> instance.
        /// </summary>
        /// <param name="element">The target <see cref="UIElement"/> instance to stop the animation for.</param>
        public void Stop(UIElement element)
        {
            if (this.cancellationTokenMap.TryGetValue(element, out CancellationTokenSource value))
            {
                value.Cancel();
            }
        }

        /// <summary>
        /// Gets the current parent <see cref="UIElement"/> instance.
        /// </summary>
        /// <returns>The <see cref="UIElement"/> reference from <see cref="ParentReference"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown if there is no parent available.</exception>
        [Pure]
        private UIElement GetParent()
        {
            UIElement? parent = null;

            if (ParentReference?.TryGetTarget(out parent) != true)
            {
                ThrowInvalidOperationException();
            }

            return parent!;

            static void ThrowInvalidOperationException() => throw new InvalidOperationException("The current AnimationSet object isn't bound to a parent UIElement instance.");
        }
    }
}