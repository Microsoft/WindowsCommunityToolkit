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
        /// <remarks>
        /// The <paramref name="callback"/> parameter is assumed to be a pointer to an <see cref="IDispatcherQueueHandler"/> object, but it
        /// is just typed as a <see cref="void"/> to avoid unnecessary generic instantiations for this method (as it just needs to pass a
        /// pointer to the native side anyway). The <see cref="IDispatcherQueueHandler"/> is an actual C# interface and not a C++ one as it
        /// is only used internally to constrain type parameters and allow calls to <see cref="IDispatcherQueueHandler.Release"/> to be inlined.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int TryEnqueue(void* callback, byte* result)
        {
            return ((delegate* unmanaged<IDispatcherQueue*, void*, byte*, int>)lpVtbl[7])((IDispatcherQueue*)Unsafe.AsPointer(ref this), callback, result);
        }

        /// <summary>
        /// Native API for <see cref="DispatcherQueue.TryEnqueue(DispatcherQueuePriority, DispatcherQueueHandler)"/>.
        /// </summary>
        /// <param name="priority">The priority for the input callback.</param>
        /// <param name="callback">A pointer to an <see cref="IDispatcherQueueHandler"/> object.</param>
        /// <param name="result">The result of the operation (the <see cref="bool"/> WinRT retval).</param>
        /// <returns>The HRESULT for the operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int TryEnqueueWithPriority(DispatcherQueuePriority priority, void* callback, byte* result)
        {
            return ((delegate* unmanaged<IDispatcherQueue*, DispatcherQueuePriority, void*, byte*, int>)lpVtbl[8])((IDispatcherQueue*)Unsafe.AsPointer(ref this), priority, callback, result);
        }
    }
}