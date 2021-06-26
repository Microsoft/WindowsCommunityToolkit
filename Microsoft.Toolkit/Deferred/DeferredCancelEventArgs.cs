// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Deferred
{
    /// <summary>
    /// <see cref="DeferredEventArgs"/> which can also be canceled.
    /// </summary>
    public class DeferredCancelEventArgs : DeferredEventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether the event should be canceled.
        /// </summary>
        public bool Cancel { get; set; }
    }
}