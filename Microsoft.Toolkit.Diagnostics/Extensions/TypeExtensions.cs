// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
#if NETSTANDARD1_4
using System.Reflection;
#endif
using System.Runtime.CompilerServices;

namespace Microsoft.Toolkit.Diagnostics
{
    /// <summary>
    /// Helpers for working with types.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// The mapping of built-in types to their simple representation.
        /// </summary>
        private static readonly IReadOnlyDictionary<Type, string> BuiltInTypesMap = new Dictionary<Type, string>
        {
            [typeof(bool)] = "bool",
            [typeof(byte)] = "byte",
            [typeof(sbyte)] = "sbyte",
            [typeof(short)] = "short",
            [typeof(ushort)] = "ushort",
            [typeof(char)] = "char",
            [typeof(int)] = "int",
            [typeof(uint)] = "uint",
            [typeof(float)] = "float",
            [typeof(long)] = "long",
            [typeof(ulong)] = "ulong",
            [typeof(double)] = "double",
            [typeof(decimal)] = "decimal",
            [typeof(object)] = "object",
            [typeof(string)] = "string",
            [typeof(void)] = "void"
        };

        /// <summary>
        /// A thread-safe mapping of precomputed string representation of types.
        /// </summary>
        private static readonly ConditionalWeakTable<Type, string> DisplayNames = new ConditionalWeakTable<Type, string>();

