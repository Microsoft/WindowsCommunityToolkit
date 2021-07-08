// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Input.GazeInteraction
{
    /// <summary>
    /// An enum that reflects the current state of progress towards dwell when a user is focused on a control
    /// </summary>
    public enum DwellProgressState
    {
        /// <summary>
        /// User is not looking at the control
        /// </summary>
        Idle,

        /// <summary>
        /// Gaze has entered control but we're not yet showing progress.
        /// </summary>
        Fixating,

        /// <summary>
        /// User is continuing to focus on a control with an intent to dwell and invoke
        /// </summary>
        Progressing,

        /// <summary>
        /// User has completed dwelling on a control
        /// </summary>
        Complete
    }
}