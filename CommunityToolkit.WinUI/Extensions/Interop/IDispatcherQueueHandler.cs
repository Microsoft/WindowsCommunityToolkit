// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Interop
{
    /// <summary>
    /// An interface mapping the native WinRT <c>IDispatcherQueueHandler</c> interface.
    /// </summary>
    internal interface IDispatcherQueueHandler
    {
        /// <summary>
        /// Implements <c>IUnknown.Release()</c>.
        /// </summary>
        /// <returns>The updated reference count for the current instance.</returns>
        uint Release();
    }
}