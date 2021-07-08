// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Xaml;

namespace CommunityToolkit.WinUI.SampleApp.Models
{
    public class ThemeChangedArgs : EventArgs
    {
        /// <summary>
        /// Gets the Current UI Theme
        /// </summary>
        public ElementTheme Theme { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the Theme was set by the User.
        /// </summary>
        public bool CustomSet { get; internal set; }
    }
}