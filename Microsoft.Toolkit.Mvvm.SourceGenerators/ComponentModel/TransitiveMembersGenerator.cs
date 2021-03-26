// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.SourceGenerators.Diagnostics;
using Microsoft.Toolkit.Mvvm.SourceGenerators.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Microsoft.CodeAnalysis.SymbolDisplayTypeQualificationStyle;

namespace Microsoft.Toolkit.Mvvm.SourceGenerators
{
    /// <summary>
    /// A source generator for the <see cref="ObservableObjectAttribute"/> type.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the source attribute to look for.</typeparam>
    public abstract partial class TransitiveMembersGenerator<TAttribute> : ISourceGenerator
        where TAttribute : Attribute
    {
        /// <summary>
        /// Gets a <see cref="DiagnosticDescriptor"/> indicating when the generation failed for a given type.
        /// </summary>
        protected abstract DiagnosticDescriptor TargetTypeErrorDescriptor { get; }

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

            // Load the syntax tree with the members to generate
            SyntaxTree sourceSyntaxTree = LoadSourceSyntaxTree();

            foreach (SyntaxReceiver.Item item in syntaxReceiver.GatheredInfo)
            {
                if (!ValidateTargetType(context, item.AttributeData, item.ClassDeclaration, item.ClassSymbol, out var descriptor))
                {
                    context.ReportDiagnostic(descriptor, item.AttributeSyntax, item.ClassSymbol);

                    continue;
                }

                try
                {
                    OnExecute(context, item.AttributeData, item.ClassDeclaration, item.ClassSymbol, sourceSyntaxTree);
                }
                catch
                {
                    context.ReportDiagnostic(TargetTypeErrorDescriptor, item.AttributeSyntax, item.ClassSymbol);
                }
            }
        }

        /// <summary>
        /// Loads the source syntax tree for the current generator.
        /// </summary>
        /// <returns>The syntax tree with the elements to emit in the generated code.</returns>
        [Pure]
        private static SyntaxTree LoadSourceSyntaxTree()
        {
            string filename = $"Microsoft.Toolkit.Mvvm.SourceGenerators.EmbeddedResources.{typeof(TAttribute).Name.Replace("Attribute", string.Empty)}.cs";

            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filename);
            StreamReader reader = new(stream);

            string observableObjectSource = reader.ReadToEnd();

            return CSharpSyntaxTree.ParseText(observableObjectSource);
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
                .AddMembers(FilterDeclaredMembers(context, attributeData, classDeclaration, classDeclarationSymbol, sourceDeclaration).ToArray());

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
                .AddUsings(usingDirectives.First().WithLeadingTrivia(TriviaList(
                    Comment("// Licensed to the .NET Foundation under one or more agreements."),
                    Comment("// The .NET Foundation licenses this file to you under the MIT license."),
                    Comment("// See the LICENSE file in the project root for more information."),
                    Trivia(PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true)))))
                .AddUsings(usingDirectives.Skip(1).ToArray())
                .NormalizeWhitespace()
                .ToFullString();

            // Add the partial type
            context.AddSource($"[{typeof(TAttribute).Name}]_[{classDeclarationSymbol.GetFullMetadataNameForFileName()}].cs", SourceText.From(source, Encoding.UTF8));
        }

        /// <summary>
        /// Validates a target type being processed.
        /// </summary>
        /// <param name="context">The input <see cref="GeneratorExecutionContext"/> instance to use.</param>
        /// <param name="attributeData">The <see cref="AttributeData"/> for the current attribute being processed.</param>
        /// <param name="classDeclaration">The <see cref="ClassDeclarationSyntax"/> node to process.</param>
        /// <param name="classDeclarationSymbol">The <see cref="INamedTypeSymbol"/> for <paramref name="classDeclaration"/>.</param>
        /// <param name="descriptor">The resulting <see cref="DiagnosticDescriptor"/> to emit in case the target type isn't valid.</param>
        /// <returns>Whether or not the target type is valid and can be processed normally.</returns>
        protected abstract bool ValidateTargetType(
            GeneratorExecutionContext context,
            AttributeData attributeData,
            ClassDeclarationSyntax classDeclaration,
            INamedTypeSymbol classDeclarationSymbol,
            [NotNullWhen(false)] out DiagnosticDescriptor? descriptor);

        /// <summary>
        /// Filters the <see cref="MemberDeclarationSyntax"/> nodes to generate from the input parsed tree.
        /// </summary>
        /// <param name="context">The input <see cref="GeneratorExecutionContext"/> instance to use.</param>
        /// <param name="attributeData">The <see cref="AttributeData"/> for the current attribute being processed.</param>
        /// <param name="classDeclaration">The <see cref="ClassDeclarationSyntax"/> node to process.</param>
        /// <param name="classDeclarationSymbol">The <see cref="INamedTypeSymbol"/> for <paramref name="classDeclaration"/>.</param>
        /// <param name="sourceDeclaration">The parsed <see cref="ClassDeclarationSyntax"/> instance with the source nodes.</param>
        /// <returns>A sequence of <see cref="MemberDeclarationSyntax"/> nodes to emit in the generated file.</returns>
        protected virtual IEnumerable<MemberDeclarationSyntax> FilterDeclaredMembers(
            GeneratorExecutionContext context,
            AttributeData attributeData,
            ClassDeclarationSyntax classDeclaration,
            INamedTypeSymbol classDeclarationSymbol,
            ClassDeclarationSyntax sourceDeclaration)
        {
            return sourceDeclaration.Members;
        }
    }
}
