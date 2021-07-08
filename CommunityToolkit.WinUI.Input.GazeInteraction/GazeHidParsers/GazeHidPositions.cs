// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Input.GazeInteraction.GazeHidParsers
{
    /// <summary>
    /// Represents the Hid positions
    /// </summary>
    public class GazeHidPositions
    {
        /// <summary>
        /// Gets or sets the left eye position
        /// </summary>
        public GazeHidPosition LeftEyePosition { get; set; }

        /// <summary>
        /// Gets or sets the right eye position
        /// </summary>
        public GazeHidPosition RightEyePosition { get; set; }

        /// <summary>
        /// Gets or sets the head position
        /// </summary>
        public GazeHidPosition HeadPosition { get; set; }

        /// <summary>
        /// Gets or sets the head rotation
        /// </summary>
        public GazeHidPosition HeadRotation { get; set; }
    }
}