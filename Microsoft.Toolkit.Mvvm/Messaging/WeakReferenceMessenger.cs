// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Microsoft.Collections.Extensions;
using Microsoft.Toolkit.Mvvm.Messaging.Internals;
#if NETSTANDARD2_0
using RecipientsTable = Microsoft.Toolkit.Mvvm.Messaging.WeakReferenceMessenger.ConditionalWeakTable<object, Microsoft.Collections.Extensions.IDictionarySlim>;
#else
using RecipientsTable = System.Runtime.CompilerServices.ConditionalWeakTable<object, Microsoft.Collections.Extensions.IDictionarySlim>;
#endif

namespace Microsoft.Toolkit.Mvvm.Messaging
{
    /// <summary>
    /// A class providing a reference implementation for the <see cref="IMessenger"/> interface.
    /// </summary>
    /// <remarks>
    /// This <see cref="IMessenger"/> implementation uses weak references to track the registered
    /// recipients, so it is not necessary to manually unregister them when they're no longer needed.
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
                    table!.TryGetValue(recipient, out IDictionarySlim? mapping) &&
                    Unsafe.As<DictionarySlim<TToken, object>>(mapping)!.ContainsKey(token);
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
                    enumerator.Value.Remove(recipient);
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
                        Unsafe.As<DictionarySlim<TToken, object>>(mapping)!.TryRemove(token, out _);
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
                var enumerator = this.recipientsMap.GetEnumerator();

