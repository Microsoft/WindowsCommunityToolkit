// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI.Automation.Peers
{
    /// <summary>
    /// Defines a framework element automation peer for the <see cref="TokenizingTextBox"/> control.
    /// </summary>
    public class TokenizingTextBoxAutomationPeer : ListViewBaseAutomationPeer, IValueProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenizingTextBoxAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="TokenizingTextBox" /> that is associated with this <see cref="T:Microsoft.Toolkit.Uwp.UI.Automation.Peers.TokenizingTextBoxAutomationPeer" />.
        /// </param>
        public TokenizingTextBoxAutomationPeer(TokenizingTextBox owner)
            : base(owner)
        {
        }

        /// <summary>Gets a value indicating whether the value of a control is read-only.</summary>
        /// <returns>**true** if the value is read-only; **false** if it can be modified.</returns>
        public bool IsReadOnly => !this.OwningTokenizingTextBox.IsEnabled;

        /// <summary>Gets the value of the control.</summary>
        /// <returns>The value of the control.</returns>
        public string Value => this.OwningTokenizingTextBox.Text;

        private TokenizingTextBox OwningTokenizingTextBox
        {
            get
            {
                return Owner as TokenizingTextBox;
            }
        }

        /// <summary>Sets the value of a control.</summary>
        /// <param name="value">The value to set. The provider is responsible for converting the value to the appropriate data type.</param>
        /// <exception cref="T:Windows.UI.Xaml.Automation.ElementNotEnabledException">Thrown if the control is in a read-only state.</exception>
        public void SetValue(string value)
        {
            if (IsReadOnly)
            {
                throw new ElementNotEnabledException($"Could not set the value of the {nameof(TokenizingTextBox)} ");
            }

            this.OwningTokenizingTextBox.Text = value;
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
        /// - Name of the owning TokenizingTextBox
        /// - TokenizingTextBox class name
        /// </returns>
        protected override string GetNameCore()
        {
            string name = this.OwningTokenizingTextBox.Name;
            if (!string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            name = AutomationProperties.GetName(this.OwningTokenizingTextBox);
            return !string.IsNullOrWhiteSpace(name) ? name : base.GetNameCore();
        }

        /// <summary>
        /// Gets the control pattern that is associated with the specified Windows.UI.Xaml.Automation.Peers.PatternInterface.
        /// </summary>
        /// <param name="patternInterface">A value from the Windows.UI.Xaml.Automation.Peers.PatternInterface enumeration.</param>
        /// <returns>The object that supports the specified pattern, or null if unsupported.</returns>
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            return patternInterface switch
            {
                PatternInterface.Value => this,
                _ => base.GetPatternCore(patternInterface)
            };
        }

        /// <summary>
        /// Gets the collection of elements that are represented in the UI Automation tree as immediate
        /// child elements of the automation peer.
        /// </summary>
        /// <returns>The children elements.</returns>
        protected override IList<AutomationPeer> GetChildrenCore()
        {
            TokenizingTextBox owner = this.OwningTokenizingTextBox;

            ItemCollection items = owner.Items;
            if (items.Count <= 0)
            {
                return null;
            }

            List<AutomationPeer> peers = new List<AutomationPeer>(items.Count);
            for (int i = 0; i < items.Count; i++)
            {
                if (owner.ContainerFromIndex(i) is TokenizingTextBoxItem element)
                {
                    peers.Add(FromElement(element) ?? CreatePeerForElement(element));
                }
            }

            return peers;
        }
    }
}
