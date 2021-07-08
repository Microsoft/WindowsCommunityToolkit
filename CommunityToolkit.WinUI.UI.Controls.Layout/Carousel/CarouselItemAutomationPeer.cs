// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.WinUI.UI.Automation.Peers
{
    /// <summary>
    /// Defines a framework element automation peer for the <see cref="CarouselItem"/>.
    /// </summary>
    public class CarouselItemAutomationPeer : FrameworkElementAutomationPeer, ISelectionItemProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CarouselItemAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="CarouselItem" /> that is associated with this <see cref="T:Microsoft.UI.Xaml.Automation.Peers.CarouselItemAutomationPeer" />.
        /// </param>
        public CarouselItemAutomationPeer(CarouselItem owner)
            : base(owner)
        {
        }

        /// <summary>Gets a value indicating whether an item is selected.</summary>
        /// <returns>True if the element is selected; otherwise, false.</returns>
        public bool IsSelected => this.OwnerCarouselItem.IsSelected;

        /// <summary>Gets the UI Automation provider that implements ISelectionProvider and acts as the container for the calling object.</summary>
        /// <returns>The UI Automation provider.</returns>
        public IRawElementProviderSimple SelectionContainer
        {
            get
            {
                Carousel parent = this.OwnerCarouselItem.ParentCarousel;
                if (parent == null)
                {
                    return null;
                }

                AutomationPeer peer = FromElement(parent);
                return peer != null ? this.ProviderFromPeer(peer) : null;
            }
        }

        private CarouselItem OwnerCarouselItem
        {
            get { return this.Owner as CarouselItem; }
        }

        /// <summary>Adds the current element to the collection of selected items.</summary>
        public void AddToSelection()
        {
            CarouselItem owner = this.OwnerCarouselItem;
            Carousel parent = owner.ParentCarousel;
            parent?.SetSelectedItem(owner);
        }

        /// <summary>Removes the current element from the collection of selected items.</summary>
        public void RemoveFromSelection()
        {
            // Cannot remove the selection of a Carousel control.
        }

        /// <summary>Clears any existing selection and then selects the current element.</summary>
        public void Select()
        {
            CarouselItem owner = this.OwnerCarouselItem;
            Carousel parent = owner.ParentCarousel;
            parent?.SetSelectedItem(owner);
        }

        /// <summary>
        /// Gets the control type for the element that is associated with the UI Automation peer.
        /// </summary>
        /// <returns>The control type.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.ListItem;
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
        /// - Name of the owning CarouselItem
        /// - Carousel class name
        /// </returns>
        protected override string GetNameCore()
        {
            string name = AutomationProperties.GetName(this.OwnerCarouselItem);
            if (!string.IsNullOrEmpty(name))
            {
                return name;
            }

            name = this.OwnerCarouselItem.Name;
            if (!string.IsNullOrEmpty(name))
            {
                return name;
            }

            var textBlock = this.OwnerCarouselItem.FindDescendant<TextBlock>();
            if (textBlock != null)
            {
                return textBlock.Text;
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
                case PatternInterface.SelectionItem:
                    return this;
            }

            return base.GetPatternCore(patternInterface);
        }

        /// <summary>
        /// Returns the size of the set where the element that is associated with the automation peer is located.
        /// </summary>
        /// <returns>
        /// The size of the set.
        /// </returns>
        protected override int GetSizeOfSetCore()
        {
            int sizeOfSet = base.GetSizeOfSetCore();

            if (sizeOfSet != -1)
            {
                return sizeOfSet;
            }

            CarouselItem owner = this.OwnerCarouselItem;
            Carousel parent = owner.ParentCarousel;
            sizeOfSet = parent.Items.Count;

            return sizeOfSet;
        }

        /// <summary>
        /// Returns the ordinal position in the set for the element that is associated with the automation peer.
        /// </summary>
        /// <returns>
        /// The ordinal position in the set.
        /// </returns>
        protected override int GetPositionInSetCore()
        {
            int positionInSet = base.GetPositionInSetCore();

            if (positionInSet != -1)
            {
                return positionInSet;
            }

            CarouselItem owner = this.OwnerCarouselItem;
            Carousel parent = owner.ParentCarousel;
            positionInSet = parent.IndexFromContainer(owner) + 1;

            return positionInSet;
        }
    }
}