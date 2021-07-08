// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace CommunityToolkit.WinUI.UI.Converters
{
    /// <summary>
    /// This class converts a Visibility enumeration to a boolean value.
    /// </summary>
    public class VisibilityToBoolConverter : IValueConverter
    {
        /// <summary>
        /// Convert a <see cref="Visibility"/> value to boolean.
        /// </summary>
        /// <param name="value">The <see cref="Visibility"/> value to convert.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">Optional parameter. Not used.</param>
        /// <param name="language">The language of the conversion. Not used</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is Visibility visibility && visibility == Visibility.Visible;
        }

        /// <summary>
        /// Convert back a boolean value to <see cref="Visibility"/>.
        /// </summary>
        /// <param name="value">The <see cref="Visibility"/> value to convert back.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">Optional parameter. Not used.</param>
        /// <param name="language">The language of the conversion. Not used</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (value is bool bl && bl) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}