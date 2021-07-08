// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Numerics;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;

namespace CommunityToolkit.WinUI.UI.Animations
{
    /// <summary>
    /// Common properties related to extensions.
    /// </summary>
    public static class AnimationExtensions
    {
        /// <summary>
        /// Gets the default delay of animations.
        /// </summary>
        public static TimeSpan DefaultDelay => default;

        /// <summary>
        /// Gets the default duration of animations.
        /// </summary>
        public static TimeSpan DefaultDuration => TimeSpan.FromMilliseconds(400);

        /// <summary>
        /// The default <see cref="EasingType"/> value used for animations.
        /// </summary>
        public const EasingType DefaultEasingType = EasingType.Default;

        /// <summary>
        /// The default <see cref="EasingMode"/> value used for animations.
        /// </summary>
        public const EasingMode DefaultEasingMode = EasingMode.EaseInOut;

        /// <summary>
        /// The default <see cref="AnimationDelayBehavior"/> value used for animations (only applies to composition animations).
        /// </summary>
        public const AnimationDelayBehavior DefaultDelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;

        /// <summary>
        /// The reusable mapping of control points for easing curves for combinations of <see cref="EasingType"/> and <see cref="EasingMode"/> values.
        /// </summary>
        internal static readonly Dictionary<(EasingType Type, EasingMode Mode), (Vector2 A, Vector2 B)> EasingMaps = new()
        {
            // The default/inout combination is missing, as in this case we just skip creating
            // an easing function entirely, and rely on the composition APIs using the implicit
            // easing automatically. This is a bit more efficient, and results in the same
            // visual behavior anyway, as that's the standard combination for animations.
            [(EasingType.Default, EasingMode.EaseOut)] = (new(0.1f, 0.9f), new(0.2f, 1.0f)),
            [(EasingType.Default, EasingMode.EaseIn)] = (new(0.7f, 0.0f), new(1.0f, 0.5f)),

            [(EasingType.Cubic, EasingMode.EaseOut)] = (new(0.215f, 0.61f), new(0.355f, 1f)),
            [(EasingType.Cubic, EasingMode.EaseIn)] = (new(0.55f, 0.055f), new(0.675f, 0.19f)),
            [(EasingType.Cubic, EasingMode.EaseInOut)] = (new(0.645f, 0.045f), new(0.355f, 1f)),

            [(EasingType.Back, EasingMode.EaseOut)] = (new(0.175f, 0.885f), new(0.32f, 1.275f)),
            [(EasingType.Back, EasingMode.EaseIn)] = (new(0.6f, -0.28f), new(0.735f, 0.045f)),
            [(EasingType.Back, EasingMode.EaseInOut)] = (new(0.68f, -0.55f), new(0.265f, 1.55f)),

            [(EasingType.Bounce, EasingMode.EaseOut)] = (new(0.58f, 1.93f), new(.08f, .36f)),
            [(EasingType.Bounce, EasingMode.EaseIn)] = (new(0.93f, 0.7f), new(0.4f, -0.93f)),
            [(EasingType.Bounce, EasingMode.EaseInOut)] = (new(0.65f, -0.85f), new(0.35f, 1.85f)),

            [(EasingType.Elastic, EasingMode.EaseOut)] = (new(0.37f, 2.68f), new(0f, 0.22f)),
            [(EasingType.Elastic, EasingMode.EaseIn)] = (new(1, .78f), new(.63f, -1.68f)),
            [(EasingType.Elastic, EasingMode.EaseInOut)] = (new(0.9f, -1.2f), new(0.1f, 2.2f)),

            [(EasingType.Circle, EasingMode.EaseOut)] = (new(0.075f, 0.82f), new(0.165f, 1f)),
            [(EasingType.Circle, EasingMode.EaseIn)] = (new(0.6f, 0.04f), new(0.98f, 0.335f)),
            [(EasingType.Circle, EasingMode.EaseInOut)] = (new(0.785f, 0.135f), new(0.15f, 0.86f)),

            [(EasingType.Quadratic, EasingMode.EaseOut)] = (new(0.25f, 0.46f), new(0.45f, 0.94f)),
            [(EasingType.Quadratic, EasingMode.EaseIn)] = (new(0.55f, 0.085f), new(0.68f, 0.53f)),
            [(EasingType.Quadratic, EasingMode.EaseInOut)] = (new(0.445f, 0.03f), new(0.515f, 0.955f)),

            [(EasingType.Quartic, EasingMode.EaseOut)] = (new(0.165f, 0.84f), new(0.44f, 1f)),
            [(EasingType.Quartic, EasingMode.EaseIn)] = (new(0.895f, 0.03f), new(0.685f, 0.22f)),
            [(EasingType.Quartic, EasingMode.EaseInOut)] = (new(0.77f, 0.0f), new(0.175f, 1.0f)),

            [(EasingType.Quintic, EasingMode.EaseOut)] = (new(0.23f, 1f), new(0.32f, 1f)),
            [(EasingType.Quintic, EasingMode.EaseIn)] = (new(0.755f, 0.05f), new(0.855f, 0.06f)),
            [(EasingType.Quintic, EasingMode.EaseInOut)] = (new(0.86f, 0.0f), new(0.07f, 1.0f)),

            [(EasingType.Sine, EasingMode.EaseOut)] = (new(0.39f, 0.575f), new(0.565f, 1f)),
            [(EasingType.Sine, EasingMode.EaseIn)] = (new(0.47f, 0.0f), new(0.745f, 0.715f)),
            [(EasingType.Sine, EasingMode.EaseInOut)] = (new(0.445f, 0.05f), new(0.55f, 0.95f))
        };

