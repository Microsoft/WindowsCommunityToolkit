// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;

namespace CommunityToolkit.WinUI.Input.GazeInteraction
{
    /// <summary>
    /// This parameter is passed to the GazeElement.DwellProgressFeedback event. The event is fired to inform the application of the user's progress towards completing dwelling on a control
    /// </summary>
    public sealed class DwellProgressEventArgs : HandledEventArgs
    {
        /// <summary>
        /// Gets an enum that reflects the current state of dwell progress
        /// </summary>
        public DwellProgressState State { get; }

        /// <summary>
        /// Gets a value between 0 and 1 that reflects the fraction of progress towards completing dwell
        /// </summary>
        public double Progress { get; }

        internal DwellProgressEventArgs(DwellProgressState state, TimeSpan elapsedDuration, TimeSpan triggerDuration)
        {
            State = state;
            Progress = ((double)elapsedDuration.Ticks) / triggerDuration.Ticks;
        }
    }
}