// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.Toolkit.Uwp.Helpers
{
#pragma warning disable CS1584 // XML comment has syntactically incorrect cref attribute
#pragma warning disable CS1658 // Warning is overriding an error
    /// <summary>
    /// Provides an enumerable way to look at query string parameters from a Uri
    /// </summary>
    /// <seealso cref="System.Collections.ObjectModel.Collection{System.Collections.Generic.KeyValuePair{string, string}}" />
    public class QueryParameterCollection : Collection<KeyValuePair<string, string>>
#pragma warning restore CS1658 // Warning is overriding an error
#pragma warning restore CS1584 // XML comment has syntactically incorrect cref attribute
    {
        private static IList<KeyValuePair<string, string>> CreatePairsFromUri(string uri)
        {
            var queryStartPosition = uri?.IndexOf('?');
            if (queryStartPosition.GetValueOrDefault(-1) != -1)
            { // Uri has a query string
                var queryString = uri.Substring(queryStartPosition.Value + 1);
                return queryString.Split('&')
                    .Select(param =>
                    {
                        var kvp = param.Split('=');
                        return new KeyValuePair<string, string>(System.Net.WebUtility.UrlDecode(kvp[0]), System.Net.WebUtility.UrlDecode(kvp[1]));
                    }).ToList();
            }
            else
            {
                return new List<KeyValuePair<string, string>>();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameterCollection"/> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public QueryParameterCollection(Uri uri)
            : this(uri?.OriginalString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameterCollection"/> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public QueryParameterCollection(string uri)
            : base(CreatePairsFromUri(uri))
        {
        }
    }
}