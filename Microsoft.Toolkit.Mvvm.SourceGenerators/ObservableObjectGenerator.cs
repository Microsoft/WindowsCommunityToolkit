using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Microsoft.CodeAnalysis.SymbolDisplayTypeQualificationStyle;

namespace Microsoft.Toolkit.Mvvm.SourceGenerators
{
    /// <summary>
    /// A source generator for the <see cref="ObservableObjectAttribute"/> type.
    /// </summary>
    [Generator]
    public class ObservableObjectGenerator : ISourceGenerator
    {
        /// <inheritdoc/>
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        /// <inheritdoc/>
        public void Execute(GeneratorExecutionContext context)
        {
            // Find all the [ObservableObject] usages
            IEnumerable<AttributeSyntax> attributes =
                from syntaxTree in context.Compilation.SyntaxTrees
                let semanticModel = context.Compilation.GetSemanticModel(syntaxTree)
                from attribute in syntaxTree.GetRoot().DescendantNodes().OfType<AttributeSyntax>()
                let typeInfo = semanticModel.GetTypeInfo(attribute)
                where typeInfo.Type is { Name: nameof(ObservableObjectAttribute) }
                select attribute;

            SyntaxTree? observableObjectSyntaxTree = null;

            foreach (AttributeSyntax attribute in attributes)
            {
                // Load the ObservableObject syntax tree if needed
                if (observableObjectSyntaxTree is null)
                {
                    string filename = "Microsoft.Toolkit.Mvvm.SourceGenerators.Resources.ObservableObject.cs";

                    Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filename);
                    StreamReader reader = new(stream);

                    string observableObjectSource = reader.ReadToEnd();

                    observableObjectSyntaxTree = CSharpSyntaxTree.ParseText(observableObjectSource);
                }

                ClassDeclarationSyntax classDeclaration = attribute.FirstAncestorOrSelf<ClassDeclarationSyntax>()!;
                SemanticModel semanticModel = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);
                INamedTypeSymbol classDeclarationSymbol = semanticModel.GetDeclaredSymbol(classDeclaration)!;

                OnExecute(context, classDeclaration, classDeclarationSymbol, observableObjectSyntaxTree);
            }
        }

        /// <summary>
        /// Processes a given target type.
        /// </summary>
        /// <param name="context">The input <see cref="GeneratorExecutionContext"/> instance to use.</param>
        /// <param name="classDeclaration">The <see cref="ClassDeclarationSyntax"/> node to process.</param>
        /// <param name="classDeclarationSymbol">The <see cref="INamedTypeSymbol"/> for <paramref name="classDeclaration"/>.</param>
        /// <param name="observableObjectSyntaxTree">The <see cref="CodeAnalysis.SyntaxTree"/> for the parsed <see cref="ObservableObject"/> source.</param>
        private static void OnExecute(
            GeneratorExecutionContext context,
            ClassDeclarationSyntax classDeclaration,
            INamedTypeSymbol classDeclarationSymbol,
            SyntaxTree observableObjectSyntaxTree)
        {
            ClassDeclarationSyntax observableObjectDeclaration = observableObjectSyntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().First();
            UsingDirectiveSyntax[] usingDirectives = observableObjectSyntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>().ToArray();

            // Create the class declaration for the user type. This will produce a tree as follows:
            //
            // <MODIFIERS> <CLASS_NAME> : INotifyPropertyChanged, INotifyPropertyChanging
            // {
            //     <ObservableObject_MEMBERS>
            // }
            var classDeclarationSyntax =
                ClassDeclaration(classDeclaration.Identifier.Text)
                .WithModifiers(classDeclaration.Modifiers)
                .WithBaseList(observableObjectDeclaration.BaseList)
                .WithMembers(observableObjectDeclaration.Members);

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

            var source =
                CompilationUnit()
                .AddMembers(NamespaceDeclaration(IdentifierName(namespaceName))
                .AddMembers(typeDeclarationSyntax))
                .NormalizeWhitespace()
                .AddUsings(usingDirectives.First().WithLeadingTrivia(TriviaList(
                    Comment("// Licensed to the .NET Foundation under one or more agreements."),
                    CarriageReturnLineFeed,
                    Comment("// The .NET Foundation licenses this file to you under the MIT license."),
                    CarriageReturnLineFeed,
                    Comment("// See the LICENSE file in the project root for more information."),
                    CarriageReturnLineFeed,
                    CarriageReturnLineFeed,
                    Trivia(PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true)
                    .WithPragmaKeyword(Token(TriviaList(), SyntaxKind.PragmaKeyword, TriviaList(Space)))
                    .WithWarningKeyword(Token(TriviaList(), SyntaxKind.WarningKeyword, TriviaList(Space)))
                    .WithEndOfDirectiveToken(Token(TriviaList(), SyntaxKind.EndOfDirectiveToken, TriviaList(CarriageReturnLineFeed)))),
                    CarriageReturnLineFeed)))
                .AddUsings(usingDirectives.Skip(1).ToArray())
                .ToFullString();

            // Add the partial type
            context.AddSource($"[{nameof(ObservableObjectAttribute)}]_[{classDeclaration.Identifier.Text}].cs", SourceText.From(source, Encoding.UTF8));
        }
    }
}
