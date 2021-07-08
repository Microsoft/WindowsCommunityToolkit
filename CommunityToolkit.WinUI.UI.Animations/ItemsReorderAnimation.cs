// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Windows.ApplicationModel;

#nullable enable

namespace CommunityToolkit.WinUI.UI.Animations
{
    /// <summary>
    /// Provides the ability to assign a reorder animation to a <see cref="ListViewBase"/>.
    /// </summary>
    public static class ItemsReorderAnimation
    {
        /// <summary>
        /// Identifies the attached "Duration" <see cref="DependencyProperty"/>.
        /// </summary>
        public static readonly DependencyProperty DurationProperty = DependencyProperty.RegisterAttached(
            "Duration",
            typeof(TimeSpan),
            typeof(ItemsReorderAnimation),
            new PropertyMetadata(TimeSpan.Zero, OnDurationChanged));

        /// <summary>
        /// Identifies the attached "ReorderAnimation" <see cref="DependencyProperty"/>.
        /// </summary>
        private static readonly DependencyProperty ReorderAnimationProperty = DependencyProperty.RegisterAttached(
            "ReorderAnimation",
            typeof(ImplicitAnimationCollection),
            typeof(ItemsReorderAnimation),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of the <see cref="DurationProperty"/> property.
        /// </summary>
        /// <param name="listView">The <see cref="ListViewBase"/> to get the value for.</param>
        /// <returns>The retrieved <see cref="TimeSpan"/> value.</returns>
        public static TimeSpan GetDuration(ListViewBase listView)
        {
            return (TimeSpan)listView.GetValue(DurationProperty);
        }

        /// <summary>
        /// Sets a value for the duration, in milliseconds, the animation should take.
        /// </summary>
        /// <param name="listView">the object to set the value on.</param>
        /// <param name="value">The duration.</param>
        public static void SetDuration(ListViewBase listView, TimeSpan value)
        {
            listView.SetValue(DurationProperty, value);
        }

        /// <summary>
        /// Callback to apply the reorder animation when <see cref="DurationProperty"/> changes.
        /// </summary>
        /// <param name="d">The target object the property was changed for.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance for the current event.</param>
        private static void OnDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignMode.DesignModeEnabled)
            {
                return;
            }

            if (d is ListViewBase listView && e.NewValue is TimeSpan duration)
            {
                AssignReorderAnimation(listView, duration);

                listView.ContainerContentChanging -= OnContainerContentChanging;
                listView.ContainerContentChanging += OnContainerContentChanging;

                listView.ChoosingItemContainer -= OnChoosingItemContainer;
                listView.ChoosingItemContainer += OnChoosingItemContainer;
            }
        }

        /// <summary>
        /// Updates the reorder animation for a target <see cref="ListViewBase"/> instance.
        /// </summary>
        /// <param name="listView">The target <see cref="ListViewBase"/> instance.</param>
        /// <param name="duration">The duration of the animation.</param>
        private static void AssignReorderAnimation(ListViewBase listView, TimeSpan duration)
        {
            Compositor compositor = ElementCompositionPreview.GetElementVisual(listView).Compositor;
            ImplicitAnimationCollection animationCollection = (ImplicitAnimationCollection)listView.GetValue(ReorderAnimationProperty);

            if (animationCollection is null)
            {
                animationCollection = compositor.CreateImplicitAnimationCollection();

                listView.SetValue(ReorderAnimationProperty, animationCollection);
            }

            if (duration == TimeSpan.Zero)
            {
                animationCollection.Remove(nameof(Visual.Offset));
            }
            else
            {
                Vector3KeyFrameAnimation offsetAnimation = compositor.CreateVector3KeyFrameAnimation();

                offsetAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
                offsetAnimation.Duration = duration;
                offsetAnimation.Target = nameof(Visual.Offset);

                CompositionAnimationGroup animationGroup = compositor.CreateAnimationGroup();

                animationGroup.Add(offsetAnimation);

                animationCollection[nameof(Visual.Offset)] = animationGroup;
            }
        }

        /// <summary>
        /// Updates the reorder animation to each container, whenever one changes.
        /// </summary>
        /// <param name="sender">The sender <see cref="ListViewBase"/> instance.</param>
        /// <param name="args">The <see cref="ContainerContentChangingEventArgs"/> instance for the current container change.</param>
        private static void OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.InRecycleQueue)
            {
                PokeUIElementZIndex(args.ItemContainer);
            }
            else
            {
                Visual visual = ElementCompositionPreview.GetElementVisual(args.ItemContainer);
                ImplicitAnimationCollection? animationCollection = sender.GetValue(ReorderAnimationProperty) as ImplicitAnimationCollection;

                visual.ImplicitAnimations = animationCollection;
            }
        }

        /// <summary>
        /// Pokes the Z index of each container when one is chosen, to ensure animations are displayed correctly.
        /// </summary>
        /// <param name="sender">The sender <see cref="ListViewBase"/> instance.</param>
        /// <param name="args">The <see cref="ContainerContentChangingEventArgs"/> instance for the current container event.</param>
        private static void OnChoosingItemContainer(ListViewBase sender, ChoosingItemContainerEventArgs args)
        {
            if (args.ItemContainer is not null)
            {
                PokeUIElementZIndex(args.ItemContainer);
            }
        }

        /// <summary>
        /// Pokes the Z index of a target <see cref="UIElement"/>.
        /// </summary>
        /// <param name="element">The target <see cref="UIElement"/> to poke the Z index for.</param>
        private static void PokeUIElementZIndex(UIElement element)
        {
            int oldZIndex = Canvas.GetZIndex(element);

            Canvas.SetZIndex(element, oldZIndex + 1);
            Canvas.SetZIndex(element, oldZIndex);
        }
    }
}