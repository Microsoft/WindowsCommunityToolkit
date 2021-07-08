// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Numerics;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;

namespace CommunityToolkit.WinUI.UI.Animations
{
    /// <summary>
    /// Provides attached dependency properties and methods for the <see cref="ScrollViewer"/> control.
    /// </summary>
    public static partial class ScrollViewerExtensions
    {
        /// <summary>
        /// Creates and starts an animation on the target element that binds either the X or Y axis of the source <see cref="ScrollViewer"/>.
        /// </summary>
        /// <param name="scroller">The source <see cref="ScrollViewer"/> control to use.</param>
        /// <param name="target">The target <see cref="UIElement"/> that will be animated.</param>
        /// <param name="axis">The scrolling axis of the source <see cref="ScrollViewer"/>.</param>
        /// <param name="property">The target <see cref="Visual"/> property to animate.</param>
        /// <returns>An <see cref="ExpressionAnimation"/> instance that represents an already running animation.</returns>
        public static ExpressionAnimation StartExpressionAnimation(
            this ScrollViewer scroller,
            UIElement target,
            Axis axis,
            VisualProperty property = VisualProperty.Translation)
        {
            return scroller.StartExpressionAnimation(target, axis, axis, property);
        }

        /// <summary>
        /// Creates and starts an animation on the target element that binds either the X or Y axis of the source <see cref="ScrollViewer"/>
        /// </summary>
        /// <param name="scroller">The source <see cref="ScrollViewer"/> control to use</param>
        /// <param name="target">The target <see cref="UIElement"/> that will be animated</param>
        /// <param name="sourceAxis">The scrolling axis of the source <see cref="ScrollViewer"/></param>
        /// <param name="targetAxis">The optional scrolling axis of the target element, if <see langword="null"/> the source axis will be used</param>
        /// <param name="property">The target <see cref="Visual"/> property to animate.</param>
        /// <returns>An <see cref="ExpressionAnimation"/> instance that represents an already running animation.</returns>
        public static ExpressionAnimation StartExpressionAnimation(
            this ScrollViewer scroller,
            UIElement target,
            Axis sourceAxis,
            Axis targetAxis,
            VisualProperty property = VisualProperty.Translation)
        {
            CompositionPropertySet scrollSet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scroller);
            ExpressionAnimation animation = scrollSet.Compositor.CreateExpressionAnimation($"{nameof(scroller)}.{nameof(UIElement.Translation)}.{sourceAxis}");

            animation.SetReferenceParameter(nameof(scroller), scrollSet);

            Visual visual = ElementCompositionPreview.GetElementVisual(target);

            switch (property)
            {
                case VisualProperty.Translation:
                    ElementCompositionPreview.SetIsTranslationEnabled(target, true);
                    visual.StartAnimation($"{nameof(Matrix4x4.Translation)}.{targetAxis}", animation);
                    break;
                case VisualProperty.Offset:
                    visual.StartAnimation($"{nameof(Visual.Offset)}.{targetAxis}", animation);
                    break;
                default:
                    ThrowArgumentException();
                    break;
            }

            return animation;

            static ExpressionAnimation ThrowArgumentException() => throw new ArgumentException("Invalid target property");
        }
    }
}