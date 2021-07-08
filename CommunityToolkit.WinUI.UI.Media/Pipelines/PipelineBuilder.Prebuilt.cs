// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI;

namespace CommunityToolkit.WinUI.UI.Media.Pipelines
{
    /// <summary>
    /// A <see langword="class"/> that allows to build custom effects pipelines and create <see cref="Microsoft.UI.Composition.CompositionBrush"/> instances from them
    /// </summary>
    public sealed partial class PipelineBuilder
    {
        /// <summary>
        /// Returns a new <see cref="PipelineBuilder"/> instance that implements the host backdrop acrylic effect
        /// </summary>
        /// <param name="tintColor">The tint color to use</param>
        /// <param name="tintOpacity">The amount of tint to apply over the current effect</param>
        /// <param name="noiseUri">The <see cref="Uri"/> for the noise texture to load for the acrylic effect</param>
        /// <param name="dpi">Indicates the current display DPI</param>
        /// <param name="cacheMode">The cache mode to use to load the image</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromHostBackdropAcrylic(
            Color tintColor,
            float tintOpacity,
            Uri noiseUri,
            float dpi,
            CacheMode cacheMode = CacheMode.Default)
        {
            var pipeline =
                FromHostBackdrop()
                .LuminanceToAlpha()
                .Opacity(0.4f)
                .Blend(FromHostBackdrop(), BlendEffectMode.Multiply)
                .Shade(tintColor, tintOpacity);

            if (noiseUri != null)
            {
                return pipeline.Blend(FromTiles(noiseUri, dpi, cacheMode: cacheMode), BlendEffectMode.Overlay);
            }

            return pipeline;
        }

