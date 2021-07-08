// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Common.Deferred;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Event arguments for <see cref="TokenizingTextBox.TokenItemRemoving"/> event.
    /// </summary>
    public class TokenItemRemovingEventArgs : DeferredCancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenItemRemovingEventArgs"/> class.
        /// </summary>
        /// <param name="item">Item being removed.</param>
        /// <param name="token"><see cref="TokenizingTextBoxItem"/> container being closed.</param>
        public TokenItemRemovingEventArgs(object item, TokenizingTextBoxItem token)
        {
            Item = item;
            Token = token;
        }

        /// <summary>
        /// Gets the Item being closed.
        /// </summary>
        public object Item { get; private set; }

        /// <summary>
        /// Gets the <see cref="TokenizingTextBoxItem"/> being removed.
        /// </summary>
        public TokenizingTextBoxItem Token { get; private set; }
    }
}