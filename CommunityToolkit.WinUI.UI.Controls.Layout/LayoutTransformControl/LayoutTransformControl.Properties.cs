// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Control that implements support for transformations as if applied by LayoutTransform.
    /// </summary>
    public partial class LayoutTransformControl : Control
    {
        /// <summary>
        /// Gets or sets the single child of the LayoutTransformControl.
        /// </summary>
        /// <remarks>
        /// Corresponds to WPF's Decorator.Child
        /// property.
        /// </remarks>
        public FrameworkElement Child
        {
            get { return (FrameworkElement)GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }

        /// <summary>
        /// Identifies the ChildProperty.
        /// </summary>
        public static readonly DependencyProperty ChildProperty = DependencyProperty.
            Register(
                "Child",
                typeof(FrameworkElement),
                typeof(LayoutTransformControl),
                new PropertyMetadata(null, ChildChanged));

        /// <summary>
        /// Handle changes to the child dependency property.
        /// </summary>
        /// <param name="o">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private static void ChildChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutTransformControl)o).OnChildChanged((FrameworkElement)e.NewValue);
        }

        /// <summary>
        /// Updates content when the child property is changed.
        /// </summary>
        /// <param name="newContent">The new child.</param>
        private void OnChildChanged(FrameworkElement newContent)
        {
            if (_layoutRoot != null)
            {
                // Clear current child
                _layoutRoot.Children.Clear();
                if (newContent != null)
                {
                    // Add the new child to the tree
                    _layoutRoot.Children.Add(newContent);
                }

                // New child means re-layout is necessary
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets or sets the Transform of the LayoutTransformControl.
        /// </summary>
        /// <remarks>
        /// Corresponds to UIElement.RenderTransform.
        /// </remarks>
        public Transform Transform
        {
            get { return (Transform)GetValue(TransformProperty); }
            set { SetValue(TransformProperty, value); }
        }

        /// <summary>
        /// Identifies the TransformProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty TransformProperty = DependencyProperty
            .Register(
                "Transform",
                typeof(Transform),
                typeof(LayoutTransformControl),
                new PropertyMetadata(null, TransformChanged));

        /// <summary>
        /// Handles changes to the Transform DependencyProperty.
        /// </summary>
        /// <param name="o">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private static void TransformChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutTransformControl)o).OnTransformChanged(
                e.OldValue as Transform,
                (Transform)e.NewValue);
        }

        /// <summary>
        /// Processes the transform when the transform is changed.
        /// </summary>
        /// <param name="oldValue">The old transform</param>
        /// <param name="newValue">The transform to process.</param>
        private void OnTransformChanged(Transform oldValue, Transform newValue)
        {
            if (oldValue != null)
            {
                UnsubscribeFromTransformPropertyChanges(oldValue);
            }

            if (newValue != null)
            {
                SubscribeToTransformPropertyChanges(newValue);
            }

            ProcessTransform();
        }

        private void UnsubscribeFromTransformPropertyChanges(Transform transform)
        {
            var propertyChangeEventSources =
                _transformPropertyChangeEventSources[transform];

            foreach (var propertyChangeEventSource in propertyChangeEventSources)
            {
                propertyChangeEventSource.ValueChanged -= OnTransformPropertyChanged;
            }

            _transformPropertyChangeEventSources.Remove(transform);
        }

        private void SubscribeToTransformPropertyChanges(Transform transform)
        {
            var transformGroup = transform as TransformGroup;

            if (transformGroup != null)
            {
                foreach (var childTransform in transformGroup.Children)
                {
                    SubscribeToTransformPropertyChanges(childTransform);
                }

                return;
            }

            var propertyChangeEventSources =
                new List<PropertyChangeEventSource<double>>();
            _transformPropertyChangeEventSources.Add(transform, propertyChangeEventSources);
            var rotateTransform = transform as RotateTransform;

            if (rotateTransform != null)
            {
                var anglePropertyChangeEventSource =
                    new PropertyChangeEventSource<double>(
                        rotateTransform,
                        "Angle");
                anglePropertyChangeEventSource.ValueChanged +=
                    OnTransformPropertyChanged;
                propertyChangeEventSources.Add(anglePropertyChangeEventSource);
                return;
            }

            var scaleTransform = transform as ScaleTransform;

            if (scaleTransform != null)
            {
                var scaleXPropertyChangeEventSource =
                    new PropertyChangeEventSource<double>(
                        scaleTransform,
                        "ScaleX");
                scaleXPropertyChangeEventSource.ValueChanged +=
                    OnTransformPropertyChanged;
                propertyChangeEventSources.Add(scaleXPropertyChangeEventSource);
                var scaleYPropertyChangeEventSource =
                    new PropertyChangeEventSource<double>(
                        scaleTransform,
                        "ScaleY");
                scaleYPropertyChangeEventSource.ValueChanged +=
                    OnTransformPropertyChanged;
                propertyChangeEventSources.Add(scaleYPropertyChangeEventSource);
                return;
            }

            var skewTransform = transform as SkewTransform;

            if (skewTransform != null)
            {
                var angleXPropertyChangeEventSource =
                    new PropertyChangeEventSource<double>(
                        skewTransform,
                        "AngleX");
                angleXPropertyChangeEventSource.ValueChanged +=
                    OnTransformPropertyChanged;
                propertyChangeEventSources.Add(angleXPropertyChangeEventSource);
                var angleYPropertyChangeEventSource =
                    new PropertyChangeEventSource<double>(
                        skewTransform,
                        "AngleY");
                angleYPropertyChangeEventSource.ValueChanged +=
                    OnTransformPropertyChanged;
                propertyChangeEventSources.Add(angleYPropertyChangeEventSource);
                return;
            }

            var matrixTransform = transform as MatrixTransform;

            if (matrixTransform != null)
            {
                var matrixPropertyChangeEventSource =
                    new PropertyChangeEventSource<double>(
                        matrixTransform,
                        "Matrix");
                matrixPropertyChangeEventSource.ValueChanged +=
                    OnTransformPropertyChanged;
                propertyChangeEventSources.Add(matrixPropertyChangeEventSource);
            }
        }

        /// <summary>
        /// Called when a property of a Transform changes.
        /// </summary>
        private void OnTransformPropertyChanged(object sender, double e)
        {
            TransformUpdated();
        }
    }
}