// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.Helpers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.UI.Controls.Primitives
{
    /// <inheritdoc/>
    public partial class ColorPickerSlider : Slider
    {
        /// <summary>
        /// Identifies the <see cref="Color"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
                nameof(Color),
                typeof(Color),
                typeof(ColorPickerSlider),
                new PropertyMetadata(
                    Colors.White,
                    (s, e) => (s as ColorPickerSlider)?.OnDependencyPropertyChanged(s, e)));

        /// <summary>
        /// Gets or sets the RGB color represented by the slider.
        /// For accuracy use <see cref="HsvColor"/> instead.
        /// </summary>
        public Color Color
        {
            get => (Color)this.GetValue(ColorProperty);
            set
            {
                if (object.Equals(value, this.GetValue(ColorProperty)) == false)
                {
                    this.SetValue(ColorProperty, value);
                }
            }
        }

        /// <summary>
        /// Identifies the <see cref="ColorChannel"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorChannelProperty =
            DependencyProperty.Register(
                nameof(ColorChannel),
                typeof(ColorChannel),
                typeof(ColorPickerSlider),
                new PropertyMetadata(
                    ColorChannel.Channel1,
                    (s, e) => (s as ColorPickerSlider)?.OnDependencyPropertyChanged(s, e)));

        /// <summary>
        /// Gets or sets the color channel represented by the slider.
        /// </summary>
        public ColorChannel ColorChannel
        {
            get => (ColorChannel)this.GetValue(ColorChannelProperty);
            set
            {
                if (object.Equals(value, this.GetValue(ColorChannelProperty)) == false)
                {
                    this.SetValue(ColorChannelProperty, value);
                }
            }
        }

        /// <summary>
        /// Identifies the <see cref="ColorRepresentation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorRepresentationProperty =
            DependencyProperty.Register(
                nameof(ColorRepresentation),
                typeof(ColorRepresentation),
                typeof(ColorPickerSlider),
                new PropertyMetadata(
                    ColorRepresentation.Rgba,
                    (s, e) => (s as ColorPickerSlider)?.OnDependencyPropertyChanged(s, e)));

        /// <summary>
        /// Gets or sets the color representation used by the slider.
        /// </summary>
        public ColorRepresentation ColorRepresentation
        {
            get => (ColorRepresentation)this.GetValue(ColorRepresentationProperty);
            set
            {
                if (object.Equals(value, this.GetValue(ColorRepresentationProperty)) == false)
                {
                    this.SetValue(ColorRepresentationProperty, value);
                }
            }
        }

        /// <summary>
        /// Identifies the <see cref="DefaultForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultForegroundProperty =
            DependencyProperty.Register(
                nameof(DefaultForeground),
                typeof(Brush),
                typeof(ColorPickerSlider),
                new PropertyMetadata(
                    new SolidColorBrush(Colors.Gray),
                    (s, e) => (s as ColorPickerSlider)?.OnDependencyPropertyChanged(s, e)));

        /// <summary>
        /// Gets or sets the default foreground brush to use when the slider background is hardly visible and nearly transparent.
        /// Generally, this should be the default Foreground text brush.
        /// </summary>
        public Brush DefaultForeground
        {
            get => (Brush)this.GetValue(DefaultForegroundProperty);
            set
            {
                if (object.Equals(value, this.GetValue(DefaultForegroundProperty)) == false)
                {
                    this.SetValue(DefaultForegroundProperty, value);
                }
            }
        }

        /// <summary>
        /// Identifies the <see cref="HsvColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HsvColorProperty =
            DependencyProperty.Register(
                nameof(HsvColor),
                typeof(HsvColor),
                typeof(ColorPickerSlider),
                new PropertyMetadata(
                    Colors.White.ToHsv(),
                    (s, e) => (s as ColorPickerSlider)?.OnDependencyPropertyChanged(s, e)));

        /// <summary>
        /// Gets or sets the HSV color represented by the slider.
        /// This is the preferred color property for accuracy.
        /// </summary>
        public HsvColor HsvColor
        {
            get => (HsvColor)this.GetValue(HsvColorProperty);
            set
            {
                if (object.Equals(value, this.GetValue(HsvColorProperty)) == false)
                {
                    this.SetValue(HsvColorProperty, value);
                }
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsAlphaMaxForced"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAlphaMaxForcedProperty =
            DependencyProperty.Register(
                nameof(IsAlphaMaxForced),
                typeof(bool),
                typeof(ColorPickerSlider),
                new PropertyMetadata(
                    true,
                    (s, e) => (s as ColorPickerSlider)?.OnDependencyPropertyChanged(s, e)));

        /// <summary>
        /// Gets or sets a value indicating whether the alpha channel is always forced to maximum for channels
        /// other than <see cref="ColorChannel"/>.
        /// This ensures that the background is always visible and never transparent regardless of the actual color.
        /// </summary>
        public bool IsAlphaMaxForced
        {
            get => (bool)this.GetValue(IsAlphaMaxForcedProperty);
            set
            {
                if (object.Equals(value, this.GetValue(IsAlphaMaxForcedProperty)) == false)
                {
                    this.SetValue(IsAlphaMaxForcedProperty, value);
                }
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsAutoUpdatingEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAutoUpdatingEnabledProperty =
            DependencyProperty.Register(
                nameof(IsAutoUpdatingEnabled),
                typeof(bool),
                typeof(ColorPickerSlider),
                new PropertyMetadata(
                    true,
                    (s, e) => (s as ColorPickerSlider)?.OnDependencyPropertyChanged(s, e)));

        /// <summary>
        /// Gets or sets a value indicating whether automatic background and foreground updates will be
        /// calculated when the set color changes. This can be disabled for performance reasons when working with
        /// multiple sliders.
        /// </summary>
        public bool IsAutoUpdatingEnabled
        {
            get => (bool)this.GetValue(IsAutoUpdatingEnabledProperty);
            set
            {
                if (object.Equals(value, this.GetValue(IsAutoUpdatingEnabledProperty)) == false)
                {
                    this.SetValue(IsAutoUpdatingEnabledProperty, value);
                }
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsSaturationValueMaxForced"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSaturationValueMaxForcedProperty =
            DependencyProperty.Register(
                nameof(IsSaturationValueMaxForced),
                typeof(bool),
                typeof(ColorPickerSlider),
                new PropertyMetadata(
                    true,
                    (s, e) => (s as ColorPickerSlider)?.OnDependencyPropertyChanged(s, e)));

        /// <summary>
        /// Gets or sets a value indicating whether the saturation and value channels are always forced to maximum values
        /// when in HSVA color representation. Only channel values other than <see cref="ColorChannel"/> will be changed.
        /// This ensures, for example, that the Hue background is always visible and never washed out regardless of the actual color.
        /// </summary>
        public bool IsSaturationValueMaxForced
        {
            get => (bool)this.GetValue(IsSaturationValueMaxForcedProperty);
            set
            {
                if (object.Equals(value, this.GetValue(IsSaturationValueMaxForcedProperty)) == false)
                {
                    this.SetValue(IsSaturationValueMaxForcedProperty, value);
                }
            }
        }
    }
}