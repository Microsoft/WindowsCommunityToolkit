// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Provides event data for the ColorChanged event.
    /// </summary>
    public sealed class EyedropperColorChangedEventArgs
    {
        /// <summary>
        /// Gets the color that is currently selected in the control.
        /// </summary>
        public Color NewColor { get; internal set; }

        /// <summary>
        /// Gets the color that was previously selected in the control.
        /// </summary>
        public Color OldColor { get; internal set; }
    }
}