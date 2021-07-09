// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.Interop
{
    /// <summary>
    /// A helper class with some shared constants from the Windows headers.
    /// </summary>
    internal static class Windows
    {
        /// <summary>
        /// The HRESULT for a successful operation.
        /// </summary>
        public const int S_OK = 0;

        /// <summary>
        /// The HRESULT for an invalid cast from <c>IUnknown.QueryInterface</c>.
        /// </summary>
        public const int E_NOINTERFACE = unchecked((int)0x80004002);

        /// <summary>
        /// The GUID for the <c>IUnknown</c> COM interface.
        /// </summary>
        public static readonly Guid GuidOfIUnknown = new(0x00000000, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

        /// <summary>
        /// The GUID for the <c>IAgileObject</c> WinRT interface.
        /// </summary>
        public static readonly Guid GuidOfIAgileObject = new(0x94EA2B94, 0xE9CC, 0x49E0, 0xC0, 0xFF, 0xEE, 0x64, 0xCA, 0x8F, 0x5B, 0x90);

        /// <summary>
        /// The GUID for the <c>IDispatcherQueueHandler</c> WinRT interface.
        /// </summary>
        public static readonly Guid GuidOfIDispatcherQueueHandler = new(0x2E0872A9, 0x4E29, 0x5F14, 0xB6, 0x88, 0xFB, 0x96, 0xD5, 0xF9, 0xD5, 0xF8);
    }
}