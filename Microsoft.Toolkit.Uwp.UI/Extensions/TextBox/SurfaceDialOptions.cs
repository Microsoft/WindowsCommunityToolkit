// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI
{
    /// <summary>
    /// A model containing options for configuring the Surface Dial support through <see cref="TextBoxExtensions"/>.
    /// </summary>
    public sealed class SurfaceDialOptions : DependencyObject
    {
        /// <summary>
        /// Gets an internal cached instance to avoid allocations from <see cref="TextBoxExtensions"/>.
        /// </summary>
        internal static SurfaceDialOptions Default { get; } = new();

        /// <summary>
        /// Gets or sets the default icon of the menu item that gets added.
        /// This will be visible if a user opens their Surface Dial menu by long-pressing the device.
        /// Defaults to <see cref="RadialControllerMenuKnownIcon.Ruler"/>.
        /// </summary>
        public RadialControllerMenuKnownIcon Icon { get; set; } = RadialControllerMenuKnownIcon.Ruler;

        /// <summary>
        /// Gets or sets the amount the <see cref="TextBox"/> value will be modified for each <see cref="RotationResolutionInDegrees"/> step on the Surface Dial.
        /// This can be any double value.
        /// </summary>
        public double StepValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable the haptic feedback when rotating the dial for the give TextBox.
        /// This is enabled by default.
        /// </summary>
        public bool EnableHapticFeedback { get; set; } = true;

        /// <summary>
        /// Gets or sets the <see cref="RadialController.RotationResolutionInDegrees"/> property for the extension which is the amount the dial needs to rotate to trigger a change. Default is 10.
        /// </summary>
        public double RotationResolutionInDegrees { get; set; } = 10;

        /// <summary>
        /// Gets or sets the minimum value the <see cref="TextBox"/> can have when modifying it using a Surface Dial.
        /// Default is -100.0.
        /// </summary>
        public double MinValue { get; set; } = -100;

        /// <summary>
        /// Gets or sets the maximum value the <see cref="TextBox"/> can have when modifying it using a Surface Dial.
        /// Default is 100.0.
        /// </summary>
        public double MaxValue { get; set; } = 100;

        /// <summary>
        /// Gets or sets a value indicating whether to automatically try to focus the next focusable element from the Surface Dial enabled <see cref="TextBox"/>.
        /// This is on by default.
        /// </summary>
        public bool EnableTapToNextControl { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to limit the value in the <see cref="TextBox"/> to <see cref="MinValue"/> and <see cref="MaxValue"/>.
        /// </summary>
        public bool EnableMinMaxValue { get; set; }
    }
}
