// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.SourceGenerators.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Microsoft.CodeAnalysis.SymbolDisplayTypeQualificationStyle;

namespace Microsoft.Toolkit.Mvvm.SourceGenerators
{
    /// <summary>
    /// A source generator for generating command properties from annotated methods.
    /// </summary>
    [Generator]
    public sealed partial class ICommandGenerator : ISourceGenerator
    {
        /// <inheritdoc/>
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(static () => new SyntaxReceiver());
        }

        /// <inheritdoc/>
        public void Execute(GeneratorExecutionContext context)
        {
            // Get the syntax receiver with the candidate nodes
            if (context.SyntaxContextReceiver is not SyntaxReceiver syntaxReceiver ||
                syntaxReceiver.GatheredInfo.Count == 0)
            {
                return;
            }

            foreach (var items in syntaxReceiver.GatheredInfo.GroupBy<IMethodSymbol, INamedTypeSymbol>(static item => item.ContainingType, SymbolEqualityComparer.Default))
            {
                if (items.Key.DeclaringSyntaxReferences.Length > 0 &&
                    items.Key.DeclaringSyntaxReferences.First().GetSyntax() is ClassDeclarationSyntax classDeclaration)
                {
                    try
                    {
                        OnExecute(context, classDeclaration, items.Key, items);
                    }
                    catch
                    {
                        // TODO
                    }
                }
            }
        }

        /// <summary>
        /// Processes a given target type.
        /// </summary>
        /// <param name="context">The input <see cref="GeneratorExecutionContext"/> instance to use.</param>
        /// <param name="classDeclaration">The <see cref="ClassDeclarationSyntax"/> node to process.</param>
        /// <param name="classDeclarationSymbol">The <see cref="INamedTypeSymbol"/> for <paramref name="classDeclaration"/>.</param>
        /// <param name="methodSymbols">The sequence of <see cref="IMethodSymbol"/> instances to process.</param>
        private static void OnExecute(
            GeneratorExecutionContext context,
            ClassDeclarationSyntax classDeclaration,
            INamedTypeSymbol classDeclarationSymbol,
            IEnumerable<IMethodSymbol> methodSymbols)
        {
            // Create the class declaration for the user type. This will produce a tree as follows:
            //
            // <MODIFIERS> <CLASS_NAME>
            // {
            //     <MEMBERS>
            // }
            var classDeclarationSyntax =
                ClassDeclaration(classDeclarationSymbol.Name)
                .WithModifiers(classDeclaration.Modifiers)
                .AddMembers(methodSymbols.Select(item => CreateCommandMembers(context, default, item)).SelectMany(static g => g).ToArray());

            TypeDeclarationSyntax typeDeclarationSyntax = classDeclarationSyntax;

            // Add all parent types in ascending order, if any
            foreach (var parentType in classDeclaration.Ancestors().OfType<TypeDeclarationSyntax>())
            {
                typeDeclarationSyntax = parentType
                    .WithMembers(SingletonList<MemberDeclarationSyntax>(typeDeclarationSyntax))
                    .WithConstraintClauses(List<TypeParameterConstraintClauseSyntax>())
                    .WithBaseList(null)
                    .WithAttributeLists(List<AttributeListSyntax>())
                    .WithoutTrivia();
            }

            // Create the compilation unit with the namespace and target member.
            // From this, we can finally generate the source code to output.
            var namespaceName = classDeclarationSymbol.ContainingNamespace.ToDisplayString(new(typeQualificationStyle: NameAndContainingTypesAndNamespaces));

            // Create the final compilation unit to generate (with leading trivia)
            var source =
                CompilationUnit().AddMembers(
                NamespaceDeclaration(IdentifierName(namespaceName)).WithLeadingTrivia(TriviaList(
                    Comment("// Licensed to the .NET Foundation under one or more agreements."),
                    Comment("// The .NET Foundation licenses this file to you under the MIT license."),
                    Comment("// See the LICENSE file in the project root for more information."),
                    Trivia(PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true))))
                .AddMembers(typeDeclarationSyntax))
                .NormalizeWhitespace()
                .ToFullString();

