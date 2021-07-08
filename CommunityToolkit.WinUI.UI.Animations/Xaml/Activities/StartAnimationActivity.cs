// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;

namespace CommunityToolkit.WinUI.UI.Animations
{
    /// <summary>
    /// An <see cref="IActivity"/> which starts the provided <see cref="AnimationSet"/> when invoked.
    /// </summary>
    public class StartAnimationActivity : Activity
    {
        /// <summary>
        /// Gets or sets the linked <see cref="AnimationSet"/> instance to start.
        /// </summary>
        public AnimationSet Animation
        {
            get => (AnimationSet)GetValue(AnimationProperty);
            set => SetValue(AnimationProperty, value);
        }

        /// <summary>
        /// Identifies the <seealso cref="Animation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimationProperty = DependencyProperty.Register(
            nameof(Animation),
            typeof(AnimationSet),
            typeof(StartAnimationActivity),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the object to start the specified animation on. If not specified, will use the current object the parent animation is running on.
        /// </summary>
        public UIElement TargetObject
        {
            get => (UIElement)GetValue(TargetObjectProperty);
            set => SetValue(TargetObjectProperty, value);
        }

        /// <summary>
        /// Identifies the <seealso cref="TargetObject"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetObjectProperty = DependencyProperty.Register(
            nameof(TargetObject),
            typeof(UIElement),
            typeof(StartAnimationActivity),
            new PropertyMetadata(null));

        /// <inheritdoc/>
        public override async Task InvokeAsync(UIElement element, CancellationToken token)
        {
            if (Animation is null)
            {
                ThrowArgumentNullException();
            }

            await base.InvokeAsync(element, token);

            // If we've specified an explicit target for the animation, we can use that. Otherwise, we can
            // check whether the target animation has an implicit parent. If that's the case, we will use
            // that to start the animation, or just use the input (usually the parent) as fallback.
            if (TargetObject is not null)
            {
                await Animation.StartAsync(TargetObject, token);
            }
            else if (Animation.ParentReference is null)
            {
                await Animation.StartAsync(element, token);
            }
            else
            {
                await Animation.StartAsync(token);
            }

            static void ThrowArgumentNullException() => throw new ArgumentNullException(nameof(Animation));
        }
    }
}