// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This extension is restricted to the .NET 5 because it shares the same BCL
// across all targets, ensuring that the layout of our Nullable<T> mapping type
// will be correct. Exposing this API on older targets (especially .NET Standard)
// is not guaranteed to be correct and could result in invalid memory accesses.
#if NET5_0

using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Toolkit.HighPerformance.Extensions
{
    /// <summary>
    /// Helpers for working with the <see cref="Nullable{T}"/> type.
    /// </summary>
    public static class NullableExtensions
    {
        /// <summary>
        /// Returns a reference to the value of the input <see cref="Nullable{T}"/> instance, regardless of whether
        /// the <see cref="Nullable{T}.HasValue"/> property is returning <see langword="true"/> or not. If that is not
        /// the case, this method will still return a reference to the underlying <see langword="default"/> value.
        /// </summary>
        /// <typeparam name="T">The type of the underlying value</typeparam>
        /// <param name="value">The <see cref="Nullable{T}"/></param>
        /// <returns>A reference to the underlying value from the input <see cref="Nullable{T}"/> instance.</returns>
        /// <remarks>
        /// Note that attempting to mutate the returned reference will not change the value returned by <see cref="Nullable{T}.HasValue"/>.
        /// That means that reassigning the value of an empty instance will not make <see cref="Nullable{T}.HasValue"/> return <see langword="true"/>.
        /// </remarks>
        public static ref T DangerousGetValueOrDefaultReference<T>(this ref T? value)
            where T : struct
        {
            return ref Unsafe.As<T?, RawNullableData<T>>(ref value).Value;
        }

        /// <summary>
        /// Mapping type that reflects the internal layout of the <see cref="Nullable{T}"/> type.
        /// See https://github.com/dotnet/runtime/blob/master/src/libraries/System.Private.CoreLib/src/System/Nullable.cs.
        /// </summary>
        /// <typeparam name="T">The value type wrapped by the current instance.</typeparam>
        private struct RawNullableData<T>
            where T : struct
        {
#pragma warning disable CS0649 // Unassigned fields
            public bool HasValue;
            public T Value;
#pragma warning restore CS0649
        }
    }
}

#endif