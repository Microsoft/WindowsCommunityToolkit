// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
    public sealed class ObservableObjectGenerator : TransitiveMembersGenerator<ObservableObjectAttribute>
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
            // Check if the type already implements INotifyPropertyChanged...
            if (classDeclarationSymbol.AllInterfaces.Any(static i => i.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == $"global::{typeof(INotifyPropertyChanged).FullName}"))
            {
                descriptor = DuplicateINotifyPropertyChangedInterfaceForObservableObjectAttributeError;

                return false;
            }

            // ...or INotifyPropertyChanging
            if (classDeclarationSymbol.AllInterfaces.Any(static i => i.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == $"global::{typeof(INotifyPropertyChanging).FullName}"))
            {
                descriptor = DuplicateINotifyPropertyChangingInterfaceForObservableObjectAttributeError;

                return false;
            }

            descriptor = null;

            return true;
        }
    }
}
