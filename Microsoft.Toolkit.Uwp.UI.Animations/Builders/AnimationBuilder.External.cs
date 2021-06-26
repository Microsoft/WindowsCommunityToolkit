// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using Windows.UI.Composition;
using Windows.UI.Xaml.Media.Animation;

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <inheritdoc cref="AnimationBuilder"/>
    public sealed partial class AnimationBuilder
    {
        /// <summary>
        /// Adds a new external animation to the current schedule, which will be executed on the same
        /// target object the current <see cref="AnimationBuilder"/> instance will be invoked upon.
        /// </summary>
        /// <param name="animation">The external <see cref="CompositionAnimation"/> instance to add to the schedule.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        public AnimationBuilder ExternalAnimation(CompositionAnimation animation)
        {
            this.compositionAnimationFactories.Add(new ExternalCompositionAnimation(null, animation));

            return this;
        }

        /// <summary>
        /// Adds a new external animation to the current schedule, which will be executed on a given
        /// <see cref="CompositionObject"/> when the current <see cref="AnimationBuilder"/> instance is invoked.
        /// </summary>
        /// <param name="target">The <see cref="CompositionObject"/> target to invoke the animation upon.</param>
        /// <param name="animation">The external <see cref="CompositionAnimation"/> instance to add to the schedule.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        public AnimationBuilder ExternalAnimation(CompositionObject target, CompositionAnimation animation)
        {
            this.compositionAnimationFactories.Add(new ExternalCompositionAnimation(target, animation));

            return this;
        }

        /// <summary>
        /// Adds a new external animation to the current schedule, which will be executed on the same
        /// target object the current <see cref="AnimationBuilder"/> instance will be invoked upon.
        /// </summary>
        /// <param name="animation">The external <see cref="Timeline"/> instance to add to the schedule.</param>
        /// <returns>The current <see cref="AnimationBuilder"/> instance.</returns>
        public AnimationBuilder ExternalAnimation(Timeline animation)
        {
            this.xamlAnimationFactories.Add(new ExternalXamlAnimation(animation));

            return this;
        }
    }
}