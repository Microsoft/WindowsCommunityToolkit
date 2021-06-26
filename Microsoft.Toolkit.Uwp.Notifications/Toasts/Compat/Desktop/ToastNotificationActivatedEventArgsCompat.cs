// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WIN32

using Windows.Foundation.Collections;

namespace Microsoft.Toolkit.Uwp.Notifications
{
    /// <summary>
    /// Provides information about an event that occurs when the app is activated because a user tapped on the body of a toast notification or performed an action inside a toast notification.
    /// </summary>
    public class ToastNotificationActivatedEventArgsCompat
    {
        internal ToastNotificationActivatedEventArgsCompat()
        {
        }

        /// <summary>
        /// Gets the arguments from the toast XML payload related to the action that was invoked on the toast.
        /// </summary>
        public string Argument { get; internal set; }

        /// <summary>
        /// Gets a set of values that you can use to obtain the user input from an interactive toast notification.
        /// </summary>
        public ValueSet UserInput { get; internal set; }
    }
}

#endif