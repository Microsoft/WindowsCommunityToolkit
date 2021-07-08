// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINDOWS_UWP

using System.Collections.Generic;
using Windows.UI.Notifications;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// Allows you to show and schedule toast notifications.
    /// </summary>
    public sealed class ToastNotifierCompat
    {
        private ToastNotifier _notifier;

        internal ToastNotifierCompat(ToastNotifier notifier)
        {
            _notifier = notifier;
        }

        /// <summary>
        /// Displays the specified toast notification.
        /// </summary>
        /// <param name="notification">The object that contains the content of the toast notification to display.</param>
        public void Show(ToastNotification notification)
        {
#if WIN32
            PreprocessToast(notification);
#endif

            _notifier.Show(notification);

#if WIN32
            ToastNotificationManagerCompat.SetHasSentToastNotification();
#endif
        }

        /// <summary>
        /// Hides the specified toast notification from the screen (moves it into Action Center).
        /// </summary>
        /// <param name="notification">The object that specifies the toast to hide.</param>
        public void Hide(ToastNotification notification)
        {
#if WIN32
            PreprocessToast(notification);
#endif

            _notifier.Hide(notification);
        }

        /// <summary>
        /// Adds a ScheduledToastNotification for later display by Windows.
        /// </summary>
        /// <param name="scheduledToast">The scheduled toast notification, which includes its content and timing instructions.</param>
        public void AddToSchedule(ScheduledToastNotification scheduledToast)
        {
#if WIN32
            ToastNotificationManagerCompat.PreRegisterIdentityLessApp();

            PreprocessScheduledToast(scheduledToast);
#endif

            _notifier.AddToSchedule(scheduledToast);
        }

        /// <summary>
        /// Cancels the scheduled display of a specified ScheduledToastNotification.
        /// </summary>
        /// <param name="scheduledToast">The notification to remove from the schedule.</param>
        public void RemoveFromSchedule(ScheduledToastNotification scheduledToast)
        {
#if WIN32
            PreprocessScheduledToast(scheduledToast);
#endif

            _notifier.RemoveFromSchedule(scheduledToast);
        }

        /// <summary>
        /// Gets the collection of ScheduledToastNotification objects that this app has scheduled for display.
        /// </summary>
        /// <returns>The collection of scheduled toast notifications that the app bound to this notifier has scheduled for timed display.</returns>
        public IReadOnlyList<ScheduledToastNotification> GetScheduledToastNotifications()
        {
            return _notifier.GetScheduledToastNotifications();
        }

        /// <summary>
        /// Updates the existing toast notification that has the specified tag and belongs to the specified notification group.
        /// </summary>
        /// <param name="data">An object that contains the updated info.</param>
        /// <param name="tag">The identifier of the toast notification to update.</param>
        /// <param name="group">The ID of the ToastCollection that contains the notification.</param>
        /// <returns>A value that indicates the result of the update (failure, success, etc).</returns>
        public NotificationUpdateResult Update(NotificationData data, string tag, string group)
        {
            return _notifier.Update(data, tag, group);
        }

        /// <summary>
        /// Updates the existing toast notification that has the specified tag.
        /// </summary>
        /// <param name="data">An object that contains the updated info.</param>
        /// <param name="tag">The identifier of the toast notification to update.</param>
        /// <returns>A value that indicates the result of the update (failure, success, etc).</returns>
        public NotificationUpdateResult Update(NotificationData data, string tag)
        {
#if WIN32
            // For apps that don't have identity...
            if (!DesktopBridgeHelpers.HasIdentity())
            {
                // If group isn't specified, we have to add a group since otherwise can't remove without a group
                return Update(data, tag, ToastNotificationManagerCompat.DEFAULT_GROUP);
            }
#endif

            return _notifier.Update(data, tag);
        }

        /// <summary>
        /// Gets a value that tells you whether there is an app, user, or system block that prevents the display of a toast notification.
        /// </summary>
        public NotificationSetting Setting
        {
            get
            {
#if WIN32
                // Just like scheduled notifications, apps need to have sent a notification
                // before checking the setting value works
                ToastNotificationManagerCompat.PreRegisterIdentityLessApp();
#endif

                return _notifier.Setting;
            }
        }

#if WIN32
        private void PreprocessToast(ToastNotification notification)
        {
            // For apps that don't have identity...
            if (!DesktopBridgeHelpers.HasIdentity())
            {
                // If tag is specified and group isn't specified
                if (!string.IsNullOrEmpty(notification.Tag) && string.IsNullOrEmpty(notification.Group))
                {
                    // We have to add a group since otherwise can't remove without a group
                    notification.Group = ToastNotificationManagerCompat.DEFAULT_GROUP;
                }
            }
        }

        private void PreprocessScheduledToast(ScheduledToastNotification notification)
        {
            // For apps that don't have identity...
            if (!DesktopBridgeHelpers.HasIdentity())
            {
                // If tag is specified and group isn't specified
                if (!string.IsNullOrEmpty(notification.Tag) && string.IsNullOrEmpty(notification.Group))
                {
                    // We have to add a group since otherwise can't remove without a group
                    notification.Group = ToastNotificationManagerCompat.DEFAULT_GROUP;
                }
            }
        }
#endif
    }
}

#endif