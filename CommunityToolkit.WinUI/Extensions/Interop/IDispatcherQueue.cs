// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using Microsoft.UI.Dispatching;

#nullable enable

#pragma warning disable CS0649, SA1023

namespace CommunityToolkit.WinUI.Interop
{
    /// <summary>
    /// A struct mapping the native WinRT <c>IDispatcherQueue</c> interface.
    /// </summary>
    internal unsafe struct IDispatcherQueue
    {
        private readonly void** lpVtbl;

        /// <summary>
        /// Native API for <see cref="DispatcherQueue.TryEnqueue(DispatcherQueueHandler)"/>.
        /// </summary>
        /// <param name="callback">A pointer to an <c>IDispatcherQueueHandler</c> object.</param>
        /// <param name="result">The result of the operation (the <see cref="bool"/> WinRT retval).</param>
        /// <returns>The HRESULT for the operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int TryEnqueue(void* callback, byte* result)
        {
            return ((delegate* unmanaged<IDispatcherQueue*, void*, byte*, int>)lpVtbl[7])((IDispatcherQueue*)Unsafe.AsPointer(ref this), callback, result);
        }

        /// <summary>
        /// Native API for <see cref="DispatcherQueue.TryEnqueue(DispatcherQueuePriority, DispatcherQueueHandler)"/>.
        /// </summary>
        /// <param name="priority">The priority for the input callback.</param>
        /// <param name="callback">A pointer to an <c>IDispatcherQueueHandler</c> object.</param>
        /// <param name="result">The result of the operation (the <see cref="bool"/> WinRT retval).</param>
        /// <returns>The HRESULT for the operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int TryEnqueueWithPriority(DispatcherQueuePriority priority, void* callback, byte* result)
        {
            return ((delegate* unmanaged<IDispatcherQueue*, DispatcherQueuePriority, void*, byte*, int>)lpVtbl[8])((IDispatcherQueue*)Unsafe.AsPointer(ref this), priority, callback, result);
        }
    }
}