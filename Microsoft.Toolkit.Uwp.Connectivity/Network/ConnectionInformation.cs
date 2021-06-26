// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace Microsoft.Toolkit.Uwp.Connectivity
{
    /// <summary>
    /// This class exposes information about the network connectivity.
    /// </summary>
    public class ConnectionInformation
    {
        private readonly List<string> networkNames = new List<string>();

        /// <summary>
        /// Updates  the current object based on profile passed.
        /// </summary>
        /// <param name="profile">instance of <see cref="ConnectionProfile"/></param>
        public void UpdateConnectionInformation(ConnectionProfile profile)
        {
            if (profile == null)
            {
                Reset();

                return;
            }

            networkNames.Clear();

            uint ianaInterfaceType = profile.NetworkAdapter?.IanaInterfaceType ?? 0;

            switch (ianaInterfaceType)
            {
                case 6:
                    ConnectionType = ConnectionType.Ethernet;
                    break;

                case 71:
                    ConnectionType = ConnectionType.WiFi;
                    break;

                case 243:
                case 244:
                    ConnectionType = ConnectionType.Data;
                    break;

                default:
                    ConnectionType = ConnectionType.Unknown;
                    break;
            }

            var names = profile.GetNetworkNames();
            if (names?.Count > 0)
            {
                networkNames.AddRange(names);
            }

            ConnectivityLevel = profile.GetNetworkConnectivityLevel();

            switch (ConnectivityLevel)
            {
                case NetworkConnectivityLevel.None:
                case NetworkConnectivityLevel.LocalAccess:
                    IsInternetAvailable = false;
                    break;

                default:
                    IsInternetAvailable = true;
                    break;
            }

            ConnectionCost = profile.GetConnectionCost();
            SignalStrength = profile.GetSignalBars();
        }

        /// <summary>
        /// Resets the current object to default values.
        /// </summary>
        internal void Reset()
        {
            networkNames.Clear();

            ConnectionType = ConnectionType.Unknown;
            ConnectivityLevel = NetworkConnectivityLevel.None;
            IsInternetAvailable = false;
            ConnectionCost = null;
            SignalStrength = null;
        }

        /// <summary>
        /// Gets a value indicating whether if the current internet connection is metered.
        /// </summary>
        public bool IsInternetOnMeteredConnection
        {
            get
            {
                return ConnectionCost != null && ConnectionCost.NetworkCostType != NetworkCostType.Unrestricted;
            }
        }

        /// <summary>
        /// Gets a value indicating whether internet is available across all connections.
        /// </summary>
        /// <returns>True if internet can be reached.</returns>
        public bool IsInternetAvailable { get; private set; }

        /// <summary>
        /// Gets connection type for the current Internet Connection Profile.
        /// </summary>
        /// <returns>value of <see cref="ConnectionType"/></returns>
        public ConnectionType ConnectionType { get; private set; }

        /// <summary>
        /// Gets connectivity level for the current Internet Connection Profile.
        /// </summary>
        /// <returns>value of <see cref="NetworkConnectivityLevel"/></returns>
        public NetworkConnectivityLevel ConnectivityLevel { get; private set; }

        /// <summary>
        /// Gets connection cost for the current Internet Connection Profile.
        /// </summary>
        /// <returns>value of <see cref="NetworkConnectivityLevel"/></returns>
        public ConnectionCost ConnectionCost { get; private set; }

        /// <summary>
        /// Gets signal strength for the current Internet Connection Profile.
        /// </summary>
        /// <returns>value of <see cref="NetworkConnectivityLevel"/></returns>
        public byte? SignalStrength { get; private set; }

        /// <summary>
        /// Gets signal strength for the current Internet Connection Profile.
        /// </summary>
        /// <returns>value of <see cref="NetworkConnectivityLevel"/></returns>
        public IReadOnlyList<string> NetworkNames
        {
            get
            {
                return networkNames.AsReadOnly();
            }
        }
    }
}