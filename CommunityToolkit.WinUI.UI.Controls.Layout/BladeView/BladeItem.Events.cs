// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Xaml;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// The Blade is used as a child in the BladeView
    /// </summary>
    public partial class BladeItem
    {
        /// <summary>
        /// Fires when the blade is opened or closed
        /// </summary>
        public event EventHandler<Visibility> VisibilityChanged;
    }
}