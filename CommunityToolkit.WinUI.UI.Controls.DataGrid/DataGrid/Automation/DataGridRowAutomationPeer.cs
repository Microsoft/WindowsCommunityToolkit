// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Automation.Peers;

namespace CommunityToolkit.WinUI.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer for DataGridRow
    /// </summary>
    public class DataGridRowAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridRowAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">DataGridRow</param>
        public DataGridRowAutomationPeer(DataGridRow owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the control type for the element that is associated with the UI Automation peer.
        /// </summary>
        /// <returns>The control type.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.DataItem;
        }

        /// <summary>
        /// Called by GetClassName that gets a human readable name that, in addition to AutomationControlType,
        /// differentiates the control represented by this AutomationPeer.
        /// </summary>
        /// <returns>The string that contains the name.</returns>
        protected override string GetClassNameCore()
        {
            string classNameCore = Owner.GetType().Name;
#if DEBUG_AUTOMATION
            System.Diagnostics.Debug.WriteLine("DataGridRowAutomationPeer.GetClassNameCore returns " + classNameCore);
#endif
            return classNameCore;
        }

        /// <summary>
        /// Returns a human-readable string that contains the item type that the UIElement for this DataGridRowAutomationPeer represents.
        /// </summary>
        /// <returns>A human-readable string that contains the item type that the UIElement for this DataGridRowAutomationPeer represents.</returns>
        protected override string GetItemTypeCore()
        {
            string itemType = base.GetItemTypeCore();
            if (!string.IsNullOrEmpty(itemType))
            {
                return itemType;
            }

            return UI.Controls.Properties.Resources.DataGridRowAutomationPeer_ItemType;
        }
    }
}