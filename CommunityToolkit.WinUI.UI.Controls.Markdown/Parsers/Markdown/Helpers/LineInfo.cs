// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.Common.Parsers.Markdown.Helpers
{
    [Obsolete(Constants.ParserObsoleteMsg)]
    internal class LineInfo
    {
        public int StartOfLine { get; set; }

        public int FirstNonWhitespaceChar { get; set; }

        public int EndOfLine { get; set; }

        public bool IsLineBlank
        {
            get
            {
                return FirstNonWhitespaceChar == EndOfLine;
            }
        }

        public int StartOfNextLine { get; set; }
    }
}