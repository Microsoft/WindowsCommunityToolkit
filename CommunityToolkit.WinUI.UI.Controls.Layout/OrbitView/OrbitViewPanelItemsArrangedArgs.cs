// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Event args used by the <see cref="OrbitViewPanel"/> ItemsArranged event
    /// </summary>
    public class OrbitViewPanelItemsArrangedArgs
    {
        /// <summary>
        /// Gets or sets a collection of all elements that were arranged.
        /// </summary>
        public List<OrbitViewElementProperties> Elements { get; set; }
    }
}