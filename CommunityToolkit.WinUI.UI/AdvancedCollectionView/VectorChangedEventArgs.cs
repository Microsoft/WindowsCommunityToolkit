// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Foundation.Collections;

namespace CommunityToolkit.WinUI.UI
{
    /// <summary>
    /// Vector changed EventArgs
    /// </summary>
    internal class VectorChangedEventArgs : IVectorChangedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VectorChangedEventArgs"/> class.
        /// </summary>
        /// <param name="cc">collection change type</param>
        /// <param name="index">index of item changed</param>
        /// <param name="item">item changed</param>
        public VectorChangedEventArgs(CollectionChange cc, int index = -1, object item = null)
        {
            CollectionChange = cc;
            Index = (uint)index;
        }

        /// <summary>
        /// Gets the type of change that occurred in the vector.
        /// </summary>
        /// <returns>
        /// The type of change in the vector.
        /// </returns>
        public CollectionChange CollectionChange { get; }

        /// <summary>
        /// Gets the position where the change occurred in the vector.
        /// </summary>
        /// <returns>
        /// The zero-based position where the change occurred in the vector, if applicable.
        /// </returns>
        public uint Index { get; }
    }
}