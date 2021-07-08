// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Provides access to unresolved token string values within the tokenizing text box control
    /// </summary>
    public interface ITokenStringContainer
    {
        /// <summary>
        /// Gets or sets the string text for this unresolved token
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Gets a value indicating whether this is the last text based token in the collection as it will always remain at the end.
        /// </summary>
        bool IsLast { get; }
    }
}