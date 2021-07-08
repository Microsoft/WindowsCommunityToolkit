// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Helpers
{
    /// <summary>
    /// Enum indicating result of <see cref="CameraHelper"/> initialization.
    /// </summary>
    public enum CameraHelperResult
    {
        /// <summary>
        /// Initialization was successful.
        /// </summary>
        Success,

        /// <summary>
        /// Initialization failed; Frame Reader Creation failed.
        /// </summary>
        CreateFrameReaderFailed,

        /// <summary>
        /// Initialization failed; Unable to start Frame Reader.
        /// </summary>
        StartFrameReaderFailed,

        /// <summary>
        /// Initialization failed; Frame Source Group is null.
        /// </summary>
        NoFrameSourceGroupAvailable,

        /// <summary>
        /// Initialization failed; Frame Source is null.
        /// </summary>
        NoFrameSourceAvailable,

        /// <summary>
        /// Access to the camera is denied.
        /// </summary>
        CameraAccessDenied,

        /// <summary>
        /// Initialization failed due to an exception.
        /// </summary>
        InitializationFailed_UnknownError,

        /// <summary>
        /// Initialization failed; No compatible frame format exposed by the frame source.
        /// </summary>
        NoCompatibleFrameFormatAvailable
    }
}