// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text;
#if !NETSTANDARD1_4
using CommunityToolkit.HighPerformance.Helpers;
#endif
using BitOperations = CommunityToolkit.HighPerformance.Helpers.Internals.BitOperations;

#nullable enable

namespace CommunityToolkit.HighPerformance.Buffers
{
    /// <summary>
    /// A configurable pool for <see cref="string"/> instances. This can be used to minimize allocations
    /// when creating multiple <see cref="string"/> instances from buffers of <see cref="char"/> values.
    /// The <see cref="GetOrAdd(ReadOnlySpan{char})"/> method provides a best-effort alternative to just creating
    /// a new <see cref="string"/> instance every time, in order to minimize the number of duplicated instances.
    /// The <see cref="StringPool"/> type will internally manage a highly efficient priority queue for the
    /// cached <see cref="string"/> instances, so that when the full capacity is reached, the least frequently
    /// used values will be automatically discarded to leave room for new values to cache.
    /// </summary>
    public sealed class StringPool
    {
        /// <summary>
        /// The size used by default by the parameterless constructor.
        /// </summary>
        private const int DefaultSize = 2048;

        /// <summary>
        /// The minimum size for <see cref="StringPool"/> instances.
        /// </summary>
        private const int MinimumSize = 32;

        /// <summary>
        /// The current array of <see cref="FixedSizePriorityMap"/> instances in use.
        /// </summary>
        private readonly FixedSizePriorityMap[] maps;

