// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.Foundation;
using Windows.UI.Text;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Provides data for <see cref="RichSuggestBox.TokenSelected"/> event.
    /// </summary>
    public class RichSuggestTokenEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the selected token.
        /// </summary>
        public RichSuggestToken Token { get; set; }

        /// <summary>
        /// Gets or sets the position and size of the selected rectangle measured from the top left of the <see cref="RichSuggestBox"/> control.
        /// </summary>
        public Rect Rect { get; set; }

        /// <summary>
        /// Gets or sets the range associated with the token.
        /// </summary>
        public ITextRange Range { get; set; }
    }
}
