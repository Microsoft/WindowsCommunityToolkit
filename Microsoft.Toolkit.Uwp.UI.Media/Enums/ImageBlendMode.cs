// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//// Composition supported version of http://microsoft.github.io/Win2D/html/T_Microsoft_Graphics_Canvas_Effects_BlendEffectMode.htm.

using Microsoft.Graphics.Canvas.Effects;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member - see http://microsoft.github.io/Win2D/html/T_Microsoft_Graphics_Canvas_Effects_BlendEffectMode.htm.

namespace Microsoft.Toolkit.Uwp.UI.Media
{
    /// <summary>
    /// Blend mode to use when compositing effects.
    /// See http://microsoft.github.io/Win2D/html/T_Microsoft_Graphics_Canvas_Effects_BlendEffectMode.htm for details.
    /// Dissolve is not supported.
    /// </summary>
    public enum ImageBlendMode
    {
        Multiply = BlendEffectMode.Multiply,
        Screen = BlendEffectMode.Screen,
        Darken = BlendEffectMode.Darken,
        Lighten = BlendEffectMode.Lighten,
        ColorBurn = BlendEffectMode.ColorBurn,
        LinearBurn = BlendEffectMode.LinearBurn,
        DarkerColor = BlendEffectMode.DarkerColor,
        LighterColor = BlendEffectMode.LighterColor,
        ColorDodge = BlendEffectMode.ColorDodge,
        LinearDodge = BlendEffectMode.LinearDodge,
        Overlay = BlendEffectMode.Overlay,
        SoftLight = BlendEffectMode.SoftLight,
        HardLight = BlendEffectMode.HardLight,
        VividLight = BlendEffectMode.VividLight,
        LinearLight = BlendEffectMode.LinearLight,
        PinLight = BlendEffectMode.PinLight,
        HardMix = BlendEffectMode.HardMix,
        Difference = BlendEffectMode.Difference,
        Exclusion = BlendEffectMode.Exclusion,

        /// <summary>
        /// Hue blend mode.
        /// </summary>
        Hue = BlendEffectMode.Hue,

        /// <summary>
        /// Saturation blend mode.
        /// </summary>
        Saturation = BlendEffectMode.Saturation,

        /// <summary>
        /// Color blend mode.
        /// </summary>
        Color = BlendEffectMode.Color,

        /// <summary>
        /// Luminosity blend mode.
        /// </summary>
        Luminosity = BlendEffectMode.Luminosity,
        Subtract = BlendEffectMode.Subtract,
        Division = BlendEffectMode.Division,
    }
}