// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.UI.Controls.Primitives;
using Microsoft.UI.Xaml.Automation.Peers;

namespace CommunityToolkit.WinUI.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer for DataGridDetailsPresenter
    /// </summary>
    public class DataGridDetailsPresenterAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridDetailsPresenterAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">DataGridDetailsPresenter</param>
        public DataGridDetailsPresenterAutomationPeer(DataGridDetailsPresenter owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the control type for the DataGridDetailsPresenter element that is associated with the UI Automation peer.
        /// </summary>
        /// <returns>The control type.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
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
            System.Diagnostics.Debug.WriteLine("DataGridDetailsPresenterAutomationPeer.GetClassNameCore returns " + classNameCore);
#endif
            return classNameCore;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the DataGridDetailsPresenter associated with this UIElementAutomationPeer
        /// is understood by the end user as interactive.
        /// </summary>
        /// <returns>True if the DataGridDetailsPresenter associated with this UIElementAutomationPeer
        /// is understood by the end user as interactive.</returns>
        protected override bool IsControlElementCore()
        {
            return true;
        }

        /// <summary>
        /// Gets a value that specifies whether the element is a content element.
        /// </summary>
        /// <returns>True if the element is a content element; otherwise false</returns>
        protected override bool IsContentElementCore()
        {
            return false;
        }
    }
}