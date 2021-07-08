// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Notifications.Adaptive.Elements;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// An inline image.
    /// </summary>
    public sealed class AdaptiveImage
        : IBaseImage,
        IToastBindingGenericChild,
        ITileBindingContentAdaptiveChild,
        IAdaptiveChild,
        IAdaptiveSubgroupChild
    {
        /// <summary>
        /// Gets or sets the desired cropping of the image.
        /// Supported on Tiles since RTM. Supported on Toast since Anniversary Update.
        /// </summary>
        public AdaptiveImageCrop HintCrop { get; set; }

        /// <summary>
        /// Gets or sets a value whether a margin is removed. images have an 8px margin around them.
        /// You can remove this margin by setting this property to true.
        /// Supported on Tiles since RTM. Supported on Toast since Anniversary Update.
        /// </summary>
        public bool? HintRemoveMargin { get; set; }

        /// <summary>
        /// Gets or sets the horizontal alignment of the image.
        /// For Toast, this is only supported when inside an <see cref="AdaptiveSubgroup"/>.
        /// </summary>
        public AdaptiveImageAlign HintAlign { get; set; }

        private string _source;

        /// <summary>
        /// Gets or sets the URI of the image (Required).
        /// Can be from your application package, application data, or the internet.
        /// Internet images must be less than 200 KB in size.
        /// </summary>
        public string Source
        {
            get { return _source; }
            set { BaseImageHelper.SetSource(ref _source, value); }
        }

        /// <summary>
        /// Gets or sets a description of the image, for users of assistive technologies.
        /// </summary>
        public string AlternateText { get; set; }

        /// <summary>
        /// Gets or sets set to true to allow Windows to append a query string to the image URI
        /// supplied in the Tile notification. Use this attribute if your server hosts
        /// images and can handle query strings, either by retrieving an image variant based
        /// on the query strings or by ignoring the query string and returning the image
        /// as specified without the query string. This query string specifies scale,
        /// contrast setting, and language.
        /// </summary>
        public bool? AddImageQuery { get; set; }

        /// <summary>
        /// Returns the image's source string.
        /// </summary>
        /// <returns>The image's source string.</returns>
        public override string ToString()
        {
            if (Source == null)
            {
                return "Source is null";
            }

            return Source;
        }

        internal Element_AdaptiveImage ConvertToElement()
        {
            Element_AdaptiveImage image = BaseImageHelper.CreateBaseElement(this);

            image.Crop = HintCrop;
            image.RemoveMargin = HintRemoveMargin;
            image.Align = HintAlign;
            image.Placement = AdaptiveImagePlacement.Inline;

            return image;
        }
    }
}