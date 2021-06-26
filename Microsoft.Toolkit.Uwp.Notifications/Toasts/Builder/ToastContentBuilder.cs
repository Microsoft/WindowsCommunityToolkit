// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Microsoft.Toolkit.Uwp.Notifications
{
    /// <summary>
    /// Builder class used to create <see cref="ToastContent"/>
    /// </summary>
    public partial class ToastContentBuilder
#if !WINRT
        : IToastActivateableBuilder<ToastContentBuilder>
#endif
    {
        private Dictionary<string, string> _genericArguments = new Dictionary<string, string>();

        private bool _customArgumentsUsedOnToastItself;

        /// <summary>
        /// Gets internal instance of <see cref="ToastContent"/>. This is equivalent to the call to <see cref="ToastContentBuilder.GetToastContent"/>.
        /// </summary>
        public ToastContent Content
        {
            get; private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToastContentBuilder"/> class.
        /// </summary>
        public ToastContentBuilder()
        {
            Content = new ToastContent();
        }

        /// <summary>
        /// Add custom time stamp on the toast to override the time display on the toast.
        /// </summary>
        /// <param name="dateTime">Custom Time to be displayed on the toast</param>
        /// <returns>The current instance of <see cref="ToastContentBuilder"/></returns>
        public ToastContentBuilder AddCustomTimeStamp(
#if WINRT
            DateTimeOffset dateTime)
#else
            DateTime dateTime)
#endif
        {
            Content.DisplayTimestamp = dateTime;

            return this;
        }

        /// <summary>
        /// Add a header to a toast.
        /// </summary>
        /// <param name="id">A developer-created identifier that uniquely identifies this header. If two notifications have the same header id, they will be displayed underneath the same header in Action Center.</param>
        /// <param name="title">A title for the header.</param>
        /// <param name="arguments">Developer-defined arguments that are returned to the app when the user clicks this header.</param>
        /// <returns>The current instance of <see cref="ToastContentBuilder"/></returns>
        /// <remarks>More info about toast header: https://docs.microsoft.com/en-us/windows/uwp/design/shell/tiles-and-notifications/toast-headers </remarks>
#if WINRT
        [Windows.Foundation.Metadata.DefaultOverload]
#endif
        public ToastContentBuilder AddHeader(string id, string title, ToastArguments arguments)
        {
            return AddHeader(id, title, arguments.ToString());
        }

        /// <summary>
        /// Add a header to a toast.
        /// </summary>
        /// <param name="id">A developer-created identifier that uniquely identifies this header. If two notifications have the same header id, they will be displayed underneath the same header in Action Center.</param>
        /// <param name="title">A title for the header.</param>
        /// <param name="arguments">A developer-defined string of arguments that is returned to the app when the user clicks this header.</param>
        /// <returns>The current instance of <see cref="ToastContentBuilder"/></returns>
        /// <remarks>More info about toast header: https://docs.microsoft.com/en-us/windows/uwp/design/shell/tiles-and-notifications/toast-headers </remarks>
        public ToastContentBuilder AddHeader(string id, string title, string arguments)
        {
            Content.Header = new ToastHeader(id, title, arguments);

            return this;
        }

        /// <summary>
        /// Adds a key (without value) to the activation arguments that will be returned when the toast notification or its buttons are clicked.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The current instance of <see cref="ToastContentBuilder"/></returns>
        public ToastContentBuilder AddArgument(string key)
        {
            return AddArgumentHelper(key, null);
        }

        /// <summary>
        /// Adds a key/value to the activation arguments that will be returned when the toast notification or its buttons are clicked.
        /// </summary>
        /// <param name="key">The key for this value.</param>
        /// <param name="value">The value itself.</param>
        /// <returns>The current instance of <see cref="ToastContentBuilder"/></returns>
#if WINRT
        [Windows.Foundation.Metadata.DefaultOverload]
        [return: System.Runtime.InteropServices.WindowsRuntime.ReturnValueName("toastContentBuilder")]
#endif
        public ToastContentBuilder AddArgument(string key, string value)
        {
            return AddArgumentHelper(key, value);
        }

        /// <summary>
        /// Adds a key/value to the activation arguments that will be returned when the toast notification or its buttons are clicked.
        /// </summary>
        /// <param name="key">The key for this value.</param>
        /// <param name="value">The value itself.</param>
        /// <returns>The current instance of <see cref="ToastContentBuilder"/></returns>
#if WINRT
        [return: System.Runtime.InteropServices.WindowsRuntime.ReturnValueName("toastContentBuilder")]
#endif
        public ToastContentBuilder AddArgument(string key, int value)
        {
            return AddArgumentHelper(key, value.ToString());
        }

        /// <summary>
        /// Adds a key/value to the activation arguments that will be returned when the toast notification or its buttons are clicked.
        /// </summary>
        /// <param name="key">The key for this value.</param>
        /// <param name="value">The value itself.</param>
        /// <returns>The current instance of <see cref="ToastContentBuilder"/></returns>
#if WINRT
        [return: System.Runtime.InteropServices.WindowsRuntime.ReturnValueName("toastContentBuilder")]
#endif
        public ToastContentBuilder AddArgument(string key, double value)
        {
            return AddArgumentHelper(key, value.ToString());
        }

        /// <summary>
        /// Adds a key/value to the activation arguments that will be returned when the toast notification or its buttons are clicked.
        /// </summary>
        /// <param name="key">The key for this value.</param>
        /// <param name="value">The value itself.</param>
        /// <returns>The current instance of <see cref="ToastContentBuilder"/></returns>
#if WINRT
        [return: System.Runtime.InteropServices.WindowsRuntime.ReturnValueName("toastContentBuilder")]
#endif
        public ToastContentBuilder AddArgument(string key, float value)
        {
            return AddArgumentHelper(key, value.ToString());
        }

        /// <summary>
        /// Adds a key/value to the activation arguments that will be returned when the toast notification or its buttons are clicked.
        /// </summary>
        /// <param name="key">The key for this value.</param>
        /// <param name="value">The value itself.</param>
        /// <returns>The current instance of <see cref="ToastContentBuilder"/></returns>
#if WINRT
        [return: System.Runtime.InteropServices.WindowsRuntime.ReturnValueName("toastContentBuilder")]
#endif
        public ToastContentBuilder AddArgument(string key, bool value)
        {
            return AddArgumentHelper(key, value ? "1" : "0"); // Encode as 1 or 0 to save string space
        }

#if !WINRT
        /// <summary>
        /// Adds a key/value to the activation arguments that will be returned when the toast notification or its buttons are clicked.
        /// </summary>
        /// <param name="key">The key for this value.</param>
        /// <param name="value">The value itself. Note that the enums are stored using their numeric value, so be aware that changing your enum number values might break existing activation of toasts currently in Action Center.</param>
        /// <returns>The current instance of <see cref="ToastContentBuilder"/></returns>
        public ToastContentBuilder AddArgument(string key, Enum value)
        {
            return AddArgumentHelper(key, ((int)(object)value).ToString());
        }
#endif

        private ToastContentBuilder AddArgumentHelper(string key, string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            bool alreadyExists = _genericArguments.ContainsKey(key);

            _genericArguments[key] = value;

            if (Content.ActivationType != ToastActivationType.Protocol && !_customArgumentsUsedOnToastItself)
            {
                Content.Launch = alreadyExists ? SerializeArgumentsHelper(_genericArguments) : AddArgumentHelper(Content.Launch, key, value);
            }

            if (Content.Actions is ToastActionsCustom actions)
            {
                foreach (var button in actions.Buttons)
                {
                    if (button is ToastButton b && b.CanAddArguments() && !b.ContainsArgument(key))
                    {
                        b.AddArgument(key, value);
                    }
                }
            }

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

            if (existing == null)
            {
                return pair;
            }
            else
            {
                return existing + ToastArguments.Separator + pair;
            }
        }

        /// <summary>
        /// Configures the toast notification to launch the specified url when the toast body is clicked.
        /// </summary>
        /// <param name="protocol">The protocol to launch.</param>
        /// <returns>The current instance of <see cref="ToastContentBuilder"/></returns>
        public ToastContentBuilder SetProtocolActivation(Uri protocol)
        {
            return SetProtocolActivation(protocol, default);
        }

        /// <summary>
        /// Configures the toast notification to launch the specified url when the toast body is clicked.
        /// </summary>
        /// <param name="protocol">The protocol to launch.</param>
        /// <param name="targetApplicationPfn">New in Creators Update: The target PFN, so that regardless of whether multiple apps are registered to handle the same protocol uri, your desired app will always be launched.</param>
        /// <returns>The current instance of <see cref="ToastContentBuilder"/></returns>
        public ToastContentBuilder SetProtocolActivation(Uri protocol, string targetApplicationPfn)
        {
            Content.Launch = protocol.ToString();
            Content.ActivationType = ToastActivationType.Protocol;

            if (targetApplicationPfn != null)
            {
                if (Content.ActivationOptions == null)
                {
                    Content.ActivationOptions = new ToastActivationOptions();
                }

                Content.ActivationOptions.ProtocolActivationTargetApplicationPfn = targetApplicationPfn;
            }

            return this;
        }

        /// <summary>
        /// Configures the toast notification to use background activation when the toast body is clicked.
        /// </summary>
        /// <returns>The current instance of <see cref="ToastContentBuilder"/></returns>
        public ToastContentBuilder SetBackgroundActivation()
        {
            Content.ActivationType = ToastActivationType.Background;
            return this;
        }

        /// <summary>
        /// Instead of this method, for foreground/background activation, it is suggested to use <see cref="AddArgument(string, string)"/> and optionally <see cref="SetBackgroundActivation"/>. For protocol activation, you should use <see cref="SetProtocolActivation(Uri)"/>. Add info that can be used by the application when the app was activated/launched by the toast.
        /// </summary>
        /// <param name="launchArgs">Custom app-defined launch arguments to be passed along on toast activation</param>
        /// <param name="activationType">Set the activation type that will be used when the user click on this toast</param>
        /// <returns>The current instance of <see cref="ToastContentBuilder"/></returns>
        public ToastContentBuilder AddToastActivationInfo(string launchArgs, ToastActivationType activationType)
        {
            Content.Launch = launchArgs;
            Content.ActivationType = activationType;
            _customArgumentsUsedOnToastItself = true;
            return this;
        }

        /// <summary>
        /// Sets the amount of time the Toast should display. You typically should use the
        /// Scenario attribute instead, which impacts how long a Toast stays on screen.
        /// </summary>
        /// <param name="duration">Duration of the toast</param>
        /// <returns>The current instance of <see cref="ToastContentBuilder"/></returns>
        public ToastContentBuilder SetToastDuration(ToastDuration duration)
        {
            Content.Duration = duration;
            return this;
        }

        /// <summary>
        ///  Sets the scenario, to make the Toast behave like an alarm, reminder, or more.
        /// </summary>
        /// <param name="scenario">Scenario to be used for the toast's behavior</param>
        /// <returns>The current instance of <see cref="ToastContentBuilder"/></returns>
        public ToastContentBuilder SetToastScenario(ToastScenario scenario)
        {
            Content.Scenario = scenario;
            return this;
        }

#if WINRT
        /// <summary>
        /// Set custom audio to go along with the toast.
        /// </summary>
        /// <param name="src">Source to the media that will be played when the toast is pop</param>
        /// <returns>The current instance of <see cref="ToastContentBuilder"/></returns>
        [Windows.Foundation.Metadata.DefaultOverload]
        public ToastContentBuilder AddAudio(Uri src)
        {
            return AddAudio(src, default, default);
        }

        /// <summary>
        /// Set custom audio to go along with the toast.
        /// </summary>
        /// <param name="src">Source to the media that will be played when the toast is pop</param>
        /// <param name="loop">Indicating whether sound should repeat as long as the Toast is shown; false to play only once (default).</param>
        /// <returns>The current instance of <see cref="ToastContentBuilder"/></returns>
        public ToastContentBuilder AddAudio(Uri src, bool? loop)
        {
            return AddAudio(src, loop, default);
        }
#endif

        /// <summary>
        /// Set custom audio to go along with the toast.
        /// </summary>
        /// <param name="src">Source to the media that will be played when the toast is pop</param>
        /// <param name="loop">Indicating whether sound should repeat as long as the Toast is shown; false to play only once (default).</param>
        /// <param name="silent">Indicating whether sound is muted; false to allow the Toast notification sound to play (default).</param>
        /// <returns>The current instance of <see cref="ToastContentBuilder"/></returns>
        public ToastContentBuilder AddAudio(
            Uri src,
#if WINRT
            bool? loop,
            bool? silent)
#else
            bool? loop = default,
            bool? silent = default)
#endif
        {
            var audio = new ToastAudio();
            audio.Src = src;

            if (loop != default)
            {
                audio.Loop = loop.Value;
            }

            if (silent != default)
            {
                audio.Silent = silent.Value;
            }

            return AddAudio(audio);
        }

        /// <summary>
        /// Set custom audio to go along with the toast.
        /// </summary>
        /// <param name="audio">The <see cref="ToastAudio"/> to set.</param>
        /// <returns>The current instance of <see cref="ToastContentBuilder"/></returns>
        public ToastContentBuilder AddAudio(ToastAudio audio)
        {
            if (audio.Src != null && !audio.Src.IsFile && audio.Src.Scheme != "ms-appx" && audio.Src.Scheme != "ms-winsoundevent")
            {
                throw new InvalidOperationException("Audio Source must either be a ms-appx file, absolute file, or ms-winsoundevent.");
            }

            Content.Audio = audio;
            return this;
        }

        /// <summary>
        /// Get the instance of <see cref="ToastContent"/> that has been built by the builder with specified configuration so far.
        /// </summary>
        /// <returns>An instance of <see cref="ToastContent"/> that can be used to create tile notification.</returns>
        public ToastContent GetToastContent()
        {
            return Content;
        }

#if WINDOWS_UWP
        /// <summary>
        /// Retrieves the notification XML content as a WinRT XmlDocument, so that it can be used with a local Toast notification's constructor on either <see cref="Windows.UI.Notifications.ToastNotification"/> or <see cref="Windows.UI.Notifications.ScheduledToastNotification"/>.
        /// </summary>
        /// <returns>The notification XML content as a WinRT XmlDocument.</returns>
        public Windows.Data.Xml.Dom.XmlDocument GetXml()
        {
            return GetToastContent().GetXml();
        }

        /// <summary>
        /// Shows a new toast notification with the current content.
        /// </summary>
        public void Show()
        {
            CustomizeToast customize = null;
            Show(customize);
        }

        /// <summary>
        /// Shows a new toast notification with the current content.
        /// </summary>
        /// <param name="customize">Allows you to set additional properties on the <see cref="Windows.UI.Notifications.ToastNotification"/> object.</param>
#if WINRT
        [Windows.Foundation.Metadata.DefaultOverload]
#endif
        public void Show(CustomizeToast customize)
        {
            var notif = new Windows.UI.Notifications.ToastNotification(GetToastContent().GetXml());
            customize?.Invoke(notif);

            ToastNotificationManagerCompat.CreateToastNotifier().Show(notif);
        }

        /// <summary>
        /// Shows a new toast notification with the current content.
        /// </summary>
        /// <param name="customize">Allows you to set additional properties on the <see cref="Windows.UI.Notifications.ToastNotification"/> object.</param>
        /// <returns>An operation that completes after your async customizations have completed.</returns>
        public Windows.Foundation.IAsyncAction Show(CustomizeToastAsync customize)
        {
            return ShowAsyncHelper(customize).AsAsyncAction();
        }

        private async System.Threading.Tasks.Task ShowAsyncHelper(CustomizeToastAsync customize)
        {
            var notif = new Windows.UI.Notifications.ToastNotification(GetToastContent().GetXml());

            if (customize != null)
            {
                await customize.Invoke(notif);
            }

            ToastNotificationManagerCompat.CreateToastNotifier().Show(notif);
        }

        /// <summary>
        /// Schedules the notification.
        /// </summary>
        /// <param name="deliveryTime">The date and time that Windows should display the toast notification. This time must be in the future.</param>
        public void Schedule(DateTimeOffset deliveryTime)
        {
            CustomizeScheduledToast customize = null;
            Schedule(deliveryTime, customize);
        }

        /// <summary>
        /// Schedules the notification.
        /// </summary>
        /// <param name="deliveryTime">The date and time that Windows should display the toast notification. This time must be in the future.</param>
        /// <param name="customize">Allows you to set additional properties on the <see cref="Windows.UI.Notifications.ScheduledToastNotification"/> object.</param>
#if WINRT
        [Windows.Foundation.Metadata.DefaultOverload]
#endif
        public void Schedule(DateTimeOffset deliveryTime, CustomizeScheduledToast customize)
        {
            var notif = new Windows.UI.Notifications.ScheduledToastNotification(GetToastContent().GetXml(), deliveryTime);
            customize?.Invoke(notif);

            ToastNotificationManagerCompat.CreateToastNotifier().AddToSchedule(notif);
        }

        /// <summary>
        /// Schedules the notification.
        /// </summary>
        /// <param name="deliveryTime">The date and time that Windows should display the toast notification. This time must be in the future.</param>
        /// <param name="customize">Allows you to set additional properties on the <see cref="Windows.UI.Notifications.ScheduledToastNotification"/> object.</param>
        /// <returns>An operation that completes after your async customizations have completed.</returns>
        public Windows.Foundation.IAsyncAction Schedule(DateTimeOffset deliveryTime, CustomizeScheduledToastAsync customize)
        {
            return ScheduleAsyncHelper(deliveryTime, customize).AsAsyncAction();
        }

        private async System.Threading.Tasks.Task ScheduleAsyncHelper(DateTimeOffset deliveryTime, CustomizeScheduledToastAsync customize = null)
        {
            var notif = new Windows.UI.Notifications.ScheduledToastNotification(GetToastContent().GetXml(), deliveryTime);

            if (customize != null)
            {
                await customize.Invoke(notif);
            }

            ToastNotificationManagerCompat.CreateToastNotifier().AddToSchedule(notif);
        }
#endif
    }
}