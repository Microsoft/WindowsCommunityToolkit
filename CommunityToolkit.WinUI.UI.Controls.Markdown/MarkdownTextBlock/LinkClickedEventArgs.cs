// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Arguments for the OnLinkClicked event which is fired then the user presses a link.
    /// </summary>
    public class LinkClickedEventArgs : EventArgs
    {
        internal LinkClickedEventArgs(string link)
        {
            Link = link;
        }

        /// <summary>
        /// Gets the link that was tapped.
        /// </summary>
        public string Link { get; }
    }
}