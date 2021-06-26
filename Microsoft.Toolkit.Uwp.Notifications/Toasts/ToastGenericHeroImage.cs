// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.Notifications.Adaptive.Elements;

namespace Microsoft.Toolkit.Uwp.Notifications
{
    /// <summary>
    /// A hero image for the Toast notification.
    /// </summary>
    public sealed class ToastGenericHeroImage : IBaseImage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToastGenericHeroImage"/> class.
        /// A hero image for the Toast notification.
        /// </summary>
        public ToastGenericHeroImage()
        {
        }

        private string _source;

        /// <summary>
        /// Gets or sets the URI of the image. Can be from your application package, application data, or the internet. Internet images must be less than 200 KB in size.
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
        /// Gets or sets a value whether Windows is allowed to append a query string to the image URI supplied in the Tile notification. Use this attribute if your server hosts images and can handle query strings, either by retrieving an image variant based on the query strings or by ignoring the query string and returning the image as specified without the query string. This query string specifies scale, contrast setting, and language.
        /// </summary>
        public bool? AddImageQuery { get; set; }

        internal Element_AdaptiveImage ConvertToElement()
        {
            Element_AdaptiveImage el = BaseImageHelper.CreateBaseElement(this);

            el.Placement = AdaptiveImagePlacement.Hero;

            return el;
        }
    }
}