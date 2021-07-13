// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Toolkit.Mvvm.Messaging.Internals;

namespace Microsoft.Toolkit.Mvvm.Messaging
{
    /// <summary>
    /// Extensions for the <see cref="IMessenger"/> type.
    /// </summary>
    public static class IMessengerExtensions
    {
        /// <summary>
        /// A class that acts as a container to load the <see cref="MethodInfo"/> instance linked to
        /// the <see cref="Register{TMessage,TToken}(IMessenger,IRecipient{TMessage},TToken)"/> method.
        /// This class is needed to avoid forcing the initialization code in the static constructor to run as soon as
        /// the <see cref="IMessengerExtensions"/> type is referenced, even if that is done just to use methods
        /// that do not actually require this <see cref="MethodInfo"/> instance to be available.
        /// We're effectively using this type to leverage the lazy loading of static constructors done by the runtime.
        /// </summary>
        private static class MethodInfos
        {
            /// <summary>
            /// The <see cref="MethodInfo"/> instance associated with <see cref="Register{TMessage,TToken}(IMessenger,IRecipient{TMessage},TToken)"/>.
            /// </summary>
            public static readonly MethodInfo RegisterIRecipient = new Action<IMessenger, IRecipient<object>, Unit>(Register).Method.GetGenericMethodDefinition();
        }

        /// <summary>
        /// A non-generic version of <see cref="DiscoveredRecipients{TToken}"/>.
        /// </summary>
        private static class DiscoveredRecipients
        {
            /// <summary>
            /// The <see cref="ConditionalWeakTable{TKey,TValue}"/> instance used to track the preloaded registration action for each recipient.
            /// </summary>
            public static readonly ConditionalWeakTable<Type, Action<IMessenger, object>?> RegistrationMethods = new();
        }

        /// <summary>
        /// A class that acts as a static container to associate a <see cref="ConditionalWeakTable{TKey,TValue}"/> instance to each
        /// <typeparamref name="TToken"/> type in use. This is done because we can only use a single type as key, but we need to track
        /// associations of each recipient type also across different communication channels, each identified by a token.
        /// Since the token is actually a compile-time parameter, we can use a wrapping class to let the runtime handle a different
        /// instance for each generic type instantiation. This lets us only worry about the recipient type being inspected.
        /// </summary>
        /// <typeparam name="TToken">The token indicating what channel to use.</typeparam>
        private static class DiscoveredRecipients<TToken>
            where TToken : IEquatable<TToken>
        {
            /// <summary>
            /// The <see cref="ConditionalWeakTable{TKey,TValue}"/> instance used to track the preloaded registration action for each recipient.
            /// </summary>
            public static readonly ConditionalWeakTable<Type, Action<IMessenger, object, TToken>> RegistrationMethods = new();
        }

        /// <summary>
        /// Checks whether or not a given recipient has already been registered for a message.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to check for the given recipient.</typeparam>
        /// <param name="messenger">The <see cref="IMessenger"/> instance to use to check the registration.</param>
        /// <param name="recipient">The target recipient to check the registration for.</param>
        /// <returns>Whether or not <paramref name="recipient"/> has already been registered for the specified message.</returns>
        /// <remarks>This method will use the default channel to check for the requested registration.</remarks>
        [Pure]
        public static bool IsRegistered<TMessage>(this IMessenger messenger, object recipient)
            where TMessage : class
        {
            return messenger.IsRegistered<TMessage, Unit>(recipient, default);
        }

