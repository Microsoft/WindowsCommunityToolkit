// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Toolkit.Uwp.UI.Media.Pipelines;

#nullable enable

namespace Microsoft.Toolkit.Uwp.UI.Media
{
    /// <summary>
    /// A sepia effect
    /// </summary>
    /// <remarks>This effect maps to the Win2D <see cref="Graphics.Canvas.Effects.SepiaEffect"/> effect</remarks>
    public sealed class SepiaEffect : PipelineEffect
    {
        private double intensity = 0.5;

        /// <summary>
        /// Gets or sets the intensity of the effect (defaults to 0.5, should be in the [0, 1] range).
        /// </summary>
        public double Intensity
        {
            get => this.intensity;
            set => this.intensity = Math.Clamp(value, 0, 1);
        }

        /// <summary>
        /// Gets the unique id for the effect, if <see cref="PipelineEffect.IsAnimatable"/> is set.
        /// </summary>
        internal string? Id { get; private set; }

        /// <inheritdoc/>
        public override PipelineBuilder AppendToBuilder(PipelineBuilder builder)
        {
            if (IsAnimatable)
            {
                builder = builder.Sepia((float)Intensity, out string id);

                Id = id;

                return builder;
            }

            return builder.Sepia((float)Intensity);
        }
    }
}