// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.UI.Media.Pipelines;

namespace Microsoft.Toolkit.Uwp.UI.Media
{
    /// <summary>
    /// An effect that loads an image and replicates it to cover all the available surface area
    /// </summary>
    /// <remarks>This effect maps to the Win2D <see cref="Graphics.Canvas.Effects.BorderEffect"/> effect</remarks>
    public sealed class TileSourceExtension : ImageSourceBaseExtension
    {
        /// <inheritdoc/>
        protected override object ProvideValue()
        {
            return PipelineBuilder.FromTiles(Uri, DpiMode, CacheMode);
        }
    }
}