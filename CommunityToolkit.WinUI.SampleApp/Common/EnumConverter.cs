// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Xaml.Data;

namespace CommunityToolkit.WinUI.SampleApp.Common
{
    public class EnumConverter : IValueConverter
    {
        private readonly Type _enumType;

        public EnumConverter(Type enumType)
        {
            _enumType = enumType;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Enum.Parse(_enumType, value.ToString());
        }
    }
}