        /// <summary>
        /// Registers all declared message handlers for a given recipient, using the default channel.
        /// </summary>
        /// <param name="messenger">The <see cref="IMessenger"/> instance to use to register the recipient.</param>
        /// <param name="recipient">The recipient that will receive the messages.</param>
        /// <remarks>See notes for <see cref="RegisterAll{TToken}(IMessenger,object,TToken)"/> for more info.</remarks>
        public static void RegisterAll(this IMessenger messenger, object recipient)
        {
            // We use this method as a callback for the conditional weak table, which will handle
            // thread-safety for us. This first callback will try to find a generated method for the
            // target recipient type, and just invoke it to get the delegate to cache and use later.
            static Action<IMessenger, object>? LoadRegistrationMethodsForType(Type recipientType)
            {
                if (recipientType.Assembly.GetType("Microsoft.Toolkit.Mvvm.Messaging.__Internals.__IMessengerExtensions") is Type extensionsType &&
                    extensionsType.GetMethod("CreateAllMessagesRegistrator", new[] { recipientType }) is MethodInfo methodInfo)
                {
                    return (Action<IMessenger, object>)methodInfo.Invoke(null, new object?[] { null })!;
                }

                return null;
            }

            // Try to get the cached delegate, if the generatos has run correctly
            Action<IMessenger, object>? registrationAction = DiscoveredRecipients.RegistrationMethods.GetValue(
                recipient.GetType(),
                static t => LoadRegistrationMethodsForType(t));

            if (registrationAction is not null)
            {
                registrationAction(messenger, recipient);
            }
            else
            {
                messenger.RegisterAll(recipient, default(Unit));
            }
        }

        /// <summary>
        /// Registers all declared message handlers for a given recipient.
        /// </summary>
        /// <typeparam name="TToken">The type of token to identify what channel to use to receive messages.</typeparam>
        /// <param name="messenger">The <see cref="IMessenger"/> instance to use to register the recipient.</param>
        /// <param name="recipient">The recipient that will receive the messages.</param>
        /// <param name="token">The token indicating what channel to use.</param>
        /// <remarks>
        /// This method will register all messages corresponding to the <see cref="IRecipient{TMessage}"/> interfaces
        /// being implemented by <paramref name="recipient"/>. If none are present, this method will do nothing.
        /// Note that unlike all other extensions, this method will use reflection to find the handlers to register.
        /// Once the registration is complete though, the performance will be exactly the same as with handlers
        /// registered directly through any of the other generic extensions for the <see cref="IMessenger"/> interface.
        /// </remarks>
        public static void RegisterAll<TToken>(this IMessenger messenger, object recipient, TToken token)
            where TToken : IEquatable<TToken>
        {
            // We use this method as a callback for the conditional weak table, which will handle
            // thread-safety for us. This first callback will try to find a generated method for the
            // target recipient type, and just invoke it to get the delegate to cache and use later.
            // In this case we also need to create a generic instantiation of the target method first.
            static Action<IMessenger, object, TToken> LoadRegistrationMethodsForType(Type recipientType)
            {
                if (recipientType.Assembly.GetType("Microsoft.Toolkit.Mvvm.Messaging.__Internals.__IMessengerExtensions") is Type extensionsType &&
                    extensionsType.GetMethod("CreateAllMessagesRegistratorWithToken", new[] { recipientType }) is MethodInfo methodInfo)
                {
                    MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(typeof(TToken));

                    return (Action<IMessenger, object, TToken>)genericMethodInfo.Invoke(null, new object?[] { null })!;
                }

                return LoadRegistrationMethodsForTypeFallback(recipientType);
            }

            // Fallback method when a generated method is not found.
            // This method is only invoked once per recipient type and token type, so we're not
            // worried about making it super efficient, and we can use the LINQ code for clarity.
            // The LINQ codegen bloat is not really important for the same reason.
            static Action<IMessenger, object, TToken> LoadRegistrationMethodsForTypeFallback(Type recipientType)
            {
                // Input parameters (IMessenger instance, non-generic recipient, token)
                ParameterExpression
                    arg0 = Expression.Parameter(typeof(IMessenger)),
                    arg1 = Expression.Parameter(typeof(object)),
                    arg2 = Expression.Parameter(typeof(TToken));

                // Declare a local resulting from the (RecipientType)recipient cast
                UnaryExpression inst1 = Expression.Convert(arg1, recipientType);

                // We want a single compiled LINQ expression that executes the registration for all
                // the declared message types in the input type. To do so, we create a block with the
                // unrolled invocations for the indivudual message registration (for each IRecipient<T>).
                // The code below will generate the following block expression:
                // ===============================================================================
                // {
                //     var inst1 = (RecipientType)arg1;
                //     IMessengerExtensions.Register<T0, TToken>(arg0, inst1, arg2);
                //     IMessengerExtensions.Register<T1, TToken>(arg0, inst1, arg2);
                //     ...
                //     IMessengerExtensions.Register<TN, TToken>(arg0, inst1, arg2);
                // }
                // ===============================================================================
                // We also add an explicit object conversion to cast the input recipient type to
                // the actual specific type, so that the exposed message handlers are accessible.
                BlockExpression body = Expression.Block(
                    from interfaceType in recipientType.GetInterfaces()
                    where interfaceType.IsGenericType &&
                          interfaceType.GetGenericTypeDefinition() == typeof(IRecipient<>)
                    let messageType = interfaceType.GenericTypeArguments[0]
                    let registrationMethod = MethodInfos.RegisterIRecipient.MakeGenericMethod(messageType, typeof(TToken))
                    select Expression.Call(registrationMethod, new Expression[]
                    {
                        arg0,
                        inst1,
                        arg2
                    }));

                return Expression.Lambda<Action<IMessenger, object, TToken>>(body, arg0, arg1, arg2).Compile();
            }

            // Get or compute the registration method for the current recipient type.
            // As in Microsoft.Toolkit.Diagnostics.TypeExtensions.ToTypeString, we use a lambda
            // expression instead of a method group expression to leverage the statically initialized
            // delegate and avoid repeated allocations for each invocation of this method.
            // For more info on this, see the related issue at https://github.com/dotnet/roslyn/issues/5835.
            Action<IMessenger, object, TToken> registrationAction = DiscoveredRecipients<TToken>.RegistrationMethods.GetValue(
                recipient.GetType(),
                static t => LoadRegistrationMethodsForType(t));

            // Invoke the cached delegate to actually execute the message registration
            registrationAction(messenger, recipient, token);
        }

