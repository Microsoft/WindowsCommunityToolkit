// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.WinUI.UI.Media.Helpers;
using CommunityToolkit.WinUI.UI.Media.Pipelines;
using Microsoft.UI.Xaml;

namespace CommunityToolkit.WinUI.UI.Media
{
    /// <summary>
    /// A <see cref="XamlCompositionBrush"/> that displays a tiled image
    /// </summary>
    public sealed class TilesBrush : XamlCompositionEffectBrushBase
    {
        /// <summary>
        /// Gets or sets the <see cref="Uri"/> to the texture to use
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
            typeof(TilesBrush),
            new PropertyMetadata(default, OnDependencyPropertyChanged));

        /// <summary>
        /// Gets or sets the DPI mode used to render the texture (the default is <see cref="Media.DpiMode.DisplayDpiWith96AsLowerBound"/>)
        /// </summary>
        public DpiMode DpiMode
        {
            get => (DpiMode)GetValue(DpiModeProperty);
            set => SetValue(DpiModeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DpiMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DpiModeProperty = DependencyProperty.Register(
            nameof(DpiMode),
            typeof(DpiMode),
            typeof(TilesBrush),
            new PropertyMetadata(DpiMode.DisplayDpiWith96AsLowerBound, OnDependencyPropertyChanged));

        /// <summary>
        /// Updates the UI when either <see cref="TextureUri"/> or <see cref="DpiMode"/> changes
        /// </summary>
        /// <param name="d">The current <see cref="TilesBrush"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance for <see cref="TextureUriProperty"/> or <see cref="DpiModeProperty"/></param>
        private static void OnDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TilesBrush brush &&
                brush.CompositionBrush != null)
            {
                brush.OnDisconnected();
                brush.OnConnected();
            }
        }

        /// <inheritdoc/>
        protected override PipelineBuilder OnPipelineRequested()
        {
            if (TextureUri is Uri uri)
            {
                float dpi = DpiHelpers.GetDpi(this);

                return PipelineBuilder.FromTiles(uri, dpi, DpiMode);
            }

            return PipelineBuilder.FromColor(default);
        }
    }
}