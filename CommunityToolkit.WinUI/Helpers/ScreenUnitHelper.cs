// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Xaml;
using Windows.Graphics.Display;

namespace CommunityToolkit.WinUI.Helpers
{
    /// <summary>
    /// This class provides static helper methods for <see cref="ScreenUnit" />.
    /// </summary>
    public static class ScreenUnitHelper
    {
        private const float PixelToCentimeterRatio = 37.79527559055f;
        private const float PixelToInchRatio = 96;
        private const float CentimeterToInchRatio = 2.54f;

        /// <summary>
        /// Converts a screen unit to another screen unit (ex: 1cm => 37.7953px).
        /// </summary>
        /// <param name="from">Start unit</param>
        /// <param name="to">End unit</param>
        /// <param name="value">The value to convert (using start unit)</param>
        /// <param name="xamlRoot">The XamlRoot that will be used to get the screen scale. Required on Xaml Islands.</param>
        /// <returns>The result of the conversion</returns>
        public static float Convert(ScreenUnit from, ScreenUnit to, float value, XamlRoot xamlRoot = null)
        {
            if (from == to)
            {
                return value;
            }

            switch (from)
            {
                case ScreenUnit.Pixel:
                    if (to == ScreenUnit.Centimeter)
                    {
                        return value / PixelToCentimeterRatio;
                    }

                    if (to == ScreenUnit.Inch)
                    {
                        return value / PixelToInchRatio;
                    }

                    if (to == ScreenUnit.EffectivePixel)
                    {
                        return value / GetScale(xamlRoot);
                    }

                    throw new ArgumentOutOfRangeException(nameof(to));

                case ScreenUnit.Centimeter:
                    if (to == ScreenUnit.Pixel)
                    {
                        return value * PixelToCentimeterRatio;
                    }

                    if (to == ScreenUnit.Inch)
                    {
                        return value / CentimeterToInchRatio;
                    }

                    throw new ArgumentOutOfRangeException(nameof(to));

                case ScreenUnit.Inch:
                    if (to == ScreenUnit.Pixel)
                    {
                        return value * PixelToInchRatio;
                    }

                    if (to == ScreenUnit.Centimeter)
                    {
                        return value * CentimeterToInchRatio;
                    }

                    throw new ArgumentOutOfRangeException(nameof(to));

                case ScreenUnit.EffectivePixel:
                    if (to == ScreenUnit.Pixel)
                    {
                        return value * GetScale(xamlRoot);
                    }

                    throw new ArgumentOutOfRangeException(nameof(to));

                default:
                    throw new ArgumentOutOfRangeException(nameof(from));
            }
        }

        private static float GetScale(XamlRoot xamlRoot)
        {
            if (xamlRoot != null)
            {
                return (float)xamlRoot.RasterizationScale;
            }
            else
            {
                return (float)DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            }
        }
    }
}