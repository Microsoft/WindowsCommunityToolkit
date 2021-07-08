// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// Specifies the horizontal alignment for an image.
    /// </summary>
    public enum AdaptiveImageAlign
    {
        /// <summary>
        /// Default value, alignment behavior determined by renderer.
        /// </summary>
        Default,

        /// <summary>
        /// Image stretches to fill available width (and potentially available height too, depending on where the image is).
        /// </summary>
        [EnumString("stretch")]
        Stretch,

        /// <summary>
        /// Align the image to the left, displaying the image at its native resolution.
        /// </summary>
        [EnumString("left")]
        Left,

        /// <summary>
        /// Align the image in the center horizontally, displaying the image at its native resolution.
        /// </summary>
        [EnumString("center")]
        Center,

        /// <summary>
        /// Align the image to the right, displaying the image at its native resolution.
        /// </summary>
        [EnumString("right")]
        Right
    }

    /// <summary>
    /// Specify the desired cropping of the image.
    /// </summary>
    public enum AdaptiveImageCrop
    {
        /// <summary>
        /// Default value, cropping behavior determined by renderer.
        /// </summary>
        Default,

        /// <summary>
        /// Image is not cropped.
        /// </summary>
        [EnumString("none")]
        None,

        /// <summary>
        /// Image is cropped to a circle shape.
        /// </summary>
        [EnumString("circle")]
        Circle
    }
}