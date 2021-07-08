// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.Common.Parsers.Markdown.Inlines
{
    /// <summary>
    /// An internal class that is the base class for all inline elements.
    /// </summary>
    [Obsolete(Constants.ParserObsoleteMsg)]
    public abstract class MarkdownInline : MarkdownElement
    {
        /// <summary>
        /// Gets or sets this element is.
        /// </summary>
        public MarkdownInlineType Type { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownInline"/> class.
        /// </summary>
        internal MarkdownInline(MarkdownInlineType type)
        {
            Type = type;
        }
    }
}