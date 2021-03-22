using System;
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
    /// <typeparam name="TAttribute">The type of the source attribute to look for.</typeparam>
    public abstract class TransitiveMembersGenerator<TAttribute> : ISourceGenerator
        where TAttribute : Attribute
    {
        /// <inheritdoc/>
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        /// <inheritdoc/>
        public void Execute(GeneratorExecutionContext context)
        {
            // Find all the target attribute usages
            IEnumerable<AttributeSyntax> attributes =
                from syntaxTree in context.Compilation.SyntaxTrees
                let semanticModel = context.Compilation.GetSemanticModel(syntaxTree)
                from attribute in syntaxTree.GetRoot().DescendantNodes().OfType<AttributeSyntax>()
                let typeInfo = semanticModel.GetTypeInfo(attribute)
                where typeInfo.Type?.Name == typeof(TAttribute).Name
                select attribute;

            SyntaxTree? sourceSyntaxTree = null;

            foreach (AttributeSyntax attribute in attributes)
            {
                // Load the source syntax tree if needed
                if (sourceSyntaxTree is null)
                {
                    string filename = $"Microsoft.Toolkit.Mvvm.SourceGenerators.EmbeddedResources.{typeof(TAttribute).Name.Replace("Attribute", string.Empty)}.cs";

                    Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filename);
                    StreamReader reader = new(stream);

                    string observableObjectSource = reader.ReadToEnd();

                    sourceSyntaxTree = CSharpSyntaxTree.ParseText(observableObjectSource);
                }

                ClassDeclarationSyntax classDeclaration = attribute.FirstAncestorOrSelf<ClassDeclarationSyntax>()!;
                SemanticModel semanticModel = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);
                INamedTypeSymbol classDeclarationSymbol = semanticModel.GetDeclaredSymbol(classDeclaration)!;
                AttributeData attributeData = classDeclarationSymbol.GetAttributes().First(a => a.ApplicationSyntaxReference?.GetSyntax() == attribute);

                OnExecute(context, attributeData, classDeclaration, classDeclarationSymbol, sourceSyntaxTree);
            }
        }

        /// <summary>
        /// Processes a given target type.
        /// </summary>
        /// <param name="context">The input <see cref="GeneratorExecutionContext"/> instance to use.</param>
        /// <param name="attributeData">The <see cref="AttributeData"/> for the current attribute being processed.</param>
        /// <param name="classDeclaration">The <see cref="ClassDeclarationSyntax"/> node to process.</param>
        /// <param name="classDeclarationSymbol">The <see cref="INamedTypeSymbol"/> for <paramref name="classDeclaration"/>.</param>
        /// <param name="sourceSyntaxTree">The <see cref="CodeAnalysis.SyntaxTree"/> for the target parsed source.</param>
        private void OnExecute(
            GeneratorExecutionContext context,
            AttributeData attributeData,
            ClassDeclarationSyntax classDeclaration,
            INamedTypeSymbol classDeclarationSymbol,
            SyntaxTree sourceSyntaxTree)
        {
            ClassDeclarationSyntax sourceDeclaration = sourceSyntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().First();
            UsingDirectiveSyntax[] usingDirectives = sourceSyntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>().ToArray();
            BaseListSyntax? baseListSyntax = BaseList(SeparatedList(
                sourceDeclaration.BaseList?.Types
                .OfType<SimpleBaseTypeSyntax>()
                .Select(static t => t.Type)
                .OfType<IdentifierNameSyntax>()
                .Where(static t => t.Identifier.ValueText.StartsWith("I"))
                .Select(static t => SimpleBaseType(t))
                .ToArray()
                ?? Array.Empty<BaseTypeSyntax>()));

            if (baseListSyntax.Types.Count == 0)
            {
                baseListSyntax = null;
            }

            // Create the class declaration for the user type. This will produce a tree as follows:
            //
            // <MODIFIERS> <CLASS_NAME> : <BASE_TYPES>
            // {
            //     <MEMBERS>
            // }
            var classDeclarationSyntax =
                ClassDeclaration(classDeclaration.Identifier.Text)
                .WithModifiers(classDeclaration.Modifiers)
                .WithBaseList(baseListSyntax)
                .AddMembers(FilterDeclaredMembers(attributeData, classDeclaration, classDeclarationSymbol, sourceDeclaration).ToArray());

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
            context.AddSource($"[{typeof(TAttribute).Name}]_[{classDeclaration.Identifier.Text}].cs", SourceText.From(source, Encoding.UTF8));
        }

        /// <summary>
        /// Filters the <see cref="MemberDeclarationSyntax"/> nodes to generate from the input parsed tree.
        /// </summary>
        /// <param name="attributeData">The <see cref="AttributeData"/> for the current attribute being processed.</param>
        /// <param name="classDeclaration">The <see cref="ClassDeclarationSyntax"/> node to process.</param>
        /// <param name="classDeclarationSymbol">The <see cref="INamedTypeSymbol"/> for <paramref name="classDeclaration"/>.</param>
        /// <param name="sourceDeclaration">The parsed <see cref="ClassDeclarationSyntax"/> instance with the source nodes.</param>
        /// <returns>A sequence of <see cref="MemberDeclarationSyntax"/> nodes to emit in the generated file.</returns>
        protected virtual IEnumerable<MemberDeclarationSyntax> FilterDeclaredMembers(
            AttributeData attributeData,
            ClassDeclarationSyntax classDeclaration,
            INamedTypeSymbol classDeclarationSymbol,
            ClassDeclarationSyntax sourceDeclaration)
        {
            return sourceDeclaration.Members;
        }
    }
}
