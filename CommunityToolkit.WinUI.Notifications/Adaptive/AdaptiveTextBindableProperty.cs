// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Notifications
{
    // Note that this code is only compiled for WinRT. It is not compiled in any of the other projects.
#if WINRT
    /// <summary>
    /// An enumeration of the properties that support data binding on <see cref="AdaptiveText"/> .
    /// </summary>
    public enum AdaptiveTextBindableProperty
    {
        /// <summary>
        /// The text to display. Added in Creators Update only for toast top-level elements.
        /// </summary>
        Text
    }
#endif
}