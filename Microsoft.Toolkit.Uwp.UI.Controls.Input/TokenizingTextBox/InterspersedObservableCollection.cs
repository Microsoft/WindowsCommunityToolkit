// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Toolkit.Uwp.Helpers;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    //// We need to implement the IList interface here for ListViewBase to listen to changes - https://github.com/microsoft/microsoft-ui-xaml/issues/1809

    internal class InterspersedObservableCollection : IList, IEnumerable<object>, INotifyCollectionChanged
    {
        public IList ItemsSource { get; private set; }

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        public int Count => ItemsSource.Count + _interspersedObjects.Count;

        public bool IsSynchronized => false;

        public object SyncRoot => new object();

        public object this[int index]
        {
            get
            {
                if (_interspersedObjects.TryGetValue(index, out var value))
                {
                    return value;
                }
                else
                {
                    // Find out the number of elements in our dictionary with keys below ours.
                    return ItemsSource[ToInnerIndex(index)];
                }
            }
            set => throw new NotImplementedException();
        }

        private Dictionary<int?, object> _interspersedObjects = new Dictionary<int?, object>();
        private bool _isInsertingOriginal = false;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public InterspersedObservableCollection(object itemsSource)
        {
            if (!(itemsSource is IList list))
            {
                ThrowArgumentException();
            }

            ItemsSource = list;

            if (ItemsSource is INotifyCollectionChanged notifier)
            {
                var weakPropertyChangedListener = new WeakEventListener<InterspersedObservableCollection, object, NotifyCollectionChangedEventArgs>(this)
                {
                    OnEventAction = (instance, source, eventArgs) => instance.ItemsSource_CollectionChanged(source, eventArgs),
                    OnDetachAction = (weakEventListener) => notifier.CollectionChanged -= weakEventListener.OnEvent // Use Local Reference Only
                };
                notifier.CollectionChanged += weakPropertyChangedListener.OnEvent;
            }

            static void ThrowArgumentException() => throw new ArgumentNullException("The input items source must be assignable to the System.Collections.IList type.");
        }

        private void ItemsSource_CollectionChanged(object source, NotifyCollectionChangedEventArgs eventArgs)
        {
            switch (eventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    // Shift any existing interspersed items after the inserted item
                    var count = eventArgs.NewItems.Count;

                    if (count > 0)
                    {
                        if (!_isInsertingOriginal)
                        {
                            MoveKeysForward(eventArgs.NewStartingIndex, count);
                        }

                        _isInsertingOriginal = false;

                        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Add,
                            eventArgs.NewItems,
                            ToOuterIndex(eventArgs.NewStartingIndex)));
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    count = eventArgs.OldItems.Count;

                    if (count > 0)
                    {
                        var outerIndex = ToOuterIndexAfterRemoval(eventArgs.OldStartingIndex);

                        MoveKeysBackward(outerIndex, count);

                        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Remove,
                            eventArgs.OldItems,
                            outerIndex));
                    }

                    break;
                case NotifyCollectionChangedAction.Reset:

                    ReadjustKeys();

                    // TODO: ListView doesn't like this notification and throws a visual tree duplication exception...
                    // Not sure what to do with that yet...
                    CollectionChanged?.Invoke(this, eventArgs);
                    break;
            }
        }

        /// <summary>
        /// Moves our interspersed keys at or past the given index forward by the amount.
        /// </summary>
        /// <param name="pivot">index of added item</param>
        /// <param name="amount">by how many</param>
        private void MoveKeysForward(int pivot, int amount)
        {
            // Sort in reverse order to work from highest to lowest
            var keys = _interspersedObjects.Keys.OrderByDescending(v => v).ToArray();
            foreach (var key in keys)
            {
                if (key < pivot) //// If it's the last item in the collection, we still want to move our last key, otherwise we'd use <=
                {
                    break;
                }

                _interspersedObjects[key + amount] = _interspersedObjects[key];
                _interspersedObjects.Remove(key);
            }
        }

        /// <summary>
        /// Moves our interspersed keys at or past the given index backward by the amount.
        /// </summary>
        /// <param name="pivot">index of removed item</param>
        /// <param name="amount">by how many</param>
        private void MoveKeysBackward(int pivot, int amount)
        {
            // Sort in regular order to work from the earliest indices onwards
            var keys = _interspersedObjects.Keys.OrderBy(v => v).ToArray();
            foreach (var key in keys)
            {
                // Skip elements before the pivot point
                if (key <= pivot) //// Include pivot point as that's the point where we start modifying beyond
                {
                    continue;
                }

                _interspersedObjects[key - amount] = _interspersedObjects[key];
                _interspersedObjects.Remove(key);
            }
        }

        /// <summary>
        /// Condenses our interspersed keys around any remaining items, mainly for when the original collection is reset.
        /// </summary>
        private void ReadjustKeys()
        {
            var count = ItemsSource.Count;
            var existing = 0;

            var keys = _interspersedObjects.Keys.OrderBy(v => v).ToArray();
            foreach (var key in keys)
            {
                if (key <= count)
                {
                    existing++;
                    continue;
                }

                _interspersedObjects[count + existing++] = _interspersedObjects[key];
                _interspersedObjects.Remove(key);
            }
        }

        /// <summary>
        /// Takes an index from the entire collection and maps it to the <see cref="ItemsSource"/> inner collection index. Assumes, mapping is valid.
        /// </summary>
        /// <param name="outerIndex">Index into the entire collection.</param>
        /// <returns>Inner ItemsSource Index.</returns>
        private int ToInnerIndex(int outerIndex)
        {
            if ((uint)outerIndex >= Count)
            {
                ThrowArgumentOutOfRangeException();
            }

            if (_interspersedObjects.ContainsKey(outerIndex))
            {
                ThrowArgumentException();
            }

            return outerIndex - _interspersedObjects.Keys.Count(key => key.Value <= outerIndex);

            static void ThrowArgumentOutOfRangeException() => throw new ArgumentOutOfRangeException(nameof(outerIndex));
            static void ThrowArgumentException() => throw new ArgumentException("The outer index can't be inserted as a key to the original collection.");
        }

        /// <summary>
        /// Takes an index from the <see cref="ItemsSource"/> inner collection and maps it to an index for this entire collection.
        /// </summary>
        /// <param name="innerIndex">Index into the ItemsSource.</param>
        /// <returns>Index into the entire collection.</returns>
        private int ToOuterIndex(int innerIndex)
        {
            if ((uint)innerIndex >= ItemsSource.Count)
            {
                ThrowArgumentOutOfRangeException();
            }

            var keys = _interspersedObjects.OrderBy(v => v.Key);

            foreach (var key in keys)
            {
                if (innerIndex >= key.Key)
                {
                    innerIndex++;
                }
                else
                {
                    break;
                }
            }

            return innerIndex;

            static void ThrowArgumentOutOfRangeException() => throw new ArgumentOutOfRangeException(nameof(innerIndex));
        }

        /// <summary>
        /// Takes an index from the <see cref="ItemsSource"/> inner collection and maps it to an index for this entire collection, projects as if an element from the provided index was still in the collection.
        /// </summary>
        /// <param name="innerIndexToProject">Previous index from ItemsSource</param>
        /// <returns>Projected index in the entire collection.</returns>
        private int ToOuterIndexAfterRemoval(int innerIndexToProject)
        {
            if ((uint)innerIndexToProject >= ItemsSource.Count + 1)
            {
                ThrowArgumentOutOfRangeException();
            }

            //// TODO: Deal with bounds (0 / Count)? Or is it the same?

            var keys = _interspersedObjects.OrderBy(v => v.Key);

            foreach (var key in keys)
            {
                if (innerIndexToProject >= key.Key)
                {
                    innerIndexToProject++;
                }
                else
                {
                    break;
                }
            }

            return innerIndexToProject;

            static void ThrowArgumentOutOfRangeException() => throw new ArgumentOutOfRangeException(nameof(innerIndexToProject));
        }

        /// <summary>
        /// Inserts an item to intersperse with the underlying collection, but not be part of the underlying collection itself.
        /// </summary>
        /// <param name="index">Position to insert the item at.</param>
        /// <param name="obj">Item to intersperse</param>
        public void Insert(int index, object obj)
        {
            MoveKeysForward(index, 1); // Move existing keys at index over to make room for new item

            _interspersedObjects[index] = obj;

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, obj, index));
        }

        /// <summary>
        /// Inserts an item into the underlying collection and moves interspersed items such that the provide item will appear at the provided index as part of the whole collection.
        /// </summary>
        /// <param name="outerIndex">Position to insert the item at.</param>
        /// <param name="obj">Item to place in wrapped collection.</param>
        public void InsertAt(int outerIndex, object obj)
        {
            // Find out our closest index based on interspersed keys
            var index = outerIndex - _interspersedObjects.Keys.Count(key => key.Value < outerIndex); // Note: we exclude the = from ToInnerIndex here

            // If we're inserting where we would normally, then just do that, otherwise we need extra room to not move other keys
            if (index != outerIndex)
            {
                MoveKeysForward(outerIndex, 1); // Skip over until the current spot unlike normal

                _isInsertingOriginal = true; // Prevent Collection callback from moving keys forward on insert
            }

            // Insert into original collection
            ItemsSource.Insert(index, obj);

            // TODO: handle manipulation/notification if not observable
        }

        public IEnumerator<object> GetEnumerator()
        {
            int i = 0; // Index of our current 'virtual' position
            int count = 0;
            int realized = 0;

            foreach (var element in ItemsSource)
            {
                while (_interspersedObjects.TryGetValue(i++, out var obj))
                {
                    realized++; // Track interspersed items used

                    yield return obj;
                }

                count++; // Track original items used

                yield return element;
            }

            // Add any remaining items in our interspersed collection past the index we reached in the original collection
            if (realized < _interspersedObjects.Count)
            {
                // Only select items past our current index, but make sure we've sorted them by their index as well.
                foreach (var keyValue in _interspersedObjects.Where(kvp => kvp.Key >= i).OrderBy(kvp => kvp.Key))
                {
                    yield return keyValue.Value;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Add(object value)
        {
            var index = ItemsSource.Add(value); //// TODO: If the collection isn't observable, we should do manipulations/notifications here...?
            return ToOuterIndex(index);
        }

        public void Clear()
        {
            ItemsSource.Clear();
            _interspersedObjects.Clear();
        }

        public bool Contains(object value)
        {
            return _interspersedObjects.ContainsValue(value) || ItemsSource.Contains(value);
        }

        /// <summary>
        /// Looks up an item's key in the _interspersedObject dictionary by its value. Handles nulls.
        /// </summary>
        /// <param name="value">Search value</param>
        /// <returns>KeyValuePair or default KeyValuePair</returns>
        private KeyValuePair<int?, object> ItemKeySearch(object value)
        {
            if (value == null)
            {
                return _interspersedObjects.FirstOrDefault(kvp => kvp.Value == null);
            }

            return _interspersedObjects.FirstOrDefault(kvp => kvp.Value?.Equals(value) == true);
        }

        public int IndexOf(object value)
        {
            var item = ItemKeySearch(value);

            if (item.Key != null)
            {
                return item.Key.Value;
            }
            else
            {
                int index = ItemsSource.IndexOf(value);

                // Find out the number of elements in our dictionary with keys below ours.
                return index == -1 ? -1 : ToOuterIndex(index);
            }
        }

        public void Remove(object value)
        {
            var item = ItemKeySearch(value);

            if (item.Key != null)
            {
                _interspersedObjects.Remove(item.Key);

                MoveKeysBackward(item.Key.Value, 1); // Move other interspersed items back

                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item.Value, item.Key.Value));
            }
            else
            {
                ItemsSource.Remove(value); // TODO: If not observable, update indices?
            }
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
    }
}