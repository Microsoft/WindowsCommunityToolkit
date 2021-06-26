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
    internal readonly struct PredicateByType : IPredicate<object>
    {
        /// <summary>
        /// The type of element to match.
        /// </summary>
        private readonly Type type;

        /// <summary>
        /// Initializes a new instance of the <see cref="PredicateByType"/> struct.
        /// </summary>
        /// <param name="type">The type of element to match.</param>
        public PredicateByType(Type type)
        {
            this.type = type;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Match(object element)
        {
            return element.GetType() == this.type;
        }
    }
}