// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Windows.UI.Xaml;

namespace UITests.App.Pages
{
    // TAEF has a different terms for the same concepts as compared with MSTest.
    // In order to allow both to use the same test files, we'll define these helper classes
    // to translate TAEF into MSTest.
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Shim helpers")]
    public static class Log
    {
        public static void Comment(string format, params object[] args)
        {
            LogMessage("Comment", format, args);
        }

        public static void Warning(string format, params object[] args)
        {
            LogMessage("Warning", "[Warning] " + format, args);
        }

        public static void Error(string format, params object[] args)
        {
            LogMessage("Error", "[Error] " + format, args);
        }

        private static void LogMessage(string level, string format, object[] args)
        {
            // string.Format() complains if we pass it something with braces, even if we have no arguments.
            // To account for that, we'll escape braces if we have no arguments.
            if (args.Length == 0)
            {
                format = format.Replace("{", "{{").Replace("}", "}}");
            }

            var message = string.Format(format, args);

            Debug.WriteLine(message);

            // Send back to Test Harness via AppService
            // TODO: Make this a cleaner connection/pattern
            _ = ((App)Application.Current).SendLogMessage(level, message);
        }
    }
}