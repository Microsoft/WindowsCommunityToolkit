// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using Microsoft.UI.Dispatching;

#pragma warning disable CS0649, SA1023

namespace CommunityToolkit.WinUI.Interop
{
    /// <summary>
    /// A struct mapping the native WinRT <c>IDispatcherQueue</c> interface.
    /// </summary>
    internal unsafe struct IDispatcherQueue
    {
        /// <summary>
        /// The vtable pointer for the current instance.
        /// </summary>
        private readonly void** lpVtbl;

        /// <summary>
        /// Native API for <see cref="DispatcherQueue.TryEnqueue(DispatcherQueueHandler)"/>.
        /// </summary>
        /// <param name="callback">A pointer to an <see cref="IDispatcherQueueHandler"/> object.</param>
        /// <param name="result">The result of the operation (the <see cref="bool"/> WinRT retval).</param>
        /// <returns>The HRESULT for the operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int TryEnqueue(IDispatcherQueueHandler* callback, byte* result)
        {
            return ((delegate* unmanaged<IDispatcherQueue*, IDispatcherQueueHandler*, byte*, int>)lpVtbl[7])((IDispatcherQueue*)Unsafe.AsPointer(ref this), callback, result);
        }

        /// <summary>
        /// Native API for <see cref="DispatcherQueue.TryEnqueue(DispatcherQueuePriority, DispatcherQueueHandler)"/>.
        /// </summary>
        /// <param name="priority">The priority for the input callback.</param>
        /// <param name="callback">A pointer to an <see cref="IDispatcherQueueHandler"/> object.</param>
        /// <param name="result">The result of the operation (the <see cref="bool"/> WinRT retval).</param>
        /// <returns>The HRESULT for the operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int TryEnqueueWithPriority(DispatcherQueuePriority priority, IDispatcherQueueHandler* callback, byte* result)
        {
            return ((delegate* unmanaged<IDispatcherQueue*, DispatcherQueuePriority, IDispatcherQueueHandler*, byte*, int>)lpVtbl[8])((IDispatcherQueue*)Unsafe.AsPointer(ref this), priority, callback, result);
        }
    }
}