// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Foundation;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Class used by the <see cref="OrbitViewPanel"/> to store XY and distance values
    /// </summary>
    public class OrbitViewElementProperties
    {
        /// <summary>
        /// Gets the <see cref="UIElement"/>
        /// </summary>
        public UIElement Element { get; internal set; }

        /// <summary>
        /// Gets the X and Y point from the center
        /// </summary>
        public Point XYFromCenter { get; internal set; }

        /// <summary>
        /// Gets the distance from the center
        /// </summary>
        public double DistanceFromCenter { get; internal set; }
    }
}