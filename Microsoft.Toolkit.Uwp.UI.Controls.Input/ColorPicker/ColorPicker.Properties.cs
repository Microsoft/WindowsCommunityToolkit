// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Contains all properties for the <see cref="ColorPicker"/>.
    /// </summary>
    public partial class ColorPicker
    {
        /// <summary>
        /// Identifies the <see cref="CustomPaletteColors"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomPaletteColorsProperty =
            DependencyProperty.Register(
                nameof(CustomPaletteColors),
                typeof(ObservableCollection<Windows.UI.Color>),
                typeof(ColorPicker),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets the list of custom palette colors.
        /// </summary>
        public ObservableCollection<Windows.UI.Color> CustomPaletteColors
        {
            get => (ObservableCollection<Windows.UI.Color>)this.GetValue(CustomPaletteColorsProperty);
        }

        /// <summary>
        /// Identifies the <see cref="CustomPaletteColumnCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomPaletteColumnCountProperty =
            DependencyProperty.Register(
                nameof(CustomPaletteColumnCount),
                typeof(int),
                typeof(ColorPicker),
                new PropertyMetadata(4));

        /// <summary>
        /// Gets or sets the number of colors in each row (section) of the custom color palette.
        /// Within a standard palette, rows are shades and columns are unique colors.
        /// </summary>
        public int CustomPaletteColumnCount
        {
            get => (int)this.GetValue(CustomPaletteColumnCountProperty);
            set
            {
                if (object.Equals(value, this.GetValue(CustomPaletteColumnCountProperty)) == false)
                {
                    this.SetValue(CustomPaletteColumnCountProperty, value);
                }
            }
        }

        /// <summary>
        /// Identifies the <see cref="CustomPalette"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomPaletteProperty =
            DependencyProperty.Register(
                nameof(CustomPalette),
                typeof(IColorPalette),
                typeof(ColorPicker),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the custom color palette.
        /// This will automatically set <see cref="CustomPaletteColors"/> and <see cref="CustomPaletteColumnCount"/>
        /// overwriting any existing values.
        /// </summary>
        public IColorPalette CustomPalette
        {
            get => (IColorPalette)this.GetValue(CustomPaletteProperty);
            set
            {
                if (object.Equals(value, this.GetValue(CustomPaletteProperty)) == false)
                {
                    this.SetValue(CustomPaletteProperty, value);
                }
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsColorPaletteVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsColorPaletteVisibleProperty =
            DependencyProperty.Register(
                nameof(IsColorPaletteVisible),
                typeof(bool),
                typeof(ColorPicker),
                new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether the color palette is visible.
        /// </summary>
        public bool IsColorPaletteVisible
        {
            get => (bool)this.GetValue(IsColorPaletteVisibleProperty);
            set
            {
                if (object.Equals(value, this.GetValue(IsColorPaletteVisibleProperty)) == false)
                {
                    this.SetValue(IsColorPaletteVisibleProperty, value);
                }
            }
        }
    }
}