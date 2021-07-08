// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// The <see cref="ImageCropperThumb"/> control is used for <see cref="ImageCropper"/>.
    /// </summary>
    public class ImageCropperThumb : Control
    {
        private readonly TranslateTransform _layoutTransform = new TranslateTransform();

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageCropperThumb"/> class.
        /// </summary>
        public ImageCropperThumb()
        {
            DefaultStyleKey = typeof(ImageCropperThumb);
            RenderTransform = _layoutTransform;
            ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            SizeChanged += ImageCropperThumb_SizeChanged;
        }

        private void ImageCropperThumb_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdatePosition();
        }

        internal ThumbPosition Position { get; set; }

        private void UpdatePosition()
        {
            if (_layoutTransform != null)
            {
                _layoutTransform.X = X - (this.ActualWidth / 2);
                _layoutTransform.Y = Y - (this.ActualHeight / 2);
            }
        }

        /// <summary>
        /// Gets or sets the X coordinate of the ImageCropperThumb.
        /// </summary>
        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        private static void OnXChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageCropperThumb)d;
            target.UpdatePosition();
        }

        /// <summary>
        /// Gets or sets the Y coordinate of the ImageCropperThumb.
        /// </summary>
        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        private static void OnYChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageCropperThumb)d;
            target.UpdatePosition();
        }

        /// <summary>
        /// Identifies the <see cref="X"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register(nameof(X), typeof(double), typeof(ImageCropperThumb), new PropertyMetadata(0d, OnXChanged));

        /// <summary>
        /// Identifies the <see cref="Y"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register(nameof(Y), typeof(double), typeof(ImageCropperThumb), new PropertyMetadata(0d, OnYChanged));
    }
}