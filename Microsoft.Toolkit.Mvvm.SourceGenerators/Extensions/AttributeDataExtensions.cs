// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Toolkit.Mvvm.SourceGenerators.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="AttributeData"/> type.
    /// </summary>
    internal static class AttributeDataExtensions
    {
        /// <summary>
        /// Checks whether a given <see cref="AttributeData"/> instance contains a specified named argument.
        /// </summary>
        /// <typeparam name="T">The type of argument to check.</typeparam>
        /// <param name="attributeData">The target <see cref="AttributeData"/> instance to check.</param>
        /// <param name="name">The name of the argument to check.</param>
        /// <param name="value">The expected value for the target named argument.</param>
        /// <returns>Whether or not <paramref name="attributeData"/> contains an argument named <paramref name="name"/> with the expected value.</returns>
        [Pure]
        public static bool HasNamedArgument<T>(this AttributeData attributeData, string name, T? value)
        {
            foreach (KeyValuePair<string, TypedConstant> properties in attributeData.NamedArguments)
            {
                if (properties.Key == name)
                {
                    return
                        properties.Value.Value is T argumentValue &&
                        EqualityComparer<T?>.Default.Equals(argumentValue, value);
                }
            }

            return false;
        }

        /// <summary>
        /// Creates an <see cref="AttributeSyntax"/> node that is equivalent to the input <see cref="AttributeData"/> instance.
        /// </summary>
        /// <param name="attributeData">The input <see cref="AttributeData"/> instance to process.</param>
        /// <returns>An <see cref="AttributeSyntax"/> replicating the data in <paramref name="attributeData"/>.</returns>
        [Pure]
        public static AttributeSyntax AsAttributeSyntax(this AttributeData attributeData)
        {
            IdentifierNameSyntax attributeType = IdentifierName(attributeData.AttributeClass!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
            AttributeArgumentSyntax[] arguments =
                attributeData.ConstructorArguments
                .Select(static arg => AttributeArgument(ToExpression(arg))).Concat(
                    attributeData.NamedArguments
                    .Select(static arg =>
                        AttributeArgument(ToExpression(arg.Value))
                        .WithNameEquals(NameEquals(IdentifierName(arg.Key))))).ToArray();

            return Attribute(attributeType, AttributeArgumentList(SeparatedList(SeparatedList(arguments))));

            static ExpressionSyntax ToExpression(TypedConstant arg)
            {
                if (arg.IsNull)
                {
                    return LiteralExpression(SyntaxKind.NullLiteralExpression);
                }

                if (arg.Kind == TypedConstantKind.Array)
                {
                    string elementType = ((IArrayTypeSymbol)arg.Type!).ElementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                    return
                        ArrayCreationExpression(
                        ArrayType(IdentifierName(elementType))
                        .AddRankSpecifiers(ArrayRankSpecifier(SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression()))))
                        .WithInitializer(InitializerExpression(SyntaxKind.ArrayInitializerExpression)
                        .AddExpressions(arg.Values.Select(ToExpression).ToArray()));
                }

                switch ((arg.Kind, arg.Value))
                {
                    case (TypedConstantKind.Primitive, string text):
                        return LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(text));
                    case (TypedConstantKind.Primitive, bool flag) when flag:
                        return LiteralExpression(SyntaxKind.TrueLiteralExpression);
                    case (TypedConstantKind.Primitive, bool):
                        return LiteralExpression(SyntaxKind.FalseLiteralExpression);
                    case (TypedConstantKind.Primitive, object value):
                        return LiteralExpression(SyntaxKind.NumericLiteralExpression, value switch
                        {
                            byte b => Literal(b),
                            char c => Literal(c),
                            double d => Literal(d),
                            float f => Literal(f),
                            int i => Literal(i),
                            long l => Literal(l),
                            sbyte sb => Literal(sb),
                            short sh => Literal(sh),
                            uint ui => Literal(ui),
                            ulong ul => Literal(ul),
                            ushort ush => Literal(ush),
                            _ => throw new ArgumentException()
                        });
                    case (TypedConstantKind.Type, ITypeSymbol type):
                        return TypeOfExpression(IdentifierName(type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));
                    case (TypedConstantKind.Enum, object value):
                        return CastExpression(
                            IdentifierName(arg.Type!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)),
                            LiteralExpression(SyntaxKind.NumericLiteralExpression, ParseToken(value.ToString())));
                    default: throw new ArgumentException();
                }
            }
        }
    }
}
