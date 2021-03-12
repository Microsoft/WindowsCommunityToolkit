// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace Microsoft.Toolkit
{
    /// <summary>
    /// Helpers for working with strings and string representations.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Regular expression for matching a phone number.
        /// </summary>
        internal const string PhoneNumberRegex = @"^[+]?(\d{1,3})?[\s.-]?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$";

        /// <summary>
        /// Regular expression for matching a string that contains only letters.
        /// </summary>
        internal const string CharactersRegex = "^[A-Za-z]+$";

        /// <summary>
        /// Regular expression for matching an email address.
        /// </summary>
        /// <remarks>General Email Regex (RFC 5322 Official Standard) from https://emailregex.com.</remarks>
        internal const string EmailRegex = "(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])";

        /// <summary>
        /// Regular expression of HTML tags to remove.
        /// </summary>
        private const string RemoveHtmlTagsRegex = @"(?></?\w+)(?>(?:[^>'""]+|'[^']*'|""[^""]*"")*)>";

        /// <summary>
        /// Regular expression for removing comments from HTML.
        /// </summary>
        private static readonly Regex RemoveHtmlCommentsRegex = new("<!--.*?-->", RegexOptions.Singleline);

        /// <summary>
        /// Regular expression for removing scripts from HTML.
        /// </summary>
        private static readonly Regex RemoveHtmlScriptsRegex = new(@"(?s)<script.*?(/>|</script>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        /// <summary>
        /// Regular expression for removing styles from HTML.
        /// </summary>
        private static readonly Regex RemoveHtmlStylesRegex = new(@"(?s)<style.*?(/>|</style>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        /// <summary>
        /// Determines whether a string is a valid email address.
        /// </summary>
        /// <param name="str">The string to test.</param>
        /// <returns><c>true</c> for a valid email address; otherwise, <c>false</c>.</returns>
        public static bool IsEmail(this string str) => Regex.IsMatch(str, EmailRegex);

        /// <summary>
        /// Determines whether a string is a valid decimal number.
        /// </summary>
        /// <param name="str">The string to test.</param>
        /// <returns><c>true</c> for a valid decimal number; otherwise, <c>false</c>.</returns>
        public static bool IsDecimal([NotNullWhen(true)] this string? str)
        {
            return decimal.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out _);
        }

        /// <summary>
        /// Determines whether a string is a valid integer.
        /// </summary>
        /// <param name="str">The string to test.</param>
        /// <returns><c>true</c> for a valid integer; otherwise, <c>false</c>.</returns>
        public static bool IsNumeric([NotNullWhen(true)] this string? str)
        {
            return int.TryParse(str, out _);
        }

        /// <summary>
        /// Determines whether a string is a valid phone number.
        /// </summary>
        /// <param name="str">The string to test.</param>
        /// <returns><c>true</c> for a valid phone number; otherwise, <c>false</c>.</returns>
        public static bool IsPhoneNumber(this string str) => Regex.IsMatch(str, PhoneNumberRegex);

        /// <summary>
        /// Determines whether a string contains only letters.
        /// </summary>
        /// <param name="str">The string to test.</param>
        /// <returns><c>true</c> if the string contains only letters; otherwise, <c>false</c>.</returns>
        public static bool IsCharacterString(this string str) => Regex.IsMatch(str, CharactersRegex);

        /// <summary>
        /// Returns a string with HTML comments, scripts, styles, and tags removed.
        /// </summary>
        /// <param name="htmlText">HTML string.</param>
        /// <returns>Decoded HTML string.</returns>
        [return: NotNullIfNotNull("htmlText")]
        public static string? DecodeHtml(this string? htmlText)
        {
            if (htmlText is null)
            {
                return null;
            }

            var ret = htmlText.FixHtml();

            // Remove html tags
            ret = new Regex(RemoveHtmlTagsRegex).Replace(ret, string.Empty);

            return WebUtility.HtmlDecode(ret);
        }

        /// <summary>
        /// Returns a string with HTML comments, scripts, and styles removed.
        /// </summary>
        /// <param name="html">HTML string to fix.</param>
        /// <returns>Fixed HTML string.</returns>
        public static string FixHtml(this string html)
        {
            // Remove comments
            var withoutComments = RemoveHtmlCommentsRegex.Replace(html, string.Empty);

            // Remove scripts
            var withoutScripts = RemoveHtmlScriptsRegex.Replace(withoutComments, string.Empty);

            // Remove styles
            var withoutStyles = RemoveHtmlStylesRegex.Replace(withoutScripts, string.Empty);

            return withoutStyles;
        }

        /// <summary>
        /// Truncates a string to the specified length.
        /// </summary>
        /// <param name="value">The string to be truncated.</param>
        /// <param name="length">The maximum length.</param>
        /// <returns>Truncated string.</returns>
        public static string Truncate(this string? value, int length) => Truncate(value, length, false);

        /// <summary>
        /// Provide better linking for resourced strings.
        /// </summary>
        /// <param name="format">The format of the string being linked.</param>
        /// <param name="args">The object which will receive the linked String.</param>
        /// <returns>Truncated string.</returns>
        [Obsolete("This method will be removed in a future version of the Toolkit. Use the native C# string interpolation syntax instead, see: https://docs.microsoft.com/dotnet/csharp/language-reference/tokens/interpolated")]
        public static string AsFormat(this string format, params object[] args)
        {
            // Note: this extension was originally added to help developers using {x:Bind} in XAML, but
            // due to a known limitation in the UWP/WinUI XAML compiler, using either this method or the
            // standard string.Format method from the BCL directly doesn't always work. Since this method
            // doesn't actually provide any benefit over the built-in one, it has been marked as obsolete.
            // For more details, see the WinUI issue on the XAML compiler limitation here:
            // https://github.com/microsoft/microsoft-ui-xaml/issues/2654.
            return string.Format(format, args);
        }

        /// <summary>
        /// Truncates a string to the specified length.
        /// </summary>
        /// <param name="value">The string to be truncated.</param>
        /// <param name="length">The maximum length.</param>
        /// <param name="ellipsis"><c>true</c> to add ellipsis to the truncated text; otherwise, <c>false</c>.</param>
        /// <returns>Truncated string.</returns>
        public static string Truncate(this string? value, int length, bool ellipsis)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value!.Trim();

                if (value.Length > length)
                {
                    if (ellipsis)
                    {
                        return value.Substring(0, length) + "...";
                    }

                    return value.Substring(0, length);
                }
            }

            return value ?? string.Empty;
        }
    }
}
