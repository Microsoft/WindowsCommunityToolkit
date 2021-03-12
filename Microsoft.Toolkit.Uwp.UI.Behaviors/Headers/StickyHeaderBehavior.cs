// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.UI.Animations.Expressions;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.UI.Behaviors
{
    /// <summary>
    /// Performs an animation on a ListView or GridView Header to make it sticky using composition.
    /// </summary>
    /// <seealso>
    ///     <cref>Microsoft.Xaml.Interactivity.Behavior{Windows.UI.Xaml.UIElement}</cref>
    /// </seealso>
    public class StickyHeaderBehavior : BehaviorBase<FrameworkElement>
    {
        /// <summary>
        /// Attaches the behavior to the associated object.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if attaching succeeded; otherwise <c>false</c>.
        /// </returns>
        protected override bool Initialize()
        {
            var result = AssignAnimation();
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
            RemoveAnimation();
            return true;
        }

        /// <summary>
        /// The UIElement that will be faded.
        /// </summary>
        public static readonly DependencyProperty HeaderElementProperty = DependencyProperty.Register(
            nameof(HeaderElement), typeof(UIElement), typeof(QuickReturnHeaderBehavior), new PropertyMetadata(null, PropertyChangedCallback));

        private ScrollViewer _scrollViewer;
        private double _previousVerticalScrollOffset;
        private CompositionPropertySet _scrollProperties;
        private CompositionPropertySet _animationProperties;
        private Visual _headerVisual;

        /// <summary>
        /// Gets or sets the target element for the ScrollHeader behavior.
        /// </summary>
        /// <remarks>
        /// Set this using the header of a ListView or GridView.
        /// </remarks>
        public UIElement HeaderElement
        {
            get { return (UIElement)GetValue(HeaderElementProperty); }
            set { SetValue(HeaderElementProperty, value); }
        }

        /// <summary>
        /// Show the header
        /// </summary>
        public void Show()
        {
            if (_headerVisual != null && _scrollViewer != null)
            {
                _previousVerticalScrollOffset = _scrollViewer.VerticalOffset;

                _animationProperties.InsertScalar("OffsetY", 0.0f);
            }
        }

        /// <summary>
        /// If any of the properties are changed then the animation is automatically started.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var b = d as StickyHeaderBehavior;
            b?.AssignAnimation();
        }

        /// <summary>
        /// Uses Composition API to get the UIElement and sets an ExpressionAnimation
        /// The ExpressionAnimation uses the height of the UIElement to calculate an opacity value
        /// for the Header as it is scrolling off-screen. The opacity reaches 0 when the Header
        /// is entirely scrolled off.
        /// </summary>
        /// <returns><c>true</c> if the assignment was successful; otherwise, <c>false</c>.</returns>
        private bool AssignAnimation()
        {
            StopAnimation();

            if (AssociatedObject == null)
            {
                return false;
            }

            if (_scrollViewer == null)
            {
                _scrollViewer = AssociatedObject as ScrollViewer ?? AssociatedObject.FindDescendant<ScrollViewer>();
            }

            if (_scrollViewer == null)
            {
                return false;
            }

            var listView = AssociatedObject as Windows.UI.Xaml.Controls.ListViewBase ?? AssociatedObject.FindDescendant<Windows.UI.Xaml.Controls.ListViewBase>();

            if (listView != null && listView.ItemsPanelRoot != null)
            {
                Canvas.SetZIndex(listView.ItemsPanelRoot, -1);
            }

            if (_scrollProperties == null)
            {
                _scrollProperties = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(_scrollViewer);
            }

            if (_scrollProperties == null)
            {
                return false;
            }

            // Implicit operation: Find the Header object of the control if it uses ListViewBase
            if (HeaderElement == null && listView != null)
            {
                HeaderElement = listView.Header as UIElement;
            }

            var headerElement = HeaderElement as FrameworkElement;
            if (headerElement == null || headerElement.RenderSize.Height == 0)
            {
                return false;
            }

            if (_headerVisual == null)
            {
                _headerVisual = ElementCompositionPreview.GetElementVisual(headerElement);
            }

            if (_headerVisual == null)
            {
                return false;
            }

            headerElement.SizeChanged -= ScrollHeader_SizeChanged;
            headerElement.SizeChanged += ScrollHeader_SizeChanged;

            _scrollViewer.GotFocus -= ScrollViewer_GotFocus;
            _scrollViewer.GotFocus += ScrollViewer_GotFocus;

            var compositor = _scrollProperties.Compositor;

            if (_animationProperties == null)
            {
                _animationProperties = compositor.CreatePropertySet();
                _animationProperties.InsertScalar("OffsetY", 0.0f);
            }

            _previousVerticalScrollOffset = _scrollViewer.VerticalOffset;

            var propSetOffset = _animationProperties.GetReference().GetScalarProperty("OffsetY");
            var scrollPropSet = _scrollProperties.GetSpecializedReference<ManipulationPropertySetReferenceNode>();
            var expressionAnimation = ExpressionFunctions.Max(propSetOffset - scrollPropSet.Translation.Y, 0);

            _headerVisual.StartAnimation("Offset.Y", expressionAnimation);

            return true;
        }

        /// <summary>
        /// Remove the animation from the UIElement.
        /// </summary>
        private void RemoveAnimation()
        {
            if (HeaderElement is FrameworkElement element)
            {
                element.SizeChanged -= ScrollHeader_SizeChanged;
            }

            if (_scrollViewer != null)
            {
                _scrollViewer.GotFocus -= ScrollViewer_GotFocus;
            }

            StopAnimation();
        }

        /// <summary>
        /// Stop the animation of the UIElement.
        /// </summary>
        private void StopAnimation()
        {
            _headerVisual?.StopAnimation("Offset.Y");

            _animationProperties?.InsertScalar("OffsetY", 0.0f);

            if (_headerVisual != null)
            {
                var offset = _headerVisual.Offset;
                offset.Y = 0.0f;
                _headerVisual.Offset = offset;
            }
        }

        private void ScrollHeader_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AssignAnimation();
        }

        private void ScrollViewer_GotFocus(object sender, RoutedEventArgs e)
        {
            var scroller = (ScrollViewer)sender;

            object focusedElement;
            if (ApiInformationHelper.IsXamlRootAvailable && scroller.XamlRoot != null)
            {
                focusedElement = FocusManager.GetFocusedElement(scroller.XamlRoot);
            }
            else
            {
                focusedElement = FocusManager.GetFocusedElement();
            }

            // To prevent Popups (Flyouts...) from triggering the autoscroll, we check if the focused element has a valid parent.
            // Popups have no parents, whereas a normal Item would have the ListView as a parent.
            if (focusedElement is UIElement element && VisualTreeHelper.GetParent(element) != null)
            {
                FrameworkElement header = (FrameworkElement)HeaderElement;

                var point = element.TransformToVisual(scroller).TransformPoint(new Point(0, 0));

                if (point.Y < header.ActualHeight)
                {
                    scroller.ChangeView(0, scroller.VerticalOffset - (header.ActualHeight - point.Y), 1, false);
                }
            }
        }
    }
}
