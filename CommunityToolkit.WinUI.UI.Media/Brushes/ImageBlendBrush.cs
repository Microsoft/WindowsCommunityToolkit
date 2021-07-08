// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//// Image loading reference from https://blogs.windows.com/buildingapps/2017/07/18/working-brushes-content-xaml-visual-layer-interop-part-one/#MA0k4EYWzqGKV501.97

using System;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using CanvasBlendEffect = Microsoft.Graphics.Canvas.Effects.BlendEffect;

namespace CommunityToolkit.WinUI.UI.Media
{
    /// <summary>
    /// Brush which blends a <see cref="BitmapImage"/> to the Backdrop in a given mode. See http://microsoft.github.io/Win2D/html/T_Microsoft_Graphics_Canvas_Effects_BlendEffect.htm.
    /// </summary>
    public class ImageBlendBrush : XamlCompositionBrushBase
    {
        private LoadedImageSurface _surface;
        private CompositionSurfaceBrush _surfaceBrush;

        /// <summary>
        /// Gets or sets the <see cref="BitmapImage"/> source of the image to composite.
        /// </summary>
        public ImageSource Source
        {
            get => (ImageSource)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Source"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            nameof(Source),
            typeof(ImageSource), // We use ImageSource type so XAML engine will automatically construct proper object from String.
            typeof(ImageBlendBrush),
            new PropertyMetadata(null, OnImageSourceChanged));

        /// <summary>
        /// Gets or sets how to stretch the image within the brush.
        /// </summary>
        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Stretch"/> dependency property.
        /// Requires 16299 or higher for modes other than None.
        /// </summary>
        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(
            nameof(Stretch),
            typeof(Stretch),
            typeof(ImageBlendBrush),
            new PropertyMetadata(Stretch.None, OnStretchChanged));

        /// <summary>
        /// Gets or sets how to blend the image with the backdrop.
        /// </summary>
        public ImageBlendMode Mode
        {
            get => (ImageBlendMode)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Mode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(
            nameof(Mode),
            typeof(ImageBlendMode),
            typeof(ImageBlendBrush),
            new PropertyMetadata(ImageBlendMode.Multiply, OnModeChanged));

        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var brush = (ImageBlendBrush)d;

            // Unbox and update surface if CompositionBrush exists
            if (brush._surfaceBrush != null)
            {
                // If UriSource is invalid, StartLoadFromUri will return a blank texture.
                var uri = (e.NewValue as BitmapImage)?.UriSource ?? new Uri("ms-appx:///");
                var newSurface = LoadedImageSurface.StartLoadFromUri(uri);

                brush._surface = newSurface;
                brush._surfaceBrush.Surface = newSurface;
            }
            else
            {
                // If we didn't initially have a valid surface, we need to recreate our effect now.
                brush.OnDisconnected();
                brush.OnConnected();
            }
        }

        private static void OnStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var brush = (ImageBlendBrush)d;

            // Unbox and update surface if CompositionBrush exists
            if (brush._surfaceBrush != null)
            {
                // Modify the stretch property on our brush.
                brush._surfaceBrush.Stretch = CompositionStretchFromStretch((Stretch)e.NewValue);
            }
        }

        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var brush = (ImageBlendBrush)d;

            // We can't animate our enum properties so recreate our internal brush.
            brush.OnDisconnected();
            brush.OnConnected();
        }

        /// <inheritdoc/>
        protected override void OnConnected()
        {
            // Delay creating composition resources until they're required.
            if (CompositionBrush == null && Source != null && Source is BitmapImage bitmap)
            {
                // Use LoadedImageSurface API to get ICompositionSurface from image uri provided
                // If UriSource is invalid, StartLoadFromUri will return a blank texture.
                _surface = LoadedImageSurface.StartLoadFromUri(bitmap.UriSource);

                // Load Surface onto SurfaceBrush
                _surfaceBrush = CompositionTarget.GetCompositorForCurrentThread().CreateSurfaceBrush(_surface);
                _surfaceBrush.Stretch = CompositionStretchFromStretch(Stretch);

                // Abort if effects aren't supported.
                var compositionCapabilities = new CompositionCapabilities();
                if (!compositionCapabilities.AreEffectsSupported())
                {
                    // Just use image straight-up, if we don't support effects.
                    CompositionBrush = _surfaceBrush;
                    return;
                }

                var backdrop = CompositionTarget.GetCompositorForCurrentThread().CreateBackdropBrush();

                // Use a Win2D invert affect applied to a CompositionBackdropBrush.
                var graphicsEffect = new CanvasBlendEffect
                {
                    Name = "Invert",
                    Mode = (BlendEffectMode)(int)Mode,
                    Background = new CompositionEffectSourceParameter("backdrop"),
                    Foreground = new CompositionEffectSourceParameter("image")
                };

                var effectFactory = CompositionTarget.GetCompositorForCurrentThread().CreateEffectFactory(graphicsEffect);
                var effectBrush = effectFactory.CreateBrush();

                effectBrush.SetSourceParameter("backdrop", backdrop);
                effectBrush.SetSourceParameter("image", _surfaceBrush);

                CompositionBrush = effectBrush;
            }
        }

        /// <inheritdoc/>
        protected override void OnDisconnected()
        {
            // Dispose of composition resources when no longer in use.
            if (CompositionBrush != null)
            {
                CompositionBrush.Dispose();
                CompositionBrush = null;
            }

            if (_surfaceBrush != null)
            {
                _surfaceBrush.Dispose();
                _surfaceBrush = null;
            }

            if (_surface != null)
            {
                _surface.Dispose();
                _surface = null;
            }
        }

        //// Helper to allow XAML developer to use XAML stretch property rather than another enum.
        private static CompositionStretch CompositionStretchFromStretch(Stretch value)
        {
            switch (value)
            {
                case Stretch.None:
                    return CompositionStretch.None;
                case Stretch.Fill:
                    return CompositionStretch.Fill;
                case Stretch.Uniform:
                    return CompositionStretch.Uniform;
                case Stretch.UniformToFill:
                    return CompositionStretch.UniformToFill;
            }

            return CompositionStretch.None;
        }
    }
}