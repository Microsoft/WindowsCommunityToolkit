// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.SampleApp.Common
{
    public class SolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string)
            {
                return value;
            }

            var brush = (SolidColorBrush)value;

            return brush.Color.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // Check if what we're getting back is a named color so that we can keep it as its name if it is.
            var prop = typeof(Colors).GetTypeInfo().GetDeclaredProperty(value.ToString());

            if (prop != null)
            {
                return value.ToString();
            }

            return new SolidColorBrush(value.ToString().ToColor());
        }
    }
}