// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml;
using Windows.Graphics.Display;

namespace CommunityToolkit.WinUI.UI.Media.Helpers
{
    internal static class DpiHelpers
    {
        internal static float GetDpi(DependencyObject dependencyObject)
        {
            if (Window.Current != null)
            {
                // UWP
                var displayInformation = DisplayInformation.GetForCurrentView();
                return displayInformation.LogicalDpi;
            }
            else
            {
                // Win32
                // var uiElement = DependencyObjectExtensions.FindAscendant<UIElement>(dependencyObject);
                // var xamlRoot = uiElement?.XamlRoot;
                // if (xamlRoot != null)
                // {
                //     return (float)xamlRoot.RasterizationScale * 96.0f;
                // }
            }

            return 96f;
        }
    }
}
