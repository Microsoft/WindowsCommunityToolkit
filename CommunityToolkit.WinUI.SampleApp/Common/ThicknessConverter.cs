// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace CommunityToolkit.WinUI.SampleApp.Common
{
    public class ThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string)
            {
                return value;
            }

            var thickness = (Thickness)value;

            return thickness.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string thicknessString)
            {
                var thicknessTokens = thicknessString.Split(',')
                                                     .Where(tkn => !string.IsNullOrWhiteSpace(tkn))
                                                     .ToArray();
                switch (thicknessTokens.Length)
                {
                    case 1:
                        var thicknessValue = double.Parse(thicknessString);
                        return new Thickness
                        {
                            Left = thicknessValue,
                            Top = thicknessValue,
                            Right = thicknessValue,
                            Bottom = thicknessValue
                        };
                    case 2:
                        var thicknessHorizontalValue = double.Parse(thicknessTokens[0]);
                        var thicknessVerticalValue = double.Parse(thicknessTokens[1]);

                        return new Thickness
                        {
                            Left = thicknessHorizontalValue,
                            Top = thicknessVerticalValue,
                            Right = thicknessHorizontalValue,
                            Bottom = thicknessVerticalValue
                        };
                    case 4:
                        return new Thickness
                        {
                            Left = double.Parse(thicknessTokens[0]),
                            Top = double.Parse(thicknessTokens[1]),
                            Right = double.Parse(thicknessTokens[2]),
                            Bottom = double.Parse(thicknessTokens[3])
                        };
                    default:
                        return default(Thickness);
                }
            }

            return value.ToString();
        }
    }
}