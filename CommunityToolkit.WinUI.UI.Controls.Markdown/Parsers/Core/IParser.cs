// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace CommunityToolkit.Common.Parsers
{
    /// <summary>
    /// Parser interface.
    /// </summary>
    /// <typeparam name="T">Type to parse into.</typeparam>
    [Obsolete(Constants.ParserObsoleteMsg)]
    public interface IParser<out T>
        where T : SchemaBase
    {
        /// <summary>
        /// Parse method which all classes must implement.
        /// </summary>
        /// <param name="data">Data to parse.</param>
        /// <returns>Strong typed parsed data.</returns>
        IEnumerable<T> Parse(string data);
    }
}