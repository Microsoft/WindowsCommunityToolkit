// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Xaml.Interactivity;

namespace Microsoft.Toolkit.Uwp.UI.Behaviors
{
    /// <summary>
    /// A custom <see cref="Trigger"/> that fires whenever a linked <see cref="AnimationSet"/> completes.
    /// </summary>
    public sealed class AnimationCompletedTriggerBehavior : Trigger<AnimationSet>
    {
        /// <summary>
        /// The current <see cref="AnimationSet"/> instance in use.
        /// </summary>
        private AnimationSet? animationCollection;

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();

            SetResolvedCollection(AssociatedObject);
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            SetResolvedCollection(null);
        }

        /// <summary>
        /// Sets the current <see cref="AnimationSet"/> instance in use.
        /// </summary>
        /// <param name="animationCollection">The <see cref="AnimationSet"/> instance in use.</param>
        private void SetResolvedCollection(AnimationSet? animationCollection)
        {
            if (this.animationCollection == animationCollection)
            {
                return;
            }

            if (this.animationCollection is not null)
            {
                this.animationCollection.Completed -= AnimationCollection_Completed;
            }

            this.animationCollection = animationCollection;

            if (animationCollection is not null)
            {
                animationCollection.Completed += AnimationCollection_Completed;
            }
        }

        /// <summary>
        /// Invokes the current actions when the linked animations completes.
        /// </summary>
        /// <param name="sender">The source <see cref="AnimationSet"/> instance.</param>
        /// <param name="e">The arguments for the event (unused).</param>
        private void AnimationCollection_Completed(object sender, System.EventArgs e)
        {
            Interaction.ExecuteActions(sender, Actions, e);
        }
    }
}