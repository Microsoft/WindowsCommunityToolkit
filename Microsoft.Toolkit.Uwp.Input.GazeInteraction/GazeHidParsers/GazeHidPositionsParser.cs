// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Devices.HumanInterfaceDevice;
using Windows.Devices.Input.Preview;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction.GazeHidParsers
{
    /// <summary>
    /// Hid Positions Parser
    /// </summary>
    public class GazeHidPositionsParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GazeHidPositionsParser"/> class.
        /// </summary>
        /// <param name="gazeDevice"><see cref="GazeDevicePreview"/> used to parse.</param>
        public GazeHidPositionsParser(GazeDevicePreview gazeDevice)
        {
            _leftEyePositionParser = new GazeHidPositionParser(gazeDevice, (ushort)GazeHidUsages.Usage_LeftEyePosition);
            _rightEyePositionParser = new GazeHidPositionParser(gazeDevice, (ushort)GazeHidUsages.Usage_RightEyePosition);
            _headPositionParser = new GazeHidPositionParser(gazeDevice, (ushort)GazeHidUsages.Usage_HeadPosition);
            _headRotationParser = new GazeHidRotationParser(gazeDevice, (ushort)GazeHidUsages.Usage_HeadDirectionPoint);
        }

        /// <summary>
        /// Parses the positions from the report.
        /// </summary>
        /// <param name="report">A <see cref="HidInputReport"/> object used on the parsing.</param>
        /// <returns>The parsed <see cref="GazeHidPositions"/> from the report.</returns>
        public GazeHidPositions GetGazeHidPositions(HidInputReport report)
        {
            return new GazeHidPositions
            {
                LeftEyePosition = this._leftEyePositionParser.GetPosition(report),
                RightEyePosition = this._rightEyePositionParser.GetPosition(report),
                HeadPosition = this._headPositionParser.GetPosition(report),
                HeadRotation = this._headRotationParser.GetRotation(report)
            };
        }

        private readonly GazeHidPositionParser _leftEyePositionParser;
        private readonly GazeHidPositionParser _rightEyePositionParser;
        private readonly GazeHidPositionParser _headPositionParser;
        private readonly GazeHidRotationParser _headRotationParser;
    }
}