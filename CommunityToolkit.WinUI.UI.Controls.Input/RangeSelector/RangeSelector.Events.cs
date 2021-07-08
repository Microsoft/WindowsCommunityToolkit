// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// RangeSelector is a "double slider" control for range values.
    /// </summary>
    public partial class RangeSelector : Control
    {
        /// <summary>
        /// Event raised when lower or upper range thumbs start being dragged.
        /// </summary>
        public event DragStartedEventHandler ThumbDragStarted;

        /// <summary>
        /// Event raised when lower or upper range thumbs end being dragged.
        /// </summary>
        public event DragCompletedEventHandler ThumbDragCompleted;

        /// <summary>
        /// Event raised when lower or upper range values are changed.
        /// </summary>
        public event EventHandler<RangeChangedEventArgs> ValueChanged;

        /// <summary>
        /// Called before the <see cref="ThumbDragStarted"/> event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected virtual void OnThumbDragStarted(DragStartedEventArgs e)
        {
            ThumbDragStarted?.Invoke(this, e);
        }

        /// <summary>
        /// Called before the <see cref="ThumbDragCompleted"/> event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected virtual void OnThumbDragCompleted(DragCompletedEventArgs e)
        {
            ThumbDragCompleted?.Invoke(this, e);
        }

        /// <summary>
        /// Called before the <see cref="ValueChanged"/> event occurs.
        /// </summary>
        /// <param name="e"><see cref="RangeChangedEventArgs"/> event data for the event.</param>
        protected virtual void OnValueChanged(RangeChangedEventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }
    }
}