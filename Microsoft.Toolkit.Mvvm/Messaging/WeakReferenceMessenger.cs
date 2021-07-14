// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Collections.Extensions;
using Microsoft.Toolkit.Mvvm.Messaging.Internals;
#if NETSTANDARD2_0
using RecipientsTable = Microsoft.Toolkit.Mvvm.Messaging.Internals.ConditionalWeakTable2<object, Microsoft.Collections.Extensions.IDictionarySlim>;
#else
using RecipientsTable = System.Runtime.CompilerServices.ConditionalWeakTable<object, Microsoft.Collections.Extensions.IDictionarySlim>;
#endif

#pragma warning disable SA1204

namespace Microsoft.Toolkit.Mvvm.Messaging
{
    /// <summary>
    /// A class providing a reference implementation for the <see cref="IMessenger"/> interface.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This <see cref="IMessenger"/> implementation uses weak references to track the registered
    /// recipients, so it is not necessary to manually unregister them when they're no longer needed.
    /// </para>
    /// <para>
    /// The <see cref="WeakReferenceMessenger"/> type will automatically perform internal trimming when
    /// full GC collections are invoked, so calling <see cref="Cleanup"/> manually is not necessary to
    /// ensure that on average the internal data structures are as trimmed and compact as possible.
    /// </para>
    /// </remarks>
    public sealed class WeakReferenceMessenger : IMessenger
    {
        // This messenger uses the following logic to link stored instances together:
        // --------------------------------------------------------------------------------------------------------
        //                          DictionarySlim<TToken, MessageHandler<TRecipient, TMessage>> mapping
        //                                           /                                   /          /
        //                      ___(Type2.TToken)___/                                   /          /
        //                     /_________________(Type2.TMessage)______________________/          /
        //                    /                                       ___________________________/
        //                   /                                       /
        // DictionarySlim<Type2, ConditionalWeakTable<object, IDictionarySlim>> recipientsMap;
        // --------------------------------------------------------------------------------------------------------
        // Just like in the strong reference variant, each pair of message and token types is used as a key in the
        // recipients map. In this case, the values in the dictionary are ConditionalWeakTable<,> instances, that
        // link each registered recipient to a map of currently registered handlers, through a weak reference.
        // The value in each conditional table is Dictionary<TToken, MessageHandler<TRecipient, TMessage>>, using
        // the same unsafe cast as before to allow the generic handler delegates to be invoked without knowing
        // what type each recipient was registered with, and without the need to use reflection.

        /// <summary>
        /// The map of currently registered recipients for all message types.
        /// </summary>
        private readonly DictionarySlim<Type2, RecipientsTable> recipientsMap = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReferenceMessenger"/> class.
        /// </summary>
        public WeakReferenceMessenger()
        {
            // Proxy function for the GC callback. This needs to be static and to take the target instance as
            // an input parameter in order to avoid rooting it from the Gen2GcCallback object invoking it.
            static void Gen2GcCallbackProxy(object target)
            {
                ((WeakReferenceMessenger)target).CleanupWithNonBlockingLock();
            }

            // Register an automatic GC callback to trigger a non-blocking cleanup. This will ensure that the
            // current messenger instance is trimmed and without leftover recipient maps that are no longer used.
            // This is necessary (as in, some form of cleanup, either explicit or automatic like in this case)
            // because the ConditionalWeakTable<TKey, TValue> instances will just remove key-value pairs on their
            // own as soon as a key (ie. a recipient) is collected, causing their own keys (ie. the Type2 instances
            // mapping to each conditional table for a pair of message and token types) to potentially remain in the
            // root mapping structure but without any remaining recipients actually registered there, which just
            // adds unnecessary overhead when trying to enumerate recipients during broadcasting operations later on.
            Gen2GcCallback.Register(Gen2GcCallbackProxy, this);
        }

        /// <summary>
        /// Gets the default <see cref="WeakReferenceMessenger"/> instance.
        /// </summary>
        public static WeakReferenceMessenger Default { get; } = new();

