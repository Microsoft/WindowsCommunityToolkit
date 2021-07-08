// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <inheritdoc />
    /// <summary>A wrapper class for <see cref="TextBlock.Inlines">TextBlock.Inlines</see> to
    /// hack the problem that <see cref="Microsoft.UI.Xaml.Documents.InlineCollection" />.
    /// has no accessible constructor</summary>
    public class InlineCollectionWrapper : IList<Inline>
    {
        private IList<Inline> _collection;

        internal InlineCollectionWrapper()
        {
            _collection = new List<Inline>();
        }

        /// <inheritdoc />
        public int Count => _collection.Count;

        /// <inheritdoc />
        public bool IsReadOnly => _collection.IsReadOnly;

        /// <inheritdoc />
        public Inline this[int index]
        {
            get => _collection[index];
            set => _collection[index] = value;
        }

        /// <inheritdoc />
        public void Add(Inline item)
        {
            _collection.Add(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            _collection.Clear();
        }

        /// <inheritdoc />
        public bool Contains(Inline item)
        {
            return _collection.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(Inline[] array, int arrayIndex)
        {
            _collection.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public IEnumerator<Inline> GetEnumerator()
        {
            foreach (var inline in _collection)
            {
                yield return inline;
            }
        }

        /// <inheritdoc />
        public int IndexOf(Inline item)
        {
            return _collection.IndexOf(item);
        }

        /// <inheritdoc />
        public void Insert(int index, Inline item)
        {
            _collection.Insert(index, item);
        }

        /// <inheritdoc />
        public bool Remove(Inline item)
        {
            return _collection.Remove(item);
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            _collection.RemoveAt(index);
        }

        /// <summary>
        /// Sets the items of this collection as <see cref="TextBlock.Inlines"/> to <paramref name="textBlock"/>.
        /// </summary>
        /// <param name="textBlock">The textBlock where the items are added.</param>
        internal void AddItemsToTextBlock(TextBlock textBlock)
        {
            if (textBlock == null)
            {
                throw new ArgumentNullException(nameof(textBlock));
            }

            foreach (var inline in _collection)
            {
                textBlock.Inlines.Add(inline);
            }

            _collection = textBlock.Inlines;
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}