        /// <summary>
        /// A static container for animatable properties. Composite properties in
        /// the nested classes are manually stored as constants to avoid having
        /// to perform string interpolation at runtime and allocating memory.
        /// </summary>
        internal static class Properties
        {
            /// <summary>
            /// Animatable properties for the composition layer.
            /// </summary>
            public static class Composition
            {
                /// <summary>
                /// Gets the path for a <see cref="Visual.AnchorPoint"/> axis.
                /// </summary>
                /// <param name="axis">The target axis.</param>
                /// <returns>The animation property.</returns>
                [Pure]
                public static string AnchorPoint(Axis axis) => axis switch
                {
                    Axis.X => "AnchorPoint.X",
                    Axis.Y => "AnchorPoint.Y",
                    Axis.Z => "AnchorPoint.Z",
                    _ => ThrowArgumentException<string>("Invalid axis")
                };

                /// <summary>
                /// Gets the path for the <see cref="Visual"/> translation property.
                /// </summary>
                /// <returns>The animation property.</returns>
                [Pure]
                public static string Translation() => "Translation";

                /// <summary>
                /// Gets the path for a <see cref="Visual"/> translation axis.
                /// </summary>
                /// <param name="axis">The target axis.</param>
                /// <returns>The animation property.</returns>
                [Pure]
                public static string Translation(Axis axis) => axis switch
                {
                    Axis.X => "Translation.X",
                    Axis.Y => "Translation.Y",
                    Axis.Z => "Translation.Z",
                    _ => ThrowArgumentException<string>("Invalid axis")
                };

                /// <summary>
                /// Gets the "Translation.XY" constant.
                /// </summary>
                /// <returns>The animation property.</returns>
                [Pure]
                public static string TranslationXY() => "Translation.XY";

                /// <summary>
                /// Gets the path for a <see cref="Visual.Offset"/> axis.
                /// </summary>
                /// <param name="axis">The target axis.</param>
                /// <returns>The animation property.</returns>
                [Pure]
                public static string Offset(Axis axis) => axis switch
                {
                    Axis.X => "Offset.X",
                    Axis.Y => "Offset.Y",
                    Axis.Z => "Offset.Z",
                    _ => ThrowArgumentException<string>("Invalid axis")
                };

                /// <summary>
                /// Gets the "Offset.XY" constant.
                /// </summary>
                /// <returns>The animation property.</returns>
                [Pure]
                public static string OffsetXY() => "Offset.XY";

                /// <summary>
                /// Gets the path for a <see cref="Visual.Scale"/> axis.
                /// </summary>
                /// <param name="axis">The target axis.</param>
                /// <returns>The animation property.</returns>
                [Pure]
                public static string Scale(Axis axis) => axis switch
                {
                    Axis.X => "Scale.X",
                    Axis.Y => "Scale.Y",
                    Axis.Z => "Scale.Z",
                    _ => ThrowArgumentException<string>("Invalid axis")
                };

                /// <summary>
                /// Gets the "Scale.XY" constant.
                /// </summary>
                /// <returns>The animation property.</returns>
                [Pure]
                public static string ScaleXY() => "Scale.XY";