                // Traverse all the existing token and message pairs matching the current type
                // arguments, and remove all the handlers with a matching token, as above.
                while (enumerator.MoveNext())
                {
                    if (enumerator.Key.Equals(type2) &&
                        enumerator.Value.TryGetValue(recipient, out IDictionarySlim? mapping))
                    {
                        Unsafe.As<DictionarySlim<TToken, object>>(mapping)!.TryRemove(token, out _);
                    }
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
                foreach (KeyValuePair<object, IDictionarySlim> pair in table!)
                {
                    var map = Unsafe.As<DictionarySlim<TToken, object>>(pair.Value);

                    if (map.TryGetValue(token, out object? handler))
                    {
                        bufferWriter.Add(handler!);
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
                        enumerator.Value.Remove(recipient);
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
                    this.recipientsMap.TryRemove(key, out _);
                }
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

#if NETSTANDARD2_0
        /// <summary>
        /// A wrapper for <see cref="System.Runtime.CompilerServices.ConditionalWeakTable{TKey,TValue}"/>
        /// that backports the enumerable support to .NET Standard 2.0 through an auxiliary list.
        /// </summary>
        /// <typeparam name="TKey">Tke key of items to store in the table.</typeparam>
        /// <typeparam name="TValue">The values to store in the table.</typeparam>
        internal sealed class ConditionalWeakTable<TKey, TValue>
            where TKey : class
            where TValue : class?
        {
            /// <summary>
            /// The underlying <see cref="System.Runtime.CompilerServices.ConditionalWeakTable{TKey,TValue}"/> instance.
            /// </summary>
            private readonly System.Runtime.CompilerServices.ConditionalWeakTable<TKey, TValue> table = new();

            /// <summary>
            /// A supporting linked list to store keys in <see cref="table"/>. This is needed to expose
            /// the ability to enumerate existing keys when there is no support for that in the BCL.
            /// </summary>
            private readonly LinkedList<WeakReference<TKey>> keys = new();

            /// <inheritdoc cref="System.Runtime.CompilerServices.ConditionalWeakTable{TKey,TValue}.TryGetValue"/>
            public bool TryGetValue(TKey key, out TValue? value)
            {
                return this.table.TryGetValue(key, out value);
            }

            /// <inheritdoc cref="System.Runtime.CompilerServices.ConditionalWeakTable{TKey,TValue}.GetValue"/>
            public TValue GetValue(TKey key, System.Runtime.CompilerServices.ConditionalWeakTable<TKey, TValue>.CreateValueCallback createValueCallback)
            {
                // Get or create the value. When this method returns, the key will be present in the table
                TValue value = this.table.GetValue(key, createValueCallback);

                // Check if the list of keys contains the given key.
                // If it does, we can just stop here and return the result.
                foreach (WeakReference<TKey> node in this.keys)
                {
                    if (node.TryGetTarget(out TKey? target) &&
                        ReferenceEquals(target, key))
                    {
                        return value;
                    }
                }

                // Add the key to the list of weak references to track it
                this.keys.AddFirst(new WeakReference<TKey>(key));

                return value;
            }

            /// <inheritdoc cref="System.Runtime.CompilerServices.ConditionalWeakTable{TKey,TValue}.Remove"/>
            public bool Remove(TKey key)
            {
                return this.table.Remove(key);
            }

            /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Enumerator GetEnumerator() => new(this);

            /// <summary>
            /// A custom enumerator that traverses items in a <see cref="ConditionalWeakTable{TKey, TValue}"/> instance.
            /// </summary>
            public ref struct Enumerator
            {
                /// <summary>
                /// The owner <see cref="ConditionalWeakTable{TKey, TValue}"/> instance for the enumerator.
                /// </summary>
                private readonly ConditionalWeakTable<TKey, TValue> owner;

                /// <summary>
                /// The current <see cref="LinkedListNode{T}"/>, if any.
                /// </summary>
                private LinkedListNode<WeakReference<TKey>>? node;

                /// <summary>
                /// The current <see cref="KeyValuePair{TKey, TValue}"/> to return.
                /// </summary>
                private KeyValuePair<TKey, TValue> current;

                /// <summary>
                /// Indicates whether or not <see cref="MoveNext"/> has been called at least once.
                /// </summary>
                private bool isFirstMoveNextPending;

                /// <summary>
                /// Initializes a new instance of the <see cref="Enumerator"/> struct.
                /// </summary>
                /// <param name="owner">The owner <see cref="ConditionalWeakTable{TKey, TValue}"/> instance for the enumerator.</param>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Enumerator(ConditionalWeakTable<TKey, TValue> owner)
                {
                    this.owner = owner;
                    this.node = null;
                    this.current = default;
                    this.isFirstMoveNextPending = true;
                }

                /// <inheritdoc cref="System.Collections.IEnumerator.MoveNext"/>
                public bool MoveNext()
                {
                    LinkedListNode<WeakReference<TKey>>? node;

                    if (!isFirstMoveNextPending)
                    {
                        node = this.node!.Next;
                    }
                    else
                    {
                        node = this.owner.keys.First;

                        this.isFirstMoveNextPending = false;
                    }

                    while (node is not null)
                    {
                        // Get the key and value for the current node
                        if (node.Value.TryGetTarget(out TKey? target) &&
                            this.owner.table.TryGetValue(target!, out TValue? value))
                        {
                            this.node = node;
                            this.current = new KeyValuePair<TKey, TValue>(target, value);

                            return true;
                        }
                        else
                        {
                            // If the current key has been collected, trim the list
                            this.owner.keys.Remove(node);
                        }

                        node = node.Next;
                    }

                    return false;
                }

                /// <inheritdoc cref="System.Collections.IEnumerator.MoveNext"/>
                public readonly KeyValuePair<TKey, TValue> Current
                {
                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    get => this.current;
                }
            }
        }
#endif

        /// <summary>
        /// A simple buffer writer implementation using pooled arrays.
        /// </summary>
        /// <typeparam name="T">The type of items to store in the list.</typeparam>
        /// <remarks>
        /// This type is a <see langword="ref"/> <see langword="struct"/> to avoid the object allocation and to
        /// enable the pattern-based <see cref="IDisposable"/> support. We aren't worried with consumers not
        /// using this type correctly since it's private and only accessible within the parent type.
        /// </remarks>
        private ref struct ArrayPoolBufferWriter<T>
        {
            /// <summary>
            /// The default buffer size to use to expand empty arrays.
            /// </summary>
            private const int DefaultInitialBufferSize = 128;

            /// <summary>
            /// The underlying <typeparamref name="T"/> array.
            /// </summary>
            private T[] array;

            /// <summary>
            /// The starting offset within <see cref="array"/>.
            /// </summary>
            private int index;

            /// <summary>
            /// Creates a new instance of the <see cref="ArrayPoolBufferWriter{T}"/> struct.
            /// </summary>
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static ArrayPoolBufferWriter<T> Create()
            {
                return new() { array = ArrayPool<T>.Shared.Rent(DefaultInitialBufferSize) };
            }

            /// <summary>
            /// Gets a <see cref="ReadOnlySpan{T}"/> with the current items.
            /// </summary>
            public ReadOnlySpan<T> Span
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this.array.AsSpan(0, this.index);
            }

            /// <summary>
            /// Adds a new item to the current collection.
            /// </summary>
            /// <param name="item">The item to add.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add(T item)
            {
                if (this.index == this.array.Length)
                {
                    ResizeBuffer();
                }

                this.array[this.index++] = item;
            }

            /// <summary>
            /// Resets the underlying array and the stored items.
            /// </summary>
            public void Reset()
            {
                Array.Clear(this.array, 0, this.index);

                this.index = 0;
            }

            /// <summary>
            /// Resizes <see cref="array"/> when there is no space left for new items.
            /// </summary>
            [MethodImpl(MethodImplOptions.NoInlining)]
            private void ResizeBuffer()
            {
                T[] rent = ArrayPool<T>.Shared.Rent(this.index << 2);

                Array.Copy(this.array, 0, rent, 0, this.index);
                Array.Clear(this.array, 0, this.index);

                ArrayPool<T>.Shared.Return(this.array);

                this.array = rent;
            }

            /// <inheritdoc cref="IDisposable.Dispose"/>
            public void Dispose()
            {
                Array.Clear(this.array, 0, this.index);

                ArrayPool<T>.Shared.Return(this.array);
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