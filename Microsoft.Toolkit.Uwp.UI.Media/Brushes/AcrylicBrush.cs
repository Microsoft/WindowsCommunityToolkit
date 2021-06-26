// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Toolkit.Uwp.UI.Media.Pipelines;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.UI.Media
{
    /// <summary>
    /// A <see cref="XamlCompositionBrush"/> that implements an acrylic effect with customizable parameters
    /// </summary>
    public sealed class AcrylicBrush : XamlCompositionEffectBrushBase
    {
        /// <summary>
        /// The <see cref="EffectSetter{T}"/> instance in use to set the blur amount
        /// </summary>
        /// <remarks>This is only set when <see cref="BackgroundSource"/> is <see cref="AcrylicBackgroundSource.Backdrop"/></remarks>
        private EffectSetter<float> blurAmountSetter;

        /// <summary>
        /// The <see cref="EffectSetter{T}"/> instance in use to set the tint color
        /// </summary>
        private EffectSetter<Color> tintColorSetter;

        /// <summary>
        /// The <see cref="EffectSetter{T}"/> instance in use to set the tint mix amount
        /// </summary>
        private EffectSetter<float> tintOpacitySetter;

        /// <summary>
        /// Gets or sets the background source mode for the effect (the default is <see cref="AcrylicBackgroundSource.Backdrop"/>).
        /// </summary>
        public AcrylicBackgroundSource BackgroundSource
        {
            get => (AcrylicBackgroundSource)GetValue(BackgroundSourceProperty);
            set => SetValue(BackgroundSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="BackgroundSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundSourceProperty = DependencyProperty.Register(
            nameof(BackgroundSource),
            typeof(AcrylicBackgroundSource),
            typeof(AcrylicBrush),
            new PropertyMetadata(AcrylicBackgroundSource.Backdrop, OnSourcePropertyChanged));

        /// <summary>
        /// Updates the UI when <see cref="BackgroundSource"/> changes
        /// </summary>
        /// <param name="d">The current <see cref="AcrylicBrush"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance for <see cref="BackgroundSourceProperty"/></param>
        private static void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AcrylicBrush brush &&
                brush.CompositionBrush != null)
            {
                brush.OnDisconnected();
                brush.OnConnected();
            }
        }

        /// <summary>
        /// Gets or sets the blur amount for the effect (must be a positive value)
        /// </summary>
        /// <remarks>This property is ignored when the active mode is <see cref="AcrylicBackgroundSource.HostBackdrop"/></remarks>
        public double BlurAmount
        {
            get => (double)GetValue(BlurAmountProperty);
            set => SetValue(BlurAmountProperty, Math.Max(value, 0));
        }

        /// <summary>
        /// Identifies the <see cref="BlurAmount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BlurAmountProperty = DependencyProperty.Register(
            nameof(BlurAmount),
            typeof(double),
            typeof(AcrylicBrush),
            new PropertyMetadata(0.0, OnBlurAmountPropertyChanged));

        /// <summary>
        /// Updates the UI when <see cref="BackgroundSource"/> changes
        /// </summary>
        /// <param name="d">The current <see cref="AcrylicBrush"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance for <see cref="BackgroundSourceProperty"/></param>
        private static void OnBlurAmountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AcrylicBrush brush &&
                brush.BackgroundSource != AcrylicBackgroundSource.HostBackdrop && // Blur is fixed by OS when using HostBackdrop source.
                brush.CompositionBrush is CompositionBrush target)
            {
                brush.blurAmountSetter?.Invoke(target, (float)(double)e.NewValue);
            }
        }

        /// <summary>
        /// Gets or sets the tint for the effect
        /// </summary>
        public Color TintColor
        {
            get => (Color)GetValue(TintColorProperty);
            set => SetValue(TintColorProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TintColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TintColorProperty = DependencyProperty.Register(
            nameof(TintColor),
            typeof(Color),
            typeof(AcrylicBrush),
            new PropertyMetadata(default(Color), OnTintColorPropertyChanged));

        /// <summary>
        /// Updates the UI when <see cref="TintColor"/> changes
        /// </summary>
        /// <param name="d">The current <see cref="AcrylicBrush"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance for <see cref="TintColorProperty"/></param>
        private static void OnTintColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AcrylicBrush brush &&
                brush.CompositionBrush is CompositionBrush target)
            {
                brush.tintColorSetter?.Invoke(target, (Color)e.NewValue);
            }
        }

        /// <summary>
        /// Gets or sets the tint opacity factor for the effect (default is 0.5, must be in the [0, 1] range)
        /// </summary>
        public double TintOpacity
        {
            get => (double)GetValue(TintOpacityProperty);
            set => SetValue(TintOpacityProperty, Math.Clamp(value, 0, 1));
        }

        /// <summary>
        /// Identifies the <see cref="TintOpacity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TintOpacityProperty = DependencyProperty.Register(
            nameof(TintOpacity),
            typeof(double),
            typeof(AcrylicBrush),
            new PropertyMetadata(0.5, OnTintOpacityPropertyChanged));

        /// <summary>
        /// Updates the UI when <see cref="TintOpacity"/> changes
        /// </summary>
        /// <param name="d">The current <see cref="AcrylicBrush"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance for <see cref="TintOpacityProperty"/></param>
        private static void OnTintOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AcrylicBrush brush &&
                brush.CompositionBrush is CompositionBrush target)
            {
                brush.tintOpacitySetter?.Invoke(target, (float)(double)e.NewValue);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Uri"/> for the texture to use
        /// </summary>
        public Uri TextureUri
        {
            get => (Uri)GetValue(TextureUriProperty);
            set => SetValue(TextureUriProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TextureUri"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextureUriProperty = DependencyProperty.Register(
            nameof(TextureUri),
            typeof(Uri),
            typeof(AcrylicBrush),
            new PropertyMetadata(default, OnTextureUriPropertyChanged));

        /// <summary>
        /// Updates the UI when <see cref="TextureUri"/> changes
        /// </summary>
        /// <param name="d">The current <see cref="AcrylicBrush"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance for <see cref="TextureUriProperty"/></param>
        private static void OnTextureUriPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AcrylicBrush brush &&
                brush.CompositionBrush != null)
            {
                brush.OnDisconnected();
                brush.OnConnected();
            }
        }

        /// <inheritdoc/>
        protected override PipelineBuilder OnPipelineRequested()
        {
            switch (BackgroundSource)
            {
                case AcrylicBackgroundSource.Backdrop:
                    return PipelineBuilder.FromBackdropAcrylic(
                        TintColor,
                        out this.tintColorSetter,
                        (float)TintOpacity,
                        out this.tintOpacitySetter,
                        (float)BlurAmount,
                        out blurAmountSetter,
                        TextureUri);
                case AcrylicBackgroundSource.HostBackdrop:
                    return PipelineBuilder.FromHostBackdropAcrylic(
                        TintColor,
                        out this.tintColorSetter,
                        (float)TintOpacity,
                        out this.tintOpacitySetter,
                        TextureUri);
                default: throw new ArgumentOutOfRangeException(nameof(BackgroundSource), $"Invalid acrylic source: {BackgroundSource}");
            }
        }
    }
}