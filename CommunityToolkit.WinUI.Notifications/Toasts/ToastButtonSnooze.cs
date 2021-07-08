// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// A system-handled snooze button that automatically handles snoozing of a Toast notification.
    /// </summary>
    public sealed class ToastButtonSnooze : IToastButton
    {
        /// <summary>
        /// Gets custom text displayed on the button that overrides the default localized "Snooze" text.
        /// </summary>
        public string CustomContent { get; private set; }

        /// <summary>
        /// Gets or sets an optional image icon for the button to display.
        /// </summary>
        public string ImageUri { get; set; }

        /// <summary>
        /// Gets or sets the ID of an existing <see cref="ToastSelectionBox"/> in order to allow the
        /// user to pick a custom snooze time. Optional. The ID's of the <see cref="ToastSelectionBoxItem"/>s
        /// inside the selection box must represent the snooze interval in minutes. For example,
        /// if the user selects an item that has an ID of "120", then the notification will be snoozed
        /// for 2 hours. When the user clicks this button, if you specified a SelectionBoxId, the system
        /// will parse the ID of the selected item and snooze by that amount of minutes. If you didn't specify
        /// a SelectionBoxId, the system will snooze by the default system snooze time.
        /// </summary>
        public string SelectionBoxId { get; set; }

        /// <summary>
        /// Gets or sets an identifier used in telemetry to identify your category of action. This should be something
        /// like "Delete", "Reply", or "Archive". In the upcoming toast telemetry dashboard in Dev Center, you will
        /// be able to view how frequently your actions are being clicked.
        /// </summary>
        public string HintActionId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToastButtonSnooze"/> class.
        /// </summary>
        public ToastButtonSnooze()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToastButtonSnooze"/> class.
        /// Initializes a system-handled snooze button that displays your text on the button and automatically handles snoozing.
        /// </summary>
        /// <param name="customContent">The text you want displayed on the button.</param>
        public ToastButtonSnooze(string customContent)
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
                Content = CustomContent ?? string.Empty, // If not using custom content, we need to provide empty string, otherwise Toast doesn't get displayed
                Arguments = "snooze",
                ActivationType = Element_ToastActivationType.System,
                InputId = SelectionBoxId,
                ImageUri = ImageUri,
                HintActionId = HintActionId
            };
        }
    }
}