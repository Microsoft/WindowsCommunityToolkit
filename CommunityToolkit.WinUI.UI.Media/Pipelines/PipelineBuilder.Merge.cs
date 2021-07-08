// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.UI.Animations;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.UI.Composition;
using Windows.Graphics.Effects;
using CanvasBlendEffect = Microsoft.Graphics.Canvas.Effects.BlendEffect;
using CanvasCrossFadeEffect = Microsoft.Graphics.Canvas.Effects.CrossFadeEffect;

namespace CommunityToolkit.WinUI.UI.Media.Pipelines
{
    /// <summary>
    /// A <see langword="class"/> that allows to build custom effects pipelines and create <see cref="CompositionBrush"/> instances from them
    /// </summary>
    public sealed partial class PipelineBuilder
    {
        /// <summary>
        /// Blends two pipelines using a <see cref="BlendEffect"/> instance with the specified mode
        /// </summary>
        /// <param name="pipeline">The second <see cref="PipelineBuilder"/> instance to blend</param>
        /// <param name="mode">The desired <see cref="BlendEffectMode"/> to use to blend the input pipelines</param>
        /// <param name="placement">The placemeht to use with the two input pipelines (the default is <see cref="Placement.Foreground"/>)</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Blend(PipelineBuilder pipeline, BlendEffectMode mode, Placement placement = Placement.Foreground)
        {
            var (foreground, background) = placement switch
            {
                Placement.Foreground => (pipeline, this),
                Placement.Background => (this, pipeline),
                _ => throw new ArgumentException($"Invalid placement value: {placement}")
            };

            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasBlendEffect
            {
                Foreground = await foreground.sourceProducer(),
                Background = await background.sourceProducer(),
                Mode = mode
            };

            return new PipelineBuilder(Factory, foreground, background);
        }

        /// <summary>
        /// Cross fades two pipelines using an <see cref="CanvasCrossFadeEffect"/> instance
        /// </summary>
        /// <param name="pipeline">The second <see cref="PipelineBuilder"/> instance to cross fade</param>
        /// <param name="factor">The cross fade factor to blend the input effects (default is 0.5, must be in the [0, 1] range)</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder CrossFade(PipelineBuilder pipeline, float factor = 0.5f)
        {
            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasCrossFadeEffect
            {
                CrossFade = factor,
                Source1 = await this.sourceProducer(),
                Source2 = await pipeline.sourceProducer()
            };

            return new PipelineBuilder(Factory, this, pipeline);
        }

        /// <summary>
        /// Cross fades two pipelines using an <see cref="CanvasCrossFadeEffect"/> instance
        /// </summary>
        /// <param name="pipeline">The second <see cref="PipelineBuilder"/> instance to cross fade</param>
        /// <param name="factor">The cross fade factor to blend the input effects (should be in the [0, 1] range)</param>
        /// <param name="setter">The optional blur setter for the effect</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder CrossFade(PipelineBuilder pipeline, float factor, out EffectSetter<float> setter)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasCrossFadeEffect
            {
                CrossFade = factor,
                Source1 = await this.sourceProducer(),
                Source2 = await pipeline.sourceProducer(),
                Name = id
            };

            setter = (brush, value) => brush.Properties.InsertScalar($"{id}.{nameof(CanvasCrossFadeEffect.CrossFade)}", value);

            return new PipelineBuilder(Factory, this, pipeline, new[] { $"{id}.{nameof(CanvasCrossFadeEffect.CrossFade)}" });
        }

        /// <summary>
        /// Cross fades two pipelines using an <see cref="CanvasCrossFadeEffect"/> instance
        /// </summary>
        /// <param name="pipeline">The second <see cref="PipelineBuilder"/> instance to cross fade</param>
        /// <param name="factor">The cross fade factor to blend the input effects (should be in the [0, 1] range)</param>
        /// <param name="animation">The optional blur animation for the effect</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder CrossFade(PipelineBuilder pipeline, float factor, out EffectAnimation<float> animation)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasCrossFadeEffect
            {
                CrossFade = factor,
                Source1 = await this.sourceProducer(),
                Source2 = await pipeline.sourceProducer(),
                Name = id
            };

            animation = (brush, value, duration) => brush.StartAnimationAsync($"{id}.{nameof(CanvasCrossFadeEffect.CrossFade)}", value, duration);

            return new PipelineBuilder(Factory, this, pipeline, new[] { $"{id}.{nameof(CanvasCrossFadeEffect.CrossFade)}" });
        }

        /// <summary>
        /// Blends two pipelines using the provided <see cref="Func{T1, T2, TResult}"/> to do so
        /// </summary>
        /// <param name="factory">The blend function to use</param>
        /// <param name="background">The background pipeline to blend with the current instance</param>
        /// <param name="animations">The list of optional animatable properties in the returned effect</param>
        /// <param name="initializers">The list of source parameters that require deferred initialization (see <see cref="CompositionEffectSourceParameter"/> for more info)</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Merge(
            Func<IGraphicsEffectSource, IGraphicsEffectSource, IGraphicsEffectSource> factory,
            PipelineBuilder background,
            IEnumerable<string> animations = null,
            IEnumerable<BrushProvider> initializers = null)
        {
            async ValueTask<IGraphicsEffectSource> Factory() => factory(await this.sourceProducer(), await background.sourceProducer());

            return new PipelineBuilder(Factory, this, background, animations?.ToArray(), initializers?.ToDictionary(item => item.Name, item => item.Initializer));
        }

        /// <summary>
        /// Blends two pipelines using the provided asynchronous <see cref="Func{T1, T2, TResult}"/> to do so
        /// </summary>
        /// <param name="factory">The asynchronous blend function to use</param>
        /// <param name="background">The background pipeline to blend with the current instance</param>
        /// <param name="animations">The list of optional animatable properties in the returned effect</param>
        /// <param name="initializers">The list of source parameters that require deferred initialization (see <see cref="CompositionEffectSourceParameter"/> for more info)</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Merge(
            Func<IGraphicsEffectSource, IGraphicsEffectSource, Task<IGraphicsEffectSource>> factory,
            PipelineBuilder background,
            IEnumerable<string> animations = null,
            IEnumerable<BrushProvider> initializers = null)
        {
            async ValueTask<IGraphicsEffectSource> Factory() => await factory(await this.sourceProducer(), await background.sourceProducer());

            return new PipelineBuilder(Factory, this, background, animations?.ToArray(), initializers?.ToDictionary(item => item.Name, item => item.Initializer));
        }
    }
}