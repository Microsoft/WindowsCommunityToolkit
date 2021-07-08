// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.UI.Media
{
    /// <summary>
    /// Indicates the DPI mode to use to load an image
    /// </summary>
    public enum DpiMode
    {
        /// <summary>
        /// Uses the original DPI settings of the loaded image
        /// </summary>
        UseSourceDpi,

        /// <summary>
        /// Uses the default value of 96 DPI
        /// </summary>
        Default96Dpi,

        /// <summary>
        /// Overrides the image DPI settings with the current screen DPI value
        /// </summary>
        DisplayDpi,

        /// <summary>
        /// Overrides the image DPI settings with the current screen DPI value and ensures the resulting value is at least 96
        /// </summary>
        DisplayDpiWith96AsLowerBound
    }
}