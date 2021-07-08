// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace CommunityToolkit.WinUI.UI.Controls.ColorPickerConverters
{
    /// <summary>
    /// Converts a color to a hex string and vice versa.
    /// </summary>
    public class ColorToHexConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            string language)
        {
            Color color;

            if (value is Color valueColor)
            {
                color = valueColor;
            }
            else if (value is SolidColorBrush valueBrush)
            {
                color = valueBrush.Color;
            }
            else
            {
                // Invalid color value provided
                return DependencyProperty.UnsetValue;
            }

            string hexColor = color.ToHex().Replace("#", string.Empty);
            return hexColor;
        }

        /// <inheritdoc/>
        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            string language)
        {
            string hexValue = value.ToString();

            if (hexValue.StartsWith("#"))
            {
                try
                {
                    return hexValue.ToColor();
                }
                catch
                {
                    // Invalid hex color value provided
                    return DependencyProperty.UnsetValue;
                }
            }
            else
            {
                try
                {
                    return ("#" + hexValue).ToColor();
                }
                catch
                {
                    // Invalid hex color value provided
                    return DependencyProperty.UnsetValue;
                }
            }
        }
    }
}