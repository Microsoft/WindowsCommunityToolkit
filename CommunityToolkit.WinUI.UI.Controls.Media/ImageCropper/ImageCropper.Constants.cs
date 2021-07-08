// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// The <see cref="ImageCropper"/> control allows user to crop image freely.
    /// </summary>
    public partial class ImageCropper
    {
        /// <summary>
        /// Key of the root layout container.
        /// </summary>
        private const string LayoutGridName = "PART_LayoutGrid";

        /// <summary>
        /// Key of the Canvas that contains the image and ImageCropperThumbs.
        /// </summary>
        private const string ImageCanvasPartName = "PART_ImageCanvas";

        /// <summary>
        /// Key of the Image Control inside the ImageCropper Control.
        /// </summary>
        private const string SourceImagePartName = "PART_SourceImage";

        /// <summary>
        /// Key of the mask layer.
        /// </summary>
        private const string MaskAreaPathPartName = "PART_MaskAreaPath";

        /// <summary>
        /// Key of the ImageCropperThumb that on the top.
        /// </summary>
        private const string TopThumbPartName = "PART_TopThumb";

        /// <summary>
        /// Key of the ImageCropperThumb on the bottom.
        /// </summary>
        private const string BottomThumbPartName = "PART_BottomThumb";

        /// <summary>
        /// Key of the ImageCropperThumb on the left.
        /// </summary>
        private const string LeftThumbPartName = "PART_LeftThumb";

        /// <summary>
        /// Key of the ImageCropperThumb on the right.
        /// </summary>
        private const string RightThumbPartName = "PART_RightThumb";

        /// <summary>
        /// Key of the ImageCropperThumb that on the upper left.
        /// </summary>
        private const string UpperLeftThumbPartName = "PART_UpperLeftThumb";

        /// <summary>
        /// Key of the ImageCropperThumb that on the upper right.
        /// </summary>
        private const string UpperRightThumbPartName = "PART_UpperRightThumb";

        /// <summary>
        /// Key of the ImageCropperThumb that on the lower left.
        /// </summary>
        private const string LowerLeftThumbPartName = "PART_LowerLeftThumb";

        /// <summary>
        /// Key of the ImageCropperThumb that on the lower right.
        /// </summary>
        private const string LowerRightThumbPartName = "PART_LowerRightThumb";
    }
}