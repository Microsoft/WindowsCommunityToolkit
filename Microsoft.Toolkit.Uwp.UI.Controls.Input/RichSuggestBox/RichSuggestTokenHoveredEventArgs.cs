// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Input;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Provides data for <see cref="RichSuggestBox.TokenHovered"/> event.
    /// </summary>
    public class RichSuggestTokenHoveredEventArgs : RichSuggestTokenSelectedEventArgs
    {
        /// <summary>
        /// Gets or sets a <see cref="PointerPoint"/> object with position relative to the <see cref="RichSuggestBox"/> instance.
        /// </summary>
        public PointerPoint CurrentPoint { get; set; }
    }
}
