// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    /// <summary>
    /// This enum reflects the states that a user's gaze through while interacting with a control using their eyes.
    /// </summary>
    public enum PointerState
    {
        /// <summary>
        /// User's gaze is no longer on the control
        /// </summary>
        Exit = 0,

        // The order of the following elements is important because
        // they represent states that linearly transition to their
        // immediate successors.

        /// <summary>
        /// For internal use only
        /// </summary>
        PreEnter = 1,

        /// <summary>
        /// User's gaze has entered a control
        /// </summary>
        Enter = 2,

        /// <summary>
        /// User eye's are focused on the control
        /// </summary>
        Fixation = 3,

        /// <summary>
        /// User is consciously dwelling on the control with an intent to invoke, e.g. click a button
        /// </summary>
        Dwell = 4,

        /// <summary>
        /// User is continuing to dwell on the control for repeated invocation. (For internal use only)
        /// </summary>
        DwellRepeat = 5
    }
}