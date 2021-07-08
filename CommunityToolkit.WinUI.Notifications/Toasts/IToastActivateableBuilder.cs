// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// Interfaces for classes that can have activation info added to them.
    /// </summary>
    /// <typeparam name="T">The type of the host object.</typeparam>
    internal interface IToastActivateableBuilder<T>
    {
        /// <summary>
        /// Adds a key (without value) to the activation arguments that will be returned when the content is clicked.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The current instance of the object.</returns>
        T AddArgument(string key);

        /// <summary>
        /// Adds a key/value to the activation arguments that will be returned when the content is clicked.
        /// </summary>
        /// <param name="key">The key for this value.</param>
        /// <param name="value">The value itself.</param>
        /// <returns>The current instance of the object.</returns>
#if WINRT
        [Windows.Foundation.Metadata.DefaultOverload]
#endif
        T AddArgument(string key, string value);

        /// <summary>
        /// Adds a key/value to the activation arguments that will be returned when the content is clicked.
        /// </summary>
        /// <param name="key">The key for this value.</param>
        /// <param name="value">The value itself.</param>
        /// <returns>The current instance of the object.</returns>
        T AddArgument(string key, int value);

        /// <summary>
        /// Adds a key/value to the activation arguments that will be returned when the content is clicked.
        /// </summary>
        /// <param name="key">The key for this value.</param>
        /// <param name="value">The value itself.</param>
        /// <returns>The current instance of the object.</returns>
        T AddArgument(string key, double value);

        /// <summary>
        /// Adds a key/value to the activation arguments that will be returned when the content is clicked.
        /// </summary>
        /// <param name="key">The key for this value.</param>
        /// <param name="value">The value itself.</param>
        /// <returns>The current instance of the object.</returns>
        T AddArgument(string key, float value);

        /// <summary>
        /// Adds a key/value to the activation arguments that will be returned when the content is clicked.
        /// </summary>
        /// <param name="key">The key for this value.</param>
        /// <param name="value">The value itself.</param>
        /// <returns>The current instance of the object.</returns>
        T AddArgument(string key, bool value);

#if !WINRT
        /// <summary>
        /// Adds a key/value to the activation arguments that will be returned when the content is clicked.
        /// </summary>
        /// <param name="key">The key for this value.</param>
        /// <param name="value">The value itself. Note that the enums are stored using their numeric value, so be aware that changing your enum number values might break existing activation of toasts currently in Action Center.</param>
        /// <returns>The current instance of the object.</returns>
        T AddArgument(string key, Enum value);
#endif

        /// <summary>
        /// Configures the content to use background activation when it is clicked.
        /// </summary>
        /// <returns>The current instance of the object.</returns>
        T SetBackgroundActivation();

        /// <summary>
        /// Configures the content to use protocol activation when it is clicked.
        /// </summary>
        /// <param name="protocol">The protocol to launch.</param>
        /// <returns>The current instance of the object.</returns>
        T SetProtocolActivation(Uri protocol);

        /// <summary>
        /// Configures the content to use protocol activation when it is clicked.
        /// </summary>
        /// <param name="protocol">The protocol to launch.</param>
        /// <param name="targetApplicationPfn">New in Creators Update: The target PFN, so that regardless of whether multiple apps are registered to handle the same protocol uri, your desired app will always be launched.</param>
        /// <returns>The current instance of the object.</returns>
        T SetProtocolActivation(Uri protocol, string targetApplicationPfn);
    }
}