// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Contracts;
using Microsoft.CodeAnalysis;

namespace Microsoft.Toolkit.Mvvm.SourceGenerators.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="INamedTypeSymbol"/> type.
    /// </summary>
    internal static class INamedTypeSymbolExtensions
    {
        /// <summary>
        /// Checks whether or not a given <see cref="INamedTypeSymbol"/> inherits from a specified type.
        /// </summary>
        /// <param name="typeSymbol">The target <see cref="INamedTypeSymbol"/> instance to check.</param>
        /// <param name="typeName">The full name of the type to check for inheritance (without global qualifier).</param>
        /// <returns>Whether or not <paramref name="typeSymbol"/> inherits from <paramref name="typeName"/>.</returns>
        [Pure]
        public static bool InheritsFrom(this INamedTypeSymbol typeSymbol, string typeName)
        {
            typeName = "global::" + typeName;

            INamedTypeSymbol? baseType = typeSymbol.BaseType;

            while (baseType != null)
            {
                if (baseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == typeName)
                {
                    return true;
                }

                baseType = baseType.BaseType;
            }

            return false;
        }
    }
}
