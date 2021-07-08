// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.UI.Media.Pipelines;

namespace CommunityToolkit.WinUI.UI.Media
{
    /// <summary>
    /// A grayscale effect
    /// </summary>
    /// <remarks>This effect maps to the Win2D <see cref="Microsoft.Graphics.Canvas.Effects.GrayscaleEffect"/> effect</remarks>
    public sealed class GrayscaleEffect : PipelineEffect
    {
        /// <inheritdoc/>
        public override PipelineBuilder AppendToBuilder(PipelineBuilder builder)
        {
            return builder.Grayscale();
        }
    }
}