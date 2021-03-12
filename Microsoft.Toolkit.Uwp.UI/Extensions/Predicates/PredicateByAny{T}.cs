// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace Microsoft.Toolkit.Uwp.UI.Predicates
{
    /// <summary>
    /// An <see cref="IPredicate{T}"/> type matching all instances of a given type.
    /// </summary>
    /// <typeparam name="T">The type of items to match.</typeparam>
    internal readonly struct PredicateByAny<T> : IPredicate<T>
        where T : class
    {
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Match(T element)
        {
            return true;
        }
    }
}