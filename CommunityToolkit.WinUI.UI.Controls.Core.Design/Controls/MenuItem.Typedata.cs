// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.UI.Controls.Design
{
    internal static partial class ControlTypes
    {
        internal const string MenuItem = RootNamespace + "." + nameof(MenuItem);
    }

    internal static class MenuItem
    {
        internal const string Header = nameof(Header);
        internal const string HeaderTemplate = nameof(HeaderTemplate);
        internal const string IsOpened = nameof(IsOpened);
        internal const string Items = nameof(Items);
    }
}