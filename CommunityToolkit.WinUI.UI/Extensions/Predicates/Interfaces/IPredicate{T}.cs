// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.UI.Predicates
{
    /// <summary>
    /// An interface representing a predicate for items of a given type.
    /// </summary>
    /// <typeparam name="T">The type of items to match.</typeparam>
    internal interface IPredicate<in T>
        where T : class
    {
        /// <summary>
        /// Performs a match with the current predicate over a target <typeparamref name="T"/> instance.
        /// </summary>
        /// <param name="element">The input element to match.</param>
        /// <returns>Whether the match evaluation was successful.</returns>
        bool Match(T element);
    }
}