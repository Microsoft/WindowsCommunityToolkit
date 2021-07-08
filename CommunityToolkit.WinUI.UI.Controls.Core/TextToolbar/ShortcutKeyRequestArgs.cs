// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Arguments relating to a CTRL Key Shortcut being activated.
    /// </summary>
    public class ShortcutKeyRequestArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShortcutKeyRequestArgs"/> class.
        /// </summary>
        /// <param name="key"><see cref="VirtualKey"/> pressed</param>
        /// <param name="shiftKeyHeld">value indicating if the SHIFT key was pressed</param>
        /// <param name="args"><see cref="KeyRoutedEventArgs"/> from the original event handler</param>
        public ShortcutKeyRequestArgs(VirtualKey key, bool shiftKeyHeld, KeyRoutedEventArgs args)
        {
            Key = key;
            ShiftKeyHeld = shiftKeyHeld;
            OriginalArgs = args;
        }

        /// <summary>
        /// Gets key pressed with CTRL
        /// </summary>
        public VirtualKey Key { get; private set; }

        /// <summary>
        /// Gets the Original KeyDown arguments
        /// </summary>
        public KeyRoutedEventArgs OriginalArgs { get; private set; }

        /// <summary>
        /// Gets a value indicating whether Shift was held down too
        /// </summary>
        public bool ShiftKeyHeld { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Key been Handled
        /// </summary>
        public bool Handled { get; set; } = false;
    }
}