// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.UI.Media.Pipelines;
using Windows.UI.Composition;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.UI.Media
{
    /// <summary>
    /// Brush which applies a SepiaEffect to the Backdrop. http://microsoft.github.io/Win2D/html/T_Microsoft_Graphics_Canvas_Effects_SepiaEffect.htm
    /// </summary>
    public class BackdropSepiaBrush : XamlCompositionEffectBrushBase
    {
        /// <summary>
        /// The <see cref="EffectSetter{T}"/> instance currently in use
        /// </summary>
        private EffectSetter<float> setter;

        /// <summary>
        /// Gets or sets the amount of gaussian blur to apply to the background.
        /// </summary>
        public double Intensity
        {
            get => (double)GetValue(IntensityProperty);
            set => SetValue(IntensityProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Intensity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IntensityProperty = DependencyProperty.Register(
            nameof(Intensity),
            typeof(double),
            typeof(BackdropSepiaBrush),
            new PropertyMetadata(0.5, new PropertyChangedCallback(OnIntensityChanged)));

        /// <summary>
        /// Updates the UI when <see cref="Intensity"/> changes
        /// </summary>
        /// <param name="d">The current <see cref="BackdropSepiaBrush"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance for <see cref="IntensityProperty"/></param>
        private static void OnIntensityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var brush = (BackdropSepiaBrush)d;

            // Clamp Value as per docs http://microsoft.github.io/Win2D/html/T_Microsoft_Graphics_Canvas_Effects_SepiaEffect.htm
            var value = (float)(double)e.NewValue;
            if (value > 1.0)
            {
                brush.Intensity = 1.0;
            }
            else if (value < 0.0)
            {
                brush.Intensity = 0.0;
            }

            // Unbox and set a new blur amount if the CompositionBrush exists.
            if (brush.CompositionBrush is CompositionBrush target)
            {
                brush.setter?.Invoke(target, (float)brush.Intensity);
            }
        }

        /// <inheritdoc/>
        protected override PipelineBuilder OnPipelineRequested()
        {
            return PipelineBuilder.FromBackdrop().Sepia((float)Intensity, out setter);
        }
    }
}