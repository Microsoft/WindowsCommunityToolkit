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
    /// <inheritdoc cref="IMessengerRegisterAllGenerator"/>
    public sealed partial class IMessengerRegisterAllGenerator
    {
        /// <summary>
        /// An <see cref="ISyntaxContextReceiver"/> that selects candidate nodes to process.
        /// </summary>
        private sealed class SyntaxReceiver : ISyntaxContextReceiver
        {
            /// <summary>
            /// The list of info gathered during exploration.
            /// </summary>
            private readonly List<INamedTypeSymbol> gatheredInfo = new();

            /// <summary>
            /// Gets the collection of gathered info to process.
            /// </summary>
            public IReadOnlyCollection<INamedTypeSymbol> GatheredInfo => this.gatheredInfo;

            /// <inheritdoc/>
            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                if (context.Node is ClassDeclarationSyntax classDeclaration &&
                    context.SemanticModel.GetDeclaredSymbol(classDeclaration) is INamedTypeSymbol { IsGenericType: false } classSymbol &&
                    context.SemanticModel.Compilation.GetTypeByMetadataName("Microsoft.Toolkit.Mvvm.Messaging.IRecipient`1") is INamedTypeSymbol iRecipientSymbol &&
                    classSymbol.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i.OriginalDefinition, iRecipientSymbol)))
                {
                    this.gatheredInfo.Add(classSymbol);
                }
            }
        }
    }
}
