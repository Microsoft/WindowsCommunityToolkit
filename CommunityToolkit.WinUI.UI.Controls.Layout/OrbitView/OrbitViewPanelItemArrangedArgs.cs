// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Event args used by the <see cref="OrbitViewPanel"/> ItemArranged event
    /// </summary>
    public class OrbitViewPanelItemArrangedArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the <see cref="ElementProperties"/> or arranged item
        /// </summary>
        public OrbitViewElementProperties ElementProperties { get; set; }

        /// <summary>
        /// Gets or sets the index of the item that was arranged
        /// </summary>
        public int ItemIndex { get; set; }
    }
}