// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Toolkit.Parsers.Markdown.Helpers;

namespace Microsoft.Toolkit.Parsers.Markdown.Inlines
{
    /// <summary>
    /// Represents a span that contains a reference for links to point to.
    /// </summary>
    public class LinkAnchorInline : MarkdownInline
    {
        internal LinkAnchorInline()
            : base(MarkdownInlineType.LinkReference)
        {
        }

        /// <summary>
        /// Gets or sets the Name of this Link Reference.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Gets or sets the raw Link Reference.
        /// </summary>
        public string Raw { get; set; }

        /// <summary>
        /// Attempts to parse a comment span.
        /// </summary>
        public new class Parser : Parser<LinkAnchorInline>
        {
            /// <inheritdoc/>
            public override IEnumerable<char> TripChar => "<";

            /// <inheritdoc/>
            protected override InlineParseResult<LinkAnchorInline> ParseInternal(string markdown, int minStart, int tripPos, int maxEnd, MarkdownDocument document, IEnumerable<Type> ignoredParsers)
            {
                if (minStart >= maxEnd - 1)
                {
                    return null;
                }

                // Check the start sequence.
                var startSequence = markdown.AsSpan(minStart, 2);
                if (startSequence.StartsWith("<a".AsSpan()))
                {
                    return null;
                }

                // Find the end of the span.  The end sequence ('-->')
                var innerStart = minStart + 2;
                int innerEnd = Common.IndexOf(markdown, "</a>", innerStart, maxEnd);
                int trueEnd = innerEnd + 4;
                if (innerEnd == -1)
                {
                    innerEnd = Common.IndexOf(markdown, "/>", innerStart, maxEnd);
                    trueEnd = innerEnd + 2;
                    if (innerEnd == -1)
                    {
                        return null;
                    }
                }

                // This link Reference wasn't closed properly if the next link starts before a close.
                var nextLink = Common.IndexOf(markdown, "<a", innerStart, maxEnd);
                if (nextLink > -1 && nextLink < innerEnd)
                {
                    return null;
                }

                var length = trueEnd - minStart;
                var contents = markdown.Substring(minStart, length);

                string link = null;

                try
                {
                    var xml = XElement.Parse(contents);
                    var attr = xml.Attribute("name");
                    if (attr != null)
                    {
                        link = attr.Value;
                    }
                }
                catch
                {
                    // Attempting to fetch link failed, ignore and continue.
                }

                // Remove whitespace if it exists.
                if (trueEnd + 1 <= maxEnd && markdown[trueEnd] == ' ')
                {
                    trueEnd += 1;
                }

                // We found something!
                var result = new LinkAnchorInline
                {
                    Raw = contents,
                    Link = link
                };
                return InlineParseResult.Create(result, minStart, trueEnd);
            }
        }

        /// <summary>
        /// Converts the object into it's textual representation.
        /// </summary>
        /// <returns> The textual representation of this object. </returns>
        public override string ToString()
        {
            return Raw;
        }
    }
}