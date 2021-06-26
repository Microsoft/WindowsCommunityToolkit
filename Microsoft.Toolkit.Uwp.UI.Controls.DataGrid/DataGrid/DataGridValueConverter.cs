// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Toolkit.Uwp.Utilities;
using Windows.UI.Xaml.Data;

namespace Microsoft.Toolkit.Uwp.UI.Controls.DataGridInternals
{
    internal class DataGridValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (targetType != null && TypeHelper.IsNullableType(targetType))
            {
                string strValue = value as string;
                if (strValue == string.Empty)
                {
                    return null;
                }
            }

            return value;
        }
    }
}