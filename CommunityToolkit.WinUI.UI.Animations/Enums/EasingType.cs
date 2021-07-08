// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.UI.Animations
{
    /// <summary>
    /// Indicates how the animation interpolates between keyframes.
    /// </summary>
    public enum EasingType
    {
        /// <summary>
        /// The default easing type, which is specified in <see cref="AnimationExtensions.DefaultEasingType"/>.
        /// Animations using this easing type follow the guidelines mentioned in the "Timing and easing" section of the docs.
        /// For more info, see: <see href="https://docs.microsoft.com/windows/uwp/design/motion/timing-and-easing"/>.
        /// </summary>
        Default,

        /// <summary>
        /// A linear acceleration and deceleration.
        /// </summary>
        Linear,

        /// <summary>
        /// An acceleration or deceleration using the formula f(t) = t3.
        /// </summary>
        Cubic,

        /// <summary>
        /// An animation that rectracts its motion slightly before it begins to animate in the path indicated.
        /// </summary>
        Back,

        /// <summary>
        /// A bouncing animation.
        /// </summary>
        Bounce,

        /// <summary>
        /// An animation that resembles a spring oscillating back and forth until it comes to rest.
        /// </summary>
        Elastic,

        /// <summary>
        /// An animation that accelerates or decelerates using a circular function.
        /// </summary>
        Circle,

        /// <summary>
        /// An animation that accelerates or decelerates using the formula f(t) = t^2.
        /// </summary>
        Quadratic,

        /// <summary>
        /// An animation that accelerates or decelerates using the formula f(t) = t^4.
        /// </summary>
        Quartic,

        /// <summary>
        /// An animation that accelerates or decelerates using the formula f(t) = t^5.
        /// </summary>
        Quintic,

        /// <summary>
        /// An animation that accelerates or decelerates using a sine formula.
        /// </summary>
        Sine
    }
}