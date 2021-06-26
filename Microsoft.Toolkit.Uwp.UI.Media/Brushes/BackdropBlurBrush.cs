// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//// Example brush from https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.media.xamlcompositionbrushbase

using Microsoft.Toolkit.Uwp.UI.Media.Pipelines;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.UI.Media
{
    /// <summary>
    /// The <see cref="BackdropBlurBrush"/> is a <see cref="Brush"/> that blurs whatever is behind it in the application.
    /// </summary>
    public class BackdropBlurBrush : XamlCompositionEffectBrushBase
    {
        /// <summary>
        /// The <see cref="EffectSetter{T}"/> instance currently in use
        /// </summary>
        private EffectSetter<float> amountSetter;

        /// <summary>
        /// Gets or sets the amount of gaussian blur to apply to the background.
        /// </summary>
        public double Amount
        {
            get => (double)GetValue(AmountProperty);
            set => SetValue(AmountProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Amount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AmountProperty = DependencyProperty.Register(
            nameof(Amount),
            typeof(double),
            typeof(BackdropBlurBrush),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnAmountChanged)));

        /// <summary>
        /// Updates the UI when <see cref="Amount"/> changes
        /// </summary>
        /// <param name="d">The current <see cref="BackdropBlurBrush"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance for <see cref="AmountProperty"/></param>
        private static void OnAmountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BackdropBlurBrush brush &&
                brush.CompositionBrush is CompositionBrush target)
            {
                brush.amountSetter?.Invoke(target, (float)brush.Amount);
            }
        }

        /// <inheritdoc/>
        protected override PipelineBuilder OnPipelineRequested()
        {
            return PipelineBuilder.FromBackdrop().Blur((float)Amount, out this.amountSetter);
        }
    }
}