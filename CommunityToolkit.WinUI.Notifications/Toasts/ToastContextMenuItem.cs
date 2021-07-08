// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// A Toast context menu item.
    /// </summary>
    public sealed class ToastContextMenuItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToastContextMenuItem"/> class.
        /// A Toast context menu item with the required properties.
        /// </summary>
        /// <param name="content">The text to display on the menu item.</param>
        /// <param name="arguments">App-defined string of arguments that the app can later retrieve once it is activated when the user clicks the menu item.</param>
        public ToastContextMenuItem(string content, string arguments)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            Content = content;
            Arguments = arguments;
        }

        /// <summary>
        /// Gets the text to display on the menu item. Required
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// Gets app-defined string of arguments that the app can later retrieve once it is activated when the user clicks the menu item. Required
        /// </summary>
        public string Arguments { get; private set; }

        /// <summary>
        /// Gets or sets what type of activation this menu item will use when clicked. Defaults to Foreground.
        /// </summary>
        public ToastActivationType ActivationType { get; set; } = ToastActivationType.Foreground;

        /// <summary>
        /// Gets or sets additional options relating to activation of the toast context menu item. New in Creators Update
        /// </summary>
        public ToastActivationOptions ActivationOptions { get; set; }

        /// <summary>
        /// Gets or sets an identifier used in telemetry to identify your category of action. This should be something
        /// like "TurnOff" or "ManageSettings". In the upcoming toast telemetry dashboard in Dev Center, you will
        /// be able to view how frequently your actions are being clicked.
        /// </summary>
        public string HintActionId { get; set; }

        internal Element_ToastAction ConvertToElement()
        {
            var el = new Element_ToastAction
            {
                Content = Content,
                Arguments = Arguments,
                ActivationType = Element_Toast.ConvertActivationType(ActivationType),
                Placement = Element_ToastActionPlacement.ContextMenu,
                HintActionId = HintActionId
            };

            ActivationOptions?.PopulateElement(el);

            return el;
        }
    }
}