// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Xaml.Interactivity;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.UI.Behaviors
{
    /// <summary>
    /// A class for listening element enter or exit the ScrollViewer viewport
    /// </summary>
    public class ViewportBehavior : BehaviorBase<FrameworkElement>
    {
        /// <summary>
        /// The ScrollViewer hosting this element.
        /// </summary>
        private ScrollViewer _hostScrollViewer;

        /// <summary>
        /// The IsFullyInViewport value of the associated element
        /// </summary>
        public static readonly DependencyProperty IsFullyInViewportProperty =
            DependencyProperty.Register(nameof(IsFullyInViewport), typeof(bool), typeof(ViewportBehavior), new PropertyMetadata(default(bool), OnIsFullyInViewportChanged));

        /// <summary>
        /// The IsInViewport value of the associated element
        /// </summary>
        public static readonly DependencyProperty IsInViewportProperty =
            DependencyProperty.Register(nameof(IsInViewport), typeof(bool), typeof(ViewportBehavior), new PropertyMetadata(default(bool), OnIsInViewportChanged));

        /// <summary>
        /// The IsAlwaysOn value of the associated element
        /// </summary>
        public static readonly DependencyProperty IsAlwaysOnProperty =
            DependencyProperty.Register(nameof(IsAlwaysOn), typeof(bool), typeof(ViewportBehavior), new PropertyMetadata(default(bool)));

        /// <summary>
        /// Associated element fully enter the ScrollViewer viewport event
        /// </summary>
        public event EventHandler EnteredViewport;

        /// <summary>
        /// Associated element fully exit the ScrollViewer viewport event
        /// </summary>
        public event EventHandler ExitedViewport;

        /// <summary>
        /// Associated element enter the ScrollViewer viewport event
        /// </summary>
        public event EventHandler EnteringViewport;

        /// <summary>
        /// Associated element exit the ScrollViewer viewport event
        /// </summary>
        public event EventHandler ExitingViewport;

        /// <summary>
        /// Gets or sets a value indicating whether this behavior will remain attached after the associated element enters the viewport. When false, the behavior will remove itself after entering.
        /// </summary>
        public bool IsAlwaysOn
        {
            get { return (bool)GetValue(IsAlwaysOnProperty); }
            set { SetValue(IsAlwaysOnProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether associated element is fully in the ScrollViewer viewport
        /// </summary>
        public bool IsFullyInViewport
        {
            get { return (bool)GetValue(IsFullyInViewportProperty); }
            private set { SetValue(IsFullyInViewportProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether associated element is in the ScrollViewer viewport
        /// </summary>
        public bool IsInViewport
        {
            get { return (bool)GetValue(IsInViewportProperty); }
            private set { SetValue(IsInViewportProperty, value); }
        }

        /// <summary>
        /// Called after the behavior is attached to the <see cref="P:Microsoft.Xaml.Interactivity.Behavior.AssociatedObject" />.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the <see cref="P:Microsoft.Xaml.Interactivity.Behavior.AssociatedObject" />
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();

            var parent = VisualTreeHelper.GetParent(AssociatedObject);
            if (parent == null)
            {
                AssociatedObject.Loaded += AssociatedObject_Loaded;
                return;
            }

            Init();
        }

        /// <summary>
        /// Called when the behavior is being detached from its <see cref="P:Microsoft.Xaml.Interactivity.Behavior.AssociatedObject" />.
        /// </summary>
        /// <remarks>
        /// Override this to unhook functionality from the <see cref="P:Microsoft.Xaml.Interactivity.Behavior.AssociatedObject" />
        /// </remarks>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            _hostScrollViewer.ViewChanged -= ParentScrollViewer_ViewChanged;
            _hostScrollViewer = null;
        }

        private static void OnIsFullyInViewportChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ViewportBehavior)d;
            var value = (bool)e.NewValue;
            if (value)
            {
                obj.EnteredViewport?.Invoke(obj.AssociatedObject, EventArgs.Empty);

                if (!obj.IsAlwaysOn)
                {
                    Interaction.GetBehaviors(obj.AssociatedObject).Remove(obj);
                }
            }
            else
            {
                obj.ExitingViewport?.Invoke(obj.AssociatedObject, EventArgs.Empty);
            }
        }

        private static void OnIsInViewportChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ViewportBehavior)d;
            var value = (bool)e.NewValue;
            if (value)
            {
                obj.EnteringViewport?.Invoke(obj.AssociatedObject, EventArgs.Empty);
            }
            else
            {
                obj.ExitedViewport?.Invoke(obj.AssociatedObject, EventArgs.Empty);
            }
        }

        private void ParentScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var associatedElementRect = AssociatedObject.TransformToVisual(_hostScrollViewer)
                .TransformBounds(new Rect(0, 0, AssociatedObject.ActualWidth, AssociatedObject.ActualHeight));
            var hostScrollViewerRect = new Rect(0, 0, _hostScrollViewer.ActualWidth, _hostScrollViewer.ActualHeight);

            if (hostScrollViewerRect.Contains(new Point(associatedElementRect.Left, associatedElementRect.Top)) ||
                hostScrollViewerRect.Contains(new Point(associatedElementRect.Right, associatedElementRect.Top)) ||
                hostScrollViewerRect.Contains(new Point(associatedElementRect.Right, associatedElementRect.Bottom)) ||
                hostScrollViewerRect.Contains(new Point(associatedElementRect.Left, associatedElementRect.Bottom)))
            {
                IsInViewport = true;

                if (hostScrollViewerRect.Contains(new Point(associatedElementRect.Left, associatedElementRect.Top)) &&
                    hostScrollViewerRect.Contains(new Point(associatedElementRect.Right, associatedElementRect.Top)) &&
                    hostScrollViewerRect.Contains(new Point(associatedElementRect.Right, associatedElementRect.Bottom)) &&
                    hostScrollViewerRect.Contains(new Point(associatedElementRect.Left, associatedElementRect.Bottom)))
                {
                    IsFullyInViewport = true;
                }
                else
                {
                    IsFullyInViewport = false;
                }
            }
            else
            {
                IsInViewport = false;
            }
        }

        private void Init()
        {
            _hostScrollViewer = AssociatedObject.FindAscendant<ScrollViewer>();
            if (_hostScrollViewer == null)
            {
                throw new InvalidOperationException("This behavior can only be attached to an element which has a ScrollViewer as a parent.");
            }

            _hostScrollViewer.ViewChanged += ParentScrollViewer_ViewChanged;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            Init();
        }
    }
}