// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.Foundation.Metadata;

#pragma warning disable CS0419

namespace CommunityToolkit.WinUI.UI.Animations
{
    /// <summary>
    /// A type describing the repeat behavior for a custom animation.
    /// </summary>
    [CreateFromString(MethodName = "CommunityToolkit.WinUI.UI.Animations.RepeatOption.Parse")]
    public readonly struct RepeatOption
    {
        /// <summary>
        /// The number of iterations for the animation.
        /// </summary>
        private readonly int value;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepeatOption"/> struct.
        /// </summary>
        /// <param name="value">The number of iterations for the animation.</param>
        private RepeatOption(int value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets a <see cref="RepeatOption"/> value representing a single iteration.
        /// </summary>
        public static RepeatOption Once => new(1);

        /// <summary>
        /// Gets a <see cref="RepeatOption"/> value indicating an animation that repeats forever.
        /// </summary>
        public static RepeatOption Forever => new(-1);

        /// <summary>
        /// Creates a <see cref="RepeatOption"/> value with the specified number of iterations.
        /// </summary>
        /// <param name="count">The number of iterations for the animation.</param>
        /// <returns>A <see cref="RepeatOption"/> value with the specified number of iterations.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="count"/> is negative.</exception>
        [Pure]
        public static RepeatOption Count(int count)
        {
            if (count < 0)
            {
                ThrowArgumentOutOfRangeForCount();
            }

            return new(count);
        }

        /// <summary>
        /// Parses a <see cref="RepeatOption"/> value from a <see cref="string"/>.
        /// The allowed values are either non-negative integers, or "Forever".
        /// </summary>
        /// <param name="text">The input text to parse.</param>
        /// <returns>The parsed <see cref="RepeatOption"/> value.</returns>
        [Pure]
        public static RepeatOption Parse(string text)
        {
            if (int.TryParse(text, out int count))
            {
                return Count(count);
            }

            if (text.Trim().Equals("Forever", StringComparison.InvariantCultureIgnoreCase))
            {
                return Forever;
            }

            return ThrowArgumentExceptionForText();
        }

        /// <summary>
        /// Gets a <see cref="RepeatBehavior"/> value corresponding to the current <see cref="RepeatOption"/> value.
        /// </summary>
        /// <returns>A <see cref="RepeatBehavior"/> value matching the current <see cref="RepeatOption"/> value.</returns>
        [Pure]
        public RepeatBehavior ToRepeatBehavior()
        {
            if (this.value < 0)
            {
                return RepeatBehavior.Forever;
            }

            return new(this.value);
        }

        /// <summary>
        /// Gets the <see cref="AnimationIterationBehavior"/> and count values matching the current <see cref="RepeatOption"/> value.
        /// If the current value represents an infinitely repeating animation, the returned count will be set to 1.
        /// </summary>
        /// <returns>The <see cref="AnimationIterationBehavior"/> and count values matching the current <see cref="RepeatOption"/> value.</returns>
        [Pure]
        public (AnimationIterationBehavior Behavior, int Count) ToBehaviorAndCount()
        {
            if (this.value < 0)
            {
                return (AnimationIterationBehavior.Forever, 1);
            }

            return (AnimationIterationBehavior.Count, this.value);
        }

        /// <summary>
        /// Throws a new <see cref="ArgumentOutOfRangeException"/> when the constructor is invoked with an incorrect parameter.
        /// </summary>
        private static void ThrowArgumentOutOfRangeForCount()
        {
            throw new ArgumentOutOfRangeException("The parameter \"count\" must be greater than or equal to 0.");
        }

        /// <summary>
        /// Throws a new <see cref="ArgumentOutOfRangeException"/> when the constructor is invoked with an incorrect parameter.
        /// </summary>
        private static RepeatOption ThrowArgumentExceptionForText()
        {
            throw new ArgumentException("The input text is not valid to parse a new RepeatOption instance. It must be either a natural number or \"Forever\".");
        }
    }
}