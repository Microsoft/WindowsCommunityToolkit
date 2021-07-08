// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Xaml.Data;

namespace CommunityToolkit.WinUI.UI.Converters
{
    /// <summary>
    /// Converter to convert Device Type to Icon
    /// </summary>
    public class RemoteSystemKindToSymbolConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string finalvalue;
            switch ((string)value)
            {
                case "Desktop":
                    finalvalue = "\xE770";
                    break;

                case "Phone":
                case "Unknown":
                    finalvalue = "\xE8EA";
                    break;

                case "Xbox":
                    finalvalue = "\xE990";
                    break;

                case "Tablet":
                    finalvalue = "\xE70A";
                    break;

                case "Laptop":
                    finalvalue = "\xE7F8";
                    break;

                case "Holographic":
                    finalvalue = "\xF4BF";
                    break;

                case "Hub":
                    finalvalue = "\xE8AE";
                    break;

                case "Iot":
                    finalvalue = "\xF22C";
                    break;

                default:
                    finalvalue = "\xE770";
                    break;
            }

            return finalvalue;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}