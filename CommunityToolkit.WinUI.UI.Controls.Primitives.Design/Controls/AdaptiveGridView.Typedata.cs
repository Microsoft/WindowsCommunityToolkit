// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.UI.Controls.Design
{
    internal static partial class ControlTypes
    {
        internal const string AdaptiveGridView = RootNamespace + "." + nameof(AdaptiveGridView);
    }

    internal static class AdaptiveGridView
    {
        internal const string DesiredWidth = nameof(DesiredWidth);
        internal const string ItemClickCommand = nameof(ItemClickCommand);
        internal const string ItemHeight = nameof(ItemHeight);
        internal const string OneRowModeEnabled = nameof(OneRowModeEnabled);
        internal const string StretchContentForSingleRow = nameof(StretchContentForSingleRow);
    }
}