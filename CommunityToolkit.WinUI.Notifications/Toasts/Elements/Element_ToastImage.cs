// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Notifications
{
    [NotificationXmlElement("image")]
    internal sealed class Element_ToastImage : IElement_ToastBindingChild
    {
        internal const ToastImagePlacement DEFAULT_PLACEMENT = ToastImagePlacement.Inline;
        internal const bool DEFAULT_ADD_IMAGE_QUERY = false;
        internal const ToastImageCrop DEFAULT_CROP = ToastImageCrop.None;

        [NotificationXmlAttribute("src")]
        public string Src { get; set; }

        [NotificationXmlAttribute("alt")]
        public string Alt { get; set; }

        [NotificationXmlAttribute("addImageQuery", DEFAULT_ADD_IMAGE_QUERY)]
        public bool AddImageQuery { get; set; } = DEFAULT_ADD_IMAGE_QUERY;

        [NotificationXmlAttribute("placement", DEFAULT_PLACEMENT)]
        public ToastImagePlacement Placement { get; set; } = DEFAULT_PLACEMENT;

        [NotificationXmlAttribute("hint-crop", DEFAULT_CROP)]
        public ToastImageCrop Crop { get; set; } = DEFAULT_CROP;
    }

    /// <summary>
    /// Specify the desired cropping of the image.
    /// </summary>
    public enum ToastImageCrop
    {
        /// <summary>
        /// Default value. Image is not cropped.
        /// </summary>
        None,

        /// <summary>
        /// Image is cropped to a circle shape.
        /// </summary>
        [EnumString("circle")]
        Circle
    }

    internal enum ToastImagePlacement
    {
        Inline,

        [EnumString("appLogoOverride")]
        AppLogoOverride,

        [EnumString("hero")]
        Hero
    }
}