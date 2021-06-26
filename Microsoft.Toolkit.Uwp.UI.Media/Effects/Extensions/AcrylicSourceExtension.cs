// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Toolkit.Uwp.UI.Media.Pipelines;
using Windows.UI;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.UI.Media
{
    /// <summary>
    /// A custom acrylic effect that can be inserted into a pipeline
    /// </summary>
    /// <remarks>This effect mirrors the look of the default <see cref="AcrylicBrush"/> implementation</remarks>
    [MarkupExtensionReturnType(ReturnType = typeof(PipelineBuilder))]
    public sealed class AcrylicSourceExtension : MarkupExtension
    {
        /// <summary>
        /// Gets or sets the background source mode for the effect (the default is <see cref="AcrylicBackgroundSource.Backdrop"/>).
        /// </summary>
        public AcrylicBackgroundSource BackgroundSource { get; set; } = AcrylicBackgroundSource.Backdrop;

        private double blurAmount;

        /// <summary>
        /// Gets or sets the blur amount for the effect (must be a positive value)
        /// </summary>
        /// <remarks>This property is ignored when the active mode is <see cref="AcrylicBackgroundSource.HostBackdrop"/></remarks>
        public double BlurAmount
        {
            get => this.blurAmount;
            set => this.blurAmount = Math.Max(value, 0);
        }

        /// <summary>
        /// Gets or sets the tint for the effect
        /// </summary>
        public Color TintColor { get; set; }

        private double tintOpacity = 0.5f;

        /// <summary>
        /// Gets or sets the color for the tint effect (default is 0.5, must be in the [0, 1] range)
        /// </summary>
        public double TintOpacity
        {
            get => this.tintOpacity;
            set => this.tintOpacity = Math.Clamp(value, 0, 1);
        }

        /// <summary>
        /// Gets or sets the <see cref="Uri"/> to the texture to use
        /// </summary>
        public Uri TextureUri { get; set; }

        /// <inheritdoc/>
        protected override object ProvideValue()
        {
            return BackgroundSource switch
            {
                AcrylicBackgroundSource.Backdrop => PipelineBuilder.FromBackdropAcrylic(this.TintColor, (float)this.TintOpacity, (float)BlurAmount, TextureUri),
                AcrylicBackgroundSource.HostBackdrop => PipelineBuilder.FromHostBackdropAcrylic(this.TintColor, (float)this.TintOpacity, TextureUri),
                _ => throw new ArgumentException($"Invalid source mode for acrylic effect: {BackgroundSource}")
            };
        }
    }
}