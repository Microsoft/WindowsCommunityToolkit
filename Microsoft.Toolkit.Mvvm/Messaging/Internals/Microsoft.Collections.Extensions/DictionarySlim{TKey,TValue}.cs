// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable SA1512

// The DictionarySlim<TKey, TValue> type is originally from CoreFX labs, see
// the source repository on GitHub at https://github.com/dotnet/corefxlab.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Microsoft.Collections.Extensions
{
    /// <summary>
    /// A lightweight Dictionary with three principal differences compared to <see cref="Dictionary{TKey, TValue}"/>
    ///
    /// 1) It is possible to do "get or add" in a single lookup. For value types, this also saves a copy of the value.
    /// 2) It assumes it is cheap to equate values.
    /// 3) It assumes the keys implement <see cref="IEquatable{TKey}"/> and they are cheap and sufficient.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <remarks>
    /// 1) This avoids having to do separate lookups (<see cref="Dictionary{TKey, TValue}.TryGetValue(TKey, out TValue)"/>
    /// followed by <see cref="Dictionary{TKey, TValue}.Add(TKey, TValue)"/>.
    /// There is not currently an API exposed to get a value by ref without adding if the key is not present.
    /// 2) This means it can save space by not storing hash codes.
    /// 3) This means it can avoid storing a comparer, and avoid the likely virtual call to a comparer.
    /// </remarks>
    [DebuggerDisplay("Count = {Count}")]
    internal class DictionarySlim<TKey, TValue> : IDictionarySlim<TKey, TValue>
        where TKey : IEquatable<TKey>
        where TValue : class
    {
        /// <summary>
        /// A reusable array of <see cref="Entry"/> items with a single value.
        /// This is used when a new <see cref="DictionarySlim{TKey,TValue}"/> instance is
        /// created, or when one is cleared. The first item being added to this collection
        /// will immediately cause the first resize (see <see cref="AddKey"/> for more info).
        /// </summary>
        private static readonly Entry[] InitialEntries = new Entry[1];

        /// <summary>
        /// The current number of items stored in the map.
        /// </summary>
        private int count;

        /// <summary>
        /// The 1-based index for the start of the free list within <see cref="entries"/>.
        /// </summary>
        private int freeList = -1;

        /// <summary>
        /// The array of 1-based indices for the <see cref="Entry"/> items stored in <see cref="entries"/>.
        /// </summary>
        private int[] buckets;

        /// <summary>
        /// The array of currently stored key-value pairs (ie. the lists for each hash group).
        /// </summary>
        private Entry[] entries;

        /// <summary>
        /// A type representing a map entry, ie. a node in a given list.
        /// </summary>
        private struct Entry
        {
            /// <summary>
            /// The key for the value in the current node.
            /// </summary>
            public TKey Key;

            /// <summary>
            /// The value in the current node, if present.
            /// </summary>
            public TValue? Value;

            /// <summary>
            /// The 0-based index for the next node in the current list.
            /// </summary>
            public int Next;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionarySlim{TKey, TValue}"/> class.
        /// </summary>
        public DictionarySlim()
        {
            this.buckets = HashHelpers.SizeOneIntArray;
            this.entries = InitialEntries;
        }

        /// <inheritdoc/>
        public int Count => this.count;

        /// <inheritdoc/>
        public TValue this[TKey key]
        {
            get
            {
                Entry[] entries = this.entries;

                for (int i = this.buckets[key.GetHashCode() & (this.buckets.Length - 1)] - 1;
                     (uint)i < (uint)entries.Length;
                     i = entries[i].Next)
                {
                    if (key.Equals(entries[i].Key))
                    {
                        return entries[i].Value!;
                    }
                }

                ThrowArgumentExceptionForKeyNotFound(key);

                return default!;
            }
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.count = 0;
            this.freeList = -1;
            this.buckets = HashHelpers.SizeOneIntArray;
            this.entries = InitialEntries;
        }

        /// <summary>
        /// Checks whether or not the dictionary contains a pair with a specified key.
        /// </summary>
        /// <param name="key">The key to look for.</param>
        /// <returns>Whether or not the key was present in the dictionary.</returns>
        public bool ContainsKey(TKey key)
        {
            Entry[] entries = this.entries;

            for (int i = this.buckets[key.GetHashCode() & (this.buckets.Length - 1)] - 1;
                 (uint)i < (uint)entries.Length;
                 i = entries[i].Next)
            {
                if (key.Equals(entries[i].Key))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the value if present for the specified key.
        /// </summary>
        /// <param name="key">The key to look for.</param>
        /// <param name="value">The value found, otherwise <see langword="default"/>.</param>
        /// <returns>Whether or not the key was present.</returns>
        public bool TryGetValue(TKey key, [NotNullWhen(true)] out TValue? value)
        {
            Entry[] entries = this.entries;

            for (int i = this.buckets[key.GetHashCode() & (this.buckets.Length - 1)] - 1;
                 (uint)i < (uint)entries.Length;
                 i = entries[i].Next)
            {
                if (key.Equals(entries[i].Key))
                {
                    value = entries[i].Value!;

                    return true;
                }
            }

            value = default!;

            return false;
        }

        /// <inheritdoc/>
        public bool TryRemove(TKey key)
        {
            Entry[] entries = this.entries;
            int bucketIndex = key.GetHashCode() & (this.buckets.Length - 1);
            int entryIndex = this.buckets[bucketIndex] - 1;
            int lastIndex = -1;

            while (entryIndex != -1)
            {
                Entry candidate = entries[entryIndex];

                if (candidate.Key.Equals(key))
                {
                    if (lastIndex != -1)
                    {
                        entries[lastIndex].Next = candidate.Next;
                    }
                    else
                    {
                        this.buckets[bucketIndex] = candidate.Next + 1;
                    }

                    entries[entryIndex] = default;
                    entries[entryIndex].Next = -3 - this.freeList;

                    this.freeList = entryIndex;
                    this.count--;

                    return true;
                }

                lastIndex = entryIndex;
                entryIndex = candidate.Next;
            }

            return false;
        }

        /// <summary>
        /// Gets the value for the specified key, or, if the key is not present,
        /// adds an entry and returns the value by ref. This makes it possible to
        /// add or update a value in a single look up operation.
        /// </summary>
        /// <param name="key">Key to look for</param>
        /// <returns>Reference to the new or existing value</returns>
        public ref TValue? GetOrAddValueRef(TKey key)
        {
            Entry[] entries = this.entries;
            int bucketIndex = key.GetHashCode() & (this.buckets.Length - 1);

            for (int i = this.buckets[bucketIndex] - 1;
                 (uint)i < (uint)entries.Length;
                 i = entries[i].Next)
            {
                if (key.Equals(entries[i].Key))
                {
                    return ref entries[i].Value;
                }
            }

            return ref AddKey(key, bucketIndex);
        }

        /// <summary>
        /// Creates a slot for a new value to add for a specified key.
        /// </summary>
        /// <param name="key">The key to use to add the new value.</param>
        /// <param name="bucketIndex">The target bucked index to use.</param>
        /// <returns>A reference to the slot for the new value to add.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private ref TValue? AddKey(TKey key, int bucketIndex)
        {
            Entry[] entries = this.entries;
            int entryIndex;

            if (this.freeList != -1)
            {
                entryIndex = this.freeList;

                this.freeList = -3 - entries[this.freeList].Next;
            }
            else
            {
                if (this.count == entries.Length || entries.Length == 1)
                {
                    entries = Resize();
                    bucketIndex = key.GetHashCode() & (this.buckets.Length - 1);
                }

                entryIndex = this.count;
            }

            entries[entryIndex].Key = key;
            entries[entryIndex].Next = this.buckets[bucketIndex] - 1;

            this.buckets[bucketIndex] = entryIndex + 1;
            this.count++;

            return ref entries[entryIndex].Value;
        }

        /// <summary>
        /// Resizes the current dictionary to reduce the number of collisions
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private Entry[] Resize()
        {
            int count = this.count;
            int newSize = this.entries.Length * 2;

            if ((uint)newSize > int.MaxValue)
            {
                ThrowInvalidOperationExceptionForMaxCapacityExceeded();
            }

            var entries = new Entry[newSize];

            Array.Copy(this.entries, 0, entries, 0, count);

            var newBuckets = new int[entries.Length];

            while (count-- > 0)
            {
                int bucketIndex = entries[count].Key.GetHashCode() & (newBuckets.Length - 1);

                entries[count].Next = newBuckets[bucketIndex] - 1;

                newBuckets[bucketIndex] = count + 1;
            }

            this.buckets = newBuckets;
            this.entries = entries;

            return entries;
        }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => new(this);

        /// <summary>
        /// Enumerator for <see cref="DictionarySlim{TKey,TValue}"/>.
        /// </summary>
        public ref struct Enumerator
        {
            private readonly Entry[] entries;
            private int index;
            private int count;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(DictionarySlim<TKey, TValue> dictionary)
            {
                this.entries = dictionary.entries;
                this.index = 0;
                this.count = dictionary.count;
            }

            /// <inheritdoc cref="IEnumerator.MoveNext"/>
            public bool MoveNext()
            {
                if (this.count == 0)
                {
                    return false;
                }

                this.count--;

                Entry[] entries = this.entries;

                while (entries[this.index].Next < -1)
                {
                    this.index++;
                }

                // We need to preemptively increment the current index so that we still correctly keep track
                // of the current position in the dictionary even if the users doesn't access any of the
                // available properties in the enumerator. As this is a possibility, we can't rely on one of
                // them to increment the index before MoveNext is invoked again. We ditch the standard enumerator
                // API surface here to expose the Key/Value properties directly and minimize the memory copies.
                // For the same reason, we also removed the KeyValuePair<TKey, TValue> field here, and instead
                // rely on the properties lazily accessing the target instances directly from the current entry
                // pointed at by the index property (adjusted backwards to account for the increment here).
                this.index++;

                return true;
            }

            /// <summary>
            /// Gets the current key.
            /// </summary>
            public TKey Key
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this.entries[this.index - 1].Key;
            }

            /// <summary>
            /// Gets the current value.
            /// </summary>
            public TValue Value
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this.entries[this.index - 1].Value!;
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when trying to load an element with a missing key.
        /// </summary>
        private static void ThrowArgumentExceptionForKeyNotFound(TKey key)
        {
            throw new ArgumentException($"The target key {key} was not present in the dictionary");
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> when trying to resize over the maximum capacity.
        /// </summary>
        private static void ThrowInvalidOperationExceptionForMaxCapacityExceeded()
        {
            throw new InvalidOperationException("Max capacity exceeded");
        }
    }
}