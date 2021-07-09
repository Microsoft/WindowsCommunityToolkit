// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// The <see cref="ConstrainedPresenter"/> is a <see cref="ContentPresenter"/> control which can restrict the
    /// available size for its content based on a scale factor and/or a specific <see cref="AspectRatio"/>.
    /// </summary>
    public class ConstrainedPresenter : ContentPresenter // TODO: Think it should be a Border? But it's sealed; hopefully can change in WinUI 3.
    {
        /// <summary>
        /// Gets or sets aspect Ratio to use for the contents of the Panel (after scaling).
        /// </summary>
        public AspectRatio AspectRatio
        {
            get { return (AspectRatio)GetValue(AspectRatioProperty); }
            set { SetValue(AspectRatioProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AspectRatio"/> property.
        /// </summary>
        public static readonly DependencyProperty AspectRatioProperty =
            DependencyProperty.Register(nameof(AspectRatio), typeof(AspectRatio), typeof(ConstrainedPresenter), new PropertyMetadata(null, AspectRatioPropertyChanged));

        private static void AspectRatioPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ConstrainedPresenter panel)
            {
                panel.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets or sets the scale for the width of the panel. Should be a value between 0-1.0. Default is 1.0.
        /// </summary>
        public double ScaleX
        {
            get { return (double)GetValue(ScaleXProperty); }
            set { SetValue(ScaleXProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ScaleX"/> property.
        /// </summary>
        public static readonly DependencyProperty ScaleXProperty =
            DependencyProperty.Register(nameof(ScaleX), typeof(double), typeof(ConstrainedPresenter), new PropertyMetadata(1.0, ScalePropertyChanged));

        /// <summary>
        /// Gets or sets the scale for the height of the panel. Should be a value between 0-1.0. Default is 1.0.
        /// </summary>
        public double ScaleY
        {
            get { return (double)GetValue(ScaleYProperty); }
            set { SetValue(ScaleYProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ScaleY"/> property.
        /// </summary>
        public static readonly DependencyProperty ScaleYProperty =
            DependencyProperty.Register(nameof(ScaleY), typeof(double), typeof(ConstrainedPresenter), new PropertyMetadata(1.0, ScalePropertyChanged));

        private static void ScalePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ConstrainedPresenter panel)
            {
                panel.InvalidateMeasure();
            }
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(CalculateConstrainedSize(availableSize));
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(CalculateConstrainedSize(finalSize));
        }

        private Size CalculateConstrainedSize(Size initialSize)
        {
            var availableSize = new Size(initialSize.Width * ScaleX, initialSize.Height * ScaleY);

            // If we don't have an Aspect Ratio, just return the scaled value.
            if (ReadLocalValue(AspectRatioProperty) == DependencyProperty.UnsetValue)
            {
                return availableSize;
            }

            // Calculate the Aspect Ratio constraint based on the newly scaled size.
            var currentAspect = availableSize.Width / availableSize.Height;
            var desiredAspect = AspectRatio.Value;

            if (currentAspect >= desiredAspect)
            {
                return new Size(availableSize.Height * desiredAspect, availableSize.Height);
            }
            else
            {
                return new Size(availableSize.Width, availableSize.Width / desiredAspect);
            }
        }
    }
}
