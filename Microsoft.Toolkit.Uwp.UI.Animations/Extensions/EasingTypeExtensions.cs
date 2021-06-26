// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;
using System.Diagnostics.Contracts;
using Windows.UI.Xaml.Media.Animation;
using static Microsoft.Toolkit.Uwp.UI.Animations.AnimationExtensions;

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// An extension <see langword="class"/> for the <see cref="EasingType"/> type.
    /// </summary>
    public static class EasingTypeExtensions
    {
        /// <summary>
        /// Gets an <see cref="EasingFunctionBase"/> instance corresponding to a given <see cref="EasingType"/> value.
        /// </summary>
        /// <param name="easingType">The desired easing function type.</param>
        /// <param name="easingMode">The desired easing mode.</param>
        /// <returns>An <see cref="EasingFunctionBase"/> instance corresponding to the input parameters.</returns>
        [Pure]
        public static EasingFunctionBase? ToEasingFunction(this EasingType easingType, EasingMode easingMode = DefaultEasingMode)
        {
            return easingType switch
            {
                EasingType.Linear => null,

                EasingType.Default when easingMode == EasingMode.EaseIn
                    => new ExponentialEase { Exponent = 4.5, EasingMode = EasingMode.EaseIn },
                EasingType.Default when easingMode == EasingMode.EaseOut
                    => new ExponentialEase { Exponent = 7, EasingMode = EasingMode.EaseOut },
                EasingType.Default when easingMode == EasingMode.EaseInOut
                    => new CircleEase { EasingMode = EasingMode.EaseInOut },

                EasingType.Cubic => new CubicEase { EasingMode = easingMode },
                EasingType.Back => new BackEase { EasingMode = easingMode },
                EasingType.Bounce => new BounceEase { EasingMode = easingMode },
                EasingType.Elastic => new ElasticEase { EasingMode = easingMode },
                EasingType.Circle => new CircleEase { EasingMode = easingMode },
                EasingType.Quadratic => new QuadraticEase { EasingMode = easingMode },
                EasingType.Quartic => new QuarticEase { EasingMode = easingMode },
                EasingType.Quintic => new QuinticEase { EasingMode = easingMode },
                EasingType.Sine => new SineEase { EasingMode = easingMode },

                _ => ThrowArgumentException()
            };

            static EasingFunctionBase ThrowArgumentException() => throw new ArgumentException("Invalid easing type");
        }
    }
}