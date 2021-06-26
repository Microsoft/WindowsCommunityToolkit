// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    internal class ExpandCollapsePatternGazeTargetItem : GazeTargetItem
    {
        internal ExpandCollapsePatternGazeTargetItem(UIElement element)
            : base(element)
        {
        }

        internal override void Invoke()
        {
            var peer = FrameworkElementAutomationPeer.FromElement(TargetElement);
            var provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider;
            switch (provider.ExpandCollapseState)
            {
                case ExpandCollapseState.Collapsed:
                    provider.Expand();
                    break;

                case ExpandCollapseState.Expanded:
                    provider.Collapse();
                    break;
            }
        }
    }
}