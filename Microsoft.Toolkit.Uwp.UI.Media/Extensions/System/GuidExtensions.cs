// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Microsoft.Toolkit.Uwp.UI.Media
{
    /// <summary>
    /// An extension <see langword="class"/> for the <see cref="Guid"/> type
    /// </summary>
    internal static class GuidExtensions
    {
        /// <summary>
        /// Returns a <see cref="string"/> representation of a <see cref="Guid"/> only made of uppercase letters
        /// </summary>
        /// <param name="guid">The input <see cref="Guid"/> to process</param>
        /// <returns>A <see cref="string"/> representation of <paramref name="guid"/> only made up of letters in the [A-Z] range</returns>
        [Pure]
        public static string ToUppercaseAsciiLetters(this Guid guid)
        {
            return new string((
                from c in guid.ToString("N")
                let l = char.IsDigit(c) ? (char)('G' + c - '0') : c
                select l).ToArray());
        }
    }
}
