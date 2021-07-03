// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.UI.Dispatching;
using WinRT;

#nullable enable

#pragma warning disable SA1000, SA1023

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

        /// <summary>
        /// A struct mapping the native WinRT <c>IDispatcherQueue</c> interface.
        /// </summary>
        private unsafe struct IDispatcherQueue
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

        /// <summary>
        /// A custom <c>IDispatcherQueueHandler</c> object, that internally stores a captured <see cref="DispatcherQueueHandler{TState}"/> instance
        /// and the input captured state. This allows consumers to enqueue a state and a cached stateless delegate without any managed allocations.
        /// </summary>
        private unsafe struct DispatcherQueueProxyHandler
        {
            private const int S_OK = 0;
            private const int E_NOINTERFACE = unchecked((int)0x80004002);

            private static readonly Guid IUnknown = new(0x00000000, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);
            private static readonly Guid IAgileObject = new(0x94EA2B94, 0xE9CC, 0x49E0, 0xC0, 0xFF, 0xEE, 0x64, 0xCA, 0x8F, 0x5B, 0x90);
            private static readonly Guid IDispatcherQueueHandler = new(0x2E0872A9, 0x4E29, 0x5F14, 0xB6, 0x88, 0xFB, 0x96, 0xD5, 0xF9, 0xD5, 0xF8);

            /// <summary>
            /// The shared vtable pointer for <see cref="DispatcherQueueProxyHandler"/> instances.
            /// </summary>
            private static readonly void** Vtbl = InitVtbl();

            /// <summary>
            /// Setups the vtable pointer for <see cref="DispatcherQueueProxyHandler"/>.
            /// </summary>
            /// <returns>The initialized vtable pointer for <see cref="DispatcherQueueProxyHandler"/>.</returns>
            /// <remarks>
            /// The vtable itself is allocated with <see cref="RuntimeHelpers.AllocateTypeAssociatedMemory(Type, int)"/>,
            /// which allocates memory in the high frequency heap associated with the input runtime type. This will be
            /// automatically cleaned up when the type is unloaded, so there is no need to ever manually free this memory.
            /// </remarks>
            private static void** InitVtbl()
            {
                void** lpVtbl = (void**)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(DispatcherQueueProxyHandler), sizeof(void*) * 4);

                lpVtbl[0] = (delegate* unmanaged<DispatcherQueueProxyHandler*, Guid*, void**, int>)&Impl.QueryInterface;
                lpVtbl[1] = (delegate* unmanaged<DispatcherQueueProxyHandler*, uint>)&Impl.AddRef;
                lpVtbl[2] = (delegate* unmanaged<DispatcherQueueProxyHandler*, uint>)&Impl.Release;
                lpVtbl[3] = (delegate* unmanaged<DispatcherQueueProxyHandler*, int>)&Impl.Invoke;

                return lpVtbl;
            }

            /// <summary>
            /// The vtable pointer for the current instance.
            /// </summary>
            private void** lpVtbl;

            /// <summary>
            /// The <see cref="GCHandle"/> to the captured <see cref="DispatcherQueueHandler{TState}"/> (for some unknown <c>TState</c> type).
            /// </summary>
            private GCHandle callbackHandle;

            /// <summary>
            /// The <see cref="GCHandle"/> to the captured state (with an unknown <c>TState</c> type).
            /// </summary>
            private GCHandle stateHandle;

            /// <summary>
            /// The current reference count for the object (from <c>IUnknown</c>).
            /// </summary>
            private volatile uint referenceCount;

            /// <summary>
            /// Creates a new <see cref="DispatcherQueueProxyHandler"/> instance for the input callback and state.
            /// </summary>
            /// <typeparam name="TState">The type of state to capture.</typeparam>
            /// <param name="handler">The input <see cref="DispatcherQueueHandler{TState}"/> callback to enqueue.</param>
            /// <param name="state">The input state to capture and pass to the callback.</param>
            /// <returns>A pointer to the newly initialized <see cref="DispatcherQueueProxyHandler"/> instance.</returns>
            public static DispatcherQueueProxyHandler* Create<TState>(DispatcherQueueHandler<TState> handler, TState state)
                where TState : class
            {
                DispatcherQueueProxyHandler* @this = (DispatcherQueueProxyHandler*)Marshal.AllocHGlobal(sizeof(DispatcherQueueProxyHandler));

                @this->lpVtbl = Vtbl;
                @this->callbackHandle = GCHandle.Alloc(handler);
                @this->stateHandle = GCHandle.Alloc(state);
                @this->referenceCount = 1;

                return @this;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public uint AddRef()
            {
                return Interlocked.Increment(ref referenceCount);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public uint Release()
            {
                uint referenceCount = Interlocked.Decrement(ref this.referenceCount);

                if (referenceCount == 0)
                {
                    Marshal.FreeHGlobal((IntPtr)Unsafe.AsPointer(ref this));
                }

                return referenceCount;
            }

            /// <summary>
            /// A private type with the implementation of the unmanaged methods for <see cref="DispatcherQueueProxyHandler"/>.
            /// These methods will be set into the shared vtable and invoked by WinRT from the object passed to it as an interface.
            /// </summary>
            private static class Impl
            {
                /// <summary>
                /// Implements <c>IUnknown.QueryInterface(REFIID, void**)</c>.
                /// </summary>
                [UnmanagedCallersOnly]
                public static int QueryInterface(DispatcherQueueProxyHandler* @this, Guid* riid, void** ppvObject)
                {
                    if (riid->Equals(IUnknown) ||
                        riid->Equals(IAgileObject) ||
                        riid->Equals(IDispatcherQueueHandler))
                    {
                        @this->AddRef();

                        *ppvObject = @this;

                        return S_OK;
                    }

                    return E_NOINTERFACE;
                }

                /// <summary>
                /// Implements <c>IUnknown.AddRef()</c>.
                /// </summary>
                [UnmanagedCallersOnly]
                public static uint AddRef(DispatcherQueueProxyHandler* @this)
                {
                    return Interlocked.Increment(ref @this->referenceCount);
                }

                /// <summary>
                /// Implements <c>IUnknown.Release()</c>.
                /// </summary>
                [UnmanagedCallersOnly]
                public static uint Release(DispatcherQueueProxyHandler* @this)
                {
                    uint referenceCount = Interlocked.Decrement(ref @this->referenceCount);

                    if (referenceCount == 0)
                    {
                        @this->callbackHandle.Free();
                        @this->stateHandle.Free();

                        Marshal.FreeHGlobal((IntPtr)@this);
                    }

                    return referenceCount;
                }

                /// <summary>
                /// Implements <c>IDispatcherQueueHandler.Invoke()</c>.
                /// </summary>
                [UnmanagedCallersOnly]
                public static int Invoke(DispatcherQueueProxyHandler* @this)
                {
                    object callback = @this->callbackHandle.Target!;
                    object state = @this->stateHandle.Target!;

                    try
                    {
                        // We do an unsafe cast here to treat the captured delegate as if the contravariant
                        // input type was actually declared as covariant. This is valid because the type
                        // parameter is constrained to be a reference type, and due to how the proxy handler
                        // is constructed we know that the captured state will always match the actual type
                        // of the captured handler at this point. This lets this whole method work without the
                        // need to make the proxy type itself generic, so without knowing the actual type argument.
                        Unsafe.As<DispatcherQueueHandler<object>>(callback)(state);
                    }
                    catch
                    {
                    }

                    return S_OK;
                }
            }
        }
    }
}