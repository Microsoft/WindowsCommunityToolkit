// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;

namespace CommunityToolkit.WinUI.UI.Automation.Peers
{
    /// <summary>
    /// Defines a framework element automation peer for the <see cref="Expander"/> control.
    /// </summary>
    public class ExpanderAutomationPeer : FrameworkElementAutomationPeer, IToggleProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpanderAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="Expander" /> that is associated with this <see cref="T:Microsoft.UI.Xaml.Automation.Peers.ExpanderAutomationPeer" />.
        /// </param>
        public ExpanderAutomationPeer(Expander owner)
            : base(owner)
        {
        }

        /// <summary>Gets the toggle state of the control.</summary>
        /// <returns>The toggle state of the control, as a value of the enumeration.</returns>
        public ToggleState ToggleState => OwningExpander.IsExpanded ? ToggleState.On : ToggleState.Off;

        private Expander OwningExpander
        {
            get
            {
                return Owner as Expander;
            }
        }

        /// <summary>Cycles through the toggle states of a control.</summary>
        public void Toggle()
        {
            if (!IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            OwningExpander.IsExpanded = !OwningExpander.IsExpanded;
        }

        /// <summary>
        /// Gets the control type for the element that is associated with the UI Automation peer.
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
            return Owner.GetType().Name;
        }

        /// <summary>
        /// Called by GetName.
        /// </summary>
        /// <returns>
        /// Returns the first of these that is not null or empty:
        /// - Value returned by the base implementation
        /// - Name of the owning Expander
        /// - Expander class name
        /// </returns>
        protected override string GetNameCore()
        {
            string name = base.GetNameCore();
            if (!string.IsNullOrEmpty(name))
            {
                return name;
            }

            if (this.OwningExpander != null)
            {
                name = this.OwningExpander.Name;
            }

            if (string.IsNullOrEmpty(name))
            {
                name = this.GetClassName();
            }

            return name;
        }

        /// <summary>
        /// Gets the control pattern that is associated with the specified Microsoft.UI.Xaml.Automation.Peers.PatternInterface.
        /// </summary>
        /// <param name="patternInterface">A value from the Microsoft.UI.Xaml.Automation.Peers.PatternInterface enumeration.</param>
        /// <returns>The object that supports the specified pattern, or null if unsupported.</returns>
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            switch (patternInterface)
            {
                case PatternInterface.Toggle:
                    return this;
            }

            return base.GetPatternCore(patternInterface);
        }
    }
}