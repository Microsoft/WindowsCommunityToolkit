// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// The panel used in the <see cref="Carousel"/> control
    /// </summary>
    public partial class CarouselPanel : Panel
    {
        // Storyboard on gesture
        private Storyboard storyboard = new Storyboard();

        // temp size
        private double desiredWidth;
        private double desiredHeight;

        private Carousel carouselControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="CarouselPanel"/> class.
        /// </summary>
        public CarouselPanel()
        {
            IsHitTestVisible = true;

            Background = new SolidColorBrush(Colors.Transparent);
            ManipulationMode = ManipulationModes.All;
            ManipulationCompleted += OnManipulationCompleted;
            ManipulationDelta += OnManipulationDelta;
            Tapped += OnTapped;
        }

        /// <summary>
        /// Gets the Current Carousel control
        /// </summary>
        public Carousel Carousel
        {
            get
            {
                if (carouselControl != null)
                {
                    return carouselControl;
                }

                carouselControl = this.FindAscendant<Carousel>();

                if (carouselControl == null)
                {
                    throw new Exception("This CarouselPanel must be used as an ItemsPanel in a Carousel control");
                }

                return carouselControl;
            }
        }

        /// <summary>
        /// Tap an item
        /// </summary>
        private void OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var position = e.GetPosition(this);

            for (int i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                var rect = child.TransformToVisual(this).TransformBounds(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));

                if (!(position.X >= rect.Left && position.X <= rect.Right && position.Y >= rect.Top && position.Y <= rect.Bottom))
                {
                    continue;
                }

                Carousel.SelectedIndex = i;

                return;
            }
        }

        /// <summary>
        /// Set a manipulation
        /// </summary>
        internal void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var previousIndex = Carousel.SelectedIndex;

            for (int i = 0; i < Children.Count; i++)
            {
                var item = Children[i];

                var delta = Carousel.Orientation == Orientation.Horizontal ? e.Delta.Translation.X : e.Delta.Translation.Y;
                var itemLength = Carousel.Orientation == Orientation.Horizontal ? item.DesiredSize.Width : item.DesiredSize.Height;

                var proj = GetProjectionFromManipulation(item, delta);

                ApplyProjection(item, proj);

                // We have to take care of the first and last items when manipulating
                if ((i == 0 && proj.Position > itemLength / 2) || (i == Children.Count - 1 && proj.Position < -itemLength))
                {
                    e.Handled = true;
                    e.Complete();
                    Carousel.SelectedIndex = i;

                    // force refresh if we are already on the first / last item
                    if (previousIndex == i)
                    {
                        UpdatePosition();
                    }

                    return;
                }

                // Calculate the Z index to be sure selected item is over all others
                var zindexItemIndex = delta > 0 ? Carousel.SelectedIndex - 1 : Carousel.SelectedIndex + 1;
                var deltaFromSelectedIndex = Math.Abs(zindexItemIndex - i);

                int zindex = (Children.Count * 100) - deltaFromSelectedIndex;
                Canvas.SetZIndex(item, zindex);
            }

            e.Handled = true;
        }

        /// <summary>
        /// End of a manipulation
        /// </summary>
        internal void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            // Need to know which direction we took for this manipulation.
            var translation = Carousel.Orientation == Orientation.Horizontal ? e.Cumulative.Translation.X : e.Cumulative.Translation.Y;

            // potentially border effects
            bool hasBreak = false;

            for (int i = 0; i < Children.Count - 1; i++)
            {
                var child = Children[i];

                PlaneProjection projection = child.Projection as PlaneProjection;
                CompositeTransform compositeTransform = child.RenderTransform as CompositeTransform;

                if (projection == null || compositeTransform == null)
                {
                    continue;
                }

                var margin = Carousel.ItemMargin;
                var size = Carousel.Orientation == Orientation.Horizontal ? desiredWidth : desiredHeight;
                var left = Carousel.Orientation == Orientation.Horizontal ? compositeTransform.TranslateX : compositeTransform.TranslateY;
                var right = left + size + margin;
                var condition = translation < 0 ? (left > 0) : (right > 0);

                if (condition)
                {
                    Carousel.SelectedIndex = i;
                    hasBreak = true;
                    break;
                }
            }

            if (!hasBreak)
            {
                Carousel.SelectedIndex = translation > 0 ? 0 : Children.Count - 1;
            }

            e.Handled = true;
        }

        /// <summary>
        /// Update all positions. Launch every animations on all items with a unique StoryBoard
        /// </summary>
        internal void UpdatePosition()
        {
            if (storyboard?.GetCurrentState() == ClockState.Active)
            {
                storyboard.SkipToFill();
            }

            storyboard = new Storyboard();
            ManipulationMode = ManipulationModes.None;

            for (int i = 0; i < Children.Count; i++)
            {
                var item = Children[i];

                PlaneProjection planeProjection = item.Projection as PlaneProjection;

                if (planeProjection == null)
                {
                    continue;
                }

                // Get target projection
                var props = GetProjectionFromSelectedIndex(i);

                // Apply projection
                ApplyProjection(item, props, storyboard);

                // Z index and Opacity
                var deltaFromSelectedIndex = Math.Abs(Carousel.SelectedIndex - i);
                int zindex = (Carousel.Items.Count * 100) - deltaFromSelectedIndex;
                Canvas.SetZIndex(item, zindex);
            }

            // When storyboard completed, Invalidate
            storyboard.Completed += (sender, o) =>
            {
                ManipulationMode = ManipulationModes.All;
            };

            storyboard.Begin();
        }

        /// <summary>
        /// Measure items
        /// </summary>
        /// <returns>Return carousel size</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            var containerWidth = 0d;
            var containerHeight = 0d;

            foreach (FrameworkElement container in Children)
            {
                container.Measure(availableSize);

                if (container.DesiredSize.Width > containerWidth)
                {
                    containerWidth = container.DesiredSize.Width;
                }

                if (container.DesiredSize.Height > containerHeight)
                {
                    containerHeight = container.DesiredSize.Height;
                }
            }

            // It's a Auto size, so we define the size should be 3 items
            double width;
            if (double.IsInfinity(availableSize.Width))
            {
                width = Carousel.Orientation == Orientation.Horizontal ? containerWidth * (Children.Count > 3 ? 3 : Children.Count) : containerWidth;
            }
            else
            {
                width = availableSize.Width;
            }

            // It's a Auto size, so we define the size should be 3 items
            double height;
            if (double.IsInfinity(availableSize.Height))
            {
                height = Carousel.Orientation == Orientation.Vertical ? containerHeight * (Children.Count > 3 ? 3 : Children.Count) : containerHeight;
            }
            else
            {
                height = availableSize.Height;
            }

            Clip = new RectangleGeometry { Rect = new Rect(0, 0, width, height) };

            return new Size(width, height);
        }

        /// <summary>
        /// Arrange all items
        /// </summary>
        /// <returns>Return an item size</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double centerLeft = 0;
            double centerTop = 0;

            Clip = new RectangleGeometry { Rect = finalSize.ToRect() };

            for (int i = 0; i < Children.Count; i++)
            {
                FrameworkElement container = Children[i] as FrameworkElement;

                Size desiredSize = container.DesiredSize;
                if (double.IsNaN(desiredSize.Width) || double.IsNaN(desiredSize.Height))
                {
                    continue;
                }

                // get the good center and top position
                if (centerLeft == 0 && centerTop == 0 && desiredSize.Width > 0 && desiredSize.Height > 0)
                {
                    desiredWidth = desiredSize.Width;
                    desiredHeight = desiredSize.Height;

                    centerLeft = (finalSize.Width / 2) - (desiredWidth / 2);
                    centerTop = (finalSize.Height - desiredHeight) / 2;
                }

                // Get absolute position from SelectedIndex
                var deltaFromSelectedIndex = Math.Abs(Carousel.SelectedIndex - i);

                // Get rect position
                var rect = new Rect(centerLeft, centerTop, desiredWidth, desiredHeight);

                // Placing the rect in the center of screen
                container.Arrange(rect);

                // Get the initial projection (without move)
                var proj = GetProjectionFromSelectedIndex(i);

                // apply the projection to the current object
                ApplyProjection(container, proj);

                // calculate z index and opacity
                int zindex = (Children.Count * 100) - deltaFromSelectedIndex;
                Canvas.SetZIndex(container, zindex);
            }

            return finalSize;
        }

        /// <summary>
        /// Apply the projection, with or without a storyboard involved
        /// </summary>
        private void ApplyProjection(UIElement element, Proj proj, Storyboard storyboard = null)
        {
            // then apply the plane projection transform
            PlaneProjection planeProjection = element.Projection as PlaneProjection;
            CompositeTransform compositeTransform = element.RenderTransform as CompositeTransform;

            if (planeProjection == null || compositeTransform == null)
            {
                return;
            }

            // Direct affectation (during first load) or with a storyboard
            if (storyboard == null)
            {
                compositeTransform.TranslateX = Carousel.Orientation == Orientation.Horizontal ? proj.Position : 0;
                compositeTransform.TranslateY = Carousel.Orientation == Orientation.Horizontal ? 0 : proj.Position;

                planeProjection.RotationX = proj.RotationX;
                planeProjection.RotationY = proj.RotationY;
                planeProjection.RotationZ = proj.RotationZ;

                planeProjection.GlobalOffsetZ = proj.Depth;
            }
            else
            {
                string localProjectionOrientation = Carousel.Orientation == Orientation.Horizontal ?
                                    "(UIElement.RenderTransform).(CompositeTransform.TranslateX)" : "(UIElement.RenderTransform).(CompositeTransform.TranslateY)";
                string localProjectionOrientationInvert = Carousel.Orientation == Orientation.Horizontal ?
                                    "(UIElement.RenderTransform).(CompositeTransform.TranslateY)" : "(UIElement.RenderTransform).(CompositeTransform.TranslateX)";

                string globalDepthProjection = "(UIElement.Projection).(PlaneProjection.GlobalOffsetZ)";
                string rotationXProjection = "(UIElement.Projection).(PlaneProjection.RotationX)";
                string rotationYProjection = "(UIElement.Projection).(PlaneProjection.RotationY)";
                string rotationZProjection = "(UIElement.Projection).(PlaneProjection.RotationZ)";

                AddAnimation(storyboard, element, Carousel.TransitionDuration, proj.Position, localProjectionOrientation, Carousel.EasingFunction);
                AddAnimation(storyboard, element, Carousel.TransitionDuration, 0, localProjectionOrientationInvert, Carousel.EasingFunction);
                AddAnimation(storyboard, element, Carousel.TransitionDuration, proj.Depth, globalDepthProjection, Carousel.EasingFunction);
                AddAnimation(storyboard, element, Carousel.TransitionDuration, proj.RotationX, rotationXProjection, Carousel.EasingFunction);
                AddAnimation(storyboard, element, Carousel.TransitionDuration, proj.RotationY, rotationYProjection, Carousel.EasingFunction);
                AddAnimation(storyboard, element, Carousel.TransitionDuration, proj.RotationZ, rotationZProjection, Carousel.EasingFunction);
            }
        }

        /// <summary>
        /// Calculate a new projection after a manipulation delta
        /// </summary>
        /// <returns>Return the new projection</returns>
        private Proj GetProjectionFromManipulation(UIElement element, double delta)
        {
            CompositeTransform compositeTransform = element.RenderTransform as CompositeTransform;

            var bounds = Carousel.Orientation == Orientation.Horizontal ? desiredWidth : desiredHeight;
            var margin = Carousel.ItemMargin;

            // this maxsize from middle is the value when depth is max
            var maxBounds = bounds + margin;

            // Calculate the new position of the current item
            var newPosition = Carousel.Orientation == Orientation.Horizontal ? compositeTransform.TranslateX : compositeTransform.TranslateY;
            newPosition = newPosition + delta;

            // Calculate the relative position (to keep something positive)
            var relativePosition = Math.Abs(newPosition);

            // max Depth
            double depth = (double)-Carousel.ItemDepth;

            // rotations
            var rotationX = Carousel.ItemRotationX;
            var rotationY = Carousel.ItemRotationY;
            var rotationZ = Carousel.ItemRotationZ;

            // if the relative position is inside the bounds so calculate the proportionals
            if (relativePosition <= maxBounds)
            {
                depth = relativePosition * depth / maxBounds;
                rotationX = relativePosition * Carousel.ItemRotationX / maxBounds;
                rotationY = relativePosition * Carousel.ItemRotationY / maxBounds;
                rotationZ = relativePosition * Carousel.ItemRotationZ / maxBounds;
            }

            if (Carousel.InvertPositive && newPosition > 0)
            {
                rotationX = -rotationX;
                rotationY = -rotationY;
                rotationZ = -rotationZ;
            }

            return new Proj { Position = newPosition, Depth = depth, RotationX = rotationX, RotationY = rotationY, RotationZ = rotationZ };
        }

        /// <summary>
        /// get the projection from a current index. Used On ArrangeOverride step
        /// </summary>
        /// <returns>Return the new projection</returns>
        private Proj GetProjectionFromSelectedIndex(int childIndex)
        {
            // margin
            var margin = Carousel.ItemMargin;

            // we want the middle image to be index 0, to be sure the centered image is with no rotation
            var relativeIndex = childIndex - Carousel.SelectedIndex;

            // size between each element
            var widthOrHeight = Carousel.Orientation == Orientation.Horizontal ? desiredWidth : desiredHeight;

            // calculate the position with the margin applied
            double position = relativeIndex * (widthOrHeight + margin);

            // Depth orientation
            var depth = relativeIndex == 0 ? 0 : -Carousel.ItemDepth;

            // Rotation on each axes
            var rotationX = relativeIndex == 0 ? 0 : Carousel.ItemRotationX;
            var rotationY = relativeIndex == 0 ? 0 : Carousel.ItemRotationY;
            var rotationZ = relativeIndex == 0 ? 0 : Carousel.ItemRotationZ;

            if (Carousel.InvertPositive && relativeIndex > 0)
            {
                rotationX = -rotationX;
                rotationY = -rotationY;
                rotationZ = -rotationZ;
            }

            return new Proj { Position = position, Depth = depth, RotationX = rotationX, RotationY = rotationY, RotationZ = rotationZ };
        }

        /// <summary>
        /// Add an animation to the current storyboard
        /// </summary>
        public static void AddAnimation(Storyboard storyboard, DependencyObject element, int duration, double toValue, string propertyPath, EasingFunctionBase easingFunction = null)
        {
            DoubleAnimation timeline = new DoubleAnimation
            {
                To = toValue,
                Duration = TimeSpan.FromMilliseconds(duration)
            };

            if (easingFunction != null)
            {
                timeline.EasingFunction = easingFunction;
            }

            storyboard.Children.Add(timeline);

            Storyboard.SetTarget(timeline, element);
            Storyboard.SetTargetProperty(timeline, propertyPath);
        }
    }

    /// <summary>
    /// Structure used when an item moves
    /// </summary>
    public struct Proj
    {
        /// <summary>
        /// Gets or sets the position of an item
        /// </summary>
        public double Position { get; set; }

        /// <summary>
        /// Gets or sets the depth of an item
        /// </summary>
        public double Depth { get; set; }

        /// <summary>
        /// Gets or sets the rotation around the X axis
        /// </summary>
        public double RotationX { get; set; }

        /// <summary>
        /// Gets or sets the rotation around the Y axis
        /// </summary>
        public double RotationY { get; set; }

        /// <summary>
        /// Gets or sets the rotation around the Z axis
        /// </summary>
        public double RotationZ { get; set; }
    }
}