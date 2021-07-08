// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// A selection box control, which lets users pick from a dropdown list of options.
    /// </summary>
    public sealed class ToastSelectionBox : IToastInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToastSelectionBox"/> class.
        /// A Toast SelectionBox input control with the required elements.
        /// </summary>
        /// <param name="id">Developer-provided ID that the developer uses later to retrieve input when the Toast is interacted with.</param>
        public ToastSelectionBox(string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        /// <summary>
        /// Gets the required ID property used so that developers can retrieve user input once the app is activated.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets or sets title text to display above the SelectionBox.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets which item is selected by default, and refers to the Id property of <see cref="ToastSelectionBoxItem"/>. If you do not provide this, the default selection will be empty (user sees nothing).
        /// </summary>
        public string DefaultSelectionBoxItemId { get; set; }

        /// <summary>
        /// Gets the selection items that the user can pick from in this SelectionBox. Only 5 items can be added.
        /// </summary>
        public IList<ToastSelectionBoxItem> Items { get; private set; } = new LimitedList<ToastSelectionBoxItem>(5);

        internal Element_ToastInput ConvertToElement()
        {
            var input = new Element_ToastInput()
            {
                Type = ToastInputType.Selection,
                Id = Id,
                DefaultInput = DefaultSelectionBoxItemId,
                Title = Title
            };

            foreach (var item in Items)
            {
                input.Children.Add(item.ConvertToElement());
            }

            return input;
        }
    }
}