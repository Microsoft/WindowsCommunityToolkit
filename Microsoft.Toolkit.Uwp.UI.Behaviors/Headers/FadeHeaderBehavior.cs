// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.UI.Animations.Expressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace Microsoft.Toolkit.Uwp.UI.Behaviors
{
    /// <summary>
    /// Performs an fade animation on a ListView or GridView Header using composition.
    /// </summary>
    /// <seealso>
    ///     <cref>Microsoft.Xaml.Interactivity.Behavior{Windows.UI.Xaml.UIElement}</cref>
    /// </seealso>
    public class FadeHeaderBehavior : BehaviorBase<FrameworkElement>
    {
        /// <summary>
        /// Attaches the behavior to the associated object.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if attaching succeeded; otherwise <c>false</c>.
        /// </returns>
        protected override bool Initialize()
        {
            var result = AssignFadeAnimation();
            return result;
        }

        /// <summary>
        /// Detaches the behavior from the associated object.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if detaching succeeded; otherwise <c>false</c>.
        /// </returns>
        protected override bool Uninitialize()
        {
            RemoveFadeAnimation();
            return true;
        }

        /// <summary>
        /// If any of the properties are changed then the animation is automatically started depending on the AutomaticallyStart property.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var b = d as FadeHeaderBehavior;
            b?.AssignFadeAnimation();
        }

        /// <summary>
        /// The UIElement that will be faded.
        /// </summary>
        public static readonly DependencyProperty HeaderElementProperty = DependencyProperty.Register(
            nameof(HeaderElement), typeof(UIElement), typeof(FadeHeaderBehavior), new PropertyMetadata(null, PropertyChangedCallback));

        /// <summary>
        /// Gets or sets the target element for the Fading behavior.
        /// </summary>
        /// <remarks>
        /// Set this using the header of a ListView or GridView. You can use the entire root of the header or an element within the header.
        ///
        /// Using this example Header:
        /// <ListView.Header>
        ///     <Grid Name="MyHeader">
        ///     </Grid>
        /// </ListView.Header>
        ///
        /// The behavior would be implemented like this
        /// <FadeHeaderBehavior HeaderElement="{Binding ElementName=HeaderPanel}" />
        /// </remarks>
        public UIElement HeaderElement
        {
            get { return (UIElement)GetValue(HeaderElementProperty); }
            set { SetValue(HeaderElementProperty, value); }
        }

        /// <summary>
        /// Uses Composition API to get the UIElement and sets an ExpressionAnimation
        /// The ExpressionAnimation uses the height of the UIElement to calculate an opacity value
        /// for the Header as it is scrolling off-screen. The opacity reaches 0 when the Header
        /// is entirely scrolled off.
        /// </summary>
        /// <returns><c>true</c> if the assignment was successful; otherwise, <c>false</c>.</returns>
        private bool AssignFadeAnimation()
        {
            if (AssociatedObject == null)
            {
                return false;
            }

            var scroller = AssociatedObject as ScrollViewer ?? AssociatedObject.FindDescendant<ScrollViewer>();
            if (scroller == null)
            {
                return false;
            }

            var listView = AssociatedObject as Windows.UI.Xaml.Controls.ListViewBase ?? AssociatedObject.FindDescendant<Windows.UI.Xaml.Controls.ListViewBase>();

            if (listView != null && listView.ItemsPanelRoot != null)
            {
                Canvas.SetZIndex(listView.ItemsPanelRoot, -1);
            }

            // Implicit operation: Find the Header object of the control if it uses ListViewBase
            if (HeaderElement == null && listView != null)
            {
                HeaderElement = listView.Header as UIElement;
            }

            // If no header is set or detected, return.
            if (HeaderElement == null || HeaderElement.RenderSize.Height == 0d)
            {
                return false;
            }

            // Get the ScrollViewer's ManipulationPropertySet
            var scrollViewerManipulationPropSet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scroller);
            var scrollPropSet = scrollViewerManipulationPropSet.GetSpecializedReference<ManipulationPropertySetReferenceNode>();

            // Use the ScrollViewer's Y offset and the header's height to calculate the opacity percentage. Clamp it between 0% and 100%
            var headerHeight = (float)HeaderElement.RenderSize.Height;
            var opacityExpression = ExpressionFunctions.Clamp(1 - (-scrollPropSet.Translation.Y / headerHeight), 0, 1);

            // Begin animating
            var targetElement = ElementCompositionPreview.GetElementVisual(HeaderElement);
            targetElement.StartAnimation("Opacity", opacityExpression);

            return true;
        }

        /// <summary>
        /// Remove the opacity animation from the UIElement.
        /// </summary>
        private void RemoveFadeAnimation()
        {
            if (HeaderElement != null)
            {
                var targetElement = ElementCompositionPreview.GetElementVisual(HeaderElement);
                targetElement.StopAnimation("Opacity");
                targetElement.Opacity = 1.0f;
            }
        }
    }
}