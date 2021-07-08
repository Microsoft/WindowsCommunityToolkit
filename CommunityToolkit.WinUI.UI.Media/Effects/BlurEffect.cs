// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.WinUI.UI.Media.Pipelines;

#nullable enable

namespace CommunityToolkit.WinUI.UI.Media
{
    /// <summary>
    /// A gaussian blur effect
    /// </summary>
    /// <remarks>This effect maps to the Win2D <see cref="Microsoft.Graphics.Canvas.Effects.GaussianBlurEffect"/> effect</remarks>
    public sealed class BlurEffect : PipelineEffect
    {
        private double amount;

        /// <summary>
        /// Gets or sets the blur amount for the effect (must be a positive value)
        /// </summary>
        public double Amount
        {
            get => this.amount;
            set => this.amount = Math.Max(value, 0);
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
                builder = builder.Blur((float)Amount, out string id);

                Id = id;

                return builder;
            }

            return builder.Blur((float)Amount);
        }
    }
}