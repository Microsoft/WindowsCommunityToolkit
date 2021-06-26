// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using Windows.Foundation;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    /// <summary>
    /// EventArgs used to send Gaze events. See <see cref="GazePointer.GazeEvent"/>
    /// </summary>
    public sealed class GazeEventArgs : HandledEventArgs
    {
        /// <summary>
        /// Gets the location of the Gaze event
        /// </summary>
        public Point Location { get; private set; }

        /// <summary>
        /// Gets the timestamp of the gaze event
        /// </summary>
        public TimeSpan Timestamp { get; private set; }

        internal void Set(Point location, TimeSpan timestamp)
        {
            Handled = false;
            Location = location;
            Timestamp = timestamp;
        }
    }
}