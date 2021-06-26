// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.UI.Media.Pipelines;
using Windows.UI.Composition;

#nullable enable

namespace Microsoft.Toolkit.Uwp.UI.Media
{
    /// <summary>
    /// The base <see langword="interface"/> for all the builder effects to be used in a <see cref="CompositionBrush"/>.
    /// </summary>
    public interface IPipelineEffect
    {
        /// <summary>
        /// Gets the current <see cref="CompositionBrush"/> instance, if one is in use.
        /// </summary>
        CompositionBrush? Brush { get; }

        /// <summary>
        /// Appends the current effect to the input <see cref="PipelineBuilder"/> instance.
        /// </summary>
        /// <param name="builder">The source <see cref="PipelineBuilder"/> instance to add the effect to.</param>
        /// <returns>A new <see cref="PipelineBuilder"/> with the new effects added to it.</returns>
        PipelineBuilder AppendToBuilder(PipelineBuilder builder);

        /// <summary>
        /// Notifies that a given <see cref="CompositionBrush"/> is now in use.
        /// </summary>
        /// <param name="brush">The <see cref="CompositionBrush"/> in use.</param>
        void NotifyCompositionBrushInUse(CompositionBrush brush);
    }
}