        /// <summary>
        /// Registers a recipient for a given type of message.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to receive.</typeparam>
        /// <param name="messenger">The <see cref="IMessenger"/> instance to use to register the recipient.</param>
        /// <param name="recipient">The recipient that will receive the messages.</param>
        /// <exception cref="InvalidOperationException">Thrown when trying to register the same message twice.</exception>
        /// <remarks>This method will use the default channel to perform the requested registration.</remarks>
        public static void Register<TMessage>(this IMessenger messenger, IRecipient<TMessage> recipient)
            where TMessage : class
        {
            messenger.Register<IRecipient<TMessage>, TMessage, Unit>(recipient, default, static (r, m) => r.Receive(m));
        }

        /// <summary>
        /// Registers a recipient for a given type of message.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to receive.</typeparam>
        /// <typeparam name="TToken">The type of token to identify what channel to use to receive messages.</typeparam>
        /// <param name="messenger">The <see cref="IMessenger"/> instance to use to register the recipient.</param>
        /// <param name="recipient">The recipient that will receive the messages.</param>
        /// <param name="token">The token indicating what channel to use.</param>
        /// <exception cref="InvalidOperationException">Thrown when trying to register the same message twice.</exception>
        /// <remarks>This method will use the default channel to perform the requested registration.</remarks>
        public static void Register<TMessage, TToken>(this IMessenger messenger, IRecipient<TMessage> recipient, TToken token)
            where TMessage : class
            where TToken : IEquatable<TToken>
        {
            messenger.Register<IRecipient<TMessage>, TMessage, TToken>(recipient, token, static (r, m) => r.Receive(m));
        }

        /// <summary>
        /// Registers a recipient for a given type of message.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to receive.</typeparam>
        /// <param name="messenger">The <see cref="IMessenger"/> instance to use to register the recipient.</param>
        /// <param name="recipient">The recipient that will receive the messages.</param>
        /// <param name="handler">The <see cref="MessageHandler{TRecipient,TMessage}"/> to invoke when a message is received.</param>
        /// <exception cref="InvalidOperationException">Thrown when trying to register the same message twice.</exception>
        /// <remarks>This method will use the default channel to perform the requested registration.</remarks>
        public static void Register<TMessage>(this IMessenger messenger, object recipient, MessageHandler<object, TMessage> handler)
            where TMessage : class
        {
            messenger.Register(recipient, default(Unit), handler);
        }

