// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.WinUI.UI.Media.Pipelines;

namespace CommunityToolkit.WinUI.UI.Media
{
    /// <summary>
    /// A temperature and tint effect
    /// </summary>
    /// <remarks>This effect maps to the Win2D <see cref="Microsoft.Graphics.Canvas.Effects.TemperatureAndTintEffect"/> effect</remarks>
    public sealed class TemperatureAndTintEffect : PipelineEffect
    {
        private double temperature;

        /// <summary>
        /// Gets or sets the value of the temperature for the current effect (defaults to 0, should be in the [-1, 1] range)
        /// </summary>
        public double Temperature
        {
            get => this.temperature;
            set => this.temperature = Math.Clamp(value, -1, 1);
        }

        private double tint;

        /// <summary>
        /// Gets or sets the value of the tint for the current effect (defaults to 0, should be in the [-1, 1] range)
        /// </summary>
        public double Tint
        {
            get => this.tint;
            set => this.tint = Math.Clamp(value, -1, 1);
        }

        /// <inheritdoc/>
        public override PipelineBuilder AppendToBuilder(PipelineBuilder builder)
        {
            return builder.TemperatureAndTint((float)Temperature, (float)Tint);
        }
    }
}