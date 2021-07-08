// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Notifications
{
    // Note that this code is only compiled for WinRT. It is not compiled in any of the other projects.
#if WINRT
    /// <summary>
    /// An enumeration of the properties that support data binding on <see cref="AdaptiveProgressBar"/> .
    /// </summary>
    public enum AdaptiveProgressBarBindableProperty
    {
        /// <summary>
        /// An optional title string
        /// </summary>
        Title,

        /// <summary>
        /// The value of the progress bar.
        /// </summary>
        Value,

        /// <summary>
        /// An optional string to be displayed instead of the default percentage string. If this isn't provided, something like "70%" will be displayed.
        /// </summary>
        ValueStringOverride,

        /// <summary>
        /// An optional status string, which is displayed underneath the progress bar. If provided, this string should reflect the status of the download, like "Downloading..." or "Installing...".
        /// </summary>
        Status
    }
#endif
}