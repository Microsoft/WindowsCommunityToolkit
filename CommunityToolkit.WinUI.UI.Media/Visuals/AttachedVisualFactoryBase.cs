// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;

namespace CommunityToolkit.WinUI.UI.Media
{
    /// <summary>
    /// A type responsible for creating <see cref="Visual"/> instances to attach to target elements.
    /// </summary>
    public abstract class AttachedVisualFactoryBase : DependencyObject
    {
        /// <summary>
        /// Creates a <see cref="Visual"/> to attach to the target element.
        /// </summary>
        /// <param name="element">The target <see cref="UIElement"/> the visual will be attached to.</param>
        /// <returns>A <see cref="Visual"/> instance that the caller will attach to the target element.</returns>
        public abstract ValueTask<Visual> GetAttachedVisualAsync(UIElement element);
    }
}