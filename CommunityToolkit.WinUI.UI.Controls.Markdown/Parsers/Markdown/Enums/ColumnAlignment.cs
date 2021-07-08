// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.Common.Parsers.Markdown
{
    /// <summary>
    /// The alignment of content in a table column.
    /// </summary>
    [Obsolete(Constants.ParserObsoleteMsg)]
    public enum ColumnAlignment
    {
        /// <summary>
        /// The alignment was not specified.
        /// </summary>
        Unspecified,

        /// <summary>
        /// Content should be left aligned.
        /// </summary>
        Left,

        /// <summary>
        /// Content should be right aligned.
        /// </summary>
        Right,

        /// <summary>
        /// Content should be centered.
        /// </summary>
        Center,
    }
}