// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI.Behaviors
{
    /// <summary>
    /// This behavior automatically sets the focus on the associated <see cref="Control"/> when it is loaded.
    /// </summary>
    public sealed class AutoFocusBehavior : BehaviorBase<Control>
    {
        /// <inheritdoc/>
        protected override void OnAssociatedObjectLoaded() => AssociatedObject.Focus(FocusState.Programmatic);
    }
}