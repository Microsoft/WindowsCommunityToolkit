// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction.GazeHidParsers
{
    /// <summary>
    /// Represents one Hid position
    /// </summary>
    public class GazeHidPosition
    {
        /// <summary>
        /// Gets or sets the X axis of this position
        /// </summary>
        public long X { get; set; }

        /// <summary>
        /// Gets or sets the Y axis of this position
        /// </summary>
        public long Y { get; set; }

        /// <summary>
        /// Gets or sets the Z axis of this position
        /// </summary>
        public long Z { get; set; }
    }
}