        /// <inheritdoc/>
        public bool IsRegistered<TMessage, TToken>(object recipient, TToken token)
            where TMessage : class
            where TToken : IEquatable<TToken>
        {
            lock (this.recipientsMap)
            {
                Type2 type2 = new(typeof(TMessage), typeof(TToken));

                // Get the conditional table associated with the target recipient, for the current pair
                // of token and message types. If it exists, check if there is a matching token.
                return
                    this.recipientsMap.TryGetValue(type2, out RecipientsTable? table) &&
                    table.TryGetValue(recipient, out IDictionarySlim? mapping) &&
                    Unsafe.As<DictionarySlim<TToken, object>>(mapping).ContainsKey(token);
            }
        }

        /// <inheritdoc/>
        public void Register<TRecipient, TMessage, TToken>(TRecipient recipient, TToken token, MessageHandler<TRecipient, TMessage> handler)
            where TRecipient : class
            where TMessage : class
            where TToken : IEquatable<TToken>
        {
            lock (this.recipientsMap)
            {
                Type2 type2 = new(typeof(TMessage), typeof(TToken));

                // Get the conditional table for the pair of type arguments, or create it if it doesn't exist
                ref RecipientsTable? mapping = ref this.recipientsMap.GetOrAddValueRef(type2);

                mapping ??= new RecipientsTable();

                // Get or create the handlers dictionary for the target recipient
                var map = Unsafe.As<DictionarySlim<TToken, object>>(mapping.GetValue(recipient, static _ => new DictionarySlim<TToken, object>()));

                // Add the new registration entry
                ref object? registeredHandler = ref map.GetOrAddValueRef(token);

                if (registeredHandler is not null)
                {
                    ThrowInvalidOperationExceptionForDuplicateRegistration();
                }

                // Store the input handler
                registeredHandler = handler;
            }
        }

        /// <inheritdoc/>
        public void UnregisterAll(object recipient)
        {
            lock (this.recipientsMap)
            {
                var enumerator = this.recipientsMap.GetEnumerator();

                // Traverse all the existing conditional tables and remove all the ones
                // with the target recipient as key. We don't perform a cleanup here,
                // as that is responsibility of a separate method defined below.
                while (enumerator.MoveNext())
                {
                    _ = enumerator.Value.Remove(recipient);
                }
            }
        }

