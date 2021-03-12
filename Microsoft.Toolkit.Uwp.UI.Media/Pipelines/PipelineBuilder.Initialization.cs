// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Toolkit.Uwp.UI.Media.Helpers;
using Microsoft.Toolkit.Uwp.UI.Media.Helpers.Cache;
using Windows.Graphics.Effects;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace Microsoft.Toolkit.Uwp.UI.Media.Pipelines
{
    /// <summary>
    /// A <see langword="class"/> that allows to build custom effects pipelines and create <see cref="CompositionBrush"/> instances from them
    /// </summary>
    public sealed partial class PipelineBuilder
    {
        /// <summary>
        /// The cache manager for backdrop brushes
        /// </summary>
        private static readonly CompositionObjectCache<CompositionBrush> BackdropBrushCache = new CompositionObjectCache<CompositionBrush>();

        /// <summary>
        /// The cache manager for host backdrop brushes
        /// </summary>
        private static readonly CompositionObjectCache<CompositionBrush> HostBackdropBrushCache = new CompositionObjectCache<CompositionBrush>();

        /// <summary>
        /// Starts a new <see cref="PipelineBuilder"/> pipeline from the <see cref="CompositionBrush"/> returned by <see cref="Compositor.CreateBackdropBrush"/>
        /// </summary>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromBackdrop()
        {
            ValueTask<CompositionBrush> Factory()
            {
                var brush = BackdropBrushCache.GetValue(Window.Current.Compositor, c => c.CreateBackdropBrush());

                return new ValueTask<CompositionBrush>(brush);
            }

            return new PipelineBuilder(Factory);
        }

        /// <summary>
        /// Starts a new <see cref="PipelineBuilder"/> pipeline from the <see cref="CompositionBrush"/> returned by <see cref="Compositor.CreateHostBackdropBrush"/>
        /// </summary>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromHostBackdrop()
        {
            ValueTask<CompositionBrush> Factory()
            {
                var brush = HostBackdropBrushCache.GetValue(Window.Current.Compositor, c => c.CreateHostBackdropBrush());

                return new ValueTask<CompositionBrush>(brush);
            }

            return new PipelineBuilder(Factory);
        }

        /// <summary>
        /// Starts a new <see cref="PipelineBuilder"/> pipeline from a solid <see cref="CompositionBrush"/> with the specified color
        /// </summary>
        /// <param name="color">The desired color for the initial <see cref="CompositionBrush"/></param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromColor(Color color)
        {
            return new PipelineBuilder(() => new ValueTask<IGraphicsEffectSource>(new ColorSourceEffect { Color = color }));
        }

        /// <summary>
        /// Starts a new <see cref="PipelineBuilder"/> pipeline from a solid <see cref="CompositionBrush"/> with the specified color
        /// </summary>
        /// <param name="color">The desired color for the initial <see cref="CompositionBrush"/></param>
        /// <param name="setter">The optional color setter for the effect</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromColor(Color color, out EffectSetter<Color> setter)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            ValueTask<IGraphicsEffectSource> Factory() => new ValueTask<IGraphicsEffectSource>(new ColorSourceEffect
            {
                Color = color,
                Name = id
            });

            setter = (brush, value) => brush.Properties.InsertColor($"{id}.{nameof(ColorSourceEffect.Color)}", value);

            return new PipelineBuilder(Factory, new[] { $"{id}.{nameof(ColorSourceEffect.Color)}" });
        }

        /// <summary>
        /// Starts a new <see cref="PipelineBuilder"/> pipeline from a solid <see cref="CompositionBrush"/> with the specified color
        /// </summary>
        /// <param name="color">The desired color for the initial <see cref="CompositionBrush"/></param>
        /// <param name="animation">The optional color animation for the effect</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromColor(Color color, out EffectAnimation<Color> animation)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            ValueTask<IGraphicsEffectSource> Factory() => new ValueTask<IGraphicsEffectSource>(new ColorSourceEffect
            {
                Color = color,
                Name = id
            });

            animation = (brush, value, duration) => brush.StartAnimationAsync($"{id}.{nameof(ColorSourceEffect.Color)}", value, duration);

            return new PipelineBuilder(Factory, new[] { $"{id}.{nameof(ColorSourceEffect.Color)}" });
        }

        /// <summary>
        /// Starts a new <see cref="PipelineBuilder"/> pipeline from a solid <see cref="CompositionBrush"/> with the specified color
        /// </summary>
        /// <param name="color">The desired color for the initial <see cref="CompositionBrush"/></param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromHdrColor(Vector4 color)
        {
            return new PipelineBuilder(() => new ValueTask<IGraphicsEffectSource>(new ColorSourceEffect { ColorHdr = color }));
        }

        /// <summary>
        /// Starts a new <see cref="PipelineBuilder"/> pipeline from a solid <see cref="CompositionBrush"/> with the specified color
        /// </summary>
        /// <param name="color">The desired color for the initial <see cref="CompositionBrush"/></param>
        /// <param name="setter">The optional color setter for the effect</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromHdrColor(Vector4 color, out EffectSetter<Vector4> setter)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            ValueTask<IGraphicsEffectSource> Factory() => new ValueTask<IGraphicsEffectSource>(new ColorSourceEffect
            {
                ColorHdr = color,
                Name = id
            });

            setter = (brush, value) => brush.Properties.InsertVector4($"{id}.{nameof(ColorSourceEffect.ColorHdr)}", value);

            return new PipelineBuilder(Factory, new[] { $"{id}.{nameof(ColorSourceEffect.ColorHdr)}" });
        }

        /// <summary>
        /// Starts a new <see cref="PipelineBuilder"/> pipeline from a solid <see cref="CompositionBrush"/> with the specified color
        /// </summary>
        /// <param name="color">The desired color for the initial <see cref="CompositionBrush"/></param>
        /// <param name="animation">The optional color animation for the effect</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromHdrColor(Vector4 color, out EffectAnimation<Vector4> animation)
        {
            string id = Guid.NewGuid().ToUppercaseAsciiLetters();

            ValueTask<IGraphicsEffectSource> Factory() => new ValueTask<IGraphicsEffectSource>(new ColorSourceEffect
            {
                ColorHdr = color,
                Name = id
            });

            animation = (brush, value, duration) => brush.StartAnimationAsync($"{id}.{nameof(ColorSourceEffect.ColorHdr)}", value, duration);

            return new PipelineBuilder(Factory, new[] { $"{id}.{nameof(ColorSourceEffect.ColorHdr)}" });
        }

        /// <summary>
        /// Starts a new <see cref="PipelineBuilder"/> pipeline from the input <see cref="CompositionBrush"/> instance
        /// </summary>
        /// <param name="brush">A <see cref="CompositionBrush"/> instance to start the pipeline</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromBrush(CompositionBrush brush)
        {
            return new PipelineBuilder(() => new ValueTask<CompositionBrush>(brush));
        }

        /// <summary>
        /// Starts a new <see cref="PipelineBuilder"/> pipeline from the input <see cref="CompositionBrush"/> instance
        /// </summary>
        /// <param name="factory">A <see cref="Func{TResult}"/> that synchronously returns a <see cref="CompositionBrush"/> instance to start the pipeline</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromBrush(Func<CompositionBrush> factory)
        {
            return new PipelineBuilder(() => new ValueTask<CompositionBrush>(factory()));
        }

        /// <summary>
        /// Starts a new <see cref="PipelineBuilder"/> pipeline from the input <see cref="CompositionBrush"/> instance
        /// </summary>
        /// <param name="factory">A <see cref="Func{TResult}"/> that asynchronously returns a <see cref="CompositionBrush"/> instance to start the pipeline</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromBrush(Func<Task<CompositionBrush>> factory)
        {
            async ValueTask<CompositionBrush> Factory() => await factory();

            return new PipelineBuilder(Factory);
        }

        /// <summary>
        /// Starts a new <see cref="PipelineBuilder"/> pipeline from the input <see cref="IGraphicsEffectSource"/> instance
        /// </summary>
        /// <param name="effect">A <see cref="IGraphicsEffectSource"/> instance to start the pipeline</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromEffect(IGraphicsEffectSource effect)
        {
            return new PipelineBuilder(() => new ValueTask<IGraphicsEffectSource>(effect));
        }

        /// <summary>
        /// Starts a new <see cref="PipelineBuilder"/> pipeline from the input <see cref="IGraphicsEffectSource"/> instance
        /// </summary>
        /// <param name="factory">A <see cref="Func{TResult}"/> that synchronously returns a <see cref="IGraphicsEffectSource"/> instance to start the pipeline</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromEffect(Func<IGraphicsEffectSource> factory)
        {
            return new PipelineBuilder(() => new ValueTask<IGraphicsEffectSource>(factory()));
        }

        /// <summary>
        /// Starts a new <see cref="PipelineBuilder"/> pipeline from the input <see cref="IGraphicsEffectSource"/> instance
        /// </summary>
        /// <param name="factory">A <see cref="Func{TResult}"/> that asynchronously returns a <see cref="IGraphicsEffectSource"/> instance to start the pipeline</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromEffect(Func<Task<IGraphicsEffectSource>> factory)
        {
            async ValueTask<IGraphicsEffectSource> Factory() => await factory();

            return new PipelineBuilder(Factory);
        }

        /// <summary>
        /// Starts a new <see cref="PipelineBuilder"/> pipeline from a Win2D image
        /// </summary>
        /// <param name="relativePath">The relative path for the image to load (eg. "/Assets/image.png")</param>
        /// <param name="dpiMode">Indicates the desired DPI mode to use when loading the image</param>
        /// <param name="cacheMode">The cache mode to use to load the image</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromImage(string relativePath, DpiMode dpiMode = DpiMode.DisplayDpiWith96AsLowerBound, CacheMode cacheMode = CacheMode.Default)
        {
            return FromImage(relativePath.ToAppxUri(), dpiMode, cacheMode);
        }

        /// <summary>
        /// Starts a new <see cref="PipelineBuilder"/> pipeline from a Win2D image
        /// </summary>
        /// <param name="uri">The path for the image to load</param>
        /// <param name="dpiMode">Indicates the desired DPI mode to use when loading the image</param>
        /// <param name="cacheMode">The cache mode to use to load the image</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromImage(Uri uri, DpiMode dpiMode = DpiMode.DisplayDpiWith96AsLowerBound, CacheMode cacheMode = CacheMode.Default)
        {
            return new PipelineBuilder(() => new ValueTask<CompositionBrush>(SurfaceLoader.LoadImageAsync(uri, dpiMode, cacheMode)));
        }

        /// <summary>
        /// Starts a new <see cref="PipelineBuilder"/> pipeline from a Win2D image tiled to cover the available space
        /// </summary>
        /// <param name="relativePath">The relative path for the image to load (eg. "/Assets/image.png")</param>
        /// <param name="dpiMode">Indicates the desired DPI mode to use when loading the image</param>
        /// <param name="cacheMode">The cache mode to use to load the image</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromTiles(string relativePath, DpiMode dpiMode = DpiMode.DisplayDpiWith96AsLowerBound, CacheMode cacheMode = CacheMode.Default)
        {
            return FromTiles(relativePath.ToAppxUri(), dpiMode, cacheMode);
        }

        /// <summary>
        /// Starts a new <see cref="PipelineBuilder"/> pipeline from a Win2D image tiled to cover the available space
        /// </summary>
        /// <param name="uri">The path for the image to load</param>
        /// <param name="dpiMode">Indicates the desired DPI mode to use when loading the image</param>
        /// <param name="cacheMode">The cache mode to use to load the image</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromTiles(Uri uri, DpiMode dpiMode = DpiMode.DisplayDpiWith96AsLowerBound, CacheMode cacheMode = CacheMode.Default)
        {
            var image = FromImage(uri, dpiMode, cacheMode);

            async ValueTask<IGraphicsEffectSource> Factory() => new BorderEffect
            {
                ExtendX = CanvasEdgeBehavior.Wrap,
                ExtendY = CanvasEdgeBehavior.Wrap,
                Source = await image.sourceProducer()
            };

            return new PipelineBuilder(image, Factory);
        }

        /// <summary>
        /// Starts a new <see cref="PipelineBuilder"/> pipeline from the <see cref="CompositionBrush"/> returned by <see cref="Compositor.CreateBackdropBrush"/> on the input <see cref="UIElement"/>
        /// </summary>
        /// <param name="element">The source <see cref="UIElement"/> to use to create the pipeline</param>
        /// <returns>A new <see cref="PipelineBuilder"/> instance to use to keep adding new effects</returns>
        [Pure]
        public static PipelineBuilder FromUIElement(UIElement element)
        {
            return new PipelineBuilder(() => new ValueTask<CompositionBrush>(ElementCompositionPreview.GetElementVisual(element).Compositor.CreateBackdropBrush()));
        }
    }
}
