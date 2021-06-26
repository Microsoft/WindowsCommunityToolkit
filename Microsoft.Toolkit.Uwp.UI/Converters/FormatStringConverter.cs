// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.UI.Xaml.Data;

namespace Microsoft.Toolkit.Uwp.UI.Converters
{
    /// <summary>
    /// Value converter that converts an <see cref="IFormattable"/> to a formatted <see cref="string"/>.
    /// The string format needs to be passed as the converter parameter.
    /// </summary>
    public class FormatStringConverter : IValueConverter
    {
        /// <summary>
        /// Convert an <see cref="IFormattable"/> value to a formatted <see cref="string"/>.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">The format string.</param>
        /// <param name="language">The language of the conversion. Not used.</param>
        /// <returns>The formatted string.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var formattableValue = value as IFormattable;
            var format = parameter as string;

            if (formattableValue == null || format == null)
            {
                return value;
            }

            return formattableValue.ToString(format, null);
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">Optional parameter. Not used.</param>
        /// <param name="language">The language of the conversion. Not used.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}