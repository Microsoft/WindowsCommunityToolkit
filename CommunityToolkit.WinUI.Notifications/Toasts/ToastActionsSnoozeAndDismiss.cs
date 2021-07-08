// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// Automatically constructs a selection box for snooze intervals, and snooze/dismiss buttons, all automatically localized, and snoozing logic is automatically handled by the system.
    /// </summary>
    public sealed class ToastActionsSnoozeAndDismiss : IToastActions
    {
        /// <summary>
        /// Gets custom context menu items, providing additional actions when the user right clicks the Toast notification.
        /// You can only have up to 5 items. New in Anniversary Update
        /// </summary>
        public IList<ToastContextMenuItem> ContextMenuItems { get; private set; } = new List<ToastContextMenuItem>();

        internal Element_ToastActions ConvertToElement()
        {
            if (ContextMenuItems.Count > 5)
            {
                throw new InvalidOperationException("You have too many context menu items. You can only have up to 5.");
            }

            var el = new Element_ToastActions()
            {
                SystemCommands = ToastSystemCommand.SnoozeAndDismiss
            };

            foreach (var item in ContextMenuItems)
            {
                el.Children.Add(item.ConvertToElement());
            }

            return el;
        }
    }
}