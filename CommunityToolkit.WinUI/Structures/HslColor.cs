// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI
{
    /// <summary>
    /// Defines a color in Hue/Saturation/Lightness (HSL) space.
    /// </summary>
    public struct HslColor
    {
        /// <summary>
        /// The Hue in 0..360 range.
        /// </summary>
        public double H;

        /// <summary>
        /// The Saturation in 0..1 range.
        /// </summary>
        public double S;

        /// <summary>
        /// The Lightness in 0..1 range.
        /// </summary>
        public double L;

        /// <summary>
        /// The Alpha/opacity in 0..1 range.
        /// </summary>
        public double A;
    }
}