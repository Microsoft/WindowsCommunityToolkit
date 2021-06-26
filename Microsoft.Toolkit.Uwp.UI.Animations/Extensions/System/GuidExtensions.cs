// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;

namespace Microsoft.Toolkit.Uwp.UI.Animations
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
        public static string ToUppercaseAsciiLetters(in this Guid guid)
        {
            // Composition IDs must only be composed of characters in the [A-Z0-9_] set,
            // and also have the restriction that the initial character cannot be a digit.
            // Because of this, we need to prepend an underscore to a serialized guid to
            // avoid cases where the first character is a digit. Additionally, we're forced
            // to use ToUpper() here because ToString("N") currently returns a lowercase
            // hexadecimal string. Note: this extension might be improved once we move to
            // .NET 5 in the WinUI 3 release, by using string.Create<TState>(...) to only
            // have a single string allocation, and then using Guid.TryFormat(...) to
            // serialize the guid in place over the Span<char> starting from the second
            // character. For now, this implementation is fine on UWP and still fast enough.
            return $"_{guid.ToString("N").ToUpper()}";
        }
    }
}