// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Uwp.Notifications
{
    /// <summary>
    /// Specify the desired cropping of the image.
    /// </summary>
    public enum ToastGenericAppLogoCrop
    {
        /// <summary>
        /// Cropping uses the default behavior of the renderer.
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