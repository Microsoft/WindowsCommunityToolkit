// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.WinUI.UI.Automation.Peers
{
    /// <summary>
    /// Defines a framework element automation peer for the <see cref="Carousel"/> control.
    /// </summary>
    public class CarouselAutomationPeer : ItemsControlAutomationPeer, ISelectionProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CarouselAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="Carousel" /> that is associated with this <see cref="T:Microsoft.UI.Xaml.Automation.Peers.CarouselAutomationPeer" />.
        /// </param>
        public CarouselAutomationPeer(Carousel owner)
            : base(owner)
        {
        }

        /// <summary>Gets a value indicating whether the Microsoft UI Automation provider allows more than one child element to be selected concurrently.</summary>
        /// <returns>True if multiple selection is allowed; otherwise, false.</returns>
        public bool CanSelectMultiple => false;

        /// <summary>Gets a value indicating whether the UI Automation provider requires at least one child element to be selected.</summary>
        /// <returns>True if selection is required; otherwise, false.</returns>
        public bool IsSelectionRequired => true;

        private Carousel OwningCarousel
        {
            get
            {
                return Owner as Carousel;
            }
        }

        /// <summary>Retrieves a UI Automation provider for each child element that is selected.</summary>
        /// <returns>An array of UI Automation providers.</returns>
        public IRawElementProviderSimple[] GetSelection()
        {
            return OwningCarousel.ContainerFromItem(this.OwningCarousel.SelectedItem) is CarouselItem selectedCarouselItem
                       ? new[] { this.ProviderFromPeer(FromElement(selectedCarouselItem)) }
                       : new IRawElementProviderSimple[] { };
        }

        /// <summary>
        /// Gets the control type for the element that is associated with the UI Automation peer.
        /// </summary>
        /// <returns>The control type.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.List;
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
        /// - Name of the owning Carousel
        /// - Carousel class name
        /// </returns>
        protected override string GetNameCore()
        {
            string name = this.OwningCarousel.Name;
            if (!string.IsNullOrEmpty(name))
            {
                return name;
            }

            name = AutomationProperties.GetName(this.OwningCarousel);
            if (!string.IsNullOrEmpty(name))
            {
                return name;
            }

            return base.GetNameCore();
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
                case PatternInterface.Selection:
                    return this;
            }

            return base.GetPatternCore(patternInterface);
        }

        /// <summary>
        /// Gets the collection of elements that are represented in the UI Automation tree as immediate
        /// child elements of the automation peer.
        /// </summary>
        /// <returns>The children elements.</returns>
        protected override IList<AutomationPeer> GetChildrenCore()
        {
            Carousel owner = OwningCarousel;

            ItemCollection items = owner.Items;
            if (items.Count <= 0)
            {
                return null;
            }

            List<AutomationPeer> peers = new List<AutomationPeer>(items.Count);
            for (int i = 0; i < items.Count; i++)
            {
                if (owner.ContainerFromIndex(i) is CarouselItem element)
                {
                    peers.Add(FromElement(element) ?? CreatePeerForElement(element));
                }
            }

            return peers;
        }
    }
}