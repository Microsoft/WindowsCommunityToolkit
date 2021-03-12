// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas.Effects;
using Windows.Graphics.Effects;
using Windows.UI;
using Windows.UI.Composition;
using CanvasExposureEffect = Microsoft.Graphics.Canvas.Effects.ExposureEffect;
using CanvasGrayscaleEffect = Microsoft.Graphics.Canvas.Effects.GrayscaleEffect;
using CanvasHueRotationEffect = Microsoft.Graphics.Canvas.Effects.HueRotationEffect;
using CanvasInvertEffect = Microsoft.Graphics.Canvas.Effects.InvertEffect;
using CanvasLuminanceToAlphaEffect = Microsoft.Graphics.Canvas.Effects.LuminanceToAlphaEffect;
using CanvasOpacityEffect = Microsoft.Graphics.Canvas.Effects.OpacityEffect;
using CanvasSaturationEffect = Microsoft.Graphics.Canvas.Effects.SaturationEffect;
using CanvasSepiaEffect = Microsoft.Graphics.Canvas.Effects.SepiaEffect;
using CanvasTemperatureAndTintEffect = Microsoft.Graphics.Canvas.Effects.TemperatureAndTintEffect;
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
        /// <param name="mode">The <see cref="EffectBorderMode"/> parameter for the effect, defaults to <see cref="EffectBorderMode.Hard"/></param>
        /// <param name="optimization">The <see cref="EffectOptimization"/> parameter to use, defaults to <see cref="EffectOptimization.Balanced"/></param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Blur(float blur, EffectBorderMode mode = EffectBorderMode.Hard, EffectOptimization optimization = EffectOptimization.Balanced)
        {
            async ValueTask<IGraphicsEffectSource> Factory() => new GaussianBlurEffect
            {
                BlurAmount = blur,
                BorderMode = mode,
                Optimization = optimization,
                Source = await this.sourceProducer()
            };

            return new PipelineBuilder(this, Factory);
        }

        /// <summary>
        /// Adds a new <see cref="GaussianBlurEffect"/> to the current pipeline
        /// </summary>
        /// <param name="blur">The initial blur amount</param>
        /// <param name="setter">The optional blur setter for the effect</param>
        /// <param name="mode">The <see cref="EffectBorderMode"/> parameter for the effect, defaults to <see cref="EffectBorderMode.Hard"/></param>
        /// <param name="optimization">The <see cref="EffectOptimization"/> parameter to use, defaults to <see cref="EffectOptimization.Balanced"/></param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Blur(float blur, out EffectSetter<float> setter, EffectBorderMode mode = EffectBorderMode.Hard, EffectOptimization optimization = EffectOptimization.Balanced)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            async ValueTask<IGraphicsEffectSource> Factory() => new GaussianBlurEffect
            {
                BlurAmount = blur,
                BorderMode = mode,
                Optimization = optimization,
                Source = await this.sourceProducer(),
                Name = id
            };

            setter = (brush, value) => brush.Properties.InsertScalar($"{id}.{nameof(GaussianBlurEffect.BlurAmount)}", value);

            return new PipelineBuilder(this, Factory, new[] { $"{id}.{nameof(GaussianBlurEffect.BlurAmount)}" });
        }

        /// <summary>
        /// Adds a new <see cref="GaussianBlurEffect"/> to the current pipeline
        /// </summary>
        /// <param name="blur">The initial blur amount</param>
        /// <param name="animation">The optional blur animation for the effect</param>
        /// <param name="mode">The <see cref="EffectBorderMode"/> parameter for the effect, defaults to <see cref="EffectBorderMode.Hard"/></param>
        /// <param name="optimization">The <see cref="EffectOptimization"/> parameter to use, defaults to <see cref="EffectOptimization.Balanced"/></param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Blur(float blur, out EffectAnimation<float> animation, EffectBorderMode mode = EffectBorderMode.Hard, EffectOptimization optimization = EffectOptimization.Balanced)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            async ValueTask<IGraphicsEffectSource> Factory() => new GaussianBlurEffect
            {
                BlurAmount = blur,
                BorderMode = mode,
                Optimization = optimization,
                Source = await this.sourceProducer(),
                Name = id
            };

            animation = (brush, value, duration) => brush.StartAnimationAsync($"{id}.{nameof(GaussianBlurEffect.BlurAmount)}", value, duration);

            return new PipelineBuilder(this, Factory, new[] { $"{id}.{nameof(GaussianBlurEffect.BlurAmount)}" });
        }

        /// <summary>
        /// Adds a new <see cref="CanvasSaturationEffect"/> to the current pipeline
        /// </summary>
        /// <param name="saturation">The saturation amount for the new effect (should be in the [0, 1] range)</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Saturation(float saturation)
        {
            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasSaturationEffect
            {
                Saturation = saturation,
                Source = await this.sourceProducer()
            };

            return new PipelineBuilder(this, Factory);
        }

        /// <summary>
        /// Adds a new <see cref="CanvasSaturationEffect"/> to the current pipeline
        /// </summary>
        /// <param name="saturation">The initial saturation amount for the new effect (should be in the [0, 1] range)</param>
        /// <param name="setter">The optional saturation setter for the effect</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Saturation(float saturation, out EffectSetter<float> setter)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasSaturationEffect
            {
                Saturation = saturation,
                Source = await this.sourceProducer(),
                Name = id
            };

            setter = (brush, value) => brush.Properties.InsertScalar($"{id}.{nameof(CanvasSaturationEffect.Saturation)}", value);

            return new PipelineBuilder(this, Factory, new[] { $"{id}.{nameof(CanvasSaturationEffect.Saturation)}" });
        }

        /// <summary>
        /// Adds a new <see cref="CanvasSaturationEffect"/> to the current pipeline
        /// </summary>
        /// <param name="saturation">The initial saturation amount for the new effect (should be in the [0, 1] range)</param>
        /// <param name="animation">The optional saturation animation for the effect</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Saturation(float saturation, out EffectAnimation<float> animation)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasSaturationEffect
            {
                Saturation = saturation,
                Source = await this.sourceProducer(),
                Name = id
            };

            animation = (brush, value, duration) => brush.StartAnimationAsync($"{id}.{nameof(CanvasSaturationEffect.Saturation)}", value, duration);

            return new PipelineBuilder(this, Factory, new[] { $"{id}.{nameof(CanvasSaturationEffect.Saturation)}" });
        }

        /// <summary>
        /// Adds a new <see cref="CanvasSepiaEffect"/> to the current pipeline
        /// </summary>
        /// <param name="intensity">The sepia effect intensity for the new effect</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Sepia(float intensity)
        {
            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasSepiaEffect
            {
                Intensity = intensity,
                Source = await this.sourceProducer()
            };

            return new PipelineBuilder(this, Factory);
        }

        /// <summary>
        /// Adds a new <see cref="CanvasSepiaEffect"/> to the current pipeline
        /// </summary>
        /// <param name="intensity">The sepia effect intensity for the new effect</param>
        /// <param name="setter">The optional sepia intensity setter for the effect</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Sepia(float intensity, out EffectSetter<float> setter)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasSepiaEffect
            {
                Intensity = intensity,
                Source = await this.sourceProducer(),
                Name = id
            };

            setter = (brush, value) => brush.Properties.InsertScalar($"{id}.{nameof(CanvasSepiaEffect.Intensity)}", value);

            return new PipelineBuilder(this, Factory, new[] { $"{id}.{nameof(CanvasSepiaEffect.Intensity)}" });
        }

        /// <summary>
        /// Adds a new <see cref="CanvasSepiaEffect"/> to the current pipeline
        /// </summary>
        /// <param name="intensity">The sepia effect intensity for the new effect</param>
        /// <param name="animation">The sepia intensity animation for the effect</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Sepia(float intensity, out EffectAnimation<float> animation)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasSepiaEffect
            {
                Intensity = intensity,
                Source = await this.sourceProducer(),
                Name = id
            };

            animation = (brush, value, duration) => brush.StartAnimationAsync($"{id}.{nameof(CanvasSepiaEffect.Intensity)}", value, duration);

            return new PipelineBuilder(this, Factory, new[] { $"{id}.{nameof(CanvasSepiaEffect.Intensity)}" });
        }

        /// <summary>
        /// Adds a new <see cref="CanvasOpacityEffect"/> to the current pipeline
        /// </summary>
        /// <param name="opacity">The opacity value to apply to the pipeline</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Opacity(float opacity)
        {
            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasOpacityEffect
            {
                Opacity = opacity,
                Source = await this.sourceProducer()
            };

            return new PipelineBuilder(this, Factory);
        }

        /// <summary>
        /// Adds a new <see cref="CanvasOpacityEffect"/> to the current pipeline
        /// </summary>
        /// <param name="opacity">The opacity value to apply to the pipeline</param>
        /// <param name="setter">The optional opacity setter for the effect</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Opacity(float opacity, out EffectSetter<float> setter)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasOpacityEffect
            {
                Opacity = opacity,
                Source = await this.sourceProducer(),
                Name = id
            };

            setter = (brush, value) => brush.Properties.InsertScalar($"{id}.{nameof(CanvasOpacityEffect.Opacity)}", value);

            return new PipelineBuilder(this, Factory, new[] { $"{id}.{nameof(CanvasOpacityEffect.Opacity)}" });
        }

        /// <summary>
        /// Adds a new <see cref="CanvasOpacityEffect"/> to the current pipeline
        /// </summary>
        /// <param name="opacity">The opacity value to apply to the pipeline</param>
        /// <param name="animation">The optional opacity animation for the effect</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Opacity(float opacity, out EffectAnimation<float> animation)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasOpacityEffect
            {
                Opacity = opacity,
                Source = await this.sourceProducer(),
                Name = id
            };

            animation = (brush, value, duration) => brush.StartAnimationAsync($"{id}.{nameof(CanvasOpacityEffect.Opacity)}", value, duration);

            return new PipelineBuilder(this, Factory, new[] { $"{id}.{nameof(CanvasOpacityEffect.Opacity)}" });
        }

        /// <summary>
        /// Applies an exposure effect on the current pipeline
        /// </summary>
        /// <param name="amount">The amount of exposure to apply over the current effect (should be in the [-2, 2] range)</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Exposure(float amount)
        {
            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasExposureEffect
            {
                Exposure = amount,
                Source = await this.sourceProducer()
            };

            return new PipelineBuilder(this, Factory);
        }

        /// <summary>
        /// Applies an exposure effect on the current pipeline
        /// </summary>
        /// <param name="amount">The initial exposure of tint to apply over the current effect (should be in the [-2, 2] range)</param>
        /// <param name="setter">The optional amount setter for the effect</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Exposure(float amount, out EffectSetter<float> setter)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasExposureEffect
            {
                Exposure = amount,
                Source = await this.sourceProducer(),
                Name = id
            };

            setter = (brush, value) => brush.Properties.InsertScalar($"{id}.{nameof(CanvasExposureEffect.Exposure)}", value);

            return new PipelineBuilder(this, Factory, new[] { $"{id}.{nameof(CanvasExposureEffect.Exposure)}" });
        }

        /// <summary>
        /// Applies an exposure effect on the current pipeline
        /// </summary>
        /// <param name="amount">The initial exposure of tint to apply over the current effect (should be in the [-2, 2] range)</param>
        /// <param name="animation">The optional amount animation for the effect</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Exposure(float amount, out EffectAnimation<float> animation)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasExposureEffect
            {
                Exposure = amount,
                Source = await this.sourceProducer(),
                Name = id
            };

            animation = (brush, value, duration) => brush.StartAnimationAsync($"{id}.{nameof(CanvasExposureEffect.Exposure)}", value, duration);

            return new PipelineBuilder(this, Factory, new[] { $"{id}.{nameof(CanvasExposureEffect.Exposure)}" });
        }

        /// <summary>
        /// Applies a hue rotation effect on the current pipeline
        /// </summary>
        /// <param name="angle">The angle to rotate the hue, in radians</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder HueRotation(float angle)
        {
            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasHueRotationEffect
            {
                Angle = angle,
                Source = await this.sourceProducer()
            };

            return new PipelineBuilder(this, Factory);
        }

        /// <summary>
        /// Applies a hue rotation effect on the current pipeline
        /// </summary>
        /// <param name="angle">The angle to rotate the hue, in radians</param>
        /// <param name="setter">The optional rotation angle setter for the effect</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder HueRotation(float angle, out EffectSetter<float> setter)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasHueRotationEffect
            {
                Angle = angle,
                Source = await this.sourceProducer(),
                Name = id
            };

            setter = (brush, value) => brush.Properties.InsertScalar($"{id}.{nameof(CanvasHueRotationEffect.Angle)}", value);

            return new PipelineBuilder(this, Factory, new[] { $"{id}.{nameof(CanvasHueRotationEffect.Angle)}" });
        }

        /// <summary>
        /// Applies a hue rotation effect on the current pipeline
        /// </summary>
        /// <param name="angle">The angle to rotate the hue, in radians</param>
        /// <param name="animation">The optional rotation angle animation for the effect</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder HueRotation(float angle, out EffectAnimation<float> animation)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasHueRotationEffect
            {
                Angle = angle,
                Source = await this.sourceProducer(),
                Name = id
            };

            animation = (brush, value, duration) => brush.StartAnimationAsync($"{id}.{nameof(CanvasHueRotationEffect.Angle)}", value, duration);

            return new PipelineBuilder(this, Factory, new[] { $"{id}.{nameof(CanvasHueRotationEffect.Angle)}" });
        }

        /// <summary>
        /// Applies a tint effect on the current pipeline
        /// </summary>
        /// <param name="color">The color to use</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Tint(Color color)
        {
            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasTintEffect
            {
                Color = color,
                Source = await this.sourceProducer()
            };

            return new PipelineBuilder(this, Factory);
        }

        /// <summary>
        /// Applies a tint effect on the current pipeline
        /// </summary>
        /// <param name="color">The color to use</param>
        /// <param name="setter">The optional color setter for the effect</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Tint(Color color, out EffectSetter<Color> setter)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasTintEffect
            {
                Color = color,
                Source = await this.sourceProducer(),
                Name = id
            };

            setter = (brush, value) => brush.Properties.InsertColor($"{id}.{nameof(CanvasTintEffect.Color)}", value);

            return new PipelineBuilder(this, Factory, new[] { $"{id}.{nameof(CanvasTintEffect.Color)}" });
        }

        /// <summary>
        /// Applies a tint effect on the current pipeline
        /// </summary>
        /// <param name="color">The color to use</param>
        /// <param name="animation">The optional color animation for the effect</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Tint(Color color, out EffectAnimation<Color> animation)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasTintEffect
            {
                Color = color,
                Source = await this.sourceProducer(),
                Name = id
            };

            animation = (brush, value, duration) => brush.StartAnimationAsync($"{id}.{nameof(CanvasTintEffect.Color)}", value, duration);

            return new PipelineBuilder(this, Factory, new[] { $"{id}.{nameof(CanvasTintEffect.Color)}" });
        }

        /// <summary>
        /// Applies a temperature and tint effect on the current pipeline
        /// </summary>
        /// <param name="temperature">The temperature value to use (should be in the [-1, 1] range)</param>
        /// <param name="tint">The tint value to use (should be in the [-1, 1] range)</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder TemperatureAndTint(float temperature, float tint)
        {
            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasTemperatureAndTintEffect
            {
                Temperature = temperature,
                Tint = tint,
                Source = await this.sourceProducer()
            };

            return new PipelineBuilder(this, Factory);
        }

        /// <summary>
        /// Applies a temperature and tint effect on the current pipeline
        /// </summary>
        /// <param name="temperature">The temperature value to use (should be in the [-1, 1] range)</param>
        /// <param name="temperatureSetter">The optional temperature setter for the effect</param>
        /// <param name="tint">The tint value to use (should be in the [-1, 1] range)</param>
        /// <param name="tintSetter">The optional tint setter for the effect</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder TemperatureAndTint(
            float temperature,
            out EffectSetter<float> temperatureSetter,
            float tint,
            out EffectSetter<float> tintSetter)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasTemperatureAndTintEffect
            {
                Temperature = temperature,
                Tint = tint,
                Source = await this.sourceProducer(),
                Name = id
            };

            temperatureSetter = (brush, value) => brush.Properties.InsertScalar($"{id}.{nameof(CanvasTemperatureAndTintEffect.Temperature)}", value);

            tintSetter = (brush, value) => brush.Properties.InsertScalar($"{id}.{nameof(CanvasTemperatureAndTintEffect.Tint)}", value);

            return new PipelineBuilder(this, Factory, new[] { $"{id}.{nameof(CanvasTemperatureAndTintEffect.Temperature)}", $"{id}.{nameof(CanvasTemperatureAndTintEffect.Tint)}" });
        }

        /// <summary>
        /// Applies a temperature and tint effect on the current pipeline
        /// </summary>
        /// <param name="temperature">The temperature value to use (should be in the [-1, 1] range)</param>
        /// <param name="temperatureAnimation">The optional temperature animation for the effect</param>
        /// <param name="tint">The tint value to use (should be in the [-1, 1] range)</param>
        /// <param name="tintAnimation">The optional tint animation for the effect</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder TemperatureAndTint(
            float temperature,
            out EffectAnimation<float> temperatureAnimation,
            float tint,
            out EffectAnimation<float> tintAnimation)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasTemperatureAndTintEffect
            {
                Temperature = temperature,
                Tint = tint,
                Source = await this.sourceProducer(),
                Name = id
            };

            temperatureAnimation = (brush, value, duration) => brush.StartAnimationAsync($"{id}.{nameof(CanvasTemperatureAndTintEffect.Temperature)}", value, duration);

            tintAnimation = (brush, value, duration) => brush.StartAnimationAsync($"{id}.{nameof(CanvasTemperatureAndTintEffect.Tint)}", value, duration);

            return new PipelineBuilder(this, Factory, new[] { $"{id}.{nameof(CanvasTemperatureAndTintEffect.Temperature)}", $"{id}.{nameof(CanvasTemperatureAndTintEffect.Tint)}" });
        }

        /// <summary>
        /// Applies a shade effect on the current pipeline
        /// </summary>
        /// <param name="color">The color to use</param>
        /// <param name="mix">The amount of mix to apply over the current effect (must be in the [0, 1] range)</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Shade(Color color, float mix)
        {
            return FromColor(color).CrossFade(this, mix);
        }

        /// <summary>
        /// Applies a shade effect on the current pipeline
        /// </summary>
        /// <param name="color">The color to use</param>
        /// <param name="colorSetter">The optional color setter for the effect</param>
        /// <param name="mix">The initial amount of mix to apply over the current effect (must be in the [0, 1] range)</param>
        /// <param name="mixSetter">The optional mix setter for the effect</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Shade(
            Color color,
            out EffectSetter<Color> colorSetter,
            float mix,
            out EffectSetter<float> mixSetter)
        {
            return FromColor(color, out colorSetter).CrossFade(this, mix, out mixSetter);
        }

        /// <summary>
        /// Applies a shade effect on the current pipeline
        /// </summary>
        /// <param name="color">The color to use</param>
        /// <param name="colorAnimation">The optional color animation for the effect</param>
        /// <param name="mix">The initial amount of mix to apply over the current effect (must be in the [0, 1] range)</param>
        /// <param name="mixAnimation">The optional mix animation for the effect</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Shade(
            Color color,
            out EffectAnimation<Color> colorAnimation,
            float mix,
            out EffectAnimation<float> mixAnimation)
        {
            return FromColor(color, out colorAnimation).CrossFade(this, mix, out mixAnimation);
        }

        /// <summary>
        /// Applies a luminance to alpha effect on the current pipeline
        /// </summary>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder LuminanceToAlpha()
        {
            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasLuminanceToAlphaEffect
            {
                Source = await this.sourceProducer()
            };

            return new PipelineBuilder(this, Factory);
        }

        /// <summary>
        /// Applies an invert effect on the current pipeline
        /// </summary>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Invert()
        {
            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasInvertEffect
            {
                Source = await this.sourceProducer()
            };

            return new PipelineBuilder(this, Factory);
        }

        /// <summary>
        /// Applies a grayscale on the current pipeline
        /// </summary>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Grayscale()
        {
            async ValueTask<IGraphicsEffectSource> Factory() => new CanvasGrayscaleEffect
            {
                Source = await this.sourceProducer()
            };

            return new PipelineBuilder(this, Factory);
        }

        /// <summary>
        /// Applies a custom effect to the current pipeline
        /// </summary>
        /// <param name="factory">A <see cref="Func{T, TResult}"/> that takes the current <see cref="IGraphicsEffectSource"/> instance and produces a new effect to display</param>
        /// <param name="animations">The list of optional animatable properties in the returned effect</param>
        /// <param name="initializers">The list of source parameters that require deferred initialization (see <see cref="CompositionEffectSourceParameter"/> for more info)</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Effect(
            Func<IGraphicsEffectSource, IGraphicsEffectSource> factory,
            IEnumerable<string> animations = null,
            IEnumerable<BrushProvider> initializers = null)
        {
            async ValueTask<IGraphicsEffectSource> Factory() => factory(await this.sourceProducer());

            return new PipelineBuilder(this, Factory, animations?.ToArray(), initializers?.ToDictionary(item => item.Name, item => item.Initializer));
        }

        /// <summary>
        /// Applies a custom effect to the current pipeline
        /// </summary>
        /// <param name="factory">An asynchronous <see cref="Func{T, TResult}"/> that takes the current <see cref="IGraphicsEffectSource"/> instance and produces a new effect to display</param>
        /// <param name="animations">The list of optional animatable properties in the returned effect</param>
        /// <param name="initializers">The list of source parameters that require deferred initialization (see <see cref="CompositionEffectSourceParameter"/> for more info)</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public PipelineBuilder Effect(
            Func<IGraphicsEffectSource, Task<IGraphicsEffectSource>> factory,
            IEnumerable<string> animations = null,
            IEnumerable<BrushProvider> initializers = null)
        {
            async ValueTask<IGraphicsEffectSource> Factory() => await factory(await this.sourceProducer());

            return new PipelineBuilder(this, Factory, animations?.ToArray(), initializers?.ToDictionary(item => item.Name, item => item.Initializer));
        }
    }
}
