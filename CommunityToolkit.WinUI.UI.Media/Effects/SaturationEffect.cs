// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.WinUI.UI.Media.Pipelines;

#nullable enable

namespace CommunityToolkit.WinUI.UI.Media
{
    /// <summary>
    /// A saturation effect
    /// </summary>
    /// <remarks>This effect maps to the Win2D <see cref="Microsoft.Graphics.Canvas.Effects.SaturationEffect"/> effect</remarks>
    public sealed class SaturationEffect : PipelineEffect
    {
        private double value = 1;

        /// <summary>
        /// Gets or sets the saturation amount to apply to the background (defaults to 1, should be in the [0, 1] range).
        /// </summary>
        public double Value
        {
            get => this.value;
            set => this.value = Math.Clamp(value, 0, 1);
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
                builder = builder.Saturation((float)Value, out string id);

                Id = id;

                return builder;
            }

            return builder.Saturation((float)Value);
        }
    }
}