// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace System
{
    /// <summary>
    /// Schedules a callback roughly every gen 2 GC (you may see a Gen 0 an Gen 1 but only once).
    /// Ported from https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Gen2GcCallback.cs.
    /// </summary>
    internal sealed class Gen2GcCallback : CriticalFinalizerObject
    {
        /// <summary>
        /// The callback to invoke at each GC.
        /// </summary>
        private readonly Action<object> callback;

        /// <summary>
        /// A weak <see cref="GCHandle"/> to the target object to pass to <see cref="callback"/>.
        /// </summary>
        private GCHandle handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="Gen2GcCallback"/> class.
        /// </summary>
        /// <param name="callback">The callback to invoke at each GC.</param>
        /// <param name="target">The target object to pass as argument to <paramref name="callback"/>.</param>
        private Gen2GcCallback(Action<object> callback, object target)
        {
            this.callback = callback;
            this.handle = GCHandle.Alloc(target, GCHandleType.Weak);
        }

        /// <summary>
        /// Schedules a callback to be called on each GC until the target is collected.
        /// </summary>
        /// <param name="callback">The callback to invoke at each GC.</param>
        /// <param name="target">The target object to pass as argument to <paramref name="callback"/>.</param>
        public static void Register(Action<object> callback, object target)
        {
            _ = new Gen2GcCallback(callback, target);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Gen2GcCallback"/> class.
        /// This finalizer is re-registered with <see cref="GC.ReRegisterForFinalize(object)"/> as long as
        /// the target object is alive, which means it will be executed again every time a generation 2
        /// collection is triggered (as the <see cref="Gen2GcCallback"/> instance itself would be moved to
        /// that generation after surviving the generation 0 and 1 collections the first time).
        /// </summary>
        ~Gen2GcCallback()
        {
            if (this.handle.Target is object target)
            {
                try
                {
                    this.callback(target);
                }
                catch
                {
                }

                GC.ReRegisterForFinalize(this);
            }
            else
            {
                handle.Free();
            }
        }
    }
}