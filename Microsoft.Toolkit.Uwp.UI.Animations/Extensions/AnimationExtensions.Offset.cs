﻿// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

using System;
using System.Numerics;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// These extension methods use composition to perform animation on visuals.
    /// </summary>
    public static partial class AnimationExtensions
    {
        /// <summary>
        /// Animates the offset of the the UIElement.
        /// </summary>
        /// <param name="associatedObject">The specified UI Element.</param>
        /// <param name="offsetX">The offset on the x axis.</param>
        /// <param name="offsetY">The offset on the y axis.</param>
        /// <param name="offsetZ">The offset on the z axis.</param>
        /// <param name="duration">The duration in milliseconds.</param>
        /// <param name="delay">The delay in milliseconds. (ignored if duration == 0)</param>
        /// <returns>
        /// An AnimationSet.
        /// </returns>
        public static AnimationSet Offset(
            this UIElement associatedObject,
            float offsetX = 0f,
            float offsetY = 0f,
            float offsetZ = 0f,
            double duration = 500d,
            double delay = 0d)
        {
            if (associatedObject == null)
            {
                return null;
            }

            var animationSet = new AnimationSet(associatedObject);
            return animationSet.Offset(offsetX, offsetY, offsetZ, duration, delay);
        }

        /// <summary>
        /// Animates the offset of the the UIElement.
        /// </summary>
        /// <param name="animationSet">The animation set.</param>
        /// <param name="offsetX">The offset on the x axis.</param>
        /// <param name="offsetY">The offset on the y axis.</param>
        /// <param name="offsetZ">The offset on the z axis.</param>
        /// <param name="duration">The duration in milliseconds.</param>
        /// <param name="delay">The delay in milliseconds. (ignored if duration == 0)</param>
        /// <returns>
        /// An AnimationSet.
        /// </returns>
        public static AnimationSet Offset(
            this AnimationSet animationSet,
            float offsetX = 0f,
            float offsetY = 0f,
            float offsetZ = 0f,
            double duration = 500d,
            double delay = 0d)
        {
            if (animationSet == null)
            {
                return null;
            }

            var visual = animationSet.Visual;
            var offsetVector = new Vector3(offsetX, offsetY, offsetZ);

            if (duration <= 0)
            {
                animationSet.AddDirectPropertyChange("Offset", offsetVector);
                return animationSet;
            }

            var compositor = visual?.Compositor;

            if (compositor == null)
            {
                return null;
            }

            var animation = compositor.CreateVector3KeyFrameAnimation();
            animation.Duration = TimeSpan.FromMilliseconds(duration);
            animation.DelayTime = TimeSpan.FromMilliseconds(delay);
            animation.InsertKeyFrame(1f, offsetVector);

            animationSet.AddAnimation("Offset", animation);

            return animationSet;
        }
    }
}
