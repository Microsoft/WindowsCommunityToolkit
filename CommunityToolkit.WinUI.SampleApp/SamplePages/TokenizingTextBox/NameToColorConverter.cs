// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace CommunityToolkit.WinUI.SampleApp.SamplePages
{
    public class NameToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var color = Colors.Black;

            if (value != null)
            {
                var hash = value.GetHashCode();

                var rnd = new Random(hash);

                color = Color.FromArgb(255, (byte)rnd.Next(64, 192), (byte)rnd.Next(64, 192), (byte)rnd.Next(64, 192));
            }

            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}