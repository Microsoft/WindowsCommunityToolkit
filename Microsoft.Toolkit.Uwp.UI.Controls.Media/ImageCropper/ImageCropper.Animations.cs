// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// The <see cref="ImageCropper"/> control allows user to crop image freely.
    /// </summary>
    public partial class ImageCropper
    {
        private static void AnimateUIElementOffset(Point to, TimeSpan duration, UIElement target)
        {
            var targetVisual = ElementCompositionPreview.GetElementVisual(target);
            var compositor = targetVisual.Compositor;
            var linear = compositor.CreateLinearEasingFunction();
            var offsetAnimation = compositor.CreateVector3KeyFrameAnimation();
            offsetAnimation.Duration = duration;
            offsetAnimation.Target = nameof(Visual.Offset);
            offsetAnimation.InsertKeyFrame(1.0f, new Vector3((float)to.X, (float)to.Y, 0), linear);
            targetVisual.StartAnimation(nameof(Visual.Offset), offsetAnimation);
        }

        private static void AnimateUIElementScale(double to, TimeSpan duration, UIElement target)
        {
            var targetVisual = ElementCompositionPreview.GetElementVisual(target);
            var compositor = targetVisual.Compositor;
            var linear = compositor.CreateLinearEasingFunction();
            var scaleAnimation = compositor.CreateVector3KeyFrameAnimation();
            scaleAnimation.Duration = duration;
            scaleAnimation.Target = nameof(Visual.Scale);
            scaleAnimation.InsertKeyFrame(1.0f, new Vector3((float)to), linear);
            targetVisual.StartAnimation(nameof(Visual.Scale), scaleAnimation);
        }

        private static DoubleAnimation CreateDoubleAnimation(double to, TimeSpan duration, DependencyObject target, string propertyName, bool enableDependentAnimation)
        {
            var animation = new DoubleAnimation()
            {
                To = to,
                Duration = duration,
                EnableDependentAnimation = enableDependentAnimation
            };

            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, propertyName);

            return animation;
        }

        private static PointAnimation CreatePointAnimation(Point to, TimeSpan duration, DependencyObject target, string propertyName, bool enableDependentAnimation)
        {
            var animation = new PointAnimation()
            {
                To = to,
                Duration = duration,
                EnableDependentAnimation = enableDependentAnimation
            };

            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, propertyName);

            return animation;
        }

        private static ObjectAnimationUsingKeyFrames CreateRectangleAnimation(Rect to, TimeSpan duration, RectangleGeometry rectangle, bool enableDependentAnimation)
        {
            var animation = new ObjectAnimationUsingKeyFrames
            {
                Duration = duration,
                EnableDependentAnimation = enableDependentAnimation
            };

            var frames = GetRectKeyframes(rectangle.Rect, to, duration);
            foreach (var item in frames)
            {
                animation.KeyFrames.Add(item);
            }

            Storyboard.SetTarget(animation, rectangle);
            Storyboard.SetTargetProperty(animation, nameof(RectangleGeometry.Rect));

            return animation;
        }

        private static List<DiscreteObjectKeyFrame> GetRectKeyframes(Rect from, Rect to, TimeSpan duration)
        {
            var rectKeyframes = new List<DiscreteObjectKeyFrame>();
            var step = TimeSpan.FromMilliseconds(10);
            var startPointFrom = new Point(from.X, from.Y);
            var endPointFrom = new Point(from.X + from.Width, from.Y + from.Height);
            var startPointTo = new Point(to.X, to.Y);
            var endPointTo = new Point(to.X + to.Width, to.Y + to.Height);
            for (var time = default(TimeSpan); time < duration; time += step)
            {
                var progress = time.TotalMilliseconds / duration.TotalMilliseconds;
                var startPoint = new Point
                {
                    X = startPointFrom.X + (progress * (startPointTo.X - startPointFrom.X)),
                    Y = startPointFrom.Y + (progress * (startPointTo.Y - startPointFrom.Y)),
                };
                var endPoint = new Point
                {
                    X = endPointFrom.X + (progress * (endPointTo.X - endPointFrom.X)),
                    Y = endPointFrom.Y + (progress * (endPointTo.Y - endPointFrom.Y)),
                };
                rectKeyframes.Add(new DiscreteObjectKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(time),
                    Value = startPoint.ToRect(endPoint)
                });
            }

            rectKeyframes.Add(new DiscreteObjectKeyFrame
            {
                KeyTime = duration,
                Value = to
            });
            return rectKeyframes;
        }
    }
}
