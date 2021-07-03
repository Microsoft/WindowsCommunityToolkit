// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.WinUI.Interop;
using Microsoft.UI.Dispatching;
using WinRT;

#nullable enable

namespace CommunityToolkit.WinUI
{
    /// <summary>
    /// A callback that will be executed on the <see cref="DispatcherQueue"/> thread.
    /// </summary>
    /// <typeparam name="TState">The type of state to receive as input.</typeparam>
    /// <param name="state">The input state for the callback.</param>
    public delegate void DispatcherQueueHandler<in TState>(TState state)
        where TState : class;

    /// <summary>
    /// Helpers for executing code in a <see cref="DispatcherQueue"/>.
    /// </summary>
    public static partial class DispatcherQueueExtensions
    {
        /// <summary>
        /// Adds a task to the <see cref="DispatcherQueue"/> which will be executed on the thread associated with it.
        /// </summary>
        /// <typeparam name="TState">The type of state to capture.</typeparam>
        /// <param name="dispatcherQueue">The target <see cref="DispatcherQueue"/> to invoke the code on.</param>
        /// <param name="callback">The input <see cref="DispatcherQueueHandler{TState}"/> callback to enqueue.</param>
        /// <param name="state">The input state to capture and pass to the callback.</param>
        /// <returns>Whether or not the task was added to the queue.</returns>
        /// <exception cref="Exception">Thrown when the enqueue operation fails.</exception>
        public static unsafe bool TryEnqueue<TState>(this DispatcherQueue dispatcherQueue, DispatcherQueueHandler<TState> callback, TState state)
            where TState : class
        {
            IDispatcherQueue* dispatcherQueuePtr = (IDispatcherQueue*)((IWinRTObject)dispatcherQueue).NativeObject.ThisPtr;
            DispatcherQueueProxyHandler* dispatcherQueueHandlerPtr = DispatcherQueueProxyHandler.Create(callback, state);

            bool success;
            int hResult;

            try
            {
                hResult = dispatcherQueuePtr->TryEnqueue(dispatcherQueueHandlerPtr, (byte*)&success);

                GC.KeepAlive(dispatcherQueue);
            }
            finally
            {
                dispatcherQueueHandlerPtr->Release();
            }

            if (hResult != 0)
            {
                ExceptionHelpers.ThrowExceptionForHR(hResult);
            }

            return success;
        }

        /// <summary>
        /// Adds a task to the <see cref="DispatcherQueue"/> which will be executed on the thread associated with it.
        /// </summary>
        /// <typeparam name="TState">The type of state to capture.</typeparam>
        /// <param name="dispatcherQueue">The target <see cref="DispatcherQueue"/> to invoke the code on.</param>
        /// <param name="priority"> The desired priority for the callback to schedule.</param>
        /// <param name="callback">The input <see cref="DispatcherQueueHandler{TState}"/> callback to enqueue.</param>
        /// <param name="state">The input state to capture and pass to the callback.</param>
        /// <returns>Whether or not the task was added to the queue.</returns>
        /// <exception cref="Exception">Thrown when the enqueue operation fails.</exception>
        public static unsafe bool TryEnqueue<TState>(this DispatcherQueue dispatcherQueue, DispatcherQueuePriority priority, DispatcherQueueHandler<TState> callback, TState state)
            where TState : class
        {
            IDispatcherQueue* dispatcherQueuePtr = (IDispatcherQueue*)((IWinRTObject)dispatcherQueue).NativeObject.ThisPtr;
            DispatcherQueueProxyHandler* dispatcherQueueHandlerPtr = DispatcherQueueProxyHandler.Create(callback, state);

            bool success;
            int hResult;

            try
            {
                hResult = dispatcherQueuePtr->TryEnqueueWithPriority(priority, dispatcherQueueHandlerPtr, (byte*)&success);

                GC.KeepAlive(dispatcherQueue);
            }
            finally
            {
                dispatcherQueueHandlerPtr->Release();
            }

            if (hResult != 0)
            {
                ExceptionHelpers.ThrowExceptionForHR(hResult);
            }

            return success;
        }
    }
}