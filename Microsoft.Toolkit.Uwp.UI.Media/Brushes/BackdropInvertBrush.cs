// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//// Example brush from https://blogs.windows.com/buildingapps/2017/07/18/working-brushes-content-xaml-visual-layer-interop-part-one/#z70vPv1QMAvZsceo.97

using Microsoft.Toolkit.Uwp.UI.Media.Pipelines;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.UI.Media
{
    /// <summary>
    /// The <see cref="BackdropInvertBrush"/> is a <see cref="Brush"/> which inverts whatever is behind it in the application.
    /// </summary>
    public class BackdropInvertBrush : XamlCompositionEffectBrushBase
    {
        /// <inheritdoc/>
        protected override PipelineBuilder OnPipelineRequested()
        {
            return PipelineBuilder.FromBackdrop().Invert();
        }
    }
}