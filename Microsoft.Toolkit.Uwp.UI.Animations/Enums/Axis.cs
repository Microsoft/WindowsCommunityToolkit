// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// Indicates an axis in the 3D space.
    /// </summary>
    public enum Axis
    {
        /// <summary>
        /// The X axis (horizontal).
        /// </summary>
        X,

        /// <summary>
        /// The Y axis (vertical).
        /// </summary>
        Y,

        /// <summary>
        /// The Z axis (depth).
        /// </summary>
        /// <remarks>
        /// This axis might only be available in certain scenarios, such as when working with composition APIs.
        /// </remarks>
        Z
    }
}