        /// <summary>
        /// Returns a new <see cref="PipelineBuilder"/> instance that implements the host backdrop acrylic effect
        /// </summary>
        /// <param name="tintColor">The tint color to use</param>
        /// <param name="tintOpacity">The amount of tint to apply over the current effect</param>
        /// <param name="noiseUri">The <see cref="Uri"/> for the noise texture to load for the acrylic effect</param>
        /// <param name="dpiFactory">Factory that return the current display DPI</param>
        /// <param name="cacheMode">The cache mode to use to load the image</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromHostBackdropAcrylic(
            Color tintColor,
            float tintOpacity,
            Uri noiseUri,
            Func<float> dpiFactory,
            CacheMode cacheMode = CacheMode.Default)
        {
            var pipeline =
                FromHostBackdrop()
                .LuminanceToAlpha()
                .Opacity(0.4f)
                .Blend(FromHostBackdrop(), BlendEffectMode.Multiply)
                .Shade(tintColor, tintOpacity);

            if (noiseUri != null)
            {
                return pipeline.Blend(FromTiles(noiseUri, dpiFactory, cacheMode: cacheMode), BlendEffectMode.Overlay);
            }

            return pipeline;
        }

        /// <summary>
        /// Returns a new <see cref="PipelineBuilder"/> instance that implements the host backdrop acrylic effect
        /// </summary>
        /// <param name="tintColor">The tint color to use</param>
        /// <param name="tintColorSetter">The optional tint color setter for the effect</param>
        /// <param name="tintOpacity">The amount of tint to apply over the current effect</param>
        /// <param name="tintOpacitySetter">The optional tint mix setter for the effect</param>
        /// <param name="noiseUri">The <see cref="Uri"/> for the noise texture to load for the acrylic effect</param>
        /// <param name="dpi">Indicates the current display DPI</param>
        /// <param name="cacheMode">The cache mode to use to load the image</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromHostBackdropAcrylic(
            Color tintColor,
            out EffectSetter<Color> tintColorSetter,
            float tintOpacity,
            out EffectSetter<float> tintOpacitySetter,
            Uri noiseUri,
            float dpi,
            CacheMode cacheMode = CacheMode.Default)
        {
            var pipeline =
                FromHostBackdrop()
                .LuminanceToAlpha()
                .Opacity(0.4f)
                .Blend(FromHostBackdrop(), BlendEffectMode.Multiply)
                .Shade(tintColor, out tintColorSetter, tintOpacity, out tintOpacitySetter);

            if (noiseUri != null)
            {
                return pipeline.Blend(FromTiles(noiseUri, dpi, cacheMode: cacheMode), BlendEffectMode.Overlay);
            }

            return pipeline;
        }

        /// <summary>
        /// Returns a new <see cref="PipelineBuilder"/> instance that implements the host backdrop acrylic effect
        /// </summary>
        /// <param name="tintColor">The tint color to use</param>
        /// <param name="tintColorAnimation">The optional tint color animation for the effect</param>
        /// <param name="tintOpacity">The amount of tint to apply over the current effect</param>
        /// <param name="tintOpacityAnimation">The optional tint mix animation for the effect</param>
        /// <param name="noiseUri">The <see cref="Uri"/> for the noise texture to load for the acrylic effect</param>
        /// <param name="dpi">Indicates the current display DPI</param>
        /// <param name="cacheMode">The cache mode to use to load the image</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromHostBackdropAcrylic(
            Color tintColor,
            out EffectAnimation<Color> tintColorAnimation,
            float tintOpacity,
            out EffectAnimation<float> tintOpacityAnimation,
            Uri noiseUri,
            float dpi,
            CacheMode cacheMode = CacheMode.Default)
        {
            var pipeline =
                FromHostBackdrop()
                .LuminanceToAlpha()
                .Opacity(0.4f)
                .Blend(FromHostBackdrop(), BlendEffectMode.Multiply)
                .Shade(tintColor, out tintColorAnimation, tintOpacity, out tintOpacityAnimation);

            if (noiseUri != null)
            {
                return pipeline.Blend(FromTiles(noiseUri, dpi, cacheMode: cacheMode), BlendEffectMode.Overlay);
            }

            return pipeline;
        }

        /// <summary>
        /// Returns a new <see cref="PipelineBuilder"/> instance that implements the in-app backdrop acrylic effect
        /// </summary>
        /// <param name="tintColor">The tint color to use</param>
        /// <param name="tintOpacity">The amount of tint to apply over the current effect (must be in the [0, 1] range)</param>
        /// <param name="blurAmount">The amount of blur to apply to the acrylic brush</param>
        /// <param name="noiseUri">The <see cref="Uri"/> for the noise texture to load for the acrylic effect</param>
        /// <param name="dpi">Indicates the current display DPI</param>
        /// <param name="cacheMode">The cache mode to use to load the image</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromBackdropAcrylic(
            Color tintColor,
            float tintOpacity,
            float blurAmount,
            Uri noiseUri,
            float dpi,
            CacheMode cacheMode = CacheMode.Default)
        {
            var pipeline = FromBackdrop().Shade(tintColor, tintOpacity).Blur(blurAmount);

            if (noiseUri != null)
            {
                return pipeline.Blend(FromTiles(noiseUri, dpi, cacheMode: cacheMode), BlendEffectMode.Overlay);
            }

            return pipeline;
        }

        /// <summary>
        /// Returns a new <see cref="PipelineBuilder"/> instance that implements the in-app backdrop acrylic effect
        /// </summary>
        /// <param name="tintColor">The tint color to use</param>
        /// <param name="tintOpacity">The amount of tint to apply over the current effect (must be in the [0, 1] range)</param>
        /// <param name="blurAmount">The amount of blur to apply to the acrylic brush</param>
        /// <param name="noiseUri">The <see cref="Uri"/> for the noise texture to load for the acrylic effect</param>
        /// <param name="dpiFactory">Factory that return the current display DPI</param>
        /// <param name="cacheMode">The cache mode to use to load the image</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromBackdropAcrylic(
            Color tintColor,
            float tintOpacity,
            float blurAmount,
            Uri noiseUri,
            Func<float> dpiFactory,
            CacheMode cacheMode = CacheMode.Default)
        {
            var pipeline = FromBackdrop().Shade(tintColor, tintOpacity).Blur(blurAmount);

            if (noiseUri != null)
            {
                return pipeline.Blend(FromTiles(noiseUri, dpiFactory, cacheMode: cacheMode), BlendEffectMode.Overlay);
            }

            return pipeline;
        }

        /// <summary>
        /// Returns a new <see cref="PipelineBuilder"/> instance that implements the in-app backdrop acrylic effect
        /// </summary>
        /// <param name="tintColor">The tint color to use</param>
        /// <param name="tintColorSetter">The optional tint color setter for the effect</param>
        /// <param name="tintOpacity">The amount of tint to apply over the current effect</param>
        /// <param name="tintOpacitySetter">The optional tint mix setter for the effect</param>
        /// <param name="blurAmount">The amount of blur to apply to the acrylic brush</param>
        /// <param name="blurAmountSetter">The optional blur setter for the effect</param>
        /// <param name="noiseUri">The <see cref="Uri"/> for the noise texture to load for the acrylic effect</param>
        /// <param name="dpi">Indicates the current display DPI</param>
        /// <param name="cacheMode">The cache mode to use to load the image</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromBackdropAcrylic(
            Color tintColor,
            out EffectSetter<Color> tintColorSetter,
            float tintOpacity,
            out EffectSetter<float> tintOpacitySetter,
            float blurAmount,
            out EffectSetter<float> blurAmountSetter,
            Uri noiseUri,
            float dpi,
            CacheMode cacheMode = CacheMode.Default)
        {
            var pipeline =
                FromBackdrop()
                .Shade(tintColor, out tintColorSetter, tintOpacity, out tintOpacitySetter)
                .Blur(blurAmount, out blurAmountSetter);

            if (noiseUri != null)
            {
                return pipeline.Blend(FromTiles(noiseUri, dpi, cacheMode: cacheMode), BlendEffectMode.Overlay);
            }

            return pipeline;
        }

        /// <summary>
        /// Returns a new <see cref="PipelineBuilder"/> instance that implements the in-app backdrop acrylic effect
        /// </summary>
        /// <param name="tintColor">The tint color to use</param>
        /// <param name="tintAnimation">The optional tint color animation for the effect</param>
        /// <param name="tintOpacity">The amount of tint to apply over the current effect</param>
        /// <param name="tintOpacityAnimation">The optional tint mix animation for the effect</param>
        /// <param name="blurAmount">The amount of blur to apply to the acrylic brush</param>
        /// <param name="blurAmountAnimation">The optional blur animation for the effect</param>
        /// <param name="noiseUri">The <see cref="Uri"/> for the noise texture to load for the acrylic effect</param>
        /// <param name="dpi">Indicates the current display DPI</param>
        /// <param name="cacheMode">The cache mode to use to load the image</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromBackdropAcrylic(
            Color tintColor,
            out EffectAnimation<Color> tintAnimation,
            float tintOpacity,
            out EffectAnimation<float> tintOpacityAnimation,
            float blurAmount,
            out EffectAnimation<float> blurAmountAnimation,
            Uri noiseUri,
            float dpi,
            CacheMode cacheMode = CacheMode.Default)
        {
            var pipeline =
                FromBackdrop()
                .Shade(tintColor, out tintAnimation, tintOpacity, out tintOpacityAnimation)
                .Blur(blurAmount, out blurAmountAnimation);

            if (noiseUri != null)
            {
                return pipeline.Blend(FromTiles(noiseUri, dpi, cacheMode: cacheMode), BlendEffectMode.Overlay);
            }

            return pipeline;
        }
    }
}