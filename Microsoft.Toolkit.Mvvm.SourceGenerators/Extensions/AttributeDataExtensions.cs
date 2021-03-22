// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.CodeAnalysis;

namespace Microsoft.Toolkit.Mvvm.SourceGenerators.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="AttributeData"/> type.
    /// </summary>
    internal static class AttributeDataExtensions
    {
        /// <summary>
        /// Checks whether a given <see cref="AttributeData"/> instance contains a specified named argument.
        /// </summary>
        /// <typeparam name="T">The type of argument to check.</typeparam>
        /// <param name="attributeData">The target <see cref="AttributeData"/> instance to check.</param>
        /// <param name="name">The name of the argument to check.</param>
        /// <param name="value">The expected value for the target named argument.</param>
        /// <returns>Whether or not <paramref name="attributeData"/> contains an argument named <paramref name="name"/> with the expected value.</returns>
        [Pure]
        public static bool HasNamedArgument<T>(this AttributeData attributeData, string name, T? value)
        {
            foreach (KeyValuePair<string, TypedConstant> properties in attributeData.NamedArguments)
            {
                if (properties.Key == name)
                {
                    return
                        properties.Value.Value is T argumentValue &&
                        EqualityComparer<T?>.Default.Equals(argumentValue, value);
                }
            }

            return false;
        }
    }
}
