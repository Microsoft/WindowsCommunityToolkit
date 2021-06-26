// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.Utilities
{
    internal static class KeyboardHelper
    {
        public static void GetMetaKeyState(out bool ctrl, out bool shift)
        {
            ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down);
            shift = Window.Current.CoreWindow.GetKeyState(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down);
        }

        public static void GetMetaKeyState(out bool ctrl, out bool shift, out bool alt)
        {
            GetMetaKeyState(out ctrl, out shift);
            alt = Window.Current.CoreWindow.GetKeyState(VirtualKey.Menu).HasFlag(CoreVirtualKeyStates.Down);
        }
    }
}