                /// <summary>
                /// Gets the path for a <see cref="Visual.CenterPoint"/> axis.
                /// </summary>
                /// <param name="axis">The target axis.</param>
                /// <returns>The animation property.</returns>
                [Pure]
                public static string CenterPoint(Axis axis) => axis switch
                {
                    Axis.X => "CenterPoint.X",
                    Axis.Y => "CenterPoint.Y",
                    Axis.Z => "CenterPoint.Z",
                    _ => ThrowArgumentException<string>("Invalid axis")
                };

                /// <summary>
                /// Gets the "CenterPoint.XY" constant.
                /// </summary>
                /// <returns>The animation property.</returns>
                [Pure]
                public static string CenterPointXY() => "CenterPoint.XY";

                /// <summary>
                /// Gets the path for an <see cref="InsetClip"/> side.
                /// </summary>
                /// <param name="side">The target side.</param>
                /// <returns>The animation property.</returns>
                [Pure]
                public static string Clip(Side side) => side switch
                {
                    Side.Top => nameof(InsetClip.TopInset),
                    Side.Bottom => nameof(InsetClip.BottomInset),
                    Side.Right => nameof(InsetClip.RightInset),
                    Side.Left => nameof(InsetClip.LeftInset),
                    _ => ThrowArgumentException<string>("Invalid clip side")
                };

                /// <summary>
                /// Gets the "Size" constant.
                /// </summary>
                /// <returns>The animation property.</returns>
                [Pure]
                public static string Size() => "Size";

                /// <summary>
                /// Gets the path for a <see cref="Visual.Size"/> axis.
                /// </summary>
                /// <param name="axis">The target axis.</param>
                /// <returns>The animation property.</returns>
                [Pure]
                public static string Size(Axis axis) => axis switch
                {
                    Axis.X => "Size.X",
                    Axis.Y => "Size.Y",
                    Axis.Z => "Size.Z",
                    _ => ThrowArgumentException<string>("Invalid axis")
                };
            }

            /// <summary>
            /// Animatable properties for the XAML layer.
            /// </summary>
            public static class Xaml
            {
                /// <summary>
                /// Gets the path for a <see cref="CompositeTransform"/> translation axis.
                /// </summary>
                /// <param name="axis">The target axis.</param>
                /// <returns>The animation property.</returns>
                [Pure]
                public static string Translation(Axis axis) => axis switch
                {
                    Axis.X => nameof(CompositeTransform.TranslateX),
                    Axis.Y => nameof(CompositeTransform.TranslateY),
                    _ => ThrowArgumentException<string>("Invalid axis")
                };

                /// <summary>
                /// Gets the path for a <see cref="CompositeTransform"/> scale axis.
                /// </summary>
                /// <param name="axis">The target axis.</param>
                /// <returns>The animation property.</returns>
                [Pure]
                public static string Scale(Axis axis) => axis switch
                {
                    Axis.X => nameof(CompositeTransform.ScaleX),
                    Axis.Y => nameof(CompositeTransform.ScaleY),
                    _ => ThrowArgumentException<string>("Invalid axis")
                };

                /// <summary>
                /// Gets the path for a <see cref="CompositeTransform"/> center point axis.
                /// </summary>
                /// <param name="axis">The target axis.</param>
                /// <returns>The animation property.</returns>
                [Pure]
                public static string CenterPoint(Axis axis) => axis switch
                {
                    Axis.X => nameof(CompositeTransform.CenterX),
                    Axis.Y => nameof(CompositeTransform.CenterY),
                    _ => ThrowArgumentException<string>("Invalid axis")
                };

                /// <summary>
                /// Gets the path for a <see cref="FrameworkElement"/> size axis.
                /// </summary>
                /// <param name="axis">The target axis.</param>
                /// <returns>The animation property.</returns>
                [Pure]
                public static string Size(Axis axis) => axis switch
                {
                    Axis.X => nameof(FrameworkElement.Width),
                    Axis.Y => nameof(FrameworkElement.Height),
                    _ => ThrowArgumentException<string>("Invalid axis")
                };
            }

            /// <summary>
            /// Throws a new <see cref="ArgumentException"/> with a given message.
            /// </summary>
            private static T ThrowArgumentException<T>(string message)
            {
                throw new ArgumentException(message);
            }
        }
    }
}