// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WIN32

using System;
using System.Collections.Generic;
using Windows.UI.Notifications;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// Manages the toast notifications for an app including the ability the clear all toast history and removing individual toasts.
    /// </summary>
    [Obsolete("We recommend switching to the new ToastNotificationManagerCompat and ToastNotificationHistoryCompat.")]
    public sealed class DesktopNotificationHistoryCompat
    {
        private string _aumid;
        private ToastNotificationHistory _history;

        /// <summary>
        /// Initializes a new instance of the <see cref="DesktopNotificationHistoryCompat"/> class.
        /// Do not call this. Instead, call <see cref="DesktopNotificationManagerCompat.History"/> to obtain an instance.
        /// </summary>
        /// <param name="aumid">An AUMID that uniquely identifies your application.</param>
        internal DesktopNotificationHistoryCompat(string aumid)
        {
            _aumid = aumid;
            _history = ToastNotificationManager.History;
        }

        /// <summary>
        /// Removes all notifications sent by this app from action center.
        /// </summary>
        public void Clear()
        {
            if (_aumid != null)
            {
                _history.Clear(_aumid);
            }
            else
            {
                _history.Clear();
            }
        }

        /// <summary>
        /// Gets all notifications sent by this app that are currently still in Action Center.
        /// </summary>
        /// <returns>A collection of toasts.</returns>
        public IReadOnlyList<ToastNotification> GetHistory()
        {
            return _aumid != null ? _history.GetHistory(_aumid) : _history.GetHistory();
        }

        /// <summary>
        /// Removes an individual toast, with the specified tag label, from action center.
        /// </summary>
        /// <param name="tag">The tag label of the toast notification to be removed.</param>
        public void Remove(string tag)
        {
            if (_aumid != null)
            {
                _history.Remove(tag, string.Empty, _aumid);
            }
            else
            {
                _history.Remove(tag);
            }
        }

        /// <summary>
        /// Removes a toast notification from the action using the notification's tag and group labels.
        /// </summary>
        /// <param name="tag">The tag label of the toast notification to be removed.</param>
        /// <param name="group">The group label of the toast notification to be removed.</param>
        public void Remove(string tag, string group)
        {
            if (_aumid != null)
            {
                _history.Remove(tag, group, _aumid);
            }
            else
            {
                _history.Remove(tag, group);
            }
        }

        /// <summary>
        /// Removes a group of toast notifications, identified by the specified group label, from action center.
        /// </summary>
        /// <param name="group">The group label of the toast notifications to be removed.</param>
        public void RemoveGroup(string group)
        {
            if (_aumid != null)
            {
                _history.RemoveGroup(group, _aumid);
            }
            else
            {
                _history.RemoveGroup(group);
            }
        }
    }
}

#endif