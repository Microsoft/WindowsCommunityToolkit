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
    /// <typeparam name="T">The type of state to receive as input.</typeparam>
    /// <param name="state">The input state for the callback.</param>
    public delegate void DispatcherQueueHandler<in T>(T state)
        where T : class;

    /// <summary>
    /// A callback that will be executed on the <see cref="DispatcherQueue"/> thread.
    /// </summary>
    /// <typeparam name="T1">The type of the first state to receive as input.</typeparam>
    /// <typeparam name="T2">The type of the second state to receive as input.</typeparam>
    /// <param name="state1">The first input state for the callback.</param>
    /// <param name="state2">The second input state for the callback.</param>
    public delegate void DispatcherQueueHandler<in T1, in T2>(T1 state1, T2 state2)
        where T1 : class
        where T2 : class;

    /// <summary>
    /// Helpers for executing code in a <see cref="DispatcherQueue"/>.
    /// </summary>
    public static partial class DispatcherQueueExtensions
    {
        /// <summary>
        /// Adds a task to the <see cref="DispatcherQueue"/> which will be executed on the thread associated with it.
        /// </summary>
        /// <typeparam name="T">The type of state to capture.</typeparam>
        /// <param name="dispatcherQueue">The target <see cref="DispatcherQueue"/> to invoke the code on.</param>
        /// <param name="callback">The input <see cref="DispatcherQueueHandler{T}"/> callback to enqueue.</param>
        /// <param name="state">The input state to capture and pass to the callback.</param>
        /// <returns>Whether or not the task was added to the queue.</returns>
        /// <exception cref="Exception">Thrown when the enqueue operation fails.</exception>
        public static unsafe bool TryEnqueue<T>(this DispatcherQueue dispatcherQueue, DispatcherQueueHandler<T> callback, T state)
            where T : class
        {
            return TryEnqueue(dispatcherQueue, DispatcherQueueProxyHandler1.Create(callback, state));
        }

        /// <summary>
        /// Adds a task to the <see cref="DispatcherQueue"/> which will be executed on the thread associated with it.
        /// </summary>
        /// <typeparam name="T">The type of state to capture.</typeparam>
        /// <param name="dispatcherQueue">The target <see cref="DispatcherQueue"/> to invoke the code on.</param>
        /// <param name="priority"> The desired priority for the callback to schedule.</param>
        /// <param name="callback">The input <see cref="DispatcherQueueHandler{T}"/> callback to enqueue.</param>
        /// <param name="state">The input state to capture and pass to the callback.</param>
        /// <returns>Whether or not the task was added to the queue.</returns>
        /// <exception cref="Exception">Thrown when the enqueue operation fails.</exception>
        public static unsafe bool TryEnqueue<T>(this DispatcherQueue dispatcherQueue, DispatcherQueuePriority priority, DispatcherQueueHandler<T> callback, T state)
            where T : class
        {
            return TryEnqueue(dispatcherQueue, priority, DispatcherQueueProxyHandler1.Create(callback, state));
        }

        /// <summary>
        /// Adds a task to the <see cref="DispatcherQueue"/> which will be executed on the thread associated with it.
        /// </summary>
        /// <typeparam name="T1">The type of the first state to capture.</typeparam>
        /// <typeparam name="T2">The type of the second state to capture.</typeparam>
        /// <param name="dispatcherQueue">The target <see cref="DispatcherQueue"/> to invoke the code on.</param>
        /// <param name="callback">The input <see cref="DispatcherQueueHandler{T}"/> callback to enqueue.</param>
        /// <param name="state1">The first input state to capture and pass to the callback.</param>
        /// <param name="state2">The second input state to capture and pass to the callback.</param>
        /// <returns>Whether or not the task was added to the queue.</returns>
        /// <exception cref="Exception">Thrown when the enqueue operation fails.</exception>
        public static unsafe bool TryEnqueue<T1, T2>(this DispatcherQueue dispatcherQueue, DispatcherQueueHandler<T1, T2> callback, T1 state1, T2 state2)
            where T1 : class
            where T2 : class
        {
            return TryEnqueue(dispatcherQueue, DispatcherQueueProxyHandler2.Create(callback, state1, state2));
        }

        /// <summary>
        /// Adds a task to the <see cref="DispatcherQueue"/> which will be executed on the thread associated with it.
        /// </summary>
        /// <typeparam name="T1">The type of the first state to capture.</typeparam>
        /// <typeparam name="T2">The type of the second state to capture.</typeparam>
        /// <param name="dispatcherQueue">The target <see cref="DispatcherQueue"/> to invoke the code on.</param>
        /// <param name="priority"> The desired priority for the callback to schedule.</param>
        /// <param name="callback">The input <see cref="DispatcherQueueHandler{T}"/> callback to enqueue.</param>
        /// <param name="state1">The first input state to capture and pass to the callback.</param>
        /// <param name="state2">The second input state to capture and pass to the callback.</param>
        /// <returns>Whether or not the task was added to the queue.</returns>
        /// <exception cref="Exception">Thrown when the enqueue operation fails.</exception>
        public static unsafe bool TryEnqueue<T1, T2>(this DispatcherQueue dispatcherQueue, DispatcherQueuePriority priority, DispatcherQueueHandler<T1, T2> callback, T1 state1, T2 state2)
            where T1 : class
            where T2 : class
        {
            return TryEnqueue(dispatcherQueue, priority, DispatcherQueueProxyHandler2.Create(callback, state1, state2));
        }

        /// <summary>
        /// Adds a task to the <see cref="DispatcherQueue"/> which will be executed on the thread associated with it.
        /// </summary>
        /// <param name="dispatcherQueue">The target <see cref="DispatcherQueue"/> to invoke the code on.</param>
        /// <param name="dispatcherQueueHandler">The input callback to enqueue.</param>
        /// <returns>Whether or not the task was added to the queue.</returns>
        /// <exception cref="Exception">Thrown when the enqueue operation fails.</exception>
        private static unsafe bool TryEnqueue(DispatcherQueue dispatcherQueue, IDispatcherQueueHandler* dispatcherQueueHandler)
        {
            bool success;
            int hResult;

            try
            {
                IDispatcherQueue* dispatcherQueuePtr = (IDispatcherQueue*)((IWinRTObject)dispatcherQueue).NativeObject.ThisPtr;

                hResult = dispatcherQueuePtr->TryEnqueue(dispatcherQueueHandler, (byte*)&success);

                GC.KeepAlive(dispatcherQueue);
            }
            finally
            {
                dispatcherQueueHandler->Release();
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
        /// <param name="dispatcherQueue">The target <see cref="DispatcherQueue"/> to invoke the code on.</param>
        /// <param name="priority"> The desired priority for the callback to schedule.</param>
        /// <param name="dispatcherQueueHandler">The input callback to enqueue.</param>
        /// <returns>Whether or not the task was added to the queue.</returns>
        /// <exception cref="Exception">Thrown when the enqueue operation fails.</exception>
        private static unsafe bool TryEnqueue(DispatcherQueue dispatcherQueue, DispatcherQueuePriority priority, IDispatcherQueueHandler* dispatcherQueueHandler)
        {
            bool success;
            int hResult;

            try
            {
                IDispatcherQueue* dispatcherQueuePtr = (IDispatcherQueue*)((IWinRTObject)dispatcherQueue).NativeObject.ThisPtr;

                hResult = dispatcherQueuePtr->TryEnqueueWithPriority(priority, dispatcherQueueHandler, (byte*)&success);

                GC.KeepAlive(dispatcherQueue);
            }
            finally
            {
                dispatcherQueueHandler->Release();
            }

            if (hResult != 0)
            {
                ExceptionHelpers.ThrowExceptionForHR(hResult);
            }

            return success;
        }
    }
}