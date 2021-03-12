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
    /// <typeparam name="TState">The type of state to use when matching items.</typeparam>
    internal readonly struct PredicateByFunc<T, TState> : IPredicate<T>
        where T : class
    {
        /// <summary>
        /// The state to give as input to <see name="predicate"/>.
        /// </summary>
        private readonly TState state;

        /// <summary>
        /// The predicatee to use to match items.
        /// </summary>
        private readonly Func<T, TState, bool> predicate;

        /// <summary>
        /// Initializes a new instance of the <see cref="PredicateByFunc{T, TState}"/> struct.
        /// </summary>
        /// <param name="state">The state to give as input to <paramref name="predicate"/>.</param>
        /// <param name="predicate">The predicatee to use to match items.</param>
        public PredicateByFunc(TState state, Func<T, TState, bool> predicate)
        {
            this.state = state;
            this.predicate = predicate;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Match(T element)
        {
            return this.predicate(element, state);
        }
    }
}
