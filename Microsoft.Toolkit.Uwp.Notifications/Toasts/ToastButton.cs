// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Microsoft.Toolkit.Uwp.Notifications
{
    /// <summary>
    /// A button that the user can click on a Toast notification.
    /// </summary>
    public sealed class ToastButton :
#if !WINRT
        IToastActivateableBuilder<ToastButton>,
#endif
        IToastButton
    {
        private Dictionary<string, string> _arguments = new Dictionary<string, string>();

        private bool _usingCustomArguments;

        private bool _usingSnoozeActivation;
        private string _snoozeSelectionBoxId;

        private bool _usingDismissActivation;

        internal bool NeedsContent()
        {
            // Snooze/dismiss buttons don't need content (the system will auto-add the localized strings).
            return !_usingDismissActivation && !_usingSnoozeActivation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToastButton"/> class.
        /// </summary>
        /// <param name="content">The text to display on the button.</param>
        /// <param name="arguments">App-defined string of arguments that the app can later retrieve once it is activated when the user clicks the button.</param>
        public ToastButton(string content, string arguments)
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

            _usingCustomArguments = arguments.Length > 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToastButton"/> class.
        /// </summary>
        public ToastButton()
        {
            // Arguments are required (we'll initialize to empty string which is fine).
            Arguments = string.Empty;
        }

        /// <summary>
        /// Gets the text to display on the button. Required
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// Gets app-defined string of arguments that the app can later retrieve once it is
        /// activated when the user clicks the button. Required
        /// </summary>
        public string Arguments { get; internal set; }

        /// <summary>
        /// Gets or sets what type of activation this button will use when clicked. Defaults to Foreground.
        /// </summary>
        public ToastActivationType ActivationType { get; set; } = ToastActivationType.Foreground;

        /// <summary>
        /// Gets or sets additional options relating to activation of the toast button. New in Creators Update
        /// </summary>
        public ToastActivationOptions ActivationOptions { get; set; }

        /// <summary>
        /// Gets or sets an optional image icon for the button to display (required for buttons adjacent to inputs like quick reply).
        /// </summary>
        public string ImageUri { get; set; }

        /// <summary>
        /// Gets or sets the ID of an existing <see cref="ToastTextBox"/> in order to have this button display
        /// to the right of the input, achieving a quick reply scenario.
        /// </summary>
        public string TextBoxId { get; set; }

        /// <summary>
        /// Gets or sets an identifier used in telemetry to identify your category of action. This should be something
        /// like "Delete", "Reply", or "Archive". In the upcoming toast telemetry dashboard in Dev Center, you will
        /// be able to view how frequently your actions are being clicked.
        /// </summary>
        public string HintActionId { get; set; }

        /// <summary>
        /// Sets the text to display on the button.
        /// </summary>
        /// <param name="content">The text to display on the button.</param>
        /// <returns>The current instance of the <see cref="ToastButton"/>.</returns>
        public ToastButton SetContent(string content)
        {
            Content = content;
            return this;
        }

        /// <summary>
        /// Adds a key (without value) to the activation arguments that will be returned when the toast notification or its buttons are clicked.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The current instance of <see cref="ToastButton"/></returns>
        public ToastButton AddArgument(string key)
        {
            return AddArgumentHelper(key, null);
        }

        /// <summary>
        /// Adds a key/value to the activation arguments that will be returned when the toast notification or its buttons are clicked.
        /// </summary>
        /// <param name="key">The key for this value.</param>
        /// <param name="value">The value itself.</param>
        /// <returns>The current instance of <see cref="ToastButton"/></returns>
#if WINRT
        [Windows.Foundation.Metadata.DefaultOverload]
        [return: System.Runtime.InteropServices.WindowsRuntime.ReturnValueName("ToastButton")]
#endif
        public ToastButton AddArgument(string key, string value)
        {
            return AddArgumentHelper(key, value);
        }

        /// <summary>
        /// Adds a key/value to the activation arguments that will be returned when the toast notification or its buttons are clicked.
        /// </summary>
        /// <param name="key">The key for this value.</param>
        /// <param name="value">The value itself.</param>
        /// <returns>The current instance of <see cref="ToastButton"/></returns>
#if WINRT
        [return: System.Runtime.InteropServices.WindowsRuntime.ReturnValueName("ToastButton")]
#endif
        public ToastButton AddArgument(string key, int value)
        {
            return AddArgumentHelper(key, value.ToString());
        }

        /// <summary>
        /// Adds a key/value to the activation arguments that will be returned when the toast notification or its buttons are clicked.
        /// </summary>
        /// <param name="key">The key for this value.</param>
        /// <param name="value">The value itself.</param>
        /// <returns>The current instance of <see cref="ToastButton"/></returns>
#if WINRT
        [return: System.Runtime.InteropServices.WindowsRuntime.ReturnValueName("ToastButton")]
#endif
        public ToastButton AddArgument(string key, double value)
        {
            return AddArgumentHelper(key, value.ToString());
        }

        /// <summary>
        /// Adds a key/value to the activation arguments that will be returned when the toast notification or its buttons are clicked.
        /// </summary>
        /// <param name="key">The key for this value.</param>
        /// <param name="value">The value itself.</param>
        /// <returns>The current instance of <see cref="ToastButton"/></returns>
#if WINRT
        [return: System.Runtime.InteropServices.WindowsRuntime.ReturnValueName("ToastButton")]
#endif
        public ToastButton AddArgument(string key, float value)
        {
            return AddArgumentHelper(key, value.ToString());
        }

        /// <summary>
        /// Adds a key/value to the activation arguments that will be returned when the toast notification or its buttons are clicked.
        /// </summary>
        /// <param name="key">The key for this value.</param>
        /// <param name="value">The value itself.</param>
        /// <returns>The current instance of <see cref="ToastButton"/></returns>
#if WINRT
        [return: System.Runtime.InteropServices.WindowsRuntime.ReturnValueName("ToastButton")]
#endif
        public ToastButton AddArgument(string key, bool value)
        {
            return AddArgumentHelper(key, value ? "1" : "0"); // Encode as 1 or 0 to save string space
        }

#if !WINRT
        /// <summary>
        /// Adds a key/value to the activation arguments that will be returned when the toast notification or its buttons are clicked.
        /// </summary>
        /// <param name="key">The key for this value.</param>
        /// <param name="value">The value itself. Note that the enums are stored using their numeric value, so be aware that changing your enum number values might break existing activation of toasts currently in Action Center.</param>
        /// <returns>The current instance of <see cref="ToastButton"/></returns>
        public ToastButton AddArgument(string key, Enum value)
        {
            return AddArgumentHelper(key, ((int)(object)value).ToString());
        }
#endif

        private ToastButton AddArgumentHelper(string key, string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (_usingCustomArguments)
            {
                throw new InvalidOperationException("You cannot use the AddArgument methods if you've set the Arguments property. Use the default ToastButton constructor instead.");
            }

            if (ActivationType == ToastActivationType.Protocol)
            {
                throw new InvalidOperationException("You cannot use the AddArgument methods when using protocol activation.");
            }

            if (_usingDismissActivation || _usingSnoozeActivation)
            {
                throw new InvalidOperationException("You cannot use the AddArgument methods when using dismiss or snooze activation.");
            }

            bool alreadyExists = _arguments.ContainsKey(key);

            _arguments[key] = value;

            Arguments = alreadyExists ? SerializeArgumentsHelper(_arguments) : AddArgumentHelper(Arguments, key, value);

            return this;
        }

        private string SerializeArgumentsHelper(IDictionary<string, string> arguments)
        {
            var args = new ToastArguments();

            foreach (var a in arguments)
            {
                args.Add(a.Key, a.Value);
            }

            return args.ToString();
        }

        private string AddArgumentHelper(string existing, string key, string value)
        {
            string pair = ToastArguments.EncodePair(key, value);

            if (string.IsNullOrEmpty(existing))
            {
                return pair;
            }
            else
            {
                return existing + ToastArguments.Separator + pair;
            }
        }

        /// <summary>
        /// Configures the button to launch the specified url when the button is clicked.
        /// </summary>
        /// <param name="protocol">The protocol to launch.</param>
        /// <returns>The current instance of <see cref="ToastButton"/></returns>
        public ToastButton SetProtocolActivation(Uri protocol)
        {
            return SetProtocolActivation(protocol, default);
        }

        /// <summary>
        /// Configures the button to launch the specified url when the button is clicked.
        /// </summary>
        /// <param name="protocol">The protocol to launch.</param>
        /// <param name="targetApplicationPfn">New in Creators Update: The target PFN, so that regardless of whether multiple apps are registered to handle the same protocol uri, your desired app will always be launched.</param>
        /// <returns>The current instance of <see cref="ToastButton"/></returns>
        public ToastButton SetProtocolActivation(Uri protocol, string targetApplicationPfn)
        {
            if (_arguments.Count > 0)
            {
                throw new InvalidOperationException("SetProtocolActivation cannot be used in conjunction with AddArgument");
            }

            Arguments = protocol.ToString();
            ActivationType = ToastActivationType.Protocol;

            if (targetApplicationPfn != null)
            {
                if (ActivationOptions == null)
                {
                    ActivationOptions = new ToastActivationOptions();
                }

                ActivationOptions.ProtocolActivationTargetApplicationPfn = targetApplicationPfn;
            }

            return this;
        }

        /// <summary>
        /// Configures the button to use background activation when the button is clicked.
        /// </summary>
        /// <returns>The current instance of <see cref="ToastButton"/></returns>
        public ToastButton SetBackgroundActivation()
        {
            ActivationType = ToastActivationType.Background;
            return this;
        }

        /// <summary>
        /// Sets the behavior that the toast should use when the user invokes this button. Desktop-only, supported in builds 16251 or higher. New in Fall Creators Update.
        /// </summary>
        /// <param name="afterActivationBehavior">The behavior that the toast should use when the user invokes this button.</param>
        /// <returns>The current instance of <see cref="ToastButton"/></returns>
        public ToastButton SetAfterActivationBehavior(ToastAfterActivationBehavior afterActivationBehavior)
        {
            if (ActivationOptions == null)
            {
                ActivationOptions = new ToastActivationOptions();
            }

            ActivationOptions.AfterActivationBehavior = afterActivationBehavior;

            return this;
        }

        /// <summary>
        /// Configures the button to use system snooze activation when the button is clicked, using the default system snooze time.
        /// </summary>
        /// <returns>The current instance of <see cref="ToastButton"/></returns>
        public ToastButton SetSnoozeActivation()
        {
            return SetSnoozeActivation(null);
        }

        /// <summary>
        /// Configures the button to use system snooze activation when the button is clicked, with a snooze time defined by the specified selection box.
        /// </summary>
        /// <param name="selectionBoxId">The ID of an existing <see cref="ToastSelectionBox"/> which allows the user to pick a custom snooze time. The ID's of the <see cref="ToastSelectionBoxItem"/>s inside the selection box must represent the snooze interval in minutes. For example, if the user selects an item that has an ID of "120", then the notification will be snoozed for 2 hours. When the user clicks this button, if you specified a SelectionBoxId, the system will parse the ID of the selected item and snooze by that amount of minutes.</param>
        /// <returns>The current instance of <see cref="ToastButton"/></returns>
        public ToastButton SetSnoozeActivation(string selectionBoxId)
        {
            if (_arguments.Count > 0)
            {
                throw new InvalidOperationException($"{nameof(SetSnoozeActivation)} cannot be used in conjunction with ${nameof(AddArgument)}.");
            }

            _usingSnoozeActivation = true;
            _snoozeSelectionBoxId = selectionBoxId;

            return this;
        }

        /// <summary>
        /// Configures the button to use system dismiss activation when the button is clicked (the toast will simply dismiss rather than activating).
        /// </summary>
        /// <returns>The current instance of <see cref="ToastButton"/></returns>
        public ToastButton SetDismissActivation()
        {
            if (_arguments.Count > 0)
            {
                throw new InvalidOperationException($"{nameof(SetDismissActivation)} cannot be used in conjunction with ${nameof(AddArgument)}.");
            }

            _usingDismissActivation = true;
            return this;
        }

        /// <summary>
        /// Sets an identifier used in telemetry to identify your category of action. This should be something like "Delete", "Reply", or "Archive". In the upcoming toast telemetry dashboard in Dev Center, you will be able to view how frequently your actions are being clicked.
        /// </summary>
        /// <param name="actionId">An identifier used in telemetry to identify your category of action.</param>
        /// <returns>The current instance of <see cref="ToastButton"/></returns>
        public ToastButton SetHintActionId(string actionId)
        {
            HintActionId = actionId;
            return this;
        }

        /// <summary>
        /// Sets an optional image icon for the button to display (required for buttons adjacent to inputs like quick reply).
        /// </summary>
        /// <param name="imageUri">An optional image icon for the button to display.</param>
        /// <returns>The current instance of <see cref="ToastButton"/></returns>
        public ToastButton SetImageUri(Uri imageUri)
        {
            ImageUri = imageUri.ToString();
            return this;
        }

        /// <summary>
        /// Sets the ID of an existing <see cref="ToastTextBox"/> in order to have this button display to the right of the input, achieving a quick reply scenario.
        /// </summary>
        /// <param name="textBoxId">The ID of an existing <see cref="ToastTextBox"/>.</param>
        /// <returns>The current instance of <see cref="ToastButton"/></returns>
        public ToastButton SetTextBoxId(string textBoxId)
        {
            TextBoxId = textBoxId;
            return this;
        }

        internal bool CanAddArguments()
        {
            return ActivationType != ToastActivationType.Protocol && !_usingCustomArguments && !_usingDismissActivation && !_usingSnoozeActivation;
        }

        internal bool ContainsArgument(string key)
        {
            return _arguments.ContainsKey(key);
        }

        internal Element_ToastAction ConvertToElement()
        {
            var el = new Element_ToastAction()
            {
                Content = Content,
                ImageUri = ImageUri,
                InputId = TextBoxId,
                HintActionId = HintActionId
            };

            if (_usingSnoozeActivation)
            {
                el.ActivationType = Element_ToastActivationType.System;
                el.Arguments = "snooze";

                if (_snoozeSelectionBoxId != null)
                {
                    el.InputId = _snoozeSelectionBoxId;
                }

                // Content needs to be specified as empty for auto-generated Snooze content
                if (el.Content == null)
                {
                    el.Content = string.Empty;
                }
            }
            else if (_usingDismissActivation)
            {
                el.ActivationType = Element_ToastActivationType.System;
                el.Arguments = "dismiss";

                // Content needs to be specified as empty for auto-generated Dismiss content
                if (el.Content == null)
                {
                    el.Content = string.Empty;
                }
            }
            else
            {
                el.ActivationType = Element_Toast.ConvertActivationType(ActivationType);
                el.Arguments = Arguments;
            }

            ActivationOptions?.PopulateElement(el);

            return el;
        }
    }
}