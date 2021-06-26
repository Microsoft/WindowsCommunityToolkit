// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Devices.HumanInterfaceDevice;
using Windows.Devices.Input.Preview;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction.GazeHidParsers
{
    internal static class GazeHidParsersHelpers
    {
        public static HidNumericControlDescription GetGazeUsageFromCollectionId(
            GazeDevicePreview gazeDevice,
            ushort childUsageId,
            ushort parentUsageId)
        {
            var numericControls = gazeDevice.GetNumericControlDescriptions(
                (ushort)GazeHidUsages.UsagePage_EyeHeadTracker, childUsageId);

            for (int i = 0; i < numericControls.Count; i++)
            {
                var parentCollections = numericControls[i].ParentCollections;
                if (parentCollections.Count > 0 &&
                    parentCollections[0].UsagePage == (ushort)GazeHidUsages.UsagePage_EyeHeadTracker &&
                    parentCollections[0].UsageId == parentUsageId)
                {
                    return numericControls[i];
                }
            }

            return null;
        }
    }
}