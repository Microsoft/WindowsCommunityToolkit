// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.Common.Parsers.Markdown
{
    /// <summary>
    /// This specifies the type of style the List will be.
    /// </summary>
    [Obsolete(Constants.ParserObsoleteMsg)]
    public enum ListStyle
    {
        /// <summary>
        /// A list with bullets
        /// </summary>
        Bulleted,

        /// <summary>
        /// A numbered list
        /// </summary>
        Numbered,
    }
}