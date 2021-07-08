// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// This class encapsulates a selected row's information necessary for the CopyingRowClipboardContent event.
    /// </summary>
    public class DataGridRowClipboardEventArgs : EventArgs
    {
        private List<DataGridClipboardCellContent> _clipboardRowContent;
        private bool _isColumnHeadersRow;
        private object _item;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridRowClipboardEventArgs"/> class.
        /// </summary>
        /// <param name="item">The row's associated data item.</param>
        /// <param name="isColumnHeadersRow">Whether or not this EventArgs is for the column headers.</param>
        internal DataGridRowClipboardEventArgs(object item, bool isColumnHeadersRow)
        {
            _isColumnHeadersRow = isColumnHeadersRow;
            _item = item;
        }

        /// <summary>
        /// Gets a list used to modify, add or remove a cell content before it gets stored into the clipboard.
        /// </summary>
        public List<DataGridClipboardCellContent> ClipboardRowContent
        {
            get
            {
                if (_clipboardRowContent == null)
                {
                    _clipboardRowContent = new List<DataGridClipboardCellContent>();
                }

                return _clipboardRowContent;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this property is true when the ClipboardRowContent represents column headers, in which case the Item is null.
        /// </summary>
        public bool IsColumnHeadersRow
        {
            get
            {
                return _isColumnHeadersRow;
            }
        }

        /// <summary>
        /// Gets the <see cref="DataGrid"/> row item used for preparing the ClipboardRowContent.
        /// </summary>
        public object Item
        {
            get
            {
                return _item;
            }
        }
    }
}