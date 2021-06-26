// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Mvvm.Messaging
{
    /// <summary>
    /// An interface for a recipient that declares a registration for a specific message type.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to receive.</typeparam>
    public interface IRecipient<in TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Receives a given <typeparamref name="TMessage"/> message instance.
        /// </summary>
        /// <param name="message">The message being received.</param>
        void Receive(TMessage message);
    }
}