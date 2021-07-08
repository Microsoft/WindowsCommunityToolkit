// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// A class used by the <see cref="OrbitView"/> ItemClicked Event
    /// </summary>
    public class OrbitViewItemClickedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrbitViewItemClickedEventArgs"/> class.
        /// </summary>
        /// <param name="item">data context of element clicked</param>
        public OrbitViewItemClickedEventArgs(object item)
        {
            Item = item;
        }

        /// <summary>
        /// Gets or sets the Item/Data Context of the clicked item
        /// </summary>
        public object Item { get; set; }
    }
}