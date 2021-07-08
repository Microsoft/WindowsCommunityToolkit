// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace CommunityToolkit.Common.Parsers.Markdown.Blocks
{
    /// <summary>
    /// This specifies the Content of the List element.
    /// </summary>
    [Obsolete(Constants.ParserObsoleteMsg)]
    public class ListItemBlock
    {
        /// <summary>
        /// Gets or sets the contents of the list item.
        /// </summary>
        public IList<MarkdownBlock> Blocks { get; set; }

        internal ListItemBlock()
        {
        }
    }
}