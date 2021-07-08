// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// Create your own custom actions, using controls like <see cref="ToastButton"/>, <see cref="ToastTextBox"/>, and <see cref="ToastSelectionBox"/>.
    /// </summary>
    public sealed class ToastActionsCustom : IToastActions
    {
        /// <summary>
        /// Gets inputs like <see cref="ToastTextBox"/> and <see cref="ToastSelectionBox"/>. Only up to 5 inputs can be added; after that, an exception is thrown.
        /// </summary>
        public IList<IToastInput> Inputs { get; private set; } = new LimitedList<IToastInput>(5);

        /// <summary>
        /// Gets buttons displayed after all the inputs (or adjacent to inputs if used as quick reply buttons). Only up to 5 buttons can be added (or fewer if you are also including context menu items). After that, an exception is thrown. You can add <see cref="ToastButton"/>, <see cref="ToastButtonSnooze"/>, or <see cref="ToastButtonDismiss"/>
        /// </summary>
        public IList<IToastButton> Buttons { get; private set; } = new LimitedList<IToastButton>(5);

        /// <summary>
        /// Gets custom context menu items, providing additional actions when the user right clicks the Toast notification.
        /// You can only have up to 5 buttons and context menu items *combined*. Thus, if you have one context menu item,
        /// you can only have four buttons, etc. New in Anniversary Update:
        /// </summary>
        public IList<ToastContextMenuItem> ContextMenuItems { get; private set; } = new List<ToastContextMenuItem>();

        internal Element_ToastActions ConvertToElement()
        {
            if (Buttons.Count + ContextMenuItems.Count > 5)
            {
                throw new InvalidOperationException("You have too many buttons/context menu items. You can only have up to 5 total.");
            }

            var el = new Element_ToastActions();

            foreach (var input in Inputs)
            {
                el.Children.Add(ConvertToInputElement(input));
            }

            foreach (var button in this.Buttons)
            {
                el.Children.Add(ConvertToActionElement(button));
            }

            foreach (var item in ContextMenuItems)
            {
                el.Children.Add(item.ConvertToElement());
            }

            return el;
        }

        private static Element_ToastAction ConvertToActionElement(IToastButton button)
        {
            if (button is ToastButton)
            {
                return (button as ToastButton).ConvertToElement();
            }

            if (button is ToastButtonDismiss)
            {
                return (button as ToastButtonDismiss).ConvertToElement();
            }

            if (button is ToastButtonSnooze)
            {
                return (button as ToastButtonSnooze).ConvertToElement();
            }

            throw new NotImplementedException("Unknown button child: " + button.GetType());
        }

        private static Element_ToastInput ConvertToInputElement(IToastInput input)
        {
            if (input is ToastTextBox)
            {
                return (input as ToastTextBox).ConvertToElement();
            }

            if (input is ToastSelectionBox)
            {
                return (input as ToastSelectionBox).ConvertToElement();
            }

            throw new NotImplementedException("Unknown input child: " + input.GetType());
        }
    }
}