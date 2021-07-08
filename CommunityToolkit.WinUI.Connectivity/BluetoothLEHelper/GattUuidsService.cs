// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.Connectivity
{
    /// <summary>
    /// Helper class used when working with UUIDs
    /// </summary>
    public static class GattUuidsService
    {
        /// <summary>
        /// Helper function to convert a UUID to a name
        /// </summary>
        /// <param name="uuid">The UUID guid.</param>
        /// <returns>Name of the UUID</returns>
        public static string ConvertUuidToName(Guid uuid)
        {
            GattNativeUuid name;

            if (Enum.TryParse(ConvertUuidToShortId(uuid).ToString(), out name))
            {
                return name.ToString();
            }

            return uuid.ToString();
        }

        /// <summary>
        /// Converts from standard 128bit UUID to the assigned 32bit UUIDs. Makes it easy to compare services
        /// that devices expose to the standard list.
        /// </summary>
        /// <param name="uuid">UUID to convert to 32 bit</param>
        /// <returns>32bit version of the input UUID</returns>
        public static ushort ConvertUuidToShortId(Guid uuid)
        {
            var bytes = uuid.ToByteArray();
            var shortUuid = (ushort)(bytes[0] | (bytes[1] << 8));

            return shortUuid;
        }
    }
}