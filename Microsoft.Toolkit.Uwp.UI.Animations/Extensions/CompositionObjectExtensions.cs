// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Diagnostics.Contracts;
using Windows.UI.Composition;

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// An extension <see langword="class"/> for the <see cref="CompositionObject"/> type.
    /// </summary>
    public static class CompositionObjectExtensions
    {
        /// <summary>
        /// Starts a given <see cref="CompositionAnimation"/> on a target <see cref="CompositionObject"/>.
        /// </summary>
        /// <param name="compositionObject">The target <see cref="CompositionObject"/> instance to animate.</param>
        /// <param name="animation">The <see cref="CompositionAnimation"/> instance to run.</param>
        /// <remarks>This method requires <paramref name="animation"/> to have its <see cref="CompositionAnimation.Target"/> property set.</remarks>
        [Pure]
        public static void StartAnimation(this CompositionObject compositionObject, CompositionAnimation animation)
        {
            compositionObject.StartAnimation(animation.Target, animation);
        }
    }
}