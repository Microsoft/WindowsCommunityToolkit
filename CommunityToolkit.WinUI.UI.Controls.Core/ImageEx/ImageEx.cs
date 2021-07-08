// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// The ImageEx control extends the default Image platform control improving the performance and responsiveness of your Apps.
    /// Source images are downloaded asynchronously showing a load indicator while in progress.
    /// Once downloaded, the source image is stored in the App local cache to preserve resources and load time next time the image needs to be displayed.
    /// </summary>
    public partial class ImageEx : ImageExBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageEx"/> class.
        /// </summary>
        public ImageEx()
            : base()
        {
            DefaultStyleKey = typeof(ImageEx);
        }
    }
}