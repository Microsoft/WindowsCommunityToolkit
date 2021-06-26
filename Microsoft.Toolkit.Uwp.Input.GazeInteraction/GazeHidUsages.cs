// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    /// <summary>
    /// This enum specifies the various HID usages specified by the EyeHeadTracker HID specification
    ///
    /// https://www.usb.org/sites/default/files/hutrr74_-_usage_page_for_head_and_eye_trackers_0.pdf
    /// </summary>
    public enum GazeHidUsages
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        UsagePage_EyeHeadTracker = 0x0012,
        Usage_EyeTracker = 0x0001,
        Usage_HeadTracker = 0x0002,

        // 0x0003-0x000F                    RESERVED
        Usage_TrackingData = 0x0010,
        Usage_Capabilities = 0x0011,
        Usage_Configuration = 0x0012,
        Usage_Status = 0x0013,
        Usage_Control = 0x0014,

        // 0x0015-0x001F                    RESERVED
        Usage_Timestamp = 0x0020,
        Usage_PositionX = 0x0021,
        Usage_PositionY = 0x0022,
        Usage_PositionZ = 0x0023,
        Usage_GazePoint = 0x0024,
        Usage_LeftEyePosition = 0x0025,
        Usage_RightEyePosition = 0x0026,
        Usage_HeadPosition = 0x0027,
        Usage_HeadDirectionPoint = 0x0028,
        Usage_RotationX = 0x0029,
        Usage_RotationY = 0x002A,
        Usage_RotationZ = 0x002B,

        // 0x002C-0x00FF                    RESERVED
        Usage_TrackerQuality = 0x0100,
        Usage_MinimumTrackingDistance = 0x0101,
        Usage_OptimumTrackingDistance = 0x0102,
        Usage_MaximumTrackingDistance = 0x0103,
        Usage_MaximumScreenPlaneWidth = 0x0104,
        Usage_MaximumScreenPlaneHeight = 0x0105,

        // 0x0106-0x01FF                    RESERVED
        Usage_DisplayManufacturerId = 0x0200,
        Usage_DisplayProductId = 0x0201,
        Usage_DisplaySerialNumber = 0x0202,
        Usage_DisplayManufacturerDate = 0x0203,
        Usage_CalibratedScreenWidth = 0x0204,
        Usage_CalibratedScreenHeight = 0x0205,

        // 0x0206-0x02FF                    RESERVED
        Usage_SamplingFrequency = 0x0300,
        Usage_ConfigurationStatus = 0x0301,

        // 0x0302-0x03FF                    RESERVED
        Usage_DeviceModeRequest = 0x0400,

        // 0x0401-0xFFFF                    RESERVED
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}