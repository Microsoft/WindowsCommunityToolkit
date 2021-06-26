// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.UI.Media.Pipelines;

#nullable enable

namespace Microsoft.Toolkit.Uwp.UI.Media
{
    /// <summary>
    /// A hue rotation effect
    /// </summary>
    /// <remarks>This effect maps to the Win2D <see cref="Graphics.Canvas.Effects.HueRotationEffect"/> effect</remarks>
    public sealed class HueRotationEffect : PipelineEffect
    {
        /// <summary>
        /// Gets or sets the angle to rotate the hue, in radians
        /// </summary>
        public double Angle { get; set; }

        /// <summary>
        /// Gets the unique id for the effect, if <see cref="PipelineEffect.IsAnimatable"/> is set.
        /// </summary>
        internal string? Id { get; private set; }

        /// <inheritdoc/>
        public override PipelineBuilder AppendToBuilder(PipelineBuilder builder)
        {
            if (IsAnimatable)
            {
                builder = builder.HueRotation((float)Angle, out string id);

                Id = id;

                return builder;
            }

            return builder.HueRotation((float)Angle);
        }
    }
}