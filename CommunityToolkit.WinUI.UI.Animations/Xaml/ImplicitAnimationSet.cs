// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;
using System.Diagnostics.Contracts;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using Windows.Foundation.Collections;

namespace CommunityToolkit.WinUI.UI.Animations
{
    /// <summary>
    /// A collection of implicit animations that can be assigned to a <see cref="UIElement"/> and configured to be run automatically
    /// when the element is either shown or hidden (through <see cref="Implicit.ShowAnimationsProperty"/> and <see cref="Implicit.HideAnimationsProperty"/>),
    /// or whenever one of the targeted properties on the underlying <see cref="Visual"/> element changes (through <see cref="Implicit.AnimationsProperty"/>).
    /// <para>
    /// Animations within an <see cref="ImplicitAnimationSet"/> should be objects implementing the <see cref="IImplicitTimeline"/> interface, such as
    /// types inheriting from <see cref="ImplicitAnimation{TValue, TKeyFrame}"/> (eg. <see cref="OpacityAnimation"/>, <see cref="TranslationAnimation"/>,
    /// <see cref="OffsetAnimation"/> and <see cref="ScaleAnimation"/>, or custom ones such as <see cref="ScalarAnimation"/> and <see cref="Vector3Animation"/>).
    /// Adding incompatible elements cannot be validated at build-time, but will result in a runtime crash.
    /// </para>
    /// <para>
    /// Animations will monitor for changes in real-time to any of their public properties. For instance, if a binding is used to dynamically update the
    /// <see cref="Animation{TValue, TKeyFrame}.To"/> or <see cref="Animation{TValue, TKeyFrame}.From"/> properties, the entire animation set will be
    /// initialized again and assigned to the underlying <see cref="Visual"/> object for the targeted <see cref="UIElement"/>. This does not currently apply
    /// to changes to the <see cref="Animation{TValue, TKeyFrame}.KeyFrames"/> property though (other than the entire property being reassigned). To achieve
    /// dynamic updates to animation sets in that case, either leverage expression keyframes or just use code-behind to manually reinitialize the animations.
    /// </para>
    /// </summary>
    /// <remarks>
    /// An <see cref="ImplicitAnimationSet"/> instance can only be used on a single <see cref="UIElement"/> target, and it cannot be shared across multiple
    /// elements. Attempting to do so will result in a runtime crash. Furthermore, it is recommended not to move <see cref="IImplicitTimeline"/> instances from
    /// one <see cref="ImplicitAnimationSet"/> to another, and doing so will add unnecessary runtime overhead over time. If you want to apply the same animations
    /// to multiple elements, simply create another <see cref="ImplicitAnimationSet"/> instance and another set of animations with the same properties within it.
    /// </remarks>
    public sealed class ImplicitAnimationSet : DependencyObjectCollection
    {
        /// <summary>
        /// Raised whenever any configuration change occurrs within the current <see cref="ImplicitAnimationSet"/> instance.
        /// </summary>
        internal event EventHandler? AnimationsChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImplicitAnimationSet"/> class.
        /// </summary>
        public ImplicitAnimationSet()
        {
            VectorChanged += ImplicitAnimationSetVectorChanged;
        }

        /// <summary>
        /// Registers <see cref="RaiseAnimationsChanged(object, EventArgs)"/> for every added animation.
        /// </summary>
        /// <param name="sender">The current vector of animations.</param>
        /// <param name="event">The <see cref="IVectorChangedEventArgs"/> instance for the current event.</param>
        private void ImplicitAnimationSetVectorChanged(IObservableVector<DependencyObject> sender, IVectorChangedEventArgs @event)
        {
            if (@event.CollectionChange == CollectionChange.ItemInserted ||
                @event.CollectionChange == CollectionChange.ItemChanged)
            {
                IImplicitTimeline item = (IImplicitTimeline)sender[(int)@event.Index];

                item.AnimationPropertyChanged -= RaiseAnimationsChanged;
                item.AnimationPropertyChanged += RaiseAnimationsChanged;
            }

            AnimationsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="AnimationsChanged"/> event.
        /// </summary>
        /// <param name="sender">The instance raising the event.</param>
        /// <param name="e">The empty <see cref="EventArgs"/> for the event.</param>
        private void RaiseAnimationsChanged(object? sender, EventArgs e)
        {
            AnimationsChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Gets or sets the weak reference to the parent that owns the current implicit animation collection.
        /// </summary>
        internal WeakReference<UIElement>? ParentReference { get; set; }

        /// <summary>
        /// Creates a <see cref="CompositionAnimationGroup"/> for the current collection.
        /// This can be used to be assigned to show/hide implicit composition animations.
        /// </summary>
        /// <param name="parent">The target <see cref="UIElement"/> to which the animations are being applied to.</param>
        /// <returns>The <see cref="CompositionAnimationGroup"/> instance to use.</returns>
        [Pure]
        internal CompositionAnimationGroup GetCompositionAnimationGroup(UIElement parent)
        {
            Compositor compositor = ElementCompositionPreview.GetElementVisual(parent).Compositor;
            CompositionAnimationGroup animations = compositor.CreateAnimationGroup();

            foreach (IImplicitTimeline timeline in this)
            {
                animations.Add(timeline.GetAnimation(parent, out _));
            }

            return animations;
        }

        /// <summary>
        /// Creates an <see cref="ImplicitAnimationCollection"/> for the current collection.
        /// This can be used to be assigned to implicit composition animations.
        /// </summary>
        /// <param name="parent">The target <see cref="UIElement"/> to which the animations are being applied to.</param>
        /// <returns>The <see cref="ImplicitAnimationCollection"/> instance to use.</returns>
        [Pure]
        internal ImplicitAnimationCollection GetImplicitAnimationCollection(UIElement parent)
        {
            Compositor compositor = ElementCompositionPreview.GetElementVisual(parent).Compositor;
            ImplicitAnimationCollection animations = compositor.CreateImplicitAnimationCollection();

            foreach (IImplicitTimeline timeline in this)
            {
                CompositionAnimation animation = timeline.GetAnimation(parent, out string? target);

                target ??= animation.Target;

                if (!animations.ContainsKey(target))
                {
                    animations[target] = animations.Compositor.CreateAnimationGroup();
                }

                ((CompositionAnimationGroup)animations[target]).Add(animation);
            }

            return animations;
        }
    }
}