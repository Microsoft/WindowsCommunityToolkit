// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
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
    /// A source generator for message registration without relying on compiled LINQ expressions.
    /// </summary>
    [Generator]
    public sealed partial class IMessengerRegisterAllGenerator : ISourceGenerator
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

            // Get the symbol for the IRecipient<T> interface type
            INamedTypeSymbol iRecipientSymbol = context.Compilation.GetTypeByMetadataName("Microsoft.Toolkit.Mvvm.Messaging.IRecipient`1")!;

            // Prepare the attributes to add to the first class declaration
            AttributeListSyntax[] classAttributes = new[]
            {
                AttributeList(SingletonSeparatedList(Attribute(IdentifierName("DebuggerNonUserCode")))),
                AttributeList(SingletonSeparatedList(Attribute(IdentifierName("ExcludeFromCodeCoverage")))),
                AttributeList(SingletonSeparatedList(
                    Attribute(IdentifierName("EditorBrowsable")).AddArgumentListArguments(
                    AttributeArgument(ParseExpression("EditorBrowsableState.Never"))))),
                AttributeList(SingletonSeparatedList(
                    Attribute(IdentifierName("Obsolete")).AddArgumentListArguments(
                    AttributeArgument(LiteralExpression(
                        SyntaxKind.StringLiteralExpression,
                        Literal("This type is not intended to be used directly by user code"))))))
            };

            foreach (INamedTypeSymbol classSymbol in syntaxReceiver.GatheredInfo)
            {
                // Create a static method to register all messages for a given recipient type.
                // There are two versions that are generated: a non-generic one doing the registration
                // with no tokens, which is the most common scenario and will help particularly in AOT
                // scenarios, and a generic version that will support all other cases with custom tokens.
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
                // using System.Diagnostics;
                // using System.Diagnostics.CodeAnalysis;
                //
                // namespace Microsoft.Toolkit.Mvvm.Messaging.__Internals
                // {
                //     [DebuggerNonUserCode]
                //     [ExcludeFromCodeCoverage]
                //     [EditorBrowsable(EditorBrowsableState.Never)]
                //     [Obsolete("This type is not intended to be used directly by user code")]
                //     internal static partial class __IMessengerExtensions
                //     {
                //         [EditorBrowsable(EditorBrowsableState.Never)]
                //         [Obsolete("This method is not intended to be called directly by user code")]
                //         public static void RegisterAll(IMessenger messenger, <RECIPIENT_TYPE> recipient)
                //         {
                //             <BODY>
                //         }
                //
                //         [EditorBrowsable(EditorBrowsableState.Never)]
                //         [Obsolete("This method is not intended to be called directly by user code")]
                //         public static void RegisterAll<TToken>(IMessenger messenger, <RECIPIENT_TYPE> recipient, TToken token)
                //             where TToken : IEquatable<TToken>
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
                    UsingDirective(IdentifierName("System.ComponentModel")),
                    UsingDirective(IdentifierName("System.Diagnostics")),
                    UsingDirective(IdentifierName("System.Diagnostics.CodeAnalysis"))).AddMembers(
                    NamespaceDeclaration(IdentifierName("Microsoft.Toolkit.Mvvm.Messaging.__Internals")).AddMembers(
                    ClassDeclaration("__IMessengerExtensions").AddModifiers(
                        Token(SyntaxKind.InternalKeyword),
                        Token(SyntaxKind.StaticKeyword),
                        Token(SyntaxKind.PartialKeyword)).AddAttributeLists(classAttributes).AddMembers(
                    MethodDeclaration(
                        PredefinedType(Token(SyntaxKind.VoidKeyword)),
                        Identifier("RegisterAll")).AddAttributeLists(
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
                            Parameter(Identifier("messenger")).WithType(IdentifierName("IMessenger")),
                            Parameter(Identifier("recipient")).WithType(IdentifierName(classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))))
                        .WithBody(Block(EnumerateRegistrationStatements(classSymbol, iRecipientSymbol).ToArray())),
                    MethodDeclaration(
                        PredefinedType(Token(SyntaxKind.VoidKeyword)),
                        Identifier("RegisterAll")).AddAttributeLists(
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
                            Parameter(Identifier("messenger")).WithType(IdentifierName("IMessenger")),
                            Parameter(Identifier("recipient")).WithType(IdentifierName(classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))),
                            Parameter(Identifier("token")).WithType(IdentifierName("TToken")))
                        .AddTypeParameterListParameters(TypeParameter("TToken"))
                        .AddConstraintClauses(
                            TypeParameterConstraintClause("TToken")
                            .AddConstraints(TypeConstraint(GenericName("IEquatable").AddTypeArgumentListArguments(IdentifierName("TToken")))))
                        .WithBody(Block(EnumerateRegistrationStatementsWithTokens(classSymbol, iRecipientSymbol).ToArray())))))
                    .NormalizeWhitespace()
                    .ToFullString();

                // Reset the attributes list (so the same class doesn't get duplicate attributes)
                classAttributes = Array.Empty<AttributeListSyntax>();

                // Add the partial type
                context.AddSource($"[IRecipient{{T}}]_[{classSymbol.GetFullMetadataNameForFileName()}].cs", SourceText.From(source, Encoding.UTF8));
            }
        }

        /// <summary>
        /// Gets a sequence of statements to register declared message handlers.
        /// </summary>
        /// <param name="classSymbol">The input <see cref="INamedTypeSymbol"/> instance to process.</param>
        /// <param name="iRecipientSymbol">The type symbol for the <c>IRecipient&lt;T&gt;</c> interface.</param>
        /// <returns>The sequence of <see cref="StatementSyntax"/> instances to register message handleers.</returns>
        [Pure]
        private static IEnumerable<StatementSyntax> EnumerateRegistrationStatements(INamedTypeSymbol classSymbol, INamedTypeSymbol iRecipientSymbol)
        {
            foreach (var interfaceSymbol in classSymbol.AllInterfaces)
            {
                if (!SymbolEqualityComparer.Default.Equals(interfaceSymbol.OriginalDefinition, iRecipientSymbol))
                {
                    continue;
                }

                // This enumerator produces a sequence of statements as follows:
                //
                // messenger.Register<<TYPE_0>>(recipient);
                // messenger.Register<<TYPE_1>>(recipient);
                // // ...
                // messenger.Register<<TYPE_N>>(recipient);
                yield return
                    ExpressionStatement(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("messenger"),
                                GenericName(Identifier("Register")).AddTypeArgumentListArguments(
                                    IdentifierName(interfaceSymbol.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)))))
                        .AddArgumentListArguments(Argument(IdentifierName("recipient"))));
            }
        }

        /// <summary>
        /// Gets a sequence of statements to register declared message handlers with custom tokens.
        /// </summary>
        /// <param name="classSymbol">The input <see cref="INamedTypeSymbol"/> instance to process.</param>
        /// <param name="iRecipientSymbol">The type symbol for the <c>IRecipient&lt;T&gt;</c> interface.</param>
        /// <returns>The sequence of <see cref="StatementSyntax"/> instances to register message handleers.</returns>
        [Pure]
        private static IEnumerable<StatementSyntax> EnumerateRegistrationStatementsWithTokens(INamedTypeSymbol classSymbol, INamedTypeSymbol iRecipientSymbol)
        {
            foreach (var interfaceSymbol in classSymbol.AllInterfaces)
            {
                if (!SymbolEqualityComparer.Default.Equals(interfaceSymbol.OriginalDefinition, iRecipientSymbol))
                {
                    continue;
                }

                // This enumerator produces a sequence of statements as follows:
                //
                // messenger.Register<<TYPE_0>, TToken>(recipient, token);
                // messenger.Register<<TYPE_1>, TToken>(recipient, token);
                // // ...
                // messenger.Register<<TYPE_N>, TToken>(recipient, token);
                yield return
                    ExpressionStatement(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("messenger"),
                                GenericName(Identifier("Register")).AddTypeArgumentListArguments(
                                    IdentifierName(interfaceSymbol.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)),
                                    IdentifierName("TToken"))))
                        .AddArgumentListArguments(Argument(IdentifierName("recipient")), Argument(IdentifierName("token"))));
            }
        }
    }
}
