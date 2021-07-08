// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.UI.Xaml;

namespace CommunityToolkit.WinUI.UI.Animations
{
    internal class ConnectedAnimationProperties
    {
        public string Key { get; set; }

        public UIElement Element { get; set; }

        public List<ConnectedAnimationListProperty> ListAnimProperties { get; set; }

        public bool IsListAnimation => ListAnimProperties != null;
    }
}