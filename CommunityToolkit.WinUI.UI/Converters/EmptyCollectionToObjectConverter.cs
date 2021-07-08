// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;

namespace CommunityToolkit.WinUI.UI.Converters
{
    /// <summary>
    /// This class converts a collection size into an other object.
    /// Can be used to convert to bind a visibility, a color or an image to the size of the collection.
    /// </summary>
    public class EmptyCollectionToObjectConverter : EmptyObjectToObjectConverter
    {
        /// <summary>
        /// Checks collection for emptiness.
        /// </summary>
        /// <param name="value">Value to be checked.</param>
        /// <returns>True if value is an empty collection or does not implement IEnumerable, false otherwise.</returns>
        protected override bool CheckValueIsEmpty(object value)
        {
            bool isEmpty = true;
            var collection = value as IEnumerable;
            if (collection != null)
            {
                var enumerator = collection.GetEnumerator();
                isEmpty = !enumerator.MoveNext();
            }

            return isEmpty;
        }
    }
}