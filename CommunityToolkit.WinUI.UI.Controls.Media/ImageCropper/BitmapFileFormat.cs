// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// This denotes the format used when saving a bitmap to a file.
    /// </summary>
    public enum BitmapFileFormat
    {
        /// <summary>
        /// Indicates Windows Imaging Component's bitmap encoder.
        /// </summary>
        Bmp,

        /// <summary>
        /// Indicates Windows Imaging Component's PNG encoder.
        /// </summary>
        Png,

        /// <summary>
        /// Indicates Windows Imaging Component's bitmap JPEG encoder.
        /// </summary>
        Jpeg,

        /// <summary>
        /// Indicates Windows Imaging Component's TIFF encoder.
        /// </summary>
        Tiff,

        /// <summary>
        /// Indicates Windows Imaging Component's GIF encoder.
        /// </summary>
        Gif,

        /// <summary>
        /// Indicates Windows Imaging Component's JPEGXR encoder.
        /// </summary>
        JpegXR
    }
}