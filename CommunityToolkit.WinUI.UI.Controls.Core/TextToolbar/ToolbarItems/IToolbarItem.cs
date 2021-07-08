// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.WinUI.UI.Controls.TextToolbarButtons
{
    /// <summary>
    /// Interface that defines the position of an item in a <see cref="TextToolbar"/>
    /// </summary>
    public interface IToolbarItem : ICommandBarElement
    {
        /// <summary>
        /// Gets or sets index of this Element
        /// </summary>
        int Position { get; set; }
    }
}