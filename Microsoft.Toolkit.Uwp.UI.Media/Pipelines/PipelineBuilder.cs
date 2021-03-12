// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Windows.Graphics.Effects;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace Microsoft.Toolkit.Uwp.UI.Media.Pipelines
{
    /// <summary>
    /// A <see langword="delegate"/> that represents a custom effect property setter that can be applied to a <see cref="CompositionBrush"/>
    /// </summary>
    /// <typeparam name="T">The type of property value to set</typeparam>
    /// <param name="brush">The target <see cref="CompositionBrush"/> instance to target</param>
    /// <param name="value">The property value to set</param>
    public delegate void EffectSetter<in T>(CompositionBrush brush, T value)
        where T : unmanaged;

    /// <summary>
    /// A <see langword="delegate"/> that represents a custom effect property animation that can be applied to a <see cref="CompositionBrush"/>
    /// </summary>
    /// <typeparam name="T">The type of property value to animate</typeparam>
    /// <param name="brush">The target <see cref="CompositionBrush"/> instance to use to start the animation</param>
    /// <param name="value">The animation target value</param>
    /// <param name="duration">The animation duration</param>
    /// <returns>A <see cref="Task"/> that completes when the target animation completes</returns>
    public delegate Task EffectAnimation<in T>(CompositionBrush brush, T value, TimeSpan duration)
        where T : unmanaged;

    /// <summary>
    /// A <see langword="class"/> that allows to build custom effects pipelines and create <see cref="CompositionBrush"/> instances from them
    /// </summary>
    public sealed partial class PipelineBuilder
    {
        /// <summary>
        /// The <see cref="Func{TResult}"/> instance used to produce the output <see cref="IGraphicsEffectSource"/> for this pipeline
        /// </summary>
        private readonly Func<ValueTask<IGraphicsEffectSource>> sourceProducer;

        /// <summary>
        /// The collection of animation properties present in the current pipeline
        /// </summary>
        private readonly IReadOnlyCollection<string> animationProperties;

        /// <summary>
        /// The collection of info on the parameters that need to be initialized after creating the final <see cref="CompositionBrush"/>
        /// </summary>
        private readonly IReadOnlyDictionary<string, Func<ValueTask<CompositionBrush>>> lazyParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineBuilder"/> class.
        /// </summary>
        /// <param name="factory">A <see cref="Func{TResult}"/> instance that will return the initial <see cref="CompositionBrush"/></param>
        private PipelineBuilder(Func<ValueTask<CompositionBrush>> factory)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            this.sourceProducer = () => new ValueTask<IGraphicsEffectSource>(new CompositionEffectSourceParameter(id));
            this.animationProperties = Array.Empty<string>();
            this.lazyParameters = new Dictionary<string, Func<ValueTask<CompositionBrush>>> { { id, factory } };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineBuilder"/> class.
        /// </summary>
        /// <param name="factory">A <see cref="Func{TResult}"/> instance that will return the initial <see cref="IGraphicsEffectSource"/></param>
        private PipelineBuilder(Func<ValueTask<IGraphicsEffectSource>> factory)
            : this(
                factory,
                Array.Empty<string>(),
                new Dictionary<string, Func<ValueTask<CompositionBrush>>>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineBuilder"/> class.
        /// </summary>
        /// <param name="factory">A <see cref="Func{TResult}"/> instance that will produce the new <see cref="IGraphicsEffectSource"/> to add to the pipeline</param>
        /// <param name="animations">The collection of animation properties for the new effect</param>
        private PipelineBuilder(
            Func<ValueTask<IGraphicsEffectSource>> factory,
            IReadOnlyCollection<string> animations)
            : this(
                factory,
                animations,
                new Dictionary<string, Func<ValueTask<CompositionBrush>>>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineBuilder"/> class.
        /// </summary>
        /// <param name="factory">A <see cref="Func{TResult}"/> instance that will produce the new <see cref="IGraphicsEffectSource"/> to add to the pipeline</param>
        /// <param name="animations">The collection of animation properties for the new effect</param>
        /// <param name="lazy">The collection of <see cref="CompositionBrush"/> instances that needs to be initialized for the new effect</param>
        private PipelineBuilder(
            Func<ValueTask<IGraphicsEffectSource>> factory,
            IReadOnlyCollection<string> animations,
            IReadOnlyDictionary<string, Func<ValueTask<CompositionBrush>>> lazy)
        {
            this.sourceProducer = factory;
            this.animationProperties = animations;
            this.lazyParameters = lazy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineBuilder"/> class.
        /// </summary>
        /// <param name="source">The source pipeline to attach the new effect to</param>
        /// <param name="factory">A <see cref="Func{TResult}"/> instance that will produce the new <see cref="IGraphicsEffectSource"/> to add to the pipeline</param>
        /// <param name="animations">The collection of animation properties for the new effect</param>
        /// <param name="lazy">The collection of <see cref="CompositionBrush"/> instances that needs to be initialized for the new effect</param>
        private PipelineBuilder(
            PipelineBuilder source,
            Func<ValueTask<IGraphicsEffectSource>> factory,
            IReadOnlyCollection<string> animations = null,
            IReadOnlyDictionary<string, Func<ValueTask<CompositionBrush>>> lazy = null)
            : this(
                factory,
                animations?.Merge(source.animationProperties) ?? source.animationProperties,
                lazy?.Merge(source.lazyParameters) ?? source.lazyParameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineBuilder"/> class.
        /// </summary>
        /// <param name="factory">A <see cref="Func{TResult}"/> instance that will produce the new <see cref="IGraphicsEffectSource"/> to add to the pipeline</param>
        /// <param name="a">The first pipeline to merge</param>
        /// <param name="b">The second pipeline to merge</param>
        /// <param name="animations">The collection of animation properties for the new effect</param>
        /// <param name="lazy">The collection of <see cref="CompositionBrush"/> instances that needs to be initialized for the new effect</param>
        private PipelineBuilder(
            Func<ValueTask<IGraphicsEffectSource>> factory,
            PipelineBuilder a,
            PipelineBuilder b,
            IReadOnlyCollection<string> animations = null,
            IReadOnlyDictionary<string, Func<ValueTask<CompositionBrush>>> lazy = null)
            : this(
                factory,
                animations?.Merge(a.animationProperties.Merge(b.animationProperties)) ?? a.animationProperties.Merge(b.animationProperties),
                lazy?.Merge(a.lazyParameters.Merge(b.lazyParameters)) ?? a.lazyParameters.Merge(b.lazyParameters))
        {
        }

        /// <summary>
        /// Builds a <see cref="CompositionBrush"/> instance from the current effects pipeline
        /// </summary>
        /// <returns>A <see cref="Task{T}"/> that returns the final <see cref="CompositionBrush"/> instance to use</returns>
        [Pure]
        public async Task<CompositionBrush> BuildAsync()
        {
            var effect = await this.sourceProducer() as IGraphicsEffect;

            // Validate the pipeline
            if (effect is null)
            {
                throw new InvalidOperationException("The pipeline doesn't contain a valid effects sequence");
            }

            // Build the effects factory
            var factory = this.animationProperties.Count > 0
                ? Window.Current.Compositor.CreateEffectFactory(effect, this.animationProperties)
                : Window.Current.Compositor.CreateEffectFactory(effect);

            // Create the effect factory and apply the final effect
            var effectBrush = factory.CreateBrush();
            foreach (var pair in this.lazyParameters)
            {
                effectBrush.SetSourceParameter(pair.Key, await pair.Value());
            }

            return effectBrush;
        }

        /// <summary>
        /// Builds the current pipeline and creates a <see cref="SpriteVisual"/> that is applied to the input <see cref="UIElement"/>
        /// </summary>
        /// <param name="target">The target <see cref="UIElement"/> to apply the brush to</param>
        /// <param name="reference">An optional <see cref="UIElement"/> to use to bind the size of the created brush</param>
        /// <returns>A <see cref="Task{T}"/> that returns the final <see cref="SpriteVisual"/> instance to use</returns>
        public async Task<SpriteVisual> AttachAsync(UIElement target, UIElement reference = null)
        {
            var visual = Window.Current.Compositor.CreateSpriteVisual();

            visual.Brush = await BuildAsync();

            ElementCompositionPreview.SetElementChildVisual(target, visual);

            if (reference != null)
            {
                visual.BindSize(reference);
            }

            return visual;
        }

        /// <summary>
        /// Creates a new <see cref="XamlCompositionBrush"/> from the current effects pipeline
        /// </summary>
        /// <returns>A <see cref="XamlCompositionBrush"/> instance ready to be displayed</returns>
        [Pure]
        public XamlCompositionBrush AsBrush()
        {
            return new XamlCompositionBrush(this);
        }
    }
}
