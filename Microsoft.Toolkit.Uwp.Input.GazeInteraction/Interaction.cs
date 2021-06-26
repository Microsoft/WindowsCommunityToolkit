// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    /// <summary>
    /// This enum indicates the current state of gaze interaction.
    /// </summary>
    public enum Interaction
    {
        /// <summary>
        /// The state of gaze interaction is inherited from the nearest parent
        /// </summary>
        Inherited,

        /// <summary>
        /// Gaze interaction is enabled
        /// </summary>
        Enabled,

        /// <summary>
        /// Gaze interaction is disabled
        /// </summary>
        Disabled
    }
}