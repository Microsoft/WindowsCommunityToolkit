// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Camera Control to preview video. Can subscribe to video frames, software bitmap when they arrive.
    /// </summary>
    public partial class CameraPreview
    {
        /// <summary>
        /// Key for the MediaPlayerElement Control inside the Camera Preview Control
        /// </summary>
        private const string Preview_MediaPlayerElementControl = "MediaPlayerElementControl";

        /// <summary>
        /// Key for the Frame Source Group Toggle Button Control inside the Camera Preview Control
        /// </summary>
        private const string Preview_FrameSourceGroupButton = "FrameSourceGroupButton";
    }
}