// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;

namespace CommunityToolkit.Mvvm.Messaging
{
    /// <summary>
    /// A <see langword="delegate"/> used to represent actions to invoke when a message is received.
    /// The recipient is given as an input argument to allow message registrations to avoid creating
    /// closures: if an instance method on a recipient needs to be invoked it is possible to just
    /// cast the recipient to the right type and then access the local method from that instance.
    /// </summary>
    /// <typeparam name="TRecipient">The type of recipient for the message.</typeparam>
    /// <typeparam name="TMessage">The type of message to receive.</typeparam>
    /// <param name="recipient">The recipient that is receiving the message.</param>
    /// <param name="message">The message being received.</param>
    public delegate void MessageHandler<in TRecipient, in TMessage>(TRecipient recipient, TMessage message)
        where TRecipient : class
        where TMessage : class;

    /// <summary>
    /// An interface for a type providing the ability to exchange messages between different objects.
    /// This can be useful to decouple different modules of an application without having to keep strong
    /// references to types being referenced. It is also possible to send messages to specific channels, uniquely
    /// identified by a token, and to have different messengers in different sections of an applications.
    /// In order to use the <see cref="IMessenger"/> functionalities, first define a message type, like so:
    /// <code>
    /// public sealed class LoginCompletedMessage { }
    /// </code>
    /// Then, register your a recipient for this message:
    /// <code>
    /// Messenger.Default.Register&lt;MyRecipientType, LoginCompletedMessage&gt;(this, (r, m) =>
    /// {
    ///     // Handle the message here...
    /// });
    /// </code>
    /// The message handler here is a lambda expression taking two parameters: the recipient and the message.
    /// This is done to avoid the allocations for the closures that would've been generated if the expression
    /// had captured the current instance. The recipient type parameter is used so that the recipient can be
    /// directly accessed within the handler without the need to manually perform type casts. This allows the
    /// code to be less verbose and more reliable, as all the checks are done just at build time. If the handler
    /// is defined within the same type as the recipient, it is also possible to directly access private members.
    /// This allows the message handler to be a static method, which enables the C# compiler to perform a number
    /// of additional memory optimizations (such as caching the delegate, avoiding unnecessary memory allocations).
    /// Finally, send a message when needed, like so:
    /// <code>
    /// Messenger.Default.Send&lt;LoginCompletedMessage&gt;();
    /// </code>
    /// Additionally, the method group syntax can also be used to specify the message handler
    /// to invoke when receiving a message, if a method with the right signature is available
    /// in the current scope. This is helpful to keep the registration and handling logic separate.
    /// Following up from the previous example, consider a class having this method:
    /// <code>
    /// private static void Receive(MyRecipientType recipient, LoginCompletedMessage message)
    /// {
    ///     // Handle the message there
    /// }
    /// </code>
    /// The registration can then be performed in a single line like so:
    /// <code>
    /// Messenger.Default.Register(this, Receive);
    /// </code>
    /// The C# compiler will automatically convert that expression to a <see cref="MessageHandler{TRecipient,TMessage}"/> instance
    /// compatible with <see cref="IMessengerExtensions.Register{TRecipient,TMessage}(IMessenger,TRecipient,MessageHandler{TRecipient,TMessage})"/>.
    /// This will also work if multiple overloads of that method are available, each handling a different
    /// message type: the C# compiler will automatically pick the right one for the current message type.
    /// It is also possible to register message handlers explicitly using the <see cref="IRecipient{TMessage}"/> interface.
    /// To do so, the recipient just needs to implement the interface and then call the
    /// <see cref="IMessengerExtensions.RegisterAll(IMessenger,object)"/> extension, which will automatically register
    /// all the handlers that are declared by the recipient type. Registration for individual handlers is supported as well.
    /// </summary>
    public interface IMessenger
    {
        /// <summary>
        /// Checks whether or not a given recipient has already been registered for a message.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to check for the given recipient.</typeparam>
        /// <typeparam name="TToken">The type of token to check the channel for.</typeparam>
        /// <param name="recipient">The target recipient to check the registration for.</param>
        /// <param name="token">The token used to identify the target channel to check.</param>
        /// <returns>Whether or not <paramref name="recipient"/> has already been registered for the specified message.</returns>
        [Pure]
        bool IsRegistered<TMessage, TToken>(object recipient, TToken token)
            where TMessage : class
            where TToken : IEquatable<TToken>;

