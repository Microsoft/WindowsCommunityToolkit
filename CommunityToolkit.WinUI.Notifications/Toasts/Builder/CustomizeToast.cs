// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINDOWS_UWP

using Windows.Foundation;
using Windows.UI.Notifications;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// Allows you to set additional properties on the <see cref="ToastNotification"/> object before the toast is displayed.
    /// </summary>
    /// <param name="toast">The toast to modify that will be displayed.</param>
    public delegate void CustomizeToast(ToastNotification toast);

    /// <summary>
    /// Allows you to set additional properties on the <see cref="ToastNotification"/> object before the toast is displayed.
    /// </summary>
    /// <param name="toast">The toast to modify that will be displayed.</param>
    /// <returns>An operation.</returns>
    public delegate IAsyncAction CustomizeToastAsync(ToastNotification toast);

    /// <summary>
    /// Allows you to set additional properties on the <see cref="ScheduledToastNotification"/> object before the toast is scheduled.
    /// </summary>
    /// <param name="toast">The scheduled toast to modify that will be scheduled.</param>
    public delegate void CustomizeScheduledToast(ScheduledToastNotification toast);

    /// <summary>
    /// Allows you to set additional properties on the <see cref="ScheduledToastNotification"/> object before the toast is scheduled.
    /// </summary>
    /// <param name="toast">The scheduled toast to modify that will be scheduled.</param>
    /// <returns>An operation.</returns>
    public delegate IAsyncAction CustomizeScheduledToastAsync(ScheduledToastNotification toast);
}

#endif