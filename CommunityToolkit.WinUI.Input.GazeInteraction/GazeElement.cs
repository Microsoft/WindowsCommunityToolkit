// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Xaml;

namespace CommunityToolkit.WinUI.Input.GazeInteraction
{
    /// <summary>
    /// Surrogate object attached to controls allowing subscription to per-control gaze events.
    /// </summary>
    public class GazeElement : DependencyObject
    {
        /// <summary>
        /// This event is fired when the state of the user's gaze on a control has changed
        /// </summary>
        public event EventHandler<StateChangedEventArgs> StateChanged;

        /// <summary>
        /// This event is fired when the user completed dwelling on a control and the control is about to be invoked by default. This event is fired to give the application an opportunity to prevent default invocation
        /// </summary>
        public event EventHandler<DwellInvokedRoutedEventArgs> Invoked;

        /// <summary>
        /// This event is fired to inform the application of the progress towards dwell
        /// </summary>
        public event EventHandler<DwellProgressEventArgs> DwellProgressFeedback;

        internal void RaiseStateChanged(object sender, StateChangedEventArgs args)
        {
            StateChanged?.Invoke(sender, args);
        }

        internal void RaiseInvoked(object sender, DwellInvokedRoutedEventArgs args)
        {
            Invoked?.Invoke(sender, args);
        }

        internal bool RaiseProgressFeedback(object sender, DwellProgressState state, TimeSpan elapsedTime, TimeSpan triggerTime)
        {
            var args = new DwellProgressEventArgs(state, elapsedTime, triggerTime);
            DwellProgressFeedback?.Invoke(sender, args);
            return args.Handled;
        }
    }
}