        /// <inheritdoc/>
        public void UnregisterAll<TToken>(object recipient, TToken token)
            where TToken : IEquatable<TToken>
        {
            lock (this.recipientsMap)
            {
                var enumerator = this.recipientsMap.GetEnumerator();

                // Same as above, with the difference being that this time we only go through
                // the conditional tables having a matching token type as key, and that we
                // only try to remove handlers with a matching token, if any.
                while (enumerator.MoveNext())
                {
                    if (enumerator.Key.TToken == typeof(TToken) &&
                        enumerator.Value.TryGetValue(recipient, out IDictionarySlim? mapping))
                    {
                        _ = Unsafe.As<DictionarySlim<TToken, object>>(mapping).TryRemove(token);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void Unregister<TMessage, TToken>(object recipient, TToken token)
            where TMessage : class
            where TToken : IEquatable<TToken>
        {
            lock (this.recipientsMap)
            {
                Type2 type2 = new(typeof(TMessage), typeof(TToken));

                // Get the target mapping table for the combination of message and token types,
                // and remove the handler with a matching token (the entire map), if present.
                if (this.recipientsMap.TryGetValue(type2, out RecipientsTable? value) &&
                    value.TryGetValue(recipient, out IDictionarySlim? mapping))
                {
                    _ = Unsafe.As<DictionarySlim<TToken, object>>(mapping).TryRemove(token);
                }
            }
        }

        /// <inheritdoc/>
        public TMessage Send<TMessage, TToken>(TMessage message, TToken token)
            where TMessage : class
            where TToken : IEquatable<TToken>
        {
            ArrayPoolBufferWriter<object> bufferWriter;
            int i = 0;

            lock (this.recipientsMap)
            {
                Type2 type2 = new(typeof(TMessage), typeof(TToken));

                // Try to get the target table
                if (!this.recipientsMap.TryGetValue(type2, out RecipientsTable? table))
                {
                    return message;
                }

                bufferWriter = ArrayPoolBufferWriter<object>.Create();

                // We need a local, temporary copy of all the pending recipients and handlers to
                // invoke, to avoid issues with handlers unregistering from messages while we're
                // holding the lock. To do this, we can just traverse the conditional table in use
                // to enumerate all the existing recipients for the token and message types pair
                // corresponding to the generic arguments for this invocation, and then track the
                // handlers with a matching token, and their corresponding recipients.
                foreach (KeyValuePair<object, IDictionarySlim> pair in table)
                {
                    var map = Unsafe.As<DictionarySlim<TToken, object>>(pair.Value);

                    if (map.TryGetValue(token, out object? handler))
                    {
                        bufferWriter.Add(handler);
                        bufferWriter.Add(pair.Key);
                        i++;
                    }
                }
            }

            try
            {
                ReadOnlySpan<object> pairs = bufferWriter.Span;

                for (int j = 0; j < i; j++)
                {
                    // Just like in the other messenger, here we need an unsafe cast to be able to
                    // invoke a generic delegate with a contravariant input argument, with a less
                    // derived reference, without reflection. This is guaranteed to work by how the
                    // messenger tracks registered recipients and their associated handlers, so the
                    // type conversion will always be valid (the recipients are the rigth instances).
                    Unsafe.As<MessageHandler<object, TMessage>>(pairs[2 * j])(pairs[(2 * j) + 1], message);
                }
            }
            finally
            {
                bufferWriter.Dispose();
            }

            return message;
        }

        /// <inheritdoc/>
        public void Cleanup()
        {
            lock (this.recipientsMap)
            {
                CleanupWithoutLock();
            }
        }

        /// <inheritdoc/>
        public void Reset()
        {
            lock (this.recipientsMap)
            {
                this.recipientsMap.Clear();
            }
        }

        /// <summary>
        /// Executes a cleanup without locking the current instance. This method has to be
        /// invoked when a lock on <see cref="recipientsMap"/> has already been acquired.
        /// </summary>
        private void CleanupWithNonBlockingLock()
        {
            object lockObject = this.recipientsMap;
            bool lockTaken = false;

            try
            {
                Monitor.TryEnter(lockObject, ref lockTaken);

                if (lockTaken)
                {
                    CleanupWithoutLock();
                }
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(lockObject);
                }
            }
        }

        /// <summary>
        /// Executes a cleanup without locking the current instance. This method has to be
        /// invoked when a lock on <see cref="recipientsMap"/> has already been acquired.
        /// </summary>
        private void CleanupWithoutLock()
        {
            using ArrayPoolBufferWriter<Type2> type2s = ArrayPoolBufferWriter<Type2>.Create();
            using ArrayPoolBufferWriter<object> emptyRecipients = ArrayPoolBufferWriter<object>.Create();

            var enumerator = this.recipientsMap.GetEnumerator();

            // First, we go through all the currently registered pairs of token and message types.
            // These represents all the combinations of generic arguments with at least one registered
            // handler, with the exception of those with recipients that have already been collected.
            while (enumerator.MoveNext())
            {
                emptyRecipients.Reset();

                bool hasAtLeastOneHandler = false;

                // Go through the currently alive recipients to look for those with no handlers left. We track
                // the ones we find to remove them outside of the loop (can't modify during enumeration).
                foreach (KeyValuePair<object, IDictionarySlim> pair in enumerator.Value)
                {
                    if (pair.Value.Count == 0)
                    {
                        emptyRecipients.Add(pair.Key);
                    }
                    else
                    {
                        hasAtLeastOneHandler = true;
                    }
                }

                // Remove the handler maps for recipients that are still alive but with no handlers
                foreach (object recipient in emptyRecipients.Span)
                {
                    _ = enumerator.Value.Remove(recipient);
                }

                // Track the type combinations with no recipients or handlers left
                if (!hasAtLeastOneHandler)
                {
                    type2s.Add(enumerator.Key);
                }
            }

            // Remove all the mappings with no handlers left
            foreach (Type2 key in type2s.Span)
            {
                _ = this.recipientsMap.TryRemove(key);
            }
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> when trying to add a duplicate handler.
        /// </summary>
        private static void ThrowInvalidOperationExceptionForDuplicateRegistration()
        {
            throw new InvalidOperationException("The target recipient has already subscribed to the target message");
        }
    }
}