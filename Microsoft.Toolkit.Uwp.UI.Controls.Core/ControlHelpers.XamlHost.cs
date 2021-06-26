// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Internal class used to provide helpers for controls
    /// </summary>
    internal static partial class ControlHelpers
    {
        internal static bool IsXamlRootAvailable { get; } = Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent("Windows.UI.Xaml.UIElement", "XamlRoot");
    }
}