// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Uwp.Connectivity
{
    /// <summary>
    /// Enumeration denoting connection type.
    /// </summary>
    public enum ConnectionType
    {
        /// <summary>
        /// Connected to wired network
        /// </summary>
        Ethernet,

        /// <summary>
        /// Connected to wireless network
        /// </summary>
        WiFi,

        /// <summary>
        /// Connected to mobile data connection
        /// </summary>
        Data,

        /// <summary>
        /// Connection type not identified
        /// </summary>
        Unknown,
    }
}