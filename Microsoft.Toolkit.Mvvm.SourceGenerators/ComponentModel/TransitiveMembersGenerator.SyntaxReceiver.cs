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
            private readonly List<(ClassDeclarationSyntax Class, AttributeSyntax Attribute, AttributeData Data)> gatheredInfo = new();

            /// <summary>
            /// Gets the collection of gathered info to process.
            /// </summary>
            public IReadOnlyCollection<(ClassDeclarationSyntax Class, AttributeSyntax Attribute, AttributeData Data)> GatheredInfo => this.gatheredInfo;

            /// <inheritdoc/>
            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                if (context.Node is ClassDeclarationSyntax classDeclaration &&
                    classDeclaration.AttributeLists.Count > 0 &&
                    context.SemanticModel.GetDeclaredSymbol(classDeclaration) is INamedTypeSymbol classSymbol &&
                    classSymbol.GetAttributes().FirstOrDefault(static a => a.AttributeClass?.ToDisplayString() == typeof(TAttribute).FullName) is AttributeData attributeData &&
                    attributeData.ApplicationSyntaxReference is SyntaxReference syntaxReference &&
                    syntaxReference.GetSyntax() is AttributeSyntax attributeSyntax)
                {
                    this.gatheredInfo.Add((classDeclaration, attributeSyntax, attributeData));
                }
            }
        }
    }
}
