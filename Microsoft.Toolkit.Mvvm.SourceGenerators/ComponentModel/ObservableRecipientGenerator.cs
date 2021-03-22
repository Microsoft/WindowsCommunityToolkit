// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.SourceGenerators.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Microsoft.Toolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace Microsoft.Toolkit.Mvvm.SourceGenerators
{
    /// <summary>
    /// A source generator for the <see cref="ObservableRecipientAttribute"/> type.
    /// </summary>
    [Generator]
    public sealed class ObservableRecipientGenerator : TransitiveMembersGenerator<ObservableRecipientAttribute>
    {
        /// <inheritdoc/>
        protected override DiagnosticDescriptor TargetTypeErrorDescriptor => ObservableRecipientGeneratorError;

        /// <inheritdoc/>
        protected override bool ValidateTargetType(
            AttributeData attributeData,
            ClassDeclarationSyntax classDeclaration,
            INamedTypeSymbol classDeclarationSymbol,
            [NotNullWhen(false)] out DiagnosticDescriptor? descriptor)
        {
            // Check if the type already inherits from ObservableRecipient
            if (classDeclarationSymbol.InheritsFrom("Microsoft.Toolkit.Mvvm.ComponentModel.ObservableRecipient"))
            {
                descriptor = DuplicateObservableRecipientError;

                return false;
            }

            // In order to use [ObservableRecipient], the target type needs to inherit from ObservableObject,
            // or be annotated with [ObservableObject] or [INotifyPropertyChanged] (with additional helpers).
            if (!classDeclarationSymbol.InheritsFrom("Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject") &&
                !classDeclarationSymbol.GetAttributes().Any(static a =>
                    a.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObjectAttribute") &&
                !classDeclarationSymbol.GetAttributes().Any(static a =>
                    a.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::Microsoft.Toolkit.Mvvm.ComponentModel.INotifyPropertyChangedAttribute" &&
                    !a.HasNamedArgument(nameof(INotifyPropertyChangedAttribute.IncludeAdditionalHelperMethods), false)))
            {
                descriptor = MissingBaseObservableObjectFunctionalityError;

                return false;
            }

            descriptor = null;

            return true;
        }

        /// <inheritdoc/>
        protected override IEnumerable<MemberDeclarationSyntax> FilterDeclaredMembers(
            AttributeData attributeData,
            ClassDeclarationSyntax classDeclaration,
            INamedTypeSymbol classDeclarationSymbol,
            ClassDeclarationSyntax sourceDeclaration)
        {
            // If the target type has no constructors, generate constructors as well
            if (classDeclarationSymbol.InstanceConstructors.Length == 1 &&
                classDeclarationSymbol.InstanceConstructors[0] is
                {
                    Parameters: { IsEmpty: true },
                    DeclaringSyntaxReferences: { IsEmpty: true },
                    IsImplicitlyDeclared: true
                })
            {
                foreach (ConstructorDeclarationSyntax ctor in sourceDeclaration.Members.OfType<ConstructorDeclarationSyntax>())
                {
                    string
                        text = ctor.NormalizeWhitespace().ToFullString(),
                        replaced = text.Replace("ObservableRecipient", classDeclarationSymbol.Name);

                    // Adjust the visibility of the constructors based on whether the target type is abstract.
                    // If that is not the case, the constructors have to be declared as public and not protected.
                    if (!classDeclarationSymbol.IsAbstract)
                    {
                        replaced = replaced.Replace("protected", "public");
                    }

                    yield return (ConstructorDeclarationSyntax)ParseMemberDeclaration(replaced)!;
                }
            }

            // Skip the SetProperty overloads if the target type inherits from ObservableValidator, to avoid conflicts
            if (classDeclarationSymbol.InheritsFrom("Microsoft.Toolkit.Mvvm.ComponentModel.ObservableValidator"))
            {
                foreach (MemberDeclarationSyntax member in sourceDeclaration.Members.Where(static member => member is not ConstructorDeclarationSyntax))
                {
                    if (member is not MethodDeclarationSyntax { Identifier: { ValueText: "SetProperty" } })
                    {
                        yield return member;
                    }
                }

                yield break;
            }

            // If the target type has at least one custom constructor, only generate methods
            foreach (MemberDeclarationSyntax member in sourceDeclaration.Members.Where(static member => member is not ConstructorDeclarationSyntax))
            {
                yield return member;
            }
        }
    }
}
