// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Helpers
{
    /// <summary>
    /// Defines Operating System version
    /// </summary>
    public struct OSVersion
    {
        /// <summary>
        /// Value describing major version
        /// </summary>
        public ushort Major;

        /// <summary>
        /// Value describing minor version
        /// </summary>
        public ushort Minor;

        /// <summary>
        /// Value describing build
        /// </summary>
        public ushort Build;

        /// <summary>
        /// Value describing revision
        /// </summary>
        public ushort Revision;

        /// <summary>
        /// Converts OSVersion to string
        /// </summary>
        /// <returns>Major.Minor.Build.Revision as a string</returns>
        public override string ToString()
        {
            return $"{Major}.{Minor}.{Build}.{Revision}";
        }
    }
}