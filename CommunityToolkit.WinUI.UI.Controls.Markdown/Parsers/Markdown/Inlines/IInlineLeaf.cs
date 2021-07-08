// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.Common.Parsers.Markdown.Inlines
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IInlineLeaf"/> class.
    /// </summary>
    [Obsolete(Constants.ParserObsoleteMsg)]
    public interface IInlineLeaf
    {
        /// <summary>
        /// Gets or sets the text for this run.
        /// </summary>
        string Text { get; set; }
    }
}