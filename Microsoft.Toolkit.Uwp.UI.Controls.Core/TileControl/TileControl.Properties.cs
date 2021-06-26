// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// A ContentControl that show an image repeated many times.
    /// The control can be synchronized with a ScrollViewer and animated easily.
    /// </summary>
    public partial class TileControl : ContentControl
    {
        /// <summary>
        /// Identifies the <see cref="ScrollViewerContainer"/> property.
        /// </summary>
        public static readonly DependencyProperty ScrollViewerContainerProperty =
            DependencyProperty.Register(nameof(ScrollViewerContainer), typeof(FrameworkElement), typeof(TileControl), new PropertyMetadata(null, OnScrollViewerContainerChange));

        /// <summary>
        /// Identifies the <see cref="ImageAlignment"/> property.
        /// </summary>
        public static readonly DependencyProperty ImageAlignmentProperty =
            DependencyProperty.Register(nameof(ImageAlignment), typeof(ImageAlignment), typeof(TileControl), new PropertyMetadata(ImageAlignment.None, OnAlignmentChange));

        /// <summary>
        /// Identifies the <see cref="ImageSource"/> property.
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(nameof(ImageSource), typeof(Uri), typeof(TileControl), new PropertyMetadata(null, OnImageSourceChanged));

        /// <summary>
        /// Identifies the <see cref="ScrollOrientation"/> property.
        /// </summary>
        public static readonly DependencyProperty ScrollOrientationProperty =
            DependencyProperty.Register(nameof(ScrollOrientation), typeof(ScrollOrientation), typeof(TileControl), new PropertyMetadata(ScrollOrientation.Both, OnOrientationChanged));

        /// <summary>
        /// Identifies the <see cref="OffsetX"/> property.
        /// </summary>
        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register(nameof(OffsetX), typeof(double), typeof(TileControl), new PropertyMetadata(0.0, OnOffsetChange));

        /// <summary>
        /// Identifies the <see cref="OffsetY"/> property.
        /// </summary>
        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register(nameof(OffsetY), typeof(double), typeof(TileControl), new PropertyMetadata(0.0, OnOffsetChange));

        /// <summary>
        /// Identifies the <see cref="ParallaxSpeedRatio"/> property.
        /// </summary>
        public static readonly DependencyProperty ParallaxSpeedRatioProperty =
            DependencyProperty.Register(nameof(ParallaxSpeedRatio), typeof(double), typeof(TileControl), new PropertyMetadata(1.0, OnScrollSpeedRatioChange));

        /// <summary>
        /// Identifies the <see cref="IsAnimated"/> property.
        /// </summary>
        public static readonly DependencyProperty IsAnimatedProperty =
            DependencyProperty.Register(nameof(IsAnimated), typeof(bool), typeof(TileControl), new PropertyMetadata(false, OnIsAnimatedChange));

        /// <summary>
        /// Identifies the <see cref="AnimationStepX"/> property.
        /// </summary>
        public static readonly DependencyProperty AnimationStepXProperty =
            DependencyProperty.Register(nameof(AnimationStepX), typeof(double), typeof(TileControl), new PropertyMetadata(1.0));

        /// <summary>
        /// Identifies the <see cref="AnimationStepY"/> property.
        /// </summary>
        public static readonly DependencyProperty AnimationStepYProperty =
            DependencyProperty.Register(nameof(AnimationStepY), typeof(double), typeof(TileControl), new PropertyMetadata(1.0));

        /// <summary>
        /// Identifies the <see cref="AnimationDuration"/> property.
        /// </summary>
        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register(nameof(AnimationDuration), typeof(double), typeof(TileControl), new PropertyMetadata(30.0, OnAnimationDuration));

        /// <summary>
        /// Gets or sets a ScrollViewer or a frameworkElement containing a ScrollViewer.
        /// The tile control is synchronized with the offset of the scrollViewer
        /// </summary>
        public FrameworkElement ScrollViewerContainer
        {
            get { return (FrameworkElement)GetValue(ScrollViewerContainerProperty); }
            set { SetValue(ScrollViewerContainerProperty, value); }
        }

        private static async void OnScrollViewerContainerChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TileControl;
            await control.InitializeScrollViewerContainer(e.OldValue as FrameworkElement, e.NewValue as FrameworkElement);
        }

        /// <summary>
        /// Gets or sets the alignment of the tile when the <see cref="ScrollOrientation"/> is set to Vertical or Horizontal.
        /// Valid values are Left or Right for <see cref="ScrollOrientation"/> set to Horizontal and Top or Bottom for <see cref="ScrollOrientation"/> set to Vertical.
        /// </summary>
        public ImageAlignment ImageAlignment
        {
            get { return (ImageAlignment)GetValue(ImageAlignmentProperty); }
            set { SetValue(ImageAlignmentProperty, value); }
        }

        private static async void OnAlignmentChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TileControl;
            await control.RefreshContainerTileLocked();
        }

        /// <summary>
        /// Gets or sets the uri of the image to load
        /// </summary>
        public Uri ImageSource
        {
            get { return (Uri)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        private static async void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TileControl;
            await control.LoadImageBrushAsync(e.NewValue as Uri);
        }

        /// <summary>
        /// Gets or sets the scroll orientation of the tile.
        /// Less images are drawn when you choose the Horizontal or Vertical value.
        /// </summary>
        public ScrollOrientation ScrollOrientation
        {
            get { return (ScrollOrientation)GetValue(ScrollOrientationProperty); }
            set { SetValue(ScrollOrientationProperty, value); }
        }

        private static async void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TileControl;
            await control.RefreshContainerTileLocked();
            await control.CreateModuloExpression(control._scrollViewer);
        }

        /// <summary>
        /// Gets or sets an X offset of the image
        /// </summary>
        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }

        private static void OnOffsetChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as TileControl;

            c.RefreshMove();
        }

        /// <summary>
        /// Gets or sets an Y offset of the image
        /// </summary>
        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }

        /// <summary>
        /// Gets or sets the speed ratio of the parallax effect with the <see cref="ScrollViewerContainer"/>
        /// </summary>
        public double ParallaxSpeedRatio
        {
            get { return (double)GetValue(ParallaxSpeedRatioProperty); }
            set { SetValue(ParallaxSpeedRatioProperty, value); }
        }

        private static void OnScrollSpeedRatioChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as TileControl;
            c.RefreshScrollSpeedRatio((double)e.NewValue);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tile is animated or not
        /// </summary>
        public bool IsAnimated
        {
            get { return (bool)GetValue(IsAnimatedProperty); }
            set { SetValue(IsAnimatedProperty, value); }
        }

        private static void OnIsAnimatedChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as TileControl;

            if ((bool)e.NewValue)
            {
                c._timerAnimation.Start();
            }
            else
            {
                c._timerAnimation.Stop();
                c._animationX = 0;
                c._animationY = 0;
            }
        }

        /// <summary>
        /// Gets or sets the animation step of the OffsetX
        /// </summary>
        public double AnimationStepX
        {
            get { return (double)GetValue(AnimationStepXProperty); }
            set { SetValue(AnimationStepXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the animation step of the OffsetY
        /// </summary>
        public double AnimationStepY
        {
            get { return (double)GetValue(AnimationStepYProperty); }
            set { SetValue(AnimationStepYProperty, value); }
        }

        /// <summary>
        /// Gets or sets a duration for the animation of the tile
        /// </summary>
        public double AnimationDuration
        {
            get { return (double)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }

        private static void OnAnimationDuration(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as TileControl;

            c._timerAnimation.Interval = TimeSpan.FromMilliseconds(c.AnimationDuration);
        }
    }
}