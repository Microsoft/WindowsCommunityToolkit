// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.UI.Media.Pipelines;
using Windows.UI;

#nullable enable

namespace Microsoft.Toolkit.Uwp.UI.Media
{
    /// <summary>
    /// A tint effect
    /// </summary>
    /// <remarks>This effect maps to the Win2D <see cref="Graphics.Canvas.Effects.TintEffect"/> effect</remarks>
    public sealed class TintEffect : PipelineEffect
    {
        /// <summary>
        /// Gets or sets the int color to use
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Gets the unique id for the effect, if <see cref="PipelineEffect.IsAnimatable"/> is set.
        /// </summary>
        internal string? Id { get; private set; }

        /// <inheritdoc/>
        public override PipelineBuilder AppendToBuilder(PipelineBuilder builder)
        {
            if (IsAnimatable)
            {
                builder = builder.Tint(Color, out string id);

                Id = id;

                return builder;
            }

            return builder.Tint(Color);
        }
    }
}