        /// <summary>
        /// Registers a recipient for a given type of message.
        /// </summary>
        /// <typeparam name="TRecipient">The type of recipient for the message.</typeparam>
        /// <typeparam name="TMessage">The type of message to receive.</typeparam>
        /// <param name="messenger">The <see cref="IMessenger"/> instance to use to register the recipient.</param>
        /// <param name="recipient">The recipient that will receive the messages.</param>
        /// <param name="handler">The <see cref="MessageHandler{TRecipient,TMessage}"/> to invoke when a message is received.</param>
        /// <exception cref="InvalidOperationException">Thrown when trying to register the same message twice.</exception>
        /// <remarks>This method will use the default channel to perform the requested registration.</remarks>
        public static void Register<TRecipient, TMessage>(this IMessenger messenger, TRecipient recipient, MessageHandler<TRecipient, TMessage> handler)
            where TRecipient : class
            where TMessage : class
        {
            messenger.Register(recipient, default(Unit), handler);
        }

        /// <summary>
        /// Registers a recipient for a given type of message.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to receive.</typeparam>
        /// <typeparam name="TToken">The type of token to use to pick the messages to receive.</typeparam>
        /// <param name="messenger">The <see cref="IMessenger"/> instance to use to register the recipient.</param>
        /// <param name="recipient">The recipient that will receive the messages.</param>
        /// <param name="token">A token used to determine the receiving channel to use.</param>
        /// <param name="handler">The <see cref="MessageHandler{TRecipient,TMessage}"/> to invoke when a message is received.</param>
        /// <exception cref="InvalidOperationException">Thrown when trying to register the same message twice.</exception>
        public static void Register<TMessage, TToken>(this IMessenger messenger, object recipient, TToken token, MessageHandler<object, TMessage> handler)
            where TMessage : class
            where TToken : IEquatable<TToken>
        {
            messenger.Register(recipient, token, handler);
        }

        /// <summary>
        /// Unregisters a recipient from messages of a given type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to stop receiving.</typeparam>
        /// <param name="messenger">The <see cref="IMessenger"/> instance to use to unregister the recipient.</param>
        /// <param name="recipient">The recipient to unregister.</param>
        /// <remarks>
        /// This method will unregister the target recipient only from the default channel.
        /// If the recipient has no registered handler, this method does nothing.
        /// </remarks>
        public static void Unregister<TMessage>(this IMessenger messenger, object recipient)
            where TMessage : class
        {
            messenger.Unregister<TMessage, Unit>(recipient, default);
        }

        /// <summary>
        /// Sends a message of the specified type to all registered recipients.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to send.</typeparam>
        /// <param name="messenger">The <see cref="IMessenger"/> instance to use to send the message.</param>
        /// <returns>The message that has been sent.</returns>
        /// <remarks>
        /// This method is a shorthand for <see cref="Send{TMessage}(IMessenger,TMessage)"/> when the
        /// message type exposes a parameterless constructor: it will automatically create
        /// a new <typeparamref name="TMessage"/> instance and send that to its recipients.
        /// </remarks>
        public static TMessage Send<TMessage>(this IMessenger messenger)
            where TMessage : class, new()
        {
            return messenger.Send(new TMessage(), default(Unit));
        }

        /// <summary>
        /// Sends a message of the specified type to all registered recipients.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to send.</typeparam>
        /// <param name="messenger">The <see cref="IMessenger"/> instance to use to send the message.</param>
        /// <param name="message">The message to send.</param>
        /// <returns>The message that was sent (ie. <paramref name="message"/>).</returns>
        public static TMessage Send<TMessage>(this IMessenger messenger, TMessage message)
            where TMessage : class
        {
            return messenger.Send(message, default(Unit));
        }

        /// <summary>
        /// Sends a message of the specified type to all registered recipients.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to send.</typeparam>
        /// <typeparam name="TToken">The type of token to identify what channel to use to send the message.</typeparam>
        /// <param name="messenger">The <see cref="IMessenger"/> instance to use to send the message.</param>
        /// <param name="token">The token indicating what channel to use.</param>
        /// <returns>The message that has been sen.</returns>
        /// <remarks>
        /// This method will automatically create a new <typeparamref name="TMessage"/> instance
        /// just like <see cref="Send{TMessage}(IMessenger)"/>, and then send it to the right recipients.
        /// </remarks>
        public static TMessage Send<TMessage, TToken>(this IMessenger messenger, TToken token)
            where TMessage : class, new()
            where TToken : IEquatable<TToken>
        {
            return messenger.Send(new TMessage(), token);
        }
    }
}