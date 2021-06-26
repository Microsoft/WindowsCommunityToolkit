// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Devices.HumanInterfaceDevice;
using Windows.Devices.Input.Preview;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction.GazeHidParsers
{
    /// <summary>
    /// Hid Position Parser
    /// </summary>
    public class GazeHidPositionParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GazeHidPositionParser"/> class.
        /// </summary>
        /// <param name="gazeDevice"><see cref="GazeDevicePreview"/> used to parse.</param>
        /// <param name="usage">The <see cref="GazeHidUsages"/> used to parse.</param>
        public GazeHidPositionParser(GazeDevicePreview gazeDevice, ushort usage)
        {
            _usage = usage;

            // Find all the position usages from the device's
            // descriptor and store them for easy access
            _x = GazeHidParsersHelpers.GetGazeUsageFromCollectionId(gazeDevice, (ushort)GazeHidUsages.Usage_PositionX, _usage);
            _y = GazeHidParsersHelpers.GetGazeUsageFromCollectionId(gazeDevice, (ushort)GazeHidUsages.Usage_PositionY, _usage);
            _z = GazeHidParsersHelpers.GetGazeUsageFromCollectionId(gazeDevice, (ushort)GazeHidUsages.Usage_PositionZ, _usage);
        }

        /// <summary>
        /// Parses the position from the report.
        /// </summary>
        /// <param name="report">A <see cref="HidInputReport"/> object used on the parsing.</param>
        /// <returns>The parsed <see cref="GazeHidPositions"/> from the report.</returns>
        public GazeHidPosition GetPosition(HidInputReport report)
        {
            GazeHidPosition result = null;

            if (_x != null &&
                _y != null &&
                _z != null &&
                _usage != 0x0000)
            {
                var descX = report.GetNumericControlByDescription(_x);
                var descY = report.GetNumericControlByDescription(_y);
                var descZ = report.GetNumericControlByDescription(_z);

                var controlDescX = descX.ControlDescription;
                var controlDescY = descY.ControlDescription;
                var controlDescZ = descZ.ControlDescription;

                if ((controlDescX.LogicalMaximum < descX.ScaledValue || controlDescX.LogicalMinimum > descX.ScaledValue) ||
                    (controlDescY.LogicalMaximum < descY.ScaledValue || controlDescY.LogicalMinimum > descY.ScaledValue) ||
                    (controlDescZ.LogicalMaximum < descZ.ScaledValue || controlDescZ.LogicalMinimum > descZ.ScaledValue))
                {
                    // One of the values is outside of the valid range.
                }
                else
                {
                    result = new GazeHidPosition
                    {
                        X = descX.ScaledValue,
                        Y = descY.ScaledValue,
                        Z = descZ.ScaledValue
                    };
                }
            }

            return result;
        }

        private readonly HidNumericControlDescription _x = null;
        private readonly HidNumericControlDescription _y = null;
        private readonly HidNumericControlDescription _z = null;
        private readonly ushort _usage = 0x0000;
    }
}