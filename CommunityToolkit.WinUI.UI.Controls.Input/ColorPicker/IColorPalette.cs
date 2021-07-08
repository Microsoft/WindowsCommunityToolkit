// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Interface to define a color palette.
    /// </summary>
    public interface IColorPalette
    {
        /// <summary>
        /// Gets the total number of colors in this palette.
        /// A color is not necessarily a single value and may be composed of several shades.
        /// </summary>
        int ColorCount { get; }

        /// <summary>
        /// Gets the total number of shades for each color in this palette.
        /// Shades are usually a variation of the color lightening or darkening it.
        /// </summary>
        int ShadeCount { get; }

        /// <summary>
        /// Gets a color in the palette by index.
        /// </summary>
        /// <param name="colorIndex">The index of the color in the palette.
        /// The index must be between zero and <see cref="ColorCount"/>.</param>
        /// <param name="shadeIndex">The index of the color shade in the palette.
        /// The index must be between zero and <see cref="ShadeCount"/>.</param>
        /// <returns>The color at the specified index or an exception.</returns>
        Color GetColor(int colorIndex, int shadeIndex);
    }
}