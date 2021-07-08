// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.UI.Controls.Design
{
    internal static partial class ControlTypes
    {
        internal const string TabbedCommandBarItem = RootNamespace + "." + nameof(TabbedCommandBarItem);
    }

    internal static class TabbedCommandBarItem
    {
        internal const string Header = nameof(Header);
        internal const string Footer = nameof(Footer);
        internal const string IsContextual = nameof(IsContextual);
        internal const string OverflowButtonAlignment = nameof(OverflowButtonAlignment);
    }
}