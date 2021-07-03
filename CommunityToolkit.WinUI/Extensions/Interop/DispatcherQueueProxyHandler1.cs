// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using static CommunityToolkit.WinUI.Interop.Windows;

#nullable enable

#pragma warning disable SA1023

namespace CommunityToolkit.WinUI.Interop
{
    /// <summary>
    /// A custom <c>IDispatcherQueueHandler</c> object, that internally stores a captured <see cref="DispatcherQueueHandler{TState}"/> instance
    /// and the input captured state. This allows consumers to enqueue a state and a cached stateless delegate without any managed allocations.
    /// </summary>
    internal unsafe struct DispatcherQueueProxyHandler1
    {
        /// <summary>
        /// The shared vtable pointer for <see cref="DispatcherQueueProxyHandler1"/> instances.
        /// </summary>
        private static readonly void** Vtbl = InitVtbl();

        /// <summary>
        /// Setups the vtable pointer for <see cref="DispatcherQueueProxyHandler1"/>.
        /// </summary>
        /// <returns>The initialized vtable pointer for <see cref="DispatcherQueueProxyHandler1"/>.</returns>
        /// <remarks>
        /// The vtable itself is allocated with <see cref="RuntimeHelpers.AllocateTypeAssociatedMemory(Type, int)"/>,
        /// which allocates memory in the high frequency heap associated with the input runtime type. This will be
        /// automatically cleaned up when the type is unloaded, so there is no need to ever manually free this memory.
        /// </remarks>
        private static void** InitVtbl()
        {
            void** lpVtbl = (void**)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(DispatcherQueueProxyHandler1), sizeof(void*) * 4);

            lpVtbl[0] = (delegate* unmanaged<DispatcherQueueProxyHandler1*, Guid*, void**, int>)&Impl.QueryInterface;
            lpVtbl[1] = (delegate* unmanaged<DispatcherQueueProxyHandler1*, uint>)&Impl.AddRef;
            lpVtbl[2] = (delegate* unmanaged<DispatcherQueueProxyHandler1*, uint>)&Impl.Release;
            lpVtbl[3] = (delegate* unmanaged<DispatcherQueueProxyHandler1*, int>)&Impl.Invoke;

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
        /// Creates a new <see cref="DispatcherQueueProxyHandler1"/> instance for the input callback and state.
        /// </summary>
        /// <param name="handler">The input <see cref="DispatcherQueueHandler{TState}"/> callback to enqueue.</param>
        /// <param name="state">The input state to capture and pass to the callback.</param>
        /// <returns>A pointer to the newly initialized <see cref="DispatcherQueueProxyHandler1"/> instance.</returns>
        public static DispatcherQueueProxyHandler1* Create(object handler, object state)
        {
            DispatcherQueueProxyHandler1* @this = (DispatcherQueueProxyHandler1*)Marshal.AllocHGlobal(sizeof(DispatcherQueueProxyHandler1));

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
                callbackHandle.Free();
                stateHandle.Free();

                Marshal.FreeHGlobal((IntPtr)Unsafe.AsPointer(ref this));
            }

            return referenceCount;
        }

        /// <summary>
        /// A private type with the implementation of the unmanaged methods for <see cref="DispatcherQueueProxyHandler1"/>.
        /// These methods will be set into the shared vtable and invoked by WinRT from the object passed to it as an interface.
        /// </summary>
        private static class Impl
        {
            /// <summary>
            /// Implements <c>IUnknown.QueryInterface(REFIID, void**)</c>.
            /// </summary>
            [UnmanagedCallersOnly]
            public static int QueryInterface(DispatcherQueueProxyHandler1* @this, Guid* riid, void** ppvObject)
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
            public static uint AddRef(DispatcherQueueProxyHandler1* @this)
            {
                return Interlocked.Increment(ref @this->referenceCount);
            }

            /// <summary>
            /// Implements <c>IUnknown.Release()</c>.
            /// </summary>
            [UnmanagedCallersOnly]
            public static uint Release(DispatcherQueueProxyHandler1* @this)
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
            public static int Invoke(DispatcherQueueProxyHandler1* @this)
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