        /// <summary>
        /// The total number of maps in use.
        /// </summary>
        private readonly int numberOfMaps;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringPool"/> class.
        /// </summary>
        public StringPool()
            : this(DefaultSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringPool"/> class.
        /// </summary>
        /// <param name="minimumSize">The minimum size for the pool to create.</param>
        public StringPool(int minimumSize)
        {
            if (minimumSize <= 0)
            {
                ThrowArgumentOutOfRangeException();
            }

            // Set the minimum size
            minimumSize = Math.Max(minimumSize, MinimumSize);

            // Calculates the rounded up factors for a specific size/factor pair
            static void FindFactors(int size, int factor, out int x, out int y)
            {
                double
                    a = Math.Sqrt((double)size / factor),
                    b = factor * a;

                x = BitOperations.RoundUpPowerOfTwo((int)a);
                y = BitOperations.RoundUpPowerOfTwo((int)b);
            }

            // We want to find two powers of 2 factors that produce a number
            // that is at least equal to the requested size. In order to find the
            // combination producing the optimal factors (with the product being as
            // close as possible to the requested size), we test a number of ratios
            // that we consider acceptable, and pick the best results produced.
            // The ratio between maps influences the number of objects being allocated,
            // as well as the multithreading performance when locking on maps.
            // We still want to contraint this number to avoid situations where we
            // have a way too high number of maps compared to total size.
            FindFactors(minimumSize, 2, out int x2, out int y2);
            FindFactors(minimumSize, 3, out int x3, out int y3);
            FindFactors(minimumSize, 4, out int x4, out int y4);

            int
                p2 = x2 * y2,
                p3 = x3 * y3,
                p4 = x4 * y4;

            if (p3 < p2)
            {
                p2 = p3;
                x2 = x3;
                y2 = y3;
            }

            if (p4 < p2)
            {
                p2 = p4;
                x2 = x4;
                y2 = y4;
            }

            Span<FixedSizePriorityMap> span = this.maps = new FixedSizePriorityMap[x2];

            // We preallocate the maps in advance, since each bucket only contains the
            // array field, which is not preinitialized, so the allocations are minimal.
            // This lets us lock on each individual maps when retrieving a string instance.
            foreach (ref FixedSizePriorityMap map in span)
            {
                map = new FixedSizePriorityMap(y2);
            }

            this.numberOfMaps = x2;

            Size = p2;
        }

        /// <summary>
        /// Gets the shared <see cref="StringPool"/> instance.
        /// </summary>
        /// <remarks>
        /// The shared pool provides a singleton, reusable <see cref="StringPool"/> instance that
        /// can be accessed directly, and that pools <see cref="string"/> instances for the entire
        /// process. Since <see cref="StringPool"/> is thread-safe, the shared instance can be used
        /// concurrently by multiple threads without the need for manual synchronization.
        /// </remarks>
        public static StringPool Shared { get; } = new();

        /// <summary>
        /// Gets the total number of <see cref="string"/> that can be stored in the current instance.
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// Stores a <see cref="string"/> instance in the internal cache.
        /// </summary>
        /// <param name="value">The input <see cref="string"/> instance to cache.</param>
        public void Add(string value)
        {
            if (value.Length == 0)
            {
                return;
            }

            int
                hashcode = GetHashCode(value.AsSpan()),
                bucketIndex = hashcode & (this.numberOfMaps - 1);

            ref FixedSizePriorityMap map = ref this.maps.DangerousGetReferenceAt(bucketIndex);

            lock (map.SyncRoot)
            {
                map.Add(value, hashcode);
            }
        }

        /// <summary>
        /// Gets a cached <see cref="string"/> instance matching the input content, or stores the input one.
        /// </summary>
        /// <param name="value">The input <see cref="string"/> instance with the contents to use.</param>
        /// <returns>A <see cref="string"/> instance with the contents of <paramref name="value"/>, cached if possible.</returns>
        public string GetOrAdd(string value)
        {
            if (value.Length == 0)
            {
                return string.Empty;
            }

            int
                hashcode = GetHashCode(value.AsSpan()),
                bucketIndex = hashcode & (this.numberOfMaps - 1);

            ref FixedSizePriorityMap map = ref this.maps.DangerousGetReferenceAt(bucketIndex);

            lock (map.SyncRoot)
            {
                return map.GetOrAdd(value, hashcode);
            }
        }

        /// <summary>
        /// Gets a cached <see cref="string"/> instance matching the input content, or creates a new one.
        /// </summary>
        /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> with the contents to use.</param>
        /// <returns>A <see cref="string"/> instance with the contents of <paramref name="span"/>, cached if possible.</returns>
        public string GetOrAdd(ReadOnlySpan<char> span)
        {
            if (span.IsEmpty)
            {
                return string.Empty;
            }

            int
                hashcode = GetHashCode(span),
                bucketIndex = hashcode & (this.numberOfMaps - 1);

            ref FixedSizePriorityMap map = ref this.maps.DangerousGetReferenceAt(bucketIndex);

            lock (map.SyncRoot)
            {
                return map.GetOrAdd(span, hashcode);
            }
        }

        /// <summary>
        /// Gets a cached <see cref="string"/> instance matching the input content (converted to Unicode), or creates a new one.
        /// </summary>
        /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> with the contents to use, in a specified encoding.</param>
        /// <param name="encoding">The <see cref="Encoding"/> instance to use to decode the contents of <paramref name="span"/>.</param>
        /// <returns>A <see cref="string"/> instance with the contents of <paramref name="span"/>, cached if possible.</returns>
        public unsafe string GetOrAdd(ReadOnlySpan<byte> span, Encoding encoding)
        {
            if (span.IsEmpty)
            {
                return string.Empty;
            }

            int maxLength = encoding.GetMaxCharCount(span.Length);

            using SpanOwner<char> buffer = SpanOwner<char>.Allocate(maxLength);

            fixed (byte* source = span)
            fixed (char* destination = &buffer.DangerousGetReference())
            {
                int effectiveLength = encoding.GetChars(source, span.Length, destination, maxLength);

                return GetOrAdd(new ReadOnlySpan<char>(destination, effectiveLength));
            }
        }

        /// <summary>
        /// Tries to get a cached <see cref="string"/> instance matching the input content, if present.
        /// </summary>
        /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> with the contents to use.</param>
        /// <param name="value">The resulting cached <see cref="string"/> instance, if present</param>
        /// <returns>Whether or not the target <see cref="string"/> instance was found.</returns>
        public bool TryGet(ReadOnlySpan<char> span, [NotNullWhen(true)] out string? value)
        {
            if (span.IsEmpty)
            {
                value = string.Empty;

                return true;
            }

            int
                hashcode = GetHashCode(span),
                bucketIndex = hashcode & (this.numberOfMaps - 1);

            ref FixedSizePriorityMap map = ref this.maps.DangerousGetReferenceAt(bucketIndex);

            lock (map.SyncRoot)
            {
                return map.TryGet(span, hashcode, out value);
            }
        }

        /// <summary>
        /// Resets the current instance and its associated maps.
        /// </summary>
        public void Reset()
        {
            foreach (ref FixedSizePriorityMap map in this.maps.AsSpan())
            {
                lock (map.SyncRoot)
                {
                    map.Reset();
                }
            }
        }

        /// <summary>
        /// A configurable map containing a group of cached <see cref="string"/> instances.
        /// </summary>
        /// <remarks>
        /// Instances of this type are stored in an array within <see cref="StringPool"/> and they are
        /// always accessed by reference - essentially as if this type had been a class. The type is
        /// also private, so there's no risk for users to directly access it and accidentally copy an
        /// instance, which would lead to bugs due to values becoming out of sync with the internal state
        /// (that is, because instances would be copied by value, so primitive fields would not be shared).
        /// The reason why we're using a struct here is to remove an indirection level and improve cache
        /// locality when accessing individual buckets from the methods in the <see cref="StringPool"/> type.
        /// </remarks>
        private struct FixedSizePriorityMap
        {
            /// <summary>
            /// The index representing the end of a given list.
            /// </summary>
            private const int EndOfList = -1;

            /// <summary>
            /// The array of 1-based indices for the <see cref="MapEntry"/> items stored in <see cref="mapEntries"/>.
            /// </summary>
            private readonly int[] buckets;

            /// <summary>
            /// The array of currently cached entries (ie. the lists for each hash group).
            /// </summary>
            private readonly MapEntry[] mapEntries;

            /// <summary>
            /// The array of priority values associated to each item stored in <see cref="mapEntries"/>.
            /// </summary>
            private readonly HeapEntry[] heapEntries;

            /// <summary>
            /// The current number of items stored in the map.
            /// </summary>
            private int count;

            /// <summary>
            /// The current incremental timestamp for the items stored in <see cref="heapEntries"/>.
            /// </summary>
            private uint timestamp;

            /// <summary>
            /// A type representing a map entry, ie. a node in a given list.
            /// </summary>
            private struct MapEntry
            {
                /// <summary>
                /// The precomputed hashcode for <see cref="Value"/>.
                /// </summary>
                public int HashCode;

                /// <summary>
                /// The <see cref="string"/> instance cached in this entry.
                /// </summary>
                public string? Value;

                /// <summary>
                /// The 0-based index for the next node in the current list.
                /// </summary>
                public int NextIndex;

                /// <summary>
                /// The 0-based index for the heap entry corresponding to the current node.
                /// </summary>
                public int HeapIndex;
            }

            /// <summary>
            /// A type representing a heap entry, used to associate priority to each item.
            /// </summary>
            private struct HeapEntry
            {
                /// <summary>
                /// The timestamp for the current entry (ie. the priority for the item).
                /// </summary>
                public uint Timestamp;

                /// <summary>
                /// The 0-based index for the map entry corresponding to the current item.
                /// </summary>
                public int MapIndex;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="FixedSizePriorityMap"/> struct.
            /// </summary>
            /// <param name="capacity">The fixed capacity of the current map.</param>
            public FixedSizePriorityMap(int capacity)
            {
                this.buckets = new int[capacity];
                this.mapEntries = new MapEntry[capacity];
                this.heapEntries = new HeapEntry[capacity];
                this.count = 0;
                this.timestamp = 0;
            }

            /// <summary>
            /// Gets an <see cref="object"/> that can be used to synchronize access to the current instance.
            /// </summary>
            public object SyncRoot
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this.buckets;
            }

            /// <summary>
            /// Implements <see cref="StringPool.Add"/> for the current instance.
            /// </summary>
            /// <param name="value">The input <see cref="string"/> instance to cache.</param>
            /// <param name="hashcode">The precomputed hashcode for <paramref name="value"/>.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add(string value, int hashcode)
            {
                ref string target = ref TryGet(value.AsSpan(), hashcode);

                if (Unsafe.IsNullRef(ref target))
                {
                    Insert(value, hashcode);
                }
                else
                {
                    target = value;
                }
            }

            /// <summary>
            /// Implements <see cref="StringPool.GetOrAdd(string)"/> for the current instance.
            /// </summary>
            /// <param name="value">The input <see cref="string"/> instance with the contents to use.</param>
            /// <param name="hashcode">The precomputed hashcode for <paramref name="value"/>.</param>
            /// <returns>A <see cref="string"/> instance with the contents of <paramref name="value"/>.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public string GetOrAdd(string value, int hashcode)
            {
                ref string result = ref TryGet(value.AsSpan(), hashcode);

                if (!Unsafe.IsNullRef(ref result))
                {
                    return result;
                }

                Insert(value, hashcode);

                return value;
            }

            /// <summary>
            /// Implements <see cref="StringPool.GetOrAdd(ReadOnlySpan{char})"/> for the current instance.
            /// </summary>
            /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> with the contents to use.</param>
            /// <param name="hashcode">The precomputed hashcode for <paramref name="span"/>.</param>
            /// <returns>A <see cref="string"/> instance with the contents of <paramref name="span"/>, cached if possible.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public string GetOrAdd(ReadOnlySpan<char> span, int hashcode)
            {
                ref string result = ref TryGet(span, hashcode);

                if (!Unsafe.IsNullRef(ref result))
                {
                    return result;
                }

                string value = span.ToString();

                Insert(value, hashcode);

                return value;
            }

            /// <summary>
            /// Implements <see cref="StringPool.TryGet"/> for the current instance.
            /// </summary>
            /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> with the contents to use.</param>
            /// <param name="hashcode">The precomputed hashcode for <paramref name="span"/>.</param>
            /// <param name="value">The resulting cached <see cref="string"/> instance, if present</param>
            /// <returns>Whether or not the target <see cref="string"/> instance was found.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryGet(ReadOnlySpan<char> span, int hashcode, [NotNullWhen(true)] out string? value)
            {
                ref string result = ref TryGet(span, hashcode);

                if (!Unsafe.IsNullRef(ref result))
                {
                    value = result;

                    return true;
                }

                value = null;

                return false;
            }

            /// <summary>
            /// Resets the current instance and throws away all the cached values.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset()
            {
                this.buckets.AsSpan().Clear();
                this.mapEntries.AsSpan().Clear();
                this.heapEntries.AsSpan().Clear();
                this.count = 0;
                this.timestamp = 0;
            }

            /// <summary>
            /// Tries to get a target <see cref="string"/> instance, if it exists, and returns a reference to it.
            /// </summary>
            /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> with the contents to use.</param>
            /// <param name="hashcode">The precomputed hashcode for <paramref name="span"/>.</param>
            /// <returns>A reference to the slot where the target <see cref="string"/> instance could be.</returns>
            [MethodImpl(MethodImplOptions.NoInlining)]
            private unsafe ref string TryGet(ReadOnlySpan<char> span, int hashcode)
            {
                ref MapEntry mapEntriesRef = ref this.mapEntries.DangerousGetReference();
                ref MapEntry entry = ref Unsafe.NullRef<MapEntry>();
                int
                    length = this.buckets.Length,
                    bucketIndex = hashcode & (length - 1);

                for (int i = this.buckets.DangerousGetReferenceAt(bucketIndex) - 1;
                     (uint)i < (uint)length;
                     i = entry.NextIndex)
                {
                    entry = ref Unsafe.Add(ref mapEntriesRef, (nint)(uint)i);

                    if (entry.HashCode == hashcode &&
                        entry.Value!.AsSpan().SequenceEqual(span))
                    {
                        UpdateTimestamp(ref entry.HeapIndex);

                        return ref entry.Value!;
                    }
                }

                return ref Unsafe.NullRef<string>();
            }

            /// <summary>
            /// Inserts a new <see cref="string"/> instance in the current map, freeing up a space if needed.
            /// </summary>
            /// <param name="value">The new <see cref="string"/> instance to store.</param>
            /// <param name="hashcode">The precomputed hashcode for <paramref name="value"/>.</param>
            [MethodImpl(MethodImplOptions.NoInlining)]
            private void Insert(string value, int hashcode)
            {
                ref int bucketsRef = ref this.buckets.DangerousGetReference();
                ref MapEntry mapEntriesRef = ref this.mapEntries.DangerousGetReference();
                ref HeapEntry heapEntriesRef = ref this.heapEntries.DangerousGetReference();
                int entryIndex, heapIndex;

                // If the current map is full, first get the oldest value, which is
                // always the first item in the heap. Then, free up a slot by
                // removing that, and insert the new value in that empty location.
                if (this.count == this.mapEntries.Length)
                {
                    entryIndex = heapEntriesRef.MapIndex;
                    heapIndex = 0;

                    ref MapEntry removedEntry = ref Unsafe.Add(ref mapEntriesRef, (nint)(uint)entryIndex);

                    // The removal logic can be extremely optimized in this case, as we
                    // can retrieve the precomputed hashcode for the target entry by doing
                    // a lookup on the target map node, and we can also skip all the comparisons
                    // while traversing the target chain since we know in advance the index of
                    // the target node which will contain the item to remove from the map.
                    Remove(removedEntry.HashCode, entryIndex);
                }
                else
                {
                    // If the free list is not empty, get that map node and update the field
                    entryIndex = this.count;
                    heapIndex = this.count;
                }

                int bucketIndex = hashcode & (this.buckets.Length - 1);
                ref int targetBucket = ref Unsafe.Add(ref bucketsRef, (nint)(uint)bucketIndex);
                ref MapEntry targetMapEntry = ref Unsafe.Add(ref mapEntriesRef, (nint)(uint)entryIndex);
                ref HeapEntry targetHeapEntry = ref Unsafe.Add(ref heapEntriesRef, (nint)(uint)heapIndex);

                // Assign the values in the new map entry
                targetMapEntry.HashCode = hashcode;
                targetMapEntry.Value = value;
                targetMapEntry.NextIndex = targetBucket - 1;
                targetMapEntry.HeapIndex = heapIndex;

                // Update the bucket slot and the current count
                targetBucket = entryIndex + 1;
                this.count++;

                // Link the heap node with the current entry
                targetHeapEntry.MapIndex = entryIndex;

                // Update the timestamp and balance the heap again
                UpdateTimestamp(ref targetMapEntry.HeapIndex);
            }

            /// <summary>
            /// Removes a specified <see cref="string"/> instance from the map to free up one slot.
            /// </summary>
            /// <param name="hashcode">The precomputed hashcode of the instance to remove.</param>
            /// <param name="mapIndex">The index of the target map node to remove.</param>
            /// <remarks>The input <see cref="string"/> instance needs to already exist in the map.</remarks>
            [MethodImpl(MethodImplOptions.NoInlining)]
            private void Remove(int hashcode, int mapIndex)
            {
                ref MapEntry mapEntriesRef = ref this.mapEntries.DangerousGetReference();
                int
                    bucketIndex = hashcode & (this.buckets.Length - 1),
                    entryIndex = this.buckets.DangerousGetReferenceAt(bucketIndex) - 1,
                    lastIndex = EndOfList;

                // We can just have an undefined loop, as the input
                // value we're looking for is guaranteed to be present
                while (true)
                {
                    ref MapEntry candidate = ref Unsafe.Add(ref mapEntriesRef, (nint)(uint)entryIndex);

                    // Check the current value for a match
                    if (entryIndex == mapIndex)
                    {
                        // If this was not the first list node, update the parent as well
                        if (lastIndex != EndOfList)
                        {
                            ref MapEntry lastEntry = ref Unsafe.Add(ref mapEntriesRef, (nint)(uint)lastIndex);

                            lastEntry.NextIndex = candidate.NextIndex;
                        }
                        else
                        {
                            // Otherwise, update the target index from the bucket slot
                            this.buckets.DangerousGetReferenceAt(bucketIndex) = candidate.NextIndex + 1;
                        }

                        this.count--;

                        return;
                    }

                    // Move to the following node in the current list
                    lastIndex = entryIndex;
                    entryIndex = candidate.NextIndex;
                }
            }

            /// <summary>
            /// Updates the timestamp of a heap node at the specified index (which is then synced back).
            /// </summary>
            /// <param name="heapIndex">The index of the target heap node to update.</param>
            [MethodImpl(MethodImplOptions.NoInlining)]
            private void UpdateTimestamp(ref int heapIndex)
            {
                int
                    currentIndex = heapIndex,
                    count = this.count;
                ref MapEntry mapEntriesRef = ref this.mapEntries.DangerousGetReference();
                ref HeapEntry heapEntriesRef = ref this.heapEntries.DangerousGetReference();
                ref HeapEntry root = ref Unsafe.Add(ref heapEntriesRef, (nint)(uint)currentIndex);
                uint timestamp = this.timestamp;

                // Check if incrementing the current timestamp for the heap node to update
                // would result in an overflow. If that happened, we could end up violating
                // the min-heap property (the value of each node has to always be <= than that
                // of its child nodes), eg. if we were updating a node that was not the root.
                // In that scenario, we could end up with a node somewhere along the heap with
                // a value lower than that of its parent node (as the timestamp would be 0).
                // To guard against this, we just check the current timestamp value, and if
                // the maximum value has been reached, we reinitialize the entire heap. This
                // is done in a non-inlined call, so we don't increase the codegen size in this
                // method. The reinitialization simply traverses the heap in breadth-first order
                // (ie. level by level), and assigns incrementing timestamps to all nodes starting
                // from 0. The value of the current timestamp is then just set to the current size.
                if (timestamp == uint.MaxValue)
                {
                    // We use a goto here as this path is very rarely taken. Doing so
                    // causes the generated asm to contain a forward jump to the fallback
                    // path if this branch is taken, whereas the normal execution path will
                    // not need to execute any jumps at all. This is done to reduce the overhead
                    // introduced by this check in all the invocations where this point is not reached.
                    goto Fallback;
                }

                Downheap:

                // Assign a new timestamp to the target heap node. We use a
                // local incremental timestamp instead of using the system timer
                // as this greatly reduces the overhead and the time spent in system calls.
                // The uint type provides a large enough range and it's unlikely users would ever
                // exhaust it anyway (especially considering each map has a separate counter).
                root.Timestamp = this.timestamp = timestamp + 1;

                // Once the timestamp is updated (which will cause the heap to become
                // unbalanced), start a sift down loop to balance the heap again.
                while (true)
                {
                    // The heap is 0-based (so that the array length can remain the same
                    // as the power of 2 value used for the other arrays in this type).
                    // This means that children of each node are at positions:
                    //   - left: (2 * n) + 1
                    //   - right: (2 * n) + 2
                    ref HeapEntry minimum = ref root;
                    int
                        left = (currentIndex * 2) + 1,
                        right = (currentIndex * 2) + 2,
                        targetIndex = currentIndex;

                    // Check and update the left child, if necessary
                    if (left < count)
                    {
                        ref HeapEntry child = ref Unsafe.Add(ref heapEntriesRef, (nint)(uint)left);

                        if (child.Timestamp < minimum.Timestamp)
                        {
                            minimum = ref child;
                            targetIndex = left;
                        }
                    }

                    // Same check as above for the right child
                    if (right < count)
                    {
                        ref HeapEntry child = ref Unsafe.Add(ref heapEntriesRef, (nint)(uint)right);

                        if (child.Timestamp < minimum.Timestamp)
                        {
                            minimum = ref child;
                            targetIndex = right;
                        }
                    }

                    // If no swap is pending, we can just stop here.
                    // Before returning, we update the target index as well.
                    if (Unsafe.AreSame(ref root, ref minimum))
                    {
                        heapIndex = targetIndex;

                        return;
                    }

                    // Update the indices in the respective map entries (accounting for the swap)
                    Unsafe.Add(ref mapEntriesRef, (nint)(uint)root.MapIndex).HeapIndex = targetIndex;
                    Unsafe.Add(ref mapEntriesRef, (nint)(uint)minimum.MapIndex).HeapIndex = currentIndex;

                    currentIndex = targetIndex;

                    // Swap the parent and child (so that the minimum value bubbles up)
                    HeapEntry temp = root;

                    root = minimum;
                    minimum = temp;

                    // Update the reference to the root node
                    root = ref Unsafe.Add(ref heapEntriesRef, (nint)(uint)currentIndex);
                }

                Fallback:

                UpdateAllTimestamps();

                // After having updated all the timestamps, if the heap contains N items, the
                // node in the bottom right corner will have a value of N - 1. Since the timestamp
                // is incremented by 1 before starting the downheap execution, here we simply
                // update the local timestamp to be N - 1, so that the code above will set the
                // timestamp of the node currently being updated to exactly N.
                timestamp = (uint)(count - 1);

                goto Downheap;
            }

            /// <summary>
            /// Updates the timestamp of all the current heap nodes in incrementing order.
            /// The heap is always guaranteed to be complete binary tree, so when it contains
            /// a given number of nodes, those are all contiguous from the start of the array.
            /// </summary>
            [MethodImpl(MethodImplOptions.NoInlining)]
            private void UpdateAllTimestamps()
            {
                int count = this.count;
                ref HeapEntry heapEntriesRef = ref this.heapEntries.DangerousGetReference();

                for (int i = 0; i < count; i++)
                {
                    Unsafe.Add(ref heapEntriesRef, (nint)(uint)i).Timestamp = (uint)i;
                }
            }
        }

        /// <summary>
        /// Gets the (positive) hashcode for a given <see cref="ReadOnlySpan{T}"/> instance.
        /// </summary>
        /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance.</param>
        /// <returns>The hashcode for <paramref name="span"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetHashCode(ReadOnlySpan<char> span)
        {
#if NETSTANDARD1_4
            return span.GetDjb2HashCode();
#else
            return HashCode<char>.Combine(span);
#endif
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when the requested size exceeds the capacity.
        /// </summary>
        private static void ThrowArgumentOutOfRangeException()
        {
            throw new ArgumentOutOfRangeException("minimumSize", "The requested size must be greater than 0");
        }
    }
}