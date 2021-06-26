// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;

#pragma warning disable CA1001

namespace Microsoft.Toolkit.Deferred
{
    /// <summary>
    /// <see cref="EventArgs"/> which can retrieve a <see cref="EventDeferral"/> in order to process data asynchronously before an <see cref="EventHandler"/> completes and returns to the calling control.
    /// </summary>
    public class DeferredEventArgs : EventArgs
    {
        /// <summary>
        /// Gets a new <see cref="DeferredEventArgs"/> to use in cases where no <see cref="EventArgs"/> wish to be provided.
        /// </summary>
        public static new DeferredEventArgs Empty => new DeferredEventArgs();

        private readonly object _eventDeferralLock = new object();

        private EventDeferral? _eventDeferral;

        /// <summary>
        /// Returns an <see cref="EventDeferral"/> which can be completed when deferred event is ready to continue.
        /// </summary>
        /// <returns><see cref="EventDeferral"/> instance.</returns>
        public EventDeferral GetDeferral()
        {
            lock (_eventDeferralLock)
            {
                return _eventDeferral ??= new EventDeferral();
            }
        }

        /// <summary>
        /// DO NOT USE - This is a support method used by <see cref="EventHandlerExtensions"/>. It is public only for
        /// additional usage within extensions for the UWP based TypedEventHandler extensions.
        /// </summary>
        /// <returns>Internal EventDeferral reference</returns>
#if !NETSTANDARD1_4
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This is an internal only method to be used by EventHandler extension classes, public callers should call GetDeferral() instead.")]
        public EventDeferral? GetCurrentDeferralAndReset()
        {
            lock (_eventDeferralLock)
            {
                var eventDeferral = _eventDeferral;

                _eventDeferral = null;

                return eventDeferral;
            }
        }
    }
}