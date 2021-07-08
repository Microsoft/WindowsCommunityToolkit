// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml.Input;
using Windows.Foundation;
using Windows.UI.Core;

namespace CommunityToolkit.WinUI.UI.Controls.Primitives
{
    internal class DataGridColumnHeaderInteractionInfo
    {
        internal Pointer CapturedPointer
        {
            get;
            set;
        }

        internal DataGridColumn DragColumn
        {
            get;
            set;
        }

        internal DataGridColumnHeader.DragMode DragMode
        {
            get;
            set;
        }

        internal uint DragPointerId
        {
            get;
            set;
        }

        internal Point? DragStart
        {
            get;
            set;
        }

        internal double FrozenColumnsWidth
        {
            get;
            set;
        }

        internal bool HasUserInteraction
        {
            get
            {
                return this.DragMode != DataGridColumnHeader.DragMode.None;
            }
        }

        internal Point? LastPointerPositionHeaders
        {
            get;
            set;
        }

        internal CoreCursor OriginalCursor
        {
            get;
            set;
        }

        internal double OriginalHorizontalOffset
        {
            get;
            set;
        }

        internal double OriginalWidth
        {
            get;
            set;
        }

        internal Point? PressedPointerPositionHeaders
        {
            get;
            set;
        }

        internal uint ResizePointerId
        {
            get;
            set;
        }
    }
}