        /// <summary>
        /// Registers a recipient for a given type of message.
        /// </summary>
        /// <typeparam name="TRecipient">The type of recipient for the message.</typeparam>
        /// <typeparam name="TMessage">The type of message to receive.</typeparam>
        /// <typeparam name="TToken">The type of token to use to pick the messages to receive.</typeparam>
        /// <param name="recipient">The recipient that will receive the messages.</param>
        /// <param name="token">A token used to determine the receiving channel to use.</param>
        /// <param name="handler">The <see cref="MessageHandler{TRecipient,TMessage}"/> to invoke when a message is received.</param>
        /// <exception cref="InvalidOperationException">Thrown when trying to register the same message twice.</exception>
        void Register<TRecipient, TMessage, TToken>(TRecipient recipient, TToken token, MessageHandler<TRecipient, TMessage> handler)
            where TRecipient : class
            where TMessage : class
            where TToken : IEquatable<TToken>;

        /// <summary>
        /// Unregisters a recipient from all registered messages.
        /// </summary>
        /// <param name="recipient">The recipient to unregister.</param>
        /// <remarks>
        /// This method will unregister the target recipient across all channels.
        /// Use this method as an easy way to lose all references to a target recipient.
        /// If the recipient has no registered handler, this method does nothing.
        /// </remarks>
        void UnregisterAll(object recipient);

        /// <summary>
        /// Unregisters a recipient from all messages on a specific channel.
        /// </summary>
        /// <typeparam name="TToken">The type of token to identify what channel to unregister from.</typeparam>
        /// <param name="recipient">The recipient to unregister.</param>
        /// <param name="token">The token to use to identify which handlers to unregister.</param>
        /// <remarks>If the recipient has no registered handler, this method does nothing.</remarks>
        void UnregisterAll<TToken>(object recipient, TToken token)
            where TToken : IEquatable<TToken>;

        /// <summary>
        /// Unregisters a recipient from messages of a given type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to stop receiving.</typeparam>
        /// <typeparam name="TToken">The type of token to identify what channel to unregister from.</typeparam>
        /// <param name="recipient">The recipient to unregister.</param>
        /// <param name="token">The token to use to identify which handlers to unregister.</param>
        /// <remarks>If the recipient has no registered handler, this method does nothing.</remarks>
        void Unregister<TMessage, TToken>(object recipient, TToken token)
            where TMessage : class
            where TToken : IEquatable<TToken>;

        /// <summary>
        /// Sends a message of the specified type to all registered recipients.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to send.</typeparam>
        /// <typeparam name="TToken">The type of token to identify what channel to use to send the message.</typeparam>
        /// <param name="message">The message to send.</param>
        /// <param name="token">The token indicating what channel to use.</param>
        /// <returns>The message that was sent (ie. <paramref name="message"/>).</returns>
        TMessage Send<TMessage, TToken>(TMessage message, TToken token)
            where TMessage : class
            where TToken : IEquatable<TToken>;

        /// <summary>
        /// Performs a cleanup on the current messenger.
        /// Invoking this method does not unregister any of the currently registered
        /// recipient, and it can be used to perform cleanup operations such as
        /// trimming the internal data structures of a messenger implementation.
        /// </summary>
        void Cleanup();

        /// <summary>
        /// Resets the <see cref="IMessenger"/> instance and unregisters all the existing recipients.
        /// </summary>
        void Reset();
    }
}