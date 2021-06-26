// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// An interface representing a XAML model for a custom keyframe.
    /// </summary>
    /// <typeparam name="T">The type of values for the current keyframe.</typeparam>
    public interface IKeyFrame<T>
    {
        /// <summary>
        /// Appends the current keyframe to a target <see cref="INormalizedKeyFrameAnimationBuilder{T}"/> instance.
        /// </summary>
        /// <param name="builder">The target <see cref="INormalizedKeyFrameAnimationBuilder{T}"/> instance to add the keyframe to.</param>
        /// <returns>The same <see cref="INormalizedKeyFrameAnimationBuilder{T}"/> instance as <paramref name="builder"/>.</returns>
        INormalizedKeyFrameAnimationBuilder<T> AppendToBuilder(INormalizedKeyFrameAnimationBuilder<T> builder);
    }
}