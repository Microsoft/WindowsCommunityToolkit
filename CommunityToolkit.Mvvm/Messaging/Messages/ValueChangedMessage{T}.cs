// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Mvvm.Messaging.Messages
{
    /// <summary>
    /// A base message that signals whenever a specific value has changed.
    /// </summary>
    /// <typeparam name="T">The type of value that has changed.</typeparam>
    public class ValueChangedMessage<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueChangedMessage{T}"/> class.
        /// </summary>
        /// <param name="value">The value that has changed.</param>
        public ValueChangedMessage(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the value that has changed.
        /// </summary>
        public T Value { get; }
    }
}