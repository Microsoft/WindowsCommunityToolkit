// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.UI.Media;
using Windows.UI;

namespace CommunityToolkit.WinUI.UI.Animations
{
    /// <summary>
    /// An effect animation that targets <see cref="TintEffect.Color"/>.
    /// </summary>
    public sealed class ColorEffectAnimation : EffectAnimation<TintEffect, Color?, Color>
    {
        /// <inheritdoc/>
        protected override string ExplicitTarget => Target.Id;

        /// <inheritdoc/>
        protected override (Color?, Color?) GetParsedValues()
        {
            return (To, From);
        }
    }
}