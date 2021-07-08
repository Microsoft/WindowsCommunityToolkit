// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using CommunityToolkit.WinUI.UI.Media.Geometry.Core;
using Microsoft.UI;
using Windows.UI;

namespace CommunityToolkit.WinUI.UI.Media.Geometry.Parsers
{
    /// <summary>
    /// Parser for Color
    /// </summary>
    internal static class ColorParser
    {
        /// <summary>
        /// Converts the color string in Hexadecimal or HDR color format to the corresponding Color object.
        /// The hexadecimal color string should be in #RRGGBB or #AARRGGBB format.
        /// The '#' character is optional.
        /// The HDR color string should be in R G B A format.
        /// (R, G, B &amp; A should have value in the range between 0 and 1, inclusive)
        /// </summary>
        /// <param name="colorString">Color string in Hexadecimal or HDR format</param>
        /// <returns>Color</returns>
        internal static Color Parse(string colorString)
        {
            var match = RegexFactory.ColorRegex.Match(colorString);

            if (!match.Success)
            {
                ThrowArgumentException();
            }

            // Perform validation to check if there are any invalid characters in the colorString that were not captured
            var preValidationCount = RegexFactory.ValidationRegex.Replace(colorString, string.Empty).Length;
            var postValidationCount = RegexFactory.ValidationRegex.Replace(match.Value, string.Empty).Length;

            // If there are invalid characters, extract them and add them to the ArgumentException message
            if (preValidationCount != postValidationCount)
            {
                [MethodImpl(MethodImplOptions.NoInlining)]
                static void ThrowForInvalidCharacters(Match match, string colorString)
                {
                    var parseIndex = 0;
                    if (!string.IsNullOrWhiteSpace(match.Value))
                    {
                        parseIndex = colorString.IndexOf(match.Value, parseIndex, StringComparison.Ordinal);
                    }

                    var errorString = colorString.Substring(parseIndex);
                    if (errorString.Length > 30)
                    {
                        errorString = $"{errorString.Substring(0, 30)}...";
                    }

                    throw new ArgumentException($"COLOR_ERR003:Color data contains invalid characters!\nIndex: {parseIndex}\n{errorString}");
                }

                ThrowForInvalidCharacters(match, colorString);
            }

            return Parse(match);

            static void ThrowArgumentException() => throw new ArgumentException("COLOR_ERR001:Invalid value provided in Color Data! No matching color found in the Color Data.");
        }

        /// <summary>
        /// Converts a Vector4 High Dynamic Range Color to Color. Negative components of the Vector4 will be sanitized by taking the absolute value of the component.
        /// The HDR Color components should have value in the range between 0 and 1, inclusive. If they are more than 1, they will be clamped at 1.
        /// </summary>
        /// <param name="hdrColor">High Dynamic Range Color</param>
        /// <returns>Color</returns>
        internal static Color Parse(Vector4 hdrColor)
        {
            // Vector4's X, Y, Z, W components match to
            // Color's   R, G, B, A components respectively
            return Parse(hdrColor.X, hdrColor.Y, hdrColor.Z, hdrColor.W);
        }

        /// <summary>
        /// Converts the given HDR color values to Color.
        /// </summary>
        /// <param name="x">Red Component</param>
        /// <param name="y">Green Component</param>
        /// <param name="z">Blue Component</param>
        /// <param name="w">Alpha Component</param>
        /// <returns>Instance of Color.</returns>
        internal static Color Parse(float x, float y, float z, float w)
        {
            Vector4 v4 = new Vector4(x, y, z, w);
            v4 = Vector4.Min(Vector4.Abs(v4) * 255f, new Vector4(255f));

            var r = (byte)v4.X;
            var g = (byte)v4.Y;
            var b = (byte)v4.Z;
            var a = (byte)v4.W;

            return Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// Parses and constructs a Color object from the specified Match object.
        /// </summary>
        /// <param name="match">Match object</param>
        /// <returns>Color</returns>
        internal static Color Parse(Match match)
        {
            if (match.Groups["RgbColor"].Success)
            {
                // Alpha component
                byte alpha = 255;
                var alphaStr = match.Groups["Alpha"].Value;
                if (!string.IsNullOrWhiteSpace(alphaStr))
                {
                    alpha = (byte)Convert.ToInt32(alphaStr, 16);
                }

                // Red component
                var red = (byte)Convert.ToInt32(match.Groups["Red"].Value, 16);

                // Green component
                var green = (byte)Convert.ToInt32(match.Groups["Green"].Value, 16);

                // Blue component
                var blue = (byte)Convert.ToInt32(match.Groups["Blue"].Value, 16);

                return Color.FromArgb(alpha, red, green, blue);
            }

            if (match.Groups["HdrColor"].Success)
            {
                float.TryParse(match.Groups["X"].Value, out var x);
                float.TryParse(match.Groups["Y"].Value, out var y);
                float.TryParse(match.Groups["Z"].Value, out var z);
                float.TryParse(match.Groups["W"].Value, out var w);

                return Parse(x, y, z, w);
            }

            return Colors.Transparent;
        }
    }
}