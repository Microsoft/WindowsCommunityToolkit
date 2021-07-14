// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Toolkit.Deferred;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Provides data for the <see cref="RichSuggestBox.SuggestionChosen"/> event.
    /// </summary>
    public class SuggestionChosenEventArgs : DeferredEventArgs
    {
        /// <summary>
        /// Gets the query used for this token.
        /// </summary>
        public string Query { get; internal set; }

        /// <summary>
        /// Gets the prefix character used for this token.
        /// </summary>
        public string Prefix { get; internal set; }

        /// <summary>
        /// Gets or sets the display text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets the suggestion item associated with this token.
        /// </summary>
        public object SelectedItem { get; internal set; }

        /// <summary>
        /// Gets token ID.
        /// </summary>
        public Guid Id { get; internal set; }

        /// <summary>
        /// Gets the formatting construct to override this token formatting.
        /// </summary>
        public RichSuggestTokenFormat Format { get; internal set; }
    }
}
