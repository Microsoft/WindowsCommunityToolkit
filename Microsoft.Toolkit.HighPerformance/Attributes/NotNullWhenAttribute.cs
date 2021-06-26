// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NETSTANDARD2_1_OR_GREATER

namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    /// Specifies that when a method returns <see cref="ReturnValue"/>, the parameter
    /// will not be null even if the corresponding type allows it.
    /// </summary>
    /// <remarks>Internal copy of the .NET Standard 2.1 attribute.</remarks>
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class NotNullWhenAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotNullWhenAttribute"/> class.
        /// </summary>
        /// <param name="returnValue">
        /// The return value condition. If the method returns this value,
        /// the associated parameter will not be <see langword="null"/>.
        /// </param>
        public NotNullWhenAttribute(bool returnValue)
        {
            ReturnValue = returnValue;
        }

        /// <summary>
        /// Gets a value indicating whether the return value should be <see langword="true"/>.
        /// </summary>
        public bool ReturnValue { get; }
    }
}

#endif