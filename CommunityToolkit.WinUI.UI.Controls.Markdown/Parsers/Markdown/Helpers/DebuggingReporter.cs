// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;

namespace CommunityToolkit.Common.Parsers.Markdown.Helpers
{
    /// <summary>
    /// Reports an error during debugging.
    /// </summary>
    [Obsolete(Constants.ParserObsoleteMsg)]
    internal class DebuggingReporter
    {
        /// <summary>
        /// Reports a critical error.
        /// </summary>
        public static void ReportCriticalError(string errorText)
        {
            Debug.WriteLine(errorText);
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }
    }
}