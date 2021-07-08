// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WIN32

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Windows.UI.Notifications;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// Apps must implement this activator to handle notification activation.
    /// </summary>
    [Obsolete("You can now subscribe to activation by simpy using the ToastNotificationManagerCompat.OnActivated event. We recommend deleting your NotificationActivator and switching to using the event.")]
    public abstract class NotificationActivator : NotificationActivator.INotificationActivationCallback
    {
        /// <inheritdoc/>
        public void Activate(string appUserModelId, string invokedArgs, NOTIFICATION_USER_INPUT_DATA[] data, uint dataCount)
        {
            OnActivated(invokedArgs, new NotificationUserInput(data), appUserModelId);
        }

        /// <summary>
        /// This method will be called when the user clicks on a foreground or background activation on a toast. Parent app must implement this method.
        /// </summary>
        /// <param name="arguments">The arguments from the original notification. This is either the launch argument if the user clicked the body of your toast, or the arguments from a button on your toast.</param>
        /// <param name="userInput">Text and selection values that the user entered in your toast.</param>
        /// <param name="appUserModelId">Your AUMID.</param>
        public abstract void OnActivated(string arguments, NotificationUserInput userInput, string appUserModelId);

        /// <summary>
        /// A single user input key/value pair.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        [Serializable]
        public struct NOTIFICATION_USER_INPUT_DATA
        {
            /// <summary>
            /// The key of the user input.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string Key;

            /// <summary>
            /// The value of the user input.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string Value;
        }

        /// <summary>
        /// The COM callback that is triggered when your notification is clicked.
        /// </summary>
        [ComImport]
        [Guid("53E31837-6600-4A81-9395-75CFFE746F94")]
        [ComVisible(true)]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface INotificationActivationCallback
        {
            /// <summary>
            /// The method called when your notification is clicked.
            /// </summary>
            /// <param name="appUserModelId">The app id of the app that sent the toast.</param>
            /// <param name="invokedArgs">The activation arguments from the toast.</param>
            /// <param name="data">The user input from the toast.</param>
            /// <param name="dataCount">The number of user inputs.</param>
            void Activate(
                [In, MarshalAs(UnmanagedType.LPWStr)]
                string appUserModelId,
                [In, MarshalAs(UnmanagedType.LPWStr)]
                string invokedArgs, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] NOTIFICATION_USER_INPUT_DATA[] data,
                [In, MarshalAs(UnmanagedType.U4)]
                uint dataCount);
        }
    }
}

#endif