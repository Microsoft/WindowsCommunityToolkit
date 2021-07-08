// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace CommunityToolkit.WinUI.SampleApp.Common
{
    internal class DoubleTopThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return new Thickness(0, (double)value, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return ((Thickness)value).Top;
        }
    }
}