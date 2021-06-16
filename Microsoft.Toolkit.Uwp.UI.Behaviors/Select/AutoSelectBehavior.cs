// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI.Behaviors
{
    /// <summary>
    /// This behavior automatically selects the entire content of the associated <see cref="TextBox"/> when it is loaded.
    /// </summary>
    public sealed class AutoSelectBehavior : BehaviorBase<TextBox>
    {
        /// <inheritdoc/>
        protected override void OnAssociatedObjectLoaded() => AssociatedObject.SelectAll();
    }
}
