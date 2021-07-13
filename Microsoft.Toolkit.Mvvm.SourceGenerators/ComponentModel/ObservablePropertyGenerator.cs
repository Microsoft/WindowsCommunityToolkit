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

            // Sets of discovered property names
            HashSet<string>
                propertyChangedNames = new(),
                propertyChangingNames = new();

            // Process the annotated fields
            foreach (var items in syntaxReceiver.GatheredInfo.GroupBy<SyntaxReceiver.Item, INamedTypeSymbol>(static item => item.FieldSymbol.ContainingType, SymbolEqualityComparer.Default))
            {
                if (items.Key.DeclaringSyntaxReferences.Length > 0 &&
                    items.Key.DeclaringSyntaxReferences.First().GetSyntax() is ClassDeclarationSyntax classDeclaration)
                {
                    try
                    {
                        OnExecuteForProperties(context, classDeclaration, items.Key, items, propertyChangedNames, propertyChangingNames);
                    }
                    catch
                    {
                        context.ReportDiagnostic(ObservablePropertyGeneratorError, items.Key, items.Key);
                    }
                }
            }

            // Process the fields for the cached args
            OnExecuteForPropertyArgs(context, propertyChangedNames, propertyChangingNames);
        }

        /// <summary>
        /// Processes a given target type for declared observable properties.
        /// </summary>
        /// <param name="context">The input <see cref="GeneratorExecutionContext"/> instance to use.</param>
        /// <param name="classDeclaration">The <see cref="ClassDeclarationSyntax"/> node to process.</param>
        /// <param name="classDeclarationSymbol">The <see cref="INamedTypeSymbol"/> for <paramref name="classDeclaration"/>.</param>
        /// <param name="items">The sequence of fields to process.</param>
        /// <param name="propertyChangedNames">The collection of discovered property changed names.</param>
        /// <param name="propertyChangingNames">The collection of discovered property changing names.</param>
        private static void OnExecuteForProperties(
            GeneratorExecutionContext context,
            ClassDeclarationSyntax classDeclaration,
            INamedTypeSymbol classDeclarationSymbol,
            IEnumerable<SyntaxReceiver.Item> items,
            ICollection<string> propertyChangedNames,
            ICollection<string> propertyChangingNames)
        {
            INamedTypeSymbol
                iNotifyPropertyChangingSymbol = context.Compilation.GetTypeByMetadataName(typeof(INotifyPropertyChanging).FullName)!,
                observableObjectSymbol = context.Compilation.GetTypeByMetadataName("Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject")!,
                observableObjectAttributeSymbol = context.Compilation.GetTypeByMetadataName(typeof(ObservableObjectAttribute).FullName)!,
                observableValidatorSymbol = context.Compilation.GetTypeByMetadataName("Microsoft.Toolkit.Mvvm.ComponentModel.ObservableValidator")!;

            // Check whether the current type implements INotifyPropertyChanging and whether it inherits from ObservableValidator
            bool
                isObservableObject = classDeclarationSymbol.InheritsFrom(observableObjectSymbol),
                isObservableValidator = classDeclarationSymbol.InheritsFrom(observableValidatorSymbol),
                isNotifyPropertyChanging =
                    isObservableObject ||
                    classDeclarationSymbol.AllInterfaces.Contains(iNotifyPropertyChangingSymbol, SymbolEqualityComparer.Default) ||
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
                .AddMembers(items.Select(item =>
                    CreatePropertyDeclaration(
                        context,
                        item.LeadingTrivia,
                        item.FieldSymbol,
                        isNotifyPropertyChanging,
                        isObservableValidator,
                        propertyChangedNames,
                        propertyChangingNames)).ToArray());

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
        /// <param name="propertyChangedNames">The collection of discovered property changed names.</param>
        /// <param name="propertyChangingNames">The collection of discovered property changing names.</param>
        /// <returns>A generated <see cref="PropertyDeclarationSyntax"/> instance for the input field.</returns>
        [Pure]
        private static PropertyDeclarationSyntax CreatePropertyDeclaration(
            GeneratorExecutionContext context,
            SyntaxTriviaList leadingTrivia,
            IFieldSymbol fieldSymbol,
            bool isNotifyPropertyChanging,
            bool isObservableValidator,
            ICollection<string> propertyChangedNames,
            ICollection<string> propertyChangingNames)
        {
            // Get the field type and the target property name
            string
                typeName = fieldSymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                propertyName = GetGeneratedPropertyName(fieldSymbol);

            INamedTypeSymbol alsoNotifyChangeForAttributeSymbol = context.Compilation.GetTypeByMetadataName(typeof(AlsoNotifyChangeForAttribute).FullName)!;
            INamedTypeSymbol? validationAttributeSymbol = context.Compilation.GetTypeByMetadataName("System.ComponentModel.DataAnnotations.ValidationAttribute");

            List<StatementSyntax> dependentPropertyNotificationStatements = new();
            List<AttributeSyntax> validationAttributes = new();

            foreach (AttributeData attributeData in fieldSymbol.GetAttributes())
            {
                // Add dependent property notifications, if needed
                if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, alsoNotifyChangeForAttributeSymbol))
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
                            propertyChangedNames.Add(dependentPropertyName);

                            // OnPropertyChanged("OtherPropertyName");
                            dependentPropertyNotificationStatements.Add(ExpressionStatement(
                                InvocationExpression(IdentifierName("OnPropertyChanged"))
                                .AddArgumentListArguments(Argument(MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("global::Microsoft.Toolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedOrChangingArgs"),
                                    IdentifierName($"{dependentPropertyName}{nameof(PropertyChangedEventArgs)}"))))));
                        }
                        else if (attributeArgument.Kind == TypedConstantKind.Array)
                        {
                            foreach (TypedConstant nestedAttributeArgument in attributeArgument.Values)
                            {
                                if (nestedAttributeArgument.IsNull)
                                {
                                    continue;
                                }

                                string currentPropertyName = (string)nestedAttributeArgument.Value!;

                                propertyChangedNames.Add(currentPropertyName);

                                // Additional property names
                                dependentPropertyNotificationStatements.Add(ExpressionStatement(
                                    InvocationExpression(IdentifierName("OnPropertyChanged"))
                                    .AddArgumentListArguments(Argument(MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName("global::Microsoft.Toolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedOrChangingArgs"),
                                        IdentifierName($"{currentPropertyName}{nameof(PropertyChangedEventArgs)}"))))));
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
                else
                {
                    propertyChangedNames.Add(propertyName);
                    propertyChangingNames.Add(propertyName);

                    // Generate the inner setter block as follows:
                    //
                    // if (!global::System.Collections.Generic.EqualityComparer<<FIELD_TYPE>>.Default.Equals(<FIELD_NAME>, value))
                    // {
                    //     OnPropertyChanging(global::Microsoft.Toolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedOrChangingArgs.PropertyNamePropertyChangingEventArgs); // Optional
                    //     <FIELD_NAME> = value;
                    //     OnPropertyChanged(global::Microsoft.Toolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedOrChangingArgs.PropertyNamePropertyChangedEventArgs);
                    //     ValidateProperty(value, <PROPERTY_NAME>);
                    //     OnPropertyChanged(global::Microsoft.Toolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedOrChangingArgs.Property1PropertyChangedEventArgs); // Optional
                    //     OnPropertyChanged(global::Microsoft.Toolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedOrChangingArgs.Property2PropertyChangedEventArgs);
                    //     ...
                    //     OnPropertyChanged(global::Microsoft.Toolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedOrChangingArgs.PropertyNPropertyChangedEventArgs);
                    // }
                    //
                    // The reason why the code is explicitly generated instead of just calling ObservableValidator.SetProperty() is so that we can
                    // take advantage of the cached property changed arguments for the current property as well, not just for the dependent ones.
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
                            Block(
                                ExpressionStatement(
                                    InvocationExpression(IdentifierName("OnPropertyChanging"))
                                    .AddArgumentListArguments(Argument(MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName("global::Microsoft.Toolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedOrChangingArgs"),
                                        IdentifierName($"{propertyName}{nameof(PropertyChangingEventArgs)}"))))),
                                ExpressionStatement(
                                    AssignmentExpression(
                                        SyntaxKind.SimpleAssignmentExpression,
                                        IdentifierName(fieldSymbol.Name),
                                        IdentifierName("value"))),
                                ExpressionStatement(
                                    InvocationExpression(IdentifierName("OnPropertyChanged"))
                                    .AddArgumentListArguments(Argument(MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName("global::Microsoft.Toolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedOrChangingArgs"),
                                        IdentifierName($"{propertyName}{nameof(PropertyChangedEventArgs)}"))))),
                                ExpressionStatement(
                                    InvocationExpression(IdentifierName("ValidateProperty"))
                                    .AddArgumentListArguments(
                                        Argument(IdentifierName("value")),
                                        Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(propertyName))))))
                            .AddStatements(dependentPropertyNotificationStatements.ToArray())));
                }
            }
            else
            {
                BlockSyntax updateAndNotificationBlock = Block();

                // Add OnPropertyChanging(global::Microsoft.Toolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedOrChangingArgs.PropertyNamePropertyChangingEventArgs) if necessary
                if (isNotifyPropertyChanging)
                {
                    propertyChangingNames.Add(propertyName);

                    updateAndNotificationBlock = updateAndNotificationBlock.AddStatements(ExpressionStatement(
                        InvocationExpression(IdentifierName("OnPropertyChanging"))
                        .AddArgumentListArguments(Argument(MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("global::Microsoft.Toolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedOrChangingArgs"),
                            IdentifierName($"{propertyName}{nameof(PropertyChangingEventArgs)}"))))));
                }

                propertyChangedNames.Add(propertyName);

                // Add the following statements:
                //
                // <FIELD_NAME> = value;
                // OnPropertyChanged(global::Microsoft.Toolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedOrChangingArgs.PropertyNamePropertyChangedEventArgs);
                updateAndNotificationBlock = updateAndNotificationBlock.AddStatements(
                    ExpressionStatement(
                        AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            IdentifierName(fieldSymbol.Name),
                            IdentifierName("value"))),
                    ExpressionStatement(
                        InvocationExpression(IdentifierName("OnPropertyChanged"))
                        .AddArgumentListArguments(Argument(MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("global::Microsoft.Toolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedOrChangingArgs"),
                            IdentifierName($"{propertyName}{nameof(PropertyChangedEventArgs)}"))))));

                // Add the dependent property notifications at the end
                updateAndNotificationBlock = updateAndNotificationBlock.AddStatements(dependentPropertyNotificationStatements.ToArray());

                // Generate the inner setter block as follows:
                //
                // if (!global::System.Collections.Generic.EqualityComparer<<FIELD_TYPE>>.Default.Equals(<FIELD_NAME>, value))
                // {
                //     OnPropertyChanging(global::Microsoft.Toolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedOrChangingArgs.PropertyNamePropertyChangingEventArgs); // Optional
                //     <FIELD_NAME> = value;
                //     OnPropertyChanged(global::Microsoft.Toolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedOrChangingArgs.PropertyNamePropertyChangedEventArgs);
                //     OnPropertyChanged(global::Microsoft.Toolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedOrChangingArgs.Property1PropertyChangedEventArgs); // Optional
                //     OnPropertyChanged(global::Microsoft.Toolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedOrChangingArgs.Property2PropertyChangedEventArgs);
                //     ...
                //     OnPropertyChanged(global::Microsoft.Toolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedOrChangingArgs.PropertyNPropertyChangedEventArgs);
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

            // Get the right type for the declared property (including nullability annotations)
            TypeSyntax propertyType = IdentifierName(typeName);

            if (fieldSymbol.Type is { IsReferenceType: true, NullableAnnotation: NullableAnnotation.Annotated })
            {
                propertyType = NullableType(propertyType);
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
            // public <FIELD_TYPE><NULLABLE_ANNOTATION?> <PROPERTY_NAME>
            // {
            //     get => <FIELD_NAME>;
            //     set
            //     {
            //         <BODY>
            //     }
            // }
            return
                PropertyDeclaration(propertyType, Identifier(propertyName))
                .AddAttributeLists(
                    AttributeList(SingletonSeparatedList(
                        Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
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

        /// <summary>
        /// Processes the cached property changed/changing args.
        /// </summary>
        /// <param name="context">The input <see cref="GeneratorExecutionContext"/> instance to use.</param>
        /// <param name="propertyChangedNames">The collection of discovered property changed names.</param>
        /// <param name="propertyChangingNames">The collection of discovered property changing names.</param>
        public void OnExecuteForPropertyArgs(GeneratorExecutionContext context, IReadOnlyCollection<string> propertyChangedNames, IReadOnlyCollection<string> propertyChangingNames)
        {
            if (propertyChangedNames.Count == 0 &&
                propertyChangingNames.Count == 0)
            {
                return;
            }

            INamedTypeSymbol
                propertyChangedEventArgsSymbol = context.Compilation.GetTypeByMetadataName(typeof(PropertyChangedEventArgs).FullName)!,
                propertyChangingEventArgsSymbol = context.Compilation.GetTypeByMetadataName(typeof(PropertyChangingEventArgs).FullName)!;

            // Create a static method to validate all properties in a given class.
            // This code takes a class symbol and produces a compilation unit as follows:
            //
            // // Licensed to the .NET Foundation under one or more agreements.
            // // The .NET Foundation licenses this file to you under the MIT license.
            // // See the LICENSE file in the project root for more information.
            //
            // #pragma warning disable
            //
            // namespace Microsoft.Toolkit.Mvvm.ComponentModel.__Internals
            // {
            //     [global::System.CodeDom.Compiler.GeneratedCode("...", "...")]
            //     [global::System.Diagnostics.DebuggerNonUserCode]
            //     [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            //     [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
            //     [global::System.Obsolete("This type is not intended to be used directly by user code")]
            //     internal static class __KnownINotifyPropertyChangedOrChangingArgs
            //     {
            //         <FIELDS>
            //     }
            // }
            var source =
                CompilationUnit().AddMembers(
                NamespaceDeclaration(IdentifierName("Microsoft.Toolkit.Mvvm.ComponentModel.__Internals")).WithLeadingTrivia(TriviaList(
                    Comment("// Licensed to the .NET Foundation under one or more agreements."),
                    Comment("// The .NET Foundation licenses this file to you under the MIT license."),
                    Comment("// See the LICENSE file in the project root for more information."),
                    Trivia(PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true)))).AddMembers(
                ClassDeclaration("__KnownINotifyPropertyChangedOrChangingArgs").AddModifiers(
                    Token(SyntaxKind.InternalKeyword),
                    Token(SyntaxKind.StaticKeyword)).AddAttributeLists(
                        AttributeList(SingletonSeparatedList(
                            Attribute(IdentifierName($"global::System.CodeDom.Compiler.GeneratedCode"))
                            .AddArgumentListArguments(
                                AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(GetType().FullName))),
                                AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(GetType().Assembly.GetName().Version.ToString())))))),
                        AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.DebuggerNonUserCode")))),
                        AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage")))),
                        AttributeList(SingletonSeparatedList(
                            Attribute(IdentifierName("global::System.ComponentModel.EditorBrowsable")).AddArgumentListArguments(
                            AttributeArgument(ParseExpression("global::System.ComponentModel.EditorBrowsableState.Never"))))),
                        AttributeList(SingletonSeparatedList(
                            Attribute(IdentifierName("global::System.Obsolete")).AddArgumentListArguments(
                            AttributeArgument(LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                Literal("This type is not intended to be used directly by user code")))))))
                    .AddMembers(propertyChangedNames.Select(name => CreateFieldDeclaration(propertyChangedEventArgsSymbol, name)).ToArray())
                    .AddMembers(propertyChangingNames.Select(name => CreateFieldDeclaration(propertyChangingEventArgsSymbol, name)).ToArray())))
                .NormalizeWhitespace()
                .ToFullString();

            // Add the partial type
            context.AddSource($"[{typeof(ObservablePropertyAttribute).Name}]_[__KnownINotifyPropertyChangedOrChangingArgs].cs", SourceText.From(source, Encoding.UTF8));
        }

        /// <summary>
        /// Creates a field declaration for a cached property change name.
        /// </summary>
        /// <param name="type">The type of cached property change argument (either <see cref="PropertyChangedEventArgs"/> or <see cref="PropertyChangingEventArgs"/>).</param>
        /// <param name="propertyName">The name of the cached property name.</param>
        /// <returns>A <see cref="FieldDeclarationSyntax"/> instance for the input cached property name.</returns>
        [Pure]
        private static FieldDeclarationSyntax CreateFieldDeclaration(INamedTypeSymbol type, string propertyName)
        {
            // Create a static field with a cached property changed/changing argument for a specified property.
            // This code produces a field declaration as follows:
            //
            // [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
            // [global::System.Obsolete("This field is not intended to be referenced directly by user code")]
            // public static readonly <ARG_TYPE> <PROPERTY_NAME><ARG_TYPE> = new("<PROPERTY_NAME>");
            return
                FieldDeclaration(
                VariableDeclaration(IdentifierName(type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)))
                .AddVariables(
                    VariableDeclarator(Identifier($"{propertyName}{type.Name}"))
                    .WithInitializer(EqualsValueClause(
                        ImplicitObjectCreationExpression()
                        .AddArgumentListArguments(Argument(
                            LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(propertyName))))))))
                .AddModifiers(
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.StaticKeyword),
                    Token(SyntaxKind.ReadOnlyKeyword))
                .AddAttributeLists(
                    AttributeList(SingletonSeparatedList(
                        Attribute(IdentifierName("global::System.ComponentModel.EditorBrowsable")).AddArgumentListArguments(
                        AttributeArgument(ParseExpression("global::System.ComponentModel.EditorBrowsableState.Never"))))),
                    AttributeList(SingletonSeparatedList(
                        Attribute(IdentifierName("global::System.Obsolete")).AddArgumentListArguments(
                        AttributeArgument(LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            Literal("This field is not intended to be referenced directly by user code")))))));
        }
    }
}
