// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

#pragma warning disable CS0649, SA1023

namespace CommunityToolkit.WinUI.Interop
{
    /// <summary>
    /// A struct mapping the native WinRT <c>IDispatcherQueueHandler</c> interface.
    /// </summary>
    internal unsafe struct IDispatcherQueueHandler
    {
        private readonly void** lpVtbl;

        /// <summary>
        /// Native API for <c>IUnknown.Release()</c>.
        /// </summary>
        /// <returns>The updated reference count for the current instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Release()
        {
            return ((delegate* unmanaged<IDispatcherQueueHandler*, uint>)lpVtbl[2])((IDispatcherQueueHandler*)Unsafe.AsPointer(ref this));
        }
    }
}