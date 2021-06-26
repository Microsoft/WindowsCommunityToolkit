// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.UI.Controls.Primitives;
using Windows.UI.Xaml.Automation.Peers;

namespace Microsoft.Toolkit.Uwp.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer for DataGridRowHeader
    /// </summary>
    public class DataGridRowHeaderAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridRowHeaderAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">DataGridRowHeader</param>
        public DataGridRowHeaderAutomationPeer(DataGridRowHeader owner)
            : base(owner)
        {
        }

        private DataGridRowHeader OwningHeader
        {
            get
            {
                return (DataGridRowHeader)Owner;
            }
        }

        /// <summary>
        /// Gets the control type for the element that is associated with the UI Automation peer.
        /// </summary>
        /// <returns>The control type.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.HeaderItem;
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
            System.Diagnostics.Debug.WriteLine("DataGridRowHeaderAutomationPeer.GetClassNameCore returns " + classNameCore);
#endif
            return classNameCore;
        }

        /// <summary>
        /// Gets the name of the element.
        /// </summary>
        /// <returns>The string that contains the name.</returns>
        protected override string GetNameCore()
        {
            return (this.OwningHeader.Content as string) ?? base.GetNameCore();
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