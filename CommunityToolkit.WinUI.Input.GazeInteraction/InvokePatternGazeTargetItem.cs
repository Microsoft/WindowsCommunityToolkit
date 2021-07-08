// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;

namespace CommunityToolkit.WinUI.Input.GazeInteraction
{
    internal class InvokePatternGazeTargetItem : GazeTargetItem
    {
        internal InvokePatternGazeTargetItem(UIElement element)
            : base(element)
        {
        }

        internal override void Invoke()
        {
            var peer = FrameworkElementAutomationPeer.FromElement(TargetElement);
            var provider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            provider.Invoke();
        }
    }
}