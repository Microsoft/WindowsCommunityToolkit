// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Placement modes for the suggestion popup in <see cref="RichSuggestBox"/>.
    /// </summary>
    public enum SuggestionPopupPlacementMode
    {
        /// <summary>
        /// Suggestion popup floats above or below the typing caret.
        /// </summary>
        Floating,

        /// <summary>
        /// Suggestion popup is attached to either the top edge or the bottom edge of the text box.
        /// </summary>
        /// <remarks>
        /// In this mode, popup width will be text box's width and the interior corners that connect the text box and the popup are square.
        /// This is the same behavior as in <see cref="AutoSuggestBox"/>.
        /// </remarks>
        Attached
    }
}
