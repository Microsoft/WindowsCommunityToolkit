// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.Common.Parsers.Core
{
    /// <summary>
    /// The StringValue attribute is used as a helper to decorate enum values with string representations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    [Obsolete(Constants.ParserObsoleteMsg)]
    public sealed class StringValueAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringValueAttribute"/> class.
        /// Constructor accepting string value.
        /// </summary>
        /// <param name="value">String value</param>
        public StringValueAttribute(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets property for string value.
        /// </summary>
        public string Value { get; }
    }
}