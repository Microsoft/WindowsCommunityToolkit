// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;

namespace Microsoft.Toolkit.Uwp.UI.Media
{
    /// <summary>
    /// An extension <see langword="class"/> for the <see cref="Uri"/> type
    /// </summary>
    internal static class UriExtensions
    {
        /// <summary>
        /// Returns an <see cref="Uri"/> that starts with the ms-appx:// prefix
        /// </summary>
        /// <param name="uri">The input <see cref="Uri"/> to process</param>
        /// <returns>A <see cref="Uri"/> equivalent to the first but relative to ms-appx://</returns>
        /// <remarks>This is needed because the XAML converter doesn't use the ms-appx:// prefix</remarks>
        [Pure]
        public static Uri ToAppxUri(this Uri uri)
        {
            if (uri.Scheme.Equals("ms-resource"))
            {
                string path = uri.AbsolutePath.StartsWith("/Files")
                    ? uri.AbsolutePath.Replace("/Files", string.Empty)
                    : uri.AbsolutePath;

                return new Uri($"ms-appx://{path}");
            }

            return uri;
        }

        /// <summary>
        /// Returns an <see cref="Uri"/> that starts with the ms-appx:// prefix
        /// </summary>
        /// <param name="path">The input relative path to convert</param>
        /// <returns>A <see cref="Uri"/> with <paramref name="path"/> relative to ms-appx://</returns>
        [Pure]
        public static Uri ToAppxUri(this string path)
        {
            string prefix = $"ms-appx://{(path.StartsWith('/') ? string.Empty : "/")}";

            return new Uri($"{prefix}{path}");
        }
    }
}
