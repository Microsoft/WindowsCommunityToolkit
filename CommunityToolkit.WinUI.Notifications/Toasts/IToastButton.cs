// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// One of <see cref="ToastButton"/>, <see cref="ToastButtonSnooze"/>, or <see cref="ToastButtonDismiss"/>.
    /// </summary>
    public interface IToastButton
    {
        /// <summary>
        /// Gets or sets an optional image icon for the button to display (required for buttons adjacent to inputs like quick reply).
        /// </summary>
        string ImageUri { get; set; }

        /// <summary>
        /// Gets or sets an identifier used in telemetry to identify your category of action. This should be something
        /// like "Delete", "Reply", or "Archive". In the upcoming toast telemetry dashboard in Dev Center, you will
        /// be able to view how frequently your actions are being clicked.
        /// </summary>
        string HintActionId { get; set; }
    }
}