// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Xaml.Data;

namespace CommunityToolkit.WinUI.SampleApp.Common
{
    public class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TimeSpan tSpan)
            {
                return tSpan.TotalMilliseconds;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is double milliseconds)
            {
                return TimeSpan.FromMilliseconds(milliseconds);
            }

            return TimeSpan.MinValue;
        }
    }
}