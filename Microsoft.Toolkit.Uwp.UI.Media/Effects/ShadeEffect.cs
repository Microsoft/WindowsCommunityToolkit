// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Toolkit.Uwp.UI.Media.Pipelines;
using Windows.UI;

namespace Microsoft.Toolkit.Uwp.UI.Media
{
    /// <summary>
    /// An effect that overlays a color layer over the current builder, with a specified intensity
    /// </summary>
    public sealed class ShadeEffect : PipelineEffect
    {
        /// <summary>
        /// Gets or sets the color to use
        /// </summary>
        public Color Color { get; set; }

        private double intensity = 0.5;

        /// <summary>
        /// Gets or sets the intensity of the color layer (default to 0.5, should be in the [0, 1] range)
        /// </summary>
        public double Intensity
        {
            get => this.intensity;
            set => this.intensity = Math.Clamp(value, 0, 1);
        }

        /// <inheritdoc/>
        public override PipelineBuilder AppendToBuilder(PipelineBuilder builder)
        {
            return builder.Shade(Color, (float)Intensity);
        }
    }
}