// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.UI.Media.Pipelines;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;

namespace CommunityToolkit.WinUI.UI.Media
{
    /// <summary>
    /// An image effect, which displays an image loaded as a Win2D surface
    /// </summary>
    public sealed class ImageSourceExtension : ImageSourceBaseExtension
    {
        /// <inheritdoc/>
        protected override object ProvideValue(IXamlServiceProvider serviceProvider)
        {
            var rootObjectProvider = serviceProvider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;

            var window = rootObjectProvider.RootObject as Window;
            var uiElement = rootObjectProvider.RootObject as UIElement;

            return PipelineBuilder.FromImage(
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