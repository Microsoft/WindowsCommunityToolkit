// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Foundation;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// The <see cref="ImageCropper"/> control allows user to crop image freely.
    /// </summary>
    public partial class ImageCropper
    {
        /// <summary>
        /// Gets or sets the minimum cropped length(in pixel).
        /// </summary>
        public double MinCroppedPixelLength { get; set; } = 40;

        /// <summary>
        /// Gets or sets the minimum selectable length.
        /// </summary>
        public double MinSelectedLength { get; set; } = 40;

        /// <summary>
        /// Gets the current cropped region.
        /// </summary>
        public Rect CroppedRegion => _currentCroppedRect;

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageCropper)d;
            if (e.NewValue is WriteableBitmap bitmap)
            {
                if (bitmap.PixelWidth < target.MinCropSize.Width || bitmap.PixelHeight < target.MinCropSize.Height)
                {
                    target.Source = null;
                    throw new ArgumentException("The resolution of the image is too small!");
                }
            }

            target.InvalidateMeasure();
            target.UpdateCropShape();
            target.InitImageLayout();
        }

        private static void OnAspectRatioChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageCropper)d;
            target.UpdateAspectRatio(true);
        }

        private static void OnCropShapeChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageCropper)d;
            target.UpdateCropShape();
            target.UpdateThumbsVisibility();
            target.UpdateAspectRatio();
            target.UpdateMaskArea();
        }

        private static void OnThumbPlacementChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageCropper)d;
            target.UpdateThumbsVisibility();
        }

        /// <summary>
        ///  Gets or sets the source of the cropped image.
        /// </summary>
        public WriteableBitmap Source
        {
            get { return (WriteableBitmap)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the aspect ratio of the cropped image, the default value is null.
        /// Only works when <see cref="CropShape"/> = <see cref="CropShape.Rectangular"/>.
        /// </summary>
        public double? AspectRatio
        {
            get { return (double?)GetValue(AspectRatioProperty); }
            set { SetValue(AspectRatioProperty, value); }
        }

        /// <summary>
        /// Gets or sets the shape to use when cropping.
        /// </summary>
        public CropShape CropShape
        {
            get { return (CropShape)GetValue(CropShapeProperty); }
            set { SetValue(CropShapeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the mask on the cropped image.
        /// </summary>
        public Brush Mask
        {
            get { return (Brush)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value for the style to use for the primary thumbs of the ImageCropper.
        /// </summary>
        public Style PrimaryThumbStyle
        {
            get { return (Style)GetValue(PrimaryThumbStyleProperty); }
            set { SetValue(PrimaryThumbStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value for the style to use for the secondary thumbs of the ImageCropper.
        /// </summary>
        public Style SecondaryThumbStyle
        {
            get { return (Style)GetValue(SecondaryThumbStyleProperty); }
            set { SetValue(SecondaryThumbStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value for thumb placement.
        /// </summary>
        public ThumbPlacement ThumbPlacement
        {
            get { return (ThumbPlacement)GetValue(ThumbPlacementProperty); }
            set { SetValue(ThumbPlacementProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AspectRatio"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AspectRatioProperty =
            DependencyProperty.Register(nameof(AspectRatio), typeof(double?), typeof(ImageCropper), new PropertyMetadata(null, OnAspectRatioChanged));

        /// <summary>
        /// Identifies the <see cref="Source"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(WriteableBitmap), typeof(ImageCropper), new PropertyMetadata(null, OnSourceChanged));

        /// <summary>
        /// Identifies the <see cref="CropShape"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CropShapeProperty =
            DependencyProperty.Register(nameof(CropShape), typeof(CropShape), typeof(ImageCropper), new PropertyMetadata(default(CropShape), OnCropShapeChanged));

        /// <summary>
        /// Identifies the <see cref="Mask"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaskProperty =
            DependencyProperty.Register(nameof(Mask), typeof(Brush), typeof(ImageCropper), new PropertyMetadata(default(Brush)));

        /// <summary>
        /// Identifies the <see cref="PrimaryThumbStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PrimaryThumbStyleProperty =
            DependencyProperty.Register(nameof(PrimaryThumbStyle), typeof(Style), typeof(ImageCropper), new PropertyMetadata(default(Style)));

        /// <summary>
        /// Identifies the <see cref="SecondaryThumbStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SecondaryThumbStyleProperty =
            DependencyProperty.Register(nameof(SecondaryThumbStyle), typeof(Style), typeof(ImageCropper), new PropertyMetadata(default(Style)));

        /// <summary>
        /// Identifies the <see cref="ThumbPlacement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ThumbPlacementProperty =
            DependencyProperty.Register(nameof(ThumbPlacement), typeof(ThumbPlacement), typeof(ImageCropper), new PropertyMetadata(default(ThumbPlacement), OnThumbPlacementChanged));
    }
}