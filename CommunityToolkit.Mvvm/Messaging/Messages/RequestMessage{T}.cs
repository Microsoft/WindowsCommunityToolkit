// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

#pragma warning disable CS8618

namespace CommunityToolkit.Mvvm.Messaging.Messages
{
    /// <summary>
    /// A <see langword="class"/> for request messages, which can either be used directly or through derived classes.
    /// </summary>
    /// <typeparam name="T">The type of request to make.</typeparam>
    public class RequestMessage<T>
    {
        private T response;

        /// <summary>
        /// Gets the message response.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="HasReceivedResponse"/> is <see langword="false"/>.</exception>
        public T Response
        {
            get
            {
                if (!HasReceivedResponse)
                {
                    ThrowInvalidOperationExceptionForNoResponseReceived();
                }

                return this.response;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a response has already been assigned to this instance.
        /// </summary>
        public bool HasReceivedResponse { get; private set; }

        /// <summary>
        /// Replies to the current request message.
        /// </summary>
        /// <param name="response">The response to use to reply to the request message.</param>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="Response"/> has already been set.</exception>
        public void Reply(T response)
        {
            if (HasReceivedResponse)
            {
                ThrowInvalidOperationExceptionForDuplicateReply();
            }

            HasReceivedResponse = true;

            this.response = response;
        }

        /// <summary>
        /// Implicitly gets the response from a given <see cref="RequestMessage{T}"/> instance.
        /// </summary>
        /// <param name="message">The input <see cref="RequestMessage{T}"/> instance.</param>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="HasReceivedResponse"/> is <see langword="false"/>.</exception>
        public static implicit operator T(RequestMessage<T> message)
        {
            return message.Response;
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> when a response is not available.
        /// </summary>
        private static void ThrowInvalidOperationExceptionForNoResponseReceived()
        {
            throw new InvalidOperationException("No response was received for the given request message");
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> when <see cref="Reply"/> is called twice.
        /// </summary>
        private static void ThrowInvalidOperationExceptionForDuplicateReply()
        {
            throw new InvalidOperationException("A response has already been issued for the current message");
        }
    }
}