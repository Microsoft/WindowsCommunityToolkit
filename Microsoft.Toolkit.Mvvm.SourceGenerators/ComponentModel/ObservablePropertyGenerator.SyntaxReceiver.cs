// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Microsoft.Toolkit.Mvvm.SourceGenerators
{
    /// <inheritdoc cref="ObservablePropertyGenerator"/>
    public sealed partial class ObservablePropertyGenerator
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
                if (context.Node is FieldDeclarationSyntax { AttributeLists: { Count: > 0 } } fieldDeclaration &&
                    context.SemanticModel.Compilation.GetTypeByMetadataName(typeof(ObservablePropertyAttribute).FullName) is INamedTypeSymbol attributeSymbol)
                {
                    SyntaxTriviaList leadingTrivia = fieldDeclaration.GetLeadingTrivia();

                    foreach (VariableDeclaratorSyntax variableDeclarator in fieldDeclaration.Declaration.Variables)
                    {
                        if (context.SemanticModel.GetDeclaredSymbol(variableDeclarator) is IFieldSymbol fieldSymbol &&
                            fieldSymbol.GetAttributes().FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol)) is AttributeData attributeData &&
                            attributeData.ApplicationSyntaxReference is SyntaxReference syntaxReference &&
                            syntaxReference.GetSyntax() is AttributeSyntax attributeSyntax)
                        {
                            this.gatheredInfo.Add(new Item(leadingTrivia, variableDeclarator, fieldSymbol, attributeSyntax, attributeData));
                        }
                    }
                }
            }

            /// <summary>
            /// A model for a group of item representing a discovered type to process.
            /// </summary>
            /// <param name="LeadingTrivia">The leading trivia for the field declaration.</param>
            /// <param name="FieldDeclarator">The <see cref="VariableDeclaratorSyntax"/> instance for the target field variable declaration.</param>
            /// <param name="FieldSymbol">The <see cref="IFieldSymbol"/> instance for <paramref name="FieldDeclarator"/>.</param>
            /// <param name="AttributeSyntax">The <see cref="AttributeSyntax"/> instance for the target attribute over <paramref name="FieldDeclarator"/>.</param>
            /// <param name="AttributeData">The <see cref="AttributeData"/> instance for <paramref name="AttributeSyntax"/>.</param>
            public sealed record Item(
                SyntaxTriviaList LeadingTrivia,
                VariableDeclaratorSyntax FieldDeclarator,
                IFieldSymbol FieldSymbol,
                AttributeSyntax AttributeSyntax,
                AttributeData AttributeData);
        }
    }
}
