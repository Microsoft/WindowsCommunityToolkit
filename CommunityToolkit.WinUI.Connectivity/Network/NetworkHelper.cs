// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.Networking.Connectivity;

namespace CommunityToolkit.WinUI.Connectivity
{
    /// <summary>
    /// This class exposes functionality of NetworkInformation through a singleton.
    /// </summary>
    public class NetworkHelper
    {
        /// <summary>
        /// Event raised when the network changes.
        /// </summary>
        public event EventHandler NetworkChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkHelper"/> class.
        /// </summary>
        protected NetworkHelper()
        {
            ConnectionInformation = new ConnectionInformation();

            UpdateConnectionInformation();

            NetworkInformation.NetworkStatusChanged += OnNetworkStatusChanged;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="NetworkHelper"/> class.
        /// </summary>
        ~NetworkHelper()
        {
            NetworkInformation.NetworkStatusChanged -= OnNetworkStatusChanged;
        }

        /// <summary>
        /// Gets public singleton property.
        /// </summary>
        public static NetworkHelper Instance { get; } = new NetworkHelper();

        /// <summary>
        /// Gets instance of <see cref="ConnectionInformation"/>.
        /// </summary>
        public ConnectionInformation ConnectionInformation { get; }

        /// <summary>
        /// Checks the current connection information and raises <see cref="NetworkChanged"/> if needed.
        /// </summary>
        private void UpdateConnectionInformation()
        {
            lock (ConnectionInformation)
            {
                try
                {
                    ConnectionInformation.UpdateConnectionInformation(NetworkInformation.GetInternetConnectionProfile());

                    NetworkChanged?.Invoke(this, EventArgs.Empty);
                }
                catch
                {
                    ConnectionInformation.Reset();
                }
            }
        }

        /// <summary>
        /// Invokes <see cref="UpdateConnectionInformation"/> when the current network status changes.
        /// </summary>
        private void OnNetworkStatusChanged(object sender)
        {
            UpdateConnectionInformation();
        }
    }
}