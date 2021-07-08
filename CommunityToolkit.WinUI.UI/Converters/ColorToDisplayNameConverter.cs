// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace CommunityToolkit.WinUI.UI.Converters
{
    /// <summary>
    /// Gets the approximated display name for the color.
    /// </summary>
    public class ColorToDisplayNameConverter : IValueConverter
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

            // return global::Microsoft.UI.Xaml.ColorDisplayNameHelper.ToDisplayName(color);
            // Removed on Reunion 0.8-RC. Not sure what to replace with.
            return DependencyProperty.UnsetValue;
        }

        /// <inheritdoc/>
        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            string language)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}