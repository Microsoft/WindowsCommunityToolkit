// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Deferred;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Provide data for <see cref="RichSuggestBox.SuggestionsRequested"/> event.
    /// </summary>
    public class SuggestionsRequestedEventArgs : DeferredCancelEventArgs
    {
        /// <summary>
        /// Gets or sets the prefix character used for the query.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the query for suggestions.
        /// </summary>
        public string Query { get; set; }
    }
}
