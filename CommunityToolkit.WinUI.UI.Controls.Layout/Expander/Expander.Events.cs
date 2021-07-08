// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// The <see cref="Expander"/> control allows user to show/hide content based on a boolean state
    /// </summary>
    public partial class Expander
    {
        /// <summary>
        /// Fires when the expander is opened
        /// </summary>
        public event EventHandler Expanded;

        /// <summary>
        /// Fires when the expander is closed
        /// </summary>
        public event EventHandler Collapsed;
    }
}