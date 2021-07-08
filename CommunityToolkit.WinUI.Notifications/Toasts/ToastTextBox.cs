// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// A text box control on the Toast that a user can type text into.
    /// </summary>
    public sealed class ToastTextBox : IToastInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToastTextBox"/> class.
        /// A new Toast TextBox input control with the required elements.
        /// </summary>
        /// <param name="id">Developer-provided ID that the developer uses later to retrieve input when the Toast is interacted with.</param>
        public ToastTextBox(string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        /// <summary>
        /// Gets the required ID property so that developers can retrieve user input once the app is activated.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets or sets title text to display above the text box.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets placeholder text to be displayed on the text box when the user hasn't typed any text yet.
        /// </summary>
        public string PlaceholderContent { get; set; }

        /// <summary>
        /// Gets or sets the initial text to place in the text box. Leave this null for a blank text box.
        /// </summary>
        public string DefaultInput { get; set; }

        internal Element_ToastInput ConvertToElement()
        {
            return new Element_ToastInput()
            {
                Id = Id,
                Type = ToastInputType.Text,
                DefaultInput = DefaultInput,
                PlaceholderContent = PlaceholderContent,
                Title = Title
            };
        }
    }
}