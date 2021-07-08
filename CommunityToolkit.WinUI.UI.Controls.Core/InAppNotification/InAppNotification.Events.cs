// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// In App Notification defines a control to show local notification in the app.
    /// </summary>
    public partial class InAppNotification
    {
        /// <summary>
        /// Event raised when the notification is opening
        /// </summary>
        public event InAppNotificationOpeningEventHandler Opening;

        /// <summary>
        /// Event raised when the notification is opened
        /// </summary>
        public event EventHandler Opened;

        /// <summary>
        /// Event raised when the notification is closing
        /// </summary>
        public event InAppNotificationClosingEventHandler Closing;

        /// <summary>
        /// Event raised when the notification is closed
        /// </summary>
        public event InAppNotificationClosedEventHandler Closed;

        private void DismissButton_Click(object sender, RoutedEventArgs e)
        {
            Dismiss(InAppNotificationDismissKind.User);
        }

        private void DismissTimer_Tick(object sender, object e)
        {
            Dismiss(InAppNotificationDismissKind.Timeout);
        }

        private void OnCurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState.Name == StateContentVisible)
            {
                Visibility = Visibility.Visible;
            }
        }

        private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            switch (e.NewState.Name)
            {
                case StateContentVisible:
                    OnNotificationVisible();
                    break;
                case StateContentCollapsed:
                    OnNotificationCollapsed();
                    break;
            }
        }

        private void OnNotificationVisible()
        {
            Opened?.Invoke(this, EventArgs.Empty);
        }

        private void OnNotificationCollapsed()
        {
            Closed?.Invoke(this, new InAppNotificationClosedEventArgs(_lastDismissKind));
            Visibility = Visibility.Collapsed;
        }

        private void RaiseAutomationNotification()
        {
            if (!AutomationPeer.ListenerExists(AutomationEvents.LiveRegionChanged))
            {
                return;
            }

            var peer = FrameworkElementAutomationPeer.CreatePeerForElement(this);
            peer.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
        }
    }
}