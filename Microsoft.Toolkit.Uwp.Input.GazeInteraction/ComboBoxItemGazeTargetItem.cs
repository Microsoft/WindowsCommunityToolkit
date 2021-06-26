// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    internal class ComboBoxItemGazeTargetItem : GazeTargetItem
    {
        internal ComboBoxItemGazeTargetItem(UIElement element)
            : base(element)
        {
        }

        internal override void Invoke()
        {
            var peer = FrameworkElementAutomationPeer.FromElement(TargetElement);
            var comboBoxItemAutomationPeer = peer as ComboBoxItemAutomationPeer;
            var comboBoxItem = (ComboBoxItem)comboBoxItemAutomationPeer.Owner;

            AutomationPeer ancestor = comboBoxItemAutomationPeer;
            var comboBoxAutomationPeer = ancestor as ComboBoxAutomationPeer;
            while (comboBoxAutomationPeer == null)
            {
                ancestor = ancestor.Navigate(AutomationNavigationDirection.Parent) as AutomationPeer;
                comboBoxAutomationPeer = ancestor as ComboBoxAutomationPeer;
            }

            comboBoxItem.IsSelected = true;
            comboBoxAutomationPeer.Collapse();
        }
    }
}