        /// <summary>
        /// Returns a simple string representation of a type.
        /// </summary>
        /// <param name="type">The input type.</param>
        /// <returns>The string representation of <paramref name="type"/>.</returns>
        [Pure]
        public static string ToTypeString(this Type type)
        {
            // Local function to create the formatted string for a given type
            static string FormatDisplayString(Type type, int genericTypeOffset, ReadOnlySpan<Type> typeArguments)
            {
                // Primitive types use the keyword name
                if (BuiltInTypesMap.TryGetValue(type, out string? typeName))
                {
                    return typeName!;
                }

                // Array types are displayed as Foo[]
                if (type.IsArray)
                {
                    var elementType = type.GetElementType()!;
                    var rank = type.GetArrayRank();

                    return $"{FormatDisplayString(elementType, 0, elementType.GetGenericArguments())}[{new string(',', rank - 1)}]";
                }

                // By checking generic types here we are only interested in specific cases,
                // ie. nullable value types or value typles. We have a separate path for custom
                // generic types, as we can't rely on this API in that case, as it doesn't show
                // a difference between nested types that are themselves generic, or nested simple
                // types from a generic declaring type. To deal with that, we need to manually track
                // the offset within the array of generic arguments for the whole constructed type.
                if (type.IsGenericType())
                {
                    var genericTypeDefinition = type.GetGenericTypeDefinition();

                    // Nullable<T> types are displayed as T?
                    if (genericTypeDefinition == typeof(Nullable<>))
                    {
                        var nullableArguments = type.GetGenericArguments();

                        return $"{FormatDisplayString(nullableArguments[0], 0, nullableArguments)}?";
                    }

                    // ValueTuple<T1, T2> types are displayed as (T1, T2)
                    if (genericTypeDefinition == typeof(ValueTuple<>) ||
                        genericTypeDefinition == typeof(ValueTuple<,>) ||
                        genericTypeDefinition == typeof(ValueTuple<,,>) ||
                        genericTypeDefinition == typeof(ValueTuple<,,,>) ||
                        genericTypeDefinition == typeof(ValueTuple<,,,,>) ||
                        genericTypeDefinition == typeof(ValueTuple<,,,,,>) ||
                        genericTypeDefinition == typeof(ValueTuple<,,,,,,>) ||
                        genericTypeDefinition == typeof(ValueTuple<,,,,,,,>))
                    {
                        var formattedTypes = type.GetGenericArguments().Select(t => FormatDisplayString(t, 0, t.GetGenericArguments()));

                        return $"({string.Join(", ", formattedTypes)})";
                    }
                }

                string displayName;

                // Generic types
                if (type.Name.Contains('`'))
                {
                    // Retrieve the current generic arguments for the current type (leaf or not)
                    var tokens = type.Name.Split('`');
                    var genericArgumentsCount = int.Parse(tokens[1]);
                    var typeArgumentsOffset = typeArguments.Length - genericTypeOffset - genericArgumentsCount;
                    var currentTypeArguments = typeArguments.Slice(typeArgumentsOffset, genericArgumentsCount).ToArray();
                    var formattedTypes = currentTypeArguments.Select(t => FormatDisplayString(t, 0, t.GetGenericArguments()));

                    // Standard generic types are displayed as Foo<T>
                    displayName = $"{tokens[0]}<{string.Join(", ", formattedTypes)}>";

                    // Track the current offset for the shared generic arguments list
                    genericTypeOffset += genericArgumentsCount;
                }
                else
                {
                    // Simple custom types
                    displayName = type.Name;
                }

                // If the type is nested, recursively format the hierarchy as well
                if (type.IsNested)
                {
                    var openDeclaringType = type.DeclaringType!;
                    var rootGenericArguments = typeArguments.Slice(0, typeArguments.Length - genericTypeOffset).ToArray();

                    // If the declaring type is generic, we need to reconstruct the closed type
                    // manually, as the declaring type instance doesn't retain type information.
                    if (rootGenericArguments.Length > 0)
                    {
                        var closedDeclaringType = openDeclaringType.GetGenericTypeDefinition().MakeGenericType(rootGenericArguments);

                        return $"{FormatDisplayString(closedDeclaringType, genericTypeOffset, typeArguments)}.{displayName}";
                    }

                    return $"{FormatDisplayString(openDeclaringType, genericTypeOffset, typeArguments)}.{displayName}";
                }

                return $"{type.Namespace}.{displayName}";
            }

            // Atomically get or build the display string for the current type.
            return DisplayNames.GetValue(type, t =>
            {
                // By-ref types are displayed as T&
                if (t.IsByRef)
                {
                    t = t.GetElementType()!;

                    return $"{FormatDisplayString(t, 0, t.GetGenericArguments())}&";
                }

                // Pointer types are displayed as T*
                if (t.IsPointer)
                {
                    int depth = 0;

                    // Calculate the pointer indirection level
                    while (t.IsPointer)
                    {
                        depth++;
                        t = t.GetElementType()!;
                    }

                    return $"{FormatDisplayString(t, 0, t.GetGenericArguments())}{new string('*', depth)}";
                }

                // Standard path for concrete types
                return FormatDisplayString(t, 0, t.GetGenericArguments());
            });
        }

        /// <summary>
        /// Returns whether or not a given type is generic.
        /// </summary>
        /// <param name="type">The input type.</param>
        /// <returns>Whether or not the input type is generic.</returns>
        [Pure]
        private static bool IsGenericType(this Type type)
        {
#if NETSTANDARD1_4
            return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
        }

#if NETSTANDARD1_4
        /// <summary>
        /// Returns an array of types representing the generic arguments.
        /// </summary>
        /// <param name="type">The input type.</param>
        /// <returns>An array of types representing the generic arguments.</returns>
        [Pure]
        private static Type[] GetGenericArguments(this Type type)
        {
            return type.GetTypeInfo().GenericTypeParameters;
        }

        /// <summary>
        /// Returns whether <paramref name="type"/> is an instance of <paramref name="value"/>.
        /// </summary>
        /// <param name="type">The input type.</param>
        /// <param name="value">The type to check against.</param>
        /// <returns><see langword="true"/> if <paramref name="type"/> is an instance of <paramref name="value"/>, <see langword="false"/> otherwise.</returns>
        [Pure]
        internal static bool IsInstanceOfType(this Type type, object value)
        {
            return type.GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo());
        }
#endif
    }
}