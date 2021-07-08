// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.UI.Media.Pipelines;
using Microsoft.UI.Xaml.Markup;
using Windows.UI;

namespace CommunityToolkit.WinUI.UI.Media
{
    /// <summary>
    /// An effect that renders a standard 8bit SDR color on the available surface
    /// </summary>
    [MarkupExtensionReturnType(ReturnType = typeof(PipelineBuilder))]
    public sealed class SolidColorSourceExtension : MarkupExtension
    {
        /// <summary>
        /// Gets or sets the color to display
        /// </summary>
        public Color Color { get; set; }

        /// <inheritdoc/>
        protected override object ProvideValue()
        {
            return PipelineBuilder.FromColor(Color);
        }
    }
}