            // Add the partial type
            context.AddSource($"[{typeof(ICommandAttribute).Name}]_[{classDeclarationSymbol.GetFullMetadataNameForFileName()}].cs", SourceText.From(source, Encoding.UTF8));
        }

        /// <summary>
        /// Creates the <see cref="MemberDeclarationSyntax"/> instances for a specified command.
        /// </summary>
        /// <param name="context">The input <see cref="GeneratorExecutionContext"/> instance to use.</param>
        /// <param name="leadingTrivia">The leading trivia for the field to process.</param>
        /// <param name="methodSymbol">The input <see cref="IMethodSymbol"/> instance to process.</param>
        /// <returns>The <see cref="MemberDeclarationSyntax"/> instances for the input command.</returns>
        [Pure]
        private static IEnumerable<MemberDeclarationSyntax> CreateCommandMembers(GeneratorExecutionContext context, SyntaxTriviaList leadingTrivia, IMethodSymbol methodSymbol)
        {
            // Get the command member names
            var (fieldName, propertyName) = GetGeneratedFieldAndPropertyNames(context, methodSymbol);

            // Get the command type symbols
            if (!TryMapCommandTypesFromMethod(
                context,
                methodSymbol,
                out ITypeSymbol? commandInterfaceTypeSymbol,
                out ITypeSymbol? commandClassTypeSymbol,
                out ITypeSymbol? delegateTypeSymbol))
            {
                return Array.Empty<MemberDeclarationSyntax>();
            }

            // Construct the generated field as follows:
            //
            // [global::System.CodeDom.Compiler.GeneratedCode("...", "...")]
            // private <COMMAND_TYPE>? <COMMAND_FIELD_NAME>;
            FieldDeclarationSyntax fieldDeclaration =
                FieldDeclaration(
                VariableDeclaration(NullableType(IdentifierName(commandInterfaceTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))))
                .AddVariables(VariableDeclarator(Identifier(fieldName))))
                .AddModifiers(Token(SyntaxKind.PrivateKeyword))
                .AddAttributeLists(AttributeList(SingletonSeparatedList(
                    Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
                    .AddArgumentListArguments(
                        AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ICommandGenerator).FullName))),
                        AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ICommandGenerator).Assembly.GetName().Version.ToString())))))));

            // Construct the generated property as follows (the explicit delegate cast is needed to avoid overload resolution conflicts):
            //
            // <METHOD_SUMMARY>
            // [global::System.CodeDom.Compiler.GeneratedCode("...", "...")]
            // [global::System.Diagnostics.DebuggerNonUserCode]
            // [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            // public <COMMAND_TYPE> <COMMAND_PROPERTY_NAME> => <COMMAND_FIELD_NAME> ??= new <RELAY_COMMAND_TYPE>(new <DELEGATE_TYPE>(<METHOD_NAME>));
            PropertyDeclarationSyntax propertyDeclaration =
                PropertyDeclaration(
                    IdentifierName(commandInterfaceTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)),
                    Identifier(propertyName))
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .AddAttributeLists(
                    AttributeList(SingletonSeparatedList(
                        Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
                        .AddArgumentListArguments(
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ICommandGenerator).FullName))),
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ICommandGenerator).Assembly.GetName().Version.ToString())))))),
                    AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.DebuggerNonUserCode")))),
                    AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage")))))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        AssignmentExpression(
                            SyntaxKind.CoalesceAssignmentExpression,
                            IdentifierName(fieldName),
                            ObjectCreationExpression(IdentifierName(commandClassTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)))
                            .AddArgumentListArguments(Argument(
                                ObjectCreationExpression(IdentifierName(delegateTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)))
                                .AddArgumentListArguments(Argument(IdentifierName(methodSymbol.Name))))))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            return new MemberDeclarationSyntax[] { fieldDeclaration, propertyDeclaration };
        }

        /// <summary>
        /// Get the generated field and property names for the input method.
        /// </summary>
        /// <param name="context">The input <see cref="GeneratorExecutionContext"/> instance to use.</param>
        /// <param name="methodSymbol">The input <see cref="IMethodSymbol"/> instance to process.</param>
        /// <returns>The generated field and property names for <paramref name="methodSymbol"/>.</returns>
        [Pure]
        private static (string FieldName, string PropertyName) GetGeneratedFieldAndPropertyNames(GeneratorExecutionContext context, IMethodSymbol methodSymbol)
        {
            string propertyName = methodSymbol.Name;

            if (SymbolEqualityComparer.Default.Equals(methodSymbol.ReturnType, context.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task")!) &&
                methodSymbol.Name.EndsWith("Async"))
            {
                propertyName = propertyName.Substring(0, propertyName.Length - "Async".Length);
            }

            propertyName += "Command";

            string fieldName = $"{char.ToLower(propertyName[0])}{propertyName.Substring(1)}";

            return (fieldName, propertyName);
        }

        /// <summary>
        /// Gets the type symbols for the input method, if supported.
        /// </summary>
        /// <param name="context">The input <see cref="GeneratorExecutionContext"/> instance to use.</param>
        /// <param name="methodSymbol">The input <see cref="IMethodSymbol"/> instance to process.</param>
        /// <param name="commandInterfaceTypeSymbol">The command interface type symbol.</param>
        /// <param name="commandClassTypeSymbol">The command class type symbol.</param>
        /// <param name="delegateTypeSymbol">The delegate type symbol for the wrapped method.</param>
        /// <returns>Whether or not <paramref name="methodSymbol"/> was valid and the requested types have been set.</returns>
        private static bool TryMapCommandTypesFromMethod(
            GeneratorExecutionContext context,
            IMethodSymbol methodSymbol,
            [NotNullWhen(true)] out ITypeSymbol? commandInterfaceTypeSymbol,
            [NotNullWhen(true)] out ITypeSymbol? commandClassTypeSymbol,
            [NotNullWhen(true)] out ITypeSymbol? delegateTypeSymbol)
        {
            // Map <void, void> to IRelayCommand, RelayCommand, Action
            if (methodSymbol.ReturnsVoid && methodSymbol.Parameters.Length == 0)
            {
                commandInterfaceTypeSymbol = context.Compilation.GetTypeByMetadataName("Microsoft.Toolkit.Mvvm.Input.IRelayCommand")!;
                commandClassTypeSymbol = context.Compilation.GetTypeByMetadataName("Microsoft.Toolkit.Mvvm.Input.RelayCommand")!;
                delegateTypeSymbol = context.Compilation.GetTypeByMetadataName("System.Action")!;

                return true;
            }

            // Map <T, void> to IRelayCommand<T>, RelayCommand<T>, Action<T>
            if (methodSymbol.ReturnsVoid &&
                methodSymbol.Parameters.Length == 1 &&
                methodSymbol.Parameters[0] is IParameterSymbol { RefKind: RefKind.None, Type: { IsRefLikeType: false, TypeKind: not TypeKind.Pointer and not TypeKind.FunctionPointer } } parameter)
            {
                commandInterfaceTypeSymbol = context.Compilation.GetTypeByMetadataName("Microsoft.Toolkit.Mvvm.Input.IRelayCommand`1")!.Construct(parameter.Type);
                commandClassTypeSymbol = context.Compilation.GetTypeByMetadataName("Microsoft.Toolkit.Mvvm.Input.RelayCommand`1")!.Construct(parameter.Type);
                delegateTypeSymbol = context.Compilation.GetTypeByMetadataName("System.Action`1")!.Construct(parameter.Type);

                return true;
            }

            if (SymbolEqualityComparer.Default.Equals(methodSymbol.ReturnType, context.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task")!))
            {
                // Map <void, Task> to IAsyncRelayCommand, AsyncRelayCommand, Func<Task>
                if (methodSymbol.Parameters.Length == 0)
                {
                    commandInterfaceTypeSymbol = context.Compilation.GetTypeByMetadataName("Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand")!;
                    commandClassTypeSymbol = context.Compilation.GetTypeByMetadataName("Microsoft.Toolkit.Mvvm.Input.AsyncRelayCommand")!;
                    delegateTypeSymbol = context.Compilation.GetTypeByMetadataName("System.Func`1")!.Construct(context.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task")!);

                    return true;
                }

                if (methodSymbol.Parameters.Length == 1 &&
                    methodSymbol.Parameters[0] is IParameterSymbol { RefKind: RefKind.None, Type: { IsRefLikeType: false, TypeKind: not TypeKind.Pointer and not TypeKind.FunctionPointer } } singleParameter)
                {
                    // Map <CancellationToken, Task> to IAsyncRelayCommand, AsyncRelayCommand, Func<CancellationToken, Task>
                    if (SymbolEqualityComparer.Default.Equals(singleParameter.Type, context.Compilation.GetTypeByMetadataName("System.Threading.CancellationToken")!))
                    {
                        commandInterfaceTypeSymbol = context.Compilation.GetTypeByMetadataName("Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand")!;
                        commandClassTypeSymbol = context.Compilation.GetTypeByMetadataName("Microsoft.Toolkit.Mvvm.Input.AsyncRelayCommand")!;
                        delegateTypeSymbol = context.Compilation.GetTypeByMetadataName("System.Func`2")!.Construct(
                            context.Compilation.GetTypeByMetadataName("System.Threading.CancellationToken")!,
                            context.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task")!);

                        return true;
                    }

                    // Map <T, Task> to IAsyncRelayCommand<T>, AsyncRelayCommand<T>, Func<T, Task>
                    commandInterfaceTypeSymbol = context.Compilation.GetTypeByMetadataName("Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand`1")!.Construct(singleParameter.Type);
                    commandClassTypeSymbol = context.Compilation.GetTypeByMetadataName("Microsoft.Toolkit.Mvvm.Input.AsyncRelayCommand`1")!.Construct(singleParameter.Type);
                    delegateTypeSymbol = context.Compilation.GetTypeByMetadataName("System.Func`2")!.Construct(
                        singleParameter.Type,
                        context.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task")!);

                    return true;
                }

                // Map <T, CancellationToken, Task> to IAsyncRelayCommand<T>, AsyncRelayCommand<T>, Func<T, CancellationToken, Task>
                if (methodSymbol.Parameters.Length == 2 &&
                    methodSymbol.Parameters[0] is IParameterSymbol { RefKind: RefKind.None, Type: { IsRefLikeType: false, TypeKind: not TypeKind.Pointer and not TypeKind.FunctionPointer } } firstParameter &&
                    methodSymbol.Parameters[1] is IParameterSymbol { RefKind: RefKind.None, Type: { IsRefLikeType: false, TypeKind: not TypeKind.Pointer and not TypeKind.FunctionPointer } } secondParameter &&
                    SymbolEqualityComparer.Default.Equals(secondParameter.Type, context.Compilation.GetTypeByMetadataName("System.Threading.CancellationToken")!))
                {
                    commandInterfaceTypeSymbol = context.Compilation.GetTypeByMetadataName("Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand`1")!.Construct(firstParameter.Type);
                    commandClassTypeSymbol = context.Compilation.GetTypeByMetadataName("Microsoft.Toolkit.Mvvm.Input.AsyncRelayCommand`1")!.Construct(firstParameter.Type);
                    delegateTypeSymbol = context.Compilation.GetTypeByMetadataName("System.Func`3")!.Construct(
                        firstParameter.Type,
                        secondParameter.Type,
                        context.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task")!);

                    return true;
                }
            }

            commandInterfaceTypeSymbol = null;
            commandClassTypeSymbol = null;
            delegateTypeSymbol = null;

            return false;
        }
    }
}
