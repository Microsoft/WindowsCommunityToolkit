// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// A button that, when clicked, is interpreted as a "dismiss" by the system, and the Toast is dismissed just like if the user swiped the Toast away.
    /// </summary>
    public sealed class ToastButtonDismiss : IToastButton
    {
        /// <summary>
        /// Gets custom text displayed on the button that overrides the default localized "Dismiss" text.
        /// </summary>
        public string CustomContent { get; private set; }

        /// <summary>
        /// Gets or sets an optional image icon for the button to display.
        /// </summary>
        public string ImageUri { get; set; }

        /// <summary>
        /// Gets or sets an identifier used in telemetry to identify your category of action. This should be something
        /// like "Delete", "Reply", or "Archive". In the upcoming toast telemetry dashboard in Dev Center, you will
        /// be able to view how frequently your actions are being clicked.
        /// </summary>
        public string HintActionId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToastButtonDismiss"/> class.
        /// </summary>
        public ToastButtonDismiss()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToastButtonDismiss"/> class.
        /// Constructs a system-handled dismiss button that displays your text on the button.
        /// </summary>
        /// <param name="customContent">The text you want displayed on the button.</param>
        public ToastButtonDismiss(string customContent)
        {
            if (customContent == null)
            {
                throw new ArgumentNullException(nameof(customContent));
            }

            CustomContent = customContent;
        }

        internal Element_ToastAction ConvertToElement()
        {
            return new Element_ToastAction()
            {
                Content = this.CustomContent == null ? string.Empty : this.CustomContent, // If not using custom content, we need to provide empty string, otherwise Toast doesn't get displayed
                Arguments = "dismiss",
                ActivationType = Element_ToastActivationType.System,
                ImageUri = ImageUri,
                HintActionId = HintActionId

                // InputId is useless since dismiss button can't be placed to the right of text box (shell doesn't display it)
            };
        }
    }
}