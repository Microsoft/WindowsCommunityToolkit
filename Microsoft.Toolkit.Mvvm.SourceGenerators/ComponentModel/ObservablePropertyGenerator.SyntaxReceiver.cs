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
                            fieldSymbol.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol)))
                        {
                            this.gatheredInfo.Add(new Item(leadingTrivia, fieldSymbol));
                        }
                    }
                }
            }

            /// <summary>
            /// A model for a group of item representing a discovered type to process.
            /// </summary>
            /// <param name="LeadingTrivia">The leading trivia for the field declaration.</param>
            /// <param name="FieldSymbol">The <see cref="IFieldSymbol"/> instance for the target field.</param>
            public sealed record Item(SyntaxTriviaList LeadingTrivia, IFieldSymbol FieldSymbol);
        }
    }
}
