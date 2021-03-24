// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Toolkit.Mvvm.SourceGenerators.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Toolkit.Mvvm.SourceGenerators
{
    /// <summary>
    /// A source generator for properties validation without relying on compiled LINQ expressions.
    /// </summary>
    [Generator]
    public sealed class ObservableValidatorValidateAllPropertiesGenerator : ISourceGenerator
    {
        /// <inheritdoc/>
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        /// <inheritdoc/>
        public void Execute(GeneratorExecutionContext context)
        {
            // Find all the class symbols inheriting from ObservableValidator, that are not generic
            IEnumerable<INamedTypeSymbol> classSymbols =
                from syntaxTree in context.Compilation.SyntaxTrees
                let semanticModel = context.Compilation.GetSemanticModel(syntaxTree)
                from classDeclaration in syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
                let classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration)
                where
                    classSymbol is { IsGenericType: false } &&
                    classSymbol.InheritsFrom("Microsoft.Toolkit.Mvvm.ComponentModel.ObservableValidator")
                select classSymbol;

            // Prepare the attributes to add to the first class declaration
            AttributeListSyntax[] classAttributes = new[]
            {
                AttributeList(SingletonSeparatedList(
                    Attribute(IdentifierName("EditorBrowsable")).AddArgumentListArguments(
                    AttributeArgument(ParseExpression("EditorBrowsableState.Never"))))),
                AttributeList(SingletonSeparatedList(
                    Attribute(IdentifierName("Obsolete")).AddArgumentListArguments(
                    AttributeArgument(LiteralExpression(
                        SyntaxKind.StringLiteralExpression,
                        Literal("This type is not intended to be used directly by user code"))))))
            };

            foreach (INamedTypeSymbol classSymbol in classSymbols)
            {
                // Create a static method to validate all properties in a given class.
                // This code takes a class symbol and produces a compilation unit as follows:
                //
                // // Licensed to the .NET Foundation under one or more agreements.
                // // The .NET Foundation licenses this file to you under the MIT license.
                // // See the LICENSE file in the project root for more information.
                //
                // #pragma warning disable
                //
                // using System;
                // using System.ComponentModel;
                //
                // namespace Microsoft.Toolkit.Mvvm.ComponentModel.__Internals
                // {
                //     [EditorBrowsable(EditorBrowsableState.Never)]
                //     [Obsolete("This type is not intended to be used directly by user code")]
                //     internal static partial class __ObservableValidatorExtensions
                //     {
                //         [EditorBrowsable(EditorBrowsableState.Never)]
                //         [Obsolete("This method is not intended to be called directly by user code")]
                //         public static void ValidateAllProperties(<INSTANCE_TYPE> instance)
                //         {
                //             <BODY>
                //         }
                //     }
                // }
                var source =
                    CompilationUnit().AddUsings(
                    UsingDirective(IdentifierName("System")).WithLeadingTrivia(TriviaList(
                        Comment("// Licensed to the .NET Foundation under one or more agreements."),
                        Comment("// The .NET Foundation licenses this file to you under the MIT license."),
                        Comment("// See the LICENSE file in the project root for more information."),
                        Trivia(PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true)))),
                    UsingDirective(IdentifierName("System.ComponentModel"))).AddMembers(
                    NamespaceDeclaration(IdentifierName("Microsoft.Toolkit.Mvvm.ComponentModel.__Internals")).AddMembers(
                    ClassDeclaration("__ObservableValidatorExtensions").AddModifiers(
                        Token(SyntaxKind.InternalKeyword),
                        Token(SyntaxKind.StaticKeyword),
                        Token(SyntaxKind.PartialKeyword)).AddAttributeLists(classAttributes).AddMembers(
                    MethodDeclaration(
                        PredefinedType(Token(SyntaxKind.VoidKeyword)),
                        Identifier("ValidateAllProperties")).AddAttributeLists(
                            AttributeList(SingletonSeparatedList(
                                Attribute(IdentifierName("EditorBrowsable")).AddArgumentListArguments(
                                AttributeArgument(ParseExpression("EditorBrowsableState.Never"))))),
                            AttributeList(SingletonSeparatedList(
                                Attribute(IdentifierName("Obsolete")).AddArgumentListArguments(
                                AttributeArgument(LiteralExpression(
                                    SyntaxKind.StringLiteralExpression,
                                    Literal("This method is not intended to be called directly by user code"))))))).AddModifiers(
                        Token(SyntaxKind.PublicKeyword),
                        Token(SyntaxKind.StaticKeyword)).AddParameterListParameters(
                            Parameter(Identifier("instance")).WithType(IdentifierName(classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))))
                        .WithBody(Block(EnumerateValidationStatements(classSymbol).ToArray())))))
                    .NormalizeWhitespace()
                    .ToFullString();

                // Reset the attributes list (so the same class doesn't get duplicate attributes)
                classAttributes = Array.Empty<AttributeListSyntax>();

                // Add the partial type
                context.AddSource($"[ObservableValidator]_[{classSymbol.GetFullMetadataNameForFileName()}].cs", SourceText.From(source, Encoding.UTF8));
            }
        }

        /// <summary>
        /// Gets a sequence of statements to validate declared properties.
        /// </summary>
        /// <param name="classSymbol">The input <see cref="INamedTypeSymbol"/> instance to process.</param>
        /// <returns>The sequence of <see cref="StatementSyntax"/> instances to validate declared properties.</returns>
        [Pure]
        private static IEnumerable<StatementSyntax> EnumerateValidationStatements(INamedTypeSymbol classSymbol)
        {
            foreach (var propertySymbol in classSymbol.GetMembers().OfType<IPropertySymbol>())
            {
                if (propertySymbol.IsIndexer)
                {
                    continue;
                }

                ImmutableArray<AttributeData> attributes = propertySymbol.GetAttributes();

                if (!attributes.Any(static a => a.AttributeClass?.InheritsFrom("System.ComponentModel.DataAnnotations.ValidationAttribute") == true))
                {
                    continue;
                }

                // This enumerator produces a sequence of statements as follows:
                //
                // __ObservableValidatorHelper.ValidateProperty(instance, instance.<PROPERTY_0>, nameof(instance.<PROPERTY_0>));
                // __ObservableValidatorHelper.ValidateProperty(instance, instance.<PROPERTY_0>, nameof(instance.<PROPERTY_0>));
                // // ...
                // __ObservableValidatorHelper.ValidateProperty(instance, instance.<PROPERTY_1>, nameof(instance.<PROPERTY_1>));
                yield return
                    ExpressionStatement(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("__ObservableValidatorHelper"),
                                IdentifierName("ValidateProperty")))
                        .AddArgumentListArguments(
                            Argument(IdentifierName("instance")),
                            Argument(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("instance"),
                                    IdentifierName(propertySymbol.Name))),
                            Argument(
                                InvocationExpression(IdentifierName("nameof"))
                                .AddArgumentListArguments(Argument(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName("instance"),
                                        IdentifierName(propertySymbol.Name)))))));
            }
        }
    }
}
