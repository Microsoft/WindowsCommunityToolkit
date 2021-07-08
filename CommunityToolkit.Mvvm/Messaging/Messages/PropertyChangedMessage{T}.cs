// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable SA1512

// This file is inspired from the MvvmLight library (lbugnion/MvvmLight),
// more info in ThirdPartyNotices.txt in the root of the project.

namespace CommunityToolkit.Mvvm.Messaging.Messages
{
    /// <summary>
    /// A message used to broadcast property changes in observable objects.
    /// </summary>
    /// <typeparam name="T">The type of the property to broadcast the change for.</typeparam>
    public class PropertyChangedMessage<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedMessage{T}"/> class.
        /// </summary>
        /// <param name="sender">The original sender of the broadcast message.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <param name="oldValue">The value that the property had before the change.</param>
        /// <param name="newValue">The value that the property has after the change.</param>
        public PropertyChangedMessage(object sender, string? propertyName, T oldValue, T newValue)
        {
            Sender = sender;
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// Gets the original sender of the broadcast message.
        /// </summary>
        public object Sender { get; }

        /// <summary>
        /// Gets the name of the property that changed.
        /// </summary>
        public string? PropertyName { get; }

        /// <summary>
        /// Gets the value that the property had before the change.
        /// </summary>
        public T OldValue { get; }

        /// <summary>
        /// Gets the value that the property has after the change.
        /// </summary>
        public T NewValue { get; }
    }
}