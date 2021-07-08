// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// Specify the desired cropping of the image.
    /// </summary>
    public enum TileBackgroundImageCrop
    {
        /// <summary>
        /// Cropping style automatically determined by renderer.
        /// </summary>
        Default,

        /// <summary>
        /// Default value. Image is not cropped.
        /// </summary>
        [EnumString("none")]
        None,

        /// <summary>
        /// Image is cropped to a circle shape.
        /// </summary>
        [EnumString("circle")]
        Circle
    }

    /// <summary>
    /// Specify the desired cropping of the image.
    /// </summary>
    public enum TilePeekImageCrop
    {
        /// <summary>
        /// Cropping style automatically determined by renderer.
        /// </summary>
        Default,

        /// <summary>
        /// Default value. Image is not cropped.
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