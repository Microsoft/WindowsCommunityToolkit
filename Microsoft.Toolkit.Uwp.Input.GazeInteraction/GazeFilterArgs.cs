// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.Foundation;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    /// <summary>
    /// This struct encapsulates the location and timestamp associated with the user's gaze
    /// and is used as an input and output parameter for the IGazeFilter.Update method
    /// </summary>
    internal struct GazeFilterArgs
    {
        /// <summary>
        /// Gets the current point in the gaze stream
        /// </summary>
        public Point Location => _location;

        /// <summary>
        /// Gets the timestamp associated with the current point
        /// </summary>
        public TimeSpan Timestamp => _timestamp;

        internal GazeFilterArgs(Point location, TimeSpan timestamp)
        {
            _location = location;
            _timestamp = timestamp;
        }

        private Point _location;
        private TimeSpan _timestamp;
    }
}