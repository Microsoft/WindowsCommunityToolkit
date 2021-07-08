// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// A selection box item (an item that the user can select from the drop down list).
    /// </summary>
    public sealed class ToastSelectionBoxItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToastSelectionBoxItem"/> class.
        /// Constructs a new Toast SelectionBoxItem with the required elements.
        /// </summary>
        /// <param name="id">Developer-provided ID that the developer uses later to retrieve input when the Toast is interacted with.</param>
        /// <param name="content">String that is displayed on the selection item. This is what the user sees.</param>
        public ToastSelectionBoxItem(string id, string content)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        /// <summary>
        /// Gets the required ID property used so that developers can retrieve user input once the app is activated.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the required string that is displayed on the selection item.
        /// </summary>
        public string Content { get; private set; }

        internal Element_ToastSelection ConvertToElement()
        {
            return new Element_ToastSelection()
            {
                Id = Id,
                Content = Content
            };
        }
    }
}