// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
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
using static Microsoft.Toolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace Microsoft.Toolkit.Mvvm.SourceGenerators
{
    /// <summary>
    /// A source generator for the <see cref="ObservablePropertyAttribute"/> type.
    /// </summary>
    [Generator]
    public sealed partial class ObservablePropertyGenerator : ISourceGenerator
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

            foreach (var items in syntaxReceiver.GatheredInfo.GroupBy<SyntaxReceiver.Item, INamedTypeSymbol>(static item => item.FieldSymbol.ContainingType, SymbolEqualityComparer.Default))
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
                        context.ReportDiagnostic(ObservablePropertyGeneratorError, items.Key, items.Key);
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
        /// <param name="items">The sequence of fields to process.</param>
        private static void OnExecute(
            GeneratorExecutionContext context,
            ClassDeclarationSyntax classDeclaration,
            INamedTypeSymbol classDeclarationSymbol,
            IEnumerable<SyntaxReceiver.Item> items)
        {
            INamedTypeSymbol
                iNotifyPropertyChangingSymbol = context.Compilation.GetTypeByMetadataName(typeof(INotifyPropertyChanging).FullName)!,
                observableObjectSymbol = context.Compilation.GetTypeByMetadataName("Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject")!,
                observableObjectAttributeSymbol = context.Compilation.GetTypeByMetadataName(typeof(ObservableObjectAttribute).FullName)!,
                observableValidatorSymbol = context.Compilation.GetTypeByMetadataName("Microsoft.Toolkit.Mvvm.ComponentModel.ObservableValidator")!;

            // Check whether the current type implements INotifyPropertyChanging and whether it inherits from ObservableValidator
            bool
                isObservableValidator = classDeclarationSymbol.InheritsFrom(observableValidatorSymbol),
                isNotifyPropertyChanging =
                    classDeclarationSymbol.AllInterfaces.Contains(iNotifyPropertyChangingSymbol, SymbolEqualityComparer.Default) ||
                    classDeclarationSymbol.InheritsFrom(observableObjectSymbol) ||
                    classDeclarationSymbol.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, observableObjectAttributeSymbol));

            // Create the class declaration for the user type. This will produce a tree as follows:
            //
            // <MODIFIERS> <CLASS_NAME>
            // {
            //     <MEMBERS>
            // }
            var classDeclarationSyntax =
                ClassDeclaration(classDeclarationSymbol.Name)
                .WithModifiers(classDeclaration.Modifiers)
                .AddMembers(items.Select(item => CreatePropertyDeclaration(context, item.LeadingTrivia, item.FieldSymbol, isNotifyPropertyChanging, isObservableValidator)).ToArray());

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
            context.AddSource($"[{typeof(ObservablePropertyAttribute).Name}]_[{classDeclarationSymbol.GetFullMetadataNameForFileName()}].cs", SourceText.From(source, Encoding.UTF8));
        }

        /// <summary>
        /// Creates a <see cref="PropertyDeclarationSyntax"/> instance for a specified field.
        /// </summary>
        /// <param name="context">The input <see cref="GeneratorExecutionContext"/> instance to use.</param>
        /// <param name="leadingTrivia">The leading trivia for the field to process.</param>
        /// <param name="fieldSymbol">The input <see cref="IFieldSymbol"/> instance to process.</param>
        /// <param name="isNotifyPropertyChanging">Indicates whether or not <see cref="INotifyPropertyChanging"/> is also implemented.</param>
        /// <param name="isObservableValidator">Indicates whether or not the containing type inherits from <c>ObservableValidator</c>.</param>
        /// <returns>A generated <see cref="PropertyDeclarationSyntax"/> instance for the input field.</returns>
        [Pure]
        private static PropertyDeclarationSyntax CreatePropertyDeclaration(
            GeneratorExecutionContext context,
            SyntaxTriviaList leadingTrivia,
            IFieldSymbol fieldSymbol,
            bool isNotifyPropertyChanging,
            bool isObservableValidator)
        {
            // Get the field type and the target property name
            string
                typeName = fieldSymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                propertyName = GetGeneratedPropertyName(fieldSymbol);

            INamedTypeSymbol alsoNotifyForAttributeSymbol = context.Compilation.GetTypeByMetadataName(typeof(AlsoNotifyForAttribute).FullName)!;
            INamedTypeSymbol? validationAttributeSymbol = context.Compilation.GetTypeByMetadataName("System.ComponentModel.DataAnnotations.ValidationAttribute");

            List<StatementSyntax> dependentPropertyNotificationStatements = new();
            List<AttributeSyntax> validationAttributes = new();

            foreach (AttributeData attributeData in fieldSymbol.GetAttributes())
            {
                // Add dependent property notifications, if needed
                if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, alsoNotifyForAttributeSymbol))
                {
                    foreach (TypedConstant attributeArgument in attributeData.ConstructorArguments)
                    {
                        if (attributeArgument.IsNull)
                        {
                            continue;
                        }

                        if (attributeArgument.Kind == TypedConstantKind.Primitive &&
                            attributeArgument.Value is string dependentPropertyName)
                        {
                            // OnPropertyChanged("OtherPropertyName");
                            dependentPropertyNotificationStatements.Add(ExpressionStatement(
                                InvocationExpression(IdentifierName("OnPropertyChanged"))
                                .AddArgumentListArguments(Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(dependentPropertyName))))));
                        }
                        else if (attributeArgument.Kind == TypedConstantKind.Array)
                        {
                            foreach (TypedConstant nestedAttributeArgument in attributeArgument.Values)
                            {
                                if (nestedAttributeArgument.IsNull)
                                {
                                    continue;
                                }

                                // Additional property names
                                dependentPropertyNotificationStatements.Add(ExpressionStatement(
                                    InvocationExpression(IdentifierName("OnPropertyChanged"))
                                    .AddArgumentListArguments(Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal((string)nestedAttributeArgument.Value!))))));
                            }
                        }
                    }
                }
                else if (validationAttributeSymbol is not null &&
                         attributeData.AttributeClass?.InheritsFrom(validationAttributeSymbol) == true)
                {
                    // Track the current validation attribute
                    validationAttributes.Add(attributeData.AsAttributeSyntax());
                }
            }

            BlockSyntax setterBlock;

            if (validationAttributes.Count > 0)
            {
                // Emit a diagnostic if the current type doesn't inherit from ObservableValidator
                if (!isObservableValidator)
                {
                    context.ReportDiagnostic(
                        MissingObservableValidatorInheritanceError,
                        fieldSymbol,
                        fieldSymbol.ContainingType,
                        fieldSymbol.Name,
                        validationAttributes.Count);

                    setterBlock = Block();
                }

                // Generate the inner setter block as follows:
                //
                // SetProperty(ref <FIELD_NAME>, value, true);
                //
                // Or in case there is at least one dependent property:
                //
                // if (SetProperty(ref <FIELD_NAME>, value, true))
                // {
                //     OnPropertyChanged("Property1"); // Optional
                //     OnPropertyChanged("Property2");
                //     ...
                //     OnPropertyChanged("PropertyN");
                // }
                InvocationExpressionSyntax setPropertyExpression =
                    InvocationExpression(IdentifierName("SetProperty"))
                    .AddArgumentListArguments(
                        Argument(IdentifierName(fieldSymbol.Name)).WithRefOrOutKeyword(Token(SyntaxKind.RefKeyword)),
                        Argument(IdentifierName("value")),
                        Argument(LiteralExpression(SyntaxKind.TrueLiteralExpression)));

                setterBlock = dependentPropertyNotificationStatements.Count switch
                {
                    0 => Block(ExpressionStatement(setPropertyExpression)),
                    _ => Block(IfStatement(setPropertyExpression, Block(dependentPropertyNotificationStatements)))
                };
            }
            else
            {
                BlockSyntax updateAndNotificationBlock = Block();

                // Add the OnPropertyChanging() call if necessary
                if (isNotifyPropertyChanging)
                {
                    updateAndNotificationBlock = updateAndNotificationBlock.AddStatements(ExpressionStatement(InvocationExpression(IdentifierName("OnPropertyChanging"))));
                }

                // Add the following statements:
                //
                // <FIELD_NAME> = value;
                // OnPropertyChanged();
                updateAndNotificationBlock = updateAndNotificationBlock.AddStatements(
                    ExpressionStatement(
                        AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            IdentifierName(fieldSymbol.Name),
                            IdentifierName("value"))),
                    ExpressionStatement(InvocationExpression(IdentifierName("OnPropertyChanged"))));

                // Add the dependent property notifications at the end
                updateAndNotificationBlock = updateAndNotificationBlock.AddStatements(dependentPropertyNotificationStatements.ToArray());

                // Generate the inner setter block as follows:
                //
                // if (!global::System.Collections.Generic.EqualityComparer<<FIELD_TYPE>>.Default.Equals(<FIELD_NAME>, value))
                // {
                //     OnPropertyChanging(); // Optional
                //     <FIELD_NAME> = value;
                //     OnPropertyChanged();
                //     OnPropertyChanged("Property1"); // Optional
                //     OnPropertyChanged("Property2");
                //     ...
                //     OnPropertyChanged("PropertyN");
                // }
                setterBlock = Block(
                    IfStatement(
                        PrefixUnaryExpression(
                            SyntaxKind.LogicalNotExpression,
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        GenericName(Identifier("global::System.Collections.Generic.EqualityComparer"))
                                        .AddTypeArgumentListArguments(IdentifierName(typeName)),
                                        IdentifierName("Default")),
                                    IdentifierName("Equals")))
                            .AddArgumentListArguments(
                                    Argument(IdentifierName(fieldSymbol.Name)),
                                    Argument(IdentifierName("value")))),
                        updateAndNotificationBlock));
            }

            // Construct the generated property as follows:
            //
            // <FIELD_TRIVIA>
            // [global::System.CodeDom.Compiler.GeneratedCode("...", "...")]
            // [global::System.Diagnostics.DebuggerNonUserCode]
            // [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            // <VALIDATION_ATTRIBUTE1> // Optional
            // <VALIDATION_ATTRIBUTE2>
            // ...
            // <VALIDATION_ATTRIBUTEN>
            // public <FIELD_TYPE> <PROPERTY_NAME>
            // {
            //     get => <FIELD_NAME>;
            //     set
            //     {
            //         <BODY>
            //     }
            // }
            return
                PropertyDeclaration(IdentifierName(typeName), Identifier(propertyName))
                .AddAttributeLists(
                    AttributeList(SingletonSeparatedList(
                        Attribute(IdentifierName($"global::System.CodeDom.Compiler.GeneratedCode"))
                        .AddArgumentListArguments(
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ObservablePropertyGenerator).FullName))),
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ObservablePropertyGenerator).Assembly.GetName().Version.ToString())))))),
                    AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.DebuggerNonUserCode")))),
                    AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage")))))
                .AddAttributeLists(validationAttributes.Select(static a => AttributeList(SingletonSeparatedList(a))).ToArray())
                .WithLeadingTrivia(leadingTrivia)
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithExpressionBody(ArrowExpressionClause(IdentifierName(fieldSymbol.Name)))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                    AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithBody(setterBlock));
        }

        /// <summary>
        /// Get the generated property name for an input field.
        /// </summary>
        /// <param name="fieldSymbol">The input <see cref="IFieldSymbol"/> instance to process.</param>
        /// <returns>The generated property name for <paramref name="fieldSymbol"/>.</returns>
        [Pure]
        private static string GetGeneratedPropertyName(IFieldSymbol fieldSymbol)
        {
            string propertyName = fieldSymbol.Name;

            if (propertyName.StartsWith("m_"))
            {
                propertyName = propertyName.Substring(2);
            }
            else if (propertyName.StartsWith("_"))
            {
                propertyName = propertyName.TrimStart('_');
            }

            return $"{char.ToUpper(propertyName[0])}{propertyName.Substring(1)}";
        }
    }
}
