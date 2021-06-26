// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using static Microsoft.Toolkit.Uwp.UI.Animations.AnimationExtensions;

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// A clip animation working on the composition layer.
    /// </summary>
    public sealed class ClipAnimation : Animation<Thickness?, Thickness>
    {
        /// <inheritdoc/>
        protected override string ExplicitTarget => throw new NotImplementedException();

        /// <inheritdoc/>
        public override AnimationBuilder AppendToBuilder(AnimationBuilder builder, TimeSpan? delayHint, TimeSpan? durationHint, EasingType? easingTypeHint, EasingMode? easingModeHint)
        {
            return builder.Clip(
                To!.Value,
                From,
                Delay ?? delayHint,
                Duration ?? durationHint,
                Repeat,
                EasingType ?? easingTypeHint ?? DefaultEasingType,
                EasingMode ?? easingModeHint ?? DefaultEasingMode);
        }

        /// <inheritdoc/>
        protected override (Thickness?, Thickness?) GetParsedValues()
        {
            throw new NotImplementedException();
        }
    }
}