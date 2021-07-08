// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace CommunityToolkit.WinUI.Input.GazeInteraction
{
    /// <summary>
    /// This parameter is passed to the GazeElement::Invoked event and allows
    /// the application to prevent default invocation when the user dwells on a control
    /// </summary>
    public sealed class DwellInvokedRoutedEventArgs : HandledEventArgs
    {
        internal DwellInvokedRoutedEventArgs()
        {
        }
    }
}