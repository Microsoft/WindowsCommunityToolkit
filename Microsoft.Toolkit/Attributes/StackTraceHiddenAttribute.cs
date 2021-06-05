// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NET6_0_OR_GREATER

namespace System.Diagnostics
{
    /// <summary>
    /// Removes annotated methods from the stacktrace in case an exception occurrs while they're on the stack.
    /// </summary>
    /// <remarks>
    /// This is a port of the attribute from the BCL in .NET 6, and it's marked as conditional so that it will
    /// be removed for package builds that are released on NuGet. This attribute is only used internally to
    /// avoid having to clutter the codebase with many compiler directive switches to check for this API.
    /// </remarks>
    [Conditional("DEBUG")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Struct, Inherited = false)]
    internal sealed class StackTraceHiddenAttribute : Attribute
    {
    }
}

#endif