// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace Microsoft.Toolkit.Uwp.UI.Converters
{
    /// <summary>
    /// Value converter that look up for the source string in the App Resources strings and returns its value, if found.
    /// </summary>
    public sealed class ResourceNameToResourceStringConverter : IValueConverter
    {
        private readonly ResourceLoader _resourceLoader = ResourceLoader.GetForViewIndependentUse();

        /// <summary>
        /// Take the source string as a resource name that will be looked up in the App Resources.
        /// If the resource exists, the value is returned, otherwise.
        /// </summary>
        /// <param name="value">The source string being passed to the target.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">Optional parameter. Not used.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>The string corresponding to the resource name.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return _resourceLoader.GetString(value.ToString());
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value">The source string being passed to the target.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">Optional parameter. Not used.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}