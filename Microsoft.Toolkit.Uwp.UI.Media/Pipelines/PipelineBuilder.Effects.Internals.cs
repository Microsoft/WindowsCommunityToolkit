// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Windows.Graphics.Effects;
using Windows.UI;
using Windows.UI.Composition;
using CanvasCrossFadeEffect = Microsoft.Graphics.Canvas.Effects.CrossFadeEffect;
using CanvasExposureEffect = Microsoft.Graphics.Canvas.Effects.ExposureEffect;
using CanvasHueRotationEffect = Microsoft.Graphics.Canvas.Effects.HueRotationEffect;
using CanvasOpacityEffect = Microsoft.Graphics.Canvas.Effects.OpacityEffect;
using CanvasSaturationEffect = Microsoft.Graphics.Canvas.Effects.SaturationEffect;
using CanvasSepiaEffect = Microsoft.Graphics.Canvas.Effects.SepiaEffect;
using CanvasTintEffect = Microsoft.Graphics.Canvas.Effects.TintEffect;

namespace Microsoft.Toolkit.Uwp.UI.Media.Pipelines
{
    /// <summary>
    /// A <see langword="class"/> that allows to build custom effects pipelines and create <see cref="CompositionBrush"/> instances from them
    /// </summary>
    public sealed partial class PipelineBuilder
    {
        /// <summary>
        /// Adds a new <see cref="GaussianBlurEffect"/> to the current pipeline
        /// </summary>
        /// <param name="blur">The blur amount to apply</param>
        /// <param name="target">The target property to animate the resulting effect.</param>
        /// <param name="mode">The <see cref="EffectBorderMode"/> parameter for the effect, defaults to <see cref="EffectBorderMode.Hard"/></param>
        /// <param name="optimization">The <see cref="EffectOptimization"/> parameter to use, defaults to <see cref="EffectOptimization.Balanced"/></param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        internal PipelineBuilder Blur(
            float blur,
            out string target,
            EffectBorderMode mode = EffectBorderMode.Hard,
            EffectOptimization optimization = EffectOptimization.Balanced)
        {
            string name = Guid.NewGuid().ToUppercaseAsciiLetters();

            target = $"{name}.{nameof(GaussianBlurEffect.BlurAmount)}";

            async ValueTask<IGraphicsEffectSource> Factory() => new GaussianBlurEffect
            {
                BlurAmount = blur,
                BorderMode = mode,
                Optimization = optimization,
                Source = await this.sourceProducer(),
                Name = name
            };

            return new PipelineBuilder(this, Factory, new[] { target });
        }

        /// <summary>
        /// Cross fades two pipelines using an <see cref="CanvasCrossFadeEffect"/> instance
        /// </summary>
        /// <param name="pipeline">The second <see cref="PipelineBuilder"/> instance to cross fade</param>
        /// <param name="factor">The cross fade factor to blend the input effects (should be in the [0, 1] range)</param>
        /// <param name="target">The target property to animate the resulting effect.</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder CrossFade(PipelineBuilder pipeline, float factor, out string target)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            target = $"{id}.{nameof(CanvasCrossFadeEffect.CrossFade)}";

            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasCrossFadeEffect
            {
                CrossFade = factor,
                Source1 = await this.sourceProducer(),
                Source2 = await pipeline.sourceProducer(),
                Name = id
            };

            return new PipelineBuilder(Factory, this, pipeline, new[] { target });
        }

        /// <summary>
        /// Applies an exposure effect on the current pipeline
        /// </summary>
        /// <param name="amount">The initial exposure of tint to apply over the current effect (should be in the [-2, 2] range)</param>
        /// <param name="target">The target property to animate the resulting effect.</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Exposure(float amount, out string target)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            target = $"{id}.{nameof(CanvasExposureEffect.Exposure)}";

            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasExposureEffect
            {
                Exposure = amount,
                Source = await this.sourceProducer(),
                Name = id
            };

            return new PipelineBuilder(this, Factory, new[] { target });
        }

        /// <summary>
        /// Applies a hue rotation effect on the current pipeline
        /// </summary>
        /// <param name="angle">The angle to rotate the hue, in radians</param>
        /// <param name="target">The target property to animate the resulting effect.</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder HueRotation(float angle, out string target)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            target = $"{id}.{nameof(CanvasHueRotationEffect.Angle)}";

            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasHueRotationEffect
            {
                Angle = angle,
                Source = await this.sourceProducer(),
                Name = id
            };

            return new PipelineBuilder(this, Factory, new[] { target });
        }

        /// <summary>
        /// Adds a new <see cref="CanvasOpacityEffect"/> to the current pipeline
        /// </summary>
        /// <param name="opacity">The opacity value to apply to the pipeline</param>
        /// <param name="target">The target property to animate the resulting effect.</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Opacity(float opacity, out string target)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            target = $"{id}.{nameof(CanvasOpacityEffect.Opacity)}";

            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasOpacityEffect
            {
                Opacity = opacity,
                Source = await this.sourceProducer(),
                Name = id
            };

            return new PipelineBuilder(this, Factory, new[] { target });
        }

        /// <summary>
        /// Adds a new <see cref="CanvasSaturationEffect"/> to the current pipeline
        /// </summary>
        /// <param name="saturation">The initial saturation amount for the new effect (should be in the [0, 1] range)</param>
        /// <param name="target">The target property to animate the resulting effect.</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Saturation(float saturation, out string target)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            target = $"{id}.{nameof(CanvasSaturationEffect.Saturation)}";

            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasSaturationEffect
            {
                Saturation = saturation,
                Source = await this.sourceProducer(),
                Name = id
            };

            return new PipelineBuilder(this, Factory, new[] { target });
        }

        /// <summary>
        /// Adds a new <see cref="CanvasSepiaEffect"/> to the current pipeline
        /// </summary>
        /// <param name="intensity">The sepia effect intensity for the new effect</param>
        /// <param name="target">The target property to animate the resulting effect.</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Sepia(float intensity, out string target)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            target = $"{id}.{nameof(CanvasSepiaEffect.Intensity)}";

            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasSepiaEffect
            {
                Intensity = intensity,
                Source = await this.sourceProducer(),
                Name = id
            };

            return new PipelineBuilder(this, Factory, new[] { target });
        }

        /// <summary>
        /// Applies a tint effect on the current pipeline
        /// </summary>
        /// <param name="color">The color to use</param>
        /// <param name="target">The target property to animate the resulting effect.</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Tint(Color color, out string target)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            target = $"{id}.{nameof(CanvasTintEffect.Color)}";

            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasTintEffect
            {
                Color = color,
                Source = await this.sourceProducer(),
                Name = id
            };

            return new PipelineBuilder(this, Factory, new[] { target });
        }
    }
}