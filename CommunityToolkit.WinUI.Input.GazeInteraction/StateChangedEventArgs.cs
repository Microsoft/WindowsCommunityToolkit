// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Xaml;

namespace CommunityToolkit.WinUI.Input.GazeInteraction
{
    /// <summary>
    /// This parameter is passed to the StateChanged event.
    /// </summary>
    public class StateChangedEventArgs
    {
        /// <summary>
        /// Gets the state of the user's gaze with respect to a control
        /// </summary>
        public PointerState PointerState { get; }

        /// <summary>
        /// Gets elapsed time since the last state
        /// </summary>
        public TimeSpan ElapsedTime => _elapsedTime;

        internal StateChangedEventArgs(UIElement target, PointerState state, TimeSpan elapsedTime)
        {
            _hitTarget = target;
            PointerState = state;
            _elapsedTime = elapsedTime;
        }

        private readonly UIElement _hitTarget;
        private TimeSpan _elapsedTime;
    }
}