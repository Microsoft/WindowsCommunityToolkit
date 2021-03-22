// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using static Microsoft.Toolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace Microsoft.Toolkit.Mvvm.SourceGenerators
{
    /// <summary>
    /// A source generator for the <see cref="ObservableObjectAttribute"/> type.
    /// </summary>
    [Generator]
    public class ObservableObjectGenerator : TransitiveMembersGenerator<ObservableObjectAttribute>
    {
        /// <inheritdoc/>
        protected override DiagnosticDescriptor TargetTypeErrorDescriptor => ObservableObjectGeneratorError;

        /// <inheritdoc/>
        protected override bool ValidateTargetType(
            AttributeData attributeData,
            ClassDeclarationSyntax classDeclaration,
            INamedTypeSymbol classDeclarationSymbol,
            [NotNullWhen(false)] out DiagnosticDescriptor? descriptor)
        {
            throw new System.NotImplementedException();
        }
    }
}
