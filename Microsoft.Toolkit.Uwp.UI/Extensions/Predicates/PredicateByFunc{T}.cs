// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Toolkit.Uwp.UI.Predicates
{
    /// <summary>
    /// An <see cref="IPredicate{T}"/> type matching items of a given type.
    /// </summary>
    /// <typeparam name="T">The type of items to match.</typeparam>
    internal readonly struct PredicateByFunc<T> : IPredicate<T>
        where T : class
    {
        /// <summary>
        /// The predicatee to use to match items.
        /// </summary>
        private readonly Func<T, bool> predicate;

        /// <summary>
        /// Initializes a new instance of the <see cref="PredicateByFunc{T}"/> struct.
        /// </summary>
        /// <param name="predicate">The predicatee to use to match items.</param>
        public PredicateByFunc(Func<T, bool> predicate)
        {
            this.predicate = predicate;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Match(T element)
        {
            return this.predicate(element);
        }
    }
}