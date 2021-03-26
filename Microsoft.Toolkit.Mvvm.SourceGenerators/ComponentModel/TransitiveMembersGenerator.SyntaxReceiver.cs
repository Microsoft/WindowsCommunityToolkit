// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.Toolkit.Mvvm.SourceGenerators
{
    /// <inheritdoc cref="TransitiveMembersGenerator{TAttribute}"/>
    public abstract partial class TransitiveMembersGenerator<TAttribute>
    {
        /// <summary>
        /// An <see cref="ISyntaxContextReceiver"/> that selects candidate nodes to process.
        /// </summary>
        private sealed class SyntaxReceiver : ISyntaxContextReceiver
        {
            /// <summary>
            /// The list of info gathered during exploration.
            /// </summary>
            private readonly List<Item> gatheredInfo = new();

            /// <summary>
            /// Gets the collection of gathered info to process.
            /// </summary>
            public IReadOnlyCollection<Item> GatheredInfo => this.gatheredInfo;

            /// <inheritdoc/>
            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                if (context.Node is ClassDeclarationSyntax classDeclaration &&
                    classDeclaration.AttributeLists.Count > 0 &&
                    context.SemanticModel.GetDeclaredSymbol(classDeclaration) is INamedTypeSymbol classSymbol &&
                    context.SemanticModel.Compilation.GetTypeByMetadataName(typeof(TAttribute).FullName) is INamedTypeSymbol attributeSymbol &&
                    classSymbol.GetAttributes().FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol)) is AttributeData attributeData &&
                    attributeData.ApplicationSyntaxReference is SyntaxReference syntaxReference &&
                    syntaxReference.GetSyntax() is AttributeSyntax attributeSyntax)
                {
                    this.gatheredInfo.Add(new Item(classDeclaration, classSymbol, attributeSyntax, attributeData));
                }
            }

            /// <summary>
            /// A model for a group of item representing a discovered type to process.
            /// </summary>
            /// <param name="ClassDeclaration">The <see cref="ClassDeclarationSyntax"/> instance for the target class declaration.</param>
            /// <param name="ClassSymbol">The <see cref="INamedTypeSymbol"/> instance for <paramref name="ClassDeclaration"/>.</param>
            /// <param name="AttributeSyntax">The <see cref="AttributeSyntax"/> instance for the target attribute over <paramref name="ClassDeclaration"/>.</param>
            /// <param name="AttributeData">The <see cref="AttributeData"/> instance for <paramref name="AttributeSyntax"/>.</param>
            public sealed record Item(
                ClassDeclarationSyntax ClassDeclaration,
                INamedTypeSymbol ClassSymbol,
                AttributeSyntax AttributeSyntax,
                AttributeData AttributeData);
        }
    }
}
