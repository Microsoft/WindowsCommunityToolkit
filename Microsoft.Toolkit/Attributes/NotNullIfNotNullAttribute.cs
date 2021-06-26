// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NETSTANDARD2_1_OR_GREATER

namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    /// Specifies that the output will be non-null if the named parameter is non-null.
    /// </summary>
    /// <remarks>Internal copy from the BCL attribute.</remarks>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
    internal sealed class NotNullIfNotNullAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotNullIfNotNullAttribute"/> class.
        /// </summary>
        /// <param name="parameterName">The associated parameter name. The output will be non-null if the argument to the parameter specified is non-null.</param>
        public NotNullIfNotNullAttribute(string parameterName) => ParameterName = parameterName;

        /// <summary>
        /// Gets the associated parameter name.
        /// </summary>
        public string ParameterName { get; }
    }
}

#endif