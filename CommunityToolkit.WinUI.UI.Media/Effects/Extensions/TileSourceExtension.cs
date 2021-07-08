// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.UI.Media.Pipelines;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;

namespace CommunityToolkit.WinUI.UI.Media
{
    /// <summary>
    /// An effect that loads an image and replicates it to cover all the available surface area
    /// </summary>
    /// <remarks>This effect maps to the Win2D <see cref="Microsoft.Graphics.Canvas.Effects.BorderEffect"/> effect</remarks>
    public sealed class TileSourceExtension : ImageSourceBaseExtension
    {
        /// <inheritdoc/>
        protected override object ProvideValue(IXamlServiceProvider serviceProvider)
        {
            var rootObjectProvider = serviceProvider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;

            var window = rootObjectProvider.RootObject as Window;
            var uiElement = rootObjectProvider.RootObject as UIElement;

            return PipelineBuilder.FromTiles(
                Uri,
                () =>
                {
                    float dpi = 96.0f;
                    if (uiElement == null && window != null)
                    {
                        uiElement = window.Content;
                    }

                    if (uiElement != null)
                    {
                        dpi = (float)uiElement.XamlRoot.RasterizationScale * 96;
                    }

                    return dpi;
                },
                DpiMode,
                CacheMode);
        }
    }
}